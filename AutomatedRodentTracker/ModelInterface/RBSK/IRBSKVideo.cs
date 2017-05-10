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
using AutomatedRodentTracker.Model.Events;
using AutomatedRodentTracker.ModelInterface;
using AutomatedRodentTracker.ModelInterface.Results;
using Emgu.CV;
using Emgu.CV.Structure;
using AutomatedRodentTracker.ModelInterface.Video;

namespace AutomatedRodentTracker.ModelInterface.RBSK
{
    public interface IRBSKVideo : IModelObjectBase, IDisposable
    {
        event RBSKVideoUpdateEventHandler ProgressUpdates;

        IVideo Video
        {
            get;
            set;
        }

        Dictionary<int, ISingleFrameResult> HeadPoints
        {
            get;
            set;
        }

        Dictionary<int, Tuple<PointF[], double>> SecondPassHeadPoints
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

        double MovementDelta
        {
            get;
            set;
        }

        bool Cancelled
        {
            get;
            set;
        }

        bool Paused
        {
            get;
            set;
        }

        Image<Gray, Byte> BackgroundImage
        {
            get;
            set;
        }

        Rectangle Roi
        {
            get;
            set;
        }

        void Process();
    }
}
