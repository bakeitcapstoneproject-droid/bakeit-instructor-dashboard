using System;
using System.Collections.Generic;
using UnityEngine;

public enum RecipeIngredientStage
{
    Base,
    Finishing,
    Decoration
}

public enum RecipeMeasureTool
{
    WholeItem = 0,
    MeasuringCup = 1,
    SmallMeasuringSpoon = 2,
    OneCup = 3,
    HalfCup = 4,
    QuarterCup = 5,
    OneTeaspoon = 6,
    HalfTeaspoon = 7,
    QuarterTeaspoon = 8,
    ChocolateChipScoop = 9,
    OneTablespoon = 10
}

public enum RecipeShapingMethod
{
    RollAndPortion,
    PourIntoPan,
    FillCupcakeLiners
}

public enum RecipePanPreparation
{
    Parchment,
    GreasedPan,
    CupcakeLiners
}

public enum RecipePostBakeFinish
{
    None,
    Icing
}

[Serializable]
public sealed class RecipeIngredientRequirement
{
    [SerializeField] private string ingredientId;
    [SerializeField] private string displayName;
    [SerializeField, Min(1)] private int requiredQuantity = 1;
    [SerializeField] private string unitLabel = "portion";
    [SerializeField] private RecipeIngredientStage stage;
    [SerializeField] private RecipeMeasureTool requiredTool;

    public string IngredientId => ingredientId;
    public string DisplayName => string.IsNullOrWhiteSpace(displayName)
        ? ingredientId
        : displayName;
    public int RequiredQuantity => Mathf.Max(1, requiredQuantity);
    public string UnitLabel => string.IsNullOrWhiteSpace(unitLabel)
        ? "portion"
        : unitLabel;
    public RecipeIngredientStage Stage => stage;
    public RecipeMeasureTool RequiredTool => requiredTool;

    public bool Matches(string value)
    {
        return !string.IsNullOrWhiteSpace(value) &&
               string.Equals(
                   ingredientId?.Trim(),
                   value.Trim(),
                   StringComparison.OrdinalIgnoreCase);
    }

    public string GetGuideText()
    {
        string amount;
        if (RequiredQuantity > 1 &&
            UnitLabel.StartsWith("1 ", StringComparison.OrdinalIgnoreCase))
        {
            string unit = UnitLabel.Substring(2).Trim();
            amount = $"{RequiredQuantity} {unit}" +
                (unit.EndsWith("s", StringComparison.OrdinalIgnoreCase) ? string.Empty : "s");
        }
        else
        {
            amount = RequiredQuantity == 1 &&
                        UnitLabel.Length > 0 &&
                        char.IsDigit(UnitLabel[0])
            ? UnitLabel
            : $"{RequiredQuantity} {UnitLabel}" +
              (RequiredQuantity == 1 || UnitLabel.EndsWith("s", StringComparison.OrdinalIgnoreCase)
                  ? string.Empty
                  : "s");
        }

        return RequiredTool == RecipeMeasureTool.WholeItem
            ? $"{DisplayName}: {amount}"
            : $"{DisplayName}: {amount}; tilt into bowl";
    }
}

[CreateAssetMenu(
    fileName = "RecipeDefinition",
    menuName = "BakeIT/Recipe Definition")]
public sealed class RecipeDefinition : ScriptableObject
{
    [Header("Ordered mixing (optional)")]
    [SerializeField] private RecipeMixingPhase[] mixingPhases = Array.Empty<RecipeMixingPhase>();
    public IReadOnlyList<RecipeMixingPhase> MixingPhases => mixingPhases ?? Array.Empty<RecipeMixingPhase>();
    [SerializeField] private bool batterPracticeOnly;
    public bool BatterPracticeOnly => batterPracticeOnly;
    [Header("Identity")]
    [SerializeField] private string recipeId = "recipe";
    [SerializeField] private string displayName = "Recipe";
    [SerializeField] private string productSingular = "item";
    [SerializeField] private string productPlural = "items";

    [Header("Ingredients")]
    [SerializeField] private RecipeIngredientRequirement[] ingredients =
        Array.Empty<RecipeIngredientRequirement>();

    [Header("Process")]
    [SerializeField, Min(1)] private int baseMixPasses = 1;
    [SerializeField, Min(0)] private int finishingMixPasses;
    [SerializeField] private RecipeShapingMethod shapingMethod;
    [SerializeField, Min(0)] private int shapingPasses;
    [SerializeField, Min(1)] private int targetPortionCount = 1;
    [SerializeField, Min(1)] private int minimumPortionCount = 1;
    [SerializeField, Min(1)] private int maximumPortionCount = 1;
    [SerializeField] private RecipePanPreparation panPreparation;
    [SerializeField] private RecipePostBakeFinish postBakeFinish;

    [Header("Runtime Ingredient Names")]
    [SerializeField] private string mixedProductIngredientName = "Dough";
    [SerializeField] private string shapedProductIngredientName = "FlattenedDough";
    [SerializeField] private string portionIngredientName = "Portion";
    [SerializeField] private string underbakedIngredientName = "UnderbakedItem";
    [SerializeField] private string bakedIngredientName = "BakedItem";
    [SerializeField] private string overcookedIngredientName = "OvercookedItem";
    [SerializeField] private string burntIngredientName = "BurntItem";

    [Header("Baking")]
    [SerializeField, Min(0.1f)] private float bakeDurationSeconds = 5f;
    [SerializeField, Min(0f)] private float minimumPerfectTemperature = 170f;
    [SerializeField, Min(0f)] private float maximumPerfectTemperature = 190f;
    [SerializeField, Min(0f)] private float minimumBurntTemperature = 220f;

    public string RecipeId => recipeId;
    public string DisplayName => displayName;
    public string ProductSingular => productSingular;
    public string ProductPlural => productPlural;
    public IReadOnlyList<RecipeIngredientRequirement> Ingredients =>
        ingredients ?? Array.Empty<RecipeIngredientRequirement>();
    public int BaseMixPasses => Mathf.Max(1, baseMixPasses);
    public int FinishingMixPasses => Mathf.Max(0, finishingMixPasses);
    public RecipeShapingMethod ShapingMethod => shapingMethod;
    public int ShapingPasses => Mathf.Max(0, shapingPasses);
    public int TargetPortionCount => Mathf.Max(1, targetPortionCount);
    public int MinimumPortionCount => Mathf.Max(1, minimumPortionCount);
    public int MaximumPortionCount => Mathf.Max(MinimumPortionCount, maximumPortionCount);
    public RecipePanPreparation PanPreparation => panPreparation;
    public RecipePostBakeFinish PostBakeFinish => postBakeFinish;
    public string MixedProductIngredientName => mixedProductIngredientName;
    public string ShapedProductIngredientName => shapedProductIngredientName;
    public string PortionIngredientName => portionIngredientName;
    public string UnderbakedIngredientName => underbakedIngredientName;
    public string BakedIngredientName => bakedIngredientName;
    public string OvercookedIngredientName => overcookedIngredientName;
    public string BurntIngredientName => burntIngredientName;
    public float BakeDurationSeconds => Mathf.Max(0.1f, bakeDurationSeconds);
    public float MinimumPerfectTemperature => minimumPerfectTemperature;
    public float MaximumPerfectTemperature => Mathf.Max(
        minimumPerfectTemperature,
        maximumPerfectTemperature);
    public float MinimumBurntTemperature => Mathf.Max(
        MaximumPerfectTemperature,
        minimumBurntTemperature);

    public bool TryGetIngredient(
        string ingredientId,
        out RecipeIngredientRequirement requirement)
    {
        foreach (RecipeIngredientRequirement candidate in Ingredients)
        {
            if (candidate != null && candidate.Matches(ingredientId))
            {
                requirement = candidate;
                return true;
            }
        }

        requirement = null;
        return false;
    }

    public bool TryGetIngredientForTool(
        string ingredientId,
        RecipeMeasureTool tool,
        out RecipeIngredientRequirement requirement)
    {
        foreach (RecipeIngredientRequirement candidate in Ingredients)
        {
            if (candidate == null || !candidate.Matches(ingredientId))
                continue;

            RecipeMeasureTool requiredTool = candidate.RequiredTool;
            if (requiredTool == tool ||
                (requiredTool == RecipeMeasureTool.MeasuringCup &&
                 tool == RecipeMeasureTool.OneCup) ||
                (requiredTool == RecipeMeasureTool.SmallMeasuringSpoon &&
                 tool == RecipeMeasureTool.ChocolateChipScoop))
            {
                requirement = candidate;
                return true;
            }
        }

        requirement = null;
        return false;
    }

    public IEnumerable<RecipeIngredientRequirement> GetIngredients(
        string ingredientId)
    {
        foreach (RecipeIngredientRequirement requirement in Ingredients)
        {
            if (requirement != null && requirement.Matches(ingredientId))
                yield return requirement;
        }
    }

    public IEnumerable<RecipeIngredientRequirement> GetIngredients(
        RecipeIngredientStage stage)
    {
        foreach (RecipeIngredientRequirement requirement in Ingredients)
        {
            if (requirement != null && requirement.Stage == stage)
                yield return requirement;
        }
    }

    public RecipeIngredientRequirement GetFirstIngredient(
        RecipeIngredientStage stage)
    {
        foreach (RecipeIngredientRequirement requirement in GetIngredients(stage))
            return requirement;

        return null;
    }

    public string GetResultIngredientName(OvenBakeResult result)
    {
        return result switch
        {
            OvenBakeResult.Underbaked => underbakedIngredientName,
            OvenBakeResult.Perfect => bakedIngredientName,
            OvenBakeResult.Overcooked => overcookedIngredientName,
            OvenBakeResult.Burnt => burntIngredientName,
            _ => portionIngredientName
        };
    }

    public string GetToolDisplayName(RecipeMeasureTool tool)
    {
        return tool switch
        {
            RecipeMeasureTool.MeasuringCup => "Measuring Cup",
            RecipeMeasureTool.SmallMeasuringSpoon => "Small Measuring Spoon",
            RecipeMeasureTool.OneCup => "1 Cup Measuring Cup",
            RecipeMeasureTool.HalfCup => "1/2 Cup Measuring Cup",
            RecipeMeasureTool.QuarterCup => "1/4 Cup Measuring Cup",
            RecipeMeasureTool.OneTeaspoon => "1 Teaspoon",
            RecipeMeasureTool.HalfTeaspoon => "1/2 Teaspoon",
            RecipeMeasureTool.QuarterTeaspoon => "1/4 Teaspoon",
            RecipeMeasureTool.OneTablespoon => "1 Tablespoon",
            RecipeMeasureTool.ChocolateChipScoop => "Chocolate Chip Scoop",
            _ => "whole ingredient"
        };
    }

    private void OnValidate()
    {
        recipeId = string.IsNullOrWhiteSpace(recipeId)
            ? name
            : recipeId.Trim();
        displayName = string.IsNullOrWhiteSpace(displayName)
            ? name
            : displayName.Trim();
        productSingular = string.IsNullOrWhiteSpace(productSingular)
            ? "item"
            : productSingular.Trim();
        productPlural = string.IsNullOrWhiteSpace(productPlural)
            ? productSingular + "s"
            : productPlural.Trim();
        baseMixPasses = Mathf.Max(1, baseMixPasses);
        finishingMixPasses = Mathf.Max(0, finishingMixPasses);
        shapingPasses = Mathf.Max(0, shapingPasses);
        targetPortionCount = Mathf.Max(1, targetPortionCount);
        minimumPortionCount = Mathf.Max(1, minimumPortionCount);
        maximumPortionCount = Mathf.Max(minimumPortionCount, maximumPortionCount);
        bakeDurationSeconds = Mathf.Max(0.1f, bakeDurationSeconds);
        minimumPerfectTemperature = Mathf.Max(0f, minimumPerfectTemperature);
        maximumPerfectTemperature = Mathf.Max(
            minimumPerfectTemperature,
            maximumPerfectTemperature);
        minimumBurntTemperature = Mathf.Max(
            maximumPerfectTemperature,
            minimumBurntTemperature);
    }
}
