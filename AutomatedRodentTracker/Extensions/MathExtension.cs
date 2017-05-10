/*Automated Rodent Tracker - A program to automatically track rodents
Copyright(C) 2015 Brett Michael Hewitt

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see<http://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AutomatedRodentTracker.Classes;

namespace AutomatedRodentTracker.Extensions
{
    public static class MathExtension
    {
        public static double Deg2Rad;
        public static double Rad2Deg;

        static MathExtension()
        {
            double pi = Math.PI;
            Deg2Rad = pi/180d;
            Rad2Deg = 180/pi;
        }

        public static int ClosestPowerOfTwo(int x)
        {
            x--;
            x |= (x >> 1);
            x |= (x >> 2);
            x |= (x >> 4);
            x |= (x >> 8);
            x |= (x >> 16);

            return (x + 1);
        }

        public static float[] NormalizeData(IEnumerable<float> data, float min, float max)
        {
            float dataMax = data.Max();
            float dataMin = data.Min();
            float range = dataMax - dataMin;

            return data.Select(d => (d - dataMin) / range).Select(n => ((1 - n) * min + n * max)).ToArray();
        }

        public static float[] NormalizeData(IEnumerable<double> data, float min, float max)
        {
            double dataMax = data.Max();
            double dataMin = data.Min();
            double range = dataMax - dataMin;

            return data.Select(d => (d - dataMin) / range).Select(n => (float)((1 - n) * min + n * max)).ToArray();
        }

        //public static float GetAreaFromPoints(Point[] points)
        //{
        //    return Math.Abs(points.Take(points.Length - 1).Select((p, i2) => (points[i2 + 1].X - p.X) * (points[i2 + 1].Y + p.Y)).Sum() / 2);
        //}

        public static float PolygonArea(Point[] polygon)
        {
            int i, j;
            float area = 0;

            for (i = 0; i < polygon.Length; i++)
            {
                j = (i + 1) % polygon.Length;

                area += polygon[i].X * polygon[j].Y;
                area -= polygon[i].Y * polygon[j].X;
            }

            area /= 2;
            return (area < 0 ? -area : area);
        }

        public static float PolygonArea(PointF[] polygon)
        {
            int i, j;
            float area = 0;

            for (i = 0; i < polygon.Length; i++)
            {
                j = (i + 1) % polygon.Length;

                area += polygon[i].X * polygon[j].Y;
                area -= polygon[i].Y * polygon[j].X;
            }

            area /= 2;
            return (area < 0 ? -area : area);
        }

        public static PointSideVector FindSide(Point p1, Point p2, Point pointToCheck)
        {
            float result = (p2.X - p1.X) * (pointToCheck.Y - p1.Y) - (p2.Y - p1.Y) * (pointToCheck.X - p1.X);

            if (result < 0)
            {
                return PointSideVector.Below;
            }

            if (result > 0)
            {
                return PointSideVector.Above;
            }

            return PointSideVector.On;
        }

        public static PointSideVector FindSide(PointF p1, PointF p2, PointF pointToCheck)
        {
            float result = (p2.X - p1.X) * (pointToCheck.Y - p1.Y) - (p2.Y - p1.Y) * (pointToCheck.X - p1.X);

            if (result < 0)
            {
                return PointSideVector.Below;
            }

            if (result > 0)
            {
                return PointSideVector.Above;
            }

            return PointSideVector.On;
        }

        public static bool PolygonContainsPoint(Point[] polygon, Point point)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < point.Y && polygon[j].Y >= point.Y || polygon[j].Y < point.Y && polygon[i].Y >= point.Y)
                {
                    if (polygon[i].X + (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < point.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        public static bool PolygonContainsPoint(Point[] polygon, PointF point)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < point.Y && polygon[j].Y >= point.Y || polygon[j].Y < point.Y && polygon[i].Y >= point.Y)
                {
                    if (polygon[i].X + (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < point.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        /// <summary>
        /// Rotates one point around another
        /// </summary>
        /// <param name="pointToRotate">The point to rotate.</param>
        /// <param name="centerPoint">The centre point of rotation.</param>
        /// <param name="angleInDegrees">The rotation angle in degrees.</param>
        /// <returns>Rotated point</returns>
        public static Point RotatePoint(Point pointToRotate, Point centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point
            {
                X =
                    (int)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (int)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }

        /// <summary>
        /// Finds the the first intersection point of a line and polygon 
        /// </summary>
        /// <param name="ps1">The first point of the line</param>
        /// <param name="pe1">The second point of the line</param>
        /// <param name="polygon">THe polygon</param>
        /// <returns>The intersection point, PointF.Empty if no intersection found</returns>
        public static PointF PolygonLineIntersectionPoint(PointF ps1, PointF pe1, PointF[] polygon)
        {
            for (int i = 1; i < polygon.Length; i++)
            {
                PointF ps2 = polygon[i - 1];
                PointF pe2 = polygon[i];

                PointF intersectionPoint = LineIntersectionPoint(ps1, pe1, ps2, pe2);

                if (intersectionPoint != Point.Empty)
                {
                    return intersectionPoint;
                }
            }

            return PointF.Empty;
        }

        /// <summary>
        /// Finds the the first intersection point of a line and polygon 
        /// </summary>
        /// <param name="ps1">The first point of the line</param>
        /// <param name="pe1">The second point of the line</param>
        /// <param name="polygon">THe polygon</param>
        /// <returns>The intersection point, PointF.Empty if no intersection found</returns>
        public static PointF PolygonLineIntersectionPoint(PointF ps1, PointF pe1, Point[] polygon)
        {
            for (int i = 1; i < polygon.Length; i++)
            {
                PointF ps2 = polygon[i - 1];
                PointF pe2 = polygon[i];

                PointF intersectionPoint = LineIntersectionPoint(ps1, pe1, ps2, pe2);

                if (!intersectionPoint.IsEmpty)
                {
                    return intersectionPoint;
                }
            }

            return PointF.Empty;
        }

        public static PointF LineIntersectionPoint(PointF ps1, PointF pe1, PointF ps2, PointF pe2)
        {
            // Get A,B,C of first line - points : ps1 to pe1
            float A1 = pe1.Y - ps1.Y;
            float B1 = ps1.X - pe1.X;
            float C1 = A1 * ps1.X + B1 * ps1.Y;

            // Get A,B,C of second line - points : ps2 to pe2
            float A2 = pe2.Y - ps2.Y;
            float B2 = ps2.X - pe2.X;
            float C2 = A2 * ps2.X + B2 * ps2.Y;

            // Get delta and check if the lines are parallel
            float delta = A1 * B2 - A2 * B1;

            if (delta == 0)
            {
                return PointF.Empty;
            }

            //Check distances
            Line line1 = new Line(ps1, pe1);
            Line line2 = new Line(ps2, pe2);

            if (IntersectionOf(line1, line2) == Intersection.None)
            {
                return PointF.Empty;
            }

            // now return the Vector2 intersection point
            return new PointF((B2 * C1 - B1 * C2) / delta, (A1 * C2 - A2 * C1) / delta);
        }

        public static Intersection IntersectionOf(Line line1, Line line2)
        {
            //  Fail if either line segment is zero-length.
            if (line1.X1 == line1.X2 && line1.Y1 == line1.Y2 || line2.X1 == line2.X2 && line2.Y1 == line2.Y2)
                return Intersection.None;

            if (line1.X1 == line2.X1 && line1.Y1 == line2.Y1 || line1.X2 == line2.X1 && line1.Y2 == line2.Y1)
                return Intersection.Intersection;
            if (line1.X1 == line2.X2 && line1.Y1 == line2.Y2 || line1.X2 == line2.X2 && line1.Y2 == line2.Y2)
                return Intersection.Intersection;

            //  (1) Translate the system so that point A is on the origin.
            line1.X2 -= line1.X1; line1.Y2 -= line1.Y1;
            line2.X1 -= line1.X1; line2.Y1 -= line1.Y1;
            line2.X2 -= line1.X1; line2.Y2 -= line1.Y1;

            //  Discover the length of segment A-B.
            float distAB = (float)Math.Sqrt(line1.X2 * line1.X2 + line1.Y2 * line1.Y2);

            //  (2) Rotate the system so that point B is on the positive X axis.
            float theCos = line1.X2 / distAB;
            float theSin = line1.Y2 / distAB;
            float newX = line2.X1 * theCos + line2.Y1 * theSin;
            line2.Y1 = line2.Y1 * theCos - line2.X1 * theSin;
            line2.X1 = newX;
            newX = line2.X2 * theCos + line2.Y2 * theSin;
            line2.Y2 = line2.Y2 * theCos - line2.X2 * theSin;
            line2.X2 = newX;

            //  Fail if segment C-D doesn't cross line A-B.
            if (line2.Y1 < 0 && line2.Y2 < 0 || line2.Y1 >= 0 && line2.Y2 >= 0)
                return Intersection.None;

            //  (3) Discover the position of the intersection point along line A-B.
            double posAB = line2.X2 + (line2.X1 - line2.X2) * line2.Y2 / (line2.Y2 - line2.Y1);

            //  Fail if segment C-D crosses line A-B outside of segment A-B.
            if (posAB < 0 || posAB > distAB)
                return Intersection.None;

            //  (4) Apply the discovered position to line A-B in the original coordinate system.
            return Intersection.Intersection;
        }

        /// <summary>
        /// Find the minimum distance between a line and a point
        /// </summary> 
        /// <param name="p1">Line point 1</param>
        /// <param name="p2">Line point 2</param>
        /// <param name="pt">Point to test</param>
        /// <returns>The minimum distance between a point and a line segment</returns>
        public static double MinDistanceFromLineToPoint(PointF p1, PointF p2, PointF pt)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            
            if ((dx == 0) && (dy == 0))
            {
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) / (dx * dx + dy * dy);

            if (t < 0)
            {
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                PointF closest = new PointF(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double MinDistanceFromLineToPoint(PointF p1, PointF p2, PointF pt, out PointF closestPoint)
        {
            closestPoint = pt;
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;

            if ((dx == 0) && (dy == 0))
            {
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) / (dx * dx + dy * dy);

            if (t < 0)
            {
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closestPoint = new PointF(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closestPoint.X;
                dy = pt.Y - closestPoint.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double FindDistanceBetweenSegments(PointF p1, PointF p2, PointF p3, PointF p4)
        {
            // See if the segments intersect.
            bool lines_intersect, segments_intersect;
            PointF intersection;

            FindIntersection(p1, p2, p3, p4, out lines_intersect, out segments_intersect, out intersection);
            if (segments_intersect)
            {
                // They intersect.
                //close1 = intersection;
                //close2 = intersection;
                return 0;
            }

            // Find the other possible distances.
            PointF closest;
            double best_dist = double.MaxValue, test_dist;

            // Try p1.
            test_dist = FindDistanceToSegment(p1, p3, p4, out closest);
            if (test_dist < best_dist)
            {
                best_dist = test_dist;
                //close1 = p1;
                //close2 = closest;
            }

            // Try p2.
            test_dist = FindDistanceToSegment(p2, p3, p4, out closest);
            if (test_dist < best_dist)
            {
                best_dist = test_dist;
                //close1 = p2;
                //close2 = closest;
            }

            // Try p3.
            test_dist = FindDistanceToSegment(p3, p1, p2, out closest);
            if (test_dist < best_dist)
            {
                best_dist = test_dist;
                //close1 = closest;
                //close2 = p3;
            }

            // Try p4.
            test_dist = FindDistanceToSegment(p4, p1, p2, out closest);
            if (test_dist < best_dist)
            {
                best_dist = test_dist;
                //close1 = closest;
                //close2 = p4;
            }

            return best_dist;
        }

        public static double FindDistanceToSegment(PointF pt, PointF p1, PointF p2, out PointF closest)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) / (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new PointF(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new PointF(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new PointF(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double FindDistanceToSegment(PointF pt, PointF p1, PointF p2)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) / (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                PointF closest = new PointF(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static void FindIntersection(PointF p1, PointF p2, PointF p3, PointF p4, out bool lines_intersect, out bool segments_intersect, out PointF intersection)
        {
            // Get the segments' parameters.
            float dx12 = p2.X - p1.X;
            float dy12 = p2.Y - p1.Y;
            float dx34 = p4.X - p3.X;
            float dy34 = p4.Y - p3.Y;
            PointF close_p1, close_p2;
            // Solve for t1 and t2
            float denominator = (dy12 * dx34 - dx12 * dy34);

            float t1 = ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34) / denominator;

            if (float.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = new PointF(float.NaN, float.NaN);
                close_p1 = new PointF(float.NaN, float.NaN);
                close_p2 = new PointF(float.NaN, float.NaN);
                return;
            }

            lines_intersect = true;

            float t2 = ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12) / -denominator;

            // Find the point of intersection.
            intersection = new PointF(p1.X + dx12 * t1, p1.Y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect = ((t1 >= 0) && (t1 <= 1) && (t2 >= 0) && (t2 <= 1));

            // Find the closest points on the segments.
            if (t1 < 0)
            {
                t1 = 0;
            }
            else if (t1 > 1)
            {
                t1 = 1;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 1)
            {
                t2 = 1;
            }

            close_p1 = new PointF(p1.X + dx12 * t1, p1.Y + dy12 * t1);
            close_p2 = new PointF(p3.X + dx34 * t2, p3.Y + dy34 * t2);
        }

        public static PointF[] GetSegmentOfPolygon(PointF[] polygon, PointF p1, PointF p2)
        {
            List<PointF> points = new List<PointF>();
            
            double startMin = Double.MaxValue, endMin = Double.MaxValue;

            int startIndex = 0, endIndex = 0;

            int counter = 0;
            foreach (PointF point in polygon)
            {
                double p1Dist = p1.DistanceSquared(point);
                if (p1Dist < startMin)
                {
                    startMin = p1Dist;
                    startIndex = counter;
                }

                double p2Dist = p2.DistanceSquared(point);
                if (p2Dist < endMin)
                {
                    endMin = p2Dist;
                    endIndex = counter;
                }

                counter ++;
            }

            int lowerBound, upperBound;
            if (startIndex < endIndex)
            {
                lowerBound = startIndex;
                upperBound = endIndex;
            }
            else
            {
                lowerBound = endIndex;
                upperBound = startIndex;
            }

            for (int i = lowerBound; i < upperBound; i++)
            {
                points.Add(polygon[i]);
            }

            return points.ToArray();
        }

        public static Point[] GetSegmentOfPolygon(Point[] polygon, Point p1, Point p2)
        {
            List<Point> points = new List<Point>();
            
            double startMin = Double.MaxValue, endMin = Double.MaxValue;

            int startIndex = 0, endIndex = 0;

            int counter = 0;
            foreach (Point point in polygon)
            {
                double p1Dist = p1.DistanceSquared(point);
                if (p1Dist < startMin)
                {
                    startMin = p1Dist;
                    startIndex = counter;
                }

                double p2Dist = p2.DistanceSquared(point);
                if (p2Dist < endMin)
                {
                    endMin = p2Dist;
                    endIndex = counter;
                }

                counter++;
            }

            bool searchLeft = true, searchRight = true;
            //bool goLeft = false;
            counter = startIndex;
            Point previousPoint = polygon[startIndex];
            double distCounter1 = 0;
            while (searchLeft)
            {
                counter--;

                if (counter < 0)
                {
                    counter = polygon.Length - 1;
                }

                Point currentPoint = polygon[counter];

                distCounter1 += currentPoint.DistanceSquared(previousPoint);

                if (currentPoint == polygon[endIndex])
                {
                    searchLeft = false;
                }
            }

            counter = startIndex;
            previousPoint = polygon[startIndex];
            double distCounter2 = 0;
            while (searchRight)
            {
                counter++;

                if (counter >= polygon.Length)
                {
                    counter = 0;
                }

                Point currentPoint = polygon[counter];

                distCounter2 += currentPoint.DistanceSquared(previousPoint);

                if (currentPoint == polygon[endIndex])
                {
                    searchRight = false;
                }
            }

            if (distCounter1 < distCounter2)
            {
                //Go left
                int index = startIndex;
                while (true)
                {
                    if (index == endIndex)
                    {
                        break;
                    }

                    points.Add(polygon[index]);
                    index--;

                    if (index < 0)
                    {
                        index = polygon.Length;
                    }
                }
            }
            else
            {
                //Go right
                int index = startIndex;
                while (true)
                {
                    if (index == endIndex)
                    {
                        break;
                    }

                    points.Add(polygon[index]);
                    index++;

                    if (index >= polygon.Length)
                    {
                        index = 0;
                    }
                }
            }

            return points.ToArray();
        }

        public static Point[] GetSegmentOfPolygon(Point[] polygon, PointF p1, PointF p2)
        {
            List<Point> points = new List<Point>();

            double startMin = Double.MaxValue, endMin = Double.MaxValue;

            int startIndex = 0, endIndex = 0;

            int counter = 0;
            foreach (Point point in polygon)
            {
                double p1Dist = p1.DistanceSquared(point);
                if (p1Dist < startMin)
                {
                    startMin = p1Dist;
                    startIndex = counter;
                }

                double p2Dist = p2.DistanceSquared(point);
                if (p2Dist < endMin)
                {
                    endMin = p2Dist;
                    endIndex = counter;
                }

                counter++;
            }

            bool searchLeft = true, searchRight = true;
            //bool goLeft = false;
            counter = startIndex;
            Point previousPoint = polygon[startIndex];
            double distCounter1 = 0;
            while (searchLeft)
            {
                counter--;

                if (counter < 0)
                {
                    counter = polygon.Length - 1;
                }

                Point currentPoint = polygon[counter];

                distCounter1 += currentPoint.DistanceSquared(previousPoint);

                if (currentPoint == polygon[endIndex])
                {
                    searchLeft = false;
                }
            }

            counter = startIndex;
            previousPoint = polygon[startIndex];
            double distCounter2 = 0;
            while (searchRight)
            {
                counter++;

                if (counter >= polygon.Length)
                {
                    counter = 0;
                }

                Point currentPoint = polygon[counter];

                distCounter2 += currentPoint.DistanceSquared(previousPoint);

                if (currentPoint == polygon[endIndex])
                {
                    searchRight = false;
                }
            }

            if (distCounter1 < distCounter2)
            {
                //Go left
                int index = startIndex;
                while (true)
                {
                    if (index == endIndex)
                    {
                        break;
                    }

                    points.Add(polygon[index]);
                    index--;

                    if (index < 0)
                    {
                        index = polygon.Length - 1;
                    }
                }
            }
            else
            {
                //Go right
                int index = startIndex;
                while (true)
                {
                    if (index == endIndex)
                    {
                        break;
                    }

                    points.Add(polygon[index]);
                    index++;

                    if (index >= polygon.Length)
                    {
                        index = 0;
                    }
                }
            }

            return points.ToArray();
        }
        
        public static PointF FindCentroid(PointF[] polygon)
        {
            // Add the first point at the end of the array.
            int num_points = polygon.Length;
            PointF[] pts = new PointF[num_points + 1];
            polygon.CopyTo(pts, 0);
            pts[num_points] = polygon[0];

            // Find the centroid.
            float X = 0;
            float Y = 0;
            float second_factor;
            for (int i = 0; i < num_points; i++)
            {
                second_factor =
                    pts[i].X * pts[i + 1].Y -
                    pts[i + 1].X * pts[i].Y;
                X += (pts[i].X + pts[i + 1].X) * second_factor;
                Y += (pts[i].Y + pts[i + 1].Y) * second_factor;
            }

            // Divide by 6 times the polygon's area.
            float polygon_area = PolygonArea(polygon);
            X /= (6 * polygon_area);
            Y /= (6 * polygon_area);

            // If the values are negative, the polygon is
            // oriented counterclockwise so reverse the signs.
            if (X < 0)
            {
                X = -X;
                Y = -Y;
            }

            return new PointF(X, Y);
        }

        public static PointF FindCentroid(Point[] polygon)
        {
            // Add the first point at the end of the array.
            int num_points = polygon.Length;
            Point[] pts = new Point[num_points + 1];
            polygon.CopyTo(pts, 0);
            pts[num_points] = polygon[0];

            // Find the centroid.
            float X = 0;
            float Y = 0;
            float second_factor;
            for (int i = 0; i < num_points; i++)
            {
                second_factor =
                    pts[i].X * pts[i + 1].Y -
                    pts[i + 1].X * pts[i].Y;
                X += (pts[i].X + pts[i + 1].X) * second_factor;
                Y += (pts[i].Y + pts[i + 1].Y) * second_factor;
            }

            // Divide by 6 times the polygon's area.
            float polygon_area = PolygonArea(polygon);
            X /= (6 * polygon_area);
            Y /= (6 * polygon_area);

            // If the values are negative, the polygon is
            // oriented counterclockwise so reverse the signs.
            if (X < 0)
            {
                X = -X;
                Y = -Y;
            }

            return new PointF(X, Y);
        }
    }

    public struct Line
    {
        public static Line Empty;

        private PointF p1;
        private PointF p2;

        public Line(PointF p1, PointF p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public PointF P1
        {
            get { return p1; }
            set { p1 = value; }
        }

        public PointF P2
        {
            get { return p2; }
            set { p2 = value; }
        }

        public float X1
        {
            get { return p1.X; }
            set { p1.X = value; }
        }

        public float X2
        {
            get { return p2.X; }
            set { p2.X = value; }
        }

        public float Y1
        {
            get { return p1.Y; }
            set { p1.Y = value; }
        }

        public float Y2
        {
            get { return p2.Y; }
            set { p2.Y = value; }
        }
    }

    //public struct Polygon : IEnumerable<PointF>
    //{
    //    private PointF[] points;

    //    public Polygon(PointF[] points)
    //    {
    //        this.points = points;
    //    }

    //    public Polygon(IEnumerable<PointF> points)
    //    {
    //        this.points = points.ToArray();
    //    }

    //    public PointF[] Points
    //    {
    //        get { return points; }
    //        set { points = value; }
    //    }

    //    public int Length
    //    {
    //        get { return points.Length; }
    //    }

    //    public PointF this[int index]
    //    {
    //        get { return points[index]; }
    //        set { points[index] = value; }
    //    }

    //    public static implicit operator PointF[](Polygon polygon)
    //    {
    //        return polygon.points;
    //    }

    //    public static implicit operator Polygon(PointF[] points)
    //    {
    //        return new Polygon(points);
    //    }

    //    IEnumerator<PointF> IEnumerable<PointF>.GetEnumerator()
    //    {
    //        return (IEnumerator<PointF>)points.GetEnumerator();
    //    }

    //    public IEnumerator GetEnumerator()
    //    {
    //        return points.GetEnumerator();
    //    }
    //}

    public enum Intersection
    {
        None,
        Tangent,
        Intersection,
        Containment
    }

    //public static class Geometry
    //{

    //    public static Intersection IntersectionOf(Line line, Polygon polygon)
    //    {
    //        if (polygon.Length == 0)
    //        {
    //            return Intersection.None;
    //        }
    //        if (polygon.Length == 1)
    //        {
    //            return IntersectionOf(polygon[0], line);
    //        }
    //        bool tangent = false;
    //        for (int index = 0; index < polygon.Length; index++)
    //        {
    //            int index2 = (index + 1) % polygon.Length;
    //            Intersection intersection = IntersectionOf(line, new Line(polygon[index], polygon[index2]));
    //            if (intersection == Intersection.Intersection)
    //            {
    //                return intersection;
    //            }
    //            if (intersection == Intersection.Tangent)
    //            {
    //                tangent = true;
    //            }
    //        }
    //        return tangent ? Intersection.Tangent : IntersectionOf(line.P1, polygon);
    //    }

    //    public static Intersection IntersectionOf(PointF point, Polygon polygon)
    //    {
    //        switch (polygon.Length)
    //        {
    //            case 0:
    //                return Intersection.None;
    //            case 1:
    //                if (polygon[0].X == point.X && polygon[0].Y == point.Y)
    //                {
    //                    return Intersection.Tangent;
    //                }
    //                else
    //                {
    //                    return Intersection.None;
    //                }
    //            case 2:
    //                return IntersectionOf(point, new Line(polygon[0], polygon[1]));
    //        }

    //        int counter = 0;
    //        int i;
    //        PointF p1;
    //        int n = polygon.Length;
    //        p1 = polygon[0];
    //        if (point == p1)
    //        {
    //            return Intersection.Tangent;
    //        }

    //        for (i = 1; i <= n; i++)
    //        {
    //            PointF p2 = polygon[i % n];
    //            if (point == p2)
    //            {
    //                return Intersection.Tangent;
    //            }
    //            if (point.Y > Math.Min(p1.Y, p2.Y))
    //            {
    //                if (point.Y <= Math.Max(p1.Y, p2.Y))
    //                {
    //                    if (point.X <= Math.Max(p1.X, p2.X))
    //                    {
    //                        if (p1.Y != p2.Y)
    //                        {
    //                            double xinters = (point.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X;
    //                            if (p1.X == p2.X || point.X <= xinters)
    //                                counter++;
    //                        }
    //                    }
    //                }
    //            }
    //            p1 = p2;
    //        }

    //        return (counter % 2 == 1) ? Intersection.Containment : Intersection.None;
    //    }

    //    public static Intersection IntersectionOf(PointF point, Line line)
    //    {
    //        float bottomY = Math.Min(line.Y1, line.Y2);
    //        float topY = Math.Max(line.Y1, line.Y2);
    //        bool heightIsRight = point.Y >= bottomY &&
    //                             point.Y <= topY;
    //        //Vertical line, slope is divideByZero error!
    //        if (line.X1 == line.X2)
    //        {
    //            if (point.X == line.X1 && heightIsRight)
    //            {
    //                return Intersection.Tangent;
    //            }
    //            else
    //            {
    //                return Intersection.None;
    //            }
    //        }
    //        float slope = (line.X2 - line.X1) / (line.Y2 - line.Y1);
    //        bool onLine = (line.Y1 - point.Y) == (slope * (line.X1 - point.X));
    //        if (onLine && heightIsRight)
    //        {
    //            return Intersection.Tangent;
    //        }
    //        else
    //        {
    //            return Intersection.None;
    //        }
    //    }

    //    public static Intersection IntersectionOf(Line line1, Line line2)
    //    {
    //        //  Fail if either line segment is zero-length.
    //        if (line1.X1 == line1.X2 && line1.Y1 == line1.Y2 || line2.X1 == line2.X2 && line2.Y1 == line2.Y2)
    //            return Intersection.None;

    //        if (line1.X1 == line2.X1 && line1.Y1 == line2.Y1 || line1.X2 == line2.X1 && line1.Y2 == line2.Y1)
    //            return Intersection.Intersection;
    //        if (line1.X1 == line2.X2 && line1.Y1 == line2.Y2 || line1.X2 == line2.X2 && line1.Y2 == line2.Y2)
    //            return Intersection.Intersection;

    //        //  (1) Translate the system so that point A is on the origin.
    //        line1.X2 -= line1.X1; line1.Y2 -= line1.Y1;
    //        line2.X1 -= line1.X1; line2.Y1 -= line1.Y1;
    //        line2.X2 -= line1.X1; line2.Y2 -= line1.Y1;

    //        //  Discover the length of segment A-B.
    //        float distAB = (float)Math.Sqrt(line1.X2 * line1.X2 + line1.Y2 * line1.Y2);

    //        //  (2) Rotate the system so that point B is on the positive X axis.
    //        float theCos = line1.X2 / distAB;
    //        float theSin = line1.Y2 / distAB;
    //        float newX = line2.X1 * theCos + line2.Y1 * theSin;
    //        line2.Y1 = line2.Y1 * theCos - line2.X1 * theSin; 
    //        line2.X1 = newX;
    //        newX = line2.X2 * theCos + line2.Y2 * theSin;
    //        line2.Y2 = line2.Y2 * theCos - line2.X2 * theSin;
    //        line2.X2 = newX;

    //        //  Fail if segment C-D doesn't cross line A-B.
    //        if (line2.Y1 < 0 && line2.Y2 < 0 || line2.Y1 >= 0 && line2.Y2 >= 0)
    //            return Intersection.None;

    //        //  (3) Discover the position of the intersection point along line A-B.
    //        double posAB = line2.X2 + (line2.X1 - line2.X2) * line2.Y2 / (line2.Y2 - line2.Y1);

    //        //  Fail if segment C-D crosses line A-B outside of segment A-B.
    //        if (posAB < 0 || posAB > distAB)
    //            return Intersection.None;

    //        //  (4) Apply the discovered position to line A-B in the original coordinate system.
    //        return Intersection.Intersection;
    //    }
    //}
}
