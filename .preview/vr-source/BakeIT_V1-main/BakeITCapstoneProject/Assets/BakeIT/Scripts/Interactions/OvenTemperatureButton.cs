using UnityEngine;

// Marker used by OvenController to identify a physical temperature button.
public class OvenTemperatureButton : MonoBehaviour
{
    [Tooltip("Use -1 for temperature down and 1 for temperature up.")]
    [SerializeField] private int direction = 1;

    public int Direction => direction < 0 ? -1 : 1;

    private void OnValidate()
    {
        direction = direction < 0 ? -1 : 1;
    }
}
