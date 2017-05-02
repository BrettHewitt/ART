using System;
using System.Drawing;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace AutomatedRodentTracker.Converters
{
    public class BitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (!(value is Bitmap))
            {
                return null;
            }

            MemoryStream ms = new MemoryStream();
            ((Bitmap)value).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
