using System.Collections.Generic;
using AutomatedRodentTracker.ModelInterface;
using AutomatedRodentTracker.ModelInterface.Datasets.Types;
using AutomatedRodentTracker.ModelInterface.Results;

namespace AutomatedRodentTracker.ModelInterface.Datasets
{
    public interface ISingleMouse : IModelObjectBase
    {
        string Name
        {
            get;
            set;
        }

        string Id
        {
            get;
            set;
        }

        ITypeBase Type
        {
            get;
            set;
        }

        List<string> Videos
        {
            get;
            set;
        }

        string Class
        {
            get;
            set;
        }

        int Age
        {
            get;
            set;
        }

        List<ISingleFile> VideoFiles
        {
            get;
            set;
        }

        Dictionary<ISingleFile, IMouseDataResult> Results
        {
            get;
            set;
        }

        void GenerateFiles(string fileLocation);
    }
}
