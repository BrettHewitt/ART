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
using AutomatedRodentTracker.Extensions;
using AutomatedRodentTracker.Model;
using AutomatedRodentTracker.Model.Events;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.ModelInterface.BodyDetection;
using AutomatedRodentTracker.Services.Mouse;
using AutomatedRodentTracker.Services.RBSK;
using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using AutomatedRodentTracker.ModelInterface.RBSK;
using AutomatedRodentTracker.ModelInterface.Results;
using AutomatedRodentTracker.ModelInterface.Video;
using AutomatedRodentTracker.Services.ImageToBitmapSource;
using Point = System.Drawing.Point;

namespace AutomatedRodentTracker.Model.RBSK
{
    internal class RBSKVideo : ModelObjectBase, IRBSKVideo
    {
        private IVideo m_Video;
        private Dictionary<int, ISingleFrameResult> m_HeadPoints;
        private Dictionary<int, Tuple<PointF[], double>> m_SecondPassHeadPoints;

        private double m_GapDistance;
        private bool m_Cancelled = false;
        private Image<Gray, Byte> m_BackgroundImage;
        private Rectangle m_Roi;

        public event RBSKVideoUpdateEventHandler ProgressUpdates;

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

                MarkAsDirty();
            }
        }

        public Dictionary<int, Tuple<PointF[], double>> SecondPassHeadPoints
        {
            get
            {
                return m_SecondPassHeadPoints;
            }
            set
            {
                if (ReferenceEquals(m_SecondPassHeadPoints, value))
                {
                    return;
                }

                m_SecondPassHeadPoints = value;

                MarkAsDirty();
            }
        }

        public double GapDistance
        {
            get
            {
                return m_GapDistance;
            }
            set
            {
                if (Equals(m_GapDistance, value))
                {
                    return;
                }

                m_GapDistance = value;

                MarkAsDirty();
            }
        }

        private int m_ThresholdValue;
        public int ThresholdValue
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

        private int m_ThresholdValue2;
        public int ThresholdValue2
        {
            get
            {
                return m_ThresholdValue2;
            }
            set
            {
                if (Equals(m_ThresholdValue2))
                {
                    return;
                }

                m_ThresholdValue2 = value;

                MarkAsDirty();
            }
        }

        private double m_MovementDelta;
        public double MovementDelta
        {
            get
            {
                return m_MovementDelta;
            }
            set
            {
                if (Equals(m_MovementDelta))
                {
                    return;
                }

                m_MovementDelta = value;

                MarkAsDirty();
            }
        }

        private object CalledLock = new object();
        public bool Cancelled
        {
            get
            {
                lock (CalledLock)
                {
                    return m_Cancelled;
                }
            }
            set
            {
                lock (CalledLock)
                {
                    if (Equals(m_Cancelled, value))
                    {
                        return;
                    }

                    m_Cancelled = value;
                }

                MarkAsDirty();
            }
        }

        private bool m_Paused;
        private object PausedLock = new object();
        public bool Paused
        {
            get
            {
                lock (PausedLock)
                {
                    return m_Paused;
                }
            }
            set
            {
                lock (PausedLock)
                {
                    if (Equals(m_Paused, value))
                    {
                        return;
                    }

                    m_Paused = value;
                }

                MarkAsDirty();
            }
        }

        public Image<Gray, Byte> BackgroundImage
        {
            get
            {
                return m_BackgroundImage;
            }
            set
            {
                if (ReferenceEquals(m_BackgroundImage, value))
                {
                    return;
                }

                m_BackgroundImage = value;

                MarkAsDirty();
            }
        }

        public Rectangle Roi
        {
            get
            {
                return m_Roi;
            }
            set
            {
                if (Equals(m_Roi, value))
                {
                    return;
                }

                m_Roi = value;

                MarkAsDirty();
            }
        }

        public RBSKVideo()
        {
            BlobDetector = new CvBlobDetector();
        }

        public void Process()
        {
            if (Video == null)
            {
                return;
            }

            HeadPoints = GenerateDictionary();
        }

        private Dictionary<int, ISingleFrameResult> GenerateDictionary()
        {
            //const int SecondBinaryThresold = 10;
            const double movementDelta = 15;

            Services.RBSK.RBSK rbsk = MouseService.GetStandardMouseRules();

            if (GapDistance == 0)
            {
                GapDistance = GetBestGapDistance(rbsk);
            }

            rbsk.Settings.GapDistance = GapDistance;
            rbsk.Settings.BinaryThreshold = ThresholdValue;

            Dictionary<int, ISingleFrameResult> results = new Dictionary<int, ISingleFrameResult>();
            Video.Reset();
            int counter = 0;

            int frameCount = Video.FrameCount;

            while (true)
            {
                if (Paused)
                {
                    continue;
                }

                if (Cancelled)
                {
                    break;
                }

                using (Image<Bgr, byte> frame = Video.GetFrameImage())
                {
                    if (frame == null)
                    {
                        break;
                    }

                    if (Roi != Rectangle.Empty)
                    {
                        frame.ROI = Roi;
                    }

                    double waist, waistArea, waistArea2, waistArea3, waistArea4;
                    PointF centroid;

                    //if (counter == 500)
                    //{
                    //    frame.ROI = new Rectangle(0,0,1,1);
                    //}

                    PointF[] headPoints = ProcessFrame(frame, rbsk, out waist, out waistArea, out waistArea2, out waistArea3, out waistArea4, out centroid, true);

                    if (Roi != Rectangle.Empty)
                    {
                        centroid.X += Roi.X;
                        centroid.Y += Roi.Y;

                        if (headPoints != null)
                        {
                            headPoints[0].X += Roi.X;
                            headPoints[0].Y += Roi.Y;
                            headPoints[1].X += Roi.X;
                            headPoints[1].Y += Roi.Y;
                            headPoints[2].X += Roi.X;
                            headPoints[2].Y += Roi.Y;
                            headPoints[3].X += Roi.X;
                            headPoints[3].Y += Roi.Y;
                            headPoints[4].X += Roi.X;
                            headPoints[4].Y += Roi.Y;
                        }
                    }

                    //Console.WriteLine(waist + " " + waistArea);
                    //if (headPoints != null)
                    //{
                    //    foreach (var point in headPoints)
                    //    {
                    //        frame.Draw(new CircleF(point, 2), new Bgr(Color.Yellow), 2);
                    //    }
                    //}

                    //ImageViewer.Show(frame);
                    ISingleFrameResult frameResult = ModelResolver.Resolve<ISingleFrameResult>();
                    frameResult.HeadPoints = headPoints;
                    frameResult.CentroidSize = waist;
                    frameResult.PelvicArea = waistArea;
                    frameResult.PelvicArea2 = waistArea2;
                    frameResult.PelvicArea3 = waistArea3;
                    frameResult.PelvicArea4 = waistArea4;
                    frameResult.Centroid = centroid;
                    results.Add(counter, frameResult);
                    counter++;

                    if (ProgressUpdates != null)
                    {
                        ProgressUpdates(this, new RBSKVideoUpdateEvent((double)counter/frameCount));
                    }
                }
            }

            if (Cancelled)
            {
                return results;
            }

            //Find the most confident run of head detections
            List<List<HeadPointHolder>> confidentPoints = new List<List<HeadPointHolder>>();
            PointF lastPoint = PointF.Empty;
            List<HeadPointHolder> currentList = new List<HeadPointHolder>();
            double currentTotalMovement = 0;
            for (int i = 0; i < results.Count; i++)
            {
                PointF[] headPoints = results[i].HeadPoints;

                if (headPoints == null)
                {
                    //No head found, use alternative methods
                    if (currentList.Count > 0)
                    {
                        confidentPoints.Add(currentList);
                    }
                    currentList = new List<HeadPointHolder>();
                    lastPoint = PointF.Empty;
                    currentTotalMovement = 0;
                    //Console.WriteLine("Head points are null " + i);
                    continue;
                }

                //Check against expected position
                PointF currentPoint = headPoints[2];

                if (lastPoint.IsEmpty)
                {
                    lastPoint = currentPoint;
                    currentList.Add(new HeadPointHolder(i, currentPoint, currentTotalMovement));
                    continue;
                }

                double distance = lastPoint.Distance(currentPoint);
                if (distance < movementDelta)
                {
                    //Acceptable
                    currentTotalMovement += distance;
                    currentList.Add(new HeadPointHolder(i, currentPoint, currentTotalMovement));
                    lastPoint = currentPoint;
                }
                else
                {
                    if (currentList.Count > 0)
                    {
                        confidentPoints.Add(currentList);
                    }
                    currentList = new List<HeadPointHolder>();
                    lastPoint = PointF.Empty;
                    currentTotalMovement = 0;
                    //Console.WriteLine("Outside of range " + i);
                }
            }

            if (currentList.Count > 0)
            {
                confidentPoints.Add(currentList);
            }

            if (confidentPoints.Count == 0)
            {
                return results;
            }

            //Find longest list with highest total movement
            List<HeadPointHolder> bestList = confidentPoints[0];
            //double currentMaxTraverse = bestList.Last().TotalDelta;
            foreach (List<HeadPointHolder> list in confidentPoints)
            {
                //double currentTotalDelta = list.Last().TotalDelta;
                if (list.Count > bestList.Count)
                {
                    //currentMaxTraverse = currentTotalDelta;
                    bestList = list;
                }
            }

            //We now have a confident set of headpoints
            int minIndex = bestList.Select(x => x.FrameNumber).Min();
            int maxIndex = bestList.Select(x => x.FrameNumber).Max();

            
            minIndex--;
            while (minIndex >= 0)
            {
                //Traverse backwards
                PointF[] lastPoints = results[minIndex + 1].HeadPoints;
                if (lastPoints != null)
                {
                    lastPoint = lastPoints[2];
                }
                else
                {
                    lastPoint = new PointF(-100, -100);
                }

                PointF[] headPoints = results[minIndex].HeadPoints;

                if (headPoints == null)
                {
                    //No head found, use alternative methods
                    int previousThreshold = rbsk.Settings.BinaryThreshold;
                    rbsk.Settings.BinaryThreshold = ThresholdValue2;
                    Video.SetFrame(minIndex);
                    PointF[] headPoints2 = null;
                    using (Image<Bgr, byte> frame = Video.GetFrameImage())
                    {
                        if (frame == null)
                        {
                            break;
                        }

                        if (Roi != Rectangle.Empty)
                        {
                            frame.ROI = Roi;
                        }

                        headPoints2 = ProcessFrame(frame, rbsk, lastPoint, movementDelta);
                    }

                    rbsk.Settings.BinaryThreshold = previousThreshold;

                    if (headPoints2 != null)
                    {
                        //We've got a good location
                        //double temp = results[minIndex].CentroidSize;
                        results[minIndex].HeadPoints = headPoints2;
                        //results[minIndex].CentroidSize = temp;
                        //results[minIndex] = new Tuple<PointF[], double>(headPoints2, temp);
                    }
                    else
                    {
                        for (int i = 0; i <= minIndex; i++)
                        {
                            if (results.ContainsKey(i))
                            {
                                //double temp = results[i].Item2;
                                //results[i] = new Tuple<PointF[], double>(null, temp);
                                results[i].HeadPoints = null;
                            }
                        }

                        break;
                    }

                    minIndex--;
                    continue;
                }

                //Check against expected position
                PointF currentPoint = headPoints[2];

                if (lastPoint.Distance(currentPoint) < movementDelta)
                {
                    //Good point
                }
                else
                {
                    //Wrong point, search for another rbsk that falls within range
                    //if (minIndex == 17 || minIndex == 16)
                    //{
                    //    Console.WriteLine("");
                    //}

                    Video.SetFrame(minIndex);
                    PointF[] headPoints2 = null;
                    using (Image<Bgr, byte> frame = Video.GetFrameImage())
                    {
                        if (frame == null)
                        {
                            break;
                        }

                        if (Roi != Rectangle.Empty)
                        {
                            frame.ROI = Roi;
                        }

                        headPoints2 = ProcessFrame(frame, rbsk, lastPoint, movementDelta);
                    }

                    if (headPoints2 != null)
                    {
                        //We've got a good location
                        //double temp = results[maxIndex].Item2;
                        //results[minIndex] = new Tuple<PointF[], double>(headPoints2, temp);
                        results[minIndex].HeadPoints = headPoints2;
                    }
                    else
                    {
                        //No other rbsk falls within range, use alternative methods
                        //Console.WriteLine("Need to use alternative methods");
                        for (int i = 0; i <= minIndex; i++)
                        {
                            if (results.ContainsKey(i))
                            {
                                //double temp = results[i].Item2;
                                //results[i] = new Tuple<PointF[], double>(null, temp);
                                results[i].HeadPoints = null;
                            }
                        }

                        break;
                    }
                }

                minIndex--;
            }

            maxIndex++;
            while (maxIndex < results.Count)
            {
                //Traverse backwards
                PointF[] lastPoints = results[maxIndex - 1].HeadPoints;
                if (lastPoints != null)
                {
                    lastPoint = lastPoints[2];
                }
                else
                {
                    lastPoint = new PointF(-100,-100);
                }

                PointF[] headPoints = results[maxIndex].HeadPoints;
                

                if (headPoints == null)
                {
                    //No head found, use alternative methods
                    int previousThreshold = rbsk.Settings.BinaryThreshold;
                    rbsk.Settings.BinaryThreshold = ThresholdValue2;
                    Video.SetFrame(maxIndex);
                    PointF[] headPoints2 = null;
                    using (Image<Bgr, byte> frame = Video.GetFrameImage())
                    {
                        if (frame == null)
                        {
                            break;
                        }

                        if (Roi != Rectangle.Empty)
                        {
                            frame.ROI = Roi;
                        }

                        headPoints2 = ProcessFrame(frame, rbsk, lastPoint, movementDelta);
                    }

                    rbsk.Settings.BinaryThreshold = previousThreshold;

                    if (headPoints2 != null)
                    {
                        //We've got a good location
                        //double temp = results[maxIndex].Item2;
                        //results[maxIndex] = new Tuple<PointF[], double>(headPoints2, temp);
                        results[maxIndex].HeadPoints = headPoints2;
                    }
                    else
                    {
                        int max = results.Keys.Max();
                        for (int i = maxIndex; i <= max; i++)
                        {
                            if (results.ContainsKey(i))
                            {
                                //double temp = results[i].Item2;
                                //results[i] = new Tuple<PointF[], double>(null, temp);
                                results[i].HeadPoints = null;
                            }
                        }

                        break;
                    }

                    maxIndex++;
                    continue;
                }

                //Check against expected position
                PointF currentPoint = headPoints[2];

                if (lastPoint.Distance(currentPoint) < movementDelta)
                {
                    //Good point
                }
                else
                {
                    //Wrong point, search for another rbsk that falls within range
                    Video.SetFrame(maxIndex);
                    PointF[] headPoints2 = null;
                    using (Image<Bgr, byte> frame = Video.GetFrameImage())
                    {
                        if (frame == null)
                        {
                            break;
                        }

                        if (Roi != Rectangle.Empty)
                        {
                            frame.ROI = Roi;
                        }

                        headPoints2 = ProcessFrame(frame, rbsk, lastPoint, movementDelta);
                    }

                    if (headPoints2 != null)
                    {
                        //We've got a good location
                        //double temp = results[maxIndex].Item2;
                        //results[maxIndex] = new Tuple<PointF[], double>(headPoints2, temp);
                        results[maxIndex].HeadPoints = headPoints2;
                    }
                    else
                    {
                        //No other rbsk falls within range, use alternative methods
                        //Console.WriteLine("Need to use alternative methods");
                        int max = results.Keys.Max();
                        for (int i = maxIndex; i <= max; i++)
                        {
                            if (results.ContainsKey(i))
                            {
                                //double temp = results[i].Item2;
                                //results[i] = new Tuple<PointF[], double>(null, temp);
                                results[i].HeadPoints = null;
                            }
                        }

                        break;
                    }
                }

                maxIndex++;
            }

            return results;
        }

        private double GetBestGapDistance(Services.RBSK.RBSK rbsk, Action<double> progressCallBack = null)
        {
            //Caluclate gap distnace if it hasn't been set
            //Auto Find the gap distance
            //Scan from 20 - 300, the range which gives us consistent results is the right one
            int start = 20;
            int end = 300;
            int interval = 1;
            Video.SetFrame(100);
            Image<Gray, Byte> firstFrame = Video.GetGrayFrameImage();
            Dictionary<int, PointF> nosePoints = new Dictionary<int, PointF>();

            for (int i = start; i <= end; i += interval)
            {
                if (Cancelled)
                {
                    return -1;
                }

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
                if (Cancelled)
                {
                    return -1;
                }

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

        private CvBlobDetector BlobDetector
        {
            get;
            set;
        }

        private PointF[] ProcessFrame(Image<Bgr, Byte> image, AutomatedRodentTracker.Services.RBSK.RBSK rbsk, out double waist, out double waistArea, out double waistArea2, out double waistArea3, out double waistArea4, out PointF centroid, bool useBackground = false)
        {
            //Rectangle roi = Rectangle.Empty;

            //if (image.IsROISet)
            //{
            //    roi = image.ROI;
            //    image.ROI = Rectangle.Empty;
            //}

            if (BackgroundImage != null && useBackground)
            {
                using (Image<Gray, Byte> grayImage = image.Convert<Gray, Byte>())
                using (Image<Gray, Byte> filteredImage = grayImage.SmoothMedian(rbsk.Settings.FilterLevel))
                using (Image<Gray, Byte> binaryImage = filteredImage.ThresholdBinary(new Gray(rbsk.Settings.BinaryThreshold), new Gray(255)))
                using (Image<Gray, Byte> backgroundNot = BackgroundImage.Not())
                using (Image<Gray, Byte> finalImage = binaryImage.Add(backgroundNot))
                //using (Image<Gray, Byte> finalImageNot = finalImage.Not())
                {
                    //ImageViewer.Show(finalImage);
                    PointF[] result = RBSKService.RBSK(finalImage, rbsk);

                    IBodyDetection bodyDetection = ModelResolver.Resolve<IBodyDetection>();
                    bodyDetection.BinaryBackground = BackgroundImage;
                    //bodyDetection.ThresholdValue = ThresholdValue;
                    
                    if (result == null)
                    {
                        waist = -1;
                        waistArea = -1;
                        waistArea2 = -1;
                        waistArea3 = -1;
                        waistArea4 = -1;
                        bodyDetection.GetBody(filteredImage, out centroid);
                        return null;
                    }

                    
                    //double vol2, vol3, vol4;
                    //image.

                    //if (!roi.IsEmpty)
                    //{
                    //    image.ROI = roi;
                    //}

                    //PointF dummy;
                    bodyDetection.FindBody(filteredImage, out waist, out waistArea, out waistArea2, out waistArea3, out waistArea4, out centroid);

                    //if (!centroid.IsEmpty)
                    //{
                    //    ImageViewer.Show(finalImage);
                    //    Console.WriteLine("Showing");
                    //}

                    //CvBlobs blobs = new CvBlobs();
                    //BlobDetector.Detect(finalImage, blobs);

                    //CvBlob mouseBlob = null;
                    //double maxArea = -1;
                    //foreach (var blob in blobs.Values)
                    //{
                    //    if (blob.Area > maxArea)
                    //    {
                    //        mouseBlob = blob;
                    //        maxArea = blob.Area;
                    //    }
                    //}
                    //waist = -1;
                    //waistArea = -1;
                    return result;
                }
            }

            waist = -1;
            waistArea = -1;
            waistArea2 = -1;
            waistArea3 = -1;
            waistArea4 = -1;
            centroid = PointF.Empty;
            return RBSKService.RBSK(image, rbsk);
        }

        private PointF[] ProcessFrame(Image<Bgr, Byte> image, AutomatedRodentTracker.Services.RBSK.RBSK rbsk, PointF previousPoint, double movementDelta, bool useBackground = false)
        {
            if (BackgroundImage != null && useBackground)
            {
                using (Image<Gray, Byte> grayImage = image.Convert<Gray, Byte>())
                using (Image<Gray, Byte> filteredImage = grayImage.SmoothMedian(rbsk.Settings.FilterLevel))
                using (Image<Gray, Byte> binaryImage = filteredImage.ThresholdBinary(new Gray(rbsk.Settings.BinaryThreshold), new Gray(255)))
                using (Image<Gray, Byte> backgroundNot = BackgroundImage.Not())
                using (Image<Gray, Byte> finalImage = binaryImage.Add(backgroundNot))
                {
                    PointF[] result = RBSKService.RBSK(finalImage, rbsk, previousPoint, movementDelta);
                    return result;
                }
            }


            return RBSKService.RBSK(image, rbsk, previousPoint, movementDelta);
        }

        public void Dispose()
        {
            if (BackgroundImage != null)
            {
                BackgroundImage.Dispose();
            }
        }

        private class HeadPointHolder
        {
            public int FrameNumber
            {
                get;
                set;
            }

            public PointF HeadPoint
            {
                get;
                set;
            }

            public double TotalDelta
            {
                get;
                set;
            }

            public HeadPointHolder(int frameNumber, PointF headPoint, double totalDelta)
            {
                FrameNumber = frameNumber;
                HeadPoint = headPoint;
                TotalDelta = totalDelta;
            }
        }
    }
}
