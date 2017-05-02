using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvTest.ModelInterface.Datasets;
using OpenCvTest.ModelInterface.Labbook;

namespace OpenCvTest.Model.Labbook
{
    internal class LabbookData : ModelObjectBase, ILabbookData
    {
        private int m_Class;
        private string m_Name;
        private string m_Id;
        private string m_Type = string.Empty;
        private int m_StartVideo;
        private int m_EndVideo;
        private int m_Age;

        public int Class
        {
            get
            {
                return m_Class;
            }
            set
            {
                if (Equals(m_Class, value))
                {
                    return;
                }

                m_Class = value;

                MarkAsDirty();
            }
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                if (Equals(m_Name, value))
                {
                    return;
                }

                m_Name = value;

                MarkAsDirty();
            }
        }

        public string Id
        {
            get
            {
                return m_Id;
            }
            set
            {
                if (Equals(m_Id, value))
                {
                    return;
                }

                m_Id = value;

                MarkAsDirty();
            }
        }

        public string Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                if (Equals(m_Type, value))
                {
                    return;
                }

                m_Type = value;

                MarkAsDirty();
            }
        }

        public int StartVideo
        {
            get
            {
                return m_StartVideo;
            }
            set
            {
                if (Equals(m_StartVideo, value))
                {
                    return;
                }

                m_StartVideo = value;

                MarkAsDirty();
            }
        }

        public int EndVideo
        {
            get
            {
                return m_EndVideo;
            }
            set
            {
                if (Equals(m_EndVideo, value))
                {
                    return;
                }

                m_EndVideo = value;

                MarkAsDirty();
            }
        }

        public int Age
        {
            get
            {
                return m_Age;
            }
            set
            {
                if (Equals(m_Age, value))
                {
                    return;
                }

                m_Age = value;

                MarkAsDirty();
            }
        }

        public bool ContainsVideo(int vidId)
        {
            return vidId >= StartVideo && vidId <= EndVideo;
        }

        public bool UpdateData(ISingleFile singleFile)
        {
            if (singleFile.Class != Class)
            {
                return false;
            }

            if (!ContainsVideo(singleFile.VideoNumber))
            {
                return false;
            }

            singleFile.Id = Id;
            singleFile.Name = Name;
            singleFile.Age = Age;

            return true;
        }
    }
}
