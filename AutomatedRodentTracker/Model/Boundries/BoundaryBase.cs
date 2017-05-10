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
using System.Drawing;
using AutomatedRodentTracker.ModelInterface.Boundries;

namespace AutomatedRodentTracker.Model.Boundries
{
    internal abstract class BoundaryBase : ModelObjectBase, IBoundaryBase
    {
        private int m_Id;
        public int Id
        {
            get
            {
                return m_Id;
            }
            set
            {
                if (Equals(m_Id, value))
                {
                    return;
                }

                m_Id = value;

                MarkAsDirty();
            }
        }

        private Point[] m_Points;
        public Point[] Points
        {
            get
            {
                return m_Points;
            }
            set
            {
                if (Equals(m_Points, value))
                {
                    return;
                }

                m_Points = value;

                MarkAsDirty();
            }
        }

        protected BoundaryType Type
        {
            get;
            set;
        }

        protected BoundaryBase(BoundaryType type)
        {
            //Id = id;
            Type = type;
        }

        protected double FindDistanceSquaredToSegment(PointF pt, PointF p1, PointF p2)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return (dx * dx) + (dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                PointF closest = new PointF(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return (dx * dx) + (dy * dy);
        }

        public double GetMinimumDistance(PointF point)
        {
            double currentMin = double.MaxValue;

            for (int i = 0; i < Points.Length; i++)
            {
                int prevIndex = i - 1;
                if (prevIndex < 0)
                {
                    prevIndex = Points.Length - 1;
                }

                double min = FindDistanceSquaredToSegment(point, Points[prevIndex], Points[i]);
                if (min < currentMin)
                {
                    currentMin = min;
                }
            }

            return Math.Sqrt(currentMin);
        }

        protected enum BoundaryType
        {
            Box,
            Artefact,
            Circle,
            Outer,
        }

        public abstract BoundaryBaseXml GetData();
    }
}
