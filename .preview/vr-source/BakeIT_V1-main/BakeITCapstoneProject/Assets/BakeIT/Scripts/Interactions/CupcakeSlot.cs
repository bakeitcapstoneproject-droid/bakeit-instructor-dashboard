using UnityEngine;

// One physical well or cooling position; batch state remains in CupcakeBatch.
public sealed class CupcakeSlot : MonoBehaviour, IPourDestination
{
    [SerializeField] private CupcakeBatch batch;
    [SerializeField] private bool coolingPosition;
    [SerializeField] private int index;
    public CupcakeBatch Batch => batch;
    public bool IsCoolingPosition => coolingPosition;
    public int Index => index;
    public CupcakePortion Portion { get; internal set; }
    public Vector3 TransferTarget => transform.position + transform.up * .026f;
    public bool HasPortion => Portion && Portion.transform.parent == transform;
    public bool CanPreviewPour(PourableIngredient source) => !coolingPosition && batch.CanFill(this, source);
    public bool TryReceive(PourableIngredient source) => batch.TryFill(this, source);
    private void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody && !SmoothPlacement.Settling(other.attachedRigidbody))
        {
            batch.TryPlaceReleased(other.attachedRigidbody);
            if (!coolingPosition) batch.TryCheckDoneness(other.attachedRigidbody, this);
        }
    }
}
