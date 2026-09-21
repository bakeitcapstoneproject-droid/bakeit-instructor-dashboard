using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class OvenDisplay : MonoBehaviour
{
    [SerializeField] private OvenController oven;
    [SerializeField] private Color readyColor =
        new Color(0.3f, 1f, 0.8f, 1f);
    [SerializeField] private Color bakingColor =
        new Color(1f, 0.65f, 0.15f, 1f);
    [SerializeField] private Color warningColor =
        new Color(1f, 0.25f, 0.2f, 1f);

    private TextMesh displayText;
    private string lastText;
    private Color lastColor;

    private void Awake()
    {
        displayText = GetComponent<TextMesh>();

        if (oven == null)
            oven = GetComponentInParent<OvenController>();

        if (oven == null)
            oven = FindAnyObjectByType<OvenController>();

        ConfigureTextMesh();
        RefreshDisplay();
    }

    private void Update()
    {
        RefreshDisplay();
    }

    private void ConfigureTextMesh()
    {
        displayText.anchor = TextAnchor.MiddleCenter;
        displayText.alignment = TextAlignment.Center;
        displayText.fontSize = 64;
        displayText.characterSize = 0.012f;
        displayText.richText = false;
    }

    private void RefreshDisplay()
    {
        string nextText;
        Color nextColor;

        if (oven == null)
        {
            nextText = "NO OVEN";
            nextColor = warningColor;
        }
        else if ((oven.IsBaking || oven.IsPreheating) &&
                 oven.IsPausedForOpenDoor)
        {
            nextText =
                $"{oven.CurrentTemperatureCelsius:0}\u00B0C\nCLOSE DOOR";
            nextColor = warningColor;
        }
        else if (oven.IsPreheating)
        {
            nextText =
                $"{oven.CurrentTemperatureCelsius:0}\u00B0C\nPREHEAT";
            nextColor = bakingColor;
        }
        else if (oven.IsBaking)
        {
            int seconds = Mathf.CeilToInt(oven.TimeRemaining);
            nextText =
                $"{oven.CurrentTemperatureCelsius:0}\u00B0C\n00:{seconds:00}";
            nextColor = bakingColor;
        }
        else if (oven.IsPreheated)
        {
            nextText =
                $"{oven.CurrentTemperatureCelsius:0}\u00B0C\nREADY";
            nextColor = readyColor;
        }
        else
        {
            nextText =
                $"{oven.TemperatureCelsius:0}\u00B0C\nSET TEMP";
            nextColor = readyColor;
        }

        if (nextText == lastText && nextColor == lastColor)
            return;

        displayText.text = nextText;
        displayText.color = nextColor;
        lastText = nextText;
        lastColor = nextColor;
    }
}
