using UnityEngine;

public class Vector2Extension
{
    /// <summary>
    /// compute ff points are going to the left or right from vector v1->v2 using cross product
    /// returns 1 if going to the left. -1 if going to the right, 0 if going straight
    /// </summary>
    public static int Orientation(Vector2 p1, Vector2 p2, Vector2 p)
    {
        float val = (p2.x - p1.x) * (p.y - p1.y) -
                    (p2.y - p1.y) * (p.x - p1.x);

        if (val == 0) return 0;  // colinear
        return (val > 0) ? 1 : -1; // CCW or CW
    }
    public static float Distance(Vector2 p1, Vector2 p2, Vector2 p)
    {
        return Mathf.Abs((p.y - p1.y) * (p2.x - p1.x) - (p2.y - p1.y) * (p.x - p1.x));
    }
}
