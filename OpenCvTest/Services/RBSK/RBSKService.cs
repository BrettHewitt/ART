using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using AutomatedRodentTracker.Classes;
using AutomatedRodentTracker.Comparers;
using AutomatedRodentTracker.Extensions;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace AutomatedRodentTracker.Services.RBSK
{
    public class RBSKService
    {
        /// <summary>
        /// Perform RBSK on a colour image
        /// </summary>
        /// <param name="image">A colour image</param>
        /// <param name="rbsk">The Rule Based Sliding Keypoints to perform</param>
        /// <returns>A set of points which matches the RBSK, returns null if no points are found</returns>
        public static PointF[] RBSK(Image<Bgr, Byte> image, AutomatedRodentTracker.Services.RBSK.RBSK rbsk)
        {
            //Perform conversions
            using (Image<Gray, Byte> grayImage = image.Convert<Gray, Byte>())
            using (Image<Gray, Byte> filteredImage = grayImage.SmoothMedian(rbsk.Settings.FilterLevel))
            using (Image<Gray, Byte> binaryImage = filteredImage.ThresholdBinary(new Gray(rbsk.Settings.BinaryThreshold), new Gray(255)))
            //using (Image<Gray, Byte> cannyImage = binaryImage.Canny(rbsk.Settings.CannyThreshold, rbsk.Settings.CannyThreshLinking))
            {
                return RBSK(binaryImage, rbsk);
            }
        }

        public static PointF[] RBSK(Image<Bgr, Byte> image, AutomatedRodentTracker.Services.RBSK.RBSK rbsk, PointF previousPoint, double movementThreshold)
        {
            //Perform conversions
            using (Image<Gray, Byte> grayImage = image.Convert<Gray, Byte>())
            using (Image<Gray, Byte> filteredImage = grayImage.SmoothMedian(rbsk.Settings.FilterLevel))
            using (Image<Gray, Byte> binaryImage = filteredImage.ThresholdBinary(new Gray(rbsk.Settings.BinaryThreshold), new Gray(255)))
            {
                return RBSK(binaryImage, rbsk, previousPoint, movementThreshold);
            }
        }

        public static PointF[] RBSK(Image<Gray, byte> image, AutomatedRodentTracker.Services.RBSK.RBSK rbsk, PointF previousPoint, double movementThreshold)
        {
            List<VectorOfPoint> convertedContours = ImageExtension.GetContours(image, rbsk.Settings.ChainApproxMethod, rbsk.Settings.RetrievalType, rbsk.Settings.PolygonApproximationAccuracy, rbsk.Settings.MinimumPolygonArea);

            //If we have no contours then don't perform RBSK and return
            if (convertedContours.Count <= 0)
            {
                return null;
            }

            PointF[] bestMatch = null;

            //Created sorted list to store results by probability
            HighestDoubleComparer comaprer = new HighestDoubleComparer();
            SortedList<double, RBSKParallelContainer> containers = new SortedList<double, RBSKParallelContainer>(comaprer);

            //Loop through all the contours
            foreach (VectorOfPoint contourSet in convertedContours)
            {
                Point[] currentContourSet = contourSet.ToArray();
                List<List<PointF>> allKeyPoints = rbsk.FindKeyPoints(currentContourSet, rbsk.Settings.NumberOfSlides, false);

                if (allKeyPoints.Count == 0)
                {
                    continue;
                }

                //For each contour we are going to generate keypoints for each slide
                for (int i = 0; i <= rbsk.Settings.NumberOfSlides; i++)
                {
                    //Generate keypoints
                    List<PointF> keyPoints = allKeyPoints[i];

                    double tempProbability = 0;

                    //Check rules against keypoints
                    PointF[] headPoints = rbsk.FindPointsFromRules(keyPoints, image, previousPoint, movementThreshold, ref tempProbability);

                    if (headPoints != null)
                    {
                        while (containers.ContainsKey(tempProbability))
                        {
                            tempProbability += 0.000000001;
                        }

                        containers.Add(tempProbability, new RBSKParallelContainer(headPoints, tempProbability, currentContourSet, keyPoints.ToArray()));
                    }
                }
            }

            foreach (var rbskContainer in containers)
            {
                RBSKParallelContainer container = rbskContainer.Value;
                //if (rbsk.CheckAdvancedRules(container.HeadPoints, image))
                //{
                    bestMatch = container.HeadPoints;
                    break;
                //}
            }

            return bestMatch;
        }

        /// <summary>
        /// Perform RBSK on an edge detected image;
        /// </summary>
        /// <param name="image">An edge detected image</param>
        /// <param name="rbsk">The Rule Based Sliding Keypoints to perform</param>
        /// <returns>A set of points which matches the RBSK, returns null if no points are found</returns>
        public static PointF[] RBSK(Image<Gray, byte> image, AutomatedRodentTracker.Services.RBSK.RBSK rbsk)
        {
            List<VectorOfPoint> convertedContours = ImageExtension.GetContours(image, rbsk.Settings.ChainApproxMethod, rbsk.Settings.RetrievalType, rbsk.Settings.PolygonApproximationAccuracy, rbsk.Settings.MinimumPolygonArea);

            //If we have no contours then don't perform RBSK and return
            if (convertedContours.Count <= 0)
            {
                return null;
            }

            PointF[] bestMatch = null;

            //Created sorted list to store results by probability
            HighestDoubleComparer comaprer = new HighestDoubleComparer();
            SortedList<double, RBSKParallelContainer> containers = new SortedList<double, RBSKParallelContainer>(comaprer);

            //Loop through all the contours
            foreach (VectorOfPoint contourSet in convertedContours)
            {
                Point[] currentContourSet = contourSet.ToArray();
                List<List<PointF>> allKeyPoints = rbsk.FindKeyPoints(currentContourSet, rbsk.Settings.NumberOfSlides, false);

                if (allKeyPoints.Count == 0)
                {
                    continue;
                }

                //For each contour we are going to generate keypoints for each slide
                for (int i = 0; i <= rbsk.Settings.NumberOfSlides; i++)
                {
                    //Generate keypoints
                    List<PointF> keyPoints = allKeyPoints[i];

                    double tempProbability = 0;

                    //Check rules against keypoints
                    PointF[] headPoints = rbsk.FindPointsFromRules(keyPoints, image, ref tempProbability);

                    if (headPoints != null)
                    {
                        while (containers.ContainsKey(tempProbability))
                        {
                            tempProbability += 0.000000001;
                        }

                        containers.Add(tempProbability, new RBSKParallelContainer(headPoints, tempProbability, currentContourSet, keyPoints.ToArray()));
                    }
                }
            }

            foreach (var rbskContainer in containers)
            {
                RBSKParallelContainer container = rbskContainer.Value;
                if (rbsk.CheckAdvancedRules(container.HeadPoints, image))
                {
                    bestMatch = container.HeadPoints;
                    break;
                }
            }

            return bestMatch;
        }

        /// <summary>
        /// Perform RBSK on an edge detected image;
        /// </summary>
        /// <param name="image">An edge detected image</param>
        /// <param name="rbsk">The Rule Based Sliding Keypoints to perform</param>
        /// <param name="contour">The contour the mouse points were found on</param>
        /// <returns>A set of points which matches the RBSK, returns null if no points are found</returns>
        public static PointF[] RBSKParallel(Image<Gray, Byte> image, AutomatedRodentTracker.Services.RBSK.RBSK rbsk, ref Point[] contour)
        {
            List<VectorOfPoint> convertedContours = ImageExtension.GetContours(image, rbsk.Settings.ChainApproxMethod, rbsk.Settings.RetrievalType, rbsk.Settings.PolygonApproximationAccuracy, rbsk.Settings.MinimumPolygonArea);

            //If we have no contours then don't perform RBSK and return
            if (convertedContours.Count <= 0)
            {
                return null;
            }

            //Created sorted list to store results by probability
            HighestDoubleComparer comaprer = new HighestDoubleComparer();
            SortedList<double, RBSKParallelContainer> tempContainer = new SortedList<double, RBSKParallelContainer>(comaprer);

            //Sorted list locker object
            object tempLocker = new object();

            //Loop through all the contours
            foreach (VectorOfPoint point in convertedContours)
            {
                Point[] currentContourSet = point.ToArray();
                List<List<PointF>> allKeyPoints = rbsk.FindKeyPoints(currentContourSet, rbsk.Settings.NumberOfSlides, false);

                //For each contour we are going to generate keypoints for each slide
                Parallel.For(0, rbsk.Settings.NumberOfSlides, (i, state) =>
                {
                    //Generate keypoints
                    List<PointF> keyPoints = allKeyPoints[i];

                    if (keyPoints.Count >= 5)
                    {
                        double tempProbability = 0;

                        //Check rules against keypoints
                        PointF[] headPoints = rbsk.FindPointsFromRules(keyPoints, image, ref tempProbability);

                        if (headPoints != null)
                        {
                            lock (tempLocker)
                            {
                                if (!tempContainer.ContainsKey(tempProbability))
                                {
                                    tempContainer.Add(tempProbability, new RBSKParallelContainer(headPoints, tempProbability, currentContourSet, keyPoints.ToArray()));
                                }
                            }
                        }
                    }
                });
            }

            //Loop through results starting with the highest probability, check it against advanced rules, return the first match (the one with highest probability
            foreach (var rbskContainer in tempContainer)
            {
                RBSKParallelContainer container = rbskContainer.Value;

                if (rbsk.CheckAdvancedRules(container.HeadPoints, image))
                {
                    contour = container.ContourSet;
                    return container.HeadPoints;
                }
            }

            return null;
        }

        /// <summary>
        /// Perform RBSK on an edge detected image;
        /// </summary>
        /// <param name="image">An edge detected image</param>
        /// <param name="rbsk">The Rule Based Sliding Keypoints to perform</param>
        /// <param name="contour">The contour on which the rules were valid</param>
        /// <returns>A set of points which matches the RBSK, returns null if no points are found</returns>
        public static PointF[] RBSK(Image<Gray, Byte> image, AutomatedRodentTracker.Services.RBSK.RBSK rbsk, ref Point[] contour)
        {
            List<VectorOfPoint> convertedContours = ImageExtension.GetContours(image, rbsk.Settings.ChainApproxMethod, rbsk.Settings.RetrievalType, rbsk.Settings.PolygonApproximationAccuracy, rbsk.Settings.MinimumPolygonArea);

            //If we have no contours then don't perform RBSK and return
            if (convertedContours.Count <= 0)
            {
                return null;
            }

            PointF[] bestMatch = null;

            //Created sorted list to store results by probability
            HighestDoubleComparer comaprer = new HighestDoubleComparer();
            SortedList<double, RBSKParallelContainer> containers = new SortedList<double, RBSKParallelContainer>(comaprer);

            //Loop through all the contours
            foreach (VectorOfPoint contourSet in convertedContours)
            {
                Point[] currentContourSet = contourSet.ToArray();
                List<List<PointF>> allKeyPoints = rbsk.FindKeyPoints(currentContourSet, rbsk.Settings.NumberOfSlides, false);
                
                if (allKeyPoints.Count == 0)
                {
                    continue;
                }
                
                //For each contour we are going to generate keypoints for each slide
                for (int i = 0; i <= rbsk.Settings.NumberOfSlides; i++)
                {
                    //Generate keypoints
                    List<PointF> keyPoints = allKeyPoints[i];

                    double tempProbability = 0;

                    //Check rules against keypoints
                    PointF[] headPoints = rbsk.FindPointsFromRules(keyPoints, image, ref tempProbability);

                    if (headPoints != null)
                    {
                        while (containers.ContainsKey(tempProbability))
                        {
                            tempProbability += 0.000000001;
                        }

                        containers.Add(tempProbability, new RBSKParallelContainer(headPoints, tempProbability, currentContourSet, keyPoints.ToArray()));
                    }
                }
            }

            foreach (var rbskContainer in containers)
            {
                RBSKParallelContainer container = rbskContainer.Value;
                if (rbsk.CheckAdvancedRules(container.HeadPoints, image))
                { 
                    bestMatch = container.HeadPoints;
                    contour = container.ContourSet;
                    break;
                }
            }

            return bestMatch;
        }
    }
}
