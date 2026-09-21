using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickupController : MonoBehaviour
{
    [SerializeField] private float pickupDistance = 3f;
    [SerializeField] private float heldObjectSurfaceGap = 0.015f;
    [SerializeField] private float initialHoldDistance = 0.75f;
    [SerializeField] private float minimumHoldDistance = 0.6f;
    [SerializeField] private float maximumHoldDistance = 2.5f;
    [SerializeField] private float scrollDistanceStep = 0.2f;
    [SerializeField] private float heldRotationSensitivity = 0.25f;
    [SerializeField, Min(0.01f)] private float stuckRecoveryDistance = 0.075f;

    private Camera playerCamera;
    private Transform holdPoint;
    private Rigidbody heldObject;
    private Vector3 heldCenterOffsetLocal;
    private RigidbodyInterpolation previousInterpolation;
    private CollisionDetectionMode previousCollisionDetection;
    private float holdDistance;
    private Quaternion heldRotationOffset = Quaternion.identity;
    private Collider[] playerColliders;
    private Collider[] heldObjectColliders;
    private bool centerHeldAtCrosshair;
    private Vector3 HeldTargetPosition => heldObject != null && centerHeldAtCrosshair
        ? playerCamera.transform.position + playerCamera.transform.forward * holdDistance
        : holdPoint.position;

    public bool IsRotatingHeldObject =>
        heldObject != null &&
        Mouse.current != null &&
        Mouse.current.rightButton.isPressed;
    public Rigidbody HeldObject => heldObject;

    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        holdPoint = playerCamera.transform.Find("HoldPoint");
        playerColliders = GetComponentsInChildren<Collider>(true);
        holdPoint.localRotation = Quaternion.identity;
        ResetHoldDistance();
    }

    private void Update()
    {
        if (heldObject != null)
        {
            AdjustHoldDistance();

            if (IsRotatingHeldObject)
                RotateHeldObject();
        }

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (TryInteractWithFridgeDoor())
                return;
            else if (heldObject != null)
                DropObject();
            else
                TryPickup();
        }
    }

    public bool TryInteractWithFridgeDoor()
    {
        if (playerCamera == null)
            return false;

        RaycastHit[] hits = Physics.RaycastAll(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            pickupDistance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide);
        System.Array.Sort(hits, (first, second) => first.distance.CompareTo(second.distance));

        foreach (RaycastHit hit in hits)
        {
            Collider hitCollider = hit.collider;

            if (hitCollider == null || IsPlayerCollider(hitCollider) ||
                IsHeldObjectCollider(hitCollider))
            {
                continue;
            }

            FridgeDoorController fridgeDoor =
                hitCollider.GetComponentInParent<FridgeDoorController>();

            if (fridgeDoor != null)
                return fridgeDoor.TryToggle();

            SlidingCabinetDoor cabinetDoor = hitCollider.GetComponentInParent<SlidingCabinetDoor>();
            if (cabinetDoor != null) return cabinetDoor.TryToggle();

            var dispenser = hitCollider.GetComponentInParent<ParchmentDispenser>();
            // Holding food means E is release, even when the roll is behind it.
            if (dispenser != null) return heldObject == null && dispenser.TryDispense(this);

            RecipeSelectionButton recipeButton =
                hitCollider.GetComponentInParent<RecipeSelectionButton>();
            // A held item keeps E as release rather than changing the recipe.
            if (recipeButton != null)
                return heldObject == null && recipeButton.TrySelect();

            // Recipe receivers are triggers and do not obscure the door.
            // A solid wall, appliance or loose object still blocks the ray.
            if (!hitCollider.isTrigger)
                return false;
        }

        return false;
    }

    private void RotateHeldObject()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        Quaternion pitch = Quaternion.AngleAxis(
            -mouseDelta.y * heldRotationSensitivity,
            Vector3.right);

        Quaternion yaw = Quaternion.AngleAxis(
            mouseDelta.x * heldRotationSensitivity,
            Vector3.up);

        heldRotationOffset =
            Quaternion.Normalize(yaw * pitch * heldRotationOffset);
    }

    private void AdjustHoldDistance()
    {
        if (Mouse.current == null)
            return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scroll) <= Mathf.Epsilon)
            return;

        SetHoldDistance(
            holdDistance + Mathf.Sign(scroll) * scrollDistanceStep);
    }

    private void LateUpdate()
    {
        if (heldObject != null)
            MoveHeldObjectToHoldPoint();
    }

    private void TryPickup()
    {
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward);

        if (!Physics.Raycast(
                ray,
                out RaycastHit hit,
                pickupDistance,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore))
            return;

        TryPickupBody(hit.rigidbody);
    }

    public void TryPickupBody(Rigidbody target)
    {
        if (target == null || heldObject != null)
            return;
        var cupcake = target.GetComponent<CupcakePortion>();
        if (cupcake && !cupcake.CanPickup)
        {
            RecipeFeedback.Warning("Keep filled liners in the tray until baking and the tester check are complete; take the tray out of the oven first.");
            return;
        }

        // Hinged objects use their own interaction system.
        if (target.GetComponent<HingeJoint>() != null)
            return;

        // Kinematic bodies are fixed scene objects, not carryable items.
        // Ingredients are allowed because some recipe results begin
        // kinematic until the player picks them up.
        if (target.isKinematic &&
            target.GetComponentInParent<Ingredient>() == null &&
            target.GetComponent<PreparationItem>() == null)
            return;

        var ingredient = target.GetComponent<Ingredient>();
        var loadedTray = target.transform.parent != null ? target.transform.parent.GetComponentInChildren<TrayReceiver>() : null;
        if (ingredient != null && ingredient.ingredientName == "ParchmentPaper" && loadedTray != null && loadedTray.HasPortionedDough)
        {
            RecipeFeedback.Warning("Remove the cookies before lifting the parchment.");
            return;
        }
        target.GetComponent<SmoothPlacement>()?.Cancel();
        heldObject = target;
        Ingredient heldIngredient = target.GetComponentInChildren<Ingredient>(true);
        centerHeldAtCrosshair = target.GetComponent<DoughPortion>() != null ||
            heldIngredient != null && string.Equals(
                heldIngredient.ingredientName,
                "Egg",
                System.StringComparison.OrdinalIgnoreCase);
        target.GetComponent<PreparationItem>()?.PickedUp();
        ResetHoldDistance();
        previousInterpolation = heldObject.interpolation;
        previousCollisionDetection = heldObject.collisionDetectionMode;

        heldObject.transform.SetParent(null, true);
        heldObjectColliders =
            heldObject.GetComponentsInChildren<Collider>(true);
        SetPlayerCollisionsIgnored(heldObjectColliders, true);

        heldObject.detectCollisions = true;
        heldObject.useGravity = false;
        heldObject.isKinematic = true;
        heldObject.interpolation = RigidbodyInterpolation.None;
        heldObject.collisionDetectionMode =
            CollisionDetectionMode.ContinuousSpeculative;

        heldCenterOffsetLocal = CalculateVisualCenterLocal();

        PickupPose pickupPose =
            heldObject.GetComponent<PickupPose>();

        if (pickupPose == null)
        {
            pickupPose =
                heldObject.GetComponentInChildren<PickupPose>(true);
        }

        heldRotationOffset = pickupPose != null
            ? pickupPose.RotationOffset
            : Quaternion.Euler(0f, 180f, 0f);

        heldObject.rotation =
            holdPoint.rotation * heldRotationOffset;
        heldObject.transform.rotation = heldObject.rotation;

        MoveHeldObjectToHoldPoint();
    }

    private void ResetHoldDistance()
    {
        SetHoldDistance(initialHoldDistance);
    }

    private void SetHoldDistance(float distance)
    {
        holdDistance = Mathf.Clamp(
            distance,
            minimumHoldDistance,
            maximumHoldDistance);
        Vector3 localPosition = holdPoint.localPosition;
        localPosition.z = holdDistance;
        holdPoint.localPosition = localPosition;
    }

    private Vector3 CalculateVisualCenterLocal()
    {
        Renderer[] renderers =
            heldObject.GetComponentsInChildren<Renderer>();

        Bounds combinedBounds = new Bounds();
        bool foundRenderer = false;

        foreach (Renderer objectRenderer in renderers)
        {
            // Item labels must not displace its held visual center.
            if (objectRenderer.GetComponent<TextMesh>() != null) continue;
            if (!objectRenderer.enabled || objectRenderer.bounds.size.sqrMagnitude <= Mathf.Epsilon) continue;
            // Do not include a separately simulated child, such as
            // flattened dough attached to a tray, in the tray's pivot.
            if (objectRenderer.GetComponentInParent<Rigidbody>() != heldObject)
                continue;

            if (!foundRenderer)
            {
                combinedBounds = objectRenderer.bounds;
                foundRenderer = true;
            }
            else
            {
                combinedBounds.Encapsulate(objectRenderer.bounds);
            }
        }

        return foundRenderer
            ? heldObject.transform.InverseTransformPoint(
                combinedBounds.center)
            : Vector3.zero;
    }

    private void MoveHeldObjectToHoldPoint()
    {
        Quaternion desiredRotation =
            holdPoint.rotation * heldRotationOffset;
        heldObject.rotation = desiredRotation;
        heldObject.transform.rotation = desiredRotation;

        Vector3 desiredPosition =
            HeldTargetPosition -
            heldObject.transform.TransformVector(
                heldCenterOffsetLocal);

        Vector3 movement = desiredPosition - heldObject.position;
        float distance = movement.magnitude;

        if (distance <= Mathf.Epsilon)
        {
            // Rigidbody.position can already be at the target while its
            // rendered Transform is still one frame behind.
            heldObject.transform.position = desiredPosition;
            return;
        }

        Vector3 direction = movement / distance;
        float allowedDistance = distance;

        RaycastHit[] hits = heldObject.SweepTestAll(
            direction,
            distance,
            QueryTriggerInteraction.Ignore);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null ||
                hit.collider.attachedRigidbody == heldObject)
                continue;

            Transform hitTransform = hit.collider.transform;

            // Do not let the player's own CharacterController block
            // the object that the player is holding.
            if (hitTransform == transform ||
                hitTransform.IsChildOf(transform))
                continue;

            // A zero-distance contact should only block movement when
            // the held object is moving farther into that surface.
            if (Vector3.Dot(direction, hit.normal) >= -0.001f)
                continue;

            allowedDistance = Mathf.Min(
                allowedDistance,
                Mathf.Max(0f, hit.distance - heldObjectSurfaceGap));
        }

        Vector3 nextPosition =
            heldObject.position + direction * allowedDistance;

        bool movementWasBlocked =
            allowedDistance < distance - 0.001f;

        // If an item was left on the far side of a prop while the
        // player moved away, let it safely catch up to the hold point.
        // A clear camera path prevents this recovery from pulling items
        // through a wall, while the overlap test prevents embedding the
        // item inside another solid collider.
        if (movementWasBlocked &&
            Vector3.Distance(nextPosition, desiredPosition) >=
                stuckRecoveryDistance)
        {
            // Recover only one scroll-wheel step toward the player. This keeps
            // the object close to the distance the player deliberately chose
            // instead of snapping it all the way to the minimum distance.
            float previousHoldDistance = holdDistance;
            SetHoldDistance(previousHoldDistance - scrollDistanceStep);
            Vector3 recoveryPosition =
                HeldTargetPosition -
                heldObject.transform.TransformVector(heldCenterOffsetLocal);

            if (HasClearCameraPathToHoldPoint() &&
                CanOccupyHeldPosition(recoveryPosition))
            {
                SetHeldPosition(recoveryPosition);
                return;
            }

            SetHoldDistance(previousHoldDistance);
        }

        SetHeldPosition(nextPosition);
    }

    private void SetHeldPosition(Vector3 position)
    {
        // Held bodies are kinematic. Updating the Transform as well as the
        // Rigidbody keeps the rendered object aligned in this LateUpdate,
        // instead of waiting for the next physics synchronization.
        heldObject.position = position;
        heldObject.transform.position = position;
    }

    private bool HasClearCameraPathToHoldPoint()
    {
        Vector3 start = playerCamera.transform.position;
        Vector3 movement = HeldTargetPosition - start;
        float distance = movement.magnitude;

        if (distance <= Mathf.Epsilon)
            return true;

        RaycastHit[] hits = Physics.RaycastAll(
            start,
            movement / distance,
            distance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore);

        foreach (RaycastHit hit in hits)
        {
            Collider hitCollider = hit.collider;

            if (hitCollider == null ||
                IsHeldObjectCollider(hitCollider) ||
                IsPlayerCollider(hitCollider))
            {
                continue;
            }

            return false;
        }

        return true;
    }

    private bool CanOccupyHeldPosition(Vector3 desiredPosition)
    {
        if (heldObjectColliders == null)
            return true;

        Collider[] sceneColliders = FindObjectsByType<Collider>();

        foreach (Collider heldCollider in heldObjectColliders)
        {
            if (heldCollider == null ||
                !heldCollider.enabled ||
                heldCollider.isTrigger)
            {
                continue;
            }

            Vector3 candidateColliderPosition =
                desiredPosition +
                (heldCollider.transform.position - heldObject.position);

            foreach (Collider sceneCollider in sceneColliders)
            {
                if (sceneCollider == null ||
                    !sceneCollider.enabled ||
                    sceneCollider.isTrigger ||
                    IsHeldObjectCollider(sceneCollider) ||
                    IsPlayerCollider(sceneCollider) ||
                    Physics.GetIgnoreCollision(
                        heldCollider,
                        sceneCollider))
                {
                    continue;
                }

                bool overlaps = Physics.ComputePenetration(
                    heldCollider,
                    candidateColliderPosition,
                    heldCollider.transform.rotation,
                    sceneCollider,
                    sceneCollider.transform.position,
                    sceneCollider.transform.rotation,
                    out _,
                    out float penetrationDistance);

                if (overlaps &&
                    penetrationDistance > heldObjectSurfaceGap)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool IsHeldObjectCollider(Collider candidate)
    {
        if (candidate == null || heldObjectColliders == null)
            return false;

        foreach (Collider heldCollider in heldObjectColliders)
        {
            if (candidate == heldCollider)
                return true;
        }

        return false;
    }

    private bool IsPlayerCollider(Collider candidate)
    {
        if (candidate == null || playerColliders == null)
            return false;

        foreach (Collider playerCollider in playerColliders)
        {
            if (candidate == playerCollider)
                return true;
        }

        return false;
    }

    public bool ReleaseForPlacement(Rigidbody target)
    {
        if (heldObject != target)
            return false;

        heldObject.interpolation = previousInterpolation;
        heldObject.collisionDetectionMode = previousCollisionDetection;
        Collider[] releasedColliders = heldObjectColliders;
        heldObjectColliders = null;
        heldObject = null;
        BeginRestorePlayerCollisions(releasedColliders);
        return true;
    }

    private void DropObject()
    {
        if (heldObject == null) return;
        foreach (var batch in FindObjectsByType<CupcakeBatch>())
            if (batch.TryPlaceReleased(heldObject)) return;
        foreach (var slot in FindObjectsByType<ToolHangingSlot>())
            if (slot.TryPlace(heldObject)) return;
        foreach (var receiver in FindObjectsByType<TrayLinerReceiver>())
            if (receiver.TryPlaceReleased(heldObject)) return;
        foreach (var receiver in FindObjectsByType<BrowniePanReceiver>())
            if (receiver.TryPlaceReleased(heldObject)) return;
        foreach (var receiver in FindObjectsByType<TrayReceiver>())
            if (receiver.TryPlaceReleased(heldObject)) return;
        foreach (var receiver in FindObjectsByType<DoughBoardController>())
            if (receiver.TryPlaceReleased(heldObject)) return;
        PreparationItem item = heldObject.GetComponent<PreparationItem>();
        if (item != null)
            foreach (var surface in FindObjectsByType<StablePlacementSurface>())
                if (surface.TryPlace(item)) return;
        heldObject.transform.SetParent(null, true);
        heldObject.isKinematic = false;
        heldObject.useGravity = true;
        heldObject.interpolation = previousInterpolation;
        heldObject.collisionDetectionMode = previousCollisionDetection;
        Collider[] releasedColliders = heldObjectColliders;
        heldObjectColliders = null;
        heldObject = null;
        BeginRestorePlayerCollisions(releasedColliders);
    }

    private void BeginRestorePlayerCollisions(
        Collider[] releasedColliders)
    {
        if (releasedColliders != null)
        {
            StartCoroutine(
                RestorePlayerCollisionsWhenClear(releasedColliders));
        }
    }

    private IEnumerator RestorePlayerCollisionsWhenClear(
        Collider[] releasedColliders)
    {
        // Keep ignoring the player until the released object is no
        // longer overlapping the player capsule. Restoring collision
        // while they overlap can violently launch the player.
        while (OverlapsPlayer(releasedColliders))
        {
            if (ContainsHeldCollider(releasedColliders))
                yield break;

            yield return null;
        }

        yield return new WaitForFixedUpdate();

        if (!ContainsHeldCollider(releasedColliders))
            SetPlayerCollisionsIgnored(releasedColliders, false);
    }

    private bool OverlapsPlayer(Collider[] objectColliders)
    {
        foreach (Collider objectCollider in objectColliders)
        {
            if (objectCollider == null ||
                !objectCollider.enabled ||
                objectCollider.isTrigger)
                continue;

            foreach (Collider playerCollider in playerColliders)
            {
                if (playerCollider == null ||
                    !playerCollider.enabled ||
                    playerCollider.isTrigger)
                    continue;

                if (objectCollider.bounds.Intersects(
                        playerCollider.bounds))
                    return true;
            }
        }

        return false;
    }

    private bool ContainsHeldCollider(Collider[] colliders)
    {
        if (heldObjectColliders == null)
            return false;

        foreach (Collider releasedCollider in colliders)
        {
            foreach (Collider heldCollider in heldObjectColliders)
            {
                if (releasedCollider != null &&
                    releasedCollider == heldCollider)
                    return true;
            }
        }

        return false;
    }

    private void SetPlayerCollisionsIgnored(
        Collider[] objectColliders,
        bool ignored)
    {
        if (objectColliders == null || playerColliders == null)
            return;

        foreach (Collider objectCollider in objectColliders)
        {
            if (objectCollider == null)
                continue;

            foreach (Collider playerCollider in playerColliders)
            {
                if (playerCollider == null ||
                    playerCollider == objectCollider)
                    continue;

                Physics.IgnoreCollision(
                    objectCollider,
                    playerCollider,
                    ignored);
            }
        }
    }
}
