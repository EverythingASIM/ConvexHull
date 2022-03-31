using System.Collections.Generic;

using UnityEngine;

public class ConvexHull2D_Example : MonoBehaviour
{
    public enum Algorithm
    {
        JarvisMarch,
        GrahamScan,
        MonotoneChain,
        QuickHull2D,
        //KirkpatrickSeidel,
        ChanAlgorithm,
    }

    public Algorithm algorithm = Algorithm.JarvisMarch;

    [SerializeField] int pointCount = 20;
    [SerializeField] GameObject prefab;

    List<GameObject> points2d = new List<GameObject>();
    List<Vector2> drawpoints;
    void Start()
    {
        for (int i = 0; i < pointCount; i++)
        {
            points2d.Add(Instantiate(prefab));
            points2d[i].transform.position = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
        }
    }
    void Update()
    {
        List<Vector2> points = new List<Vector2>();
        foreach (GameObject go in points2d)
        {
            if (go)
            {
                points.Add(go.transform.position);
            }
        }

        switch (algorithm)
        {
            case Algorithm.JarvisMarch:
                {
                    drawpoints = ConvexHull.JarvisMarch(points);
                    break;
                }
            case Algorithm.GrahamScan:
                {
                    drawpoints = ConvexHull.GrahamScan(points);
                    break;
                }
            case Algorithm.MonotoneChain:
                {
                    drawpoints = ConvexHull.MonotoneChain(points);
                    break;
                }
            case Algorithm.QuickHull2D:
                {
                    drawpoints = ConvexHull.QuickHull2D(points);
                    break;
                }
            //case Algorithm.KirkpatrickSeidel:
            //    {
            //        drawpoints = ConvexHull.kirkpatrick_seidel(points);
            //        break;
            //    }
            case Algorithm.ChanAlgorithm:
                {
                    drawpoints = ConvexHull.ChanAlgorithm(points);
                    break;
                }
        }
    }
    void OnDrawGizmos()
    {
        if (drawpoints == null || drawpoints.Count == 0) return;

        if(algorithm == Algorithm.JarvisMarch)
        {
            //Draw points without looping to start
            for (int i = 0; i < drawpoints.Count - 1; i++)
            {
                Vector2 start = drawpoints[i];
                Vector2 end = drawpoints[(i + 1)];
                Gizmos.DrawLine(new Vector3(start.x, start.y, 0), new Vector3(end.x, end.y, 0));
            }
        }
        else
        {
            //Draw points looping to start
            Vector2 start = default;
            Vector2 end = default;
            for (int i = 0; i < drawpoints.Count - 1; i++)
            {
                start = drawpoints[i];
                end = drawpoints[(i + 1)];
                Gizmos.DrawLine(new Vector3(start.x, start.y, 0), new Vector3(end.x, end.y, 0));
            }
            start = drawpoints[0];
            Gizmos.DrawLine(new Vector3(start.x, start.y, 0), new Vector3(end.x, end.y, 0));
        }
    }
}
