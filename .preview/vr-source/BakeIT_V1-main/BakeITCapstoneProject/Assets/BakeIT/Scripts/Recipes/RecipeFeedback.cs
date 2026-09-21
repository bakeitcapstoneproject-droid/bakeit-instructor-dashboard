using System;
using UnityEngine;

public enum RecipeFeedbackTone
{
    Information,
    Warning
}

public static class RecipeFeedback
{
    public static event Action<string, RecipeFeedbackTone> MessageChanged;

    public static void Report(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        Debug.Log(message);
        MessageChanged?.Invoke(message, RecipeFeedbackTone.Information);
    }

    public static void Warning(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        Debug.Log(message);
        MessageChanged?.Invoke(message, RecipeFeedbackTone.Warning);
    }

    public static void SystemWarning(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        Debug.LogWarning(message);
        MessageChanged?.Invoke(message, RecipeFeedbackTone.Warning);
    }
}
