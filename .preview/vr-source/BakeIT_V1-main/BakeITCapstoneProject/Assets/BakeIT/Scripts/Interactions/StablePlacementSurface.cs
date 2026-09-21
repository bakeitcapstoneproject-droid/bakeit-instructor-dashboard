using UnityEngine;

// Release assistance: keep the chosen X/Z/yaw and smoothly settle near the surface.
[RequireComponent(typeof(BoxCollider))]
public sealed class StablePlacementSurface : MonoBehaviour
{
    [SerializeField] private bool isPreparationBoard;
    [SerializeField, Min(.01f)] private float maximumSettleDistance = .22f;
    [SerializeField, Min(0f)] private float toolEdgeAllowance;
    [SerializeField] private FridgeDoorController fridgeDoor;
    private BoxCollider support;
    private PickupController pickup;
    public bool IsPreparationBoard => isPreparationBoard;

    private void OnCollisionEnter(Collision collision) => RegisterReleasedLanding(collision);
    private void OnCollisionStay(Collision collision) => RegisterReleasedLanding(collision);
    private void RegisterReleasedLanding(Collision collision)
    {
        var body = collision.rigidbody;
        if (body == null) return;
        if (pickup == null) pickup = FindAnyObjectByType<PickupController>();
        if (pickup != null && pickup.HeldObject == body) return;
        var item = body.GetComponent<PreparationItem>();
        if (item != null && item.RestingSurface != this) TryPlace(item);
    }

    public bool TryPlace(PreparationItem item)
    {
        if (item == null || !isActiveAndEnabled || (fridgeDoor != null && !fridgeDoor.IsOpen)) return false;
        if (item.RestingSurface == this) return true;
        if (support == null) support = GetComponent<BoxCollider>();
        var body = item.GetComponent<Rigidbody>();
        if (SmoothPlacement.Settling(body)) return false;
        float allowance = item.GetComponent<MeasuringScoop>() != null || item.GetComponent<MixingTool>() != null ? toolEdgeAllowance : 0f;
        if (!PlacementGeometry.TryGetPose(body, support, item.RestingRotation, out var position, out var rotation, maximumSettleDistance, allowance))
            return false;
        FindAnyObjectByType<PickupController>()?.ReleaseForPlacement(body);
        item.PickedUp();
        SmoothPlacement.Begin(body, position, rotation, null, () => item.Placed(this));
        return true;
    }
}
