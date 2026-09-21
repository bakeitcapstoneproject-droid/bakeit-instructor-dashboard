using UnityEngine;

/// <summary>
/// Scene marker for the physical recipe paper shown at the start of a session.
/// Recipe availability remains explicit on each row. A partially implemented
/// dish may be exposed only when its guide has a clear development boundary.
/// </summary>
public sealed class RecipeSelectionClipboard : MonoBehaviour
{
    [SerializeField] private RecipeDefinition[] recipes;

    public bool TryGetRecipe(string recipeId, out RecipeDefinition recipe)
    {
        if (!string.IsNullOrWhiteSpace(recipeId) && recipes != null)
        {
            foreach (RecipeDefinition candidate in recipes)
            {
                if (candidate != null && string.Equals(
                        candidate.RecipeId,
                        recipeId.Trim(),
                        System.StringComparison.OrdinalIgnoreCase))
                {
                    recipe = candidate;
                    return true;
                }
            }
        }

        recipe = null;
        return false;
    }
}
