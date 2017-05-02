﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.Rotation;

namespace AutomatedRodentTracker.ViewModel.Results.Behaviour.Rotation
{
    public class SlowTurningViewModel : RotationBehaviourBaseViewModel
    {
        public SlowTurningViewModel() : base(ModelResolver.Resolve<ISlowTurning>())
        {
            
        }
    }
}
