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
