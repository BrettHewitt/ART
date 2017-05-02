using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;

namespace AutomatedRodentTracker.Services.RBSK
{
    public class RBSKRules
    {
        private List<RBSKRule> m_Rules = new List<RBSKRule>();

        public RBSKRules()
        {
            //m_NumberOfPointsNeeded = numberOfPointsNeeded;
        }

        public void AddRule(RBSKRule rule)
        {
            m_Rules.Add(rule);
        }

        public void AddRules(IEnumerable<RBSKRule> rules)
        {
            foreach (RBSKRule rule in rules)
            {
                m_Rules.Add(rule);
            }
        }

        public void RemoveRule(RBSKRule rule)
        {
            m_Rules.Remove(rule);
        }

        public void ClearRules()
        {
            m_Rules.Clear();
        }

        public bool CheckRulesAgainstPoints(PointF[] points, RBSKSettings settings, Image<Gray, Byte> binaryImage)
        {
            return m_Rules.All(rule => rule.CheckRule(points, settings, binaryImage));
        }
    }
}
