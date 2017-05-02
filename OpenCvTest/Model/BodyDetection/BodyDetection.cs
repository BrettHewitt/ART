using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using AutomatedRodentTracker.Extensions;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.ModelInterface.BodyDetection;
using AutomatedRodentTracker.ModelInterface.Skeletonisation;
using AutomatedRodentTracker.Services.Mouse;
using AutomatedRodentTracker.Services.RBSK;
using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Point = System.Drawing.Point;

namespace AutomatedRodentTracker.Model.BodyDetection
{
    internal class BodyDetection : ModelObjectBase, IBodyDetection
    {
        private Image<Gray, Byte> m_BinaryBackground;
        public Image<Gray, Byte> BinaryBackground
        {
            get
            {
                return m_BinaryBackground;
            }
            set
            {
                if (Equals(m_BinaryBackground, value))
                {
                    return;
                }

                if (m_BinaryBackground != null)
                {
                    m_BinaryBackground.Dispose();
                }

                m_BinaryBackground = value;

                MarkAsDirty();
            }
        }

        private CvBlobDetector BlobDetector
        {
            get;
            set;
        }

        private double m_ThresholdValue;
        public double ThresholdValue
        {
            get
            {
                return m_ThresholdValue;
            }
            set
            {
                if (Equals(m_ThresholdValue, value))
                {
                    return;
                }

                m_ThresholdValue = value;

                MarkAsDirty();
            }
        }

        private Services.RBSK.RBSK RBSK
        {
            get;
            set;
        }

        private Image<Gray, Byte> SkelImage
        {
            get;
            set;
        }

        public BodyDetection()
        {
            ThresholdValue = 20;
            //VideoSettings = ModelResolver.ModelResolver.Resolve<IVideoSettings>();
            BlobDetector = new CvBlobDetector();
            RBSK = MouseService.GetStandardMouseRules();
        }

        public void GetBody(Image<Gray, Byte> filteredImage, out PointF centroid)
        {
            using (Image<Gray, Byte> binary = filteredImage.ThresholdBinary(new Gray(ThresholdValue), new Gray(255)))
            using (Image<Gray, Byte> backgroundNot = BinaryBackground.Not())
            using (Image<Gray, Byte> finalImage = binary.Add(backgroundNot))
            using (Image<Gray, Byte> subbed = finalImage.Not())
            {
                centroid = PointF.Empty;
                CvBlobs blobs = new CvBlobs();
                BlobDetector.Detect(subbed, blobs);

                CvBlob mouseBlob = null;
                double maxArea = -1;
                foreach (var blob in blobs.Values)
                {
                    if (blob.Area > maxArea)
                    {
                        mouseBlob = blob;
                        maxArea = blob.Area;
                    }
                }

                if (mouseBlob != null)
                {
                    centroid = mouseBlob.Centroid;
                }
            }
        }

        public void GetBody(Image<Bgr, Byte> frame, out PointF centroid)
        {
            using (Image<Gray, Byte> origGray = frame.Convert<Gray, Byte>())
            using (Image<Gray, Byte> filteredImage = origGray.SmoothMedian(13))
            using (Image<Gray, Byte> binary = filteredImage.ThresholdBinary(new Gray(ThresholdValue), new Gray(255)))
            using (Image<Gray, Byte> backgroundNot = BinaryBackground.Not())
            using (Image<Gray, Byte> finalImage = binary.Add(backgroundNot))
            using (Image<Gray, Byte> subbed = finalImage.Not())
            {
                centroid = PointF.Empty;
                CvBlobs blobs = new CvBlobs();
                BlobDetector.Detect(subbed, blobs);

                CvBlob mouseBlob = null;
                double maxArea = -1;
                foreach (var blob in blobs.Values)
                {
                    if (blob.Area > maxArea)
                    {
                        mouseBlob = blob;
                        maxArea = blob.Area;
                    }
                }

                if (mouseBlob != null)
                {
                    centroid = mouseBlob.Centroid;
                }
            }
        }

        public void FindBody(Image<Gray, Byte> filteredImage, out double waistLength, out double waistVolume, out double waistVolume2, out double waistVolume3, out double waistVolume4, out PointF centroid)
        {
            //using (Image<Gray, Byte> grayImage = image.Convert<Gray, Byte>())
            //using (Image<Gray, Byte> filteredImage = grayImage.SmoothMedian(rbsk.Settings.FilterLevel))
            //using (Image<Gray, Byte> binaryImage = filteredImage.ThresholdBinary(new Gray(rbsk.Settings.BinaryThreshold), new Gray(255)))
            //using (Image<Gray, Byte> backgroundNot = BackgroundImage.Not())
            //using (Image<Gray, Byte> finalImage = binaryImage.Add(backgroundNot))
            //bool debugTest = false;
            //if (frame.IsROISet)
            //{
            //    frame.ROI = Rectangle.Empty;
            //    debugTest = true;
            //}

            //using (Image<Gray, Byte> origGray = frame.Convert<Gray, Byte>())
            //using (Image<Gray, Byte> filteredImage = origGray.SmoothMedian(13))
            using (Image<Gray, Byte> binary = filteredImage.ThresholdBinary(new Gray(ThresholdValue), new Gray(255)))
            using (Image<Gray, Byte> backgroundNot = BinaryBackground.Not())
            using (Image<Gray, Byte> finalImage = binary.Add(backgroundNot))
            using (Image<Gray, Byte> subbed = finalImage.Not())
            //using (Image<Gray, Byte> subbed = BinaryBackground.AbsDiff(binary))
            {
                //ImageViewer.Show(finalImage);
                //ImageViewer.Show(subbed);

                //if (debugTest)
                //{
                //    ImageViewer.Show(subbed);
                //}

                CvBlobs blobs = new CvBlobs();
                BlobDetector.Detect(subbed, blobs);

                CvBlob mouseBlob = null;
                double maxArea = -1;
                foreach (var blob in blobs.Values)
                {
                    if (blob.Area > maxArea)
                    {
                        mouseBlob = blob;
                        maxArea = blob.Area;
                    }
                }

                //double gapDistance = GetBestGapDistance(rbsk);
                double gapDistance = 50;
                RBSK.Settings.GapDistance = gapDistance;
                //PointF[] headPoints = ProcessFrame(orig, RBSK);
                PointF center = mouseBlob.Centroid;
                centroid = mouseBlob.Centroid;
                //LineSegment2DF[] targetPoints = null;

                waistLength = -1;
                waistVolume = -1;
                waistVolume2 = -1;
                waistVolume3 = -1;
                waistVolume4 = -1;
                return;

                Point[] mouseContour = mouseBlob.GetContour();
                

                double prob = 0;
                Services.RBSK.RBSK headRbsk = MouseService.GetStandardMouseRules();
                headRbsk.Settings.GapDistance = 65;

                List<List<PointF>> allKeyPoints = headRbsk.FindKeyPoints(mouseContour, headRbsk.Settings.NumberOfSlides, false);

                if (allKeyPoints == null || allKeyPoints.Count == 0)
                {
                    waistLength = -1;
                    waistVolume = -1;
                    waistVolume2 = -1;
                    waistVolume3 = -1;
                    waistVolume4 = -1;
                    return;
                }

                PointF[] result = headRbsk.FindPointsFromRules(allKeyPoints[0], binary, ref prob);

                if (result == null)
                {
                    waistLength = -1;
                    waistVolume = -1;
                    waistVolume2 = -1;
                    waistVolume3 = -1;
                    waistVolume4 = -1;
                    return;
                }

                RotatedRect rotatedRect = CvInvoke.MinAreaRect(mouseContour.Select(x => new PointF(x.X, x.Y)).ToArray());
                //Console.WriteLine("Size: " + rotatedRect.Size);

                ISkeleton skel = ModelResolver.Resolve<ISkeleton>();
                Image<Gray, Byte> tempBinary = binary.Clone();

                Rectangle rect = mouseBlob.BoundingBox;

                if (rect.X >= tempBinary.Width || rect.X < 0 || rect.Width < 0 || rect.Width > tempBinary.Width)
                {
                    //Console.WriteLine(rect.ToString());
                    waistLength = -1;
                    waistVolume = -1;
                    waistVolume2 = -1;
                    waistVolume3 = -1;
                    waistVolume4 = -1;
                    return;
                }

                if (rect.Y >= tempBinary.Height || rect.Y < 0 || rect.Height < 0 || rect.Height > tempBinary.Height)
                {
                    //Console.WriteLine(rect.ToString());
                    waistLength = -1;
                    waistVolume = -1;
                    waistVolume2 = -1;
                    waistVolume3 = -1;
                    waistVolume4 = -1;
                    return;
                }

                if (rect.X < 0)
                {
                    rect.X = 0;
                }

                if (rect.Y < 0)
                {
                    rect.Y = 0;
                }

                if (rect.X + rect.Width > tempBinary.Width)
                {
                    rect.Width = tempBinary.Width - rect.X - 1;
                }

                if (rect.Y + rect.Height > tempBinary.Height)
                {
                    rect.Height = tempBinary.Height - rect.Y;
                }

                Image<Gray, Byte> binaryRoi;
                try
                {
                    binaryRoi = tempBinary.GetSubRect(rect);
                }
                catch (Exception)
                {
                    //Console.WriteLine(rect.ToString() + " - " + tempBinary.Width + " - " + tempBinary.Height);
                    waistLength = -1;
                    waistVolume = -1;
                    waistVolume2 = -1;
                    waistVolume3 = -1;
                    waistVolume4 = -1;
                    return;
                }

                using (Image<Bgr, Byte> displayImage = subbed.Convert<Bgr, Byte>())

                using (Image<Gray, Byte> skelImage = skel.GetSkeleton(binaryRoi))
                //using (Image<Bgr, Byte> drawImage = frame.Clone())
                using (Image<Bgr, Byte> tempImage2 = new Image<Bgr, byte>(filteredImage.Size))
                {
                    //-----------------------------------------
                    if (SkelImage != null)
                    {
                        SkelImage.Dispose();
                    }
                    SkelImage = skelImage.Clone();
                    //--------------------------------------------

                    tempImage2.SetValue(new Bgr(Color.Black));
                    ISpineFinding spineFinder = ModelResolver.Resolve<ISpineFinding>();
                    spineFinder.NumberOfCycles = 3;
                    spineFinder.NumberOfIterations = 1;
                    spineFinder.SkeletonImage = skelImage;

                    const int delta = 20;
                    double smallestAngle = double.MaxValue;
                    Point tailPoint = Point.Empty;
                    for (int i = 0; i < mouseContour.Length; i++)
                    {
                        int leftDelta = i - delta;
                        int rightDelta = i + delta;

                        if (leftDelta < 0)
                        {
                            leftDelta += mouseContour.Length;
                        }

                        if (rightDelta >= mouseContour.Length)
                        {
                            rightDelta -= mouseContour.Length;
                        }

                        Point testPoint = mouseContour[i];
                        Point leftPoint = mouseContour[leftDelta];
                        Point rightPoint = mouseContour[rightDelta];

                        Vector v1 = new Vector(leftPoint.X - testPoint.X, leftPoint.Y - testPoint.Y);
                        Vector v2 = new Vector(rightPoint.X - testPoint.X, rightPoint.Y - testPoint.Y);

                        double angle = Math.Abs(Vector.AngleBetween(v1, v2));

                        if (angle < 30 && angle > 9)
                        {
                            if (angle < smallestAngle)
                            {
                                smallestAngle = angle;
                                tailPoint = testPoint;
                            }
                        }
                    }

                    PointF headCornerCorrect = new PointF(result[2].X - rect.X, result[2].Y - rect.Y);
                    PointF tailCornerCorrect = new PointF(tailPoint.X - rect.X, tailPoint.Y - rect.Y);

                    PointF[] spine = spineFinder.GenerateSpine(headCornerCorrect, tailCornerCorrect);

                    if (spine == null)
                    {
                        waistLength = -1;
                        waistVolume = -1;
                        waistVolume2 = -1;
                        waistVolume3 = -1;
                        waistVolume4 = -1;
                        return;
                    }

                    Point topCorner = rect.Location;

                    PointF[] spineCornerCorrected = new PointF[spine.Length];

                    for (int i = 0; i < spine.Length; i++)
                    {
                        spineCornerCorrected[i] = new PointF(spine[i].X + topCorner.X, spine[i].Y + topCorner.Y);
                    }

                    ITailFinding tailFinding = ModelResolver.Resolve<ITailFinding>();
                    double rotatedWidth = rotatedRect.Size.Width < rotatedRect.Size.Height ? rotatedRect.Size.Width : rotatedRect.Size.Height;
                    List<Point> bodyPoints;

                    if (result != null)
                    {
                        double firstDist = result[2].DistanceSquared(spineCornerCorrected.First());
                        double lastDist = result[2].DistanceSquared(spineCornerCorrected.Last());

                        if (firstDist < lastDist)
                        {
                            spineCornerCorrected = spineCornerCorrected.Reverse().ToArray();
                        }
                    }

                    double pelvicArea1, pelvicArea2;
                    tailFinding.FindTail(mouseContour, spineCornerCorrected, displayImage, rotatedWidth, center, out bodyPoints, out waistLength, out pelvicArea1, out pelvicArea2);

                    waistVolume3 = pelvicArea1;
                    waistVolume4 = pelvicArea2;

                    if (bodyPoints != null)
                    {
                        Point[] bPoints = bodyPoints.ToArray();
                        waistVolume = MathExtension.PolygonArea(bPoints);
                        var rect2 = CvInvoke.MinAreaRect(bPoints.Select(x => x.ToPointF()).ToArray());
                        waistVolume2 = rect2.Size.Height * rect2.Size.Width;
                        
                    }
                    else
                    {
                        waistVolume = -1;
                        waistVolume2 = -1;
                    }
                    //Console.WriteLine(waistLength + " " + waistVolume);
                    //double rotatedRectArea = rotatedRect.Size.Width * rotatedRect.Size.Height;
                }
            }
        }

        //public void FindBody(Image<Gray, Byte> binary, Image<Gray, Byte> bianryNot, PointF headPoint, PointF tailPoint, out double waistLength)
        //{
        //    CvBlobs blobs = new CvBlobs();
        //    BlobDetector.Detect(bianryNot, blobs);

        //    CvBlob mouseBlob = null;
        //    double maxArea = -1;
        //    foreach (var blob in blobs.Values)
        //    {
        //        if (blob.Area > maxArea)
        //        {
        //            mouseBlob = blob;
        //            maxArea = blob.Area;
        //        }
        //    }

        //    Point[] mouseContour = mouseBlob.GetContour();
        //    Rectangle rect = mouseBlob.BoundingBox;
        //    RotatedRect rotatedRect = CvInvoke.MinAreaRect(mouseContour.Select(x => x.ToPointF()).ToArray());

        //    ISkeleton skel = ModelResolver.ModelResolver.Resolve<ISkeleton>();

        //    using (Image<Gray, Byte> tempBinary = binary.Clone())
        //    using (Image<Gray, Byte> binaryRoi = tempBinary.GetSubRect(rect))
        //    using (Image<Gray, Byte> skelImage = skel.GetSkeleton(binaryRoi))
        //    {
        //        ISpineFinding spineFinder = ModelResolver.ModelResolver.Resolve<ISpineFinding>();
        //        spineFinder.NumberOfCycles = 3;
        //        spineFinder.NumberOfIterations = 1;
        //        spineFinder.SkeletonImage = skelImage;

        //        PointF[] spine = spineFinder.GenerateSpine();
        //        Point topCorner = mouseBlob.BoundingBox.Location;
        //        PointF[] spineCornerCorrected = new PointF[spine.Length];

        //        for (int i = 0; i < spine.Length; i++)
        //        {
        //            spineCornerCorrected[i] = new PointF(spine[i].X + topCorner.X, spine[i].Y + topCorner.Y);
        //        }

        //        ITailFinding tailFinding = ModelResolver.ModelResolver.Resolve<ITailFinding>();
        //        double rotatedWidth = rotatedRect.Size.Width < rotatedRect.Size.Height ? rotatedRect.Size.Width : rotatedRect.Size.Height;

        //        double firstDist = headPoint.DistanceSquared(spineCornerCorrected.First());
        //        double lastDist = headPoint.DistanceSquared(spineCornerCorrected.Last());

        //        if (firstDist < lastDist)
        //        {
        //            spineCornerCorrected = spineCornerCorrected.Reverse().ToArray();
        //        }

        //        Point[] bodyleft;
        //        Point[] bodyRight;
        //        Point[] headPoints;
        //        Point[] tailPoints;
        //        double waistLength;

        //        tailFinding.FindTail(mouseContour, spineCornerCorrected, rotatedWidth, out tailPoints, out headPoints, out bodyleft, out bodyRight, out waistLength);
        //    }
        //}
    }
}
