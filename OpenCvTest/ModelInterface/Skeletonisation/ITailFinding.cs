using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace AutomatedRodentTracker.ModelInterface.Skeletonisation
{
    public interface ITailFinding : IModelObjectBase
    {
        //void FindTail(Point[] mouseContour, Point[] spine);
        void FindTail(Point[] mouseContour, PointF[] spine, Image<Bgr, Byte> drawImage, double width, PointF centroid, out List<Point> bodyPoints, out double waistLength, out double pelvicArea, out double pelvicArea2);
        //void FindTail(Point[] mouseContour, PointF[] spine, double width, out Point[] tail, out Point[] head, out Point[] bodyLeft, out Point[] bodyRight, out double wasitLength);
    }
}
