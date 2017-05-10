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
using System.ComponentModel;
using AutomatedRodentTracker.Commands;

namespace AutomatedRodentTracker.ViewModel
{
    public abstract class WindowViewModelBase : ViewModelBase
    {
        private bool m_Close;
        private ActionCommandWithParameter m_ClosingCommand;
        private WindowExitResult m_ExitResult = WindowExitResult.Notset;

        public bool Close
        {
            get
            {
                return m_Close;
            }
            set
            {
                if (Equals(m_Close, value))
                {
                    return;
                }

                m_Close = value;

                NotifyPropertyChanged();
            }
        }

        public ActionCommandWithParameter ClosingCommand
        {
            get
            {
                return m_ClosingCommand ?? (m_ClosingCommand = new ActionCommandWithParameter()
                {
                    ExecuteAction = OnWindowClosing,
                });
            }
        }

        public WindowExitResult ExitResult
        {
            get
            {
                return m_ExitResult;
            }
            protected set
            {
                m_ExitResult = value;
            }
        }

        protected void CloseWindow()
        {
            Close = true;
        }

        private void OnWindowClosing(object closingEventArgs)
        {
            CancelEventArgs e = closingEventArgs as CancelEventArgs;

            if (e != null)
            {
                WindowClosing(e);
            }
        }

        protected virtual void WindowClosing(CancelEventArgs closingEventArgs)
        {

        }
    }

    public enum WindowExitResult
    {
        Notset,
        Ok,
        Cancel,
    }
}
