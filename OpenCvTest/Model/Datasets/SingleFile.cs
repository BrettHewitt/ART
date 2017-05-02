using AutomatedRodentTracker.Model;
using AutomatedRodentTracker.ModelInterface.Datasets;

namespace AutomatedRodentTracker.Model.Datasets
{
    internal class SingleFile : ModelObjectBase, ISingleFile
    {
        private string m_VideoFileName;
        private string m_VideoNumber;

        public string VideoFileName
        {
            get
            {
                return m_VideoFileName;
            }
            set
            {
                if (Equals(m_VideoFileName, value))
                {
                    return;
                }

                m_VideoFileName = value;

                UpdateData();
                MarkAsDirty();
            }
        }

        public string VideoNumber
        {
            get
            {
                return m_VideoNumber;
            }
            set
            {
                if (Equals(m_VideoNumber, value))
                {
                    return;
                }

                m_VideoNumber = value;

                MarkAsDirty();
            }
        }

        private void UpdateData()
        {
            int lastIndex = VideoFileName.LastIndexOf(@"\");
            lastIndex++;
            string fileName = VideoFileName.Substring(lastIndex);

            int dashIndex = fileName.IndexOf(@"-");
            string videoNumber = fileName.Substring(dashIndex+1);
            
            VideoNumber = videoNumber;
        }
    }
}
