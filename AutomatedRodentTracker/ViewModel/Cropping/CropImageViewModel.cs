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
using System.Windows.Media.Imaging;
using AutomatedRodentTracker.Commands;
using AutomatedRodentTracker.ViewModel.Interfaces;

namespace AutomatedRodentTracker.ViewModel.Cropping
{
    public class CropImageViewModel : WindowViewModelBase
    {
        private BitmapSource m_DisplayImage;
        private double m_X;
        private double m_Y;
        private double m_Width;
        private double m_Height;
        private ActionCommand m_OkCommand;
        private ActionCommand m_CancelCommand;

        public ActionCommand OkCommand
        {
            get
            {
                return m_OkCommand ?? (m_OkCommand = new ActionCommand()
                {
                    ExecuteAction = Ok,
                });
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

        public BitmapSource DisplayImage
        {
            get
            {
                return m_DisplayImage;
            }
            set
            {
                if (ReferenceEquals(m_DisplayImage, value))
                {
                    return;
                }

                m_DisplayImage = value;

                NotifyPropertyChanged();
            }
        }

        public double X
        {
            get
            {
                return m_X;
            }
            set
            {
                if (Equals(m_X, value))
                {
                    return;
                }

                m_X = value;

                NotifyPropertyChanged();
            }
        }

        public double Y
        {
            get
            {
                return m_Y;
            }
            set
            {
                if (Equals(m_Y, value))
                {
                    return;
                }

                m_Y = value;

                NotifyPropertyChanged();
            }
        }

        public double Width
        {
            get
            {
                return m_Width;
            }
            set
            {
                if (Equals(m_Width, value))
                {
                    return;
                }

                m_Width = value;

                NotifyPropertyChanged();
            }
        }

        public double Height
        {
            get
            {
                return m_Height;
            }
            set
            {
                if (Equals(m_Height, value))
                {
                    return;
                }

                m_Height = value;

                NotifyPropertyChanged();
            }
        }

        public void AssignRegionOfInterestToValues(Rect roi)
        {
            X = roi.X;
            Y = roi.Y;
            Width = roi.Width;
            Height = roi.Height;
        }

        public Rect GetRoi()
        {
            return new Rect(X, Y, Width, Height);
        }

        protected override void WindowClosing(CancelEventArgs closingEventArgs)
        {
            if (ExitResult != WindowExitResult.Ok)
            {
                ExitResult = WindowExitResult.Cancel;
            }
        }

        private void Ok()
        {
            ExitResult = WindowExitResult.Ok;
            CloseWindow();
        }

        private void Cancel()
        {
            ExitResult = WindowExitResult.Cancel;
            CloseWindow();
        }
    }
}
