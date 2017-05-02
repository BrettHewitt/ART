using AutomatedRodentTracker.ModelInterface;

namespace AutomatedRodentTracker.ModelInterface.Datasets.Types
{
    public interface ITypeBase : IModelObjectBase
    {
        string Name
        {
            get;
        }
    }
}
