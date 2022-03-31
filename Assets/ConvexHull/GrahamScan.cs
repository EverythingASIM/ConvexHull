using UnityEngine;

using System.Linq;
using System.Collections.Generic;

public partial class ConvexHull
{
    //https://en.wikipedia.org/wiki/Graham_scan
    //Graham_scan 
    //1. start at bottom left most point,
    //2. sort all points based on the polar angle of point to bottom left most point
    //3. slowly loop though the points adding slowly to a list(stack is more efficient because of LIFO), check for each new added point,
    //the line is going inwards or outwards (couter-clockwise  or clockwise),
    //we only want added points results in lines that only goes couter clockwise,
    //so each time we add a new point, check the previously added points (last added order,pop()) to see which points will give a ccw,
    //and ommit points
    public static List<Vector2> GrahamScan(List<Vector2> points)
    {
        if (points.Count == 0) return null;

        //1.obtain point with lowest x,y position
        Vector2 P = points[0];
        for (int index = 1; index < points.Count; index++)
        {
            if (points[index].y < P.y)
            {
                P = points[index];
            }
            else if (points[index].y == P.y &&
                     points[index].x < P.x)
            {
                P = points[index];
            }
        }

        //2. sort points by polar angle with P, polar angle is angle on x axis, if points have the same polar angle then only keep the farthest
        //points = points.OrderBy(point => (point.y - P.y) / (point.x - P.x)).ToList();
        points.Sort(compare);

        //3.add all the points to a stack(to check one by one)
        Stack<Vector2> stack = new Stack<Vector2>();
        foreach (Vector2 point in points)
        {
            while (stack.Count >= 2 && Vector2Extension.Orientation(nextToTop(stack), top(stack), point) <= 0)//checking happens when stack is 2 or more
            {
                stack.Pop();//we remove this point, because we dont want the new points added to form a inner hole (by turning clockwise) (only accept turning of ccw)
            }
            stack.Push(point);
        }

        return stack.ToList();

        int compare(Vector2 v1, Vector2 v2)
        {
            // Find orientation
            int o = Vector2Extension.Orientation(P, v1, v2);
            if (o == 0)
            {
                return Vector2.Distance(P, v1) >= Vector2.Distance(P, v2) ? 1 : -1;
            }

            //if P,v1,v2 is going ccw, then v1 < v2; , p->v1 has a smaller polar angle than p->v2
            return (o == 1) ? -1 : 1;
        }

        Vector2 top(Stack<Vector2> stack)
        {
            return stack.Peek();
        }
        Vector2 nextToTop(Stack<Vector2> stack)
        {
            Vector2 p = stack.Pop();
            Vector2 nexttotop = stack.Peek();
            stack.Push(p);
            return nexttotop;
        }
    }
}
