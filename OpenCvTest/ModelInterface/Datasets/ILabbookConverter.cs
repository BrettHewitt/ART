using System.Collections.Generic;
using AutomatedRodentTracker.ModelInterface;

namespace AutomatedRodentTracker.ModelInterface.Datasets
{
    public interface ILabbookConverter : IModelObjectBase
    {
        List<ISingleMouse> GenerateLabbookData(string[] lines, string fileLocation, int age = -1);
    }
}
