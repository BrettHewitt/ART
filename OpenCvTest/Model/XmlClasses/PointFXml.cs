using System.Drawing;
using System.Xml.Serialization;

namespace AutomatedRodentTracker.Model.XmlClasses
{
    public class PointFXml
    {
        [XmlElement(ElementName = "X")]
        public float X
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Y")]
        public float Y
        {
            get;
            set;
        }

        public PointFXml()
        {
            
        }

        public PointFXml(PointF point)
        {
            X = point.X;
            Y = point.Y;
        }

        public PointFXml(float x, float y)
        {
            X = x;
            Y = y;
        }

        public PointF GetPoint()
        {
            return new PointF(X, Y);
        }
    }
}
