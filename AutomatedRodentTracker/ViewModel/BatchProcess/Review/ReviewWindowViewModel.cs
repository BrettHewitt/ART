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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AutomatedRodentTracker.Commands;
using AutomatedRodentTracker.Extensions;
using AutomatedRodentTracker.Model;
using AutomatedRodentTracker.Model.Events;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.Model.Results;
using AutomatedRodentTracker.ModelInterface.Behaviours;
using AutomatedRodentTracker.ModelInterface.Boundries;
using AutomatedRodentTracker.ModelInterface.Datasets.Types;
using AutomatedRodentTracker.ModelInterface.RBSK;
using AutomatedRodentTracker.ModelInterface.Smoothing;
using AutomatedRodentTracker.ModelInterface.VideoSettings;
using AutomatedRodentTracker.Services.Mouse;
using AutomatedRodentTracker.Services.RBSK;
using AutomatedRodentTracker.View.Progress;
using AutomatedRodentTracker.ViewModel.Behaviours;
using AutomatedRodentTracker.ViewModel.Datasets;
using AutomatedRodentTracker.ModelInterface.Datasets;
using AutomatedRodentTracker.ModelInterface.Results;
using AutomatedRodentTracker.ModelInterface.Video;
using AutomatedRodentTracker.Services.Excel;
using AutomatedRodentTracker.Services.FileBrowser;
using AutomatedRodentTracker.Services.ImageToBitmapSource;
using AutomatedRodentTracker.ViewModel;
using AutomatedRodentTracker.ViewModel.NewWizard;
using AutomatedRodentTracker.ViewModel.Progress;
using Emgu.CV.Structure;
using Emgu.CV;

namespace AutomatedRodentTracker.ViewModel.BatchProcess.Review
{
    public class ReviewWindowViewModel : WindowViewModelBase, IDisposable
    {
        private ObservableCollection<SingleFileViewModel> m_SingleFiles;
        public ObservableCollection<SingleFileViewModel> SingleFiles
        {
            get
            {
                return m_SingleFiles;
            }
            set
            {
                if (ReferenceEquals(m_SingleFiles, value))
                {
                    return;
                }

                m_SingleFiles = value;

                NotifyPropertyChanged();
            }
        }

        private SingleFileViewModel m_SelectedVideo;
        public SingleFileViewModel SelectedVideo
        {
            get
            {
                return m_SelectedVideo;
            }
            set
            {
                if (Equals(m_SelectedVideo, value))
                {
                    return;
                }

                m_SelectedVideo = value;

                NotifyPropertyChanged();
                SelectedVideoChanged();
            }
        }

        private ISingleMouse m_Model;
        public ISingleMouse Model
        {
            get
            {
                return m_Model;
            }
            private set
            {
                if (Equals(m_Model, value))
                {
                    return;
                }

                m_Model = value;

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

        private int m_AnalyseStart;
        public int AnalyseStart
        {
            get
            {
                return m_AnalyseStart;
            }
            set
            {
                if (Equals(m_AnalyseStart, value))
                {
                    return;
                }

                m_AnalyseStart = value;

                NotifyPropertyChanged();
                CurrentResult.StartFrame = value;
                SaveCommand.RaiseCanExecuteChangedNotification();
            }
        }

        private int m_AnalyseEnd;
        public int AnalyseEnd
        {
            get
            {
                return m_AnalyseEnd;
            }
            set
            {
                if (Equals(m_AnalyseEnd, value))
                {
                    return;
                }

                m_AnalyseEnd = value;

                NotifyPropertyChanged();
                CurrentResult.EndFrame = value;
                SaveCommand.RaiseCanExecuteChangedNotification();
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

        private Dictionary<ISingleFile, IMouseDataResult> m_Results;
        public Dictionary<ISingleFile, IMouseDataResult> Results
        {
            get
            {
                return m_Results;
            }
            set
            {
                if (Equals(m_Results, value))
                {
                    return;
                }

                m_Results = value;

                NotifyPropertyChanged();
            }
        }

        private IMouseDataResult m_CurrentResult;
        public IMouseDataResult CurrentResult
        {
            get
            {
                return m_CurrentResult;
            }
            set
            {
                if (Equals(m_CurrentResult, value))
                {
                    return;
                }

                m_CurrentResult = value;

                NotifyPropertyChanged();
                SaveCommand.RaiseCanExecuteChangedNotification();
            }
        }

        private string m_DisplayText;
        public string DisplayText
        {
            get
            {
                return m_DisplayText;
            }
            set
            {
                if (Equals(m_DisplayText, value))
                {
                    return;
                }

                m_DisplayText = value;

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
                ReRunCommand.RaiseCanExecuteChangedNotification();
                ExportCommand.RaiseCanExecuteChangedNotification();
                SaveCommand.RaiseCanExecuteChangedNotification();
                SetStartFrameCommand.RaiseCanExecuteChangedNotification();
                SetEndFrameCommand.RaiseCanExecuteChangedNotification();
            }
        }

        private string m_Name;
        public string Name
        {
            get
            {
                return m_Name;
            }
            private set
            {
                if (Equals(m_Name, value))
                {
                    return;
                }

                m_Name = value;

                NotifyPropertyChanged();
            }
        }

        private int m_Age;
        public int Age
        {
            get
            {
                return m_Age;
            }
            private set
            {
                if (Equals(m_Age, value))
                {
                    return;
                }

                m_Age = value;

                NotifyPropertyChanged();
            }
        }

        private ITypeBase m_Type;
        public ITypeBase Type
        {
            get
            {
                return m_Type;
            }
            private set
            {
                if (Equals(m_Type, value))
                {
                    return;
                }

                m_Type = value;

                NotifyPropertyChanged();
            }
        }

        private double m_DistanceTravelled;
        public double DistanceTravelled
        {
            get
            {
                return m_DistanceTravelled;
            }
            set
            {
                if (Equals(m_DistanceTravelled, value))
                {
                    return;
                }

                m_DistanceTravelled = value;

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

                if (CurrentResult != null)
                {
                    CurrentResult.SmoothMotion = value;
                    SmoothMotionChanged();
                }
                
            }
        }

        private PointF[] m_MotionTrack;
        public PointF[] MotionTrack
        {
            get
            {
                return m_MotionTrack;
            }
            set
            {
                if (ReferenceEquals(m_MotionTrack, value))
                {
                    return;
                }

                m_MotionTrack = value;

                NotifyPropertyChanged();
            }
        }

        private double m_SmoothingValue = 0.68;
        public double SmoothingValue
        {
            get
            {
                return m_SmoothingValue;
            }
            set
            {
                if (Equals(m_SmoothingValue, value))
                {
                    return;
                }

                m_SmoothingValue = value;

                NotifyPropertyChanged();
                //MotionTrack = TestSmooth1();
                //MotionTrack = TestSmooth1(MotionTrack);
                //UpdateDisplayImage();
                //Console.WriteLine(value);
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

        private double m_Duration;
        public double Duration
        {
            get
            {
                return m_Duration;
            }
            set
            {
                if (Equals(m_Duration, value))
                {
                    return;
                }

                m_Duration = value;

                NotifyPropertyChanged();
            }
        }

        private double m_AvgVelocity;
        public double AvgVelocity
        {
            get
            {
                return m_AvgVelocity;
            }
            set
            {
                if (Equals(m_AvgVelocity, value))
                {
                    return;
                }

                m_AvgVelocity = value;

                NotifyPropertyChanged();
            }
        }

        private double m_AvgAngularVelocity;
        public double AvgAngularVelocity
        {
            get
            {
                return m_AvgAngularVelocity;
            }
            set
            {
                if (Equals(m_AvgAngularVelocity, value))
                {
                    return;
                }

                m_AvgAngularVelocity = value;

                NotifyPropertyChanged();
            }
        }

        private double m_CentroidSize;
        public double CentroidSize
        {
            get
            {
                return m_CentroidSize;
            }
            set
            {
                if (Equals(m_CentroidSize, value))
                {
                    return;
                }

                m_CentroidSize = value;

                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<BehaviourHolderViewModel> m_SelectedMouseBehaviours;
        public ObservableCollection<BehaviourHolderViewModel> SelectedMouseBehaviours
        {
            get
            {
                return m_SelectedMouseBehaviours;
            }
            set
            {
                if (Equals(m_SelectedMouseBehaviours, value))
                {
                    return;
                }

                m_SelectedMouseBehaviours = value;

                NotifyPropertyChanged();
            }
        }

        private ActionCommand m_OkCommand;
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

        private ActionCommand m_CancelCommand;
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

        private ActionCommand m_ReRunCommand;
        public ActionCommand ReRunCommand
        {
            get
            {
                return m_ReRunCommand ?? (m_ReRunCommand = new ActionCommand()
                {
                    ExecuteAction = ReRun,
                    CanExecuteAction = CanReRun,
                });
            }
        }

        private ActionCommand m_AutoGapDistanceCommand;
        public ActionCommand AutoGapDistanceCommand
        {
            get
            {
                return m_AutoGapDistanceCommand ?? (m_AutoGapDistanceCommand = new ActionCommand()
                {
                    ExecuteAction = AutoFindGapDistance,
                });
            }
        }

        private ActionCommand m_CancelReRunCommand;
        public ActionCommand CancelReRunCommand
        {
            get
            {
                return m_CancelReRunCommand ?? (m_CancelReRunCommand = new ActionCommand()
                {
                    ExecuteAction = CancelReRun,
                });
            }
        }

        private ActionCommand m_SaveCommand;
        public ActionCommand SaveCommand
        {
            get
            {
                return m_SaveCommand ?? (m_SaveCommand = new ActionCommand()
                {
                    ExecuteAction = SaveFile,
                    CanExecuteAction = CanSaveFile,
                });
            }
        }

        private ActionCommand m_ExportCommand;
        public ActionCommand ExportCommand
        {
            get
            {
                return m_ExportCommand ?? (m_ExportCommand = new ActionCommand()
                {
                    ExecuteAction = Export,
                    CanExecuteAction = CanExport,
                });
            }
        }

        private ActionCommand m_SetStartFrameCommand;
        public ActionCommand SetStartFrameCommand
        {
            get
            {
                return m_SetStartFrameCommand ?? (m_SetStartFrameCommand = new ActionCommand()
                {
                    ExecuteAction = SetStartFrame,
                    CanExecuteAction = CanSetStartFrame,
                });
            }
        }

        private ActionCommand m_SetEndFrameCommand;
        public ActionCommand SetEndFrameCommand
        {
            get
            {
                return m_SetEndFrameCommand ?? (m_SetEndFrameCommand = new ActionCommand()
                {
                    ExecuteAction = SetEndFrame,
                    CanExecuteAction = CanSetEndFrame,
                });
            }
        }

        public ReviewWindowViewModel(ISingleMouse model, Dictionary<ISingleFile, IMouseDataResult> results)
        {
            ObservableCollection<SingleFileViewModel> videos = new ObservableCollection<SingleFileViewModel>();
            foreach (ISingleFile singleFile in model.VideoFiles)
            {
                SingleFileViewModel vm = new SingleFileViewModel(singleFile, "");
                IMouseDataResult data = results[singleFile];
                if (data != null)
                {
                    vm.VideoOutcome = data.VideoOutcome;

                    if (data.VideoOutcome != SingleFileResult.Ok)
                    {
                        vm.Comments = data.Message;
                    }
                    else
                    {
                        vm.Comments = data.MainBehaviour;
                    }
                }
                videos.Add(vm);
            }

            SingleFiles = videos;
            Model = model;
            Results = results;

            GapDistanceMin = 5;
            GapDistanceMax = 300;
            BinaryThresholdMin = 0;
            BinaryThresholdMax = 255;
            BinaryThreshold2Min = 0;
            BinaryThreshold2Max = 255;

            Name = Model.Name;
            Age = Model.Age;
            Type = Model.Type;
        }

        private void SelectedVideoChanged()
        {
            VideoSelected = SelectedVideo != null;

            if (SelectedVideo == null)
            {
                DisplayText = "";
                ShowVideo = false;
                return;
            }

            CurrentResult = Results[SelectedVideo.Model];

            if (CurrentResult.VideoOutcome == SingleFileResult.Ok)
            {
                Video = ModelResolver.Resolve<IVideo>();
                Video.SetVideo(SelectedVideo.VideoFileName);
                Maximum = Video.FrameCount - 1;
                Minimum = 0;
                m_SliderValue = -1;
                
                ShowVideo = true;
                m_GapDistance = CurrentResult.GapDistance;
                m_BinaryThreshold = CurrentResult.ThresholdValue;
                m_BinaryThreshold2 = CurrentResult.ThresholdValue2;
                AnalyseStart = CurrentResult.StartFrame;
                AnalyseEnd = CurrentResult.EndFrame;
                DistanceTravelled = CurrentResult.DistanceTravelled;
                MotionTrack = CurrentResult.MotionTrack;
                FrameRate = CurrentResult.FrameRate;
                SmoothMotion = CurrentResult.SmoothMotion;
                Duration = CurrentResult.Duration;
                AvgVelocity = CurrentResult.AverageVelocity;
                AvgAngularVelocity = CurrentResult.AverageAngularVelocity;
                CentroidSize = CurrentResult.CentroidSize;

                List<BehaviourHolderViewModel> behaviours = new List<BehaviourHolderViewModel>();
                Dictionary<IBoundaryBase, IBehaviourHolder[]> interactingBoundries = CurrentResult.InteractingBoundries;

                foreach (KeyValuePair<IBoundaryBase, IBehaviourHolder[]> kvp in interactingBoundries)
                {
                    BoundaryBaseViewModel boundaryVm = BoundaryBaseViewModel.GetBoundaryFromModel(kvp.Key);
                    IBehaviourHolder[] currentBehaviours = kvp.Value;
                    behaviours.AddRange(currentBehaviours.Select(currentBehaviour => new BehaviourHolderViewModel(boundaryVm, currentBehaviour.Interaction, currentBehaviour.FrameNumber)));
                }

                var tempBehaviours = behaviours.OrderBy(x => x.FrameNumber);
                SelectedMouseBehaviours = new ObservableCollection<BehaviourHolderViewModel>(tempBehaviours);


                NotifyPropertyChanged("GapDistance");
                NotifyPropertyChanged("BinaryThreshold");
                NotifyPropertyChanged("BinaryThreshold2");

                //if (SmoothMotion)
                //{
                //    SmoothMotionChanged(true);
                //}
                
                SliderValue = 0;
            }
            else
            {
                DisplayText = CurrentResult.Message;
                ShowVideo = false;
            }
        }

        private void SliderValueChanged()
        {
            Video.SetFrame(SliderValue);
            CurrentImage = Video.GetFrameImage();
            UpdateDisplayImage();
        }

        private void UpdateDisplayImage()
        {
            using (Image<Bgr, Byte> img = CurrentImage.Clone())
            {
                img.DrawPolyline(MotionTrack.Select(x => x.ToPoint()).ToArray(), false, new Bgr(Color.Blue), 2);
                DisplayImage = ImageService.ToBitmapSource(img);
            }
        }

        public void Dispose()
        {
            if (Video != null)
            {
                Video.Dispose();
            }
        }

        private void Ok()
        {
            Close = true;
        }

        private void Cancel()
        {
            Close = true;
        }

        private void ReRun()
        {
            if (SelectedVideo == null)
            {
                return;
            }

            ReRunVideo(SelectedVideo.VideoFileName);
        }

        private void UpdateGapDistance()
        {
            if (CurrentResult != null)
            {
                CurrentResult.GapDistance = GapDistance;
            }
            
            RBSK rbsk = MouseService.GetStandardMouseRules();
            rbsk.Settings.GapDistance = GapDistance;
            rbsk.Settings.BinaryThreshold = BinaryThreshold;
            PointF[] result = RBSKService.RBSK(CurrentImage, rbsk);
            using (Image<Bgr, Byte> img = CurrentImage.Clone())
            {
                img.DrawPolyline(MotionTrack.Select(x => x.ToPoint()).ToArray(), false, new Bgr(Color.Blue), 2);

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
            if (CurrentResult != null)
            {
                CurrentResult.ThresholdValue = BinaryThreshold;
            }

            using (Image<Gray, Byte> grayImage = CurrentImage.Convert<Gray, Byte>())
            using (Image<Gray, Byte> binaryImage = grayImage.ThresholdBinary(new Gray(BinaryThreshold), new Gray(255)))
            {
                DisplayImage = ImageService.ToBitmapSource(binaryImage);
            }
        }

        private void UpdateBinaryThreshold2()
        {
            if (CurrentResult != null)
            {
                CurrentResult.ThresholdValue2 = BinaryThreshold2;
            }

            using (Image<Gray, Byte> grayImage = CurrentImage.Convert<Gray, Byte>())
            using (Image<Gray, Byte> binaryImage = grayImage.ThresholdBinary(new Gray(BinaryThreshold2), new Gray(255)))
            {
                DisplayImage = ImageService.ToBitmapSource(binaryImage);
            }
        }

        private void AutoFindGapDistance()
        {
            
        }

        private bool m_CancelTask;
        private object m_CancelTaskLock = new object();
        private bool CancelTask
        {
            get
            {
                lock (m_CancelTaskLock)
                {
                    return m_CancelTask;
                }
            }
            set
            {
                lock (m_CancelTaskLock)
                {
                    m_CancelTask = value;
                }
            }
        }

        private IRBSKVideo RbskVideo
        {
            get;
            set;
        }

        private void ReRunVideo(string fileName)
        {
            ProgressView view = new ProgressView();
            ProgressViewModel viewModel = new ProgressViewModel();
            view.DataContext = viewModel;
            viewModel.CancelPressed += (sender, args) =>
            {
                CancelReRun();
            };

            viewModel.WindowAboutToClose += (sender, args) =>
            {
                if (RbskVideo != null)
                {
                    RbskVideo.Paused = true;
                }
            };

            viewModel.WindowClosingCancelled += (sender, args) =>
            {
                if (RbskVideo != null)
                {
                    RbskVideo.Paused = false;
                }
            };

            CancelTask = false;

            Task.Factory.StartNew(() =>
            {
                IMouseDataResult result = Model.Results[SelectedVideo.Model];
                try
                {
                    IVideoSettings videoSettings = ModelResolver.Resolve<IVideoSettings>();
                    using (IRBSKVideo rbskVideo = ModelResolver.Resolve<IRBSKVideo>())
                    using (IVideo video = ModelResolver.Resolve<IVideo>())
                    {
                        RbskVideo = rbskVideo;
                        video.SetVideo(fileName);
                        if (video.FrameCount < 100)
                        {
                            result.VideoOutcome = SingleFileResult.FrameCountTooLow;
                            result.Message = "Exception: " + SelectedVideo.VideoFileName + " - Frame count too low";
                            //allResults.TryAdd(file, result);
                            viewModel.ProgressValue = 1;
                            //continue;
                            return;
                        }

                        videoSettings.FileName = SelectedVideo.VideoFileName;
                        videoSettings.ThresholdValue = BinaryThreshold;


                        video.SetFrame(0);
                        Image<Gray, Byte> binaryBackground;
                        IEnumerable<IBoundaryBase> boundaries;
                        videoSettings.GeneratePreview(video, out binaryBackground, out boundaries);
                        result.Boundaries = boundaries.ToArray();

                        rbskVideo.GapDistance = GapDistance;
                        rbskVideo.ThresholdValue = BinaryThreshold;

                        rbskVideo.Video = video;
                        rbskVideo.BackgroundImage = binaryBackground;

                        rbskVideo.ProgressUpdates += (s, e) =>
                        {
                            double progress = e.Progress;
                            if (progress >= 1)
                            {
                                progress = 0.999;
                            }
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                viewModel.ProgressValue = progress;
                            });
                        };

                        rbskVideo.Process();
                        RbskVideo = null;
                        //if (Stop)
                        //{
                        //    state.Stop();
                        //    return;
                        //}
                        if (CancelTask)
                        {
                            return;
                        }
                        result.Results = rbskVideo.HeadPoints;
                        result.GenerateResults();
                        result.VideoOutcome = SingleFileResult.Ok;
                        //allResults.TryAdd(SelectedVideo, result);
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Results[SelectedVideo.Model] = result;
                        CurrentResult = result;
                        SliderValueChanged();
                        SaveCommand.RaiseCanExecuteChangedNotification();
                        viewModel.ProgressValue = 1;
                    });
                }
                catch (Exception e)
                {
                    result.VideoOutcome = SingleFileResult.Error;
                    result.Message = "Exception: " + SelectedVideo.VideoFileName + " - " + e.Message;
                    //allResults.TryAdd(SelectedVideo, result);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        viewModel.ProgressValue = 1;
                    });
                    RbskVideo = null;
                }
            });

            view.ShowDialog();
        }

        private bool CanReRun()
        {
            return VideoSelected;
        }

        private void CancelReRun()
        {
            if (RbskVideo != null)
            {
                RbskVideo.Cancelled = true;
            }
            
            CancelTask = true;
        }

        protected override void WindowClosing(CancelEventArgs closingEventArgs)
        {
            if (CurrentImage != null)
            {
                CurrentImage.Dispose();
            }
        }

        private void SaveFile()
        {
            string saveFile = FileBrowser.SaveFile("ART|*.art");

            if (string.IsNullOrWhiteSpace(saveFile))
            {
                return;
            }

            ISaveArtFile save = ModelResolver.Resolve<ISaveArtFile>();
            string videoFile = SelectedVideo.VideoFileName;
            save.SaveFile(saveFile, videoFile, CurrentResult);
        }

        private bool CanSaveFile()
        {
            if (CurrentResult == null)
            {
                return false;
            }

            return CurrentResult.ModelObjectState == ModelObjectState.Dirty;
        }

        private void Export()
        {
            string artFile = SelectedVideo.ArtFileLocation;
            string folderLocation = Path.GetDirectoryName(artFile);

            string saveLocation = FileBrowser.SaveFile("Excel|*.xlsx", folderLocation);

            if (string.IsNullOrWhiteSpace(saveLocation))
            {
                return;
            }

            ExportRawData(saveLocation);
        }

        private void ExportRawData(string saveLocation)
        {
            Dictionary<int, ISingleFrameResult> results = CurrentResult.Results;
            PointF[] motionTrack = CurrentResult.MotionTrack;
            Vector[] orientationTrack = CurrentResult.OrientationTrack;
            object[,] data = new object[CurrentResult.Results.Count + 6, 3];

            data[0, 0] = "Frame";
            data[0, 1] = "X";
            data[0, 2] = "Y";

            PointF previousPoint = PointF.Empty;
            double distanceCounter = 0;

            double maxAngularVelocity = 0;
            double maxDistInOneFrame = 0;
            Vector? previousDir = null;
            int arrayDelta = 0;
            for (int j = 1; j <= results.Count; j++)
            {
                PointF[] headPoints = results[j - 1].HeadPoints;
                //PointF point = PointF.Empty;
                if (headPoints == null)
                {
                    data[j, 0] = j - 1;
                    data[j, 1] = "null";
                    data[j, 2] = "null";
                    arrayDelta = j;
                    continue;
                }

                PointF point = results[j - 1].HeadPoints[2];
                data[j, 0] = j - 1;
                data[j, 1] = point.X;
                data[j, 2] = point.Y;

                if (!previousPoint.IsEmpty)
                {
                    double currentDist = point.Distance(previousPoint);
                    distanceCounter += currentDist;

                    if (currentDist > maxDistInOneFrame)
                    {
                        maxDistInOneFrame = currentDist;
                    }
                }
                previousPoint = point;

                int index = j - 1 - arrayDelta;
                if (index >= orientationTrack.Length)
                {
                    continue;
                }

                Vector currentDir = orientationTrack[index];
                if (previousDir.HasValue)
                {
                    double angularVelocity = Vector.AngleBetween(currentDir, previousDir.Value);
                    if (angularVelocity > maxAngularVelocity)
                    {
                        maxAngularVelocity = angularVelocity;
                    }
                }

                previousDir = currentDir;
            }

            maxAngularVelocity *= Video.FrameRate;
            double time = motionTrack.Length / Video.FrameRate;
            double maxSpeed = maxDistInOneFrame * Video.FrameRate;
            double avgSpeed = distanceCounter / time;

            data[results.Count + 1, 0] = "Distance Travelled: ";
            data[results.Count + 1, 1] = distanceCounter;
            data[results.Count + 2, 0] = "Average Speed: ";
            data[results.Count + 2, 1] = avgSpeed;
            data[results.Count + 3, 0] = "Max Speed: ";
            data[results.Count + 3, 1] = maxSpeed;
            data[results.Count + 4, 0] = "Max Angular Velocity: ";
            data[results.Count + 4, 1] = maxAngularVelocity;
            data[results.Count + 5, 0] = "Distance per Frame";
            data[results.Count + 5, 1] = distanceCounter / motionTrack.Length;

            ExcelService.WriteData(data, saveLocation);
        }

        private bool CanExport()
        {
            return VideoSelected;
        }

        private void SetStartFrame()
        {
            AnalyseStart = SliderValue;
            CurrentResult.StartFrame = SliderValue;
            CurrentResult.GenerateResults();

            DistanceTravelled = CurrentResult.DistanceTravelled;
            Duration = CurrentResult.Duration;
            AvgVelocity = CurrentResult.AverageVelocity;
            AvgAngularVelocity = CurrentResult.AverageAngularVelocity;
            CentroidSize = CurrentResult.CentroidSize;
        }

        private bool CanSetStartFrame()
        {
            return VideoSelected;
        }

        private void SetEndFrame()
        {
            AnalyseEnd = SliderValue;
            CurrentResult.EndFrame = SliderValue;
            CurrentResult.GenerateResults();
            UpdateResults();
        }

        private void UpdateResults()
        {
            DistanceTravelled = CurrentResult.DistanceTravelled;
            Duration = CurrentResult.Duration;
            AvgVelocity = CurrentResult.AverageVelocity;
            AvgAngularVelocity = CurrentResult.AverageAngularVelocity;
            CentroidSize = CurrentResult.CentroidSize;
        }

        private bool CanSetEndFrame()
        {
            return VideoSelected;
        }

        private void SmoothMotionChanged()
        {
            CurrentResult.GenerateResults();
            MotionTrack = CurrentResult.GetMotionTrack();
            UpdateResults();
            //ITrackSmoothing smoothing = ModelResolver.Resolve<ITrackSmoothing>();
            //DistanceTravelled = smoothing.GetTrackLength(MotionTrack);

            UpdateDisplayImage();
        }

        //private PointF[] TestSmooth1()
        //{
        //    //double scaleFactor = 0.5;
        //    PointF[] originalTrack = CurrentResult.MotionTrack;
        //    int length = originalTrack.Length;
        //    PointF[] smoothedMotion = new PointF[length];
        //    smoothedMotion[0] = originalTrack[0];
        //    smoothedMotion[length - 1] = originalTrack[length - 1];

        //    for (int i = 1; i < length - 1; i++)
        //    {
        //        PointF prevPoint = originalTrack[i - 1];
        //        PointF nextPoint = originalTrack[i + 1];

        //        PointF midPoint = prevPoint.MidPoint(nextPoint);

        //        PointF currentPoint = originalTrack[i];
        //        Vector dir = new Vector(midPoint.X - currentPoint.X, midPoint.Y - currentPoint.Y);
        //        dir *= SmoothingValue;
        //        smoothedMotion[i] = new PointF((float)(currentPoint.X + dir.X), (float)(currentPoint.Y + dir.Y));
        //    }

        //    double dist = 0;

        //    for (int i = 1; i < length; i++)
        //    {
        //        PointF p1 = smoothedMotion[i - 1];
        //        PointF p2 = smoothedMotion[i];
        //        dist += p1.Distance(p2);
        //    }

        //    DistanceTravelled = dist;

        //    return smoothedMotion;
        //}

        //private PointF[] TestSmooth1(PointF[] originalTrack)
        //{
        //    //double scaleFactor = 0.5;
        //    int length = originalTrack.Length;
        //    PointF[] smoothedMotion = new PointF[length];
        //    smoothedMotion[0] = originalTrack[0];
        //    smoothedMotion[length - 1] = originalTrack[length - 1];

        //    for (int i = 1; i < length - 1; i++)
        //    {
        //        PointF prevPoint = originalTrack[i - 1];
        //        PointF nextPoint = originalTrack[i + 1];

        //        PointF midPoint = prevPoint.MidPoint(nextPoint);

        //        PointF currentPoint = originalTrack[i];
        //        Vector dir = new Vector(midPoint.X - currentPoint.X, midPoint.Y - currentPoint.Y);
        //        dir *= SmoothingValue;
        //        smoothedMotion[i] = new PointF((float)(currentPoint.X + dir.X), (float)(currentPoint.Y + dir.Y));
        //    }

        //    double dist = 0;

        //    for (int i = 1; i < length; i++)
        //    {
        //        PointF p1 = smoothedMotion[i - 1];
        //        PointF p2 = smoothedMotion[i];
        //        dist += p1.Distance(p2);
        //    }

        //    DistanceTravelled = dist;

        //    return smoothedMotion;
        //}
    }
}
