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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AutomatedRodentTracker.Classes;
using AutomatedRodentTracker.Commands;
using AutomatedRodentTracker.Extensions;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.ModelInterface.Boundries;
using AutomatedRodentTracker.ModelInterface.Results;
using AutomatedRodentTracker.ModelInterface.Skeletonisation;
using AutomatedRodentTracker.Services.FileBrowser;
using AutomatedRodentTracker.Services.Mouse;
using AutomatedRodentTracker.Services.RBSK;
using Emgu.CV.Structure;
using AutomatedRodentTracker.ModelInterface.Video;
using Emgu.CV;
using Emgu.CV.Cvb;
using AutomatedRodentTracker.ModelInterface.VideoSettings;
using AutomatedRodentTracker.Services.ImageToBitmapSource;
using Point = System.Drawing.Point;

namespace AutomatedRodentTracker.ViewModel.BodyDetection
{
    public class BodyDetectionViewModel : WindowViewModelBase, IDisposable
    {
        private ActionCommand m_PreviousCommand;
        private ActionCommand m_NextCommand;
        private ActionCommand m_SaveSkelCommand;

        public ActionCommand PreviousCommand
        {
            get
            {
                return m_PreviousCommand ?? (m_PreviousCommand = new ActionCommand()
                {
                    ExecuteAction = GetPreviousFrame,
                    CanExecuteAction = CanGetPreviousFrame,
                });
            }
        }

        public ActionCommand NextCommand
        {
            get
            {
                return m_NextCommand ?? (m_NextCommand = new ActionCommand()
                {
                    ExecuteAction = GetNextFrame,
                    CanExecuteAction = CanGetNextFrame,
                });
            }
        }

        public ActionCommand SaveSkelCommand
        {
            get
            {
                return m_SaveSkelCommand ?? (m_SaveSkelCommand = new ActionCommand()
                {
                    ExecuteAction = SaveSkel,
                });
            }
        }

        private int m_SliderValue = -1;
        private int m_SliderMaximum = 0;

        public int SliderValue
        {
            get
            {
                return m_SliderValue;
            }
            set
            {
                if (Equals(m_SliderValue, value))
                {
                    return;
                }

                m_SliderValue = value;

                NotifyPropertyChanged();
                UpdateFrameNumber();
                NextCommand.RaiseCanExecuteChangedNotification();
                PreviousCommand.RaiseCanExecuteChangedNotification();
            }
        }

        public int SliderMaximum
        {
            get
            {
                return m_SliderMaximum;
            }
            set
            {
                if (Equals(m_SliderMaximum, value))
                {
                    return;
                }

                m_SliderMaximum = value;

                NotifyPropertyChanged();
            }
        }

        private BitmapSource m_Image1;
        private BitmapSource m_Image2;
        private BitmapSource m_Image3;
        private BitmapSource m_Image4;
        private BitmapSource m_Image5;
        private BitmapSource m_Image6;

        public BitmapSource Image1
        {
            get
            {
                return m_Image1;
            }
            set
            {
                if (ReferenceEquals(m_Image1, value))
                {
                    return;
                }

                m_Image1 = value;

                NotifyPropertyChanged();
            }
        }

        public BitmapSource Image2
        {
            get
            {
                return m_Image2;
            }
            set
            {
                if (ReferenceEquals(m_Image2, value))
                {
                    return;
                }

                m_Image2 = value;

                NotifyPropertyChanged();
            }
        }

        public BitmapSource Image3
        {
            get
            {
                return m_Image3;
            }
            set
            {
                if (ReferenceEquals(m_Image3, value))
                {
                    return;
                }

                m_Image3 = value;

                NotifyPropertyChanged();
            }
        }

        public BitmapSource Image4
        {
            get
            {
                return m_Image4;
            }
            set
            {
                if (ReferenceEquals(m_Image4, value))
                {
                    return;
                }

                m_Image4 = value;

                NotifyPropertyChanged();
            }
        }

        public BitmapSource Image5
        {
            get
            {
                return m_Image5;
            }
            set
            {
                if (ReferenceEquals(m_Image5, value))
                {
                    return;
                }

                m_Image5 = value;

                NotifyPropertyChanged();
            }
        }

        public BitmapSource Image6
        {
            get
            {
                return m_Image6;
            }
            set
            {
                if (ReferenceEquals(m_Image6, value))
                {
                    return;
                }

                m_Image6 = value;

                NotifyPropertyChanged();
            }
        }

        private IVideo Video
        {
            get;
            set;
        }

        private IVideoSettings VideoSettings
        {
            get;
            set;
        }

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

                NotifyPropertyChanged();
            }
        }

        private CvBlobDetector BlobDetector
        {
            get;
            set;
        }

        private int ThresholdValue
        {
            get;
            set;
        }

        private RBSK RBSK
        {
            get;
            set;
        }

        public BodyDetectionViewModel(ITrackedVideo headPoints = null, Dictionary<int, ISingleFrameResult> results = null, string file = "")
        {
            ThresholdValue = headPoints.ThresholdValue;
            Video = ModelResolver.Resolve<IVideo>();
            VideoSettings = ModelResolver.Resolve<IVideoSettings>();
            BlobDetector = new CvBlobDetector();
            RBSK = MouseService.GetStandardMouseRules();
            HeadPoints = results;
            FileLocation = file;
            OpenVideo();
        }

        private void GetNextFrame()
        {
            SliderValue++;
        }

        private bool CanGetNextFrame()
        {
            return SliderValue < SliderMaximum - 1;
        }

        private void GetPreviousFrame()
        {
            SliderValue--;
        }

        private bool CanGetPreviousFrame()
        {
            return SliderValue > 0;
        }

        private Image<Gray, Byte> SkelImage
        {
            get;
            set;
        }

        private Dictionary<int, ISingleFrameResult> m_HeadPoints;
        public Dictionary<int, ISingleFrameResult> HeadPoints
        {
            get
            {
                return m_HeadPoints;
            }
            set
            {
                if (ReferenceEquals(m_HeadPoints, value))
                {
                    return;
                }

                m_HeadPoints = value;

                NotifyPropertyChanged();
            }
        }

        private void UpdateFrameNumber()
        {
            Video.SetFrame(SliderValue);

            using (Image<Bgr, Byte> orig = Video.GetFrameImage())
            using (Image<Gray, Byte> origGray = orig.Convert<Gray, Byte>())
            using (Image<Gray, Byte> binary = origGray.ThresholdBinary(new Gray(ThresholdValue), new Gray(255)))
            using (Image<Gray, Byte> subbed = BinaryBackground.AbsDiff(binary))
            {
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
                //LineSegment2DF[] targetPoints = null;

                Point[] mouseContour = mouseBlob.GetContour();

                orig.DrawPolyline(mouseContour, true, new Bgr(Color.Cyan));
                Image1 = ImageService.ToBitmapSource(orig);
                
                PointF[] result;
                if (HeadPoints != null)
                {
                    result = HeadPoints[SliderValue].HeadPoints;
                }
                else
                {
                    double prob = 0;
                    RBSK headRbsk = MouseService.GetStandardMouseRules();
                    headRbsk.Settings.GapDistance = 65;
                    headRbsk.Settings.BinaryThreshold = 20;

                    List<List<PointF>> allKeyPoints = headRbsk.FindKeyPoints(mouseContour, headRbsk.Settings.NumberOfSlides, false);
                    result = headRbsk.FindPointsFromRules(allKeyPoints[0], binary, ref prob);
                }

                if (result != null)
                {
                    using (Image<Bgr, Byte> test = orig.Clone())
                    {
                        foreach (var point in result)
                        {
                            test.Draw(new CircleF(point, 3), new Bgr(Color.Red), 3);
                        }

                        Image1 = ImageService.ToBitmapSource(test);
                    }
                }
                else
                {
                    return;
                }

                RotatedRect rotatedRect = CvInvoke.MinAreaRect(mouseContour.Select(x => new PointF(x.X, x.Y)).ToArray());
                //Console.WriteLine("Size: " + rotatedRect.Size);

                ISkeleton skel = ModelResolver.Resolve<ISkeleton>();
                Image<Gray, Byte> tempBinary = binary.Clone();
                
                System.Drawing.Rectangle rect = mouseBlob.BoundingBox;
                Image<Gray, Byte> binaryRoi = tempBinary.GetSubRect(rect);
                using (Image<Bgr, Byte> displayImage = subbed.Convert<Bgr, Byte>())
                
                using (Image<Gray, Byte> skelImage = skel.GetSkeleton(binaryRoi))
                using (Image<Bgr, Byte> drawImage = orig.Clone())
                using (Image<Bgr, Byte> tempImage2 = new Image<Bgr, byte>(drawImage.Size))
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
                    //spineFinder.RotatedRectangle = rotatedRect;
                    Image5 = ImageService.ToBitmapSource(skelImage);

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
                    Point topCorner = mouseBlob.BoundingBox.Location;
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

                    double waistLength;
                    double pelvicArea1, pelvicArea2;
                    tailFinding.FindTail(mouseContour, spineCornerCorrected, displayImage, rotatedWidth, mouseBlob.Centroid, out bodyPoints, out waistLength, out pelvicArea1, out pelvicArea2);
                    
                    Console.WriteLine(smallestAngle);
                    if (!tailPoint.IsEmpty)
                    {
                        drawImage.Draw(new CircleF(tailPoint, 4), new Bgr(Color.Red), 3);
                    }

                    if (bodyPoints != null && bodyPoints.Count > 0)
                    {
                        Point[] bPoints = bodyPoints.ToArray();
                        double volume = MathExtension.PolygonArea(bPoints);
                        Emgu.CV.Structure.Ellipse fittedEllipse = PointCollection.EllipseLeastSquareFitting(bPoints.Select(x => x.ToPointF()).ToArray());
                        //CvInvoke.Ellipse(drawImage, fittedEllipse.RotatedRect, new MCvScalar(0, 0, 255), 2);
                        
                        Console.WriteLine("Volume: " + volume + " - " + (fittedEllipse.RotatedRect.Size.Width*fittedEllipse.RotatedRect.Size.Height) + ", Waist Length: " + waistLength);

                        //Alter this to something better
                        if (MathExtension.PolygonArea(bPoints) > (rotatedRect.Size.Height*rotatedRect.Size.Width)/6 || true)
                        {
                            //tempImage2.FillConvexPoly(bPoints, new Bgr(Color.White));
                            tempImage2.DrawPolyline(bPoints, true, new Bgr(Color.White));
                            PointF centroid = MathExtension.FindCentroid(bPoints);
                            System.Drawing.Rectangle minRect;
                            Image<Gray, Byte> temp2 = new Image<Gray, byte>(tempImage2.Width + 2, tempImage2.Height + 2);
                            CvInvoke.FloodFill(tempImage2, temp2, centroid.ToPoint(), new MCvScalar(255, 255, 255), out minRect, new MCvScalar(5, 5, 5), new MCvScalar(5, 5, 5));

                            using (Image<Gray, Byte> nonZeroImage = tempImage2.Convert<Gray, Byte>())
                            {
                                int[] volume2 = nonZeroImage.CountNonzero();
                                Console.WriteLine("Volume2: " + volume2[0]);

                                //int tester = 9;
                                //using (Image<Gray, Byte> t1 = nonZeroImage.Erode(tester))
                                //using (Image<Gray, Byte> t2 = t1.Dilate(tester))
                                //using (Image<Gray, Byte> t3 = t2.Erode(tester))
                                //using (Image<Gray, Byte> t4 = t3.Dilate(tester))
                                //using (Image<Gray, Byte> t5 = t4.Erode(tester))
                                //using (Image<Gray, Byte> t6 = t5.Dilate(tester))
                                //using (Image<Gray, Byte> t7 = t6.Erode(tester))
                                //{
                                //    Image6 = ImageService.ToBitmapSource(t7);
                                //}
                            }

                            tempImage2.Draw(new CircleF(centroid, 2), new Bgr(Color.Blue), 2);
                            double distanceToSpine = double.MaxValue;
                            PointF p11 = PointF.Empty, p22 = PointF.Empty;
                            for (int i = 1; i < spineCornerCorrected.Length; i++)
                            {
                                PointF point1 = spineCornerCorrected[i - 1];
                                PointF point2 = spineCornerCorrected[i];

                                double cDist = MathExtension.MinDistanceFromLineToPoint(point1, point2, centroid);
                                if (cDist < distanceToSpine)
                                {
                                    p11 = point1;
                                    p22 = point2;
                                    distanceToSpine = cDist;
                                }
                            }

                            PointSideVector psv = MathExtension.FindSide(p11, p22, centroid);

                            if (psv == PointSideVector.Below)
                            {
                                distanceToSpine *= -1;
                            }

                            Console.WriteLine(distanceToSpine + ",");
                        }
                    }

                    for (int i = 1; i < spine.Length; i++)
                    {
                        PointF point1 = spine[i - 1];
                        PointF point2 = spine[i];

                        point1.X += topCorner.X;
                        point1.Y += topCorner.Y;
                        point2.X += topCorner.X;
                        point2.Y += topCorner.Y;

                        LineSegment2D line = new LineSegment2D(new Point((int)point1.X, (int)point1.Y), new Point((int)point2.X, (int)point2.Y));
                        drawImage.Draw(line, new Bgr(Color.Aqua), 2);
                        tempImage2.Draw(line, new Bgr(Color.Cyan), 2);
                    }

                    drawImage.Draw(new CircleF(mouseBlob.Centroid, 2), new Bgr(Color.Blue), 2);
                    Image3 = ImageService.ToBitmapSource(drawImage);

                    Image6 = ImageService.ToBitmapSource(tempImage2);

                    double rotatedRectArea = rotatedRect.Size.Width*rotatedRect.Size.Height;

                    if (rotatedRectArea < 75000)
                    {
                        //Console.WriteLine(rotatedRectArea);
                        //return;
                    }
                    else
                    {
                        //Console.WriteLine(rotatedRectArea);
                    }

                    double height = rotatedRect.Size.Height;
                    double width = rotatedRect.Size.Width;

                    //double angle = rotatedRect.Angle;
                    bool heightLong = height > width;

                    double halfLength;

                    PointF[] vertices = rotatedRect.GetVertices();

                    if (heightLong)
                    {
                        halfLength = height;
                    }
                    else
                    {
                        halfLength = width;
                    }

                    halfLength /= 2;

                    PointF[] sidePoints1 = new PointF[4], midPoints = new PointF[2];
                    PointF p1 = vertices[0], p2 = vertices[1], p3 = vertices[2], p4 = vertices[3];

                    double d1 = p1.DistanceSquared(p2);
                    double d2 = p2.DistanceSquared(p3);

                    if (d1 < d2)
                    {
                        //p1 and p2, p3 and p4 are side points
                        sidePoints1[0] = p1;
                        sidePoints1[1] = p2;
                        sidePoints1[2] = p4;
                        sidePoints1[3] = p3;

                        midPoints[0] = p1.MidPoint(p4);
                        midPoints[1] = p2.MidPoint(p3);
                    }
                    else
                    {
                        //p2 and p3, p1 and p4 are side points
                        sidePoints1[0] = p1;
                        sidePoints1[1] = p4;
                        sidePoints1[2] = p2;
                        sidePoints1[3] = p3;

                        midPoints[0] = p1.MidPoint(p2);
                        midPoints[1] = p3.MidPoint(p4);
                    }

                    PointF intersection1 = PointF.Empty;
                    PointF intersection2 = PointF.Empty;

                    using (Image<Gray, Byte> halfTest1 = origGray.CopyBlank())
                    using (Image<Gray, Byte> halfTest2 = origGray.CopyBlank())
                    {
                        Point[] rect1 = new Point[] { new Point((int)sidePoints1[0].X, (int)sidePoints1[0].Y), new Point((int)midPoints[0].X, (int)midPoints[0].Y), new Point((int)midPoints[1].X, (int)midPoints[1].Y), new Point((int)sidePoints1[1].X, (int)sidePoints1[1].Y) };
                        Point[] rect2 = new Point[] { new Point((int)sidePoints1[2].X, (int)sidePoints1[2].Y), new Point((int)midPoints[0].X, (int)midPoints[0].Y), new Point((int)midPoints[1].X, (int)midPoints[1].Y), new Point((int)sidePoints1[3].X, (int)sidePoints1[3].Y) };

                        if (MathExtension.PolygonContainsPoint(rect1, center))
                        {
                            //Rect 1 is head, look for line in r2

                        }
                        else if (MathExtension.PolygonContainsPoint(rect2, center))
                        {
                            //Rect 2 is head, look for line in r1

                        }
                        else
                        {
                            //Something has gone wrong
                        }

                        halfTest1.FillConvexPoly(rect1, new Gray(255));
                        halfTest2.FillConvexPoly(rect2, new Gray(255));
                        //Image5 = ImageService.ToBitmapSource(halfTest1);
                        //Image6 = ImageService.ToBitmapSource(halfTest2);

                        //binary.Copy(holder1, halfTest1);
                        //binary.Copy(holder2, halfTest2);
                        int count1, count2;

                        //using (Image<Gray, Byte> binaryInverse = subbed.Not())
                        using (Image<Gray, Byte> holder1 = subbed.Copy(halfTest1))
                        using (Image<Gray, Byte> holder2 = subbed.Copy(halfTest2))
                        {
                            //Image4 = ImageService.ToBitmapSource(subbed);
                            //Image5 = ImageService.ToBitmapSource(holder1);
                            //Image6 = ImageService.ToBitmapSource(holder2);

                            count1 = holder1.CountNonzero()[0];
                            count2 = holder2.CountNonzero()[0];
                        }

                        PointF qr1 = PointF.Empty, qr2 = PointF.Empty, qr3 = PointF.Empty, qr4 = PointF.Empty;
                        if (count1 > count2)
                        {
                            //holder 1 is head, holder 2 is rear
                            qr1 = sidePoints1[2];
                            qr2 = sidePoints1[2].MidPoint(midPoints[0]);
                            qr3 = sidePoints1[3].MidPoint(midPoints[1]);
                            qr4 = sidePoints1[3];
                        }
                        else if (count1 < count2)
                        {
                            //holder 2 is head, holder 1 is year
                            qr1 = sidePoints1[0];
                            qr2 = sidePoints1[0].MidPoint(midPoints[0]);
                            qr3 = sidePoints1[1].MidPoint(midPoints[1]);
                            qr4 = sidePoints1[1];
                        }

                        //fat line is qr2, qr3
                        PointF centerPoint = qr2.MidPoint(qr3);
                        PointF i1 = qr2;
                        PointF i2 = qr3;
                        intersection1 = MathExtension.PolygonLineIntersectionPoint(centerPoint, i1, mouseContour);
                        intersection2 = MathExtension.PolygonLineIntersectionPoint(centerPoint, i2, mouseContour);
                    }

                    double deltaX = halfLength * Math.Cos(rotatedRect.Angle * MathExtension.Deg2Rad);
                    double deltaY = halfLength * Math.Sin(rotatedRect.Angle * MathExtension.Deg2Rad);

                    const double scaleFactor = 0.25;

                    PointF newPoint = new PointF((float)(center.X - (deltaX * scaleFactor)), (float)(center.Y - (deltaY * scaleFactor)));
                    PointF intersectionPoint1 = PointF.Empty;
                    PointF intersectionPoint2 = PointF.Empty;
                    Point[] temp = null;
                    PointF[] headPoints = RBSKService.RBSKParallel(binary, MouseService.GetStandardMouseRules(), ref temp);
                    if (headPoints != null)
                    {
                        PointF tip = headPoints[2];
                        //targetPoints = new LineSegment2DF[3];
                        Point centerInt = new Point((int)newPoint.X, (int)newPoint.Y);
                        //targetPoints[0] = new LineSegment2DF(centerInt, new PointF(tip.X, tip.Y));
                        Vector forwardVec = new Vector(tip.X - newPoint.X, tip.Y - newPoint.Y);
                        Vector rotatedVec = new Vector(-forwardVec.Y, forwardVec.X);

                        PointF i1 = new PointF((float)(newPoint.X + (rotatedVec.X * 1)), (float)(newPoint.Y + (rotatedVec.Y * 1)));
                        PointF i2 = new PointF((float)(newPoint.X - (rotatedVec.X * 1)), (float)(newPoint.Y - (rotatedVec.Y * 1)));
                        //targetPoints[1] = new LineSegment2DF(centerInt, i1);
                        //targetPoints[2] = new LineSegment2DF(centerInt, i2);
                        intersectionPoint1 = MathExtension.PolygonLineIntersectionPoint(newPoint, i1, mouseContour);
                        intersectionPoint2 = MathExtension.PolygonLineIntersectionPoint(newPoint, i2, mouseContour);
                    }



                    //displayImage.Draw(mouseBlob.BoundingBox, new Bgr(Color.Red), 2);
                    displayImage.Draw(new CircleF(mouseBlob.Centroid, 3), new Bgr(Color.Blue), 2);
                    
                    displayImage.Draw(rotatedRect, new Bgr(Color.Yellow), 3);
                    //displayImage.Draw(mouseContour, new Bgr(Color.Aqua), 2);
                    //displayImage.FillConvexPoly(new Point[] { new Point((int)sidePoints1[0].X, (int)sidePoints1[0].Y), new Point((int)midPoints[0].X, (int)midPoints[0].Y), new Point((int)midPoints[1].X, (int)midPoints[1].Y), new Point((int)sidePoints1[1].X, (int)sidePoints1[1].Y) }, new Bgr(Color.Blue));
                    //if (targetPoints != null)
                    //{
                    //    displayImage.Draw(targetPoints[0], new Bgr(Color.Green), 2);
                    //    displayImage.Draw(targetPoints[1], new Bgr(Color.Green), 2);
                    //    displayImage.Draw(targetPoints[2], new Bgr(Color.Green), 2);
                    //}

                    //if (!intersection1.IsEmpty && !intersection2.IsEmpty)
                    //{
                    //    LineSegment2DF lineSegment = new LineSegment2DF(intersection1, intersection2);
                    //    displayImage.Draw(lineSegment, new Bgr(Color.MediumPurple), 4);
                    //    //Console.WriteLine(lineSegment.Length);
                    //}

                    //displayImage.Draw(new CircleF(newPoint, 4), new Bgr(Color.MediumPurple), 3);

                    //Console.WriteLine(rotatedRect.Angle);
                    Image4 = ImageService.ToBitmapSource(displayImage);
                }
            }
        }

        private double GetBestGapDistance(RBSK rbsk, Action<double> progressCallBack = null)
        {
            //Caluclate gap distnace if it hasn't been set
            //Auto Find the gap distance
            //Scan from 20 - 300, the range which gives us consistent results is the right one
            int start = 20;
            int end = 300;
            int interval = 1;
            Video.SetFrame(0);
            Image<Gray, Byte> firstFrame = Video.GetGrayFrameImage();
            Dictionary<int, PointF> nosePoints = new Dictionary<int, PointF>();

            for (int i = start; i <= end; i += interval)
            {
                using (Image<Gray, Byte> filteredImage = firstFrame.SmoothMedian(5))
                using (Image<Gray, Byte> binaryImage = filteredImage.ThresholdBinary(new Gray(30), new Gray(255)))
                {
                    rbsk.Settings.GapDistance = i;
                    Point[] temp = null;
                    PointF[] mousePoints = RBSKService.RBSKParallel(binaryImage, rbsk, ref temp);

                    if (mousePoints != null)
                    {
                        //We've found a set of points for this gap distance, store it
                        nosePoints.Add(i, mousePoints[2]);
                    }
                }
                double progressValue = ((i - start) / ((double)end - start)) * 100;
                if (progressCallBack != null)
                {
                    progressCallBack(progressValue);
                }
            }

            const double threshold = 20;
            PointF? previousPoint = null;
            List<int> currentSelection = new List<int>();
            int bestCounter = 0;
            double bestDistanceSoFar = -1;

            foreach (KeyValuePair<int, PointF> kvp in nosePoints)
            {
                PointF currentPoint = kvp.Value;

                //Do we have a value?
                if (previousPoint.HasValue)
                {
                    //Is the previous point within the threshold distance of the current point
                    if (currentPoint.Distance(previousPoint.Value) < threshold)
                    {
                        currentSelection.Add(kvp.Key);
                        previousPoint = currentPoint;
                    }
                    else
                    {
                        //We're not within the threshold, compare the current list to see if it's the best
                        if (currentSelection.Count > bestCounter)
                        {
                            bestCounter = currentSelection.Count;
                            bestDistanceSoFar = currentSelection.Average();
                        }

                        currentSelection.Clear();
                        previousPoint = null;
                    }
                }
                else
                {
                    previousPoint = currentPoint;
                }
            }

            if (currentSelection.Count > bestCounter)
            {
                bestDistanceSoFar = currentSelection.Average();
            }

            if (bestDistanceSoFar == -1)
            {
                bestDistanceSoFar = 100;
            }

            return bestDistanceSoFar;
        }

        private PointF[] ProcessFrame(Image<Bgr, Byte> image, RBSK rbsk, bool useBackground = false)
        {
            if (BinaryBackground != null && useBackground)
            {
                using (Image<Gray, Byte> grayImage = image.Convert<Gray, Byte>())
                using (Image<Gray, Byte> filteredImage = grayImage.SmoothMedian(rbsk.Settings.FilterLevel))
                using (Image<Gray, Byte> binaryImage = filteredImage.ThresholdBinary(new Gray(rbsk.Settings.BinaryThreshold), new Gray(255)))
                using (Image<Gray, Byte> backgroundNot = BinaryBackground.Not())
                using (Image<Gray, Byte> finalImage = binaryImage.Add(backgroundNot))
                {
                    PointF[] result = RBSKService.RBSK(finalImage, rbsk);
                    return result;
                }
            }


            return RBSKService.RBSK(image, rbsk);
        }

        private string FileLocation
        {
            get;
            set;
        }

        private void OpenVideo()
        {
            string fileLocation;
            if (string.IsNullOrWhiteSpace(FileLocation))
            {
                fileLocation = FileBrowser.BroseForVideoFiles();
            }
            else
            {
                fileLocation = FileLocation;
            }

            if (string.IsNullOrWhiteSpace(fileLocation))
            {
                return;
            }

            Video.SetVideo(fileLocation);
            VideoSettings.FileName = fileLocation;
            VideoSettings.ThresholdValue = ThresholdValue;
            //VideoSettings.
            SliderMaximum = Video.FrameCount;
            
            Image<Gray, Byte> binaryBackground;
            IEnumerable<IBoundaryBase> boundaries;
            VideoSettings.GeneratePreview(Video, out binaryBackground, out boundaries);

            BinaryBackground = binaryBackground;

            Image2 = ImageService.ToBitmapSource(BinaryBackground);

            SliderValue = 0;
        }

        public void Dispose()
        {
            if (BinaryBackground != null)
            {
                BinaryBackground.Dispose();
            }
        }

        private void SaveSkel()
        {
            if (SkelImage != null)
            {
                string fileLocation = FileBrowser.SaveFile("png|*.png");


                if (string.IsNullOrWhiteSpace(fileLocation))
                {
                    return;
                }

                SkelImage.Save(fileLocation);
            }
        }
    }
}
