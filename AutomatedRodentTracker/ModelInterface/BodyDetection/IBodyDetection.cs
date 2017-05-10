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
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace AutomatedRodentTracker.ModelInterface.BodyDetection
{
    public interface IBodyDetection : IModelObjectBase
    {
        double ThresholdValue
        {
            get;
            set;
        }

        Image<Gray, Byte> BinaryBackground
        {
            get;
            set;
        }

        void GetBody(Image<Gray, Byte> frame, out PointF centroid);
        void GetBody(Image<Bgr, Byte> frame, out PointF centroid);

        void FindBody(Image<Gray, Byte> frame, out double waistLength, out double waistVolume, out double waistVolume2, out double waistVolume3, out double waistVolume4, out PointF centroid);
        //void FindBody(Image<Gray, Byte> binary, Image<Gray, Byte> bianryNot, PointF headPoint, PointF tailPoint, out double waistLength);
    }
}
