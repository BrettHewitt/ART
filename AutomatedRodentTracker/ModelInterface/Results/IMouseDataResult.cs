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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AutomatedRodentTracker.Model.Results;
using AutomatedRodentTracker.ModelInterface;
using AutomatedRodentTracker.ModelInterface.Behaviours;
using AutomatedRodentTracker.ModelInterface.Boundries;
using AutomatedRodentTracker.ModelInterface.Datasets.Types;

namespace AutomatedRodentTracker.ModelInterface.Results
{
    public interface IMouseDataResult : IModelObjectBase
    {
        string Name
        {
            get;
            set;
        }

        int Age
        {
            get;
            set;
        }
        
        ITypeBase Type
        {
            get;
            set;
        }
        
        double CentroidSize
        {
            get;
            set;
        }
        
        double DistanceTravelled
        {
            get;
            set;
        }
        
        double Duration
        {
            get;
            set;
        }
        
        double MaxSpeed
        {
            get;
            set;
        }

        double MaxCentroidSpeed
        {
            get;
            set;
        }

        double MaxAngularVelocty
        {
            get;
            set;
        }

        double AverageWaist
        {
            get;
            set;
        }

        double PelvicArea
        {
            get;
            set;
        }

        double PelvicArea2
        {
            get;
            set;
        }

        double PelvicArea3
        {
            get;
            set;
        }

        double PelvicArea4
        {
            get;
            set;
        }

        double Dummy
        {
            get;
            set;
        }

        double Dummy2
        {
            get;
            set;
        }

        double Dummy3
        {
            get;
            set;
        }

        double Dummy4
        {
            get;
            set;
        }

        double Dummy5
        {
            get;
            set;
        }

        Dictionary<int, ISingleFrameResult> Results
        {
            get;
            set;
        }

        int StartFrame
        {
            get;
            set;
        }

        int EndFrame
        {
            get;
            set;
        }

        SingleFileResult VideoOutcome
        {
            get;
            set;
        }

        double AverageVelocity
        {
            get;
            set;
        }

        double AverageCentroidVelocity
        {
            get;
            set;
        }

        double AverageAngularVelocity
        {
            get;
            set;
        }

        PointF[] MotionTrack
        {
            get;
            set;
        }

        PointF[] SmoothedMotionTrack
        {
            get;
            set;
        }
        
        Vector[] OrientationTrack
        {
            get;
            set;
        }
        
        IBoundaryBase[] Boundaries
        {
            get;
            set;
        }

        double GapDistance
        {
            get;
            set;
        }

        int ThresholdValue
        {
            get;
            set;
        }

        int ThresholdValue2
        {
            get;
            set;
        }
        
        double MinInteractionDistance
        {
            get;
            set;
        }
        
        Dictionary<IBoundaryBase, IBehaviourHolder[]> InteractingBoundries
        {
            get;
            set;
        }
        
        IBehaviourHolder[] Events
        {
            get;
            set;
        }

        string MainBehaviour
        {
            get;
            set;
        }

        string Message
        {
            get;
            set;
        }

        bool SmoothMotion
        {
            get;
            set;
        }

        double FrameRate
        {
            get;
            set;
        }

        double SmoothFactor
        {
            get;
            set;
        }
        
        double UnitsToMilimeters
        {
            get;
            set;
        }

        Rectangle ROI
        {
            get;
            set;
        }

        void GenerateResults();
        void GenerateResults(string file);
        PointF[] GetMotionTrack();
        void ResetFrames();

        List<Tuple<int, int>> GetFrameNumbersForRunning();

        List<Tuple<int, int>> GetFrameNumbersForMoving();

        List<Tuple<int, int>> GetFrameNumbersForTurning();

        List<Tuple<int, int>> GetFrameNumbersForStill();

        List<Tuple<int, int>> GetFrameNumbesrForInteracting();

        double GetAverageSpeedForMoving();

        double GetAverageAngularSpeedForTurning();

        double GetCentroidWidthForRunning();

        double GetCentroidWidthForPelvic1();

        double GetCentroidWidthForPelvic2();

        double GetCentroidWidthForPelvic3();

        double GetCentroidWidthForPelvic4();

        double GetAverageCentroidSpeedForMoving();

        List<double> CentroidsTest
        {
            get;
            set;
        }
    }
}
