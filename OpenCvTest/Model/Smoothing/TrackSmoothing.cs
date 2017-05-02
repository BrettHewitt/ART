using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AutomatedRodentTracker.Extensions;
using AutomatedRodentTracker.ModelInterface.Smoothing;

namespace AutomatedRodentTracker.Model.Smoothing
{
    internal class TrackSmoothing : ModelObjectBase, ITrackSmoothing
    {
        public PointF[] SmoothTrack(PointF[] originalTrack, double smoothingFactor = 0.68)
        {
            int length = originalTrack.Length;
            PointF[] smoothedMotion = new PointF[length];
            smoothedMotion[0] = originalTrack[0];
            smoothedMotion[length - 1] = originalTrack[length - 1];

            for (int i = 1; i < length - 1; i++)
            {
                PointF prevPoint = originalTrack[i - 1];
                PointF nextPoint = originalTrack[i + 1];

                PointF midPoint = prevPoint.MidPoint(nextPoint);

                PointF currentPoint = originalTrack[i];
                Vector dir = new Vector(midPoint.X - currentPoint.X, midPoint.Y - currentPoint.Y);
                dir *= smoothingFactor;
                smoothedMotion[i] = new PointF((float)(currentPoint.X + dir.X), (float)(currentPoint.Y + dir.Y));
            }

            return smoothedMotion;
        }

        public double GetTrackLength(PointF[] track)
        {
            double dist = 0;
            int length = track.Length;

            for (int i = 1; i < length; i++)
            {
                PointF p1 = track[i - 1];
                PointF p2 = track[i];
                dist += p1.Distance(p2);
            }

            return dist;
        }
    }
}
