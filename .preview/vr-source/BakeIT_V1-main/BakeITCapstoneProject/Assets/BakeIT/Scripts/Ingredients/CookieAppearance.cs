using System.Collections.Generic;
using UnityEngine;

// Reuses the existing dough model. Decoration never participates in recipe volume.
[DisallowMultipleComponent]
public sealed class CookieAppearance : MonoBehaviour
{
    [SerializeField] private Mesh sourceMesh;
    [SerializeField] private Material doughMaterial, chipMaterial;
    [SerializeField] private int seed = 173;
    [SerializeField] private int chipCount = 24;
    [SerializeField] private bool portion;
    private Mesh ownedDough, ownedChips;
    private Material ownedMaterial;
    private Vector3 lastScale;
    private bool initialized;
    private Vector3[] surfaceVertices;
    private int[] surfaceTriangles;

    private void OnEnable() { if (!initialized && Application.isPlaying) Build(); }
    public void ConfigurePortion(int index)
    {
        seed = 173 + (index + 1) * 7919;
        chipCount = 8 + index % 3;
        portion = true;
        Build();
    }

    private void Build()
    {
        if (sourceMesh == null || doughMaterial == null) return;
        if (!sourceMesh.isReadable) { Debug.LogError("CookieAppearance needs Read/Write enabled on its source dough mesh.",this); return; }
        if (ownedDough != null) Destroy(ownedDough);
        if (ownedMaterial != null) Destroy(ownedMaterial);
        ownedDough = Instantiate(sourceMesh); ownedDough.name = "Cookie dough variation " + seed;
        var bounds = sourceMesh.bounds;
        var vertices = sourceMesh.vertices;
        for (int i=0;i<vertices.Length;i++)
        {
            var p=vertices[i]-bounds.center;
            float angle=Mathf.Atan2(p.z,p.x);
            float ripple=1+.026f*Mathf.Sin(angle*7+seed)+.018f*Mathf.Sin(angle*11+seed*.13f);
            p.x*=ripple;p.z*=ripple;
            float y=Mathf.InverseLerp(bounds.min.y,bounds.max.y,vertices[i].y);
            p.y+=bounds.size.y*.055f*Mathf.Sin(p.x*170+seed)*Mathf.Cos(p.z*135+seed)*Mathf.Sin(y*Mathf.PI);
            vertices[i]=bounds.center+p;
        }
        ownedDough.vertices=vertices;ownedDough.RecalculateBounds();
        // Keep original represented bounds/volume and the flat supporting base.
        var generated=ownedDough.bounds;
        for(int i=0;i<vertices.Length;i++)
        {
            var p=vertices[i];
            vertices[i]=new Vector3(Mathf.Lerp(bounds.min.x,bounds.max.x,Mathf.InverseLerp(generated.min.x,generated.max.x,p.x)),
                Mathf.Lerp(bounds.min.y,bounds.max.y,Mathf.InverseLerp(generated.min.y,generated.max.y,p.y)),
                Mathf.Lerp(bounds.min.z,bounds.max.z,Mathf.InverseLerp(generated.min.z,generated.max.z,p.z)));
        }
        ownedDough.vertices=vertices;ownedDough.RecalculateNormals();ownedDough.RecalculateBounds();
        surfaceVertices=vertices;surfaceTriangles=ownedDough.triangles;
        GetComponent<MeshFilter>().sharedMesh=ownedDough;
        var collider=GetComponent<MeshCollider>();if(collider!=null){collider.sharedMesh=ownedDough;collider.convex=true;}
        ownedMaterial=new Material(doughMaterial);
        ownedMaterial.SetVector("_BoundsMin",bounds.min);ownedMaterial.SetVector("_BoundsSize",bounds.size);ownedMaterial.SetFloat("_Seed",seed*.01f);
        GetComponent<Renderer>().sharedMaterial=ownedMaterial;
        initialized=true;RebuildChips();
    }

    private void LateUpdate()
    {
        if(initialized && (transform.lossyScale-lastScale).sqrMagnitude>.000001f) RebuildChips();
    }

    private void RebuildChips()
    {
        // A portion can rebuild twice in its creation frame. Destroy is deferred,
        // so remove every prior decoration, not just the first matching child.
        foreach(Transform child in transform)
            if(child.name=="Scattered Chocolate Chips"){child.gameObject.SetActive(false);Destroy(child.gameObject);}
        if(ownedChips!=null)Destroy(ownedChips);
        lastScale=transform.lossyScale;
        var random=new System.Random(seed);
        var bounds=ownedDough.bounds;
        var positions=new List<Vector2>();var vertices=new List<Vector3>();var triangles=new List<int>();
        for(int i=0;i<chipCount;i++)
        {
            Vector2 point=Vector2.zero;
            for(int attempt=0;attempt<80;attempt++)
            {
                float angle=(float)random.NextDouble()*Mathf.PI*2;
                float radius=Mathf.Sqrt((float)random.NextDouble())*.80f;
                point=new Vector2(Mathf.Cos(angle)*radius,Mathf.Sin(angle)*radius);
                bool clear=true;foreach(var previous in positions)if(Vector2.Distance(point,previous)<(portion?.38f:.23f))clear=false;
                if(clear)break;
            }
            positions.Add(point);
            float x=bounds.center.x+point.x*bounds.extents.x,z=bounds.center.z+point.y*bounds.extents.z;
            float size=(portion?.010f:.0115f)*Mathf.Lerp(.85f,1.18f,(float)random.NextDouble());
            Vector3 center=new Vector3(x,SurfaceHeight(x,z)-size*.15f/Mathf.Max(.01f,lastScale.y),z);
            Quaternion tilt=Quaternion.Euler(Mathf.Lerp(-25,25,(float)random.NextDouble()),(float)random.NextDouble()*360,Mathf.Lerp(-25,25,(float)random.NextDouble()));
            int start=vertices.Count;
            for(int ring=0;ring<4;ring++)for(int side=0;side<8;side++)
            {
                float h=new[]{0f,.22f,.66f,1f}[ring];float r=new[]{.88f,1f,.64f,.10f}[ring];
                float a=side*Mathf.PI/4;
                Vector3 offset=tilt*new Vector3(Mathf.Cos(a)*size*.5f*r,h*size*.72f,Mathf.Sin(a)*size*.5f*r);
                vertices.Add(center+transform.InverseTransformVector(transform.rotation*offset));
                if(ring<3){int j=start+ring*8+side,k=start+ring*8+(side+1)%8;triangles.AddRange(new[]{j,j+8,k,k,j+8,k+8});}
            }
            for(int side=1;side<7;side++)triangles.AddRange(new[]{start+24,start+25+side,start+24+side});
        }
        ownedChips=new Mesh{name="Randomized chocolate chips "+seed};ownedChips.SetVertices(vertices);ownedChips.SetTriangles(triangles,0);ownedChips.RecalculateNormals();ownedChips.RecalculateBounds();
        var root=new GameObject("Scattered Chocolate Chips");root.transform.SetParent(transform,false);root.AddComponent<ChocolateChipVisual>();
        root.AddComponent<MeshFilter>().sharedMesh=ownedChips;root.AddComponent<MeshRenderer>().sharedMaterial=chipMaterial;
    }

    private float SurfaceHeight(float x,float z)
    {
        var vertices=surfaceVertices;var triangles=surfaceTriangles;float top=ownedDough.bounds.center.y;
        for(int i=0;i<triangles.Length;i+=3)
        {
            var a=vertices[triangles[i]];var b=vertices[triangles[i+1]];var c=vertices[triangles[i+2]];
            float den=(b.z-c.z)*(a.x-c.x)+(c.x-b.x)*(a.z-c.z);if(Mathf.Abs(den)<.00000001f)continue;
            float u=((b.z-c.z)*(x-c.x)+(c.x-b.x)*(z-c.z))/den;
            float v=((c.z-a.z)*(x-c.x)+(a.x-c.x)*(z-c.z))/den;
            if(u>=0&&v>=0&&u+v<=1)top=Mathf.Max(top,u*a.y+v*b.y+(1-u-v)*c.y);
        }
        return top;
    }

    private void OnDestroy()
    {
        if(ownedDough!=null)Destroy(ownedDough);if(ownedChips!=null)Destroy(ownedChips);if(ownedMaterial!=null)Destroy(ownedMaterial);
    }
}
