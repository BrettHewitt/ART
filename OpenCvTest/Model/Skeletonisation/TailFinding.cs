using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using AutomatedRodentTracker.Extensions;
using AutomatedRodentTracker.Model;
using AutomatedRodentTracker.ModelInterface.Skeletonisation;
using Emgu.CV;
using Emgu.CV.Structure;
using Point = System.Drawing.Point;

namespace AutomatedRodentTracker.Model.Skeletonisation
{
    internal class TailFinding : ModelObjectBase, ITailFinding
    {
        public void FindTail(Point[] mouseContour, PointF[] spine, Image<Bgr, Byte> drawImage, double width, PointF centroid, out List<Point> bodyPoints, out double waistLength, out double pelvicArea, out double pelvicArea2)
        {
            //Generate graph for both sides of spine
            List<Tuple<PointF, double>> r1Distances = new List<Tuple<PointF, double>>();
            List<Tuple<PointF, double>> r2Distances = new List<Tuple<PointF, double>>();

            double halfWidth = width / 2;
            double quarterWidth = halfWidth / 2;
            //Console.WriteLine("Quarter Width: " + quarterWidth);
            //bool bodyStarted1 = false, bodyStarted2 = false;
            //bool bodyEnded1 = false, bodyEnded2 = false;
            PointF r1BodyStart = PointF.Empty, r1BodyEnd = PointF.Empty, r2BodyStart = PointF.Empty, r2BodyEnd = PointF.Empty;

            for (int i = 1; i < spine.Length; i++)
            {
                LineSegment2DF line = new LineSegment2DF(spine[i - 1], spine[i]);

                //Create direction vector
                Vector lineDirection = new Vector(line.P2.X - line.P1.X, line.P2.Y - line.P1.Y);
                lineDirection.Normalize();

                //Create vector perpendicular to spine
                Vector r1 = new Vector(lineDirection.Y, -lineDirection.X);
                Vector r2 = new Vector(-lineDirection.Y, lineDirection.X);

                //Get starting point
                Vector startingPoint = new Vector(line.P1.X, line.P1.Y);

                //We have our vectors for this part of the spine, iterate over spine length by 1
                double length = line.Length;

                for (int spinePos = 0; spinePos < length; spinePos++)
                {
                    Vector p = startingPoint + (spinePos * lineDirection);

                    Vector test = p + (r1 * 300);
                    Vector test2 = p + (r2 * 300);

                    PointF start = new PointF((float)p.X, (float)p.Y);
                    PointF p1 = new PointF((float)test.X, (float)test.Y);
                    PointF p2 = new PointF((float)test2.X, (float)test2.Y);

                    PointF intersect1 = MathExtension.PolygonLineIntersectionPoint(start, p1, mouseContour);
                    PointF intersect2 = MathExtension.PolygonLineIntersectionPoint(start, p2, mouseContour);

                    if (!intersect1.IsEmpty)
                    {
                        double distance = intersect1.Distance(start);
                        r1Distances.Add(new Tuple<PointF, double>(intersect1, distance));
                    }

                    if (!intersect2.IsEmpty)
                    {
                        double distance = intersect2.Distance(start);
                        r2Distances.Add(new Tuple<PointF, double>(intersect2, distance));
                    }
                }
            }

            //Iterate forwards to find body start, backwards to find body end
            int r1Count = r1Distances.Count;
            for (int i = 0; i < r1Count; i++)
            {
                Tuple<PointF, double> currentItem = r1Distances.ElementAt(i);
                if (currentItem.Item2 > quarterWidth)
                {
                    r1BodyStart = currentItem.Item1;
                    break;
                }
            }

            for (int i = r1Count - 1; i >= 0; i--)
            {
                Tuple<PointF, double> currentItem = r1Distances.ElementAt(i);
                if (currentItem.Item2 > quarterWidth)
                {
                    r1BodyEnd = currentItem.Item1;
                    break;
                }
            }

            int r2Count = r2Distances.Count;
            for (int i = 0; i < r2Count; i++)
            {
                Tuple<PointF, double> currentItem = r2Distances.ElementAt(i);
                if (currentItem.Item2 > quarterWidth)
                {
                    r2BodyStart = currentItem.Item1;
                    break;
                }
            }

            for (int i = r2Count - 1; i >= 0; i--)
            {
                Tuple<PointF, double> currentItem = r2Distances.ElementAt(i);
                if (currentItem.Item2 > quarterWidth)
                {
                    r2BodyEnd = currentItem.Item1;
                    break;
                }
            }

            if (r1BodyStart.IsEmpty || r1BodyEnd.IsEmpty || r2BodyStart.IsEmpty || r2BodyEnd.IsEmpty)
            {
                bodyPoints = null;
                waistLength = -1;
                pelvicArea = -1;
                pelvicArea2 = -1;
                return;
            }

            Point[] alternateSegment1 = MathExtension.GetSegmentOfPolygon(mouseContour, r1BodyStart, r2BodyStart);
            Point[] alternateSegment2 = MathExtension.GetSegmentOfPolygon(mouseContour, r1BodyEnd, r2BodyEnd);

            if (alternateSegment1 == null || alternateSegment1.Length == 0 || alternateSegment2 == null || alternateSegment2.Length == 0)
            {
                bodyPoints = null;
                waistLength = -1;
                pelvicArea = -1;
                pelvicArea2 = -1;
                return;
            }

            for (int i = 1; i < alternateSegment1.Length; i++)
            {
                LineSegment2D line = new LineSegment2D(alternateSegment1[i - 1], alternateSegment1[i]);
                drawImage.Draw(line, new Bgr(Color.Green), 2);
            }

            for (int i = 1; i < alternateSegment2.Length; i++)
            {
                LineSegment2D line = new LineSegment2D(alternateSegment2[i - 1], alternateSegment2[i]);
                drawImage.Draw(line, new Bgr(Color.Red), 2);
            }

            Point[] bodySegment1 = MathExtension.GetSegmentOfPolygon(mouseContour, r1BodyStart, r1BodyEnd);
            Point[] bodySegment2 = MathExtension.GetSegmentOfPolygon(mouseContour, r2BodyStart, r2BodyEnd);

            if (bodySegment1 == null || bodySegment2 == null || bodySegment1.Length == 0 || bodySegment2.Length == 0)
            {
                bodyPoints = null;
                waistLength = -1;
                pelvicArea = -1;
                pelvicArea2 = -1;
                return;
            }

            double d1 = alternateSegment2[0].DistanceSquared(bodySegment2[0]);
            double d2 = alternateSegment2[0].DistanceSquared(bodySegment2.Last());
            double d3 = alternateSegment2[0].DistanceSquared(bodySegment1[0]);
            double d4 = alternateSegment2[0].DistanceSquared(bodySegment1.Last());

            Point[] noTailPoints = null;
            if (d1 < d2 && d1 < d3 && d1 < d4)
            {
                Point[] p1 = bodySegment2.Reverse().ToArray();
                Point[] p2 = alternateSegment2;
                Point[] p3 = null;
                double f1 = alternateSegment2.Last().DistanceSquared(bodySegment1[0]);
                double f2 = alternateSegment2.Last().DistanceSquared(bodySegment1.Last());

                if (f1 < f2)
                {
                    p3 = bodySegment1;
                }
                else
                {
                    p3 = bodySegment1.Reverse().ToArray();
                }

                noTailPoints = p1.Concat(p2).Concat(p3).ToArray();
            }
            else if (d2 < d3 && d2 < d4)
            {
                Point[] p1 = bodySegment2;
                Point[] p2 = alternateSegment2;
                Point[] p3 = null;
                double f1 = alternateSegment2.Last().DistanceSquared(bodySegment1[0]);
                double f2 = alternateSegment2.Last().DistanceSquared(bodySegment1.Last());

                if (f1 < f2)
                {
                    p3 = bodySegment1;
                }
                else
                {
                    p3 = bodySegment1.Reverse().ToArray();
                }

                noTailPoints = p1.Concat(p2).Concat(p3).ToArray();
            }
            else if (d3 < d4)
            {
                Point[] p1 = bodySegment1;
                Point[] p2 = alternateSegment2;
                Point[] p3 = null;
                double f1 = alternateSegment2.Last().DistanceSquared(bodySegment2[0]);
                double f2 = alternateSegment2.Last().DistanceSquared(bodySegment2.Last());

                if (f1 < f2)
                {
                    p3 = bodySegment2;
                }
                else
                {
                    p3 = bodySegment2.Reverse().ToArray();
                }

                noTailPoints = p1.Concat(p2).Concat(p3).ToArray();
            }
            else
            {
                Point[] p1 = bodySegment1;
                Point[] p2 = alternateSegment2;
                Point[] p3 = null;
                double f1 = alternateSegment2.Last().DistanceSquared(bodySegment2[0]);
                double f2 = alternateSegment2.Last().DistanceSquared(bodySegment2.Last());

                if (f1 < f2)
                {
                    p3 = bodySegment2;
                }
                else
                {
                    p3 = bodySegment2.Reverse().ToArray();
                }

                noTailPoints = p1.Concat(p2).Concat(p3).ToArray();
            }


            RotatedRect rotatedRect = CvInvoke.MinAreaRect(noTailPoints.Select(x => new PointF(x.X, x.Y)).ToArray());


            //bodyPoints = new List<Point>(noTailPoints);

            //PointF lastTail = alternateSegment1[0];
            //PointF[] allBody = bodySegment1.Concat(bodySegment2.Reverse()).Select(x => new PointF(x.X, x.Y)).ToArray();
            //Point[] allBody = mouseContour;
            double spineDistance1 = SpineExtension.FindDistanceAlongSpine(spine, centroid);
            double spineDistance2 = SpineExtension.FindDistanceAlongSpine(spine, alternateSegment1[0]);
            double halfWayDistance = (spineDistance1 + spineDistance2) / 2d;
            //Console.WriteLine("Spine Distance from point: " + spineDistance1);
            //spineDistance1 += 50;

            PointF intersection;
            Vector targetDirection2;

            PointF intersection2;
            Vector targetDirection3;

            PointF intersection3;
            Vector targetDirection4;

            bool result = SpineExtension.GetPerpendicularLineDistanceFromBase(spine, spineDistance1, out intersection, out targetDirection2);
            bool result2 = SpineExtension.GetPerpendicularLineDistanceFromBase(spine, halfWayDistance, out intersection2, out targetDirection3);
            bool result3 = SpineExtension.GetPerpendicularLineDistanceFromBase(spine, spineDistance2, out intersection3, out targetDirection4);

            
            PointF[] temp = rotatedRect.GetVertices();

            bool widthShorter = temp[1].DistanceSquared(temp[0]) < temp[1].DistanceSquared(temp[2]);
            Vector targetDirection;
            if (widthShorter)
            {
                targetDirection = new Vector(temp[1].X - temp[0].X, temp[1].Y - temp[0].Y);
            }
            else
            {
                targetDirection = new Vector(temp[1].X - temp[2].X, temp[1].Y - temp[2].Y);
            }


            if (!result)
            {
                //Console.WriteLine("Unable to get perpendiular length");
                bodyPoints = null;
                waistLength = -1;
                pelvicArea = -1;
                pelvicArea2 = -1;
                return;
            }

            PointF i1 = new PointF(intersection.X + (float)(targetDirection.X * 100), intersection.Y + (float)(targetDirection.Y * 100));
            PointF i2 = new PointF(intersection.X + (float)(targetDirection.X * -100), intersection.Y + (float)(targetDirection.Y * -100));

            PointF i3 = new PointF(intersection2.X + (float)(targetDirection.X * 100), intersection2.Y + (float)(targetDirection.Y * 100));
            PointF i4 = new PointF(intersection2.X + (float)(targetDirection.X * -100), intersection2.Y + (float)(targetDirection.Y * -100));

            PointF i5 = new PointF(intersection3.X + (float)(targetDirection.X * 100), intersection3.Y + (float)(targetDirection.Y * 100));
            PointF i6 = new PointF(intersection3.X + (float)(targetDirection.X * -100), intersection3.Y + (float)(targetDirection.Y * -100));
            

            PointF bodyI1 = MathExtension.PolygonLineIntersectionPoint(intersection, i1, mouseContour);
            PointF bodyI2 = MathExtension.PolygonLineIntersectionPoint(intersection, i2, mouseContour);

            PointF bodyI3 = MathExtension.PolygonLineIntersectionPoint(intersection2, i3, mouseContour);
            PointF bodyI4 = MathExtension.PolygonLineIntersectionPoint(intersection2, i4, mouseContour);

            PointF bodyI5 = MathExtension.PolygonLineIntersectionPoint(intersection3, i5, mouseContour);
            PointF bodyI6 = MathExtension.PolygonLineIntersectionPoint(intersection3, i6, mouseContour);

            //drawImage.Draw(new CircleF(bodyI1, 4), new Bgr(Color.Red), 3);
            //drawImage.Draw(new CircleF(bodyI2, 4), new Bgr(Color.Red), 3);
            //drawImage.Draw(new CircleF(bodyI3, 4), new Bgr(Color.Red), 3);
            //drawImage.Draw(new CircleF(bodyI4, 4), new Bgr(Color.Red), 3);
            //drawImage.Draw(new CircleF(bodyI5, 4), new Bgr(Color.Red), 3);
            //drawImage.Draw(new CircleF(bodyI6, 4), new Bgr(Color.Red), 3);

            //drawImage.Draw(new LineSegment2DF(bodyI3, bodyI4), new Bgr(Color.Yellow), 3);
            //drawImage.Draw(new LineSegment2DF(bodyI5, bodyI6), new Bgr(Color.Red), 3);

            drawImage.Draw(new LineSegment2DF(bodyI1, bodyI2), new Bgr(Color.MediumPurple), 3);
            //drawImage.Draw(new LineSegment2DF(bodyI3, bodyI4), new Bgr(Color.MediumPurple), 3);
            waistLength = bodyI1.Distance(bodyI2);
            //Console.WriteLine("Waist Length = " + waistLength);
            //List<Point> tempBodyPoints = new List<Point>();

            //double r1BodyStartToTail = r1BodyStart.DistanceSquared(alternateSegment1[0]);
            //double r1BodyEndToTail = r1BodyEnd.DistanceSquared(alternateSegment1[0]);

            //bool startCloser = r1BodyStartToTail < r1BodyEndToTail;

            //if (!r1BodyStart.IsEmpty && !r1BodyEnd.IsEmpty)
            //{
            //    //Find segment from mouse contour
            //    Point[] bodySegment = MathExtension.GetSegmentOfPolygon(mouseContour, r1BodyStart, r1BodyEnd);
            //    //tempBodyPoints.AddRange(bodySegment);

            //    ////PointF midPoint = r1BodyStart.MidPoint(r1BodyEnd);
            //    //Point[] newBodySegment;
            //    //if (startCloser)
            //    //{
            //    //    newBodySegment = MathExtension.GetSegmentOfPolygon(mouseContour, bodyI3, bodyI5);
            //    //}
            //    //else
            //    //{
            //    //    newBodySegment = MathExtension.GetSegmentOfPolygon(mouseContour, bodyI5, bodyI3);
            //    //}

            //    //tempBodyPoints.AddRange(newBodySegment);

            //    //for (int i = 1; i < bodySegment.Length; i++)
            //    //{
            //    //    LineSegment2D line = new LineSegment2D(bodySegment[i - 1], bodySegment[i]);
            //    //    drawImage.Draw(line, new Bgr(Color.Cyan), 2);
            //    //}
            //}

            //if (!r2BodyStart.IsEmpty && !r2BodyEnd.IsEmpty)
            //{
            //    //Find segment from mouse contour
            //    Point[] bodySegment = MathExtension.GetSegmentOfPolygon(mouseContour, r2BodyStart, r2BodyEnd);
            //    //tempBodyPoints.AddRange(bodySegment.Reverse());

            //    ////PointF midPoint = r2BodyStart.MidPoint(r2BodyEnd);
            //    //Point[] newBodySegment;
            //    //if (startCloser)
            //    //{
            //    //    newBodySegment = MathExtension.GetSegmentOfPolygon(mouseContour, bodyI4, bodyI6);
            //    //}
            //    //else
            //    //{
            //    //    newBodySegment = MathExtension.GetSegmentOfPolygon(mouseContour, bodyI6, bodyI4);
            //    //}

            //    //tempBodyPoints.AddRange(newBodySegment.Reverse());

            //    //for (int i = 1; i < bodySegment.Length; i++)
            //    //{
            //    //    LineSegment2D line = new LineSegment2D(bodySegment[i - 1], bodySegment[i]);
            //    //    drawImage.Draw(line, new Bgr(Color.Cyan), 2);
            //    //}
            //}

            Point[] bodySegment3 = MathExtension.GetSegmentOfPolygon(mouseContour, r1BodyStart, r1BodyEnd);
            Point[] bodySegment4 = MathExtension.GetSegmentOfPolygon(mouseContour, r2BodyStart, r2BodyEnd);
            var bodyPoints2 = new List<Point>(bodySegment3.Concat(bodySegment4.Reverse()));
            PointF[] bodyPoints2F = bodyPoints2.Select(x => x.ToPointF()).ToArray();
            Point[] squareBodyPoints = new Point[] {bodyI3.ToPoint(), bodyI4.ToPoint(), bodyI6.ToPoint(), bodyI5.ToPoint()};
            //PointF[] tempBodyPointsF = tempBodyPoints.Select(x => x.ToPointF()).ToArray();

            if (bodyPoints2F != null && bodyPoints2F.Length > 0)
            {
                var rect1 = CvInvoke.MinAreaRect(bodyPoints2F);
                rect1.Angle += 90;
                //CvInvoke.Ellipse(drawImage, rect1, new MCvScalar(0, 0, 255), 3);
            }

            pelvicArea = -1;
            pelvicArea2 = -1;
            if (squareBodyPoints != null && squareBodyPoints.Length > 0)
            {
                var rect2 = CvInvoke.MinAreaRect(squareBodyPoints.Select(x => x.ToPointF()).ToArray());
                rect2.Angle += 90;
                //CvInvoke.Ellipse(drawImage, rect2, new MCvScalar(0, 255, 255), 3);
                pelvicArea = MathExtension.PolygonArea(squareBodyPoints);
                //Console.WriteLine("Pelvic Area tail: " + pelvicArea + " - " + (rect2.Size.Height * rect2.Size.Width));
                pelvicArea2 = rect2.Size.Height * rect2.Size.Width;
            }

            //Ellipse fittedEllipse = PointCollection.EllipseLeastSquareFitting(bodyPoints2F);
            //CvInvoke.Ellipse(drawImage, fittedEllipse.RotatedRect, new MCvScalar(0, 0, 255), 3);
            //fittedEllipse = PointCollection.EllipseLeastSquareFitting(tempBodyPointsF);
            //CvInvoke.Ellipse(drawImage, fittedEllipse.RotatedRect, new MCvScalar(0, 0, 255), 3);



            //drawImage.DrawPolyline(rect1.GetVertices().Select(x => x.ToPoint()).ToArray(), true, new Bgr(Color.Orange), 4);
            //drawImage.DrawPolyline(rect2.GetVertices().Select(x => x.ToPoint()).ToArray(), true, new Bgr(Color.Orange), 4);
            //drawImage.DrawPolyline(fittedEllipse.RotatedRect.GetVertices().Select(x => x.ToPoint()).ToArray(), true, new Bgr(Color.Cyan), 4);
            bodyPoints = bodyPoints2;
            //pelvicArea = MathExtension.PolygonArea(squareBodyPoints);
            
            //bodyPoints = new List<Point>(new Point[] { bodyI3.ToPoint(), bodyI4.ToPoint(), bodyI6.ToPoint(), bodyI5.ToPoint() });
            //bodyPoints = tempBodyPoints;
            //StringBuilder s1 = new StringBuilder();
            //StringBuilder s2 = new StringBuilder();

            //foreach (double dist in r1Distances)
            //{
            //    s1.Append(dist + ",");
            //}

            //foreach (double dist in r2Distances)
            //{
            //    s2.Append(dist + ",");
            //}

            //System.IO.File.WriteAllText(@"D:\PhD\r1test.txt", s1.ToString());
            //System.IO.File.WriteAllText(@"D:\PhD\r2test.txt", s2.ToString());
        }

        private double GetMinBoundingArea(Point[] points)
        {
            int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;

            foreach (var point in points)
            {
                if (point.X < minX)
                    minX = point.X;

                if (point.X > maxX)
                    maxX = point.X;

                if (point.Y < minY)
                    minY = point.Y;

                if (point.Y > maxY)
                    maxY = point.Y;
            }

            int width = maxX - minX;
            int height = maxY - minY;

            return width*height;
        }

        //private PointF FindCentroid(Point[] points)
        //{
        //    // Add the first point at the end of the array.
        //    int num_points = points.Length;
        //    Point[] pts = new Point[num_points + 1];
        //    points.CopyTo(pts, 0);
        //    pts[num_points] = points[0];

        //    // Find the centroid.
        //    float X = 0;
        //    float Y = 0;
        //    float second_factor;
        //    for (int i = 0; i < num_points; i++)
        //    {
        //        second_factor =
        //            pts[i].X * pts[i + 1].Y -
        //            pts[i + 1].X * pts[i].Y;
        //        X += (pts[i].X + pts[i + 1].X) * second_factor;
        //        Y += (pts[i].Y + pts[i + 1].Y) * second_factor;
        //    }

        //    // Divide by 6 times the polygon's area.
        //    float polygon_area = PolygonArea(points);
        //    X /= (6 * polygon_area);
        //    Y /= (6 * polygon_area);

        //    // If the values are negative, the polygon is
        //    // oriented counterclockwise so reverse the signs.
        //    if (X < 0)
        //    {
        //        X = -X;
        //        Y = -Y;
        //    }

        //    return new PointF(X, Y);
        //}

        //private float PolygonArea(Point[] points)
        //{
        //    // Return the absolute value of the signed area.
        //    // The signed area is negative if the polyogn is
        //    // oriented clockwise.
        //    return Math.Abs(SignedPolygonArea(points));
        //}

        //private float SignedPolygonArea(Point[] points)
        //{
        //    // Add the first point to the end.
        //    int num_points = points.Length;
        //    Point[] pts = new Point[num_points + 1];
        //    points.CopyTo(pts, 0);
        //    pts[num_points] = points[0];

        //    // Get the areas.
        //    float area = 0;
        //    for (int i = 0; i < num_points; i++)
        //    {
        //        area +=
        //            (pts[i + 1].X - pts[i].X) *
        //            (pts[i + 1].Y + pts[i].Y) / 2;
        //    }

        //    // Return the result.
        //    return area;
        //}
    }
}
