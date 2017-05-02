using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.ModelInterface;
using Emgu.CV;
using Emgu.CV.Structure;

namespace AutomatedRodentTracker.ModelInterface.Boundries
{
    public interface IGenerateBoundries : IModelObjectBase
    {
        void GetBoundries(Image<Gray, Byte> binaryBackground/*, Image<Gray, Byte> binaryMouse, out Point[] mousePoints*/, out List<Point[]> boundries, out List<Point[]> artefacts, out List<RotatedRect> boxes);
    }
}
