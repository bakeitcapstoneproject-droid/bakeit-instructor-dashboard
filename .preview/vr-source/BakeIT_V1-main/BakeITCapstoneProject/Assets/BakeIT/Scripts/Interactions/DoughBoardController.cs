using UnityEngine;

public class DoughBoardController : MonoBehaviour
{
    private const string FallbackUnportionedDoughName = "Dough";
    private const string FallbackFlattenedDoughName = "FlattenedDough";
    private const string FallbackPortionName = "CookieDoughPortion";

    [SerializeField] private Transform placementPoint;
    [SerializeField] private int requiredRollingPasses = 4;
    [SerializeField] private float distancePerPass = 0.2f;
    [SerializeField, Min(0f)]
    private float minimumTimeBetweenRollingPasses = 1.2f;
    [SerializeField] private Vector3 flattenedScaleMultiplier =
        new Vector3(1.4f, 0.4f, 1.4f);
    [SerializeField] private float flattenAnimationSpeed = 8f;

    [Header("Cookie Portioning")]
    [Tooltip("Rendered world-space volume of the reference batch (unchanged by decorative chips).")]
    [SerializeField, Min(0.000001f)]
    private float referenceFlattenedDoughVolume = 0.00255f;
    [SerializeField, Min(1)] private int referenceCookieYield = 6;
    [SerializeField, Min(1)] private int minimumCookieYield = 6;
    [SerializeField, Min(1)] private int maximumCookieYield = 6;
    [SerializeField, Min(0.01f)] private float portionSpacing = 0.14f;
    [SerializeField, Min(0f)] private float portionHeightOffset = 0.012f;

    private PickupController pickupController;
    private Ingredient dough;
    private Ingredient preparedDough;
    private Rigidbody doughBody;
    private RollingPinTool activeRollingPin;
    private Vector3 lastRollingPinPosition;
    private Vector3 unrolledLocalScale;
    private Vector3 targetLocalScale;
    private float rollingDistance;
    private float nextRollingPassTime;
    private int rollingPasses;
    private MiseEnPlaceStation preparation;
    private float nextClearBoardWarning;
    private RecipeDefinition activeRecipe;

    private string UnportionedDoughName => activeRecipe != null
        ? activeRecipe.MixedProductIngredientName
        : FallbackUnportionedDoughName;
    private string FlattenedDoughName => activeRecipe != null
        ? activeRecipe.ShapedProductIngredientName
        : FallbackFlattenedDoughName;
    private string PortionIngredientName => activeRecipe != null
        ? activeRecipe.PortionIngredientName
        : FallbackPortionName;
    private string ProductSingular => activeRecipe != null
        ? activeRecipe.ProductSingular
        : "cookie";

    public int LastPortionCount { get; private set; }
    public bool HasDough =>
        dough != null && dough.transform.IsChildOf(GetBoardTransform());
    public bool IsDoughFlattened =>
        HasDough && dough.ingredientName == FlattenedDoughName;
    public int RollingPassCount => rollingPasses;
    public int RequiredRollingPasses => requiredRollingPasses;
    public int TargetCookieCount => referenceCookieYield;
    public RecipeDefinition ActiveRecipe => activeRecipe;

    private void Update()
    {
        if (doughBody == null || SmoothPlacement.Settling(doughBody))
            return;

        Transform board = GetBoardTransform();

        // The target scale below is local to the preparation board.
        // Stop applying it as soon as the dough moves to another object
        // (for example, the differently-scaled tray).
        if (!doughBody.transform.IsChildOf(board))
        {
            ClearDough(preservePreparation: true);
            return;
        }

        float blend = 1f - Mathf.Exp(
            -flattenAnimationSpeed * Time.deltaTime);

        doughBody.transform.localScale = Vector3.Lerp(
            doughBody.transform.localScale,
            targetLocalScale,
            blend);

        if (Vector3.SqrMagnitude(
                doughBody.transform.localScale - targetLocalScale) < 0.000001f)
        {
            doughBody.transform.localScale = targetLocalScale;
        }
    }

    private void Awake()
    {
        activeRecipe = FindAnyObjectByType<BowlReceiver>()?.ActiveRecipe;
        if (activeRecipe != null)
        {
            requiredRollingPasses = Mathf.Max(1, activeRecipe.ShapingPasses);
            referenceCookieYield = activeRecipe.TargetPortionCount;
            minimumCookieYield = activeRecipe.MinimumPortionCount;
            maximumCookieYield = activeRecipe.MaximumPortionCount;
        }

        preparation = FindAnyObjectByType<MiseEnPlaceStation>();
        if (placementPoint == null)
            placementPoint = transform;

        pickupController =
            FindAnyObjectByType<PickupController>();

        requiredRollingPasses = Mathf.Max(1, requiredRollingPasses);
        distancePerPass = Mathf.Max(0.01f, distancePerPass);
        minimumTimeBetweenRollingPasses = Mathf.Max(
            0f,
            minimumTimeBetweenRollingPasses);
        flattenAnimationSpeed = Mathf.Max(0.1f, flattenAnimationSpeed);
        referenceFlattenedDoughVolume = Mathf.Max(
            0.000001f,
            referenceFlattenedDoughVolume);
        referenceCookieYield = Mathf.Max(1, referenceCookieYield);
        minimumCookieYield = Mathf.Max(1, minimumCookieYield);
        maximumCookieYield = Mathf.Max(
            minimumCookieYield,
            maximumCookieYield);
        portionSpacing = Mathf.Max(0.01f, portionSpacing);
        portionHeightOffset = Mathf.Max(0f, portionHeightOffset);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryPlaceDough(other, allowHeldPlacement: false);

        TryPortionDough(other);

        RollingPinTool rollingPin =
            other.GetComponentInParent<RollingPinTool>();

        if (rollingPin == null)
            return;

        activeRollingPin = rollingPin;
        lastRollingPinPosition = rollingPin.transform.position;
    }

    private void OnTriggerStay(Collider other)
    {
        // A released slab can still overlap the zone without producing
        // another enter event. Do not snatch it back during pickup.
        TryPlaceDough(other, allowHeldPlacement: false);

        RollingPinTool rollingPin =
            other.GetComponentInParent<RollingPinTool>();

        if (rollingPin == null ||
            rollingPin != activeRollingPin ||
            !HasDough || SmoothPlacement.Settling(doughBody) ||
            dough.ingredientName != UnportionedDoughName)
            return;

        Vector3 movement =
            rollingPin.transform.position - lastRollingPinPosition;

        lastRollingPinPosition = rollingPin.transform.position;

        if (Time.time < nextRollingPassTime)
        {
            // Movement during the cooldown belongs to the roll that just
            // counted. Require a fresh stroke for the next pass.
            rollingDistance = 0f;
            return;
        }

        rollingDistance += Vector3.ProjectOnPlane(
            movement,
            transform.up).magnitude;

        if (rollingDistance < distancePerPass)
            return;

        rollingDistance = 0f;
        nextRollingPassTime =
            Time.time + minimumTimeBetweenRollingPasses;
        RegisterRollingPass();
    }

    private void OnTriggerExit(Collider other)
    {
        RollingPinTool rollingPin =
            other.GetComponentInParent<RollingPinTool>();

        if (rollingPin == activeRollingPin)
            activeRollingPin = null;

        Ingredient exitingIngredient =
            other.GetComponentInParent<Ingredient>();

        if (exitingIngredient == null || dough == null)
            return;

        Transform board =
            transform.parent != null ? transform.parent : transform;

        if (exitingIngredient == dough &&
            !exitingIngredient.transform.IsChildOf(board))
        {
            ClearDough(preservePreparation: true);
        }
    }

    private void ClearDough(bool preservePreparation = false)
    {
        dough = null;
        doughBody = null;
        activeRollingPin = null;
        if (!preservePreparation)
        {
            preparedDough = null;
            rollingPasses = 0;
            rollingDistance = 0f;
        }
    }

    private void TryPlaceDough(Collider other, bool allowHeldPlacement = true)
    {
        Bounds nearby = GetComponent<Collider>().bounds;
        nearby.Expand(new Vector3(.06f,.44f,.06f));
        if (!nearby.Intersects(other.bounds)) return;
        if (dough != null && !HasDough)
            ClearDough(preservePreparation: true);

        if (dough != null)
            return;

        Ingredient ingredient =
            other.GetComponentInParent<Ingredient>();

        if (ingredient == null ||
            (ingredient.ingredientName != UnportionedDoughName &&
             ingredient.ingredientName != FlattenedDoughName))
            return;

        if (preparation != null && preparation.SuppliesOnBoard)
        {
            if (Time.unscaledTime >= nextClearBoardWarning)
            {
                nextClearBoardWarning = Time.unscaledTime + 2f;
                RecipeFeedback.Warning("Move the ingredient containers and tools beside the board before placing dough.");
            }
            return;
        }

        Rigidbody ingredientBody =
            ingredient.GetComponent<Rigidbody>();

        if (ingredientBody == null)
            return;

        if (!allowHeldPlacement && pickupController != null &&
            pickupController.HeldObject == ingredientBody)
            return;

        if (SmoothPlacement.Settling(ingredientBody)) return;
        Quaternion restingRotation = Quaternion.Euler(0, ingredientBody.transform.eulerAngles.y, 0);
        if (!PlacementGeometry.TryGetPose(ingredientBody, GetBoardTransform().GetComponent<BoxCollider>(),
                restingRotation, out var restingPosition, out restingRotation)) return;

        bool resumePreparation = ingredient == preparedDough;
        dough = ingredient;
        doughBody = ingredientBody;

        if (pickupController != null)
            pickupController.ReleaseForPlacement(doughBody);

        StopDynamicMotion(doughBody);
        doughBody.useGravity = false;
        doughBody.isKinematic = true;

        Transform board = GetBoardTransform();
        Vector3 worldScale = doughBody.transform.lossyScale;

        doughBody.transform.SetParent(board, true);

        if (resumePreparation)
        {
            // Restore the saved board-space target after held rotation
            // and reparenting. Never flatten the already-flattened shape
            // again or reset the passes completed before pickup.
            doughBody.transform.localScale = targetLocalScale;
        }
        else
        {
            doughBody.transform.localScale = DivideScale(
                worldScale,
                board.lossyScale);
            unrolledLocalScale = doughBody.transform.localScale;
            targetLocalScale = unrolledLocalScale;
            rollingPasses = ingredient.ingredientName == FlattenedDoughName
                ? requiredRollingPasses
                : 0;
            rollingDistance = 0f;
            nextRollingPassTime = 0f;
            preparedDough = ingredient;
        }

        SmoothPlacement.Begin(doughBody, restingPosition, restingRotation, board);

        RecipeFeedback.Report(IsDoughFlattened
            ? "Flattened dough returned to the board. Use the knife to divide it into cookie portions."
            : resumePreparation
                ? $"Dough returned to the board. Rolling passes: {rollingPasses}/{requiredRollingPasses}."
                : "Dough placed on preparation board.");
    }

    private void RegisterRollingPass()
    {
        if (!HasDough || SmoothPlacement.Settling(doughBody)) return;
        rollingPasses++;

        float flattenProgress =
            (float)rollingPasses / requiredRollingPasses;

        Vector3 completelyFlattenedScale = Vector3.Scale(
            unrolledLocalScale,
            flattenedScaleMultiplier);

        targetLocalScale = Vector3.Lerp(
            unrolledLocalScale,
            completelyFlattenedScale,
            flattenProgress);

        RecipeFeedback.Report(
            $"Rolling dough: {rollingPasses}/{requiredRollingPasses}");

        if (rollingPasses < requiredRollingPasses)
            return;

        doughBody.transform.localScale = targetLocalScale;
        dough.ingredientName = FlattenedDoughName;

        RecipeFeedback.Report("Dough flattened. Use the knife to divide it into cookie portions.");
    }

    private void TryPortionDough(Collider other)
    {
        DoughPortioningTool portioningTool =
            other.GetComponentInParent<DoughPortioningTool>();

        if (portioningTool == null)
            return;

        if (!HasDough || SmoothPlacement.Settling(doughBody))
        {
            RecipeFeedback.Warning("Place dough on the preparation board before portioning it.");
            return;
        }

        if (dough.ingredientName != FlattenedDoughName)
        {
            RecipeFeedback.Warning("Flatten the dough completely before portioning it.");
            return;
        }

        CreateCookiePortions();
    }

    private void CreateCookiePortions()
    {
        Ingredient sourceIngredient = dough;
        Rigidbody sourceBody = doughBody;
        Transform board =
            transform.parent != null ? transform.parent : transform;
        Vector3 sourceLocalScale = sourceBody.transform.localScale;
        float sourceVolume = CalculateRenderedWorldVolume(
            sourceIngredient.gameObject);
        int portionCount = CalculatePortionCount(sourceVolume);
        float planarScale = Mathf.Sqrt(1f / portionCount);
        Vector3 portionLocalScale = new Vector3(
            sourceLocalScale.x * planarScale,
            sourceLocalScale.y,
            sourceLocalScale.z * planarScale);
        string batchId = System.Guid.NewGuid().ToString("N");

        for (int portionIndex = 0;
             portionIndex < portionCount;
             portionIndex++)
        {
            GameObject portionObject = portionIndex == 0
                ? sourceIngredient.gameObject
                : Instantiate(sourceIngredient.gameObject, board);

            portionObject.name =
                $"CookieDoughPortion_{portionIndex + 1:00}";

            Ingredient portionIngredient =
                portionObject.GetComponent<Ingredient>();
            portionIngredient.ingredientName = PortionIngredientName;

            DoughPortion portion =
                portionObject.GetComponent<DoughPortion>();

            if (portion == null)
                portion = portionObject.AddComponent<DoughPortion>();

            portion.Configure(
                batchId,
                portionIndex,
                portionCount,
                sourceVolume);

            Rigidbody portionBody =
                portionObject.GetComponent<Rigidbody>();

            StopDynamicMotion(portionBody);
            portionBody.useGravity = false;
            portionBody.isKinematic = true;
            portionBody.detectCollisions = true;
            portionBody.transform.SetParent(board, true);
            portionBody.transform.localScale = portionLocalScale;
            portionObject.GetComponent<CookieAppearance>()?.ConfigurePortion(portionIndex);
            if (portionObject.GetComponent<PreparationItem>() == null) portionObject.AddComponent<PreparationItem>();
            SmoothPlacement.Begin(portionBody,
                GetBoardPortionPosition(
                    portionIndex,
                    portionCount,
                    board),
                placementPoint.rotation, board);

            foreach (MeshCollider meshCollider in
                     portionObject.GetComponentsInChildren<MeshCollider>(true))
            {
                meshCollider.convex = true;
            }
        }

        LastPortionCount = portionCount;
        ClearDough();

        RecipeFeedback.Report(
            $"Dough divided into {portionCount} equal {ProductSingular} portions.");
    }

    private int CalculatePortionCount(float sourceVolume)
    {
        float volumeRatio = sourceVolume > Mathf.Epsilon
            ? sourceVolume / referenceFlattenedDoughVolume
            : 1f;

        return Mathf.Clamp(
            Mathf.RoundToInt(referenceCookieYield * volumeRatio),
            minimumCookieYield,
            maximumCookieYield);
    }

    public bool TryPlaceReleased(Rigidbody body)
    {
        if (body == null) return false;
        var collider = body.GetComponent<Collider>();
        if (collider == null) return false;
        TryPlaceDough(collider);
        return doughBody == body && HasDough;
    }

    private Vector3 GetBoardPortionPosition(
        int portionIndex,
        int portionCount,
        Transform board)
    {
        int columns = Mathf.CeilToInt(Mathf.Sqrt(portionCount));
        int rows = Mathf.CeilToInt((float)portionCount / columns);
        int row = portionIndex / columns;
        int column = portionIndex % columns;
        int itemsInRow = Mathf.Min(
            columns,
            portionCount - row * columns);
        float horizontalOffset =
            (column - (itemsInRow - 1) * 0.5f) * portionSpacing;
        float depthOffset =
            (row - (rows - 1) * 0.5f) * portionSpacing;

        Vector3 position = placementPoint.position +
               board.right * horizontalOffset +
               board.forward * depthOffset;
        position.y = board.GetComponent<BoxCollider>().bounds.max.y + portionHeightOffset;
        return position;
    }

    private static float CalculateRenderedWorldVolume(GameObject target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>(true);
        Bounds bounds = new Bounds();
        bool foundRenderer = false;

        foreach (Renderer targetRenderer in renderers)
        {
            // Decorative chips must not change the quantity of dough.
            if (targetRenderer.GetComponentInParent<ChocolateChipVisual>() != null)
                continue;

            if (!foundRenderer)
            {
                bounds = targetRenderer.bounds;
                foundRenderer = true;
            }
            else
            {
                bounds.Encapsulate(targetRenderer.bounds);
            }
        }

        if (!foundRenderer)
            return 0f;

        Vector3 size = bounds.size;
        return Mathf.Abs(size.x * size.y * size.z);
    }

    private static void StopDynamicMotion(Rigidbody body)
    {
        if (body == null || body.isKinematic)
            return;

        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
    }

    private Transform GetBoardTransform()
    {
        return transform.parent != null ? transform.parent : transform;
    }

    private static Vector3 DivideScale(Vector3 scale, Vector3 parentScale)
    {
        return new Vector3(
            Mathf.Abs(parentScale.x) > Mathf.Epsilon
                ? scale.x / parentScale.x : scale.x,
            Mathf.Abs(parentScale.y) > Mathf.Epsilon
                ? scale.y / parentScale.y : scale.y,
            Mathf.Abs(parentScale.z) > Mathf.Epsilon
                ? scale.z / parentScale.z : scale.z);
    }

    private void OnValidate()
    {
        requiredRollingPasses = Mathf.Max(1, requiredRollingPasses);
        distancePerPass = Mathf.Max(0.01f, distancePerPass);
        flattenAnimationSpeed = Mathf.Max(0.1f, flattenAnimationSpeed);
        referenceFlattenedDoughVolume = Mathf.Max(
            0.000001f,
            referenceFlattenedDoughVolume);
        referenceCookieYield = Mathf.Max(1, referenceCookieYield);
        minimumCookieYield = Mathf.Max(1, minimumCookieYield);
        maximumCookieYield = Mathf.Max(
            minimumCookieYield,
            maximumCookieYield);
        portionSpacing = Mathf.Max(0.01f, portionSpacing);
        portionHeightOffset = Mathf.Max(0f, portionHeightOffset);
    }
}
