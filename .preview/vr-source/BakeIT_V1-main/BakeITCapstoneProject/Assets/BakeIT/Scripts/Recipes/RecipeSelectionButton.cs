using UnityEngine;

public sealed class RecipeSelectionButton : MonoBehaviour
{
    [SerializeField] private string recipeId;
    [SerializeField] private string displayName;
    [SerializeField] private bool available;
    [SerializeField] private TextMesh recipeSummary;
    [SerializeField] private string[] ingredientLineIds;
    private string[] plainLines;
    private CookieRecipeSessionController session;

    private void Awake()
    {
        if (recipeSummary != null) plainLines = recipeSummary.text.Split('\n');
    }

    private void LateUpdate()
    {
        if (recipeSummary == null || plainLines == null) return;
        if (session == null) session = FindAnyObjectByType<CookieRecipeSessionController>();
        bool showProgress = session != null && session.IsGuidedPractice && session.HasSelectedRecipe && session.SelectedRecipeId == recipeId;
        string[] lines = (string[])plainLines.Clone();
        for (int i = 0; ingredientLineIds != null && i < ingredientLineIds.Length && i < lines.Length; i++)
            if (showProgress && session.IsIngredientSatisfied(ingredientLineIds[i]))
                lines[i] = "<color=#247C37>" + lines[i] + "</color>";
        string text = string.Join("\n", lines);
        if (recipeSummary.text != text) recipeSummary.text = text;
    }

    public string RecipeId => recipeId;
    public string DisplayName => displayName;
    public bool IsAvailable => available;

    public bool TrySelect()
    {
        string safeDisplayName = string.IsNullOrWhiteSpace(displayName)
            ? "This recipe"
            : displayName;

        if (!available)
        {
            RecipeFeedback.Warning(
                $"{safeDisplayName} is still in development. Choose an available recipe.");
            return true;
        }

        CookieRecipeSessionController session =
            FindAnyObjectByType<CookieRecipeSessionController>();

        if (session == null)
        {
            RecipeFeedback.SystemWarning(
                "The recipe guide is still loading. Try the recipe paper again.");
            return true;
        }

        session.TrySelectRecipe(recipeId, safeDisplayName);
        return true;
    }
}
