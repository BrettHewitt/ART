using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.BodyOption;

namespace AutomatedRodentTracker.ViewModel.Results.Behaviour.BodyOptions
{
    public class HeadBodyTailVisibleViewModel : BodyOptionsBaseViewModel
    {
        public HeadBodyTailVisibleViewModel() : base(ModelResolver.Resolve<IHeadBodyTailVisible>())
        {
        }
    }
}
