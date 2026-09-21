using UnityEngine;

public static class PlacementGeometry
{
    // Validate a near-surface upright pose, preserving the player's exact X/Z.
    // Restore the preview pose before returning; SmoothPlacement owns the move.
    public static bool TryGetPose(Rigidbody body, BoxCollider support, Quaternion rotation,
        out Vector3 position, out Quaternion orientation, float maxDistance = .22f, float edgeAllowance = 0f)
    {
        position = body != null ? body.transform.position : Vector3.zero;
        orientation = rotation;
        if (body == null || support == null || !support.enabled || support.isTrigger ||
            !support.gameObject.activeInHierarchy || Vector3.Dot(support.transform.up, Vector3.up) < .95f) return false;
        Vector3 original = body.transform.position;
        Quaternion originalRotation = body.transform.rotation;
        body.position = original;
        body.rotation = originalRotation;
        Physics.SyncTransforms();
        var colliders = body.GetComponentsInChildren<Collider>();
        Bounds before = BoundsOf(colliders, body);
        Bounds surface = support.bounds;
        float gap = before.min.y - surface.max.y;
        if (gap < -.025f || gap > maxDistance) return false;
        body.transform.rotation = rotation;
        body.rotation = rotation;
        Physics.SyncTransforms();
        Bounds upright = BoundsOf(colliders, body);
        Vector3 shift = Vector3.up * (surface.max.y + .003f - upright.min.y);
        Bounds target = new Bounds(upright.center + shift, upright.size);
        bool fits = target.min.x >= surface.min.x + .003f - edgeAllowance && target.max.x <= surface.max.x - .003f + edgeAllowance &&
            target.min.z >= surface.min.z + .003f - edgeAllowance && target.max.z <= surface.max.z - .003f + edgeAllowance &&
            target.center.x >= surface.min.x && target.center.x <= surface.max.x && target.center.z >= surface.min.z && target.center.z <= surface.max.z && Mathf.Abs(shift.y) <= maxDistance;
        if (fits)
        {
            var nearby = Physics.OverlapBox(target.center, target.extents, Quaternion.identity,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            foreach (var own in colliders)
            {
                if (!own.enabled || own.isTrigger || own.attachedRigidbody != body) continue;
                foreach (var obstacle in nearby)
                {
                    if (obstacle == support || obstacle.attachedRigidbody == body) continue;
                    if (Physics.ComputePenetration(own, own.transform.position + shift, own.transform.rotation,
                        obstacle, obstacle.transform.position, obstacle.transform.rotation, out _, out float depth) && depth > .002f)
                        fits = false;
                }
            }
        }
        position = original + shift;
        body.transform.SetPositionAndRotation(original, originalRotation);
        body.position = original;
        body.rotation = originalRotation;
        Physics.SyncTransforms();
        return fits;
    }

    private static Bounds BoundsOf(Collider[] colliders, Rigidbody body)
    {
        Bounds bounds = new Bounds(body.transform.position, Vector3.zero);
        bool found = false;
        foreach (var c in colliders)
        {
            if (!c.enabled || c.isTrigger || c.attachedRigidbody != body) continue;
            if (!found) { bounds = c.bounds; found = true; } else bounds.Encapsulate(c.bounds);
        }
        return bounds;
    }
}
