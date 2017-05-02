using System;
using System.Linq;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.Model.XmlClasses;
using AutomatedRodentTracker.ModelInterface.Boundries;

namespace AutomatedRodentTracker.Model.Boundries
{
    [Serializable]
    public class BoxBoundaryXml : BoundaryBaseXml
    {
        public BoxBoundaryXml()
        {
            
        }

        public BoxBoundaryXml(int id, PointXml[] points) : base(id, points)
        {
        }

        public override IBoundaryBase GetBoundary()
        {
            IBoxBoundary boundary = ModelResolver.Resolve<IBoxBoundary>();
            boundary.Id = Id;
            boundary.Points = Points.Select(x => x.GetPoint()).ToArray(); ;
            return boundary;
        }
    }
}
