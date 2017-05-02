using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.ModelInterface;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;

namespace AutomatedRodentTracker.ModelInterface.Motion.BackgroundSubtraction
{
    public interface IMotionBackgroundSubtraction : IModelObjectBase, IDisposable
    {
        BackgroundSubtractor ForegroundDetector
        {
            get;
            set;
        }

        Mat ForegroundMask
        {
            get;
        }

        void UpdateDetector(int history = 500, float threshold = 16, bool shadowDetection = true);
        void ProcessFrame(IInputArray image);
    }
}
