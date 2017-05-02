using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedRodentTracker.ModelInterface.Results.Behaviour.BodyOption
{
    public interface IBodyOptionsBase : IModelObjectBase
    {
        string Name
        {
            get;
            set;
        }

        int Id
        {
            get;
            set;
        }
    }
}
