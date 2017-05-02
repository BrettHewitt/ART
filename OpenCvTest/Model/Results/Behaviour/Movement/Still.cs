﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.Movement;

namespace AutomatedRodentTracker.Model.Results.Behaviour.Movement
{
    internal class Still : MovementBehaviourBase, IStill
    {
        public Still() : base("Still", 1)
        {
            
        }
    }
}
