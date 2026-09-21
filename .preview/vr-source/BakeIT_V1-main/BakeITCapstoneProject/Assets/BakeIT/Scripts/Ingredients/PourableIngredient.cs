using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public sealed class PourableIngredient : MonoBehaviour
{
    [SerializeField] private MixingSequence dryBlendSource;
    [SerializeField] private CupcakeBatch cupcakeBatterSource;
    public CupcakeBatch CupcakeBatterSource => cupcakeBatterSource;
    [SerializeField] private string ingredientName = "MeltedButter";
    [SerializeField] private string displayName = "Melted Butter";
    [SerializeField] private string quantityLabel = "1 cup";
    [SerializeField] private GameObject contentsVisual;
    [SerializeField] private Transform pourPoint;
    [SerializeField, Range(45f, 120f)] private float pourAngle = 55f;
    [SerializeField, Min(0.1f)] private float pourHoldSeconds = 1.6f;
    [SerializeField, Min(0.05f)] private float maximumPourDistance = 0.35f;
    [SerializeField, Min(0.001f)] private float mouthDetectionRadius = 0.012f;

    private PickupController pickup;
    private Rigidbody body;
    private IPourDestination previousDestination;
    private float pourTime;
    private LineRenderer stream;
    private Material streamMaterial;
    private bool dryPouring;
    private float nextDryEmission;
    private Vector3 fullFillPosition, fullFillScale;
    public bool IsPouring => dryPouring || (stream != null && stream.enabled);

    public string IngredientName => ingredientName;
    public string DisplayName => string.IsNullOrWhiteSpace(displayName)
        ? ingredientName
        : displayName;
    public string QuantityLabel => string.IsNullOrWhiteSpace(quantityLabel)
        ? "1 portion"
        : quantityLabel;
    private bool hasContents = true;
    public bool HasContents => dryBlendSource ? dryBlendSource.HasDryBlend : cupcakeBatterSource ? cupcakeBatterSource.BatterReady && cupcakeBatterSource.RemainingBatterPortions > 0 : hasContents;
    public bool IsHeld => pickup != null && pickup.HeldObject == body;
    public bool IsTilted => Vector3.Angle(transform.up, Vector3.up) >= pourAngle;
    public float PourProgress => Mathf.Clamp01(pourTime / pourHoldSeconds);
    public Vector3 PourOrigin => pourPoint != null
        ? pourPoint.position
        : transform.position;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        pickup = FindAnyObjectByType<PickupController>();
        if (contentsVisual) { fullFillPosition = contentsVisual.transform.localPosition; fullFillScale = contentsVisual.transform.localScale; }
        UpdateContentsVisual();
    }

    private void Update()
    {
        if (!IsHeld || !HasContents || !IsTilted)
        {
            StopStream();
            return;
        }

        IPourDestination destination = FindPourDestination();

        if (destination == null || !destination.CanPreviewPour(this))
        {
            StopStream();
            return;
        }

        if (destination != previousDestination)
        {
            previousDestination = destination;
            pourTime = destination is CupcakeSlot slot && slot.Portion ? slot.Portion.FillProgress * pourHoldSeconds : 0f;
        }

        pourTime += Time.deltaTime;
        if (destination is CupcakeSlot cupcakeSlot && cupcakeSlot.Portion)
            cupcakeSlot.Portion.PreviewFill(PourProgress);
        ShowStream(destination.TransferTarget);
        UpdateContentsVisual();

        if (pourTime < pourHoldSeconds)
            return;

        destination.TryReceive(this);
    }

    public IPourDestination FindPourDestination()
    {
        Vector3 origin = PourOrigin;

        foreach (Collider candidate in Physics.OverlapSphere(
                     origin,
                     mouthDetectionRadius,
                     Physics.DefaultRaycastLayers,
                     QueryTriggerInteraction.Collide))
        {
            if (candidate.attachedRigidbody == body)
                continue;

            IPourDestination receiver = candidate.GetComponent<IPourDestination>();

            if (receiver != null)
                return receiver;
        }

        RaycastHit[] hits = Physics.RaycastAll(
            origin + Vector3.up * 0.01f,
            Vector3.down,
            maximumPourDistance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide);
        System.Array.Sort(hits, (first, second) =>
            first.distance.CompareTo(second.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.rigidbody == body ||
                (pickup != null && hit.transform.IsChildOf(pickup.transform)))
            {
                continue;
            }

            IPourDestination receiver = hit.collider.GetComponent<IPourDestination>();

            if (receiver != null)
                return receiver;

            if (!hit.collider.isTrigger)
                return null;
        }

        return null;
    }

    public bool TryCompleteTransfer(
        IPourDestination receiver,
        Vector3 visualTarget)
    {
        if (receiver == null || !HasContents || !IsHeld || !IsTilted || PourProgress < 1f ||
            FindPourDestination() != receiver)
        {
            return false;
        }

        if (dryBlendSource) dryBlendSource.ConsumeDryPortion();
        else if (!cupcakeBatterSource) hasContents = false;
        ResetPourProgress();
        UpdateContentsVisual();
        return true;
    }

    public void ResetContents()
    {
        hasContents = true;
        ResetPourProgress();
        UpdateContentsVisual();
    }

    private void ResetPourProgress()
    {
        pourTime = 0f;
        previousDestination = null;
        StopStream();
    }

    private void UpdateContentsVisual()
    {
        if (contentsVisual != null)
        {
            contentsVisual.SetActive(HasContents);
            contentsVisual.transform.localPosition = Vector3.Lerp(fullFillPosition, new Vector3(fullFillPosition.x, .035f, fullFillPosition.z), PourProgress);
            contentsVisual.transform.localScale = Vector3.Lerp(fullFillScale, new Vector3(fullFillScale.x * .84f, fullFillScale.y, fullFillScale.z * .84f), PourProgress);
        }
    }

    private void StopStream() { dryPouring = false; if (stream) stream.enabled = false; }
    private void ShowStream(Vector3 target)
    {
        if (dryBlendSource)
        {
            dryPouring = true;
            if (Time.time >= nextDryEmission)
            {
                nextDryEmission = Time.time + .12f;
                MeasureTransferVisual.Show(PourOrigin, target, "CupcakeDryBlend");
            }
            return;
        }
        if (!stream)
        {
            var root = new GameObject(DisplayName + " stream (visual)");
            root.transform.SetParent(transform, false);
            stream = root.AddComponent<LineRenderer>();
            streamMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            streamMaterial.color = cupcakeBatterSource ? new Color(.94f,.81f,.49f) : new Color(.96f,.66f,.12f);
            stream.sharedMaterial = streamMaterial;
            stream.useWorldSpace = true; stream.positionCount = 12;
            stream.numCapVertices = 5; stream.numCornerVertices = 3;
            stream.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        stream.enabled = true;
        stream.startWidth = .009f; stream.endWidth = .006f;
        for (int i = 0; i < 12; i++)
        {
            float t = i / 11f;
            stream.SetPosition(i, Vector3.Lerp(PourOrigin, target, t) + Vector3.down * Mathf.Sin(t * Mathf.PI) * .015f);
        }
    }
    private void OnDisable() => StopStream();
    private void OnDestroy() { if (streamMaterial) Destroy(streamMaterial); }
}
