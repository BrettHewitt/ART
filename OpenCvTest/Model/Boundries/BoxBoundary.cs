using System.Linq;
using AutomatedRodentTracker.Model.XmlClasses;
using AutomatedRodentTracker.ModelInterface.Boundries;

namespace AutomatedRodentTracker.Model.Boundries
{
    internal class BoxBoundary : BoundaryBase, IBoxBoundary
    {
        public BoxBoundary() : base(BoundaryType.Box)
        {

        }

        public override BoundaryBaseXml GetData()
        {
            int id = Id;
            PointXml[] points = Points.Select(x => new PointXml(x.X, x.Y )).ToArray();
            BoxBoundaryXml data = new BoxBoundaryXml(id, points);

            return data;
        }
    }
}
