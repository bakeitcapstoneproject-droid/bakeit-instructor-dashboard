using UnityEngine;

[DefaultExecutionOrder(90)]
public class TrayLinerReceiver : MonoBehaviour
{
    private const string RequiredIngredientName = "ParchmentPaper";

    [SerializeField] private Transform placementPoint;

    [Header("Placement Assist")]
    [SerializeField, Min(0.01f)] private float maximumPlacementDistance = 0.35f;
    [SerializeField, Min(0f)] private float horizontalDetectionPadding = 0.12f;
    [SerializeField, Min(0f)] private float verticalDetectionPadding = 0.35f;
    [SerializeField, Min(0f)] private float magneticSnapRadius = 0.15f;

    private Ingredient placedLiner;
    private Rigidbody placedLinerBody;
    private PickupController pickupController;
    private Collider[] linerColliders;
    private Collider[] trayColliders;
    private float nextContactTry;

    public bool HasParchment => placedLiner != null && !SmoothPlacement.Settling(placedLinerBody);
    public BoxCollider PlacementSurface => HasParchment ? placedLiner.GetComponent<BoxCollider>() : null;

    private void Awake()
    {
        if (placementPoint == null)
            placementPoint = transform;

        maximumPlacementDistance = Mathf.Max(0.01f, maximumPlacementDistance);
        horizontalDetectionPadding = Mathf.Max(0f, horizontalDetectionPadding);
        verticalDetectionPadding = Mathf.Max(0f, verticalDetectionPadding);
        magneticSnapRadius = Mathf.Max(0f, magneticSnapRadius);

        pickupController = FindAnyObjectByType<PickupController>();
    }

    private void Update()
    {
        if (placedLiner == null)
            return;

        Transform tray = GetTrayTransform();

        if (!placedLiner.transform.IsChildOf(tray))
            ClearLiner();
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
        nearby.Expand(new Vector3(
            horizontalDetectionPadding * 2f,
            verticalDetectionPadding * 2f,
            horizontalDetectionPadding * 2f));
        if (!nearby.Intersects(other.bounds)) return false;
        if (placedLiner != null)
            return false;

        Ingredient ingredient =
            other.GetComponentInParent<Ingredient>();

        if (ingredient == null ||
            !IsParchment(ingredient.ingredientName))
        {
            return false;
        }

        Rigidbody ingredientBody =
            ingredient.GetComponentInParent<Rigidbody>();

        if (ingredientBody == null)
            return false;

        if (SmoothPlacement.Settling(ingredientBody)) return false;
        var surface = GetTrayTransform().GetComponent<BoxCollider>();
        Quaternion restingRotation = Quaternion.Euler(0, ingredientBody.transform.eulerAngles.y, 0);
        if (!TryGetAssistedPose(
                ingredientBody,
                surface,
                restingRotation,
                out var restingPosition,
                out restingRotation))
        {
            return false;
        }

        placedLiner = ingredient;
        placedLinerBody = ingredientBody;

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

        linerColliders =
            ingredient.GetComponentsInChildren<Collider>(true);
        trayColliders = tray.GetComponentsInChildren<Collider>(true);
        SetTrayCollisionIgnored(true);
        SmoothPlacement.Begin(ingredientBody, restingPosition, restingRotation, tray);

        RecipeFeedback.Report("Parchment paper placed on tray.");
        return true;
    }

    private bool TryGetAssistedPose(
        Rigidbody body,
        BoxCollider surface,
        Quaternion playerRotation,
        out Vector3 restingPosition,
        out Quaternion restingRotation)
    {
        if (PlacementGeometry.TryGetPose(
                body,
                surface,
                playerRotation,
                out restingPosition,
                out restingRotation,
                maximumPlacementDistance))
        {
            return true;
        }

        Vector3 originalPosition = body.position;
        Quaternion originalRotation = body.rotation;
        Vector3 snapCenter = placementPoint != null
            ? placementPoint.position
            : surface.bounds.center;
        Vector2 horizontalOffset = new Vector2(
            originalPosition.x - snapCenter.x,
            originalPosition.z - snapCenter.z);

        if (horizontalOffset.magnitude > magneticSnapRadius)
            return false;

        Vector3 centeredPreviewPosition = new Vector3(
            snapCenter.x,
            originalPosition.y,
            snapCenter.z);
        Quaternion centeredPreviewRotation = Quaternion.Euler(
            0f,
            GetTrayTransform().eulerAngles.y,
            0f);
        body.transform.SetPositionAndRotation(
            centeredPreviewPosition,
            centeredPreviewRotation);
        body.position = centeredPreviewPosition;
        body.rotation = centeredPreviewRotation;
        Physics.SyncTransforms();

        bool foundPose = PlacementGeometry.TryGetPose(
            body,
            surface,
            body.rotation,
            out restingPosition,
            out restingRotation,
            maximumPlacementDistance);

        body.transform.SetPositionAndRotation(
            originalPosition,
            originalRotation);
        body.position = originalPosition;
        body.rotation = originalRotation;
        Physics.SyncTransforms();
        return foundPose;
    }

    private void OnTriggerExit(Collider other)
    {
        Ingredient ingredient =
            other.GetComponentInParent<Ingredient>();

        if (ingredient == null ||
            placedLiner == null ||
            ingredient != placedLiner)
            return;

        if (ingredient.transform.IsChildOf(GetTrayTransform()))
            return;

        ClearLiner();
    }

    private void ClearLiner()
    {
        SetTrayCollisionIgnored(false);
        placedLiner = null;
        placedLinerBody = null;
        linerColliders = null;
        trayColliders = null;
    }

    private void SetTrayCollisionIgnored(bool ignored)
    {
        if (linerColliders == null || trayColliders == null)
            return;

        foreach (Collider linerCollider in linerColliders)
        {
            if (linerCollider == null)
                continue;

            foreach (Collider trayCollider in trayColliders)
            {
                if (trayCollider == null ||
                    trayCollider == linerCollider ||
                    trayCollider.transform.IsChildOf(
                        placedLiner.transform))
                {
                    continue;
                }

                Physics.IgnoreCollision(
                    linerCollider,
                    trayCollider,
                    ignored);
            }
        }
    }

    private Transform GetTrayTransform()
    {
        return transform.parent != null
            ? transform.parent
            : transform;
    }

    private static bool IsParchment(string ingredientName)
    {
        return string.Equals(
            ingredientName?.Trim(),
            RequiredIngredientName,
            System.StringComparison.OrdinalIgnoreCase);
    }

    private static void StopDynamicMotion(Rigidbody body)
    {
        if (body == null || body.isKinematic)
            return;

        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
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
}
