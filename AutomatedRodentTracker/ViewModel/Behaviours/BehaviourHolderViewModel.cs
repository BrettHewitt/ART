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

using AutomatedRodentTracker.Model.Behaviours;
using AutomatedRodentTracker.ViewModel.NewWizard;

namespace AutomatedRodentTracker.ViewModel.Behaviours
{
    public class BehaviourHolderViewModel : ViewModelBase
    {
        public BoundaryBaseViewModel Boundary
        {
            get;
            set;
        }

        public InteractionBehaviour Interaction
        {
            get;
            set;
        }

        public int FrameNumber
        {
            get;
            set;
        }

        public string BoundaryName
        {
            get
            {
                return Boundary.Name;
            }
        }

        public string Name
        {
            get
            {
                string firstComponent;

                if (Interaction == InteractionBehaviour.Started)
                {
                    firstComponent = "Begun ";
                }
                else
                {
                    firstComponent = "Ended ";
                }

                firstComponent += "interaction";

                return firstComponent;
            }
        }

        public BehaviourHolderViewModel(BoundaryBaseViewModel boundary, InteractionBehaviour interaction, int frameNumber)
        {
            Boundary = boundary;
            Interaction = interaction;
            FrameNumber = frameNumber;
        }
    }
}
