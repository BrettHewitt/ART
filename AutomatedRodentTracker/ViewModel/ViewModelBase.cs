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
