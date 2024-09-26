using System.Collections.Generic;

using UnityEngine;

[ExecuteInEditMode]
public class ConvexHull2D_Example : MonoBehaviour
{
    public enum Algorithm
    {
        JarvisMarch,
        GrahamScan,
        MonotoneChain,
        Incremental,
        QuickHull2D,
        //KirkpatrickSeidel,
        ChanAlgorithm,
    }

    [SerializeField] GameObject Points;

    public Algorithm algorithm = Algorithm.JarvisMarch;

    List<Vector2> drawpoints;

    void Update()
    {
        List<Vector2> points = new();
        for (int i = 0; i < Points.transform.childCount; i++)
        {
            var point = Points.transform.GetChild(i).transform.position;
            points.Add(point);
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
            case Algorithm.Incremental:
                {
                    drawpoints = ConvexHull.Incremental(points);
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
                Gizmos.DrawLine(start, end);
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
                Gizmos.DrawLine(start, end);
            }
            start = drawpoints[0];
            Gizmos.DrawLine(start, end);
        }
    }
}
