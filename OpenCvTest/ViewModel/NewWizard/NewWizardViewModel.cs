using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using AutomatedRodentTracker.Commands;
using AutomatedRodentTracker.Extensions;
using AutomatedRodentTracker.Model.Boundries;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.Model.XmlClasses;
using AutomatedRodentTracker.ModelInterface.Boundries;
using AutomatedRodentTracker.Services.Mouse;
using AutomatedRodentTracker.Services.RBSK;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using AutomatedRodentTracker.ModelInterface.Video;
using AutomatedRodentTracker.ModelInterface.VideoSettings;
using AutomatedRodentTracker.Services.ImageToBitmapSource;
using AutomatedRodentTracker.View.Cropping;
using AutomatedRodentTracker.ViewModel.Cropping;
using Microsoft.Office.Core;
using Point = System.Drawing.Point;

namespace AutomatedRodentTracker.ViewModel.NewWizard
{
    public class NewWizardViewModel : WindowViewModelBase
    {
        private ActionCommand m_OkCommand;
        private ActionCommand m_PreviewCommand;
        private ActionCommand m_CancelCommand;
        private ActionCommand m_ResetCommand;
        private ActionCommand m_SetRoiCommand;
        private ActionCommand m_RemoveRoiCommand;
        
        private bool m_ShowBinary = true;
        private bool m_ShowBoundries = false;
        private bool m_PreviewGenerated = false;

        private int m_FrameNumber = -1;
        private int m_MotionLength = 100;
        private int m_MaxFrame;
        private Rectangle m_Roi = Rectangle.Empty;

        public bool ShowBinary
        {
            get
            {
                return m_ShowBinary;
            }
            set
            {
                if (Equals(m_ShowBinary, value))
                {
                    return;
                }

                m_ShowBinary = value;

                NotifyPropertyChanged();
            }
        }

        public bool ShowBoundries
        {
            get
            {
                return m_ShowBoundries;
            }
            set
            {
                if (Equals(m_ShowBoundries, value))
                {
                    return;
                }

                m_ShowBoundries = value;

                NotifyPropertyChanged();
            }
        }

        public bool PreviewGenerated
        {
            get
            {
                return m_PreviewGenerated;
            }
            set
            {
                if (Equals(m_PreviewGenerated, value))
                {
                    return;
                }

                m_PreviewGenerated = value;

                NotifyPropertyChanged();
                OkCommand.RaiseCanExecuteChangedNotification();
            }
        }

        public int MotionLength
        {
            get
            {
                return m_MotionLength;
            }
            set
            {
                if (Equals(m_MotionLength, value))
                {
                    return;
                }

                m_MotionLength = value;

                NotifyPropertyChanged();
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
                UpdateFrameNumber();
                Console.WriteLine(FrameNumber);

                NotifyPropertyChanged();
            }
        }

       public int MaxFrame
        {
            get
            {
                return m_MaxFrame;
            }
            set
            {
                if (Equals(m_MaxFrame, value))
                {
                    return;
                }

                m_MaxFrame = value;

                NotifyPropertyChanged();
            }
        }

        public Rectangle Roi
        {
            get
            {
                return m_Roi;
            }
            set
            {
                if (Equals(m_Roi, value))
                {
                    return;
                }

                m_Roi = value;
                UpdateRoi();

                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<BoundaryBaseViewModel> m_Boundries; 
        public ObservableCollection<BoundaryBaseViewModel> Boundries
        {
            get
            {
                return m_Boundries;
            }
            set
            {
                if (ReferenceEquals(m_Boundries, value))
                {
                    return;
                }

                m_Boundries = value;

                NotifyPropertyChanged();
            }
        } 
           

        public ActionCommand OkCommand
        {
            get
            {
                return m_OkCommand ?? (m_OkCommand = new ActionCommand()
                {
                    ExecuteAction = Ok,
                    CanExecuteAction = CanOk,
                });
            }
        }

        public ActionCommand PreviewCommand
        {
            get
            {
                return m_PreviewCommand ?? (m_PreviewCommand = new ActionCommand()
                {
                    ExecuteAction = Preview,
                    CanExecuteAction = CanPreview,
                });
            }
        }

        public ActionCommand CancelCommand
        {
            get
            {
                return m_CancelCommand ?? (m_CancelCommand = new ActionCommand()
                {
                    ExecuteAction = Cancel,
                });
            }
        }

        public ActionCommand ResetCommand
        {
            get
            {
                return m_ResetCommand ?? (m_ResetCommand = new ActionCommand()
                {
                    ExecuteAction = Reset,
                });
            }
        }

        public ActionCommand SetRoiCommand
        {
            get
            {
                return m_SetRoiCommand ?? (m_SetRoiCommand = new ActionCommand()
                {
                    ExecuteAction = SetRoi,
                });
            }
        }

        public ActionCommand RemoveRoiCommand
        {
            get
            {
                return m_RemoveRoiCommand ?? (m_RemoveRoiCommand = new ActionCommand()
                {
                    ExecuteAction = RemoveRoi,
                    CanExecuteAction = CanRemoveRoi,
                });
            }
        }

        private IVideo m_Video;
        public IVideo Video
        {
            get
            {
                return m_Video;
            }
            set
            {
                if (Equals(m_Video, value))
                {
                    return;
                }

                m_Video = value;

                NotifyPropertyChanged();
            }
        }

        private IVideoSettings m_VideoSettings;
        public IVideoSettings VideoSettings
        {
            get
            {
                return m_VideoSettings;
            }
            set
            {
                if (Equals(m_VideoSettings, value))
                {
                    return;
                }

                m_VideoSettings = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged("ThresholdValue");
                NotifyPropertyChanged("MaxThreshold");
                NotifyPropertyChanged("MinInteractionDistance");
            }
        }

        public int ThresholdValue
        {
            get
            {
                return VideoSettings.ThresholdValue;
            }
            set
            {
                if (Equals(VideoSettings.ThresholdValue, value))
                {
                    return;
                }

                VideoSettings.ThresholdValue = value;

                NotifyPropertyChanged();
                ProcessFrame();
                //Console.WriteLine(ThresholdValue);
            }
        }

        public double MaxThreshold
        {
            get
            {
                return VideoSettings.MaxThreshold;
            }
            set
            {
                if (Equals(VideoSettings.MaxThreshold, value))
                {
                    return;
                }

                VideoSettings.MaxThreshold = value;

                NotifyPropertyChanged();
            }
        }

        public double InteractionValue
        {
            get
            {
                return VideoSettings.MinimumInteractionDistance;
            }
            set
            {
                if (Equals(VideoSettings.MinimumInteractionDistance))
                {
                    return;
                }

                VideoSettings.MinimumInteractionDistance = value;

                NotifyPropertyChanged();
            }
        }

        private double m_GapDistance;
        public double GapDistance
        {
            get
            {
                //return VideoSettings.GapDistance;
                return m_GapDistance;
            }
            set
            {
                if (Equals(m_GapDistance, value))
                {
                    return;
                }

                m_GapDistance = value;

                NotifyPropertyChanged();

                UpdateGapDistance();

                //if (Equals(VideoSettings.GapDistance))
                //{
                //    return;
                //}

                //VideoSettings.GapDistance = value;

                //NotifyPropertyChanged();
            }
        }

        public int ThresholdValue2
        {
            get
            {
                return VideoSettings.ThresholdValue2;
            }
            set
            {
                if (Equals(VideoSettings.ThresholdValue2, value))
                {
                    return;
                }

                VideoSettings.ThresholdValue2 = value;

                NotifyPropertyChanged();
            }
        }

        private BitmapSource m_DisplayImage;
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

        private Image<Bgr, Byte> m_OriginalImage;
        public Image<Bgr, Byte> OriginalImage
        {
            get
            {
                return m_OriginalImage;
            }
            set
            {
                if (ReferenceEquals(m_OriginalImage, value))
                {
                    return;
                }

                m_OriginalImage = value;

                NotifyPropertyChanged();
            }
        }

        private Image<Gray, Byte> m_BinaryBackground;
        public Image<Gray, Byte> BinaryBackground
        {
            get
            {
                return m_BinaryBackground;
            }
            set
            {
                if (ReferenceEquals(m_BinaryBackground, value))
                {
                    return;
                }

                m_BinaryBackground = value;

                NotifyPropertyChanged();
            }
        }  

        public NewWizardViewModel(string fileName)
        {
            VideoSettings = ModelResolver.Resolve<IVideoSettings>();
            VideoSettings.FileName = fileName;
            Video = ModelResolver.Resolve<IVideo>();
            Video.SetVideo(fileName);
            MaxFrame = Video.FrameCount - 1;
            FrameNumber = 0;
            //if (fileName.StartsWith(@"C:\Users\10488835\Desktop\PhD\Datasets\100722 -d"))
            //{
                //ThresholdValue = 20;
            //}
            //else
            //{
                ThresholdValue = 60;
            //}
           
            InteractionValue = 15;
            GapDistance = 60;
            ThresholdValue2 = 10;
            using (Image<Gray, Byte> tempImage = Video.GetGrayFrameImage())
            {
                ThresholdValue = (int)CalculateOtsu(tempImage);
            }

        }

        private double CalculateOtsu(Image<Gray, Byte> image)
        {
            using (Image<Gray, Byte> tempOriginal = image.CopyBlank())
            {
                return CvInvoke.Threshold(image, tempOriginal, 0, 255, ThresholdType.Otsu | ThresholdType.Binary);
            }
        }

        private void ProcessFrame()
        {
            using (Image<Gray, Byte> displayImage = OriginalImage.Convert<Gray, Byte>())
            using (Image<Gray, Byte> binaryimage = displayImage.ThresholdBinary(new Gray(ThresholdValue), new Gray(255)))
            {
                DisplayImage = ImageService.ToBitmapSource(binaryimage);
            }
            PreviewGenerated = false;
            Boundries = new ObservableCollection<BoundaryBaseViewModel>();
        }

        private void UpdateGapDistance()
        {
            RBSK rbsk = MouseService.GetStandardMouseRules();
            rbsk.Settings.GapDistance = GapDistance;
            rbsk.Settings.BinaryThreshold = ThresholdValue;

            PointF[] result = RBSKService.RBSK(OriginalImage, rbsk);

            if (result == null || !result.Any())
            {
                DisplayImage = ImageService.ToBitmapSource(OriginalImage);
                return;
            }

            using (Image<Gray, Byte> displayImage = OriginalImage.Convert<Gray, Byte>())
            using (Image<Gray, Byte> binaryimage = displayImage.ThresholdBinary(new Gray(ThresholdValue), new Gray(255)))
            using (Image<Bgr, byte> drawImage = binaryimage.Convert<Bgr, byte>())
            {
                foreach (PointF point in result)
                {
                    drawImage.Draw(new CircleF(point, 2), new Bgr(Color.Yellow), 2);
                }

                DisplayImage = ImageService.ToBitmapSource(drawImage);
            }

            PreviewGenerated = false;
            Boundries = new ObservableCollection<BoundaryBaseViewModel>();
            Console.WriteLine(GapDistance);
        }

        public void Ok()
        {
            ExitResult = WindowExitResult.Ok;
            CloseWindow();
        }

        private bool CanOk()
        {
            return PreviewGenerated;
        }

        private void Cancel()
        {
            ExitResult = WindowExitResult.Cancel;
            CloseWindow();
        }

        private void Reset()
        {
            ThresholdValue = 120;
        }

        protected override void WindowClosing(CancelEventArgs closingEventArgs)
        {
            if (ExitResult != WindowExitResult.Ok)
            {
                ExitResult = WindowExitResult.Cancel;
            }
        }

        public void Preview()
        {
            if (FrameNumber + MotionLength > MaxFrame)
            {
                //Motion length too long
                MessageBox.Show("Motion length extends beyond the length of the video, resetting to last possible location");
                FrameNumber = MaxFrame - MotionLength;
            }

            Image<Gray, Byte> binaryBackground;
            IEnumerable<IBoundaryBase> boundaries;
            VideoSettings.GeneratePreview(Video, out binaryBackground, out boundaries, MotionLength, FrameNumber);
            //string fileName = Path.GetFileNameWithoutExtension(Video.FilePath);
            //binaryBackground.Save(string.Format(@"D:\Vids\New folder\Vids\Images\{0}-Binary.png", fileName));
            BinaryBackground = binaryBackground;
            //Video.SetFrame(100);
            ObservableCollection<BoundaryBaseViewModel> boundries = new ObservableCollection<BoundaryBaseViewModel>();

            int boxId = 1;
            foreach (RotatedRect rect in VideoSettings.Boxes)
            {
                IBoxBoundary boxModel = ModelResolver.Resolve<IBoxBoundary>();
                boxModel.Id = boxId;
                boxModel.Points = rect.GetVertices().Select(x => x.ToPoint()).ToArray();
                BoxesBoundary box = new BoxesBoundary(boxModel, rect);
                box.EnabledChanged += EnabledChanged;
                boundries.Add(box);
                boxId++;
            }

            int artefactId = 1;
            foreach (Point[] points in VideoSettings.Artefacts)
            {
                IArtefactsBoundary artefactModel = ModelResolver.Resolve<IArtefactsBoundary>();
                artefactModel.Id = artefactId;
                artefactModel.Points = points;
                ArtefactsBoundaryViewModel artefact = new ArtefactsBoundaryViewModel(artefactModel, points);
                artefact.EnabledChanged += EnabledChanged;
                boundries.Add(artefact);
                artefactId++;
            }

            int boundaryId = 1;
            foreach (Point[] points in VideoSettings.Boundries)
            {
                IOuterBoundary outerModel = ModelResolver.Resolve<IOuterBoundary>();
                outerModel.Id = boundaryId;
                outerModel.Points = points;
                OuterBoundaryViewModel outerBoundary = new OuterBoundaryViewModel(outerModel, points);
                outerBoundary.EnabledChanged += EnabledChanged;
                boundries.Add(outerBoundary);
                boundaryId++;
            }

            Boundries = boundries;

            ShowBoundries = true;
            UpdateDisplayImage();
            PreviewGenerated = true;
        }

        private void UpdateDisplayImage()
        {
            Video.SetFrame(FrameNumber);

            using (Image<Bgr, Byte> displayImage = Video.GetFrameImage())
            {
                displayImage.ROI = Roi;
                using (Image<Gray, Byte> grayImage = displayImage.Convert<Gray, Byte>())
                using (Image<Gray, Byte> binaryImage = grayImage.ThresholdBinary(new Gray(VideoSettings.ThresholdValue), new Gray(255)))
                using (Image<Gray, Byte> finalMouseImage = binaryImage.AbsDiff(BinaryBackground))
                {
                    Point[] mousePoints = null;
                    using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                    {
                        CvInvoke.FindContours(finalMouseImage, contours, null, RetrType.External, ChainApproxMethod.ChainApproxNone);

                        int count = contours.Size;
                        double maxArea = 0;
                        for (int j = 0; j < count; j++)
                        {
                            using (VectorOfPoint contour = contours[j])
                            {
                                double contourArea = CvInvoke.ContourArea(contour);
                                if (contourArea >= maxArea)
                                {
                                    maxArea = contourArea;
                                    mousePoints = contour.ToArray();
                                }
                            }
                        }
                    }

                    displayImage.DrawPolyline(mousePoints, true, new Bgr(Color.Yellow), 2);

                    //foreach (var boundary in VideoSettings.Boundries)
                    //{
                    //    displayImage.DrawPolyline(VideoSettings.Boundries.ToArray(), true, new Bgr(Color.Red), 2);
                    //}
                    

                    foreach (BoundaryBaseViewModel boundry in Boundries)
                    {
                        if (boundry.Enabled)
                        {
                            displayImage.DrawPolyline(boundry.Points, true, boundry.Color, 2);
                        }
                    }

                    displayImage.ROI = Roi;

                    DisplayImage = ImageService.ToBitmapSource(displayImage);
                    //displayImage.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\5-Result.png");
                }
            }
        }

        private ActionCommand m_TestCommand;
        public ActionCommand TestCommand
        {
            get
            {
                return m_TestCommand ?? (m_TestCommand = new ActionCommand()
                {
                    ExecuteAction = Test
                });
            }
        }

        private void Test()
        {
            OriginalImage.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\1-Original.png");
            using (Image<Gray, Byte> displayImage = OriginalImage.Convert<Gray, Byte>())
            using (Image<Gray, Byte> binaryimage = displayImage.ThresholdBinary(new Gray(ThresholdValue), new Gray(255)))
            {
                binaryimage.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\2-BinaryImage.png");

                Image<Gray, Byte> binaryBackground;
                IEnumerable<IBoundaryBase> boundaries;
                VideoSettings.GeneratePreview(Video, out binaryBackground, out boundaries);

                binaryBackground.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\3-BinaryBackground.png");

                using (Image<Gray, Byte> subbed = binaryBackground.AbsDiff(binaryimage))
                using (Image<Gray, Byte> not = binaryBackground.Not())
                using (Image<Gray, Byte> added = binaryimage.Add(not))
                {
                    added.Save(@"C:\Users\10488835\Desktop\PhD\Papers\Software Practise and Experience\Latex\images\BackgroundSub\4-BinaryAdded.png");
                }
            }
        }

        private bool CanPreview()
        {
            return true;
        }

        private void EnabledChanged(object sender, EventArgs e)
        {
            UpdateDisplayImage();
        }

        private void SetRoi()
        {
            CropImageViewModel viewModel = new CropImageViewModel();
            viewModel.DisplayImage = ImageService.ToBitmapSource(OriginalImage);
            viewModel.AssignRegionOfInterestToValues(new Rect(Roi.X, Roi.Y, Roi.Width, Roi.Height));
            CropImageView view = new CropImageView();
            view.DataContext = viewModel;
            view.ShowDialog();

            if (viewModel.ExitResult != WindowExitResult.Ok)
            {
                return;
            }

            Rect roiRect = viewModel.GetRoi();
            if (roiRect.Width == 0 || roiRect.Height == 0)
            {
                Roi = Rectangle.Empty;
                return;
            }

            Roi = new Rectangle((int)roiRect.X, (int)roiRect.Y, (int)roiRect.Width, (int)roiRect.Height);
            VideoSettings.Roi = Roi;
        }

        private void RemoveRoi()
        {
            Roi = Rectangle.Empty;
        }

        private bool CanRemoveRoi()
        {
            return !Roi.IsEmpty;
        }

        private void UpdateRoi()
        {
            if (OriginalImage == null)
            {
                return;
            }

            OriginalImage.ROI = Roi;
            RemoveRoiCommand.RaiseCanExecuteChangedNotification();
            ProcessFrame();
        }

        private void UpdateFrameNumber()
        {
            Video.SetFrame(m_FrameNumber);
            OriginalImage = Video.GetFrameImage();
            OriginalImage.ROI = Roi;
            ProcessFrame();
        }
    }

    public abstract class BoundaryBaseViewModel : ViewModelBase
    {
        private bool m_Enabled = true;
        public int Id { get; set; }
        public Bgr Color { get; set; }
        public Point[] Points { get; set; }
        public string Name { get; set; }
        
        public event EventHandler EnabledChanged;
        private IBoundaryBase m_Model;

        public IBoundaryBase Model
        {
            get
            {
                return m_Model;
            }
            set
            {
                if (Equals(m_Model, value))
                {
                    return;
                }

                m_Model = value;

                NotifyPropertyChanged();
            }
        }

        public bool Enabled
        {
            get
            {
                return m_Enabled;
            }
            set
            {
                if (Equals(m_Enabled, value))
                {
                    return;
                }

                m_Enabled = value;
                if (EnabledChanged != null)
                {
                    EnabledChanged(this, EventArgs.Empty);
                }

                NotifyPropertyChanged();
            }
        }

        protected BoundaryBaseViewModel(IBoundaryBase model)
        {
            Model = model;
            Id = Model.Id;
        }

        public static BoundaryBaseViewModel GetBoundaryFromModel(IBoundaryBase model)
        {
            IArtefactsBoundary artefactModel = model as IArtefactsBoundary;
            if (artefactModel != null)
            {
                return new ArtefactsBoundaryViewModel(artefactModel, model.Points);
            }

            IBoxBoundary boxModel = model as IBoxBoundary;
            if (model is IBoxBoundary)
            {
                return new BoxesBoundary(boxModel, model.Points);
            }

            ICircleBoundary circleModel = model as ICircleBoundary;
            if (model is ICircleBoundary)
            {
                return new CircularBoundary(circleModel, model.Points);
            }

            IOuterBoundary outerModel = model as IOuterBoundary;
            if (model is IOuterBoundary)
            {
                return new OuterBoundaryViewModel(outerModel, model.Points);
            }

            return null;
        }

        protected double FindDistanceSquaredToSegment(PointF pt, PointF p1, PointF p2)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return (dx * dx) + (dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                PointF closest = new PointF(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return (dx * dx) + (dy * dy);
        }

        public double GetMinimumDistance(PointF point)
        {
            double currentMin = double.MaxValue;

            for (int i = 0; i < Points.Length; i++)
            {
                int prevIndex = i - 1;
                if (prevIndex < 0)
                {
                    prevIndex = Points.Length - 1;
                }

                double min = FindDistanceSquaredToSegment(point, Points[prevIndex], Points[i]);
                if (min < currentMin)
                {
                    currentMin = min;
                }
            }

            return Math.Sqrt(currentMin);
        }
    }

    public class ArtefactsBoundaryViewModel : BoundaryBaseViewModel
    {
        public ArtefactsBoundaryViewModel(IArtefactsBoundary model, Point[] points) : base(model)
        {
            Points = points;
            Color = new Bgr(System.Drawing.Color.LightGreen);
            Name = "Artefact - " + Id;
        }
    }

    public class BoxesBoundary : BoundaryBaseViewModel
    {
        public BoxesBoundary(IBoxBoundary model, RotatedRect rect) : base(model)
        {
            Points = rect.GetVertices().Select(x => new Point((int)x.X, (int)x.Y)).ToArray();
            Color = new Bgr(System.Drawing.Color.Aqua);
            Name = "Box - " + Id;
        }

        public BoxesBoundary(IBoxBoundary model, Point[] points) : base(model)
        {
            Points = points;
            Color = new Bgr(System.Drawing.Color.Aqua);
            Name = "Box - " + Id;
        }
    }

    public class CircularBoundary : BoundaryBaseViewModel
    {
        private const int CircularEdges = 20;
        private double deltaAngle = 2*Math.PI/CircularEdges;
        public CircularBoundary(ICircleBoundary model, CircleF circle) : base(model)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < CircularEdges; i++)
            {
                Point point = new Point();
                point.X = (int)(circle.Center.X + (circle.Radius*Math.Sin(i*deltaAngle)));
                point.Y = (int)(circle.Center.X + (circle.Radius * Math.Cos(i * deltaAngle)));
                points.Add(point);
            }

            Points = points.ToArray();
            Color = new Bgr(System.Drawing.Color.Lime);
            Name = "Circle - " + Id;
        }

        public CircularBoundary(ICircleBoundary model, Point[] points) : base(model)
        {
            Points = points;
            Color = new Bgr(System.Drawing.Color.Lime);
            Name = "Circle - " + Id;
        }
    }

    public class OuterBoundaryViewModel : BoundaryBaseViewModel
    {
        public OuterBoundaryViewModel(IOuterBoundary model, Point[] points) : base(model)
        {
            Points = points;
            Color = new Bgr(System.Drawing.Color.Red);
            Name = "Outer Boundary - " + Id;
        }
    }
}
