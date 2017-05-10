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

using System.Drawing;
using System.Windows;

namespace AutomatedRodentTracker.Extensions
{
    public static class SpineExtension
    {
        public static PointF GetDistancePointFromBase(PointF[] spine, double distance, PointF offset = new PointF())
        {
            if (spine == null || spine.Length <= 1)
            {
                return PointF.Empty;
            }

            //double targetDistance = 0;

            if (!offset.IsEmpty)
            {
                
            }

            double distanceCounter = 0;
            for (int i = 1; i < spine.Length; i++)
            {
                PointF previousPoint = spine[i - 1];
                PointF currentPoint = spine[i];

                distanceCounter += previousPoint.Distance(currentPoint);

                if (distanceCounter > distance)
                {
                    //The point is somewhere along this line
                    Vector dir = new Vector(currentPoint.X - previousPoint.X, currentPoint.Y - previousPoint.Y);
                    dir.Normalize();
                    dir *= distance;
                    return new PointF((float) (previousPoint.X + dir.X), (float) (previousPoint.Y + dir.Y));
                }
            }

            return PointF.Empty;
        }

        public static bool GetPerpendicularLineDistanceFromBase(PointF[] spine, double distance, out PointF intersection, out Vector direction, PointF offset = new PointF())
        {
            intersection = PointF.Empty;
            direction = new Vector();

            if (spine == null || spine.Length <= 1)
            {
                return false;
            }

            double distanceCounter = 0;
            for (int i = 1; i < spine.Length; i++)
            {
                PointF previousPoint = spine[i - 1];
                PointF currentPoint = spine[i];

                double currentSegmentDistance = previousPoint.Distance(currentPoint);
                distanceCounter += currentSegmentDistance;

                if (distanceCounter > distance)
                {
                    //The point is somewhere along this line
                    distanceCounter -= currentSegmentDistance;

                    double delta = distance - distanceCounter;

                    Vector dir = new Vector(currentPoint.X - previousPoint.X, currentPoint.Y - previousPoint.Y);
                    dir.Normalize();
                    dir *= delta;
                    intersection = new PointF((float)(previousPoint.X + dir.X), (float)(previousPoint.Y + dir.Y));

                    //Create perpendicular Vector
                    direction = new Vector(dir.Y, -dir.X);
                    return true;
                }
            }

            return false;
        }

        public static PointF FindClosestPointOnSpine(PointF[] spine, PointF point)
        {
            if (spine == null || spine.Length <= 1)
            {
                return PointF.Empty;
            }

            double minDist = double.MaxValue;
            PointF bestPoint = PointF.Empty;
            for (int i = 1; i < spine.Length; i++)
            {
                PointF previousPoint = spine[i - 1];
                PointF currentPoint = spine[i];
                PointF closestPoint;
                double currentMinDist = MathExtension.MinDistanceFromLineToPoint(previousPoint, currentPoint, point, out closestPoint);

                if (currentMinDist < minDist)
                {
                    minDist = currentMinDist;
                    bestPoint = closestPoint;
                }
            }

            return bestPoint;
        }

        public static double FindDistanceAlongSpine(PointF[] spine, PointF point)
        {
            if (spine == null || spine.Length <= 1)
            {
                return -1;
            }

            double minDist = double.MaxValue;
            PointF bestPoint = PointF.Empty;
            PointF[] closestPoints = new PointF[2];
            double totalDist = 0;
            for (int i = 1; i < spine.Length; i++)
            {
                PointF previousPoint = spine[i - 1];
                PointF currentPoint = spine[i];
                PointF closestPoint;
                double currentMinDist = MathExtension.MinDistanceFromLineToPoint(previousPoint, currentPoint, point, out closestPoint);

                if (currentMinDist < minDist)
                {
                    minDist = currentMinDist;
                    bestPoint = closestPoint;
                    closestPoints[0] = previousPoint;
                    closestPoints[1] = currentPoint;

                }
            }

            for (int i = 1; i < spine.Length; i++)
            {
                PointF previousPoint = spine[i - 1];
                PointF currentPoint = spine[i];

                if (previousPoint == closestPoints[0])
                {
                    //We're on the line
                    totalDist += previousPoint.Distance(bestPoint);
                    break;
                }
                else
                {
                    //Not on this line
                    totalDist += previousPoint.Distance(currentPoint);
                }

            }

            return totalDist;
        }
    }
}
