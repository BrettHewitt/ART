using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvTest.ModelInterface.Labbook;

namespace OpenCvTest.Model.Labbook
{
    internal class LabbookConverter : ModelObjectBase, ILabbookConverter
    {
        public List<ILabbookData> GenerateLabbookData(string[] lines, int age = -1)
        {
            if (lines == null || lines.Length == 0)
            {
                return null;
            }

            List<ILabbookData> allData = new List<ILabbookData>();
            
            int vidClass = -1;
            string firstLine = lines[0];
            if (firstLine.StartsWith(@"#"))
            {
                if (!int.TryParse(firstLine.Replace(@"#", ""), out vidClass))
                {
                    return null;
                }
            }

            int length = lines.Length;
            for (int i = 1; i < length; i++)
            {
                string currentLine = lines[i];

                if (!currentLine.StartsWith(@"#"))
                {
                    continue;
                }

                //We begin with a hash, next look for "..." on either 3rd of 4th index

                bool goodToGo = false;
                bool thirdLine = true;
                if (i + 3 < length)
                {
                    if (lines[i + 3] == "...")
                    {
                        goodToGo = true;
                    }
                }

                if (i + 4 < length && !goodToGo)
                {
                    if (lines[i + 4] == "...")
                    {
                        goodToGo = true;
                        thirdLine = false;
                    }
                }

                if (!goodToGo)
                {
                    continue;
                }

                //Current line is the name (probably unknown)
                string id = lines[i + 1];
                string type = string.Empty;
                string startVid = string.Empty;
                string endVid = string.Empty;

                if (thirdLine)
                {
                    startVid = lines[i + 2];
                    endVid = lines[i + 4];
                }
                else
                {
                    type = lines[i + 2];
                    startVid = lines[i + 3];
                    endVid = lines[i + 5];
                }

                int startVideo, endVideo;

                if (!int.TryParse(startVid, out startVideo))
                {
                    continue;
                }

                if (!int.TryParse(endVid, out endVideo))
                {
                    continue;
                }

                ILabbookData labbookData = ModelResolver.ModelResolver.Resolve<ILabbookData>();
                labbookData.Class = vidClass;
                labbookData.Name = currentLine.Replace(@"#", "");
                labbookData.Id = id;
                labbookData.Type = type;
                labbookData.StartVideo = startVideo;
                labbookData.EndVideo = endVideo;

                if (age > 0)
                {
                    labbookData.Age = age;
                }

                allData.Add(labbookData);
            }

            return allData;
        }
    }
}
