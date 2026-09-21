using System.Collections.Generic;
using UnityEngine;

// Ordered recipe state; ingredient physics remains in the shared receivers.
public sealed class MixingSequence : MonoBehaviour
{
    [SerializeField] private RecipeDefinition recipe;
    [SerializeField] private GameObject supplies;
    [SerializeField] private PreparationItem[] stagedBowls;
    [SerializeField] private StablePlacementSurface[] bowlRests;
    [SerializeField] private GameObject[] otherRecipeBowls;
    [SerializeField] private CupcakeBatch cupcakeBatch;
    public CupcakeBatch CupcakeBatch => cupcakeBatch;
    public bool EquipmentReady => !cupcakeBatch || cupcakeBatch.ReadyToMix;
    private bool initialized;
    private readonly Dictionary<string,int> phaseAmounts = new();
    private readonly Dictionary<string,int> totals = new();
    private CookieRecipeSessionController session;
    private int dryTransfers;
    public RecipeDefinition Recipe => recipe;
    public int PhaseIndex { get; private set; }
    public int Passes { get; private set; }
    public bool IsActive => recipe && session && session.SelectedRecipeId == recipe.RecipeId;
    public bool IsComplete => recipe && PhaseIndex >= recipe.MixingPhases.Count;
    public RecipeMixingPhase Current => !recipe || IsComplete ? null : recipe.MixingPhases[PhaseIndex];
    public bool HasDryBlend => PhaseIndex > 0 && dryTransfers < 2;
    public int DryPortionsRemaining => HasDryBlend ? 2 - dryTransfers : 0;
    private static string Key(string id, RecipeMeasureTool tool) => id + "#" + (int)tool;
    private void Update()
    {
        if (!session) session = FindAnyObjectByType<CookieRecipeSessionController>();
        if (supplies && supplies.activeSelf != IsActive) supplies.SetActive(IsActive);
        if (IsActive && !initialized)
        {
            initialized = true;
            for (int i = 0; stagedBowls != null && i < stagedBowls.Length; i++)
                if (stagedBowls[i] && bowlRests != null && i < bowlRests.Length && bowlRests[i]) stagedBowls[i].Placed(bowlRests[i]);
            foreach (var other in otherRecipeBowls) if (other) other.SetActive(false);
        }
    }
    public int Amount(RecipeIngredientRequirement r) => phaseAmounts.TryGetValue(Key(r.IngredientId,r.RequiredTool), out int n) ? n : 0;
    public int Total(string id, RecipeMeasureTool tool) => totals.TryGetValue(Key(id,tool), out int n) ? n : 0;
    public bool CanReceive(string role, string id, RecipeMeasureTool tool)
    {
        if (!IsActive || !EquipmentReady || Current == null || Current.BowlRole != role) return false;
        foreach (var r in Current.Ingredients)
            if (r.Matches(id) && r.RequiredTool == tool && Amount(r) < r.RequiredQuantity) return true;
        return false;
    }
    public void Record(string id, RecipeMeasureTool tool)
    {
        string key = Key(id,tool);
        phaseAmounts.TryGetValue(key,out int count); phaseAmounts[key] = count+1;
        totals.TryGetValue(key,out count); totals[key] = count+1;
        RecipeFeedback.Report(id + " added. " + Current.Title);
    }
    public bool InputsReady
    {
        get { if (Current == null) return false; foreach(var r in Current.Ingredients) if(Amount(r)<r.RequiredQuantity)return false; return true; }
    }
    public bool TryWork(string role, MixingAction action)
    {
        if (!IsActive || !EquipmentReady || Current == null || Current.BowlRole != role || Current.Action != action)
        { RecipeFeedback.Warning("Use the indicated tool in the bowl named by the current step."); return false; }
        if (!InputsReady) { RecipeFeedback.Warning("Add the ingredients for this step before mixing."); return false; }
        Passes++;
        if (Passes >= Current.Passes) { PhaseIndex++; Passes=0; phaseAmounts.Clear(); }
        RecipeFeedback.Report(IsComplete ? cupcakeBatch ? "Batter ready. Tilt the bowl over each lined cupcake well to fill it three-quarters." : "Cupcake batter ready. Baking and frosting are the next development stage." : Current.Title);
        return true;
    }
    public void ConsumeDryPortion() { dryTransfers = Mathf.Min(2, dryTransfers + 1); }
    public void ResetProcess() { PhaseIndex=0; Passes=0; dryTransfers=0; phaseAmounts.Clear(); totals.Clear(); }
}
