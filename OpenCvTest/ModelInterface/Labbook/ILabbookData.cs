using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.ModelInterface;
using AutomatedRodentTracker.ModelInterface.Datasets;

namespace AutomatedRodentTracker.ModelInterface.Labbook
{
    public interface ILabbookData : IModelObjectBase
    {
        int Class
        {
            get;
            set;
        }

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

        string Type
        {
            get;
            set;
        }

        int StartVideo
        {
            get;
            set;
        }

        int EndVideo
        {
            get;
            set;
        }

        int Age
        {
            get;
            set;
        }

        bool ContainsVideo(int vidId);
        bool UpdateData(ISingleFile singleFile);
    }
}
