using System;
using System.Linq;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.Model.XmlClasses;
using AutomatedRodentTracker.ModelInterface.Boundries;

namespace AutomatedRodentTracker.Model.Boundries
{
    [Serializable]
    public class ArtefactsBoundaryXml : BoundaryBaseXml
    {
        public ArtefactsBoundaryXml()
        {
            
        }

        public ArtefactsBoundaryXml(int id, PointXml[] points) : base(id, points)
        {
        }

        public override IBoundaryBase GetBoundary()
        {
            IArtefactsBoundary boundary = ModelResolver.Resolve<IArtefactsBoundary>();
            boundary.Id = Id;
            boundary.Points = Points.Select(x => x.GetPoint()).ToArray();
            return boundary;
        }
    }
}
