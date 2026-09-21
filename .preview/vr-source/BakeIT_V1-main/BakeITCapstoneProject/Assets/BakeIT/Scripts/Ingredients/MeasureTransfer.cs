using UnityEngine;

public enum MeasureDisposition { AddedToBowl, ReturnedToSource, Discarded }

// Prototype interaction units, not grams or a grade. Consumers can subscribe later.
public readonly struct MeasureTransfer
{
    public readonly string Ingredient, Tool, Unit, Destination;
    public readonly MeasureDisposition Disposition;
    public readonly float Time;
    public int Quantity => 1;
    public MeasureTransfer(string ingredient, string tool, string unit, string destination, MeasureDisposition disposition)
    {
        Ingredient=ingredient; Tool=tool; Unit=unit; Destination=destination;
        Disposition=disposition; Time=UnityEngine.Time.unscaledTime;
    }
}
