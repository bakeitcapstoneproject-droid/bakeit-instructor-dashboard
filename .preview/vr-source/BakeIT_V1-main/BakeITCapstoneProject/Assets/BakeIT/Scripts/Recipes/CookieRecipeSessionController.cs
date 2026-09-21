using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public sealed class CookieRecipeSessionController : MonoBehaviour
{
    private const float CorrectionDisplaySeconds = 4.5f;
    private const float GuideUiScale = 1.5f;
    private const float RestartHoldSeconds = 3f;
    [SerializeField] private bool guidedPractice = true;
    private float selectionHoldTime;
    private bool cookieBakeFinished;
    private ServingPlateReceiver servingPlate;
    private MixingSequence cupcakeSequence;
    public bool IsCupcakeRecipeActive => activeRecipe != null && activeRecipe.RecipeId == "cupcakes";
    public bool IsGuidedPractice => guidedPractice;
    public void SetGuidedPractice(bool value) => guidedPractice = value;

    public bool IsIngredientSatisfied(string ingredientId)
    {
        if (!HasSelectedRecipe || activeRecipe == null) return false;
        bool found = false;
        foreach (var requirement in activeRecipe.GetIngredients(ingredientId))
        {
            found = true;
            if (IsCupcakeRecipeActive)
            {
                if (!cupcakeSequence || cupcakeSequence.Total(ingredientId, requirement.RequiredTool) != requirement.RequiredQuantity) return false;
                continue;
            }
            if (IsBrownieRecipeActive ? brownieBowl == null || !brownieBowl.HasIngredientRequirement(requirement)
                : bowl == null || !bowl.HasIngredientRequirement(requirement)) return false;
        }
        return found;
    }

    public void ReturnToRecipeSelection()
    {
        if (resetInProgress) return;
        selectedRecipeId = string.Empty;
        RestartRecipe();
    }

    private static CookieRecipeSessionController instance;

    private readonly List<GuideItem> guideItems = new();
    private readonly List<MeasureTransfer> measureTransfers = new();
    public IReadOnlyList<MeasureTransfer> MeasureTransfers => measureTransfers.AsReadOnly();
    public int DiscardedMeasureCount { get; private set; }

    private void RecordMeasureTransfer(MeasureTransfer transfer)
    {
        measureTransfers.Add(transfer);
        if(transfer.Disposition==MeasureDisposition.Discarded)DiscardedMeasureCount+=transfer.Quantity;
    }

    private BowlReceiver bowl;
    private PourableIngredientReceiver brownieBowl;
    private BrowniePanReceiver browniePan;
    private DoughBoardController board;
    private TrayLinerReceiver trayLiner;
    private TrayReceiver tray;
    private OvenBakeZone ovenZone;
    private OvenController oven;
    private OvenDoorController ovenDoor;
    private RecipeDefinition activeRecipe;
    private RecipeSelectionClipboard recipeClipboard;
    private PickupController pickupController;
    private MiseEnPlaceStation miseEnPlace;
    private string sectionHeading = "Choose a Recipe";
    private string stageLabel = "LOADING KITCHEN";
    private string selectedRecipeId = string.Empty;
    private string correctionMessage = string.Empty;
    private float correctionExpiresAt;
    private bool resetInProgress;
    private bool movementPracticed;
    private bool lookPracticed;
    private bool whiskPickupPracticed;
    private bool rotationPracticed;
    private bool distancePracticed;
    private bool fridgeOpened;
    private bool cabinetOpened;
    private FridgeDoorController[] fridgeDoors;
    private SlidingCabinetDoor[] cabinetDoors;
    private bool tutorialComplete;
    private bool recipeCompletionLatched;
    private float restartHoldTime;
    private OvenBakeResult completedBakeResult = OvenBakeResult.None;
    private GUIStyle titleStyle;
    private GUIStyle stageStyle;
    private GUIStyle pendingItemStyle;
    private GUIStyle completedItemStyle;
    private GUIStyle correctionStyle;
    private GUIStyle hintStyle;
    private GUIStyle buttonStyle;

    public string CurrentMessage => correctionMessage;
    public string CurrentSectionHeading => sectionHeading;
    public string CurrentStage => stageLabel;
    public bool IsResetInProgress => resetInProgress;
    public bool IsTutorialComplete => tutorialComplete;
    public bool IsRecipeCompletionLatched => recipeCompletionLatched;
    public bool MovementPracticed => movementPracticed;
    public bool LookPracticed => lookPracticed;
    public bool WhiskPickupPracticed => whiskPickupPracticed;
    public bool RotationPracticed => rotationPracticed;
    public bool DistancePracticed => distancePracticed;
    public bool FridgeOpened => fridgeOpened;
    public bool CabinetOpened => cabinetOpened;
    public float RestartHoldProgress => Mathf.Clamp01(restartHoldTime / RestartHoldSeconds);
    public int GuideItemCount => guideItems.Count;
    public RecipeDefinition ActiveRecipe => activeRecipe;
    public string SelectedRecipeId => selectedRecipeId;
    public bool IsBrownieRecipeActive => activeRecipe != null && string.Equals(
        activeRecipe.RecipeId, "brownies", System.StringComparison.OrdinalIgnoreCase);
    public bool IsActiveMiseEnPlaceComplete => IsCupcakeRecipeActive || miseEnPlace == null ||
        (IsBrownieRecipeActive ? miseEnPlace.BrownieIsPrepared : miseEnPlace.IsPrepared);
    public bool HasSelectedRecipe =>
        recipeClipboard == null || !string.IsNullOrEmpty(selectedRecipeId);

    public bool TrySelectRecipe(string recipeId, string displayName)
    {
        if (!string.IsNullOrEmpty(selectedRecipeId))
        {
            RecipeFeedback.Warning("Hold M for 3 seconds to leave this batch and choose a recipe.");
            return false;
        }
        if (!tutorialComplete)
        {
            RecipeFeedback.Warning(
                "Complete the Movement Tutorial before choosing a recipe.");
            return false;
        }

        if (recipeClipboard == null || activeRecipe == null)
            ResolveSceneReferences();

        if (recipeClipboard == null ||
            !recipeClipboard.TryGetRecipe(recipeId, out RecipeDefinition selectedRecipe))
        {
            RecipeFeedback.Warning(
                $"{displayName} does not have a playable recipe flow yet.");
            return false;
        }

        activeRecipe = selectedRecipe;
        selectedRecipeId = selectedRecipe.RecipeId;
        ApplyActiveRecipeToMeasuringTools();
        if (oven != null)
            oven.SetActiveRecipe(activeRecipe);
        correctionMessage = string.Empty;
        RecipeFeedback.Report($"Selected {activeRecipe.DisplayName}.");
        RefreshGuide();
        return true;
    }

    public string GetGuideItemText(int index)
    {
        return index >= 0 && index < guideItems.Count
            ? guideItems[index].Text
            : string.Empty;
    }

    public bool IsGuideItemComplete(int index)
    {
        return index >= 0 &&
               index < guideItems.Count &&
               guideItems[index].Complete;
    }

    private readonly struct GuideItem
    {
        public GuideItem(string text, bool complete)
        {
            Text = text;
            Complete = complete;
        }

        public string Text { get; }
        public bool Complete { get; }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureSessionController()
    {
        if (instance != null)
            return;

        CookieRecipeSessionController existing =
            FindAnyObjectByType<CookieRecipeSessionController>();

        if (existing != null)
        {
            instance = existing;
            return;
        }

        GameObject sessionObject = new GameObject("BakeIT Recipe Session");
        instance = sessionObject.AddComponent<CookieRecipeSessionController>();
        DontDestroyOnLoad(sessionObject);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        ResolveSceneReferences();
    }

    private void OnEnable()
    {
        RecipeFeedback.MessageChanged += HandleRecipeFeedback;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        MeasuringScoop.ContentsTransferred += RecordMeasureTransfer;
    }

    private void OnDisable()
    {
        RecipeFeedback.MessageChanged -= HandleRecipeFeedback;
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        MeasuringScoop.ContentsTransferred -= RecordMeasureTransfer;
    }

    private void Update()
    {
        TrackRestartHold();
        if (IsBrownieRecipeActive && browniePan != null && browniePan.IsSliced && servingPlate != null && Keyboard.current != null)
        {
            if (Keyboard.current.digit2Key.wasPressedThisFrame) servingPlate.TrySetBrownieServingCount(2);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) servingPlate.TrySetBrownieServingCount(3);
        }
        if (!resetInProgress && HasSelectedRecipe && Keyboard.current != null && Keyboard.current.mKey.isPressed)
        {
            selectionHoldTime += Time.unscaledDeltaTime;
            if (selectionHoldTime >= RestartHoldSeconds)
            {
                selectionHoldTime = 0f;
                ReturnToRecipeSelection();
            }
        }
        else selectionHoldTime = 0f;

        if (!string.IsNullOrEmpty(correctionMessage) &&
            Time.unscaledTime >= correctionExpiresAt)
        {
            correctionMessage = string.Empty;
        }

        TrackTutorialProgress();
        RefreshGuide();
    }

    private void TrackRestartHold()
    {
        bool held = Keyboard.current != null && Keyboard.current.rKey.isPressed;
        if (AdvanceRestartHold(held, Time.unscaledDeltaTime))
            RestartRecipe();
    }

    public bool AdvanceRestartHold(bool held, float elapsedSeconds)
    {
        if (resetInProgress || !held)
        {
            restartHoldTime = 0f;
            return false;
        }

        restartHoldTime += Mathf.Max(0f, elapsedSeconds);
        if (restartHoldTime < RestartHoldSeconds)
            return false;

        restartHoldTime = 0f;
        return true;
    }

    public void RestartRecipe()
    {
        if (resetInProgress)
            return;

        Scene activeScene = SceneManager.GetActiveScene();

        if (!activeScene.IsValid() || string.IsNullOrWhiteSpace(activeScene.name))
        {
            ShowCorrection(
                "The recipe could not restart because no active scene is available.");
            return;
        }

        resetInProgress = true;
        cookieBakeFinished = false;
        recipeCompletionLatched = false;
        completedBakeResult = OvenBakeResult.None;
        stageLabel = "RESTARTING RECIPE";
        guideItems.Clear();
        correctionMessage = string.Empty;
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(activeScene.name, LoadSceneMode.Single);
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        measureTransfers.Clear();DiscardedMeasureCount=0;
        resetInProgress = false;
        correctionMessage = string.Empty;
        ResolveSceneReferences();
        RefreshGuide();
    }

    private void ResolveSceneReferences()
    {
        servingPlate = FindAnyObjectByType<ServingPlateReceiver>();
        bowl = FindAnyObjectByType<BowlReceiver>();
        brownieBowl = PourableIngredientReceiver.FindBrownieBowl();
        cupcakeSequence = FindAnyObjectByType<MixingSequence>();
        browniePan = FindAnyObjectByType<BrowniePanReceiver>();
        recipeClipboard = FindAnyObjectByType<RecipeSelectionClipboard>();
        RecipeDefinition cookieRecipe = bowl != null ? bowl.ActiveRecipe : null;
        activeRecipe = cookieRecipe;
        if (!string.IsNullOrWhiteSpace(selectedRecipeId) &&
            recipeClipboard != null &&
            recipeClipboard.TryGetRecipe(selectedRecipeId, out RecipeDefinition selectedRecipe))
        {
            activeRecipe = selectedRecipe;
        }
        ApplyActiveRecipeToMeasuringTools();
        board = FindAnyObjectByType<DoughBoardController>();
        trayLiner = FindAnyObjectByType<TrayLinerReceiver>();
        tray = FindAnyObjectByType<TrayReceiver>();
        ovenZone = FindAnyObjectByType<OvenBakeZone>();
        oven = FindAnyObjectByType<OvenController>();
        ovenDoor = FindAnyObjectByType<OvenDoorController>();
        pickupController = FindAnyObjectByType<PickupController>();
        miseEnPlace = FindAnyObjectByType<MiseEnPlaceStation>();
        fridgeDoors = FindObjectsByType<FridgeDoorController>();
        cabinetDoors = FindObjectsByType<SlidingCabinetDoor>();
        fridgeOpened = false;
        cabinetOpened = false;
        if (oven != null)
            oven.SetActiveRecipe(activeRecipe);
    }

    private void ApplyActiveRecipeToMeasuringTools()
    {
        foreach (MeasuringScoop scoop in FindObjectsByType<MeasuringScoop>())
        {
            if (scoop != null)
                scoop.SetActiveRecipe(activeRecipe);
        }
    }

    private void TrackTutorialProgress()
    {
        foreach (var door in fridgeDoors)
            if (door != null && (door.IsOpen || door.IsMoving)) fridgeOpened = true;
        foreach (var door in cabinetDoors)
            if (door != null && (door.IsOpen || door.IsMoving)) cabinetOpened = true;
        if (tutorialComplete) return;

        Keyboard keyboard = Keyboard.current;

        if (keyboard != null &&
            (keyboard.wKey.isPressed ||
             keyboard.aKey.isPressed ||
             keyboard.sKey.isPressed ||
             keyboard.dKey.isPressed))
        {
            movementPracticed = true;
        }

        Mouse mouse = Mouse.current;

        if (mouse != null && !mouse.rightButton.isPressed && mouse.delta.ReadValue().sqrMagnitude >= 1f)
            lookPracticed = true;

        Rigidbody heldObject = pickupController != null
            ? pickupController.HeldObject
            : null;

        if (heldObject != null &&
            heldObject.GetComponentInChildren<MixingTool>(true) != null)
        {
            whiskPickupPracticed = true;
            if (mouse != null)
            {
                rotationPracticed |= mouse.rightButton.isPressed && mouse.delta.ReadValue().sqrMagnitude >= 1f;
                distancePracticed |= Mathf.Abs(mouse.scroll.ReadValue().y) > .01f;
            }
        }

        tutorialComplete =
            movementPracticed &&
            lookPracticed &&
            whiskPickupPracticed && rotationPracticed && distancePracticed &&
            (miseEnPlace == null || miseEnPlace.WhiskOnUtensilsArea);
    }

    private void HandleRecipeFeedback(
        string message,
        RecipeFeedbackTone tone)
    {
        // Successful actions update the checklist through component state.
        // Only corrective feedback temporarily supplements the guide.
        if (tone == RecipeFeedbackTone.Warning)
            ShowCorrection(message);
    }

    private void ShowCorrection(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        correctionMessage = message;
        correctionExpiresAt = Time.unscaledTime + CorrectionDisplaySeconds;
    }

    private void RefreshCupcakeGuide()
    {
        if (!cupcakeSequence) cupcakeSequence = FindAnyObjectByType<MixingSequence>();
        sectionHeading = "Cupcake Batter Practice";
        stageLabel = "HARD | PREPARING EQUIPMENT";
        if (!cupcakeSequence) return;
        var batch = cupcakeSequence.CupcakeBatch;
        if (batch && !cupcakeSequence.EquipmentReady && !cupcakeSequence.IsComplete)
        {
            sectionHeading = "Prepare the Cupcake Tray";
            stageLabel = "HARD | PREHEAT AND LINE 12 WELLS";
            guideItems.Add(new GuideItem($"Set oven to 180 C; PREHEAT ({oven.PreheatProgress:P0})", oven.IsPreheated));
            guideItems.Add(new GuideItem("Keep the oven empty while preheating", !ovenZone.HasAnyBakeware));
            guideItems.Add(new GuideItem("Cupcake tray on LINE & LOAD worktop", batch.TrayOnWorktop));
            guideItems.Add(new GuideItem($"E at paper stack; place one liner per well: {batch.LinedCount}/12", batch.LinedCount==12));
            return;
        }
        if (batch && cupcakeSequence.IsComplete)
        {
            RefreshCupcakeBakingGuide(batch);
            return;
        }
        if (cupcakeSequence.IsComplete)
        {
            sectionHeading = "Cupcake Batter Ready";
            stageLabel = "BATTER PRACTICE COMPLETE";
            guideItems.Add(new GuideItem("All batter preparation steps completed", true));
            guideItems.Add(new GuideItem("Baking, cooling and from-scratch frosting: coming next", false));
            guideItems.Add(new GuideItem("Hold R: practice again | Hold M: choose recipe", false));
            return;
        }
        var phase = cupcakeSequence.Current;
        stageLabel = $"HARD | STEP {cupcakeSequence.PhaseIndex + 1}/{activeRecipe.MixingPhases.Count}";
        sectionHeading = phase.Title;
        bool bowlOnRest = false;
        foreach (var receiver in FindObjectsByType<PourableIngredientReceiver>())
            if (receiver.StagedProcess == cupcakeSequence && receiver.BowlRole == phase.BowlRole)
                bowlOnRest = receiver.IsBowlReady;
        guideItems.Add(new GuideItem($"Cupcake {phase.BowlRole} Bowl | {(phase.BowlRole == "Dry" ? "CUPCAKE DRY" : "MIX")} rest", bowlOnRest));
        foreach (var requirement in phase.Ingredients)
            guideItems.Add(new GuideItem($"{requirement.DisplayName}: {requirement.UnitLabel} x {requirement.RequiredQuantity} ({cupcakeSequence.Amount(requirement)}/{requirement.RequiredQuantity})", cupcakeSequence.Amount(requirement) >= requirement.RequiredQuantity));
        guideItems.Add(new GuideItem($"{(phase.Action == MixingAction.Scrape ? "Scraper: sweep the inside of the bowl" : "Whisk: move through the ingredients")}: {cupcakeSequence.Passes}/{phase.Passes}", false));
        guideItems.Add(new GuideItem(batch ? "Next: fill each lined well three-quarters" : "Batter practice; baking and frosting follow later", false));
    }

    private void RefreshCupcakeBakingGuide(CupcakeBatch batch)
    {
        if (!batch.BakeCompleted)
        {
            if (batch.FilledCount < 12)
            {
                sectionHeading = "Fill the Cupcake Liners"; stageLabel = "HARD | FILL THREE-QUARTERS";
                guideItems.Add(new GuideItem("Cupcake tray on LINE & LOAD worktop", batch.TrayOnWorktop));
                guideItems.Add(new GuideItem("Hold and tilt the BATTER BOWL above one lined well", false));
                guideItems.Add(new GuideItem($"Three-quarter fills: {batch.FilledCount}/12", false));
                guideItems.Add(new GuideItem("Each well stops filling automatically; move to the next", false));
            }
            else
            {
                sectionHeading = "Bake the Cupcakes"; stageLabel = "HARD | BAKE";
                guideItems.Add(new GuideItem("12 lined wells filled", true));
                guideItems.Add(new GuideItem($"Preheat to 180 C ({oven.PreheatProgress:P0})", oven.IsPreheated));
                guideItems.Add(new GuideItem("Load the tray into the oven; close the door", batch.IsInOven && ovenDoor.IsClosed));
                guideItems.Add(new GuideItem(oven.IsBaking ? $"Baking: {oven.TimeRemaining:0.0}s remaining" : "Press BAKE (10 seconds)", false));
            }
            return;
        }
        sectionHeading = "Check and Cool the Cupcakes"; stageLabel = "HARD | COOL BEFORE FROSTING";
        guideItems.Add(new GuideItem("Remove tray to LINE & LOAD worktop", batch.TrayOnWorktop));
        guideItems.Add(new GuideItem("Pick up TESTER; touch its tip into a cupcake", batch.DonenessChecked));
        if (batch.DonenessChecked) guideItems.Add(new GuideItem(GetResultLabel(batch.BakeResult), batch.BakeResult == OvenBakeResult.Perfect));
        guideItems.Add(new GuideItem($"Move individual cupcakes to wire rack: {batch.OnRackCount}/12", batch.OnRackCount==12));
        guideItems.Add(new GuideItem($"Cooled on rack: {batch.CooledCount}/12 (10s each)", batch.AllCooled));
        if (batch.AllCooled)
        {
            sectionHeading = "Cupcakes Cooled"; stageLabel = "BAKING PRACTICE COMPLETE";
            guideItems.Add(new GuideItem("From-scratch buttercream, piping and serving: next", false));
            guideItems.Add(new GuideItem("Hold R: repeat | Hold M: choose recipe", false));
        }
    }

    private void RefreshGuide()
    {
        guideItems.Clear();

        if (resetInProgress)
            return;

        if (HasSelectedRecipe && IsCupcakeRecipeActive)
        {
            RefreshCupcakeGuide();
            return;
        }

        if (bowl == null || board == null || tray == null || oven == null)
        {
            sectionHeading = "Preparing the Kitchen";
            stageLabel = "LOADING KITCHEN";
            guideItems.Add(new GuideItem("Find recipe stations", false));
            return;
        }

        if (oven.BakeCompleted)
        {
            completedBakeResult = oven.LastBakeResult;
            if (!IsBrownieRecipeActive)
            {
                cookieBakeFinished = true;
                foreach (var food in tray.PlacedIngredients)
                {
                    if (!food) continue;
                    var portion = food.GetComponent<ServingPortion>();
                    if (!portion) portion = food.gameObject.AddComponent<ServingPortion>();
                    portion.Configure(activeRecipe.RecipeId, completedBakeResult);
                }
            }
        }

        if (HasSelectedRecipe && (IsBrownieRecipeActive ? browniePan != null && browniePan.IsSliced : cookieBakeFinished))
        {
            RefreshServingGuide();
            return;
        }

        if (recipeCompletionLatched)
        {
            sectionHeading = "Recipe Complete";
            stageLabel = "RECIPE COMPLETE";
            guideItems.Add(new GuideItem(
                GetResultLabel(completedBakeResult),
                completedBakeResult == OvenBakeResult.Perfect));
            guideItems.Add(new GuideItem(
                "Hold R for 3 seconds to bake another batch",
                false));
            return;
        }

        if (!tutorialComplete)
        {
            sectionHeading = "Movement Tutorial";
            stageLabel = "BASIC CONTROLS";
            if (!movementPracticed || !lookPracticed || !whiskPickupPracticed)
            {
                guideItems.Add(new GuideItem("W A S D: move", movementPracticed));
                guideItems.Add(new GuideItem("Mouse: look around", lookPracticed));
                guideItems.Add(new GuideItem("Look at the WHISK; press E to pick up", whiskPickupPracticed));
            }
            else
            {
                sectionHeading = "Practice Holding Tools";
                stageLabel = "HANDLE THE WHISK";
                guideItems.Add(new GuideItem("Hold right click + move mouse: rotate", rotationPracticed));
                guideItems.Add(new GuideItem("Scroll wheel: change holding distance", distancePracticed));
                guideItems.Add(new GuideItem("E: place whisk on Utensils Area", miseEnPlace != null && miseEnPlace.WhiskOnUtensilsArea));
            }
            return;
        }

        if (!HasSelectedRecipe)
        {
            sectionHeading = "Choose a Recipe";
            stageLabel = "RECIPE SELECTION";
            guideItems.Add(new GuideItem(
                "Look at the framed recipe papers beneath BakeIT",
                false));
            guideItems.Add(new GuideItem(
                "E: Cookies, Brownies or Cupcake batter practice",
                false));
            return;
        }

        if (activeRecipe != null &&
            string.Equals(activeRecipe.RecipeId, "brownies",
                System.StringComparison.OrdinalIgnoreCase))
        {
            RefreshBrownieDevelopmentGuide();
            return;
        }

        if (miseEnPlace != null && !miseEnPlace.IsPrepared)
        {
            sectionHeading = "Let's Practice Mise En Place";
            stageLabel = "COLLECT COLD INGREDIENTS";
            if (!miseEnPlace.ColdIngredientsPrepared)
            {
                guideItems.Add(new GuideItem("E: open the fridge", fridgeOpened));
                guideItems.Add(new GuideItem("Egg on Cold Ingredient Rest", miseEnPlace.EggPrepared));
                guideItems.Add(new GuideItem("Butter on Cold Ingredient Rest", miseEnPlace.ButterPrepared));
                guideItems.Add(new GuideItem("Chips on Cold Ingredient Rest", miseEnPlace.ChipsPrepared));
            }
            else if (!miseEnPlace.DryIngredientsPrepared)
            {
                stageLabel = "COLLECT DRY INGREDIENTS";
                guideItems.Add(new GuideItem("E: open the dry-ingredient cabinet", cabinetOpened));
                guideItems.Add(new GuideItem("Flour tin on Cold Ingredient Rest", miseEnPlace.FlourPrepared));
                guideItems.Add(new GuideItem("Sugar jar on Cold Ingredient Rest", miseEnPlace.SugarPrepared));
                guideItems.Add(new GuideItem("Vanilla on Cold Ingredient Rest", miseEnPlace.VanillaPrepared));
                guideItems.Add(new GuideItem("Baking soda on Cold Ingredient Rest", miseEnPlace.BakingSodaPrepared));
                guideItems.Add(new GuideItem("Salt on Cold Ingredient Rest", miseEnPlace.SaltPrepared));
            }
            else if (!miseEnPlace.ToolsPrepared)
            {
                sectionHeading = "Let's Prepare the Tools";
                if (!miseEnPlace.LargeSpoonPrepared)
                {
                    stageLabel = "TOOLS NEEDED FOR THIS RECIPE";
                    guideItems.Add(new GuideItem(
                        "Grab the 1 Cup Measuring Cup",
                        false));
                    guideItems.Add(new GuideItem(
                        "Put it on the Utensils Area",
                        miseEnPlace.LargeSpoonPrepared));
                }
                else
                {
                    stageLabel = "GREAT! PREPARE THE REMAINING TOOLS";
                    guideItems.Add(new GuideItem("1/2 Cup Measuring Cup", miseEnPlace.HalfCupPrepared));
                    guideItems.Add(new GuideItem("1 Teaspoon Measuring Spoon", miseEnPlace.OneTeaspoonPrepared));
                    guideItems.Add(new GuideItem("1/2 Teaspoon Measuring Spoon", miseEnPlace.HalfTeaspoonPrepared));
                    guideItems.Add(new GuideItem("1/4 Teaspoon Measuring Spoon", miseEnPlace.QuarterTeaspoonPrepared));
                    guideItems.Add(new GuideItem("Whisk", miseEnPlace.WhiskPrepared));
                }
            }
            else
            {
                stageLabel = "PLACE BOWL IN MIX";
                guideItems.Add(new GuideItem("Collect the WHITE COOKIE BOWL from the rack", false));
                guideItems.Add(new GuideItem("Place in MIX, between tools and board", miseEnPlace.BowlPlaced));
            }
            return;
        }

        if (!bowl.IsRecipeComplete && !bowl.IsBowlReady)
        {
            sectionHeading = "Return to Mixing";
            stageLabel = "RETURN BOWL TO MIX";
            guideItems.Add(new GuideItem("Place the bowl between tools and board", false));
            return;
        }

        if (!bowl.HasAllBaseIngredients)
        {
            sectionHeading = "Let's Build the Dough";
            stageLabel = "ADD BASE INGREDIENTS TO BOWL";
            if (activeRecipe != null)
            {
                foreach (RecipeIngredientRequirement requirement in
                         activeRecipe.GetIngredients(RecipeIngredientStage.Base))
                {
                    guideItems.Add(new GuideItem(
                        requirement.GetGuideText(),
                        bowl.HasIngredientRequirement(requirement)));
                }
            }
            else
            {
                guideItems.Add(new GuideItem("Flour: 1 Measuring Cup; tilt into bowl", bowl.HasRequiredFlour));
                guideItems.Add(new GuideItem("Sugar: 1 Measuring Cup; tilt into bowl", bowl.HasRequiredSugar));
                guideItems.Add(new GuideItem("1 butter portion", bowl.HasRequiredButter));
                guideItems.Add(new GuideItem("1 egg", bowl.HasRequiredEgg));
            }
            return;
        }

        if (!bowl.IsBaseDoughMixed)
        {
            sectionHeading = "Let's Practice Mixing";
            stageLabel = "WHISK BASE DOUGH";
            guideItems.Add(new GuideItem(
                $"Whisk passes: {bowl.WhiskPassCount} of {bowl.RequiredWhiskPasses}",
                false));
            return;
        }

        if (!bowl.HasRequiredChocolateChips)
        {
            sectionHeading = "Add the Chocolate Chips";
            RecipeIngredientRequirement finishingIngredient = activeRecipe != null
                ? activeRecipe.GetFirstIngredient(RecipeIngredientStage.Finishing)
                : null;
            string finishingName = finishingIngredient != null
                ? finishingIngredient.DisplayName.ToUpperInvariant()
                : "CHOCOLATE CHIPS";
            stageLabel = $"ADD {finishingName}";
            guideItems.Add(new GuideItem(
                $"Chocolate chips: {bowl.ChocolateChipScoopCount} of {bowl.RequiredChocolateChipScoops} half-cup measures; tilt into bowl",
                false));
            return;
        }

        if (!bowl.IsRecipeComplete)
        {
            sectionHeading = "Finish Mixing";
            RecipeIngredientRequirement finishingIngredient = activeRecipe != null
                ? activeRecipe.GetFirstIngredient(RecipeIngredientStage.Finishing)
                : null;
            string finishingName = finishingIngredient != null
                ? finishingIngredient.DisplayName.ToUpperInvariant()
                : "CHOCOLATE CHIPS";
            stageLabel = $"COMBINE {finishingName}";
            guideItems.Add(new GuideItem(
                $"Whisk passes: {bowl.ChipMixPassCount} of {bowl.RequiredChipMixPasses}", false));
            return;
        }

        if (board.LastPortionCount == 0 && !board.HasDough)
        {
            if (miseEnPlace != null && miseEnPlace.SuppliesOnBoard)
            {
                sectionHeading = "Prepare the Work Surface";
                stageLabel = "CLEAR THE CHOPPING BOARD";
                guideItems.Add(new GuideItem("Return supplies to their preparation rests", false));
                guideItems.Add(new GuideItem("Keep the board clear for dough", false));
                return;
            }
            sectionHeading = "Move the Dough";
            stageLabel = "MOVE DOUGH TO BOARD";
            guideItems.Add(new GuideItem("Place mixed dough on the board", false));
            return;
        }

        if (board.LastPortionCount == 0 && !board.IsDoughFlattened)
        {
            sectionHeading = "Let's Practice Rolling";
            stageLabel = "FLATTEN DOUGH";
            guideItems.Add(new GuideItem(
                $"Rolling passes: {board.RollingPassCount} of {board.RequiredRollingPasses}",
                false));
            return;
        }

        if (board.LastPortionCount == 0)
        {
            sectionHeading = "Let's Practice Portioning";
            stageLabel = "PORTION DOUGH";
            guideItems.Add(new GuideItem($"Cut into {board.TargetCookieCount} equal portions", false));
            return;
        }

        if (!tray.HasCompleteCookieBatch)
        {
            sectionHeading = "Prepare the Baking Tray";
            int expectedPortions = tray.ExpectedPortionCount > 0
                ? tray.ExpectedPortionCount
                : board.LastPortionCount;
            stageLabel = "PREPARE TRAY";
            guideItems.Add(new GuideItem(
                "Collect the FLAT COOKIE TRAY from the storage rack",
                trayLiner != null && trayLiner.HasParchment));
            guideItems.Add(new GuideItem(
                "E: pull parchment; place it on the flat tray",
                trayLiner != null && trayLiner.HasParchment));
            guideItems.Add(new GuideItem(
                $"Move portions: {tray.PlacedPortionCount} of {expectedPortions}",
                tray.HasCompleteCookieBatch));
            return;
        }

        if (!oven.IsPreheated)
        {
            sectionHeading = "Preheat the Oven";
            stageLabel = "PREHEAT OVEN";
            bool correctTemperature =
                oven.TemperatureCelsius >= oven.MinimumPerfectTemperature &&
                oven.TemperatureCelsius <= oven.MaximumPerfectTemperature;
            guideItems.Add(new GuideItem(
                $"Set {oven.MinimumPerfectTemperature:0}-{oven.MaximumPerfectTemperature:0} C  (target {oven.TemperatureCelsius:0} C)",
                correctTemperature));
            guideItems.Add(new GuideItem(
                "Close oven door",
                ovenDoor != null && ovenDoor.IsClosed));

            if (ovenZone != null && ovenZone.HasLoadedTray)
            {
                guideItems.Add(new GuideItem(
                    "Remove tray before preheating",
                    false));
            }
            else if (oven.IsPreheating)
            {
                guideItems.Add(new GuideItem(
                    $"Heating: {oven.CurrentTemperatureCelsius:0} of {oven.TemperatureCelsius:0} C",
                    false));
            }
            else
            {
                guideItems.Add(new GuideItem("Press PREHEAT", false));
            }

            return;
        }

        if (ovenZone == null || !ovenZone.HasLoadedTray)
        {
            sectionHeading = "Load the Oven";
            stageLabel = "LOAD OVEN";
            guideItems.Add(new GuideItem("Oven preheated", true));
            guideItems.Add(new GuideItem("Place prepared tray in oven", false));
            return;
        }

        if (!oven.BakeCompleted)
        {
            sectionHeading = "Bake the Cookies";
            string productPlural = activeRecipe != null
                ? activeRecipe.ProductPlural.ToUpperInvariant()
                : "COOKIES";
            stageLabel = $"BAKE {productPlural}";
            bool correctTemperature =
                oven.TemperatureCelsius >= oven.MinimumPerfectTemperature &&
                oven.TemperatureCelsius <= oven.MaximumPerfectTemperature;
            guideItems.Add(new GuideItem(
                $"Set {oven.MinimumPerfectTemperature:0}-{oven.MaximumPerfectTemperature:0} C  (now {oven.TemperatureCelsius:0} C)",
                correctTemperature));
            guideItems.Add(new GuideItem(
                "Close oven door",
                ovenDoor != null && ovenDoor.IsClosed));

            if (oven.IsBaking)
            {
                guideItems.Add(new GuideItem(
                    $"Baking: {Mathf.CeilToInt(oven.TimeRemaining)} seconds left",
                    false));
            }
            else
            {
                guideItems.Add(new GuideItem("Start oven", false));
            }

            return;
        }

    }

    private void RefreshBrownieDevelopmentGuide()
    {
        sectionHeading = "Let's Practice Brownie Mise En Place";

        if (brownieBowl == null)
        {
            stageLabel = "NEXT PART STILL IN DEVELOPMENT";
            guideItems.Add(new GuideItem(
                "The Brownie Mixing Bowl is not available in this scene",
                false));
            return;
        }

        if (miseEnPlace != null && !miseEnPlace.BrownieIsPrepared)
        {
            stageLabel = "COLLECT COLD INGREDIENTS";
            if (!miseEnPlace.BrownieColdIngredientsPrepared)
            {
                guideItems.Add(new GuideItem("E: open the fridge", fridgeOpened));
                guideItems.Add(new GuideItem(
                    $"Large eggs on Cold Ingredient Rest: " +
                    $"{miseEnPlace.BrownieEggsPreparedCount} of 4",
                    miseEnPlace.BrownieColdIngredientsPrepared));
            }
            else if (!miseEnPlace.BrownieDryIngredientsPrepared)
            {
                stageLabel = "COLLECT DRY INGREDIENTS";
                guideItems.Add(new GuideItem(
                    "E: open the dry-ingredient cabinet", cabinetOpened));
                guideItems.Add(new GuideItem("All-purpose flour on Cold Ingredient Rest", IsPreparedOn(miseEnPlace.Flour, miseEnPlace.IngredientRest)));
                guideItems.Add(new GuideItem("Granulated white sugar on Cold Ingredient Rest", IsPreparedOn(miseEnPlace.Sugar, miseEnPlace.IngredientRest)));
                guideItems.Add(new GuideItem("Melted butter cup on Cold Ingredient Rest", IsPreparedOn(miseEnPlace.MeltedButterCup, miseEnPlace.IngredientRest)));
                guideItems.Add(new GuideItem("Cocoa powder on Cold Ingredient Rest", IsPreparedOn(miseEnPlace.Cocoa, miseEnPlace.IngredientRest)));
                guideItems.Add(new GuideItem("Vanilla extract on Cold Ingredient Rest", IsPreparedOn(miseEnPlace.Vanilla, miseEnPlace.IngredientRest)));
                guideItems.Add(new GuideItem("Baking powder on Cold Ingredient Rest", IsPreparedOn(miseEnPlace.BakingPowder, miseEnPlace.IngredientRest)));
                guideItems.Add(new GuideItem("Salt on Cold Ingredient Rest", IsPreparedOn(miseEnPlace.Salt, miseEnPlace.IngredientRest)));
                guideItems.Add(new GuideItem("Walnuts on Cold Ingredient Rest", IsPreparedOn(miseEnPlace.Walnuts, miseEnPlace.IngredientRest)));
            }
            else if (!miseEnPlace.BrownieToolsPrepared)
            {
                stageLabel = "PREPARE UTENSILS";
                guideItems.Add(new GuideItem("1 cup measuring cup", IsPreparedOn(miseEnPlace.OneCup, miseEnPlace.ToolRest)));
                guideItems.Add(new GuideItem("1/2 cup measuring cup", IsPreparedOn(miseEnPlace.HalfCup, miseEnPlace.ToolRest)));
                guideItems.Add(new GuideItem("1/4 cup measuring cup", IsPreparedOn(miseEnPlace.GetMeasuringTool(RecipeMeasureTool.QuarterCup), miseEnPlace.ToolRest)));
                guideItems.Add(new GuideItem("1 teaspoon measuring spoon", IsPreparedOn(miseEnPlace.OneTeaspoon, miseEnPlace.ToolRest)));
                guideItems.Add(new GuideItem("1/2 teaspoon measuring spoon", IsPreparedOn(miseEnPlace.HalfTeaspoon, miseEnPlace.ToolRest)));
                guideItems.Add(new GuideItem("Whisk", IsPreparedOn(miseEnPlace.Whisk, miseEnPlace.ToolRest)));
            }
            else
            {
                stageLabel = "PLACE LARGE BOWL";
                guideItems.Add(new GuideItem(
                    "Collect the BROWNIE MIXING BOWL from the rack",
                    false));
                guideItems.Add(new GuideItem(
                    "Place it in the MIX area",
                    miseEnPlace.BrownieBowlPlaced));
            }
            return;
        }

        sectionHeading = "Let's Practice Brownies";

        if (!brownieBowl.IsBatterComplete && !brownieBowl.IsBowlReady)
        {
            stageLabel = "PLACE LARGE BOWL";
            guideItems.Add(new GuideItem(
                "Place the BROWNIE MIXING BOWL in the MIX area",
                false));
            return;
        }

        if (!brownieBowl.HasAllBaseIngredients)
        {
            stageLabel = "MEASURE INGREDIENTS";
            foreach (RecipeIngredientRequirement requirement in
                     activeRecipe.GetIngredients(RecipeIngredientStage.Base))
            {
                guideItems.Add(new GuideItem(
                    requirement.GetGuideText(),
                    brownieBowl.HasIngredientRequirement(requirement)));
            }
            return;
        }

        if (!brownieBowl.IsBaseMixed)
        {
            stageLabel = "MIX BROWNIE BATTER";
            guideItems.Add(new GuideItem(
                $"Whisk passes: {brownieBowl.BaseMixPassCount} of " +
                brownieBowl.RequiredBaseMixPasses,
                false));
            return;
        }

        if (!brownieBowl.HasAllFinishingIngredients)
        {
            stageLabel = "ADD WALNUTS";
            foreach (RecipeIngredientRequirement requirement in
                     activeRecipe.GetIngredients(RecipeIngredientStage.Finishing))
            {
                guideItems.Add(new GuideItem(
                    requirement.GetGuideText(),
                    brownieBowl.HasIngredientRequirement(requirement)));
            }
            return;
        }

        if (!brownieBowl.IsBatterComplete)
        {
            stageLabel = "FOLD IN WALNUTS";
            guideItems.Add(new GuideItem(
                $"Whisk passes: {brownieBowl.FinishingMixPassCount} of " +
                brownieBowl.RequiredFinishingMixPasses,
                false));
            return;
        }

        if (browniePan == null)
        {
            sectionHeading = "Preparing the Brownie Pan";
            stageLabel = "BROWNIE PAN UNAVAILABLE";
            guideItems.Add(new GuideItem(
                "The deep Brownie Pan is not configured in this scene",
                false));
            return;
        }

        if (!browniePan.HasParchment)
        {
            sectionHeading = "Prepare the Brownie Pan";
            stageLabel = "LINE THE DEEP PAN";
            guideItems.Add(new GuideItem(
                "Collect the DEEP BROWNIE PAN from the storage rack",
                false));
            guideItems.Add(new GuideItem(
                "E: pull parchment; place it inside the deep pan",
                false));
            return;
        }

        if (!browniePan.HasBatter)
        {
            sectionHeading = "Pour the Brownie Batter";
            stageLabel = "FILL THE LINED PAN";
            guideItems.Add(new GuideItem("Deep pan lined with parchment", true));
            guideItems.Add(new GuideItem(
                "Tilt the BROWNIE MIXING BOWL over the pan",
                false));
            return;
        }

        if (!browniePan.IsSpread)
        {
            sectionHeading = "Spread the Brownie Batter";
            stageLabel = "SPREAD BATTER EVENLY";
            guideItems.Add(new GuideItem(
                "Use the RUBBER SPATULA in the pan",
                false));
            guideItems.Add(new GuideItem(
                $"Spreading passes: {browniePan.SpreadPassCount} of " +
                browniePan.RequiredSpreadPasses,
                false));
            return;
        }

        if (browniePan.BakeCompleted)
        {
            sectionHeading = browniePan.IsSliced ? "Brownies Ready" : "Cut the Brownies";
            stageLabel = browniePan.IsSliced ? "12 BROWNIE SQUARES" : "SLICE INTO SQUARES";
            guideItems.Add(new GuideItem(
                GetResultLabel(browniePan.BakeResult),
                browniePan.BakeResult == OvenBakeResult.Perfect));
            guideItems.Add(new GuideItem(
                "Remove the pan from the oven and set it on the worktop",
                ovenZone != null && !ovenZone.HasLoadedBrowniePan));
            guideItems.Add(new GuideItem(
                $"Move the knife across the brownies: {browniePan.SlicePassCount} of 5 cuts (4 by 3 squares)",
                browniePan.IsSliced));
            return;
        }

        if (!oven.IsPreheated)
        {
            sectionHeading = "Preheat the Oven";
            stageLabel = "PREHEAT FOR BROWNIES";
            bool correctTemperature =
                oven.TemperatureCelsius >= oven.MinimumPerfectTemperature &&
                oven.TemperatureCelsius <= oven.MaximumPerfectTemperature;
            guideItems.Add(new GuideItem(
                $"Set {oven.MinimumPerfectTemperature:0}-{oven.MaximumPerfectTemperature:0} C  " +
                $"(target {oven.TemperatureCelsius:0} C)",
                correctTemperature));
            guideItems.Add(new GuideItem(
                "Close oven door",
                ovenDoor != null && ovenDoor.IsClosed));

            if (ovenZone != null && ovenZone.HasLoadedBakeware)
                guideItems.Add(new GuideItem("Remove pan before preheating", false));
            else if (oven.IsPreheating)
                guideItems.Add(new GuideItem(
                    $"Heating: {oven.CurrentTemperatureCelsius:0} of " +
                    $"{oven.TemperatureCelsius:0} C",
                    false));
            else
                guideItems.Add(new GuideItem("Press PREHEAT", false));
            return;
        }

        if (ovenZone == null || !ovenZone.HasLoadedBrowniePan)
        {
            sectionHeading = "Load the Oven";
            stageLabel = "LOAD THE BROWNIE PAN";
            guideItems.Add(new GuideItem("Oven preheated", true));
            guideItems.Add(new GuideItem(
                "Place the prepared deep pan inside the oven",
                false));
            return;
        }

        if (!browniePan.BakeCompleted)
        {
            sectionHeading = "Bake the Brownies";
            stageLabel = "BAKE BROWNIES";
            guideItems.Add(new GuideItem(
                $"Bake for {activeRecipe.BakeDurationSeconds:0.#} seconds",
                oven.IsBaking));
            guideItems.Add(new GuideItem(
                "Close oven door",
                ovenDoor != null && ovenDoor.IsClosed));
            guideItems.Add(new GuideItem(
                oven.IsBaking
                    ? $"Baking: {Mathf.CeilToInt(oven.TimeRemaining)} seconds left"
                    : "Press BAKE",
                false));
            return;
        }

        sectionHeading = "Bake the Brownies";
        stageLabel = "COMPLETE THE CURRENT OVEN STEP";
    }

    private static bool IsPreparedOn(
        Transform target,
        StablePlacementSurface surface)
    {
        PreparationItem item = target != null
            ? target.GetComponent<PreparationItem>()
            : null;
        return item != null && item.RestingSurface == surface;
    }

    private static string GetResultLabel(OvenBakeResult result)
    {
        return result switch
        {
            OvenBakeResult.Underbaked => "Result: undercooked",
            OvenBakeResult.Perfect => "Result: baked successfully",
            OvenBakeResult.Overcooked => "Result: overcooked",
            OvenBakeResult.Burnt => "Result: burnt",
            _ => "Check the baked result"
        };
    }

    private void RefreshServingGuide()
    {
        int required = servingPlate != null ? servingPlate.RequiredCount : IsBrownieRecipeActive ? 2 : 3;
        int placed = servingPlate != null ? servingPlate.PlacedCount : 0;
        recipeCompletionLatched = servingPlate != null && servingPlate.IsServingComplete;
        sectionHeading = recipeCompletionLatched ? "Ready to Serve" : "Plate a Serving";
        stageLabel = recipeCompletionLatched ? "SERVING COMPLETE" : "PLATE THE FINISHED FOOD";
        if (IsBrownieRecipeActive) guideItems.Add(new GuideItem($"Serving size: [2] two / [3] three brownies (selected: {required})", false));
        guideItems.Add(new GuideItem($"Place {required} {activeRecipe.ProductPlural} on the serving plate: {placed} of {required}", recipeCompletionLatched));
        if (recipeCompletionLatched)
        {
            var result = IsBrownieRecipeActive ? browniePan.BakeResult : completedBakeResult;
            guideItems.Add(new GuideItem(GetResultLabel(result), result == OvenBakeResult.Perfect));
            guideItems.Add(new GuideItem("Hold R: same recipe | Hold M: choose another", false));
        }
        else guideItems.Add(new GuideItem("Set bakeware on the worktop; E to pick up / place food", false));
    }

    private float GuideRowHeight(GuideItem item, float width, float minimum)
    {
        if (!IsCupcakeRecipeActive) return minimum;
        GUIStyle style = item.Complete ? completedItemStyle : pendingItemStyle;
        string text = (item.Complete ? "[x] " : "[ ] ") + item.Text;
        return Mathf.Max(minimum, style.CalcHeight(new GUIContent(text), width) + 6f);
    }

    private void OnGUI()
    {
        EnsureStyles();

        float panelWidth = Mathf.Min(
            410f * GuideUiScale,
            Mathf.Max(320f * GuideUiScale, Screen.width * 0.34f));
        float itemHeight = 24f * GuideUiScale;
        float guideRowsHeight = 0f;
        foreach (var item in guideItems) guideRowsHeight += GuideRowHeight(item, panelWidth - 36f * GuideUiScale, itemHeight);
        float headerHeight = 82f * GuideUiScale;
        var heldMeasure=pickupController!=null && pickupController.HeldObject!=null ? pickupController.HeldObject.GetComponent<MeasuringScoop>() : null;
        bool showMeasure=heldMeasure!=null && heldMeasure.IsFilled;
        float correctionHeight = string.IsNullOrEmpty(correctionMessage)
            ? 0f
            : 42f * GuideUiScale;
        float panelHeight =
            headerHeight +
            guideRowsHeight +
            (showMeasure ? itemHeight : 0f) +
            correctionHeight +
            60f * GuideUiScale;
        Rect panel = new Rect(
            14f * GuideUiScale,
            14f * GuideUiScale,
            panelWidth,
            panelHeight);
        Color previousColor = GUI.color;
        GUI.color = new Color(0.04f, 0.035f, 0.03f, 0.9f);
        GUI.Box(panel, GUIContent.none);
        GUI.color = previousColor;

        GUI.Label(
            new Rect(
                panel.x + 14f * GuideUiScale,
                panel.y + 9f * GuideUiScale,
                panel.width - 28f * GuideUiScale,
                24f * GuideUiScale),
            sectionHeading,
            titleStyle);
        GUI.Label(
            new Rect(
                panel.x + 14f * GuideUiScale,
                panel.y + 34f * GuideUiScale,
                panel.width - 28f * GuideUiScale,
                38f * GuideUiScale),
            activeRecipe != null && HasSelectedRecipe
                ? (IsCupcakeRecipeActive ? "CUPCAKES" : activeRecipe.DisplayName.ToUpperInvariant()) + "  |  " + stageLabel
                : stageLabel,
            stageStyle);

        float y = panel.y + headerHeight;
        foreach (GuideItem item in guideItems)
        {
            float rowHeight = GuideRowHeight(item, panel.width - 36f * GuideUiScale, itemHeight);
            GUI.Label(
                new Rect(
                    panel.x + 18f * GuideUiScale,
                    y,
                    panel.width - 36f * GuideUiScale,
                    rowHeight),
                (item.Complete ? "[x] " : "[ ] ") + item.Text,
                item.Complete ? completedItemStyle : pendingItemStyle);
            y += rowHeight;
        }

        if(showMeasure)
        {
            string measureText=heldMeasure.DiscardProgress>0 ? $"Discarding {heldMeasure.CurrentIngredient}: {heldMeasure.DiscardProgress:P0} (release Q to cancel)"
                : $"{heldMeasure.ToolName}: {heldMeasure.CurrentIngredient} | Tilt to pour / return";
            GUI.Label(new Rect(panel.x+14f*GuideUiScale,y,panel.width-28f*GuideUiScale,itemHeight),measureText,hintStyle);
            y+=itemHeight;
        }

        if (!string.IsNullOrEmpty(correctionMessage))
        {
            GUI.Label(
                new Rect(
                    panel.x + 14f * GuideUiScale,
                    y + 2f * GuideUiScale,
                    panel.width - 28f * GuideUiScale,
                    38f * GuideUiScale),
                "! " + correctionMessage,
                correctionStyle);
            y += correctionHeight;
        }

        float footerY = panel.yMax - 56f * GuideUiScale;
        GUI.Label(
            new Rect(
                panel.x + 14f * GuideUiScale,
                footerY,
                panel.width - 28f * GuideUiScale,
                28f * GuideUiScale),
            showMeasure ? "RMB: tilt  |  Hold Q: discard" : tutorialComplete && miseEnPlace != null && !IsActiveMiseEnPlaceComplete &&
            !recipeCompletionLatched
                ? "E: open / pick up / place on rest"
                : restartHoldTime > 0f
                    ? $"Hold R: {RestartHoldProgress:P0}"
                    : "Hold R 3 sec: restart  |  Esc: cursor",
            hintStyle);
        if (HasSelectedRecipe)
            GUI.Label(new Rect(panel.x + 14f * GuideUiScale, footerY + 24f * GuideUiScale,
                panel.width - 28f * GuideUiScale, 24f * GuideUiScale),
                selectionHoldTime > 0f ? $"Leaving batch: hold M {selectionHoldTime / RestartHoldSeconds:P0}"
                    : "Hold M 3 sec: leave batch / choose recipe", hintStyle);
    }

    private void EnsureStyles()
    {
        if (titleStyle != null)
            return;

        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(16f * GuideUiScale),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };
        titleStyle.normal.textColor = new Color(1f, 0.78f, 0.32f);

        stageStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(13f * GuideUiScale),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };
        stageStyle.normal.textColor = new Color(0.96f, 0.93f, 0.84f);

        pendingItemStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(14f * GuideUiScale),
            fontStyle = FontStyle.Bold,
            wordWrap = true,
            alignment = TextAnchor.MiddleLeft
        };
        pendingItemStyle.normal.textColor = new Color(0.96f, 0.93f, 0.84f);

        completedItemStyle = new GUIStyle(pendingItemStyle);
        completedItemStyle.normal.textColor = new Color(0.55f, 0.72f, 0.55f);

        correctionStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(12f * GuideUiScale),
            fontStyle = FontStyle.Bold,
            wordWrap = true,
            alignment = TextAnchor.UpperLeft
        };
        correctionStyle.normal.textColor = new Color(1f, 0.67f, 0.26f);

        hintStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(11f * GuideUiScale),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };
        hintStyle.normal.textColor = new Color(0.72f, 0.72f, 0.72f);

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = Mathf.RoundToInt(11f * GuideUiScale),
            fontStyle = FontStyle.Bold
        };
    }
}
