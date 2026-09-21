using UnityEngine;

[DisallowMultipleComponent]
public class PickupPose : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Rotation applied relative to the player's camera while this object is held.")]
    private Vector3 heldEulerAngles;

    public Quaternion RotationOffset =>
        Quaternion.Euler(heldEulerAngles);
}
