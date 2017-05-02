using System.Drawing;
using AutomatedRodentTracker.Model.Boundries;

namespace AutomatedRodentTracker.ModelInterface.Boundries
{
    public interface IBoundaryBase : IModelObjectBase
    {
        int Id
        {
            get;
            set;
        }
        
        Point[] Points
        {
            get;
            set;
        }

        double GetMinimumDistance(PointF point);
        BoundaryBaseXml GetData();
    }
}
