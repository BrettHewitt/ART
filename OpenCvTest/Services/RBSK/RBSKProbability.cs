using System;
using System.Drawing;

namespace AutomatedRodentTracker.Services.RBSK
{
    public class RBSKProbability
    {
        private Func<PointF[], RBSKSettings, double> m_ProbabilityFunc;

        public RBSKProbability(Func<PointF[], RBSKSettings, double> probabilityFunc)
        {
            m_ProbabilityFunc = probabilityFunc;
        }

        public double GetProbability(PointF[] points, RBSKSettings settings)
        {
            return m_ProbabilityFunc(points, settings);
        }
    }
}
