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
using Emgu.CV;
using Emgu.CV.Structure;

namespace AutomatedRodentTracker.ModelInterface.Skeletonisation
{
    public interface ITailFinding : IModelObjectBase
    {
        //void FindTail(Point[] mouseContour, Point[] spine);
        void FindTail(Point[] mouseContour, PointF[] spine, Image<Bgr, Byte> drawImage, double width, PointF centroid, out List<Point> bodyPoints, out double waistLength, out double pelvicArea, out double pelvicArea2);
        //void FindTail(Point[] mouseContour, PointF[] spine, double width, out Point[] tail, out Point[] head, out Point[] bodyLeft, out Point[] bodyRight, out double wasitLength);
    }
}
