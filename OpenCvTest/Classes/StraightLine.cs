using System;
using System.Drawing;
using System.Windows;
using Point = System.Drawing.Point;

namespace AutomatedRodentTracker.Classes
{
    public class StraightLine
    {
        private PointF m_StartPoint;
        private PointF m_EndPoint;

        private float m_Gradient;
        private float m_C;

        private Vector m_NormalizedVector;

        private Func<float, float> Equation
        {
            get;
            set;
        }

        public PointF StartPoint
        {
            get
            {
                return m_StartPoint;
            }
            set
            {
                if (Equals(m_StartPoint, value))
                {
                    return;
                }

                m_StartPoint = value;

                CalculateLineEquation();
            }
        }

        public PointF EndPoint
        {
            get
            {
                return m_EndPoint;
            }
            set
            {
                if (Equals(m_EndPoint, value))
                {
                    return;
                }

                m_EndPoint = value;

                CalculateLineEquation();
            }
        }

        public float Gradient
        {
            get
            {
                return m_Gradient;
            }
            private set
            {
                m_Gradient = value;
            }
        }

        public float C
        {
            get
            {
                return m_C;
            }
            private set
            {
                m_C = value;
            }
        }

        public Vector NormalizedVector
        {
            get
            {
                return m_NormalizedVector;
            }
            private set
            {
                m_NormalizedVector = value;
            }
        }

        public StraightLine(PointF startPoint, PointF endPoint)
        {
            m_StartPoint = startPoint;
            m_EndPoint = endPoint;
            CalculateLineEquation();
        }

        public float Result(float xValue)
        {
            return Equation(xValue);
        }

        private void CalculateLineEquation()
        {
            //Calculate gradient
            float deltaY = EndPoint.Y - StartPoint.Y;
            float deltaX = EndPoint.X - StartPoint.X;

            NormalizedVector = new Vector(EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y);
            double distance = NormalizedVector.Length;
            NormalizedVector = new Vector(NormalizedVector.X / distance, NormalizedVector.Y / distance);

            if (deltaX == 0)
            {
                Equation = x => StartPoint.Y;
                return;
            }
            
            if (deltaY == 0)
            {
                Equation = x => StartPoint.X;
                return;
            }

            Gradient = deltaY/deltaX;

            //Caluclate C value
            C = StartPoint.Y - (Gradient*StartPoint.X);

            Equation = x => (x * Gradient + C);
        }

        public Point DistanceFromStart(double distance)
        {
            double x = StartPoint.X + (distance*NormalizedVector).X;
            double y = StartPoint.Y + (distance*NormalizedVector).Y;

            return new Point((int)x, (int)y);
        }

        public PointF DistanceFromStartFloat(double distance)
        {
            double x = StartPoint.X + (distance * NormalizedVector).X;
            double y = StartPoint.Y + (distance * NormalizedVector).Y;

            return new PointF((float)x, (float)y);
        }

        public PointF DistanceFromEndFloat(double distance)
        {
            double x = EndPoint.X - (distance * NormalizedVector).X;
            double y = EndPoint.Y - (distance * NormalizedVector).Y;

            return new PointF((float)x, (float)y);
        }

        public double FindDistanceToSegment(PointF pt)
        {
            PointF closestPoint;
            float dx = EndPoint.X - StartPoint.X;
            float dy = EndPoint.Y - StartPoint.Y;

            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closestPoint = StartPoint;
                dx = pt.X - StartPoint.X;
                dy = pt.Y - StartPoint.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - StartPoint.X) * dx + (pt.Y - StartPoint.Y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closestPoint = new PointF(StartPoint.X, StartPoint.Y);
                dx = pt.X - StartPoint.X;
                dy = pt.Y - StartPoint.Y;
            }
            else if (t > 1)
            {
                closestPoint = new PointF(EndPoint.X, EndPoint.Y);
                dx = pt.X - EndPoint.X;
                dy = pt.Y - EndPoint.Y;
            }
            else
            {
                closestPoint = new PointF(StartPoint.X + t * dx, StartPoint.Y + t * dy);
                dx = pt.X - closestPoint.X;
                dy = pt.Y - closestPoint.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }
        
        public double FindDistanceToSegment(PointF pt, out PointF closestPoint)
        {
            float dx = EndPoint.X - StartPoint.X;
            float dy = EndPoint.Y - StartPoint.Y;

            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closestPoint = StartPoint;
                dx = pt.X - StartPoint.X;
                dy = pt.Y - StartPoint.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - StartPoint.X) * dx + (pt.Y - StartPoint.Y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closestPoint = new PointF(StartPoint.X, StartPoint.Y);
                dx = pt.X - StartPoint.X;
                dy = pt.Y - StartPoint.Y;
            }
            else if (t > 1)
            {
                closestPoint = new PointF(EndPoint.X, EndPoint.Y);
                dx = pt.X - EndPoint.X;
                dy = pt.Y - EndPoint.Y;
            }
            else
            {
                closestPoint = new PointF(StartPoint.X + t * dx, StartPoint.Y + t * dy);
                dx = pt.X - closestPoint.X;
                dy = pt.Y - closestPoint.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public float GetMagnitude()
        {
            float dx = EndPoint.X - StartPoint.X;
            float dy = EndPoint.Y - StartPoint.Y;

            return (float)Math.Sqrt((dx*dx) + (dy*dy));
        }
    }
}
