using UnityEngine;

// A finite supply of paper sheets/liners; existing rolls retain one sheet.
public sealed class ParchmentDispenser : MonoBehaviour
{
    [SerializeField] private GameObject sheet;
    [SerializeField] private Transform outlet;
    [SerializeField] private TextMesh instruction;
    [SerializeField] private GameObject[] additionalSheets;
    [SerializeField] private string supplyLabel = "PARCHMENT";
    [SerializeField] private Transform stackVisual;
    private bool dispensed;
    private int dispensedCount;
    public int RemainingCount => 1 + (additionalSheets?.Length ?? 0) - dispensedCount;

    public bool HasDispensed => dispensed;
    public bool TryDispense(PickupController pickup)
    {
        if (pickup == null || sheet == null || outlet == null) return false;
        if (pickup.HeldObject != null)
        {
            RecipeFeedback.Warning("Set down your tool, then press E to take " + supplyLabel.ToLowerInvariant() + ".");
            return true;
        }
        if (RemainingCount <= 0)
        {
            RecipeFeedback.Warning("Use the supplies already taken. Restart replenishes this dispenser.");
            return true;
        }
        var nextSheet = dispensedCount == 0 ? sheet : additionalSheets[dispensedCount-1];
        nextSheet.transform.SetPositionAndRotation(outlet.position, outlet.rotation);
        nextSheet.SetActive(true);
        var body = nextSheet.GetComponent<Rigidbody>();
        body.isKinematic = true; body.useGravity = false;
        Physics.SyncTransforms();
        pickup.TryPickupBody(body);
        if (pickup.HeldObject != body) { nextSheet.SetActive(false); return true; }
        dispensed = true;
        dispensedCount++;
        if (stackVisual && dispensedCount <= stackVisual.childCount) stackVisual.GetChild(dispensedCount-1).gameObject.SetActive(false);
        if (instruction != null) instruction.text = supplyLabel + (RemainingCount > 0 ? $"\nE: TAKE ONE ({RemainingCount} LEFT)" : "\nALL TAKEN");
        RecipeFeedback.Report(supplyLabel + " taken. Release it close above the tray.");
        return true;
    }
}
