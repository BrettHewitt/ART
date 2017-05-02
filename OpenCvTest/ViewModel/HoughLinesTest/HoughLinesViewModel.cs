using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;
using AutomatedRodentTracker.Commands;
using AutomatedRodentTracker.Extensions;
using AutomatedRodentTracker.Services.FileBrowser;
using Emgu.CV;
using Emgu.CV.Structure;
using AutomatedRodentTracker.Services.ImageToBitmapSource;
using AutomatedRodentTracker.ViewModel;

namespace AutomatedRodentTracker.ViewModel.HoughLinesTest
{
    public class HoughLinesViewModel : WindowViewModelBase
    {
        private ActionCommand m_OpenCommand;

        public ActionCommand OpenCommand
        {
            get
            {
                return m_OpenCommand ?? (m_OpenCommand = new ActionCommand()
                {
                    ExecuteAction = OpenImage,
                });
            }
        }

        private ActionCommand m_SaveCommand;

        public ActionCommand SaveCommand
        {
            get
            {
                return m_SaveCommand ?? (m_SaveCommand = new ActionCommand()
                {
                    ExecuteAction = Save
                });
            }
        }

        private double m_DistanceResolution;
        private double m_AngleResolution;
        private int m_Threshold;
        private double m_MinLineWidth;
        private double m_GapBetweenLines;
        private Image<Gray, Byte> m_MainImage;
        private BitmapSource m_DisplayImage;
        private BitmapSource m_DisplayImage2;
        private BitmapSource m_DisplayImage3;

        public double DistanceResolution
        {
            get
            {
                return m_DistanceResolution;
            }
            set
            {
                if (Equals(m_DistanceResolution, value))
                {
                    return;
                }

                m_DistanceResolution = value;

                NotifyPropertyChanged();
                UpdateImage();
            }
        }

        public double AngleResolution
        {
            get
            {
                return m_AngleResolution;
            }
            set
            {
                if (Equals(m_AngleResolution, value))
                {
                    return;
                }

                m_AngleResolution = value;

                NotifyPropertyChanged();
                UpdateImage();
            }
        }

        public int Threshold
        {
            get
            {
                return m_Threshold;
            }
            set
            {
                if (Equals(m_Threshold, value))
                {
                    return;
                }

                m_Threshold = value;

                NotifyPropertyChanged();
                UpdateImage();
            }
        }

        public double MinLineWidth
        {
            get
            {
                return m_MinLineWidth;
            }
            set
            {
                if (Equals(m_MinLineWidth, value))
                {
                    return;
                }

                m_MinLineWidth = value;

                NotifyPropertyChanged();
                UpdateImage();
            }
        }

        public double GapBetweenLines
        {
            get
            {
                return m_GapBetweenLines;
            }
            set
            {
                if (Equals(m_GapBetweenLines, value))
                {
                    return;
                }

                m_GapBetweenLines = value;

                NotifyPropertyChanged();
                UpdateImage();
            }
        }

        public Image<Gray, Byte> MainImage
        {
            get
            {
                return m_MainImage;
            }
            set
            {
                if (ReferenceEquals(m_MainImage, value))
                {
                    return;
                }

                if (m_MainImage != null)
                {
                    m_MainImage.Dispose();
                }

                m_MainImage = value;

                NotifyPropertyChanged();
                UpdateImage();
            }
        }

        public BitmapSource DisplayImage
        {
            get
            {
                return m_DisplayImage;
            }
            set
            {
                if (ReferenceEquals(m_DisplayImage, value))
                {
                    return;
                }

                m_DisplayImage = value;

                NotifyPropertyChanged();
            }
        }

        public BitmapSource DisplayImage2
        {
            get
            {
                return m_DisplayImage2;
            }
            set
            {
                if (ReferenceEquals(m_DisplayImage2, value))
                {
                    return;
                }

                m_DisplayImage2 = value;

                NotifyPropertyChanged();
            }
        }

        public BitmapSource DisplayImage3
        {
            get
            {
                return m_DisplayImage3;
            }
            set
            {
                if (ReferenceEquals(m_DisplayImage3, value))
                {
                    return;
                }

                m_DisplayImage3 = value;

                NotifyPropertyChanged();
            }
        }

        public HoughLinesViewModel()
        {
            DistanceResolution = 1;
            AngleResolution = 45;
            Threshold = 20;
            MinLineWidth = 5;
            GapBetweenLines = 40;
            Random = new Random();
        }

        private void OpenImage()
        {
            string image = FileBrowser.BrowseForImageFiles();

            if (string.IsNullOrWhiteSpace(image))
            {
                return;
            }

            MainImage = new Image<Gray, byte>(image);
        }

        Image<Gray, Byte> SaveImage
        {
            get;
            set;
        }

        private void UpdateImage()
        {
            const int iterations = 1;

            if (MainImage == null)
            {
                return;
            }

            Image<Gray, Byte> testImage;

            using (Image<Gray, Byte> dilate1 = MainImage.Dilate(iterations))
            using (Image<Gray, Byte> erode1 = dilate1.Erode(iterations))
            using (Image<Gray, Byte> dilate2 = erode1.Dilate(iterations))
            using (Image<Gray, Byte> erode2 = dilate2.Erode(iterations))
            using (Image<Gray, Byte> dilate3 = erode2.Dilate(iterations))
            using (Image<Gray, Byte> erode3 = dilate3.Erode(iterations))
            {
                testImage = erode3.Clone();
                DisplayImage = ImageService.ToBitmapSource(testImage);
                //DisplayImage2 = ImageService.ToBitmapSource(MainImage);
            }

            LineSegment2D[] lines = CvInvoke.HoughLinesP(testImage,
                   DistanceResolution, //Distance resolution in pixel-related units
                   Math.PI / AngleResolution, //Angle resolution measured in radians.
                   Threshold, //threshold
                   MinLineWidth, //min Line width
                   GapBetweenLines); //gap between lines

            FindSpine(lines, RotatedRect.Empty, testImage);
            using (Image<Bgr, Byte> moddedImage = testImage.Convert<Bgr, Byte>())
            {
                SaveImage = new Image<Gray, byte>(moddedImage.Width, moddedImage.Height, new Gray(0));
                foreach (var line in lines)
                {
                    moddedImage.Draw(line, new Bgr(Color.Aqua), 2);
                    SaveImage.Draw(line, new Gray(255), 1);
                }

                DisplayImage2 = ImageService.ToBitmapSource(moddedImage);
            }
        }

        private void FindSpine(LineSegment2D[] lines, RotatedRect rotatedRectangle, Image<Gray, Byte> img)
        {
            LineSegment2DF[] initialLines = new LineSegment2DF[2];

            if (!rotatedRectangle.Size.IsEmpty)
            {
                //Use one of the smaller boundries from rotatedRect for initial detection
                PointF[] vertices = rotatedRectangle.GetVertices();
                PointF p1 = vertices[0];
                PointF p2 = vertices[1];
                PointF p3 = vertices[2];
                PointF p4 = vertices[3];

                if (p2.DistanceSquared(p1) < p2.DistanceSquared(p3))
                {
                    //p1 and p2 are paired, p3 and p4 are paired
                    initialLines[0] = new LineSegment2DF(p1, p2);
                    initialLines[1] = new LineSegment2DF(p3, p4);
                }
                else
                {
                    //p2 and p3 are paired, p1 and p4 are paired
                    initialLines[0] = new LineSegment2DF(p2, p3);
                    initialLines[1] = new LineSegment2DF(p1, p4);
                }
            }
            else
            {
                //Use one of the image sides for intial detection
                initialLines[0] = new LineSegment2DF(new PointF(0, 0), new PointF(0, img.Height - 1));
                initialLines[1] = new LineSegment2DF(new PointF(img.Width - 1, 0), new PointF(img.Width - 1, img.Height - 1));
            }

            //Find closest line segment to initial line
            double minDistance = double.MaxValue;

            LineSegment2D? targetLine = null;
            foreach (var line in lines)
            {
                double minDistance1 = MathExtension.MinDistanceFromLineToPoint(initialLines[0].P1, initialLines[0].P2, line.P1);
                double minDistance2 = MathExtension.MinDistanceFromLineToPoint(initialLines[0].P1, initialLines[0].P2, line.P2);

                double currentDist = minDistance1 < minDistance2 ? minDistance1 : minDistance2;

                if (currentDist < minDistance)
                {
                    minDistance = currentDist;
                    targetLine = line;
                }
            }
            List<LineSegment2D> previousLines = new List<LineSegment2D>();

            //We have our target line, try to traverse to the other side
            LineSegment2D? nextLine = null;
            
            Image<Bgr, Byte> moddedImage = img.Convert<Bgr, Byte>();
            if (targetLine.HasValue)
            {
                previousLines.Add(targetLine.Value);

                //We have a starting position, lets test it!
                moddedImage.Draw(targetLine.Value, new Bgr(Color.Red), 2);
            }

            do
            {
                GetValue(lines, initialLines[1], previousLines.ToArray(), targetLine, ref nextLine);

                if (nextLine.HasValue)
                {
                    targetLine = nextLine;
                    previousLines.Add(nextLine.Value);
                    moddedImage.Draw(nextLine.Value, new Bgr(Color.Red), 2);
                }
            }
            while (nextLine.HasValue);
            
            DisplayImage3 = ImageService.ToBitmapSource(moddedImage);
        }

        private Random Random
        {
            get;
            set;
        }

        private Color GetRandomColor()
        {
            int r = Random.Next(0, 255);
            int g = Random.Next(0, 255);
            int b = Random.Next(0, 255);

            return Color.FromArgb(r, g, b);
        }

        private void GetValue(IEnumerable<LineSegment2D> lines, LineSegment2DF endLine, LineSegment2D[] previousLines, LineSegment2D? targetLine, ref LineSegment2D? nextLine)
        {
            const double minDistanceThreshold = 20;
            double minDistanceToEnd = double.MaxValue;
            nextLine = null;
            //targetPoint = null;
            foreach (var line in lines)
            {
                if (previousLines.Contains(line))
                {
                    continue;
                }

                if (line.P1 == targetLine.Value.P1 && line.P2 == targetLine.Value.P2)
                {
                    continue;
                }

                double dist1 = MathExtension.FindDistanceBetweenSegments(targetLine.Value.P1, targetLine.Value.P2, line.P1, line.P2);

                double minDistToEnd = MathExtension.FindDistanceBetweenSegments(line.P1, line.P2, endLine.P1, endLine.P2);
                double currentDistToEnd = MathExtension.FindDistanceBetweenSegments(targetLine.Value.P1, targetLine.Value.P2, endLine.P1, endLine.P2);

                if (dist1 < minDistanceThreshold && minDistToEnd < currentDistToEnd && minDistToEnd < minDistanceToEnd)
                {
                    minDistanceToEnd = minDistToEnd;
                    nextLine = line;
                }
            }
        }

        private void Save()
        {
            SaveImage.Save(@"G:\WhiskerImages\Gabor2\HoughJSI.png");
        }
    }
}
