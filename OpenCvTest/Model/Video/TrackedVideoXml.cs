using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Serialization;
using AutomatedRodentTracker.Model.Behaviours;
using AutomatedRodentTracker.Model.Boundries;
using AutomatedRodentTracker.Model.Resolver;
using AutomatedRodentTracker.Model.Results;
using AutomatedRodentTracker.Model.XmlClasses;
using AutomatedRodentTracker.ModelInterface.Results;
using AutomatedRodentTracker.ModelInterface.Video;

namespace AutomatedRodentTracker.Model.Video
{
    public class TrackedVideoXml
    {
        [XmlElement(ElementName = "FileName")]
        public string FileName
        {
            get;
            set;
        }

        [XmlElement(ElementName = "HeadPoints")]
        public DictionaryXml<int, SingleFrameResultXml> Results
        {
            get;
            set;
        }
        
        [XmlArray(ElementName = "MotionTrack")]
        [XmlArrayItem(ElementName = "Point")]
        public PointFXml[] MotionTrack
        {
            get;
            set;
        }

        [XmlArray(ElementName = "SmoothedMotionTrack")]
        [XmlArrayItem(ElementName = "Point")]
        public PointFXml[] SmoothedMotionTrack
        {
            get;
            set;
        }

        [XmlArray(ElementName = "OrientationTrack")]
        [XmlArrayItem(ElementName = "Direction")]
        public VectorXml[] OrientationTrack
        {
            get;
            set;
        }

        [XmlArray(ElementName = "Boundries")]
        [XmlArrayItem(ElementName = "Boundary")]
        public BoundaryBaseXml[] Boundries
        {
            get;
            set;
        }

        [XmlArray(ElementName = "Events")]
        [XmlArrayItem(ElementName = "Event")]
        public BehaviourHolderXml[] Events
        {
            get;
            set;
        }

        [XmlElement(ElementName = "InteractingBoundries")]
        public DictionaryXml<BoundaryBaseXml, BehaviourHolderXml[]> InteractingBoundries
        {
            get;
            set;
        }

        [XmlElement(ElementName = "MinInteractionDistance")]
        public double MinInteractionDistance
        {
            get;
            set;
        }

        [XmlElement(ElementName = "GapDistance")]
        public double GapDistance
        {
            get;
            set;
        }

        [XmlElement(ElementName = "ThresholdValue")]
        public int ThresholdValue
        {
            get;
            set;
        }

        [XmlElement(ElementName = "ThresholdValue2")]
        public int ThresholdValue2
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Result")]
        public SingleFileResult Result
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Message")]
        public string Message
        {
            get;
            set;
        }

        [XmlElement(ElementName = "StartFrame")]
        public int StartFrame
        {
            get;
            set;
        }

        [XmlElement(ElementName = "EndFrame")]
        public int EndFrame
        {
            get;
            set;
        }

        [XmlElement(ElementName = "FrameRate")]
        public double FrameRate
        {
            get;
            set;
        }

        [XmlElement(ElementName = "SmoothMotion")]
        public bool SmoothMotion
        {
            get;
            set;
        }

        [XmlElement(ElementName="SmoothFactor")]
        public double SmoothFactor
        {
            get;
            set;
        }

        [XmlElement(ElementName = "CentroidWidth")]
        public double CentroidSize
        {
            get;
            set;
        }

        [XmlElement(ElementName = "PelvicArea1")]
        public double PelvicArea1
        {
            get;
            set;
        }

        [XmlElement(ElementName = "PelvicArea2")]
        public double PelvicArea2
        {
            get;
            set;
        }

        [XmlElement(ElementName = "PelvicArea3")]
        public double PelvicArea3
        {
            get;
            set;
        }

        [XmlElement(ElementName = "PelvicArea4")]
        public double PelvicArea4
        {
            get;
            set;
        }

        [XmlElement(ElementName="UnitsToMilimeters")]
        public double UnitsToMilimeters
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Roi")]
        public RectangleXml ROI
        {
            get;
            set;
        }

        public TrackedVideoXml()
        {
            
        }

        public TrackedVideoXml(string fileName, SingleFileResult result, DictionaryXml<int, SingleFrameResultXml> results, PointFXml[] motionTrack, PointFXml[] smoothedMotionTrack, VectorXml[] orientationTrack, BoundaryBaseXml[] boundries, BehaviourHolderXml[] events, DictionaryXml<BoundaryBaseXml, BehaviourHolderXml[]> interactingBoundries, double minInteractionDistance, double gapDistance, int thresholdValue, int thresholdValue2, int startFrame, int endFrame, double frameRate, bool smoothMotion, double smoothingFactor, double centroidSize, double pelvicArea1, double pelvicArea2, double pelvicArea3, double pelvicArea4, RectangleXml roi, string message = "")
        {
            FileName = fileName;
            Result = result;
            Results = results;
            MotionTrack = motionTrack;
            SmoothedMotionTrack = smoothedMotionTrack;
            OrientationTrack = orientationTrack;
            Boundries = boundries;
            Events = events;
            InteractingBoundries = interactingBoundries;
            MinInteractionDistance = minInteractionDistance;
            GapDistance = gapDistance;
            ThresholdValue = thresholdValue;
            ThresholdValue2 = thresholdValue2;
            StartFrame = startFrame;
            EndFrame = endFrame;
            SmoothMotion = smoothMotion;
            SmoothFactor = smoothingFactor;
            FrameRate = frameRate;
            CentroidSize = centroidSize;
            PelvicArea1 = pelvicArea1;
            PelvicArea2 = pelvicArea2;
            PelvicArea3 = pelvicArea3;
            PelvicArea4 = pelvicArea4;
            ROI = roi;
            Message = message;
        }

        public ITrackedVideo GetData()
        {
            ITrackedVideo trackedVideo = ModelResolver.Resolve<ITrackedVideo>();

            trackedVideo.FileName = FileName;
            trackedVideo.Result = Result;
            trackedVideo.SmoothMotion = SmoothMotion;
            //trackedVideo.SmoothFactor = SmoothFactor;
            trackedVideo.FrameRate = FrameRate;
            trackedVideo.CentroidSize = CentroidSize;
            trackedVideo.PelvicArea1 = PelvicArea1;
            trackedVideo.PelvicArea2 = PelvicArea2;
            trackedVideo.PelvicArea3 = PelvicArea3;
            trackedVideo.PelvicArea4 = PelvicArea4;
            trackedVideo.Message = Message;

            if (Results != null)
            {
                Dictionary<int, SingleFrameResultXml> headPoints = Results.GetData();
                Dictionary<int, ISingleFrameResult> finalResults = new Dictionary<int, ISingleFrameResult>();

                foreach (var entry in headPoints)
                {
                    int key = entry.Key;
                    SingleFrameResultXml currentEntry = entry.Value;

                    if (currentEntry == null)
                    {
                        finalResults.Add(key, null);
                    }
                    else
                    {
                        finalResults.Add(key, currentEntry.GetData());
                    }
                }

                trackedVideo.Results = finalResults;
            }

            //if (WaistSizes != null)
            //{
            //    trackedVideo.WaistSizes = WaistSizes.GetData();
            //}

            if (MotionTrack != null)
            {
                trackedVideo.MotionTrack = MotionTrack.Select(x => x.GetPoint()).ToArray();
            }

            if (SmoothedMotionTrack != null)
            {
                trackedVideo.SmoothedMotionTrack = SmoothedMotionTrack.Select(x => x.GetPoint()).ToArray();
            }

            if (OrientationTrack != null)
            {
                trackedVideo.OrientationTrack = OrientationTrack.Select(x => x.GetVector()).ToArray(); 
            }

            if (Boundries != null)
            {
                trackedVideo.Boundries = Boundries.Select(x => x.GetBoundary()).ToArray();
            }

            if (Events != null)
            {
                trackedVideo.Events = Events.Select(x => x.GetData()).ToArray();
            }

            if (InteractingBoundries != null)
            {
                trackedVideo.InteractingBoundries = InteractingBoundries.GetData().ToDictionary(kvp => kvp.Key.GetBoundary(), kvp => kvp.Value.Select(x => x.GetData()).ToArray());
            }
            
            trackedVideo.MinInteractionDistance = MinInteractionDistance;
            trackedVideo.GapDistance = GapDistance;
            trackedVideo.ThresholdValue = ThresholdValue;
            trackedVideo.ThresholdValue2 = ThresholdValue2;
            trackedVideo.StartFrame = StartFrame;
            trackedVideo.EndFrame = EndFrame;
            trackedVideo.UnitsToMilimeters = UnitsToMilimeters;

            trackedVideo.UpdateTrack();
            return trackedVideo;
        }
    }
}
