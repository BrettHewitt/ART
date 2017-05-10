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

using AutomatedRodentTracker.ModelInterface.Behaviours;
using AutomatedRodentTracker.ModelInterface.Boundries;

namespace AutomatedRodentTracker.Model.Behaviours
{
    internal class BehaviourHolder : ModelObjectBase, IBehaviourHolder
    {
        private IBoundaryBase m_Boundary;
        private InteractionBehaviour m_Interaction;
        private int m_FrameNumber;

        public IBoundaryBase Boundary
        {
            get
            {
                return m_Boundary;
            }
            set
            {
                if (Equals(m_Boundary, value))
                {
                    return;
                }

                m_Boundary = value;

                MarkAsDirty();
            }
        }

        public InteractionBehaviour Interaction
        {
            get
            {
                return m_Interaction;
            }
            set
            {
                if (Equals(m_Interaction, value))
                {
                    return;
                }

                m_Interaction = value;

                MarkAsDirty();
            }
        }

        public int FrameNumber
        {
            get
            {
                return m_FrameNumber;
            }
            set
            {
                if (Equals(m_FrameNumber, value))
                {
                    return;
                }

                m_FrameNumber = value;

                MarkAsDirty();
            }
        }

        private int m_StartFrame;
        public int StartFrame
        {
            get
            {
                return m_StartFrame;
            }
            set
            {
                if (Equals(m_StartFrame, value))
                {
                    return;
                }

                m_StartFrame = value;

                MarkAsDirty();
            }
        }

        private int m_EndFrame;
        public int EndFrame
        {
            get
            {
                return m_EndFrame;
            }
            set
            {
                if (Equals(m_EndFrame, value))
                {
                    return;
                }

                m_EndFrame = value;

                MarkAsDirty();
            }
        }

    }
}
