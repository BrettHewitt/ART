using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.BodyOption;

namespace AutomatedRodentTracker.Model.Results.Behaviour.BodyOption
{
    internal abstract class BodyOptionsBase : ModelObjectBase, IBodyOptionsBase
    {
        private string m_Name;
        private int m_Id;

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

        protected BodyOptionsBase(string name, int id)
        {
            Name = name;
            Id = id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            IBodyOptionsBase bodyOption = obj as IBodyOptionsBase;

            if (bodyOption == null)
            {
                return false;
            }

            return Equals(bodyOption);
        }

        public bool Equals(IBodyOptionsBase bodyOption)
        {
            return bodyOption.Id == Id;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
