using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;

public partial class ConvexHull
{
    //https://en.wikipedia.org/wiki/Quickhull
    //uses divide and conqure,
    //1. first construct the largest triangle, buy using 3 points, the minimum point , and maximum point, and the point with the largest distance to vector min->max
    //2. the min and max point, and third point of triangle are confirmed to be inside the hull
    //3. split the remaining points to left side of the triangle, and right side of triangle, discard points inside the triangle
    //4. recursively repeat the steps 1,2, until theres no more points
    public static List<Vector2> hull = new List<Vector2>();
    public static List<Vector2> QuickHull2D(List<Vector2> points)
    {
        if (points.Count < 3) return points;

        hull.Clear();

        List<Vector2> resultList = new List<Vector2>();

        //1.find min and max point
        Vector2 minPoint = points[0];
        Vector2 maxPoint = points[0];
        for (int i = 1; i < points.Count; i++)
        {
            if (points[i].x < minPoint.x)
            {
                minPoint = points[i];
            }
            else if (points[i].x == minPoint.x &&
                    points[i].y < minPoint.y)
            {
                minPoint = points[i];
            }
            if (points[i].x > maxPoint.x)
            {
                maxPoint = points[i];
            }
            else if (points[i].x == maxPoint.x &&
                    points[i].y > maxPoint.y)
            {
                maxPoint = points[i];
            }
        }

        //2. add to hull
        hull.Add(minPoint);
        hull.Add(maxPoint);

        //3. split points base on left, and right of line/vector min->max 
        List<Vector2> left = new List<Vector2>();
        List<Vector2> right = new List<Vector2>();
        for (int i = 0; i < points.Count; i++)
        {
            int o = Vector2Extension.Orientation(minPoint, maxPoint, points[i]);
            if (o > 0)
            {
                left.Add(points[i]);

            }
            else if (o < 0)
            {
                right.Add(points[i]);
            }
        }

        //4. find hull points for the left points and right points
        FindHull(left, minPoint, maxPoint);
        FindHull(right, maxPoint, minPoint);

        return hull;

        void FindHull(List<Vector2> points, Vector2 P, Vector2 Q)
        {
            if (points.Count == 0) return;

            int pos = hull.IndexOf(Q);
            if (points.Count == 1)
            {
                hull.Insert(pos, points[0]);
                return;
            }

            //5. Check all points, which has the largest distance to P, Q
            float maxdist = 0;
            int maxPointIndex = 0;
            for (int i = 0; i < points.Count; i++)
            {
                float dist = Vector2Extension.MaxDistance(points[i],P, Q);
                if (dist > maxdist)
                {
                    maxdist = dist;

                    maxPointIndex = i;
                }
            }
            hull.Insert(pos, points[maxPointIndex]);

            //create a triangle, and split points base on left or right of the triangle, ignoring points that are inside
            List<Vector2> P_Points = new List<Vector2>();
            List<Vector2> Points_Q = new List<Vector2>();
            for (int i = 0; i < points.Count; i++)
            {
                int o = Vector2Extension.Orientation(P, points[maxPointIndex], points[i]);
                if (o > 0)
                {
                    P_Points.Add(points[i]);
                }
                else
                {
                    o = Vector2Extension.Orientation(points[maxPointIndex], Q, points[i]);
                    if (o > 0)
                    {
                        Points_Q.Add(points[i]);
                    }
                }
            }

            //6. recursive
            FindHull(P_Points, P, points[maxPointIndex]);
            FindHull(Points_Q, points[maxPointIndex], Q);
        }
    }
}
