using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.Model.Results.Behaviour.Movement;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.Rotation;

namespace AutomatedRodentTracker.Model.Results.Behaviour.Rotation
{
    internal class RotationBehaviourBase : MovementBehaviourBase, IRotationBehaviour
    {


        protected RotationBehaviourBase(string name, int id) : base(name, id)
        {
            
        }

        public override bool Equals(object obj)
        {
            IRotationBehaviour behaviour = obj as IRotationBehaviour;

            if (behaviour == null)
            {
                return false;
            }

            return Equals(behaviour);
        }

        public bool Equals(IRotationBehaviour behaviour)
        {
            return behaviour.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
