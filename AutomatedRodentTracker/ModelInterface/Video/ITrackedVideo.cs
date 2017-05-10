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

using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using AutomatedRodentTracker.Model.Results;
using AutomatedRodentTracker.ModelInterface.Behaviours;
using AutomatedRodentTracker.ModelInterface.Boundries;
using AutomatedRodentTracker.ModelInterface.Results;

namespace AutomatedRodentTracker.ModelInterface.Video
{
    public interface ITrackedVideo : IModelObjectBase
    {
        string FileName
        {
            get;
            set;
        }

        Dictionary<int, ISingleFrameResult> Results
        {
            get;
            set;
        }

        int StartFrame
        {
            get;
            set;
        }

        int EndFrame
        {
            get;
            set;
        }

        PointF[] MotionTrack
        {
            get;
            set;
        }

        PointF[] SmoothedMotionTrack
        {
            get;
            set;
        }

        Vector[] OrientationTrack
        {
            get;
            set;
        }

        IBoundaryBase[] Boundries
        {
            get;
            set;
        }

        IBehaviourHolder[] Events
        {
            get;
            set;
        }

        Dictionary<IBoundaryBase, IBehaviourHolder[]> InteractingBoundries
        {
            get;
            set;
        }

        double MinInteractionDistance
        {
            get;
            set;
        }

        double GapDistance
        {
            get;
            set;
        }

        int ThresholdValue
        {
            get;
            set;
        }

        int ThresholdValue2
        {
            get;
            set;
        }

        SingleFileResult Result
        {
            get;
            set;
        }

        string Message
        {
            get;
            set;
        }

        bool SmoothMotion
        {
            get;
            set;
        }

        double FrameRate
        {
            get;
            set;
        }

        double PelvicArea1
        {
            get;
            set;
        }

        double PelvicArea2
        {
            get;
            set;
        }

        double PelvicArea3
        {
            get;
            set;
        }

        double PelvicArea4
        {
            get;
            set;
        }

        double CentroidSize
        {
            get;
            set;
        }

        double UnitsToMilimeters
        {
            get;
            set;
        }

        void UpdateTrack();
    }
}
