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
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;
using AutomatedRodentTracker.ModelInterface.Motion.BackgroundSubtraction;

namespace AutomatedRodentTracker.Model.Motion.BackgroundSubtraction
{
    internal class MotionBackgroundSubtraction : ModelObjectBase, IMotionBackgroundSubtraction
    {
        private BackgroundSubtractor m_ForegroundDetector;
        private Mat m_ForegroundMask;

        public BackgroundSubtractor ForegroundDetector
        {
            get
            {
                return m_ForegroundDetector;
            }
            set
            {
                if (Equals(m_ForegroundDetector, value))
                {
                    return;
                }

                m_ForegroundDetector = value;

                MarkAsDirty();
            }
        }

        public Mat ForegroundMask
        {
            get
            {
                return m_ForegroundMask;
            }
            private set
            {
                if (Equals(m_ForegroundMask, value))
                {
                    return;
                }

                if (m_ForegroundMask != null)
                {
                    m_ForegroundMask.Dispose();
                }

                m_ForegroundMask = value;

                MarkAsDirty();
            }
        }

        public MotionBackgroundSubtraction()
        {
            ForegroundDetector = new BackgroundSubtractorKNN(500, 16, false);
            ForegroundMask = new Mat();
        }

        public void UpdateDetector(int history = 500, float threshold = 16, bool shadowDetection = true)
        {
            ForegroundDetector = new BackgroundSubtractorKNN(history, threshold, shadowDetection);// new BackgroundSubtractorMOG2(history, threshold, shadowDetection);
        }

        public void ProcessFrame(IInputArray image)
        {
            ForegroundDetector.Apply(image, ForegroundMask);
        }

        public void Dispose()
        {
            if (ForegroundMask != null)
            {
                ForegroundMask.Dispose();
            }

            if (ForegroundDetector != null)
            {
                ForegroundDetector.Dispose();
            }
        }
    }
}
