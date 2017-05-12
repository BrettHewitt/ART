/*Automated Rodent Tracker - A program to automatically track rodents
Copyright(C) 2015 Brett Michael Hewitt

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see<http://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using AutomatedRodentTracker.Extensions;
using AutomatedRodentTracker.Model.Behaviours;
using AutomatedRodentTracker.Model.Boundries;
using AutomatedRodentTracker.Model.Results;
using AutomatedRodentTracker.Model.Video;
using AutomatedRodentTracker.Model.XmlClasses;
using AutomatedRodentTracker.ModelInterface.Behaviours;
using AutomatedRodentTracker.ModelInterface.Boundries;
using AutomatedRodentTracker.ModelInterface.Datasets;
using AutomatedRodentTracker.Services.FileBrowser;
using AutomatedRodentTracker.Model;
using AutomatedRodentTracker.ModelInterface.Results;
using AutomatedRodentTracker.ViewModel.NewWizard;

namespace AutomatedRodentTracker.Model.Datasets
{
    internal class SaveArtFile : ModelObjectBase, ISaveArtFile
    {
        public void SaveFile(string videoFileName, SingleFileResult result, Dictionary<int, ISingleFrameResult> headPoints, PointF[] motionTrack, PointF[] smoothedMotionTrack, Vector[] orientationTrack, IEnumerable<IBehaviourHolder> events, IEnumerable<IBoundaryBase> boundaries, Dictionary<IBoundaryBase, IBehaviourHolder[]> interactions, double minInteractionDistance, double gapDistance, int thresholdValue, int thresholdValue2, int startFrame, int endFrame, double frameRate, bool smoothMotion, double smoothingFactor, double centroidSize, double pelvicArea1, double pelvicArea2, double pelvicArea3, double pelvicArea4, double unitsToMilimeters, Rectangle roi, string message = "")
        {
            string fileLocation = FileBrowser.SaveFile("ART|*.art");

            if (string.IsNullOrWhiteSpace(fileLocation))
            {
                return;
            }

            SaveFile(fileLocation, videoFileName, result, headPoints, motionTrack, smoothedMotionTrack, orientationTrack, events, boundaries, interactions, minInteractionDistance, gapDistance, thresholdValue, thresholdValue2, startFrame, endFrame, frameRate, smoothMotion, smoothingFactor, centroidSize, pelvicArea1, pelvicArea2, pelvicArea3, pelvicArea4, unitsToMilimeters, roi, message);
        }

        public void SaveFile(string videoFileName, IMouseDataResult data)
        {
            string fileLocation = FileBrowser.SaveFile("ART|*.art");

            if (string.IsNullOrWhiteSpace(fileLocation))
            {
                return;
            }

            SaveFile(fileLocation, videoFileName, data);
        }

        public void SaveFile(string fileLocation, string videoFileName, IMouseDataResult data)
        {
            SaveFile(fileLocation, videoFileName, data.VideoOutcome, data.Results, data.MotionTrack, data.SmoothedMotionTrack, data.OrientationTrack, data.Events, data.Boundaries, data.InteractingBoundries, data.MinInteractionDistance, data.GapDistance, data.ThresholdValue, data.ThresholdValue2, data.StartFrame, data.EndFrame, data.FrameRate, data.SmoothMotion, data.SmoothFactor, data.CentroidSize, data.PelvicArea, data.PelvicArea2, data.PelvicArea3, data.PelvicArea4, data.UnitsToMilimeters, data.ROI, data.Message);
        }

        public void SaveFile(string fileLocation, string videoFileName, SingleFileResult result, Dictionary<int, ISingleFrameResult> headPoints, PointF[] motionTrack, PointF[] smoothedMotionTrack, Vector[] orientationTrack, IEnumerable<IBehaviourHolder> events, IEnumerable<IBoundaryBase> boundaries, Dictionary<IBoundaryBase, IBehaviourHolder[]> interactions, double minInteractionDistance, double gapDistance, int thresholdValue, int thresholdValue2, int startFrame, int endFrame, double frameRate, bool smoothMotion, double smoothingFactor, double centroidSize, double pelvicArea1, double pelvicArea2, double pelvicArea3, double pelvicArea4, double unitsToMilimeters, Rectangle roi, string message = "")
        {
            if (string.IsNullOrWhiteSpace(fileLocation))
            {
                return;
            }

            TrackedVideoXml fileXml;
            XmlSerializer serializer;
            RectangleXml roiXml = new RectangleXml(roi);

            if (result != SingleFileResult.Ok)
            {
                fileXml = new TrackedVideoXml(videoFileName, result, null, null, null, null, null, null, null, minInteractionDistance, gapDistance, thresholdValue, thresholdValue2, startFrame, endFrame, frameRate, smoothMotion, smoothingFactor, centroidSize, pelvicArea1, pelvicArea2, pelvicArea3, pelvicArea4, unitsToMilimeters, roiXml, message);
                serializer = new XmlSerializer(typeof(TrackedVideoXml));

                using (StreamWriter writer = new StreamWriter(fileLocation))
                {
                    serializer.Serialize(writer, fileXml);
                }

                return;
            }

            int headCount = headPoints.Count;
            SingleFrameResultXml[] allPoints = new SingleFrameResultXml[headCount];
            for (int i = 0; i < headCount; i++)
            {
                allPoints[i] = new SingleFrameResultXml(headPoints[i]);
            }

            DictionaryXml<int, SingleFrameResultXml> headPointsXml = new DictionaryXml<int, SingleFrameResultXml>(headPoints.Keys.ToArray(), allPoints);
            PointFXml[] motionTrackXml = motionTrack.Select(point => new PointFXml(point.X, point.Y)).ToArray();
            PointFXml[] smoothedMotionTrackXml = smoothedMotionTrack.Select(point => new PointFXml(point)).ToArray();
            VectorXml[] orientationTrackXml = orientationTrack.Select(vector => new VectorXml(vector)).ToArray();

            List<BoundaryBaseXml> boundariesXml = boundaries.Select(boundary => boundary.GetData()).ToList();

            List<BehaviourHolderXml> eventsXml = new List<BehaviourHolderXml>();
            foreach (IBehaviourHolder behaviour in events)
            {
                BoundaryBaseXml boundary = null;
                InteractionBehaviour interaction = behaviour.Interaction;
                int frameNumber = behaviour.FrameNumber;
                boundary = behaviour.Boundary.GetData();

                eventsXml.Add(new BehaviourHolderXml(boundary, interaction, frameNumber));
            }

            BoundaryBaseXml[] keys = interactions.Keys.Select(key => key.GetData()).ToArray();
            BehaviourHolderXml[][] values = interactions.Values.Select(value => value.Select(behavHolder => new BehaviourHolderXml(behavHolder.Boundary.GetData(), behavHolder.Interaction, behavHolder.FrameNumber)).ToArray()).ToArray();
            DictionaryXml<BoundaryBaseXml, BehaviourHolderXml[]> interactionBoundries = new DictionaryXml<BoundaryBaseXml, BehaviourHolderXml[]>(keys, values);
            
            fileXml = new TrackedVideoXml(videoFileName, result, headPointsXml, motionTrackXml, smoothedMotionTrackXml, orientationTrackXml, boundariesXml.ToArray(), eventsXml.ToArray(), interactionBoundries, minInteractionDistance, gapDistance, thresholdValue, thresholdValue2, startFrame, endFrame, frameRate, smoothMotion, smoothingFactor, centroidSize, pelvicArea1, pelvicArea2, pelvicArea3, pelvicArea4, unitsToMilimeters, roiXml, message);
            serializer = new XmlSerializer(typeof(TrackedVideoXml));

            using (StreamWriter writer = new StreamWriter(fileLocation))
            {
                serializer.Serialize(writer, fileXml);
            }
        }
    }
}
