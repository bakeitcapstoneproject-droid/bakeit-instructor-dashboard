using UnityEngine;

// Presentation only. Exact quantities and order are owned by MixingSequence.
public sealed class StagedMixingVisual : MonoBehaviour
{
    [SerializeField] private PourableIngredientReceiver receiver;
    [SerializeField] private Transform fill;
    private Material material;
    private PourableIngredient drySource;
    private void Awake()
    {
        if (fill) material = fill.GetComponent<Renderer>().material;
        drySource = GetComponent<PourableIngredient>();
    }
    private void LateUpdate()
    {
        if (!receiver || !fill || !receiver.StagedProcess) return;
        var sequence = receiver.StagedProcess;
        bool dry = receiver.BowlRole == "Dry";
        int amount = dry
            ? receiver.GetTotalIngredientAmount("Flour") + receiver.GetTotalIngredientAmount("BakingPowder") + receiver.GetTotalIngredientAmount("Salt")
            : receiver.GetTotalIngredientAmount("CupcakeButter") + receiver.GetTotalIngredientAmount("Sugar") + receiver.GetTotalIngredientAmount("Oil") + receiver.GetTotalIngredientAmount("Vanilla") + receiver.GetTotalIngredientAmount("Egg") + receiver.GetTotalIngredientAmount("Milk") + receiver.GetTotalIngredientAmount("CupcakeDryBlend");
        float fraction = dry && sequence.PhaseIndex > 0 ? sequence.DryPortionsRemaining / 2f : Mathf.Clamp01(amount / (dry ? 5f : 14f));
        if (dry && drySource && sequence.HasDryBlend) fraction = Mathf.Max(0, fraction - drySource.PourProgress * .5f);
        if (!dry && drySource && drySource.CupcakeBatterSource && sequence.IsComplete)
            fraction *= Mathf.Max(0, drySource.CupcakeBatterSource.RemainingBatterVolume) / 12f;
        fill.gameObject.SetActive(amount > 0 && fraction > 0);
        fill.localPosition = new Vector3(0, Mathf.Lerp(.035f, .069f, fraction), 0);
        float width = Mathf.Lerp(.095f, .16f, fraction);
        fill.localScale = new Vector3(width, .014f, width);
        if (material) material.color = dry ? new Color(.92f,.88f,.75f) : Color.Lerp(new Color(.97f,.77f,.35f), new Color(.93f,.83f,.60f), Mathf.Clamp01(sequence.PhaseIndex / 8f));
    }
    private void OnDestroy() { if (material) Destroy(material); }
}
