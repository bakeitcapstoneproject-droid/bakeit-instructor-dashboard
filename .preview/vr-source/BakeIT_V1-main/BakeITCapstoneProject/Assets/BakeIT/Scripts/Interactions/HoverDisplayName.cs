using UnityEngine;

[DisallowMultipleComponent]
public sealed class HoverDisplayName : MonoBehaviour
{
    [SerializeField] private string displayName;
    [SerializeField] private Transform labelAnchor;

    public string DisplayName => string.IsNullOrWhiteSpace(displayName)
        ? gameObject.name
        : displayName.Trim();

    public Vector3 WorldAnchor
    {
        get
        {
            if (labelAnchor != null)
                return labelAnchor.position;

            Renderer[] renderers = GetComponentsInChildren<Renderer>(false);
            Bounds bounds = new Bounds(transform.position, Vector3.zero);
            bool found = false;
            foreach (Renderer candidate in renderers)
            {
                if (candidate == null || !candidate.enabled ||
                    candidate.GetComponent<TextMesh>() != null)
                    continue;

                if (!found)
                {
                    bounds = candidate.bounds;
                    found = true;
                }
                else
                {
                    bounds.Encapsulate(candidate.bounds);
                }
            }

            return found
                ? new Vector3(bounds.center.x, bounds.max.y + .06f, bounds.center.z)
                : transform.position + Vector3.up * .12f;
        }
    }
}
