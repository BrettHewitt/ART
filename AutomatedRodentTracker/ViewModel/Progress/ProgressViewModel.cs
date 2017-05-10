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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AutomatedRodentTracker.Commands;

namespace AutomatedRodentTracker.ViewModel.Progress
{
    public class ProgressViewModel : WindowViewModelBase
    {
        private double m_Min = 0;
        private double m_Max = 1;
        private double m_Progress = 0;

        private ActionCommand m_CancelCommand;
        public event EventHandler CancelPressed;
        public event EventHandler WindowAboutToClose;
        public event EventHandler WindowClosingCancelled;

        public double Min
        {
            get
            {
                return m_Min;
            }
            set
            {
                if (Equals(m_Min, value))
                {
                    return;
                }

                m_Min = value;

                NotifyPropertyChanged();
            }
        }

        public double Max
        {
            get
            {
                return m_Max;
            }
            set
            {
                if (Equals(m_Max, value))
                {
                    return;
                }

                m_Max = value;

                NotifyPropertyChanged();
            }
        }

        public double ProgressValue
        {
            get
            {
                return m_Progress;
            }
            set
            {
                if (Equals(m_Progress, value))
                {
                    return;
                }

                m_Progress = value;

                NotifyPropertyChanged();

                if (ProgressValue >= 1)
                {
                    Close = true;
                }
            }
        }

        public ActionCommand CancelCommand
        {
            get
            {
                return m_CancelCommand ?? (m_CancelCommand = new ActionCommand()
                {
                    ExecuteAction = Cancel,
                });
            }
        }

        protected override void WindowClosing(CancelEventArgs closingEventArgs)
        {
            if ((ProgressValue / Max) < 1)
            {
                if (WindowAboutToClose != null)
                {
                    WindowAboutToClose(this, EventArgs.Empty);
                }

                var result = MessageBox.Show("Are you sure you want to cancel?", "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    closingEventArgs.Cancel = true;
                    Close = false;

                    if (WindowClosingCancelled != null)
                    {
                        WindowClosingCancelled(this, EventArgs.Empty);
                    }
                }
                else
                {
                    if (CancelPressed != null)
                    {
                        CancelPressed(this, EventArgs.Empty);
                    }
                }
            }
        }

        private void Cancel()
        {
            Close = true;
        }
    }
}
