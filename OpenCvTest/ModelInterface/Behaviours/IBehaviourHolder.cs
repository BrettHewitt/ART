using AutomatedRodentTracker.Model.Behaviours;
using AutomatedRodentTracker.ModelInterface.Boundries;

namespace AutomatedRodentTracker.ModelInterface.Behaviours
{
    public interface IBehaviourHolder : IModelObjectBase
    {
        IBoundaryBase Boundary
        {
            get;
            set;
        }

        InteractionBehaviour Interaction
        {
            get;
            set;
        }

        int FrameNumber
        {
            get;
            set;
        }

        int StartFrame
        {
            get;
            set;
        }

        int EndFrame
        {
            get;
            set;
        }
    }
}
