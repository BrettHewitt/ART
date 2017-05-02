using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace AutomatedRodentTracker.ModelInterface.BodyDetection
{
    public interface IBodyDetection : IModelObjectBase
    {
        double ThresholdValue
        {
            get;
            set;
        }

        Image<Gray, Byte> BinaryBackground
        {
            get;
            set;
        }

        void GetBody(Image<Gray, Byte> frame, out PointF centroid);
        void GetBody(Image<Bgr, Byte> frame, out PointF centroid);

        void FindBody(Image<Gray, Byte> frame, out double waistLength, out double waistVolume, out double waistVolume2, out double waistVolume3, out double waistVolume4, out PointF centroid);
        //void FindBody(Image<Gray, Byte> binary, Image<Gray, Byte> bianryNot, PointF headPoint, PointF tailPoint, out double waistLength);
    }
}
