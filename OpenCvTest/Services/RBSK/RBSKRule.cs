using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace AutomatedRodentTracker.Services.RBSK
{
    public class RBSKRule
    {
        private Func<PointF[], RBSKSettings, Image<Gray, Byte>, bool> m_Rule;

        public RBSKRule(Func<PointF[], RBSKSettings, Image<Gray, Byte>, bool> rule)
        {
            m_Rule = rule;
        }

        public bool CheckRule(PointF[] points, RBSKSettings settings, Image<Gray, Byte> binaryImage)
        {
            return m_Rule(points, settings, binaryImage);
        }
    }
}
