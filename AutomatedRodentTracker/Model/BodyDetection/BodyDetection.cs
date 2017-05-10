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
using System.Windows;
using AutomatedRodentTracker.Extensions;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.ModelInterface.BodyDetection;
using AutomatedRodentTracker.ModelInterface.Skeletonisation;
using AutomatedRodentTracker.Services.Mouse;
using AutomatedRodentTracker.Services.RBSK;
using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Point = System.Drawing.Point;

namespace AutomatedRodentTracker.Model.BodyDetection
{
    internal class BodyDetection : ModelObjectBase, IBodyDetection
    {
        private Image<Gray, Byte> m_BinaryBackground;
        public Image<Gray, Byte> BinaryBackground
        {
            get
            {
                return m_BinaryBackground;
            }
            set
            {
                if (Equals(m_BinaryBackground, value))
                {
                    return;
                }

                if (m_BinaryBackground != null)
                {
                    m_BinaryBackground.Dispose();
                }

                m_BinaryBackground = value;

                MarkAsDirty();
            }
        }

        private CvBlobDetector BlobDetector
        {
            get;
            set;
        }

        private double m_ThresholdValue;
        public double ThresholdValue
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

        private Services.RBSK.RBSK RBSK
        {
            get;
            set;
        }

        private Image<Gray, Byte> SkelImage
        {
            get;
            set;
        }

        public BodyDetection()
        {
            ThresholdValue = 20;
            BlobDetector = new CvBlobDetector();
            RBSK = MouseService.GetStandardMouseRules();
        }

        public void GetBody(Image<Gray, Byte> filteredImage, out PointF centroid)
        {
            using (Image<Gray, Byte> binary = filteredImage.ThresholdBinary(new Gray(ThresholdValue), new Gray(255)))
            using (Image<Gray, Byte> backgroundNot = BinaryBackground.Not())
            using (Image<Gray, Byte> finalImage = binary.Add(backgroundNot))
            using (Image<Gray, Byte> subbed = finalImage.Not())
            {
                centroid = PointF.Empty;
                CvBlobs blobs = new CvBlobs();
                BlobDetector.Detect(subbed, blobs);

                CvBlob mouseBlob = null;
                double maxArea = -1;
                foreach (var blob in blobs.Values)
                {
                    if (blob.Area > maxArea)
                    {
                        mouseBlob = blob;
                        maxArea = blob.Area;
                    }
                }

                if (mouseBlob != null)
                {
                    centroid = mouseBlob.Centroid;
                }
            }
        }

        public void GetBody(Image<Bgr, Byte> frame, out PointF centroid)
        {
            using (Image<Gray, Byte> origGray = frame.Convert<Gray, Byte>())
            using (Image<Gray, Byte> filteredImage = origGray.SmoothMedian(13))
            using (Image<Gray, Byte> binary = filteredImage.ThresholdBinary(new Gray(ThresholdValue), new Gray(255)))
            using (Image<Gray, Byte> backgroundNot = BinaryBackground.Not())
            using (Image<Gray, Byte> finalImage = binary.Add(backgroundNot))
            using (Image<Gray, Byte> subbed = finalImage.Not())
            {
                centroid = PointF.Empty;
                CvBlobs blobs = new CvBlobs();
                BlobDetector.Detect(subbed, blobs);

                CvBlob mouseBlob = null;
                double maxArea = -1;
                foreach (var blob in blobs.Values)
                {
                    if (blob.Area > maxArea)
                    {
                        mouseBlob = blob;
                        maxArea = blob.Area;
                    }
                }

                if (mouseBlob != null)
                {
                    centroid = mouseBlob.Centroid;
                }
            }
        }

        public void FindBody(Image<Gray, Byte> filteredImage, out double waistLength, out double waistVolume, out double waistVolume2, out double waistVolume3, out double waistVolume4, out PointF centroid)
        {
            using (Image<Gray, Byte> binary = filteredImage.ThresholdBinary(new Gray(ThresholdValue), new Gray(255)))
            using (Image<Gray, Byte> backgroundNot = BinaryBackground.Not())
            using (Image<Gray, Byte> finalImage = binary.Add(backgroundNot))
            using (Image<Gray, Byte> subbed = finalImage.Not())
            {
                CvBlobs blobs = new CvBlobs();
                BlobDetector.Detect(subbed, blobs);

                CvBlob mouseBlob = null;
                double maxArea = -1;
                foreach (var blob in blobs.Values)
                {
                    if (blob.Area > maxArea)
                    {
                        mouseBlob = blob;
                        maxArea = blob.Area;
                    }
                }
                
                double gapDistance = 50;
                RBSK.Settings.GapDistance = gapDistance;
                
                centroid = mouseBlob.Centroid;

                waistLength = -1;
                waistVolume = -1;
                waistVolume2 = -1;
                waistVolume3 = -1;
                waistVolume4 = -1;
            }
        }
    }
}
