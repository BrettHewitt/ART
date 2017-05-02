using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.Commands;

namespace AutomatedRodentTracker.ViewModel.Inputs
{
    public abstract class SingleInputBase : WindowViewModelBase
    {
        public abstract string Title
        {
            get;
        }

        public abstract string LabelText
        {
            get;
        }

        public abstract string Text
        {
            get;
            set;
        }

        public abstract ActionCommand OkCommand
        {
            get;
        }

        public abstract ActionCommand CancelCommand
        {
            get;
        }
    }
}
