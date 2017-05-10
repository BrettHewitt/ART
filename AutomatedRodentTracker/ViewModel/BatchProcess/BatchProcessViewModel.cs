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
        private ActionCommand m_AddNtgFileCommand;
        private ActionCommand m_AddNtgFolderCommand;
        private ActionCommand m_RemoveNtgFileCommand;
        private ActionCommand m_ClearNtgFilesCommand;
        private ActionCommand m_ProcessCommand;
        private ActionCommand m_LoadLabbookCommand;
        private ActionCommand m_SetOutputFolderCommand;
        private ActionCommand m_LoadOutputFolderCommand;
        private ActionCommand m_ExportAllCommand;
        private ActionCommand m_BatchSettingsCommand;
        //private ActionCommandWithParameter m_ClosingCommand;
        private ActionCommand m_ExportBatchCommand;

        public ActionCommand AddTgFileCommand
        {
            get
            {
                return m_AddTgFileCommand ?? (m_AddTgFileCommand = new ActionCommand()
                {
                    ExecuteAction = AddTgFile,
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
                });
            }
        }

        public ActionCommand AddNtgFileCommand
        {
            get
            {
                return m_AddNtgFileCommand ?? (m_AddNtgFileCommand = new ActionCommand()
                {
                    ExecuteAction = AddNtgFile,
                });
            }
        }

        public ActionCommand AddNtgFolderCommand
        {
            get
            {
                return m_AddNtgFolderCommand ?? (m_AddNtgFolderCommand = new ActionCommand()
                {
                    ExecuteAction = AddNtgFolder,
                });
            }
        }

        public ActionCommand RemoveNtgFileCommand
        {
            get
            {
                return m_RemoveNtgFileCommand ?? (m_RemoveNtgFileCommand = new ActionCommand()
                {
                    ExecuteAction = RemoveNtgFile,
                    CanExecuteAction = CanRemoveNtgFile,
                });
            }
        }

        public ActionCommand ClearNtgFilesCommand
        {
            get
            {
                return m_ClearNtgFilesCommand ?? (m_ClearNtgFilesCommand = new ActionCommand()
                {
                    ExecuteAction = ClearNtgList,
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

        public ActionCommand LoadLabbookCommand
        {
            get
            {
                return m_LoadLabbookCommand ?? (m_LoadLabbookCommand = new ActionCommand()
                {
                    ExecuteAction = LoadLabbook,
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
                });
            }
        }

        public ActionCommand ExportBatchCommand
        {
            get
            {
                return m_ExportBatchCommand ?? (m_ExportBatchCommand = new ActionCommand()
                {
                    ExecuteAction = ExportBatch,
                    CanExecuteAction = CanExportBatch,
                });
            }
        }

        private ObservableCollection<SingleMouseViewModel> m_TgItemsSource;
        private SingleMouseViewModel m_SelectedTgItem;

        private ObservableCollection<SingleMouseViewModel> m_NtgItemsSource;
        private SingleMouseViewModel m_SelectedNtgItem;

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

        public static object NtgLock = new object();
        public ObservableCollection<SingleMouseViewModel> NtgItemsSource
        {
            get
            {
                lock (NtgLock)
                {
                    return m_NtgItemsSource;
                }
            }
            set
            {
                if (Equals(m_NtgItemsSource, value))
                {
                    return;
                }

                m_NtgItemsSource = value;

                NotifyPropertyChanged();
                LoadOutputFolderCommand.RaiseCanExecuteChangedNotification();
            }
        }

        public SingleMouseViewModel SelectedNtgItem
        {
            get
            {
                return m_SelectedNtgItem;
            }
            set
            {
                if (Equals(m_SelectedNtgItem, value))
                {
                    return;
                }

                m_SelectedNtgItem = value;

                NotifyPropertyChanged();
                RemoveNtgFileCommand.RaiseCanExecuteChangedNotification();
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
                ProcessCommand.RaiseCanExecuteChangedNotification();
                SetOutputFolderCommand.RaiseCanExecuteChangedNotification();
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
            ClearNtgList();
            ClearTgList();
        }

        private void ClearTgList()
        {
            SelectedTgItem = null;
            TgItemsSource = new ObservableCollection<SingleMouseViewModel>();
        }

        private void ClearNtgList()
        {
            SelectedNtgItem = null;
            NtgItemsSource = new ObservableCollection<SingleMouseViewModel>();
        }

        private void AddTgFile()
        {
            string fileLocation = FileBrowser.BroseForVideoFiles();

            if (string.IsNullOrWhiteSpace(fileLocation))
            {
                return;
            }

            ISingleMouse newFile = ModelResolver.Resolve<ISingleMouse>();
            SingleMouseViewModel viewModel = new SingleMouseViewModel(newFile);
            //newFile.VideoFileName = fileLocation;

            ObservableCollection<SingleMouseViewModel> currentList = new ObservableCollection<SingleMouseViewModel>(TgItemsSource);
            currentList.Add(viewModel);
            TgItemsSource = currentList;
        }

        private void LoadLabbook()
        {
            LookForLabbook();
        }

        private void LookForLabbook()
        {
            IRepository repo = RepositoryResolver.Resolve<IRepository>();
            string initialLocation = repo.GetValue<string>("LabbookLocation");

            string fileLocation = FileBrowser.BrowseForFile("Mouse Collection|*.mcd", initialLocation);

            if (string.IsNullOrWhiteSpace(fileLocation))
            {
                return;
            }

            repo.SetValue("LabbookLocation", Path.GetDirectoryName(fileLocation));
            repo.Save();

            XmlSerializer serializer = new XmlSerializer(typeof(MouseCollectionXml));
            MouseCollectionXml mc;

            try
            {
                using (StreamReader reader = new StreamReader(fileLocation))
                {
                    mc = (MouseCollectionXml)serializer.Deserialize(reader);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("There was an error opening the file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ObservableCollection<SingleMouseViewModel> tgList = new ObservableCollection<SingleMouseViewModel>(TgItemsSource);
            ObservableCollection<SingleMouseViewModel> ntgList = new ObservableCollection<SingleMouseViewModel>(NtgItemsSource);

            foreach (SingleMouseXml mouse in mc.Mice)
            {
                ISingleMouse modelMouse = mouse.GetMouse();

                if (modelMouse == null)
                {
                    continue;
                }

                ITypeBase type = modelMouse.Type;

                if (type is INonTransgenic)
                {
                    ntgList.Add(new SingleMouseViewModel(modelMouse));
                }
                else if (type is ITransgenic)
                {
                    tgList.Add(new SingleMouseViewModel(modelMouse));
                }
                else if (type is IUndefined)
                {
                    
                }
            }

            TgItemsSource = tgList;
            NtgItemsSource = ntgList;

            //ObservableCollection<SingleMouseViewModel> mice2 = new ObservableCollection<SingleMouseViewModel>();
            //string fileLocation2 = Path.GetDirectoryName(test);
            //foreach (var mouse in mc.Mice)
            //{
            //    mice2.Add(new SingleMouseViewModel(mouse.GetMouse()));
            //}

            //var result = MessageBox.Show("Is there a lab book associated with this set?", "Browse for lab book?", MessageBoxButton.YesNo, MessageBoxImage.Question);

            //if (result != MessageBoxResult.Yes)
            //{
            //    return;
            //}

            //AgeSingleInputViewModel viewModel = new AgeSingleInputViewModel();
            //SingleInput view = new SingleInput();
            //view.DataContext = viewModel;
            //view.ShowDialog();

            //if (viewModel.ExitResult != WindowExitResult.Ok)
            //{
            //    return;
            //}

            //int age = viewModel.Age;

            //var fileLocation = FileBrowser.BrowseForFile("Labbook|*.labbook");

            //if (string.IsNullOrWhiteSpace(fileLocation))
            //{
            //    return;
            //}

            //string[] lines = File.ReadAllLines(fileLocation);

            //ILabbookConverter converter = ModelResolver.Resolve<ILabbookConverter>();
            //List<ILabbookData> data = converter.GenerateLabbookData(lines, age);
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
                    //newFile.VideoFileName = file;
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
        }

        private bool CanRemoveTgFile()
        {
            if (SelectedTgItem == null)
            {
                return false;
            }

            return true;
        }

        private void AddNtgFile()
        {
            string fileLocation = FileBrowser.BroseForVideoFiles();

            if (string.IsNullOrWhiteSpace(fileLocation))
            {
                return;
            }

            ISingleMouse newFile = ModelResolver.Resolve<ISingleMouse>();
           // newFile.VideoFileName = fileLocation;

            ObservableCollection<SingleMouseViewModel> currentList = new ObservableCollection<SingleMouseViewModel>(NtgItemsSource);
            currentList.Add(new SingleMouseViewModel(newFile));
            NtgItemsSource = currentList;
        }

        private void AddNtgFolder()
        {
            string folderLocation = FileBrowser.BrowseForFolder();

            if (string.IsNullOrWhiteSpace(folderLocation))
            {
                return;
            }

            ObservableCollection<SingleMouseViewModel> currentList = new ObservableCollection<SingleMouseViewModel>(NtgItemsSource);

            string[] files = Directory.GetFiles(folderLocation);

            foreach (string file in files)
            {
                string extension = Path.GetExtension(file);

                if (CheckIfExtensionIsVideo(extension))
                {
                    ISingleMouse newFile = ModelResolver.Resolve<ISingleMouse>();
                    //newFile.VideoFileName = file;
                    currentList.Add(new SingleMouseViewModel(newFile));
                }
            }

            NtgItemsSource = currentList;
        }

        private void RemoveNtgFile()
        {
            NtgItemsSource.Remove(SelectedNtgItem);
            NotifyPropertyChanged("NtgItemsSource");
        }

        private bool CanRemoveNtgFile()
        {
            if (SelectedNtgItem == null)
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
            IEnumerable<SingleMouseViewModel> allMice = TgItemsSource.Concat(NtgItemsSource);

            Parallel.ForEach(allMice, new ParallelOptions() {MaxDegreeOfParallelism = 3}, (mouse, state) =>
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
                ExportBatchCommand.RaiseCanExecuteChangedNotification();
            });
        }

        private void GenerateAllResults()
        {
            List<IMouseDataResult> tgResultsList = new List<IMouseDataResult>();
            foreach (SingleMouseViewModel mouse in TgItemsSource)
            {
                tgResultsList.AddRange(mouse.VideoFiles.Select(singleFile => mouse.Results[singleFile]));
            }

            List<IMouseDataResult> ntgResultsList = new List<IMouseDataResult>();
            foreach (SingleMouseViewModel mouse in NtgItemsSource)
            {
                ntgResultsList.AddRange(mouse.VideoFiles.Select(singleFile => mouse.Results[singleFile]));
            }

            IMouseDataResult[] tgResults = tgResultsList.OrderBy(x => x.Age).ToArray();
            IMouseDataResult[] ntgResults = ntgResultsList.OrderBy(x => x.Age).ToArray();

            int rows = tgResults.Length + ntgResults.Length + 10;
            const int columns = 14;

            object[,] finalResults = new object[rows, columns];

            finalResults[0, 0] = "Name";
            finalResults[0, 1] = "Type";
            finalResults[0, 2] = "Age";
            finalResults[0, 3] = "Waist";
            finalResults[0, 4] = "Duration";
            finalResults[0, 5] = "Distance";
            finalResults[0, 6] = "Max Velocity";
            finalResults[0, 7] = "Max Ang Velocity";
            finalResults[0, 8] = "Average Velocity";
            finalResults[0, 9] = "Average Ang Velocity";
            finalResults[0, 10] = "Average Pelvic area";
            finalResults[0, 11] = "Average Pelvic area2";
            finalResults[0, 12] = "Average Pelvic area3";
            finalResults[0, 13] = "Average Pelvic area4";

            try
            {
                int tgLength = tgResults.Length;
                for (int i = 1; i <= tgLength; i++)
                {
                    finalResults[i, 0] = tgResults[i - 1].Name;
                    finalResults[i, 1] = "Transgenic";
                    finalResults[i, 2] = tgResults[i - 1].Age;
                    finalResults[i, 3] = tgResults[i - 1].CentroidSize;
                    finalResults[i, 4] = tgResults[i - 1].Duration;
                    finalResults[i, 5] = tgResults[i - 1].DistanceTravelled;
                    finalResults[i, 6] = tgResults[i - 1].MaxSpeed;
                    finalResults[i, 7] = tgResults[i - 1].MaxAngularVelocty;
                    finalResults[i, 8] = tgResults[i - 1].AverageVelocity;
                    finalResults[i, 9] = tgResults[i - 1].AverageAngularVelocity;
                    finalResults[i, 10] = tgResults[i - 1].PelvicArea;
                    finalResults[i, 11] = tgResults[i - 1].PelvicArea2;
                    finalResults[i, 12] = tgResults[i - 1].PelvicArea3;
                    finalResults[i, 13] = tgResults[i - 1].PelvicArea4;
                }

                for (int i = 1; i <= ntgResults.Length; i++)
                {
                    finalResults[i + tgLength, 0] = ntgResults[i - 1].Name;
                    finalResults[i + tgLength, 1] = "Non Transgenic";
                    finalResults[i + tgLength, 2] = ntgResults[i - 1].Age;
                    finalResults[i + tgLength, 3] = ntgResults[i - 1].CentroidSize;
                    finalResults[i + tgLength, 4] = ntgResults[i - 1].Duration;
                    finalResults[i + tgLength, 5] = ntgResults[i - 1].DistanceTravelled;
                    finalResults[i + tgLength, 6] = ntgResults[i - 1].MaxSpeed;
                    finalResults[i + tgLength, 7] = ntgResults[i - 1].MaxAngularVelocty;
                    finalResults[i + tgLength, 8] = ntgResults[i - 1].AverageVelocity;
                    finalResults[i + tgLength, 9] = ntgResults[i - 1].AverageAngularVelocity;
                    finalResults[i + tgLength, 10] = ntgResults[i - 1].PelvicArea;
                    finalResults[i + tgLength, 11] = ntgResults[i - 1].PelvicArea2;
                    finalResults[i + tgLength, 12] = ntgResults[i - 1].PelvicArea3;
                    finalResults[i + tgLength, 13] = ntgResults[i - 1].PelvicArea4;
                }

                ExcelService.WriteData(finalResults, @"D:\FirstTestResults9.xlsx");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                GenerateBatchResults();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
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

        private void GenerateBatchResults()
        {
            Dictionary<BrettTuple<Type, int>, IMouseDataResult> sortedResults = new Dictionary<BrettTuple<Type, int>, IMouseDataResult>(); 

            List<IMouseDataResult> tgResultsList = new List<IMouseDataResult>();
            foreach (SingleMouseViewModel mouse in TgItemsSource)
            {
                tgResultsList.AddRange(mouse.VideoFiles.Select(singleFile => mouse.Results[singleFile]));
            }

            List<IMouseDataResult> ntgResultsList = new List<IMouseDataResult>();
            foreach (SingleMouseViewModel mouse in NtgItemsSource)
            {
                ntgResultsList.AddRange(mouse.VideoFiles.Select(singleFile => mouse.Results[singleFile]));
            }

            //IMouseDataResult[] tgResults = tgResultsList.OrderBy(x => x.Age).ToArray();
            //IMouseDataResult[] ntgResults = ntgResultsList.OrderBy(x => x.Age).ToArray();

            try
            {
                int tgLength = tgResultsList.Count;
                
                for (int i = 1; i <= tgLength; i++)
                {
                    BrettTuple<Type, int> currentResult = new BrettTuple<Type, int>(typeof(ITransgenic), tgResultsList[i - 1].Age);
                    IMouseDataResult cumulativeResult;
                    
                    if (sortedResults.ContainsKey(currentResult))
                    {
                        cumulativeResult = sortedResults[currentResult];

                        double centroid = tgResultsList[i - 1].GetCentroidWidthForRunning();
                        if (centroid > 0)
                        {
                            if (cumulativeResult.CentroidsTest == null)
                            {
                                cumulativeResult.CentroidsTest = new List<double>();
                            }
                            cumulativeResult.CentroidsTest.Add(centroid);
                        }

                        cumulativeResult.Duration += tgResultsList[i - 1].Duration;
                        cumulativeResult.DistanceTravelled += tgResultsList[i - 1].DistanceTravelled;

                        double maxSpeed = tgResultsList[i - 1].MaxSpeed;
                        if (maxSpeed > cumulativeResult.MaxSpeed)
                        {
                            cumulativeResult.MaxSpeed = maxSpeed;
                        }

                        double maxAngSpeed = tgResultsList[i - 1].MaxAngularVelocty;
                        if (maxAngSpeed > cumulativeResult.MaxAngularVelocty)
                        {
                            cumulativeResult.MaxAngularVelocty = maxAngSpeed;
                        }
                        
                        cumulativeResult.AverageVelocity += tgResultsList[i - 1].GetAverageSpeedForMoving();
                        cumulativeResult.AverageAngularVelocity += tgResultsList[i - 1].GetAverageAngularSpeedForTurning();
                        cumulativeResult.PelvicArea += tgResultsList[i - 1].GetCentroidWidthForPelvic1();
                        cumulativeResult.PelvicArea2 += tgResultsList[i - 1].GetCentroidWidthForPelvic2();
                        cumulativeResult.PelvicArea3 += tgResultsList[i - 1].GetCentroidWidthForPelvic3();
                        cumulativeResult.PelvicArea4 += tgResultsList[i - 1].GetCentroidWidthForPelvic4();
                        cumulativeResult.Dummy++;
                    }
                    else
                    {
                        cumulativeResult = ModelResolver.Resolve<IMouseDataResult>();
                        double centroid = tgResultsList[i - 1].GetCentroidWidthForRunning();
                        if (centroid > 0)
                        {
                            if (cumulativeResult.CentroidsTest == null)
                            {
                                cumulativeResult.CentroidsTest = new List<double>();
                            }
                            cumulativeResult.CentroidsTest.Add(centroid);
                        }
                        
                        cumulativeResult.CentroidSize = tgResultsList[i - 1].GetCentroidWidthForRunning();
                        cumulativeResult.Duration = tgResultsList[i - 1].Duration;
                        cumulativeResult.DistanceTravelled = tgResultsList[i - 1].DistanceTravelled;
                        cumulativeResult.MaxSpeed = tgResultsList[i - 1].MaxSpeed;
                        cumulativeResult.MaxAngularVelocty = tgResultsList[i - 1].MaxAngularVelocty;
                        cumulativeResult.AverageVelocity = tgResultsList[i - 1].GetAverageSpeedForMoving();
                        cumulativeResult.AverageAngularVelocity = tgResultsList[i - 1].GetAverageAngularSpeedForTurning();
                        cumulativeResult.PelvicArea = tgResultsList[i - 1].GetCentroidWidthForPelvic1();
                        cumulativeResult.PelvicArea2 += tgResultsList[i - 1].GetCentroidWidthForPelvic2();
                        cumulativeResult.PelvicArea3 += tgResultsList[i - 1].GetCentroidWidthForPelvic3();
                        cumulativeResult.PelvicArea4 += tgResultsList[i - 1].GetCentroidWidthForPelvic4();
                        cumulativeResult.Dummy = 1;

                        sortedResults.Add(currentResult, cumulativeResult);
                    }
                }

                int ntgLength = ntgResultsList.Count;
                for (int i = 1; i <= ntgLength; i++)
                {
                    BrettTuple<Type, int> currentResult = new BrettTuple<Type, int>(typeof(INonTransgenic), ntgResultsList[i - 1].Age);
                    IMouseDataResult cumulativeResult;
                    if (sortedResults.ContainsKey(currentResult))
                    {
                        cumulativeResult = sortedResults[currentResult];
                        
                        double centroid = ntgResultsList[i - 1].GetCentroidWidthForRunning();
                        if (centroid > 0)
                        {
                            if (cumulativeResult.CentroidsTest == null)
                            {
                                cumulativeResult.CentroidsTest = new List<double>();
                            }
                            cumulativeResult.CentroidsTest.Add(centroid);
                        }

                        cumulativeResult.Duration += ntgResultsList[i - 1].Duration;
                        cumulativeResult.DistanceTravelled += ntgResultsList[i - 1].DistanceTravelled;

                        double maxSpeed = ntgResultsList[i - 1].MaxSpeed;
                        if (maxSpeed > cumulativeResult.MaxSpeed)
                        {
                            cumulativeResult.MaxSpeed = maxSpeed;
                        }

                        double maxAngSpeed = ntgResultsList[i - 1].MaxAngularVelocty;
                        if (maxAngSpeed > cumulativeResult.MaxAngularVelocty)
                        {
                            cumulativeResult.MaxAngularVelocty = maxAngSpeed;
                        }

                        cumulativeResult.AverageVelocity += ntgResultsList[i - 1].GetAverageSpeedForMoving();
                        cumulativeResult.AverageAngularVelocity += ntgResultsList[i - 1].GetAverageAngularSpeedForTurning();
                        cumulativeResult.PelvicArea += ntgResultsList[i - 1].GetCentroidWidthForPelvic1();
                        cumulativeResult.PelvicArea2 += ntgResultsList[i - 1].GetCentroidWidthForPelvic2();
                        cumulativeResult.PelvicArea3 += ntgResultsList[i - 1].GetCentroidWidthForPelvic3();
                        cumulativeResult.PelvicArea4 += ntgResultsList[i - 1].GetCentroidWidthForPelvic4();
                        cumulativeResult.Dummy++;
                    }
                    else
                    {
                        cumulativeResult = ModelResolver.Resolve<IMouseDataResult>();

                        double centroid = ntgResultsList[i - 1].GetCentroidWidthForRunning();
                        if (centroid > 0)
                        {
                            if (cumulativeResult.CentroidsTest == null)
                            {
                                cumulativeResult.CentroidsTest = new List<double>();
                            }
                            cumulativeResult.CentroidsTest.Add(centroid);
                        }

                        cumulativeResult.Duration = ntgResultsList[i - 1].Duration;
                        cumulativeResult.DistanceTravelled = ntgResultsList[i - 1].DistanceTravelled;
                        cumulativeResult.MaxSpeed = ntgResultsList[i - 1].MaxSpeed;
                        cumulativeResult.MaxAngularVelocty = ntgResultsList[i - 1].MaxAngularVelocty;
                        cumulativeResult.AverageVelocity = ntgResultsList[i - 1].GetAverageSpeedForMoving();
                        cumulativeResult.AverageAngularVelocity = ntgResultsList[i - 1].GetAverageAngularSpeedForTurning();
                        cumulativeResult.PelvicArea = ntgResultsList[i - 1].GetCentroidWidthForPelvic1();
                        cumulativeResult.PelvicArea2 += ntgResultsList[i - 1].GetCentroidWidthForPelvic2();
                        cumulativeResult.PelvicArea3 += ntgResultsList[i - 1].GetCentroidWidthForPelvic3();
                        cumulativeResult.PelvicArea4 += ntgResultsList[i - 1].GetCentroidWidthForPelvic4();
                        cumulativeResult.Dummy = 1;

                        sortedResults.Add(currentResult, cumulativeResult);
                    }
                }

                int rows = tgResultsList.Count + ntgResultsList.Count + 10;
                const int columns = 13;
                object[,] finalResults = new object[rows, columns];

                finalResults[0, 0] = "Type";
                finalResults[0, 1] = "Age";
                finalResults[0, 2] = "Waist";
                finalResults[0, 3] = "Duration";
                finalResults[0, 4] = "Distance";
                finalResults[0, 5] = "Max Velocity";
                finalResults[0, 6] = "Max Ang Velocity";
                finalResults[0, 7] = "Average Velocity";
                finalResults[0, 8] = "Average Ang Velocity";
                finalResults[0, 9] = "Average Pelvic area";
                finalResults[0, 10] = "Average Pelvic area2";
                finalResults[0, 11] = "Average Pelvic area3";
                finalResults[0, 12] = "Average Pelvic area4";

                int counter = 1;
                foreach (var kvp in sortedResults)
                {
                    IMouseDataResult currentResult = kvp.Value;
                    double totalCount = currentResult.Dummy;
                    finalResults[counter, 0] = kvp.Key.Item1.Name;
                    finalResults[counter, 1] = kvp.Key.Item2;

                    if (currentResult.CentroidsTest == null || !currentResult.CentroidsTest.Any())
                    {
                        finalResults[counter, 2] = 0;
                    }
                    else
                    {
                        finalResults[counter, 2] = currentResult.CentroidsTest.Average();
                    }
                    
                    finalResults[counter, 3] = currentResult.Duration;
                    finalResults[counter, 4] = currentResult.DistanceTravelled;
                    finalResults[counter, 5] = currentResult.MaxSpeed;
                    finalResults[counter, 6] = currentResult.MaxAngularVelocty;
                    finalResults[counter, 7] = currentResult.AverageVelocity / totalCount;
                    finalResults[counter, 8] = currentResult.AverageAngularVelocity / totalCount;
                    finalResults[counter, 9] = currentResult.PelvicArea / totalCount;
                    finalResults[counter, 10] = currentResult.PelvicArea2 / totalCount;
                    finalResults[counter, 11] = currentResult.PelvicArea3 / totalCount;
                    finalResults[counter, 12] = currentResult.PelvicArea4 / totalCount;

                    counter++;
                }

                ExcelService.WriteData(finalResults, @"D:\BatchTestResults6.xlsx");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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
                IEnumerable<SingleMouseViewModel> allMice = TgItemsSource.Concat(NtgItemsSource);
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
                    mouse = NtgItemsSource.FirstOrDefault(x => x.VideoFiles.FirstOrDefault(y => y.VideoFileName.Contains(videoFileName)) != null);
                }

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
                    //return;
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
                        //singleFrame.Distance = tempResult.Distance;
                        singleFrame.SmoothedHeadPoint = tempResult.SmoothedHeadPoint;
                        singleFrame.Centroid = tempResult.Centroid;
                    }

                    results.Add(kvp.Key, singleFrame);
                }

                //double frameRate = -1;
                //using (IVideo video = ModelResolver.Resolve<IVideo>())
                //{
                //    video.SetVideo(trackedVideo.FileName);
                //    frameRate = video.FrameRate;
                //}

                //if (frameRate == -1)
                //{
                //    return;
                //}

                mouseDataResult.Name = mouse.Id;//string.Format("{0}", file);
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
                //mouseDataResult.PelvicArea = trackedVideo.PelvicArea1;
                //mouseDataResult.CentroidSize = trackedVideo.CentroidSize;
                //mouseDataResult.DistanceTravelled = trackedVideo.Di
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

            ExportBatchCommand.RaiseCanExecuteChangedNotification();
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
            return TgItemsSource.Count > 0 || NtgItemsSource.Count > 0;
        }

        private void ShowBatchSettings()
        {
            SingleMouseViewModel[] selectedModels = GetSelectedViewModels();

            if (selectedModels.Length == 0)
            {
                selectedModels = NtgItemsSource.Concat(TgItemsSource).ToArray();
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
            return NtgItemsSource.Where(x => x.IsSelected).Concat(TgItemsSource.Where(x => x.IsSelected)).ToArray();
        }

        private void ExportBatch()
        {
            BatchExportViewModel viewModel = new BatchExportViewModel(NtgItemsSource.Concat(TgItemsSource));
            BatchExportView view = new BatchExportView();
            view.DataContext = viewModel;
            view.ShowDialog();
        }

        private bool CanExportBatch()
        {
            if (!TgItemsSource.Any() && !NtgItemsSource.Any())
            {
                return false;
            }

            if (TgItemsSource.Any(mouse => mouse.Progress < 1))
            {
                return false;
            }

            return NtgItemsSource.All(mouse => !(mouse.Progress < 1));
        }
    }
}
