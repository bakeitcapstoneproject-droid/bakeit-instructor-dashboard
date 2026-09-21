using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class TrayReceiver : MonoBehaviour
{
    private const string FallbackPortionName = "CookieDoughPortion";
    private const string FallbackFlattenedDoughName = "FlattenedDough";
    private const string ParchmentIngredientName = "ParchmentPaper";

    [SerializeField] private Transform placementPoint;
    [SerializeField] private Vector2 portionSpacing =
        new Vector2(0.17f, 0.13f);

    private readonly List<DoughPortion> placedPortions = new();
    private readonly Dictionary<DoughPortion, Rigidbody> portionBodies = new();
    private readonly Dictionary<DoughPortion, Collider[]> portionColliders =
        new();
    private PickupController pickupController;
    private TrayLinerReceiver linerReceiver;
    private Collider[] trayColliders;
    private string activeBatchId;
    private int expectedPortionCount;
    private float nextContactTry;
    private RecipeDefinition activeRecipe;

    private string PortionIngredientName => activeRecipe != null
        ? activeRecipe.PortionIngredientName
        : FallbackPortionName;
    private string FlattenedDoughName => activeRecipe != null
        ? activeRecipe.ShapedProductIngredientName
        : FallbackFlattenedDoughName;
    private string ProductSingular => activeRecipe != null
        ? activeRecipe.ProductSingular
        : "cookie";
    private string ProductPlural => activeRecipe != null
        ? activeRecipe.ProductPlural
        : "cookies";

    public bool HasFlattenedDough => HasCompleteCookieBatch;
    public bool HasPortionedDough => placedPortions.Count > 0;
    public bool HasCompleteCookieBatch =>
        expectedPortionCount > 0 &&
        placedPortions.Count == expectedPortionCount && linerReceiver != null && linerReceiver.HasParchment && AllPortionsSettled();
    public int PlacedPortionCount => placedPortions.Count;
    public int ExpectedPortionCount => expectedPortionCount;
    public Ingredient PlacedIngredient =>
        placedPortions.Count > 0 && placedPortions[0] != null
            ? placedPortions[0].GetComponent<Ingredient>()
            : null;
    public RecipeDefinition ActiveRecipe => activeRecipe;

    public Ingredient[] PlacedIngredients
    {
        get
        {
            List<Ingredient> ingredients = new();

            foreach (DoughPortion portion in placedPortions)
            {
                if (portion == null)
                    continue;

                Ingredient ingredient = portion.GetComponent<Ingredient>();

                if (ingredient != null)
                    ingredients.Add(ingredient);
            }

            return ingredients.ToArray();
        }
    }

    private void Awake()
    {
        activeRecipe = FindAnyObjectByType<BowlReceiver>()?.ActiveRecipe;

        if (placementPoint == null)
            placementPoint = transform;

        portionSpacing.x = Mathf.Max(0.01f, portionSpacing.x);
        portionSpacing.y = Mathf.Max(0.01f, portionSpacing.y);
        pickupController = FindAnyObjectByType<PickupController>();

        Transform tray = GetTrayTransform();
        linerReceiver = tray.GetComponentInChildren<TrayLinerReceiver>(true);
        trayColliders = GetTrayOnlyColliders(tray);
    }

    private void Update()
    {
        Transform tray = GetTrayTransform();

        for (int index = placedPortions.Count - 1; index >= 0; index--)
        {
            DoughPortion portion = placedPortions[index];

            if (portion == null || !portion.transform.IsChildOf(tray))
                RemovePortionAt(index);
        }
    }

    private void OnTriggerEnter(Collider other) => TryReleasedContact(other);
    private void OnTriggerStay(Collider other) => TryReleasedContact(other);
    private void TryReleasedContact(Collider other)
    {
        var body = other.attachedRigidbody;
        if (body == null || (pickupController != null && pickupController.HeldObject == body)) return;
        if (Time.time < nextContactTry) return;
        nextContactTry = Time.time + .35f;
        TryPlace(other);
    }
    public bool TryPlaceReleased(Rigidbody body)
    {
        if (body == null || body.GetComponent<Collider>() == null) return false;
        return TryPlace(body.GetComponent<Collider>());
    }

    private bool TryPlace(Collider other)
    {
        Bounds nearby = GetComponent<Collider>().bounds;
        nearby.Expand(new Vector3(.06f,.44f,.06f));
        if (!nearby.Intersects(other.bounds)) return false;
        Ingredient ingredient = other.GetComponentInParent<Ingredient>();

        if (ingredient == null)
            return false;

        if (IsIngredient(ingredient.ingredientName, ParchmentIngredientName))
            return false;

        if (linerReceiver != null && !linerReceiver.HasParchment)
        {
            RecipeFeedback.Warning("Place parchment paper on the tray first.");
            return false;
        }

        if (IsIngredient(ingredient.ingredientName, FlattenedDoughName))
        {
            RecipeFeedback.Warning("Divide the flattened dough into cookie portions first.");
            return false;
        }

        DoughPortion portion = ingredient.GetComponent<DoughPortion>();

        if (portion == null || !IsAcceptedPortionName(ingredient.ingredientName))
        {
            RecipeFeedback.Warning(
                $"Tray rejected {ingredient.ingredientName}. " +
                $"Only portioned {ProductSingular} dough belongs on this tray.");
            return false;
        }

        if (placedPortions.Contains(portion))
            return false;

        if (!CanAcceptBatch(portion))
            return false;

        Rigidbody ingredientBody =
            ingredient.GetComponentInParent<Rigidbody>();

        if (ingredientBody == null)
            return false;

        if (SmoothPlacement.Settling(ingredientBody)) return false;
        var surface = linerReceiver != null ? linerReceiver.PlacementSurface : GetTrayTransform().GetComponent<BoxCollider>();
        Quaternion restingRotation = Quaternion.Euler(0, ingredientBody.transform.eulerAngles.y, 0);
        if (!PlacementGeometry.TryGetPose(ingredientBody, surface, restingRotation, out var restingPosition, out restingRotation)) return false;

        if (pickupController != null)
            pickupController.ReleaseForPlacement(ingredientBody);

        StopDynamicMotion(ingredientBody);
        ingredientBody.useGravity = false;
        ingredientBody.isKinematic = true;
        ingredientBody.detectCollisions = true;

        Transform tray = GetTrayTransform();
        Vector3 worldScale = ingredient.transform.lossyScale;

        ingredientBody.transform.SetParent(tray, true);

        ingredientBody.transform.localScale = DivideScale(
            worldScale,
            tray.lossyScale);

        placedPortions.Add(portion);
        if (string.IsNullOrWhiteSpace(activeBatchId))
        {
            activeBatchId = portion.BatchId;
            expectedPortionCount = portion.TotalPortions;
        }
        portionBodies[portion] = ingredientBody;
        portionColliders[portion] =
            ingredient.GetComponentsInChildren<Collider>(true);
        SetTrayCollisionIgnored(portion, true);
        SmoothPlacement.Begin(ingredientBody, restingPosition, restingRotation, tray);

        RecipeFeedback.Report(
            $"{ProductSingular} portion {placedPortions.Count}/" +
            $"{expectedPortionCount} placed on tray.");

        if (HasCompleteCookieBatch)
            RecipeFeedback.Report($"All {ProductPlural} portions are on the prepared tray.");
        return true;
    }

    private void OnTriggerExit(Collider other)
    {
        Ingredient ingredient = other.GetComponentInParent<Ingredient>();

        if (ingredient == null)
            return;

        DoughPortion portion = ingredient.GetComponent<DoughPortion>();

        if (portion == null || !placedPortions.Contains(portion))
            return;

        if (portion.transform.IsChildOf(GetTrayTransform()))
            return;

        RemovePortion(portion);
    }

    private bool CanAcceptBatch(DoughPortion portion)
    {
        if (string.IsNullOrWhiteSpace(activeBatchId))
        {
            return true;
        }

        if (!string.Equals(
                activeBatchId,
                portion.BatchId,
                System.StringComparison.Ordinal))
        {
            RecipeFeedback.Warning($"Finish loading the current {ProductSingular} batch first.");
            return false;
        }

        if (portion.TotalPortions != expectedPortionCount)
        {
            RecipeFeedback.SystemWarning($"{ProductSingular} portion metadata does not match its batch.");
            return false;
        }

        return placedPortions.Count < expectedPortionCount;
    }

    private void RemovePortion(DoughPortion portion)
    {
        int index = placedPortions.IndexOf(portion);

        if (index >= 0)
            RemovePortionAt(index);
    }

    private void RemovePortionAt(int index)
    {
        DoughPortion portion = placedPortions[index];

        if (portion != null)
            SetTrayCollisionIgnored(portion, false);

        placedPortions.RemoveAt(index);

        if (portion != null)
        {
            portionBodies.Remove(portion);
            portionColliders.Remove(portion);
        }

        if (placedPortions.Count == 0)
        {
            activeBatchId = null;
            expectedPortionCount = 0;
        }
    }

    private bool AllPortionsSettled()
    {
        foreach (var portion in placedPortions)
            if (portion == null || !portionBodies.TryGetValue(portion, out var body) || SmoothPlacement.Settling(body)) return false;
        return true;
    }

    private void SetTrayCollisionIgnored(
        DoughPortion portion,
        bool ignored)
    {
        if (portion == null ||
            !portionColliders.TryGetValue(
                portion,
                out Collider[] ingredientColliders) ||
            trayColliders == null)
        {
            return;
        }

        foreach (Collider ingredientCollider in ingredientColliders)
        {
            if (ingredientCollider == null)
                continue;

            foreach (Collider trayCollider in trayColliders)
            {
                if (trayCollider == null ||
                    trayCollider == ingredientCollider)
                {
                    continue;
                }

                Physics.IgnoreCollision(
                    ingredientCollider,
                    trayCollider,
                    ignored);
            }
        }
    }

    private static Collider[] GetTrayOnlyColliders(Transform tray)
    {
        List<Collider> colliders = new();

        foreach (Collider trayCollider in
                 tray.GetComponentsInChildren<Collider>(true))
        {
            if (trayCollider != null &&
                trayCollider.GetComponentInParent<DoughPortion>() == null)
            {
                colliders.Add(trayCollider);
            }
        }

        return colliders.ToArray();
    }

    private Transform GetTrayTransform()
    {
        return transform.parent != null
            ? transform.parent
            : transform;
    }

    private static void StopDynamicMotion(Rigidbody body)
    {
        if (body == null || body.isKinematic)
            return;

        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
    }

    private static bool IsIngredient(string value, string expected)
    {
        return string.Equals(
            value?.Trim(),
            expected,
            System.StringComparison.OrdinalIgnoreCase);
    }

    private bool IsAcceptedPortionName(string ingredientName)
    {
        if (IsIngredient(ingredientName, PortionIngredientName))
            return true;

        if (activeRecipe == null)
        {
            return IsIngredient(ingredientName, "BakedCookie") ||
                   IsIngredient(ingredientName, "UnderbakedCookie") ||
                   IsIngredient(ingredientName, "OvercookedCookie") ||
                   IsIngredient(ingredientName, "BurntCookie");
        }

        return IsIngredient(ingredientName, activeRecipe.UnderbakedIngredientName) ||
               IsIngredient(ingredientName, activeRecipe.BakedIngredientName) ||
               IsIngredient(ingredientName, activeRecipe.OvercookedIngredientName) ||
               IsIngredient(ingredientName, activeRecipe.BurntIngredientName);
    }

    private static Vector3 DivideScale(
        Vector3 worldScale,
        Vector3 parentScale)
    {
        return new Vector3(
            SafeDivide(worldScale.x, parentScale.x),
            SafeDivide(worldScale.y, parentScale.y),
            SafeDivide(worldScale.z, parentScale.z));
    }

    private static float SafeDivide(float value, float divisor)
    {
        return Mathf.Abs(divisor) > Mathf.Epsilon
            ? value / divisor
            : value;
    }

    private void OnValidate()
    {
        portionSpacing.x = Mathf.Max(0.01f, portionSpacing.x);
        portionSpacing.y = Mathf.Max(0.01f, portionSpacing.y);
    }
}
