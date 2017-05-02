using System;
using System.Windows.Shapes;
using AutomatedRodentTracker.Model;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using AutomatedRodentTracker.ModelInterface.Video;

namespace AutomatedRodentTracker.Model.Video
{
    internal class Video : ModelObjectBase, IVideo
    {
        private Capture _Capture;
        public int _TotalFrames = 0;
        private string _FilePath;

        public int FrameNumber
        {
            get
            {
                return (int)_Capture.GetCaptureProperty(CapProp.PosFrames);
            }
        }

        public int FrameCount
        {
            get
            {
                return (int)_Capture.GetCaptureProperty(CapProp.FrameCount);
            }
        }

        public double ElpasedTime
        {
            get
            {
                return _Capture.GetCaptureProperty(CapProp.PosMsec);
            }
        }

        public double FrameRate
        {
            get
            {
                return _Capture.GetCaptureProperty(CapProp.Fps);
            }
        }

        public double Width
        {
            get
            {
                return _Capture.GetCaptureProperty(CapProp.FrameWidth);
            }
        }

        public double Height
        {
            get
            {
                return _Capture.GetCaptureProperty(CapProp.FrameHeight);
            }
        }

        public string FilePath
        {
            get
            {
                return _FilePath;
            }
        }

        public Rectangle Rect
        {
            get;
            set;
        }

        public Mat GetFrameMat()
        {
            return _Capture.QueryFrame();
        }

        public Image<Bgr, Byte> GetFrameImage()
        {
            using (Mat mat = _Capture.QueryFrame())
            {
                if (mat == null)
                {
                    return null;
                }

                return mat.ToImage<Bgr, Byte>();
            }
        }

        public Image<Gray, Byte> GetGrayFrameImage()
        {
            using (Mat mat = _Capture.QueryFrame())
            {
                if (mat == null)
                {
                    return null;
                }

                return mat.ToImage<Gray, Byte>();
            }
        }

        public void SetVideo(string fileName)
        {
            if (_Capture != null)
            {
                _Capture.Dispose();
            }

            _FilePath = fileName;
            _Capture = new Capture(fileName);

            _TotalFrames = (int)_Capture.GetCaptureProperty(CapProp.FrameCount);

            if (_TotalFrames == 0)
            {
                throw new Exception("Video file does not exist: " + fileName);
            }
        }

        public void Reset()
        {
            _Capture.SetCaptureProperty(CapProp.PosFrames, 0);
        }

        public void SetFrame(int frameNumber)
        {
            _Capture.SetCaptureProperty(CapProp.PosFrames, frameNumber);
        }

        public void SetFrame(double relativePosition)
        {
            _Capture.SetCaptureProperty(CapProp.PosAviRatio, relativePosition);
        }

        public void Dispose()
        {
            if (_Capture != null)
            {
                _Capture.Dispose();
            }
        }
    }
}
