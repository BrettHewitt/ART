using System;
using System.Collections.Generic;
using AutomatedRodentTracker.Model.Behaviours;
using AutomatedRodentTracker.Model.Boundries;
using AutomatedRodentTracker.Model.Results;
using AutomatedRodentTracker.Model.Results.Behaviour;
using AutomatedRodentTracker.Model.Results.Behaviour.BodyOption;
using AutomatedRodentTracker.Model.Results.Behaviour.Movement;
using AutomatedRodentTracker.Model.Skeletonisation;
using AutomatedRodentTracker.Model.Smoothing;
using AutomatedRodentTracker.Model.Video;
using AutomatedRodentTracker.ModelInterface.Behaviours;
using AutomatedRodentTracker.ModelInterface.BodyDetection;
using AutomatedRodentTracker.ModelInterface.Boundries;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.BodyOption;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.Movement;
using AutomatedRodentTracker.ModelInterface.Skeletonisation;
using AutomatedRodentTracker.ModelInterface.Smoothing;
using AutomatedRodentTracker.ModelInterface.Video;
using AutomatedRodentTracker.Model.Datasets;
using AutomatedRodentTracker.Model.Datasets.Types;
using AutomatedRodentTracker.Model.Motion.BackgroundSubtraction;
using AutomatedRodentTracker.Model.Motion.MotionBackground;
using AutomatedRodentTracker.Model.RBSK;
using AutomatedRodentTracker.Model.Results.Behaviour.Rotation;
using AutomatedRodentTracker.ModelInterface.Datasets;
using AutomatedRodentTracker.ModelInterface.Datasets.Types;
using AutomatedRodentTracker.ModelInterface.Motion.BackgroundSubtraction;
using AutomatedRodentTracker.ModelInterface.Motion.MotionBackground;
using AutomatedRodentTracker.ModelInterface.RBSK;
using AutomatedRodentTracker.ModelInterface.Results;
using AutomatedRodentTracker.ModelInterface.Results.Behaviour.Rotation;
using AutomatedRodentTracker.ModelInterface.VideoSettings;

namespace AutomatedRodentTracker.Model.Resolver
{
    public static class ModelResolver
    {
        private static Dictionary<Type, Func<object>> _TypeDictionary = new Dictionary<Type, Func<object>>(); 

        public static T Resolve<T>() where T : class
        {
            return _TypeDictionary[typeof(T)].Invoke() as T;
        }

        static ModelResolver()
        {
            MWA_Model.Model.ModelResolver.AddModels(_TypeDictionary);

            _TypeDictionary.Add(typeof(IVideo), () => new Video.Video());
            _TypeDictionary.Add(typeof(IMotionBackgroundSubtraction), () => new MotionBackgroundSubtraction());
            _TypeDictionary.Add(typeof(IVideoSettings), () => new VideoSettings.VideoSettings());
            _TypeDictionary.Add(typeof(IGenerateBoundries), () => new GenerateBoundries());
            _TypeDictionary.Add(typeof(IMotionBackground), () => new MotionBackground());
            _TypeDictionary.Add(typeof(IRBSKVideo), () => new RBSKVideo());
            //_TypeDictionary.Add(typeof(ILargeMemoryVideo), () => new LargeMemoryVideo());
            _TypeDictionary.Add(typeof(ISkeleton), () => new Skeleton());
            _TypeDictionary.Add(typeof(ISpineFinding), () => new SpineFinding());
            _TypeDictionary.Add(typeof(ITailFinding), () => new TailFinding());
            //_TypeDictionary.Add(typeof(ILabbookConverter), () => new LabbookConverter());
            //_TypeDictionary.Add(typeof(ILabbookData), () => new LabbookData());
            _TypeDictionary.Add(typeof(ISingleFile), () => new SingleFile());
            _TypeDictionary.Add(typeof(INonTransgenic), () => new NonTransgenic());
            _TypeDictionary.Add(typeof(ITransgenic), () => new Transgenic());
            _TypeDictionary.Add(typeof(IUndefined), () => new Undefined());
            _TypeDictionary.Add(typeof(ISingleMouse), () => new SingleMouse());
            _TypeDictionary.Add(typeof(IBodyDetection), () => new BodyDetection.BodyDetection());
            _TypeDictionary.Add(typeof(IMouseDataResult), () => new MouseDataResult());
            _TypeDictionary.Add(typeof(IArtefactsBoundary), () => new ArtefactsBoundary());
            _TypeDictionary.Add(typeof(IBoxBoundary), () => new BoxBoundary());
            _TypeDictionary.Add(typeof(ICircleBoundary), () => new CircleBoundary());
            _TypeDictionary.Add(typeof(IOuterBoundary), () => new OuterBoundary());
            _TypeDictionary.Add(typeof(ITrackedVideo), () => new TrackedVideo());
            _TypeDictionary.Add(typeof(IBehaviourHolder), () => new BehaviourHolder());
            _TypeDictionary.Add(typeof(ISaveArtFile), () => new SaveArtFile());
            _TypeDictionary.Add(typeof(ISingleFrameResult), () => new SingleFrameResult());
            _TypeDictionary.Add(typeof(ITrackSmoothing), () => new TrackSmoothing());

            _TypeDictionary.Add(typeof(IBehaviourSpeedDefinitions), () => new BehaviourSpeedDefinitions());
            _TypeDictionary.Add(typeof(IStill), () => new Still());
            _TypeDictionary.Add(typeof(IWalking), () => new Walking());
            _TypeDictionary.Add(typeof(IRunning), () => new Running());
            _TypeDictionary.Add(typeof(INoRotation), () => new NoRotation());
            _TypeDictionary.Add(typeof(ISlowTurning), () => new SlowTurning());
            _TypeDictionary.Add(typeof(IFastTurning), () => new FastTurning());
            _TypeDictionary.Add(typeof(IShaking), () => new Shaking());
            _TypeDictionary.Add(typeof(IHeadVisible), () => new HeadVisible());
            _TypeDictionary.Add(typeof(IBodyVisible), () => new BodyVisible());
            _TypeDictionary.Add(typeof(ITailVisible), () => new TailVisible());
            _TypeDictionary.Add(typeof(IHeadBodyTailVisible), () => new HeadBodyTailVisible());

        }
    }
}
