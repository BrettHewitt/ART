using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.Movement;

namespace AutomatedRodentTracker.ViewModel.Results.Behaviour.Movement
{
    public abstract class MovementBehaviourBaseViewModel : ViewModelBase
    {
        private IMovementBehaviour m_Model;
        public IMovementBehaviour Model
        {
            get
            {
                return m_Model;
            }
            set
            {
                if (Equals(m_Model, value))
                {
                    return;
                }

                m_Model = value;

                NotifyPropertyChanged();
            }
        }

        public string Name
        {
            get
            {
                return Model.Name;
            }
        }

        public int Id
        {
            get
            {
                return Model.Id;
            }
        }

        protected MovementBehaviourBaseViewModel(IMovementBehaviour model)
        {
            Model = model;
        }
    }
}
