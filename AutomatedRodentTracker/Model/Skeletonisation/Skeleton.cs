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
