using System.Windows;
using System.Xml.Serialization;

namespace AutomatedRodentTracker.Model.XmlClasses
{
    public class VectorXml
    {
        [XmlElement(ElementName = "X")]
        public double X
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Y")]
        public double Y
        {
            get;
            set;
        }

        public VectorXml()
        {

        }

        public VectorXml(int x, int y)
        {
            X = x;
            Y = y;
        }

        public VectorXml(Vector vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        public Vector GetVector()
        {
            return new Vector(X, Y);
        }
    }
}
