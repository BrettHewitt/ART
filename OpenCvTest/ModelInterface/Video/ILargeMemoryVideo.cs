using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.ModelInterface;
using Emgu.CV;
using Emgu.CV.Structure;

namespace AutomatedRodentTracker.ModelInterface.Video
{
    public interface ILargeMemoryVideo : IModelObjectBase
    {
        Image<Bgr, Byte>[] Frames
        {
            get;
            set;
        }

        int FrameCount
        {
            get;
        }

        void LoadVideo(string filePath);
    }
}
