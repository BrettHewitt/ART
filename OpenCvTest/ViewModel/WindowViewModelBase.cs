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
