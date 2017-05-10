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
using System.Xml.Serialization;
using AutomatedRodentTracker.Model.XmlClasses;
using AutomatedRodentTracker.ModelInterface.Boundries;

namespace AutomatedRodentTracker.Model.Boundries
{
    [XmlInclude(typeof(BoxBoundaryXml))]
    [XmlInclude(typeof(ArtefactsBoundaryXml))]
    [XmlInclude(typeof(CircleBoundaryXml))]
    [XmlInclude(typeof(OuterBoundaryXml))]
    [Serializable]
    public abstract class BoundaryBaseXml
    {
        [XmlElement(ElementName = "ID")]
        public int Id
        {
            get;
            set;
        }

        [XmlArray(ElementName = "Points")]
        [XmlArrayItem(ElementName = "Point")]
        public PointXml[] Points
        {
            get;
            set;
        }

        protected BoundaryBaseXml()
        {
            
        }

        protected BoundaryBaseXml(int id, PointXml[] points)
        {
            Id = id;
            Points = points;
        }

        public abstract IBoundaryBase GetBoundary();
    }
}
