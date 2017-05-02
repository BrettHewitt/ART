using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AutomatedRodentTracker.Model.Results;
using AutomatedRodentTracker.ModelInterface;
using AutomatedRodentTracker.ModelInterface.Behaviours;
using AutomatedRodentTracker.ModelInterface.Boundries;
using AutomatedRodentTracker.ModelInterface.Results;

namespace AutomatedRodentTracker.ModelInterface.Datasets
{
    public interface ISaveArtFile : IModelObjectBase
    {
        void SaveFile(string videoFileName, SingleFileResult result, Dictionary<int, ISingleFrameResult> headPoints, PointF[] motionTrack, PointF[] smoothedMotionTrack, Vector[] orientationTrack, IEnumerable<IBehaviourHolder> events, IEnumerable<IBoundaryBase> boundaries, Dictionary<IBoundaryBase, IBehaviourHolder[]> interactions, double minInteractionDistance, double gapDistance, int thresholdValue, int thresholdValue2, int startFrame, int endFrame, double frameRate, bool smoothMotion, double smoothingFactor, double centroidSize, double pelvicArea1, double pelvicArea2, double pelvicArea3, double pelvicArea4, Rectangle roi, string message = "");
        void SaveFile(string videoFileName, IMouseDataResult data);
        void SaveFile(string fileLocation, string videoFileName, IMouseDataResult data);
        void SaveFile(string fileLocation, string videoFileName, SingleFileResult result, Dictionary<int, ISingleFrameResult> headPoints, PointF[] motionTrack, PointF[] smoothedMotionTrack, Vector[] orientationTrack, IEnumerable<IBehaviourHolder> events, IEnumerable<IBoundaryBase> boundaries, Dictionary<IBoundaryBase, IBehaviourHolder[]> interactions, double minInteractionDistance, double gapDistance, int thresholdValue, int thresholdValue2, int startFrame, int endFrame, double frameRate, bool smoothMotion, double smoothingFactor, double centroidSize, double pelvicArea1, double pelvicArea2, double pelvicArea3, double pelvicArea4, Rectangle roi, string message = "");
    }
}
