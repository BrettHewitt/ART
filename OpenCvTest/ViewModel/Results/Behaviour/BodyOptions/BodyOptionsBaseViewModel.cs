using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.BodyOption;

namespace AutomatedRodentTracker.ViewModel.Results.Behaviour.BodyOptions
{
    public abstract class BodyOptionsBaseViewModel : ViewModelBase
    {
        private IBodyOptionsBase m_Model;
        public IBodyOptionsBase Model
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

        protected BodyOptionsBaseViewModel(IBodyOptionsBase model)
        {
            Model = model;
        }
    }
}
