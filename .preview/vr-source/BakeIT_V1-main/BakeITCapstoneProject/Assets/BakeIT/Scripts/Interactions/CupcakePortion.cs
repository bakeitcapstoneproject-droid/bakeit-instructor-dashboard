using UnityEngine;

[DefaultExecutionOrder(250)]
public sealed class CupcakePortion : MonoBehaviour
{
    [SerializeField] private CupcakeBatch batch;
    [SerializeField] private MeshFilter cake;
    [SerializeField] private Mesh bakedMesh;
    private Rigidbody body;
    private Material material;
    private float cooledSeconds;
    public CupcakeBatch Batch => batch;
    public CupcakeSlot AttachedSlot { get; internal set; }
    public bool IsFilled { get; private set; }
    public float FillProgress { get; private set; }
    public bool IsBaked { get; private set; }
    public bool IsOnRack => AttachedSlot && AttachedSlot.IsCoolingPosition && transform.parent == AttachedSlot.transform;
    public float CoolingProgress => Mathf.Clamp01(cooledSeconds / batch.CoolingSeconds);
    public bool IsCooled => IsBaked && CoolingProgress >= 1;
    public bool CanPickup => FillProgress <= 0 || IsBaked && batch.DonenessChecked && !batch.IsInOven;
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        material = cake.GetComponent<Renderer>().material;
        cake.gameObject.SetActive(false);
    }
    public void Fill()
    {
        PreviewFill(1f);
        IsFilled = true;
        cake.gameObject.SetActive(true);
        GetComponent<Ingredient>().ingredientName = "CupcakeBatterPortion";
    }
    public void PreviewFill(float progress)
    {
        if (IsFilled) return;
        FillProgress = Mathf.Clamp01(progress);
        cake.gameObject.SetActive(FillProgress > 0);
        // Keep the lower face on the liner floor while the batter rises.
        cake.transform.localScale = new Vector3(1, Mathf.Max(.001f,FillProgress), 1);
    }
    public void Bake(OvenBakeResult result, string ingredientName, Color color)
    {
        IsBaked = true;
        GetComponent<Ingredient>().ingredientName = ingredientName;
        if (result != OvenBakeResult.Underbaked) { cake.sharedMesh = bakedMesh; material.color = color; }
    }
    private void LateUpdate()
    {
        if (AttachedSlot && transform.parent == AttachedSlot.transform && !SmoothPlacement.Settling(body))
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            body.position = transform.position;
            body.rotation = transform.rotation;
        }
        if (IsBaked && IsOnRack && !SmoothPlacement.Settling(body))
            cooledSeconds = Mathf.Min(batch.CoolingSeconds, cooledSeconds + Time.deltaTime);
    }
    private void OnDestroy() { if (material) Destroy(material); }
}
