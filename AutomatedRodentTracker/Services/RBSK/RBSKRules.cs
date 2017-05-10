/*Automated Rodent Tracker - A program to automatically track rodents
Copyright(C) 2015 Brett Michael Hewitt

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see<http://www.gnu.org/licenses/>.*/

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
