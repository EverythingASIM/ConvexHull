using UnityEngine;

using System.Linq;
using System.Collections.Generic;

public partial class ConvexHull
{
    //Similar to Graham_scan, but doubles the job into upper and lowerhull,
    //lowerhull starts from top left,
    //upperhull starts from bot right, checking for // goes clockwise, splits into upper and lower hull
    //can be obtimized memory by using one stack
    //can be obtimized by doing parrrelly
    public static List<Vector2> MonotoneChain(List<Vector2> points)
    {
        if (points.Count == 0) return null;

        //1.Sort the points of P by x-coordinate (in case of a tie, sort by y-coordinate)
        points.Sort(compare);

        //2a Sort lower hull
        Stack<Vector2> LowerPoints = new Stack<Vector2>();
        for (int i = 0; i < points.Count; i++)
        {
            while (LowerPoints.Count >= 2 && Vector2Extension.Orientation(nextToTop(LowerPoints), top(LowerPoints), points[i]) <= 0)//check for ccw
            {
                LowerPoints.Pop();//do not accept cw
            }

            LowerPoints.Push(points[i]);
        }

        //2b Sort upper hull - goes in reverse index
        Stack<Vector2> UpperPoints = new Stack<Vector2>();
        for (int i = points.Count - 1; i >= 0; i--)
        {
            while (UpperPoints.Count >= 2 && Vector2Extension.Orientation(nextToTop(UpperPoints), top(UpperPoints), points[i]) <= 0)//check for ccw
            {
                UpperPoints.Pop();//do not accept cw
            }

            UpperPoints.Push(points[i]);
        }

        UpperPoints.Pop();
        LowerPoints.Pop();

        return LowerPoints.Concat(UpperPoints).ToList();

        int compare(Vector2 v1, Vector2 v2)
        {
            if (v1.x == v2.x)
            {
                return v1.y >= v2.y ? 1 : -1;
            }
            else
            {
                return v1.x > v2.x ? 1 : -1;
            }
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
