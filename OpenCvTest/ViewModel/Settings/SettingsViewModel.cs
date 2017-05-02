using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using AutomatedRodentTracker.Commands;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.ModelInterface.Video;
using AutomatedRodentTracker.Services.ImageToBitmapSource;
using AutomatedRodentTracker.Services.Mouse;
using AutomatedRodentTracker.Services.RBSK;
using AutomatedRodentTracker.ViewModel.Datasets;
using Emgu.CV;
using Emgu.CV.Structure;

namespace AutomatedRodentTracker.ViewModel.Settings
{
    public class SettingsViewModel : WindowViewModelBase
    {
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

        private ObservableCollection<SingleFileViewModel> m_Mice;
        public ObservableCollection<SingleFileViewModel> Mice
        {
            get
            {
                return m_Mice;
            }
            set
            {
                if (ReferenceEquals(m_Mice, value))
                {
                    return;
                }

                m_Mice = value;

                NotifyPropertyChanged();
            }
        }

        private SingleFileViewModel m_SelectedMouse;
        public SingleFileViewModel SelectedMouse
        {
            get
            {
                return m_SelectedMouse;
            }
            set
            {
                if (Equals(m_SelectedMouse, value))
                {
                    return;
                }

                m_SelectedMouse = value;

                SelectedMouseChanged();
                NotifyPropertyChanged();
            }
        }

        private BitmapSource m_Image;
        public BitmapSource DisplayImage
        {
            get
            {
                return m_Image;
            }
            set
            {
                if (ReferenceEquals(m_Image, value))
                {
                    return;
                }

                m_Image = value;

                NotifyPropertyChanged();
            }
        }

        private int m_SliderValue;
        public int SliderValue
        {
            get
            {
                return m_SliderValue;
            }
            set
            {
                if (Equals(m_SliderValue, value))
                {
                    return;
                }

                m_SliderValue = value;

                NotifyPropertyChanged();
                SliderValueChanged();
            }
        }

        private int m_Minimum;
        public int Minimum
        {
            get
            {
                return m_Minimum;
            }
            set
            {
                if (Equals(m_Minimum, value))
                {
                    return;
                }

                m_Minimum = value;

                NotifyPropertyChanged();
            }
        }

        private int m_Maximum;
        public int Maximum
        {
            get
            {
                return m_Maximum;
            }
            set
            {
                if (Equals(m_Maximum, value))
                {
                    return;
                }

                m_Maximum = value;

                NotifyPropertyChanged();
            }
        }

        private IVideo m_Video;
        public IVideo Video
        {
            get
            {
                return m_Video;
            }
            set
            {
                if (Equals(m_Video, value))
                {
                    return;
                }

                if (m_Video != null)
                {
                    m_Video.Dispose();
                }

                m_Video = value;

                NotifyPropertyChanged();
            }
        }

        private bool m_ShowVideo;
        public bool ShowVideo
        {
            get
            {
                return m_ShowVideo;
            }
            set
            {
                if (Equals(m_ShowVideo, value))
                {
                    return;
                }

                m_ShowVideo = value;

                NotifyPropertyChanged();
            }
        }

        private double m_GapDistance;
        public double GapDistance
        {
            get
            {
                return m_GapDistance;
            }
            set
            {
                if (Equals(m_GapDistance, value))
                {
                    return;
                }

                m_GapDistance = value;

                NotifyPropertyChanged();
                UpdateGapDistance();
            }
        }

        private double m_GapDistanceMin;
        public double GapDistanceMin
        {
            get
            {
                return m_GapDistanceMin;
            }
            set
            {
                if (Equals(m_GapDistanceMin, value))
                {
                    return;
                }

                m_GapDistanceMin = value;

                NotifyPropertyChanged();
            }
        }

        private double m_GapDistanceMax;
        public double GapDistanceMax
        {
            get
            {
                return m_GapDistanceMax;
            }
            set
            {
                if (Equals(m_GapDistanceMax))
                {
                    return;
                }

                m_GapDistanceMax = value;

                NotifyPropertyChanged();
            }
        }

        private int m_BinaryThreshold;
        public int BinaryThreshold
        {
            get
            {
                return m_BinaryThreshold;
            }
            set
            {
                if (Equals(m_BinaryThreshold, value))
                {
                    return;
                }

                m_BinaryThreshold = value;

                NotifyPropertyChanged();
                UpdateBinaryThreshold();
            }
        }

        private int m_BinaryThresholdMax;
        public int BinaryThresholdMax
        {
            get
            {
                return m_BinaryThresholdMax;
            }
            set
            {
                if (Equals(m_BinaryThresholdMax, value))
                {
                    return;
                }

                m_BinaryThresholdMax = value;

                NotifyPropertyChanged();
            }
        }

        private int m_BinaryThresholdMin;
        public int BinaryThresholdMin
        {
            get
            {
                return m_BinaryThresholdMin;
            }
            set
            {
                if (Equals(m_BinaryThresholdMin, value))
                {
                    return;
                }

                m_BinaryThresholdMin = value;

                NotifyPropertyChanged();
            }
        }

        private int m_BinaryThreshold2;
        public int BinaryThreshold2
        {
            get
            {
                return m_BinaryThreshold2;
            }
            set
            {
                if (Equals(m_BinaryThreshold2, value))
                {
                    return;
                }

                m_BinaryThreshold2 = value;

                NotifyPropertyChanged();
                UpdateBinaryThreshold2();
            }
        }

        private int m_BinaryThreshold2Max;
        public int BinaryThreshold2Max
        {
            get
            {
                return m_BinaryThreshold2Max;
            }
            set
            {
                if (Equals(m_BinaryThreshold2Max, value))
                {
                    return;
                }

                m_BinaryThreshold2Max = value;

                NotifyPropertyChanged();
            }
        }

        private int m_BinaryThreshold2Min;
        public int BinaryThreshold2Min
        {
            get
            {
                return m_BinaryThreshold2Min;
            }
            set
            {
                if (Equals(m_BinaryThreshold2Min, value))
                {
                    return;
                }

                m_BinaryThreshold2Min = value;

                NotifyPropertyChanged();
            }
        }

        private Image<Bgr, Byte> m_CurrentImage;
        public Image<Bgr, Byte> CurrentImage
        {
            get
            {
                return m_CurrentImage;
            }
            set
            {
                if (Equals(m_CurrentImage, value))
                {
                    return;
                }

                if (m_CurrentImage != null)
                {
                    m_CurrentImage.Dispose();
                }

                m_CurrentImage = value;

                NotifyPropertyChanged();
            }
        }

        private bool m_VideoSelected;
        public bool VideoSelected
        {
            get
            {
                return m_VideoSelected;
            }
            set
            {
                if (Equals(m_VideoSelected, value))
                {
                    return;
                }

                m_VideoSelected = value;

                NotifyPropertyChanged();
            }
        }

        private bool m_SmoothMotion;
        public bool SmoothMotion
        {
            get
            {
                return m_SmoothMotion;
            }
            set
            {
                if (Equals(m_SmoothMotion, value))
                {
                    return;
                }

                m_SmoothMotion = value;

                NotifyPropertyChanged();
            }
        }

        private double m_FrameRate;
        public double FrameRate
        {
            get
            {
                return m_FrameRate;
            }
            set
            {
                if (Equals(m_FrameRate, value))
                {
                    return;
                }

                m_FrameRate = value;

                NotifyPropertyChanged();
            }
        }

        public SettingsViewModel(IEnumerable<SingleMouseViewModel> mice)
        {
            GapDistanceMin = 5;
            GapDistanceMax = 300;
            BinaryThresholdMin = 0;
            BinaryThresholdMax = 255;
            BinaryThreshold2Min = 0;
            BinaryThreshold2Max = 255;

            m_GapDistance = 35;
            m_BinaryThreshold = 20;
            m_BinaryThreshold2 = 10;
            SmoothMotion = true;

            ObservableCollection<SingleFileViewModel> singleFiles = new ObservableCollection<SingleFileViewModel>();
            foreach (var mouse in mice)
            {
                foreach (var file in mouse.VideoFiles)
                {
                    singleFiles.Add(new SingleFileViewModel(file, ""));
                }
            }

            Mice = singleFiles;

            using (IVideo video = ModelResolver.Resolve<IVideo>())
            {
                video.SetVideo(Mice.First().VideoFileName);
                FrameRate = video.FrameRate;
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

        private void SliderValueChanged()
        {
            Video.SetFrame(SliderValue);
            CurrentImage = Video.GetFrameImage();
            UpdateDisplayImage();
        }

        private void UpdateDisplayImage()
        {
            RBSK rbsk = MouseService.GetStandardMouseRules();
            rbsk.Settings.GapDistance = GapDistance;
            rbsk.Settings.BinaryThreshold = BinaryThreshold;
            PointF[] result = RBSKService.RBSK(CurrentImage, rbsk);
            using (Image<Bgr, Byte> img = CurrentImage.Clone())
            {
                if (result != null)
                {
                    foreach (PointF point in result)
                    {
                        img.Draw(new CircleF(point, 2), new Bgr(Color.Yellow), 2);
                    }
                }

                DisplayImage = ImageService.ToBitmapSource(img);
            }
        }

        private void UpdateGapDistance()
        {
            RBSK rbsk = MouseService.GetStandardMouseRules();
            rbsk.Settings.GapDistance = GapDistance;
            rbsk.Settings.BinaryThreshold = BinaryThreshold;
            PointF[] result = RBSKService.RBSK(CurrentImage, rbsk);
            using (Image<Bgr, Byte> img = CurrentImage.Clone())
            {
                if (result != null)
                {
                    foreach (PointF point in result)
                    {
                        img.Draw(new CircleF(point, 2), new Bgr(Color.Yellow), 2);
                    }
                }

                DisplayImage = ImageService.ToBitmapSource(img);
            }
        }

        private void UpdateBinaryThreshold()
        {
            using (Image<Gray, Byte> grayImage = CurrentImage.Convert<Gray, Byte>())
            using (Image<Gray, Byte> binaryImage = grayImage.ThresholdBinary(new Gray(BinaryThreshold), new Gray(255)))
            {
                DisplayImage = ImageService.ToBitmapSource(binaryImage);
            }
        }

        private void UpdateBinaryThreshold2()
        {
            using (Image<Gray, Byte> grayImage = CurrentImage.Convert<Gray, Byte>())
            using (Image<Gray, Byte> binaryImage = grayImage.ThresholdBinary(new Gray(BinaryThreshold2), new Gray(255)))
            {
                DisplayImage = ImageService.ToBitmapSource(binaryImage);
            }
        }

        private void SelectedMouseChanged()
        {
            if (SelectedMouse == null)
            {
                Video = null;
                ShowVideo = false;
                return;
            }

            Video = ModelResolver.Resolve<IVideo>();
            Video.SetVideo(SelectedMouse.VideoFileName);
            Maximum = Video.FrameCount - 1;
            Minimum = 0;
            m_SliderValue = -1;
            SliderValue = 0;
            ShowVideo = true;
        }

        protected override void WindowClosing(CancelEventArgs closingEventArgs)
        {
            if (Video != null)
            {
                Video.Dispose();
            }

            if (CurrentImage != null)
            {
                CurrentImage.Dispose();
            }
        }
    }
}
