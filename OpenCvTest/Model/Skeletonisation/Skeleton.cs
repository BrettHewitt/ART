using System;
using AutomatedRodentTracker.ModelInterface.Skeletonisation;
using Emgu.CV;
using Emgu.CV.Structure;

namespace AutomatedRodentTracker.Model.Skeletonisation
{
    internal class Skeleton : ModelObjectBase, ISkeleton
    {
        public int Iterations
        {
            get;
            set;
        }

        public Skeleton()
        {
            Iterations = 3;
        }

        public Image<Gray, Byte> GetSkeleton(Image<Gray, Byte> input)
        {
            Image<Gray, Byte> skel = new Image<Gray, byte>(input.Size);
            skel.SetValue(0);

            using (Image<Gray, Byte> gray = input.Clone())
            using (Image<Gray, Byte> filteredImage = gray.SmoothGaussian(5))
            using (Image<Gray, Byte> img = filteredImage.ThresholdBinary(new Gray(20), new Gray(255)))
            {
                Image<Gray, Byte> img2 = img.Not();

                bool done = false;

                while (!done)
                {
                    using (Image<Gray, Byte> eroded = img2.Erode(Iterations))
                    using (Image<Gray, Byte> temp = eroded.Dilate(Iterations))
                    using (Image<Gray, Byte> temp3 = img2.Sub(temp))
                    using (Image<Gray, Byte> temp2 = skel | temp3)
                    {
                        ;
                        skel.Dispose();
                        skel = temp2.Clone();
                        img2.Dispose();
                        img2 = eroded.Clone();
                        int[] nonZero = img2.CountNonzero();
                        if (nonZero[0] == 0)
                        {
                            done = true;
                        }
                    }
                }

                img2.Dispose();

            }

            return skel;
        }
    }
}
