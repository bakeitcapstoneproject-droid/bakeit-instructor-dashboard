using UnityEngine;

[RequireComponent(typeof(Light))]
public class OvenInteriorLight : MonoBehaviour
{
    [SerializeField] private OvenController oven;
    [SerializeField] private OvenDoorController ovenDoor;
    [SerializeField] private Renderer glowSurface;
    [SerializeField] private bool illuminateWhenDoorIsOpen = true;
    [SerializeField] private Color warmLightColor =
        new Color(1f, 0.55f, 0.22f, 1f);
    [ColorUsage(false, true)]
    [SerializeField] private Color glowEmissionColor =
        new Color(4f, 1.2f, 0.15f, 1f);
    [SerializeField] private float lightIntensity = 3f;
    [SerializeField] private float lightRange = 2f;

    private Light interiorLight;
    private Material glowMaterial;
    private bool? lastLightState;

    private void Awake()
    {
        interiorLight = GetComponent<Light>();

        if (oven == null)
            oven = GetComponentInParent<OvenController>();

        if (oven == null)
            oven = FindAnyObjectByType<OvenController>();

        if (ovenDoor == null)
            ovenDoor = GetComponentInParent<OvenDoorController>();

        if (ovenDoor == null)
            ovenDoor = FindAnyObjectByType<OvenDoorController>();

        interiorLight.type = LightType.Point;
        interiorLight.color = warmLightColor;
        interiorLight.intensity = lightIntensity;
        interiorLight.range = lightRange;
        interiorLight.shadows = LightShadows.Soft;

        if (glowSurface != null)
        {
            glowMaterial = glowSurface.material;

            if (glowMaterial.HasProperty("_EmissionColor"))
                glowMaterial.EnableKeyword("_EMISSION");
        }

        RefreshLight();
    }

    private void Update()
    {
        RefreshLight();
    }

    private void RefreshLight()
    {
        bool doorOpen =
            illuminateWhenDoorIsOpen &&
            ovenDoor != null &&
            !ovenDoor.IsClosed;

        bool shouldBeOn =
            (oven != null &&
             (oven.IsPreheating || oven.IsPreheated || oven.IsBaking)) ||
            doorOpen;

        if (lastLightState == shouldBeOn)
            return;

        lastLightState = shouldBeOn;
        interiorLight.enabled = shouldBeOn;

        if (glowMaterial != null &&
            glowMaterial.HasProperty("_EmissionColor"))
        {
            glowMaterial.SetColor(
                "_EmissionColor",
                shouldBeOn ? glowEmissionColor : Color.black);
        }
    }

    private void OnValidate()
    {
        lightIntensity = Mathf.Max(0f, lightIntensity);
        lightRange = Mathf.Max(0.1f, lightRange);
    }
}
