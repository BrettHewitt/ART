using AutomatedRodentTracker.Model;
using AutomatedRodentTracker.ModelInterface.Datasets.Types;

namespace AutomatedRodentTracker.Model.Datasets.Types
{
    internal class NonTransgenic : ModelObjectBase, INonTransgenic
    {
        public string Name
        {
            get
            {
                return "Non-Transgenic";
            }
        }
    }
}
