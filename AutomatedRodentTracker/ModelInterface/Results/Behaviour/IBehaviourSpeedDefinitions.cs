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
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.Movement;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.Rotation;

namespace AutomatedRodentTracker.ModelInterface.Results.Behaviour
{
    public interface IBehaviourSpeedDefinitions : IModelObjectBase
    {
        double StillCutOff
        {
            get;
        }

        double WalkingCutOff
        {
            get;
        }

        double NoRotationCutOff
        {
            get;
        }

        double SlowRotationCutOff
        {
            get;
        }

        /// <summary>
        /// Get the movement model for the velocity
        /// </summary>
        /// <param name="velocity">The vlocity over 1 second</param>
        /// <returns></returns>
        IMovementBehaviour GetMovementBehaviour(double velocity);

        /// <summary>
        /// Get the rotational model for the velocity
        /// </summary>
        /// <param name="angularVelocity">The angular velocity over 1 second</param>
        /// <returns></returns>
        IRotationBehaviour GetRotationBehaviour(double angularVelocity);

        /// <summary>
        /// Update the cut offs with a calibration metric
        /// </summary>
        /// <param name="pixelCalibration">Units per pixel</param>
        void UpdateCutOffs(double pixelCalibration);
    }
}
