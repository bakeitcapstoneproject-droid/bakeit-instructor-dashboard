using UnityEngine;

// A source container is carryable, but is not a measured Ingredient for the bowl.
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public sealed class PreparationItem : MonoBehaviour
{
    [SerializeField] private Vector3 restingEuler;
    public bool WasPrepared { get; private set; }
    public StablePlacementSurface RestingSurface { get; private set; }
    public Quaternion RestingRotation => Quaternion.Euler(restingEuler.x, transform.eulerAngles.y, restingEuler.z);
    public void PickedUp() => RestingSurface = null;
    public void Placed(StablePlacementSurface surface)
    {
        RestingSurface = surface;
        WasPrepared |= surface.IsPreparationBoard;
    }
}
