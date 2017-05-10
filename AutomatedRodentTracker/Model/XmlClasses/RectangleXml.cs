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
using System.Xml.Serialization;

namespace AutomatedRodentTracker.Model.XmlClasses
{
    public class RectangleXml
    {
        [XmlElement(ElementName = "X")]
        public int X
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Y")]
        public int Y
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Width")]
        public int Width
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Height")]
        public int Height
        {
            get;
            set;
        }

        public RectangleXml()
        {
            
        }

        public RectangleXml(Rectangle roi)
        {
            X = roi.X;
            Y = roi.Y;
            Width = roi.Width;
            Height = roi.Height;
        }

        public RectangleXml(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rectangle GetRect()
        {
            return new Rectangle(X, Y, Width, Height);
        }
    }
}
