using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class OvenBakeZone : MonoBehaviour
{
    private TrayReceiver trayInOven;
    private BrowniePanReceiver browniePanInOven;
    private CupcakeBatch cupcakeBatchInOven;
    public CupcakeBatch CupcakeBatchInOven => cupcakeBatchInOven;
    public bool HasLoadedCupcakes => cupcakeBatchInOven && (cupcakeBatchInOven.IsReadyForOven || cupcakeBatchInOven.BakeCompleted);
    public bool HasAnyBakeware => trayInOven || browniePanInOven || cupcakeBatchInOven;

    public TrayReceiver TrayInOven => trayInOven;
    public BrowniePanReceiver BrowniePanInOven => browniePanInOven;
    public bool HasLoadedTray =>
        trayInOven != null && trayInOven.HasCompleteCookieBatch;
    public bool HasLoadedBrowniePan =>
        browniePanInOven != null &&
        (browniePanInOven.IsReadyForOven || browniePanInOven.BakeCompleted);
    public bool HasLoadedBakeware => HasLoadedTray || HasLoadedBrowniePan || HasLoadedCupcakes;
    public RecipeDefinition ActiveRecipe => HasLoadedCupcakes ? cupcakeBatchInOven.ActiveRecipe : HasLoadedBrowniePan
        ? browniePanInOven.ActiveRecipe
        : null;

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryRegisterTray(other);
        TryRegisterBrowniePan(other);
        TryRegisterCupcakeTray(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (trayInOven == null)
            TryRegisterTray(other);
        if (browniePanInOven == null)
            TryRegisterBrowniePan(other);
        if (!cupcakeBatchInOven) TryRegisterCupcakeTray(other);
    }

    private void OnTriggerExit(Collider other)
    {
        var exitingCupcake = FindCupcakeBatch(other);
        if (exitingCupcake && exitingCupcake == cupcakeBatchInOven && !StillInside(exitingCupcake.TrayBody.transform))
        {
            cupcakeBatchInOven = null;
            RecipeFeedback.Report("Cupcake tray removed from oven.");
        }
        TrayReceiver exitingTray = FindTrayReceiver(other);
        if (exitingTray != null && exitingTray == trayInOven &&
            !StillInside(exitingTray.transform))
        {
            trayInOven = null;
            RecipeFeedback.Report("Tray removed from oven.");
        }

        BrowniePanReceiver exitingPan = FindBrowniePan(other);
        if (exitingPan != null && exitingPan == browniePanInOven &&
            !StillInside(exitingPan.transform))
        {
            browniePanInOven = null;
            RecipeFeedback.Report("Brownie pan removed from oven.");
        }
    }

    private static CupcakeBatch FindCupcakeBatch(Collider other)
    {
        if (!other.attachedRigidbody) return null;
        var well = other.attachedRigidbody.GetComponentInChildren<CupcakeSlot>();
        return well && !well.IsCoolingPosition && well.Batch.TrayBody == other.attachedRigidbody ? well.Batch : null;
    }
    private void TryRegisterCupcakeTray(Collider other)
    {
        var candidate = FindCupcakeBatch(other);
        if (!candidate || candidate == cupcakeBatchInOven) return;
        cupcakeBatchInOven = candidate;
        RecipeFeedback.Report(candidate.IsReadyForOven ? "Twelve filled cupcake liners are in the oven." : "Cupcake tray in oven; all twelve lined wells must be filled before baking.");
    }

    private void TryRegisterBrowniePan(Collider other)
    {
        BrowniePanReceiver candidate = FindBrowniePan(other);
        if (candidate == null || candidate == browniePanInOven)
            return;

        browniePanInOven = candidate;
        if (candidate.IsReadyForOven)
            RecipeFeedback.Report("Prepared brownie pan is inside the oven.");
        else if (candidate.HasBatter)
            RecipeFeedback.Warning(
                "Brownie pan is inside the oven, but the batter is not spread evenly.");
        else
            RecipeFeedback.Warning(
                "Brownie pan is inside the oven, but it is not ready for baking.");
    }

    private void TryRegisterTray(Collider other)
    {
        TrayReceiver candidate = FindTrayReceiver(other);

        if (candidate == null || candidate == trayInOven)
            return;

        trayInOven = candidate;

        if (trayInOven.HasCompleteCookieBatch)
            RecipeFeedback.Report("Loaded cookie tray is inside the oven.");
        else if (trayInOven.HasPortionedDough)
            RecipeFeedback.Warning(
                "Tray is inside the oven, but the cookie batch is incomplete.");
        else
            RecipeFeedback.Warning("Tray is inside the oven, but it has no cookie portions.");
    }

    private static TrayReceiver FindTrayReceiver(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            TrayReceiver receiver =
                other.attachedRigidbody.GetComponentInChildren<TrayReceiver>();

            if (receiver != null)
                return receiver;
        }

        return other.GetComponentInParent<TrayReceiver>();
    }

    private static BrowniePanReceiver FindBrowniePan(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            BrowniePanReceiver receiver =
                other.attachedRigidbody.GetComponentInChildren<BrowniePanReceiver>(true);
            if (receiver != null)
                return receiver;
        }

        return other.GetComponentInParent<BrowniePanReceiver>();
    }

    private bool StillInside(Transform receiverTransform)
    {
        if (receiverTransform == null)
            return false;

        Rigidbody body = receiverTransform.GetComponentInParent<Rigidbody>();
        Transform root = body != null ? body.transform : receiverTransform;
        Bounds zoneBounds = GetComponent<BoxCollider>().bounds;
        foreach (Collider candidate in root.GetComponentsInChildren<Collider>(true))
        {
            if (candidate == null || !candidate.enabled || candidate.isTrigger)
                continue;
            if (zoneBounds.Intersects(candidate.bounds))
                return true;
        }
        return false;
    }
}
