using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.Rotation;

namespace AutomatedRodentTracker.Model.Results.Behaviour.Rotation
{
    internal class FastTurning : RotationBehaviourBase, IFastTurning
    {
        public FastTurning() : base("Fast Turning", 3)
        {
            
        }
    }
}
