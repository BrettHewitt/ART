using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutomatedRodentTracker.ModelInterface.Results
{
    public interface ISingleFrameResult : IModelObjectBase
    {
        PointF[] HeadPoints
        {
            get;
            set;
        }

        PointF HeadPoint
        {
            get;
            set;
        }

        PointF SmoothedHeadPoint
        {
            get;
            set;
        }

        Vector Orientation
        {
            get;
            set;
        }

        double CentroidSize
        {
            get;
            set;
        }

        double PelvicArea
        {
            get;
            set;
        }

        double PelvicArea2
        {
            get;
            set;
        }

        double PelvicArea3
        {
            get;
            set;
        }

        double PelvicArea4
        {
            get;
            set;
        }

        double Dummy
        {
            get;
            set;
        }

        double Velocity
        {
            get;
            set;
        }

        double AngularVelocity
        {
            get;
            set;
        }

        double Distance
        {
            get;
            set;
        }

        PointF Centroid
        {
            get;
            set;
        }

        double CentroidVelocity
        {
            get;
            set;
        }
    }
}
