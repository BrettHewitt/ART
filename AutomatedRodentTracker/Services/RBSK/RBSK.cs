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
using AutomatedRodentTracker.Classes;
using AutomatedRodentTracker.Extensions;
using Emgu.CV;
using Emgu.CV.Structure;

namespace AutomatedRodentTracker.Services.RBSK
{
    public class RBSK
    {
        public RBSKProbability ProbabilityFunc
        {
            get;
            set;
        }

        public RBSKSettings Settings
        {
            get;
            set;
        }

        public RBSKRules Rules
        {
            get;
            set;
        }

        public RBSKRules AdvancedRules
        {
            get;
            set;
        }

        public RBSK(RBSKRules rules, RBSKSettings settings, RBSKProbability probability)
        {
            Settings = settings;
            Rules = rules;
            ProbabilityFunc = probability;
        }

        public RBSK(RBSKRules rules, RBSKRules advanedRules, RBSKSettings settings, RBSKProbability probability)
        {
            Settings = settings;
            Rules = rules;
            ProbabilityFunc = probability;
            AdvancedRules = advanedRules;
        }

        public List<PointF> FindKeyPoints(Point[] points, float offset = 0, bool removeDuplicates = true)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //Point[] allPoints = points.ToArray();
            List<PointF> result = new List<PointF>();
            List<PointF> visitedPoints = new List<PointF>();
            PointF currentPoint = points[0];

            if (removeDuplicates)
            {
                visitedPoints.Add(currentPoint);
            }

            double distanceCounter = 0;

            if (offset == 0)
            {
                result.Add(currentPoint);
            }
            else
            {
                distanceCounter += offset;
            }

            int pointCounter = 1;
            while (true)
            {
                Point targetPoint = points[pointCounter];

                double dist = currentPoint.Distance(targetPoint);

                if (distanceCounter + dist >= Settings.GapDistance)
                {
                    StraightLine line = new StraightLine(currentPoint, targetPoint);
                    double addOn = Settings.GapDistance - distanceCounter;
                    PointF keyPoint = line.DistanceFromStart(addOn);
                    result.Add(keyPoint);
                    currentPoint = keyPoint;
                    distanceCounter = 0;
                }
                else
                {
                    distanceCounter += dist;
                    currentPoint = points[pointCounter];
                    pointCounter++;

                    if (pointCounter == points.Length)
                    {
                        break;
                    }

                    if (removeDuplicates)
                    {
                        if (visitedPoints.Contains(points[pointCounter]))
                        {
                            break;
                        }
                        else
                        {
                            visitedPoints.Add(points[pointCounter]);
                        }
                    }
                }
            }

            //sw.Stop();
            //Console.WriteLine("FindKeyPoints: " + sw.ElapsedMilliseconds);
            return result;
        }

        public List<List<PointF>> FindKeyPoints(Point[] points, int numberOfSlides, bool removeDuplicates = true)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //Point[] allPoints = points.ToArray();
            List<List<PointF>> result = new List<List<PointF>>();

            if (points.Length <= 1)
            {
                return result;
            }

            int listCounter = 0;

            for (int i = 0; i <= numberOfSlides; i++)
            {
                result.Add(new List<PointF>());
            }

            List<PointF> visitedPoints = new List<PointF>();
            PointF currentPoint = points[0];

            if (removeDuplicates)
            {
                visitedPoints.Add(currentPoint);
            }

            double distanceCounter = 0;
            double offset = Settings.GapDistance/numberOfSlides;

            result[listCounter].Add(currentPoint);
            listCounter++;

            if (listCounter > numberOfSlides)
            {
                listCounter = 0;
            }
            
            int pointCounter = 0;
            int targetLimit = Settings.NumberOfPoints + 1;
            int limitCounter = 0;
            int? targetList = null;

            while (true)
            {
                Point targetPoint = points[pointCounter];

                double dist = currentPoint.Distance(targetPoint);

                if (distanceCounter + dist >= offset)
                {
                    StraightLine line = new StraightLine(currentPoint, targetPoint);
                    double addOn = offset - distanceCounter;
                    PointF keyPoint = line.DistanceFromStartFloat(addOn);
                    result[listCounter].Add(keyPoint);

                    if (listCounter == targetList)
                    {
                        limitCounter++;

                        if (limitCounter == targetLimit)
                        {
                            break;
                        }
                    }

                    listCounter++;

                    if (listCounter > numberOfSlides)
                    {
                        listCounter = 0;
                    }

                    currentPoint = keyPoint;
                    distanceCounter = 0;
                }
                else
                {
                    distanceCounter += dist;
                    currentPoint = points[pointCounter];
                    pointCounter++;

                    if (pointCounter == points.Length)
                    {
                        targetList = listCounter;
                        pointCounter -= points.Length;
                    }

                    if (removeDuplicates)
                    {
                        if (visitedPoints.Contains(points[pointCounter]))
                        {
                            break;
                        }
                        else
                        {
                            visitedPoints.Add(points[pointCounter]);
                        }
                    }
                }
            }

            //sw.Stop();
            //Console.WriteLine("FindKeyPoints: " + sw.ElapsedMilliseconds);
            return result;
        }

        public PointF[] FindPointsFromRules(List<PointF> pointsList, Image<Gray, Byte> binaryImage, ref double probability)
        {
            //List<PointF> pointsList = points;

            if (pointsList.Count < Settings.NumberOfPoints)
            {
                return null;
            }

            int leftDelta, rightDelta;

            if (Settings.NumberOfPoints%2 == 0)
            {
                //Even number
                leftDelta = (Settings.NumberOfPoints / 2) - 1;
                rightDelta = Settings.NumberOfPoints / 2;
            }
            else
            {
                //Odd number
                rightDelta = ((Settings.NumberOfPoints - 1) / 2);
                leftDelta = rightDelta;
            }

            probability = 0;
            PointF[] bestPoints = null;
            int pointsListCount = pointsList.Count;
            for (int i = leftDelta; i < pointsListCount - rightDelta; i++)
            {
                PointF[] currentPoints = pointsList.GetRange(i - leftDelta, Settings.NumberOfPoints).ToArray();

                if (Rules.CheckRulesAgainstPoints(currentPoints, Settings, binaryImage))
                {
                    //The rules have been validated against these set of points
                    //Check probability
                    double tempProbability = ProbabilityFunc.GetProbability(currentPoints, Settings);

                    //If this is the highest probability so far then store the points
                    if (tempProbability > probability)
                    {
                        probability = tempProbability;
                        bestPoints = currentPoints;
                    }
                }
            }

            //Return result
            return bestPoints;
        }

        public PointF[] FindPointsFromRules(List<PointF> pointsList, Image<Gray, Byte> binaryImage, PointF previousPoint, double movementThreshold, ref double probability)
        {
            //List<PointF> pointsList = points;

            if (pointsList.Count < Settings.NumberOfPoints)
            {
                return null;
            }

            int leftDelta, rightDelta;

            if (Settings.NumberOfPoints % 2 == 0)
            {
                //Even number
                leftDelta = (Settings.NumberOfPoints / 2) - 1;
                rightDelta = Settings.NumberOfPoints / 2;
            }
            else
            {
                //Odd number
                rightDelta = ((Settings.NumberOfPoints - 1) / 2);
                leftDelta = rightDelta;
            }

            probability = 0;
            PointF[] bestPoints = null;
            int pointsListCount = pointsList.Count;
            for (int i = leftDelta; i < pointsListCount - rightDelta; i++)
            {
                PointF[] currentPoints = pointsList.GetRange(i - leftDelta, Settings.NumberOfPoints).ToArray();

                if (Rules.CheckRulesAgainstPoints(currentPoints, Settings, binaryImage) && currentPoints[2].Distance(previousPoint) < movementThreshold)
                {
                    //The rules have been validated against these set of points
                    //Check probability
                    double tempProbability = ProbabilityFunc.GetProbability(currentPoints, Settings);

                    //If this is the highest probability so far then store the points
                    if (tempProbability > probability)
                    {
                        probability = tempProbability;
                        bestPoints = currentPoints;
                    }
                }
            }

            //Return result
            return bestPoints;
        }

        public bool CheckAdvancedRules(PointF[] points, Image<Gray, Byte> binaryImage)
        {
            return AdvancedRules.CheckRulesAgainstPoints(points, Settings, binaryImage);
        }
    }
}
