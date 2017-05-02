using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.Rotation;

namespace AutomatedRodentTracker.ViewModel.Results.Behaviour.Rotation
{
    public abstract class RotationBehaviourBaseViewModel : ViewModelBase
    {
        private IRotationBehaviour m_Model;
        public IRotationBehaviour Model
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

        protected RotationBehaviourBaseViewModel(IRotationBehaviour model)
        {
            Model = model;
        }
    }
}
