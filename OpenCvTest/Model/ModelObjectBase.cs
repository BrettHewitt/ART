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
