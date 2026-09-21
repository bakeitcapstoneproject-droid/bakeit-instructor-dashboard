using System;
using UnityEngine;

// One shared short settle for resting supplies, dough, parchment and cookies.
// Local-space targets keep a placed object attached correctly to a moving tray.
[DefaultExecutionOrder(200)]
[DisallowMultipleComponent]
public sealed class SmoothPlacement : MonoBehaviour
{
    private Rigidbody body;
    private Vector3 startPosition, targetPosition;
    private Quaternion startRotation, targetRotation;
    private Transform anchor;
    private float elapsed;
    private Action completed;
    private bool restoreCollisions;
    private const float Duration = .24f;
    public bool IsSettling { get; private set; }
    public static bool Settling(Rigidbody body) => body != null && body.GetComponent<SmoothPlacement>()?.IsSettling == true;

    public static void Begin(Rigidbody body, Vector3 position, Quaternion rotation, Transform parent, Action onComplete = null)
    {
        var motion = body.GetComponent<SmoothPlacement>();
        if (motion == null) motion = body.gameObject.AddComponent<SmoothPlacement>();
        motion.Cancel();
        motion.body = body;
        if (!body.isKinematic) { body.linearVelocity = Vector3.zero; body.angularVelocity = Vector3.zero; }
        body.isKinematic = true;
        body.useGravity = false;
        body.transform.SetParent(parent, true);
        motion.anchor = parent;
        motion.startPosition = body.transform.localPosition;
        motion.startRotation = body.transform.localRotation;
        motion.targetPosition = parent != null ? parent.InverseTransformPoint(position) : position;
        motion.targetRotation = parent != null ? Quaternion.Inverse(parent.rotation) * rotation : rotation;
        motion.elapsed = 0;
        motion.completed = onComplete;
        motion.restoreCollisions = body.detectCollisions;
        // The validated kinematic interpolation must not shove the player or
        // adjacent tools while it is uprighting. Restore collision on arrival.
        body.detectCollisions = false;
        motion.IsSettling = true;
        motion.enabled = true;
    }

    public void Cancel()
    {
        if (IsSettling && body != null) body.detectCollisions = restoreCollisions;
        IsSettling = false;
        completed = null;
        enabled = false;
    }

    private void LateUpdate()
    {
        if (body == null || transform.parent != anchor) { Cancel(); return; }
        elapsed += Time.deltaTime;
        float t = Mathf.SmoothStep(0, 1, Mathf.Clamp01(elapsed / Duration));
        Vector3 localPosition = Vector3.Lerp(startPosition, targetPosition, t);
        Quaternion localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
        body.position = anchor != null ? anchor.TransformPoint(localPosition) : localPosition;
        body.rotation = anchor != null ? anchor.rotation * localRotation : localRotation;
        if (IsSettling && elapsed >= Duration)
        {
            IsSettling = false;
            body.detectCollisions = restoreCollisions;
            var callback = completed; completed = null; callback?.Invoke();
        }
    }
}
