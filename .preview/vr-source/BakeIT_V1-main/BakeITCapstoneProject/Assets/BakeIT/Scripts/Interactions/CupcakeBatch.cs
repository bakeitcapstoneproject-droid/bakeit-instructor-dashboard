using System.Linq;
using UnityEngine;

// Twelve independent wells and rack positions, using shared pickup, pouring and oven controls.
public sealed class CupcakeBatch : MonoBehaviour
{
    [SerializeField] private MixingSequence sequence;
    [SerializeField] private Rigidbody trayBody;
    [SerializeField] private CupcakeSlot[] wells;
    [SerializeField] private CupcakeSlot[] coolingPositions;
    [SerializeField] private CupcakePortion[] portions;
    [SerializeField] private ParchmentDispenser linerDispenser;
    [SerializeField] private StablePlacementSurface startingRest;
    [SerializeField, Min(.1f)] private float coolingSeconds = 10;
    private PickupController pickup;
    private OvenController oven;
    private OvenBakeZone ovenZone;
    private bool equipmentPrepared;
    public Rigidbody TrayBody => trayBody;
    public CupcakeSlot[] Wells => wells;
    public CupcakeSlot[] CoolingPositions => coolingPositions;
    public CupcakePortion[] Portions => portions;
    public RecipeDefinition ActiveRecipe => sequence.Recipe;
    public bool IsActive => sequence.IsActive;
    public int LinedCount => wells.Count(s => s.HasPortion);
    public int FilledCount => portions.Count(p => p.IsFilled);
    public int RemainingBatterPortions => 12 - FilledCount;
    public float RemainingBatterVolume => 12 - portions.Sum(p=>p.FillProgress);
    public int OnRackCount => portions.Count(p => p.IsOnRack);
    public int CooledCount => portions.Count(p => p.IsCooled && p.IsOnRack);
    public bool AllCooled => CooledCount == 12;
    public bool BatterReady => sequence.IsComplete;
    public bool BakeCompleted { get; private set; }
    public bool DonenessChecked { get; private set; }
    public OvenBakeResult BakeResult { get; private set; }
    public float CoolingSeconds => coolingSeconds;
    public bool IsReadyForOven => LinedCount == 12 && FilledCount == 12 && !BakeCompleted;
    public bool IsInOven => ovenZone && ovenZone.CupcakeBatchInOven == this;
    public bool ReadyToMix => equipmentPrepared && LinedCount == 12;
    public Transform GetGuidanceTarget()
    {
        if (!IsActive || AllCooled) return null;
        var held = pickup ? pickup.HeldObject : null;
        if (!equipmentPrepared && oven && !oven.IsPreheated && !oven.IsPreheating) return oven.transform;
        if (!TrayOnWorktop && !BakeCompleted && FilledCount < 12) return GameObject.Find("Tray Worktop Rest")?.transform;
        if (LinedCount < 12 && !BakeCompleted)
            return held && held.GetComponent<CupcakePortion>() ? wells.FirstOrDefault(w=>!w.HasPortion)?.transform : linerDispenser.transform;
        if (!BatterReady) return null;
        if (FilledCount < 12) return wells.FirstOrDefault(w=>w.HasPortion&&!w.Portion.IsFilled)?.transform;
        if (!BakeCompleted) return oven && !oven.IsBaking ? oven.transform : null;
        if (IsInOven || !TrayOnWorktop) return trayBody.transform;
        if (!DonenessChecked) return held && held.GetComponent<CupcakeDonenessProbe>() ? wells[0].transform : FindAnyObjectByType<CupcakeDonenessProbe>()?.transform;
        return held && held.GetComponent<CupcakePortion>() ? coolingPositions.FirstOrDefault(w=>!w.HasPortion)?.transform : portions.FirstOrDefault(p=>!p.IsOnRack)?.transform;
    }
    public bool TrayOnWorktop => trayBody && trayBody.GetComponent<PreparationItem>().RestingSurface != null &&
        trayBody.GetComponent<PreparationItem>().RestingSurface.name == "Tray Worktop Rest" &&
        pickup && pickup.HeldObject != trayBody && !IsInOven && Vector3.Angle(trayBody.transform.up,Vector3.up)<12;
    private void Awake()
    {
        pickup = FindAnyObjectByType<PickupController>();
        oven = FindAnyObjectByType<OvenController>();
        ovenZone = FindAnyObjectByType<OvenBakeZone>();
        if (startingRest) trayBody.GetComponent<PreparationItem>().Placed(startingRest);
    }
    private void Update()
    {
        if (IsActive && !equipmentPrepared && LinedCount == 12 && oven && oven.IsPreheated) equipmentPrepared = true;
    }
    public bool TryPlaceReleased(Rigidbody body)
    {
        if (!IsActive || !body || SmoothPlacement.Settling(body) || pickup && pickup.HeldObject == body) return false;
        var portion = body.GetComponent<CupcakePortion>();
        if (!portion || portion.Batch != this) return false;
        if (portion.AttachedSlot && portion.transform.parent == portion.AttachedSlot.transform) return false;
        bool toRack = portion.IsBaked;
        if (toRack ? !DonenessChecked || IsInOven : portion.FillProgress > 0 || !TrayOnWorktop) return false;
        var candidates = toRack ? coolingPositions : wells;
        CupcakeSlot chosen = null;
        float nearest = .065f;
        foreach (var slot in candidates)
        {
            // An occupied nearby well must not redirect the liner into its neighbour.
            Vector3 local = slot.transform.InverseTransformPoint(body.position);
            float distance = new Vector2(local.x,local.z).magnitude;
            if (local.y < -.025f || local.y > .16f || distance >= nearest) continue;
            chosen = slot; nearest = distance;
        }
        if (!chosen || chosen.HasPortion) return false;
        if (portion.AttachedSlot) portion.AttachedSlot.Portion = null;
        pickup.ReleaseForPlacement(body);
        chosen.Portion = portion;
        portion.AttachedSlot = chosen;
        // All roots have unit scale; preserve the existing liner size while attaching.
        body.transform.SetParent(chosen.transform,true);
        SmoothPlacement.Begin(body, chosen.transform.position, chosen.transform.rotation, chosen.transform);
        foreach (var a in body.GetComponentsInChildren<Collider>())
            foreach (var b in trayBody.GetComponentsInChildren<Collider>())
                if (a != b && b.attachedRigidbody == trayBody) Physics.IgnoreCollision(a,b,true);
        RecipeFeedback.Report(toRack ? $"Cupcake placed on cooling rack: {OnRackCount}/12." : $"Paper liner placed in well {chosen.Index+1}: {LinedCount}/12.");
        return true;
    }
    public bool CanFill(CupcakeSlot slot, PourableIngredient source)
    {
        return IsActive && source && source.CupcakeBatterSource == this && BatterReady && TrayOnWorktop &&
            LinedCount == 12 && !BakeCompleted && slot.HasPortion && !slot.Portion.IsFilled;
    }
    public bool TryFill(CupcakeSlot slot, PourableIngredient source)
    {
        if (!CanFill(slot,source) || !source.TryCompleteTransfer(slot,slot.TransferTarget)) return false;
        slot.Portion.Fill();
        RecipeFeedback.Report($"Cupcake well {slot.Index+1} filled three-quarters: {FilledCount}/12.");
        return true;
    }
    public bool TryCheckDoneness(Rigidbody body,CupcakeSlot well)
    {
        if (!body || !BakeCompleted || !TrayOnWorktop || pickup.HeldObject != body || !well.HasPortion) return false;
        var probe = body.GetComponent<CupcakeDonenessProbe>();
        if (!probe) return false;
        Vector3 tipLocal = well.transform.InverseTransformPoint(probe.TipPosition);
        if (new Vector2(tipLocal.x,tipLocal.z).magnitude > .038f || tipLocal.y < .015f || tipLocal.y > .057f) return false;
        // The thin tip must physically reach a cupcake, not just share its batch.
        var tip = body.GetComponent<Collider>();
        if (!tip || !well.GetComponent<Collider>().bounds.Intersects(tip.bounds)) return false;
        DonenessChecked = true;
        probe.ShowResult(BakeResult == OvenBakeResult.Underbaked);
        RecipeFeedback.Report(BakeResult == OvenBakeResult.Underbaked ? "Wet batter on the tester: cupcakes are underbaked." : BakeResult == OvenBakeResult.Perfect ? "Tester comes out clean. Move each cupcake to the cooling rack." : "Tester is dry, but the cupcakes are overcooked. Move them to the rack to finish this practice batch.");
        return true;
    }
    public void ApplyBakeResult(OvenBakeResult result,string ingredientName,Color color)
    {
        BakeCompleted = true; BakeResult = result;
        foreach (var portion in portions) if (portion.IsFilled) portion.Bake(result,ingredientName,color);
    }
}
