using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedRodentTracker.ModelInterface.Results.Behaviour
{
    public interface IBehaviourBase : IModelObjectBase
    {
        int Id
        {
            get;
            set;
        }

        string Name
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
