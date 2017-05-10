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
