using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FridgeDoorController : MonoBehaviour
{
    [SerializeField] private float openAngle = 100f;
    [SerializeField, Min(1f)] private float degreesPerSecond = 110f;
    [SerializeField] private BoxCollider[] doorColliders;
    [SerializeField] private LayerMask obstructionLayers = ~0;
    [SerializeField, Min(0f)] private float contactTolerance = 0.002f;
    [SerializeField, Min(0.1f)] private float feedbackInterval = 2f;

    private Quaternion closedLocalRotation;
    private Rigidbody doorBody;
    private PickupController pickupController;
    private float currentAngle;
    private float targetAngle;
    private float nextFeedbackTime;

    public bool IsClosed => Mathf.Abs(currentAngle) <= 3f;
    public bool IsOpen => Mathf.Abs(currentAngle - openAngle) <= 3f;
    public bool IsMoving => Mathf.Abs(currentAngle - targetAngle) > 0.01f;
    public float OpenAngle => currentAngle;

    private void Awake()
    {
        doorBody = GetComponent<Rigidbody>();
        doorBody.useGravity = false;
        doorBody.isKinematic = true;
        doorBody.interpolation = RigidbodyInterpolation.None;
        doorBody.collisionDetectionMode =
            CollisionDetectionMode.ContinuousSpeculative;
        closedLocalRotation = transform.localRotation;
        pickupController = FindAnyObjectByType<PickupController>();

        if (doorColliders == null || doorColliders.Length == 0)
            doorColliders = GetComponentsInChildren<BoxCollider>(true);

        SetMovingColliders(false);
    }

    // Desktop pickup calls this for the targeted door. Future VR input
    // can use the same action without introducing a second keyboard reader.
    public bool TryToggle()
    {
        if (!isActiveAndEnabled)
            return false;

        targetAngle = Mathf.Abs(targetAngle) > 0.01f ? 0f : openAngle;
        SetMovingColliders(IsMoving);
        return true;
    }

    private void FixedUpdate()
    {
        if (!IsMoving)
            return;

        // Small angular increments keep the obstruction check valid even
        // when the physics timestep is increased for a slower device.
        float nextAngle = Mathf.MoveTowards(
            currentAngle,
            targetAngle,
            Mathf.Min(3f, degreesPerSecond * Time.fixedDeltaTime));
        Quaternion nextLocalRotation =
            closedLocalRotation * Quaternion.Euler(0f, nextAngle, 0f);
        Quaternion nextWorldRotation = transform.parent != null
            ? transform.parent.rotation * nextLocalRotation
            : nextLocalRotation;

        if (WouldObstruct(nextWorldRotation))
        {
            if (Time.unscaledTime >= nextFeedbackTime)
            {
                nextFeedbackTime = Time.unscaledTime + feedbackInterval;
                RecipeFeedback.Warning(
                    "Make room for the fridge door, or press E to reverse it.");
            }

            return;
        }

        currentAngle = nextAngle;
        doorBody.rotation = nextWorldRotation;
        SetMovingColliders(IsMoving);
    }

    private bool WouldObstruct(Quaternion candidateRotation)
    {
        Quaternion rotationChange =
            candidateRotation * Quaternion.Inverse(transform.rotation);

        foreach (BoxCollider doorCollider in doorColliders)
        {
            if (doorCollider == null || !doorCollider.enabled)
                continue;

            Vector3 candidatePosition = transform.position +
                rotationChange *
                (doorCollider.transform.position - transform.position);
            Quaternion candidateColliderRotation =
                rotationChange * doorCollider.transform.rotation;
            Vector3 candidateCenter = transform.position +
                rotationChange *
                (doorCollider.transform.TransformPoint(doorCollider.center) -
                 transform.position);
            Vector3 scale = doorCollider.transform.lossyScale;
            Vector3 halfExtents = Vector3.Scale(
                doorCollider.size * 0.5f,
                new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z)));

            Collider[] nearby = Physics.OverlapBox(
                candidateCenter,
                halfExtents,
                candidateColliderRotation,
                obstructionLayers,
                QueryTriggerInteraction.Ignore);

            foreach (Collider obstacle in nearby)
            {
                if (!IsMovableObstacle(obstacle))
                    continue;

                if (Physics.ComputePenetration(
                        doorCollider,
                        candidatePosition,
                        candidateColliderRotation,
                        obstacle,
                        obstacle.transform.position,
                        obstacle.transform.rotation,
                        out _,
                        out float distance) && distance > contactTolerance)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsMovableObstacle(Collider obstacle)
    {
        if (obstacle == null || obstacle.transform.IsChildOf(transform))
            return false;

        if (obstacle is CharacterController ||
            obstacle.GetComponentInParent<PickupController>() != null)
        {
            return true;
        }

        Rigidbody body = obstacle.attachedRigidbody;

        if (body == null || body == doorBody ||
            body.GetComponent<FridgeDoorController>() != null)
        {
            return false;
        }

        // Fixed fridge shell/shelf colliders are excluded; stored food and
        // carried tools are still checked, even when their bodies are kinematic.
        return !body.isKinematic ||
            body.GetComponentInChildren<Ingredient>() != null ||
            (pickupController != null && pickupController.HeldObject == body);
    }

    private void SetMovingColliders(bool moving)
    {
        if (doorColliders == null)
            return;

        foreach (BoxCollider doorCollider in doorColliders)
        {
            if (doorCollider != null)
                doorCollider.isTrigger = moving;
        }
    }

    private void OnValidate()
    {
        float direction = openAngle < 0f ? -1f : 1f;
        openAngle = direction * Mathf.Clamp(Mathf.Abs(openAngle), 60f, 120f);
        degreesPerSecond = Mathf.Max(1f, degreesPerSecond);
        contactTolerance = Mathf.Max(0f, contactTolerance);
        feedbackInterval = Mathf.Max(0.1f, feedbackInterval);
    }
}
