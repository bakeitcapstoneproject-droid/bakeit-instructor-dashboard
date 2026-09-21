using System;
using UnityEngine;

public enum MixingAction { Mix, Scrape }

[Serializable]
public sealed class RecipeMixingPhase
{
    [SerializeField] private string title;
    [SerializeField] private string bowlRole;
    [SerializeField] private RecipeIngredientRequirement[] ingredients = Array.Empty<RecipeIngredientRequirement>();
    [SerializeField] private MixingAction action;
    [SerializeField, Min(1)] private int passes = 1;
    public string Title => title;
    public string BowlRole => bowlRole;
    public RecipeIngredientRequirement[] Ingredients => ingredients ?? Array.Empty<RecipeIngredientRequirement>();
    public MixingAction Action => action;
    public int Passes => Mathf.Max(1, passes);
}
