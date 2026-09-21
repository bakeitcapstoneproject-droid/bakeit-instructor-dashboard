using UnityEngine;

[DisallowMultipleComponent]
public sealed class HoverLabelController : MonoBehaviour
{
    [SerializeField, Min(.5f)] private float maximumDistance = 3f;

    private Camera playerCamera;
    private PickupController pickupController;
    private HoverDisplayName currentTarget;
    private GUIStyle labelStyle;
    private GUIStyle shadowStyle;

    public string CurrentLabel => currentTarget != null
        ? currentTarget.DisplayName
        : string.Empty;

    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        pickupController = GetComponent<PickupController>();
    }

    private void Update()
    {
        RefreshNow();
    }

    public void RefreshNow()
    {
        if (pickupController == null)
            pickupController = GetComponent<PickupController>();

        if (pickupController != null && pickupController.HeldObject != null)
        {
            currentTarget = null;
            return;
        }

        currentTarget = FindDirectTarget();
    }

    private HoverDisplayName FindDirectTarget()
    {
        if (playerCamera == null)
            return null;

        RaycastHit[] hits = Physics.RaycastAll(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            maximumDistance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            Collider collider = hit.collider;
            if (collider == null || collider.transform.IsChildOf(transform))
                continue;

            HoverDisplayName label = collider.GetComponentInParent<HoverDisplayName>();
            if (label != null)
                return label;

            if (!collider.isTrigger)
                return null;
        }

        return null;
    }

    private void OnGUI()
    {
        if (currentTarget == null || playerCamera == null)
            return;

        EnsureStyles();
        Vector3 screen = playerCamera.WorldToScreenPoint(currentTarget.WorldAnchor);
        if (screen.z <= 0f)
            return;

        GUIContent content = new GUIContent(currentTarget.DisplayName);
        Vector2 size = labelStyle.CalcSize(content);
        size.x = Mathf.Max(110f, size.x + 28f);
        size.y = Mathf.Max(34f, size.y + 12f);
        Rect rect = new Rect(
            Mathf.Clamp(screen.x - size.x * .5f, 8f, Screen.width - size.x - 8f),
            Mathf.Clamp(Screen.height - screen.y - size.y, 8f, Screen.height - size.y - 8f),
            size.x,
            size.y);

        Color previous = GUI.color;
        GUI.color = new Color(.08f, .055f, .035f, .92f);
        GUI.Box(rect, GUIContent.none);
        GUI.color = previous;
        GUI.Label(new Rect(rect.x + 1f, rect.y + 2f, rect.width, rect.height), content, shadowStyle);
        GUI.Label(rect, content, labelStyle);
    }

    private void EnsureStyles()
    {
        if (labelStyle != null)
            return;

        labelStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 18,
            fontStyle = FontStyle.Bold
        };
        labelStyle.normal.textColor = new Color(1f, .88f, .55f);
        shadowStyle = new GUIStyle(labelStyle);
        shadowStyle.normal.textColor = new Color(0f, 0f, 0f, .75f);
    }
}
