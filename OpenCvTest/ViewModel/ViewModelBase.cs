using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutomatedRodentTracker.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private ViewModelState m_ViewModelState = ViewModelState.Clean;

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModelState ViewModelState
        {
            get
            {
                return m_ViewModelState;
            }
            private set
            {
                m_ViewModelState = value;
            }
        }

        protected void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void MarkAsDirty()
        {
            ViewModelState = ViewModelState.Dirty;
        }

        protected void MarkAsDirtyAndNotifyPropertyChange([CallerMemberName]string propertyName = "")
        {
            ViewModelState = ViewModelState.Dirty;
            NotifyPropertyChanged(propertyName);
        }

        protected void Initialise()
        {
            ViewModelState = ViewModelState.Clean;
        }
    }

    public enum ViewModelState
    {
        Clean,
        Dirty,
    }
}
