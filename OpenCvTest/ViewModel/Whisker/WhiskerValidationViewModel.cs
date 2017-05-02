using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using AutomatedRodentTracker.Commands;
using AutomatedRodentTracker.Extensions;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.ModelInterface.BodyDetection;
using AutomatedRodentTracker.Services.FileBrowser;
using AutomatedRodentTracker.Services.ImageToBitmapSource;
using AutomatedRodentTracker.Services.Mouse;
using AutomatedRodentTracker.Services.RBSK;
using Emgu.CV.Structure;
using Emgu.CV;

namespace AutomatedRodentTracker.ViewModel.Whisker
{
    public class WhiskerValidationViewModel : WindowViewModelBase
    {
        private BitmapSource m_Image1;
        public BitmapSource Image1
        {
            get
            {
                return m_Image1;
            }

            set
            {
                if (Equals(m_Image1, value))
                {
                    return;
                }

                m_Image1 = value;

                NotifyPropertyChanged();
            }
        }
        
        private BitmapSource m_Image2;
        public BitmapSource Image2
        {
            get
            {
                return m_Image2;
            }

            set
            {
                if (Equals(m_Image2, value))
                {
                    return;
                }

                m_Image2 = value;

                NotifyPropertyChanged();
            }
        }

        private BitmapSource m_Image3;
        public BitmapSource Image3
        {
            get
            {
                return m_Image3;
            }

            set
            {
                if (Equals(m_Image3, value))
                {
                    return;
                }

                m_Image3 = value;

                NotifyPropertyChanged();
            }
        }
        

        private int m_SliderValue;
        public int SliderValue
        {
            get
            {
                return m_SliderValue;
            }

            set
            {
                if (Equals(m_SliderValue, value))
                {
                    return;
                }

                m_SliderValue = value;

                NotifyPropertyChanged();
                UpdateBinaryImage();
            }
        }


        private string m_ImageLocation;
        public string ImageLocation
        {
            get
            {
                return m_ImageLocation;
            }

            set
            {
                if (Equals(m_ImageLocation, value))
                {
                    return;
                }

                m_ImageLocation = value;

                NotifyPropertyChanged();
            }
        }

        private string m_BackgroundLocation;
        public string BackgroundLocation
        {
            get
            {
                return m_BackgroundLocation;
            }

            set
            {
                if (Equals(m_BackgroundLocation, value))
                {
                    return;
                }

                m_BackgroundLocation = value;

                NotifyPropertyChanged();
            }
        }
        
        private ActionCommand m_OpenCommand;
        public ActionCommand OpenCommand
        {
            get
            {
                return m_OpenCommand ?? (m_OpenCommand = new ActionCommand()
                {
                    ExecuteAction = Open
                });
            }
        }

        private void Open()
        {
            string imageLocation = FileBrowser.BrowseForImageFiles();
            if (string.IsNullOrWhiteSpace(imageLocation))
            {
                ImageLocation = string.Empty;
                BackgroundLocation = string.Empty;
                return;
            }

            string bgImageLocation = FileBrowser.BrowseForImageFiles();
            if (string.IsNullOrWhiteSpace(bgImageLocation))
            {
                ImageLocation = string.Empty;
                BackgroundLocation = string.Empty;
                return;
            }

            ImageLocation = imageLocation;
            BackgroundLocation = bgImageLocation;

            ProcessFrame();
        }

        private void ProcessFrame()
        {
            if (string.IsNullOrWhiteSpace(ImageLocation))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(BackgroundLocation))
            {
                return;
            }

            RBSK rbsk = MouseService.GetStandardMouseRules();
            rbsk.Settings.BinaryThreshold = SliderValue;

            using (Image<Bgr, Byte> image = new Image<Bgr, byte>(ImageLocation))
            using (Image<Gray, Byte> backgroundImage = new Image<Gray, byte>(BackgroundLocation))
            using (Image<Gray, Byte> grayImage = image.Convert<Gray, Byte>())
            using (Image<Gray, Byte> filteredImage = grayImage.SmoothMedian(rbsk.Settings.FilterLevel))
            using (Image<Gray, Byte> binaryImage = filteredImage.ThresholdBinary(new Gray(rbsk.Settings.BinaryThreshold), new Gray(255)))
            using (Image<Gray, Byte> backgroundNot = backgroundImage.Not())
            using (Image<Gray, Byte> finalImage = binaryImage.Add(backgroundNot))
            //using (Image<Gray, Byte> finalImageNot = finalImage.Not())
            {
                double gapDistance = GetBestGapDistance(grayImage, rbsk);

                rbsk.Settings.GapDistance = gapDistance;
                
                PointF[] result = RBSKService.RBSK(finalImage, rbsk);

                Image1 = ImageService.ToBitmapSource(image);

                using (Image<Bgr, Byte> dImage = image.Clone())
                {
                    foreach (PointF point in result)
                    {
                        dImage.Draw(new CircleF(point, 3), new Bgr(Color.Yellow), 3);
                    }

                    Image3 = ImageService.ToBitmapSource(dImage);
                }
            }
        }

        private void UpdateBinaryImage()
        {
            RBSK rbsk = MouseService.GetStandardMouseRules();
            rbsk.Settings.BinaryThreshold = SliderValue;

            using (Image<Bgr, Byte> image = new Image<Bgr, byte>(ImageLocation))
            using (Image<Gray, Byte> grayImage = image.Convert<Gray, Byte>())
            using (Image<Gray, Byte> filteredImage = grayImage.SmoothMedian(rbsk.Settings.FilterLevel))
            using (Image<Gray, Byte> binaryImage = filteredImage.ThresholdBinary(new Gray(rbsk.Settings.BinaryThreshold), new Gray(255)))
            {
                Image2 = ImageService.ToBitmapSource(binaryImage);
            }
        }

        private double GetBestGapDistance(Image<Gray, Byte> firstFrame, Services.RBSK.RBSK rbsk, Action<double> progressCallBack = null)
        {
            //Caluclate gap distnace if it hasn't been set
            //Auto Find the gap distance
            //Scan from 20 - 300, the range which gives us consistent results is the right one
            int start = 20;
            int end = 300;
            int interval = 1;
            Dictionary<int, PointF> nosePoints = new Dictionary<int, PointF>();

            for (int i = start; i <= end; i += interval)
            {

                using (Image<Gray, Byte> filteredImage = firstFrame.SmoothMedian(5))
                using (Image<Gray, Byte> binaryImage = filteredImage.ThresholdBinary(new Gray(30), new Gray(255)))
                {
                    rbsk.Settings.GapDistance = i;
                    Point[] temp = null;
                    PointF[] mousePoints = RBSKService.RBSKParallel(binaryImage, rbsk, ref temp);

                    if (mousePoints != null)
                    {
                        //We've found a set of points for this gap distance, store it
                        nosePoints.Add(i, mousePoints[2]);
                    }
                }
                double progressValue = ((i - start) / ((double)end - start)) * 100;
                if (progressCallBack != null)
                {
                    progressCallBack(progressValue);
                }
            }

            const double threshold = 20;
            PointF? previousPoint = null;
            List<int> currentSelection = new List<int>();
            int bestCounter = 0;
            double bestDistanceSoFar = -1;

            foreach (KeyValuePair<int, PointF> kvp in nosePoints)
            {
                PointF currentPoint = kvp.Value;

                //Do we have a value?
                if (previousPoint.HasValue)
                {
                    //Is the previous point within the threshold distance of the current point
                    if (currentPoint.Distance(previousPoint.Value) < threshold)
                    {
                        currentSelection.Add(kvp.Key);
                        previousPoint = currentPoint;
                    }
                    else
                    {
                        //We're not within the threshold, compare the current list to see if it's the best
                        if (currentSelection.Count > bestCounter)
                        {
                            bestCounter = currentSelection.Count;
                            bestDistanceSoFar = currentSelection.Average();
                        }

                        currentSelection.Clear();
                        previousPoint = null;
                    }
                }
                else
                {
                    previousPoint = currentPoint;
                }
            }

            if (currentSelection.Count > bestCounter)
            {
                bestDistanceSoFar = currentSelection.Average();
            }

            if (bestDistanceSoFar == -1)
            {
                bestDistanceSoFar = 100;
            }

            return bestDistanceSoFar;
        }
    }
}
