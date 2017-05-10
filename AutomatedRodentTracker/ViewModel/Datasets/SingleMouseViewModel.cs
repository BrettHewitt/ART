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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AutomatedRodentTracker.Commands;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.Model.Results;
using AutomatedRodentTracker.ModelInterface.Boundries;
using AutomatedRodentTracker.Services.RBSK;
using Emgu.CV;
using Emgu.CV.Structure;
using AutomatedRodentTracker.ModelInterface.Datasets;
using AutomatedRodentTracker.ModelInterface.Datasets.Types;
using AutomatedRodentTracker.ModelInterface.RBSK;
using AutomatedRodentTracker.ModelInterface.Results;
using AutomatedRodentTracker.ModelInterface.Video;
using AutomatedRodentTracker.ModelInterface.VideoSettings;
using AutomatedRodentTracker.Services.Excel;
using AutomatedRodentTracker.View.BatchProcess.Review;
using AutomatedRodentTracker.ViewModel;
using AutomatedRodentTracker.ViewModel.BatchProcess.Review;
using Emgu.CV.CvEnum;

namespace AutomatedRodentTracker.ViewModel.Datasets
{
    public class SingleMouseViewModel : ViewModelBase
    {
        private bool m_IsSelected;
        public bool IsSelected
        {
            get
            {
                return m_IsSelected;
            }
            set
            {
                if (Equals(m_IsSelected, value))
                {
                    return;
                }

                m_IsSelected = value;

                NotifyPropertyChanged();
            }
        }

        private ISingleMouse m_Model;
        public ISingleMouse Model
        {
            get
            {
                return m_Model;
            }
            set
            {
                if (Equals(m_Model, value))
                {
                    return;
                }

                m_Model = value;

                NotifyPropertyChanged();
            }
        }

        private double m_Progress = 0;
        public double Progress
        {
            get
            {
                return m_Progress;
            }
            private set
            {
                if (Equals(m_Progress, value))
                {
                    return;
                }

                m_Progress = value;

                NotifyPropertyChanged();
            }
        }

        private ConcurrentDictionary<ISingleFile, double> m_ProgressDictionary;
        public ConcurrentDictionary<ISingleFile, double> ProgressDictionary
        {
            get
            {
                return m_ProgressDictionary;
            }
            set
            {
                if (ReferenceEquals(m_ProgressDictionary, value))
                {
                    return;
                }

                m_ProgressDictionary = value;

                NotifyPropertyChanged();
            }
        }

        public string Name
        {
            get
            {
                return Model.Name;
            }
            set
            {
                if (Equals(Model.Name, value))
                {
                    return;
                }

                Model.Name = value;

                NotifyPropertyChanged();
            }
        }

        public string Id
        {
            get
            {
                return Model.Id;
            }
            set
            {
                if (Equals(Model.Id, value))
                {
                    return;
                }

                Model.Id = value;

                NotifyPropertyChanged();
            }
        }

        public ITypeBase Type
        {
            get
            {
                return Model.Type;
            }
            set
            {
                if (Equals(Model.Type, value))
                {
                    return;
                }

                Model.Type = value;

                NotifyPropertyChanged();
            }
        }

        public List<string> Videos
        {
            get
            {
                return Model.Videos;
            }
            set
            {
                if (ReferenceEquals(Model.Videos, value))
                {
                    return;
                }

                Model.Videos = value;

                NotifyPropertyChanged();
            }
        }

        public string Class
        {
            get
            {
                return Model.Class;
            }
            set
            {
                if (Equals(Model.Class, value))
                {
                    return;
                }

                Model.Class = value;

                NotifyPropertyChanged();
            }
        }

        public int Age
        {
            get
            {
                return Model.Age;
            }
            set
            {
                if (Equals(Model.Age, value))
                {
                    return;
                }

                Model.Age = value;

                NotifyPropertyChanged();
            }
        }

        public List<ISingleFile> VideoFiles
        {
            get
            {
                return Model.VideoFiles;
            }
            set
            {
                if (ReferenceEquals(Model.VideoFiles, value))
                {
                    return;
                }

                Model.VideoFiles = value;

                NotifyPropertyChanged();
            }
        }

        public Dictionary<ISingleFile, IMouseDataResult> Results
        {
            get
            {
                return Model.Results;
            }
            set
            {
                if (ReferenceEquals(Model.Results, value))
                {
                    return;
                }

                Model.Results = value;

                NotifyPropertyChanged();
                ReviewCommand.RaiseCanExecuteChangedNotification();
            }
        }

        private ActionCommand m_SettingsCommand;

        public ActionCommand SettingsCommand
        {
            get
            {
                return m_SettingsCommand ?? (m_SettingsCommand = new ActionCommand()
                {
                    ExecuteAction = ShowSettings,
                    CanExecuteAction = CanShowSettings,
                });
            }
        }

        private ActionCommand m_ReviewCommand;
        public ActionCommand ReviewCommand
        {
            get
            {
                return m_ReviewCommand ?? (m_ReviewCommand = new ActionCommand()
                {
                    ExecuteAction = ReviewFiles,
                    CanExecuteAction = CanReviewFiles,
                });
            }
        }

        private readonly object m_CancelLock = new object();
        private bool m_Cancel = false;
        public bool Cancel
        {
            get
            {
                lock (m_CancelLock)
                {
                    return m_Cancel;
                }
            }
            set
            {
                lock (m_CancelLock)
                {
                    if (Equals(m_Cancel, value))
                    {
                        return;
                    }

                    m_Cancel = value;
                }

                NotifyPropertyChanged();
            }
        }

        private object m_PauseLock = new object();
        private bool m_Paused = false;
        public bool Paused
        {
            get
            {
                lock (m_PauseLock)
                {
                    return m_Paused;
                }
            }
            set
            {
                lock (m_PauseLock)
                {
                    if (Equals(m_Paused, value))
                    {
                        return;
                    }

                    m_Paused = value;
                }

                NotifyPropertyChanged();
            }
        }

        private readonly object m_StopLock = new object();
        private bool m_Stop = false;
        public bool Stop
        {
            get
            {
                lock (m_StopLock)
                {
                    return m_Stop;
                }
            }
            set
            {
                lock (m_StopLock)
                {
                    if (Equals(m_Stop, value))
                    {
                        return;
                    }

                    m_Stop = value;
                }

                NotifyPropertyChanged();

                if (Rbsk != null)
                {
                    foreach (var rbsk in Rbsk)
                    {
                        rbsk.Cancelled = true;
                    }
                }
            }
        }

        private readonly object m_RbskLock = new object();
        private ConcurrentBag<IRBSKVideo> m_Rbsk;
        public ConcurrentBag<IRBSKVideo> Rbsk
        {
            get
            {
                lock (m_RbskLock)
                {
                    return m_Rbsk;
                }
            }
            set
            {
                lock (m_RbskLock)
                {
                    if (Equals(m_Rbsk, value))
                    {
                        return;
                    }

                    m_Rbsk = value;
                }

                NotifyPropertyChanged();
            }
        }

        private ParallelOptions ParallelOptions
        {
            get;
            set;
        }

        public int ThresholdValue
        {
            get;
            set;
        }

        public int ThresholdValue2
        {
            get;
            set;
        }

        public double GapDistance
        {
            get;
            set;
        }

        public bool SmoothMotion
        {
            get;
            set;
        }

        public double FrameRate
        {
            get;
            set;
        }

        public SingleMouseViewModel(ISingleMouse model)
        {
            Model = model;
            ResetProgress();
            ParallelOptions = new ParallelOptions();
            ParallelOptions.MaxDegreeOfParallelism = 4;

            ThresholdValue = 20;
            ThresholdValue2 = 10;
            GapDistance = -1;
            SmoothMotion = true;
        }

        public void UpdateProgress(ISingleFile file, double progress)
        {
            int fileCount = ProgressDictionary.Count;

            if (ProgressDictionary.ContainsKey(file))
            {
                ProgressDictionary[file] = progress;
            }

            double totalProgress = ProgressDictionary.Values.Sum();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Progress = (totalProgress / fileCount) * 100;
            });
        }

        public void ResetProgress()
        {
            ProgressDictionary = new ConcurrentDictionary<ISingleFile, double>();
            foreach (ISingleFile file in VideoFiles)
            {
                ProgressDictionary.TryAdd(file, 0);
            }
        }

        public void RunFiles(string outputLocation)
        {
            if (!outputLocation.EndsWith("\\"))
            {
                outputLocation += "\\";
            }

            //ConcurrentBag<IMouseDataResult> allResults = new ConcurrentBag<IMouseDataResult>();
            Rbsk = new ConcurrentBag<IRBSKVideo>();
            ConcurrentDictionary<ISingleFile, IMouseDataResult> allResults = new ConcurrentDictionary<ISingleFile, IMouseDataResult>();
            List<ISingleFile> files = VideoFiles;
            Parallel.ForEach(files, new ParallelOptions() { MaxDegreeOfParallelism = 2 }, (file, state) =>
            //foreach (var file in files)
            {
                IMouseDataResult result = ModelResolver.Resolve<IMouseDataResult>();
                //result.Name = $"{Name} - {Id} - {Class} - {file}";
                result.Name = string.Format("{0} - {1} - {2} - {3}", Name, Id, Class, file);
                result.Age = Age;
                result.Type = Type;
                
                ISaveArtFile save = ModelResolver.Resolve<ISaveArtFile>();
                string artFile;
                string videoFile = file.VideoFileName;
                if (string.IsNullOrWhiteSpace(outputLocation))
                {

                    string extension = Path.GetExtension(videoFile);
                    artFile = videoFile.Replace(extension, ".art");
                }
                else
                {
                    string fileName = Path.GetFileNameWithoutExtension(videoFile);
                    fileName += ".art";
                    artFile = outputLocation + fileName;
                }

                if (File.Exists(artFile))
                {
                    UpdateProgress(file, 1);
                    return;
                    //continue;
                }

                try
                {
                    IVideoSettings videoSettings = ModelResolver.Resolve<IVideoSettings>();
                    using (IRBSKVideo rbskVideo = ModelResolver.Resolve<IRBSKVideo>())
                    using (IVideo video = ModelResolver.Resolve<IVideo>())
                    {
                        Rbsk.Add(rbskVideo);
                        video.SetVideo(file.VideoFileName);
                        if (video.FrameCount <= 100)
                        {
                            result.VideoOutcome = SingleFileResult.FrameCountTooLow;
                            result.Message = "Exception: " + file.VideoFileName + " - Frame count too low";
                            allResults.TryAdd(file, result);
                            UpdateProgress(file, 1);
                            save.SaveFile(artFile, videoFile, result);
                            return;
                            //continue;
                        }

                        result.FrameRate = video.FrameRate;
                        video.SetFrame(0);
                        //int threshold = 20;
                        //using (var temp = video.GetGrayFrameImage())
                        //{
                        //    threshold = (int)CalculateOtsu(temp) + 10;
                        //}

                        videoSettings.FileName = file.VideoFileName;
                        videoSettings.ThresholdValue = ThresholdValue;

                        Image<Gray, Byte> binaryBackground;
                        IEnumerable<IBoundaryBase> boundaries;
                        videoSettings.GeneratePreview(video, out binaryBackground, out boundaries);
                        result.Boundaries = boundaries.ToArray();

                        rbskVideo.Video = video;
                        rbskVideo.GapDistance = GapDistance;
                        rbskVideo.BackgroundImage = binaryBackground;
                        rbskVideo.ThresholdValue = ThresholdValue;
                        rbskVideo.ThresholdValue2 = ThresholdValue2;
                        
                        rbskVideo.ProgressUpdates += (s, e) => UpdateProgress(file, e.Progress);
                        rbskVideo.Process();

                        if (Stop)
                        {
                            state.Stop();
                            //return;
                        }

                        result.GapDistance = rbskVideo.GapDistance;
                        result.MinInteractionDistance = 15;
                        result.ThresholdValue = rbskVideo.ThresholdValue;
                        result.ThresholdValue2 = rbskVideo.ThresholdValue2;

                        result.Results = rbskVideo.HeadPoints;
                        result.ResetFrames();
                        result.FrameRate = FrameRate;
                        result.SmoothMotion = SmoothMotion;
                        result.GenerateResults();
                        result.VideoOutcome = SingleFileResult.Ok;
                        result.DataLoadComplete();
                        allResults.TryAdd(file, result);
                        
                        save.SaveFile(artFile, videoFile, result);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine(e.Message);
                    result.VideoOutcome = SingleFileResult.Error;
                    result.Message = "Exception: " + file.VideoFileName + " - " + e.Message + " - " + e.StackTrace;
                    result.DataLoadComplete();
                    allResults.TryAdd(file, result);
                    UpdateProgress(file, 1);
                    save.SaveFile(artFile, videoFile, result);
                }

                if (Cancel)
                {
                    state.Break();
                    //break;
                }

                if (Stop)
                {
                    state.Stop();
                    //return;
                }
            });

            if (Stop)
            {
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    Results = allResults.ToDictionary(x => x.Key, x => x.Value);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in single mouse viewmodel!!! " + e.InnerException);
                }
            });
        }

        private double CalculateOtsu(Image<Gray, Byte> image)
        {
            using (Image<Gray, Byte> tempOriginal = image.CopyBlank())
            {
                return CvInvoke.Threshold(image, tempOriginal, 0, 255, ThresholdType.Otsu | ThresholdType.Binary);
            }
        }

        private void ReviewFiles()
        {
            ReviewWindowViewModel viewModel = new ReviewWindowViewModel(Model, Results);
            ReviewView view = new ReviewView();
            view.DataContext = viewModel;
            view.ShowDialog();
        }

        private bool CanReviewFiles()
        {
            if (Results == null)
            {
                return false;
            }

            if (Results.Count == 0)
            {
                return false;
            }

            return true;
        }

        public void UpdateMaxDegreesOfParallelism(int newMaxDegrees)
        {
            if (newMaxDegrees < 1)
            {
                throw new Exception("Max Degrees of Parallelism must be greater than 0");
            }

            ParallelOptions.MaxDegreeOfParallelism = newMaxDegrees;
        }

        private void ShowSettings()
        {
            
        }

        private bool CanShowSettings()
        {
            return true;
        }
    }
}
