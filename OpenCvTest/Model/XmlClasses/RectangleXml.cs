using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AutomatedRodentTracker.Model.XmlClasses
{
    public class RectangleXml
    {
        [XmlElement(ElementName = "X")]
        public int X
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Y")]
        public int Y
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Width")]
        public int Width
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Height")]
        public int Height
        {
            get;
            set;
        }

        public RectangleXml()
        {
            
        }

        public RectangleXml(Rectangle roi)
        {
            X = roi.X;
            Y = roi.Y;
            Width = roi.Width;
            Height = roi.Height;
        }

        public RectangleXml(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rectangle GetRect()
        {
            return new Rectangle(X, Y, Width, Height);
        }
    }
}
