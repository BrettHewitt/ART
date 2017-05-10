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
using System.Xml;
using System.Xml.Serialization;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.Model.XmlClasses;
using AutomatedRodentTracker.ModelInterface.Results;

namespace AutomatedRodentTracker.Model.Results
{
    public class SingleFrameResultXml
    {
        [XmlArray(ElementName = "HeadPoints")]
        [XmlArrayItem(ElementName = "Point")]
        public PointFXml[] HeadPoints
        {
            get;
            set;
        }

        [XmlElement(ElementName = "HeadPoint")]
        public PointFXml HeadPoint
        {
            get;
            set;
        }

        [XmlElement(ElementName = "SmoothedHeadPoint")]
        public PointFXml SmoothedHeadPoint
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Orientation")]
        public VectorXml Orientation
        {
            get;
            set;
        }

        [XmlElement(ElementName = "CentroidSize")]
        public double CentroidSize
        {
            get;
            set;
        }

        [XmlElement(ElementName = "PelvicArea")]
        public double PelvicArea
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Velocity")]
        public double Velocity
        {
            get;
            set;
        }

        [XmlElement(ElementName = "AngularVelocity")]
        public double AngularVelocity
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Distance")]
        public double Distance
        {
            get;
            set;
        }

        [XmlElement(ElementName = "PelvicArea2")]
        public double PelvicArea2
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Centroid")]
        public PointFXml Centroid
        {
            get;
            set;
        }

        [XmlElement(ElementName = "PelvicArea3")]
        public double PelvicArea3
        {
            get;
            set;
        }

        [XmlElement(ElementName = "PelvicArea4")]
        public double PelvicArea4
        {
            get;
            set;
        }

        public SingleFrameResultXml()
        {
            
        }

        public SingleFrameResultXml(ISingleFrameResult result)
        {
            HeadPoints = result.HeadPoints.Select(x => new PointFXml(x.X, x.Y)).ToArray();

            if (HeadPoints == null)
            {
                HeadPoint = null;
            }
            else
            {
                HeadPoint = HeadPoints[2];
            }

            SmoothedHeadPoint = new PointFXml(result.SmoothedHeadPoint);
            
            Orientation = new VectorXml(result.Orientation);
            CentroidSize = result.CentroidSize;
            PelvicArea = result.PelvicArea;
            Velocity = result.Velocity;
            AngularVelocity = result.AngularVelocity;
            Distance = result.Distance;
            PelvicArea2 = result.PelvicArea2;
            PelvicArea3 = result.PelvicArea3;
            PelvicArea4 = result.PelvicArea4;
            Centroid = new PointFXml(result.Centroid);
        }

        public ISingleFrameResult GetData()
        {
            ISingleFrameResult result = ModelResolver.Resolve<ISingleFrameResult>();

            if (HeadPoints != null)
            {
                result.HeadPoints = HeadPoints.Select(x => x.GetPoint()).ToArray();
            }
            else
            {
                result.HeadPoints = null;
            }

            if (HeadPoint != null)
            {
                result.HeadPoint = HeadPoint.GetPoint();
            }

            if (SmoothedHeadPoint != null)
            {
                result.SmoothedHeadPoint = SmoothedHeadPoint.GetPoint();
            }

            if (Orientation != null)
            {
                result.Orientation = Orientation.GetVector();
            }
            
            result.CentroidSize = CentroidSize;
            result.PelvicArea = PelvicArea;
            result.Velocity = Velocity;
            result.AngularVelocity = AngularVelocity;
            result.Distance = Distance;
            result.PelvicArea2 = PelvicArea2;
            result.PelvicArea3 = PelvicArea3;
            result.PelvicArea4 = PelvicArea4;

            if (Centroid != null)
            {
                result.Centroid = Centroid.GetPoint();
            }

            result.DataLoadComplete();
            return result;
        }
    }
}
