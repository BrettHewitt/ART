using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AutomatedRodentTracker.ModelInterface.Results;

namespace AutomatedRodentTracker.Model.Results
{
    internal class SingleFrameResult : ModelObjectBase, ISingleFrameResult
    {
        private PointF[] m_HeadPoints;
        private PointF m_HeadPoint;
        private PointF m_SmoothedHeadPoint;
        private Vector m_Orientation;
        private double m_CentroidSize;
        private double m_PelvicArea;
        private double m_Velocity;
        private double m_AngularVelocity;
        private double m_Distance;
        private double m_PelvicArea2;
        private double m_PelvicArea3;
        private double m_PelvicArea4;
        private PointF m_Centroid;
        private double m_CentroidVelocity;

        public PointF[] HeadPoints
        {
            get
            {
                return m_HeadPoints;
            }
            set
            {
                if (ReferenceEquals(m_HeadPoints, value))
                {
                    return;
                }

                m_HeadPoints = value;

                MarkAsDirty();
            }
        }

        public PointF HeadPoint
        {
            get
            {
                return m_HeadPoint;
            }
            set
            {
                if (Equals(m_HeadPoint, value))
                {
                    return;
                }

                m_HeadPoint = value;

                MarkAsDirty();
            }
        }

        public PointF SmoothedHeadPoint
        {
            get
            {
                return m_SmoothedHeadPoint;
            }
            set
            {
                if (Equals(m_SmoothedHeadPoint, value))
                {
                    return;
                }

                m_SmoothedHeadPoint = value;

                MarkAsDirty();
            }
        }

        public Vector Orientation
        {
            get
            {
                return m_Orientation;
            }
            set
            {
                if (Equals(m_Orientation, value))
                {
                    return;
                }

                m_Orientation = value;

                MarkAsDirty();
            }
        }

        public double CentroidSize
        {
            get
            {
                return m_CentroidSize;
            }
            set
            {
                if (Equals(m_CentroidSize, value))
                {
                    return;
                }

                m_CentroidSize = value;

                MarkAsDirty();
            }
        }

        public double PelvicArea
        {
            get
            {
                return m_PelvicArea;
            }
            set
            {
                if (Equals(m_PelvicArea, value))
                {
                    return;
                }

                m_PelvicArea = value;

                MarkAsDirty();
            }
        }

        public double PelvicArea2
        {
            get
            {
                return m_PelvicArea2;
            }
            set
            {
                if (Equals(m_PelvicArea2, value))
                {
                    return;
                }

                m_PelvicArea2 = value;

                MarkAsDirty();
            }
        }

        public double PelvicArea3
        {
            get
            {
                return m_PelvicArea3;
            }
            set
            {
                if (Equals(m_PelvicArea3, value))
                {
                    return;
                }

                m_PelvicArea3 = value;

                MarkAsDirty();
            }
        }

        public double PelvicArea4
        {
            get
            {
                return m_PelvicArea4;
            }
            set
            {
                if (Equals(m_PelvicArea4, value))
                {
                    return;
                }

                m_PelvicArea4 = value;

                MarkAsDirty();
            }
        }

        private double m_Dummy;
        public double Dummy
        {
            get
            {
                return m_Dummy;
            }
            set
            {
                if (Equals(m_Dummy, value))
                {
                    return;
                }

                m_Dummy = value;

                MarkAsDirty();
            }
        }


        public double Velocity
        {
            get
            {
                return m_Velocity;
            }
            set
            {
                if (Equals(m_Velocity, value))
                {
                    return;
                }

                m_Velocity = value;

                MarkAsDirty();
            }
        }

        public double AngularVelocity
        {
            get
            {
                return m_AngularVelocity;
            }
            set
            {
                if (Equals(m_AngularVelocity, value))
                {
                    return;
                }

                m_AngularVelocity = value;

                MarkAsDirty();
            }
        }

        public double Distance
        {
            get
            {
                return m_Distance;
            }
            set
            {
                if (Equals(m_Distance, value))
                {
                    return;
                }

                m_Distance = value;

                MarkAsDirty();
            }
        }

        public PointF Centroid
        {
            get
            {
                return m_Centroid;
            }
            set
            {
                if (Equals(m_Centroid, value))
                {
                    return;
                }

                m_Centroid = value;

                MarkAsDirty();
            }
        }

        public double CentroidVelocity
        {
            get
            {
                return m_CentroidVelocity;
            }
            set
            {
                if (Equals(m_CentroidVelocity, value))
                {
                    return;
                }

                m_CentroidVelocity = value;

                MarkAsDirty();
            }
        }
    }
}
