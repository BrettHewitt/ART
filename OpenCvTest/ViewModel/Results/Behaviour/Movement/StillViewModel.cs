﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.Movement;

namespace AutomatedRodentTracker.ViewModel.Results.Behaviour.Movement
{
    public class StillViewModel : MovementBehaviourBaseViewModel
    {
        public StillViewModel() : base(ModelResolver.Resolve<IStill>())
        {
            
        }
    }
}
