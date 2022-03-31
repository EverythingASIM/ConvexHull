using GK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvexHull3D_Example : MonoBehaviour
{
    public enum Algorithm
    {
        Giftwrapping,
        QuickHull
    }

    public Algorithm algorithm = Algorithm.Giftwrapping;

    [SerializeField] int pointCount = 20;
    [SerializeField] GameObject prefab;
    [SerializeField] MeshFilter meshFilter;

    List<GameObject> points3d = new List<GameObject>();
    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();
    List<Vector3> normals = new List<Vector3>();

    void Start()
    {
        for (int i = 0; i < pointCount; i++)
        {
            points3d.Add(Instantiate(prefab));
            points3d[i].transform.position = Random.insideUnitSphere * 10;
        }
    }
    void Update()
    {
        if (points3d.Count == 0) return;

        List<Vector3> points = new List<Vector3>();
        foreach (GameObject go in points3d)
        {
            if (go)
            {
                points.Add(go.transform.position);
            }
        }

        verts.Clear();
        tris.Clear();
        normals.Clear();
        switch (algorithm)
        {
            case Algorithm.Giftwrapping:
                {
                    ConvexHull.GiftWrapping(points, ref verts, ref tris, ref normals);
                    break;
                }
            case Algorithm.QuickHull:
                {
                    new ConvexHullCalculator().GenerateHull(points, true, ref verts, ref tris, ref normals);
                    break;
                }
        }
        if (meshFilter)
        {
            if (!meshFilter.sharedMesh) meshFilter.sharedMesh = new Mesh();
            Mesh mesh = meshFilter.sharedMesh;
            mesh.Clear(); //need to remeber to clear first,because setting verts,tris and normals some takes time
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < verts.Count; i++)
        {
            Gizmos.DrawSphere(verts[i], 0.05f);
        }
    }
}
