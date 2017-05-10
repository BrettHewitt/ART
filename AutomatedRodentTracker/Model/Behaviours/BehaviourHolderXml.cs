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
using System.Xml.Serialization;
using AutomatedRodentTracker.Model.Boundries;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.ModelInterface.Behaviours;

namespace AutomatedRodentTracker.Model.Behaviours
{
    [Serializable]
    public class BehaviourHolderXml
    {
        [XmlElement(ElementName = "Boundary")]
        public BoundaryBaseXml Boundary
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Interaction")]
        public InteractionBehaviour Interaction
        {
            get;
            set;
        }

        [XmlElement(ElementName = "FrameNumber")]
        public int FrameNumber
        {
            get;
            set;
        }

        public BehaviourHolderXml()
        {
            
        }

        public BehaviourHolderXml(BoundaryBaseXml boundary, InteractionBehaviour interaction, int frameNumber)
        {
            Boundary = boundary;
            Interaction = interaction;
            FrameNumber = frameNumber;
        }

        public IBehaviourHolder GetData()
        {
            IBehaviourHolder data = ModelResolver.Resolve<IBehaviourHolder>();

            data.Boundary = Boundary.GetBoundary();
            data.Interaction = Interaction;
            data.FrameNumber = FrameNumber;

            return data;
        }
    }
}
