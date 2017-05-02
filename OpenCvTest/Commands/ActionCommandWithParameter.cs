using System;
using System.Windows.Input;

namespace AutomatedRodentTracker.Commands
{
    public class ActionCommandWithParameter : ICommand
    {
        private Action<object> m_ExecuteAction;
        private Func<bool> m_CanExecuteAction = () => true;

        public event EventHandler CanExecuteChanged;

        public Action<object> ExecuteAction
        {
            get
            {
                return m_ExecuteAction;
            }
            set
            {
                m_ExecuteAction = value;
            }
        }

        public Func<bool> CanExecuteAction
        {
            get
            {
                return m_CanExecuteAction;
            }
            set
            {
                m_CanExecuteAction = value;

                if (CanExecuteChanged != null)
                {
                    CanExecuteChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteAction();
        }

        public void Execute(object parameter)
        {
            ExecuteAction(parameter);
        }

        public void RaiseCanExecuteChangedNotification()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
