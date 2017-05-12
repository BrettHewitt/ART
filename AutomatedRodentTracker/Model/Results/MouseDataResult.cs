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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;
using AutomatedRodentTracker.Extensions;
using AutomatedRodentTracker.Model;
using AutomatedRodentTracker.Model.Behaviours;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.Model.Results.Behaviour.Movement;
using AutomatedRodentTracker.Model.Results.Behaviour.Rotation;
using AutomatedRodentTracker.ModelInterface.Behaviours;
using AutomatedRodentTracker.ModelInterface.Boundries;
using AutomatedRodentTracker.ModelInterface.Datasets;
using AutomatedRodentTracker.ModelInterface.Datasets.Types;
using AutomatedRodentTracker.ModelInterface.Results;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.Movement;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.Rotation;
using AutomatedRodentTracker.ModelInterface.Smoothing;

namespace AutomatedRodentTracker.Model.Results
{
    internal class MouseDataResult : ModelObjectBase, IMouseDataResult
    {
        private int m_Age;
        public int Age
        {
            get
            {
                return m_Age;
            }
            set
            {
                if (Equals(m_Age, value))
                {
                    return;
                }

                m_Age = value;

                MarkAsDirty();
            }
        }

        private ITypeBase m_Type;
        public ITypeBase Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                if (Equals(m_Type, value))
                {
                    return;
                }

                m_Type = value;

                MarkAsDirty();
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

                MarkAsDirty();
            }
        }

        private List<double> m_CentroidsTest;
        public List<double> CentroidsTest
        {
            get
            {
                return m_CentroidsTest;
            }
            set
            {
                if (Equals(m_CentroidsTest, value))
                {
                    return;
                }

                m_CentroidsTest = value;

                MarkAsDirty();
            }
        }


        private double m_PelvicArea;
        public double PelvicArea
        {
            get
            {
                return m_PelvicArea;
            }
            set
            {
                if (Equals(m_PelvicArea, value))
                {
                    return;
                }

                m_PelvicArea = value;

                MarkAsDirty();
            }
        }

        private double m_PelvicArea2;
        public double PelvicArea2
        {
            get
            {
                return m_PelvicArea2;
            }
            set
            {
                if (Equals(m_PelvicArea2, value))
                {
                    return;
                }

                m_PelvicArea2 = value;

                MarkAsDirty();
            }
        }

        private double m_PelvicArea3;
        public double PelvicArea3
        {
            get
            {
                return m_PelvicArea3;
            }
            set
            {
                if (Equals(m_PelvicArea3, value))
                {
                    return;
                }

                m_PelvicArea3 = value;

                MarkAsDirty();
            }
        }

        private double m_PelvicArea4;
        public double PelvicArea4
        {
            get
            {
                return m_PelvicArea4;
            }
            set
            {
                if (Equals(m_PelvicArea4, value))
                {
                    return;
                }

                m_PelvicArea4 = value;

                MarkAsDirty();
            }
        }

        private double m_Dummy;
        public double Dummy
        {
            get
            {
                return m_Dummy;
            }
            set
            {
                if (Equals(m_Dummy, value))
                {
                    return;
                }

                m_Dummy = value;

                MarkAsDirty();
            }
        }

        private double m_Dummy2;
        public double Dummy2
        {
            get
            {
                return m_Dummy2;
            }
            set
            {
                if (Equals(m_Dummy2, value))
                {
                    return;
                }

                m_Dummy2 = value;

                MarkAsDirty();
            }
        }

        private double m_Dummy3;
        public double Dummy3
        {
            get
            {
                return m_Dummy3;
            }
            set
            {
                if (Equals(m_Dummy3, value))
                {
                    return;
                }

                m_Dummy3 = value;

                MarkAsDirty();
            }
        }

        private double m_Dummy4;
        public double Dummy4
        {
            get
            {
                return m_Dummy4;
            }
            set
            {
                if (Equals(m_Dummy4, value))
                {
                    return;
                }

                m_Dummy4 = value;

                MarkAsDirty();
            }
        }

        private double m_Dummy5;
        public double Dummy5
        {
            get
            {
                return m_Dummy5;
            }
            set
            {
                if (Equals(m_Dummy5, value))
                {
                    return;
                }

                m_Dummy5 = value;

                MarkAsDirty();
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

                MarkAsDirty();
            }
        }

        private double m_CentroidDistanceTravelled;
        public double CentroidDistanceTravelled
        {
            get
            {
                return m_CentroidDistanceTravelled;
            }
            set
            {
                if (Equals(m_CentroidDistanceTravelled, value))
                {
                    return;
                }

                m_CentroidDistanceTravelled = value;

                MarkAsDirty();
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

                MarkAsDirty();
            }
        }

        private double m_HeadPointDuration;
        public double HeadPointDuration
        {
            get
            {
                return m_HeadPointDuration;
            }
            set
            {
                if (Equals(m_HeadPointDuration, value))
                {
                    return;
                }

                m_HeadPointDuration = value;

                MarkAsDirty();
            }
        }

        private double m_CentroidDuration;
        public double CentroidDuration
        {
            get
            {
                return m_CentroidDuration;
            }
            set
            {
                if (Equals(m_CentroidDuration, value))
                {
                    return;
                }

                m_CentroidDuration = value;

                MarkAsDirty();
            }
        }


        private double m_MaxSpeed;
        public double MaxSpeed
        {
            get
            {
                return m_MaxSpeed;
            }
            set
            {
                if (Equals(m_MaxSpeed, value))
                {
                    return;
                }

                m_MaxSpeed = value;

                MarkAsDirty();
            }
        }

        private double m_MaxAngularVelocty;
        public double MaxAngularVelocty
        {
            get
            {
                return m_MaxAngularVelocty;
            }
            set
            {
                if (Equals(m_MaxAngularVelocty, value))
                {
                    return;
                }

                m_MaxAngularVelocty = value;

                MarkAsDirty();
            }
        }

        private double m_AverageWaist;
        public double AverageWaist
        {
            get
            {
                return m_AverageWaist;
            }
            set
            {
                if (Equals(m_AverageWaist, value))
                {
                    return;
                }

                m_AverageWaist = value;

                MarkAsDirty();
            }
        }

        private string m_Name;
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                if (Equals(m_Name, value))
                {
                    return;
                }

                m_Name = value;

                MarkAsDirty();
            }
        }

        private SingleFileResult m_VideoOutcome;
        public SingleFileResult VideoOutcome
        {
            get
            {
                return m_VideoOutcome;
            }
            set
            {
                if (Equals(m_VideoOutcome, value))
                {
                    return;
                }

                m_VideoOutcome = value;

                MarkAsDirty();
            }
        }

        private Dictionary<int, ISingleFrameResult> m_Results;
        public Dictionary<int, ISingleFrameResult> Results
        {
            get
            {
                return m_Results;
            }
            set
            {
                if (ReferenceEquals(m_Results, value))
                {
                    return;
                }

                m_Results = value;

                MarkAsDirty();
            }
        }

        private int m_StartFrame;
        public int StartFrame
        {
            get
            {
                return m_StartFrame;
            }
            set
            {
                if (Equals(m_StartFrame, value))
                {
                    return;
                }

                m_StartFrame = value;

                MarkAsDirty();
            }
        }

        private int m_EndFrame;
        public int EndFrame
        {
            get
            {
                return m_EndFrame;
            }
            set
            {
                if (Equals(m_EndFrame, value))
                {
                    return;
                }

                m_EndFrame = value;

                MarkAsDirty();
            }
        }

        private double m_AverageVelocity;
        public double AverageVelocity
        {
            get
            {
                return m_AverageVelocity;
            }
            set
            {
                if (Equals(m_AverageVelocity, value))
                {
                    return;
                }

                m_AverageVelocity = value;

                MarkAsDirty();
            }
        }

        private double m_AverageCentroidVelocity;
        public double AverageCentroidVelocity
        {
            get
            {
                return m_AverageCentroidVelocity;
            }
            set
            {
                if (Equals(m_AverageCentroidVelocity, value))
                {
                    return;
                }

                m_AverageCentroidVelocity = value;

                MarkAsDirty();
            }
        }


        private double m_AverageAngularVelocity;
        public double AverageAngularVelocity
        {
            get
            {
                return m_AverageAngularVelocity;
            }
            set
            {
                if (Equals(m_AverageAngularVelocity, value))
                {
                    return;
                }

                m_AverageAngularVelocity = value;

                MarkAsDirty();
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

                MarkAsDirty();
            }
        }

        private PointF[] m_SmoothedMotionTrack;
        public PointF[] SmoothedMotionTrack
        {
            get
            {
                return m_SmoothedMotionTrack;
            }
            set
            {
                if (ReferenceEquals(m_SmoothedMotionTrack, value))
                {
                    return;
                }

                m_SmoothedMotionTrack = value;

                MarkAsDirty();
            }
        }

        private PointF[] m_CentroidMotionTrack;
        public PointF[] CentroidMotionTrack
        {
            get
            {
                return m_CentroidMotionTrack;
            }
            set
            {
                if (Equals(m_CentroidMotionTrack, value))
                {
                    return;
                }

                m_CentroidMotionTrack = value;

                MarkAsDirty();
            }
        }


        private Vector[] m_OrientationTrack;
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

                MarkAsDirty();
            }
        }

        private IBoundaryBase[] m_Boundaries;
        public IBoundaryBase[] Boundaries
        {
            get
            {
                return m_Boundaries;
            }
            set
            {
                if (ReferenceEquals(m_Boundaries, value))
                {
                    return;
                }

                m_Boundaries = value;

                MarkAsDirty();
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

                MarkAsDirty();
            }
        }

        private int m_ThresholdValue;
        public int ThresholdValue
        {
            get
            {
                return m_ThresholdValue;
            }
            set
            {
                if (Equals(m_ThresholdValue))
                {
                    return;
                }

                m_ThresholdValue = value;

                MarkAsDirty();
            }
        }

        private int m_ThresholdValue2;
        public int ThresholdValue2
        {
            get
            {
                return m_ThresholdValue2;
            }
            set
            {
                if (Equals(m_ThresholdValue2))
                {
                    return;
                }

                m_ThresholdValue2 = value;

                MarkAsDirty();
            }
        }

        private double m_MinInteractionDistance = 15;
        public double MinInteractionDistance
        {
            get
            {
                return m_MinInteractionDistance;
            }
            set
            {
                if (Equals(m_MinInteractionDistance, value))
                {
                    return;
                }

                m_MinInteractionDistance = value;

                MarkAsDirty();
            }
        }

        private Dictionary<IBoundaryBase, IBehaviourHolder[]> m_InteractingBoundries;
        public Dictionary<IBoundaryBase, IBehaviourHolder[]> InteractingBoundries
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

                MarkAsDirty();
            }
        }

        private IBehaviourHolder[] m_Events;
        public IBehaviourHolder[] Events
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

                MarkAsDirty();
            }
        }

        private string m_MainBehaviour;
        public string MainBehaviour
        {
            get
            {
                return m_MainBehaviour;
            }
            set
            {
                if (Equals(m_MainBehaviour, value))
                {
                    return;
                }

                m_MainBehaviour = value;

                MarkAsDirty();
            }
        }

        private string m_Message;
        public string Message
        {
            get
            {
                return m_Message;
            }
            set
            {
                if (Equals(m_Message, value))
                {
                    return;
                }

                m_Message = value;

                MarkAsDirty();
            }
        }

        private bool m_SmoothMotion = false;
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

                //ITrackSmoothing smoothing = ModelResolver.Resolve<ITrackSmoothing>();
                //DistanceTravelled = smoothing.GetTrackLength(SmoothMotion ? SmoothedMotionTrack : MotionTrack);

                MarkAsDirty();
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

                MarkAsDirty();
            }
        }

        private double m_SmoothFactor;
        public double SmoothFactor
        {
            get
            {
                return m_SmoothFactor;
            }
            set
            {
                if (Equals(m_SmoothFactor, value))
                {
                    return;
                }

                m_SmoothFactor = value;

                MarkAsDirty();
            }
        }

        private double FrameCount
        {
            get
            {
                return Results.Count;
            }
        }

        private IMovementBehaviour[] m_MovementBehaviours;
        public IMovementBehaviour[] MovementBehaviours   
        {
            get
            {
                return m_MovementBehaviours;
            }
            set
            {
                if (Equals(m_MovementBehaviours, value))
                {
                    return;
                }

                m_MovementBehaviours = value;

                MarkAsDirty();
            }
        }

        private IRotationBehaviour[] m_RotationBehaviours;
        public IRotationBehaviour[] RotationBehaviours
        {
            get
            {
                return m_RotationBehaviours;
            }
            set
            {
                if (Equals(m_RotationBehaviours, value))
                {
                    return;
                }

                m_RotationBehaviours = value;

                MarkAsDirty();
            }
        }

        private double m_MaxCentroidSpeed;
        public double MaxCentroidSpeed
        {
            get
            {
                return m_MaxCentroidSpeed;
            }
            set
            {
                if (Equals(m_MaxCentroidSpeed, value))
                {
                    return;
                }

                m_MaxCentroidSpeed = value;

                MarkAsDirty();
            }
        }

        private double m_UnitsToMilimeters = 1;
        public double UnitsToMilimeters
        {
            get
            {
                return m_UnitsToMilimeters;
            }
            set
            {
                if (Equals(m_UnitsToMilimeters, value))
                {
                    return;
                }

                m_UnitsToMilimeters = value;

                MarkAsDirty();
            }
        }

        private Rectangle m_ROI;
        public Rectangle ROI
        {
            get
            {
                return m_ROI;
            }
            set
            {
                if (Equals(m_ROI, value))
                {
                    return;
                }

                m_ROI = value;

                MarkAsDirty();
            }
        }


        public void ResetFrames()
        {
            StartFrame = Results.Where(x => x.Value.HeadPoints != null).Select(x => x.Key).Min();
            EndFrame = Results.Where(x => x.Value.HeadPoints != null).Select(x => x.Key).Max();
        }

        private string File
        {
            get;
            set;
        }

        public void GenerateResults(string file)
        {
            File = file;
            GenerateResults();
        }

        private bool m_ResultsGenerated = false;
        public bool ResultsGenerated
        {
            get
            {
                return m_ResultsGenerated;
            }
            set
            {
                if (Equals(m_ResultsGenerated, value))
                {
                    return;
                }

                m_ResultsGenerated = value;

                MarkAsDirty();
            }
        }


        public void GenerateResults()
        {
            if (Results == null || Results.Count == 0)
            {
                return;
            }

            ResultsGenerated = true;
            SmoothMotion = true;
            GenerateMotionTrack();

            //int frameCount = Results.Count;
            int startCount = Results.Where(x => x.Value.HeadPoints != null).Select(x => x.Key).Min();

            int targetStart = startCount > StartFrame ? startCount : StartFrame;
            
            ISingleFrameResult previousFrame = Results[targetStart];
            UpdateSingleFrameResult(previousFrame);

            MaxSpeed = -1;
            MaxCentroidSpeed = -1;
            MaxAngularVelocty = -1;
            AverageAngularVelocity = 0;
            AverageVelocity = 0;
            AverageCentroidVelocity = 0;
            DistanceTravelled = 0;
            CentroidDistanceTravelled = 0;
            CentroidSize = 0;
            PelvicArea = 0;
            PelvicArea2 = 0;
            PelvicArea3 = 0;
            PelvicArea4 = 0;

            //List<double> centroidWidths = new List<double>();
            
            int nextStartCount = targetStart + 1;
            int frameCounter = 0;
            int targetEnd = EndFrame + 1;

            int headPointCounter = 0;
            int centroidCounter = 0;

            double dist = 0;
            double cDist = 0;

            Duration = EndFrame - StartFrame + 1;

            for (int frameNumber = targetStart; frameNumber <= EndFrame; frameNumber++)
            {
                ISingleFrameResult frameResult = Results[frameNumber];

                if (frameResult == null || frameResult.HeadPoints == null)
                {
                    break;
                }

                UpdateSingleFrameResult(frameResult, previousFrame, FrameRate);

                if (frameResult.HeadPoints != null)
                {
                    if (frameResult.Velocity > MaxSpeed)
                    {
                        MaxSpeed = frameResult.Velocity;
                    }

                    if (frameResult.AngularVelocity > MaxAngularVelocty)
                    {
                        MaxAngularVelocty = frameResult.AngularVelocity;
                    }

                    AverageVelocity += frameResult.Velocity;
                    AverageAngularVelocity += frameResult.AngularVelocity;
                    dist += frameResult.Distance;
                    headPointCounter++;
                }
                

                if (frameResult.CentroidVelocity > MaxCentroidSpeed)
                {
                    MaxCentroidSpeed = frameResult.CentroidVelocity;
                }
                
                AverageCentroidVelocity += frameResult.CentroidVelocity;
                cDist += frameResult.CentroidDistance;
                centroidCounter++;
                frameCounter++;
                previousFrame = frameResult;
            }

            //ITrackSmoothing smoothing = ModelResolver.Resolve<ITrackSmoothing>();
            //DistanceTravelled = smoothing.GetTrackLength(SmoothMotion ? SmoothedMotionTrack : MotionTrack);
            Duration = frameCounter;

            AverageVelocity /= headPointCounter;
            AverageAngularVelocity /= headPointCounter;
            AverageCentroidVelocity /= centroidCounter;

            DistanceTravelled = dist;
            CentroidDistanceTravelled = cDist;

            CentroidDuration = centroidCounter;
            HeadPointDuration = headPointCounter;
            
            IBehaviourSpeedDefinitions speedDef = ModelResolver.Resolve<IBehaviourSpeedDefinitions>();

            List<IMovementBehaviour> movements = new List<IMovementBehaviour>();
            List<IRotationBehaviour> rotations = new List<IRotationBehaviour>();
            IMovementBehaviour previousMovement = null;
            IRotationBehaviour previousRotation = null;
            
            //Generate behaviours

            //List<double> v5 = new List<double>();
            List<double> v = new List<double>();

            //List<double> c5 = new List<double>();
            List<double> c = new List<double>();

            //List<double> o5 = new List<double>();
            List<double> o = new List<double>();

            const int frameDelta = 40;

            for (int i = 0; i < Results.Count; i++)
            {
                //Test1(i, o5, v5, c5, 5, false);
                Test1(i, o, v, c, frameDelta, true);
            }
            
            int counter = 0;
            double maxSpeed = -1;
            bool counterHit = false;
            foreach (var speed in c)
            {
                if (counter >= EndFrame)
                {
                    if (movements.Any())
                    {
                        movements.Last().EndFrame = counter;
                    }
                    counterHit = true;
                    break;
                }

                if (speed == 0)
                {
                    previousMovement = null;
                    counter++;
                    continue;
                }

                if (speed > maxSpeed)
                {
                    maxSpeed = speed;
                }

                IMovementBehaviour currentMovement = speedDef.GetMovementBehaviour(speed);
                if (!currentMovement.Equals(previousMovement))
                {
                    if (movements.Any())
                    {
                        movements.Last().EndFrame = counter + frameDelta - 1;
                    }

                    currentMovement.StartFrame = counter + frameDelta;
                    movements.Add(currentMovement);
                }

                previousMovement = currentMovement;
                counter++;
            }

            //MaxSpeed = maxSpeed;

            if (!counterHit)
            {
                if (movements.Any())
                {
                    movements.Last().EndFrame = counter + frameDelta - 1;
                }
            }

            counter = 0;
            double maxAngSpeed = -1;
            counterHit = false;
            foreach (var angle in o)
            {
                if (counter >= EndFrame)
                {
                    if (rotations.Any())
                    {
                        rotations.Last().EndFrame = counter;
                    }
                    counterHit = true;
                    break;
                }

                if (angle == 0)
                {
                    previousRotation = null;
                    counter++;
                    continue;
                }

                if (angle > maxAngSpeed)
                {
                    maxAngSpeed = angle;
                }

                IRotationBehaviour currentRotation = speedDef.GetRotationBehaviour(angle);
                if (!currentRotation.Equals(previousRotation))
                {
                    if (rotations.Any())
                    {
                        rotations.Last().EndFrame = counter + frameDelta - 1;
                    }

                    currentRotation.StartFrame = counter + frameDelta;
                    rotations.Add(currentRotation);
                }

                previousRotation = currentRotation;
                counter++;
            }

            //MaxAngularVelocty = maxAngSpeed;

            if (!counterHit)
            {
                if (rotations.Any())
                {
                    rotations.Last().EndFrame = counter + frameDelta - 1;
                }
            }

            const int minDelta = 5;
            int mCount = movements.Count - 1;
            for (int i = mCount; i >= 0; i--)
            {
                var m = movements[i];
                if (m.EndFrame - m.StartFrame < minDelta)
                {
                    movements.RemoveAt(i);
                }
            }

            int rCount = rotations.Count - 1;
            for (int i = rCount; i >= 0; i--)
            {
                var r = rotations[i];
                if (r.EndFrame - r.StartFrame < minDelta)
                {
                    rotations.RemoveAt(i);
                }
            }

            MovementBehaviours = movements.ToArray();
            RotationBehaviours = rotations.ToArray();
            //Console.WriteLine("");
            //foreach (var m in MovementBehaviours)
            //{
            //    Console.WriteLine(string.Format("{0}-{1}: {2}", m.StartFrame, m.EndFrame, m));
            //}

            //Console.WriteLine("");
            //foreach (var m in RotationBehaviours)
            //{
            //    Console.WriteLine(string.Format("{0}-{1}: {2}", m.StartFrame, m.EndFrame, m));
            //}
            //if (AverageVelocity < 4)
            //{
            //    MainBehaviour = "Not moving";
            //}
            //else if (AverageVelocity < 8)
            //{
            //    MainBehaviour = "Moving";
            //}
            //else
            //{
            //    MainBehaviour = "Running";
            //}

            //Console.WriteLine(Name + " " + AverageVelocity);
        }

        private void Test1(int i, List<double> o2, List<double> v1, List<double> c1, int frameDelta, bool useAbs)
        {
            ISingleFrameResult currentFrame = Results[i];
            PointF[] headPoints = currentFrame.HeadPoints;

            double deltaTime = frameDelta/FrameRate;

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

                        double angle = Vector.AngleBetween(orientation, prevOrientation);
                        if (useAbs)
                        {
                            angle = Math.Abs(angle);
                        }
                        angle = angle/deltaTime;
                        o2.Add(angle);

                        double vDist = headPoint.Distance(prevHeadPoint);
                        vDist *= UnitsToMilimeters;
                        vDist = vDist/deltaTime;
                        v1.Add(vDist);

                        double dist = centroidPoint.Distance(prevCentroidPoint);
                        dist *= UnitsToMilimeters;
                        dist = dist/deltaTime;
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

        private void UpdateSingleFrameResult(ISingleFrameResult singleFrameResult)
        {
            try
            {
                PointF midPoint = singleFrameResult.HeadPoints[1].MidPoint(singleFrameResult.HeadPoints[3]);
                PointF headPoint = singleFrameResult.HeadPoint;
                //singleFrameResult.HeadPoint = headPoint;
                singleFrameResult.Orientation = new Vector(headPoint.X - midPoint.X, headPoint.Y - midPoint.Y);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        private void UpdateSingleFrameResult(ISingleFrameResult singleFrame, ISingleFrameResult previousFrame, double frameRate)
        {
            UpdateSingleFrameResult(singleFrame);

            PointF headPoint, previousHeadPoint, centroidPoint, previousCentroidPoint;

            if (SmoothMotion)
            {
                headPoint = singleFrame.SmoothedHeadPoint;
                previousHeadPoint = previousFrame.SmoothedHeadPoint;
            }
            else
            {
                headPoint = singleFrame.HeadPoint;
                previousHeadPoint = previousFrame.HeadPoint;
            }

            centroidPoint = singleFrame.Centroid;
            previousCentroidPoint = previousFrame.Centroid;

            double dist = headPoint.Distance(previousHeadPoint);
            double centroidDist = centroidPoint.Distance(previousCentroidPoint);

            dist *= UnitsToMilimeters;
            centroidDist *= UnitsToMilimeters;

            singleFrame.Distance = dist;
            singleFrame.Velocity = dist * frameRate;
            singleFrame.CentroidVelocity = centroidDist*frameRate;
            singleFrame.CentroidDistance = centroidDist;
            Vector up = new Vector(0, 1);
            double angle1 = Vector.AngleBetween(singleFrame.Orientation, up);
            double angle2 = Vector.AngleBetween(previousFrame.Orientation, up);
            double deltaAngle = Math.Abs(angle1 - angle2);
            singleFrame.AngularVelocity = deltaAngle * frameRate;
        }

        private void GenerateMotionTrack()
        {
            List<PointF> motionTrack = new List<PointF>();
            List<PointF> centroidMotionTrack = new List<PointF>();
            List<Vector> orientationTrack = new List<Vector>();
            int startOffset = 0;
            bool inStart = true;
            for (int i = 0; i < Results.Count; i++)
            {
                PointF[] headPoints = Results[i].HeadPoints;
                if (headPoints != null)
                {
                    PointF midPoint = headPoints[1].MidPoint(headPoints[3]);
                    //Vector up = new Vector(0,1);
                    Vector dir = new Vector(headPoints[2].X - midPoint.X, headPoints[2].Y - midPoint.Y);
                    orientationTrack.Add(dir);

                    motionTrack.Add(headPoints[2]);

                    PointF centroid = Results[i].Centroid;

                    if (!centroid.IsEmpty)
                    {
                        centroidMotionTrack.Add(centroid);
                    }

                    inStart = false;
                }
                else if (inStart)
                {
                    startOffset++;
                }
            }

            UpdateSmoothing(motionTrack, startOffset);
            CentroidMotionTrack = centroidMotionTrack.ToArray();
            OrientationTrack = orientationTrack.ToArray();
            GenerateBehaviouralAnalysis(MotionTrack, startOffset);
        }

        private void UpdateSmoothing(IEnumerable<PointF> motionTrack, int startOffset)
        {
            ITrackSmoothing smoothing = ModelResolver.Resolve<ITrackSmoothing>();
            MotionTrack = motionTrack.ToArray();
            SmoothedMotionTrack = smoothing.SmoothTrack(MotionTrack);
            DistanceTravelled = smoothing.GetTrackLength(SmoothMotion ? SmoothedMotionTrack : MotionTrack);
            
            for (int i = 0; i < MotionTrack.Length; i++)
            {
                Results[i + startOffset].HeadPoint = MotionTrack[i];
                Results[i + startOffset].SmoothedHeadPoint = SmoothedMotionTrack[i];
            }
            //int counter = 0;
            //foreach (PointF point in SmoothedMotionTrack)
            //{
            //    Results[counter + startOffset].SmoothedHeadPoint = point;
            //    counter++;
            //}
        }


        private void GenerateBehaviouralAnalysis(PointF[] motionTrack, int startOffset)
        {
            Dictionary<IBoundaryBase, IBehaviourHolder[]> interactingBoundries = new Dictionary<IBoundaryBase, IBehaviourHolder[]>();
            int trackCount = motionTrack.Length;
            foreach (IBoundaryBase boundary in Boundaries)
            {
                //Manually handle first point
                PointF currentPoint = motionTrack[0];
                double distance = boundary.GetMinimumDistance(currentPoint);
                IBehaviourHolder previousHolder;
                List<IBehaviourHolder> behaviours = new List<IBehaviourHolder>();
                if (distance < MinInteractionDistance)
                {
                    //Interaction
                    previousHolder = ModelResolver.Resolve<IBehaviourHolder>(); //new IBehaviourHolder(boundary, InteractionBehaviour.Started, 0);
                    previousHolder.Boundary = boundary;
                    previousHolder.Interaction = InteractionBehaviour.Started;
                    previousHolder.FrameNumber = startOffset;
                    //interactingBoundries[boundary].Add(previousHolder);
                    behaviours.Add(previousHolder);
                }
                else
                {
                    //No Interaction
                    previousHolder = ModelResolver.Resolve<IBehaviourHolder>();
                    previousHolder.Boundary = boundary;
                    previousHolder.Interaction = InteractionBehaviour.Ended;
                    previousHolder.FrameNumber = startOffset;
                    //interactingBoundries[boundary].Add(previousHolder);
                    behaviours.Add(previousHolder);
                }


                for (int i = 1; i < trackCount; i++)
                {
                    currentPoint = motionTrack[i];
                    distance = boundary.GetMinimumDistance(currentPoint);

                    if (distance < MinInteractionDistance)
                    {
                        //Interaction
                        if (previousHolder.Interaction != InteractionBehaviour.Started)
                        {
                            previousHolder = ModelResolver.Resolve<IBehaviourHolder>();
                            previousHolder.Boundary = boundary;
                            previousHolder.Interaction = InteractionBehaviour.Started;
                            previousHolder.FrameNumber = i + startOffset;
                            //previousHolder = new IBehaviourHolder(boundary, InteractionBehaviour.Started, i + startOffset);
                            behaviours.Add(previousHolder);
                            //interactingBoundries[boundary].Add(previousHolder);
                        }
                    }
                    else
                    {
                        //No Interaction
                        if (previousHolder.Interaction != InteractionBehaviour.Ended)
                        {
                            previousHolder = ModelResolver.Resolve<IBehaviourHolder>();
                            previousHolder.Boundary = boundary;
                            previousHolder.Interaction = InteractionBehaviour.Ended;
                            previousHolder.FrameNumber = i + startOffset;
                            //previousHolder = new IBehaviourHolder(boundary, InteractionBehaviour.Ended, i + startOffset);
                            behaviours.Add(previousHolder);
                            //interactingBoundries[boundary].Add(previousHolder);
                        }
                    }
                }

                interactingBoundries.Add(boundary, behaviours.ToArray());
            }

            InteractingBoundries = interactingBoundries;

            List<IBehaviourHolder> events = new List<IBehaviourHolder>();
            foreach (var boundary in interactingBoundries)
            {
                if (boundary.Key is IOuterBoundary && boundary.Key.Id == 1)
                {
                    continue;
                }

                IBehaviourHolder currentBehaviour = null;
                IBehaviourHolder previousBehaviour = null;
                foreach (IBehaviourHolder behaviour in boundary.Value)
                {
                    if (behaviour.Interaction == InteractionBehaviour.Started)
                    {
                        if (previousBehaviour != null && behaviour.FrameNumber - previousBehaviour.EndFrame < 10)
                        {
                            currentBehaviour = previousBehaviour;
                        }
                        else
                        {
                            currentBehaviour = ModelResolver.Resolve<IBehaviourHolder>();
                            currentBehaviour.Boundary = behaviour.Boundary;
                            currentBehaviour.StartFrame = behaviour.FrameNumber;
                        }
                    }
                    else
                    {
                        if (currentBehaviour == null)
                        {
                            continue;
                        }

                        currentBehaviour.EndFrame = behaviour.FrameNumber;

                        previousBehaviour = currentBehaviour;

                        if (!events.Contains(currentBehaviour))
                        {
                            events.Add(currentBehaviour);
                        }
                    }
                    //events.Add(behaviour);
                }

                if (currentBehaviour != null && !events.Contains(currentBehaviour))
                {
                    currentBehaviour.EndFrame = EndFrame;
                    events.Add(currentBehaviour);
                }
            }



            //IBehaviourHolder[] eventsOrdered = events.OrderBy(x => x.FrameNumber).ToArray();
            //int count = eventsOrdered.Length;

            //if (count > 0)
            //{
            //    for (int i = 1; i < count; i++)
            //    {
            //        IBehaviourHolder previousHolder
            //    }
            //}

            Events = events.OrderBy(x => x.StartFrame).ToArray();
        }

        public PointF[] GetMotionTrack()
        {
            if (!SmoothMotion)
            {
                return MotionTrack;
            }

            return SmoothedMotionTrack;
        }

        public List<Tuple<int, int>> GetFrameNumbersForRunning()
        {
            if (MovementBehaviours == null || !MovementBehaviours.Any())
            {
                return null;
            }

            return MovementBehaviours.OfType<IRunning>().Select(m => new Tuple<int, int>(m.StartFrame, m.EndFrame)).ToList();
        }

        public List<Tuple<int, int>> GetFrameNumbersForMoving()
        {
            if (MovementBehaviours == null || !MovementBehaviours.Any())
            {
                return null;
            }

            return (from m in MovementBehaviours where !(m is IStill) select new Tuple<int, int>(m.StartFrame, m.EndFrame)).ToList();
        }

        public List<Tuple<int, int>> GetFrameNumbersForTurning()
        {
            if (RotationBehaviours == null || !RotationBehaviours.Any())
            {
                return null;
            }

            return (from r in RotationBehaviours where !(r is INoRotation) select new Tuple<int, int>(r.StartFrame, r.EndFrame)).ToList();
        }

        public List<Tuple<int, int>> GetFrameNumbesrForFastTurning()
        {
            if (RotationBehaviours == null || !RotationBehaviours.Any())
            {
                return null;
            }

            return RotationBehaviours.OfType<IFastTurning>().Select(r => new Tuple<int, int>(r.StartFrame, r.EndFrame)).ToList();
        }

        public List<Tuple<int, int>> GetFrameNumbersForStill()
        {
            if (MovementBehaviours == null || !MovementBehaviours.Any())
            {
                return null;
            }

            return (from m in MovementBehaviours where m is IStill select new Tuple<int, int>(m.StartFrame, m.EndFrame)).ToList();
        }

        public List<Tuple<int, int>> GetFrameNumbesrForInteracting()
        {
            if (Events == null || !Events.Any())
            {
                return null;
            }

            return Events.Select(x => new Tuple<int, int>(x.StartFrame, x.EndFrame)).ToList();
        }

        public const double LimitCutOff = 0.2;

        public double GetAverageSpeedForMoving()
        {
            List<Tuple<int, int>> frameNumbers = GetFrameNumbersForMoving();

            if (frameNumbers == null || !frameNumbers.Any())
            {
                return 0;
            }

            List<double> speeds = new List<double>();
            foreach (var tuple in frameNumbers)
            {
                for (int i = tuple.Item1; i <= tuple.Item2; i++)
                {
                    double velocity = Results[i].Velocity;
                    if (velocity > 0)
                    {
                        speeds.Add(velocity);
                    }
                }
            }

            if (speeds.Count > 10)
            {
                return RemoveRangesAndAverage(speeds);
            }
            else
            {
                return 0;
            }
        }

        public double GetAverageCentroidSpeedForMoving()
        {
            List<Tuple<int, int>> frameNumbers = GetFrameNumbersForMoving();

            if (frameNumbers == null || !frameNumbers.Any())
            {
                return 0;
            }

            List<double> speeds = new List<double>();
            foreach (var tuple in frameNumbers)
            {
                for (int i = tuple.Item1; i <= tuple.Item2; i++)
                {
                    double velocity = Results[i].CentroidVelocity;
                    if (velocity > 0)
                    {
                        speeds.Add(velocity);
                    }
                }
            }

            if (speeds.Any())
            {
                return RemoveRangesAndAverage(speeds);
            }
            else
            {
                return 0;
            }
        }

        public double GetAverageSpeedForRunning()
        {
            List<Tuple<int, int>> frameNumbers = GetFrameNumbersForRunning();

            if (frameNumbers == null || !frameNumbers.Any())
            {
                return 0;
            }

            List<double> speeds = new List<double>();
            foreach (var tuple in frameNumbers)
            {
                for (int i = tuple.Item1; i <= tuple.Item2; i++)
                {
                    double velocity = Results[i].Velocity;
                    if (velocity > 0)
                    {
                        speeds.Add(velocity);
                    }
                }
            }

            if (speeds.Any())
            {
                return RemoveRangesAndAverage(speeds);
            }
            else
            {
                return 0;
            }
        }

        public double GetAverageAngularSpeedForTurning()
        {
            List<Tuple<int, int>> frameNumbers = GetFrameNumbersForTurning();

            if (frameNumbers == null || !frameNumbers.Any())
            {
                return 0;
            }

            List<double> speeds = new List<double>();
            foreach (var tuple in frameNumbers)
            {
                for (int i = tuple.Item1; i <= tuple.Item2; i++)
                {
                    double velocity = Results[i].AngularVelocity;
                    if (velocity > 0)
                    {
                        speeds.Add(velocity);
                    }
                }
            }

            if (speeds.Any())
            {
                return RemoveRangesAndAverage(speeds);
            }
            else
            {
                return 0;
            }
        }

        public double GetCentroidWidthForRunning()
        {
            List<Tuple<int, int>> frameNumbers = GetFrameNumbersForRunning();

            if (frameNumbers == null || !frameNumbers.Any())
            {
                return 0;
            }

            List<double> centroids = new List<double>();
            foreach (var tuple in frameNumbers)
            {
                for (int i = tuple.Item1; i <= tuple.Item2; i++)
                {
                    double width = Results[i].CentroidSize;
                    if (width > 30 && width < 250)
                    {
                        centroids.Add(width);
                    }
                }
            }

            //if (centroids.Count > 10)
            //{
            //    List<double> finalWidths = new List<double>();
            //    foreach (double width in centroids)
            //    {
            //        if (width < 30)
            //        {
            //            continue;
            //        }

            //        if (width > 200)
            //        {
            //            continue;
            //        }

            //        finalWidths.Add(width);
            //    }

            //    if (finalWidths.Count < 10)
            //    {
            //        return 0;
            //    }

            //    finalWidths.Sort();

            //    int finalCount = finalWidths.Count;

            //    //20%
            //    int startRange = (int)(finalCount * 0.2d);

            //    finalWidths.RemoveRange(finalWidths.Count - startRange, startRange);
            //    finalWidths.RemoveRange(0, startRange);

            //    return finalWidths.Average();
            //}

            //return 0;

            if (centroids.Count > 50)
            {
                return RemoveRangesAndAverage(centroids) * UnitsToMilimeters;
            }
            else
            {
                return 0;
            }
        }

        public double GetCentroidWidthForPelvic1()
        {
            List<Tuple<int, int>> frameNumbers = GetFrameNumbersForRunning();

            if (frameNumbers == null || !frameNumbers.Any())
            {
                return 0;
            }

            List<double> pelvic = new List<double>();
            foreach (var tuple in frameNumbers)
            {
                for (int i = tuple.Item1; i <= tuple.Item2; i++)
                {
                    double area = Results[i].PelvicArea;
                    if (area > 0)
                    {
                        pelvic.Add(area);
                    }
                }
            }

            if (pelvic.Any())
            {
                return RemoveRangesAndAverage(pelvic);
            }
            else
            {
                return 0;
            }
        }

        private double RemoveRangesAndAverage(List<double> list)
        {
            if (list.Count < 50)
            {
                return 0;
            }

            list.Sort();

            int count = list.Count;
            int bottomLimit = (int)(count * LimitCutOff);
            int topLimit = (int)((1 - LimitCutOff) * count);

            list.RemoveRange(topLimit, bottomLimit);
            list.RemoveRange(0, bottomLimit);

            return list.Average();
        }

        public double GetCentroidWidthForPelvic2()
        {
            List<Tuple<int, int>> frameNumbers = GetFrameNumbersForRunning();

            if (frameNumbers == null || !frameNumbers.Any())
            {
                return 0;
            }

            List<double> pelvic = new List<double>();
            foreach (var tuple in frameNumbers)
            {
                for (int i = tuple.Item1; i <= tuple.Item2; i++)
                {
                    double area = Results[i].PelvicArea2;
                    if (area > 0)
                    {
                        pelvic.Add(area);
                    }
                }
            }

            if (pelvic.Any())
            {
                return RemoveRangesAndAverage(pelvic);
            }
            else
            {
                return 0;
            }
        }

        public double GetCentroidWidthForPelvic3()
        {
            List<Tuple<int, int>> frameNumbers = GetFrameNumbersForRunning();

            if (frameNumbers == null || !frameNumbers.Any())
            {
                return 0;
            }

            List<double> pelvic = new List<double>();
            foreach (var tuple in frameNumbers)
            {
                for (int i = tuple.Item1; i <= tuple.Item2; i++)
                {
                    double area = Results[i].PelvicArea3;
                    if (area > 0)
                    {
                        pelvic.Add(area);
                    }
                }
            }

            if (pelvic.Any())
            {
                return RemoveRangesAndAverage(pelvic);
            }
            else
            {
                return 0;
            }
        }

        public double GetCentroidWidthForPelvic4()
        {
            List<Tuple<int, int>> frameNumbers = GetFrameNumbersForRunning();

            if (frameNumbers == null || !frameNumbers.Any())
            {
                return 0;
            }

            List<double> pelvic = new List<double>();
            foreach (var tuple in frameNumbers)
            {
                for (int i = tuple.Item1; i <= tuple.Item2; i++)
                {
                    double area = Results[i].PelvicArea4;
                    if (area > 0)
                    {
                        pelvic.Add(area);
                    }
                }
            }

            if (pelvic.Any())
            {
                return RemoveRangesAndAverage(pelvic);
            }
            else
            {
                return 0;
            }
        }

        public object[,] GetResults()
        {
            object[,] data = new object[Results.Count + 6, 13];

            data[0, 0] = "Frame";
            data[0, 1] = "X";
            data[0, 2] = "Y";
            data[0, 3] = "Centroid X: ";
            data[0, 4] = "Centroid Y: ";

            for (int j = 1; j <= Results.Count; j++)
            {
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

                PointF[] headPoints = Results[j - 1].HeadPoints;

                if (headPoints == null)
                {
                    data[j, 0] = j - 1;
                    data[j, 1] = "null";
                    data[j, 2] = "null";
                    continue;
                }

                PointF point = Results[j - 1].HeadPoints[2];
                data[j, 0] = j - 1;
                data[j, 1] = point.X;
                data[j, 2] = point.Y;
            }

            data[0, 6] = "Distance Travelled: ";
            data[0, 7] = DistanceTravelled;
            data[1, 6] = "Centroid Distance Travelled: ";
            data[1, 7] = CentroidDistanceTravelled;
            data[2, 6] = "Average Speed: ";
            data[2, 7] = AverageVelocity;
            data[3, 6] = "Max Speed: ";
            data[3, 7] = MaxSpeed;
            data[4, 6] = "Average Centroid Velocity: ";
            data[4, 7] = AverageCentroidVelocity;
            data[5, 6] = "Max Centroid Velocity: ";
            data[5, 7] = MaxCentroidSpeed;
            data[6, 6] = "Average Angular Velocity: ";
            data[6, 7] = AverageAngularVelocity;
            data[7, 6] = "Max Angular Velocity: ";
            data[7, 7] = MaxAngularVelocty;
            data[8, 6] = "Distance per Frame: ";
            data[8, 7] = DistanceTravelled / HeadPointDuration;
            data[9, 6] = "Centroid Distance per Frame: ";
            data[9, 7] = CentroidDistanceTravelled / CentroidDuration;
            data[10, 6] = "Start Frame: ";
            data[10, 7] = StartFrame;
            data[11, 6] = "End Frame: ";
            data[11, 7] = EndFrame;
            data[12, 6] = "Duration: ";
            data[12, 7] = Duration;

            return data;
        }
    }

    public enum SingleFileResult
    {
        [XmlEnum(Name="Ok")]
        Ok = 0,
        [XmlEnum(Name="LowDetectionRate")]
        LowDetectionRate = 1,
        [XmlEnum(Name="FrameCountTooLow")]
        FrameCountTooLow = 2,
        [XmlEnum(Name="Error")]
        Error = 3,
    }
}
