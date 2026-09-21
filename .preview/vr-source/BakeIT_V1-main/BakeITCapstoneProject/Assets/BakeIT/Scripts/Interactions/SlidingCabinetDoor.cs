using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public sealed class SlidingCabinetDoor : MonoBehaviour
{
    [SerializeField] private Vector3 openOffset = new Vector3(-0.72f, 0, 0);
    [SerializeField, Min(0.01f)] private float speed = 0.65f;
    private Vector3 closedPosition;
    private Rigidbody body;
    private BoxCollider box;
    private bool open;
    private float nextWarning;
    public bool IsOpen => open && !IsMoving;
    public bool IsMoving => Vector3.Distance(transform.localPosition, Target) > 0.001f;
    private Vector3 Target => closedPosition + (open ? openOffset : Vector3.zero);
    private void Awake()
    {
        closedPosition = transform.localPosition;
        body = GetComponent<Rigidbody>(); box = GetComponent<BoxCollider>();
        body.isKinematic = true; body.useGravity = false;
    }
    public bool TryToggle() { open = !open; box.isTrigger = true; return true; }
    private void FixedUpdate()
    {
        if (!IsMoving) { box.isTrigger = false; return; }
        Vector3 next = Vector3.MoveTowards(transform.localPosition, Target, speed * Time.fixedDeltaTime);
        Vector3 shift = transform.parent.TransformPoint(next) - transform.position;
        foreach (Collider c in Physics.OverlapBox(box.bounds.center + shift, box.bounds.extents,
                     Quaternion.identity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (c == box || c.GetComponentInParent<SlidingCabinetDoor>() != null) continue;
            if (c.attachedRigidbody == null && c is not CharacterController) continue;
            if (Physics.ComputePenetration(box, transform.position + shift, transform.rotation,
                c, c.transform.position, c.transform.rotation, out _, out float depth) && depth > .002f)
            {
                if (Time.unscaledTime >= nextWarning)
                {
                    nextWarning = Time.unscaledTime + 2;
                    RecipeFeedback.Warning("Make room for the cabinet door, or press E to reverse it.");
                }
                return;
            }
        }
        body.position = transform.parent.TransformPoint(next);
        box.isTrigger = IsMoving;
    }
}
