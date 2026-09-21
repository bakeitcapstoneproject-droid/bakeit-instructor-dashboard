using UnityEngine;

[DisallowMultipleComponent]
public sealed class BrownieFoodVisual : MonoBehaviour
{
    [SerializeField] private MeshFilter surfaceFilter;
    [SerializeField] private Renderer surfaceRenderer;
    [SerializeField] private Mesh rawMesh;
    [SerializeField] private Mesh bakedMesh;
    [SerializeField] private Material rawMaterial;
    [SerializeField] private Material bakedMaterial;
    [SerializeField] private GameObject rawDetailsRoot;
    [SerializeField] private GameObject bakedDetailsRoot;
    [SerializeField] private Mesh[] spreadStages;
    private Material ownedSurfaceMaterial;
    private Material ownedBakedMaterial;
    private GameObject slices;
    private Mesh sliceMesh;
    private Material crumbMaterial;
    private float crustSeed;
    private readonly System.Collections.Generic.List<GameObject> createdPieces = new();
    private readonly System.Collections.Generic.List<Transform> topWalnuts = new();
    private OvenBakeResult bakedResult;
    private bool walnutsScattered;
    private bool portionsReady;
    public System.Collections.Generic.IReadOnlyList<GameObject> CutPieces => createdPieces;
    [Header("Progressive bowl fill")]
    [SerializeField] private Vector3 fullLocalPosition;
    [SerializeField] private Vector3 fullLocalScale = Vector3.one;
    [SerializeField, Range(.1f, 1f)] private float minimumFillRadius = .48f;
    [SerializeField, Range(.1f, 1f)] private float minimumFillHeight = .26f;

    public Renderer SurfaceRenderer => surfaceRenderer;

    private void Awake()
    {
        crustSeed=Random.Range(0f,500f);
        if (surfaceFilter == null)
            surfaceFilter = GetComponent<MeshFilter>();
        if (surfaceRenderer == null)
            surfaceRenderer = GetComponent<Renderer>();
    }

    public void ShowRaw(bool showRawDetails = true)
    {
        if(slices) slices.SetActive(false);
        if(surfaceRenderer) surfaceRenderer.enabled=true;
        if (surfaceFilter != null && rawMesh != null)
            surfaceFilter.sharedMesh = rawMesh;
        if (surfaceRenderer != null && rawMaterial != null)
            surfaceRenderer.sharedMaterials = new[]{ownedSurfaceMaterial ? ownedSurfaceMaterial : rawMaterial};
        if (rawDetailsRoot != null)
            rawDetailsRoot.SetActive(showRawDetails);
        if (bakedDetailsRoot != null)
            bakedDetailsRoot.SetActive(false);
        SetTextureVariation(surfaceRenderer,crustSeed);
    }

    public void SetFillProgress(float progress, bool showRawDetails)
    {
        progress = Mathf.Clamp01(progress);
        if (rawDetailsRoot != null)
            rawDetailsRoot.SetActive(showRawDetails);
        if (surfaceFilter == null || surfaceFilter.sharedMesh == null)
            return;

        Vector3 scale = fullLocalScale;
        float radius = Mathf.Lerp(minimumFillRadius, 1f, progress);
        scale.x *= radius;
        scale.z *= radius;
        scale.y *= Mathf.Lerp(minimumFillHeight, 1f, progress);

        float meshBottom = surfaceFilter.sharedMesh.bounds.min.y;
        float fullBottom = Mathf.Lerp(.020f, fullLocalPosition.y + meshBottom * fullLocalScale.y, progress);
        Vector3 position = fullLocalPosition;
        position.y = fullBottom - meshBottom * scale.y;
        transform.localScale = scale;
        transform.localPosition = position;
    }

    public void SetSpreadProgress(float progress)
    {
        if(spreadStages==null || spreadStages.Length==0 || !surfaceFilter) return;
        int index=Mathf.RoundToInt(Mathf.Clamp01(progress)*(spreadStages.Length-1));
        surfaceFilter.sharedMesh=spreadStages[index];
        if(rawDetailsRoot)
        {
            rawDetailsRoot.transform.localScale=new Vector3(Mathf.Lerp(.55f,1f,progress),1,Mathf.Lerp(.55f,1f,progress));
            rawDetailsRoot.transform.localPosition=Vector3.zero;
            // Seat every kernel on the actual changing surface, not an elevated detail plane.
            foreach(Transform kernel in rawDetailsRoot.transform)
            {
                Vector3 p=kernel.localPosition;
                Vector3 scaled=Vector3.Scale(p,rawDetailsRoot.transform.localScale);
                Vector3 nearest=Vector3.zero; float distance=float.MaxValue;
                foreach(Vector3 vertex in surfaceFilter.sharedMesh.vertices)
                {
                    if(vertex.y<=0) continue;
                    float d=new Vector2(vertex.x-scaled.x,vertex.z-scaled.z).sqrMagnitude;
                    if(d<distance){distance=d;nearest=vertex;}
                }
                p.y=nearest.y; kernel.localPosition=p;
            }
        }
    }

    public void ShowCuts(int passes)
    {
        if(!slices)
        {
            slices=new GameObject("Twelve thick brownie squares"); slices.transform.SetParent(transform,false);
            sliceMesh=BrownieFoodGeometry.Pan(1,true);
            Material crust=ownedBakedMaterial ? ownedBakedMaterial : bakedMaterial;
            crumbMaterial=new Material(crust); crumbMaterial.color=crust.color*.57f;
            crumbMaterial.SetFloat("_Smoothness",.08f);
            for(int z=0;z<3;z++) for(int x=0;x<4;x++)
            {
                var piece=new GameObject($"Brownie square {z*4+x+1}");piece.transform.SetParent(slices.transform,false);
                createdPieces.Add(piece);
                piece.transform.localPosition=new Vector3((x-1.5f)/4,0,(z-1f)/3);
                piece.transform.localScale=new Vector3(.243f,1,.326f);
                piece.AddComponent<MeshFilter>().sharedMesh=sliceMesh;
                var pieceRenderer=piece.AddComponent<MeshRenderer>();
                pieceRenderer.sharedMaterials=new[]{crust,crumbMaterial};
                SetTextureVariation(pieceRenderer,crustSeed+(z*4+x)*13.7f);
                AddCutFaceWalnuts(piece.transform,z*4+x);
            }
        }
        slices.SetActive(passes>=5);
        surfaceRenderer.enabled=passes<5;
        if (passes >= 5 && !portionsReady)
        {
            portionsReady = true;
            // Keep the visible top kernels with their physical square when it is lifted.
            if (bakedDetailsRoot)
            {
                foreach (Transform walnut in bakedDetailsRoot.transform) topWalnuts.Add(walnut);
                foreach (Transform walnut in topWalnuts)
                {
                    Vector3 p = transform.InverseTransformPoint(walnut.position);
                    int x = Mathf.Clamp(Mathf.FloorToInt((p.x + .5f) * 4), 0, 3);
                    int z = Mathf.Clamp(Mathf.FloorToInt((p.z + .5f) * 3), 0, 2);
                    walnut.SetParent(createdPieces[z * 4 + x].transform, true);
                }
            }
            foreach (var piece in createdPieces)
            {
                var box = piece.AddComponent<BoxCollider>();
                box.center = sliceMesh.bounds.center;
                box.size = sliceMesh.bounds.size;
                var body = piece.AddComponent<Rigidbody>();
                body.mass = .06f; body.isKinematic = true; body.useGravity = false;
                var ingredient = piece.AddComponent<Ingredient>();
                ingredient.ingredientName = "BrowniePortion";
                piece.AddComponent<ServingPortion>().Configure("brownies", bakedResult);
            }
        }
        // Partial cuts are recessed straight seams; completed cuts reveal separate solid sides.
        for(int i=0;i<5;i++)
        {
            Transform seam=transform.Find("Knife seam "+i);
            if(!seam)
            {
                var go=GameObject.CreatePrimitive(PrimitiveType.Cube);Destroy(go.GetComponent<Collider>());go.name="Knife seam "+i;
                seam=go.transform;seam.SetParent(transform,false);go.GetComponent<Renderer>().sharedMaterial=crumbMaterial;
                seam.localPosition=i<3?new Vector3((i-1)*.25f,.925f,0):new Vector3(0,.925f,(i-3.5f)/3f);
                seam.localScale=i<3?new Vector3(.005f,.008f,.99f):new Vector3(.99f,.008f,.005f);
            }
            seam.gameObject.SetActive(i<passes && passes<5);
        }
    }

    public void ResetCuts()
    {
        foreach (var walnut in topWalnuts) if (walnut && bakedDetailsRoot) walnut.SetParent(bakedDetailsRoot.transform, true);
        topWalnuts.Clear();
        foreach (var piece in createdPieces) if (piece) Destroy(piece);
        createdPieces.Clear(); portionsReady = false; walnutsScattered = false;
        if(slices){slices.SetActive(false);Destroy(slices);slices=null;}
        if(sliceMesh){Destroy(sliceMesh);sliceMesh=null;}
        if(crumbMaterial){Destroy(crumbMaterial);crumbMaterial=null;}
        crustSeed=Random.Range(0f,500f);
        for(int i=0;i<5;i++){var seam=transform.Find("Knife seam "+i);if(seam)seam.gameObject.SetActive(false);}
        if(surfaceRenderer)surfaceRenderer.enabled=true;
    }

    public void ShowBaked(OvenBakeResult result, Color resultColor, float smoothness)
    {
        bakedResult = result;
        if (surfaceFilter != null && bakedMesh != null)
            surfaceFilter.sharedMesh = bakedMesh;
        if (surfaceRenderer != null && bakedMaterial != null)
        {
            if(!ownedBakedMaterial)ownedBakedMaterial=new Material(bakedMaterial);
            ownedBakedMaterial.color=bakedMaterial.color;
            surfaceRenderer.sharedMaterials = new[]{ownedBakedMaterial,ownedBakedMaterial};
        }
        if (rawDetailsRoot != null)
            rawDetailsRoot.SetActive(false);
        if (bakedDetailsRoot != null)
            bakedDetailsRoot.SetActive(true);
        ScatterBakedWalnuts();
        SetTextureVariation(surfaceRenderer,crustSeed);

        if (surfaceRenderer == null)
            return;

        Material material = surfaceRenderer.sharedMaterial;
        if (result != OvenBakeResult.Perfect)
        {
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", resultColor);
            else if (material.HasProperty("_Color"))
                material.SetColor("_Color", resultColor);
        }
        if (material.HasProperty("_Metallic"))
            material.SetFloat("_Metallic", 0f);
        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", result == OvenBakeResult.Perfect
                ? .26f
                : smoothness);
    }

    public void ApplyRawColor(Color color)
    {
        if (surfaceRenderer == null)
            return;
        if (!ownedSurfaceMaterial) ownedSurfaceMaterial = new Material(rawMaterial);
        surfaceRenderer.sharedMaterial=ownedSurfaceMaterial;
        Material material = ownedSurfaceMaterial;
        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", color);
        else if (material.HasProperty("_Color"))
            material.SetColor("_Color", color);
    }

    private static void SetTextureVariation(Renderer target,float seed)
    {
        if(!target)return;
        MaterialPropertyBlock properties=new();target.GetPropertyBlock(properties);
        properties.SetFloat("_Seed",seed);
        properties.SetVector("_FoodScale",target.transform.lossyScale);
        target.SetPropertyBlock(properties);
    }

    private void AddCutFaceWalnuts(Transform piece,int index)
    {
        if(!bakedDetailsRoot)return;
        var source=bakedDetailsRoot.GetComponentInChildren<MeshFilter>(true);
        if(!source || !source.sharedMesh)return;
        var material=source.GetComponent<Renderer>().sharedMaterial;
        var mesh=source.sharedMesh;
        for(int i=0;i<5;i++)
        {
            var bit=new GameObject("Embedded walnut cross-section");bit.transform.SetParent(piece,false);
            bool front=i<3;
            float along=Random.Range(-.36f,.36f);
            float height=Random.Range(.22f,.70f);
            // Each fragment straddles the cut face with most of its volume inside the brownie.
            bit.transform.localPosition=front?new Vector3(along,height,-.485f):new Vector3(.485f,height,along);
            Vector3 worldScale=piece.lossyScale;
            float size=Random.Range(.0055f,.009f);
            bit.transform.localScale=new Vector3(size/worldScale.x/mesh.bounds.size.x,size*.75f/worldScale.y/mesh.bounds.size.y,size/worldScale.z/mesh.bounds.size.z);
            bit.transform.localPosition-=Vector3.Scale(mesh.bounds.center,bit.transform.localScale);
            bit.AddComponent<MeshFilter>().sharedMesh=mesh;
            bit.AddComponent<MeshRenderer>().sharedMaterial=material;
        }
    }

    private void ScatterBakedWalnuts()
    {
        if (walnutsScattered || !bakedDetailsRoot) return;
        walnutsScattered = true;
        var placed = new System.Collections.Generic.List<Vector2>();
        foreach (Transform kernel in bakedDetailsRoot.transform)
        {
            Vector2 point = default;
            for (int attempt = 0; attempt < 80; attempt++)
            {
                point = new Vector2(Random.Range(-.43f,.43f), Random.Range(-.42f,.42f));
                bool overlaps = false;
                foreach (Vector2 prior in placed) if (Vector2.Distance(point, prior) < .085f) { overlaps = true; break; }
                if (!overlaps) break;
            }
            placed.Add(point);
            kernel.localPosition = new Vector3(point.x, Random.Range(.81f,.85f), point.y);
            // Rotate the mesh itself; rotating the non-uniformly scaled food parent distorts kernels.
            if (kernel.childCount > 0)
            {
                var mesh = kernel.GetChild(0);
                mesh.localRotation = Quaternion.Euler(0, Random.Range(0f,360f), 0);
            }
        }
    }

    private void OnValidate()
    {
        if (surfaceFilter == null)
            surfaceFilter = GetComponent<MeshFilter>();
        if (surfaceRenderer == null)
            surfaceRenderer = GetComponent<Renderer>();
    }
    private void OnDestroy() { if(ownedSurfaceMaterial) Destroy(ownedSurfaceMaterial);if(ownedBakedMaterial)Destroy(ownedBakedMaterial); if(sliceMesh)Destroy(sliceMesh);if(crumbMaterial)Destroy(crumbMaterial); }
}
