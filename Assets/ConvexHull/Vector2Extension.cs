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

    /// <summary>
    /// return the max distance of p of two other points
    /// </summary>
    public static float MaxDistance(Vector2 p,Vector2 p1, Vector2 p2) 
    {
        return Mathf.Abs((p.y - p1.y) * (p2.x - p1.x) - (p2.y - p1.y) * (p.x - p1.x));
    }

    //SignedVolume are volumes that can be either positive or negative, depending on the winding
    //depending on the orientation in space of the region whose volume is being measured.
    //The volume is positive if d is to the left of the plane defined by the triangle(a, b, c).
    //IMPORTANT NOTE, this might result in floating point precision issue, which causes wrong values
    public static double SignedVolume(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        //first check if any points are same this will prevent possible floating point errors when doing dot and cross product
        if (a == b || b == c || a == c) return 0;

        return Vector3.Dot(a - d, Vector3.Cross(b - d, c - d)) / 6;
    }
}
