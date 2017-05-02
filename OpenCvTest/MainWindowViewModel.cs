using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using AutomatedRodentTracker.Commands;
using AutomatedRodentTracker.Extensions;
using AutomatedRodentTracker.Model.Behaviours;
using AutomatedRodentTracker.Model.Boundries;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.Model.Results;
using AutomatedRodentTracker.Model.Video;
using AutomatedRodentTracker.Model.XmlClasses;
using AutomatedRodentTracker.ModelInterface.Behaviours;
using AutomatedRodentTracker.ModelInterface.Boundries;
using AutomatedRodentTracker.ModelInterface.Smoothing;
using AutomatedRodentTracker.ModelInterface.Video;
using AutomatedRodentTracker.Services.FileBrowser;
using AutomatedRodentTracker.View.Progress;
using AutomatedRodentTracker.View.Whisker;
using AutomatedRodentTracker.ViewModel.Behaviours;
using AutomatedRodentTracker.ViewModel.HoughLinesTest;
using AutomatedRodentTracker.ViewModel.Progress;
using AutomatedRodentTracker.ViewModel.Whisker;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using AutomatedRodentTracker.ModelInterface.RBSK;
using AutomatedRodentTracker.ModelInterface.Results;
using AutomatedRodentTracker.ModelInterface.VideoSettings;
using AutomatedRodentTracker.Services.Excel;
using AutomatedRodentTracker.Services.ImageToBitmapSource;
using AutomatedRodentTracker.View.BatchProcess;
using AutomatedRodentTracker.View.BodyDetection;
using AutomatedRodentTracker.View.HoughLinesTest;
using AutomatedRodentTracker.View.Image;
using AutomatedRodentTracker.View.NewWizard;
using AutomatedRodentTracker.ViewModel;
using AutomatedRodentTracker.ViewModel.BatchProcess;
using AutomatedRodentTracker.ViewModel.BodyDetection;
using AutomatedRodentTracker.ViewModel.NewWizard;
//using BoundaryBase = AutomatedRodentTracker.ViewModel.NewWizard.BoundaryBaseViewModel;
using Microsoft.Office.Core;
using MWA_Model.ModelInterface.MouseFrame;
using MWA_Model.ModelInterface.Tracker;
using MWA_Model.ModelInterface.Whisker;
using Point = System.Drawing.Point;

namespace AutomatedRodentTracker
{
    public class MainWindowViewModel : WindowViewModelBase
    {
        private BitmapSource m_Image;
        private IVideo m_Video;
        private IVideoSettings m_VideoSettings;
        private int m_FrameCount;
        private int m_CurrentFrame = 0;
        private bool m_SliderEnabled;
        private bool m_VideoLoaded;
        private Dictionary<int, ISingleFrameResult> m_HeadPoints;
        private PointF[] m_MotionTrack;
        private Vector[] m_OrientationTrack;
        private ObservableCollection<BoundaryBaseViewModel> m_Boundries;
        private ObservableCollection<BehaviourHolderViewModel> m_Events;
        private Dictionary<BoundaryBaseViewModel, List<BehaviourHolderViewModel>> m_InteractingBoundries = new Dictionary<BoundaryBaseViewModel, List<BehaviourHolderViewModel>>();
        private string m_FrameNumberDisplay = "Frame 0";

        private string WorkingFile
        {
            get;
            set;
        }

        private string WorkingArtFile
        {
            get;
            set;
        }

        public double FrameRate
        {
            get;
            set;
        }

        private ActionCommand m_OpenVideoCommand;
        private ActionCommand m_OpenArtCommand;
        private ActionCommand m_NextFrameCommand;
        private ActionCommand m_DisposeCommand;
        private ActionCommand m_ExportRawDataCommand;
        private ActionCommand m_ExportInteractionsCommand;
        private ActionCommand m_BatchProcessCommand;
        private ActionCommand m_BodyTestCommand;
        private ActionCommand m_HoughTestCommand;
        private ActionCommand m_SaveArtFileCommand;
        private ActionCommand m_ValidateCommand;
        private ActionCommand m_WhiskerTestCommand;

        public BitmapSource Image
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

        
        public IVideoSettings VideoSettings
        {
            get
            {
                return m_VideoSettings;
            }
            set
            {
                if (Equals(m_VideoSettings, value))
                {
                    return;
                }

                m_VideoSettings = value;

                NotifyPropertyChanged();
            }
        }

        public IVideo Video
        {
            get
            {
                return m_Video;
            }
            private set
            {
                if (ReferenceEquals(m_Video, value))
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

        public int FrameCount
        {
            get
            {
                return m_FrameCount;
            }
            private set
            {
                if (Equals(m_FrameCount, value))
                {
                    return;
                }

                m_FrameCount = value;

                NotifyPropertyChanged();
            }
        }

        public int CurrentFrame
        {
            get
            {
                return m_CurrentFrame;
            }
            set
            {
                if (Equals(m_CurrentFrame, value))
                {
                    return;
                }

                m_CurrentFrame = value;

                NotifyPropertyChanged();

                UpdateDisplayImage();
                FrameNumberDisplay = "Frame: " + CurrentFrame;
            }
        }

        public string FrameNumberDisplay
        {
            get
            {
                return m_FrameNumberDisplay;
            }
            set
            {
                if (Equals(m_FrameNumberDisplay, value))
                {
                    return;
                }

                m_FrameNumberDisplay = value;

                NotifyPropertyChanged();
            }
        }

        public bool SliderEnabled
        {
            get
            {
                return m_SliderEnabled;
            }
            private set
            {
                if (Equals(m_SliderEnabled, value))
                {
                    return;
                }

                m_SliderEnabled = value;

                NotifyPropertyChanged();
            }
        }

        public bool VideoLoaded
        {
            get
            {
                return m_VideoLoaded;
            }
            set
            {
                if (Equals(m_VideoLoaded, value))
                {
                    return;
                }

                m_VideoLoaded = value;

                NotifyPropertyChanged();
                NextFrameCommand.RaiseCanExecuteChangedNotification();
                SaveArtFileCommand.RaiseCanExecuteChangedNotification();
                BodyTestCommand.RaiseCanExecuteChangedNotification();
                ValidateCommand.RaiseCanExecuteChangedNotification();
            }
        }

        public Dictionary<int, ISingleFrameResult> Results
        {
            get
            {
                return m_HeadPoints;
            }
            set
            {
                if (ReferenceEquals(m_HeadPoints, value))
                {
                    return;
                }

                m_HeadPoints = value;

                NotifyPropertyChanged();
            }
        }

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
                ExportRawDataCommand.RaiseCanExecuteChangedNotification();
                ExportInteractionsCommand.RaiseCanExecuteChangedNotification();
            }
        }

        public Vector[] OrientationTrack
        {
            get
            {
                return m_OrientationTrack;
            }
            set
            {
                if (ReferenceEquals(m_OrientationTrack, value))
                {
                    return;
                }

                m_OrientationTrack = value;

                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<BoundaryBaseViewModel> Boundries
        {
            get
            {
                return m_Boundries;
            }
            set
            {
                if (ReferenceEquals(m_Boundries, value))
                {
                    return;
                }

                m_Boundries = value;

                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<BehaviourHolderViewModel> Events
        {
            get
            {
                return m_Events;
            }
            set
            {
                if (ReferenceEquals(m_Events, value))
                {
                    return;
                }

                m_Events = value;

                NotifyPropertyChanged();
            }
        }

        public Dictionary<BoundaryBaseViewModel, List<BehaviourHolderViewModel>> InteractingBoundries
        {
            get
            {
                return m_InteractingBoundries;
            }
            set
            {
                if (ReferenceEquals(m_InteractingBoundries, value))
                {
                    return;
                }

                m_InteractingBoundries = value;

                NotifyPropertyChanged();
            }
        }

        public ActionCommand OpenVideoCommand
        {
            get
            {
                return m_OpenVideoCommand ?? (m_OpenVideoCommand = new ActionCommand()
                {
                    ExecuteAction = OpenVideo,
                });
            }
        }

        public ActionCommand OpenArtCommand
        {
            get
            {
                return m_OpenArtCommand ?? (m_OpenArtCommand = new ActionCommand()
                {
                    ExecuteAction = OpenArtFile,
                });
            }
        }

        public ActionCommand NextFrameCommand
        {
            get
            {
                return m_NextFrameCommand ?? (m_NextFrameCommand = new ActionCommand()
                {
                    ExecuteAction = GetNextFrame,
                    CanExecuteAction = () => VideoLoaded,
                });
            }
        }

        public ActionCommand DisposeCommand
        {
            get
            {
                return m_DisposeCommand ?? (m_DisposeCommand = new ActionCommand()
                {
                    ExecuteAction = TestDispose,
                });
            }
        }

        public ActionCommand ExportRawDataCommand
        {
            get
            {
                return m_ExportRawDataCommand ?? (m_ExportRawDataCommand = new ActionCommand()
                {
                    ExecuteAction = ExportRawData,
                    CanExecuteAction = CanExportRawData,
                });
            }
        }

        public ActionCommand ExportInteractionsCommand
        {
            get
            {
                return m_ExportInteractionsCommand ?? (m_ExportInteractionsCommand = new ActionCommand()
                {
                    ExecuteAction = ExportInteractions,
                    CanExecuteAction = CanExportInteractions,
                });
            }
        }

        public ActionCommand BatchProcessCommand
        {
            get
            {
                return m_BatchProcessCommand ?? (m_BatchProcessCommand = new ActionCommand()
                {
                    ExecuteAction = BatchProcess,
                });
            }
        }

        public ActionCommand BodyTestCommand
        {
            get
            {
                return m_BodyTestCommand ?? (m_BodyTestCommand = new ActionCommand()
                {
                    ExecuteAction = BeginBodyTest,
                    CanExecuteAction = CanBeginBodyTest,
                });
            }
        }

        public ActionCommand HoughTestCommand
        {
            get
            {
                return m_HoughTestCommand ?? (m_HoughTestCommand = new ActionCommand()
                {
                    ExecuteAction = HoughTest,
                });
            }
        }

        public ActionCommand SaveArtFileCommand
        {
            get
            {
                return m_SaveArtFileCommand ?? (m_SaveArtFileCommand = new ActionCommand()
                {
                    ExecuteAction = SaveArtFile,
                    CanExecuteAction = CanSaveArtFile,
                });
            }
        }

        public ActionCommand ValidateCommand
        {
            get
            {
                return m_ValidateCommand ?? (m_ValidateCommand = new ActionCommand()
                {
                    ExecuteAction = Validate,
                    CanExecuteAction = CanValidate,
                });
            }
        }

        public ActionCommand WhiskerTestCommand
        {
            get
            {
                return m_WhiskerTestCommand ?? (m_WhiskerTestCommand = new ActionCommand()
                {
                    ExecuteAction = WhiskerTest
                });
            }
        }

        private ActionCommand m_GenCommand;

        public ActionCommand GenCommand
        {
            get
            {
                return m_GenCommand ?? (m_GenCommand = new ActionCommand()
                {
                    ExecuteAction = Gen,
                });
            }
        }

        //private ActionCommand m_GoCommand;

        //public ActionCommand GoCommand
        //{
        //    get
        //    {
        //        return m_GoCommand ?? (m_GoCommand = new ActionCommand()
        //        {
        //            ExecuteAction = Go
        //        });
        //    }
        //}

        //private void Go()
        //{
        //    string[] files = Directory.GetFiles(@"D:\Emma Dataset\", "*.avi");

        //    double max = double.MinValue, min = double.MaxValue;

        //    foreach (string file in files)
        //    {
        //        using (Capture cap = new Capture(file))
        //        {
        //            double frameCount = cap.GetCaptureProperty(CapProp.FrameCount);

        //            if (frameCount < min)
        //            {
        //                min = frameCount;
        //            }

        //            if (frameCount > max)
        //            {
        //                max = frameCount;
        //            }

        //            Console.WriteLine(file + " - " + frameCount);
        //        }
        //    }

        //    Console.WriteLine("Max: {0} - Min: {1}", max, min);
        //}

        private Rectangle m_Roi;

        public Rectangle ROI
        {
            get
            {
                return m_Roi;
            }
            set
            {
                if (Equals(m_Roi, value))
                {
                    return;
                }

                m_Roi = value;

                NotifyPropertyChanged();
            }
        }

        //private ActionCommand m_GoCommand;
        //public ActionCommand GoCommand
        //{
        //    get
        //    {
        //        return m_GoCommand ?? (m_GoCommand = new ActionCommand()
        //        {
        //            ExecuteAction = Go,
        //        });
        //    }
        //}

        //private void Go()
        //{
        //    string[] allfiles = Directory.GetFiles(@"F:\Datasets\Sheffield\100205", "*.art");

        //    double averageCounter = 0;
        //    object[,] data = new object[allfiles.Length + 1, 4];
        //    data[0, 0] = "File";
        //    data[0, 1] = "Double";
        //    data[0, 2] = "Percentage";
        //    data[0, 3] = "Number of interactions";
        //    int indexCounter = 1;
        //    foreach (string file in allfiles)
        //    {
        //        double ratio = 0;
        //        double percentage = 0;
        //        int interactionCounter = 0;
        //        try
        //        {
        //            OpenArtFile(file);

        //            BehaviourHolderViewModel[] goodEvents = Events.Where(x => x.BoundaryName != "Outer Boundary - 1").ToArray();
        //            int trackLength = MotionTrack.Length;
        //            int eventCount = goodEvents.Length;
        //            int previousFrameCount = 0;
        //            int frameCounter = 0;
        //            BehaviourHolderViewModel currentInteraction = null;
        //            int startDelta = HeadPoints.FirstOrDefault(x => x.Value.Item1 != null).Key;
        //            int endDelta = Video.FrameCount - HeadPoints.LastOrDefault(x => x.Value.Item1 != null).Key;
                    
        //            for (int i = 0; i < eventCount; i++)
        //            {
        //                BehaviourHolderViewModel behaviour = goodEvents[i];
                        
        //                if (i == 0 && behaviour.Interaction == InteractionBehaviour.Ended)
        //                {
        //                    //Interacting since beginning of video
        //                    frameCounter += behaviour.FrameNumber - startDelta;
        //                    interactionCounter++;
        //                    continue;
        //                }

        //                if (i == eventCount - 1 && behaviour.Interaction == InteractionBehaviour.Started)
        //                {
        //                    //Interacting until end of video
        //                    frameCounter += (Video.FrameCount - behaviour.FrameNumber - endDelta);
        //                    interactionCounter++;
        //                    break;
        //                }

        //                if (behaviour.Interaction == InteractionBehaviour.Started)
        //                {
        //                    if (currentInteraction != null)
        //                    {
        //                        continue;
        //                    }

        //                    previousFrameCount = behaviour.FrameNumber;
        //                    currentInteraction = behaviour;
        //                }
        //                else if (currentInteraction == behaviour)
        //                {
        //                    frameCounter += (behaviour.FrameNumber - previousFrameCount);
        //                    interactionCounter++;
        //                    currentInteraction = null;
        //                }
                        
        //            }

        //            ratio = (double)frameCounter/trackLength;
        //            percentage = ratio*100;

        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(file + " - " + e.Message);
        //        }

        //        data[indexCounter, 0] = file;
        //        data[indexCounter, 1] = ratio;
        //        data[indexCounter, 2] = percentage;
        //        data[indexCounter, 3] = interactionCounter;

        //        indexCounter++;
        //        Console.WriteLine(indexCounter);
        //    }

        //    Console.WriteLine("Done!");
        //    ExcelService.WriteData(data, @"F:\Datasets\Sheffield\100205\AllInteractions.xlsx");

        //    //int qCount1 = 3 * allfiles.Length / 4;
        //    //int qCount2 = 4 * allfiles.Length / 4;
        //    //for (int i = qCount1; i < qCount2; i++)
        //    //{
        //    //    string file = allfiles[i];
        //    ////Parallel.ForEach(files, new ParallelOptions() {MaxDegreeOfParallelism = 4}, file =>
        //    ////foreach (string file in files)
        //    ////{
        //    //    try
        //    //    {
        //    //        OpenVideo(file);
        //    //        string ext = Path.GetExtension(file);
        //    //        string saveFile = file.Replace(ext, "");
        //    //        string artSaveFile = saveFile + ".art";
        //    //        SaveArtFile(artSaveFile);

        //    //        string rawDataSaveFile = saveFile + "-raw.xlsx";
        //    //        string interactionsSaveFile = saveFile + "-interactions.xlsx";
        //    //        ExportRawData(rawDataSaveFile);
        //    //        ExportInteractions(interactionsSaveFile);
        //    //    }
        //    //    catch (Exception e)
        //    //    {
        //    //        Console.WriteLine("Exception! :" + e.Message + " - " + file);
        //    //        continue;
        //    //    }
        //    //}//);
        //}

        private void GenerateBgs(string fileName)
        {
            NewWizardViewModel viewModel = new NewWizardViewModel(fileName);
            viewModel.Preview();
            string saveFile = Path.GetFileName(fileName);
            saveFile = @"D:\USB\WhiskerImages\Final11\" + saveFile.Replace(".avi", ".png");
            viewModel.BinaryBackground.Save(saveFile);
            Console.WriteLine(saveFile);
        }

        private void OpenVideo()
        {
            string workingFile = FileBrowser.BroseForVideoFiles();

            if (string.IsNullOrWhiteSpace(workingFile))
            {
                return;
            }

            NewWizardView view = new NewWizardView();
            NewWizardViewModel viewModel = new NewWizardViewModel(workingFile);
            view.DataContext = viewModel;

            try
            {
                view.ShowDialog();
                //view.Show();
                //viewModel.Preview();
                //viewModel.Ok();
            }
            catch (Exception e)
            {
                viewModel.Close = true;
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (viewModel.ExitResult != WindowExitResult.Ok)
            {
                return;
            }

            //viewModel.BinaryBackground.Save(@"D:\USB\WhiskerImages\Final11\" + Path.GetFileName(workingFile).Replace(".avi", ".png"));
            //viewModel.Video.SetFrame(159);
            //Image<Bgr, Byte> tImage = viewModel.Video.GetFrameImage();
            //tImage.ROI = viewModel.Roi;
            //tImage.Save(@"D:\USB\WhiskerImages\Final11\" + Path.GetFileName(workingFile).Replace(".avi", "-159(2).png"));
            //return;

            VideoSettings = viewModel.VideoSettings;
            Video = viewModel.Video;
            //viewModel.Video.Dispose();
            //Video = ModelResolver.Resolve<IVideo>();
            //Video.SetVideo(workingFile);
            Boundries = viewModel.Boundries;
            VideoLoaded = true;
            SliderEnabled = true;
            WorkingFile = workingFile;
            CurrentFrame = 0;
            IRBSKVideo rbskVideo = ModelResolver.Resolve<IRBSKVideo>();
            rbskVideo.Video = Video;
            rbskVideo.BackgroundImage = viewModel.BinaryBackground;
            //ImageService.DisplayImage(rbskVideo.BackgroundImage);
            rbskVideo.Roi = VideoSettings.Roi;
            ROI = VideoSettings.Roi;
            rbskVideo.GapDistance = VideoSettings.GapDistance;

            ProgressView progressView = new ProgressView();
            ProgressViewModel progressViewModel = new ProgressViewModel();
            progressView.DataContext = progressViewModel;
            rbskVideo.ProgressUpdates += (sender, args) => progressViewModel.ProgressValue = args.Progress;

            Task.Factory.StartNew(rbskVideo.Process).ContinueWith(x => Application.Current.Dispatcher.Invoke(() => PostProcess(rbskVideo)));
            //rbskVideo.Process();
            //PostProcess(rbskVideo);
            progressView.ShowDialog();
            //rbskVideo.Process();
            //Results = rbskVideo.HeadPoints;

            //if (Video.FrameCount != Results.Count)
            //{
            //    MessageBox.Show("The expected frame count does not match the generated frame count, this is often an indication the video is corrupt and can lead to inaccurate results, proceed with caution", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}

            //FrameCount = Results.Count - 1;

            ////FrameCount = HeadPoints.Count - 1;
            //GenerateMotionTrack();
            //UpdateDisplayImage();
        }

        private void PostProcess(IRBSKVideo video)
        {
            Results = video.HeadPoints;

            if (Video.FrameCount != Results.Count)
            {
                MessageBox.Show("The expected frame count does not match the generated frame count, this is often an indication the video is corrupt and can lead to inaccurate results, proceed with caution", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if (!ROI.IsEmpty)
            {
                foreach (BoundaryBaseViewModel boundary in Boundries)
                {
                    Point[] points = boundary.Model.Points;
                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i].X += ROI.X;
                        points[i].Y += ROI.Y;
                    }
                }
            }

            FrameCount = Results.Count - 1;

            //FrameCount = HeadPoints.Count - 1;
            GenerateMotionTrack();
            UpdateDisplayImage();
        }

        private void OpenVideo(string workingFile)
        {
            if (string.IsNullOrWhiteSpace(workingFile))
            {
                return;
            }

            NewWizardView view = new NewWizardView();
            NewWizardViewModel viewModel = new NewWizardViewModel(workingFile);
            view.DataContext = viewModel;

            try
            {
                //view.ShowDialog();
                view.Show();
                viewModel.Preview();
                viewModel.Ok();
            }
            catch (Exception e)
            {
                viewModel.Close = true;
                throw e;
                //MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (viewModel.ExitResult != WindowExitResult.Ok)
            {
                return;
            }

            VideoSettings = viewModel.VideoSettings;
            Video = viewModel.Video;
            //viewModel.Video.Dispose();
            //Video = ModelResolver.Resolve<IVideo>();
            //Video.SetVideo(workingFile);
            Boundries = viewModel.Boundries;
            VideoLoaded = true;
            SliderEnabled = true;
            WorkingFile = workingFile;
            CurrentFrame = 0;
            IRBSKVideo rbskVideo = ModelResolver.Resolve<IRBSKVideo>();
            rbskVideo.Video = Video;
            rbskVideo.BackgroundImage = viewModel.BinaryBackground;
            //ImageService.DisplayImage(rbskVideo.BackgroundImage);
            rbskVideo.Roi = VideoSettings.Roi;
            ROI = VideoSettings.Roi;
            rbskVideo.GapDistance = 65;
            //VideoSettings.GapDistance = 65;
            rbskVideo.Process();
            Results = rbskVideo.HeadPoints;

            if (Video.FrameCount != Results.Count)
            {
                MessageBox.Show("The expected frame count does not match the generated frame count, this is often an indication the video is corrupt and can lead to inaccurate results, proceed with caution", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            FrameCount = Results.Count - 1;

            //FrameCount = HeadPoints.Count - 1;
            GenerateMotionTrack();
            //UpdateDisplayImage();
        }

        //private void InitialiseVideo(string fileLocation)
        //{
        //    VideoLoaded = true;
        //    SliderEnabled = true;
        //    WorkingFile = fileLocation;
        //    FrameCount = Video.FrameCount - 1;
        //    CurrentFrame = 0;
        //    IRBSKVideo rbskVideo = ModelResolver.Resolve<IRBSKVideo>();
        //    rbskVideo.Video = Video;
        //    rbskVideo.Process();
        //    HeadPoints = rbskVideo.HeadPoints;
        //    GenerateMotionTrack();
        //    UpdateDisplayImage();
        //}

        private CircleF[] FindCircles(Image<Gray, Byte> binaryBackground)
        {
            double cannyThreshold = 180.0;
            double circleAccumulatorThreshold = 120;
            return CvInvoke.HoughCircles(binaryBackground, HoughType.Gradient, 2.0, 20.0, cannyThreshold, circleAccumulatorThreshold, 5);
        }
        
        private void GetNextFrame()
        {
            
        }

        protected override void WindowClosing(CancelEventArgs closingEventArgs)
        {
            OnExit();
        }

        private void OnExit()
        {
            if (Video != null)
            {
                Video.Dispose();
            }
        }

        private void TestDispose()
        {
            GC.Collect();
        }

        private void UpdateDisplayImage()
        {
            if (Results == null)
            {
                return;
            }

            PointF[] headPoint = Results[CurrentFrame].HeadPoints;
            PointF centroid = Results[CurrentFrame].Centroid;
            Video.SetFrame(CurrentFrame);
            using (Image<Bgr, Byte> currentFrame = Video.GetFrameImage())
            {
                if (currentFrame == null)
                {
                    return;
                }

                //currentFrame.ROI = VideoSettings.Roi;

                foreach (var interaction in InteractingBoundries)
                {
                    BoundaryBaseViewModel boundary = interaction.Key;
                    if (boundary.Enabled)
                    {
                        currentFrame.Draw(boundary.Points, boundary.Color, 2);
                        
                        Point avgPoint = new Point
                        {
                            X = (int) Math.Round((double) boundary.Points.Select(p => p.X).Min() + 5),
                            Y = (int) Math.Round((double) boundary.Points.Select(p => p.Y).Max() - 5),
                        };

                        CvInvoke.PutText(currentFrame, boundary.Name, avgPoint, FontFace.HersheyComplex, 0.6, boundary.Color.MCvScalar);
                    }
                }

                if (MotionTrack != null)
                {
                    currentFrame.DrawPolyline(MotionTrack.Select(x => new Point((int)x.X, (int)x.Y)).ToArray(), false, new Bgr(Color.Blue), 2);
                }

                if (!centroid.IsEmpty)
                {
                    currentFrame.Draw(new CircleF(centroid, 4), new Bgr(Color.Red), 2);
                }


                //if (CurrentFrame > 0)
                //{
                //    ISingleFrameResult prev = Results[CurrentFrame-1];
                //    Vector prevOrientation = prev.Orientation;
                //    Vector currentOrientation = Results[CurrentFrame].Orientation;
                //    Console.WriteLine(Vector.AngleBetween(prevOrientation, currentOrientation));
                //}

                

                //PointF smoothedHeadPoint = Results[CurrentFrame].HeadPoint;
                //LineSegment2DF line = new LineSegment2DF(smoothedHeadPoint, new PointF((float) (smoothedHeadPoint.X - Results[CurrentFrame].Orientation.X), (float) (smoothedHeadPoint.Y - Results[CurrentFrame].Orientation.Y)));
                //o1.Add(Results[CurrentFrame].AngularVelocity/500);
                //Console.WriteLine(Results[CurrentFrame].AngularVelocity);
                //currentFrame.Draw(line, new Bgr(Color.Red), 2);

                

                //if ( != null)
                //{
                //    currentFrame.DrawPolyline(MotionTrack.Select(x => new Point((int)x.X, (int)x.Y)).ToArray(), false, new Bgr(Color.Blue), 2);
                //}
                if (headPoint != null)
                {
                    currentFrame.Draw(new CircleF(headPoint[0], 2), new Bgr(Color.Yellow));
                    currentFrame.Draw(new CircleF(headPoint[1], 2), new Bgr(Color.Yellow));
                    currentFrame.Draw(new CircleF(headPoint[2], 2), new Bgr(Color.Yellow));
                    currentFrame.Draw(new CircleF(headPoint[3], 2), new Bgr(Color.Yellow));
                    currentFrame.Draw(new CircleF(headPoint[4], 2), new Bgr(Color.Yellow));
                    currentFrame.Draw(new CircleF(headPoint[3].MidPoint(headPoint[1]), 2), new Bgr(Color.Red));
                }

                Image = ImageService.ToBitmapSource(currentFrame);
            }
        }

        private void GenerateMotionTrack()
        {
            List<PointF> motionTrack = new List<PointF>();
            List<PointF> noseTrack = new List<PointF>();
            List<Vector> orientationTrack = new List<Vector>();
            int startOffset = 0;
            bool inStart = true;
            for (int i = 0; i < FrameCount; i++)
            {
                PointF[] headPoints = Results[i].HeadPoints;

                if (!Results[i].Centroid.IsEmpty)
                {
                    motionTrack.Add(Results[i].Centroid);
                }

                if (headPoints != null)
                {
                    PointF midPoint = headPoints[1].MidPoint(headPoints[3]);
                    //Vector up = new Vector(0,1);
                    Vector dir = new Vector(headPoints[2].X - midPoint.X, headPoints[2].Y - midPoint.Y);
                    orientationTrack.Add(dir);
                    noseTrack.Add(headPoints[2]);
                    //motionTrack.Add(headPoints[2]);
                    //motionTrack.Add(midPoint);
                    inStart = false;
                }
                else if (inStart)
                {
                    startOffset++;
                }
            }

            MotionTrack = motionTrack.ToArray();
            OrientationTrack = orientationTrack.ToArray();
            GenerateBheaviouralAnalysis(noseTrack.ToArray(), Boundries, startOffset);
        }

        private void GenerateBheaviouralAnalysis(PointF[] motionTrack, IEnumerable<BoundaryBaseViewModel> objects, int startOffset)
        {
            double minInteractionDistance = VideoSettings.MinimumInteractionDistance;
            Dictionary<BoundaryBaseViewModel, List<BehaviourHolderViewModel>> interactingBoundries = new Dictionary<BoundaryBaseViewModel, List<BehaviourHolderViewModel>>();
            int trackCount = motionTrack.Length;
            foreach (BoundaryBaseViewModel boundary in objects)
            {
                if (!boundary.Enabled)
                {
                    continue;
                }

                interactingBoundries.Add(boundary, new List<BehaviourHolderViewModel>());

                //Manually handle first point
                PointF currentPoint = motionTrack[0];
                double distance = boundary.GetMinimumDistance(currentPoint);
                BehaviourHolderViewModel previousHolder;
                if (distance < minInteractionDistance)
                {
                    //Interaction
                    previousHolder = new BehaviourHolderViewModel(boundary, InteractionBehaviour.Started, 0);
                    //interactingBoundries[boundary].Add(previousHolder);
                }
                else
                {
                    //No Interaction
                    previousHolder = new BehaviourHolderViewModel(boundary, InteractionBehaviour.Ended, 0);
                    //interactingBoundries[boundary].Add(previousHolder);
                }

                for (int i = 1; i < trackCount; i++)
                {
                    currentPoint = motionTrack[i];
                    distance = boundary.GetMinimumDistance(currentPoint);

                    if (distance < minInteractionDistance)
                    {
                        //Interaction
                        if (previousHolder.Interaction != InteractionBehaviour.Started)
                        {
                            previousHolder = new BehaviourHolderViewModel(boundary, InteractionBehaviour.Started, i + startOffset);
                            interactingBoundries[boundary].Add(previousHolder);
                        }
                    }
                    else
                    {
                        //No Interaction
                        if (previousHolder.Interaction != InteractionBehaviour.Ended)
                        {
                            previousHolder = new BehaviourHolderViewModel(boundary, InteractionBehaviour.Ended, i + startOffset);
                            interactingBoundries[boundary].Add(previousHolder);
                        }
                    }
                }
            }

            foreach (var interaction in interactingBoundries)
            {
                List<BehaviourHolderViewModel> behaviours = interaction.Value;
                int bCount = behaviours.Count;
                bCount--;
                int prevFrameNumber = -10;

                for (int i = bCount; i >= 0; i--)
                {
                    BehaviourHolderViewModel vm = behaviours[i];
                    int currentFrameNumber = vm.FrameNumber;
                    if (prevFrameNumber > 0 && Math.Abs(vm.FrameNumber - prevFrameNumber) <= 2)
                    {
                        behaviours.RemoveAt(i + 1);
                        behaviours.RemoveAt(i);
                        prevFrameNumber = -10;
                        continue;
                    }

                    prevFrameNumber = currentFrameNumber;
                }
            }
            
            InteractingBoundries = interactingBoundries;
            ObservableCollection<BehaviourHolderViewModel> events = new ObservableCollection<BehaviourHolderViewModel>();
            foreach (var boundary in interactingBoundries)
            {
                foreach (BehaviourHolderViewModel behaviour in boundary.Value)
                {
                    events.Add(behaviour);
                }
            }

            Events = new ObservableCollection<BehaviourHolderViewModel>(events.OrderBy(x => x.FrameNumber));
        }

        private void ExportRawData()
        {
            string fileName = Path.GetFileNameWithoutExtension(Video.FilePath);
            string saveLocation = FileBrowser.SaveFile("Excel|*.xlsx", fileName, string.Empty);

            if (string.IsNullOrWhiteSpace(saveLocation))
            {
                return;
            }

            object[,] data = new object[Results.Count + 6, 5];

            data[0, 0] = "Frame";
            data[0, 1] = "X";
            data[0, 2] = "Y";
            data[0, 3] = "cX";
            data[0, 4] = "cY";

            PointF previousPoint = PointF.Empty;
            double distanceCounter = 0;
            
            double maxAngularVelocity = 0;
            double maxDistInOneFrame = 0;
            Vector? previousDir = null;
            int arrayDelta = 0;
            for (int j = 1; j <= Results.Count; j++)
            {
                PointF[] headPoints = Results[j - 1].HeadPoints;
                PointF cPoint = Results[j - 1].Centroid;

                if (!cPoint.IsEmpty)
                {
                    data[j, 3] = cPoint.X;
                    data[j, 4] = cPoint.Y;
                }
                else
                {
                    data[j, 3] = "null";
                    data[j, 4] = "null";
                }

                if (headPoints == null)
                {
                    data[j, 0] = j - 1;
                    data[j, 1] = "null";
                    data[j, 2] = "null";
                    arrayDelta = j;
                    continue;
                }

                PointF point = Results[j - 1].HeadPoints[2];
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
                if (index >= OrientationTrack.Length)
                {
                    continue;
                }

                Vector currentDir = OrientationTrack[index];
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
            double time = MotionTrack.Length / Video.FrameRate;
            double maxSpeed = maxDistInOneFrame*Video.FrameRate;
            double avgSpeed = distanceCounter/time;

            data[Results.Count + 1, 0] = "Distance Travelled: ";
            data[Results.Count + 1, 1] = distanceCounter;
            data[Results.Count + 2, 0] = "Average Speed: ";
            data[Results.Count + 2, 1] = avgSpeed;
            data[Results.Count + 3, 0] = "Max Speed: ";
            data[Results.Count + 3, 1] = maxSpeed;
            data[Results.Count + 4, 0] = "Max Angular Velocity: ";
            data[Results.Count + 4, 1] = maxAngularVelocity;
            data[Results.Count + 5, 0] = "Distance per Frame";
            data[Results.Count + 5, 1] = distanceCounter/MotionTrack.Length;

            ExcelService.WriteData(data, saveLocation);
        }

        private void ExportRawData(string saveLocation)
        {
            if (string.IsNullOrWhiteSpace(saveLocation))
            {
                return;
            }

            object[,] data = new object[Results.Count + 6, 3];

            data[0, 0] = "Frame";
            data[0, 1] = "X";
            data[0, 2] = "Y";

            PointF previousPoint = PointF.Empty;
            double distanceCounter = 0;

            double maxAngularVelocity = 0;
            double maxDistInOneFrame = 0;
            Vector? previousDir = null;
            int arrayDelta = 0;
            for (int j = 1; j <= Results.Count; j++)
            {
                PointF[] headPoints = Results[j - 1].HeadPoints;
                //PointF point = PointF.Empty;
                if (headPoints == null)
                {
                    data[j, 0] = j - 1;
                    data[j, 1] = "null";
                    data[j, 2] = "null";
                    arrayDelta = j;
                    continue;
                }

                PointF point = Results[j - 1].HeadPoints[2];
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
                if (index >= OrientationTrack.Length)
                {
                    continue;
                }

                Vector currentDir = OrientationTrack[index];
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
            double time = MotionTrack.Length / Video.FrameRate;
            double maxSpeed = maxDistInOneFrame * Video.FrameRate;
            double avgSpeed = distanceCounter / time;

            data[Results.Count + 1, 0] = "Distance Travelled: ";
            data[Results.Count + 1, 1] = distanceCounter;
            data[Results.Count + 2, 0] = "Average Speed: ";
            data[Results.Count + 2, 1] = avgSpeed;
            data[Results.Count + 3, 0] = "Max Speed: ";
            data[Results.Count + 3, 1] = maxSpeed;
            data[Results.Count + 4, 0] = "Max Angular Velocity: ";
            data[Results.Count + 4, 1] = maxAngularVelocity;
            data[Results.Count + 5, 0] = "Distance per Frame";
            data[Results.Count + 5, 1] = distanceCounter / MotionTrack.Length;

            ExcelService.WriteData(data, saveLocation);
        }

        private bool CanExportRawData()
        {
            return MotionTrack != null;
        }

        private void ExportInteractions()
        {
            string saveLocation = FileBrowser.SaveFile("Excel|*.xlsx");

            if (string.IsNullOrWhiteSpace(saveLocation))
            {
                return;
            }

            ExportInteractions(saveLocation);
            return;

            object[,] data = new object[Events.Count + 1, 3];

            data[0, 0] = "Frame";
            data[0, 1] = "Event";
            data[0, 2] = "Object";

            for (int j = 1; j <= Events.Count; j++)
            {
                BehaviourHolderViewModel behaviour = Events[j - 1];

                data[j, 0] = behaviour.FrameNumber;
                data[j, 1] = behaviour.Interaction.ToString();
                data[j, 2] = behaviour.BoundaryName;
            }

            ExcelService.WriteData(data, saveLocation);
        }

        private void ExportInteractions(string saveLocation)
        {
            if (string.IsNullOrWhiteSpace(saveLocation))
            {
                return;
            }

            int boundaryCount = InteractingBoundries.Count;

            List<List<object>> boundaries = new List<List<object>>();
            if (boundaryCount > 0)
            {
                int xCounter = 0, yCounter = 0;
                boundaries.Add(new List<object>());
                boundaries[yCounter].Add("Boundary");
                boundaries[yCounter].Add("Contact");
                boundaries[yCounter].Add("Stops contact");
                boundaries[yCounter].Add("Duration (frames)");
                boundaries[yCounter].Add("Duration (seconds)");
                yCounter++;
                foreach (var kvp in InteractingBoundries)
                {
                    List<BehaviourHolderViewModel> behaviourList = kvp.Value;
                    int behaviourCount = behaviourList.Count;

                    if (behaviourCount == 0)
                    {
                        continue;
                    }

                    boundaries.Add(new List<object>());
                    BoundaryBaseViewModel vm = kvp.Key;
                    BehaviourHolderViewModel startBehaviour = null;

                    for (int i = 0; i < behaviourList.Count; i++)
                    {
                        BehaviourHolderViewModel currentBehaviour = behaviourList[i];
                        if (startBehaviour == null && currentBehaviour.Interaction == InteractionBehaviour.Started)
                        {
                            startBehaviour = currentBehaviour;
                        }
                        else if (startBehaviour != null && currentBehaviour.Interaction == InteractionBehaviour.Ended)
                        {
                            //Log length
                            int startNumber = startBehaviour.FrameNumber;
                            int endNumber = currentBehaviour.FrameNumber;
                            int interactionLength = endNumber - startNumber;
                            boundaries[yCounter].Add(vm.Name);
                            boundaries[yCounter].Add(startNumber);
                            boundaries[yCounter].Add(endNumber);
                            boundaries[yCounter].Add(interactionLength);
                            boundaries[yCounter].Add(interactionLength/Video.FrameRate);
                            boundaries.Add(new List<object>());
                            yCounter++;
                            startBehaviour = null;
                        }
                    }

                    if (startBehaviour != null)
                    {
                        boundaries[yCounter].Add(vm.Name);
                        boundaries[yCounter].Add(startBehaviour.FrameNumber);
                        boundaries[yCounter].Add("Until end of video");
                        boundaries[yCounter].Add("N/A");
                        boundaries[yCounter].Add("N/A");
                    }
                }
            }

            int behaviourYDelta = 0;
            int behaviourXDelta = 3;

            if (boundaries.Count > 0)
            {
                behaviourXDelta = 5;
                behaviourYDelta = boundaries.Count;
            }

            object[,] data = new object[Events.Count + 1 + behaviourYDelta, behaviourXDelta];

            data[0, 0] = "Frame";
            data[0, 1] = "Event";
            data[0, 2] = "Object";

            for (int j = 1; j <= Events.Count; j++)
            {
                BehaviourHolderViewModel behaviour = Events[j - 1];

                data[j, 0] = behaviour.FrameNumber;
                data[j, 1] = behaviour.Interaction.ToString();
                data[j, 2] = behaviour.BoundaryName;
            }

            int yPos = Events.Count + 1;
            foreach (List<object> list in boundaries)
            {
                data[yPos, 0] = list[0];
                data[yPos, 1] = list[1];
                data[yPos, 2] = list[2];
                data[yPos, 3] = list[3];
                data[yPos, 4] = list[4];
                yPos++;
            }

            ExcelService.WriteData(data, saveLocation);
        }

        private bool CanExportInteractions()
        {
            return MotionTrack != null;
        }

        public MainWindowViewModel()
        {
            //string[] files = Directory.GetFiles(@"D:\Vids\New folder\Vids");

            //IEnumerable<string> goodFiles = files.Where(x => x.Contains("120503") || x.Contains("120504"));

            //foreach (string file in goodFiles)
            //{
            //    if (file.Contains("070123"))
            //    {
            //        continue;
            //    }

            //    GenerateBgs(file);
            //}

            //string[] artFiles = Directory.GetFiles(@"C:\Emma Dataset", "*.art");
            //object[,] writeData = new object[70,2];
            //int counter = 0;
            //foreach (string artFile in artFiles)
            //{
            //    string mwaFile = artFile.Replace(".art", ".mwa");

            //    if (!File.Exists(mwaFile))
            //    {
            //        continue;
            //    }

            //    OpenArtFile(artFile);
            //    double dist = Validate(mwaFile);
            //    writeData[counter, 0] = artFile;
            //    writeData[counter, 1] = dist;
            //    counter++;
            //}

            //ExcelService.WriteData(writeData, @"C:\Emma Dataset\MWA-Validate2.xlsx");

            //List<double> distances1 = CompareMwaFiles(@"D:\Emma Dataset\091119-0002.mwa", @"D:\Emma Dataset\091119-0002(2).mwa");
            //List<double> distances2 = CompareMwaFiles(@"D:\Emma Dataset\091119-0014.mwa", @"D:\Emma Dataset\091119-0014(2).mwa");
            //List<double> distances3 = CompareMwaFiles(@"D:\Emma Dataset\091119-0026.mwa", @"D:\Emma Dataset\091119-0026(2).mwa");
            //List<double> distances4 = CompareMwaFiles(@"D:\Emma Dataset\091119-0040.mwa", @"D:\Emma Dataset\091119-0040(2).mwa");
            //List<double> distances5 = CompareMwaFiles(@"D:\Emma Dataset\091119-0051.mwa", @"D:\Emma Dataset\091119-0051(2).mwa");

            //int[] lengths = new int[]{distances1.Count, distances2.Count, distances3.Count, distances4.Count, distances5.Count };
            //int max = lengths.Max();
            //object[,] data = new object[max, 5];

            //int counter = 0;
            //foreach (double dist in distances1)
            //{
            //    data[counter, 0] = dist;
            //    counter++;
            //}

            //counter = 0;
            //foreach (double dist in distances2)
            //{
            //    data[counter, 1] = dist;
            //    counter++;
            //}

            //counter = 0;
            //foreach (double dist in distances3)
            //{
            //    data[counter, 2] = dist;
            //    counter++;
            //}

            //counter = 0;
            //foreach (double dist in distances4)
            //{
            //    data[counter, 3] = dist;
            //    counter++;
            //}

            //counter = 0;
            //foreach (double dist in distances5)
            //{
            //    data[counter, 4] = dist;
            //    counter++;
            //}

            //ExcelService.WriteData(data, @"D:\MwaCompare.xlsx");
        }
        //public class BehaviourHolderViewModel : ViewModelBase
        //{
        //    public BoundaryBaseViewModel Boundary
        //    {
        //        get;
        //        set;
        //    }

        //    public InteractionBehaviour Interaction
        //    {
        //        get;
        //        set;
        //    }

        //    public int FrameNumber
        //    {
        //        get;
        //        set;
        //    }

        //    public string BoundaryName
        //    {
        //        get
        //        {
        //            return Boundary.Name;
        //        }
        //    }

        //    public string Name
        //    {
        //        get
        //        {
        //            string firstComponent;

        //            if (Interaction == InteractionBehaviour.Started)
        //            {
        //                firstComponent = "Begun ";
        //            }
        //            else
        //            {
        //                firstComponent = "Ended ";
        //            }

        //            firstComponent += "interaction";

        //            return firstComponent;
        //        }
        //    }

        //    public BehaviourHolderViewModel(BoundaryBaseViewModel boundary, InteractionBehaviour interaction, int frameNumber)
        //    {
        //        Boundary = boundary;
        //        Interaction = interaction;
        //        FrameNumber = frameNumber;
        //    }
        //}

        //public enum InteractionBheaviour
        //{
        //    Started,
        //    Ended,
        //}

        private void BatchProcess()
        {
            BatchProcessView view = new BatchProcessView();
            BatchProcessViewModel viewModel = new BatchProcessViewModel();
            view.DataContext = viewModel;
            view.Show();
        }

        private void BeginBodyTest()
        {
            //BatchProcessView view = new BatchProcessView();
            //BatchVideosViewModel viewModel = new BatchVideosViewModel();
            //view.DataContext = viewModel;
            //view.ShowDialog();

            //if (viewModel.ExitResult != WindowExitResult.Ok)
            //{
            //    return;
            //}

            //string[] tgFileNames = viewModel.TgItemsSource.ToArray();
            //string[] ntgFileNames = viewModel.NtgItemsSource.ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(TrackedVideoXml));
            TrackedVideoXml trackedVideoXml;
            using (StreamReader reader = new StreamReader(WorkingArtFile))
            {
                trackedVideoXml = (TrackedVideoXml)serializer.Deserialize(reader);
            }

            ITrackedVideo trackedVideo = trackedVideoXml.GetData();

            BodyDetectionViewModel viewModel = new BodyDetectionViewModel(trackedVideo, Results, WorkingFile);
            BodyDetectionView view = new BodyDetectionView();
            view.DataContext = viewModel;
            view.Show();
        }

        private bool CanBeginBodyTest()
        {
            return VideoLoaded;
        }

        private void HoughTest()
        {
            HoughLines view = new HoughLines();
            HoughLinesViewModel viewModel = new HoughLinesViewModel();
            view.DataContext = viewModel;
            view.Show();
        }

        private void OpenArtFile()
        {
            string filePath = FileBrowser.BrowseForFile("ART|*.art");

            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            OpenArtFile(filePath);
        }

        private void OpenArtFile(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TrackedVideoXml));
            TrackedVideoXml trackedVideoXml;
            using (StreamReader reader = new StreamReader(filePath))
            {
                trackedVideoXml = (TrackedVideoXml)serializer.Deserialize(reader);
            }

            ITrackedVideo trackedVideo = trackedVideoXml.GetData();

            if (!File.Exists(trackedVideo.FileName))
            {
                MessageBoxResult result = MessageBox.Show("Can't find video, would you like to browse for it?", "Video doesn't exist", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    return;
                }

                string newFilePath = FileBrowser.BroseForVideoFiles();

                if (string.IsNullOrWhiteSpace(newFilePath))
                {
                    return;
                }

                trackedVideo.FileName = newFilePath;
            }

            IVideo video = ModelResolver.Resolve<IVideo>();
            video.SetVideo(trackedVideo.FileName);

            Dictionary<int, ISingleFrameResult> results = new Dictionary<int, ISingleFrameResult>();
            foreach (var kvp in trackedVideo.Results)
            {
                int key = kvp.Key;
                ISingleFrameResult frame = ModelResolver.Resolve<ISingleFrameResult>();

                if (kvp.Value != null)
                {
                    results.Add(key, kvp.Value);
                }
                else
                {
                    results.Add(key, frame);
                }
            }
            //Dictionary<int, ISingleFrameResult> results = trackedVideo.Results;

            List<BoundaryBaseViewModel> boundries = new List<BoundaryBaseViewModel>();
            foreach (IBoundaryBase boundary in trackedVideo.Boundries)
            {
                int id = boundary.Id;
                Point[] points = boundary.Points;

                IArtefactsBoundary artModel = boundary as IArtefactsBoundary;
                IBoxBoundary boxModel = boundary as IBoxBoundary;
                ICircleBoundary circleModel = boundary as ICircleBoundary;
                IOuterBoundary outerModel = boundary as IOuterBoundary;

                if (artModel != null)
                {
                    boundries.Add(new ArtefactsBoundaryViewModel(artModel, points));
                }
                else if (boxModel != null)
                {
                    boundries.Add(new BoxesBoundary(boxModel, points));
                }
                else if (circleModel != null)
                {
                    boundries.Add(new CircularBoundary(circleModel, points));
                }
                else if (outerModel != null)
                {
                    boundries.Add(new OuterBoundaryViewModel(outerModel, points));
                }
            }

            IBoundaryBase[] boundries2 = trackedVideo.InteractingBoundries.Keys.ToArray();
            IBehaviourHolder[][] behaviours = trackedVideo.InteractingBoundries.Values.ToArray();

            BoundaryBaseViewModel[] newKeys = boundries2.Select(BoundaryBaseViewModel.GetBoundaryFromModel).ToArray();
            List<BehaviourHolderViewModel>[] newValues = behaviours.Select(x => new List<BehaviourHolderViewModel>(x.Where(z => z.FrameNumber > 0).Select(y => new BehaviourHolderViewModel(BoundaryBaseViewModel.GetBoundaryFromModel(y.Boundary), y.Interaction, y.FrameNumber)))).ToArray();

            Dictionary<BoundaryBaseViewModel, List<BehaviourHolderViewModel>> interactingBoundries = new Dictionary<BoundaryBaseViewModel, List<BehaviourHolderViewModel>>();

            int length = newKeys.Length;
            for (int i = 0; i < length; i++)
            {
                interactingBoundries.Add(newKeys[i], newValues[i]);
            }

            Video = video;
            Boundries = new ObservableCollection<BoundaryBaseViewModel>(boundries);
            VideoLoaded = true;
            SliderEnabled = true;
            WorkingFile = trackedVideo.FileName;//filePath;
            WorkingArtFile = filePath;
            CurrentFrame = 0;
            Results = results;
            FrameCount = Results.Count - 1;

            //MotionTrack = trackedVideo.SmoothedMotionTrack;
            //List<PointF> motion = new List<PointF>();
            //foreach (var result in Results)
            //{
            //    if (result.Value != null && result.Value.HeadPoints != null)
            //    {
            //        motion.Add(result.Value.HeadPoints[1].MidPoint(result.Value.HeadPoints[3]));
            //    }
            //}

            //MotionTrack = motion.ToArray();
            OrientationTrack = trackedVideo.OrientationTrack;

            //int headCount = HeadPoints.Count;
            //PointF[] previousPoints = headPoints[0].Item1;
            //double peviousAngle = 0;
            //List<double> allAngVelocities = new List<double>();
            //Vector previousDir = new Vector();

            //if (previousPoints != null)
            //{
            //    PointF midPoint = previousPoints[1].MidPoint(previousPoints[3]);
            //    //Vector up = new Vector(0,1);
            //    previousDir = new Vector(previousPoints[2].X - midPoint.X, previousPoints[2].Y - midPoint.Y);
            //}

            //double frameTime = 1/Video.FrameRate;
            //for (int i = 1; i < headCount; i++)
            //{
            //    PointF[] currentPoints = headPoints[i].Item1;

            //    if (currentPoints != null && previousPoints != null)
            //    {
            //        PointF midPoint = currentPoints[1].MidPoint(currentPoints[3]);
            //        //Vector up = new Vector(0,1);
            //        Vector dir = new Vector(currentPoints[2].X - midPoint.X, currentPoints[2].Y - midPoint.Y);
            //        double currentDeltaAngle = Math.Abs(Vector.AngleBetween(dir, previousDir));
            //        allAngVelocities.Add(currentDeltaAngle/frameTime);
            //        previousDir = dir;
            //    }

            //    previousPoints = currentPoints;
            //}
            //Console.WriteLine("Average Angular velocity: " + allAngVelocities.Average() + ", Max: " + allAngVelocities.Max() + " Min: " + allAngVelocities.Min()) ;

            //List<Vector?> orientationTrack = new List<Vector?>();
            
            //for (int i = 0; i < FrameCount; i++)
            //{
            //    PointF[] headPoints2 = HeadPoints[i].Item1;
            //    if (headPoints2 != null)
            //    {
            //        PointF midPoint = headPoints2[1].MidPoint(headPoints2[3]);
            //        //Vector up = new Vector(0,1);
            //        Vector dir = new Vector(headPoints2[2].X - midPoint.X, headPoints2[2].Y - midPoint.Y);
            //        orientationTrack.Add(dir);
            //    }
            //    else
            //    {
            //        orientationTrack.Add(null);
            //    }
            //}

            //double maxAngularVelocity = 0;
            //Vector? previousDir2 = null;
            //for (int j = 1; j < orientationTrack.Count; j++)
            //{
            //    Vector? currentDir = orientationTrack[j];
            //    if (previousDir2.HasValue && currentDir.HasValue)
            //    {
            //        double angularVelocity = Math.Abs(Vector.AngleBetween(currentDir.Value, previousDir2.Value));
            //        if (angularVelocity > maxAngularVelocity)
            //        {
            //            maxAngularVelocity = angularVelocity;
            //        }
            //    }

            //    previousDir2 = currentDir;
            //}

            //Console.WriteLine("Test2: " + maxAngularVelocity*Video.FrameRate);

            //Events = new ObservableCollection<BehaviourHolderViewModel>(trackedVideo.Events.Select(x => new BehaviourHolderViewModel(BoundaryBaseViewModel.GetBoundaryFromModel(x.Boundary), x.Interaction, x.FrameNumber)));
            InteractingBoundries = interactingBoundries;

            ObservableCollection<BehaviourHolderViewModel> events = new ObservableCollection<BehaviourHolderViewModel>();
            foreach (var boundary in interactingBoundries)
            {
                foreach (BehaviourHolderViewModel behaviour in boundary.Value)
                {
                    events.Add(behaviour);
                }
            }

            Events = new ObservableCollection<BehaviourHolderViewModel>(events.OrderBy(x => x.FrameNumber));

            VideoSettings = ModelResolver.Resolve<IVideoSettings>();
            VideoSettings.MinimumInteractionDistance = trackedVideo.MinInteractionDistance;

            if (Video.FrameCount != Results.Count)
            {
                MessageBox.Show("The expected frame count does not match the generated frame count, this is often an indication the video is corrupt and can lead to inaccurate results, proceed with caution", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            //WorkingFile, headPoints, waistSizes, motionTrack, boundries.ToArray(), events.ToArray(),interactionBoundries, VideoSettings.MinimumInteractionDistance

            //FrameCount = HeadPoints.Count - 1;
            GenerateMotionTrack();
            UpdateDisplayImage();

            IMouseDataResult mouseDataResult = ModelResolver.Resolve<IMouseDataResult>();
            //mouseDataResult.Name = $"{file}";
            mouseDataResult.Results = results;
            mouseDataResult.Age = 10;
            mouseDataResult.Boundaries = trackedVideo.Boundries;
            mouseDataResult.VideoOutcome = SingleFileResult.Ok;
            mouseDataResult.GapDistance = trackedVideo.GapDistance;
            mouseDataResult.ThresholdValue = trackedVideo.ThresholdValue;
            mouseDataResult.ThresholdValue2 = trackedVideo.ThresholdValue2;
            mouseDataResult.StartFrame = trackedVideo.StartFrame;
            mouseDataResult.EndFrame = trackedVideo.EndFrame;
            mouseDataResult.SmoothMotion = trackedVideo.SmoothMotion;
            mouseDataResult.FrameRate = trackedVideo.FrameRate;
            mouseDataResult.UnitsToMilimeters = trackedVideo.UnitsToMilimeters;
            //mouseDataResult.PelvicArea = trackedVideo.PelvicArea1;
            //mouseDataResult.CentroidSize = trackedVideo.CentroidSize;
            //mouseDataResult.DistanceTravelled = trackedVideo.Di
            mouseDataResult.SmoothFactor = 0.68;
            mouseDataResult.GenerateResults();
            mouseDataResult.GetAverageSpeedForMoving();

            return;

            List<double> v1 = new List<double>();
            List<double> v2 = new List<double>();
            List<double> v3 = new List<double>();
            List<double> v4 = new List<double>();

            List<double> c1 = new List<double>();
            List<double> c2 = new List<double>();
            List<double> c3 = new List<double>();
            List<double> c4 = new List<double>();

            List<double> o1 = new List<double>();
            List<double> o2 = new List<double>();
            List<double> o3 = new List<double>();
            List<double> o4 = new List<double>();

            for (int i = 0; i < Results.Count; i++)
            {
                ISingleFrameResult currentFrame = Results[i];
                //PointF headPoint = currentFrame.SmoothedHeadPoint;
                
                Test1(currentFrame, i, o1, v1, c1, 5);
                Test1(currentFrame, i, o2, v2, c2, 20);
                Test1(currentFrame, i, o3, v3, c3, 30);
                Test1(currentFrame, i, o4, v4, c4, 40);

                //if (i < Results.Count - 5)
                //{
                //    ISingleFrameResult prev = Results[i + 5];
                //    Vector prevOrientation = prev.Orientation;
                //    Vector currentOrientation = Results[i].Orientation;
                //    o3.Add(Vector.AngleBetween(prevOrientation, currentOrientation));
                //    //Console.WriteLine(Vector.AngleBetween(prevOrientation, currentOrientation));
                //}

                //if (i < Results.Count - 20)
                //{
                //    ISingleFrameResult prev = Results[i + 20];
                //    Vector prevOrientation = prev.Orientation;
                //    Vector currentOrientation = Results[i].Orientation;
                //    o4.Add(Vector.AngleBetween(prevOrientation, currentOrientation));
                //    //Console.WriteLine(Vector.AngleBetween(prevOrientation, currentOrientation));
                //}
            }
            
            int o1Count = o1.Count;
            int o2Count = o2.Count;
            int o3Count = o3.Count;
            int o4Count = o4.Count;

            int[] counts = new int[] { o1Count, o2Count, o3Count, o4Count };

            int max = counts.Max();

            object[,] data = new object[14, max];

            int counter = 0;
            foreach (var val in o1)
            {
                data[0, counter] = val;
                data[5, counter] = v1[counter];
                data[10, counter] = c1[counter];
                counter++;
            }

            counter = 0;
            foreach (var val in o2)
            {
                data[1, counter] = val;
                data[6, counter] = v2[counter];
                data[11, counter] = c2[counter];
                counter++;
            }

            counter = 0;
            foreach (var val in o3)
            {
                data[2, counter] = val;
                data[7, counter] = v3[counter];
                data[12, counter] = c3[counter];
                counter++;
            }

            counter = 0;
            foreach (var val in o4)
            {
                data[3, counter] = val;
                data[8, counter] = v4[counter];
                data[13, counter] = c4[counter];
                counter++;
            }

            ExcelService.WriteData(data, @"D:\PhD\AngData15.xlsx");
        }

        private void Test1(ISingleFrameResult currentFrame, int i, List<double> o2, List<double> v1, List<double> c1, int frameDelta)
        {
            PointF[] headPoints = currentFrame.HeadPoints;

            if (i < Results.Count - frameDelta)
            {
                if (headPoints != null)
                {
                    PointF midPoint = headPoints[1].MidPoint(headPoints[3]);
                    PointF headPoint = headPoints[2];
                    PointF centroidPoint = currentFrame.Centroid;

                    ISingleFrameResult prev = Results[i + frameDelta];

                    PointF[] prevHeadPoints = prev.HeadPoints;
                    if (prevHeadPoints != null)
                    {
                        PointF prevHeadPoint = prevHeadPoints[2];
                        PointF prevMidPoint = prevHeadPoints[1].MidPoint(prevHeadPoints[3]);
                        PointF prevCentroidPoint = prev.Centroid;

                        Vector orientation = new Vector(headPoint.X - midPoint.X, headPoint.Y - midPoint.Y);
                        Vector prevOrientation = new Vector(prevHeadPoint.X - prevMidPoint.X, prevHeadPoint.Y - prevMidPoint.Y);
                        
                        o2.Add((Vector.AngleBetween(orientation, prevOrientation)));
                        v1.Add(headPoint.Distance(prevHeadPoint));

                        double dist = centroidPoint.Distance(prevCentroidPoint);

                        if (!centroidPoint.IsEmpty && !prevCentroidPoint.IsEmpty)
                        {
                            c1.Add(dist);
                        }
                        else
                        {
                            c1.Add(0);
                        }
                    }
                    else
                    {
                        o2.Add(0);
                        v1.Add(0);
                        c1.Add(0);
                    }
                }
                else
                {
                    o2.Add(0);
                    v1.Add(0);
                    c1.Add(0);
                }

                //Console.WriteLine(Vector.AngleBetween(prevOrientation, currentOrientation));
            }
        }

        private void SaveArtFile()
        {
            string fileName = Path.GetFileNameWithoutExtension(Video.FilePath);
            string filePath = FileBrowser.SaveFile("ART|*.art", fileName, string.Empty);

            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            SaveArtFile(filePath);
        }

        private void SaveArtFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            int headCount = Results.Count;
            SingleFrameResultXml[] allPoints = new SingleFrameResultXml[headCount];
            for (int i = 0; i < headCount; i++)
            {
                if (Results[i].HeadPoints == null)
                {
                    allPoints[i] = null;
                }
                else
                {
                    allPoints[i] = new SingleFrameResultXml(Results[i]);
                }
            }

            DictionaryXml<int, SingleFrameResultXml> results = new DictionaryXml<int, SingleFrameResultXml>(Results.Keys.ToArray(), allPoints);
            //DictionaryXml<int, PointFXml[]> headPoints = new DictionaryXml<int, PointFXml[]>(HeadPoints.Keys.ToArray(), allPoints);
            //DictionaryXml<int, double> waistSizes = new DictionaryXml<int, double>(HeadPoints.Keys.ToArray(), HeadPoints.Values.Select(tuple => tuple.CentroidSize).ToArray());
            PointFXml[] motionTrack = MotionTrack.Select(point => new PointFXml(point.X, point.Y)).ToArray();
            ITrackSmoothing smoothing = ModelResolver.Resolve<ITrackSmoothing>();
            PointF[] smoothedMotionTrack = smoothing.SmoothTrack(MotionTrack);
            PointFXml[] smoothedMotionTrackXml = smoothedMotionTrack.Select(point => new PointFXml(point)).ToArray();
            VectorXml[] orientationTrack = OrientationTrack.Select(vector => new VectorXml(vector)).ToArray();

            List<BoundaryBaseXml> boundries = new List<BoundaryBaseXml>();
            foreach (BoundaryBaseViewModel boundary in Boundries)
            {
                boundries.Add(boundary.Model.GetData());
            }

            List<BehaviourHolderXml> events = new List<BehaviourHolderXml>();
            foreach (BehaviourHolderViewModel behaviour in Events)
            {
                BoundaryBaseXml boundary = behaviour.Boundary.Model.GetData();
                InteractionBehaviour interaction = behaviour.Interaction;
                int frameNumber = behaviour.FrameNumber;
                events.Add(new BehaviourHolderXml(boundary, interaction, frameNumber));
            }

            BoundaryBaseXml[] keys = InteractingBoundries.Keys.Select(key => key.Model.GetData()).ToArray();
            BehaviourHolderXml[][] values = InteractingBoundries.Values.Select(value => value.Select(behavHolder => new BehaviourHolderXml(behavHolder.Boundary.Model.GetData(), behavHolder.Interaction, behavHolder.FrameNumber)).ToArray()).ToArray();
            DictionaryXml<BoundaryBaseXml, BehaviourHolderXml[]> interactionBoundries = new DictionaryXml<BoundaryBaseXml, BehaviourHolderXml[]>(keys, values);
            RectangleXml roiXml = new RectangleXml(ROI);

            TrackedVideoXml filXml = new TrackedVideoXml(WorkingFile, SingleFileResult.Ok, results, motionTrack, smoothedMotionTrackXml, orientationTrack, boundries.ToArray(), events.ToArray(), interactionBoundries, VideoSettings.MinimumInteractionDistance, VideoSettings.GapDistance, VideoSettings.ThresholdValue, VideoSettings.ThresholdValue2, 0, Video.FrameCount - 1, FrameRate, false, 0.68, 0, 0, 0, 0, 0, roiXml);
            XmlSerializer serializer = new XmlSerializer(typeof(TrackedVideoXml));

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, filXml);
            }
        }

        private bool CanSaveArtFile()
        {
            return VideoLoaded;
        }

        private void Validate()
        {
            string mwaFile = FileBrowser.BrowseForFile("MWA|*.mwa");

            if (string.IsNullOrWhiteSpace(mwaFile))
            {
                return;
            }
            //Debug.WriteLine("Test!");
            //string[] mwaFiles = Directory.GetFiles(@"C:\Emma Dataset", "*.mwa").Where(x => !x.Contains('(')).OrderBy(x => x).ToArray();
            ////string[] artFiles = Directory.GetFiles(@"C:\Emma Dataset", "*.art").Where(x => !x.Contains('(')).OrderBy(x => x).ToArray();
            //int counter = 0;
            //object[,] writeData = new object[40000, 9];
            //writeData[0, 0] = "File";
            //writeData[0, 1] = "ART X";
            //writeData[0, 2] = "ART Y";
            //writeData[0, 3] = "MWA X";
            //writeData[0, 4] = "MWA Y";
            //writeData[0, 5] = "Average X";
            //writeData[0, 6] = "Average Y";
            //writeData[0, 7] = "Diff X";
            //writeData[0, 8] = "Diff Y";
            //counter++;
            ////int tCounter = 0;
            //foreach (string mwaFile in mwaFiles)
            //{
            //    string artFile = mwaFile.Replace(".mwa", ".art");
            //    OpenArtFile(artFile);

            //    double[] artX, artY, mwaX, mwaY, avgX, avgY, diffX, diffY;
            //    Validate(mwaFile, out artX, out artY, out mwaX, out mwaY, out avgX, out avgY, out diffX, out diffY);
            //    writeData[counter, 0] = mwaFile;

            //    for (int j = 0; j < artX.Length; j++)
            //    {
            //        writeData[counter, 1] = artX[j];
            //        writeData[counter, 2] = artY[j];
            //        writeData[counter, 3] = mwaX[j];
            //        writeData[counter, 4] = mwaY[j];
            //        writeData[counter, 5] = avgX[j];
            //        writeData[counter, 6] = avgY[j];
            //        writeData[counter, 7] = diffX[j];
            //        writeData[counter, 8] = diffY[j];
            //        counter++;
            //    }

            //    //Console.WriteLine("Finished {0}", mwaFile);

            //    //tCounter++;
            //    //if (tCounter == 3)
            //    //{
            //    //    break;
            //    //}
            //}

            Validate(mwaFile);
            //ExcelService.WriteData(writeData, @"C:\Emma Dataset\ArtVsMwa3.xlsx");
            //MessageBox.Show("Finished!");
        }

        private List<double> CompareMwaFiles(string file1, string file2)
        {
            IWhiskerTrackerKey key1 = ModelResolver.Resolve<IWhiskerTrackerKey>();
            IWhiskerTrackerKey key2 = ModelResolver.Resolve<IWhiskerTrackerKey>();

            key1.Key = file1;
            key2.Key = file2;

            IWhiskerTracker t1 = key1.Model;
            IWhiskerTracker t2 = key2.Model;

            IWhisker[][] whiskers1 = t1.Frames.Select(x => x.Whiskers).ToArray();
            IWhisker[] nosePoints1 = whiskers1.Select(x => x.First(y => y.WhiskerId == -1)).ToArray();

            IWhisker[][] whiskers2 = t2.Frames.Select(x => x.Whiskers).ToArray();
            IWhisker[] nosePoints2 = whiskers2.Select(x => x.First(y => y.WhiskerId == -1)).ToArray();

            int startFrame = t1.ClipSettings.StartFrame;
            int endFrame = t1.ClipSettings.EndFrame;
            int frameInterval = t1.ClipSettings.FrameInterval;

            string videoLoc = t1.ClipSettings.ClipFilePath;
            IVideo video = ModelResolver.Resolve<IVideo>();
            video.SetVideo(videoLoc);
            int count = t1.Frames.Length;
            for (int i = 0; i < count; i++)
            {
                IMouseFrame frame1 = t1.Frames[i];
                IMouseFrame frame2 = t2.Frames[i];
                video.SetFrame(frame2.FrameNumber - 1);

                Image<Bgr, Byte> cFrame = video.GetFrameImage();
                frame1.Frame = cFrame;
                frame2.Frame = cFrame;
            }

            IMouseFrame frame = t1.Frames.First();
            double imgWidth = frame.OriginalWidth;
            double imgHeight = frame.OriginalHeight;

            List<double> distances = new List<double>();
            int counter = 0;
            for (int i = startFrame; i < endFrame; i += frameInterval)
            {
                IWhisker nose1 = nosePoints1[counter];
                IWhiskerPoint nosePoint1 = nose1.WhiskerPoints[0];

                IWhisker nose2 = nosePoints2[counter];
                IWhiskerPoint nosePoint2 = nose2.WhiskerPoints[0];

                if ((nosePoint1.XRatio == 0 && nosePoint1.YRatio == 0) || (nosePoint2.XRatio == 0 && nosePoint2.YRatio == 0))
                {
                    counter++;
                    continue;
                }

                double aX1 = nosePoint1.XRatio * imgWidth;
                double aY1 = nosePoint1.YRatio * imgHeight;

                double aX2 = nosePoint2.XRatio * imgWidth;
                double aY2 = nosePoint2.YRatio * imgHeight;

                System.Windows.Point aPoint1 = new System.Windows.Point(aX1, aY1);
                System.Windows.Point aPoint2 = new System.Windows.Point(aX2, aY2);

                double dist = aPoint1.Distance(aPoint2);
                distances.Add(dist);

                counter++;
            }

           return distances;
        }

        private void Validate(string mwaFile)
        {
            IWhiskerTrackerKey key = ModelResolver.Resolve<IWhiskerTrackerKey>();
            try
            {
                key.Key = mwaFile;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            IWhiskerTracker tracker = key.Model;

            if (tracker == null)
            {
                Console.WriteLine("Null! {0}", mwaFile);
                //return -100;
            }

            IWhisker[][] whiskers = tracker.Frames.Select(x => x.Whiskers).ToArray();
            IWhisker[] nosePoints = whiskers.Select(x => x.First(y => y.WhiskerId == -1)).ToArray();

            if (nosePoints.Length != Results.Count)
            {
                Console.WriteLine("Frame count does not match! {0}", mwaFile);
            }

            int startFrame = tracker.ClipSettings.StartFrame;
            int endFrame = tracker.ClipSettings.EndFrame;
            int frameInterval = tracker.ClipSettings.FrameInterval;

            string videoLoc = tracker.ClipSettings.ClipFilePath;
            IVideo video = ModelResolver.Resolve<IVideo>();
            video.SetVideo(videoLoc);
            foreach (var frame2 in tracker.Frames)
            {
                video.SetFrame(frame2.FrameNumber - 1);
                frame2.Frame = video.GetFrameImage();
            }

            IMouseFrame frame = tracker.Frames.First();
            double imgWidth = frame.OriginalWidth;
            double imgHeight = frame.OriginalHeight;

            List<double> xs = new List<double>();
            List<double> ys = new List<double>();
            int counter = 0;
            //We have our nose points
            for (int i = startFrame; i < endFrame; i += frameInterval)
            {
                int index = i - 1;

                PointF headPoint = Results[index].HeadPoint;
                if (headPoint.IsEmpty)
                {
                    counter++;
                    continue;
                }

                IWhisker nose = nosePoints[counter];
                IWhiskerPoint nosePoint = nose.WhiskerPoints[0];

                if (nosePoint.XRatio == 0 && nosePoint.YRatio == 0)
                {
                    counter++;
                    continue;
                }

                double aX = nosePoint.XRatio * imgWidth;
                double aY = nosePoint.YRatio * imgHeight;

                System.Windows.Point aPoint = new System.Windows.Point(aX, aY);

                xs.Add(aX - headPoint.X);
                ys.Add(aY - headPoint.Y);

                counter++;
            }

            List<double> errors = new List<double>();
            double deltaX = xs.Average();
            double deltaY = ys.Average();
            counter = 0;
            for (int i = startFrame; i < endFrame; i += frameInterval)
            {
                int index = i - 1;

                PointF headPoint = Results[index].HeadPoint;
                if (headPoint.IsEmpty)
                {
                    counter++;
                    continue;
                }

                IWhisker nose = nosePoints[counter];
                IWhiskerPoint nosePoint = nose.WhiskerPoints[0];

                if (nosePoint.XRatio == 0 && nosePoint.YRatio == 0)
                {
                    counter++;
                    continue;
                }

                double aX = nosePoint.XRatio*imgWidth;
                double aY = nosePoint.YRatio*imgHeight;

                System.Windows.Point aPoint = new System.Windows.Point(aX, aY);
                PointF mHeadPoint = new PointF(headPoint.X + (float)deltaX, headPoint.Y + (float)deltaY);
                double dist = mHeadPoint.Distance(aPoint);
                errors.Add(dist);

                counter++;
            }

            //return errors;
            MessageBox.Show(string.Format("Avg Error: {0}", errors.Average()));
        }

        private void Validate(string mwaFile, out double[] artX, out double[] artY, out double[] ethoX, out double[] ethoY, out double[] avgX, out double[] avgY, out double[] diffX, out double[] diffY)
        {
            IWhiskerTrackerKey key = ModelResolver.Resolve<IWhiskerTrackerKey>();
            try
            {
                key.Key = mwaFile;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            IWhiskerTracker tracker = key.Model;

            if (tracker == null)
            {
                Console.WriteLine("Null! {0}", mwaFile);
                //return -100;
            }

            IWhisker[][] whiskers = tracker.Frames.Select(x => x.Whiskers).ToArray();
            IWhisker[] nosePoints = whiskers.Select(x => x.First(y => y.WhiskerId == -1)).ToArray();

            if (nosePoints.Length != Results.Count)
            {
                Console.WriteLine("Frame count does not match! {0}", mwaFile);
            }

            int startFrame = tracker.ClipSettings.StartFrame;
            int endFrame = tracker.ClipSettings.EndFrame;
            int frameInterval = tracker.ClipSettings.FrameInterval;

            string videoLoc = tracker.ClipSettings.ClipFilePath;
            IVideo video = ModelResolver.Resolve<IVideo>();
            video.SetVideo(videoLoc);
            foreach (var frame2 in tracker.Frames)
            {
                video.SetFrame(frame2.FrameNumber - 1);
                frame2.Frame = video.GetFrameImage();
            }

            IMouseFrame frame = tracker.Frames.First();
            double imgWidth = frame.OriginalWidth;
            double imgHeight = frame.OriginalHeight;

            List<double> xs = new List<double>();
            List<double> ys = new List<double>();
            int counter = 0;
            //We have our nose points
            for (int i = startFrame; i < endFrame; i += frameInterval)
            {
                int index = i - 1;

                PointF headPoint = Results[index].HeadPoint;
                if (headPoint.IsEmpty)
                {
                    counter++;
                    continue;
                }

                IWhisker nose = nosePoints[counter];
                IWhiskerPoint nosePoint = nose.WhiskerPoints[0];

                if (nosePoint.XRatio == 0 && nosePoint.YRatio == 0)
                {
                    counter++;
                    continue;
                }

                double aX = nosePoint.XRatio * imgWidth;
                double aY = nosePoint.YRatio * imgHeight;

                System.Windows.Point aPoint = new System.Windows.Point(aX, aY);

                xs.Add(aX - headPoint.X);
                ys.Add(aY - headPoint.Y);

                counter++;
            }

            List<double> errors = new List<double>();
            double deltaX = xs.Average();
            double deltaY = ys.Average();
            counter = 0;
            List<double> arX = new List<double>(), arY = new List<double>(), mwX = new List<double>(), mwY = new List<double>(), avX = new List<double>(), avY = new List<double>(), difX = new List<double>(), difY = new List<double>();

            for (int i = startFrame; i < endFrame; i += frameInterval)
            {
                int index = i - 1;

                PointF headPoint = Results[index].HeadPoint;
                if (headPoint.IsEmpty)
                {
                    counter++;
                    continue;
                }

                IWhisker nose = nosePoints[counter];
                IWhiskerPoint nosePoint = nose.WhiskerPoints[0];

                if (nosePoint.XRatio == 0 && nosePoint.YRatio == 0)
                {
                    counter++;
                    continue;
                }

                double aX = nosePoint.XRatio * imgWidth;
                double aY = nosePoint.YRatio * imgHeight;

                System.Windows.Point aPoint = new System.Windows.Point(aX, aY);
                PointF mHeadPoint = new PointF(headPoint.X + (float)deltaX, headPoint.Y + (float)deltaY);

                arX.Add(aX);
                arY.Add(aY);
                mwX.Add(mHeadPoint.X);
                mwY.Add(mHeadPoint.Y);

                double averageX = (aX + mHeadPoint.X) / 2d;
                double averageY = (aY + mHeadPoint.Y) / 2d;
                double differX = aX - mHeadPoint.X;
                double differY = aY - mHeadPoint.Y;

                avX.Add(averageX);
                avY.Add(averageY);
                difX.Add(differX);
                difY.Add(differY);

                //double dist = mHeadPoint.Distance(aPoint);
                //errors.Add(dist);

                counter++;
            }

            artX = arX.ToArray();
            artY = arY.ToArray();
            ethoX = mwX.ToArray();
            ethoY = mwY.ToArray();
            avgX = avX.ToArray();
            avgY = avY.ToArray();
            diffX = difX.ToArray();
            diffY = difY.ToArray();
            //return errors.Average();
            //MessageBox.Show(string.Format("Avg Error: {0}", errors.Average()));
        }

        private bool CanValidate()
        {
            //return true;
            return VideoLoaded;
        }

        private void WhiskerTest()
        {
            WhiskerValidationViewModel vm = new WhiskerValidationViewModel();
            WhiskerValidationView view = new WhiskerValidationView();
            view.DataContext = vm;
            view.ShowDialog();
        }

        private void Gen()
        {
            
        }
    }
}
