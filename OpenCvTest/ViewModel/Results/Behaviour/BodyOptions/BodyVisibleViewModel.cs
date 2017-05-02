using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.BodyOption;

namespace AutomatedRodentTracker.ViewModel.Results.Behaviour.BodyOptions
{
    public class BodyVisibleViewModel : BodyOptionsBaseViewModel
    {
        public BodyVisibleViewModel() : base(ModelResolver.Resolve<IBodyVisible>())
        {
        }
    }
}
