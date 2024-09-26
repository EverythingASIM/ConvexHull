using UnityEngine;

using System.Collections.Generic;

using asim.unity.utils.geometry;
using asim.unity.extensions;

public partial class ConvexHull
{
    //Incremental Convex Hull? by Michael Kallay.? Naive
    //https://dccg.upc.edu/wp-content/uploads/2020/10/3-DAG-Convex-Hull-2D.pdf
    //https://www.cs.jhu.edu/~misha/Spring16/07.pdf
    //Uses Orientation or 3 points
    //Uses modulus for looping backwards
    //The idea is to start with a small hull, and grow the hull by adding points one by one
    //Check to see if new point is inside or outside the hull by comparing with the convexHull orientation
    public static List<Vector2> Incremental(List<Vector2> points)
    {
        if (points.Count < 3) return points;

        //1. Start with first 3 points to be part of the hull
        Vector2 InitP1 = points[0];
        Vector2 InitP2 = points[1];
        Vector2 InitP3 = points[2];
        List<Vector2> convexHull = new () { InitP1, InitP2, InitP3 };

        //1a. Calculate the direction of the hull, from the initial 3 points, this also determins which direction to check, if a point is inside or outside the hull
        int ConvexDir = GeometryUtils.Orientation(InitP1, InitP2, InitP3);
        
        //2. Use For every new point
        for (int i = 3; i < points.Count; i++)
        {
            var newPoint = points[i];

            int? pli = null;
            int? pri = null;

            //2b. Check Convex hull for the first edge whose points with new Point has a "right" orintation.
            for (int j = 0; j < convexHull.Count; j++)
            {
                int pcurri = j;
                int pnext = (j + 1) % convexHull.Count;
                var edge = (convexHull[pcurri], convexHull[pnext]);
                int orientation = GeometryUtils.Orientation(edge.Item1, edge.Item2, newPoint);

                //2c. If found an edge with "right" oreintation,set pl,pr : point left,point right
                //2d. Contiue to see if the next edge, is also having the same oreintation, if so, then this is a "chain"
                //2e. Continue the chain, find the last point on the chain, and update "pr" : point right most of the chain
                if (orientation != ConvexDir)
                {
                    pri = pnext;

                    if (pli.HasValue == false)
                    {
                        pli = pcurri;
                    }
                }
                else
                {
                    if (pri.HasValue)
                    {
                        break;
                    }
                }
            }

            //2f. If found no pr, means the newPoint is inside the convext hull, we can skip
            if (pri.HasValue == false) continue;

            //3. Loop backwards, starting from 0, count - 1, count - 2 ..... 0, look if backward edges have "right orentation",
            for (int j = 0; j < convexHull.Count - 1; j++)
            {
                int pcurri = MathExtensions.mod(convexHull.Count - j, convexHull.Count);
                int pprevi = MathExtensions.mod(convexHull.Count - j - 1, convexHull.Count);
                var edge = (convexHull[pprevi], convexHull[pcurri]);
                int orientation = GeometryUtils.Orientation(edge.Item1, edge.Item2, newPoint);

                //3b. Continue the chain, backwards, and update "pl" : point left most of the chain
                if (orientation != ConvexDir)
                {
                    pli = pprevi;
                }
                else
                {
                    break;
                }
            }

            //4. After finding pl,pr, replace the points in between with the new point
            convexHull.AddReplaceBetween(pli.Value, pri.Value, points[i]);
        }

        return convexHull;
    }
}
