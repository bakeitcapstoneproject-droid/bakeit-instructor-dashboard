using UnityEngine;

// A forgiving wall-rack return point for one exact measuring tool or the whisk.
[DisallowMultipleComponent]
public sealed class ToolHangingSlot : MonoBehaviour
{
    [SerializeField] private Transform placementPoint;
    [SerializeField] private RecipeMeasureTool acceptedMeasureTool;
    [SerializeField] private bool acceptsWhisk;
    [SerializeField, Min(0.05f)] private float snapDistance = 0.55f;
    [SerializeField, Min(0.05f)] private float crosshairAssistRadius = 0.22f;
    [SerializeField, Min(0.25f)] private float crosshairAssistDistance = 2.5f;
    [SerializeField] private Vector3 hangingEuler;

    public RecipeMeasureTool AcceptedMeasureTool => acceptedMeasureTool;

    public bool TryPlace(Rigidbody body)
    {
        if (body == null || !isActiveAndEnabled)
            return false;

        MeasuringScoop scoop = body.GetComponent<MeasuringScoop>();
        bool matchesMeasure = scoop != null &&
                              scoop.MeasureTool == acceptedMeasureTool;
        var mixingTool = body.GetComponent<MixingTool>();
        bool matchesWhisk = acceptsWhisk && mixingTool != null && mixingTool.Action == MixingAction.Mix;

        if (!matchesMeasure && !matchesWhisk)
            return false;

        Transform point = placementPoint != null ? placementPoint : transform;
        // Kinematic tools can keep a stale computed worldCenterOfMass for one
        // frame after being detached from a rack parent. The object pivot is
        // stable and is the pose that the hook actually snaps.
        PickupController pickup = FindAnyObjectByType<PickupController>();
        bool nearHook = Vector3.Distance(body.position, point.position) <= snapDistance;
        bool aimingAtHook = IsAimingAtHook(pickup, point);
        if (!nearHook && !aimingAtHook)
            return false;

        if (pickup == null || !pickup.ReleaseForPlacement(body))
            return false;

        body.isKinematic = false;
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.useGravity = false;
        body.isKinematic = true;
        body.transform.SetParent(point, true);
        body.transform.SetPositionAndRotation(
            point.position,
            Quaternion.Euler(hangingEuler));
        body.GetComponent<PreparationItem>()?.PickedUp();
        RecipeFeedback.Report($"Returned {body.name} to the hanging rack.");
        return true;
    }

    private bool IsAimingAtHook(PickupController pickup, Transform point)
    {
        Camera playerCamera = pickup != null
            ? pickup.GetComponentInChildren<Camera>()
            : null;
        if (playerCamera == null)
            return false;

        Vector3 toHook = point.position - playerCamera.transform.position;
        float forwardDistance = Vector3.Dot(
            toHook,
            playerCamera.transform.forward);
        if (forwardDistance <= 0f || forwardDistance > crosshairAssistDistance)
            return false;

        Vector3 nearestPointOnAimRay =
            playerCamera.transform.position +
            playerCamera.transform.forward * forwardDistance;
        return Vector3.Distance(nearestPointOnAimRay, point.position) <=
               crosshairAssistRadius;
    }
}
