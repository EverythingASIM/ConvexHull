using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;

public partial class ConvexHull
{
    public static void QuickHull3D(List<Vector3> points, ref List<Vector3> verts, ref List<int> tris, ref List<Vector3> normals)
    {
        //TODO - DirkGregorius_ImplementingQuickHull.pdf
    }

    //https://github.com/darshan3105/Convex-Hull---Jarvis-March-Graham-Scan-Kirkpatrick-Seidel-Algorithm/blob/master/classes/KPS.h
    //TODO - not working
    public static List<Vector2> kirkpatrick_seidel(List<Vector2> points)
    {
        if (points.Count < 3) return points;

        Vector2 pmin_u, pmin_l;
        Vector2 pmax_u, pmax_l;

        pmin_l = pmin_u = pmax_u = pmax_l = points[0];
        int i;
        for (i = 1; i < points.Count; i++)
        {
            Vector2 curr_point = points[i];
            if (curr_point.x < pmin_l.x)
            {
                pmin_l = curr_point;
                pmin_u = curr_point;
            }
            else if (curr_point.x > pmax_l.x)
            {
                pmax_l = curr_point;
                pmax_u = curr_point;
            }
            else if (curr_point.x == pmin_l.x)
            {
                if (curr_point.y > pmin_u.y)
                {
                    pmin_u = curr_point;
                }
                else if (curr_point.y < pmin_l.y)
                {
                    pmin_l = curr_point;
                }
            }
            else if (curr_point.x == pmax_l.x)
            {
                if (curr_point.y > pmax_u.y)
                {
                    pmax_u = curr_point;
                }
                else if (curr_point.y < pmax_l.y)
                {
                    pmax_l = curr_point;
                }
            }
        }

        List<Vector2> upper_T = get_T(pmin_u, pmax_u, points, false);
        List<Vector2> upper_hull = get_upper_hull(pmin_u, pmax_u, upper_T);

        List<Vector2> lower_T = get_T(pmin_l, pmax_l, points, true);
        List<Vector2> lower_hull = get_lower_hull(pmin_l, pmax_l, lower_T);


        List<Vector2> hull_edges = new List<Vector2>();
        hull_edges.AddRange(upper_hull);
        hull_edges.AddRange(lower_hull);

        if (pmin_u != pmin_l)
        {
            hull_edges.Add(pmin_l);
            hull_edges.Add(pmin_u);
        }
        if (pmax_l != pmax_u)
        {
            hull_edges.Add(pmax_l);
            hull_edges.Add(pmax_u);
        }

        hull_edges.Sort(delegate (Vector2 a, Vector2 b) {
            if (a.x < b.x)
            {
                return 1;
            }
            else if (a.x > b.x)
            {
                return -1;
            }
            else
            {
                return a.y < b.y ? 1:-1;
            }
        });

        List<Vector2> hull = new List<Vector2>();
        hull.Add(hull_edges[0]);
        i = 1;
        while (i < hull_edges.Count)
        {
            while (i < hull_edges.Count && hull_edges[i] == hull_edges[i - 1])
                i++;

            if (i < hull_edges.Count)
                hull.Add(hull_edges[i]);

            i++;
        }

        return hull;

        List<Vector2> get_lower_hull(Vector2 pmin, Vector2 pmax, List<Vector2> points)
        {
            List<Vector2> lower_hull = new List<Vector2>(points.Count);
            int n = points.Count;
            double[] arr = new double[n];
            for (int i = 0; i < n; i++)
            {
                arr[i] = points[i].x;
            }
            double median;
            if (n == 1)
                median = arr[0];
            else
                median = kthSmallest(arr, 0, n - 1, (n + 1) / 2);
            Tuple<Vector2, Vector2> lower_bridge = get_lower_bridge(points, median);

            Vector2 pl = lower_bridge.Item1;
            Vector2 pr = lower_bridge.Item2;

            if (pl.x > pr.x)
            {
                Vector2 temp = pl;
                pl = pr;
                pr = temp;
            }

            lower_hull.Add(pl);
            lower_hull.Add(pr);

            if (pmin !=pl)
            {
                List<Vector2> lower_T_left = get_T(pmin, pl, points, true);
                List<Vector2> left = get_lower_hull(pmin, pl, lower_T_left);
                lower_hull.AddRange(left);
            }
            if (pmax != pr)
            {
                List<Vector2> lower_T_right = get_T(pr, pmax, points, true);
                List<Vector2> right = get_lower_hull(pr, pmax, lower_T_right);
                lower_hull.AddRange(right);
            }

            return lower_hull;
        }

        List<Vector2> get_upper_hull(Vector2 pmin, Vector2 pmax, List<Vector2> points)
        {

            List<Vector2> upper_hull = new List<Vector2>(points.Count);
            int n = points.Count;
            double[] arr = new double[n];
            for (int i = 0; i < n; i++)
            {
                arr[i] = points[i].x;
            }

            double median;
            if (n == 1)
                median = arr[0];
            else
                median = kthSmallest(arr, 0, n - 1, (n + 1) / 2);
            Tuple<Vector2, Vector2> upper_bridge = get_upper_bridge(points, median);

            Vector2 pl = upper_bridge.Item1;
            Vector2 pr = upper_bridge.Item2;

            if (pl.x > pr.x)
            {
                Vector2 temp = pl;
                pl = pr;
                pr = temp;
            }

            upper_hull.Add(pl);
            upper_hull.Add(pr);

            if (pmin != pl)
            {
                List<Vector2> upper_T_left = get_T(pmin, pl, points, false);
                List<Vector2> left = get_upper_hull(pmin, pl, upper_T_left);
                upper_hull.AddRange(left);
            }

            if (pmax != pr)
            {
                List<Vector2> upper_T_right = get_T(pr, pmax, points, false);
                List<Vector2> right = get_upper_hull(pr, pmax, upper_T_right);
                upper_hull.AddRange(right);
            }

            return upper_hull;
        }

        Tuple<Vector2, Vector2> get_lower_bridge(List<Vector2> points, double median)
        {
            points.Sort(delegate (Vector2 a, Vector2 b)
            {
                return a.x < b.x ? 1 : -1;
            });

            List<Vector2> candidates = new List<Vector2>(points.Count);
            List<Tuple<Vector2, Vector2>> pairs = new List<Tuple<Vector2, Vector2>>(points.Count / 2 + 1);

            if (points.Count % 2 == 0)
            {
                for (int i = 0; i < points.Count; i += 2)
                {
                    Vector2 first_pt = points[i];
                    Vector2 second_pt = points[i + 1];

                    Tuple<Vector2, Vector2> curr_pair = new Tuple<Vector2, Vector2>(first_pt, second_pt);
                    pairs.Add(curr_pair);
                }
            }
            else
            {
                candidates.Add(points[0]);
                for (int i = 1; i < points.Count; i += 2)
                {
                    Vector2 first_pt = points[i];
                    Vector2 second_pt = points[i + 1];

                    Tuple<Vector2, Vector2> curr_pair = new Tuple<Vector2, Vector2>(first_pt, second_pt);
                    pairs.Add(curr_pair);
                }
            }

            int slopes_len = pairs.Count;
            double[] slopes = new double[slopes_len];
            for (int i = 0; i < pairs.Count; i++)
            {

                Vector2 p1 = pairs[i].Item1;
                Vector2 p2 = pairs[i].Item2;
                double x1 = p1.x;
                double x2 = p2.x;
                double y1 = p1.y;
                double y2 = p2.y;

                if (x1 == x2)
                {
                    if (y1 > y2)
                    {
                        candidates.Add(p2);
                    }
                    else
                    {
                        candidates.Add(p1);
                    }
                    slopes[i] = int.MaxValue;
                }
                else
                {
                    double slope = (y2 - y1) / (x2 - x1);
                    slopes[i] = slope;
                }

            }

            double[] arr = new double[slopes_len];
            int len = 0;
            for (int i = 0; i < slopes_len; i++)
            {
                if (slopes[i] != int.MaxValue)
                {
                    arr[len++] = slopes[i];
                }
            }

            double median_slope;
            if (len == 1)
                median_slope = arr[0];
            else
                median_slope = kthSmallest(arr, 0, len - 1, (len + 1) / 2);

            List<Tuple<Vector2, Vector2>> SMALL = new List<Tuple<Vector2, Vector2>>(pairs.Count);
            List<Tuple<Vector2, Vector2>> EQUAL = new List<Tuple<Vector2, Vector2>>(pairs.Count);
            List<Tuple<Vector2, Vector2>> LARGE = new List<Tuple<Vector2, Vector2>>(pairs.Count);

            for (int i = 0; i < pairs.Count; i++)
            {
                Vector2 p1 = pairs[i].Item1;
                Vector2 p2 = pairs[i].Item2;
                double x1 = p1.x;
                double x2 = p2.x;
                double y1 = p1.y;
                double y2 = p2.y;

                if (x1 != x2)
                {
                    double slope = (y2 - y1) / (x2 - x1);
                    if (abss(slope - median_slope) < 0.001)
                    {
                        Tuple<Vector2, Vector2> curr_pair = new Tuple<Vector2, Vector2>(p1, p2);
                        EQUAL.Add(curr_pair);
                    }
                    else if (slope < median_slope)
                    {
                        Tuple<Vector2, Vector2> curr_pair = new Tuple<Vector2, Vector2>(p1, p2);
                        SMALL.Add(curr_pair);
                    }
                    else if (slope > median_slope)
                    {
                        Tuple<Vector2, Vector2> curr_pair = new Tuple<Vector2, Vector2>(p1, p2);
                        LARGE.Add(curr_pair);
                    }

                }
            }

            double min_c = int.MaxValue;

            for (int i = 0; i < points.Count; i++)
            {
                double x = points[i].x;
                double y = points[i].y;
                double curr_c = (y - median_slope * x);

                if (curr_c < min_c)
                {
                    min_c = curr_c;
                }

            }

            Vector2 pmin = new Vector2(int.MaxValue, int.MaxValue);
            Vector2 pmax = new Vector2(int.MinValue, int.MinValue);

            for (int i = 0; i < points.Count; i++)
            {
                float x = points[i].x;
                float y = points[i].y;
                double curr_c = (y - median_slope * x);

                if (abss(curr_c - min_c) < 0.001)
                {

                    if (x < pmin.x)
                    {
                        pmin = new Vector2(x, y);
                    }
                    if (x > pmax.x)
                    {
                        pmax = new Vector2(x, y);
                    }
                }
            }


            if (pmin.x <= median && pmax.x > median)
            {
                Tuple<Vector2, Vector2> lower_bridge = new Tuple<Vector2, Vector2>(pmin, pmax);
                return lower_bridge;
            }
            else if (pmax.x <= median)
            {
                for (int i = 0; i < EQUAL.Count; i++)
                {
                    Vector2 pt = EQUAL[i].Item2;
                    candidates.Add(pt);
                }
                for (int i = 0; i < LARGE.Count; i++)
                {
                    Vector2 pt1 = LARGE[i].Item1;
                    Vector2 pt2 = LARGE[i].Item2;
                    candidates.Add(pt1);
                    candidates.Add(pt2);
                }
                for (int i = 0; i < SMALL.Count; i++)
                {
                    Vector2 pt = SMALL[i].Item2;
                    candidates.Add(pt);
                }
                return get_lower_bridge(candidates, median);
            }
            else if (pmin.x > median)
            {
                for (int i = 0; i < EQUAL.Count; i++)
                {
                    Vector2 pt = EQUAL[i].Item1;
                    candidates.Add(pt);
                }
                for (int i = 0; i < LARGE.Count; i++)
                {
                    Vector2 pt = LARGE[i].Item1;
                    candidates.Add(pt);
                }
                for (int i = 0; i < SMALL.Count; i++)
                {
                    Vector2 pt1 = SMALL[i].Item1;
                    Vector2 pt2 = SMALL[i].Item2;
                    candidates.Add(pt1);
                    candidates.Add(pt2);
                }
                return get_lower_bridge(candidates, median);
            }

            return null;
        }

        Tuple<Vector2, Vector2> get_upper_bridge(List<Vector2> points, double median)
        {
            points.Sort(delegate (Vector2 a, Vector2 b)
            {
                return a.x < b.x ? 1 : -1;
            });

            List<Vector2> candidates = new List<Vector2>(points.Count);
            List<Tuple<Vector2, Vector2>> pairs = new List<Tuple<Vector2, Vector2>>(points.Count / 2 + 1);
            if (points.Count % 2 == 0)
            {
                for (int i = 0; i < points.Count; i += 2)
                {
                    Vector2 first_pt = points[i];
                    Vector2 second_pt = points[i + 1];

                    Tuple<Vector2, Vector2> curr_pair = new Tuple<Vector2, Vector2>(first_pt, second_pt);
                    pairs.Add(curr_pair);
                }
            }
            else
            {
                candidates.Add(points[0]);
                for (int i = 1; i < points.Count; i += 2)
                {
                    Vector2 first_pt = points[i];
                    Vector2 second_pt = points[i + 1];

                    Tuple<Vector2, Vector2> curr_pair = new Tuple<Vector2, Vector2>(first_pt, second_pt);
                    pairs.Add(curr_pair);
                }
            }

            int slopes_len = pairs.Count;
            double[] slopes = new double[slopes_len];
            for (int i = 0; i < pairs.Count; i++)
            {
                Vector2 p1 = pairs[i].Item1;
                Vector2 p2 = pairs[i].Item2;
                double x1 = p1.x;
                double x2 = p2.x;
                double y1 = p1.y;
                double y2 = p2.y;

                if (x1 == x2)
                {
                    if (y1 > y2)
                    {
                        candidates.Add(p1);
                    }
                    else
                    {
                        candidates.Add(p2);
                    }
                    slopes[i] = int.MaxValue;
                }
                else
                {
                    double slope = (y2 - y1) / (double)(x2 - x1);
                    slopes[i] = slope;
                }

            }

            double[] arr = new double[slopes_len];
            int len = 0;
            for (int i = 0; i < slopes_len; i++)
            {
                if (slopes[i] != int.MaxValue)
                {
                    arr[len++] = slopes[i];
                }
            }

            double median_slope;
            if (len == 1)
                median_slope = arr[0];
            else
                median_slope = kthSmallest(arr, 0, len - 1, (len + 1) / 2);

            List<Tuple<Vector2, Vector2>> SMALL = new List<Tuple<Vector2, Vector2>>(pairs.Count);
            List<Tuple<Vector2, Vector2>> EQUAL = new List<Tuple<Vector2, Vector2>>(pairs.Count);
            List<Tuple<Vector2, Vector2>> LARGE = new List<Tuple<Vector2, Vector2>>(pairs.Count);

            for (int i = 0; i < pairs.Count; i++)
            {
                Vector2 p1 = pairs[i].Item1;
                Vector2 p2 = pairs[i].Item2;
                double x1 = p1.x;
                double x2 = p2.x;
                double y1 = p1.y;
                double y2 = p2.y;

                if (x1 != x2)
                {
                    double slope = (y2 - y1) / (x2 - x1);
                    if (abss(slope - median_slope) < 0.001)
                    {
                        Tuple<Vector2, Vector2> curr_pair = new Tuple<Vector2, Vector2>(p1, p2);
                        EQUAL.Add(curr_pair);
                    }
                    else if (slope < median_slope)
                    {
                        Tuple<Vector2, Vector2> curr_pair = new Tuple<Vector2, Vector2>(p1, p2);
                        SMALL.Add(curr_pair);
                    }
                    else if (slope > median_slope)
                    {
                        Tuple<Vector2, Vector2> curr_pair = new Tuple<Vector2, Vector2>(p1, p2);
                        LARGE.Add(curr_pair);
                    }
                }
            }

            double max_c = int.MinValue;
            for (int i = 0; i < points.Count; i++)
            {

                double x = points[i].x;
                double y = points[i].y;
                double curr_c = (y - (double)median_slope * x);

                if (curr_c > max_c)
                {
                    max_c = curr_c;
                }
            }

            Vector2 pmin = new Vector2(int.MaxValue, int.MaxValue);
            Vector2 pmax = new Vector2(int.MinValue, int.MinValue);

            for (int i = 0; i < points.Count; i++)
            {

                float x = points[i].x;
                float y = points[i].y;

                double curr_c = y - (double)median_slope * x;

                if (abss((double)curr_c - max_c) < 0.001)
                {

                    if (x < pmin.x)
                    {
                        pmin = new Vector2(x, y);
                    }
                    if (x > pmax.x)
                    {
                        pmax = new Vector2(x, y);
                    }
                }
            }

            if (pmin.x <= median && pmax.x > median)
            {
                Tuple<Vector2, Vector2> upper_bridge = new Tuple<Vector2, Vector2>(pmin, pmax);
                return upper_bridge;
            }
            else if (pmax.x <= median)
            {
                for (int i = 0; i < EQUAL.Count; i++)
                {
                    Vector2 pt = EQUAL[i].Item2;
                    candidates.Add(pt);
                }
                for (int i = 0; i < LARGE.Count; i++)
                {
                    Vector2 pt = LARGE[i].Item2;
                    candidates.Add(pt);
                }
                for (int i = 0; i < SMALL.Count; i++)
                {
                    Vector2 pt1 = SMALL[i].Item1;
                    Vector2 pt2 = SMALL[i].Item2;
                    candidates.Add(pt1);
                    candidates.Add(pt2);
                }
                return get_upper_bridge(candidates, median);

            }
            else if (pmin.x > median)
            {
                for (int i = 0; i < EQUAL.Count; i++)
                {
                    Vector2 pt = EQUAL[i].Item1;
                    candidates.Add(pt);
                }
                for (int i = 0; i < LARGE.Count; i++)
                {
                    Vector2 pt1 = LARGE[i].Item1;
                    Vector2 pt2 = LARGE[i].Item2;
                    candidates.Add(pt1);
                    candidates.Add(pt2);
                }
                for (int i = 0; i < SMALL.Count; i++)
                {
                    Vector2 pt = SMALL[i].Item1;
                    candidates.Add(pt);
                }
                return get_upper_bridge(candidates, median);
            }

            return null;
        }

        List<Vector2> get_T(Vector2 p1, Vector2 p2, List<Vector2> points, bool flag)
        {
            List<Vector2> upper_T = new List<Vector2>();
            double slope = (p1.y - p2.y) / (p1.x - p2.x);
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 curr_point = points[i];

                if (curr_point.x > p1.x && curr_point.x < p2.x)
                {

                    double curr_slope = (p1.y - curr_point.y) / (p1.x - curr_point.x);
                    if (flag == false)
                    {
                        if (curr_slope > slope)
                            upper_T.Add(curr_point);
                    }
                    else
                    {
                        if (curr_slope < slope)
                        {
                            upper_T.Add(curr_point);
                        }
                    }
                }
            }
            upper_T.Add(p1);
            upper_T.Add(p2);

            return upper_T;
        }
        double abss(double a)
        {
            if (a < 0) return 0 - a;
            else return a;
        }
        double kthSmallest(double[] arr, int l, int r, int k)
        {
            if (k > 0 && k <= r - l + 1)
            {
                int n = r - l + 1;

                int i;
                double[] median = new double[(n + 4) / 5];
                for (i = 0; i < n / 5; i++)
                    median[i] = findMedian(arr.Skip(l + i * 5).ToArray(), 5);
                if (i * 5 < n)
                {
                    median[i] = findMedian(arr.Skip(l + i * 5).ToArray(), n % 5);
                    i++;
                }

                double medOfMed = (i == 1) ? median[i - 1] :
                                        kthSmallest(median, 0, i - 1, i / 2);

                int pos = partition(arr, l, r, medOfMed);


                if (pos - l == k - 1)
                    return arr[pos];
                if (pos - l > k - 1)
                    return kthSmallest(arr, l, pos - 1, k);


                return kthSmallest(arr, pos + 1, r, k - pos + l - 1);
            }

            return int.MaxValue;
        }
        double findMedian(double[] arr, int n)
        {
            Array.Sort(arr);
            return arr[n / 2];
        }
        void swap(ref double a, ref double b)
        {
            double temp = a;
            a = b;
            b = temp;
        }
        int partition(double[] arr, int l, int r, double x)
        {
            int i;
            for (i = l; i < r; i++)
                if (arr[i] == x)
                    break;

            swap(ref arr[i], ref arr[r]);

            i = l;
            for (int j = l; j <= r - 1; j++)
            {
                if (arr[j] <= x)
                {
                    swap(ref arr[i], ref arr[j]);
                    i++;
                }
            }
            swap(ref arr[i], ref arr[r]);

            return i;
        }
    }

    //TODO //ConvexHulls3D-Roger-Hernando.pdf cg-hull3d.pdf
    public static List<Vector2> Incremental(List<Vector2> points)
    {
        return null;
    }

    ///new convex hull algo
    ///https://github.com/nguy1708/Convex-Hull-Approximation/blob/master/convex-hull-approximation.pdf
    ///https://github.com/rowanwins/convex-hull-wp/blob/master/KP_Paper.pdf An Efficient Convex Hull Algorithm for a Planer Set of Points - by Wijeweera & Pinidiyaarachchi Interestingly
}
