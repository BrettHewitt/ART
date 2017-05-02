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
