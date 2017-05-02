using System.Xml.Serialization;

namespace AutomatedRodentTracker.Model.Datasets
{
    public class MouseCollectionXml
    {
        [XmlArray(ElementName = "Mice")]
        [XmlArrayItem(ElementName = "Mouse")]
        public SingleMouseXml[] Mice
        {
            get;
            set;
        }

        public MouseCollectionXml()
        {
            
        }

        public MouseCollectionXml(SingleMouseXml[] mice)
        {
            Mice = mice;
        }
    }
}
