using AutomatedRodentTracker.ModelInterface;

namespace AutomatedRodentTracker.ModelInterface.Datasets
{
    public interface ILabbookData : IModelObjectBase
    {
        string Class
        {
            get;
            set;
        }

        string Calib
        {
            get;
            set;
        }

        int Year
        {
            get;
            set;
        }

        int Month
        {
            get;
            set;
        }

        int Day
        {
            get;
            set;
        }
    }
}
