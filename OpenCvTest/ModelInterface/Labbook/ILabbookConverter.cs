using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.ModelInterface;

namespace AutomatedRodentTracker.ModelInterface.Labbook
{
    public interface ILabbookConverter : IModelObjectBase
    {
        List<ILabbookData> GenerateLabbookData(string[] lines, int age = -1);
    }
}
