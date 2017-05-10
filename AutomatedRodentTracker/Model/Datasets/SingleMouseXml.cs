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

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.ModelInterface.Datasets;
using AutomatedRodentTracker.ModelInterface.Datasets.Types;

namespace AutomatedRodentTracker.Model.Datasets
{
    public class SingleMouseXml
    {
        [XmlElement(ElementName = "Version")]
        public double Version
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Name")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Id")]
        public string Id
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Type")]
        public string Type
        {
            get;
            set;
        }

        [XmlArray(ElementName = "Videos")]
        [XmlArrayItem(ElementName = "Filename")]
        public string[] Videos
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Class")]
        public string Class
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Age")]
        public int Age
        {
            get;
            set;
        }

        public SingleMouseXml()
        {

        }

        public SingleMouseXml(string name, string id, ITypeBase type, string[] videos, string mClass, int age)
        {
            Version = 1.0;
            Name = name;
            Id = id;
            Type = type.Name;
            Videos = videos;
            Class = mClass;
            Age = age;
        }

        public ISingleMouse GetMouse()
        {
            if (Videos.Length == 0)
            {
                return null;
            }

            ISingleMouse mouse = ModelResolver.Resolve<ISingleMouse>();

            mouse.Name = Name;
            mouse.Id = Id;
            mouse.Age = Age;
            mouse.Class = Class;

            //ITypeBase type;
            switch (Type)
            {
                case "Non-Transgenic":
                    mouse.Type = ModelResolver.Resolve<INonTransgenic>();
                    break;

                case "Transgenic":
                    mouse.Type = ModelResolver.Resolve<ITransgenic>();
                    break;

                case "Undefined":
                    mouse.Type = ModelResolver.Resolve<IUndefined>();
                    break;
            }

            List<string> videos = new List<string>();
            foreach (string video in Videos)
            {
                string fileName = Path.GetFileName(video);
                string extension = Path.GetExtension(fileName);
                fileName = fileName.Replace(extension, "");
                int index = fileName.IndexOf("-");
                index++;
                fileName = fileName.Substring(index, fileName.Length-index);
                videos.Add(fileName);
            }

            mouse.Videos = videos;
            mouse.GenerateFiles(Videos[0]);

            return mouse;
        }
    }
}
