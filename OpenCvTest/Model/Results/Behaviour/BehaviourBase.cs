using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour;

namespace AutomatedRodentTracker.Model.Results.Behaviour
{
    internal abstract class BehaviourBase : ModelObjectBase, IBehaviourBase
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


        private string m_Name;
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                if (Equals(m_Name, value))
                {
                    return;
                }

                m_Name = value;

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

        protected BehaviourBase(string name, int id)
        {
            Name = name;
            Id = id;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
