using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;

public partial class ConvexHull
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Chan%27s_algorithm - i tried implementing from wiki, but could not understand how javis binary searchwork
    /// there are multiple differnt naming for functions and algorithm psudocode online, which makes it confusing and a non explain each step in detail.
    /// This implementation is a direct c# conversion of https://github.com/lisa-yaqing-xu/ChanConvexHull
    /// </summary>
    public static List<Vector2> ChanAlgorithm(List<Vector2> points)
    {
        if (points.Count <= 3) return points;

        List<Vector2> finalHull = null;
        List<List<Vector2>> partialHulls = null;

        for (int t = 1; finalHull == null; t++)
        {
            int m = (int)Math.Min(Math.Pow(2, Math.Pow(2, t)), points.Count);

            partialHulls = calculatePartialHulls(m, points);
            finalHull = jarvisMarch(m, partialHulls);
        }

        return finalHull;

        List<List<Vector2>> calculatePartialHulls(int m, List<Vector2> points)
        {
            int numpartition = (int)Math.Ceiling((double)points.Count / m);
            int ph_index = 0;

            List<List<Vector2>> partition = new List<List<Vector2>>();
            partition.Add(new List<Vector2>());
            for (int i = 0; i < points.Count; i++)
            {
                if (i >= (ph_index + 1) * m)
                {
                    ph_index++;
                    partition.Add(new List<Vector2>());
                }
                partition[ph_index].Add(points[i]);
            }
            List<List<Vector2>> hulls = new List<List<Vector2>>();
            for (int i = 0; i < partition.Count; i++)
            {
                hulls.Add(GrahamScan(partition[i]));
            }

            return hulls;
        }

        List<Vector2> jarvisMarch(int m, List<List<Vector2>> subHulls)
        {
            //We do not need to Jarvis march if there is only one subhull. This is our convex hull.
            if (subHulls.Count == 1) return subHulls[0];

            //sort our sub hulls by their lowest point.
            subHulls.Sort(delegate (List<Vector2> a, List<Vector2> b) {
                if (a[0].y < b[0].y) return 1;
                else return -1;
            });

            List<Vector2> convexhull = new List<Vector2>();
            convexhull.Add(subHulls[0][0]);

            //initial search point set to (0, first point in the full hull) for tangent search purposes
            Vector2 p0 = new Vector2(0, convexhull[0].y);

            for (int i = 0; i < m; i++)
            {
                double maxAngle = -99999999;
                Vector2 pk_1 = default;
                Vector2 last = (i == 0) ? p0 : convexhull[i - 1];
                for (int j = 0; j < subHulls.Count; j++)
                {
                    int result = tangentBinarySearch(subHulls[j], last, convexhull[i]);
                    double angle = getAngleBetween3Points(last, convexhull[i], subHulls[j][result]);

                    if (!double.IsNaN(angle) && angle > maxAngle)
                    {
                        maxAngle = angle;
                        pk_1 = subHulls[j][result];
                    }
                }
                //we went full circle, have convex hull
                if (pk_1.x == convexhull[0].x && pk_1.y == convexhull[0].y)
                {
                    return convexhull;
                }
                convexhull.Add(pk_1);
            }
            return null;
        }

        double getAngleBetween3Points(Vector2 pt1, Vector2 pt2, Vector2 pt3)
        {
            double ab = Math.Sqrt(Math.Pow(pt2.x - pt1.x, 2) + Math.Pow(pt2.y - pt1.y, 2));
            double bc = Math.Sqrt(Math.Pow(pt2.x - pt3.x, 2) + Math.Pow(pt2.y - pt3.y, 2));
            double ac = Math.Sqrt(Math.Pow(pt3.x - pt1.x, 2) + Math.Pow(pt3.y - pt1.y, 2));
            return Math.Acos((bc * bc + ab * ab - ac * ac) / (2 * bc * ab));
        }

        int tangentBinarySearch(List<Vector2> hull, Vector2 p1, Vector2 p2)
        {
            //int index = -1;
            //int value = -999;
            int length = hull.Count;

            int start = 0;
            int end = length - 1;
            int leftSplit = -1;
            int rightSplit = -1;

            //int direction = null;
            int searchSize = (end - start) + 1;
            //doing a variation of binary search by comparing range of values instead-- because the order is wrapped around
            //the section containing the larger value will be larger on the ends

            if (searchSize == 1)
            {
                return 0;
            }
            else if (searchSize == 2)
            {
                double ret0 = findAngle(0);
                double ret1 = findAngle(1);

                if (ret0 > ret1)
                {
                    return 0;
                }

                return 1;
            }
            while (searchSize > 2)
            {
                searchSize = (end - start) + 1;

                double startAngle = findAngle(start);
                double endAngle = findAngle(end);
                int split = (int)Math.Floor((searchSize) / 2.0) + start;
                int mid = -1;
                if (searchSize % 2 == 0)
                {//even case
                    leftSplit = split - 1;
                    rightSplit = split;
                }
                else
                {
                    mid = split;
                    leftSplit = split - 1;
                    rightSplit = split + 1;
                }

                double leftAngle = findAngle(leftSplit);
                double rightAngle = findAngle(rightSplit);
                double midAngle = mid != -1 ? findAngle(mid) : -9999;
                double maxLeft = Math.Max(startAngle, leftAngle);
                double maxRight = Math.Max(rightAngle, endAngle);

                if (midAngle >= leftAngle && midAngle >= rightAngle)
                {
                    return mid;
                }
                else if (maxLeft > maxRight)
                {
                    end = leftSplit;
                    if (startAngle == leftAngle) return end;
                }
                else
                {
                    start = rightSplit;
                    if (rightAngle == endAngle) return start;
                }
            }
            return start;

            double findAngle(int param)
            {
                return (p2 == hull[param]) ? -999 : getAngleBetween3Points(p1, p2, hull[param]);
            }
        }
    }
}
