using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;

public partial class ConvexHull
{
    /// <summary>
    /// code source http://www.ams.sunysb.edu/~jsbm/courses/345/13/melkman.pdf 
    /// explanation http://cgm.cs.mcgill.ca/~athens/cs601/Melkman.html
    /// this asumes that no 3 points in a row are colinear
    /// only works with points connected to form a simple polygon (no intersecting lines)
    /// </summary>
    public static List<Vector2> Melkman(List<Vector2> points)
    {
        if (points.Count < 3) return null;

        Vector2 p0 = points[0];
        Vector2 p1 = points[1];
        Vector2 p2 = points[2];

        LinkedList<Vector2> deque = new LinkedList<Vector2>();

        //1. check if first 3 points are going to the left or right, insert to deque accordingly
        if (Vector2Extension.Orientation(p0, p1, p2) == 1)
        {
            deque.AddFirst(p0);
            deque.AddFirst(p1);
            deque.AddFirst(p2);

            deque.AddLast(p2);
        }
        else
        {
            deque.AddFirst(p1);
            deque.AddFirst(p0);
            deque.AddFirst(p2);

            deque.AddLast(p2);
        }
        int i = 3;

        try
        {

            while (i < points.Count)
            {
                //2.
                while ((Vector2Extension.Orientation(deque.First.Next.Value, deque.First.Value, points[i]) == 1) &&
                    (Vector2Extension.Orientation(deque.Last.Value, deque.Last.Previous.Value, points[i]) == 1))
                {
                    i++;

                    if (i >= points.Count) return deque.ToList();
                }

                //3
                while (Vector2Extension.Orientation(deque.First.Next.Value, deque.First.Value, points[i]) != 1)
                {
                    deque.RemoveFirst();
                }
                deque.AddFirst(points[i]);

                while (Vector2Extension.Orientation(points[i], deque.Last.Value, deque.Last.Previous.Value) != 1)
                {
                    deque.RemoveLast();
                }
                deque.AddLast(points[i]);

                i++;
            }
        }
        catch
        {
            Debug.LogError(i);
        }

        return deque.ToList();
    }
}
