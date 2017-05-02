using System;
using AutomatedRodentTracker.ModelInterface;
using Emgu.CV;
using Emgu.CV.Structure;

namespace AutomatedRodentTracker.ModelInterface.Video
{
    public interface IVideo : IModelObjectBase, IDisposable
    {
        int FrameNumber
        {
            get;
        }

        int FrameCount
        {
            get;
        }

        double ElpasedTime
        {
            get;
        }

        double FrameRate
        {
            get;
        }

        double Width
        {
            get;
        }

        double Height
        {
            get;
        }

        string FilePath
        {
            get;
        }

        Mat GetFrameMat();
        Image<Bgr, Byte> GetFrameImage();
        Image<Gray, Byte> GetGrayFrameImage();

        void SetVideo(string filepath);
        void Reset();
        void SetFrame(int frameNumber);
        void SetFrame(double relativePosition);
    }
}
