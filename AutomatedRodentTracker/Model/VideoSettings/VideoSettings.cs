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
using AutomatedRodentTracker.Model;
using AutomatedRodentTracker.Model.Resolver;
using Emgu.CV;
using Emgu.CV.Structure;
using AutomatedRodentTracker.ModelInterface.Boundries;
using AutomatedRodentTracker.ModelInterface.Motion.MotionBackground;
using AutomatedRodentTracker.ModelInterface.Video;
using AutomatedRodentTracker.ModelInterface.VideoSettings;

namespace AutomatedRodentTracker.Model.VideoSettings
{
    internal class VideoSettings : ModelObjectBase, IVideoSettings
    {
        private string m_FileName;
        private int m_ThresholdValue = 120;
        private int m_ThresholdValue2 = 10;
        private double m_GapDistance = 0;
        private double m_MaxThreshold = 255;
        private double m_MinimumInteractionDistance = 25;

        private Point[] m_MousePoints;
        private List<Point[]> m_Boundries, m_Artefacts;
        private List<RotatedRect> m_Boxes;
        private Rectangle m_Roi;

        public string FileName
        {
            get
            {
                return m_FileName;
            }
            set
            {
                if (Equals(m_FileName, value))
                {
                    return;
                }

                m_FileName = value;

                MarkAsDirty();
            }
        }

        public int ThresholdValue
        {
            get
            {
                return m_ThresholdValue;
            }
            set
            {
                if (Equals(m_ThresholdValue, value))
                {
                    return;
                }

                m_ThresholdValue = value;

                MarkAsDirty();
            }
        }

        public int ThresholdValue2
        {
            get
            {
                return m_ThresholdValue2;
            }
            set
            {
                if (Equals(m_ThresholdValue2, value))
                {
                    return;
                }

                m_ThresholdValue2 = value;

                MarkAsDirty();
            }
        }

        public double GapDistance
        {
            get
            {
                return m_GapDistance;
            }
            set
            {
                if (Equals(m_GapDistance, value))
                {
                    return;
                }

                m_GapDistance = value;

                MarkAsDirty();
            }
        }

        public double MaxThreshold
        {
            get
            {
                return m_MaxThreshold;
            }
            set
            {
                if (Equals(m_MaxThreshold, value))
                {
                    return;
                }

                m_MaxThreshold = value;

                MarkAsDirty();
            }
        }

        public double MinimumInteractionDistance
        {
            get
            {
                return m_MinimumInteractionDistance;
            }
            set
            {
                if (Equals(m_MinimumInteractionDistance, value))
                {
                    return;
                }

                m_MinimumInteractionDistance = value;

                MarkAsDirty();
            }
        }

        public Point[] MousePoints
        {
            get
            {
                return m_MousePoints;
            }
            set
            {
                if (ReferenceEquals(m_MousePoints, value))
                {
                    return;
                }

                m_MousePoints = value;

                MarkAsDirty();
            }
        }

        public List<Point[]> Artefacts
        {
            get
            {
                return m_Artefacts;
            }
            set
            {
                if (ReferenceEquals(m_Artefacts, value))
                {
                    return;
                }

                m_Artefacts = value;

                MarkAsDirty();
            }
        }

        public List<Point[]> Boundries
        {
            get
            {
                return m_Boundries;
            }
            set
            {
                if (ReferenceEquals(m_Boundries, value))
                {
                    return;
                }

                m_Boundries = value;

                MarkAsDirty();
            }
        }

        public List<RotatedRect> Boxes
        {
            get
            {
                return m_Boxes;
            }
            set
            {
                if (ReferenceEquals(m_Boxes, value))
                {
                    return;
                }

                m_Boxes = value;

                MarkAsDirty();
            }
        }

        public Rectangle Roi
        {
            get
            {
                return m_Roi;
            }
            set
            {
                if (Equals(m_Roi, value))
                {
                    return;
                }

                m_Roi = value;

                MarkAsDirty();
            }
        }

        public void GeneratePreview(IVideo video, out Image<Gray, Byte> binaryBackground, out IEnumerable<IBoundaryBase> boundaries, int motionLength = 100, int startFrame = 0)
        {
            IMotionBackground motionBackground = ModelResolver.Resolve<IMotionBackground>();
            motionBackground.SetVideo(video);
            motionBackground.MotionLength = motionLength;

            //Image<Gray, Byte> binaryMouse;
            motionBackground.GenerateMotionBackground(ThresholdValue, out binaryBackground, Roi, startFrame);
            //motionBackground.Video.Dispose();

            IGenerateBoundries generateBoundries = ModelResolver.Resolve<IGenerateBoundries>();

            //Point[] mousePoints;
            List<Point[]> boundries, artefacts;
            List<RotatedRect> boxes;
            generateBoundries.GetBoundries(binaryBackground, out boundries, out artefacts, out boxes);

            //MousePoints = mousePoints;
            Boundries = boundries;
            Artefacts = artefacts;
            Boxes = boxes;

            List<IBoundaryBase> boundaryList = new List<IBoundaryBase>();

            int boxId = 1;
            foreach (RotatedRect rect in Boxes)
            {
                IBoxBoundary boxModel = ModelResolver.Resolve<IBoxBoundary>();
                boxModel.Id = boxId;
                boxModel.Points = rect.GetVertices().Select(x => new Point((int)x.X, (int)x.Y)).ToArray();
                boundaryList.Add(boxModel);
                boxId++;
            }

            int artefactId = 1;
            foreach (Point[] points in Artefacts)
            {
                IArtefactsBoundary artefactModel = ModelResolver.Resolve<IArtefactsBoundary>();
                artefactModel.Id = artefactId;
                artefactModel.Points = points;
                boundaryList.Add(artefactModel);
                artefactId++;
            }

            int boundaryId = 1;
            foreach (Point[] points in Boundries)
            {
                IOuterBoundary outerModel = ModelResolver.Resolve<IOuterBoundary>();
                outerModel.Id = boundaryId;
                outerModel.Points = points;
                boundaryList.Add(outerModel);
                boundaryId++;
            }

            boundaries = boundaryList;
        }
    }
}
