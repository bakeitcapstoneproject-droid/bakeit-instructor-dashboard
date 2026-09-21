using UnityEngine;
using UnityEngine.InputSystem;

public enum OvenBakeResult
{
    None,
    Underbaked,
    Perfect,
    Overcooked,
    Burnt
}

public class OvenController : MonoBehaviour
{
    [Header("Oven Parts")]
    [SerializeField] private OvenBakeZone bakeZone;
    [SerializeField] private OvenDoorController ovenDoor;

    [Header("Prototype Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private float temperatureCelsius = 0f;
    [SerializeField] private float minimumTemperature = 100f;
    [SerializeField] private float maximumTemperature = 250f;
    [SerializeField] private float temperatureStep = 10f;
    [SerializeField] private float bakeDuration = 5f;

    [Header("Preheat")]
    [SerializeField] private float ambientTemperature = 0f;
    [SerializeField, Min(0.1f)] private float preheatDuration = 5f;

    [Header("Temperature Results")]
    [SerializeField] private float minimumPerfectTemperature = 170f;
    [SerializeField] private float maximumPerfectTemperature = 190f;
    [SerializeField] private float minimumBurntTemperature = 220f;
    [SerializeField] private Color bakedColor =
        new Color(0.88f, 0.67f, 0.36f, 1f);
    [SerializeField] private Color overcookedColor =
        new Color(0.36f, 0.14f, 0.035f, 1f);
    [SerializeField] private Color burntColor =
        new Color(0.035f, 0.022f, 0.012f, 1f);

    [SerializeField, Range(0f, 1f)]
    private float bakedSmoothness = 0.25f;
    [SerializeField, Range(0f, 1f)]
    private float overcookedSmoothness = 0.1f;
    [SerializeField, Range(0f, 1f)]
    private float burntSmoothness = 0.05f;

    [Header("Completion Beep")]
    [SerializeField] private AudioSource completionAudioSource;
    [SerializeField] private AudioClip completionBeep;
    [SerializeField, Range(0f, 1f)]
    private float completionBeepVolume = 0.65f;
    [SerializeField] private float completionBeepFrequency = 880f;
    [SerializeField] private int completionBeepCount = 3;
    [SerializeField] private float completionBeepDuration = 0.12f;
    [SerializeField] private float completionBeepGap = 0.08f;

    private Camera playerCamera;
    private bool isBaking;
    private bool isPreheating;
    [SerializeField, HideInInspector] private bool isPreheated;
    private bool pausedForOpenDoor;
    private bool bakeCompleted;
    private float timeRemaining;
    private float currentTemperatureCelsius;
    private float preheatStartTemperature;
    private float preheatElapsed;
    private int lastDisplayedSecond = -1;
    private OvenBakeResult lastBakeResult = OvenBakeResult.None;
    private RecipeDefinition activeRecipe;

    public bool IsBaking => isBaking;
    public bool IsPreheating => isPreheating;
    public bool IsPreheated => isPreheated;
    public bool IsPausedForOpenDoor => pausedForOpenDoor;
    public bool BakeCompleted => bakeCompleted;
    public float TimeRemaining => timeRemaining;
    public float TemperatureCelsius => temperatureCelsius;
    public float CurrentTemperatureCelsius => currentTemperatureCelsius;
    public float PreheatProgress => isPreheated
        ? 1f
        : Mathf.Clamp01(preheatElapsed / preheatDuration);
    public OvenBakeResult LastBakeResult => lastBakeResult;
    public float MinimumPerfectTemperature => minimumPerfectTemperature;
    public float MaximumPerfectTemperature => maximumPerfectTemperature;
    public float BakeDuration => bakeDuration;
    public RecipeDefinition ActiveRecipe => activeRecipe;

    private void Awake()
    {
        activeRecipe = FindAnyObjectByType<BowlReceiver>()?.ActiveRecipe;
        if (activeRecipe != null)
        {
            bakeDuration = activeRecipe.BakeDurationSeconds;
            minimumPerfectTemperature = activeRecipe.MinimumPerfectTemperature;
            maximumPerfectTemperature = activeRecipe.MaximumPerfectTemperature;
            minimumBurntTemperature = activeRecipe.MinimumBurntTemperature;
        }

        playerCamera = Camera.main;

        if (bakeZone == null)
            bakeZone = GetComponentInChildren<OvenBakeZone>(true);

        if (ovenDoor == null)
            ovenDoor = GetComponentInChildren<OvenDoorController>(true);

        ConfigureCompletionAudio();

        bakeDuration = Mathf.Max(0.1f, bakeDuration);
        preheatDuration = Mathf.Max(0.1f, preheatDuration);
        ambientTemperature = Mathf.Clamp(
            ambientTemperature,
            0f,
            maximumTemperature);
        currentTemperatureCelsius = ambientTemperature;
        interactionDistance = Mathf.Max(0.1f, interactionDistance);
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryUseControl();
        }

        if (isPreheating)
        {
            UpdatePreheat();
            return;
        }

        if (!isBaking)
            return;

        if (bakeZone == null || !bakeZone.HasLoadedBakeware)
        {
            CancelBake("Baking cancelled: the loaded bakeware was removed.");
            return;
        }

        if (ovenDoor != null && !ovenDoor.IsClosed)
        {
            if (!pausedForOpenDoor)
            {
                pausedForOpenDoor = true;
                RecipeFeedback.Warning("Baking paused: close the oven door.");
            }

            return;
        }

        if (pausedForOpenDoor)
        {
            pausedForOpenDoor = false;
            RecipeFeedback.Report("Oven door closed. Baking resumed.");
        }

        timeRemaining = Mathf.Max(
            0f,
            timeRemaining - Time.deltaTime);

        int displayedSecond = Mathf.CeilToInt(timeRemaining);

        if (displayedSecond != lastDisplayedSecond)
        {
            lastDisplayedSecond = displayedSecond;
            RecipeFeedback.Report($"Oven: {displayedSecond} seconds remaining.");
        }

        if (timeRemaining <= 0f)
            CompleteBake();
    }

    private void UpdatePreheat()
    {
        if (ovenDoor != null && !ovenDoor.IsClosed)
        {
            if (!pausedForOpenDoor)
            {
                pausedForOpenDoor = true;
                RecipeFeedback.Warning("Preheating paused: close the oven door.");
            }

            return;
        }

        if (pausedForOpenDoor)
        {
            pausedForOpenDoor = false;
            RecipeFeedback.Report("Oven door closed. Preheating resumed.");
        }

        preheatElapsed = Mathf.Min(
            preheatDuration,
            preheatElapsed + Time.deltaTime);
        currentTemperatureCelsius = Mathf.Lerp(
            preheatStartTemperature,
            temperatureCelsius,
            PreheatProgress);

        if (preheatElapsed < preheatDuration)
            return;

        currentTemperatureCelsius = temperatureCelsius;
        isPreheating = false;
        isPreheated = true;
        pausedForOpenDoor = false;
        RecipeFeedback.Report(
            $"Oven preheated to {temperatureCelsius:0} C. Load the prepared tray.");
    }

    private void TryUseControl()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerCamera == null)
            return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward);

        if (!Physics.Raycast(
                ray,
                out RaycastHit hit,
                interactionDistance,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore))
            return;

        OvenTemperatureButton temperatureButton =
            hit.collider.GetComponentInParent<OvenTemperatureButton>();

        if (temperatureButton != null)
        {
            AdjustTemperature(temperatureButton.Direction);
            return;
        }

        if (hit.collider.GetComponentInParent<OvenStartButton>() != null)
        {
            if (isPreheated)
                TryStartBake();
            else
                TryStartPreheat();
        }
    }

    private void AdjustTemperature(int direction)
    {
        if (isBaking || isPreheating)
        {
            RecipeFeedback.Warning(
                isBaking
                    ? "Temperature is locked while the oven is baking."
                    : "Temperature is locked while the oven is preheating.");
            return;
        }

        if (direction == 0)
            return;

        if (temperatureCelsius <= 0f)
        {
            if (direction < 0)
                return;

            temperatureCelsius = minimumTemperature;
        }
        else
        {
            temperatureCelsius = Mathf.Clamp(
                temperatureCelsius + Mathf.Sign(direction) * temperatureStep,
                minimumTemperature,
                maximumTemperature);
        }

        bakeCompleted = false;
        isPreheated = false;
        preheatElapsed = 0f;
        RecipeFeedback.Report(
            $"Oven target set to {temperatureCelsius:0} C. Press PREHEAT when ready.");
    }

    public bool TryStartPreheat()
    {
        if (isBaking)
        {
            RecipeFeedback.Warning("The oven is already baking.");
            return false;
        }

        if (isPreheating)
        {
            RecipeFeedback.Warning("The oven is already preheating.");
            return false;
        }

        if (isPreheated)
        {
            RecipeFeedback.Report(
                $"The oven is already preheated to {temperatureCelsius:0} C.");
            return false;
        }

        if (temperatureCelsius <= 0f)
        {
            RecipeFeedback.Warning(
                "Set the oven temperature before preheating.");
            return false;
        }

        if (bakeZone != null && bakeZone.HasAnyBakeware)
        {
            RecipeFeedback.Warning(
                "Remove the tray or pan before preheating the oven.");
            return false;
        }

        if (ovenDoor != null && !ovenDoor.IsClosed)
        {
            RecipeFeedback.Warning("Close the oven door before preheating.");
            return false;
        }

        isPreheating = true;
        isPreheated = false;
        pausedForOpenDoor = false;
        bakeCompleted = false;
        lastBakeResult = OvenBakeResult.None;
        preheatElapsed = 0f;
        preheatStartTemperature = currentTemperatureCelsius;
        RecipeFeedback.Report(
            $"Preheating oven to {temperatureCelsius:0} C.");
        return true;
    }

    public bool TryStartBake()
    {
        if (activeRecipe != null && activeRecipe.BatterPracticeOnly)
        {
            RecipeFeedback.Warning("Cupcake batter practice ends before baking. Baking, cooling and frosting are still in development.");
            return false;
        }
        if (isPreheating)
        {
            RecipeFeedback.Warning("Wait for the oven to finish preheating.");
            return false;
        }

        if (!isPreheated)
        {
            RecipeFeedback.Warning("Preheat the oven before baking.");
            return false;
        }

        if (isBaking)
        {
            RecipeFeedback.Warning("The oven is already baking.");
            return false;
        }

        if (bakeZone == null)
        {
            RecipeFeedback.SystemWarning("Oven has no OvenBakeZone assigned.");
            return false;
        }

        if (!bakeZone.HasLoadedBakeware)
        {
            string product = activeRecipe != null
                ? activeRecipe.ProductSingular
                : "cookie";
            RecipeFeedback.Warning($"Load the complete {product} batch first.");
            return false;
        }

        if (bakeZone.ActiveRecipe != null)
            SetActiveRecipe(bakeZone.ActiveRecipe);

        if (ovenDoor != null && !ovenDoor.IsClosed)
        {
            RecipeFeedback.Warning("Close the oven door before starting.");
            return false;
        }

        isBaking = true;
        pausedForOpenDoor = false;
        bakeCompleted = false;
        lastBakeResult = OvenBakeResult.None;
        timeRemaining = bakeDuration;
        lastDisplayedSecond = Mathf.CeilToInt(timeRemaining);

        RecipeFeedback.Report(
            $"Oven started at {temperatureCelsius:0} C " +
            $"for {bakeDuration:0.#} seconds.");
        RecipeFeedback.Report($"Oven: {lastDisplayedSecond} seconds remaining.");
        return true;
    }

    private void CompleteBake()
    {
        BrowniePanReceiver browniePan = bakeZone.BrowniePanInOven;
        CupcakeBatch cupcakes = bakeZone.CupcakeBatchInOven;
        Ingredient[] results =
            bakeZone.TrayInOven != null
                ? bakeZone.TrayInOven.PlacedIngredients
                : System.Array.Empty<Ingredient>();

        if (results.Length == 0 && browniePan == null && cupcakes == null)
        {
            string product = activeRecipe != null
                ? activeRecipe.ProductSingular
                : "cookie";
            CancelBake($"Baking cancelled: no {product} portions were found on the tray.");
            return;
        }

        Color resultColor;
        float resultSmoothness;
        string resultIngredientName;
        string resultMessage;
        bool applyVisualChanges;
        string productPlural = activeRecipe != null
            ? activeRecipe.ProductPlural
            : "cookies";

        if (temperatureCelsius < minimumPerfectTemperature)
        {
            lastBakeResult = OvenBakeResult.Underbaked;
            resultIngredientName = activeRecipe != null
                ? activeRecipe.GetResultIngredientName(lastBakeResult)
                : "UnderbakedCookie";
            resultColor = default;
            resultSmoothness = default;
            applyVisualChanges = false;
            resultMessage = $"Baking complete: the {productPlural} are underbaked.";
        }
        else if (temperatureCelsius >= minimumBurntTemperature)
        {
            lastBakeResult = OvenBakeResult.Burnt;
            resultIngredientName = activeRecipe != null
                ? activeRecipe.GetResultIngredientName(lastBakeResult)
                : "BurntCookie";
            // Pull the color close to black so it reads as dry, crispy char
            // instead of smooth chocolate.
            resultColor = Color.Lerp(Color.black, burntColor, 0.2f);
            resultSmoothness = 0f;
            applyVisualChanges = true;
            resultMessage = $"Baking complete: the {productPlural} are burnt.";
        }
        else if (temperatureCelsius > maximumPerfectTemperature)
        {
            lastBakeResult = OvenBakeResult.Overcooked;
            resultIngredientName = activeRecipe != null
                ? activeRecipe.GetResultIngredientName(lastBakeResult)
                : "OvercookedCookie";
            resultColor = overcookedColor;
            resultSmoothness = overcookedSmoothness;
            applyVisualChanges = true;
            resultMessage = $"Baking complete: the {productPlural} are overcooked.";
        }
        else
        {
            lastBakeResult = OvenBakeResult.Perfect;
            resultIngredientName = activeRecipe != null
                ? activeRecipe.GetResultIngredientName(lastBakeResult)
                : "BakedCookie";
            resultColor = bakedColor;
            resultSmoothness = bakedSmoothness;
            applyVisualChanges = true;
            resultMessage = $"Baking complete: perfectly baked {productPlural}!";
        }

        foreach (Ingredient result in results)
        {
            if (result == null)
                continue;

            result.ingredientName = resultIngredientName;

            foreach (Renderer resultRenderer in
                     result.GetComponentsInChildren<Renderer>())
            {
                if (resultRenderer.GetComponentInParent<ChocolateChipVisual>() != null)
                    continue;

                if (!applyVisualChanges)
                    continue;

                Material resultMaterial = resultRenderer.material;
                if (resultMaterial.HasProperty("_BakeAmount"))
                    resultMaterial.SetFloat("_BakeAmount", 1f);

                if (resultMaterial.HasProperty("_BaseColor"))
                    resultMaterial.SetColor("_BaseColor", resultColor);
                else if (resultMaterial.HasProperty("_Color"))
                    resultMaterial.SetColor("_Color", resultColor);

                if (resultMaterial.HasProperty("_Metallic"))
                    resultMaterial.SetFloat("_Metallic", 0f);

                if (resultMaterial.HasProperty("_Smoothness"))
                    resultMaterial.SetFloat(
                        "_Smoothness",
                        resultSmoothness);
            }
        }

        if (browniePan != null)
        {
            browniePan.ApplyBakeResult(
                lastBakeResult,
                resultIngredientName,
                resultColor,
                resultSmoothness,
                applyVisualChanges);
        }

        if (cupcakes) cupcakes.ApplyBakeResult(lastBakeResult, resultIngredientName, applyVisualChanges ? resultColor : new Color(.94f,.82f,.52f));
        isBaking = false;
        bakeCompleted = true;
        currentTemperatureCelsius = temperatureCelsius;
        timeRemaining = 0f;
        lastDisplayedSecond = 0;

        if (completionAudioSource != null && completionBeep != null)
        {
            completionAudioSource.PlayOneShot(
                completionBeep,
                completionBeepVolume);
        }

        RecipeFeedback.Report(resultMessage);
    }

    public void SetActiveRecipe(RecipeDefinition recipe)
    {
        if (recipe == null)
            return;

        activeRecipe = recipe;
        bakeDuration = Mathf.Max(.1f, recipe.BakeDurationSeconds);
        minimumPerfectTemperature = recipe.MinimumPerfectTemperature;
        maximumPerfectTemperature = recipe.MaximumPerfectTemperature;
        minimumBurntTemperature = recipe.MinimumBurntTemperature;
    }

    private void ConfigureCompletionAudio()
    {
        if (completionAudioSource == null)
            completionAudioSource = GetComponent<AudioSource>();

        if (completionAudioSource == null)
            completionAudioSource = gameObject.AddComponent<AudioSource>();

        completionAudioSource.playOnAwake = false;
        completionAudioSource.loop = false;
        completionAudioSource.spatialBlend = 1f;
        completionAudioSource.minDistance = 1f;
        completionAudioSource.maxDistance = 12f;

        if (completionBeep == null)
            completionBeep = CreateCompletionBeep();
    }

    private AudioClip CreateCompletionBeep()
    {
        const int sampleRate = 44100;
        int beepSamples = Mathf.Max(
            1,
            Mathf.CeilToInt(completionBeepDuration * sampleRate));
        int gapSamples = Mathf.Max(
            0,
            Mathf.CeilToInt(completionBeepGap * sampleRate));
        int beepCount = Mathf.Max(1, completionBeepCount);
        int totalSamples =
            beepCount * beepSamples + (beepCount - 1) * gapSamples;
        float[] samples = new float[totalSamples];

        for (int beepIndex = 0; beepIndex < beepCount; beepIndex++)
        {
            int startSample = beepIndex * (beepSamples + gapSamples);

            for (int sampleIndex = 0;
                 sampleIndex < beepSamples;
                 sampleIndex++)
            {
                float progress = beepSamples > 1
                    ? sampleIndex / (beepSamples - 1f)
                    : 0f;
                float envelope = Mathf.Sin(Mathf.PI * progress);
                float phase =
                    2f * Mathf.PI * completionBeepFrequency *
                    sampleIndex / sampleRate;

                samples[startSample + sampleIndex] =
                    Mathf.Sin(phase) * envelope * 0.45f;
            }
        }

        AudioClip generatedBeep = AudioClip.Create(
            "Generated Oven Completion Beep",
            totalSamples,
            1,
            sampleRate,
            false);
        generatedBeep.SetData(samples, 0);
        return generatedBeep;
    }

    private void CancelBake(string message)
    {
        isBaking = false;
        pausedForOpenDoor = false;
        bakeCompleted = false;
        lastBakeResult = OvenBakeResult.None;
        timeRemaining = 0f;
        lastDisplayedSecond = -1;
        RecipeFeedback.Warning(message);
    }

    private void OnValidate()
    {
        interactionDistance = Mathf.Max(0.1f, interactionDistance);
        bakeDuration = Mathf.Max(0.1f, bakeDuration);
        preheatDuration = Mathf.Max(0.1f, preheatDuration);
        ambientTemperature = Mathf.Clamp(
            ambientTemperature,
            0f,
            maximumTemperature);
        minimumTemperature = Mathf.Max(0f, minimumTemperature);
        temperatureStep = Mathf.Max(1f, temperatureStep);
        maximumTemperature = Mathf.Max(
            minimumTemperature + temperatureStep * 2f,
            maximumTemperature);
        temperatureCelsius = temperatureCelsius <= 0f
            ? 0f
            : Mathf.Clamp(
                temperatureCelsius,
                minimumTemperature,
                maximumTemperature);
        minimumPerfectTemperature = Mathf.Clamp(
            minimumPerfectTemperature,
            minimumTemperature,
            maximumTemperature - temperatureStep * 2f);
        maximumPerfectTemperature = Mathf.Clamp(
            maximumPerfectTemperature,
            minimumPerfectTemperature,
            maximumTemperature - temperatureStep);
        minimumBurntTemperature = Mathf.Clamp(
            minimumBurntTemperature,
            maximumPerfectTemperature + temperatureStep,
            maximumTemperature);
        bakedSmoothness = Mathf.Clamp01(bakedSmoothness);
        overcookedSmoothness = Mathf.Clamp01(overcookedSmoothness);
        burntSmoothness = Mathf.Clamp01(burntSmoothness);
        completionBeepVolume = Mathf.Clamp01(completionBeepVolume);
        completionBeepFrequency = Mathf.Max(
            100f,
            completionBeepFrequency);
        completionBeepCount = Mathf.Max(1, completionBeepCount);
        completionBeepDuration = Mathf.Max(
            0.02f,
            completionBeepDuration);
        completionBeepGap = Mathf.Max(0f, completionBeepGap);
    }
}
