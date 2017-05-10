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

using System.Drawing;

namespace AutomatedRodentTracker.Services.Mouse
{
    public class CurrentBestPoint
    {
        private Point m_Point;
        private bool m_HasValue = false;

        public int WhiteCounter
        {
            get;
            set;
        }

        public int BlackCounter
        {
            get;
            set;
        }

        public bool HasValue
        {
            get
            {
                return m_HasValue;
            }
        }

        public Point Point
        {
            get
            {
                return m_Point;
            }
            set
            {
                m_Point = value;

                m_HasValue = true;
            }
        }

        public CurrentBestPoint(Point point)
        {
            Point = point;
        }

        public CurrentBestPoint ComparePoints(CurrentBestPoint testPoint)
        {
            if (BlackCounter > testPoint.BlackCounter)
            {
                return this;
            }

            if (BlackCounter < testPoint.BlackCounter)
            {
                return testPoint;
            }

            if (WhiteCounter >= testPoint.WhiteCounter)
            {
                return this;
            }
            else
            {
                return testPoint;
            }
        }
    }
}
