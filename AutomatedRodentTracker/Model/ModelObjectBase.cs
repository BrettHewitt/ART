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

using AutomatedRodentTracker.ModelInterface;

namespace AutomatedRodentTracker.Model
{
    internal abstract class ModelObjectBase : IModelObjectBase
    {
        private ModelObjectState m_ModelObjectState = ModelObjectState.New;

        public string ErrorMessage
        {
            get;
            set;
        }

        public ModelObjectState ModelObjectState
        {
            get
            {
                return m_ModelObjectState;
            }
            protected set
            {
                if (Equals(m_ModelObjectState, value))
                {
                    return;
                }

                m_ModelObjectState = value;
            }
        }

        public void MarkAsDirty()
        {
            if (ModelObjectState == ModelObjectState.Clean)
            {
                ModelObjectState = ModelObjectState.Dirty;
            }
        }

        public void DataLoadComplete()
        {
            ModelObjectState = ModelObjectState.Clean;
        }

        public void Commit()
        {
            switch (ModelObjectState)
            {
                case ModelObjectState.New:

                    //Insert
                    Insert();

                    break;

                case ModelObjectState.Dirty:

                    //Update
                    Update();

                    break;

                case ModelObjectState.Delete:

                    //Delete
                    Delete();

                    break;
            }
        }

        protected virtual void Delete() { }
        protected virtual void Update() { }

        protected virtual void Insert()
        {
            ModelObjectState = ModelObjectState.Clean;
        }

        public void MarkAsDeleted()
        {
            ModelObjectState = ModelObjectState.Delete;
        }

        protected void ErrorOccured(string error)
        {
            ModelObjectState = ModelObjectState.Error;
            ErrorMessage = error;
        }
    }

    public enum ModelObjectState
    {
        New,
        Clean,
        Dirty,
        Error,
        Delete,
    }
}
