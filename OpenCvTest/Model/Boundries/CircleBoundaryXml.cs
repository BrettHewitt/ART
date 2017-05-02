using System;
using System.Linq;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.Model.XmlClasses;
using AutomatedRodentTracker.ModelInterface.Boundries;

namespace AutomatedRodentTracker.Model.Boundries
{
    [Serializable]
    public class CircleBoundaryXml : BoundaryBaseXml
    {
        public CircleBoundaryXml()
        {
            
        }

        public CircleBoundaryXml(int id, PointXml[] points) : base(id, points)
        {
        }

        public override IBoundaryBase GetBoundary()
        {
            ICircleBoundary boundary = ModelResolver.Resolve<ICircleBoundary>();
            boundary.Id = Id;
            boundary.Points = Points.Select(x => x.GetPoint()).ToArray(); ;
            return boundary;
        }

        
    }
}
