using System.Collections.Generic;
using UnityEngine;

public class IngredientStorageSlot : MonoBehaviour
{
    [SerializeField] private string acceptedIngredientName = "Egg";
    [SerializeField] private Transform placementPoint;
    [SerializeField] private Ingredient initialItem;
    [SerializeField] private FridgeDoorController accessDoor;
    [SerializeField, Min(0.1f)] private float feedbackInterval = 2f;

    private PickupController pickupController;
    private Ingredient storedItem;
    private Rigidbody storedBody;
    private float nextFeedbackTime;
    private readonly Dictionary<Ingredient, HashSet<Collider>> overlaps = new();
    private readonly HashSet<Ingredient> retrievedUntilExit = new();

    public Ingredient StoredItem
    {
        get
        {
            RefreshStoredItem();
            return storedItem;
        }
    }

    public bool HasItem => StoredItem != null;
    public bool WasRetrieved { get; private set; }
    public string AcceptedIngredientName => acceptedIngredientName;

    private void Awake()
    {
        if (placementPoint == null)
            placementPoint = transform;

        pickupController = FindAnyObjectByType<PickupController>();
    }

    private void Start()
    {
        if (initialItem != null && initialItem.gameObject.activeInHierarchy)
            StoreItem(initialItem, report: false);
    }

    private void Update()
    {
        RefreshStoredItem();
    }

    private void RefreshStoredItem()
    {
        if (storedItem == null)
        {
            storedBody = null;
            return;
        }

        bool wasPickedUp = storedBody != null &&
            pickupController != null &&
            pickupController.HeldObject == storedBody;
        bool leftSlot = storedBody == null ||
            !storedBody.transform.IsChildOf(placementPoint);

        if (!storedItem.gameObject.activeInHierarchy || wasPickedUp || leftSlot)
        {
            if (wasPickedUp || leftSlot)
            {
                WasRetrieved = true;
                if (overlaps.ContainsKey(storedItem))
                    retrievedUntilExit.Add(storedItem);
            }

            storedItem = null;
            storedBody = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Ingredient ingredient = other.GetComponentInParent<Ingredient>();

        if (ingredient == null)
            return;

        if (!overlaps.TryGetValue(ingredient, out HashSet<Collider> colliders))
        {
            colliders = new HashSet<Collider>();
            overlaps.Add(ingredient, colliders);
        }

        colliders.Add(other);
        TryStore(ingredient);
    }

    private void OnTriggerStay(Collider other)
    {
        Ingredient ingredient = other.GetComponentInParent<Ingredient>();

        if (ingredient != null)
            TryStore(ingredient, allowHeldPlacement: false);
    }

    private void OnTriggerExit(Collider other)
    {
        Ingredient ingredient = other.GetComponentInParent<Ingredient>();

        if (ingredient == null ||
            !overlaps.TryGetValue(ingredient, out HashSet<Collider> colliders))
        {
            return;
        }

        colliders.Remove(other);

        if (colliders.Count == 0)
        {
            overlaps.Remove(ingredient);
            retrievedUntilExit.Remove(ingredient);
        }

        RefreshStoredItem();
    }

    // Trigger entry and a future VR placement action share this receiver.
    // Trigger stay may recover a released item, but never takes an item
    // that the player is still carrying out of storage.
    public bool TryStore(Ingredient ingredient, bool allowHeldPlacement = true)
    {
        RefreshStoredItem();

        if (ingredient == null || !ingredient.gameObject.activeInHierarchy)
            return false;

        if (ingredient == storedItem)
            return true;

        Rigidbody body = ingredient.GetComponentInParent<Rigidbody>();

        if (body == null)
            return false;

        bool isHeld = pickupController != null && pickupController.HeldObject == body;

        if (isHeld && (!allowHeldPlacement || retrievedUntilExit.Contains(ingredient)))
            return false;

        if (!IsCompatible(ingredient))
        {
            ReportRejection($"This fridge space is for {acceptedIngredientName.ToLowerInvariant()}.");
            return false;
        }

        if (storedItem != null)
        {
            ReportRejection("This fridge space already holds an ingredient.");
            return false;
        }

        if (accessDoor != null && !accessDoor.IsOpen)
        {
            ReportRejection("Open this fridge door before returning an ingredient.");
            return false;
        }

        return StoreItem(ingredient, report: true);
    }

    private bool StoreItem(Ingredient ingredient, bool report)
    {
        if (!IsCompatible(ingredient))
            return false;

        Rigidbody body = ingredient.GetComponentInParent<Rigidbody>();

        if (body == null)
            return false;

        if (pickupController != null)
            pickupController.ReleaseForPlacement(body);

        if (!body.isKinematic)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        Vector3 worldScale = body.transform.lossyScale;
        body.useGravity = false;
        body.isKinematic = true;
        body.detectCollisions = true;
        body.transform.SetParent(placementPoint, true);
        body.transform.SetPositionAndRotation(placementPoint.position, placementPoint.rotation);
        Vector3 parentScale = placementPoint.lossyScale;
        body.transform.localScale = new Vector3(
            DivideScale(worldScale.x, parentScale.x),
            DivideScale(worldScale.y, parentScale.y),
            DivideScale(worldScale.z, parentScale.z));
        storedItem = ingredient;
        storedBody = body;
        retrievedUntilExit.Remove(ingredient);

        if (report)
            RecipeFeedback.Report($"{acceptedIngredientName} returned to fridge storage.");

        return true;
    }

    private bool IsCompatible(Ingredient ingredient)
    {
        return ingredient != null && string.Equals(
            ingredient.ingredientName?.Trim(),
            acceptedIngredientName?.Trim(),
            System.StringComparison.OrdinalIgnoreCase);
    }

    private void ReportRejection(string message)
    {
        if (Time.unscaledTime < nextFeedbackTime)
            return;

        nextFeedbackTime = Time.unscaledTime + feedbackInterval;
        RecipeFeedback.Warning(message);
    }

    private static float DivideScale(float value, float divisor)
    {
        return Mathf.Abs(divisor) > Mathf.Epsilon ? value / divisor : value;
    }

    private void OnValidate()
    {
        acceptedIngredientName = acceptedIngredientName?.Trim() ?? string.Empty;
        feedbackInterval = Mathf.Max(0.1f, feedbackInterval);
    }
}
