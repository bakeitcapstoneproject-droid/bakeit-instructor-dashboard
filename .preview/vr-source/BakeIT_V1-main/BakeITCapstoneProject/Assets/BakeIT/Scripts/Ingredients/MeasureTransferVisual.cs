using UnityEngine;

// Brief presentation only: particles do not collide or own ingredient quantities.
public sealed class MeasureTransferVisual : MonoBehaviour
{
    private Vector3 start, end;
    private float elapsed;
    private Material material;
    private bool walnuts;
    public static void Show(Vector3 from, Vector3 to, string ingredient, Mesh[] kernels = null)
    {
        var root=new GameObject("Measured transfer (visual)");
        var visual=root.AddComponent<MeasureTransferVisual>(); visual.start=from;visual.end=to;
        visual.material=new Material(Shader.Find("Universal Render Pipeline/Lit"));
        bool chips=ingredient=="ChocolateChips";
        visual.walnuts = ingredient == "Walnuts";
        bool meltedButter=ingredient=="MeltedButter";
        Color color=ingredient switch {
            "ChocolateChips" => new Color(.15f,.07f,.025f),
            "Walnuts" => new Color(.70f,.43f,.17f),
            "BrownieBatter" => new Color32(102,60,2,255),
            "MeltedButter" => new Color(.96f,.68f,.16f),
            "Sugar" => Color.white,
            "Cocoa" => new Color(.32f,.15f,.045f),
            "Vanilla" => new Color(.36f,.13f,.025f),
            "Milk" => new Color(.98f,.97f,.91f),
            "Oil" => new Color(.96f,.72f,.16f),
            _ => new Color(.96f,.93f,.84f)
        };
        visual.material.SetColor("_BaseColor",color);
        visual.material.SetFloat("_Smoothness",.1f);
        for(int i=0;i<(visual.walnuts ? 24 : 12);i++)
        {
            var grain = new GameObject("Falling ingredient"); grain.transform.SetParent(root.transform, false);
            Mesh kernel = visual.walnuts && kernels != null && kernels.Length > 0 ? kernels[i % kernels.Length] : null;
            if (kernel)
            {
                var part = new GameObject("Walnut chunk", typeof(MeshFilter), typeof(MeshRenderer));
                part.transform.SetParent(grain.transform, false);
                float scale = (.008f + i % 4 * .001f) / Mathf.Max(kernel.bounds.size.x, kernel.bounds.size.z);
                part.transform.localScale = Vector3.one * scale;
                part.transform.localPosition = -kernel.bounds.center * scale;
                part.GetComponent<MeshFilter>().sharedMesh = kernel;
                part.GetComponent<Renderer>().sharedMaterial = visual.material;
            }
            else
            {
                var part = GameObject.CreatePrimitive(PrimitiveType.Sphere); part.transform.SetParent(grain.transform, false);
                var collider = part.GetComponent<Collider>(); collider.enabled = false; Destroy(collider);
                part.GetComponent<Renderer>().sharedMaterial = visual.material;
                part.transform.localScale = Vector3.one * (chips ? .006f : visual.walnuts ? .009f : meltedButter ? .005f : .003f);
            }
            grain.transform.position=from;
        }
    }
    private void Update()
    {
        elapsed+=Time.deltaTime;
        for(int i=0;i<transform.childCount;i++)
        {
            float t=Mathf.Clamp01((elapsed-i*(walnuts?.020f:.009f))/(walnuts?.42f:.25f));
            Vector3 scatter=new Vector3(Mathf.Sin(i*17),0,Mathf.Cos(i*13))*(walnuts?.020f:.008f)*Mathf.Sin(t*Mathf.PI);
            transform.GetChild(i).position=Vector3.Lerp(start,end,t*t)+scatter;
            if (walnuts) { transform.GetChild(i).rotation = Quaternion.Euler(t * (90 + i * 13), i * 47, t * 150); transform.GetChild(i).gameObject.SetActive(t > 0 && t < 1); }
        }
        if(elapsed>(walnuts?1.0f:.40f))Destroy(gameObject);
    }
    private void OnDestroy(){if(material!=null)Destroy(material);}
}
