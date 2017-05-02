using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.Commands;
using AutomatedRodentTracker.Model.Datasets;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.Model.Results;
using AutomatedRodentTracker.ModelInterface.Datasets;
using AutomatedRodentTracker.ModelInterface.Datasets.Types;
using AutomatedRodentTracker.ModelInterface.Results;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.Movement;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.Rotation;
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
            //GenerateBatchResults();
            //GenerateIndividualResults();
            //GenerateIndividualResults3();
            //GenerateIndividualResults4();
            //GenerateIndividualResults5();
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
            //data[0, 18] = "Percentage Walking";
            //data[0, 18] = "Percentage Still";
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
                    //string mouseClass = mouse.Class.ToLower();
                    //if (mouse.File.Contains("100118-0002") || mouse.File.Contains("100118-0003"))
                    //{
                    //    avgVelocity /= 1000;
                    //    avgCentroidVelocity /= 1000;
                    //    maxSpeed /= 1000;
                    //    maxCentroidVelocity /= 1000;
                    //}
                    //else if (mouseClass.Contains("091119") || mouseClass.Contains("091120") || mouseClass.Contains("091218"))
                    //{
                    //    avgVelocity /= 1000;
                    //    avgCentroidVelocity /= 1000;
                    //    maxSpeed /= 1000;
                    //    maxCentroidVelocity /= 1000;
                    //}
                    //else
                    //{
                    //    avgVelocity /= 1000;
                    //    avgCentroidVelocity /= 1000;
                    //    maxSpeed /= 1000;
                    //    maxCentroidVelocity /= 1000;
                    //}

                    //averageAngVelocity *= 0.02;
                    //maxAngVelocity *= 0.02;

                    int frameDelta = result.EndFrame - result.StartFrame;
                    List<Tuple<int, int>> movingFrameNumbers = result.GetFrameNumbersForMoving();
                    List<Tuple<int, int>> runningFrameNumbers = result.GetFrameNumbersForRunning();
                    List<Tuple<int, int>> turningFrameNumbers = result.GetFrameNumbersForTurning();
                    List<Tuple<int, int>> stillFrameNumbers = result.GetFrameNumbersForStill();
                    List<Tuple<int, int>> interactingFrameNumbers = result.GetFrameNumbesrForInteracting();

                    //if (mouse.File.Contains("091120-0018"))
                    //{
                    //    Console.WriteLine("");
                    //}

                    int movingFrameCount = 0, runningFrameCount = 0, turningFrameCount = 0, stillFrameCount = 0, interactingFrameCount = 0;

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

        private void GenerateIndividualResults()
        {
            Dictionary<BatchProcessViewModel.BrettTuple<string, int>, IMouseDataResult> sortedResults = new Dictionary<BatchProcessViewModel.BrettTuple<string, int>, IMouseDataResult>();

            List<IMouseDataResult> resultsList = new List<IMouseDataResult>();
            foreach (SingleMouseViewModel mouse in Videos)
            {
                resultsList.AddRange(mouse.VideoFiles.Select(singleFile => mouse.Results[singleFile]));
            }

            try
            {
                int resultsLength = resultsList.Count;

                for (int i = 1; i <= resultsLength; i++)
                {
                    IMouseDataResult currentMouseResult = resultsList[i - 1];

                    if (currentMouseResult.Type == null || currentMouseResult.Type is IUndefined || currentMouseResult.Age <= 0)
                    {
                        continue;
                    }

                    BatchProcessViewModel.BrettTuple<string, int> currentResult = new BatchProcessViewModel.BrettTuple<string, int>(currentMouseResult.Name, currentMouseResult.Age);
                    IMouseDataResult cumulativeResult;

                    if (sortedResults.ContainsKey(currentResult))
                    {
                        cumulativeResult = sortedResults[currentResult];
                    }
                    else
                    {
                        cumulativeResult = ModelResolver.Resolve<IMouseDataResult>();
                    }

                    double centroid = currentMouseResult.GetCentroidWidthForRunning();

                    if (centroid > 0)
                    {
                        if (cumulativeResult.CentroidsTest == null)
                        {
                            cumulativeResult.CentroidsTest = new List<double>();
                        }
                        cumulativeResult.CentroidsTest.Add(centroid);
                    }

                    cumulativeResult.Duration += currentMouseResult.Duration;
                    cumulativeResult.DistanceTravelled += currentMouseResult.DistanceTravelled;

                    double maxSpeed = currentMouseResult.MaxSpeed;
                    if (maxSpeed > cumulativeResult.MaxSpeed)
                    {
                        cumulativeResult.MaxSpeed = maxSpeed;
                    }

                    double maxAngSpeed = currentMouseResult.MaxAngularVelocty;
                    if (maxAngSpeed > cumulativeResult.MaxAngularVelocty)
                    {
                        cumulativeResult.MaxAngularVelocty = maxAngSpeed;
                    }

                    double maxCentroidSpeed = currentMouseResult.MaxCentroidSpeed;
                    if (maxCentroidSpeed > cumulativeResult.MaxCentroidSpeed)
                    {
                        cumulativeResult.MaxCentroidSpeed = maxCentroidSpeed;
                    }

                    //if (mouse.Class.ToLower().Contains("100118"))
                    //{
                    //    result.AverageVelocity *= factor100118;
                    //    result.AverageCentroidVelocity *= factor100118;
                    //    result.CentroidSize *= 0.08;
                    //    result.AverageWaist = result.GetCentroidWidthForRunning() * 0.08;
                    //    result.DistanceTravelled *= 0.08;
                    //    result.MaxSpeed *= factor100118;
                    //    result.MaxCentroidSpeed *= factor100118;
                    //}
                    //else
                    //{
                    //    result.AverageVelocity *= factorOther;
                    //    result.AverageCentroidVelocity *= factorOther;
                    //    result.CentroidSize *= 0.195;
                    //    result.AverageWaist = result.GetCentroidWidthForRunning() * 0.195;
                    //    result.DistanceTravelled *= 0.195;
                    //    result.MaxSpeed *= factorOther;
                    //    result.MaxCentroidSpeed *= factorOther;
                    //}

                    //result.AverageAngularVelocity *= 0.02;
                    //result.MaxAngularVelocty *= 0.02;

                    cumulativeResult.AverageVelocity += currentMouseResult.GetAverageSpeedForMoving();
                    cumulativeResult.AverageCentroidVelocity += currentMouseResult.GetAverageCentroidSpeedForMoving();
                    cumulativeResult.AverageAngularVelocity += currentMouseResult.GetAverageAngularSpeedForTurning();
                    cumulativeResult.PelvicArea += currentMouseResult.GetCentroidWidthForPelvic1();
                    cumulativeResult.PelvicArea2 += currentMouseResult.GetCentroidWidthForPelvic2();
                    cumulativeResult.PelvicArea3 += currentMouseResult.GetCentroidWidthForPelvic3();
                    cumulativeResult.PelvicArea4 += currentMouseResult.GetCentroidWidthForPelvic4();
                    cumulativeResult.Dummy++;

                    if (!sortedResults.ContainsKey(currentResult))
                    {
                        sortedResults.Add(currentResult, cumulativeResult);
                    }
                }

                const int columns = (14 * 10) + 1;
                object[,] finalResults = new object[7, columns];

                finalResults[3, 0] = 30;
                finalResults[4, 0] = 60;
                finalResults[5, 0] = 90;
                finalResults[6, 0] = 120;

                //finalResults[0, 0] = "Type";
                //finalResults[0, 1] = "Age";
                finalResults[0, 1] = "Waist";
                finalResults[0, 11] = "Duration";
                finalResults[0, 21] = "Distance";
                finalResults[0, 31] = "Max Velocity";
                finalResults[0, 41] = "Max Ang Velocity";
                finalResults[0, 51] = "Average Velocity";
                finalResults[0, 61] = "Average Ang Velocity";
                finalResults[0, 71] = "Average Pelvic area";
                finalResults[0, 81] = "Average Pelvic area2";
                finalResults[0, 91] = "Average Pelvic area3";
                finalResults[0, 101] = "Average Pelvic area4";
                finalResults[0, 111] = "Average Centroid Velocity";
                finalResults[0, 121] = "Max Centroid Velocity";

                for (int j = 0; j < 13; j++)
                {
                    //for (int i = 0; i < 10; i++)
                    //{
                        finalResults[1, (j * 10) + 1] = "3452";
                        finalResults[1, (j * 10) + 2] = "3450";
                        finalResults[1, (j * 10) + 3] = "3449";
                        finalResults[1, (j * 10) + 4] = "3435";
                        finalResults[1, (j * 10) + 5] = "3441";
                        finalResults[1, (j * 10) + 6] = "3454";
                        finalResults[1, (j * 10) + 7] = "3451";
                        finalResults[1, (j * 10) + 8] = "3453";
                        finalResults[1, (j * 10) + 9] = "3436";
                        finalResults[1, (j * 10) + 10] = "3440";
                    //}
                }

                for (int i = 0; i < 5; i++)
                {
                    finalResults[2, 1 + i] = "Non-Transgenic";
                    finalResults[2, 11 + i] = "Non-Transgenic";
                    finalResults[2, 21 + i] = "Non-Transgenic";
                    finalResults[2, 31 + i] = "Non-Transgenic";
                    finalResults[2, 41 + i] = "Non-Transgenic";
                    finalResults[2, 51 + i] = "Non-Transgenic";
                    finalResults[2, 61 + i] = "Non-Transgenic";
                    finalResults[2, 71 + i] = "Non-Transgenic";
                    finalResults[2, 81 + i] = "Non-Transgenic";
                    finalResults[2, 91 + i] = "Non-Transgenic";
                    finalResults[2, 101 + i] = "Non-Transgenic";
                    finalResults[2, 111 + i] = "Non-Transgenic";
                    finalResults[2, 121 + i] = "Non-Transgenic";

                    finalResults[2, 6 + i] = "Transgenic";
                    finalResults[2, 16 + i] = "Transgenic";
                    finalResults[2, 26 + i] = "Transgenic";
                    finalResults[2, 36 + i] = "Transgenic";
                    finalResults[2, 46 + i] = "Transgenic";
                    finalResults[2, 56 + i] = "Transgenic";
                    finalResults[2, 66 + i] = "Transgenic";
                    finalResults[2, 76 + i] = "Transgenic";
                    finalResults[2, 86 + i] = "Transgenic";
                    finalResults[2, 96 + i] = "Transgenic";
                    finalResults[2, 106 + i] = "Transgenic";
                    finalResults[2, 116 + i] = "Transgenic";
                    finalResults[2, 126 + i] = "Transgenic";
                }

                //int counter = 1;

                foreach (var kvp in sortedResults)
                {
                    int row;
                    switch (kvp.Key.Item2)
                    {
                        case 30:
                            row = 3;
                            break;

                        case 60:
                            row = 4;
                            break;

                        case 90:
                            row = 5;
                            break;

                        case 120:
                            row = 6;
                            break;

                        default:
                            continue;
                    }
                    IMouseDataResult currentResult = kvp.Value;
                    double totalCount = currentResult.Dummy;

                    string mouseName = kvp.Key.Item1;


                    int tgCounter = -1;
                    if (mouseName.Contains("3452"))
                    {
                        tgCounter = 0;
                    }
                    else if (mouseName.Contains("3450"))
                    {
                        tgCounter = 1;
                    }
                    else if (mouseName.Contains("3449"))
                    {
                        tgCounter = 2;
                    }
                    else if (mouseName.Contains("3435"))
                    {
                        tgCounter = 3;
                    }
                    else if (mouseName.Contains("3441"))
                    {
                        tgCounter = 4;
                    }
                    else if (mouseName.Contains("3454"))
                    {
                        tgCounter = 5;
                    }
                    else if (mouseName.Contains("3451"))
                    {
                        tgCounter = 6;
                    }
                    else if (mouseName.Contains("3453"))
                    {
                        tgCounter = 7;
                    }
                    else if (mouseName.Contains("3436"))
                    {
                        tgCounter = 8;
                    }
                    else if (mouseName.Contains("3440"))
                    {
                        tgCounter = 9;
                    }

                    //finalResults[counter, 0] = kvp.Key.Item1.Name;
                    //finalResults[counter, 1] = kvp.Key.Item2;

                    if (currentResult.CentroidsTest == null || !currentResult.CentroidsTest.Any())
                    {
                        finalResults[row, 1 + tgCounter] = 0;
                    }
                    else
                    {
                        finalResults[row, 1 + tgCounter] = currentResult.CentroidsTest.Average();
                    }

                    finalResults[row, 11 + tgCounter] = currentResult.Duration;
                    finalResults[row, 21 + tgCounter] = currentResult.DistanceTravelled;
                    finalResults[row, 31 + tgCounter] = currentResult.MaxSpeed;
                    finalResults[row, 41 + tgCounter] = currentResult.MaxAngularVelocty;
                    finalResults[row, 51 + tgCounter] = currentResult.AverageVelocity / totalCount;
                    finalResults[row, 61 + tgCounter] = currentResult.AverageAngularVelocity / totalCount;
                    finalResults[row, 71 + tgCounter] = currentResult.PelvicArea / totalCount;
                    finalResults[row, 81 + tgCounter] = currentResult.PelvicArea2 / totalCount;
                    finalResults[row, 91 + tgCounter] = currentResult.PelvicArea3 / totalCount;
                    finalResults[row, 101 + tgCounter] = currentResult.PelvicArea4 / totalCount;
                    finalResults[row, 111 + tgCounter] = currentResult.AverageCentroidVelocity / totalCount;
                    finalResults[row, 121 + tgCounter] = currentResult.MaxCentroidSpeed;

                    //counter++;
                }

                string fileLocation = FileBrowser.SaveFile("Excel|*.xlsx");

                if (string.IsNullOrWhiteSpace(fileLocation))
                {
                    return;
                }

                ExcelService.WriteData(finalResults, fileLocation);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private class CounterClass
        {
            public string Name
            {
                get;
                set;
            }

            public int Age
            {
                get;
                set;
            }

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

            public int MaxVelocity
            {
                get;
                set;
            }

            public int MaxAngVelocity
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

            public int MaxCentroidVelocity
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
            //data[0, 18] = "Percentage Walking";
            //data[0, 18] = "Percentage Still";
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

                    //data[rowCounter, 0] = mouse.Mouse.Name + " - " + mouse.Mouse.Id;
                    //data[rowCounter, 1] = mouse.Type;
                    //data[rowCounter, 2] = mouse.Age;
                    //data[rowCounter, 3] = mouse.File;

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

                //int count = sb2.Length;
                //sb2.Remove(count - 1, 1);
                sb.AppendLine(sb2.ToString());
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileLocation))
            {
                file.WriteLine(sb.ToString());
            }

            //ExcelService.WriteData(data, fileLocation);
        }

        private void GenerateIndividualResults2()
        {
            Dictionary<BatchProcessViewModel.BrettTuple<string, int>, IMouseDataResult> sortedResults = new Dictionary<BatchProcessViewModel.BrettTuple<string, int>, IMouseDataResult>();

            List<IMouseDataResult> resultsList = new List<IMouseDataResult>();
            foreach (SingleMouseViewModel mouse in Videos)
            {
                resultsList.AddRange(mouse.VideoFiles.Select(singleFile => mouse.Results[singleFile]));
            }

            try
            {
                int resultsLength = resultsList.Count;

                for (int i = 1; i <= resultsLength; i++)
                {
                    IMouseDataResult currentMouseResult = resultsList[i - 1];

                    if (currentMouseResult.Type == null || currentMouseResult.Type is IUndefined || currentMouseResult.Age <= 0)
                    {
                        continue;
                    }

                    BatchProcessViewModel.BrettTuple<string, int> currentResult = new BatchProcessViewModel.BrettTuple<string, int>(currentMouseResult.Name, currentMouseResult.Age);
                    IMouseDataResult cumulativeResult;

                    if (sortedResults.ContainsKey(currentResult))
                    {
                        cumulativeResult = sortedResults[currentResult];
                    }
                    else
                    {
                        cumulativeResult = ModelResolver.Resolve<IMouseDataResult>();
                    }

                    double centroid = currentMouseResult.GetCentroidWidthForRunning();

                    if (centroid > 0)
                    {
                        if (cumulativeResult.CentroidsTest == null)
                        {
                            cumulativeResult.CentroidsTest = new List<double>();
                        }
                        cumulativeResult.CentroidsTest.Add(centroid);
                    }

                    cumulativeResult.Duration += currentMouseResult.Duration;
                    cumulativeResult.DistanceTravelled += currentMouseResult.DistanceTravelled;

                    double maxSpeed = currentMouseResult.MaxSpeed;
                    if (maxSpeed > cumulativeResult.MaxSpeed)
                    {
                        cumulativeResult.MaxSpeed = maxSpeed;
                    }

                    double maxAngSpeed = currentMouseResult.MaxAngularVelocty;
                    if (maxAngSpeed > cumulativeResult.MaxAngularVelocty)
                    {
                        cumulativeResult.MaxAngularVelocty = maxAngSpeed;
                    }

                    double maxCentroidSpeed = currentMouseResult.MaxCentroidSpeed;
                    if (maxCentroidSpeed > cumulativeResult.MaxCentroidSpeed)
                    {
                        cumulativeResult.MaxCentroidSpeed = maxCentroidSpeed;
                    }

                    cumulativeResult.AverageVelocity += currentMouseResult.GetAverageSpeedForMoving();
                    cumulativeResult.AverageCentroidVelocity += currentMouseResult.GetAverageCentroidSpeedForMoving();
                    cumulativeResult.AverageAngularVelocity += currentMouseResult.GetAverageAngularSpeedForTurning();
                    cumulativeResult.PelvicArea += currentMouseResult.GetCentroidWidthForPelvic1();
                    cumulativeResult.PelvicArea2 += currentMouseResult.GetCentroidWidthForPelvic2();
                    cumulativeResult.PelvicArea3 += currentMouseResult.GetCentroidWidthForPelvic3();
                    cumulativeResult.PelvicArea4 += currentMouseResult.GetCentroidWidthForPelvic4();
                    cumulativeResult.Dummy++;

                    if (!sortedResults.ContainsKey(currentResult))
                    {
                        sortedResults.Add(currentResult, cumulativeResult);
                    }
                }

                const int columns = (14 * 1) + 1;
                Dictionary<string, object[,]> allResults = new Dictionary<string, object[,]>();
                
                foreach (var kvp in sortedResults)
                {
                    int row;
                    switch (kvp.Key.Item2)
                    {
                        case 30:
                            row = 1;
                            break;

                        case 60:
                            row = 2;
                            break;

                        case 90:
                            row = 3;
                            break;

                        case 120:
                            row = 4;
                            break;

                        default:
                            continue;
                    }

                    object[,] finalResults;

                    IMouseDataResult currentResult = kvp.Value;
                    double totalCount = currentResult.Dummy;

                    string mouseName = kvp.Key.Item1;

                    if (!allResults.ContainsKey(mouseName))
                    {
                        finalResults = new object[7, columns];

                        finalResults[0, 0] = "Mouse";
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
                        finalResults[0, 13] = "Average Centroid Velocity";
                        finalResults[0, 14] = "Max Centroid Velocity";

                        finalResults[1, 0] = mouseName;
                        
                        finalResults[1, 1] = 30;
                        finalResults[2, 1] = 60;
                        finalResults[3, 1] = 90;
                        finalResults[4, 1] = 120;
                        
                        allResults.Add(mouseName, finalResults);
                    }
                    else
                    {
                        finalResults = allResults[mouseName];
                    }

                    if (currentResult.CentroidsTest == null || !currentResult.CentroidsTest.Any())
                    {
                        finalResults[row, 2] = 0;
                    }
                    else
                    {
                        finalResults[row, 2] = currentResult.CentroidsTest.Average();
                    }

                    finalResults[row, 3] = currentResult.Duration;
                    finalResults[row, 4] = currentResult.DistanceTravelled;
                    finalResults[row, 5] = currentResult.MaxSpeed;
                    finalResults[row, 6] = currentResult.MaxAngularVelocty;
                    finalResults[row, 7] = currentResult.AverageVelocity / totalCount;
                    finalResults[row, 8] = currentResult.AverageAngularVelocity / totalCount;
                    finalResults[row, 9] = currentResult.PelvicArea / totalCount;
                    finalResults[row, 10] = currentResult.PelvicArea2 / totalCount;
                    finalResults[row, 11] = currentResult.PelvicArea3 / totalCount;
                    finalResults[row, 12] = currentResult.PelvicArea4 / totalCount;
                    finalResults[row, 13] = currentResult.AverageCentroidVelocity / totalCount;
                    finalResults[row, 14] = currentResult.MaxCentroidSpeed;

                    //counter++;
                }

                //string fileLocation = FileBrowser.SaveFile("Excel|*.xlsx");
                string folder = FileBrowser.BrowseForFolder();

                if (string.IsNullOrWhiteSpace(folder))
                {
                    return;
                }

                foreach (var result in allResults)
                {
                    string fileLoc = folder + @"\" + result.Key + ".xlsx";
                    ExcelService.WriteData(result.Value, fileLoc);
                }
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void GenerateIndividualResults3()
        {
            Dictionary<BatchProcessViewModel.BrettTuple<string, int>, IMouseDataResult> sortedResults = new Dictionary<BatchProcessViewModel.BrettTuple<string, int>, IMouseDataResult>();

            List<IMouseDataResult> resultsList = new List<IMouseDataResult>();
            List<Tuple<int, double, string, double, double, double>> allCentroids = new List<Tuple<int, double, string, double, double, double>>();
            foreach (SingleMouseViewModel mouse in Videos)
            {
                IMouseDataResult[] resultsForMouse = mouse.VideoFiles.Select(singleFile => mouse.Results[singleFile]).ToArray();

                foreach (IMouseDataResult result in resultsForMouse)
                {
                    double cent = result.GetCentroidWidthForRunning();

                    if (cent <= 0)
                    {
                        continue;
                    }

                    allCentroids.Add(new Tuple<int, double, string, double, double, double>(mouse.Age, result.GetCentroidWidthForPelvic1(), mouse.Id, result.GetCentroidWidthForPelvic2(), result.GetCentroidWidthForPelvic3(), result.GetCentroidWidthForPelvic4()));
                }
            }

            string fileLoc = FileBrowser.SaveFile("xlsx|*.xlsx");

            if (string.IsNullOrWhiteSpace(fileLoc))
            {
                return;
            }

            object[,] centroids = new object[allCentroids.Count,8];

            IEnumerable<Tuple<int, double, string, double, double, double>> p30 = allCentroids.Where(x => x.Item1 == 30);
            IEnumerable<Tuple<int, double, string, double, double, double>> p60 = allCentroids.Where(x => x.Item1 == 60);
            IEnumerable<Tuple<int, double, string, double, double, double>> p90 = allCentroids.Where(x => x.Item1 == 90);
            IEnumerable<Tuple<int, double, string, double, double, double>> p120 = allCentroids.Where(x => x.Item1 == 120);

            int rowC = 0;
            foreach (var cen in p30)
            {
                centroids[rowC, 0] = 30;
                centroids[rowC, 1] = cen.Item2;
                centroids[rowC, 2] = cen.Item4;
                centroids[rowC, 3] = cen.Item5;
                centroids[rowC, 4] = cen.Item6;

                if (cen.Item3.Contains("3452"))
                {
                    centroids[rowC, 5] = 0;
                    centroids[rowC, 6] = 0;
                    centroids[rowC, 7] = 0;
                }
                else if (cen.Item3.Contains("3450"))
                {
                    centroids[rowC, 5] = 0.373;
                    centroids[rowC, 6] = 0.163;
                    centroids[rowC, 7] = 0.536;
                }
                else if (cen.Item3.Contains("3449"))
                {
                    centroids[rowC, 5] = 0.305;
                    centroids[rowC, 6] = 0.163;
                    centroids[rowC, 7] = 0.468;
                }
                else if (cen.Item3.Contains("3435"))
                {
                    centroids[rowC, 5] = 0.294;
                    centroids[rowC, 6] = 0.198;
                    centroids[rowC, 7] = 0.492;
                }
                else if (cen.Item3.Contains("3441"))
                {
                    centroids[rowC, 5] = 0.326;
                    centroids[rowC, 6] = 0.206;
                    centroids[rowC, 7] = 0.532;
                }
                else if (cen.Item3.Contains("3454"))
                {
                    centroids[rowC, 5] = 0.232;
                    centroids[rowC, 6] = 0.145;
                    centroids[rowC, 7] = 0.377;
                }
                else if (cen.Item3.Contains("3451"))
                {
                    centroids[rowC, 5] = 0.266;
                    centroids[rowC, 6] = 0.152;
                    centroids[rowC, 7] = 0.418;
                }
                else if (cen.Item3.Contains("3453"))
                {
                    centroids[rowC, 5] = 0.253;
                    centroids[rowC, 6] = 0.142;
                    centroids[rowC, 7] = 0.395;
                }
                else if (cen.Item3.Contains("3436"))
                {
                    centroids[rowC, 5] = 0.28;
                    centroids[rowC, 6] = 0.166;
                    centroids[rowC, 7] = 0.446;
                }
                else if (cen.Item3.Contains("3440"))
                {
                    centroids[rowC, 5] = 0.283;
                    centroids[rowC, 6] = 0.17;
                    centroids[rowC, 7] = 0.453;
                }

                rowC++;
            }

            foreach (var cen in p60)
            {
                centroids[rowC, 0] = 60;
                centroids[rowC, 1] = cen.Item2;
                centroids[rowC, 2] = cen.Item4;
                centroids[rowC, 3] = cen.Item5;
                centroids[rowC, 4] = cen.Item6;

                if (cen.Item3.Contains("3452"))
                {
                    centroids[rowC, 5] = 0.538;
                    centroids[rowC, 6] = 0.206;
                    centroids[rowC, 7] = 0.744;
                }
                else if (cen.Item3.Contains("3450"))
                {
                    centroids[rowC, 5] = 0.534;
                    centroids[rowC, 6] = 0.222;
                    centroids[rowC, 7] = 0.756;
                }
                else if (cen.Item3.Contains("3449"))
                {
                    centroids[rowC, 5] = 0.434;
                    centroids[rowC, 6] = 0.18;
                    centroids[rowC, 7] = 0.614;
                }
                else if (cen.Item3.Contains("3435"))
                {
                    centroids[rowC, 5] = 0.428;
                    centroids[rowC, 6] = 0.19;
                    centroids[rowC, 7] = 0.618;
                }
                else if (cen.Item3.Contains("3441"))
                {
                    centroids[rowC, 5] = 0.512;
                    centroids[rowC, 6] = 0.214;
                    centroids[rowC, 7] = 0.726;
                }
                else if (cen.Item3.Contains("3454"))
                {
                    centroids[rowC, 5] = 0.23;
                    centroids[rowC, 6] = 0.121;
                    centroids[rowC, 7] = 0.351;
                }
                else if (cen.Item3.Contains("3451"))
                {
                    centroids[rowC, 5] = 0.21;
                    centroids[rowC, 6] = 0.157;
                    centroids[rowC, 7] = 0.367;
                }
                else if (cen.Item3.Contains("3453"))
                {
                    centroids[rowC, 5] = 0.253;
                    centroids[rowC, 6] = 0;
                    centroids[rowC, 7] = 0;
                }
                else if (cen.Item3.Contains("3436"))
                {
                    centroids[rowC, 5] = 0.258;
                    centroids[rowC, 6] = 0.129;
                    centroids[rowC, 7] = 0.387;
                }
                else if (cen.Item3.Contains("3440"))
                {
                    centroids[rowC, 5] = 0.298;
                    centroids[rowC, 6] = 0.125;
                    centroids[rowC, 7] = 0.423;
                }

                rowC++;
            }

            foreach (var cen in p90)
            {
                centroids[rowC, 0] = 90;
                centroids[rowC, 1] = cen.Item2;
                centroids[rowC, 2] = cen.Item4;
                centroids[rowC, 3] = cen.Item5;
                centroids[rowC, 4] = cen.Item6;

                if (cen.Item3.Contains("3452"))
                {
                    centroids[rowC, 5] = 0.528;
                    centroids[rowC, 6] = 0.244;
                    centroids[rowC, 7] = 0.772;
                }
                else if (cen.Item3.Contains("3450"))
                {
                    centroids[rowC, 5] = 0.532;
                    centroids[rowC, 6] = 0.222;
                    centroids[rowC, 7] = 0.754;
                }
                else if (cen.Item3.Contains("3449"))
                {
                    centroids[rowC, 5] = 0.529;
                    centroids[rowC, 6] = 0.2611;
                    centroids[rowC, 7] = 0.7901;
                }
                else if (cen.Item3.Contains("3435"))
                {
                    centroids[rowC, 5] = 0.519;
                    centroids[rowC, 6] = 0.238;
                    centroids[rowC, 7] = 0.757;
                }
                else if (cen.Item3.Contains("3441"))
                {
                    centroids[rowC, 5] = 0.535;
                    centroids[rowC, 6] = 0.237;
                    centroids[rowC, 7] = 0.772;
                }
                else if (cen.Item3.Contains("3454"))
                {
                    centroids[rowC, 5] = 0.1332;
                    centroids[rowC, 6] = 0.138;
                    centroids[rowC, 7] = 0.2712;
                }
                else if (cen.Item3.Contains("3451"))
                {
                    centroids[rowC, 5] = 0.184;
                    centroids[rowC, 6] = 0.143;
                    centroids[rowC, 7] = 0.327;
                }
                else if (cen.Item3.Contains("3453"))
                {
                    centroids[rowC, 5] = 0.16;
                    centroids[rowC, 6] = 0.123;
                    centroids[rowC, 7] = 0.283;
                }
                else if (cen.Item3.Contains("3436"))
                {
                    centroids[rowC, 5] = 0.199;
                    centroids[rowC, 6] = 0.105;
                    centroids[rowC, 7] = 0.304;
                }
                else if (cen.Item3.Contains("3440"))
                {
                    centroids[rowC, 5] = 0.198;
                    centroids[rowC, 6] = 0.114;
                    centroids[rowC, 7] = 0.312;
                }

                rowC++;
            }

            foreach (var cen in p120)
            {
                centroids[rowC, 0] = 120;
                centroids[rowC, 1] = cen.Item2;
                centroids[rowC, 2] = cen.Item4;
                centroids[rowC, 3] = cen.Item5;
                centroids[rowC, 4] = cen.Item6;

                if (cen.Item3.Contains("3452"))
                {
                    centroids[rowC, 5] = 0.552;
                    centroids[rowC, 6] = 0.246;
                    centroids[rowC, 7] = 0.798;
                }
                else if (cen.Item3.Contains("3450"))
                {
                    centroids[rowC, 5] = 0.544;
                    centroids[rowC, 6] = 0.24;
                    centroids[rowC, 7] = 0.784;
                }
                else if (cen.Item3.Contains("3449"))
                {
                    centroids[rowC, 5] = 0.564;
                    centroids[rowC, 6] = 0.27;
                    centroids[rowC, 7] = 0.834;
                }
                else if (cen.Item3.Contains("3435"))
                {
                    centroids[rowC, 5] = 0.535;
                    centroids[rowC, 6] = 0.226;
                    centroids[rowC, 7] = 0.761;
                }
                else if (cen.Item3.Contains("3441"))
                {
                    centroids[rowC, 5] = 0.574;
                    centroids[rowC, 6] = 0.267;
                    centroids[rowC, 7] = 0.841;
                }
                else if (cen.Item3.Contains("3454"))
                {
                    centroids[rowC, 5] = 0.102;
                    centroids[rowC, 6] = 0.108;
                    centroids[rowC, 7] = 0.21;
                }
                else if (cen.Item3.Contains("3451"))
                {
                    centroids[rowC, 5] = 0.116;
                    centroids[rowC, 6] = 0.093;
                    centroids[rowC, 7] = 0.209;
                }
                else if (cen.Item3.Contains("3453"))
                {
                    centroids[rowC, 5] = 0.111;
                    centroids[rowC, 6] = 0.1;
                    centroids[rowC, 7] = 0.211;
                }
                else if (cen.Item3.Contains("3436"))
                {
                    centroids[rowC, 5] = 0.1288;
                    centroids[rowC, 6] = 0.085;
                    centroids[rowC, 7] = 0.2138;
                }
                else if (cen.Item3.Contains("3440"))
                {
                    centroids[rowC, 5] = 0.123;
                    centroids[rowC, 6] = 0.104;
                    centroids[rowC, 7] = 0.227;
                }

                rowC++;
            }
            //foreach (var cen in p30)
            //{
            //    centroids[rowC, 0] = 30;
            //    centroids[rowC, 1] = cen.Item2;
            //    centroids[rowC, 2] = cen.Item4;
            //    centroids[rowC, 3] = cen.Item5;
            //    centroids[rowC, 4] = cen.Item6;

            //    if (cen.Item3.Contains("3452"))
            //    {
            //        centroids[rowC, 2] = 14;
            //    }
            //    else if (cen.Item3.Contains("3450"))
            //    {
            //        centroids[rowC, 2] = 14.5;
            //    }
            //    else if (cen.Item3.Contains("3449"))
            //    {
            //        centroids[rowC, 2] = 14.5;
            //    }
            //    else if (cen.Item3.Contains("3435"))
            //    {
            //        centroids[rowC, 2] = 16;
            //    }
            //    else if (cen.Item3.Contains("3441"))
            //    {
            //        centroids[rowC, 2] = 18.5;
            //    }
            //    else if (cen.Item3.Contains("3454"))
            //    {
            //        centroids[rowC, 2] = 12;
            //    }
            //    else if (cen.Item3.Contains("3451"))
            //    {
            //        centroids[rowC, 2] = 13;
            //    }
            //    else if (cen.Item3.Contains("3453"))
            //    {
            //        centroids[rowC, 2] = 13.5;
            //    }
            //    else if (cen.Item3.Contains("3436"))
            //    {
            //        centroids[rowC, 2] = 15;
            //    }
            //    else if (cen.Item3.Contains("3440"))
            //    {
            //        centroids[rowC, 2] = 16;
            //    }

            //    rowC++;
            //}

            //foreach (var cen in p60)
            //{
            //    centroids[rowC, 0] = 60;
            //    centroids[rowC, 1] = cen.Item2;

            //    if (cen.Item3.Contains("3452"))
            //    {
            //        centroids[rowC, 2] = 20;
            //    }
            //    else if (cen.Item3.Contains("3450"))
            //    {
            //        centroids[rowC, 2] = 17.5;
            //    }
            //    else if (cen.Item3.Contains("3449"))
            //    {
            //        centroids[rowC, 2] = 20;
            //    }
            //    else if (cen.Item3.Contains("3435"))
            //    {
            //        centroids[rowC, 2] = 19;
            //    }
            //    else if (cen.Item3.Contains("3441"))
            //    {
            //        centroids[rowC, 2] = 17.5;
            //    }
            //    else if (cen.Item3.Contains("3454"))
            //    {
            //        centroids[rowC, 2] = 15.5;
            //    }
            //    else if (cen.Item3.Contains("3451"))
            //    {
            //        centroids[rowC, 2] = 17.5;
            //    }
            //    else if (cen.Item3.Contains("3453"))
            //    {
            //        centroids[rowC, 2] = 18.5;
            //    }
            //    else if (cen.Item3.Contains("3436"))
            //    {
            //        centroids[rowC, 2] = 17.5;
            //    }
            //    else if (cen.Item3.Contains("3440"))
            //    {
            //        centroids[rowC, 2] = 17.5;
            //    }

            //    rowC++;
            //}

            //foreach (var cen in p90)
            //{
            //    centroids[rowC, 0] = 90;
            //    centroids[rowC, 1] = cen.Item2;

            //    if (cen.Item3.Contains("3452"))
            //    {
            //        centroids[rowC, 2] = 24.5;
            //    }
            //    else if (cen.Item3.Contains("3450"))
            //    {
            //        centroids[rowC, 2] = 20.5;
            //    }
            //    else if (cen.Item3.Contains("3449"))
            //    {
            //        centroids[rowC, 2] = 23;
            //    }
            //    else if (cen.Item3.Contains("3435"))
            //    {
            //        centroids[rowC, 2] = 22;
            //    }
            //    else if (cen.Item3.Contains("3441"))
            //    {
            //        centroids[rowC, 2] = 25;
            //    }
            //    else if (cen.Item3.Contains("3454"))
            //    {
            //        centroids[rowC, 2] = 16.5;
            //    }
            //    else if (cen.Item3.Contains("3451"))
            //    {
            //        centroids[rowC, 2] = 18;
            //    }
            //    else if (cen.Item3.Contains("3453"))
            //    {
            //        centroids[rowC, 2] = 18.5;
            //    }
            //    else if (cen.Item3.Contains("3436"))
            //    {
            //        centroids[rowC, 2] = 18;
            //    }
            //    else if (cen.Item3.Contains("3440"))
            //    {
            //        centroids[rowC, 2] = 18;
            //    }

            //    rowC++;
            //}

            //foreach (var cen in p120)
            //{
            //    centroids[rowC, 0] = 120;
            //    centroids[rowC, 1] = cen.Item2;

            //    if (cen.Item3.Contains("3452"))
            //    {
            //        centroids[rowC, 2] = 24;
            //    }
            //    else if (cen.Item3.Contains("3450"))
            //    {
            //        centroids[rowC, 2] = 21.5;
            //    }
            //    else if (cen.Item3.Contains("3449"))
            //    {
            //        centroids[rowC, 2] = 24;
            //    }
            //    else if (cen.Item3.Contains("3435"))
            //    {
            //        centroids[rowC, 2] = 22;
            //    }
            //    else if (cen.Item3.Contains("3441"))
            //    {
            //        centroids[rowC, 2] = 25;
            //    }
            //    else if (cen.Item3.Contains("3454"))
            //    {
            //        centroids[rowC, 2] = 15;
            //    }
            //    else if (cen.Item3.Contains("3451"))
            //    {
            //        centroids[rowC, 2] = 17;
            //    }
            //    else if (cen.Item3.Contains("3453"))
            //    {
            //        centroids[rowC, 2] = 16;
            //    }
            //    else if (cen.Item3.Contains("3436"))
            //    {
            //        centroids[rowC, 2] = 15;
            //    }
            //    else if (cen.Item3.Contains("3440"))
            //    {
            //        centroids[rowC, 2] = 15;
            //    }

            //    rowC++;
            //}

            ExcelService.WriteData(centroids, fileLoc);
        }

        private void GenerateIndividualResults4()
        {
            int rowCount = 1000;

            object[,] finalResultsWaist = new object[rowCount, 3];
            object[,] finalResultsDuration = new object[rowCount, 3];
            object[,] finalResultsDistance = new object[rowCount, 3];
            object[,] finalResultsMaxVelocity = new object[rowCount, 3];
            object[,] finalResultsMaxAngVelocity = new object[rowCount, 3];
            object[,] finalResultsAvgVelocity = new object[rowCount, 3];
            object[,] finalResultsAvgAngVelocity = new object[rowCount, 3];
            object[,] finalResultsAvgPelvicArea = new object[rowCount, 3];
            object[,] finalResultsAvgPelvicArea2 = new object[rowCount, 3];
            object[,] finalResultsAvgPelvicArea3 = new object[rowCount, 3];
            object[,] finalResultsAvgPelvicArea4 = new object[rowCount, 3];
            object[,] finalResultsAvgCentroidVelocity = new object[rowCount, 3];
            object[,] finalResultsMaxCentroidVelocity = new object[rowCount, 3];

            finalResultsWaist[0, 0] = "Age";
            finalResultsWaist[0, 1] = "TG";
            finalResultsWaist[0, 2] = "NTG";

            finalResultsDuration[0, 0] = "Age";
            finalResultsDuration[0, 1] = "TG";
            finalResultsDuration[0, 2] = "NTG";

            finalResultsDistance[0, 0] = "Age";
            finalResultsDistance[0, 1] = "TG";
            finalResultsDistance[0, 2] = "NTG";

            finalResultsMaxVelocity[0, 0] = "Age";
            finalResultsMaxVelocity[0, 1] = "TG";
            finalResultsMaxVelocity[0, 2] = "NTG";

            finalResultsMaxAngVelocity[0, 0] = "Age";
            finalResultsMaxAngVelocity[0, 1] = "TG";
            finalResultsMaxAngVelocity[0, 2] = "NTG";

            finalResultsAvgVelocity[0, 0] = "Age";
            finalResultsAvgVelocity[0, 1] = "TG";
            finalResultsAvgVelocity[0, 2] = "NTG";

            finalResultsAvgAngVelocity[0, 0] = "Age";
            finalResultsAvgAngVelocity[0, 1] = "TG";
            finalResultsAvgAngVelocity[0, 2] = "NTG";

            finalResultsAvgPelvicArea[0, 0] = "Age";
            finalResultsAvgPelvicArea[0, 1] = "TG";
            finalResultsAvgPelvicArea[0, 2] = "NTG";

            finalResultsAvgPelvicArea2[0, 0] = "Age";
            finalResultsAvgPelvicArea2[0, 1] = "TG";
            finalResultsAvgPelvicArea2[0, 2] = "NTG";

            finalResultsAvgPelvicArea3[0, 0] = "Age";
            finalResultsAvgPelvicArea3[0, 1] = "TG";
            finalResultsAvgPelvicArea3[0, 2] = "NTG";

            finalResultsAvgPelvicArea4[0, 0] = "Age";
            finalResultsAvgPelvicArea4[0, 1] = "TG";
            finalResultsAvgPelvicArea4[0, 2] = "NTG";

            finalResultsAvgCentroidVelocity[0, 0] = "Age";
            finalResultsAvgCentroidVelocity[0, 1] = "TG";
            finalResultsAvgCentroidVelocity[0, 2] = "NTG";

            finalResultsMaxCentroidVelocity[0, 0] = "Age";
            finalResultsMaxCentroidVelocity[0, 1] = "TG";
            finalResultsMaxCentroidVelocity[0, 2] = "NTG";

            //finalResults[1, 0] = "30";

            //finalResults[1, 1] = "Waist";
            //finalResults[1, 2] = "Duration";
            //finalResults[1, 3] = "Distance";
            //finalResults[1, 4] = "Max Velocity";
            //finalResults[1, 5] = "Max Ang Velocity";
            //finalResults[1, 6] = "Average Velocity";
            //finalResults[1, 7] = "Average Ang Velocity";
            //finalResults[1, 8] = "Average Pelvic area";
            //finalResults[1, 9] = "Average Pelvic area2";
            //finalResults[1, 10] = "Average Pelvic area3";
            //finalResults[1, 11] = "Average Pelvic area4";
            //finalResults[1, 12] = "Average Centroid Velocity";
            //finalResults[1, 13] = "Max Centroid Velocity";

            //List<IMouseDataResult> allResults = new List<IMouseDataResult>();

            List<IMouseDataResult> tg30 = new List<IMouseDataResult>(), tg60 = new List<IMouseDataResult>(), tg90 = new List<IMouseDataResult>(), tg120 = new List<IMouseDataResult>(), ntg30 = new List<IMouseDataResult>(), ntg60 = new List<IMouseDataResult>(), ntg90 = new List<IMouseDataResult>(), ntg120 = new List<IMouseDataResult>();

            //tg30 = Videos.Select(vid => vid.VideoFiles.Where(x => vid.Results[x].Type is ITransgenic && vid.Results[x].Age == 30));

            foreach (SingleMouseViewModel mouse in Videos)
            {
                IMouseDataResult[] resultsForMouse = mouse.VideoFiles.Select(singleFile => mouse.Results[singleFile]).ToArray();

                tg30.AddRange(resultsForMouse.Where(x => x.Type is ITransgenic && x.Age == 30));
                tg60.AddRange(resultsForMouse.Where(x => x.Type is ITransgenic && x.Age == 60));
                tg90.AddRange(resultsForMouse.Where(x => x.Type is ITransgenic && x.Age == 90));
                tg120.AddRange(resultsForMouse.Where(x => x.Type is ITransgenic && x.Age == 120));
                ntg30.AddRange(resultsForMouse.Where(x => x.Type is INonTransgenic && x.Age == 30));
                ntg60.AddRange(resultsForMouse.Where(x => x.Type is INonTransgenic && x.Age == 60));
                ntg90.AddRange(resultsForMouse.Where(x => x.Type is INonTransgenic && x.Age == 90));
                ntg120.AddRange(resultsForMouse.Where(x => x.Type is INonTransgenic && x.Age == 120));
            }

            int tgRowCounter = 1;
            int ntgRowCounter = 1;
            foreach (IMouseDataResult mouse in tg30)
            {
                int colNumber = 1;
                string age = "30";
                finalResultsWaist[tgRowCounter, 0] = age;
                finalResultsWaist[tgRowCounter, colNumber] = mouse.GetCentroidWidthForRunning();

                finalResultsDuration[tgRowCounter, 0] = age;
                finalResultsDuration[tgRowCounter, colNumber] = mouse.Duration;

                finalResultsDistance[tgRowCounter, 0] = age;
                finalResultsDistance[tgRowCounter, colNumber] = mouse.DistanceTravelled;

                finalResultsMaxVelocity[tgRowCounter, 0] = age;
                finalResultsMaxVelocity[tgRowCounter, colNumber] = mouse.MaxSpeed;

                finalResultsMaxAngVelocity[tgRowCounter, 0] = age;
                finalResultsMaxAngVelocity[tgRowCounter, colNumber] = mouse.MaxAngularVelocty;

                finalResultsAvgVelocity[tgRowCounter, 0] = age;
                finalResultsAvgVelocity[tgRowCounter, colNumber] = mouse.AverageVelocity;

                finalResultsAvgAngVelocity[tgRowCounter, 0] = age;
                finalResultsAvgAngVelocity[tgRowCounter, colNumber] = mouse.AverageAngularVelocity;

                finalResultsAvgPelvicArea[tgRowCounter, 0] = age;
                finalResultsAvgPelvicArea[tgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic1();

                finalResultsAvgPelvicArea2[tgRowCounter, 0] = age;
                finalResultsAvgPelvicArea2[tgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic2();

                finalResultsAvgPelvicArea3[tgRowCounter, 0] = age;
                finalResultsAvgPelvicArea3[tgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic3();

                finalResultsAvgPelvicArea4[tgRowCounter, 0] = age;
                finalResultsAvgPelvicArea4[tgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic4();

                finalResultsAvgCentroidVelocity[tgRowCounter, 0] = age;
                finalResultsAvgCentroidVelocity[tgRowCounter, colNumber] = mouse.AverageCentroidVelocity;

                finalResultsMaxCentroidVelocity[tgRowCounter, 0] = age;
                finalResultsMaxCentroidVelocity[tgRowCounter, colNumber] = mouse.MaxCentroidSpeed;

                tgRowCounter++;
            }

            foreach (IMouseDataResult mouse in ntg30)
            {
                int colNumber = 2;
                string age = "30";
                finalResultsWaist[ntgRowCounter, 0] = age;
                finalResultsWaist[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForRunning();

                finalResultsDuration[ntgRowCounter, 0] = age;
                finalResultsDuration[ntgRowCounter, colNumber] = mouse.Duration;

                finalResultsDistance[ntgRowCounter, 0] = age;
                finalResultsDistance[ntgRowCounter, colNumber] = mouse.DistanceTravelled;

                finalResultsMaxVelocity[ntgRowCounter, 0] = age;
                finalResultsMaxVelocity[ntgRowCounter, colNumber] = mouse.MaxSpeed;

                finalResultsMaxAngVelocity[ntgRowCounter, 0] = age;
                finalResultsMaxAngVelocity[ntgRowCounter, colNumber] = mouse.MaxAngularVelocty;

                finalResultsAvgVelocity[ntgRowCounter, 0] = age;
                finalResultsAvgVelocity[ntgRowCounter, colNumber] = mouse.AverageVelocity;

                finalResultsAvgAngVelocity[ntgRowCounter, 0] = age;
                finalResultsAvgAngVelocity[ntgRowCounter, colNumber] = mouse.AverageAngularVelocity;

                finalResultsAvgPelvicArea[ntgRowCounter, 0] = age;
                finalResultsAvgPelvicArea[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic1();

                finalResultsAvgPelvicArea2[ntgRowCounter, 0] = age;
                finalResultsAvgPelvicArea2[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic2();

                finalResultsAvgPelvicArea3[ntgRowCounter, 0] = age;
                finalResultsAvgPelvicArea3[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic3();

                finalResultsAvgPelvicArea4[ntgRowCounter, 0] = age;
                finalResultsAvgPelvicArea4[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic4();

                finalResultsAvgCentroidVelocity[ntgRowCounter, 0] = age;
                finalResultsAvgCentroidVelocity[ntgRowCounter, colNumber] = mouse.AverageCentroidVelocity;

                finalResultsMaxCentroidVelocity[ntgRowCounter, 0] = age;
                finalResultsMaxCentroidVelocity[ntgRowCounter, colNumber] = mouse.MaxCentroidSpeed;

                ntgRowCounter++;
            }

            if (ntgRowCounter > tgRowCounter)
            {
                tgRowCounter = ntgRowCounter;
            }
            else
            {
                ntgRowCounter = tgRowCounter;
            }

            foreach (IMouseDataResult mouse in tg60)
            {
                int colNumber = 1;
                string age = "60";
                finalResultsWaist[tgRowCounter, 0] = age;
                finalResultsWaist[tgRowCounter, colNumber] = mouse.GetCentroidWidthForRunning();

                finalResultsDuration[tgRowCounter, 0] = age;
                finalResultsDuration[tgRowCounter, colNumber] = mouse.Duration;

                finalResultsDistance[tgRowCounter, 0] = age;
                finalResultsDistance[tgRowCounter, colNumber] = mouse.DistanceTravelled;

                finalResultsMaxVelocity[tgRowCounter, 0] = age;
                finalResultsMaxVelocity[tgRowCounter, colNumber] = mouse.MaxSpeed;

                finalResultsMaxAngVelocity[tgRowCounter, 0] = age;
                finalResultsMaxAngVelocity[tgRowCounter, colNumber] = mouse.MaxAngularVelocty;

                finalResultsAvgVelocity[tgRowCounter, 0] = age;
                finalResultsAvgVelocity[tgRowCounter, colNumber] = mouse.AverageVelocity;

                finalResultsAvgAngVelocity[tgRowCounter, 0] = age;
                finalResultsAvgAngVelocity[tgRowCounter, colNumber] = mouse.AverageAngularVelocity;

                finalResultsAvgPelvicArea[tgRowCounter, 0] = age;
                finalResultsAvgPelvicArea[tgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic1();

                finalResultsAvgPelvicArea2[tgRowCounter, 0] = age;
                finalResultsAvgPelvicArea2[tgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic2();

                finalResultsAvgPelvicArea3[tgRowCounter, 0] = age;
                finalResultsAvgPelvicArea3[tgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic3();

                finalResultsAvgPelvicArea4[tgRowCounter, 0] = age;
                finalResultsAvgPelvicArea4[tgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic4();

                finalResultsAvgCentroidVelocity[tgRowCounter, 0] = age;
                finalResultsAvgCentroidVelocity[tgRowCounter, colNumber] = mouse.AverageCentroidVelocity;

                finalResultsMaxCentroidVelocity[tgRowCounter, 0] = age;
                finalResultsMaxCentroidVelocity[tgRowCounter, colNumber] = mouse.MaxCentroidSpeed;

                tgRowCounter++;
            }

            foreach (IMouseDataResult mouse in ntg60)
            {
                int colNumber = 2;
                string age = "60";
                finalResultsWaist[ntgRowCounter, 0] = age;
                finalResultsWaist[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForRunning();

                finalResultsDuration[ntgRowCounter, 0] = age;
                finalResultsDuration[ntgRowCounter, colNumber] = mouse.Duration;

                finalResultsDistance[ntgRowCounter, 0] = age;
                finalResultsDistance[ntgRowCounter, colNumber] = mouse.DistanceTravelled;

                finalResultsMaxVelocity[ntgRowCounter, 0] = age;
                finalResultsMaxVelocity[ntgRowCounter, colNumber] = mouse.MaxSpeed;

                finalResultsMaxAngVelocity[ntgRowCounter, 0] = age;
                finalResultsMaxAngVelocity[ntgRowCounter, colNumber] = mouse.MaxAngularVelocty;

                finalResultsAvgVelocity[ntgRowCounter, 0] = age;
                finalResultsAvgVelocity[ntgRowCounter, colNumber] = mouse.AverageVelocity;

                finalResultsAvgAngVelocity[ntgRowCounter, 0] = age;
                finalResultsAvgAngVelocity[ntgRowCounter, colNumber] = mouse.AverageAngularVelocity;

                finalResultsAvgPelvicArea[ntgRowCounter, 0] = age;
                finalResultsAvgPelvicArea[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic1();

                finalResultsAvgPelvicArea2[ntgRowCounter, 0] = age;
                finalResultsAvgPelvicArea2[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic2();

                finalResultsAvgPelvicArea3[ntgRowCounter, 0] = age;
                finalResultsAvgPelvicArea3[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic3();

                finalResultsAvgPelvicArea4[ntgRowCounter, 0] = age;
                finalResultsAvgPelvicArea4[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic4();

                finalResultsAvgCentroidVelocity[ntgRowCounter, 0] = age;
                finalResultsAvgCentroidVelocity[ntgRowCounter, colNumber] = mouse.AverageCentroidVelocity;

                finalResultsMaxCentroidVelocity[ntgRowCounter, 0] = age;
                finalResultsMaxCentroidVelocity[ntgRowCounter, colNumber] = mouse.MaxCentroidSpeed;

                ntgRowCounter++;
            }

            if (ntgRowCounter > tgRowCounter)
            {
                tgRowCounter = ntgRowCounter;
            }
            else
            {
                ntgRowCounter = tgRowCounter;
            }

            foreach (IMouseDataResult mouse in tg90)
            {
                int colNumber = 1;
                string age = "90";
                finalResultsWaist[tgRowCounter, 0] = age;
                finalResultsWaist[tgRowCounter, colNumber] = mouse.GetCentroidWidthForRunning();

                finalResultsDuration[tgRowCounter, 0] = age;
                finalResultsDuration[tgRowCounter, colNumber] = mouse.Duration;

                finalResultsDistance[tgRowCounter, 0] = age;
                finalResultsDistance[tgRowCounter, colNumber] = mouse.DistanceTravelled;

                finalResultsMaxVelocity[tgRowCounter, 0] = age;
                finalResultsMaxVelocity[tgRowCounter, colNumber] = mouse.MaxSpeed;

                finalResultsMaxAngVelocity[tgRowCounter, 0] = age;
                finalResultsMaxAngVelocity[tgRowCounter, colNumber] = mouse.MaxAngularVelocty;

                finalResultsAvgVelocity[tgRowCounter, 0] = age;
                finalResultsAvgVelocity[tgRowCounter, colNumber] = mouse.AverageVelocity;

                finalResultsAvgAngVelocity[tgRowCounter, 0] = age;
                finalResultsAvgAngVelocity[tgRowCounter, colNumber] = mouse.AverageAngularVelocity;

                finalResultsAvgPelvicArea[tgRowCounter, 0] = age;
                finalResultsAvgPelvicArea[tgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic1();

                finalResultsAvgPelvicArea2[tgRowCounter, 0] = age;
                finalResultsAvgPelvicArea2[tgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic2();

                finalResultsAvgPelvicArea3[tgRowCounter, 0] = age;
                finalResultsAvgPelvicArea3[tgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic3();

                finalResultsAvgPelvicArea4[tgRowCounter, 0] = age;
                finalResultsAvgPelvicArea4[tgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic4();

                finalResultsAvgCentroidVelocity[tgRowCounter, 0] = age;
                finalResultsAvgCentroidVelocity[tgRowCounter, colNumber] = mouse.AverageCentroidVelocity;

                finalResultsMaxCentroidVelocity[tgRowCounter, 0] = age;
                finalResultsMaxCentroidVelocity[tgRowCounter, colNumber] = mouse.MaxCentroidSpeed;

                tgRowCounter++;
            }

            foreach (IMouseDataResult mouse in ntg90)
            {
                int colNumber = 2;
                string age = "90";
                finalResultsWaist[ntgRowCounter, 0] = age;
                finalResultsWaist[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForRunning();

                finalResultsDuration[ntgRowCounter, 0] = age;
                finalResultsDuration[ntgRowCounter, colNumber] = mouse.Duration;

                finalResultsDistance[ntgRowCounter, 0] = age;
                finalResultsDistance[ntgRowCounter, colNumber] = mouse.DistanceTravelled;

                finalResultsMaxVelocity[ntgRowCounter, 0] = age;
                finalResultsMaxVelocity[ntgRowCounter, colNumber] = mouse.MaxSpeed;

                finalResultsMaxAngVelocity[ntgRowCounter, 0] = age;
                finalResultsMaxAngVelocity[ntgRowCounter, colNumber] = mouse.MaxAngularVelocty;

                finalResultsAvgVelocity[ntgRowCounter, 0] = age;
                finalResultsAvgVelocity[ntgRowCounter, colNumber] = mouse.AverageVelocity;

                finalResultsAvgAngVelocity[ntgRowCounter, 0] = age;
                finalResultsAvgAngVelocity[ntgRowCounter, colNumber] = mouse.AverageAngularVelocity;

                finalResultsAvgPelvicArea[ntgRowCounter, 0] = age;
                finalResultsAvgPelvicArea[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic1();

                finalResultsAvgPelvicArea2[ntgRowCounter, 0] = age;
                finalResultsAvgPelvicArea2[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic2();

                finalResultsAvgPelvicArea3[ntgRowCounter, 0] = age;
                finalResultsAvgPelvicArea3[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic3();

                finalResultsAvgPelvicArea4[ntgRowCounter, 0] = age;
                finalResultsAvgPelvicArea4[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic4();

                finalResultsAvgCentroidVelocity[ntgRowCounter, 0] = age;
                finalResultsAvgCentroidVelocity[ntgRowCounter, colNumber] = mouse.AverageCentroidVelocity;

                finalResultsMaxCentroidVelocity[ntgRowCounter, 0] = age;
                finalResultsMaxCentroidVelocity[ntgRowCounter, colNumber] = mouse.MaxCentroidSpeed;

                ntgRowCounter++;
            }

            if (ntgRowCounter > tgRowCounter)
            {
                tgRowCounter = ntgRowCounter;
            }
            else
            {
                ntgRowCounter = tgRowCounter;
            }

            foreach (IMouseDataResult mouse in tg120)
            {
                int colNumber = 1;
                string age = "120";
                finalResultsWaist[tgRowCounter, 0] = age;
                finalResultsWaist[tgRowCounter, colNumber] = mouse.GetCentroidWidthForRunning();

                finalResultsDuration[tgRowCounter, 0] = age;
                finalResultsDuration[tgRowCounter, colNumber] = mouse.Duration;

                finalResultsDistance[tgRowCounter, 0] = age;
                finalResultsDistance[tgRowCounter, colNumber] = mouse.DistanceTravelled;

                finalResultsMaxVelocity[tgRowCounter, 0] = age;
                finalResultsMaxVelocity[tgRowCounter, colNumber] = mouse.MaxSpeed;

                finalResultsMaxAngVelocity[tgRowCounter, 0] = age;
                finalResultsMaxAngVelocity[tgRowCounter, colNumber] = mouse.MaxAngularVelocty;

                finalResultsAvgVelocity[tgRowCounter, 0] = age;
                finalResultsAvgVelocity[tgRowCounter, colNumber] = mouse.AverageVelocity;

                finalResultsAvgAngVelocity[tgRowCounter, 0] = age;
                finalResultsAvgAngVelocity[tgRowCounter, colNumber] = mouse.AverageAngularVelocity;

                finalResultsAvgPelvicArea[tgRowCounter, 0] = age;
                finalResultsAvgPelvicArea[tgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic1();

                finalResultsAvgPelvicArea2[tgRowCounter, 0] = age;
                finalResultsAvgPelvicArea2[tgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic2();

                finalResultsAvgPelvicArea3[tgRowCounter, 0] = age;
                finalResultsAvgPelvicArea3[tgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic3();

                finalResultsAvgPelvicArea4[tgRowCounter, 0] = age;
                finalResultsAvgPelvicArea4[tgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic4();

                finalResultsAvgCentroidVelocity[tgRowCounter, 0] = age;
                finalResultsAvgCentroidVelocity[tgRowCounter, colNumber] = mouse.AverageCentroidVelocity;

                finalResultsMaxCentroidVelocity[tgRowCounter, 0] = age;
                finalResultsMaxCentroidVelocity[tgRowCounter, colNumber] = mouse.MaxCentroidSpeed;

                tgRowCounter++;
            }

            foreach (IMouseDataResult mouse in ntg120)
            {
                int colNumber = 2;
                string age = "120";
                finalResultsWaist[ntgRowCounter, 0] = age;
                finalResultsWaist[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForRunning();

                finalResultsDuration[ntgRowCounter, 0] = age;
                finalResultsDuration[ntgRowCounter, colNumber] = mouse.Duration;

                finalResultsDistance[ntgRowCounter, 0] = age;
                finalResultsDistance[ntgRowCounter, colNumber] = mouse.DistanceTravelled;

                finalResultsMaxVelocity[ntgRowCounter, 0] = age;
                finalResultsMaxVelocity[ntgRowCounter, colNumber] = mouse.MaxSpeed;

                finalResultsMaxAngVelocity[ntgRowCounter, 0] = age;
                finalResultsMaxAngVelocity[ntgRowCounter, colNumber] = mouse.MaxAngularVelocty;

                finalResultsAvgVelocity[ntgRowCounter, 0] = age;
                finalResultsAvgVelocity[ntgRowCounter, colNumber] = mouse.AverageVelocity;

                finalResultsAvgAngVelocity[ntgRowCounter, 0] = age;
                finalResultsAvgAngVelocity[ntgRowCounter, colNumber] = mouse.AverageAngularVelocity;

                finalResultsAvgPelvicArea[ntgRowCounter, 0] = age;
                finalResultsAvgPelvicArea[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic1();

                finalResultsAvgPelvicArea2[ntgRowCounter, 0] = age;
                finalResultsAvgPelvicArea2[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic2();

                finalResultsAvgPelvicArea3[ntgRowCounter, 0] = age;
                finalResultsAvgPelvicArea3[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic3();

                finalResultsAvgPelvicArea4[ntgRowCounter, 0] = age;
                finalResultsAvgPelvicArea4[ntgRowCounter, colNumber] = mouse.GetCentroidWidthForPelvic4();

                finalResultsAvgCentroidVelocity[ntgRowCounter, 0] = age;
                finalResultsAvgCentroidVelocity[ntgRowCounter, colNumber] = mouse.AverageCentroidVelocity;

                finalResultsMaxCentroidVelocity[ntgRowCounter, 0] = age;
                finalResultsMaxCentroidVelocity[ntgRowCounter, colNumber] = mouse.MaxCentroidSpeed;

                ntgRowCounter++;
            }

            string folderLocation = FileBrowser.BrowseForFolder();

            if (string.IsNullOrWhiteSpace(folderLocation))
            {
                return;
            }

            if (!folderLocation.EndsWith(@"\"))
            {
                folderLocation += @"\";
            }

            ExcelService.WriteData(finalResultsWaist, folderLocation + "Waist.xlsx");
            ExcelService.WriteData(finalResultsDuration, folderLocation + "Duration.xlsx");
            ExcelService.WriteData(finalResultsDistance, folderLocation + "Distance.xlsx");
            ExcelService.WriteData(finalResultsMaxVelocity, folderLocation + "MaxVelcoity.xlsx");
            ExcelService.WriteData(finalResultsMaxAngVelocity, folderLocation + "MaxAngVelocity.xlsx");
            ExcelService.WriteData(finalResultsAvgVelocity, folderLocation + "AvgVelocity.xlsx");
            ExcelService.WriteData(finalResultsAvgAngVelocity, folderLocation + "AvgAngVelocity.xlsx");
            ExcelService.WriteData(finalResultsAvgPelvicArea, folderLocation + "PelvicArea1.xlsx");
            ExcelService.WriteData(finalResultsAvgPelvicArea2, folderLocation + "PelvicArea2.xlsx");
            ExcelService.WriteData(finalResultsAvgPelvicArea3, folderLocation + "PelvicArea3.xlsx");
            ExcelService.WriteData(finalResultsAvgPelvicArea4, folderLocation + "PelvicArea4.xlsx");
            ExcelService.WriteData(finalResultsAvgCentroidVelocity, folderLocation + "AvgCentroidVelocity.xlsx");
            ExcelService.WriteData(finalResultsMaxCentroidVelocity, folderLocation + "MaxCentroidVelocity.xlsx");
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

        private void GenerateIndividualResults5()
        {
            int rowCount = 1000;
            int columnCount = 7;

            object[,] finalResultsWaist = new object[rowCount, columnCount];
            object[,] finalResultsDuration = new object[rowCount, columnCount];
            object[,] finalResultsDistance = new object[rowCount, columnCount];
            object[,] finalResultsMaxVelocity = new object[rowCount, columnCount];
            object[,] finalResultsMaxAngVelocity = new object[rowCount, columnCount];
            object[,] finalResultsAvgVelocity = new object[rowCount, columnCount];
            object[,] finalResultsAvgAngVelocity = new object[rowCount, columnCount];
            object[,] finalResultsAvgPelvicArea = new object[rowCount, columnCount];
            object[,] finalResultsAvgPelvicArea2 = new object[rowCount, columnCount];
            object[,] finalResultsAvgPelvicArea3 = new object[rowCount, columnCount];
            object[,] finalResultsAvgPelvicArea4 = new object[rowCount, columnCount];
            object[,] finalResultsAvgCentroidVelocity = new object[rowCount, columnCount];
            object[,] finalResultsMaxCentroidVelocity = new object[rowCount, columnCount];

            object[,] finalResultsWaistMouse = new object[rowCount, columnCount];
            object[,] finalResultsDurationMouse = new object[rowCount, columnCount];
            object[,] finalResultsDistanceMouse = new object[rowCount, columnCount];
            object[,] finalResultsMaxVelocityMouse = new object[rowCount, columnCount];
            object[,] finalResultsMaxAngVelocityMouse = new object[rowCount, columnCount];
            object[,] finalResultsAvgVelocityMouse = new object[rowCount, columnCount];
            object[,] finalResultsAvgAngVelocityMouse = new object[rowCount, columnCount];
            object[,] finalResultsAvgPelvicAreaMouse = new object[rowCount, columnCount];
            object[,] finalResultsAvgPelvicArea2Mouse = new object[rowCount, columnCount];
            object[,] finalResultsAvgPelvicArea3Mouse = new object[rowCount, columnCount];
            object[,] finalResultsAvgPelvicArea4Mouse = new object[rowCount, columnCount];
            object[,] finalResultsAvgCentroidVelocityMouse = new object[rowCount, columnCount];
            object[,] finalResultsMaxCentroidVelocityMouse = new object[rowCount, columnCount];

            finalResultsWaist[0, 0] = "Variable";
            finalResultsWaist[0, 1] = "Average";
            finalResultsWaist[0, 2] = "Age";
            finalResultsWaist[0, 3] = "Mice";
            finalResultsWaist[0, 4] = "File";
            finalResultsWaist[0, 5] = "Mouse";
            finalResultsWaist[0, 6] = "Class";

            finalResultsDuration[0, 0] = "Variable";
            finalResultsDuration[0, 1] = "Average";
            finalResultsDuration[0, 2] = "Age";
            finalResultsDuration[0, 3] = "Mice";
            finalResultsDuration[0, 4] = "File";
            finalResultsDuration[0, 5] = "Mouse";
            finalResultsDuration[0, 6] = "Class";

            finalResultsDistance[0, 0] = "Variable";
            finalResultsDistance[0, 1] = "Average";
            finalResultsDistance[0, 2] = "Age";
            finalResultsDistance[0, 3] = "Mice";
            finalResultsDistance[0, 4] = "File";
            finalResultsDistance[0, 5] = "Mouse";
            finalResultsDistance[0, 6] = "Class";

            finalResultsMaxVelocity[0, 0] = "Variable";
            finalResultsMaxVelocity[0, 1] = "Average";
            finalResultsMaxVelocity[0, 2] = "Age";
            finalResultsMaxVelocity[0, 3] = "Mice";
            finalResultsMaxVelocity[0, 4] = "File";
            finalResultsMaxVelocity[0, 5] = "Mouse";
            finalResultsMaxVelocity[0, 6] = "Class";

            finalResultsMaxAngVelocity[0, 0] = "Variable";
            finalResultsMaxAngVelocity[0, 1] = "Average";
            finalResultsMaxAngVelocity[0, 2] = "Age";
            finalResultsMaxAngVelocity[0, 3] = "Mice";
            finalResultsMaxAngVelocity[0, 4] = "File";
            finalResultsMaxAngVelocity[0, 5] = "Mouse";
            finalResultsMaxAngVelocity[0, 6] = "Class";

            finalResultsAvgVelocity[0, 0] = "Variable";
            finalResultsAvgVelocity[0, 1] = "Average";
            finalResultsAvgVelocity[0, 2] = "Age";
            finalResultsAvgVelocity[0, 3] = "Mice";
            finalResultsAvgVelocity[0, 4] = "File";
            finalResultsAvgVelocity[0, 5] = "Mouse";
            finalResultsAvgVelocity[0, 6] = "Class";

            finalResultsAvgAngVelocity[0, 0] = "Variable";
            finalResultsAvgAngVelocity[0, 1] = "Average";
            finalResultsAvgAngVelocity[0, 2] = "Age";
            finalResultsAvgAngVelocity[0, 3] = "Mice";
            finalResultsAvgAngVelocity[0, 4] = "File";
            finalResultsAvgAngVelocity[0, 5] = "Mouse";
            finalResultsAvgAngVelocity[0, 6] = "Class";

            finalResultsAvgPelvicArea[0, 0] = "Variable";
            finalResultsAvgPelvicArea[0, 1] = "Average";
            finalResultsAvgPelvicArea[0, 2] = "Age";
            finalResultsAvgPelvicArea[0, 3] = "Mice";
            finalResultsAvgPelvicArea[0, 4] = "File";
            finalResultsAvgPelvicArea[0, 5] = "Mouse";
            finalResultsAvgPelvicArea[0, 6] = "Class";

            finalResultsAvgPelvicArea2[0, 0] = "Variable";
            finalResultsAvgPelvicArea2[0, 1] = "Average";
            finalResultsAvgPelvicArea2[0, 2] = "Age";
            finalResultsAvgPelvicArea2[0, 3] = "Mice";
            finalResultsAvgPelvicArea2[0, 4] = "File";
            finalResultsAvgPelvicArea2[0, 5] = "Mouse";
            finalResultsAvgPelvicArea2[0, 6] = "Class";

            finalResultsAvgPelvicArea3[0, 0] = "Variable";
            finalResultsAvgPelvicArea3[0, 1] = "Average";
            finalResultsAvgPelvicArea3[0, 2] = "Age";
            finalResultsAvgPelvicArea3[0, 3] = "Mice";
            finalResultsAvgPelvicArea3[0, 4] = "File";
            finalResultsAvgPelvicArea3[0, 5] = "Mouse";
            finalResultsAvgPelvicArea3[0, 6] = "Class";

            finalResultsAvgPelvicArea4[0, 0] = "Variable";
            finalResultsAvgPelvicArea4[0, 1] = "Average";
            finalResultsAvgPelvicArea4[0, 2] = "Age";
            finalResultsAvgPelvicArea4[0, 3] = "Mice";
            finalResultsAvgPelvicArea4[0, 4] = "File";
            finalResultsAvgPelvicArea4[0, 5] = "Mouse";
            finalResultsAvgPelvicArea4[0, 6] = "Class";

            finalResultsAvgCentroidVelocity[0, 0] = "Variable";
            finalResultsAvgCentroidVelocity[0, 1] = "Average";
            finalResultsAvgCentroidVelocity[0, 2] = "Age";
            finalResultsAvgCentroidVelocity[0, 3] = "Mice";
            finalResultsAvgCentroidVelocity[0, 4] = "File";
            finalResultsAvgCentroidVelocity[0, 5] = "Mouse";
            finalResultsAvgCentroidVelocity[0, 6] = "Class";

            finalResultsMaxCentroidVelocity[0, 0] = "Variable";
            finalResultsMaxCentroidVelocity[0, 1] = "Average";
            finalResultsMaxCentroidVelocity[0, 2] = "Age";
            finalResultsMaxCentroidVelocity[0, 3] = "Mice";
            finalResultsMaxCentroidVelocity[0, 4] = "File";
            finalResultsMaxCentroidVelocity[0, 5] = "Mouse";
            finalResultsMaxCentroidVelocity[0, 6] = "Class";

            //finalResults[1, 0] = "30";

            //finalResults[1, 1] = "Waist";
            //finalResults[1, 2] = "Duration";
            //finalResults[1, 3] = "Distance";
            //finalResults[1, 4] = "Max Velocity";
            //finalResults[1, 5] = "Max Ang Velocity";
            //finalResults[1, 6] = "Average Velocity";
            //finalResults[1, 7] = "Average Ang Velocity";
            //finalResults[1, 8] = "Average Pelvic area";
            //finalResults[1, 9] = "Average Pelvic area2";
            //finalResults[1, 10] = "Average Pelvic area3";
            //finalResults[1, 11] = "Average Pelvic area4";
            //finalResults[1, 12] = "Average Centroid Velocity";
            //finalResults[1, 13] = "Max Centroid Velocity";

            //List<IMouseDataResult> allResults = new List<IMouseDataResult>();

            List<MouseHolder> tg30 = new List<MouseHolder>(), tg60 = new List<MouseHolder>(), tg90 = new List<MouseHolder>(), tg120 = new List<MouseHolder>(), ntg30 = new List<MouseHolder>(), ntg60 = new List<MouseHolder>(), ntg90 = new List<MouseHolder>(), ntg120 = new List<MouseHolder>();

            //tg30 = Videos.Select(vid => vid.VideoFiles.Where(x => vid.Results[x].Type is ITransgenic && vid.Results[x].Age == 30));

            const double factor100118 = 0.0016;
            const double factorOther = 0.0039;

            foreach (SingleMouseViewModel mouse in Videos)
            {
                //List<IMouseDataResult> enumTg30;
                foreach (ISingleFile file in mouse.VideoFiles)
                {
                    IMouseDataResult result = mouse.Results[file];

                    if (mouse.Class.ToLower().Contains("100118"))
                    {
                        result.AverageVelocity *= factor100118;
                        result.AverageCentroidVelocity *= factor100118;
                        result.CentroidSize *= 0.08;
                        result.AverageWaist = result.GetCentroidWidthForRunning() * 0.08;
                        result.DistanceTravelled *= 0.08;
                        result.MaxSpeed *= factor100118;
                        result.MaxCentroidSpeed *= factor100118;
                    }
                    else
                    {
                        result.AverageVelocity *= factorOther;
                        result.AverageCentroidVelocity *= factorOther;
                        result.CentroidSize *= 0.195;
                        result.AverageWaist = result.GetCentroidWidthForRunning() * 0.195;
                        result.DistanceTravelled *= 0.195;
                        result.MaxSpeed *= factorOther;
                        result.MaxCentroidSpeed *= factorOther;
                    }

                    result.AverageAngularVelocity *= 0.02;
                    result.MaxAngularVelocty *= 0.02;

                    List<MouseHolder> listToUse = null;
                    if (mouse.Type is ITransgenic)
                    {
                        switch (mouse.Age)
                        {
                            case 30:
                                listToUse = tg30;
                                break;

                            case 60:
                                listToUse = tg60;
                                break;

                            case 90:
                                listToUse = tg90;
                                break;

                            case 120:
                                listToUse = tg120;
                                break;

                        }
                    }
                    else if (mouse.Type is INonTransgenic)
                    {
                        switch (mouse.Age)
                        {
                            case 30:
                                listToUse = ntg30;
                                break;

                            case 60:
                                listToUse = ntg60;
                                break;

                            case 90:
                                listToUse = ntg90;
                                break;

                            case 120:
                                listToUse = ntg120;
                                break;
                        }
                    }

                    if (listToUse != null)
                    {
                        listToUse.Add(new MouseHolder()
                        {
                            Age = mouse.Age.ToString(),
                            Class = mouse.Class,
                            File = file.VideoFileName,
                            Mouse = mouse,
                            Result = result,
                            Type = mouse.Type.ToString(),
                        });
                    }
                }

                //IMouseDataResult[] resultsForMouse = mouse.VideoFiles.Select(singleFile => mouse.Results[singleFile]).ToArray();

                //foreach (IMouseDataResult mouse2 in resultsForMouse)
                //{
                //    if (mouse.Class.ToLower().Contains("100118"))
                //    {
                //        mouse2.AverageVelocity *= factor100118;
                //        mouse2.AverageCentroidVelocity *= factor100118;
                //        mouse2.CentroidSize *= 0.08;
                //        mouse2.AverageWaist = mouse2.GetCentroidWidthForRunning()*0.08;
                //        mouse2.DistanceTravelled *= 0.08;
                //        mouse2.MaxSpeed *= factor100118;
                //        mouse2.MaxCentroidSpeed *= factor100118;
                //    }
                //    else
                //    {
                //        mouse2.AverageVelocity *= factorOther;
                //        mouse2.AverageCentroidVelocity *= factorOther;
                //        mouse2.CentroidSize *= 0.195;
                //        mouse2.AverageWaist = mouse2.GetCentroidWidthForRunning()*0.195;
                //        mouse2.DistanceTravelled *= 0.195;
                //        mouse2.MaxSpeed *= factorOther;
                //        mouse2.MaxCentroidSpeed *= factorOther;
                //    }

                //    mouse2.AverageAngularVelocity *= 0.02;
                //    mouse2.MaxAngularVelocty *= 0.02;
                //}

                //IEnumerable<IMouseDataResult> enumTg30 = resultsForMouse.Where(x => x.Type is ITransgenic && x.Age == 30);
                //IEnumerable<IMouseDataResult> enumTg60 = resultsForMouse.Where(x => x.Type is ITransgenic && x.Age == 60);
                //IEnumerable<IMouseDataResult> enumTg90 = resultsForMouse.Where(x => x.Type is ITransgenic && x.Age == 90);
                //IEnumerable<IMouseDataResult> enumTg120 = resultsForMouse.Where(x => x.Type is ITransgenic && x.Age == 120);
                //IEnumerable<IMouseDataResult> enumNtg30 = resultsForMouse.Where(x => x.Type is INonTransgenic && x.Age == 30);
                //IEnumerable<IMouseDataResult> enumNtg60 = resultsForMouse.Where(x => x.Type is INonTransgenic && x.Age == 60);
                //IEnumerable<IMouseDataResult> enumNtg90 = resultsForMouse.Where(x => x.Type is INonTransgenic && x.Age == 90);
                //IEnumerable<IMouseDataResult> enumNtg120 = resultsForMouse.Where(x => x.Type is INonTransgenic && x.Age == 120);

                ////tg30.AddRange(resultsForMouse.Where(x => x.Type is ITransgenic && x.Age == 30));
                //tg30.AddRange(enumTg30.Select(md => new MouseHolder()
                //{
                //    Age = "30", Class = mouse.Class, Mouse = mouse, Result = md, Type = "Transgenic",
                //}));
                
                ////tg60.AddRange(resultsForMouse.Where(x => x.Type is ITransgenic && x.Age == 60));
                //tg60.AddRange(enumTg60.Select(md => new MouseHolder()
                //{
                //    Age = "60",
                //    Class = mouse.Class,
                //    Mouse = mouse,
                //    Result = md,
                //    Type = "Transgenic",
                //}));

                ////tg90.AddRange(resultsForMouse.Where(x => x.Type is ITransgenic && x.Age == 90));
                //tg90.AddRange(enumTg90.Select(md => new MouseHolder()
                //{
                //    Age = "90",
                //    Class = mouse.Class,
                //    Mouse = mouse,
                //    Result = md,
                //    Type = "Transgenic",
                //}));
                
                ////tg120.AddRange(resultsForMouse.Where(x => x.Type is ITransgenic && x.Age == 120));
                //tg120.AddRange(enumTg120.Select(md => new MouseHolder()
                //{
                //    Age = "120",
                //    Class = mouse.Class,
                //    Mouse = mouse,
                //    Result = md,
                //    Type = "Transgenic",
                //}));

                ////ntg30.AddRange(resultsForMouse.Where(x => x.Type is INonTransgenic && x.Age == 30));
                //ntg30.AddRange(enumNtg30.Select(md => new MouseHolder()
                //{
                //    Age = "30",
                //    Class = mouse.Class,
                //    Mouse = mouse,
                //    Result = md,
                //    Type = "Non-Transgenic",
                //}));

                ////ntg60.AddRange(resultsForMouse.Where(x => x.Type is INonTransgenic && x.Age == 60));
                //ntg60.AddRange(enumNtg60.Select(md => new MouseHolder()
                //{
                //    Age = "60",
                //    Class = mouse.Class,
                //    Mouse = mouse,
                //    Result = md,
                //    Type = "Non-Transgenic",
                //}));

                ////ntg90.AddRange(resultsForMouse.Where(x => x.Type is INonTransgenic && x.Age == 90));
                //ntg90.AddRange(enumNtg90.Select(md => new MouseHolder()
                //{
                //    Age = "90",
                //    Class = mouse.Class,
                //    Mouse = mouse,
                //    Result = md,
                //    Type = "Non-Transgenic",
                //}));

                ////ntg120.AddRange(resultsForMouse.Where(x => x.Type is INonTransgenic && x.Age == 120));
                //ntg120.AddRange(enumNtg120.Select(md => new MouseHolder()
                //{
                //    Age = "120",
                //    Class = mouse.Class,
                //    Mouse = mouse,
                //    Result = md,
                //    Type = "Non-Transgenic",
                //}));
            }

            int rowCounter = 1;

            int waistCounter = 1;
            int pelvic1Counter = 1;
            int pelvic2Counter = 1;
            int pelvic3Counter = 1;
            int pelvic4Counter = 1;

            foreach (MouseHolder mouse in tg30)
            {
                //int colNumber = 1;
                string age = "30";
                string type = "SOD1";
                
                UpdateData(mouse, finalResultsWaist, ref waistCounter, age, type, finalResultsDuration, rowCounter, finalResultsDistance, finalResultsMaxVelocity, finalResultsMaxAngVelocity, finalResultsAvgVelocity, finalResultsAvgAngVelocity, finalResultsAvgPelvicArea, finalResultsAvgPelvicArea2, finalResultsAvgPelvicArea3, finalResultsAvgPelvicArea4, finalResultsAvgCentroidVelocity, finalResultsMaxCentroidVelocity, ref pelvic1Counter, ref pelvic2Counter, ref pelvic3Counter, ref pelvic4Counter);

                rowCounter++;


            }



            foreach (MouseHolder mouse in ntg30)
            {
                //int colNumber = 2;
                string age = "30";
                string type = "Control";

                UpdateData(mouse, finalResultsWaist, ref waistCounter, age, type, finalResultsDuration, rowCounter, finalResultsDistance, finalResultsMaxVelocity, finalResultsMaxAngVelocity, finalResultsAvgVelocity, finalResultsAvgAngVelocity, finalResultsAvgPelvicArea, finalResultsAvgPelvicArea2, finalResultsAvgPelvicArea3, finalResultsAvgPelvicArea4, finalResultsAvgCentroidVelocity, finalResultsMaxCentroidVelocity, ref pelvic1Counter, ref pelvic2Counter, ref pelvic3Counter, ref pelvic4Counter);

                rowCounter++;
            }

            foreach (MouseHolder mouse in tg60)
            {
                string age = "60";
                string type = "SOD1";

                UpdateData(mouse, finalResultsWaist, ref waistCounter, age, type, finalResultsDuration, rowCounter, finalResultsDistance, finalResultsMaxVelocity, finalResultsMaxAngVelocity, finalResultsAvgVelocity, finalResultsAvgAngVelocity, finalResultsAvgPelvicArea, finalResultsAvgPelvicArea2, finalResultsAvgPelvicArea3, finalResultsAvgPelvicArea4, finalResultsAvgCentroidVelocity, finalResultsMaxCentroidVelocity, ref pelvic1Counter, ref pelvic2Counter, ref pelvic3Counter, ref pelvic4Counter);
                
                rowCounter++;
            }

            foreach (MouseHolder mouse in ntg60)
            {
                string age = "60";
                string type = "Control";

                UpdateData(mouse, finalResultsWaist, ref waistCounter, age, type, finalResultsDuration, rowCounter, finalResultsDistance, finalResultsMaxVelocity, finalResultsMaxAngVelocity, finalResultsAvgVelocity, finalResultsAvgAngVelocity, finalResultsAvgPelvicArea, finalResultsAvgPelvicArea2, finalResultsAvgPelvicArea3, finalResultsAvgPelvicArea4, finalResultsAvgCentroidVelocity, finalResultsMaxCentroidVelocity, ref pelvic1Counter, ref pelvic2Counter, ref pelvic3Counter, ref pelvic4Counter);

                rowCounter++;
            }

            foreach (MouseHolder mouse in tg90)
            {
                string age = "90";
                string type = "SOD1";

                UpdateData(mouse, finalResultsWaist, ref waistCounter, age, type, finalResultsDuration, rowCounter, finalResultsDistance, finalResultsMaxVelocity, finalResultsMaxAngVelocity, finalResultsAvgVelocity, finalResultsAvgAngVelocity, finalResultsAvgPelvicArea, finalResultsAvgPelvicArea2, finalResultsAvgPelvicArea3, finalResultsAvgPelvicArea4, finalResultsAvgCentroidVelocity, finalResultsMaxCentroidVelocity, ref pelvic1Counter, ref pelvic2Counter, ref pelvic3Counter, ref pelvic4Counter);

                rowCounter++;
            }

            foreach (MouseHolder mouse in ntg90)
            {
                string age = "90";
                string type = "Control";

                UpdateData(mouse, finalResultsWaist, ref waistCounter, age, type, finalResultsDuration, rowCounter, finalResultsDistance, finalResultsMaxVelocity, finalResultsMaxAngVelocity, finalResultsAvgVelocity, finalResultsAvgAngVelocity, finalResultsAvgPelvicArea, finalResultsAvgPelvicArea2, finalResultsAvgPelvicArea3, finalResultsAvgPelvicArea4, finalResultsAvgCentroidVelocity, finalResultsMaxCentroidVelocity, ref pelvic1Counter, ref pelvic2Counter, ref pelvic3Counter, ref pelvic4Counter);

                rowCounter++;
            }

            foreach (MouseHolder mouse in tg120)
            {
                string age = "120";
                string type = "SOD1";

                UpdateData(mouse, finalResultsWaist, ref waistCounter, age, type, finalResultsDuration, rowCounter, finalResultsDistance, finalResultsMaxVelocity, finalResultsMaxAngVelocity, finalResultsAvgVelocity, finalResultsAvgAngVelocity, finalResultsAvgPelvicArea, finalResultsAvgPelvicArea2, finalResultsAvgPelvicArea3, finalResultsAvgPelvicArea4, finalResultsAvgCentroidVelocity, finalResultsMaxCentroidVelocity, ref pelvic1Counter, ref pelvic2Counter, ref pelvic3Counter, ref pelvic4Counter);

                rowCounter++;
            }

            foreach (MouseHolder mouse in ntg120)
            {
                string age = "120";
                string type = "Control";

                UpdateData(mouse, finalResultsWaist, ref waistCounter, age, type, finalResultsDuration, rowCounter, finalResultsDistance, finalResultsMaxVelocity, finalResultsMaxAngVelocity, finalResultsAvgVelocity, finalResultsAvgAngVelocity, finalResultsAvgPelvicArea, finalResultsAvgPelvicArea2, finalResultsAvgPelvicArea3, finalResultsAvgPelvicArea4, finalResultsAvgCentroidVelocity, finalResultsMaxCentroidVelocity, ref pelvic1Counter, ref pelvic2Counter, ref pelvic3Counter, ref pelvic4Counter);

                rowCounter++;
            }

            string folderLocation = FileBrowser.BrowseForFolder();

            if (string.IsNullOrWhiteSpace(folderLocation))
            {
                return;
            }

            if (!folderLocation.EndsWith(@"\"))
            {
                folderLocation += @"\";
            }

            ExcelService.WriteData(finalResultsWaist, folderLocation + "Waist.xlsx");
            ExcelService.WriteData(finalResultsDuration, folderLocation + "Duration.xlsx");
            ExcelService.WriteData(finalResultsDistance, folderLocation + "Distance.xlsx");
            ExcelService.WriteData(finalResultsMaxVelocity, folderLocation + "MaxVelcoity.xlsx");
            ExcelService.WriteData(finalResultsMaxAngVelocity, folderLocation + "MaxAngVelocity.xlsx");
            ExcelService.WriteData(finalResultsAvgVelocity, folderLocation + "AvgVelocity.xlsx");
            ExcelService.WriteData(finalResultsAvgAngVelocity, folderLocation + "AvgAngVelocity.xlsx");
            ExcelService.WriteData(finalResultsAvgPelvicArea, folderLocation + "PelvicArea1.xlsx");
            ExcelService.WriteData(finalResultsAvgPelvicArea2, folderLocation + "PelvicArea2.xlsx");
            ExcelService.WriteData(finalResultsAvgPelvicArea3, folderLocation + "PelvicArea3.xlsx");
            ExcelService.WriteData(finalResultsAvgPelvicArea4, folderLocation + "PelvicArea4.xlsx");
            ExcelService.WriteData(finalResultsAvgCentroidVelocity, folderLocation + "AvgCentroidVelocity.xlsx");
            ExcelService.WriteData(finalResultsMaxCentroidVelocity, folderLocation + "MaxCentroidVelocity.xlsx");
        }

        private void UpdateData(MouseHolder mouseHolder, object[,] finalResultsWaist, ref int waistCounter, string age, string type, object[,] finalResultsDuration, int rowCounter, object[,] finalResultsDistance, object[,] finalResultsMaxVelocity, object[,] finalResultsMaxAngVelocity, object[,] finalResultsAvgVelocity, object[,] finalResultsAvgAngVelocity, object[,] finalResultsAvgPelvicArea, object[,] finalResultsAvgPelvicArea2, object[,] finalResultsAvgPelvicArea3, object[,] finalResultsAvgPelvicArea4, object[,] finalResultsAvgCentroidVelocity, object[,] finalResultsMaxCentroidVelocity, ref int pelvic1Counter, ref int pelvic2Counter, ref int pelvic3Counter, ref int pelvic4Counter)
        {
            //File, mouse, class
            IMouseDataResult mouse = mouseHolder.Result;
            double centroid = mouse.GetCentroidWidthForRunning();
            if (centroid > 0 && centroid < 1000)
            {
                finalResultsWaist[waistCounter, 0] = "Centroid Width";
                finalResultsWaist[waistCounter, 1] = mouse.AverageWaist;
                finalResultsWaist[waistCounter, 2] = mouseHolder.Age;
                finalResultsWaist[waistCounter, 3] = mouseHolder.Type;
                finalResultsWaist[waistCounter, 4] = mouseHolder.File;
                finalResultsWaist[waistCounter, 5] = mouseHolder.Mouse.Name + " - " + mouseHolder.Mouse.Id;
                finalResultsWaist[waistCounter, 6] = mouseHolder.Mouse.Class;
                waistCounter++;
            }

            finalResultsDuration[rowCounter, 0] = "Duration";
            finalResultsDuration[rowCounter, 1] = mouse.Duration;
            finalResultsDuration[rowCounter, 2] = age;
            finalResultsDuration[rowCounter, 3] = type;
            finalResultsDuration[rowCounter, 4] = mouseHolder.File;
            finalResultsDuration[rowCounter, 5] = mouseHolder.Mouse.Name + " - " + mouseHolder.Mouse.Id;
            finalResultsDuration[rowCounter, 6] = mouseHolder.Mouse.Class;

            finalResultsDistance[rowCounter, 0] = "Distance";
            finalResultsDistance[rowCounter, 1] = mouse.DistanceTravelled;
            finalResultsDistance[rowCounter, 2] = age;
            finalResultsDistance[rowCounter, 3] = type;
            finalResultsDistance[rowCounter, 4] = mouseHolder.File;
            finalResultsDistance[rowCounter, 5] = mouseHolder.Mouse.Name + " - " + mouseHolder.Mouse.Id;
            finalResultsDistance[rowCounter, 6] = mouseHolder.Mouse.Class;

            finalResultsMaxVelocity[rowCounter, 0] = "Max Velocity";
            finalResultsMaxVelocity[rowCounter, 1] = mouse.MaxSpeed;
            finalResultsMaxVelocity[rowCounter, 2] = age;
            finalResultsMaxVelocity[rowCounter, 3] = type;
            finalResultsMaxVelocity[rowCounter, 4] = mouseHolder.File;
            finalResultsMaxVelocity[rowCounter, 5] = mouseHolder.Mouse.Name + " - " + mouseHolder.Mouse.Id;
            finalResultsMaxVelocity[rowCounter, 6] = mouseHolder.Mouse.Class;

            finalResultsMaxAngVelocity[rowCounter, 0] = "Max Angular Velocity";
            finalResultsMaxAngVelocity[rowCounter, 1] = mouse.MaxAngularVelocty;
            finalResultsMaxAngVelocity[rowCounter, 2] = age;
            finalResultsMaxAngVelocity[rowCounter, 3] = type;
            finalResultsMaxAngVelocity[rowCounter, 4] = mouseHolder.File;
            finalResultsMaxAngVelocity[rowCounter, 5] = mouseHolder.Mouse.Name + " - " + mouseHolder.Mouse.Id;
            finalResultsMaxAngVelocity[rowCounter, 6] = mouseHolder.Mouse.Class;

            finalResultsAvgVelocity[rowCounter, 0] = "Average Velocity";
            finalResultsAvgVelocity[rowCounter, 1] = mouse.AverageVelocity;
            finalResultsAvgVelocity[rowCounter, 2] = age;
            finalResultsAvgVelocity[rowCounter, 3] = type;
            finalResultsAvgVelocity[rowCounter, 4] = mouseHolder.File;
            finalResultsAvgVelocity[rowCounter, 5] = mouseHolder.Mouse.Name + " - " + mouseHolder.Mouse.Id;
            finalResultsAvgVelocity[rowCounter, 6] = mouseHolder.Mouse.Class;

            finalResultsAvgAngVelocity[rowCounter, 0] = "Average Angular Velocity";
            finalResultsAvgAngVelocity[rowCounter, 1] = mouse.AverageAngularVelocity;
            finalResultsAvgAngVelocity[rowCounter, 2] = age;
            finalResultsAvgAngVelocity[rowCounter, 3] = type;
            finalResultsAvgAngVelocity[rowCounter, 4] = mouseHolder.File;
            finalResultsAvgAngVelocity[rowCounter, 5] = mouseHolder.Mouse.Name + " - " + mouseHolder.Mouse.Id;
            finalResultsAvgAngVelocity[rowCounter, 6] = mouseHolder.Mouse.Class;

            double pelvic1 = mouse.GetCentroidWidthForPelvic1();
            if (pelvic1 > 0)
            {
                finalResultsAvgPelvicArea[pelvic1Counter, 0] = "Average Pelvic Area 1";
                finalResultsAvgPelvicArea[pelvic1Counter, 1] = pelvic1;
                finalResultsAvgPelvicArea[pelvic1Counter, 2] = age;
                finalResultsAvgPelvicArea[pelvic1Counter, 3] = type;
                finalResultsAvgPelvicArea[pelvic1Counter, 4] = mouseHolder.File;
                finalResultsAvgPelvicArea[pelvic1Counter, 5] = mouseHolder.Mouse.Name + " - " + mouseHolder.Mouse.Id;
                finalResultsAvgPelvicArea[pelvic1Counter, 6] = mouseHolder.Mouse.Class;
                pelvic1Counter++;
            }

            double pelvic2 = mouse.GetCentroidWidthForPelvic2();
            if (pelvic2 > 0)
            {
                finalResultsAvgPelvicArea2[pelvic2Counter, 0] = "Average Pelvic Area 2";
                finalResultsAvgPelvicArea2[pelvic2Counter, 1] = pelvic2;
                finalResultsAvgPelvicArea2[pelvic2Counter, 2] = age;
                finalResultsAvgPelvicArea2[pelvic2Counter, 3] = type;
                finalResultsAvgPelvicArea2[pelvic2Counter, 4] = mouseHolder.File;
                finalResultsAvgPelvicArea2[pelvic2Counter, 5] = mouseHolder.Mouse.Name + " - " + mouseHolder.Mouse.Id;
                finalResultsAvgPelvicArea2[pelvic2Counter, 6] = mouseHolder.Mouse.Class;
                pelvic2Counter++;
            }

            double pelvic3 = mouse.GetCentroidWidthForPelvic3();
            if (pelvic3 > 0)
            {
                finalResultsAvgPelvicArea3[pelvic3Counter, 0] = "Average Pelvic Area 3";
                finalResultsAvgPelvicArea3[pelvic3Counter, 1] = pelvic3;
                finalResultsAvgPelvicArea3[pelvic3Counter, 2] = age;
                finalResultsAvgPelvicArea3[pelvic3Counter, 3] = type;
                finalResultsAvgPelvicArea3[pelvic3Counter, 4] = mouseHolder.File;
                finalResultsAvgPelvicArea3[pelvic3Counter, 5] = mouseHolder.Mouse.Name + " - " + mouseHolder.Mouse.Id;
                finalResultsAvgPelvicArea3[pelvic3Counter, 6] = mouseHolder.Mouse.Class;
                pelvic3Counter++;
            }

            double pelvic4 = mouse.GetCentroidWidthForPelvic4();
            if (pelvic4 > 0)
            {
                finalResultsAvgPelvicArea4[pelvic4Counter, 0] = "Average Pelvic Area 4";
                finalResultsAvgPelvicArea4[pelvic4Counter, 1] = pelvic4;
                finalResultsAvgPelvicArea4[pelvic4Counter, 2] = age;
                finalResultsAvgPelvicArea4[pelvic4Counter, 3] = type;
                finalResultsAvgPelvicArea4[pelvic4Counter, 4] = mouseHolder.File;
                finalResultsAvgPelvicArea4[pelvic4Counter, 5] = mouseHolder.Mouse.Name + " - " + mouseHolder.Mouse.Id;
                finalResultsAvgPelvicArea4[pelvic4Counter, 6] = mouseHolder.Mouse.Class;
                pelvic4Counter++;
            }

            finalResultsAvgCentroidVelocity[rowCounter, 0] = "Average Centroid Velocity";
            finalResultsAvgCentroidVelocity[rowCounter, 1] = mouse.AverageCentroidVelocity;
            finalResultsAvgCentroidVelocity[rowCounter, 2] = age;
            finalResultsAvgCentroidVelocity[rowCounter, 3] = type;
            finalResultsAvgCentroidVelocity[rowCounter, 4] = mouseHolder.File;
            finalResultsAvgCentroidVelocity[rowCounter, 5] = mouseHolder.Mouse.Name + " - " + mouseHolder.Mouse.Id;
            finalResultsAvgCentroidVelocity[rowCounter, 6] = mouseHolder.Mouse.Class;

            finalResultsMaxCentroidVelocity[rowCounter, 0] = "Max Centroid Velocity";
            finalResultsMaxCentroidVelocity[rowCounter, 1] = mouse.MaxCentroidSpeed;
            finalResultsMaxCentroidVelocity[rowCounter, 2] = age;
            finalResultsMaxCentroidVelocity[rowCounter, 3] = type;
            finalResultsMaxCentroidVelocity[rowCounter, 4] = mouseHolder.File;
            finalResultsMaxCentroidVelocity[rowCounter, 5] = mouseHolder.Mouse.Name + " - " + mouseHolder.Mouse.Id;
            finalResultsMaxCentroidVelocity[rowCounter, 6] = mouseHolder.Mouse.Class;
        }

        private void GenerateBatchResults()
        {
            Dictionary<BatchProcessViewModel.BrettTuple<Type, int>, IMouseDataResult> sortedResults = new Dictionary<BatchProcessViewModel.BrettTuple<Type, int>, IMouseDataResult>();

            List<IMouseDataResult> resultsList = new List<IMouseDataResult>();
            foreach (SingleMouseViewModel mouse in Videos)
            {
                resultsList.AddRange(mouse.VideoFiles.Select(singleFile => mouse.Results[singleFile]));
            }

            //IMouseDataResult[] tgResults = tgResultsList.OrderBy(x => x.Age).ToArray();
            //IMouseDataResult[] ntgResults = ntgResultsList.OrderBy(x => x.Age).ToArray();

            try
            {
                int resultsLength = resultsList.Count;

                for (int i = 1; i <= resultsLength; i++)
                {
                    IMouseDataResult currentMouseResult = resultsList[i - 1];

                    if (currentMouseResult.Type == null || currentMouseResult.Type is IUndefined || currentMouseResult.Age <= 0)
                    {
                        continue;
                    }

                    BatchProcessViewModel.BrettTuple<Type, int> currentResult = new BatchProcessViewModel.BrettTuple<Type, int>(currentMouseResult.Type.GetType(), currentMouseResult.Age);
                    IMouseDataResult cumulativeResult;

                    if (sortedResults.ContainsKey(currentResult))
                    {
                        cumulativeResult = sortedResults[currentResult];
                    }
                    else
                    {
                        cumulativeResult = ModelResolver.Resolve<IMouseDataResult>();
                    }

                    double centroid = currentMouseResult.GetCentroidWidthForRunning();

                    if (centroid > 0)
                    {
                        if (cumulativeResult.CentroidsTest == null)
                        {
                            cumulativeResult.CentroidsTest = new List<double>();
                        }
                        cumulativeResult.CentroidsTest.Add(centroid);
                    }

                    cumulativeResult.Duration += currentMouseResult.Duration;
                    cumulativeResult.DistanceTravelled += currentMouseResult.DistanceTravelled;

                    double maxSpeed = currentMouseResult.MaxSpeed;
                    if (maxSpeed > cumulativeResult.MaxSpeed)
                    {
                        cumulativeResult.MaxSpeed = maxSpeed;
                    }

                    double maxAngSpeed = currentMouseResult.MaxAngularVelocty;
                    if (maxAngSpeed > cumulativeResult.MaxAngularVelocty)
                    {
                        cumulativeResult.MaxAngularVelocty = maxAngSpeed;
                    }

                    double maxCentroidSpeed = currentMouseResult.MaxCentroidSpeed;
                    if (maxCentroidSpeed > cumulativeResult.MaxCentroidSpeed)
                    {
                        cumulativeResult.MaxCentroidSpeed = maxCentroidSpeed;
                    }

                    cumulativeResult.AverageVelocity += currentMouseResult.GetAverageSpeedForMoving();
                    cumulativeResult.AverageCentroidVelocity += currentMouseResult.GetAverageCentroidSpeedForMoving();
                    cumulativeResult.AverageAngularVelocity += currentMouseResult.GetAverageAngularSpeedForTurning();
                    cumulativeResult.PelvicArea += currentMouseResult.GetCentroidWidthForPelvic1();
                    cumulativeResult.PelvicArea2 += currentMouseResult.GetCentroidWidthForPelvic2();
                    cumulativeResult.PelvicArea3 += currentMouseResult.GetCentroidWidthForPelvic3();
                    cumulativeResult.PelvicArea4 += currentMouseResult.GetCentroidWidthForPelvic4();
                    cumulativeResult.Dummy++;

                    if (!sortedResults.ContainsKey(currentResult))
                    {
                        sortedResults.Add(currentResult, cumulativeResult);
                    }

                    //}
                    //else
                    //{
                    //    cumulativeResult = ModelResolver.Resolve<IMouseDataResult>();
                    //    double centroid = currentMouseResult.GetCentroidWidthForRunning();
                    //    if (centroid > 0)
                    //    {
                    //        if (cumulativeResult.CentroidsTest == null)
                    //        {
                    //            cumulativeResult.CentroidsTest = new List<double>();
                    //        }
                    //        cumulativeResult.CentroidsTest.Add(centroid);
                    //    }

                    //    cumulativeResult.CentroidSize = currentMouseResult.GetCentroidWidthForRunning();
                    //    cumulativeResult.Duration = currentMouseResult.Duration;
                    //    cumulativeResult.DistanceTravelled = currentMouseResult.DistanceTravelled;
                    //    cumulativeResult.MaxSpeed = currentMouseResult.MaxSpeed;
                    //    cumulativeResult.MaxAngularVelocty = currentMouseResult.MaxAngularVelocty;
                    //    cumulativeResult.AverageVelocity = currentMouseResult.GetAverageSpeedForMoving();
                    //    cumulativeResult.AverageAngularVelocity = currentMouseResult.GetAverageAngularSpeedForTurning();
                    //    cumulativeResult.PelvicArea = currentMouseResult.GetCentroidWidthForPelvic1();
                    //    cumulativeResult.PelvicArea2 += currentMouseResult.GetCentroidWidthForPelvic2();
                    //    cumulativeResult.PelvicArea3 += currentMouseResult.GetCentroidWidthForPelvic3();
                    //    cumulativeResult.PelvicArea4 += currentMouseResult.GetCentroidWidthForPelvic4();
                    //    cumulativeResult.Dummy = 1;

                    //    sortedResults.Add(currentResult, cumulativeResult);
                    //}
                }
                
                //int rows = resultsLength + 10;
                const int columns = (13*2) + 1;
                object[,] finalResults = new object[6, columns];

                finalResults[2, 0] = 30;
                finalResults[3, 0] = 60;
                finalResults[4, 0] = 90;
                finalResults[5, 0] = 120;

                //finalResults[0, 0] = "Type";
                //finalResults[0, 1] = "Age";
                finalResults[0, 1] = "Waist";
                finalResults[0, 3] = "Duration";
                finalResults[0, 5] = "Distance";
                finalResults[0, 7] = "Max Velocity";
                finalResults[0, 9] = "Max Ang Velocity";
                finalResults[0, 11] = "Average Velocity";
                finalResults[0, 13] = "Average Ang Velocity";
                finalResults[0, 15] = "Average Pelvic area";
                finalResults[0, 17] = "Average Pelvic area2";
                finalResults[0, 19] = "Average Pelvic area3";
                finalResults[0, 21] = "Average Pelvic area4";
                finalResults[0, 23] = "Average Centroid Velocity";
                finalResults[0, 25] = "Max Centroid Velocity";

                finalResults[1, 1] = "Non-Transgenic";
                finalResults[1, 3] = "Non-Transgenic";
                finalResults[1, 5] = "Non-Transgenic";
                finalResults[1, 7] = "Non-Transgenic";
                finalResults[1, 9] = "Non-Transgenic";
                finalResults[1, 11] = "Non-Transgenic";
                finalResults[1, 13] = "Non-Transgenic";
                finalResults[1, 15] = "Non-Transgenic";
                finalResults[1, 17] = "Non-Transgenic";
                finalResults[1, 19] = "Non-Transgenic";
                finalResults[1, 21] = "Non-Transgenic";
                finalResults[1, 23] = "Non-Transgenic";
                finalResults[1, 25] = "Non-Transgenic";

                finalResults[1, 2] = "Transgenic";
                finalResults[1, 4] = "Transgenic";
                finalResults[1, 6] = "Transgenic";
                finalResults[1, 7 + 1] = "Transgenic";
                finalResults[1, 9 + 1] = "Transgenic";
                finalResults[1, 11 + 1] = "Transgenic";
                finalResults[1, 13 + 1] = "Transgenic";
                finalResults[1, 15 + 1] = "Transgenic";
                finalResults[1, 17 + 1] = "Transgenic";
                finalResults[1, 19 + 1] = "Transgenic";
                finalResults[1, 21 + 1] = "Transgenic";
                finalResults[1, 23 + 1] = "Transgenic";
                finalResults[1, 25 + 1] = "Transgenic";

                //int counter = 1;

                foreach (var kvp in sortedResults)
                {
                    int row;
                    switch (kvp.Key.Item2)
                    {
                        case 30:
                            row = 2;
                            break;

                        case 60:
                            row = 3;
                            break;

                        case 90:
                            row = 4;
                            break;

                        case 120:
                            row = 5;
                            break;

                        default:
                            continue;
                    }
                    IMouseDataResult currentResult = kvp.Value;
                    double totalCount = currentResult.Dummy;
                    Type type = kvp.Key.Item1;
                    int tgCounter;
                    if (type.Name.Contains("Non"))
                    {
                        tgCounter = 0;
                    }
                    else
                    {
                        tgCounter = 1;
                    }

                    //finalResults[counter, 0] = kvp.Key.Item1.Name;
                    //finalResults[counter, 1] = kvp.Key.Item2;

                    if (currentResult.CentroidsTest == null || !currentResult.CentroidsTest.Any())
                    {
                        finalResults[row, 1 + tgCounter] = 0;
                    }
                    else
                    {
                        finalResults[row, 1 + tgCounter] = currentResult.CentroidsTest.Average();
                    }

                    finalResults[row, 3 + tgCounter] = currentResult.Duration;
                    finalResults[row, 5 + tgCounter] = currentResult.DistanceTravelled;
                    finalResults[row, 7 + tgCounter] = currentResult.MaxSpeed;
                    finalResults[row, 9 + tgCounter] = currentResult.MaxAngularVelocty;
                    finalResults[row, 11 + tgCounter] = currentResult.AverageVelocity / totalCount;
                    finalResults[row, 13 + tgCounter] = currentResult.AverageAngularVelocity / totalCount;
                    finalResults[row, 15 + tgCounter] = currentResult.PelvicArea / totalCount;
                    finalResults[row, 17 + tgCounter] = currentResult.PelvicArea2 / totalCount;
                    finalResults[row, 19 + tgCounter] = currentResult.PelvicArea3 / totalCount;
                    finalResults[row, 21 + tgCounter] = currentResult.PelvicArea4 / totalCount;
                    finalResults[row, 23 + tgCounter] = currentResult.AverageCentroidVelocity / totalCount;
                    finalResults[row, 25 + tgCounter] = currentResult.MaxCentroidSpeed;

                    //counter++;
                }

                string fileLocation = FileBrowser.SaveFile("Excel|*.xlsx");

                if (string.IsNullOrWhiteSpace(fileLocation))
                {
                    return;
                }

                ExcelService.WriteData(finalResults, fileLocation);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
