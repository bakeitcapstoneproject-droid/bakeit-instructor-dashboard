using System.Collections.Generic;
using UnityEngine;

/// <summary>Accepts a small serving of released baked food. No grading policy.</summary>
[DisallowMultipleComponent]
[DefaultExecutionOrder(250)]
public sealed class ServingPlateReceiver : MonoBehaviour
{
    [SerializeField, Min(1)] private int cookieServingCount = 3;
    [SerializeField, Range(2, 3)] private int brownieServingCount = 2;
    [SerializeField] private Transform plateRoot;
    [SerializeField] private float surfaceLocalY = .015f;
    [SerializeField] private float servingRadius = .12f;
    private readonly List<ServingPortion> portions = new();
    private PickupController pickup;
    private CookieRecipeSessionController session;
    public int RequiredCount
    {
        get
        {
            if (!session) session = FindAnyObjectByType<CookieRecipeSessionController>();
            return session != null && session.IsBrownieRecipeActive ? brownieServingCount : cookieServingCount;
        }
    }
    public int PlacedCount { get { Prune(); return portions.Count; } }
    public bool IsServingComplete => PlacedCount == RequiredCount;
    public float SurfaceHeight => surfaceLocalY;
    public bool TrySetBrownieServingCount(int count)
    {
        if (count < 2 || count > 3 || !session || !session.IsBrownieRecipeActive) return false;
        if (PlacedCount > count) { RecipeFeedback.Warning("Remove a brownie before choosing a smaller serving."); return false; }
        brownieServingCount = count;
        Arrange();
        return true;
    }

    private void Awake()
    {
        if (!plateRoot) plateRoot = transform;
        pickup = FindAnyObjectByType<PickupController>();
        session = FindAnyObjectByType<CookieRecipeSessionController>();
        brownieServingCount = Mathf.Clamp(brownieServingCount, 2, 3);
    }

    private void Update() => Prune();
    // Keep child Rigidbody poses attached during the plate's own smooth placement.
    private void LateUpdate() { if (portions.Count > 0) Arrange(); }
    private void Prune()
    {
        if (portions.RemoveAll(p => !p || !p.transform.IsChildOf(plateRoot)) > 0) Arrange();
    }
    private void OnTriggerStay(Collider other)
    {
        var body = other.attachedRigidbody;
        if (body != null) TryPlaceReleased(body);
    }

    public bool TryPlaceReleased(Rigidbody body)
    {
        if (!body || !pickup || pickup.HeldObject == body || SmoothPlacement.Settling(body)) return false;
        if (!session) session = FindAnyObjectByType<CookieRecipeSessionController>();
        if (!session || !session.HasSelectedRecipe) return false;
        if (pickup.HeldObject == plateRoot.GetComponent<Rigidbody>()) return false;
        var portion = body.GetComponent<ServingPortion>();
        if (!portion || !portion.IsBaked || portion.RecipeId != session.SelectedRecipeId) return false;
        if (portions.Contains(portion) || PlacedCount >= RequiredCount) return false;
        if (FindAnyObjectByType<OvenBakeZone>() is OvenBakeZone oven &&
            (session.IsBrownieRecipeActive ? oven.HasLoadedBrowniePan : oven.HasLoadedTray)) return false;
        Vector3 local = plateRoot.InverseTransformPoint(body.worldCenterOfMass);
        if (new Vector2(local.x, local.z).magnitude > servingRadius + .04f ||
            local.y < surfaceLocalY - .03f || local.y > surfaceLocalY + .20f) return false;

        if (!body.isKinematic) { body.linearVelocity = Vector3.zero; body.angularVelocity = Vector3.zero; }
        body.isKinematic = true;
        body.interpolation = RigidbodyInterpolation.None;
        body.useGravity = false;
        body.transform.SetParent(plateRoot, true);
        portions.Add(portion);
        Arrange();
        RecipeFeedback.Report($"Serving plate: {portions.Count} of {RequiredCount} {session.ActiveRecipe.ProductPlural}.");
        return true;
    }

    private void Arrange()
    {
        bool stack = session && session.IsBrownieRecipeActive;
        float top = surfaceLocalY;
        for (int i = 0; i < portions.Count; i++)
        {
            var item = portions[i]; if (!item) continue;
            float angle = (90f + i * 360f / cookieServingCount) * Mathf.Deg2Rad;
            Vector3 position = stack ? new Vector3(i == 1 ? .012f : -.006f, 0, i == 1 ? .008f : -.006f)
                : new Vector3(Mathf.Cos(angle) * .071f, 0, Mathf.Sin(angle) * .071f);
            item.transform.localRotation = Quaternion.Euler(0, stack ? (i % 2 == 0 ? -8 : 12) : 0, 0);
            item.transform.localPosition = position;
            Bounds bounds = LocalFoodBounds(item.transform);
            position.y = (stack ? top : surfaceLocalY) - bounds.min.y;
            item.transform.localPosition += Vector3.up * position.y;
            top = bounds.max.y + position.y + .0015f;
            var body = item.GetComponent<Rigidbody>();
            body.position = item.transform.position; body.rotation = item.transform.rotation;
        }
        Physics.SyncTransforms();
    }

    private Bounds LocalFoodBounds(Transform food)
    {
        bool found = false; Bounds bounds = default;
        // Crust supports the next square; protruding decorative kernels sit inside it.
        foreach (var filter in food.GetComponents<MeshFilter>())
        {
            var renderer = filter.GetComponent<Renderer>();
            if (!filter.sharedMesh || !renderer || !renderer.enabled || filter.GetComponent<TextMesh>()) continue;
            var box = filter.sharedMesh.bounds;
            for (int i = 0; i < 8; i++)
            {
                var vertex = box.center + Vector3.Scale(box.extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
                var p = plateRoot.InverseTransformPoint(filter.transform.TransformPoint(vertex));
                if (!found) { bounds = new Bounds(p, Vector3.zero); found = true; } else bounds.Encapsulate(p);
            }
        }
        return bounds;
    }
}
