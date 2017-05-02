using System;
using System.Linq;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.Model.XmlClasses;
using AutomatedRodentTracker.ModelInterface.Boundries;

namespace AutomatedRodentTracker.Model.Boundries
{
    [Serializable]
    public class OuterBoundaryXml : BoundaryBaseXml
    {
        public OuterBoundaryXml()
        {
            
        }

        public OuterBoundaryXml(int id, PointXml[] points) : base(id, points)
        {
        }

        public override IBoundaryBase GetBoundary()
        {
            IOuterBoundary boundary = ModelResolver.Resolve<IOuterBoundary>();
            boundary.Id = Id;
            boundary.Points = Points.Select(x => x.GetPoint()).ToArray(); ;
            return boundary;
        }
    }
}
