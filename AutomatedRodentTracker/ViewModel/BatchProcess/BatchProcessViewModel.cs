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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using AutomatedRodentTracker.Commands;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.Model.Results;
using AutomatedRodentTracker.Model.Video;
using AutomatedRodentTracker.ModelInterface.Behaviours;
using AutomatedRodentTracker.ModelInterface.Boundries;
using AutomatedRodentTracker.Repository;
using AutomatedRodentTracker.RepositoryInterface;
using AutomatedRodentTracker.Services.FileBrowser;
using AutomatedRodentTracker.View.BatchProcess.Export;
using AutomatedRodentTracker.ViewModel.BatchProcess.BatchExport;
using AutomatedRodentTracker.ViewModel.Datasets;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using AutomatedRodentTracker.Model.Datasets;
using AutomatedRodentTracker.Model.VideoSettings;
using AutomatedRodentTracker.ModelInterface.Datasets;
using AutomatedRodentTracker.ModelInterface.Datasets.Types;
using AutomatedRodentTracker.ModelInterface.RBSK;
using AutomatedRodentTracker.ModelInterface.Results;
using AutomatedRodentTracker.ModelInterface.Video;
using AutomatedRodentTracker.ModelInterface.VideoSettings;
using AutomatedRodentTracker.Services.Excel;
using AutomatedRodentTracker.View.Inputs;
using AutomatedRodentTracker.View.Settings;
using AutomatedRodentTracker.ViewModel.NewWizard;
using AutomatedRodentTracker.ViewModel.Settings;
using Point = System.Drawing.Point;

namespace AutomatedRodentTracker.ViewModel.BatchProcess
{
    public class BatchProcessViewModel : WindowViewModelBase
    {
        private ActionCommand m_AddTgFileCommand;
        private ActionCommand m_AddTgFolderCommand;
        private ActionCommand m_RemoveTgFileCommand;
        private ActionCommand m_ClearTgFilesCommand;
        private ActionCommand m_ProcessCommand;
        private ActionCommand m_SetOutputFolderCommand;
        private ActionCommand m_LoadOutputFolderCommand;
        private ActionCommand m_ExportAllCommand;
        private ActionCommand m_BatchSettingsCommand;
        //private ActionCommandWithParameter m_ClosingCommand;

        public ActionCommand AddTgFileCommand
        {
            get
            {
                return m_AddTgFileCommand ?? (m_AddTgFileCommand = new ActionCommand()
                {
                    ExecuteAction = AddTgFile,
                    CanExecuteAction = CanButtonExecute,
                });
            }
        }

        public ActionCommand AddTgFolderCommand
        {
            get
            {
                return m_AddTgFolderCommand ?? (m_AddTgFolderCommand = new ActionCommand()
                {
                    ExecuteAction = AddTgFolder,
                    CanExecuteAction = CanButtonExecute,
                });
            }
        }

        public ActionCommand RemoveTgFileCommand
        {
            get
            {
                return m_RemoveTgFileCommand ?? (m_RemoveTgFileCommand = new ActionCommand()
                {
                    ExecuteAction = RemoveTgFile,
                    CanExecuteAction = CanRemoveTgFile,
                });
            }
        }

        public ActionCommand ClearTgFilesCommand
        {
            get
            {
                return m_ClearTgFilesCommand ?? (m_ClearTgFilesCommand = new ActionCommand()
                {
                    ExecuteAction = ClearTgList,
                    CanExecuteAction = CanButtonExecute,
                });
            }
        }

        public ActionCommand ProcessCommand
        {
            get
            {
                return m_ProcessCommand ?? (m_ProcessCommand = new ActionCommand()
                {
                    ExecuteAction = ProcessVideos,
                    CanExecuteAction = CanProcessVideos,
                });
            }
        }

        public ActionCommand SetOutputFolderCommand
        {
            get
            {
                return m_SetOutputFolderCommand ?? (m_SetOutputFolderCommand = new ActionCommand()
                {
                    ExecuteAction = SetOutputFolder,
                    CanExecuteAction = CanSetOutputFolder,
                });
            }
        }

        public ActionCommand LoadOutputFolderCommand
        {
            get
            {
                return m_LoadOutputFolderCommand ?? (m_LoadOutputFolderCommand = new ActionCommand()
                {
                    ExecuteAction = LoadOutputFolder,
                    CanExecuteAction = CanLoadOutputFolder,
                });
            }
        }

        public ActionCommand ExportAllCommand
        {
            get
            {
                return m_ExportAllCommand ?? (m_ExportAllCommand = new ActionCommand()
                {
                    ExecuteAction = ExportAll,
                    CanExecuteAction = CanButtonExecute,
                });
            }
        }

        public ActionCommand BatchSettingsCommand
        {
            get
            {
                return m_BatchSettingsCommand ?? (m_BatchSettingsCommand = new ActionCommand()
                {
                    ExecuteAction = ShowBatchSettings,
                    CanExecuteAction = CanShowBatchSettings,
                });
            }
        }

        private ObservableCollection<SingleMouseViewModel> m_TgItemsSource;
        private SingleMouseViewModel m_SelectedTgItem;

        public static object TgLock = new object();
        public ObservableCollection<SingleMouseViewModel> TgItemsSource
        {
            get
            {
                lock (TgLock)
                {
                    return m_TgItemsSource;
                }
            }
            set
            {
                if (Equals(m_TgItemsSource, value))
                {
                    return;
                }

                m_TgItemsSource = value;

                NotifyPropertyChanged();
                LoadOutputFolderCommand.RaiseCanExecuteChangedNotification();
                BatchSettingsCommand.RaiseCanExecuteChangedNotification();
            }
        }

        public SingleMouseViewModel SelectedTgItem
        {
            get
            {
                return m_SelectedTgItem;
            }
            set
            {
                if (Equals(m_SelectedTgItem, value))
                {
                    return;
                }

                m_SelectedTgItem = value;

                NotifyPropertyChanged();
                RemoveTgFileCommand.RaiseCanExecuteChangedNotification();
            }
        }
        

        private bool m_Running = false;
        public bool Running
        {
            get
            {
                return m_Running;
            }
            set
            {
                if (Equals(m_Running, value))
                {
                    return;
                }

                m_Running = value;

                NotifyPropertyChanged();
                RaiseButtonNotifications();
            }
        }

        private string m_OutputFolder = "";
        public string OutputFolder
        {
            get
            {
                return m_OutputFolder;
            }
            set
            {
                if (Equals(m_OutputFolder, value))
                {
                    return;
                }

                m_OutputFolder = value;

                NotifyPropertyChanged();
            }
        }

        public BatchProcessViewModel()
        {
            //LookForLabbook();
            ResetData();
        }

        private void ResetData()
        {
            ClearTgList();
        }

        private void ClearTgList()
        {
            SelectedTgItem = null;
            TgItemsSource = new ObservableCollection<SingleMouseViewModel>();
            BatchSettingsCommand.RaiseCanExecuteChangedNotification();
        }

        private void AddTgFile()
        {
            string fileLocation = FileBrowser.BroseForVideoFiles();

            if (string.IsNullOrWhiteSpace(fileLocation))
            {
                return;
            }


            ISingleMouse newFile = ModelResolver.Resolve<ISingleMouse>();
            newFile.AddFile(GetSingleFile(fileLocation));
            newFile.Name = Path.GetFileNameWithoutExtension(fileLocation);

            SingleMouseViewModel viewModel = new SingleMouseViewModel(newFile);

            ObservableCollection<SingleMouseViewModel> currentList = new ObservableCollection<SingleMouseViewModel>(TgItemsSource);
            currentList.Add(viewModel);
            TgItemsSource = currentList;
        }

        private ISingleFile GetSingleFile(string fileName)
        {
            ISingleFile singleFile = ModelResolver.Resolve<ISingleFile>();

            singleFile.VideoFileName = fileName;
            singleFile.VideoNumber = Path.GetFileNameWithoutExtension(fileName);

            return singleFile;
        }

        private void AddTgFolder()
        {
            string folderLocation = FileBrowser.BrowseForFolder();

            if (string.IsNullOrWhiteSpace(folderLocation))
            {
                return;
            }

            ObservableCollection<SingleMouseViewModel> currentList = new ObservableCollection<SingleMouseViewModel>(TgItemsSource);

            string[] files = Directory.GetFiles(folderLocation);

            foreach (string file in files)
            {
                string extension = Path.GetExtension(file);

                if (CheckIfExtensionIsVideo(extension))
                {
                    ISingleMouse newFile = ModelResolver.Resolve<ISingleMouse>();
                    newFile.AddFile(GetSingleFile(file));
                    newFile.Name = Path.GetFileNameWithoutExtension(file);
                    currentList.Add(new SingleMouseViewModel(newFile));
                }
            }

            TgItemsSource = currentList;
        }

        private bool CheckIfExtensionIsVideo(string extension)
        {
            return extension.Contains("avi") || extension.Contains("mpg") || extension.Contains("mpeg") || extension.Contains("mp4") || extension.Contains("mov");
        }

        private void RemoveTgFile()
        {
            TgItemsSource.Remove(SelectedTgItem);
            NotifyPropertyChanged("TgItemsSource");
            BatchSettingsCommand.RaiseCanExecuteChangedNotification();
        }

        private bool CanRemoveTgFile()
        {
            if (SelectedTgItem == null || Running)
            {
                return false;
            }

            return true;
        }
        
        private static object lockObject = new object();
        private bool m_Continue = true;

        public bool Continue
        {
            get
            {
                lock (lockObject)
                {
                    return m_Continue;
                }
            }
            set
            {
                lock (lockObject)
                {
                    m_Continue = value;
                }
            }
        }

        private void RunVideo()
        {
            IEnumerable<SingleMouseViewModel> allMice = TgItemsSource;

            Parallel.ForEach(allMice, new ParallelOptions() {MaxDegreeOfParallelism = 6}, (mouse, state) =>
            //foreach (SingleMouseViewModel mouse in allMice)
            {
                if (!Running)
                {
                    return;
                }

                mouse.RunFiles(OutputFolder);
            });

            Application.Current.Dispatcher.Invoke(() =>
            {
                Running = false;
                ExportAll();
            });
        }

        private void GenerateAllResults()
        {
            string saveLocation = FileBrowser.SaveFile("Excel|*.xlsx");

            if (string.IsNullOrWhiteSpace(saveLocation))
            {
                return;
            }

            List<IMouseDataResult> tgResultsList = new List<IMouseDataResult>();
            foreach (SingleMouseViewModel mouse in TgItemsSource)
            {
                tgResultsList.AddRange(mouse.VideoFiles.Select(singleFile => mouse.Results[singleFile]));
            }

            IMouseDataResult[] tgResults = tgResultsList.ToArray();

            int rows = tgResults.Length + 10;
            const int columns = 14;

            object[,] finalResults = new object[rows, columns];

            finalResults[0, 0] = "Name";
            finalResults[0, 1] = "Start Frame";
            finalResults[0, 2] = "End Frame";
            finalResults[0, 3] = "Duration";
            finalResults[0, 4] = "Distance";
            finalResults[0, 5] = "Centroid Distance";
            finalResults[0, 6] = "Max Velocity";
            finalResults[0, 7] = "Max Centroid Velocity";
            finalResults[0, 8] = "Max Ang Velocity";
            finalResults[0, 9] = "Average Velocity";
            finalResults[0, 10] = "Average Centroid Velocity";
            finalResults[0, 11] = "Average Angular Velocity";
            
            try
            {
                int tgLength = tgResults.Length;
                for (int i = 1; i <= tgLength; i++)
                {
                    finalResults[i, 0] = tgResults[i - 1].Name;
                    finalResults[i, 1] = tgResults[i - 1].StartFrame;
                    finalResults[i, 2] = tgResults[i - 1].EndFrame;
                    finalResults[i, 3] = tgResults[i - 1].Duration;
                    finalResults[i, 4] = tgResults[i - 1].DistanceTravelled;
                    finalResults[i, 5] = tgResults[i - 1].CentroidDistanceTravelled;
                    finalResults[i, 6] = tgResults[i - 1].MaxSpeed;
                    finalResults[i, 7] = tgResults[i - 1].MaxCentroidSpeed;
                    finalResults[i, 8] = tgResults[i - 1].MaxAngularVelocty;
                    finalResults[i, 9] = tgResults[i - 1].AverageVelocity;
                    finalResults[i, 10] = tgResults[i - 1].AverageCentroidVelocity;
                    finalResults[i, 11] = tgResults[i - 1].AverageAngularVelocity;
                }

                ExcelService.WriteData(finalResults, saveLocation);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //try
            //{
            //    GenerateBatchResults();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
        }

        public struct BrettTuple<T1, T2>
        {
            public readonly T1 Item1;
            public readonly T2 Item2;

            public BrettTuple(T1 item1, T2 item2)
            {
                Item1 = item1;
                Item2 = item2;
            }
        }

        private bool CanProcessVideos()
        {
            return !Running;
        }

        private void ProcessVideos()
        {
            if (!Running)
            {
                ResetProgress();

                Running = true;
                Task.Factory.StartNew(RunVideo);
                //RunVideo();
            }
            else
            {
                Continue = false;
                Running = false;
            }
        }

        private void ResetProgress()
        {
            foreach (var mouse in TgItemsSource)
            {
                mouse.ResetProgress();
            }
        }

        private void SetOutputFolder()
        {
            string folderLocation = FileBrowser.BrowseForFolder();

            if (string.IsNullOrWhiteSpace(folderLocation))
            {
                return;
            }

            OutputFolder = folderLocation;
        }

        private bool CanSetOutputFolder()
        {
            return !Running;
        }

        protected override void WindowClosing(CancelEventArgs closingEventArgs)
        {
            if (!Running)
            {
                return;
            }

            var result = MessageBox.Show("The program is currently running, are you sure you want to cancel it?", "Batch Running", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                IEnumerable<SingleMouseViewModel> allMice = TgItemsSource;
                foreach (SingleMouseViewModel mouse in allMice)
                {
                    mouse.Stop = true;
                }
                Running = false;
                return;
            }

            if (closingEventArgs != null)
            {
                closingEventArgs.Cancel = true;
            }
        }

        //private void Closing(object args)
        //{
        //    if (!Running)
        //    {
        //        return;
        //    }

        //    var result = MessageBox.Show("The program is currently running, are you sure you want to cancel it?", "Batch Running", MessageBoxButton.YesNo, MessageBoxImage.Question);

        //    if (result == MessageBoxResult.Yes)
        //    {
        //        IEnumerable<SingleMouseViewModel> allMice = TgItemsSource.Concat(NtgItemsSource);
        //        foreach (SingleMouseViewModel mouse in allMice)
        //        {
        //            mouse.Stop = true;
        //        }
        //        Running = false;
        //        return;
        //    }

        //    CancelEventArgs cancelEventArgs = args as CancelEventArgs;
        //    if (cancelEventArgs != null)
        //    {
        //        cancelEventArgs.Cancel = true;
        //    }
        //}

        private void LoadOutputFolder()
        {
            IRepository repo = RepositoryResolver.Resolve<IRepository>();
            string initialLocation = repo.GetValue<string>("OutputFolderLocation");

            string folderLocation = FileBrowser.BrowseForFolder(initialLocation);

            if (string.IsNullOrWhiteSpace(folderLocation))
            {
                return;
            }

            repo.SetValue("OutputFolderLocation", folderLocation);
            repo.Save();

            string[] artFiles = Directory.GetFiles(folderLocation, "*.art");

            foreach (string file in artFiles)
            //Parallel.ForEach(artFiles, file =>
            {
                string videoFileName = Path.GetFileNameWithoutExtension(file);

                //if (videoFileName.Contains("091119-0038"))
                //{
                //    Console.WriteLine("h");
                //}

                if (string.IsNullOrWhiteSpace(videoFileName))
                {
                    continue;
                    //return;
                }

                //if (videoFileName.Contains("091218-0179"))
                //{
                //    Console.WriteLine("test");
                //}

                //SingleMouseViewModel mouse = null;
                ////videoFileName = file.Replace(".art", "");
                //bool testBreak = false;
                //foreach (var listMouse in TgItemsSource)
                //{
                //    foreach (var temp in listMouse.VideoFiles)
                //    {
                //        if (temp.VideoFileName.Contains(videoFileName))
                //        {
                //            mouse = listMouse;
                //            testBreak = true;
                //            break;
                //        }
                //    }

                //    if (testBreak)
                //    {
                //        break;
                //    }
                //}

                SingleMouseViewModel mouse = TgItemsSource.FirstOrDefault(x => x.VideoFiles.FirstOrDefault(y => y.VideoFileName.Contains(videoFileName)) != null);
                
                if (mouse == null)
                {
                    //Can't find mouse
                    //return;
                    continue;
                }

                ISingleFile singleFile = mouse.VideoFiles.First(x => x.VideoFileName.Contains(videoFileName));

                XmlSerializer serializer = new XmlSerializer(typeof (TrackedVideoXml));
                TrackedVideoXml trackedVideoXml;
                using (StreamReader reader = new StreamReader(file))
                {
                    trackedVideoXml = (TrackedVideoXml) serializer.Deserialize(reader);
                }

                ITrackedVideo trackedVideo = trackedVideoXml.GetData();

                IMouseDataResult mouseDataResult = ModelResolver.Resolve<IMouseDataResult>();

                if (trackedVideo.Result != SingleFileResult.Ok)
                {
                    mouseDataResult.Message = trackedVideo.Message;
                    mouseDataResult.VideoOutcome = trackedVideo.Result;

                    if (mouse.Results == null)
                    {
                        mouse.Results = new Dictionary<ISingleFile, IMouseDataResult>();
                    }
                    mouse.Results.Add(singleFile, mouseDataResult);
                    mouse.ReviewCommand.RaiseCanExecuteChangedNotification();
                    mouse.UpdateProgress(singleFile, 1);
                    
                    continue;
                }

                var tempResults = trackedVideo.Results;
                Dictionary<int, ISingleFrameResult> results = new Dictionary<int, ISingleFrameResult>();
                foreach (var kvp in tempResults)
                {
                    ISingleFrameResult singleFrame = ModelResolver.Resolve<ISingleFrameResult>();

                    if (kvp.Value != null)
                    {
                        ISingleFrameResult tempResult = kvp.Value;
                        singleFrame.HeadPoints = tempResult.HeadPoints;
                        singleFrame.CentroidSize = tempResult.CentroidSize;
                        singleFrame.PelvicArea = tempResult.PelvicArea;
                        singleFrame.PelvicArea2 = tempResult.PelvicArea2;
                        singleFrame.PelvicArea3 = tempResult.PelvicArea3;
                        singleFrame.PelvicArea4 = tempResult.PelvicArea4;
                        singleFrame.SmoothedHeadPoint = tempResult.SmoothedHeadPoint;
                        singleFrame.Centroid = tempResult.Centroid;
                    }

                    results.Add(kvp.Key, singleFrame);
                }
                

                mouseDataResult.Name = mouse.Id;
                mouseDataResult.Results = results;
                mouseDataResult.Age = mouse.Age;
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
                mouseDataResult.SmoothFactor = 0.68;
                mouseDataResult.GenerateResults(file);
                mouseDataResult.PelvicArea = trackedVideo.PelvicArea1;
                mouseDataResult.PelvicArea2 = trackedVideo.PelvicArea2;
                mouseDataResult.PelvicArea3 = trackedVideo.PelvicArea3;
                mouseDataResult.PelvicArea4 = trackedVideo.PelvicArea4;
                mouseDataResult.Type = mouse.Type;
                mouseDataResult.DataLoadComplete();

                if (mouse.Results == null)
                {
                    mouse.Results = new Dictionary<ISingleFile, IMouseDataResult>();
                }

                mouse.UpdateProgress(singleFile, 1);
                mouse.Results.Add(singleFile, mouseDataResult);
                mouse.ReviewCommand.RaiseCanExecuteChangedNotification();
            }//);

            ExportAllCommand.RaiseCanExecuteChangedNotification();
        }

        private void ExportAll()
        {
            var msgBoxResult = MessageBox.Show("Would you like to export the results?", "Save results?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (msgBoxResult == MessageBoxResult.Yes)
            {
                GenerateAllResults();
            }
        }

        private bool CanLoadOutputFolder()
        {
            //return !string.IsNullOrWhiteSpace()
            if (Running)
            {
                return false;
            }

            return TgItemsSource.Count > 0;
        }

        private void ShowBatchSettings()
        {
            SingleMouseViewModel[] selectedModels = GetSelectedViewModels();

            if (!selectedModels.Any())
            {
                selectedModels = TgItemsSource.ToArray();
            }
            
            SettingsView view = new SettingsView();
            SettingsViewModel viewModel = new SettingsViewModel(selectedModels);
            view.DataContext = viewModel;
            view.ShowDialog();

            if (viewModel.ExitResult != WindowExitResult.Ok)
            {
                return;
            }

            foreach (var mouse in selectedModels)
            {
                mouse.GapDistance = viewModel.GapDistance;
                mouse.ThresholdValue = viewModel.BinaryThreshold;
                mouse.ThresholdValue2 = viewModel.BinaryThreshold2;
                mouse.SmoothMotion = viewModel.SmoothMotion;
                mouse.FrameRate = viewModel.FrameRate;
            }
        }

        private SingleMouseViewModel[] GetSelectedViewModels()
        {
            return TgItemsSource.Where(x => x.IsSelected).ToArray();
        }

        private void ExportBatch()
        {
            BatchExportViewModel viewModel = new BatchExportViewModel(TgItemsSource);
            BatchExportView view = new BatchExportView();
            view.DataContext = viewModel;
            view.ShowDialog();
        }
        

        private bool CanShowBatchSettings()
        {
            if (Running)
            {
                return false;
            }

            return TgItemsSource.Any();
        }

        private void RaiseButtonNotifications()
        {
            AddTgFileCommand.RaiseCanExecuteChangedNotification();
            AddTgFolderCommand.RaiseCanExecuteChangedNotification();
            RemoveTgFileCommand.RaiseCanExecuteChangedNotification();
            ClearTgFilesCommand.RaiseCanExecuteChangedNotification();
            ProcessCommand.RaiseCanExecuteChangedNotification();
            SetOutputFolderCommand.RaiseCanExecuteChangedNotification();
            LoadOutputFolderCommand.RaiseCanExecuteChangedNotification();
            ExportAllCommand.RaiseCanExecuteChangedNotification();
            BatchSettingsCommand.RaiseCanExecuteChangedNotification();
        }

        private bool CanButtonExecute()
        {
            return !Running;
        }
    }
}
