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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.Movement;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.Rotation;

namespace AutomatedRodentTracker.Model.Results.Behaviour
{
    internal class BehaviourSpeedDefinitions : ModelObjectBase, IBehaviourSpeedDefinitions
    {
        //Speed defs need to be in mm/sec
        private const double DefaultStillCutOff = 25;
        private const double DefaultWalkingCutOff = 60;

        //Rotation defs need to be in degrees/sec
        private const double DefaultNoRotationCutOff = 200;
        private const double DefaultSlowRotationCutOff = 400;

        private double m_ModifiedStillCutOff;
        private double m_ModifiedWalkingCutOff;
        private double m_ModifiedNoRotationCutOff;
        private double m_ModifiedSlowRotationCutOff;

        public double StillCutOff
        {
            get
            {
                return m_ModifiedStillCutOff;
            }
        }

        public double WalkingCutOff
        {
            get
            {
                return m_ModifiedWalkingCutOff;
            }
        }

        public double NoRotationCutOff
        {
            get
            {
                return m_ModifiedNoRotationCutOff;
            }
        }

        public double SlowRotationCutOff
        {
            get
            {
                return m_ModifiedSlowRotationCutOff;
            }
        }

        public BehaviourSpeedDefinitions()
        {
            m_ModifiedStillCutOff = DefaultStillCutOff;
            m_ModifiedWalkingCutOff = DefaultWalkingCutOff;
            m_ModifiedNoRotationCutOff = DefaultNoRotationCutOff;
            m_ModifiedSlowRotationCutOff = DefaultSlowRotationCutOff;
        }

        /// <summary>
        /// Get the movement model for the velocity
        /// </summary>
        /// <param name="velocity">The vlocity over 1 second</param>
        /// <returns></returns>
        public IMovementBehaviour GetMovementBehaviour(double velocity)
        {
            if (velocity < StillCutOff)
            {
                return ModelResolver.Resolve<IStill>();
            }

            if (velocity < WalkingCutOff)
            {
                return ModelResolver.Resolve<IWalking>();
            }

            return ModelResolver.Resolve<IRunning>();
        }

        /// <summary>
        /// Get the rotational model for the velocity
        /// </summary>
        /// <param name="angularVelocity">The angular velocity over 1 second</param>
        /// <returns></returns>
        public IRotationBehaviour GetRotationBehaviour(double angularVelocity)
        {
            if (angularVelocity < NoRotationCutOff)
            {
                return ModelResolver.Resolve<INoRotation>();
            }

            if (angularVelocity < SlowRotationCutOff)
            {
                return ModelResolver.Resolve<ISlowTurning>();
            }

            //Check if shaking
            return ModelResolver.Resolve<IFastTurning>();
        }

        //public IRotationBehaviour[] GetRotations(Vector[] orientationTrack)
        //{
            
        //}

        /// <summary>
        /// Update the cut offs with a calibration metric
        /// </summary>
        /// <param name="pixelCalibration">Units per pixel</param>
        public void UpdateCutOffs(double pixelCalibration)
        {
            if (pixelCalibration <= 0)
            {
                throw new Exception("Pixel Calibration can not be less than 0");
            }

            m_ModifiedStillCutOff = DefaultStillCutOff * pixelCalibration;
            m_ModifiedWalkingCutOff = DefaultWalkingCutOff * pixelCalibration;
            m_ModifiedNoRotationCutOff = DefaultNoRotationCutOff * pixelCalibration;
            m_ModifiedSlowRotationCutOff = DefaultSlowRotationCutOff * pixelCalibration;
        }
    }
}
