using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(HingeJoint))]
public class OvenDoorController : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private float minimumAngle = 0f;
    [SerializeField] private float maximumOpenAngle = 90f;

    private Camera playerCamera;
    private Rigidbody doorBody;
    private HingeJoint hinge;
    private Quaternion closedRotation;
    private float currentAngle;
    private float targetAngle;
    private float dragStartDoorAngle;
    private float grabDistance;
    private Vector3 hingeWorldPoint;
    private Vector3 hingeWorldAxis;
    private Vector3 dragStartRadial;
    private bool isDragging;

    public bool IsClosed => currentAngle <= 5f;
    public float OpenAngle => currentAngle;

    private void Awake()
    {
        playerCamera = Camera.main;
        doorBody = GetComponent<Rigidbody>();
        hinge = GetComponent<HingeJoint>();

        maximumOpenAngle = Mathf.Clamp(maximumOpenAngle, 45f, 90f);

        JointLimits limits = hinge.limits;
        limits.min = minimumAngle;
        limits.max = maximumOpenAngle;
        limits.bounciness = 0f;
        hinge.limits = limits;
        hinge.useLimits = true;
        hinge.useSpring = false;

        // Camera dragging is deterministic on desktop. Keeping the body
        // kinematic prevents the physics solver from fighting the hinge.
        doorBody.useGravity = false;
        doorBody.isKinematic = true;

        closedRotation = transform.localRotation;
        currentAngle = 0f;
        targetAngle = 0f;
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (!isDragging)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame &&
                TryGetDoorHit(out RaycastHit hit))
            {
                BeginDrag(hit);
            }
        }
        else if (!Keyboard.current.eKey.isPressed)
        {
            isDragging = false;
        }

    }

    private void LateUpdate()
    {
        // Run after the player's camera Update so the door uses the
        // crosshair's newest position instead of trailing by one frame.
        if (isDragging)
        {
            Vector3 crosshairPoint =
                playerCamera.transform.position +
                playerCamera.transform.forward * grabDistance;

            Vector3 targetRadial = Vector3.ProjectOnPlane(
                crosshairPoint - hingeWorldPoint,
                hingeWorldAxis);

            if (targetRadial.sqrMagnitude > 0.0001f)
            {
                float angleFromGrab = Vector3.SignedAngle(
                    dragStartRadial,
                    targetRadial,
                    hingeWorldAxis);

                targetAngle = Mathf.Clamp(
                    dragStartDoorAngle + angleFromGrab,
                    minimumAngle,
                    maximumOpenAngle);
            }
        }

        currentAngle = targetAngle;

        transform.localRotation =
            closedRotation * Quaternion.Euler(currentAngle, 0f, 0f);
    }

    private bool TryGetDoorHit(out RaycastHit hit)
    {
        hit = default;

        if (playerCamera == null)
            return false;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward);

        if (!Physics.Raycast(
                ray,
                out hit,
                interactionDistance))
            return false;

        return hit.rigidbody == doorBody;
    }

    private void BeginDrag(RaycastHit hit)
    {
        isDragging = true;
        dragStartDoorAngle = currentAngle;
        grabDistance = hit.distance;

        hingeWorldPoint = transform.TransformPoint(hinge.anchor);
        hingeWorldAxis =
            transform.TransformDirection(hinge.axis).normalized;

        dragStartRadial = Vector3.ProjectOnPlane(
            hit.point - hingeWorldPoint,
            hingeWorldAxis);
    }

    private void OnDisable()
    {
        isDragging = false;
    }

    private void OnValidate()
    {
        maximumOpenAngle = Mathf.Clamp(maximumOpenAngle, 45f, 90f);
    }
}
