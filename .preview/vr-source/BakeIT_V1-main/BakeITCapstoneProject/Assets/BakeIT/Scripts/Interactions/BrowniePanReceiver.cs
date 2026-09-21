using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
public sealed class BrowniePanReceiver : MonoBehaviour
{
    private const string ParchmentIngredientName = "ParchmentPaper";

    [SerializeField] private RecipeDefinition recipeDefinition;
    [SerializeField] private Rigidbody panBody;
    [SerializeField] private Transform parchmentAnchor;
    [SerializeField] private GameObject parchmentLinerVisual;
    [SerializeField] private GameObject brownieVisual;
    [SerializeField] private Renderer brownieRenderer;
    [SerializeField] private BrownieFoodVisual styledVisual;
    [SerializeField] private Vector3 unspreadVisualScale = new(.18f, .018f, .12f);
    [SerializeField] private Vector3 spreadVisualScale = new(.27f, .012f, .17f);

    [Header("Batter transfer")]
    [SerializeField, Min(.1f)] private float pourHoldSeconds = .4f;

    [Header("Spreading")]
    [SerializeField, Min(.001f)] private float movementPerPass = .055f;
    [SerializeField, Min(.1f)] private float minimumTimeBetweenPasses = 1f;

    private PickupController pickup;
    private Ingredient placedParchment;
    private Rigidbody placedParchmentBody;
    private Collider[] disabledParchmentColliders;
    private Renderer[] hiddenParchmentRenderers;
    private bool[] originalParchmentRendererStates;
    private Transform originalParchmentParent;
    private Vector3 originalParchmentLocalPosition;
    private Quaternion originalParchmentLocalRotation;
    private Vector3 originalParchmentLocalScale;
    private bool originalParchmentUseGravity;
    private bool originalParchmentIsKinematic;
    private bool originalParchmentDetectCollisions;
    private RigidbodyConstraints originalParchmentConstraints;
    private PourableIngredientReceiver pouringSource;
    private float pourTime;
    private BrownieSpreadingTool activeSpatula;
    private Vector3 lastSpatulaPosition;
    private float spreadingTravel;
    private float nextSpreadPassTime;
    private float nextFeedbackTime;
    private int slicePassCount;
    private Transform activeKnife;
    private Vector3 lastKnifePosition;
    private float cutTravel, nextCutTime;
    public int SlicePassCount => slicePassCount;
    public bool IsSliced => slicePassCount>=5;
    [SerializeField, HideInInspector] private int spreadPassCount;
    [SerializeField, HideInInspector] private bool hasBatter;
    [SerializeField, HideInInspector] private bool bakeCompleted;
    [SerializeField, HideInInspector] private OvenBakeResult bakeResult;
    [SerializeField, HideInInspector] private string resultIngredientName = "BrownieBatter";

    public RecipeDefinition ActiveRecipe => recipeDefinition;
    public bool HasParchment => placedParchment != null;
    public bool HasBatter => hasBatter;
    public int SpreadPassCount => spreadPassCount;
    public int RequiredSpreadPasses => recipeDefinition != null
        ? Mathf.Max(1, recipeDefinition.ShapingPasses)
        : 4;
    public bool IsSpread => hasBatter && spreadPassCount >= RequiredSpreadPasses;
    public bool IsReadyForOven => HasParchment && IsSpread && !bakeCompleted;
    public bool BakeCompleted => bakeCompleted;
    public OvenBakeResult BakeResult => bakeResult;
    public string ResultIngredientName => resultIngredientName;

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
        if (panBody == null)
            panBody = GetComponentInParent<Rigidbody>();
        if (parchmentAnchor == null)
            parchmentAnchor = transform;
        if (brownieRenderer == null && brownieVisual != null)
            brownieRenderer = brownieVisual.GetComponentInChildren<Renderer>(true);
        if (styledVisual == null && brownieVisual != null)
            styledVisual = brownieVisual.GetComponent<BrownieFoodVisual>();
        pickup = FindAnyObjectByType<PickupController>();
        if (parchmentLinerVisual != null)
            parchmentLinerVisual.SetActive(false);
        UpdateBrownieVisual();
    }

    private void OnTriggerEnter(Collider other)
    {
        TryBeginSpreading(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryTrackBatterPour(other);
        TrackSpreading(other);
        TrackSlicing(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.attachedRigidbody && activeKnife==other.attachedRigidbody.transform) {activeKnife=null;cutTravel=0;}
        PourableIngredientReceiver source = FindBrownieBowl(other);
        if (source != null && source == pouringSource)
            ResetPour();

        BrownieSpreadingTool spatula = FindSpatula(other);
        if (spatula != null && spatula == activeSpatula)
            ResetSpreading();
    }

    public bool TryPlaceReleased(Rigidbody body)
    {
        if (body == null || HasParchment)
            return false;

        Ingredient ingredient = body.GetComponentInChildren<Ingredient>(true);
        if (ingredient == null || !string.Equals(
                ingredient.ingredientName,
                ParchmentIngredientName,
                StringComparison.OrdinalIgnoreCase))
            return false;

        Collider bodyCollider = body.GetComponent<Collider>();
        if (bodyCollider == null)
            return false;

        Bounds acceptance = GetComponent<BoxCollider>().bounds;
        acceptance.Expand(new Vector3(.18f, .45f, .18f));
        if (!acceptance.Intersects(bodyCollider.bounds))
            return false;

        if (pickup != null)
            pickup.ReleaseForPlacement(body);

        placedParchment = ingredient;
        placedParchmentBody = body;
        originalParchmentParent = body.transform.parent;
        originalParchmentLocalPosition = body.transform.localPosition;
        originalParchmentLocalRotation = body.transform.localRotation;
        originalParchmentLocalScale = body.transform.localScale;
        originalParchmentUseGravity = body.useGravity;
        originalParchmentIsKinematic = body.isKinematic;
        originalParchmentDetectCollisions = body.detectCollisions;
        originalParchmentConstraints = body.constraints;
        if (!body.isKinematic)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
        body.useGravity = false;
        body.isKinematic = true;
        body.detectCollisions = false;
        body.constraints = RigidbodyConstraints.FreezeAll;
        body.transform.SetParent(parchmentAnchor, false);
        body.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        Vector3 parentScale = parchmentAnchor.lossyScale;
        body.transform.localScale = new Vector3(
            SafeDivide(.48f, parentScale.x),
            SafeDivide(.002f, parentScale.y),
            SafeDivide(.30f, parentScale.z));

        disabledParchmentColliders =
            ingredient.GetComponentsInChildren<Collider>(true);
        foreach (Collider parchmentCollider in disabledParchmentColliders)
            parchmentCollider.enabled = false;

        hiddenParchmentRenderers =
            ingredient.GetComponentsInChildren<Renderer>(true);
        originalParchmentRendererStates = new bool[hiddenParchmentRenderers.Length];
        for (int index = 0; index < hiddenParchmentRenderers.Length; index++)
        {
            originalParchmentRendererStates[index] = hiddenParchmentRenderers[index].enabled;
            hiddenParchmentRenderers[index].enabled = false;
        }
        if (parchmentLinerVisual != null)
            parchmentLinerVisual.SetActive(true);

        RecipeFeedback.Report("Parchment paper placed in the deep brownie pan.");
        return true;
    }

    public void ApplyBakeResult(
        OvenBakeResult result,
        string ingredientName,
        Color resultColor,
        float smoothness,
        bool applyVisualChanges)
    {
        if (!IsSpread)
            return;

        bakeCompleted = true;
        bakeResult = result;
        resultIngredientName = string.IsNullOrWhiteSpace(ingredientName)
            ? "Brownie"
            : ingredientName;

        if (!applyVisualChanges || brownieRenderer == null)
            return;

        if (styledVisual != null)
        {
            styledVisual.ShowBaked(result, resultColor, smoothness);
            Vector3 styledScale = brownieVisual.transform.localScale;
            styledScale.y = spreadVisualScale.y * 1.18f;
            brownieVisual.transform.localScale = styledScale;
            brownieRenderer = styledVisual.SurfaceRenderer;
            return;
        }

        Material material = brownieRenderer.material;
        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", resultColor);
        else if (material.HasProperty("_Color"))
            material.SetColor("_Color", resultColor);
        if (material.HasProperty("_Metallic"))
            material.SetFloat("_Metallic", 0f);
        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", smoothness);

        Vector3 scale = brownieVisual.transform.localScale;
        scale.y = spreadVisualScale.y * 1.18f;
        brownieVisual.transform.localScale = scale;
    }

    public void ResetReceiver()
    {
        if (disabledParchmentColliders != null)
            foreach (Collider parchmentCollider in disabledParchmentColliders)
                if (parchmentCollider != null)
                    parchmentCollider.enabled = true;

        if (hiddenParchmentRenderers != null && originalParchmentRendererStates != null)
            for (int index = 0; index < hiddenParchmentRenderers.Length; index++)
                if (hiddenParchmentRenderers[index] != null &&
                    index < originalParchmentRendererStates.Length)
                    hiddenParchmentRenderers[index].enabled =
                        originalParchmentRendererStates[index];

        if (placedParchmentBody != null)
        {
            placedParchmentBody.transform.SetParent(originalParchmentParent, false);
            placedParchmentBody.transform.SetLocalPositionAndRotation(
                originalParchmentLocalPosition, originalParchmentLocalRotation);
            placedParchmentBody.transform.localScale = originalParchmentLocalScale;
            placedParchmentBody.isKinematic = originalParchmentIsKinematic;
            placedParchmentBody.useGravity = originalParchmentUseGravity;
            placedParchmentBody.detectCollisions = originalParchmentDetectCollisions;
            placedParchmentBody.constraints = originalParchmentConstraints;
        }
        if (parchmentLinerVisual != null)
            parchmentLinerVisual.SetActive(false);

        placedParchment = null;
        placedParchmentBody = null;
        disabledParchmentColliders = null;
        hiddenParchmentRenderers = null;
        originalParchmentRendererStates = null;
        hasBatter = false;
        spreadPassCount = 0;
        bakeCompleted = false;
        slicePassCount=0;activeKnife=null;cutTravel=0;
        if(styledVisual) styledVisual.ResetCuts();
        bakeResult = OvenBakeResult.None;
        resultIngredientName = "BrownieBatter";
        if (styledVisual != null)
            styledVisual.ShowRaw();
        ResetPour();
        ResetSpreading();
        UpdateBrownieVisual();
    }

    private void TryTrackBatterPour(Collider other)
    {
        if (hasBatter || !HasParchment)
            return;

        PourableIngredientReceiver source = FindBrownieBowl(other);
        if (source == null || !source.CanPourCompletedBatter)
        {
            if (source != null && source == pouringSource)
                ResetPour();
            return;
        }

        if (source != pouringSource)
        {
            pouringSource = source;
            pourTime = 0f;
        }

        pourTime += Time.deltaTime;
        if (pourTime < pourHoldSeconds)
            return;

        if (!TryReceiveCompletedBatter(source))
        {
            ResetPour();
            return;
        }
        ResetPour();
    }

    public bool TryReceiveCompletedBatter(PourableIngredientReceiver source)
    {
        if (source == null || hasBatter || !HasParchment ||
            !source.CanPourCompletedBatter ||
            !source.TryTransferCompletedBatter(transform.position))
            return false;

        hasBatter = true;
        resultIngredientName = recipeDefinition != null
            ? recipeDefinition.MixedProductIngredientName
            : "BrownieBatter";
        spreadPassCount = 0;
        UpdateBrownieVisual();
        RecipeFeedback.Report(
            "Brownie batter poured into the lined pan. Spread it evenly with the Rubber Spatula.");
        return true;
    }

    private void TryBeginSpreading(Collider other)
    {
        BrownieSpreadingTool spatula = FindSpatula(other);
        if (spatula == null)
            return;
        activeSpatula = spatula;
        lastSpatulaPosition = transform.InverseTransformPoint(spatula.transform.position);
        spreadingTravel = 0f;
    }

    private void TrackSpreading(Collider other)
    {
        BrownieSpreadingTool spatula = FindSpatula(other);
        if (spatula == null)
            return;

        if (!hasBatter)
        {
            Warn("Pour the brownie batter into the lined pan before spreading.");
            return;
        }

        if (IsSpread || IsPanHeld() || !IsSpatulaHeld(spatula))
            return;

        if (activeSpatula != spatula)
        {
            TryBeginSpreading(other);
            return;
        }

        Vector3 current = transform.InverseTransformPoint(spatula.transform.position);
        Vector3 delta = current - lastSpatulaPosition;
        lastSpatulaPosition = current;
        delta.y = 0f;
        float travelled = delta.magnitude;
        if (travelled > movementPerPass * 5f)
            return;

        if (Time.time < nextSpreadPassTime)
        {
            spreadingTravel = 0f;
            return;
        }

        spreadingTravel += travelled;
        if (spreadingTravel < movementPerPass)
            return;

        spreadingTravel = 0f;
        nextSpreadPassTime = Time.time + minimumTimeBetweenPasses;
        spreadPassCount = Mathf.Min(spreadPassCount + 1, RequiredSpreadPasses);
        UpdateBrownieVisual();
        RecipeFeedback.Report(
            $"Spreading brownie batter: {spreadPassCount} of {RequiredSpreadPasses} passes.");
        if (IsSpread)
            RecipeFeedback.Report("Brownie batter spread evenly. Preheat the oven.");
    }

    private void UpdateBrownieVisual()
    {
        if (brownieVisual == null)
            return;

        brownieVisual.SetActive(hasBatter);
        if (!hasBatter)
            return;

        float progress = RequiredSpreadPasses > 0
            ? (float)spreadPassCount / RequiredSpreadPasses
            : 1f;
        brownieVisual.transform.localScale = Vector3.Lerp(
            unspreadVisualScale,
            spreadVisualScale,
            Mathf.Clamp01(progress));
        if(styledVisual && !bakeCompleted) styledVisual.SetSpreadProgress(progress);
    }

    private void TrackSlicing(Collider other)
    {
        Rigidbody body=other.attachedRigidbody;
        var oven=FindAnyObjectByType<OvenBakeZone>();
        if(!bakeCompleted || IsSliced || IsPanHeld() || (oven && oven.HasLoadedBrowniePan) ||
            !body || !body.GetComponent<DoughPortioningTool>() || !pickup || pickup.HeldObject!=body) return;
        Vector3 current=transform.InverseTransformPoint(body.position);
        if(activeKnife!=body.transform){activeKnife=body.transform;lastKnifePosition=current;cutTravel=0;return;}
        Vector3 delta=current-lastKnifePosition;lastKnifePosition=current;delta.y=0;
        if(Time.time<nextCutTime || delta.magnitude>.25f) {cutTravel=0;return;}
        cutTravel+=delta.magnitude;
        if(cutTravel<.12f)return;
        cutTravel=0;nextCutTime=Time.time+1f;slicePassCount++;
        if(styledVisual)styledVisual.ShowCuts(slicePassCount);
        RecipeFeedback.Report(IsSliced?"Brownies cut into 12 squares.":$"Knife cuts: {slicePassCount} of 5.");
    }

    private bool IsPanHeld() => pickup != null && pickup.HeldObject == panBody;

    private bool IsSpatulaHeld(BrownieSpreadingTool spatula)
    {
        if (pickup == null || pickup.HeldObject == null || spatula == null)
            return false;
        Rigidbody spatulaBody = spatula.GetComponent<Rigidbody>();
        return spatulaBody != null && pickup.HeldObject == spatulaBody;
    }

    private static PourableIngredientReceiver FindBrownieBowl(Collider other)
    {
        if (other == null)
            return null;
        if (other.attachedRigidbody != null)
            return other.attachedRigidbody.GetComponentInChildren<PourableIngredientReceiver>(true);
        return other.GetComponentInParent<PourableIngredientReceiver>();
    }

    private static BrownieSpreadingTool FindSpatula(Collider other)
    {
        if (other == null)
            return null;
        if (other.attachedRigidbody != null)
            return other.attachedRigidbody.GetComponent<BrownieSpreadingTool>();
        return other.GetComponentInParent<BrownieSpreadingTool>();
    }

    private void ResetPour()
    {
        pouringSource = null;
        pourTime = 0f;
    }

    private void ResetSpreading()
    {
        activeSpatula = null;
        spreadingTravel = 0f;
    }

    private void Warn(string message)
    {
        if (Time.unscaledTime < nextFeedbackTime)
            return;
        nextFeedbackTime = Time.unscaledTime + 2f;
        RecipeFeedback.Warning(message);
    }

    private static float SafeDivide(float value, float divisor) =>
        Mathf.Abs(divisor) > Mathf.Epsilon ? value / divisor : value;

    private void OnValidate()
    {
        BoxCollider zone = GetComponent<BoxCollider>();
        if (zone != null)
            zone.isTrigger = true;
        pourHoldSeconds = Mathf.Max(.1f, pourHoldSeconds);
        movementPerPass = Mathf.Max(.001f, movementPerPass);
        minimumTimeBetweenPasses = Mathf.Max(.1f, minimumTimeBetweenPasses);
    }
}
