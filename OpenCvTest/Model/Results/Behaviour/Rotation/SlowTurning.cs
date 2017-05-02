using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.Rotation;

namespace AutomatedRodentTracker.Model.Results.Behaviour.Rotation
{
    internal class SlowTurning : RotationBehaviourBase, ISlowTurning
    {
        public SlowTurning() : base("Slow Turning", 2)
        {
            
        }
    }
}
