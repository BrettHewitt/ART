using AutomatedRodentTracker.ModelInterface;

namespace AutomatedRodentTracker.ModelInterface.Datasets
{
    public interface ISingleFile : IModelObjectBase
    {
        string VideoFileName
        {
            get;
            set;
        }

        string VideoNumber
        {
            get;
            set;
        }
    }
}
