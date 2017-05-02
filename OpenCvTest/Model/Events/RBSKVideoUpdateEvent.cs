using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedRodentTracker.Model.Events
{
    public class RBSKVideoUpdateEvent : EventArgs
    {
        public double Progress
        {
            get;
            set;
        }

        public RBSKVideoUpdateEvent(double progress)
        {
            Progress = progress;
        }
    }

    public delegate void RBSKVideoUpdateEventHandler(object sender, RBSKVideoUpdateEvent e);
}
