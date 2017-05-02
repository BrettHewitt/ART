using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedRodentTracker.ModelInterface.Smoothing
{
    public interface ITrackSmoothing : IModelObjectBase
    {
        PointF[] SmoothTrack(PointF[] originalTrack, double smoothingParameter = 0.68);
        double GetTrackLength(PointF[] track);
    }
}
