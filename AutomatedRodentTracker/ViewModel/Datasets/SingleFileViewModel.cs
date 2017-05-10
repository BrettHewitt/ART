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
