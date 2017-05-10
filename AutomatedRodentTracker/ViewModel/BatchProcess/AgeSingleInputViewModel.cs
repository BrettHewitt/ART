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
