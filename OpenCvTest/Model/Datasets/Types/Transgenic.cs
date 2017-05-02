using AutomatedRodentTracker.ModelInterface.Datasets.Types;

namespace AutomatedRodentTracker.Model.Datasets.Types
{
    internal class Transgenic : TypeBase, ITransgenic
    {
        public override string Name
        {
            get
            {
                return "Transgenic";
            }
        }
    }
}
