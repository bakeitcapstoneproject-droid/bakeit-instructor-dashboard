using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float crouchMoveSpeed = 2.5f;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float gravity = -20f;
    [Header("Crouching")]
    [SerializeField] private float crouchHeight = 1.2f;
    [SerializeField] private float crouchCameraHeight = 0.9f;
    [SerializeField] private float crouchTransitionSpeed = 6f;

    private CharacterController controller;
    private PickupController pickupController;
    private Transform cameraTransform;
    private float standingHeight;
    private Vector3 standingCenter;
    private Vector3 crouchingCenter;
    private Vector3 standingCameraPosition;
    private bool crouchToggled;
    private float verticalVelocity;
    private float cameraPitch;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        pickupController = GetComponent<PickupController>();
        cameraTransform = GetComponentInChildren<Camera>().transform;

        standingHeight = controller.height;
        standingCenter = controller.center;
        standingCameraPosition = cameraTransform.localPosition;

        crouchHeight = Mathf.Clamp(crouchHeight,
            controller.radius * 2f, standingHeight);

        crouchingCenter = standingCenter;
        crouchingCenter.y -= (standingHeight - crouchHeight) * 0.5f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        UpdateCrouch();

        Vector2 input = Vector2.zero;
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed) input.y++;
            if (keyboard.sKey.isPressed) input.y--;
            if (keyboard.dKey.isPressed) input.x++;
            if (keyboard.aKey.isPressed) input.x--;
        }

        input = Vector2.ClampMagnitude(input, 1f);

        Vector3 movement =
            transform.forward * input.y +
            transform.right * input.x;

        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
        movement.y = verticalVelocity;

        float currentMoveSpeed =
            controller.height < standingHeight - 0.01f
                ? crouchMoveSpeed
                : moveSpeed;

        controller.Move(movement * currentMoveSpeed * Time.deltaTime);

        bool rotatingHeldObject =
            pickupController != null &&
            pickupController.IsRotatingHeldObject;

        if (!rotatingHeldObject && Mouse.current != null)
        {
            Vector2 mouseDelta =
                Mouse.current.delta.ReadValue() * mouseSensitivity;

            transform.Rotate(Vector3.up * mouseDelta.x);

            cameraPitch = Mathf.Clamp(
                cameraPitch - mouseDelta.y, -80f, 80f);

            cameraTransform.localRotation =
                Quaternion.Euler(cameraPitch, 0f, 0f);
        }

        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void UpdateCrouch()
    {
        Keyboard keyboard = Keyboard.current;
        bool crouchPressed =
            keyboard != null &&
            (keyboard.leftCtrlKey.wasPressedThisFrame ||
             keyboard.rightCtrlKey.wasPressedThisFrame);

        if (crouchPressed)
            crouchToggled = !crouchToggled;

        bool currentlyCrouched =
            controller.height < standingHeight - 0.01f;

        bool shouldCrouch = crouchToggled ||
            (currentlyCrouched && !CanStand());

        float targetHeight =
            shouldCrouch ? crouchHeight : standingHeight;

        Vector3 targetCenter =
            shouldCrouch ? crouchingCenter : standingCenter;

        float targetCameraY = shouldCrouch
            ? crouchCameraHeight
            : standingCameraPosition.y;

        controller.height = Mathf.MoveTowards(
            controller.height,
            targetHeight,
            crouchTransitionSpeed * Time.deltaTime);

        controller.center = Vector3.MoveTowards(
            controller.center,
            targetCenter,
            crouchTransitionSpeed * Time.deltaTime);

        Vector3 cameraPosition = cameraTransform.localPosition;
        cameraPosition.y = Mathf.MoveTowards(
            cameraPosition.y,
            targetCameraY,
            crouchTransitionSpeed * Time.deltaTime);
        cameraTransform.localPosition = cameraPosition;
    }

    private bool CanStand()
    {
        float radius = controller.radius * 0.95f;
        Vector3 up = transform.up;

        Vector3 currentCenter =
            transform.TransformPoint(controller.center);

        Vector3 standingWorldCenter =
            transform.TransformPoint(standingCenter);

        Vector3 currentHeadCenter = currentCenter + up * Mathf.Max(
            0f, controller.height * 0.5f - controller.radius);

        Vector3 standingHeadCenter = standingWorldCenter + up * Mathf.Max(
            0f, standingHeight * 0.5f - controller.radius);

        Collider[] overlaps = Physics.OverlapCapsule(
            currentHeadCenter,
            standingHeadCenter,
            radius,
            ~0,
            QueryTriggerInteraction.Ignore);

        foreach (Collider overlap in overlaps)
        {
            if (overlap.transform == transform ||
                overlap.transform.IsChildOf(transform))
                continue;

            return false;
        }

        return true;
    }
}
