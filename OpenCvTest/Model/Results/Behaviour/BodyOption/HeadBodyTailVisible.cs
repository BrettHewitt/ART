using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.BodyOption;

namespace AutomatedRodentTracker.Model.Results.Behaviour.BodyOption
{
    internal class HeadBodyTailVisible : BodyOptionsBase, IHeadBodyTailVisible
    {
        public HeadBodyTailVisible() : base("Head, body and Tail Visible", 1)
        {

        }
    }
}
