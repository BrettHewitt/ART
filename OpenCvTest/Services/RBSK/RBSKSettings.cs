using Emgu.CV.CvEnum;

namespace AutomatedRodentTracker.Services.RBSK
{
    public class RBSKSettings
    {
        public int NumberOfPoints
        {
            get;
            set;
        }

        public double GapDistance
        {
            get;
            set;
        }

        public int NumberOfSlides
        {
            get;
            set;
        }

        public double Offset
        {
            get
            {
                return GapDistance / NumberOfSlides;
            }
        }

        public int BinaryThreshold
        {
            get;
            set;
        }

        public int FilterLevel
        {
            get;
            set;
        }

        public double CannyThreshold
        {
            get;
            set;
        }

        public double CannyThreshLinking
        {
            get;
            set;
        }

        public int CannyAperatureSize
        {
            get;
            set;
        }

        public ChainApproxMethod ChainApproxMethod
        {
            get;
            set;
        }

        public RetrType RetrievalType
        {
            get;
            set;
        }

        public double PolygonApproximationAccuracy
        {
            get;
            set;
        }

        public double MinimumPolygonArea
        {
            get;
            set;
        }

        public RBSKSettings(int numberOfPoints)
        {
            NumberOfPoints = numberOfPoints;
            NumberOfSlides = 20;
            GapDistance = 50;
            BinaryThreshold = 20;
            FilterLevel = 13;
            CannyThreshold = 50;
            CannyThreshLinking = 300;
            CannyAperatureSize = 3;

            ChainApproxMethod = ChainApproxMethod.ChainApproxSimple;
            RetrievalType = RetrType.List;

            PolygonApproximationAccuracy = 0.001d;
            MinimumPolygonArea = 10d;
        }
    }
}
