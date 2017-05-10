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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Emgu.CV;
using AutomatedRodentTracker.View.Image;
using AutomatedRodentTracker.ViewModel.Image;

namespace AutomatedRodentTracker.Services.ImageToBitmapSource
{
    public static class ImageService
    {
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap();

                BitmapSource bs = Imaging.CreateBitmapSourceFromHBitmap(ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr);
                return bs;
            }
        }

        public static void DisplayImage(IImage image)
        {
            BitmapSource bitmap = ToBitmapSource(image);
            DisplayImage(bitmap);
        }

        public static void DisplayImage(BitmapSource bitmap)
        {
            ImageViewer view = new ImageViewer();
            ImageViewerViewModel viewModel = new ImageViewerViewModel();
            viewModel.Image = bitmap;
            view.DataContext = viewModel;
            view.Show();
        }
    }
}
