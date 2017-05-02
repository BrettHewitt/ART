using AutomatedRodentTracker.Model;
using AutomatedRodentTracker.ModelInterface.Datasets.Types;

namespace AutomatedRodentTracker.Model.Datasets.Types
{
    internal abstract class TypeBase : ModelObjectBase, ITypeBase
    {
        public abstract string Name
        {
            get;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
