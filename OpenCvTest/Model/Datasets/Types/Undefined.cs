using AutomatedRodentTracker.ModelInterface.Datasets.Types;

namespace AutomatedRodentTracker.Model.Datasets.Types
{
    internal class Undefined : TypeBase, IUndefined
    {
        public override string Name
        {
            get
            {
                return "Undefined";
            }
        }
    }
}
