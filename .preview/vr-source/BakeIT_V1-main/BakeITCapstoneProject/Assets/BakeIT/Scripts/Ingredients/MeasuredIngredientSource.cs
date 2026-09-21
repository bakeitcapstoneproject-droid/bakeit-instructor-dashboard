using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
public class MeasuredIngredientSource : MonoBehaviour
{
    [SerializeField] private string ingredientName = "Flour";
    public string IngredientName => ingredientName;

    public bool TryReturnMeasure(MeasuringScoop scoop)
    {
        if(scoop==null || !scoop.IsHeld || !scoop.IsFilled || !scoop.IsTilted || scoop.FindPourDestination()!=this)return false;
        if(!string.Equals(scoop.CurrentIngredient,ingredientName,System.StringComparison.OrdinalIgnoreCase))
        {
            RecipeFeedback.Warning($"Return {scoop.CurrentIngredient} to its matching container, not {ingredientName}, or hold Q to discard.");
            return false;
        }
        if(!scoop.CompleteTransfer(MeasureDisposition.ReturnedToSource,name,GetComponent<Collider>().bounds.center))return false;
        scoop.BlockRefillUntilLifted(this);
        RecipeFeedback.Report($"Returned {ingredientName}; no waste. Lift clear before refilling.");
        return true;
    }

    private void Reset()
    {
        Collider sourceCollider = GetComponent<Collider>();
        sourceCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryFillScoop(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryFillScoop(other);
    }

    private void TryFillScoop(Collider other)
    {
        MeasuringScoop scoop =
            other.GetComponentInParent<MeasuringScoop>();

        if (scoop != null && scoop.CanFillFrom(this))
            scoop.TryFill(ingredientName);
    }

    private void OnValidate()
    {
        Collider sourceCollider = GetComponent<Collider>();

        if (sourceCollider != null)
            sourceCollider.isTrigger = true;
    }
}
