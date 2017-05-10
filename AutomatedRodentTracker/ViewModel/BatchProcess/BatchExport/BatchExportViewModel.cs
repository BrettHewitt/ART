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
using System.Linq;
using System.Text;
using AutomatedRodentTracker.Commands;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.ModelInterface.Results;
using AutomatedRodentTracker.Services.Excel;
using AutomatedRodentTracker.Services.FileBrowser;
using AutomatedRodentTracker.ViewModel.Datasets;
using AutomatedRodentTracker.ViewModel.Results.Behaviour.Movement;
using AutomatedRodentTracker.ViewModel.Results.Behaviour.Rotation;

namespace AutomatedRodentTracker.ViewModel.BatchProcess.BatchExport
{
    public class BatchExportViewModel : WindowViewModelBase
    {
        private ActionCommand m_OkCommand;
        public ActionCommand OkCommand
        {
            get
            {
                return m_OkCommand ?? (m_OkCommand = new ActionCommand()
                {
                    ExecuteAction = Ok
                });
            }
        }

        private ActionCommand m_ProcessCommand;
        public ActionCommand ProcessCommand
        {
            get
            {
                return m_ProcessCommand ?? (m_ProcessCommand = new ActionCommand()
                {
                    ExecuteAction = Process,
                });
            }
        }

        private ObservableCollection<SingleMouseViewModel> m_Videos;
        public ObservableCollection<SingleMouseViewModel> Videos
        {
            get
            {
                return m_Videos;
            }
            set
            {
                if (ReferenceEquals(m_Videos, value))
                {
                    return;
                }

                m_Videos = value;

                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<MovementBehaviourBaseViewModel> m_VelocityOptions;
        public ObservableCollection<MovementBehaviourBaseViewModel> VelocityOptions
        {
            get
            {
                return m_VelocityOptions;
            }
            set
            {
                if (ReferenceEquals(m_VelocityOptions, value))
                {
                    return;
                }

                m_VelocityOptions = value;

                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<RotationBehaviourBaseViewModel> m_RotationOptions;
        public ObservableCollection<RotationBehaviourBaseViewModel> RotationOptions
        {
            get
            {
                return m_RotationOptions;
            }
            set
            {
                if (ReferenceEquals(m_RotationOptions, value))
                {
                    return;
                }

                m_RotationOptions = value;

                NotifyPropertyChanged();
            }
        }

        private MovementBehaviourBaseViewModel m_SelectedVelocityOption;
        public MovementBehaviourBaseViewModel SelectedVelocityOption
        {
            get
            {
                return m_SelectedVelocityOption;
            }
            set
            {
                if (Equals(m_SelectedVelocityOption, value))
                {
                    return;
                }

                m_SelectedVelocityOption = value;

                NotifyPropertyChanged();
            }
        }

        private RotationBehaviourBaseViewModel m_SelectedRotationOption;
        public RotationBehaviourBaseViewModel SelectedRotationOption
        {
            get
            {
                return m_SelectedRotationOption;
            }
            set
            {
                if (Equals(m_SelectedRotationOption, value))
                {
                    return;
                }

                m_SelectedRotationOption = value;

                NotifyPropertyChanged();
            }
        }

        public BatchExportViewModel(IEnumerable<SingleMouseViewModel> mice)
        {
            Videos = new ObservableCollection<SingleMouseViewModel>(mice);

            ObservableCollection<MovementBehaviourBaseViewModel> movements = new ObservableCollection<MovementBehaviourBaseViewModel>();
            movements.Add(new StillViewModel());
            movements.Add(new WalkingViewModel());
            movements.Add(new RunningViewModel());
            VelocityOptions = movements;

            ObservableCollection<RotationBehaviourBaseViewModel> rotations = new ObservableCollection<RotationBehaviourBaseViewModel>();
            rotations.Add(new NoRotationViewModel());
            rotations.Add(new SlowTurningViewModel());
            rotations.Add(new FastTurningViewModel());
            RotationOptions = rotations;

            SelectedVelocityOption = VelocityOptions.First();
            SelectedRotationOption = RotationOptions.First();
        }

        private void Ok()
        {
            CloseWindow();
        }

        private void Process()
        {
            GenerateAllResults();
            GenerateIndivResultsFinal();
        }

        private void GenerateAllResults()
        {
            int rows = 1000;
            int columns = 22;
            object[,] data = new object[rows, columns];

            int rowCounter = 1;
            data[0, 0] = "Mouse";
            data[0, 1] = "Type";
            data[0, 2] = "Age";
            data[0, 3] = "Clip";
            data[0, 4] = "Centroid Width";
            data[0, 5] = "Distance";
            data[0, 6] = "Max Velocity";
            data[0, 7] = "Max Angular Velocity";
            data[0, 8] = "Average Velocity";
            data[0, 9] = "Average Angular Velocity";
            data[0, 10] = "Average Pelvic Area 1";
            data[0, 11] = "Average Pelvic Area 2";
            data[0, 12] = "Average Pelvic Area 3";
            data[0, 13] = "Average Pelvic Area 4";
            data[0, 14] = "Average Centroid Velocity";
            data[0, 15] = "Max Centroid Velocity";
            data[0, 16] = "Clip Duration";
            data[0, 17] = "Percentage Running";
            data[0, 18] = "Percentage moving";
            data[0, 19] = "Percentage turning";
            data[0, 20] = "Percentage Still";
            data[0, 21] = "Percentage Interacting";


            List<MouseHolder> mice = new List<MouseHolder>();

            foreach (SingleMouseViewModel mouse in Videos)
            {
                mice.AddRange(from video in mouse.VideoFiles let result = mouse.Results[video] select new MouseHolder() {Age = mouse.Age.ToString(), Class = mouse.Class, File = video.VideoFileName, Mouse = mouse, Result = result, Type = mouse.Type.Name});
            }
            try
            {
                foreach (MouseHolder mouse in mice)
                {
                    IMouseDataResult result = mouse.Result;

                    if (result.EndFrame - result.StartFrame < 100)
                    {
                        continue;
                    }

                    data[rowCounter, 0] = mouse.Mouse.Name + " - " + mouse.Mouse.Id;
                    data[rowCounter, 1] = mouse.Type;
                    data[rowCounter, 2] = mouse.Age;
                    data[rowCounter, 3] = mouse.File;
                
                    double centroidWidth = result.GetCentroidWidthForRunning();
                    double distanceTravelled = result.DistanceTravelled;
                    double maxSpeed = result.MaxSpeed;
                    double maxAngVelocity = result.MaxAngularVelocty;
                    double avgVelocity = result.GetAverageSpeedForMoving();
                    double averageAngVelocity = result.AverageAngularVelocity;
                    double avgPelvic1 = result.GetCentroidWidthForPelvic1();
                    double avgPelvic2 = result.GetCentroidWidthForPelvic2();
                    double avgPelvic3 = result.GetCentroidWidthForPelvic3();
                    double avgPelvic4 = result.GetCentroidWidthForPelvic4();
                    double avgCentroidVelocity = result.GetAverageCentroidSpeedForMoving();
                    double maxCentroidVelocity = result.MaxCentroidSpeed;
                    double duration = result.EndFrame - result.StartFrame;

                    avgVelocity /= 1000;
                    avgCentroidVelocity /= 1000;
                    maxSpeed /= 1000;
                    maxCentroidVelocity /= 1000;
                    averageAngVelocity /= 1000;
                    maxAngVelocity /= 1000;

                    int frameDelta = result.EndFrame - result.StartFrame;
                    List<Tuple<int, int>> movingFrameNumbers = result.GetFrameNumbersForMoving();
                    List<Tuple<int, int>> runningFrameNumbers = result.GetFrameNumbersForRunning();
                    List<Tuple<int, int>> turningFrameNumbers = result.GetFrameNumbersForTurning();
                    List<Tuple<int, int>> stillFrameNumbers = result.GetFrameNumbersForStill();
                    List<Tuple<int, int>> interactingFrameNumbers = result.GetFrameNumbesrForInteracting();
                    
                    int movingFrameCount = 0, runningFrameCount = 0, turningFrameCount = 0, stillFrameCount = 0;

                    if (movingFrameNumbers != null && movingFrameNumbers.Any())
                    {
                        foreach (Tuple<int, int> t in movingFrameNumbers)
                        {
                            int delta = t.Item2 - t.Item1;
                            movingFrameCount += delta;
                        }
                    }

                    if (runningFrameNumbers != null && runningFrameNumbers.Any())
                    {
                        foreach (Tuple<int, int> t in runningFrameNumbers)
                        {
                            int delta = t.Item2 - t.Item1;
                            runningFrameCount += delta;
                        }
                    }

                    if (stillFrameNumbers != null && stillFrameNumbers.Any())
                    {
                        foreach (Tuple<int, int> t in stillFrameNumbers)
                        {
                            int delta = t.Item2 - t.Item1;
                            stillFrameCount += delta;
                        }
                    }

                    if (turningFrameNumbers != null && turningFrameNumbers.Any())
                    {
                        foreach (Tuple<int, int> t in turningFrameNumbers)
                        {
                            int delta = t.Item2 - t.Item1;
                            turningFrameCount += delta;
                        }
                    }

                    List<int> allInteractingFrameNumbers = new List<int>();
                    if (interactingFrameNumbers != null && interactingFrameNumbers.Any())
                    {
                        foreach (Tuple<int, int> t in interactingFrameNumbers)
                        {
                            int start = t.Item1;
                            int end = t.Item2;

                            for (int i = start; i <= end; i++)
                            {
                                if (!allInteractingFrameNumbers.Contains(i))
                                {
                                    allInteractingFrameNumbers.Add(i);
                                }
                            }
                        }
                    }

                    double movingPercentage = (double) movingFrameCount/frameDelta;
                    double runningPercentage = (double) runningFrameCount/frameDelta;
                    double turningPercentage = (double) turningFrameCount/frameDelta;
                    double stillPercentage = (double) stillFrameCount/frameDelta;

                    int fCount = allInteractingFrameNumbers.Count - 1;
                    if (fCount < 0)
                    {
                        fCount = 0;
                    }
                    double interactionPercentage = (double)fCount / frameDelta;

                    if (centroidWidth > 0)
                    {
                        data[rowCounter, 4] = centroidWidth;
                    }

                    if (distanceTravelled > 0)
                    {
                        data[rowCounter, 5] = distanceTravelled;
                    }

                    if (maxSpeed > 0)
                    {
                        data[rowCounter, 6] = maxSpeed;
                    }

                    if (maxAngVelocity > 0)
                    {
                        data[rowCounter, 7] = maxAngVelocity;
                    }

                    if (avgVelocity > 0)
                    {
                        data[rowCounter, 8] = avgVelocity;
                    }

                    if (averageAngVelocity > 0)
                    {
                        data[rowCounter, 9] = averageAngVelocity;
                    }

                    if (avgPelvic1 > 0)
                    {
                        data[rowCounter, 10] = avgPelvic1;
                    }

                    if (avgPelvic2 > 0)
                    {
                        data[rowCounter, 11] = avgPelvic2;
                    }

                    if (avgPelvic3 > 0)
                    {
                        data[rowCounter, 12] = avgPelvic3;
                    }

                    if (avgPelvic4 > 0)
                    {
                        data[rowCounter, 13] = avgPelvic4;
                    }

                    if (avgCentroidVelocity > 0)
                    {
                        data[rowCounter, 14] = avgCentroidVelocity;
                    }

                    if (maxCentroidVelocity > 0)
                    {
                        data[rowCounter, 15] = maxCentroidVelocity;
                    }

                    if (duration > 0)
                    {
                        data[rowCounter, 16] = duration;
                    }

                    //if (runningPercentage > 0)
                    //{
                        data[rowCounter, 17] = runningPercentage;
                    //}

                    //if (movingPercentage > 0)
                    //{
                        data[rowCounter, 18] = movingPercentage;
                    //}

                    //if (turningPercentage > 0)
                    //{
                        data[rowCounter, 19] = turningPercentage;
                    //}

                    //if (stillPercentage > 0)
                    //{
                        data[rowCounter, 20] = stillPercentage;
                    //}

                    //if (interactionPercentage > 0)
                    //{
                        data[rowCounter, 21] = interactionPercentage;
                    //}
                
                    rowCounter++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            string fileLocation = FileBrowser.SaveFile("Excel|*.xlsx");

            if (string.IsNullOrWhiteSpace(fileLocation))
            {
                return;
            }

            ExcelService.WriteData(data, fileLocation);
        }

        private class CounterClass
        {
            public int CentroidWidth
            {
                get;
                set;
            }

            public int Distance
            {
                get;
                set;
            }

            public int AvgVelocity
            {
                get;
                set;
            }

            public int AvgAngVelocity
            {
                get;
                set;
            }

            public int AvgPelvic1
            {
                get;
                set;
            }

            public int AvgPelvic2
            {
                get;
                set;
            }

            public int AvgPelvic3
            {
                get;
                set;
            }

            public int AvgPelvic4
            {
                get;
                set;
            }

            public int AvgCentroidVelocity
            {
                get;
                set;
            }

            public int ClipDuration
            {
                get;
                set;
            }

            public int PercentageRunning
            {
                get;
                set;
            }

            public int PercentageMoving
            {
                get;
                set;
            }

            public int PercentageTurning
            {
                get;
                set;
            }

            public int PercentageStill
            {
                get;
                set;
            }

            public int PercentageInteracting
            {
                get;
                set;
            }
        }

        private void GenerateIndivResultsFinal()
        {
            int rows = 1000;
            int columns = 22;
            object[,] data = new object[rows, columns];

            int rowCounter = 1;
            data[0, 0] = "Mouse";
            data[0, 1] = "Type";
            data[0, 2] = "Age";
            data[0, 3] = "Clip";
            data[0, 4] = "Centroid Width";
            data[0, 5] = "Distance";
            data[0, 6] = "Max Velocity";
            data[0, 7] = "Max Angular Velocity";
            data[0, 8] = "Average Velocity";
            data[0, 9] = "Average Angular Velocity";
            data[0, 10] = "Average Pelvic Area 1";
            data[0, 11] = "Average Pelvic Area 2";
            data[0, 12] = "Average Pelvic Area 3";
            data[0, 13] = "Average Pelvic Area 4";
            data[0, 14] = "Average Centroid Velocity";
            data[0, 15] = "Max Centroid Velocity";
            data[0, 16] = "Clip Duration";
            data[0, 17] = "Percentage Running";
            data[0, 18] = "Percentage moving";
            data[0, 19] = "Percentage turning";
            data[0, 20] = "Percentage Still";
            data[0, 21] = "Percentage Interacting";

            Dictionary<BatchProcessViewModel.BrettTuple<string, int>, IMouseDataResult> sortedResults = new Dictionary<BatchProcessViewModel.BrettTuple<string, int>, IMouseDataResult>();
            Dictionary<BatchProcessViewModel.BrettTuple<string, int>, CounterClass> sortedCounters = new Dictionary<BatchProcessViewModel.BrettTuple<string, int>, CounterClass>();
            List<MouseHolder> mice = new List<MouseHolder>();

            foreach (SingleMouseViewModel mouse in Videos)
            {
                mice.AddRange(from video in mouse.VideoFiles let result = mouse.Results[video] select new MouseHolder() { Age = mouse.Age.ToString(), Class = mouse.Class, File = video.VideoFileName, Mouse = mouse, Result = result, Type = mouse.Type.Name });
            }
            try
            {


                foreach (MouseHolder mouse in mice)
                {
                    BatchProcessViewModel.BrettTuple<string, int> key = new BatchProcessViewModel.BrettTuple<string, int>(mouse.Mouse.Id, mouse.Mouse.Age);

                    IMouseDataResult finalResult;
                    CounterClass counter;
                    if (sortedResults.ContainsKey(key))
                    {
                        finalResult = sortedResults[key];
                        counter = sortedCounters[key];
                    }
                    else
                    {
                        finalResult = ModelResolver.Resolve<IMouseDataResult>();
                        finalResult.Name = mouse.Mouse.Id;
                        finalResult.Age = mouse.Mouse.Age;
                        finalResult.Type = mouse.Mouse.Type;

                        counter = new CounterClass();
                        sortedResults.Add(key, finalResult);
                        sortedCounters.Add(key, counter);
                    }

                    IMouseDataResult result = mouse.Result;

                    if (result.EndFrame - result.StartFrame < 100)
                    {
                        continue;
                    }
                    
                    double centroidWidth = result.GetCentroidWidthForRunning();
                    double distanceTravelled = result.DistanceTravelled;
                    double maxSpeed = result.MaxSpeed;
                    double maxAngVelocity = result.MaxAngularVelocty;
                    double avgVelocity = result.GetAverageSpeedForMoving();
                    double averageAngVelocity = result.AverageAngularVelocity;
                    double avgPelvic1 = result.GetCentroidWidthForPelvic1();
                    double avgPelvic2 = result.GetCentroidWidthForPelvic2();
                    double avgPelvic3 = result.GetCentroidWidthForPelvic3();
                    double avgPelvic4 = result.GetCentroidWidthForPelvic4();
                    double avgCentroidVelocity = result.GetAverageCentroidSpeedForMoving();
                    double maxCentroidVelocity = result.MaxCentroidSpeed;
                    double duration = result.EndFrame - result.StartFrame;

                    avgVelocity /= 1000;
                    avgCentroidVelocity /= 1000;
                    maxSpeed /= 1000;
                    maxCentroidVelocity /= 1000;
                    averageAngVelocity /= 1000;
                    maxAngVelocity /= 1000;

                    int frameDelta = result.EndFrame - result.StartFrame;
                    List<Tuple<int, int>> movingFrameNumbers = result.GetFrameNumbersForMoving();
                    List<Tuple<int, int>> runningFrameNumbers = result.GetFrameNumbersForRunning();
                    List<Tuple<int, int>> turningFrameNumbers = result.GetFrameNumbersForTurning();
                    List<Tuple<int, int>> stillFrameNumbers = result.GetFrameNumbersForStill();
                    List<Tuple<int, int>> interactingFrameNumbers = result.GetFrameNumbesrForInteracting();

                    int movingFrameCount = 0, runningFrameCount = 0, turningFrameCount = 0, stillFrameCount = 0;

                    if (movingFrameNumbers != null && movingFrameNumbers.Any())
                    {
                        foreach (Tuple<int, int> t in movingFrameNumbers)
                        {
                            int delta = t.Item2 - t.Item1;
                            movingFrameCount += delta;
                        }
                    }

                    if (runningFrameNumbers != null && runningFrameNumbers.Any())
                    {
                        foreach (Tuple<int, int> t in runningFrameNumbers)
                        {
                            int delta = t.Item2 - t.Item1;
                            runningFrameCount += delta;
                        }
                    }

                    if (stillFrameNumbers != null && stillFrameNumbers.Any())
                    {
                        foreach (Tuple<int, int> t in stillFrameNumbers)
                        {
                            int delta = t.Item2 - t.Item1;
                            stillFrameCount += delta;
                        }
                    }

                    if (turningFrameNumbers != null && turningFrameNumbers.Any())
                    {
                        foreach (Tuple<int, int> t in turningFrameNumbers)
                        {
                            int delta = t.Item2 - t.Item1;
                            turningFrameCount += delta;
                        }
                    }

                    List<int> allInteractingFrameNumbers = new List<int>();
                    if (interactingFrameNumbers != null && interactingFrameNumbers.Any())
                    {
                        foreach (Tuple<int, int> t in interactingFrameNumbers)
                        {
                            int start = t.Item1;
                            int end = t.Item2;

                            for (int i = start; i <= end; i++)
                            {
                                if (!allInteractingFrameNumbers.Contains(i))
                                {
                                    allInteractingFrameNumbers.Add(i);
                                }
                            }
                        }
                    }

                    double movingPercentage = (double)movingFrameCount / frameDelta;
                    double runningPercentage = (double)runningFrameCount / frameDelta;
                    double turningPercentage = (double)turningFrameCount / frameDelta;
                    double stillPercentage = (double)stillFrameCount / frameDelta;

                    int fCount = allInteractingFrameNumbers.Count - 1;
                    if (fCount < 0)
                    {
                        fCount = 0;
                    }
                    double interactionPercentage = (double)fCount / frameDelta;

                    if (centroidWidth > 0)
                    {
                        finalResult.CentroidSize += centroidWidth;
                        counter.CentroidWidth++;
                    }

                    if (distanceTravelled > 0)
                    {
                        finalResult.DistanceTravelled += distanceTravelled;
                        counter.Distance++;
                    }

                    if (maxSpeed > 0)
                    {
                        if (maxSpeed > finalResult.MaxSpeed)
                        {
                            finalResult.MaxSpeed = maxSpeed;
                        }
                        
                        //counter.MaxVelocity++;
                    }

                    if (maxAngVelocity > 0)
                    {
                        if (maxAngVelocity > finalResult.MaxAngularVelocty)
                        {
                            finalResult.MaxAngularVelocty = maxAngVelocity;
                        }
                        
                        //counter.MaxAngVelocity++;
                    }

                    if (avgVelocity > 0)
                    {
                        finalResult.AverageVelocity += avgVelocity;
                        counter.AvgVelocity++;
                    }

                    if (averageAngVelocity > 0)
                    {
                        finalResult.AverageAngularVelocity += averageAngVelocity;
                        counter.AvgAngVelocity++;
                    }

                    if (avgPelvic1 > 0)
                    {
                        finalResult.PelvicArea += avgPelvic1;
                        counter.AvgPelvic1++;
                    }

                    if (avgPelvic2 > 0)
                    {
                        finalResult.PelvicArea2 += avgPelvic2;
                        counter.AvgPelvic2++;
                    }

                    if (avgPelvic3 > 0)
                    {
                        finalResult.PelvicArea3 += avgPelvic3;
                        counter.AvgPelvic3++;
                    }

                    if (avgPelvic4 > 0)
                    {
                        finalResult.PelvicArea4 += avgPelvic4;
                        counter.AvgPelvic4++;
                    }

                    if (avgCentroidVelocity > 0)
                    {
                        finalResult.AverageCentroidVelocity += avgCentroidVelocity;
                        counter.AvgCentroidVelocity++;
                    }

                    if (maxCentroidVelocity > 0)
                    {
                        if (maxCentroidVelocity > finalResult.MaxCentroidSpeed)
                        {
                            finalResult.MaxCentroidSpeed += maxCentroidVelocity;
                        }

                        //counter.MaxCentroidVelocity++;
                    }

                    //if (duration > 0)
                    //{
                        finalResult .Duration += duration;
                        counter.ClipDuration++;
                    //}

                    //if (runningPercentage > 0)
                    //{
                        finalResult.Dummy += runningPercentage;
                        counter.PercentageRunning++;
                    //}

                    //if (movingPercentage > 0)
                    //{
                        finalResult.Dummy2 += movingPercentage;
                        counter.PercentageMoving++;
                    //}

                    //if (turningPercentage > 0)
                    //{
                        finalResult.Dummy3 += turningPercentage;
                        counter.PercentageTurning++;
                    //}

                    //if (stillPercentage > 0)
                    //{
                        finalResult.Dummy4 += stillPercentage;
                        counter.PercentageStill++;
                    //}

                    finalResult.Dummy5 += interactionPercentage;
                    counter.PercentageInteracting++;

                    //rowCounter++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            string fileLocation = FileBrowser.SaveFile("Excel CSV|*.csv");

            if (string.IsNullOrWhiteSpace(fileLocation))
            {
                return;
            }

            foreach (var kvp in sortedResults)
            {
                var key = kvp.Key;
                var counter = sortedCounters[key];
                IMouseDataResult result = kvp.Value;

                data[rowCounter, 0] = result.Name;
                data[rowCounter, 1] = result.Type;
                data[rowCounter, 2] = result.Age;
                //data[rowCounter, 3] = "Clip";
                data[rowCounter, 4] = result.CentroidSize/counter.CentroidWidth;
                data[rowCounter, 5] = result.DistanceTravelled/counter.Distance;
                data[rowCounter, 6] = result.MaxSpeed;
                data[rowCounter, 7] = result.MaxAngularVelocty;
                data[rowCounter, 8] = result.AverageVelocity/counter.AvgVelocity;
                data[rowCounter, 9] = result.AverageAngularVelocity/counter.AvgAngVelocity;
                data[rowCounter, 10] = result.PelvicArea/counter.AvgPelvic1;
                data[rowCounter, 11] = result.PelvicArea2/counter.AvgPelvic2;
                data[rowCounter, 12] = result.PelvicArea3/counter.AvgPelvic3;
                data[rowCounter, 13] = result.PelvicArea4/counter.AvgPelvic4;
                data[rowCounter, 14] = result.AverageCentroidVelocity/counter.AvgCentroidVelocity;
                data[rowCounter, 15] = result.MaxSpeed;
                data[rowCounter, 16] = result.Duration/counter.ClipDuration;
                data[rowCounter, 17] = result.Dummy/counter.PercentageRunning;
                data[rowCounter, 18] = result.Dummy2/counter.PercentageMoving;
                data[rowCounter, 19] = result.Dummy3/counter.PercentageTurning;
                data[rowCounter, 20] = result.Dummy4/counter.PercentageStill;
                data[rowCounter, 21] = result.Dummy5/counter.PercentageInteracting;
                rowCounter++;
            }
            int rowCount = data.GetLength(0);
            int columnCount = data.GetLength(1);

            StringBuilder sb = new StringBuilder();
            for (int i = 1; i <= rowCount; i++)
            {
                StringBuilder sb2 = new StringBuilder();
                for (int j = 1; j <= columnCount; j++)
                {
                    object dat = data[i - 1, j - 1];
                    sb2.Append(dat);

                    if (j < columnCount)
                    {
                        sb2.Append(",");
                    }
                }
                
                sb.AppendLine(sb2.ToString());
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileLocation))
            {
                file.WriteLine(sb.ToString());
            }
        }

        private class MouseHolder
        {
            public SingleMouseViewModel Mouse
            {
                get;
                set;
            }

            public IMouseDataResult Result
            {
                get;
                set;
            }

            public string Age
            {
                get;
                set;
            }

            public string Class
            {
                get;
                set;
            }

            public string Type
            {
                get;
                set;
            }

            public string File
            {
                get;
                set;
            }
        }
    }
}
