using UnityEngine;

// Shared contract for bowl-to-bowl and bowl-to-lined-well pouring.
public interface IPourDestination
{
    Vector3 TransferTarget { get; }
    bool CanPreviewPour(PourableIngredient source);
    bool TryReceive(PourableIngredient source);
}
