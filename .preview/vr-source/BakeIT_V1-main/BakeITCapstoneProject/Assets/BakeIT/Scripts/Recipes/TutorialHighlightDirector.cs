using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class TutorialHighlightDirector : MonoBehaviour
{
    [Header("Scene references")]
    [SerializeField] private CookieRecipeSessionController session;
    [SerializeField] private PickupController pickup;
    [SerializeField] private MiseEnPlaceStation miseEnPlace;
    [SerializeField] private PourableIngredientReceiver brownieBowl;
    [SerializeField] private BrowniePanReceiver browniePan;
    [SerializeField] private BrownieSpreadingTool brownieSpatula;
    [SerializeField] private BowlReceiver cookieBowl;
    [SerializeField] private DoughBoardController doughBoard;
    [SerializeField] private TrayLinerReceiver trayLiner;
    [SerializeField] private TrayReceiver tray;
    [SerializeField] private ParchmentDispenser parchmentDispenser;
    [SerializeField] private OvenController oven;
    [SerializeField] private OvenBakeZone ovenZone;
    [SerializeField] private Transform fridge;
    [SerializeField] private Transform dryStorage;
    [SerializeField] private Transform recipePosters;

    [Header("Correction timing")]
    [SerializeField, Min(1f)] private float misplacedDelaySeconds = 3.5f;

    private readonly HashSet<Transform> introducedAreas = new();
    private GuidanceBeacon yellowBeacon;
    private GuidanceBeacon redBeacon;
    private Transform previousNormalTarget;
    private Rigidbody previousHeldBody;
    private PreparationItem pendingWrongItem;
    private StablePlacementSurface pendingWrongSurface;
    private float pendingWrongAt;
    private bool wrongActive;

    public Transform CurrentYellowTarget { get; private set; }
    public Transform CurrentRedTarget { get; private set; }
    public bool IsCorrectionActive => wrongActive;

    private void Awake()
    {
        RemoveOrphanedBeacons();
        ResolveReferences();
        EnsureBeacons();
    }

    private static void RemoveOrphanedBeacons()
    {
        foreach (GameObject candidate in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (candidate == null ||
                (candidate.name != "Tutorial Yellow Guidance" &&
                 candidate.name != "Tutorial Red Correction"))
                continue;

            DestroyImmediate(candidate);
        }
    }

    private void OnDestroy()
    {
        yellowBeacon?.Dispose();
        redBeacon?.Dispose();
    }

    private void Update()
    {
        EvaluateNow();
    }

    public void EvaluateNow()
    {
        EnsureBeacons();
        ResolveReferences();
        TrackReleasedItem();
        UpdateWrongPlacement();

        Transform yellowTarget;
        Transform redTarget = null;
        if (wrongActive && pendingWrongItem != null && pendingWrongSurface != null)
        {
            redTarget = pendingWrongItem.transform;
            yellowTarget = pendingWrongSurface.transform;
        }
        else
        {
            yellowTarget = GetNormalTarget();
        }

        if (IsHeldTarget(yellowTarget))
            yellowTarget = null;

        CurrentYellowTarget = yellowTarget;
        CurrentRedTarget = redTarget;
        Camera camera = pickup != null ? pickup.GetComponentInChildren<Camera>() : Camera.main;
        yellowBeacon.Update(yellowTarget, camera, Time.unscaledTime);
        redBeacon.Update(redTarget, camera, Time.unscaledTime + .4f);
    }

    private bool IsHeldTarget(Transform target)
    {
        Rigidbody held = pickup != null ? pickup.HeldObject : null;
        return target != null && held != null &&
            (target == held.transform || target.IsChildOf(held.transform) ||
             held.transform.IsChildOf(target));
    }

    private void EnsureBeacons()
    {
        yellowBeacon ??= new GuidanceBeacon("Tutorial Yellow Guidance", new Color(1f, .88f, .02f));
        redBeacon ??= new GuidanceBeacon("Tutorial Red Correction", new Color(1f, .08f, .04f));
    }

    private void ResolveReferences()
    {
        if (session == null) session = FindAnyObjectByType<CookieRecipeSessionController>();
        if (pickup == null) pickup = FindAnyObjectByType<PickupController>();
        if (miseEnPlace == null) miseEnPlace = FindAnyObjectByType<MiseEnPlaceStation>();
        if (brownieBowl == null) brownieBowl = PourableIngredientReceiver.FindBrownieBowl();
        if (browniePan == null) browniePan = FindAnyObjectByType<BrowniePanReceiver>();
        if (brownieSpatula == null) brownieSpatula = FindAnyObjectByType<BrownieSpreadingTool>();
        if (cookieBowl == null) cookieBowl = FindAnyObjectByType<BowlReceiver>();
        if (doughBoard == null) doughBoard = FindAnyObjectByType<DoughBoardController>();
        if (trayLiner == null) trayLiner = FindAnyObjectByType<TrayLinerReceiver>();
        if (tray == null) tray = FindAnyObjectByType<TrayReceiver>();
        if (parchmentDispenser == null) parchmentDispenser = FindAnyObjectByType<ParchmentDispenser>();
        if (oven == null) oven = FindAnyObjectByType<OvenController>();
        if (ovenZone == null) ovenZone = FindAnyObjectByType<OvenBakeZone>();
    }

    private void TrackReleasedItem()
    {
        Rigidbody held = pickup != null ? pickup.HeldObject : null;
        if (previousHeldBody != null && held == null && miseEnPlace != null &&
            session != null && !session.IsActiveMiseEnPlaceComplete)
        {
            PreparationItem item = previousHeldBody.GetComponent<PreparationItem>();
            if (item != null && miseEnPlace.TryGetExpectedSurface(
                    item, session.ActiveRecipe, out StablePlacementSurface expected) &&
                item.RestingSurface != expected)
            {
                pendingWrongItem = item;
                pendingWrongSurface = expected;
                pendingWrongAt = Time.unscaledTime + misplacedDelaySeconds;
                wrongActive = false;
            }
        }

        if (held != null && pendingWrongItem != null && held.gameObject == pendingWrongItem.gameObject)
        {
            pendingWrongItem = null;
            pendingWrongSurface = null;
            wrongActive = false;
        }

        previousHeldBody = held;
    }

    private void UpdateWrongPlacement()
    {
        if (brownieBowl != null && brownieBowl.HasTransferredBatter && pendingWrongItem != null &&
            pendingWrongItem.transform == miseEnPlace.BrownieMixingBowl)
        { pendingWrongItem = null; pendingWrongSurface = null; wrongActive = false; }
        if (pendingWrongItem == null || pendingWrongSurface == null)
            return;

        if (pendingWrongItem.RestingSurface == pendingWrongSurface)
        {
            pendingWrongItem = null;
            pendingWrongSurface = null;
            wrongActive = false;
            return;
        }

        wrongActive = Time.unscaledTime >= pendingWrongAt;
    }

    private Transform GetNormalTarget()
    {
        if (session == null || miseEnPlace == null)
            return null;

        if (session.IsCupcakeRecipeActive)
        {
            var sequence = FindAnyObjectByType<MixingSequence>();
            if (!session.IsGuidedPractice || !sequence) return null;
            if (sequence.CupcakeBatch && (!sequence.EquipmentReady || sequence.IsComplete)) return sequence.CupcakeBatch.GetGuidanceTarget();
            if (sequence.Current == null) return null;
            foreach (var receiver in FindObjectsByType<PourableIngredientReceiver>())
                if (receiver.StagedProcess == sequence && receiver.BowlRole == sequence.Current.BowlRole)
                    return receiver.GetComponentInParent<Rigidbody>().transform;
            return null;
        }

        if (session.CurrentStage == "SERVING COMPLETE") return null;
        if (session.CurrentStage == "PLATE THE FINISHED FOOD")
        {
            var plate = FindAnyObjectByType<ServingPlateReceiver>();
            if (pickup != null && pickup.HeldObject != null && pickup.HeldObject.GetComponent<ServingPortion>())
                return plate != null ? plate.transform : null;
            foreach (var portion in FindObjectsByType<ServingPortion>())
                if (portion.IsBaked && portion.RecipeId == session.SelectedRecipeId && portion.GetComponentInParent<ServingPlateReceiver>() == null &&
                    (plate == null || !portion.transform.IsChildOf(plate.transform.parent))) return portion.transform;
            return plate != null ? plate.transform : null;
        }

        Rigidbody held = pickup != null ? pickup.HeldObject : null;
        PreparationItem heldItem = held != null ? held.GetComponent<PreparationItem>() : null;
        if (heldItem != null && !session.IsActiveMiseEnPlaceComplete &&
            miseEnPlace.TryGetExpectedSurface(
                heldItem, session.ActiveRecipe, out StablePlacementSurface expected))
            return UseNormalTarget(expected.transform, true);

        Transform target = null;
        bool area = false;
        if (!session.IsTutorialComplete)
        {
            if (!session.WhiskPickupPracticed ||
                (!session.RotationPracticed || !session.DistancePracticed) && held != null)
                target = miseEnPlace.Whisk;
            else if (!miseEnPlace.WhiskOnUtensilsArea)
            {
                target = miseEnPlace.ToolRest != null ? miseEnPlace.ToolRest.transform : null;
                area = true;
            }
        }
        else if (!session.HasSelectedRecipe)
        {
            target = recipePosters;
            area = true;
        }
        else if (session.IsBrownieRecipeActive)
        {
            target = GetBrownieTarget(held, out area);
        }
        else if (!miseEnPlace.ColdIngredientsPrepared)
        {
            if (!session.FridgeOpened)
            {
                target = fridge;
                area = true;
            }
            else target = FirstIncomplete(
                miseEnPlace.EggPrepared, miseEnPlace.Egg,
                miseEnPlace.ButterPrepared, miseEnPlace.Butter,
                miseEnPlace.ChipsPrepared, miseEnPlace.ChocolateChips);
        }
        else if (!miseEnPlace.DryIngredientsPrepared)
        {
            if (!session.CabinetOpened)
            {
                target = dryStorage;
                area = true;
            }
            else target = FirstIncomplete(
                miseEnPlace.FlourPrepared, miseEnPlace.Flour,
                miseEnPlace.SugarPrepared, miseEnPlace.Sugar,
                miseEnPlace.VanillaPrepared, miseEnPlace.Vanilla,
                miseEnPlace.BakingSodaPrepared, miseEnPlace.BakingSoda,
                miseEnPlace.SaltPrepared, miseEnPlace.Salt);
        }
        else if (!miseEnPlace.ToolsPrepared)
        {
            target = FirstIncomplete(
                miseEnPlace.LargeSpoonPrepared, miseEnPlace.OneCup,
                miseEnPlace.HalfCupPrepared, miseEnPlace.HalfCup,
                miseEnPlace.OneTeaspoonPrepared, miseEnPlace.OneTeaspoon,
                miseEnPlace.HalfTeaspoonPrepared, miseEnPlace.HalfTeaspoon,
                miseEnPlace.QuarterTeaspoonPrepared, miseEnPlace.QuarterTeaspoon,
                miseEnPlace.WhiskPrepared, miseEnPlace.Whisk);
        }
        else if (!miseEnPlace.BowlPlaced)
        {
            target = miseEnPlace.WhiteCookieBowl;
        }
        else
        {
            target = GetCookieTarget(held, out area);
        }

        return UseNormalTarget(target, area);
    }

    private Transform GetCookieTarget(Rigidbody held, out bool area)
    {
        area = false;
        if (cookieBowl == null || doughBoard == null || tray == null)
            return null;

        if (!cookieBowl.IsBowlReady)
        {
            area = true;
            return miseEnPlace.BowlRest != null
                ? miseEnPlace.BowlRest.transform
                : cookieBowl.transform;
        }

        if (!cookieBowl.HasAllBaseIngredients)
            return GetCookieIngredientTarget(
                session.ActiveRecipe.GetIngredients(RecipeIngredientStage.Base), held);

        if (!cookieBowl.IsBaseDoughMixed)
            return IsHoldingComponent<MixingTool>(held)
                ? cookieBowl.transform
                : miseEnPlace.Whisk;

        if (!cookieBowl.HasRequiredChocolateChips)
            return GetCookieIngredientTarget(
                session.ActiveRecipe.GetIngredients(RecipeIngredientStage.Finishing), held);

        if (!cookieBowl.IsRecipeComplete)
            return IsHoldingComponent<MixingTool>(held)
                ? cookieBowl.transform
                : miseEnPlace.Whisk;

        if (doughBoard.LastPortionCount == 0 && !doughBoard.HasDough)
            return cookieBowl.CompletedDough != null
                ? cookieBowl.CompletedDough
                : doughBoard.transform;

        if (doughBoard.LastPortionCount == 0 && !doughBoard.IsDoughFlattened)
        {
            RollingPinTool rollingPin = FindAnyObjectByType<RollingPinTool>();
            return IsHoldingComponent<RollingPinTool>(held)
                ? doughBoard.transform
                : rollingPin != null ? rollingPin.transform : doughBoard.transform;
        }

        if (doughBoard.LastPortionCount == 0)
        {
            DoughPortioningTool knife = FindAnyObjectByType<DoughPortioningTool>();
            return IsHoldingComponent<DoughPortioningTool>(held)
                ? doughBoard.transform
                : knife != null ? knife.transform : doughBoard.transform;
        }

        if (!tray.HasCompleteCookieBatch)
            return GetTrayPreparationTarget(held, out area);

        if (oven == null)
            return null;

        if (!oven.IsPreheated)
        {
            area = true;
            return oven.transform;
        }

        if (ovenZone == null || !ovenZone.HasLoadedTray)
        {
            Transform trayRoot = GetTrayRoot();
            if (held != null && trayRoot != null && held.transform == trayRoot)
            {
                area = true;
                return ovenZone != null ? ovenZone.transform : oven.transform;
            }
            return trayRoot;
        }

        area = true;
        return oven.transform;
    }

    private Transform GetCookieIngredientTarget(
        IEnumerable<RecipeIngredientRequirement> requirements,
        Rigidbody held)
    {
        foreach (RecipeIngredientRequirement requirement in requirements)
        {
            if (requirement == null || cookieBowl.HasIngredientRequirement(requirement))
                continue;

            Transform source = miseEnPlace.GetBrownieIngredientSource(requirement.IngredientId);
            if (requirement.RequiredTool == RecipeMeasureTool.WholeItem)
            {
                return MatchesHeld(held, source)
                    ? cookieBowl.transform
                    : source;
            }

            MeasuringScoop heldScoop = held != null
                ? held.GetComponent<MeasuringScoop>()
                : null;
            if (heldScoop != null && heldScoop.MeasureTool == requirement.RequiredTool)
                return heldScoop.IsFilled ? cookieBowl.transform : source;

            return miseEnPlace.GetMeasuringTool(requirement.RequiredTool);
        }
        return null;
    }

    private Transform GetTrayPreparationTarget(Rigidbody held, out bool area)
    {
        area = false;
        Transform trayRoot = GetTrayRoot();
        if (trayRoot == null)
            return null;

        if (trayLiner == null || !trayLiner.HasParchment)
        {
            if (MatchesHeld(held, trayRoot))
            {
                StablePlacementSurface worktop = FindTrayWorktopSurface();
                area = worktop != null;
                return worktop != null ? worktop.transform : trayRoot;
            }

            if (IsHoldingParchment(held))
            {
                area = true;
                return trayLiner != null ? trayLiner.transform : trayRoot;
            }

            PreparationItem trayItem = trayRoot.GetComponent<PreparationItem>();
            if (trayItem == null || trayItem.RestingSurface == null)
                return trayRoot;

            if (parchmentDispenser != null && !parchmentDispenser.HasDispensed)
                return parchmentDispenser.transform;

            Transform looseParchment = FindLooseParchment();
            return looseParchment != null ? looseParchment : trayRoot;
        }

        DoughPortion[] portions = FindObjectsByType<DoughPortion>();
        Ingredient[] placed = tray.PlacedIngredients;
        foreach (DoughPortion portion in portions)
        {
            if (portion == null || !portion.gameObject.activeInHierarchy || IsPlaced(portion, placed))
                continue;

            if (MatchesHeld(held, portion.transform))
            {
                area = true;
                return trayLiner.transform;
            }
            return portion.transform;
        }

        area = true;
        return trayLiner.transform;
    }

    private Transform GetTrayRoot()
    {
        if (tray == null)
            return null;
        Rigidbody body = tray.GetComponentInParent<Rigidbody>();
        return body != null ? body.transform : tray.transform;
    }

    private static bool IsHoldingComponent<T>(Rigidbody held) where T : Component =>
        held != null && held.GetComponentInChildren<T>(true) != null;

    private static bool MatchesHeld(Rigidbody held, Transform target) =>
        held != null && target != null &&
        (held.transform == target || held.transform.IsChildOf(target) ||
         target.IsChildOf(held.transform));

    private static bool IsHoldingParchment(Rigidbody held)
    {
        Ingredient ingredient = held != null ? held.GetComponent<Ingredient>() : null;
        return ingredient != null && string.Equals(
            ingredient.ingredientName,
            "ParchmentPaper",
            System.StringComparison.OrdinalIgnoreCase);
    }

    private static Transform FindLooseParchment()
    {
        foreach (Ingredient ingredient in FindObjectsByType<Ingredient>())
            if (ingredient != null && ingredient.gameObject.activeInHierarchy &&
                string.Equals(ingredient.ingredientName, "ParchmentPaper",
                    System.StringComparison.OrdinalIgnoreCase))
                return ingredient.transform;
        return null;
    }

    private static bool IsPlaced(DoughPortion portion, Ingredient[] placed)
    {
        Ingredient ingredient = portion.GetComponent<Ingredient>();
        if (ingredient == null || placed == null)
            return false;
        foreach (Ingredient candidate in placed)
            if (candidate == ingredient)
                return true;
        return false;
    }

    private static StablePlacementSurface FindTrayWorktopSurface()
    {
        foreach (StablePlacementSurface surface in
                 FindObjectsByType<StablePlacementSurface>())
            if (surface != null && surface.name.IndexOf(
                    "tray", System.StringComparison.OrdinalIgnoreCase) >= 0)
                return surface;
        return null;
    }

    private Transform GetBrownieTarget(Rigidbody held, out bool area)
    {
        area = false;
        if (miseEnPlace == null)
            return null;

        if (brownieBowl != null && brownieBowl.HasTransferredBatter)
            return GetBrowniePanTarget(held, out area);

        if (!miseEnPlace.BrownieColdIngredientsPrepared)
        {
            if (session != null && !session.FridgeOpened)
            {
                area = true;
                return fridge;
            }
            return miseEnPlace.GetFirstUnpreparedBrownieEgg();
        }

        if (!miseEnPlace.BrownieDryIngredientsPrepared)
        {
            if (session != null && !session.CabinetOpened)
            {
                area = true;
                return dryStorage;
            }
            return FirstIncomplete(
                IsOnSurface(miseEnPlace.Flour, miseEnPlace.IngredientRest), miseEnPlace.Flour,
                IsOnSurface(miseEnPlace.Sugar, miseEnPlace.IngredientRest), miseEnPlace.Sugar,
                IsOnSurface(miseEnPlace.MeltedButterCup, miseEnPlace.IngredientRest), miseEnPlace.MeltedButterCup,
                IsOnSurface(miseEnPlace.Cocoa, miseEnPlace.IngredientRest), miseEnPlace.Cocoa,
                IsOnSurface(miseEnPlace.Vanilla, miseEnPlace.IngredientRest), miseEnPlace.Vanilla,
                IsOnSurface(miseEnPlace.BakingPowder, miseEnPlace.IngredientRest), miseEnPlace.BakingPowder,
                IsOnSurface(miseEnPlace.Salt, miseEnPlace.IngredientRest), miseEnPlace.Salt,
                IsOnSurface(miseEnPlace.Walnuts, miseEnPlace.IngredientRest), miseEnPlace.Walnuts);
        }

        if (!miseEnPlace.BrownieToolsPrepared)
        {
            return FirstIncomplete(
                IsOnSurface(miseEnPlace.OneCup, miseEnPlace.ToolRest), miseEnPlace.OneCup,
                IsOnSurface(miseEnPlace.HalfCup, miseEnPlace.ToolRest), miseEnPlace.HalfCup,
                IsOnSurface(miseEnPlace.GetMeasuringTool(RecipeMeasureTool.QuarterCup), miseEnPlace.ToolRest), miseEnPlace.GetMeasuringTool(RecipeMeasureTool.QuarterCup),
                IsOnSurface(miseEnPlace.OneTeaspoon, miseEnPlace.ToolRest), miseEnPlace.OneTeaspoon,
                IsOnSurface(miseEnPlace.HalfTeaspoon, miseEnPlace.ToolRest), miseEnPlace.HalfTeaspoon,
                IsOnSurface(miseEnPlace.Whisk, miseEnPlace.ToolRest), miseEnPlace.Whisk);
        }

        if (!miseEnPlace.BrownieBowlPlaced)
            return miseEnPlace.BrownieMixingBowl;

        if (brownieBowl == null || session == null || session.ActiveRecipe == null)
            return null;

        if (!brownieBowl.IsBatterComplete && !brownieBowl.IsBowlReady)
        {
            if (held != null && held.transform == miseEnPlace.BrownieMixingBowl)
            {
                area = true;
                return miseEnPlace.BowlRest != null
                    ? miseEnPlace.BowlRest.transform
                    : null;
            }
            return miseEnPlace.BrownieMixingBowl;
        }

        if (!brownieBowl.HasAllBaseIngredients)
            return GetBrownieIngredientTarget(
                session.ActiveRecipe.GetIngredients(RecipeIngredientStage.Base), held);

        if (!brownieBowl.IsBaseMixed)
            return held != null && held.GetComponentInChildren<MixingTool>(true) != null
                ? brownieBowl.transform
                : miseEnPlace.Whisk;

        if (!brownieBowl.HasAllFinishingIngredients)
            return GetBrownieIngredientTarget(
                session.ActiveRecipe.GetIngredients(RecipeIngredientStage.Finishing), held);

        if (!brownieBowl.IsBatterComplete)
            return held != null && held.GetComponentInChildren<MixingTool>(true) != null
                ? brownieBowl.transform
                : miseEnPlace.Whisk;

        return GetBrowniePanTarget(held, out area);
    }

    private Transform GetBrowniePanTarget(Rigidbody held, out bool area)
    {
        area = false;
        if (browniePan == null)
            return null;

        Transform panRoot = GetBrowniePanRoot();
        if (!browniePan.HasParchment)
        {
            if (IsHoldingParchment(held))
                return browniePan.transform;

            PreparationItem panItem = panRoot != null
                ? panRoot.GetComponent<PreparationItem>()
                : null;
            if (panItem == null || panItem.RestingSurface == null)
                return panRoot;

            if (parchmentDispenser != null && !parchmentDispenser.HasDispensed)
                return parchmentDispenser.transform;

            Transform looseParchment = FindLooseParchment();
            return looseParchment != null ? looseParchment : panRoot;
        }

        if (!browniePan.HasBatter)
            return MatchesHeld(held, brownieBowl.transform)
                ? browniePan.transform
                : brownieBowl.transform;

        if (!browniePan.IsSpread)
            return IsHoldingComponent<BrownieSpreadingTool>(held)
                ? browniePan.transform
                : brownieSpatula != null ? brownieSpatula.transform : browniePan.transform;

        if (browniePan.BakeCompleted)
        {
            if(browniePan.IsSliced)return null;
            if(ovenZone && ovenZone.HasLoadedBrowniePan)return panRoot;
            if(IsHoldingComponent<DoughPortioningTool>(held))return browniePan.transform;
            var knife=FindAnyObjectByType<DoughPortioningTool>();
            return knife ? knife.transform : panRoot;
        }

        if (oven == null)
            return null;

        if (!oven.IsPreheated)
        {
            area = true;
            return oven.transform;
        }

        if (ovenZone == null || !ovenZone.HasLoadedBrowniePan)
        {
            if (MatchesHeld(held, panRoot))
            {
                area = true;
                return ovenZone != null ? ovenZone.transform : oven.transform;
            }
            return panRoot;
        }

        area = true;
        return oven.transform;
    }

    private Transform GetBrowniePanRoot()
    {
        if (browniePan == null)
            return null;
        Rigidbody body = browniePan.GetComponentInParent<Rigidbody>();
        return body != null ? body.transform : browniePan.transform;
    }

    private Transform GetBrownieIngredientTarget(
        IEnumerable<RecipeIngredientRequirement> requirements,
        Rigidbody held)
    {
        foreach (RecipeIngredientRequirement requirement in requirements)
        {
            if (requirement == null || brownieBowl.HasIngredientRequirement(requirement))
                continue;

            Transform source = miseEnPlace.GetBrownieIngredientSource(requirement.IngredientId);
            if (requirement.RequiredTool == RecipeMeasureTool.WholeItem)
            {
                if (held != null && source != null &&
                    (held.transform == source || held.transform.IsChildOf(source) ||
                     source.IsChildOf(held.transform)))
                {
                    return brownieBowl.transform;
                }
                return source;
            }

            MeasuringScoop heldScoop = held != null
                ? held.GetComponent<MeasuringScoop>()
                : null;
            if (heldScoop != null && heldScoop.MeasureTool == requirement.RequiredTool)
                return heldScoop.IsFilled ? brownieBowl.transform : source;

            return miseEnPlace.GetMeasuringTool(requirement.RequiredTool);
        }
        return null;
    }

    private static bool IsOnSurface(
        Transform target,
        StablePlacementSurface surface)
    {
        PreparationItem item = target != null
            ? target.GetComponent<PreparationItem>()
            : null;
        return item != null && item.RestingSurface == surface;
    }

    private Transform UseNormalTarget(Transform target, bool area)
    {
        if (target != previousNormalTarget)
        {
            if (previousNormalTarget != null && IsIntroductoryArea(previousNormalTarget))
                introducedAreas.Add(previousNormalTarget);
            previousNormalTarget = target;
        }

        return area && target != null && introducedAreas.Contains(target)
            ? null
            : target;
    }

    private bool IsIntroductoryArea(Transform target)
    {
        return target == fridge || target == dryStorage || target == recipePosters ||
               (miseEnPlace != null && (target == miseEnPlace.ToolRest?.transform ||
                                        target == miseEnPlace.IngredientRest?.transform ||
                                        target == miseEnPlace.BowlRest?.transform));
    }

    private static Transform FirstIncomplete(params object[] pairs)
    {
        for (int i = 0; i + 1 < pairs.Length; i += 2)
            if (pairs[i] is bool complete && !complete)
                return pairs[i + 1] as Transform;
        return null;
    }

    private sealed class GuidanceBeacon
    {
        private readonly GameObject root;
        private readonly Material outlineMaterial;
        private readonly Material surfaceMaterial;
        private readonly List<OutlinePart> parts = new();
        private Transform currentTarget;
        private LineRenderer surfaceBorder;
        private BoxCollider surfaceSource;

        public GuidanceBeacon(string name, Color beaconColor)
        {
            root = new GameObject(name) { hideFlags = HideFlags.DontSave };
            Shader shader = Shader.Find("BakeIT/Tutorial Silhouette Outline");
            outlineMaterial = shader != null ? new Material(shader) : null;
            if (outlineMaterial != null)
            {
                outlineMaterial.hideFlags = HideFlags.DontSave;
                outlineMaterial.SetColor("_OutlineColor", beaconColor);
            }
            Shader surfaceShader = Shader.Find("Universal Render Pipeline/Unlit");
            surfaceMaterial = surfaceShader != null ? new Material(surfaceShader) : null;
            if (surfaceMaterial != null)
            {
                surfaceMaterial.hideFlags = HideFlags.DontSave;
                surfaceMaterial.SetColor("_BaseColor", beaconColor);
                surfaceMaterial.SetColor("_Color", beaconColor);
            }
            root.SetActive(false);
        }

        public void Update(Transform target, Camera camera, float time)
        {
            if (target != currentTarget)
                Rebuild(target);

            if (target == null || parts.Count == 0 && surfaceBorder == null)
            {
                root.SetActive(false);
                return;
            }

            root.SetActive(true);
            if (outlineMaterial != null)
                outlineMaterial.SetFloat(
                    "_OutlinePixels", 9.75f + Mathf.Sin(time * 2.4f) * 1.25f);

            if (surfaceBorder != null && surfaceSource != null)
            {
                surfaceBorder.widthMultiplier =
                    .018f + Mathf.Sin(time * 2.4f) * .003f;
                surfaceBorder.transform.SetPositionAndRotation(
                    surfaceSource.transform.position,
                    surfaceSource.transform.rotation);
                surfaceBorder.transform.localScale =
                    surfaceSource.transform.lossyScale;
            }

            foreach (OutlinePart part in parts)
            {
                if (part.Source == null || part.Shell == null)
                    continue;
                part.Shell.gameObject.SetActive(
                    part.Source.enabled && part.Source.gameObject.activeInHierarchy);
                part.Shell.SetPositionAndRotation(
                    part.Source.transform.position,
                    part.Source.transform.rotation);
                part.Shell.localScale = part.Source.transform.lossyScale;
            }
        }

        private void Rebuild(Transform target)
        {
            foreach (OutlinePart part in parts)
                if (part.Shell != null) Object.DestroyImmediate(part.Shell.gameObject);
            parts.Clear();
            if (surfaceBorder != null)
                Object.DestroyImmediate(surfaceBorder.gameObject);
            surfaceBorder = null;
            surfaceSource = null;
            currentTarget = target;

            if (target == null || outlineMaterial == null)
                return;

            StablePlacementSurface placementSurface =
                target.GetComponent<StablePlacementSurface>();
            BoxCollider placementBounds = placementSurface != null
                ? placementSurface.GetComponent<BoxCollider>()
                : null;
            if (placementBounds != null && surfaceMaterial != null)
            {
                CreateSurfaceBorder(placementBounds);
                return;
            }

            foreach (MeshRenderer source in
                     target.GetComponentsInChildren<MeshRenderer>(false))
            {
                if (source == null || source.GetComponent<TextMesh>() != null)
                    continue;
                MeshFilter sourceFilter = source.GetComponent<MeshFilter>();
                if (sourceFilter == null || sourceFilter.sharedMesh == null)
                    continue;

                GameObject shellObject = new GameObject(
                    $"{source.gameObject.name} Tutorial Contour")
                {
                    hideFlags = HideFlags.DontSave
                };
                shellObject.transform.SetParent(root.transform, false);
                MeshFilter shellFilter = shellObject.AddComponent<MeshFilter>();
                shellFilter.sharedMesh = sourceFilter.sharedMesh;
                MeshRenderer shellRenderer = shellObject.AddComponent<MeshRenderer>();
                int materialCount = Mathf.Max(1, source.sharedMaterials.Length);
                Material[] materials = new Material[materialCount];
                for (int i = 0; i < materials.Length; i++)
                    materials[i] = outlineMaterial;
                shellRenderer.sharedMaterials = materials;
                shellRenderer.shadowCastingMode =
                    UnityEngine.Rendering.ShadowCastingMode.Off;
                shellRenderer.receiveShadows = false;
                parts.Add(new OutlinePart(source, shellObject.transform));
            }
        }

        private void CreateSurfaceBorder(BoxCollider source)
        {
            GameObject borderObject = new GameObject(
                $"{source.gameObject.name} Tutorial Perimeter")
            {
                hideFlags = HideFlags.DontSave
            };
            borderObject.transform.SetParent(root.transform, false);
            surfaceBorder = borderObject.AddComponent<LineRenderer>();
            surfaceSource = source;
            surfaceBorder.sharedMaterial = surfaceMaterial;
            surfaceBorder.useWorldSpace = false;
            surfaceBorder.loop = true;
            surfaceBorder.positionCount = 4;
            surfaceBorder.alignment = LineAlignment.View;
            surfaceBorder.textureMode = LineTextureMode.Stretch;
            surfaceBorder.numCornerVertices = 4;
            surfaceBorder.numCapVertices = 4;
            surfaceBorder.shadowCastingMode =
                UnityEngine.Rendering.ShadowCastingMode.Off;
            surfaceBorder.receiveShadows = false;

            Vector3 center = source.center;
            Vector3 extent = source.size * .5f;
            float top = center.y + extent.y + .004f;
            surfaceBorder.SetPositions(new[]
            {
                new Vector3(center.x - extent.x, top, center.z - extent.z),
                new Vector3(center.x - extent.x, top, center.z + extent.z),
                new Vector3(center.x + extent.x, top, center.z + extent.z),
                new Vector3(center.x + extent.x, top, center.z - extent.z)
            });
        }

        public void Dispose()
        {
            if (root != null)
                Object.DestroyImmediate(root);
            if (outlineMaterial != null)
                Object.DestroyImmediate(outlineMaterial);
            if (surfaceMaterial != null)
                Object.DestroyImmediate(surfaceMaterial);
        }

        private sealed class OutlinePart
        {
            public OutlinePart(MeshRenderer source, Transform shell)
            {
                Source = source;
                Shell = shell;
            }

            public MeshRenderer Source { get; }
            public Transform Shell { get; }
        }
    }
}
