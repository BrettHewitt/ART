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

namespace AutomatedRodentTracker.Model.Results.Behaviour.Movement
{
    internal abstract class MovementBehaviourBase : BehaviourBase, IMovementBehaviour
    {
        protected MovementBehaviourBase(string name, int id) : base(name, id)
        {
            
        }

        public override bool Equals(object obj)
        {
            IMovementBehaviour behaviour = obj as IMovementBehaviour;

            if (behaviour == null)
            {
                return false;
            }

            return Equals(behaviour);
        }

        public bool Equals(IMovementBehaviour behaviour)
        {
            return behaviour.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
