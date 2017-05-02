using System.Drawing;

namespace AutomatedRodentTracker.Classes
{
    public class RBSKParallelContainer
    {
        public PointF[] HeadPoints
        {
            get;
            set;
        }

        public double Probability
        {
            get;
            set;
        }

        public Point[] ContourSet
        {
            get;
            set;
        }

        public PointF[] KeypointSet
        {
            get;
            set;
        }

        public RBSKParallelContainer(PointF[] headPoints, double probability, Point[] contourSet, PointF[] keyPointSet)
        {
            HeadPoints = headPoints;
            Probability = probability;
            ContourSet = contourSet;
            KeypointSet = keyPointSet;
        }

        public RBSKParallelContainer()
        {
            HeadPoints = null;
            Probability = 0;
        }
    }
}
