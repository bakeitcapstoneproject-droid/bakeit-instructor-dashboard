using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
public sealed class PourableIngredientReceiver : MonoBehaviour, IPourDestination
{
    [SerializeField] private MixingSequence stagedProcess;
    [SerializeField] private string bowlRole;
    public MixingSequence StagedProcess => stagedProcess;
    public string BowlRole => bowlRole;
    public static PourableIngredientReceiver FindBrownieBowl()
    {
        foreach (var receiver in FindObjectsByType<PourableIngredientReceiver>())
            if (!receiver.stagedProcess && receiver.ActiveRecipe && receiver.ActiveRecipe.RecipeId == "brownies") return receiver;
        return null;
    }
    [SerializeField] private RecipeDefinition recipeDefinition;
    [SerializeField] private string acceptedIngredient = "MeltedButter";
    [SerializeField] private string receiverDisplayName = "Brownie Mixing Bowl";
    [SerializeField] private string requiredSurfaceName = "Mix Bowl Rest";
    [SerializeField] private PreparationItem bowlItem;
    [SerializeField] private GameObject receivedContentsVisual;
    [SerializeField] private GameObject batterContentsVisual;
    [SerializeField] private Vector3 transferTargetLocal = new(0f, -0.025f, 0f);

    [Header("Mixing")]
    [SerializeField, Min(.001f)] private float whiskDistancePerPass = .06f;
    [SerializeField, Min(.1f)] private float minimumTimeBetweenPasses = 1.2f;
    [SerializeField] private Color unmixedBatterColor = new(.46f, .17f, .055f, 1f);
    [SerializeField] private Color mixedBatterColor = new(.34f, .105f, .035f, 1f);

    private readonly Dictionary<string, int> ingredientAmounts =
        new(StringComparer.OrdinalIgnoreCase);
    private MixingTool activeMixingTool;
    private Vector3 lastWhiskPosition;
    private float whiskTravelSinceLastPass;
    private float nextWhiskPassTime;
    private float nextFeedbackTime;
    private int baseMixPasses;
    private int finishingMixPasses;
    [SerializeField, HideInInspector] private bool baseMixed;
    [SerializeField, HideInInspector] private bool batterComplete;
    [SerializeField, HideInInspector] private bool batterTransferred;
    private PickupController pickup;
    private Rigidbody bowlBody;
    private BrowniePreparationVisual preparationVisual;

    public bool HasReceivedIngredient =>
        GetIngredientAmount(acceptedIngredient, RecipeMeasureTool.WholeItem) > 0;
    public string AcceptedIngredient => acceptedIngredient;
    public RecipeDefinition ActiveRecipe => recipeDefinition;
    public bool HasAllBaseIngredients => HasAllIngredients(RecipeIngredientStage.Base);
    public bool HasAllFinishingIngredients => HasAllIngredients(RecipeIngredientStage.Finishing);
    public bool IsBowlReady => IsBowlOnRequiredSurface();
    public bool IsBaseMixed => baseMixed;
    public bool IsBatterComplete => batterComplete;
    public bool HasTransferredBatter => batterTransferred;
    public Vector3 TransferTarget => transform.TransformPoint(transferTargetLocal);
    public bool CanPreviewPour(PourableIngredient source)
    {
        if (!source || !IsBowlOnRequiredSurface() || batterComplete) return false;
        if (stagedProcess) return stagedProcess.CanReceive(bowlRole, source.IngredientName, RecipeMeasureTool.WholeItem);
        if (recipeDefinition == null) return source.IngredientName == acceptedIngredient && !HasReceivedIngredient;
        if (!TryGetRequirement(source.IngredientName, RecipeMeasureTool.WholeItem, out var requirement) || requirement == null) return false;
        return !HasIngredientRequirement(requirement) &&
            (requirement.Stage == RecipeIngredientStage.Base ? !baseMixed : baseMixed);
    }
    public bool CanPourCompletedBatter =>
        batterComplete && !batterTransferred &&
        pickup != null && pickup.HeldObject == bowlBody &&
        bowlBody != null &&
        Vector3.Angle(bowlBody.transform.up, Vector3.up) >= 65f;
    public int BaseMixPassCount => baseMixPasses;
    public int FinishingMixPassCount => finishingMixPasses;
    public int RequiredBaseMixPasses => recipeDefinition != null ? recipeDefinition.BaseMixPasses : 1;
    public int RequiredFinishingMixPasses => recipeDefinition != null ? recipeDefinition.FinishingMixPasses : 0;

    private void Awake()
    {
        if (bowlItem == null)
            bowlItem = GetComponentInParent<PreparationItem>();
        bowlBody = GetComponentInParent<Rigidbody>();
        pickup = FindAnyObjectByType<PickupController>();
        UpdateContentsVisual();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<MeasuringScoop>() != null)
            return;

        Ingredient ingredient = other.GetComponentInParent<Ingredient>();
        if (ingredient != null)
        {
            TryReceiveWholeIngredient(ingredient);
            return;
        }

        MixingTool mixingTool = other.GetComponentInParent<MixingTool>();
        if (mixingTool != null)
            BeginTrackingWhisk(mixingTool);
    }

    private void OnTriggerStay(Collider other)
    {
        MixingTool mixingTool = other.GetComponentInParent<MixingTool>();
        if (mixingTool != null)
            TrackWhiskMovement(mixingTool);
    }

    private void OnTriggerExit(Collider other)
    {
        MixingTool mixingTool = other.GetComponentInParent<MixingTool>();
        if (mixingTool == null || mixingTool != activeMixingTool)
            return;
        activeMixingTool = null;
        whiskTravelSinceLastPass = 0f;
    }

    public bool TryReceive(PourableIngredient source)
    {
        if (source == null || !source.HasContents || !source.IsHeld ||
            !source.IsTilted || !ReferenceEquals(source.FindPourDestination(), this))
            return false;

        if (recipeDefinition == null)
            return TryReceiveLegacy(source);

        if (!TryGetRequirement(source.IngredientName, RecipeMeasureTool.WholeItem,
                out RecipeIngredientRequirement requirement))
        {
            Warn($"{receiverDisplayName} cannot accept {source.DisplayName}.");
            return false;
        }

        if (!CanAccept(requirement))
            return false;

        Vector3 target = transform.TransformPoint(transferTargetLocal);
        if (!source.TryCompleteTransfer(this, target))
            return false;

        AddRequirementAmount(requirement);
        UpdateContentsVisual();
        RecipeFeedback.Report(
            $"{source.QuantityLabel} of {source.DisplayName} poured into {receiverDisplayName}.");
        return true;
    }

    private bool TryReceiveLegacy(PourableIngredient source)
    {
        if (!string.Equals(
                source.IngredientName,
                acceptedIngredient,
                StringComparison.OrdinalIgnoreCase))
        {
            Warn($"{receiverDisplayName} cannot accept {source.DisplayName}.");
            return false;
        }

        if (HasReceivedIngredient)
        {
            Warn($"{receiverDisplayName} already contains {source.DisplayName}.");
            return false;
        }

        if (!IsBowlOnRequiredSurface())
        {
            Warn($"Place {receiverDisplayName} on {requiredSurfaceName} before pouring.");
            return false;
        }

        if (!source.TryCompleteTransfer(
                this,
                transform.TransformPoint(transferTargetLocal)))
            return false;

        AddIngredientAmount(acceptedIngredient, RecipeMeasureTool.WholeItem);
        UpdateContentsVisual();
        RecipeFeedback.Report(
            $"{source.QuantityLabel} of {source.DisplayName} poured into {receiverDisplayName}.");
        return true;
    }

    public bool TryPourMeasure(MeasuringScoop scoop)
    {
        if (scoop == null || !scoop.IsHeld || !scoop.IsFilled ||
            !scoop.IsTilted || scoop.FindPourDestination() != this)
            return false;

        if (!TryGetRequirement(scoop.CurrentIngredient, scoop.MeasureTool,
                out RecipeIngredientRequirement requirement))
        {
            Warn($"{scoop.ToolName} is not the required measure for " +
                 $"{scoop.CurrentIngredient} in {receiverDisplayName}.");
            return false;
        }

        if (!CanAccept(requirement))
            return false;

        Vector3 target = transform.TransformPoint(transferTargetLocal);
        if (!scoop.CompleteTransfer(
                MeasureDisposition.AddedToBowl, receiverDisplayName, target))
            return false;

        AddRequirementAmount(requirement);
        UpdateContentsVisual();
        RecipeFeedback.Report(
            $"{requirement.UnitLabel} of {requirement.DisplayName} poured into {receiverDisplayName}.");
        return true;
    }

    public bool TryReceiveWholeIngredient(Ingredient ingredient)
    {
        if (ingredient == null || !ingredient.gameObject.activeInHierarchy)
            return false;
        if (stagedProcess && (pickup == null || pickup.HeldObject != ingredient.GetComponentInParent<Rigidbody>())) return false;

        if (!TryGetRequirement(ingredient.ingredientName, RecipeMeasureTool.WholeItem,
                out RecipeIngredientRequirement requirement) ||
            requirement.Matches(acceptedIngredient))
            return false;

        if (!CanAccept(requirement))
            return false;

        AddRequirementAmount(requirement);
        Rigidbody ingredientBody = ingredient.GetComponentInParent<Rigidbody>();
        GameObject consumedObject = ingredientBody != null
            ? ingredientBody.gameObject
            : ingredient.gameObject;
        consumedObject.SetActive(false);
        UpdateContentsVisual();
        RecipeFeedback.Report(
            $"{requirement.DisplayName} added to {receiverDisplayName}.");
        return true;
    }

    public int GetIngredientAmount(string ingredientId, RecipeMeasureTool tool)
    {
        if (string.IsNullOrWhiteSpace(ingredientId))
            return 0;
        ingredientAmounts.TryGetValue(GetRequirementKey(ingredientId, tool), out int amount);
        return amount;
    }

    public int GetTotalIngredientAmount(string ingredientId)
    {
        int total=0;
        foreach(var pair in ingredientAmounts)
            if(pair.Key.StartsWith(ingredientId + "#", StringComparison.OrdinalIgnoreCase)) total+=pair.Value;
        return total;
    }

    public bool HasIngredientRequirement(RecipeIngredientRequirement requirement)
    {
        return requirement != null &&
               GetIngredientAmount(requirement.IngredientId, requirement.RequiredTool) >=
               requirement.RequiredQuantity;
    }

    public void ResetReceiver()
    {
        ingredientAmounts.Clear();
        baseMixPasses = 0;
        finishingMixPasses = 0;
        baseMixed = false;
        batterComplete = false;
        batterTransferred = false;
        activeMixingTool = null;
        whiskTravelSinceLastPass = 0f;
        nextWhiskPassTime = 0f;
        UpdateContentsVisual();
    }

    public bool TryTransferCompletedBatter(Vector3 visualTarget)
    {
        if (!CanPourCompletedBatter)
            return false;

        Vector3 origin = batterContentsVisual != null
            ? batterContentsVisual.transform.position
            : transform.position;
        MeasureTransferVisual.Show(origin, visualTarget, "BrownieBatter");
        batterTransferred = true;
        UpdateContentsVisual();
        return true;
    }

    private bool TryGetRequirement(
        string ingredientId,
        RecipeMeasureTool tool,
        out RecipeIngredientRequirement requirement)
    {
        if (stagedProcess)
        {
            requirement = null;
            if (stagedProcess.Current != null)
                foreach (var candidate in stagedProcess.Current.Ingredients)
                    if (candidate.Matches(ingredientId) && candidate.RequiredTool == tool) { requirement = candidate; return true; }
            return false;
        }
        if (recipeDefinition != null)
            return recipeDefinition.TryGetIngredientForTool(ingredientId, tool, out requirement);

        requirement = null;
        return tool == RecipeMeasureTool.WholeItem &&
               string.Equals(ingredientId, acceptedIngredient, StringComparison.OrdinalIgnoreCase);
    }

    private bool CanAccept(RecipeIngredientRequirement requirement)
    {
        if (requirement == null || batterComplete)
            return false;

        if (!IsBowlOnRequiredSurface())
        {
            Warn($"Place {receiverDisplayName} on {requiredSurfaceName} before adding ingredients.");
            return false;
        }

        if (stagedProcess)
        {
            bool accepted = stagedProcess.CanReceive(bowlRole, requirement.IngredientId, requirement.RequiredTool);
            if (!accepted) Warn("Follow the current Cupcake step and use the named bowl; this addition is not needed now.");
            return accepted;
        }

        if (requirement.Stage == RecipeIngredientStage.Finishing && !baseMixed)
        {
            Warn("Mix the base brownie batter before adding the walnuts.");
            return false;
        }

        if (requirement.Stage == RecipeIngredientStage.Base && baseMixed)
        {
            Warn("The base brownie batter is already mixed; add only the finishing ingredient.");
            return false;
        }

        if (HasIngredientRequirement(requirement))
        {
            Warn($"{receiverDisplayName} already has the required " +
                 $"{requirement.UnitLabel} of {requirement.DisplayName}.");
            return false;
        }

        return true;
    }

    private bool HasAllIngredients(RecipeIngredientStage stage)
    {
        if (recipeDefinition == null)
            return stage != RecipeIngredientStage.Base || HasReceivedIngredient;

        bool found = false;
        foreach (RecipeIngredientRequirement requirement in recipeDefinition.GetIngredients(stage))
        {
            found = true;
            if (!HasIngredientRequirement(requirement))
                return false;
        }
        return found || stage != RecipeIngredientStage.Base;
    }

    private void AddRequirementAmount(RecipeIngredientRequirement requirement)
    {
        AddIngredientAmount(requirement.IngredientId, requirement.RequiredTool);
        if (stagedProcess) stagedProcess.Record(requirement.IngredientId, requirement.RequiredTool);
    }

    private void AddIngredientAmount(string ingredientId, RecipeMeasureTool tool)
    {
        string key = GetRequirementKey(ingredientId, tool);
        ingredientAmounts.TryGetValue(key, out int amount);
        ingredientAmounts[key] = amount + 1;
    }

    private static string GetRequirementKey(string ingredientId, RecipeMeasureTool tool)
    {
        return $"{ingredientId?.Trim()}#{(int)tool}";
    }

    private void BeginTrackingWhisk(MixingTool mixingTool)
    {
        activeMixingTool = mixingTool;
        lastWhiskPosition = transform.InverseTransformPoint(mixingTool.transform.position);
        whiskTravelSinceLastPass = 0f;
    }

    private void TrackWhiskMovement(MixingTool mixingTool)
    {
        if (stagedProcess && (pickup == null || pickup.HeldObject != mixingTool.GetComponentInParent<Rigidbody>())) return;
        if (batterComplete || !IsBowlOnRequiredSurface())
        {
            activeMixingTool = null;
            whiskTravelSinceLastPass = 0f;
            return;
        }

        if (activeMixingTool != mixingTool)
        {
            BeginTrackingWhisk(mixingTool);
            return;
        }

        Vector3 currentPosition = transform.InverseTransformPoint(mixingTool.transform.position);
        float travelled = Vector3.Distance(lastWhiskPosition, currentPosition);
        lastWhiskPosition = currentPosition;

        if (travelled > whiskDistancePerPass * 4f)
            return;

        if (Time.time < nextWhiskPassTime)
        {
            whiskTravelSinceLastPass = 0f;
            return;
        }

        whiskTravelSinceLastPass += travelled;
        if (whiskTravelSinceLastPass < whiskDistancePerPass)
            return;

        whiskTravelSinceLastPass = 0f;
        nextWhiskPassTime = Time.time + minimumTimeBetweenPasses;
        RegisterMixPass();
    }

    private void RegisterMixPass()
    {
        if (stagedProcess)
        {
            stagedProcess.TryWork(bowlRole, activeMixingTool ? activeMixingTool.Action : MixingAction.Mix);
            return;
        }
        if (!HasAllBaseIngredients)
        {
            Warn("Add every measured base ingredient before mixing the brownie batter.");
            return;
        }

        if (!baseMixed)
        {
            baseMixPasses = Mathf.Min(baseMixPasses + 1, RequiredBaseMixPasses);
            RecipeFeedback.Report(
                $"Mixing brownie batter: {baseMixPasses}/{RequiredBaseMixPasses}.");
            if (baseMixPasses >= RequiredBaseMixPasses)
            {
                baseMixed = true;
                RecipeFeedback.Report("Base brownie batter mixed. Add the measured walnuts.");
            }
            UpdateContentsVisual();
            return;
        }

        if (!HasAllFinishingIngredients)
        {
            Warn("Add the measured walnuts before the final mixing passes.");
            return;
        }

        finishingMixPasses = Mathf.Min(
            finishingMixPasses + 1, RequiredFinishingMixPasses);
        RecipeFeedback.Report(
            $"Folding walnuts into batter: {finishingMixPasses}/{RequiredFinishingMixPasses}.");
        if (finishingMixPasses >= RequiredFinishingMixPasses)
        {
            batterComplete = true;
            RecipeFeedback.Report(
                "Brownie batter complete. Pour it into the parchment-lined deep brownie pan.");
        }
        UpdateContentsVisual();
    }

    private bool IsBowlOnRequiredSurface()
    {
        if (string.IsNullOrWhiteSpace(requiredSurfaceName))
            return true;
        return bowlItem != null && bowlItem.RestingSurface != null &&
               string.Equals(
                   bowlItem.RestingSurface.name,
                   requiredSurfaceName,
                   StringComparison.OrdinalIgnoreCase);
    }

    private void Warn(string message)
    {
        if (Time.unscaledTime < nextFeedbackTime)
            return;
        nextFeedbackTime = Time.unscaledTime + 2f;
        RecipeFeedback.Warning(message);
    }

    private void UpdateContentsVisual()
    {
        if (stagedProcess) return;
        if (Application.isPlaying && bowlBody != null)
        {
            if (preparationVisual == null)
            {
                GameObject presentation = new("Brownie separate ingredients");
                presentation.transform.SetParent(bowlBody.transform, false);
                preparationVisual = presentation.AddComponent<BrowniePreparationVisual>();
            }
            preparationVisual.Refresh(this);
        }
        int total = 0;
        int nonButter = 0;
        foreach (KeyValuePair<string, int> pair in ingredientAmounts)
        {
            total += pair.Value;
            if (!pair.Key.StartsWith(
                    acceptedIngredient + "#", StringComparison.OrdinalIgnoreCase))
                nonButter += pair.Value;
        }

        if (receivedContentsVisual != null)
            receivedContentsVisual.SetActive(false);

        if (batterContentsVisual == null)
            return;
        batterContentsVisual.SetActive(baseMixPasses > 0 && !batterTransferred);
        if (baseMixPasses <= 0 || batterTransferred)
            return;

        int totalRequired = 0;
        int totalReceived = 0;
        if (recipeDefinition != null)
        {
            foreach (RecipeIngredientRequirement requirement in recipeDefinition.Ingredients)
            {
                if (requirement == null)
                    continue;
                totalRequired += requirement.RequiredQuantity;
                totalReceived += Mathf.Min(
                    requirement.RequiredQuantity,
                    GetIngredientAmount(requirement.IngredientId, requirement.RequiredTool));
            }
        }

        float ingredientProgress = totalRequired > 0
            ? (float)totalReceived / totalRequired
            : 0f;
        float mixProgress = baseMixed
            ? .75f + .25f * (RequiredFinishingMixPasses > 0
                ? (float)finishingMixPasses / RequiredFinishingMixPasses
                : 1f)
            : RequiredBaseMixPasses > 0
                ? .75f * baseMixPasses / RequiredBaseMixPasses
                : 0f;

        bool hasCocoa = GetIngredientAmount("Cocoa", RecipeMeasureTool.HalfCup) > 0;

        BrownieFoodVisual styledVisual =
            batterContentsVisual.GetComponent<BrownieFoodVisual>();
        if (styledVisual != null)
        {
            styledVisual.ShowRaw(false);
            styledVisual.SetFillProgress(Mathf.Clamp01((float)baseMixPasses / Mathf.Max(1, RequiredBaseMixPasses)), HasAllFinishingIngredients);
            Color initialColor = hasCocoa
                ? unmixedBatterColor
                : new Color(.98f, .97f, .93f, 1f);
            Color color = new Color32(102, 60, 2, 255);
            styledVisual.ApplyRawColor(color);
            return;
        }

        foreach (Renderer renderer in batterContentsVisual.GetComponentsInChildren<Renderer>(true))
        {
            Material material = renderer.material;
            Color initialColor = hasCocoa
                ? unmixedBatterColor
                : new Color(.98f, .97f, .93f, 1f);
            Color color = Color.Lerp(
                initialColor, mixedBatterColor, Mathf.Clamp01(mixProgress));
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            else if (material.HasProperty("_Color"))
                material.SetColor("_Color", color);
        }
    }

    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
        bowlItem = GetComponentInParent<PreparationItem>();
    }

    private void OnValidate()
    {
        BoxCollider receiver = GetComponent<BoxCollider>();
        if (receiver != null)
            receiver.isTrigger = true;
        whiskDistancePerPass = Mathf.Max(.001f, whiskDistancePerPass);
        minimumTimeBetweenPasses = Mathf.Max(.1f, minimumTimeBetweenPasses);
    }
}
