using System.Drawing;

namespace AutomatedRodentTracker.Services.Mouse
{
    public class CurrentBestPoint
    {
        private Point m_Point;
        private bool m_HasValue = false;

        public int WhiteCounter
        {
            get;
            set;
        }

        public int BlackCounter
        {
            get;
            set;
        }

        public bool HasValue
        {
            get
            {
                return m_HasValue;
            }
        }

        public Point Point
        {
            get
            {
                return m_Point;
            }
            set
            {
                m_Point = value;

                m_HasValue = true;
            }
        }

        public CurrentBestPoint(Point point)
        {
            Point = point;
        }

        public CurrentBestPoint ComparePoints(CurrentBestPoint testPoint)
        {
            if (BlackCounter > testPoint.BlackCounter)
            {
                return this;
            }

            if (BlackCounter < testPoint.BlackCounter)
            {
                return testPoint;
            }

            if (WhiteCounter >= testPoint.WhiteCounter)
            {
                return this;
            }
            else
            {
                return testPoint;
            }
        }
    }
}
