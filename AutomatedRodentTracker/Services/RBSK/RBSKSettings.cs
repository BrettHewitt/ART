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

using Emgu.CV.CvEnum;

namespace AutomatedRodentTracker.Services.RBSK
{
    public class RBSKSettings
    {
        public int NumberOfPoints
        {
            get;
            set;
        }

        public double GapDistance
        {
            get;
            set;
        }

        public int NumberOfSlides
        {
            get;
            set;
        }

        public double Offset
        {
            get
            {
                return GapDistance / NumberOfSlides;
            }
        }

        public int BinaryThreshold
        {
            get;
            set;
        }

        public int FilterLevel
        {
            get;
            set;
        }

        public double CannyThreshold
        {
            get;
            set;
        }

        public double CannyThreshLinking
        {
            get;
            set;
        }

        public int CannyAperatureSize
        {
            get;
            set;
        }

        public ChainApproxMethod ChainApproxMethod
        {
            get;
            set;
        }

        public RetrType RetrievalType
        {
            get;
            set;
        }

        public double PolygonApproximationAccuracy
        {
            get;
            set;
        }

        public double MinimumPolygonArea
        {
            get;
            set;
        }

        public RBSKSettings(int numberOfPoints)
        {
            NumberOfPoints = numberOfPoints;
            NumberOfSlides = 20;
            GapDistance = 50;
            BinaryThreshold = 20;
            FilterLevel = 13;
            CannyThreshold = 50;
            CannyThreshLinking = 300;
            CannyAperatureSize = 3;

            ChainApproxMethod = ChainApproxMethod.ChainApproxSimple;
            RetrievalType = RetrType.List;

            PolygonApproximationAccuracy = 0.001d;
            MinimumPolygonArea = 10d;
        }
    }
}
