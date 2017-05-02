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
