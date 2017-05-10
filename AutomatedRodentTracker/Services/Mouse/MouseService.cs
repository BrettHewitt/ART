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
using AutomatedRodentTracker.Extensions;
using AutomatedRodentTracker.Services.RBSK;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Point = System.Drawing.Point;

namespace AutomatedRodentTracker.Services.Mouse
{
    public class MouseService
    {
        public static Point FindMouseNoseTip(IEnumerable<Point> points, IEnumerable<Point> contourPoints)
        {
            List<Point> headPoints = points.ToList();
            List<Point> allContourPoints = contourPoints.ToList();

            StraightLine vectorPointsLine = new StraightLine(headPoints[1], headPoints[3]);
            PointF closestPointToNose;
            double distance = vectorPointsLine.FindDistanceToSegment(headPoints[2], out closestPointToNose);
            Point closestIntPoint = new Point((int)closestPointToNose.X, (int)closestPointToNose.Y);
            StraightLine direction = new StraightLine(closestIntPoint, headPoints[2]);

            float targetExtensionDistance = 3;
            Point targetLinePoint1 = new Point((int)(vectorPointsLine.StartPoint.X + (direction.NormalizedVector * distance * targetExtensionDistance).X), (int)(vectorPointsLine.StartPoint.Y + (direction.NormalizedVector * distance * targetExtensionDistance).Y));
            Point targetLinePoint2 = new Point((int)(vectorPointsLine.EndPoint.X + (direction.NormalizedVector * distance * targetExtensionDistance).X), (int)(vectorPointsLine.EndPoint.Y + (direction.NormalizedVector * distance * targetExtensionDistance).Y));

            StraightLine targetLine = new StraightLine(targetLinePoint1, targetLinePoint2);

            int numberOfTargets = 5;
            float offset = targetLine.GetMagnitude() / numberOfTargets;

            float finalDist = 10000;
            Point nosePoint = new Point();
            for (float currentTargetLength = 0; currentTargetLength <= targetLine.GetMagnitude(); currentTargetLength += offset)
            {
                Point currentTargetPoint = targetLine.DistanceFromStart(currentTargetLength);

                foreach (Point point in allContourPoints)
                {
                    float dist = currentTargetPoint.Distance(point);

                    if (dist < finalDist)
                    {
                        finalDist = dist;
                        nosePoint = point;
                    }
                }
            }

            return nosePoint;
        }

        public static AutomatedRodentTracker.Services.RBSK.RBSK GetStandardMouseRules()
        {
            RBSKRules mouseRules = new RBSKRules();

            //Always make sure the number of parameters is correct first
            mouseRules.AddRule(new RBSKRule((x, s, b) =>
            {
                if (x.Length != s.NumberOfPoints)
                {
                    return false;
                }

                return true;
            }));

            //Rules regarding the points layout
            mouseRules.AddRule(new RBSKRule((x, s, b) =>
            {
                PointF nosePoint = x[2];
                PointF[] vectorPoints = { x[1], x[3] };
                PointF[] rearPoints = { x[0], x[4] };

                double gapDistance = s.GapDistance;

                //Calculate target areas
                double gapDistanceSquared = gapDistance*gapDistance;
                double targetHeadArea = 0.433f * gapDistanceSquared;
                //float targetArea = gapDistanceSquared + targetHeadArea;

                //Check total area
                //float area = MathExtension.PolygonArea(x);
                //if (area < targetArea / 1.5)
                //{
                //    return false;
                //}

                //check head area
                float headArea = MathExtension.PolygonArea(new[] { vectorPoints[0], nosePoint, vectorPoints[1] });
                if (headArea < targetHeadArea / 2)
                {
                    return false;
                }

                //Check which side the points lie on
                PointSideVector result = MathExtension.FindSide(vectorPoints[0], vectorPoints[1], nosePoint);

                if (result == PointSideVector.On)
                {
                    return false;
                }

                PointSideVector rearResult1 = MathExtension.FindSide(vectorPoints[0], vectorPoints[1], rearPoints[0]);
                PointSideVector rearResult2 = MathExtension.FindSide(vectorPoints[0], vectorPoints[1], rearPoints[1]);

                //If the points are on the line then continue
                if (rearResult1 == PointSideVector.On || rearResult2 == PointSideVector.On)
                {
                    return false;
                }

                //Make sure the rear points are on the opposite side to the nose point
                if (!(rearResult1 == rearResult2 && rearResult1 != result))
                {
                    return false;
                }

                //Check rear points are correctly aligned with the center line
                PointF vectorMidPoint = new PointF((vectorPoints[0].X + vectorPoints[1].X)/2, (vectorPoints[0].Y + vectorPoints[1].Y)/2);

                rearResult1 = MathExtension.FindSide(nosePoint, vectorMidPoint, rearPoints[0]);
                rearResult2 = MathExtension.FindSide(nosePoint, vectorMidPoint, rearPoints[1]);

                if (rearResult1 == rearResult2)
                {
                    return false;
                }

                StraightLine centerLine = new StraightLine(nosePoint, vectorMidPoint);
                PointF targetRearMidPoint = centerLine.DistanceFromEndFloat(-s.GapDistance);
                centerLine = new StraightLine(nosePoint, targetRearMidPoint);

                double rearDist1 = centerLine.FindDistanceToSegment(rearPoints[0]);

                if (rearDist1 < s.GapDistance/3)
                {
                    return false;
                }

                rearDist1 = centerLine.FindDistanceToSegment(rearPoints[1]);

                if (rearDist1 < s.GapDistance/3)
                {
                    return false;
                }

                //The points are correctly aligned, make sure it's covering black pixels
                int blackCounter = 0;
                int whiteCounter = 0;
                StraightLine line = new StraightLine(vectorPoints[0], vectorPoints[1]);
                double distance = line.GetMagnitude();
                float increment = (float)distance / 10.0f;

                for (int j = 1; j < 9; j++)
                {
                    Point pointToCheck = line.DistanceFromStart(j * increment);
                    if (b[pointToCheck].Intensity <= 120)
                    {
                        blackCounter++;
                    }
                    else
                    {
                        whiteCounter++;
                    }
                }

                if (whiteCounter > blackCounter)
                {
                    return false;
                }

                //Don't want the distance to be too great between the vector points
                //double distance = vectorPoints[0].Distance(vectorPoints[1]);
                //double distance = Math.Sqrt((vectorPoints[1].X - vectorPoints[0].X) + (vectorPoints[1].Y - vectorPoints[0].Y));
                if (distance > 1.4f * gapDistance)
                {
                    return false;
                }

                //Rear distance must fall between a certain range
                double rearDistance = rearPoints[0].Distance(rearPoints[1]);
                if (rearDistance > 2.5f * gapDistance)
                {
                    return false;
                }

                if (rearDistance < 1.2*gapDistance)
                {
                    return false;
                }

                return true;
            }));

            RBSKRules advancedRules = new RBSKRules();
            advancedRules.AddRule(new RBSKRule((x, s, b) =>
            {
                Point intNosePoint = x[2].ToPoint();
                Point intVectorPoint1 = x[1].ToPoint();
                Point intVectorPoint2 = x[3].ToPoint();

                Image<Gray, Byte> mask = new Image<Gray, byte>(b.Cols, b.Rows, new Gray(0));
                mask.FillConvexPoly(new[] { intNosePoint, intVectorPoint1, intVectorPoint2 }, new Gray(255), LineType.FourConnected);

                double avgIntensity = b.GetAverage(mask).Intensity;

                if (avgIntensity > 10)
                {
                    return false;
                }

                return true;
            }));

            //Create probability funcation
            RBSKProbability rbskProbability = new RBSKProbability((p, s) =>
            {
                PointF[] points = p;

                if (points.Length != 5)
                {
                    return 0;
                }

                //float gapDistanceSquared = s.GapDistance*s.GapDistance;

                StraightLine line = new StraightLine(points[1], points[3]);

                //Need to normalize the line
                double targetNoseDistance = 0.866d * s.GapDistance;
                double actualNoseDistance = line.FindDistanceToSegment(points[2]);
                double noseProbability;

                if (targetNoseDistance > actualNoseDistance)
                {
                    noseProbability = actualNoseDistance / targetNoseDistance;
                }
                else
                {
                    noseProbability = targetNoseDistance / actualNoseDistance;
                }

                //float targetHeadArea = 0.433f * gapDistanceSquared;
                //float targetHeadArea = (float)MathExtension.PolygonArea(new[]
                //{
                //    new Point(0, 0),
                //    new Point((int)s.GapDistance, 0),
                //    new Point((int)(s.GapDistance/2), (int)(s.GapDistance*0.866f)), 
                //});
                //float actualHeadArea = MathExtension.PolygonArea(new[]
                //{
                //    points[1], points[2], points[3]
                //});
                //float areaProbability;

                //if (targetHeadArea > actualHeadArea)
                //{
                //    areaProbability = actualHeadArea / targetHeadArea;
                //}
                //else
                //{
                //    areaProbability = targetHeadArea / actualHeadArea;
                //}

                return noseProbability;// * areaProbability;
            });

            return new AutomatedRodentTracker.Services.RBSK.RBSK(mouseRules, advancedRules, new RBSKSettings(5), rbskProbability);
        }

        public static AutomatedRodentTracker.Services.RBSK.RBSK GetStandardMouseTailRules()
        {
            RBSKRules mouseRules = new RBSKRules();

            //Always make sure the number of parameters is correct first
            mouseRules.AddRule(new RBSKRule((x, s, b) =>
            {
                if (x.Length != s.NumberOfPoints)
                {
                    return false;
                }

                return true;
            }));

            //Rules regarding the points layout
            mouseRules.AddRule(new RBSKRule((x, s, b) =>
            {
                PointF nosePoint = x[2];
                PointF[] vectorPoints = { x[1], x[3] };
                PointF[] rearPoints = { x[0], x[4] };

                double gapDistance = s.GapDistance;

                //Calculate target areas
                double gapDistanceSquared = gapDistance * gapDistance;
                //float targetArea = gapDistanceSquared + targetHeadArea;

                //Check total area
                //float area = MathExtension.PolygonArea(x);
                //if (area < targetArea / 1.5)
                //{
                //    return false;
                //}

                //Check which side the points lie on
                PointSideVector result = MathExtension.FindSide(vectorPoints[0], vectorPoints[1], nosePoint);

                if (result == PointSideVector.On)
                {
                    return false;
                }

                PointSideVector rearResult1 = MathExtension.FindSide(vectorPoints[0], vectorPoints[1], rearPoints[0]);
                PointSideVector rearResult2 = MathExtension.FindSide(vectorPoints[0], vectorPoints[1], rearPoints[1]);

                //If the points are on the line then continue
                if (rearResult1 == PointSideVector.On || rearResult2 == PointSideVector.On)
                {
                    return false;
                }

                //Make sure the rear points are on the opposite side to the nose point
                if (!(rearResult1 == rearResult2 && rearResult1 != result))
                {
                    return false;
                }

                //Check rear points are correctly aligned with the center line
                PointF vectorMidPoint = new PointF((vectorPoints[0].X + vectorPoints[1].X) / 2, (vectorPoints[0].Y + vectorPoints[1].Y) / 2);

                rearResult1 = MathExtension.FindSide(nosePoint, vectorMidPoint, rearPoints[0]);
                rearResult2 = MathExtension.FindSide(nosePoint, vectorMidPoint, rearPoints[1]);

                if (rearResult1 == rearResult2)
                {
                    return false;
                }

                StraightLine centerLine = new StraightLine(nosePoint, vectorMidPoint);
                PointF targetRearMidPoint = centerLine.DistanceFromEndFloat(-s.GapDistance);
                centerLine = new StraightLine(nosePoint, targetRearMidPoint);

                double rearDist1 = centerLine.FindDistanceToSegment(rearPoints[0]);

                if (rearDist1 < s.GapDistance / 3)
                {
                    return false;
                }

                rearDist1 = centerLine.FindDistanceToSegment(rearPoints[1]);

                if (rearDist1 < s.GapDistance / 3)
                {
                    return false;
                }

                //The points are correctly aligned, make sure it's covering black pixels
                int blackCounter = 0;
                int whiteCounter = 0;
                StraightLine line = new StraightLine(vectorPoints[0], vectorPoints[1]);
                double distance = line.GetMagnitude();
                float increment = (float)distance / 10.0f;

                for (int j = 1; j < 9; j++)
                {
                    Point pointToCheck = line.DistanceFromStart(j * increment);
                    if (b[pointToCheck].Intensity <= 120)
                    {
                        blackCounter++;
                    }
                    else
                    {
                        whiteCounter++;
                    }
                }

                if (whiteCounter > blackCounter)
                {
                    return false;
                }

                //We want a small distance between vector points
                if (distance > 0.3f * gapDistance)
                {
                    return false;
                }

                //Rear distance must be small as well
                double rearDistance = rearPoints[0].Distance(rearPoints[1]);
                if (rearDistance > 0.6f * gapDistance)
                {
                    return false;
                }

                return true;
            }));

            RBSKRules advancedRules = new RBSKRules();
            advancedRules.AddRule(new RBSKRule((x, s, b) =>
            {
                Point intNosePoint = x[2].ToPoint();
                Point intVectorPoint1 = x[1].ToPoint();
                Point intVectorPoint2 = x[3].ToPoint();

                Image<Gray, Byte> mask = new Image<Gray, byte>(b.Cols, b.Rows, new Gray(0));
                mask.FillConvexPoly(new[] { intNosePoint, intVectorPoint1, intVectorPoint2 }, new Gray(255), LineType.FourConnected);

                double avgIntensity = b.GetAverage(mask).Intensity;

                if (avgIntensity > 10)
                {
                    return false;
                }

                return true;
            }));

            //Create probability funcation
            RBSKProbability rbskProbability = new RBSKProbability((p, s) =>
            {
                PointF[] points = p;

                if (points.Length != 5)
                {
                    return 0;
                }

                //float gapDistanceSquared = s.GapDistance*s.GapDistance;

                StraightLine line = new StraightLine(points[1], points[3]);

                //Need to normalize the line
                double targetNoseDistance = 0.866d * s.GapDistance;
                double actualNoseDistance = line.FindDistanceToSegment(points[2]);
                double noseProbability;

                if (targetNoseDistance > actualNoseDistance)
                {
                    noseProbability = actualNoseDistance / targetNoseDistance;
                }
                else
                {
                    noseProbability = targetNoseDistance / actualNoseDistance;
                }

                //float targetHeadArea = 0.433f * gapDistanceSquared;
                //float targetHeadArea = (float)MathExtension.PolygonArea(new[]
                //{
                //    new Point(0, 0),
                //    new Point((int)s.GapDistance, 0),
                //    new Point((int)(s.GapDistance/2), (int)(s.GapDistance*0.866f)), 
                //});
                //float actualHeadArea = MathExtension.PolygonArea(new[]
                //{
                //    points[1], points[2], points[3]
                //});
                //float areaProbability;

                //if (targetHeadArea > actualHeadArea)
                //{
                //    areaProbability = actualHeadArea / targetHeadArea;
                //}
                //else
                //{
                //    areaProbability = targetHeadArea / actualHeadArea;
                //}

                return noseProbability;// * areaProbability;
            });

            return new AutomatedRodentTracker.Services.RBSK.RBSK(mouseRules, advancedRules, new RBSKSettings(5), rbskProbability);
        }

        public static double GetOrientation(PointF[] points)
        {
            if (points.Length != 5)
            {
                throw new InvalidOperationException("Points Count Must Be Equal To 5");
            }

            PointF midPoint = new PointF((points[1].X + points[3].X)/2, (points[1].Y + points[3].Y)/2);
            return Math.Atan2(points[2].X - midPoint.X, points[2].Y - midPoint.Y);
        }

        public static double GetOrientation(PointF[] points, out PointF midPoint)
        {
            if (points.Length != 5)
            {
                throw new InvalidOperationException("Points Count Must Be Equal To 5");
            }

            midPoint = new PointF((points[1].X + points[3].X) / 2, (points[1].Y + points[3].Y) / 2);
            return Math.Atan2(points[2].X - midPoint.X, points[2].Y - midPoint.Y);
        }
    }
}
