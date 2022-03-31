using UnityEngine;

using System.Collections.Generic;

public class MelkmanConvexHull_Example : MonoBehaviour
{
    public MeshFilter meshFilter;
    public Transform pointsContainer;

    List<Vector2> points;
    List<Vector2> drawpoints;

    void Update()
    {
        points = new List<Vector2>();
        foreach (Transform child in pointsContainer)
        {
            points.Add(child.transform.position);
        }

        List<Vector3> verts = new List<Vector3>();
        List<int> indexs = new List<int>();
        foreach (var p in points)
        {
            indexs.Add(verts.Count);
            verts.Add(new Vector3(p.x, p.y, 0));
        }

        if (meshFilter)
        {
            if (!meshFilter.sharedMesh) meshFilter.sharedMesh = new Mesh();
            Mesh mesh = meshFilter.sharedMesh;
            mesh.Clear(); //need to remeber to clear first,because setting verts,tris and normals some takes time
            mesh.SetVertices(verts);
            mesh.SetIndices(indexs, MeshTopology.LineStrip, 0);
        }

        drawpoints = ConvexHull.Melkman(points);
    }
    void OnDrawGizmos()
    {
        if (drawpoints == null || drawpoints.Count == 0) return;

        //Draw (looped) Points
        Gizmos.color = Color.red;
        for (int i = 0; i < drawpoints.Count; i++)
        {
            Vector2 start = drawpoints[i];
            Vector2 end = drawpoints[(i + 1) % drawpoints.Count];
            Gizmos.DrawLine(new Vector3(start.x, start.y, 0), new Vector3(end.x, end.y, 0));
        }
    }
}
