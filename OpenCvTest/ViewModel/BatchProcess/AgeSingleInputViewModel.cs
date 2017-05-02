using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.Commands;
using AutomatedRodentTracker.ViewModel.Inputs;

namespace AutomatedRodentTracker.ViewModel.BatchProcess
{
    public class AgeSingleInputViewModel : SingleInputBase
    {
        private string m_Text;
        private ActionCommand m_OkCommand;
        private ActionCommand m_CancelCommand;
        
        public override string Title
        {
            get
            {
                return "Age";
            }
        }

        public override string LabelText
        {
            get
            {
                return "Age: ";
            }
        }

        public override string Text
        {
            get
            {
                return m_Text;
            }
            set
            {
                if (Equals(m_Text, value))
                {
                    return;
                }

                m_Text = value;

                OkCommand.RaiseCanExecuteChangedNotification();
                NotifyPropertyChanged();
            }
        }

        public int Age
        {
            get;
            set;
        }

        public override ActionCommand OkCommand
        {
            get
            {
                return m_OkCommand ?? (m_OkCommand = new ActionCommand()
                {
                    ExecuteAction = OkPressed,
                    CanExecuteAction = CanOk,
                });
            }
        }

        public override ActionCommand CancelCommand
        {
            get
            {
                return m_CancelCommand ?? (m_CancelCommand = new ActionCommand()
                {
                    ExecuteAction = CancelPressed,
                });
            }
        }

        private void OkPressed()
        {
            ExitResult = WindowExitResult.Ok;
            CloseWindow();
        }

        private bool CanOk()
        {
            int age;
            if (int.TryParse(Text, out age))
            {
                Age = age;
                return true;
            }

            return false;
        }

        private void CancelPressed()
        {
            ExitResult = WindowExitResult.Cancel;
            CloseWindow();
        }
    }
}
