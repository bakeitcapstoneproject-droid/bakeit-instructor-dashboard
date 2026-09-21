using System.Collections.Generic;
using UnityEngine;

public static class BrownieFoodGeometry
{
    // Every stage has matching topology, allowing a clump to ease into a rectangle.
    public static Mesh Pan(float spread, bool baked = false)
    {
        const int n=24;
        List<Vector3> vertices=new(); List<Vector2> uv=new(); List<int> triangles=new();
        for(int z=0;z<=n;z++) for(int x=0;x<=n;x++)
        {
            float u=x/(float)n*2-1, v=z/(float)n*2-1;
            float radius=Mathf.Max(Mathf.Abs(u),Mathf.Abs(v));
            float roundedX=u*Mathf.Sqrt(1-v*v*.5f), roundedZ=v*Mathf.Sqrt(1-u*u*.5f);
            float irregular=1+.035f*Mathf.Sin(u*11+v*5)+.02f*Mathf.Cos(v*15-u*4);
            float px=Mathf.Lerp(roundedX*.32f*irregular,u*.5f,spread);
            float pz=Mathf.Lerp(roundedZ*.32f*irregular,v*.5f,spread);
            float swirl=Mathf.Sin(u*10+v*6+Mathf.Sin(v*7))*.035f+Mathf.Cos(v*13-u*5)*.022f;
            float clump=.26f+1.7f*Mathf.Pow(1-radius*radius, .65f)+swirl*2.5f;
            float even=(baked?.91f:.73f)+swirl*(baked?1.5f:2.4f);
            vertices.Add(new Vector3(px,Mathf.Lerp(clump,even,spread),pz)); uv.Add(new Vector2(x/(float)n,z/(float)n));
        }
        for(int z=0;z<n;z++) for(int x=0;x<n;x++)
        { int a=z*(n+1)+x,b=a+1,c=a+n+1,d=c+1;
          triangles.AddRange(new[]{a,c,b,b,c,d}); }
        List<int> edge=new();
        for(int x=0;x<=n;x++)edge.Add(x);
        for(int z=1;z<=n;z++)edge.Add(z*(n+1)+n);
        for(int x=n-1;x>=0;x--)edge.Add(n*(n+1)+x);
        for(int z=n-1;z>0;z--)edge.Add(z*(n+1));
        int bottom=vertices.Count;
        foreach(int i in edge){var p=vertices[i];vertices.Add(new Vector3(p.x,0,p.z));uv.Add(uv[i]);}
        int center=vertices.Count;vertices.Add(Vector3.zero);uv.Add(Vector2.zero);
        for(int i=0;i<edge.Count;i++) {int j=(i+1)%edge.Count;
            triangles.AddRange(new[]{edge[i],bottom+j,bottom+i,edge[i],edge[j],bottom+j,center,bottom+i,bottom+j});}
        Mesh mesh=new(){name=baked?"Brownie baked volume":"Brownie spread "+spread};
        mesh.SetVertices(vertices);mesh.SetUVs(0,uv);
        if(baked) {mesh.subMeshCount=2;mesh.SetTriangles(triangles.GetRange(0,n*n*6),0);mesh.SetTriangles(triangles.GetRange(n*n*6,triangles.Count-n*n*6),1);}
        else mesh.SetTriangles(triangles,0);
        mesh.RecalculateNormals();mesh.RecalculateTangents();mesh.RecalculateBounds();return mesh;
    }
}
