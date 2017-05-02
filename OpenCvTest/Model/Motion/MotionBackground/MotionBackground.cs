using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AutomatedRodentTracker.Model;
using AutomatedRodentTracker.Model.Resolver;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using AutomatedRodentTracker.ModelInterface.Motion.BackgroundSubtraction;
using AutomatedRodentTracker.ModelInterface.Motion.MotionBackground;
using AutomatedRodentTracker.ModelInterface.Video;
using AutomatedRodentTracker.Services.ImageToBitmapSource;
using Point = System.Drawing.Point;

namespace AutomatedRodentTracker.Model.Motion.MotionBackground
{
    internal class MotionBackground : ModelObjectBase, IMotionBackground
    {
        private int m_MotionLength;
        private IVideo m_Video;
        private double m_CannyThreshold1 = 120;
        private double m_CannyThreshold2 = 180;

        public IVideo Video
        {
            get
            {
                return m_Video;
            }
            set
            {
                if (Equals(m_Video, value))
                {
                    return;
                }

                m_Video = value;

                MarkAsDirty();
            }
        }

        public int MotionLength
        {
            get
            {
                return m_MotionLength;
                ;
            }
            set
            {
                if (Equals(m_MotionLength, value))
                {
                    return;
                }

                m_MotionLength = value;

                MarkAsDirty();
            }
        }

        public void GenerateMotionBackground(double threshold, out Image<Gray, Byte> binaryBackground, Rectangle roi, int startFrame = 0)
        {
            if (Video == null || Video.FrameCount - startFrame <= MotionLength)
            {
                throw new Exception("Video can not be null and motion length must be over " + MotionLength);
            }

            Mat[] motionImages = new Mat[MotionLength];

            IMotionBackgroundSubtraction motion = ModelResolver.Resolve<IMotionBackgroundSubtraction>();
            motion.UpdateDetector(Video.FrameCount, 16, false);

            Video.SetFrame(startFrame);
            for (int i = 0; i < MotionLength; i++)
            {
                motionImages[i] = Video.GetFrameMat();

                if (roi.IsEmpty)
                {
                    motion.ProcessFrame(motionImages[i]);
                }
                else
                {
                    using (Image<Gray, Byte> tempImage = motionImages[i].ToImage<Gray, Byte>(true))
                    using (Image<Gray, Byte> roiImage = tempImage.GetSubRect(roi))
                    {
                        motion.ProcessFrame(roiImage);
                    }
                }
            }
            
            Image<Gray, Byte> original = motionImages[MotionLength - 1].ToImage<Gray, Byte>(true);
            original.ROI = roi;
            

            //Image<Gray, Byte> tempOriginal = original.CopyBlank();
            //double otsuThreshold = CvInvoke.Threshold(original, tempOriginal, 20, 255, ThresholdType.Otsu | ThresholdType.Binary);
            Image<Gray, Byte> originBinary = original.ThresholdBinary(new Gray(threshold), new Gray(255));

            using (Mat motionMat = motion.ForegroundMask)
            using (Image<Gray, Byte> motionImage = motionMat.ToImage<Gray, Byte>())
            using (Image<Gray, Byte> filteredImage = motionImage.SmoothMedian(3))
            {
                //motionImage.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\011 - Final.png");
                //Hough lines
                using (Mat canneyEdges = new Mat())
                {
                    CvInvoke.Canny(filteredImage, canneyEdges, m_CannyThreshold1, m_CannyThreshold2);
                    //filteredImage.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\010 - Final.png");
                    //canneyEdges.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\09 - Final.png");
                    LineSegment2D[] lines = CvInvoke.HoughLinesP(canneyEdges,
                   1, //Distance resolution in pixel-related units
                   Math.PI / 45, //Angle resolution measured in radians.
                   20, //threshold
                   5, //min Line width
                   40); //gap between lines

                    using (VectorOfVectorOfPoint apxContour = new VectorOfVectorOfPoint())
                    {
                        using (Image<Gray, Byte> test1 = new Image<Gray, byte>(original.Width, original.Height, new Gray(0)))
                        using (Image<Gray, Byte> test3 = test1.Clone())
                        {
                            foreach (var line in lines)
                            {
                                test1.Draw(line, new Gray(255), 1);
                            }
                            //test1.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\08 - Final.png");
                            CvInvoke.FindContours(test1, apxContour, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                            List<Point> pointsF = new List<Point>();

                            for (int i = 0; i < apxContour.Size; i++)
                            {
                                CvInvoke.DrawContours(test3, apxContour, i, new MCvScalar(255), 4);
                                pointsF.AddRange(apxContour[i].ToArray());
                            }
                            //test3.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\07 - Final.png");
                            PointF[] finalPoints = pointsF.Select(x => new PointF(x.X, x.Y)).ToArray();
                            PointF[] convexHull = CvInvoke.ConvexHull(finalPoints, true);
                            Point[] finalfinalPoints = convexHull.Select(x => new Point((int)x.X, (int)x.Y)).ToArray();

                            test3.Draw(finalfinalPoints, new Gray(255), 4);
                            //using (Image<Bgr, Byte> temp = motionImages[0].ToImage<Bgr, Byte>())
                            ////using (Image<Gray, Byte> temp2 = new Image<Gray, byte>(temp.Size))
                            //{
                            //    //temp2.SetValue(0);
                            //    //temp2.Draw(finalfinalPoints, new Gray(255), -1);

                            //    using (Image<Gray, Byte> temp3 = originBinary.Clone())
                            //    {
                            //        temp3.Draw(finalfinalPoints, new Gray(255), -1);
                            //        temp3.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\06c-Final.png");
                            //    }
                            //using (Image<Bgr, Byte> temp = original.Convert<Bgr, Byte>())
                            //{
                            //    temp.Draw(finalfinalPoints, new Bgr(Color.Red), 3);
                            //    temp.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\06d-Final.png");
                            //}
                            //}
                            //test3.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\06 - Final.png");
                            byte[,,] imageData = originBinary.Data;

                            Point[] currentContour = convexHull.Select(x => new Point((int)x.X, (int)x.Y)).ToArray();
                            List<Point> currentContourPoints = new List<Point>(currentContour);

                            List<List<Point>> allPoints = new List<List<Point>>();
                            allPoints.Add(new List<Point>());
                            allPoints[0].Add(currentContourPoints[0]);
                            int allPointsIndex = 0;
                            bool blackFlag = false;

                            for (int j = 1; j < currentContourPoints.Count; j++)
                            {
                                Point firstPoint = currentContourPoints[j - 1];
                                Point secondPoint = currentContourPoints[j];

                                //bool go2 = firstPoint.X == 441 && firstPoint.Y == 356 && secondPoint.X == 794 && secondPoint.Y == 771;

                                List<Point> pixelPoints = GetBresenhamLine(firstPoint, secondPoint);

                                for (int i = 0; i < pixelPoints.Count; i++)
                                {
                                    Point pixel = pixelPoints[i];
                                    Byte currentPixel = imageData[pixel.Y, pixel.X, 0];

                                    if (currentPixel == 0)
                                    {
                                        //It's black
                                        if (!blackFlag && j != 0 && i != 0)
                                        {
                                            allPoints[allPointsIndex].Add(new Point(pixel.X, pixel.Y));
                                            allPoints.Add(new List<Point>());
                                            allPointsIndex++;
                                        }

                                        blackFlag = true;
                                    }
                                    else
                                    {
                                        if (blackFlag)
                                        {
                                            //We've gone from black to white
                                            allPoints[allPointsIndex].Add(new Point(pixel.X, pixel.Y));
                                            blackFlag = false;
                                        }
                                    }
                                }

                                if (!blackFlag)
                                {
                                    allPoints[allPointsIndex].Add(secondPoint);
                                }
                            }

                            //First contour dealt with
                            List<List<Point>> tempList = new List<List<Point>>();
                            for (int j = 0; j < allPoints.Count; j++)
                            {
                                List<Point> list = allPoints[j];
                                if (list.Count > 1)
                                {
                                    tempList.Add(list);
                                }
                            }

                            List<List<Point>> finalWhitePoints = new List<List<Point>>();
                            for (int j = 0; j < tempList.Count; j++)
                            {
                                List<Point> list = tempList[j];
                                double distanceCounter = 0;
                                for (int i = 1; i < list.Count; i++)
                                {
                                    Point p1 = list[i - 1];
                                    Point p2 = list[i];

                                    distanceCounter += Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
                                }

                                if (distanceCounter < 10)
                                {
                                    int nextIndex = j + 1;
                                    int prevIndex = j - 1;

                                    if (nextIndex == tempList.Count)
                                    {
                                        nextIndex = 0;
                                    }

                                    if (prevIndex == -1)
                                    {
                                        prevIndex = tempList.Count - 1;
                                    }

                                    List<Point> nextList = tempList[nextIndex];
                                    List<Point> prevList = tempList[prevIndex];

                                    Point closestNextPoint = nextList.First();
                                    Point closestPrevPoint = prevList.Last();
                                    Point firstPoint = list.First();
                                    Point lastPoint = list.Last();

                                    Vector first = new Vector(Math.Abs(firstPoint.X - closestPrevPoint.X), Math.Abs(firstPoint.Y - closestPrevPoint.Y));
                                    Vector last = new Vector(Math.Abs(lastPoint.X - closestNextPoint.X), Math.Abs(lastPoint.Y - closestNextPoint.Y));

                                    if (first.Length <= 5 && last.Length <= 5)
                                    {
                                        finalWhitePoints.Add(list);
                                    }
                                }
                                else
                                {
                                    finalWhitePoints.Add(list);
                                }
                            }

                            List<List<Point>> superFinalList = new List<List<Point>>();
                            
                            foreach (List<Point> list in finalWhitePoints)
                            {
                                superFinalList.Add(new List<Point>());
                                List<Point> listToAdd = new List<Point>();
                                foreach (Point point in list)
                                {
                                    double closestDistance = double.MaxValue;
                                    Point? bestPoint = null;

                                    //Search for closest point in apxContour
                                    foreach (var firstTest in apxContour.ToArrayOfArray())
                                    {
                                        foreach (Point secondTest in firstTest)
                                        {
                                            double distance = GetSquaredDistance(point, secondTest);
                                            if (distance < closestDistance)
                                            {
                                                closestDistance = distance;
                                                bestPoint = secondTest;
                                            }
                                        }
                                    }

                                    listToAdd.Add(bestPoint.Value);
                                }

                                superFinalList.Add(listToAdd);
                            }

                            List<Point> megaFinal = new List<Point>();

                            foreach (var list in superFinalList)
                            {
                                foreach (var point in list)
                                {
                                    megaFinal.Add(point);
                                }
                            }
                            
                            using (Image<Gray, Byte> finalImage = test1.CopyBlank())
                            {
                                finalImage.SetValue(new Gray(0));

                                for (int i = 0; i < apxContour.Size; i++)
                                {
                                    CvInvoke.DrawContours(finalImage, apxContour, i, new MCvScalar(255), 3);
                                }
                                //finalImage.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\05 - Final.png");
                                finalImage.DrawPolyline(megaFinal.ToArray(), true, new Gray(255), 3);

                                //using (Image<Bgr, Byte> temp3 = original.Convert<Bgr, Byte>())
                                //using (Image<Gray, Byte> temp5 = new Image<Gray, byte>(temp3.Size))
                                //{
                                //    temp5.SetValue(0);
                                //    temp5.Draw(megaFinal.ToArray(), new Gray(255), -1);
                                //    using (Image<Gray, Byte> temp6 = originBinary.Add(temp5))
                                //    {
                                //        temp6.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\04b-Final.png");
                                //    }

                                //    //temp3.DrawPolyline(megaFinal.ToArray(), true, new Bgr(Color.Red), 3);
                                //    //temp3.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\04-Final.png");
                                //}

                                //finalImage.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\04 - Final.png");
                                Image<Gray, Byte> mask2 = new Image<Gray, byte>(finalImage.Width + 2, finalImage.Height + 2);
                                Rectangle rect2;

                                //Find correct place to start fill
                                double lowestX = double.MaxValue, highestX = 0, lowestY = double.MaxValue, highestY = 0;
                                foreach (var point in megaFinal)
                                {
                                    if (point.X < lowestX)
                                        lowestX = point.X;

                                    if (point.X > highestX)
                                        highestX = point.X;

                                    if (point.Y < lowestY)
                                        lowestY = point.Y;

                                    if (point.Y > highestY)
                                        highestY = point.Y;
                                }

                                //Choose highest delta
                                double leftDelta = lowestX;
                                double rightDelta = finalImage.Width - highestX;
                                double topDelta = lowestY;
                                double bottomDelta = finalImage.Height - highestY;
                                Point seedPoint;
                                if (leftDelta > rightDelta)
                                {
                                    //We're on the left
                                    if (topDelta > bottomDelta)
                                    {
                                        //top left
                                        seedPoint = new Point(0, 0);
                                    }
                                    else
                                    {
                                        //bottom left
                                        seedPoint = new Point(0, finalImage.Height - 1);
                                    }
                                }
                                else
                                {
                                    //We're on the right
                                    if (topDelta > bottomDelta)
                                    {
                                        //top right
                                        seedPoint = new Point(finalImage.Width - 1, 0);
                                    }
                                    else
                                    {
                                        //bottom right
                                        seedPoint = new Point(finalImage.Width - 1, finalImage.Height - 1);
                                    }
                                }

                                CvInvoke.FloodFill(finalImage, mask2, seedPoint, new MCvScalar(255, 0, 0), out rect2, new MCvScalar(5, 5, 5), new MCvScalar(5, 5, 5));
                                //finalImage.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\03 - Final.png");
                                for (int i = 0; i < apxContour.Size; i++)
                                {
                                    CvInvoke.DrawContours(finalImage, apxContour, i, new MCvScalar(0), 3);
                                }
                                //finalImage.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\02 - Final.png");
                                finalImage.DrawPolyline(megaFinal.ToArray(), true, new Gray(0), 3);
                                //finalImage.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\0 - Final.png");

                                //Image5 = ImageService.ToBitmapSource(finalImage);
                                using (Image<Gray, Byte> test4 = finalImage.Not())
                                //using (Image<Gray, Byte> binaryFirstFrame = motionImages[0].ToImage<Gray, Byte>())
                                //using (Image<Gray, Byte> bianryFirstFrame2 = binaryFirstFrame.ThresholdBinary(new Gray(threshold), new Gray(255)))
                                {
                                    binaryBackground = test4.Or(originBinary);
                                    //binaryMouse = binaryBackground.AbsDiff(bianryFirstFrame2);
                                }
                            }
                        }
                    }
                }
            }

            foreach (Mat mat in motionImages)
            {
                mat.Dispose();
            }

            motion.Dispose();
        }

        public void SetVideo(string fileName)
        {
            if (Video == null)
            {
                Video = ModelResolver.Resolve<IVideo>();
            }

            Video.SetVideo(fileName);
        }

        public void SetVideo(IVideo video)
        {
            Video = video;
        }

        public static List<Point> GetBresenhamLine(Point p0, Point p1)
        {
            int x0 = p0.X;
            int y0 = p0.Y;
            int x1 = p1.X;
            int y1 = p1.Y;
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);

            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;

            int err = dx - dy;

            var points = new List<Point>();

            while (true)
            {
                points.Add(new Point(x0, y0));
                if (x0 == x1 && y0 == y1) break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    x0 = x0 + sx;
                }
                if (e2 < dx)
                {
                    err = err + dx;
                    y0 = y0 + sy;
                }
            }

            return points;
        }

        private double GetSquaredDistance(Point p1, Point p2)
        {
            return Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2);
        }
    }
}
