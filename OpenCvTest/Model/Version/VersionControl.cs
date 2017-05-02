using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.Model.Video;
using AutomatedRodentTracker.ModelInterface.Version;

namespace AutomatedRodentTracker.Model.Version
{
    internal class VersionControl : ModelObjectBase, IVersionControl
    {
        private const double m_CurrentVersion = 1.1;

        public double CurrentVersion
        {
            get
            {
                return m_CurrentVersion;
            }
        }

        public void UpdateToLatestVersion(TrackedVideoXml video)
        {
            
        }
    }
}
