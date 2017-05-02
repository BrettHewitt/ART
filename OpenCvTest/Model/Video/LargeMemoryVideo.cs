using System;
using System.Collections.Generic;
using AutomatedRodentTracker.Model;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using AutomatedRodentTracker.ModelInterface.Video;

namespace AutomatedRodentTracker.Model.Video
{
    internal class LargeMemoryVideo : ModelObjectBase, IVideo
    {
        private Image<Gray, Byte>[] m_Frames;
        private int m_FrameNumber = 0;

        public Image<Gray, Byte>[] Frames
        {
            get
            {
                return m_Frames;
            }
            set
            {
                if (ReferenceEquals(m_Frames, value))
                {
                    return;
                }

                m_Frames = value;

                MarkAsDirty();
            }
        }

        public int FrameNumber
        {
            get
            {
                return m_FrameNumber;
            }
            set
            {
                if (Equals(m_FrameNumber, value))
                {
                    return;
                }

                m_FrameNumber = value;

                MarkAsDirty();
            }
        }

        public int FrameCount
        {
            get
            {
                return Frames.Length;
            }
        }

        public double ElpasedTime
        {
            get
            {
                return FrameNumber/FrameRate;
            }
        }

        public double FrameRate
        {
            get;
            private set;
        }

        public double Width
        {
            get;
            private set;
        }

        public double Height
        {
            get;
            private set;
        }

        public string FilePath
        {
            get;
            private set;
        }

        public Mat GetFrameMat()
        {
            //if (FrameNumber >= Frames.Length)
            //{
            //    return null;
            //}

            //Mat mat = Frames[FrameNumber];
            //FrameNumber++;
            //return mat;
            if (FrameNumber >= Frames.Length)
            {
                return null;
            }

            Image<Gray, Byte> image = Frames[FrameNumber];
            FrameNumber++;
            return image.Mat;
        }

        public Image<Bgr, byte> GetFrameImage()
        {
            if (FrameNumber >= Frames.Length)
            {
                return null;
            }

            Image<Bgr, Byte> image = Frames[FrameNumber].Convert<Bgr, Byte>();
            FrameNumber++;
            return image;
        }

        public Image<Gray, byte> GetGrayFrameImage()
        {
            if (FrameNumber >= Frames.Length)
            {
                return null;
            }

            Image<Gray, Byte> image = Frames[FrameNumber];
            FrameNumber++;
            return image;
        }

        public void SetVideo(string filepath)
        {
            LoadVideo(filepath);
        }

        public void Reset()
        {
            FrameNumber = 0;
        }

        public void SetFrame(int frameNumber)
        {
            FrameNumber = frameNumber;
        }

        public void SetFrame(double relativePosition)
        {
            FrameNumber = (int)(relativePosition*FrameCount);
        }

        public void LoadVideo(string filePath)
        {
            List<Image<Gray, Byte>> frames = new List<Image<Gray, Byte>>();
            
            Capture capture = new Capture(filePath);
            //Image<Gray, Byte>[] frames = new Image<Gray, byte>[(int)capture.GetCaptureProperty(CapProp.FrameCount)];
            int counter = 0;
            while (true)
            {
                using (Mat currentFrame = capture.QueryFrame())
                {
                    if (currentFrame == null)
                    {
                        break;
                    }

                    Image<Gray, Byte> image = currentFrame.ToImage<Gray, Byte>();

                    frames.Add(image);
                }

                Console.WriteLine(counter);
                counter++;
            }
            
            Frames = frames.ToArray();
            FilePath = filePath;
            Width = Frames[0].Width;
            Height = Frames[0].Height;
            FrameRate = capture.GetCaptureProperty(CapProp.Fps);
            capture.Dispose();
        }

        public void Dispose()
        {
            foreach (Image<Gray, Byte> image in Frames)
            {
                image.Dispose();
            }
        }
    }
}
