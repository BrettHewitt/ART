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

using System.Windows;
using System.Xml.Serialization;

namespace AutomatedRodentTracker.Model.XmlClasses
{
    public class VectorXml
    {
        [XmlElement(ElementName = "X")]
        public double X
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Y")]
        public double Y
        {
            get;
            set;
        }

        public VectorXml()
        {

        }

        public VectorXml(int x, int y)
        {
            X = x;
            Y = y;
        }

        public VectorXml(Vector vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        public Vector GetVector()
        {
            return new Vector(X, Y);
        }
    }
}
