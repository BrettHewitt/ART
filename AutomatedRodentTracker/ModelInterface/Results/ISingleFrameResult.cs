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

namespace AutomatedRodentTracker.ModelInterface.Results
{
    public interface ISingleFrameResult : IModelObjectBase
    {
        PointF[] HeadPoints
        {
            get;
            set;
        }

        PointF HeadPoint
        {
            get;
            set;
        }

        PointF SmoothedHeadPoint
        {
            get;
            set;
        }

        Vector Orientation
        {
            get;
            set;
        }

        double CentroidSize
        {
            get;
            set;
        }

        double PelvicArea
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

        double Dummy
        {
            get;
            set;
        }

        double Velocity
        {
            get;
            set;
        }

        double AngularVelocity
        {
            get;
            set;
        }

        double Distance
        {
            get;
            set;
        }

        double CentroidDistance
        {
            get;
            set;
        }

        PointF Centroid
        {
            get;
            set;
        }

        double CentroidVelocity
        {
            get;
            set;
        }
    }
}
