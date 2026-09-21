using UnityEngine;

/// <summary>A baked portion from the current batch, eligible for the serving plate.</summary>
[DisallowMultipleComponent]
public sealed class ServingPortion : MonoBehaviour
{
    public string RecipeId { get; private set; }
    public OvenBakeResult BakeResult { get; private set; }
    public bool IsBaked => BakeResult != OvenBakeResult.None;
    public void Configure(string recipeId, OvenBakeResult result)
    {
        RecipeId = recipeId;
        BakeResult = result;
    }
}
