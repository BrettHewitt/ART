using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.Model.Results;
using AutomatedRodentTracker.ModelInterface.Datasets;
using AutomatedRodentTracker.ViewModel;

namespace AutomatedRodentTracker.ViewModel.Datasets
{
    public class SingleFileViewModel : ViewModelBase
    {
        private ISingleFile m_Model;
        public ISingleFile Model
        {
            get
            {
                return m_Model;
            }
            private set
            {
                if (Equals(m_Model, value))
                {
                    return;
                }

                m_Model = value;

                NotifyPropertyChanged();
            }
        }

        public string VideoFileName
        {
            get
            {
                return Model.VideoFileName;
            }
        }

        private string m_ArtFileLocation;
        public string ArtFileLocation
        {
            get
            {
                return m_ArtFileLocation;
            }
            private set
            {
                m_ArtFileLocation = value;

                NotifyPropertyChanged();
            }
        }

        private SingleFileResult m_VideoOutcome;
        public SingleFileResult VideoOutcome
        {
            get
            {
                return m_VideoOutcome;
            }
            set
            {
                if (Equals(m_VideoOutcome, value))
                {
                    return;
                }

                m_VideoOutcome = value;

                NotifyPropertyChanged();
            }
        }

        private string m_Comments;
        public string Comments
        {
            get
            {
                return m_Comments;
            }
            set
            {
                if (Equals(m_Comments, value))
                {
                    return;
                }

                m_Comments = value;

                NotifyPropertyChanged();
            }
        }



        public SingleFileViewModel(ISingleFile model, string artFileLocation)
        {
            Model = model;
            ArtFileLocation = artFileLocation;
        }
    }
}
