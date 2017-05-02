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
