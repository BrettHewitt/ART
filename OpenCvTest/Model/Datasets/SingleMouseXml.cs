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
