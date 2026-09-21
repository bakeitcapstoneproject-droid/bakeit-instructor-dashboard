using System.Collections.Generic;
using UnityEngine;

public class BowlReceiver : MonoBehaviour
{
    [Header("Recipe")]
    [SerializeField] private RecipeDefinition recipeDefinition;
    [SerializeField] private GameObject mixtureResult;

    [Header("Chocolate-Chip Cookie Ingredients")]
    [SerializeField, Min(1)] private int requiredFlourScoops = 1;
    [SerializeField, Min(1)] private int requiredSugarScoops = 1;
    [SerializeField, Min(1)] private int requiredButterPortions = 1;
    [SerializeField, Min(1)] private int requiredEggs = 1;
    [SerializeField, Min(1)] private int requiredChocolateChipScoops = 3;

    [Header("Mixing")]
    [SerializeField, Min(1)] private int requiredWhiskPasses = 4;
    [SerializeField, Min(0)] private int requiredChipMixPasses = 3;
    [SerializeField, Min(0.001f)] private float whiskDistancePerPass = 0.06f;
    [SerializeField, Min(0f)] private float minimumTimeBetweenPasses = 1.2f;
    [SerializeField, Range(0f, 1f)] private float rawDoughSmoothness = 0.18f;
    [SerializeField] private Color rawCookieDoughColor = new Color(.93f, .80f, .57f, 1f);

    [Header("Ingredient Visuals")]
    [Tooltip("Contents center in the bowl parent's local space, independent of the scoop-detection trigger.")]
    [SerializeField] private Vector3 ingredientVisualCenter = new Vector3(0f, 0.06f, 0f);
    [SerializeField] private GameObject chocolateChipVisualPrefab;
    [SerializeField] private Color flourColor =
        new Color(0.96f, 0.93f, 0.84f, 1f);
    [SerializeField] private Color sugarColor =
        new Color(1f, 0.985f, 0.94f, 1f);
    [SerializeField] private Color butterColor =
        new Color(0.96f, 0.84f, 0.48f, 1f);
    [SerializeField] private Color eggWhiteColor =
        new Color(1f, 0.98f, 0.9f, 1f);
    [SerializeField] private Color yolkColor =
        new Color(1f, 0.55f, 0.03f, 1f);

    private readonly HashSet<Ingredient> ingredients = new();
    private readonly Dictionary<string, int> ingredientAmounts =
        new(System.StringComparer.OrdinalIgnoreCase);
    private bool recipeComplete;
    private bool baseDoughMixed;
    private int whiskPasses;
    private int chipMixPasses;
    private MixingTool activeMixingTool;
    private Vector3 lastWhiskPosition;
    private float whiskTravelSinceLastPass;
    private float nextWhiskPassTime;
    private Transform ingredientVisualRoot;
    private Transform flourVisual;
    private Transform sugarVisual;
    private Transform butterVisual;
    private Transform eggWhiteVisual;
    private Transform eggYolkVisual;
    private Transform chocolateChipVisual;
    private Transform vanillaVisual;
    private Transform bakingSodaVisual;
    private Transform saltVisual;
    private Transform mixingProgressVisual;
    private BoxCollider bowlTrigger;
    private MiseEnPlaceStation preparation;
    private float nextPreparationWarning;

    public RecipeDefinition ActiveRecipe => recipeDefinition;
    public bool IsRecipeComplete => recipeComplete;
    public bool HasAllCookieIngredients => HasCookieIngredients();
    public bool HasAllBaseIngredients => HasBaseIngredients();
    public bool IsBaseDoughMixed => baseDoughMixed;
    public bool HasRequiredFlour =>
        GetIngredientAmount("Flour") >= requiredFlourScoops;
    public bool HasRequiredSugar =>
        GetIngredientAmount("Sugar") >= requiredSugarScoops;
    public bool HasRequiredButter =>
        GetIngredientAmount("Butter") >= requiredButterPortions;
    public bool HasRequiredEgg =>
        GetIngredientAmount("Egg") >= requiredEggs;
    public bool HasRequiredChocolateChips =>
        GetIngredientAmount("ChocolateChips") >= requiredChocolateChipScoops;
    public int ChocolateChipScoopCount =>
        GetIngredientAmount("ChocolateChips");
    public int RequiredChocolateChipScoops => requiredChocolateChipScoops;
    public int WhiskPassCount => whiskPasses;
    public int RequiredWhiskPasses => requiredWhiskPasses;
    public int ChipMixPassCount => chipMixPasses;
    public int RequiredChipMixPasses => requiredChipMixPasses;
    public bool IsBowlReady => preparation == null || preparation.BowlPlaced;
    public Transform CompletedDough => mixtureResult != null
        ? mixtureResult.transform
        : null;

    private void Awake()
    {
        ApplyRecipeDefinition();
    }

    private void Start()
    {
        preparation = FindAnyObjectByType<MiseEnPlaceStation>();
        if (mixtureResult != null)
            mixtureResult.SetActive(false);

        CreateIngredientVisuals();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (preparation != null && (!preparation.IsPrepared || !IsBowlReady) &&
            (other.GetComponentInParent<Ingredient>() != null || other.GetComponentInParent<MeasuringScoop>() != null))
        {
            if (Time.unscaledTime >= nextPreparationWarning)
            {
                nextPreparationWarning = Time.unscaledTime + 2f;
                RecipeFeedback.Warning(!preparation.IsPrepared
                    ? "Stage ingredients on Cold Ingredient Rest, tools on Utensils Area, and the bowl in MIX first."
                    : "Place the bowl back in MIX before adding ingredients.");
            }
            return;
        }
        MeasuringScoop scoop =
            other.GetComponentInParent<MeasuringScoop>();

        if (scoop != null)
        {
            // MeasuringScoop resolves a deliberate held tilt over the mouth.
            // Merely touching the bowl must not empty the measure.
            return;
        }

        Ingredient ingredient =
            other.GetComponentInParent<Ingredient>();

        if (ingredient != null)
        {
            if (mixtureResult != null &&
                (ingredient.gameObject == mixtureResult ||
                 ingredient.transform.IsChildOf(mixtureResult.transform)))
            {
                return;
            }

            if (!CanAcceptIngredient(
                    ingredient.ingredientName,
                    out string ingredientName))
            {
                return;
            }

            if (ingredients.Add(ingredient))
            {
                AddIngredientAmount(ingredientName);
                UpdateIngredientVisual(ingredientName);

                if (ShouldConsumeWholeIngredient(ingredientName))
                {
                    ConsumeWholeIngredient(ingredient);
                }

                RecipeFeedback.Report(
                    $"{ingredientName} added to bowl. " +
                    $"Amount: " +
                    $"{GetIngredientAmount(ingredientName)}");
            }

            return;
        }

        MixingTool mixingTool =
            other.GetComponentInParent<MixingTool>();

        if (mixingTool != null)
            BeginTrackingWhisk(mixingTool);
    }

    private void OnTriggerStay(Collider other)
    {
        MixingTool mixingTool =
            other.GetComponentInParent<MixingTool>();

        if (mixingTool == null)
            return;

        TrackWhiskMovement(mixingTool);
    }

    public bool TryPourMeasure(MeasuringScoop scoop)
    {
        if(scoop==null || !scoop.IsHeld || !scoop.IsFilled || !scoop.IsTilted || scoop.FindPourDestination()!=this)return false;
        if(preparation!=null && (!preparation.IsPrepared || !IsBowlReady))
        {
            RecipeFeedback.Warning("Prepare the ingredients and utensils, and place the bowl in MIX before pouring.");
            return false;
        }
        if(!scoop.CanMeasure(scoop.CurrentIngredient))
        {
            if (recipeDefinition != null &&
                recipeDefinition.TryGetIngredient(
                    scoop.CurrentIngredient,
                    out RecipeIngredientRequirement requirement))
            {
                RecipeFeedback.Warning(
                    $"Use {recipeDefinition.GetToolDisplayName(requirement.RequiredTool)} for " +
                    $"{requirement.DisplayName}. Return or discard this measure first.");
            }
            else
            {
                RecipeFeedback.Warning("Use Measuring Cup for flour/sugar and Small Measuring Spoon for chips. Return or discard this measure first.");
            }
            return false;
        }
        if(!CanAcceptIngredient(scoop.CurrentIngredient,out var ingredient))return false;
        if(!scoop.CompleteTransfer(MeasureDisposition.AddedToBowl,name,transform.parent.TransformPoint(ingredientVisualCenter)))return false;
        AddIngredientAmount(ingredient);UpdateIngredientVisual(ingredient);
        RecipeFeedback.Report($"1 {scoop.MeasureUnit} of {ingredient} poured into bowl.");
        if(IsIngredient(ingredient,"ChocolateChips") && HasRequiredChocolateChips)
        {
            whiskTravelSinceLastPass=0;activeMixingTool=null;
            RecipeFeedback.Report($"Chips added. Whisk {requiredChipMixPasses} more passes to combine.");
        }
        return true;
    }

    private void OnTriggerExit(Collider other)
    {
        MixingTool mixingTool =
            other.GetComponentInParent<MixingTool>();

        if (mixingTool == null || mixingTool != activeMixingTool)
            return;

        activeMixingTool = null;
        whiskTravelSinceLastPass = 0f;
    }

    private void BeginTrackingWhisk(MixingTool mixingTool)
    {
        activeMixingTool = mixingTool;
        lastWhiskPosition = transform.InverseTransformPoint(mixingTool.transform.position);
        whiskTravelSinceLastPass = 0f;

        if (!recipeComplete && !baseDoughMixed && !HasBaseIngredients())
            LogMissingBaseIngredients();
    }

    private void TrackWhiskMovement(MixingTool mixingTool)
    {
        if (recipeComplete || !IsBowlReady || (baseDoughMixed && !HasRequiredChocolateChips))
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
        float travelled = Vector3.Distance(
            lastWhiskPosition,
            currentPosition);
        lastWhiskPosition = currentPosition;

        if (!HasBaseIngredients())
        {
            whiskTravelSinceLastPass = 0f;
            return;
        }

        // Ignore a teleport-sized jump when the whisk is first picked up or
        // snapped to the hold point. Ordinary stirring movement is small.
        if (travelled > whiskDistancePerPass * 4f)
            return;

        if (Time.time < nextWhiskPassTime)
        {
            // Movement during the cooldown belongs to the gesture that just
            // counted. Do not bank it toward another pass.
            whiskTravelSinceLastPass = 0f;
            return;
        }

        whiskTravelSinceLastPass += travelled;

        if (whiskTravelSinceLastPass < whiskDistancePerPass)
            return;

        whiskTravelSinceLastPass = 0f;
        nextWhiskPassTime = Time.time + minimumTimeBetweenPasses;
        RegisterWhiskPass();
    }

    private void RegisterWhiskPass()
    {
        if (recipeComplete || !IsBowlReady || (preparation != null && !preparation.IsPrepared))
            return;

        if (!HasBaseIngredients())
        {
            LogMissingBaseIngredients();
            return;
        }

        if (baseDoughMixed)
        {
            if (!HasRequiredChocolateChips) return;
            chipMixPasses = Mathf.Min(chipMixPasses + 1, requiredChipMixPasses);
            // Sink the chip mound gradually into the base dough while combining.
            if (chocolateChipVisual != null && mixingProgressVisual != null)
                chocolateChipVisual.localPosition = Vector3.Lerp(chocolateChipVisual.localPosition,
                    mixingProgressVisual.localPosition, (float)chipMixPasses / requiredChipMixPasses);
            RecipeFeedback.Report($"Combining chocolate chips: {chipMixPasses}/{requiredChipMixPasses}");
            TryCompleteCookieDough();
            return;
        }

        whiskPasses++;
        UpdateMixingProgressVisual();

        RecipeFeedback.Report(
            $"Whisking mixture: {whiskPasses}/{requiredWhiskPasses}");

        if (whiskPasses < requiredWhiskPasses)
            return;

        baseDoughMixed = true;
        RecipeFeedback.Report(
            $"Base cookie dough mixed. Add {requiredChocolateChipScoops} small scoops of chocolate chips.");
    }

    private void UpdateMixingProgressVisual()
    {
        if (mixingProgressVisual == null)
            return;

        float progress = Mathf.Clamp01(
            (float)whiskPasses / requiredWhiskPasses);

        float bowlDiameter = bowlTrigger != null
            ? Mathf.Min(bowlTrigger.size.x, bowlTrigger.size.z)
            : 0.2f;

        // The mixture starts wide and runny, then becomes smaller, taller,
        // and more dough-like with every completed whisk pass.
        Vector3 runnyScale = new Vector3(
            bowlDiameter * 0.68f,
            bowlDiameter * 0.07f,
            bowlDiameter * 0.62f);
        Vector3 clumpyScale = new Vector3(
            bowlDiameter * 0.54f,
            bowlDiameter * 0.23f,
            bowlDiameter * 0.54f);

        mixingProgressVisual.localScale = Vector3.Lerp(
            runnyScale,
            clumpyScale,
            progress);
        mixingProgressVisual.localPosition = new Vector3(
            0f,
            Mathf.Lerp(
                -bowlDiameter * 0.06f,
                bowlDiameter * 0.015f,
                progress),
            0f);
        mixingProgressVisual.gameObject.SetActive(true);

        if (chocolateChipVisual != null &&
            GetIngredientAmount("ChocolateChips") > 0)
        {
            chocolateChipVisual.localPosition =
                mixingProgressVisual.localPosition;
            chocolateChipVisual.localScale =
                mixingProgressVisual.localScale;
        }

        SetVisualColor(
            mixingProgressVisual,
            Color.Lerp(flourColor, rawCookieDoughColor, progress));

        // Let the original ingredients disappear in stages instead of
        // abruptly switching everything off on the final pass.
        if (eggYolkVisual != null && whiskPasses >= 1)
            eggYolkVisual.gameObject.SetActive(false);

        if (eggWhiteVisual != null && progress >= 0.5f)
            eggWhiteVisual.gameObject.SetActive(false);

        if (butterVisual != null && progress >= 0.5f)
            butterVisual.gameObject.SetActive(false);

        if (flourVisual != null && progress >= 0.75f)
            flourVisual.gameObject.SetActive(false);

        if (sugarVisual != null && progress >= 0.75f)
            sugarVisual.gameObject.SetActive(false);

        if (vanillaVisual != null && progress >= 0.5f)
            vanillaVisual.gameObject.SetActive(false);

        if (bakingSodaVisual != null && progress >= 0.75f)
            bakingSodaVisual.gameObject.SetActive(false);

        if (saltVisual != null && progress >= 0.75f)
            saltVisual.gameObject.SetActive(false);
    }

    private void SetVisualColor(Transform visual, Color color)
    {
        Renderer visualRenderer = visual.GetComponent<Renderer>();

        if (visualRenderer == null)
            return;

        Material visualMaterial = visualRenderer.material;

        if (visualMaterial.HasProperty("_BaseColor"))
            visualMaterial.SetColor("_BaseColor", color);
        else if (visualMaterial.HasProperty("_Color"))
            visualMaterial.SetColor("_Color", color);
    }

    private void ApplyRawDoughAppearance()
    {
        foreach (Renderer doughRenderer in
                 mixtureResult.GetComponentsInChildren<Renderer>(true))
        {
            if (doughRenderer.GetComponentInParent<ChocolateChipVisual>() != null)
                continue;

            Material doughMaterial = doughRenderer.material;

            if (doughMaterial.HasProperty("_BaseColor"))
                doughMaterial.SetColor("_BaseColor", rawCookieDoughColor);
            else if (doughMaterial.HasProperty("_Color"))
                doughMaterial.SetColor("_Color", rawCookieDoughColor);

            if (doughMaterial.HasProperty("_Metallic"))
                doughMaterial.SetFloat("_Metallic", 0f);

            if (doughMaterial.HasProperty("_Smoothness"))
            {
                doughMaterial.SetFloat(
                    "_Smoothness",
                    rawDoughSmoothness);
            }
        }
    }

    private bool HasCookieIngredients()
    {
        if (recipeDefinition != null)
        {
            foreach (RecipeIngredientRequirement requirement in recipeDefinition.Ingredients)
            {
                if (requirement != null &&
                    requirement.Stage != RecipeIngredientStage.Decoration &&
                    GetIngredientAmount(requirement.IngredientId) < requirement.RequiredQuantity)
                {
                    return false;
                }
            }

            return true;
        }

        return HasBaseIngredients() &&
               GetIngredientAmount("ChocolateChips") >= requiredChocolateChipScoops;
    }

    private bool HasBaseIngredients()
    {
        if (recipeDefinition != null)
        {
            foreach (RecipeIngredientRequirement requirement in
                     recipeDefinition.GetIngredients(RecipeIngredientStage.Base))
            {
                if (GetIngredientAmount(requirement.IngredientId) < requirement.RequiredQuantity)
                    return false;
            }

            return true;
        }

        return GetIngredientAmount("Flour") >= requiredFlourScoops &&
               GetIngredientAmount("Sugar") >= requiredSugarScoops &&
               GetIngredientAmount("Butter") >= requiredButterPortions &&
               GetIngredientAmount("Egg") >= requiredEggs;
    }

    private bool CanAcceptIngredient(string value, out string ingredientName)
    {
        ingredientName = value?.Trim();

        if (recipeComplete)
        {
            RecipeFeedback.Warning("This cookie batch is already mixed. No more ingredients can be added.");
            return false;
        }

        int requiredAmount;
        RecipeIngredientStage ingredientStage;

        if (recipeDefinition != null)
        {
            if (!recipeDefinition.TryGetIngredient(ingredientName, out RecipeIngredientRequirement requirement))
            {
                RecipeFeedback.Warning(
                    $"The {recipeDefinition.ProductSingular} bowl cannot accept " +
                    $"'{ingredientName ?? "unnamed ingredient"}'. Use only the listed recipe ingredients.");
                return false;
            }

            ingredientName = requirement.IngredientId;
            requiredAmount = requirement.RequiredQuantity;
            ingredientStage = requirement.Stage;
        }
        else
        {
            ingredientStage = RecipeIngredientStage.Base;

            switch (ingredientName?.ToLowerInvariant())
            {
                case "flour":
                    ingredientName = "Flour";
                    requiredAmount = requiredFlourScoops;
                    break;
                case "sugar":
                    ingredientName = "Sugar";
                    requiredAmount = requiredSugarScoops;
                    break;
                case "butter":
                    ingredientName = "Butter";
                    requiredAmount = requiredButterPortions;
                    break;
                case "egg":
                    ingredientName = "Egg";
                    requiredAmount = requiredEggs;
                    break;
                case "chocolatechips":
                    ingredientName = "ChocolateChips";
                    requiredAmount = requiredChocolateChipScoops;
                    ingredientStage = RecipeIngredientStage.Finishing;
                    break;
                default:
                    RecipeFeedback.Warning(
                        $"The cookie bowl cannot accept '{ingredientName ?? "unnamed ingredient"}'. " +
                        "Use flour, sugar, butter, egg, and chocolate chips.");
                    return false;
            }
        }

        bool isFinishingIngredient =
            ingredientStage == RecipeIngredientStage.Finishing;

        if (isFinishingIngredient && !baseDoughMixed)
        {
            RecipeFeedback.Warning(
                "Whisk the base ingredients before adding the finishing ingredient.");
            return false;
        }

        if (!isFinishingIngredient && baseDoughMixed)
        {
            RecipeFeedback.Warning(
                "The base dough is already mixed. Return the measure to its matching source, or hold Q to discard. Use Small Measuring Spoon for chips.");
            return false;
        }

        if (GetIngredientAmount(ingredientName) >= requiredAmount)
        {
            RecipeFeedback.Warning(
                $"The bowl already has the required amount of {ingredientName}. " +
                "This ingredient was not consumed.");
            return false;
        }

        return true;
    }

    private void LogMissingBaseIngredients()
    {
        var missing = new List<string>();

        if (recipeDefinition != null)
        {
            foreach (RecipeIngredientRequirement requirement in
                     recipeDefinition.GetIngredients(RecipeIngredientStage.Base))
            {
                AddMissingIngredient(
                    missing,
                    requirement.IngredientId,
                    requirement.RequiredQuantity,
                    requirement.DisplayName + " (" + requirement.UnitLabel + ")");
            }

            RecipeFeedback.Warning("Before mixing, add " + string.Join(", ", missing) + ".");
            return;
        }

        AddMissingIngredient(missing, "Flour", requiredFlourScoops, "flour scoop(s)");
        AddMissingIngredient(missing, "Sugar", requiredSugarScoops, "sugar scoop(s)");
        AddMissingIngredient(missing, "Butter", requiredButterPortions, "butter portion(s)");
        AddMissingIngredient(missing, "Egg", requiredEggs, "egg(s)");
        RecipeFeedback.Warning("Before mixing, add " + string.Join(", ", missing) + ".");
    }

    private void TryCompleteCookieDough()
    {
        if (recipeComplete ||
            !baseDoughMixed ||
            !HasRequiredChocolateChips ||
            chipMixPasses < requiredChipMixPasses)
        {
            return;
        }

        recipeComplete = true;

        if (mixtureResult != null)
        {
            AttachChocolateChipsToDough();
            mixtureResult.SetActive(true);
            ApplyRawDoughAppearance();
        }

        if (ingredientVisualRoot != null)
            ingredientVisualRoot.gameObject.SetActive(false);

        RecipeFeedback.Report(
            "Chocolate chips combined. Move the finished cookie dough to the chopping board.");
    }

    private void AddMissingIngredient(
        List<string> missing,
        string ingredientName,
        int requiredAmount,
        string displayName)
    {
        int remaining = requiredAmount - GetIngredientAmount(ingredientName);

        if (remaining > 0)
            missing.Add($"{remaining} {displayName}");
    }

    public int GetIngredientAmount(string ingredientName)
    {
        if (string.IsNullOrWhiteSpace(ingredientName))
            return 0;

        return ingredientAmounts.TryGetValue(
            ingredientName.Trim(),
            out int amount)
            ? amount
            : 0;
    }

    public bool HasIngredientRequirement(RecipeIngredientRequirement requirement)
    {
        return requirement != null &&
               GetIngredientAmount(requirement.IngredientId) >= requirement.RequiredQuantity;
    }

    public bool TryGetIngredientRequirement(
        string ingredientName,
        out RecipeIngredientRequirement requirement)
    {
        if (recipeDefinition != null)
            return recipeDefinition.TryGetIngredient(ingredientName, out requirement);

        requirement = null;
        return false;
    }

    private bool ShouldConsumeWholeIngredient(string ingredientName)
    {
        return recipeDefinition != null &&
               recipeDefinition.TryGetIngredient(ingredientName, out RecipeIngredientRequirement requirement)
            ? requirement.RequiredTool == RecipeMeasureTool.WholeItem
            : IsIngredient(ingredientName, "Egg") || IsIngredient(ingredientName, "Butter");
    }

    private void ApplyRecipeDefinition()
    {
        if (recipeDefinition == null)
            return;

        requiredWhiskPasses = recipeDefinition.BaseMixPasses;
        requiredChipMixPasses = recipeDefinition.FinishingMixPasses;

        ApplyRequirementQuantity("Flour", ref requiredFlourScoops);
        ApplyRequirementQuantity("Sugar", ref requiredSugarScoops);
        ApplyRequirementQuantity("Butter", ref requiredButterPortions);
        ApplyRequirementQuantity("Egg", ref requiredEggs);
        ApplyRequirementQuantity("ChocolateChips", ref requiredChocolateChipScoops);
    }

    private void ApplyRequirementQuantity(string ingredientName, ref int target)
    {
        if (recipeDefinition.TryGetIngredient(
                ingredientName,
                out RecipeIngredientRequirement requirement))
        {
            target = requirement.RequiredQuantity;
        }
    }

    private void AddIngredientAmount(string ingredientName)
    {
        if (string.IsNullOrWhiteSpace(ingredientName))
            return;

        ingredientName = ingredientName.Trim();
        ingredientAmounts.TryGetValue(
            ingredientName,
            out int currentAmount);

        ingredientAmounts[ingredientName] = currentAmount + 1;
    }

    private void CreateIngredientVisuals()
    {
        bowlTrigger = GetComponent<BoxCollider>();

        GameObject visualRoot = new GameObject("Bowl Ingredient Visuals");
        ingredientVisualRoot = visualRoot.transform;
        ingredientVisualRoot.SetParent(transform, false);

        float bowlDiameter = bowlTrigger != null
            ? Mathf.Min(bowlTrigger.size.x, bowlTrigger.size.z)
            : 0.2f;

        // Detection volume and vessel interior are different spaces. Deriving
        // contents height from the trigger used to put flour below the bowl.
        Transform bowlTransform = transform.parent != null ? transform.parent : transform;
        ingredientVisualRoot.position = bowlTransform.TransformPoint(ingredientVisualCenter);

        flourVisual = CreateVisualSphere(
            "Flour Mound",
            ingredientVisualRoot,
            flourColor,
            new Vector3(
                bowlDiameter * 0.62f,
                bowlDiameter * 0.22f,
                bowlDiameter * 0.62f),
            0.08f,
            1f);
        flourVisual.localPosition =
            new Vector3(
                bowlDiameter * 0.02f,
                -bowlDiameter * 0.12f,
                0f);
        flourVisual.gameObject.SetActive(false);

        sugarVisual = CreateVisualSphere(
            "Sugar Mound",
            ingredientVisualRoot,
            sugarColor,
            new Vector3(
                bowlDiameter * 0.44f,
                bowlDiameter * 0.15f,
                bowlDiameter * 0.44f),
            0.3f,
            1f);
        sugarVisual.localPosition =
            new Vector3(
                -bowlDiameter * 0.08f,
                -bowlDiameter * 0.08f,
                bowlDiameter * 0.04f);
        sugarVisual.gameObject.SetActive(false);

        vanillaVisual = CreateVisualSphere(
            "Vanilla Extract",
            ingredientVisualRoot,
            new Color(0.30f, 0.08f, 0.025f, 1f),
            new Vector3(
                bowlDiameter * 0.25f,
                bowlDiameter * 0.025f,
                bowlDiameter * 0.22f),
            0.4f,
            0.9f);
        vanillaVisual.localPosition = new Vector3(
            bowlDiameter * 0.14f,
            -bowlDiameter * 0.035f,
            -bowlDiameter * 0.12f);
        vanillaVisual.gameObject.SetActive(false);

        bakingSodaVisual = CreateVisualSphere(
            "Baking Soda Mound",
            ingredientVisualRoot,
            new Color(0.92f, 0.93f, 0.88f, 1f),
            Vector3.one * bowlDiameter * 0.16f,
            0.08f,
            1f);
        bakingSodaVisual.localPosition = new Vector3(
            bowlDiameter * 0.17f,
            -bowlDiameter * 0.04f,
            bowlDiameter * 0.13f);
        bakingSodaVisual.gameObject.SetActive(false);

        saltVisual = CreateVisualSphere(
            "Salt Pinch",
            ingredientVisualRoot,
            new Color(0.99f, 0.98f, 0.94f, 1f),
            Vector3.one * bowlDiameter * 0.10f,
            0.12f,
            1f);
        saltVisual.localPosition = new Vector3(
            -bowlDiameter * 0.18f,
            -bowlDiameter * 0.035f,
            bowlDiameter * 0.11f);
        saltVisual.gameObject.SetActive(false);

        butterVisual = CreateVisualSphere(
            "Softened Butter",
            ingredientVisualRoot,
            butterColor,
            new Vector3(
                bowlDiameter * 0.34f,
                bowlDiameter * 0.13f,
                bowlDiameter * 0.28f),
            0.32f,
            1f);
        butterVisual.localPosition =
            new Vector3(
                -bowlDiameter * 0.13f,
                -bowlDiameter * 0.055f,
                -bowlDiameter * 0.15f);
        butterVisual.gameObject.SetActive(false);

        eggWhiteVisual = CreateVisualSphere(
            "Cracked Egg White",
            ingredientVisualRoot,
            eggWhiteColor,
            new Vector3(
                bowlDiameter * 0.52f,
                bowlDiameter * 0.05f,
                bowlDiameter * 0.42f),
            0.75f,
            0.38f);
        eggWhiteVisual.localPosition =
            new Vector3(
                bowlDiameter * 0.08f,
                bowlDiameter * 0.015f,
                bowlDiameter * 0.03f);
        eggWhiteVisual.gameObject.SetActive(false);

        eggYolkVisual = CreateVisualSphere(
            "Egg Yolk",
            ingredientVisualRoot,
            yolkColor,
            Vector3.one * bowlDiameter * 0.32f,
            0.55f,
            1f);
        eggYolkVisual.localScale = new Vector3(
            bowlDiameter * 0.32f,
            bowlDiameter * 0.22f,
            bowlDiameter * 0.32f);
        eggYolkVisual.localPosition =
            new Vector3(
                bowlDiameter * 0.08f,
                bowlDiameter * 0.075f,
                bowlDiameter * 0.03f);
        eggYolkVisual.gameObject.SetActive(false);

        mixingProgressVisual = CreateVisualSphere(
            "Forming Dough",
            ingredientVisualRoot,
            flourColor,
            new Vector3(
                bowlDiameter * 0.68f,
                bowlDiameter * 0.07f,
                bowlDiameter * 0.62f),
            0.22f,
            1f);
        mixingProgressVisual.localPosition =
            new Vector3(0f, -bowlDiameter * 0.06f, 0f);
        mixingProgressVisual.gameObject.SetActive(false);

        if (chocolateChipVisualPrefab != null)
        {
            chocolateChipVisual = Instantiate(
                chocolateChipVisualPrefab,
                ingredientVisualRoot).transform;
            chocolateChipVisual.name = "Chocolate Chips";
            chocolateChipVisual.localPosition =
                new Vector3(-bowlDiameter * 0.03f, -bowlDiameter * 0.02f, 0f);
            chocolateChipVisual.localRotation = Quaternion.identity;
            chocolateChipVisual.localScale = new Vector3(
                bowlDiameter * 0.56f,
                bowlDiameter * 0.2f,
                bowlDiameter * 0.56f);
            chocolateChipVisual.gameObject.SetActive(false);
        }
    }

    private void AttachChocolateChipsToDough()
    {
        if (mixtureResult.GetComponent<CookieAppearance>() != null) return;
        if (chocolateChipVisualPrefab == null ||
            GetIngredientAmount("ChocolateChips") == 0 ||
            mixtureResult.GetComponentInChildren<ChocolateChipVisual>(true) != null)
        {
            return;
        }

        MeshFilter doughMesh = mixtureResult.GetComponent<MeshFilter>();

        if (doughMesh == null || doughMesh.sharedMesh == null)
            return;

        Bounds doughBounds = doughMesh.sharedMesh.bounds;
        Transform chips = Instantiate(
            chocolateChipVisualPrefab,
            mixtureResult.transform).transform;
        chips.name = "Chocolate Chips";
        chips.localPosition = doughBounds.center;
        chips.localRotation = Quaternion.identity;
        chips.localScale = doughBounds.size;
        chips.gameObject.SetActive(true);
    }

    private Transform CreateVisualSphere(
        string visualName,
        Transform parent,
        Color color,
        Vector3 localScale,
        float smoothness,
        float opacity)
    {
        GameObject visual = GameObject.CreatePrimitive(
            PrimitiveType.Sphere);
        visual.name = visualName;
        visual.transform.SetParent(parent, false);
        visual.transform.localScale = localScale;

        Collider generatedCollider = visual.GetComponent<Collider>();

        if (generatedCollider != null)
            Destroy(generatedCollider);

        Renderer visualRenderer = visual.GetComponent<Renderer>();

        if (visualRenderer != null)
        {
            Shader visualShader =
                Shader.Find("Universal Render Pipeline/Lit");

            if (visualShader == null)
                visualShader = Shader.Find("Standard");

            if (visualShader != null)
            {
                Material visualMaterial = new Material(visualShader);
                Color displayColor = color;
                displayColor.a = opacity;

                if (visualMaterial.HasProperty("_BaseColor"))
                    visualMaterial.SetColor("_BaseColor", displayColor);
                else if (visualMaterial.HasProperty("_Color"))
                    visualMaterial.SetColor("_Color", displayColor);

                if (visualMaterial.HasProperty("_Metallic"))
                    visualMaterial.SetFloat("_Metallic", 0f);

                if (visualMaterial.HasProperty("_Smoothness"))
                    visualMaterial.SetFloat("_Smoothness", smoothness);

                if (opacity < 0.999f)
                {
                    visualMaterial.SetOverrideTag(
                        "RenderType",
                        "Transparent");

                    if (visualMaterial.HasProperty("_Surface"))
                        visualMaterial.SetFloat("_Surface", 1f);

                    if (visualMaterial.HasProperty("_Blend"))
                        visualMaterial.SetFloat("_Blend", 0f);

                    if (visualMaterial.HasProperty("_SrcBlend"))
                        visualMaterial.SetFloat(
                            "_SrcBlend",
                            (float)UnityEngine.Rendering.BlendMode.SrcAlpha);

                    if (visualMaterial.HasProperty("_DstBlend"))
                        visualMaterial.SetFloat(
                            "_DstBlend",
                            (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

                    if (visualMaterial.HasProperty("_ZWrite"))
                        visualMaterial.SetFloat("_ZWrite", 0f);

                    visualMaterial.EnableKeyword(
                        "_SURFACE_TYPE_TRANSPARENT");
                    visualMaterial.renderQueue =
                        (int)UnityEngine.Rendering.RenderQueue.Transparent;
                }

                visualRenderer.material = visualMaterial;
            }
        }

        return visual.transform;
    }

    private void UpdateIngredientVisual(string ingredientName)
    {
        if (ingredientVisualRoot == null)
            return;

        if (IsIngredient(ingredientName, "Flour") &&
            flourVisual != null)
        {
            int amount = GetIngredientAmount("Flour");
            float bowlDiameter = bowlTrigger != null
                ? Mathf.Min(bowlTrigger.size.x, bowlTrigger.size.z)
                : 0.2f;
            float widthMultiplier = Mathf.Clamp(
                0.62f + (amount - 1) * 0.05f,
                0.62f,
                0.8f);
            float heightMultiplier = Mathf.Clamp(
                0.16f + amount * 0.04f,
                0.2f,
                0.34f);

            flourVisual.localScale = new Vector3(
                bowlDiameter * widthMultiplier,
                bowlDiameter * heightMultiplier,
                bowlDiameter * widthMultiplier);
            flourVisual.gameObject.SetActive(true);
        }

        if (IsIngredient(ingredientName, "Sugar") &&
            sugarVisual != null)
        {
            int amount = GetIngredientAmount("Sugar");
            float bowlDiameter = bowlTrigger != null
                ? Mathf.Min(bowlTrigger.size.x, bowlTrigger.size.z)
                : 0.2f;
            float widthMultiplier = Mathf.Clamp(
                0.44f + (amount - 1) * 0.04f,
                0.44f,
                0.62f);
            float heightMultiplier = Mathf.Clamp(
                0.15f + (amount - 1) * 0.025f,
                0.15f,
                0.25f);

            sugarVisual.localScale = new Vector3(
                bowlDiameter * widthMultiplier,
                bowlDiameter * heightMultiplier,
                bowlDiameter * widthMultiplier);
            sugarVisual.gameObject.SetActive(true);
        }

        if (IsIngredient(ingredientName, "Butter") &&
            butterVisual != null)
        {
            butterVisual.gameObject.SetActive(true);
        }

        if (IsIngredient(ingredientName, "ChocolateChips") &&
            chocolateChipVisual != null)
        {
            int amount = GetIngredientAmount("ChocolateChips");
            float fillProgress = Mathf.Clamp01(
                (float)amount / requiredChocolateChipScoops);
            float bowlDiameter = bowlTrigger != null
                ? Mathf.Min(bowlTrigger.size.x, bowlTrigger.size.z)
                : 0.2f;
            chocolateChipVisual.localScale = new Vector3(
                bowlDiameter * Mathf.Lerp(0.36f, 0.62f, fillProgress),
                bowlDiameter * Mathf.Lerp(0.13f, 0.24f, fillProgress),
                bowlDiameter * Mathf.Lerp(0.36f, 0.62f, fillProgress));
            chocolateChipVisual.gameObject.SetActive(true);
        }

        if (IsIngredient(ingredientName, "Egg"))
        {
            if (eggWhiteVisual != null)
                eggWhiteVisual.gameObject.SetActive(true);

            if (eggYolkVisual != null)
                eggYolkVisual.gameObject.SetActive(true);
        }

        if (IsIngredient(ingredientName, "Vanilla") && vanillaVisual != null)
            vanillaVisual.gameObject.SetActive(true);

        if (IsIngredient(ingredientName, "BakingSoda") && bakingSodaVisual != null)
            bakingSodaVisual.gameObject.SetActive(true);

        if (IsIngredient(ingredientName, "Salt") && saltVisual != null)
            saltVisual.gameObject.SetActive(true);
    }

    private void ConsumeWholeIngredient(Ingredient ingredient)
    {
        Rigidbody ingredientBody =
            ingredient.GetComponentInParent<Rigidbody>();

        if (ingredientBody != null)
        {
            PickupController pickupController =
                FindAnyObjectByType<PickupController>();

            if (pickupController != null)
                pickupController.ReleaseForPlacement(ingredientBody);

            if (!ingredientBody.isKinematic)
            {
                ingredientBody.linearVelocity = Vector3.zero;
                ingredientBody.angularVelocity = Vector3.zero;
            }

            ingredientBody.isKinematic = true;
        }

        ingredient.gameObject.SetActive(false);
    }

    private bool IsIngredient(string value, string expected)
    {
        return string.Equals(
            value?.Trim(),
            expected,
            System.StringComparison.OrdinalIgnoreCase);
    }

    private void OnValidate()
    {
        requiredFlourScoops = Mathf.Max(1, requiredFlourScoops);
        requiredSugarScoops = Mathf.Max(1, requiredSugarScoops);
        requiredButterPortions = Mathf.Max(1, requiredButterPortions);
        requiredEggs = Mathf.Max(1, requiredEggs);
        requiredChocolateChipScoops = Mathf.Max(1, requiredChocolateChipScoops);
        requiredChipMixPasses = Mathf.Max(0, requiredChipMixPasses);
    }
}
