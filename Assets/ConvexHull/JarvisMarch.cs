using UnityEngine;

using System;
using System.Collections.Generic;

public partial class ConvexHull
{
    //https://en.wikipedia.org/wiki/Gift_wrapping_algorithm#Complexity
    //points are looped
    //1. Start with left most point,
    //2. for each of the remaining the points, calculate which one has the lowest angle(turn), and add to the list
    //3. take a latest added point and repeat the process (step 2)
    //4. ends when the latest added point is the same as the starting point
    //5. Start point == end point
    public static List<Vector2> JarvisMarch(List<Vector2> points)
    {
        if (points.Count == 0) return null;

        //a.First get the left most point
        Vector2 leftMostPoint = points[0];
        float minX = points[0].x;
        for (int index = 1; index < points.Count; index++)
        {
            if (points[index].x < minX)
            {
                leftMostPoint = points[index];
                minX = points[index].x;
            }
        }
        //b.Create output list, and added the left most point as the first point
        List<Vector2> P = new List<Vector2>();
        P.Add(leftMostPoint);

        //c.Loop though all points
        int i = 0;
        Vector2 currentEndPoint;
        do
        {
            currentEndPoint = points[0];
            for (int j = 1; j < points.Count; j++)
            {
                //c.a.check if current end point is the first point by chance , just continue
                if (currentEndPoint == leftMostPoint && i == 0)
                {
                    currentEndPoint = points[j];
                }
                else
                {
                    //check if both are same points, skip
                    if (currentEndPoint.x == points[j].x && currentEndPoint.y == points[j].y) continue;
                    //check the next point to see if the next point is left of our currentLine
                    //get slope of currentpointline vs new slope of nextpointline slope/gradient = y/x
                    float pos = (currentEndPoint.x - P[i].x) * (points[j].y - P[i].y) - (currentEndPoint.y - P[i].y) * (points[j].x - P[i].x);

                    //Check for Collinarty when both slopes is the same
                    if (pos == 0)
                    {
                        //check if the new line is longer,change point
                        if (Vector2.Distance(points[j], P[i]) > Vector2.Distance(currentEndPoint, P[i]))
                        {
                            currentEndPoint = points[j];
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (pos > 0) //isnewpointonleftofline
                    {
                        currentEndPoint = points[j];   // found greater left turn, update endpoint
                    }
                }
            }
            P.Add(currentEndPoint);
            i++;
        }
        while (currentEndPoint.x != P[0].x || currentEndPoint.y != P[0].y);

        return P;
    }

    //code refernece : https://www.cs.jhu.edu/~misha/Spring16/09.pdf
    //todo-calculate normals
    //some issue when some points are co-planar
    //1. First calculate the first Face/Triangle that will lie on the convexhull, by
    public class GiftWrappingEdge
    {
        public GiftWrappingEdge(Vector3 start, Vector3 end)
        {
            StartPoint = start;
            EndPoint = end;
        }
        public Vector3 StartPoint;
        public Vector3 EndPoint;
    }
    public static void GiftWrapping(List<Vector3> points, ref List<Vector3> verts, ref List<int> tris, ref List<Vector3> normals)
    {
        //1. Get the first Face that lies tangent to the convex hull , by first obtaining an edge on the hull,
        //lookthough all points, and checking to see which set of triangles(edge+point) is the CCW most or CW most rotating using the edge as the pivot
        Vector3[] firstFace = FindFirstFaceOnHull(points);

        //2. once we got our first triangle/face, add it to our mesh, and queue the 3 edges.
        //order matters, as it will affect when we add verts,and tris in drawing order
        Stack<GiftWrappingEdge> EdgesToCheck = new Stack<GiftWrappingEdge>();
        EdgesToCheck.Push(new GiftWrappingEdge(firstFace[1], firstFace[0]));
        EdgesToCheck.Push(new GiftWrappingEdge(firstFace[2], firstFace[1]));
        EdgesToCheck.Push(new GiftWrappingEdge(firstFace[0], firstFace[2]));
        verts.Add(firstFace[0]);
        verts.Add(firstFace[1]);
        verts.Add(firstFace[2]);
        tris.Add(0);
        tris.Add(1);
        tris.Add(2);

        List<string> processedEdges = new List<string>();

        //3. for each edges queued, we use one by one as the base edge, and run though PivotOnEdge,
        //to find which new third point, can be used with the base edge to construct a triangle that will lie on the convex hull
        while (EdgesToCheck.Count != 0)
        {
            GiftWrappingEdge edge = EdgesToCheck.Pop();
            if (NotProcessed(edge))
            {
                Vector3 q = PivotOnEdge(edge, points);

                //4. when we found our third point , this forms a new triangle so just add it to the mesh
                //edge start & edge end is already part of our mesh verts list, check to see if our third point is already included else we add it as a new vert
                //Reuse verts by finding within verts list, - will slow down a generation, but will reduce mesh vert count
                //e0,e1 is already part of verts list, check if q is already included, so we try find a reuse index
                int t1 = verts.IndexOf(edge.StartPoint);
                int t2 = verts.IndexOf(edge.EndPoint);
                int t3 = verts.IndexOf(q);
                tris.Add(t1);
                tris.Add(t2);
                if (t3 == -1)
                {
                    verts.Add(q);
                    t3 = verts.Count - 1;
                }
                tris.Add(t3);

                //or just add verts and tris for each triangle
                //verts.Add(edge.StartPoint); tris.Add(verts.Count - 1);
                //verts.Add(edge.EndPoint); tris.Add(verts.Count - 1);
                //verts.Add(q); tris.Add(verts.Count - 1);

                //TODO: Optimize, there is a chance that the triangle already added, but the triangle we are added is just in different order. (adding them anyways for now)
                tris.Add(t1);
                tris.Add(t2);
                tris.Add(t3);

                Vector3[] newFace = new Vector3[] { edge.StartPoint, edge.EndPoint, q };
                GiftWrappingEdge edge1 = new GiftWrappingEdge(newFace[1], newFace[0]);
                GiftWrappingEdge edge2 = new GiftWrappingEdge(newFace[2], newFace[1]);
                GiftWrappingEdge edge3 = new GiftWrappingEdge(newFace[0], newFace[2]);

                //5. check the edges of the new triangle/face if it's already processed, else queue it to process next loop
                EdgesToCheck.Push(edge1);
                EdgesToCheck.Push(edge2);
                EdgesToCheck.Push(edge3);

                //6. remeber the edge that we processed
                MarkProcessedEdges(edge);
            }
        }

        return;

        bool NotProcessed(GiftWrappingEdge edge)
        {
            //Edge does not have a direction, so we check on both direction
            Vector3 v1 = edge.StartPoint;
            Vector3 v2 = edge.EndPoint;

            string id = $"{v1.x}{v1.y}{v1.z}{v2.x}{v2.y}{v2.z}";
            string id2 = $"{v2.x}{v2.y}{v2.z}{v1.x}{v1.y}{v1.z}";

            return !processedEdges.Contains(id);
        }
        void MarkProcessedEdges(GiftWrappingEdge edge)
        {
            //Save the Edge as Start End Vector Point as a unique string ID
            Vector3 v1 = edge.StartPoint;
            Vector3 v2 = edge.EndPoint;
            string id = $"{v1.x}{v1.y}{v1.z}{v2.x}{v2.y}{v2.z}";
            processedEdges.Add(id);
        }

        Vector3[] FindFirstFaceOnHull(List<Vector3> P)
        {
            FindEdgeOnHull(P, out GiftWrappingEdge edge);

            Vector3 r = PivotOnEdge(edge, P);

            return new Vector3[] { edge.StartPoint, edge.EndPoint, r };
        }

        void FindEdgeOnHull(List<Vector3> P, out GiftWrappingEdge edge)
        {
            //a. First get the BottomMostLeftMostBackMost point
            Vector3 p = P[0];
            for (int index = 1; index < points.Count; index++)
            {
                if (points[index].y < p.y)
                {
                    p = points[index];
                }
                else if (points[index].y == p.y && points[index].x < p.x)
                {
                    p = points[index];
                }
                else if (points[index].y == p.y && points[index].x == p.x && points[index].z > p.z)
                {
                    p = points[index];
                }
            }

            //b. loop though all points and check to see if we have any point that lies on the same YZ plane
            //not sure if this is needed, as most likely dont have a point, and even if there's a point,
            //from my understanding this only helps isolate and find the endpoint, but computational wise, it still goes though the entire point list anyways
            Vector3 q = p;
            foreach (Vector3 point in P)
            {
                if (q.z == point.z && q.y == point.y && q.x < point.x)
                {
                    q = point;
                }
            }

            //if theres no point on the same YZ plane, Created a temp point as the end point of the edge on the same YZ plane, any distance away 
            if (q == p)
            {
                q = p + new Vector3(1f, 0, 0);
            }

            //d. Using the temp Edge, right though PivotOnEdge to check and obtain a third point, which is guaranteed to be on convex hull
            GiftWrappingEdge tempedge = new GiftWrappingEdge(p, q);
            Vector3 r = PivotOnEdge(tempedge, P);

            edge = new GiftWrappingEdge(p, r);
        }
        //Complexity O(n)
        Vector3 PivotOnEdge(GiftWrappingEdge edge, List<Vector3> P)
        {
            //a. we first get any point to start constructing a triangle as our base of comparison
            //also record the unsigned area of the triangle - to be used later
            Vector3 p = P[0];
            double area2 = SquaredArea(edge.StartPoint, edge.EndPoint, p);

            for (int i = 1; i < P.Count; i++)
            {
                //b. for point calculate if the point lies Left or Right of Triangle
                //this can be done by using the signedvolume of a tetrahedron
                //IMPORTANT NOTE, this might result in floating point precision issue, which causes wrong values
                double volume = SignedVolume(edge.StartPoint, edge.EndPoint, p, P[i]);

                //b. check and see what sides the point lies on the our triangle
                //checking volume < 0 or volume > 0, CCW/CW doesnt not matter and is depends on the order of inputs to SignedVolume?
                if (volume < 0)
                {
                    p = P[i];
                    area2 = SquaredArea(edge.StartPoint, edge.EndPoint, p);
                }

                //c. if the volume is 0, means that all 4 points are co-planar (lies on the same triangle/plane) and does not create a tetrahedron
                // with that, we want to compare the new point with our original point to see which one creates the bigger triangle by just compare the area
                else if (volume == 0)
                {
                    double _area2 = SquaredArea(edge.StartPoint, edge.EndPoint, P[i]);
                    if (_area2 > area2)
                    {
                        p = P[i];
                        area2 = _area2;
                    }
                }
            }

            return p;
        }

        double SquaredArea(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            //Heron's Formula for the area of a triangle, use when you know length of triangle
            //p is half the perimeter, (a+b+c) /2
            //Area = sqrt(p * (p-a) * (p-b) * (p-c));

            double a = Vector3.Distance(p1, p2);
            double b = Vector3.Distance(p2, p3);
            double c = Vector3.Distance(p3, p1);

            double p = (a + b + c) / 2;
            double area = Math.Sqrt(p * (p - a) * (p - b) * (p - c));

            return area * area;
        }

        //SignedVolume are volumes that can be either positive or negative, depending on the winding
        //depending on the orientation in space of the region whose volume is being measured.
        //The volume is positive if 𝑑 is to the left of the plane defined by the triangle(𝑎, 𝑏, 𝑐).
        //IMPORTANT NOTE, this might result in floating point precision issue, which causes wrong values
        double SignedVolume(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            //first check if any points are same this will prevent possible floating point errors when doing dot and cross product
            if (a == b || b == c || a == c) return 0;

            return Vector3.Dot(a - d, Vector3.Cross(b - d, c - d)) / 6;
        }
    }
}