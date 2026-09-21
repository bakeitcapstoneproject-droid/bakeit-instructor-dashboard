using System.Collections.Generic;
using UnityEngine;

// Presentation only. The receiver remains the authority for accepted quantities.
public sealed class BrowniePreparationVisual : MonoBehaviour
{
    private readonly Dictionary<string, Transform> piles = new();
    private readonly List<Material> materials = new();

    public void Refresh(PourableIngredientReceiver receiver)
    {
        if (piles.Count == 0) Build();
        float mixing = receiver.RequiredBaseMixPasses > 0
            ? (float)receiver.BaseMixPassCount / receiver.RequiredBaseMixPasses : 0f;
        foreach (var pair in piles)
        {
            int quantity = receiver.GetTotalIngredientAmount(pair.Key);
            bool visible = quantity > 0 && !receiver.IsBaseMixed && !receiver.HasTransferredBatter;
            pair.Value.gameObject.SetActive(visible);
            if (!visible) continue;
            float growth = pair.Key == "Egg" ? 1f : Mathf.Min(1.30f, .85f + quantity * .15f);
            float remaining = Mathf.Lerp(1f, .22f, mixing);
            pair.Value.localScale = new Vector3(growth * remaining, remaining, growth * remaining);
            if (pair.Key == "Egg")
                for (int i = 0; i < pair.Value.childCount; i++)
                    pair.Value.GetChild(i).gameObject.SetActive(i < quantity);
        }
    }

    private Material Food(string name, Color color, float smoothness)
    {
        Material mat = new(Shader.Find("Universal Render Pipeline/Lit")) { name = name };
        mat.SetColor("_BaseColor", color);
        mat.SetFloat("_Metallic", 0f);
        mat.SetFloat("_Smoothness", smoothness);
        materials.Add(mat);
        return mat;
    }

    private Transform Pile(string id, Vector3 position)
    {
        Transform root = new GameObject(id + " — unmixed").transform;
        root.SetParent(transform, false);
        root.localPosition = position;
        piles.Add(id, root);
        return root;
    }

    private static void Lump(Transform parent, string name, Material material, Vector3 position, Vector3 size)
    {
        GameObject part = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        part.name = name;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = position;
        part.transform.localScale = size;
        Collider collider = part.GetComponent<Collider>();
        collider.enabled = false;
        Destroy(collider);
        part.GetComponent<Renderer>().sharedMaterial = material;
    }

    private void Build()
    {
        Material flour = Food("Unmixed Flour — ivory", new Color(.96f,.93f,.85f), .05f);
        Material sugar = Food("Unmixed Sugar — white", Color.white, .28f);
        Material butter = Food("Unmixed melted butter — gold", new Color(1f,.69f,.10f), .65f);
        Material white = Food("Egg whites", new Color(.94f,.87f,.59f), .75f);
        Material yolk = Food("Egg yolks", new Color(1f,.48f,.015f), .6f);
        Material cocoa = Food("Unmixed cocoa", new Color(.32f,.15f,.045f), .06f);
        Material vanilla = Food("Vanilla pool", new Color(.36f,.13f,.025f), .68f);

        // Coordinates are relative to the bowl, not its elevated trigger.
        // At radius 0.05 the inner wall rises above the central floor.
        Transform f = Pile("Flour", new Vector3(.024f,.022f,-.020f));
        Lump(f,"Soft flour mound",flour,new Vector3(0,.011f,0),new Vector3(.069f,.023f,.060f));
        for(int i=0;i<9;i++)
        {
            float a=i*2.4f, radius=.023f*Mathf.Sqrt((i+.5f)/9f);
            float y=.011f+.011f*Mathf.Sqrt(1-radius*radius/(.034f*.034f));
            Lump(f,"Flour clump",flour,new Vector3(Mathf.Cos(a)*radius,y-.002f,Mathf.Sin(a)*radius),new Vector3(.012f,.005f,.010f));
        }
        Transform s = Pile("Sugar",new Vector3(-.025f,.021f,-.017f));
        Lump(s,"White sugar mound",sugar,new Vector3(0,.009f,0),new Vector3(.060f,.019f,.054f));
        for(int i=0;i<35;i++)
        {
            float r=.023f*Mathf.Sqrt((i+.5f)/35f), a=i*2.399963f;
            float y=.009f+.009f*Mathf.Sqrt(Mathf.Max(0,1-r*r/(.027f*.027f)));
            Lump(s,"Sugar grain",sugar,new Vector3(Mathf.Cos(a)*r,y,Mathf.Sin(a)*r),Vector3.one*.0024f);
        }
        Transform b = Pile("MeltedButter",new Vector3(0,.020f,.011f));
        Lump(b,"Golden butter pool",butter,new Vector3(0,.002f,0),new Vector3(.086f,.004f,.068f));
        Transform e = Pile("Egg",new Vector3(0,.025f,.024f));
        for(int i=0;i<4;i++)
        {
            Transform egg = new GameObject("Egg " + (i+1)).transform; egg.SetParent(e,false);
            egg.localPosition=new Vector3((i%2-.5f)*.028f,0,(i/2-.5f)*.025f);
            Lump(egg,"Egg white",white,Vector3.zero,new Vector3(.036f,.003f,.031f));
            Lump(egg,"Intact yolk",yolk,new Vector3(0,.005f,0),new Vector3(.017f,.010f,.017f));
        }
        Transform c=Pile("Cocoa",new Vector3(.035f,.031f,.015f));
        Lump(c,"Dry cocoa mound",cocoa,new Vector3(0,.006f,0),new Vector3(.036f,.013f,.037f));
        Transform v=Pile("Vanilla",new Vector3(-.028f,.028f,.030f));
        Lump(v,"Amber vanilla",vanilla,new Vector3(0,.001f,0),new Vector3(.019f,.003f,.015f));
        Transform p=Pile("BakingPowder",new Vector3(.009f,.043f,-.020f));
        Lump(p,"Baking powder",flour,Vector3.zero,new Vector3(.014f,.007f,.012f));
        Transform salt=Pile("Salt",new Vector3(-.012f,.037f,-.029f));
        Lump(salt,"Salt crystals",sugar,Vector3.zero,new Vector3(.010f,.005f,.009f));
    }

    private void OnDestroy() { foreach(Material mat in materials) if(mat) Destroy(mat); }
}
