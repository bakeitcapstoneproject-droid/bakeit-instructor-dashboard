using UnityEngine;

// Deliberate released placement prepares an item; hovering/early consumption do not.
[DisallowMultipleComponent]
public sealed class MiseEnPlaceStation : MonoBehaviour
{
    [Header("Ingredient and Tool Preparation")]
    [SerializeField] private Transform egg;
    [SerializeField] private Transform butter;
    [SerializeField] private Transform smallSpoon;
    [SerializeField] private Transform whisk;

    [SerializeField] private Transform flour, sugar, chocolateChips;
    [SerializeField] private Transform largeSpoon, bowl;
    [SerializeField] private Transform vanilla, bakingSoda, salt;
    [SerializeField] private Transform halfCup, quarterCup;
    [SerializeField] private Transform oneTeaspoon, halfTeaspoon, quarterTeaspoon;
    [SerializeField] private StablePlacementSurface ingredientRest, toolRest, bowlRest;
    [Header("Brownie Preparation")]
    [SerializeField] private Transform[] brownieEggs;
    [SerializeField] private Transform meltedButterCup, cocoa, bakingPowder, walnuts;
    [SerializeField] private Transform brownieBowl;
    private bool workspaceReady;
    private bool brownieWorkspaceReady;

    public bool EggPrepared => Prepared(egg, ingredientRest);
    public bool ButterPrepared => Prepared(butter, ingredientRest);
    public bool SmallSpoonPrepared => Prepared(smallSpoon, toolRest);
    public bool LargeSpoonPrepared => Prepared(largeSpoon, toolRest);
    public bool WhiskPrepared => Prepared(whisk, toolRest);
    public bool FlourPrepared => Prepared(flour, ingredientRest);
    public bool SugarPrepared => Prepared(sugar, ingredientRest);
    public bool ChipsPrepared => Prepared(chocolateChips, ingredientRest);
    public bool VanillaPrepared => Prepared(vanilla, ingredientRest);
    public bool BakingSodaPrepared => Prepared(bakingSoda, ingredientRest);
    public bool SaltPrepared => Prepared(salt, ingredientRest);
    public bool HalfCupPrepared => Prepared(halfCup, toolRest);
    public bool QuarterCupPrepared => Prepared(quarterCup, toolRest);
    public bool OneTeaspoonPrepared => Prepared(oneTeaspoon, toolRest);
    public bool HalfTeaspoonPrepared => Prepared(halfTeaspoon, toolRest);
    public bool QuarterTeaspoonPrepared => Prepared(quarterTeaspoon, toolRest);
    public bool BowlPlaced => OnSurface(bowl, bowlRest);
    public bool ToolsPrepared => LargeSpoonPrepared && HalfCupPrepared &&
        OneTeaspoonPrepared &&
        HalfTeaspoonPrepared && QuarterTeaspoonPrepared && WhiskPrepared;
    public bool IngredientsPrepared => EggPrepared && ButterPrepared && FlourPrepared &&
        SugarPrepared && ChipsPrepared && VanillaPrepared && BakingSodaPrepared && SaltPrepared;
    public bool ColdIngredientsPrepared => EggPrepared && ButterPrepared && ChipsPrepared;
    public bool DryIngredientsPrepared => FlourPrepared && SugarPrepared && VanillaPrepared &&
        BakingSodaPrepared && SaltPrepared;
    public bool WhiskOnUtensilsArea => OnSurface(whisk, toolRest);
    public bool IsPrepared => workspaceReady;
    public int BrownieEggsPreparedCount => brownieWorkspaceReady
        ? brownieEggs != null ? brownieEggs.Length : 0
        : CountPrepared(brownieEggs, ingredientRest);
    public int BrownieEggCount => brownieEggs != null ? brownieEggs.Length : 0;
    public bool BrownieColdIngredientsPrepared =>
        BrownieEggCount >= 4 && BrownieEggsPreparedCount >= 4;
    public bool BrownieDryIngredientsPrepared =>
        PreparedFor(brownieWorkspaceReady, flour, ingredientRest) &&
        PreparedFor(brownieWorkspaceReady, sugar, ingredientRest) &&
        PreparedFor(brownieWorkspaceReady, meltedButterCup, ingredientRest) &&
        PreparedFor(brownieWorkspaceReady, cocoa, ingredientRest) &&
        PreparedFor(brownieWorkspaceReady, vanilla, ingredientRest) &&
        PreparedFor(brownieWorkspaceReady, bakingPowder, ingredientRest) &&
        PreparedFor(brownieWorkspaceReady, salt, ingredientRest) &&
        PreparedFor(brownieWorkspaceReady, walnuts, ingredientRest);
    public bool BrownieToolsPrepared =>
        PreparedFor(brownieWorkspaceReady, largeSpoon, toolRest) &&
        PreparedFor(brownieWorkspaceReady, halfCup, toolRest) &&
        PreparedFor(brownieWorkspaceReady, quarterCup, toolRest) &&
        PreparedFor(brownieWorkspaceReady, oneTeaspoon, toolRest) &&
        PreparedFor(brownieWorkspaceReady, halfTeaspoon, toolRest) &&
        PreparedFor(brownieWorkspaceReady, whisk, toolRest);
    public bool BrownieBowlPlaced => OnSurface(brownieBowl, bowlRest);
    public bool BrownieIsPrepared => brownieWorkspaceReady;
    public Transform Egg => egg;
    public Transform Butter => butter;
    public Transform ChocolateChips => chocolateChips;
    public Transform Flour => flour;
    public Transform Sugar => sugar;
    public Transform Vanilla => vanilla;
    public Transform BakingSoda => bakingSoda;
    public Transform Salt => salt;
    public Transform OneCup => largeSpoon;
    public Transform HalfCup => halfCup;
    public Transform OneTeaspoon => oneTeaspoon;
    public Transform HalfTeaspoon => halfTeaspoon;
    public Transform QuarterTeaspoon => quarterTeaspoon;
    public Transform Whisk => whisk;
    public Transform WhiteCookieBowl => bowl;
    public Transform BrownieMixingBowl => brownieBowl;
    public Transform MeltedButterCup => meltedButterCup;
    public Transform Cocoa => cocoa;
    public Transform BakingPowder => bakingPowder;
    public Transform Walnuts => walnuts;
    public StablePlacementSurface IngredientRest => ingredientRest;
    public StablePlacementSurface ToolRest => toolRest;
    public StablePlacementSurface BowlRest => bowlRest;
    private void Update()
    {
        workspaceReady |= IngredientsPrepared && ToolsPrepared && BowlPlaced;
        brownieWorkspaceReady |= BrownieColdIngredientsPrepared &&
            BrownieDryIngredientsPrepared && BrownieToolsPrepared && BrownieBowlPlaced;
    }

    public bool SuppliesOnBoard => OnBoard(flour) || OnBoard(sugar) || OnBoard(chocolateChips) ||
        OnBoard(vanilla) || OnBoard(bakingSoda) || OnBoard(salt) || OnBoard(smallSpoon) ||
        OnBoard(largeSpoon) || OnBoard(halfCup) || OnBoard(quarterCup) || OnBoard(oneTeaspoon) ||
        OnBoard(halfTeaspoon) || OnBoard(quarterTeaspoon) || OnBoard(whisk) || OnBoard(bowl);
    // Readiness latches only after every supply is resting in its correct area.
    // Consuming food or picking up a prepared tool must not undo that milestone.
    private bool Prepared(Transform t, StablePlacementSurface surface) => workspaceReady || OnSurface(t, surface);
    private static bool PreparedFor(
        bool workspaceLatched,
        Transform item,
        StablePlacementSurface surface) => workspaceLatched || OnSurface(item, surface);

    private static int CountPrepared(
        Transform[] items,
        StablePlacementSurface surface)
    {
        if (items == null)
            return 0;
        int count = 0;
        foreach (Transform item in items)
            if (OnSurface(item, surface)) count++;
        return count;
    }
    private static bool OnSurface(Transform t, StablePlacementSurface surface)
    {
        var item = t != null ? t.GetComponent<PreparationItem>() : null;
        return surface != null && item != null && item.RestingSurface == surface;
    }
    private static bool OnBoard(Transform t)
    {
        var item = t != null ? t.GetComponent<PreparationItem>() : null;
        return item != null && item.RestingSurface != null && item.RestingSurface.IsPreparationBoard;
    }

    public bool TryGetExpectedSurface(
        PreparationItem item,
        out StablePlacementSurface surface)
    {
        return TryGetExpectedSurface(item, null, out surface);
    }

    public bool TryGetExpectedSurface(
        PreparationItem item,
        RecipeDefinition recipe,
        out StablePlacementSurface surface)
    {
        surface = null;
        if (item == null)
            return false;

        Transform target = item.transform;
        bool brownies = recipe != null && string.Equals(
            recipe.RecipeId, "brownies", System.StringComparison.OrdinalIgnoreCase);
        if (brownies)
        {
            if (MatchesAny(target, brownieEggs) || Matches(target, flour) ||
                Matches(target, sugar) || Matches(target, meltedButterCup) ||
                Matches(target, cocoa) || Matches(target, vanilla) ||
                Matches(target, bakingPowder) || Matches(target, salt) ||
                Matches(target, walnuts))
            {
                surface = ingredientRest;
            }
            else if (Matches(target, largeSpoon) || Matches(target, halfCup) ||
                     Matches(target, quarterCup) || Matches(target, oneTeaspoon) ||
                     Matches(target, halfTeaspoon) || Matches(target, whisk))
            {
                surface = toolRest;
            }
            else if (Matches(target, brownieBowl))
            {
                surface = bowlRest;
            }
            return surface != null;
        }

        if (Matches(target, egg) || Matches(target, butter) ||
            Matches(target, chocolateChips) || Matches(target, flour) ||
            Matches(target, sugar) || Matches(target, vanilla) ||
            Matches(target, bakingSoda) || Matches(target, salt))
        {
            surface = ingredientRest;
        }
        else if (Matches(target, largeSpoon) || Matches(target, halfCup) ||
                 Matches(target, oneTeaspoon) || Matches(target, halfTeaspoon) ||
                 Matches(target, quarterTeaspoon) || Matches(target, whisk))
        {
            surface = toolRest;
        }
        else if (Matches(target, bowl))
        {
            surface = bowlRest;
        }

        return surface != null;
    }

    public Transform GetFirstUnpreparedBrownieEgg()
    {
        if (brownieEggs == null)
            return null;
        foreach (Transform candidate in brownieEggs)
            if (!PreparedFor(brownieWorkspaceReady, candidate, ingredientRest))
                return candidate;
        return null;
    }

    public Transform GetBrownieIngredientSource(string ingredientId)
    {
        if (string.IsNullOrWhiteSpace(ingredientId))
            return null;
        switch (ingredientId.Trim().ToLowerInvariant())
        {
            case "egg":
                if (brownieEggs != null)
                    foreach (Transform candidate in brownieEggs)
                        if (candidate != null && candidate.gameObject.activeInHierarchy)
                            return candidate;
                return null;
            case "butter": return butter;
            case "chocolatechips": return chocolateChips;
            case "flour": return flour;
            case "sugar": return sugar;
            case "meltedbutter": return meltedButterCup;
            case "cocoa": return cocoa;
            case "vanilla": return vanilla;
            case "bakingpowder": return bakingPowder;
            case "bakingsoda": return bakingSoda;
            case "salt": return salt;
            case "walnuts": return walnuts;
            default: return null;
        }
    }

    public Transform GetMeasuringTool(RecipeMeasureTool tool)
    {
        return tool switch
        {
            RecipeMeasureTool.OneCup or RecipeMeasureTool.MeasuringCup => largeSpoon,
            RecipeMeasureTool.HalfCup => halfCup,
            RecipeMeasureTool.QuarterCup => quarterCup,
            RecipeMeasureTool.OneTeaspoon => oneTeaspoon,
            RecipeMeasureTool.HalfTeaspoon => halfTeaspoon,
            RecipeMeasureTool.QuarterTeaspoon => quarterTeaspoon,
            _ => null
        };
    }

    private static bool MatchesAny(Transform candidate, Transform[] configured)
    {
        if (configured == null)
            return false;
        foreach (Transform item in configured)
            if (Matches(candidate, item)) return true;
        return false;
    }

    private static bool Matches(Transform candidate, Transform configured)
    {
        return candidate != null && configured != null &&
               (candidate == configured || candidate.IsChildOf(configured) ||
                configured.IsChildOf(candidate));
    }
}
