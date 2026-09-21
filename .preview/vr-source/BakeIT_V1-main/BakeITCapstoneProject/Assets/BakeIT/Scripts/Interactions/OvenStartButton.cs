using UnityEngine;

// Marker used by OvenController to identify the physical Start button.
public class OvenStartButton : MonoBehaviour
{
    [SerializeField] private OvenController oven;
    [SerializeField] private TextMesh label;

    private void Awake()
    {
        if (oven == null)
            oven = FindAnyObjectByType<OvenController>();

        if (label == null)
        {
            foreach (TextMesh candidate in
                     FindObjectsByType<TextMesh>(FindObjectsInactive.Include))
            {
                if (candidate.name == "OvenStartButton Label")
                {
                    label = candidate;
                    break;
                }
            }
        }

        RefreshLabel();
    }

    private void Update()
    {
        RefreshLabel();
    }

    private void RefreshLabel()
    {
        if (label == null || oven == null)
            return;

        string nextLabel = oven.IsBaking
            ? "BAKING"
            : oven.IsPreheating
                ? "HEATING"
                : oven.IsPreheated
                    ? "BAKE"
                    : "PREHEAT";

        if (label.text != nextLabel)
            label.text = nextLabel;
    }
}
