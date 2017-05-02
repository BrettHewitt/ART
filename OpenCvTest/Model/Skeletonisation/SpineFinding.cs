using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.Extensions;
using AutomatedRodentTracker.Model;
using AutomatedRodentTracker.ModelInterface.Skeletonisation;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using AutomatedRodentTracker.Services.ImageToBitmapSource;

namespace AutomatedRodentTracker.Model.Skeletonisation
{
    internal class SpineFinding : ModelObjectBase, ISpineFinding, IDisposable
    {
        private int m_NumberOfIterations;
        private int m_NumberOfCycles;
        private Image<Gray, Byte> m_SkeletonImage;

        public int NumberOfIterations
        {
            get
            {
                return m_NumberOfIterations;
            }
            set
            {
                if (Equals(m_NumberOfIterations, value))
                {
                    return;
                }

                m_NumberOfIterations = value;

                MarkAsDirty();
            }
        }

        public int NumberOfCycles
        {
            get
            {
                return m_NumberOfCycles;
            }
            set
            {
                if (Equals(m_NumberOfCycles, value))
                {
                    return;
                }

                m_NumberOfCycles = value;

                MarkAsDirty();
            }
        }

        public Image<Gray, Byte> SkeletonImage
        {
            get
            {
                return m_SkeletonImage;
            }
            set
            {
                if (ReferenceEquals(m_SkeletonImage, value))
                {
                    return;
                }

                if (m_SkeletonImage != null)
                {
                    m_SkeletonImage.Dispose();
                }

                m_SkeletonImage = value;

                MarkAsDirty();
            }
        }

        public SpineFinding()
        {
            NumberOfIterations = 3;
        }

        private RotatedRect m_RotatedRectangle = RotatedRect.Empty;
        public RotatedRect RotatedRectangle
        {
            get
            {
                return m_RotatedRectangle;
            }
            set
            {
                if (Equals(m_RotatedRectangle, value))
                {
                    return;
                }

                m_RotatedRectangle = value;

                MarkAsDirty();
            }
        }

        public PointF[] GenerateSpine(PointF headPoint, PointF tailPoint)
        {
            int cycles = NumberOfCycles;
            Image<Gray, Byte> workingImage = SkeletonImage.Clone();
            while (cycles > 0)
            {
                using (Image<Gray, Byte> dilate1 = workingImage.Dilate(NumberOfIterations))
                using (Image<Gray, Byte> erode1 = dilate1.Erode(NumberOfIterations))
                {
                    workingImage.Dispose();
                    workingImage = erode1.Clone();
                }

                cycles--;
            }

            LineSegment2D[] lines = CvInvoke.HoughLinesP(workingImage,
                   1, //Distance resolution in pixel-related units
                   Math.PI / 45, //Angle resolution measured in radians.
                   20, //threshold
                   5, //min Line width
                   40); //gap between lines

            LineSegment2D[] spineLines = FindSpine(lines, RotatedRectangle, workingImage, headPoint, tailPoint);

            if (spineLines == null)
            {
                return null;
            }

            //Smooth lines out
            List<PointF> smoothPoints = new List<PointF>();
            smoothPoints.Add(spineLines[0].P1);

            foreach (var line in spineLines)
            {
                smoothPoints.Add(line.P2);
            }

            return smoothPoints.ToArray();
        }

        private LineSegment2D[] FindSpine(LineSegment2D[] lines, RotatedRect rotatedRectangle, Image<Gray, Byte> img, PointF headPoint, PointF tailPoint)
        {
            //LineSegment2DF[] initialLines = new LineSegment2DF[2];

            //if (!rotatedRectangle.Size.IsEmpty)
            //{
            //    //Use one of the smaller boundries from rotatedRect for initial detection
            //    PointF[] vertices = rotatedRectangle.GetVertices();
            //    PointF p1 = vertices[0];
            //    PointF p2 = vertices[1];
            //    PointF p3 = vertices[2];
            //    PointF p4 = vertices[3];

            //    //PointF p1 = new PointF(rotatedRectangle.Left, rotatedRectangle.Top);
            //    //PointF p2 = new PointF(rotatedRectangle.Left, rotatedRectangle.Bottom);
            //    //PointF p3 = new PointF(rotatedRectangle.Right, rotatedRectangle.Bottom);
            //    //PointF p4 = new PointF(rotatedRectangle.Right, rotatedRectangle.Top);

            //    if (p2.DistanceSquared(p1) < p2.DistanceSquared(p3))
            //    {
            //        //p1 and p2 are paired, p3 and p4 are paired
            //        initialLines[0] = new LineSegment2DF(p1, p2);
            //        initialLines[1] = new LineSegment2DF(p3, p4);
            //    }
            //    else
            //    {
            //        //p2 and p3 are paired, p1 and p4 are paired
            //        initialLines[0] = new LineSegment2DF(p2, p3);
            //        initialLines[1] = new LineSegment2DF(p1, p4);
            //    }
            //}
            //else
            //{
            //    //Use one of the image sides for intial detection
            //    initialLines[1] = new LineSegment2DF(new PointF(0, 0), new PointF(0, img.Height - 1));
            //    initialLines[0] = new LineSegment2DF(new PointF(img.Width - 1, 0), new PointF(img.Width - 1, img.Height - 1));
            //    //initialLines[0] = new LineSegment2DF(new PointF(0, 0), new PointF(img.Width - 1, 0));
            //    //initialLines[1] = new LineSegment2DF(new PointF(0 - 1, img.Height-1), new PointF(img.Width - 1, img.Height - 1));
            //}

            //Find closest line segment to initial line
            double minDistance = double.MaxValue;

            LineSegment2D? targetLine = null;
            foreach (LineSegment2D line in lines)
            {
                //double minDistance1 = MathExtension.MinDistanceFromLineToPoint(initialLines[0].P1, initialLines[0].P2, line.P1);
                //double minDistance2 = MathExtension.MinDistanceFromLineToPoint(initialLines[0].P1, initialLines[0].P2, line.P2);

                double minDistance1 = headPoint.DistanceSquared(line.P1);
                double minDistance2 = headPoint.DistanceSquared(line.P2);

                double currentDist = minDistance1 < minDistance2 ? minDistance1 : minDistance2;

                if (currentDist < minDistance)
                {
                    minDistance = currentDist;
                    targetLine = line;
                }
            }

            List<LineSegment2D> previousLines = new List<LineSegment2D>();

            //We have our target line, try to traverse to the other side
            LineSegment2D? nextLine = null;
            
            if (targetLine.HasValue)
            {
                previousLines.Add(targetLine.Value);
            }

            do
            {
                GetValue(lines, tailPoint, previousLines.ToArray(), targetLine, ref nextLine);

                if (nextLine.HasValue)
                {
                    targetLine = nextLine;
                    previousLines.Add(nextLine.Value);
                }
            }
            while (nextLine.HasValue);

            if (previousLines.Count == 0)
            {
                return null;
            }

            double minDistanceFromEnd1 = tailPoint.Distance(previousLines.Last().P1);//MathExtension.MinDistanceFromLineToPoint(initialLines[1].P1, initialLines[1].P2, previousLines.Last().P1));
            double minDistanceFromEnd2 = tailPoint.Distance(previousLines.Last().P2);//MathExtension.MinDistanceFromLineToPoint(initialLines[1].P1, initialLines[1].P2, previousLines.Last().P2);

            if (minDistanceFromEnd1 < 30 || minDistanceFromEnd2 < 30)
            {
                //Made it!
                return previousLines.ToArray();
            }

            minDistance = double.MaxValue;

            targetLine = null;
            foreach (var line in lines)
            {
                double minDistance1 = tailPoint.Distance(line.P1);//MathExtension.MinDistanceFromLineToPoint(initialLines[1].P1, initialLines[1].P2, line.P1));
                double minDistance2 = tailPoint.Distance(line.P2);//MathExtension.MinDistanceFromLineToPoint(initialLines[1].P1, initialLines[1].P2, line.P2));

                double currentDist = minDistance1 < minDistance2 ? minDistance1 : minDistance2;

                if (currentDist < minDistance)
                {
                    minDistance = currentDist;
                    targetLine = line;
                }
            }


            List<LineSegment2D> previousLines2 = new List<LineSegment2D>();

            //We have our target line, try to traverse to the other side
            nextLine = null;
            if (targetLine.HasValue)
            {
                previousLines2.Add(targetLine.Value);
            }

            do
            {
                GetValue(lines, headPoint, previousLines2.ToArray(), targetLine, ref nextLine);

                if (nextLine.HasValue)
                {
                    targetLine = nextLine;
                    previousLines2.Add(nextLine.Value);
                }
            }
            while (nextLine.HasValue);

            previousLines2.Reverse();
            previousLines.AddRange(previousLines2);

            return previousLines.ToArray();
        }

        private void GetValue(IEnumerable<LineSegment2D> lines, LineSegment2DF endLine, LineSegment2D[] previousLines, LineSegment2D? targetLine, ref LineSegment2D? nextLine)
        {
            const double minDistanceThreshold = 20;
            double minDistanceToEnd = double.MaxValue;
            nextLine = null;
            //targetPoint = null;
            foreach (var line in lines)
            {
                if (previousLines.Contains(line))
                {
                    continue;
                }

                if (line.P1 == targetLine.Value.P1 && line.P2 == targetLine.Value.P2)
                {
                    continue;
                }

                double dist1 = MathExtension.FindDistanceBetweenSegments(targetLine.Value.P1, targetLine.Value.P2, line.P1, line.P2);

                double minDistToEnd = MathExtension.FindDistanceBetweenSegments(line.P1, line.P2, endLine.P1, endLine.P2);
                double currentDistToEnd = MathExtension.FindDistanceBetweenSegments(targetLine.Value.P1, targetLine.Value.P2, endLine.P1, endLine.P2);

                if (dist1 < minDistanceThreshold && minDistToEnd < currentDistToEnd && minDistToEnd < minDistanceToEnd)
                {
                    minDistanceToEnd = minDistToEnd;
                    nextLine = line;
                }
            }
        }

        private void GetValue(IEnumerable<LineSegment2D> lines, PointF endPoint, LineSegment2D[] previousLines, LineSegment2D? targetLine, ref LineSegment2D? nextLine)
        {
            const double minDistanceThreshold = 20;
            double minDistanceToEnd = double.MaxValue;
            nextLine = null;
            //targetPoint = null;
            foreach (var line in lines)
            {
                if (previousLines.Contains(line))
                {
                    continue;
                }

                if (line.P1 == targetLine.Value.P1 && line.P2 == targetLine.Value.P2)
                {
                    continue;
                }

                double dist1 = MathExtension.FindDistanceBetweenSegments(targetLine.Value.P1, targetLine.Value.P2, line.P1, line.P2);

                double minDistToEnd = MathExtension.FindDistanceToSegment(endPoint, line.P1, line.P2);//.FindDistanceBetweenSegments(line.P1, line.P2, endLine.P1, endLine.P2);
                double currentDistToEnd = MathExtension.FindDistanceToSegment(endPoint, targetLine.Value.P1, targetLine.Value.P2);//.FindDistanceBetweenSegments(targetLine.Value.P1, targetLine.Value.P2, endLine.P1, endLine.P2);

                if (dist1 < minDistanceThreshold && minDistToEnd < currentDistToEnd && minDistToEnd < minDistanceToEnd)
                {
                    minDistanceToEnd = minDistToEnd;
                    nextLine = line;
                }
            }
        }

        public void Dispose()
        {
            if (SkeletonImage != null)
            {
                SkeletonImage.Dispose();
            }
        }
    }
}
