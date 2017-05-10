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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutomatedRodentTracker.View.Cropping
{
    /// <summary>
    /// Interaction logic for CropImageView.xaml
    /// </summary>
    public partial class CropImageView : Window
    {
        private int ImagePixelWidth
        {
            get;
            set;
        }

        private int ImagePixelHeight
        {
            get;
            set;
        }

        private double StartX
        {
            get;
            set;
        }

        private double StartY
        {
            get;
            set;
        }

        private bool Dragging
        {
            get;
            set;
        }

        public CropImageView()
        {
            InitializeComponent();
        }

        private void Grid_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Dragging)
            {
                Rectangle.Width = 0;
                Rectangle.Height = 0;
                Dragging = false;
            }

            DrawRectFromPixelRect();
        }

        private void Image_OnTargetUpdated(object sender, DataTransferEventArgs e)
        {
            BitmapSource bitmapImage = Image.Source as BitmapSource;
            if (bitmapImage != null)
            {
                ImagePixelWidth = bitmapImage.PixelWidth;
                ImagePixelHeight = bitmapImage.PixelHeight;
            }
        }

        private void Image_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle.Width = 0;
            Rectangle.Height = 0;
            Point mousePosition = e.GetPosition(Canvas);
            StartX = mousePosition.X;
            StartY = mousePosition.Y;

            Rectangle.SetValue(Canvas.LeftProperty, StartX);
            Rectangle.SetValue(Canvas.TopProperty, StartY);
            Dragging = true;
        }

        private void Image_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!Dragging)
            {
                return;
            }

            Point mousePosition = e.GetPosition(Canvas);

            if (mousePosition.X < StartX)
            {
                Rectangle.SetValue(Canvas.LeftProperty, mousePosition.X);
                Rectangle.Width = StartX - mousePosition.X;
            }
            else
            {
                Rectangle.Width = mousePosition.X - StartX;
            }

            if (mousePosition.Y < StartY)
            {
                Rectangle.SetValue(Canvas.TopProperty, mousePosition.Y);
                Rectangle.Height = StartY - mousePosition.Y;
            }
            else
            {
                Rectangle.Height = mousePosition.Y - StartY;
            }

        }

        private void Image_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Dragging = false;
            UpdatePixelRect();
        }

        private void UpdatePixelRect()
        {
            Point imageStartPoint = Grid.TranslatePoint(new Point(StartX, StartY), Image);

            double pixelStartX = (imageStartPoint.X / Image.ActualWidth) * ImagePixelWidth;
            double pixelStartY = (imageStartPoint.Y / Image.ActualHeight) * ImagePixelHeight;
            double pixelWidth = (Rectangle.Width / Image.ActualWidth) * ImagePixelWidth;
            double pixelHeight = (Rectangle.Height / Image.ActualHeight) * ImagePixelHeight;

            PixelRectangle.SetValue(Canvas.LeftProperty, pixelStartX);
            PixelRectangle.SetValue(Canvas.TopProperty, pixelStartY);
            PixelRectangle.Width = pixelWidth;
            PixelRectangle.Height = pixelHeight;
        }

        private void DrawRectFromPixelRect()
        {
            if (PixelRectangle.Width == 0 || PixelRectangle.Height == 0)
            {
                Rectangle.Width = 0;
                Rectangle.Height = 0;
                return;
            }

            //Get Size in relation to current image size
            double? pixelX = PixelRectangle.GetValue(Canvas.LeftProperty) as double?;
            double? pixelY = PixelRectangle.GetValue(Canvas.TopProperty) as double?;

            if (!pixelX.HasValue || !pixelY.HasValue)
            {
                return;
            }

            Point imagePoint = Image.TranslatePoint(new Point(0, 0), Grid);

            double pixelWidth = PixelRectangle.Width;
            double pixelHeight = PixelRectangle.Height;

            double screenX = ((pixelX.Value / ImagePixelWidth) * Image.ActualWidth) + imagePoint.X;
            double screenY = ((pixelY.Value / ImagePixelHeight) * Image.ActualHeight) + imagePoint.Y;
            double screenWidth = (pixelWidth / ImagePixelWidth) * Image.ActualWidth;
            double screenHeight = (pixelHeight / ImagePixelHeight) * Image.ActualHeight;

            Rectangle.SetValue(Canvas.LeftProperty, screenX);
            Rectangle.SetValue(Canvas.TopProperty, screenY);
            Rectangle.Width = screenWidth;
            Rectangle.Height = screenHeight;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            PixelRectangle.SetValue(Canvas.LeftProperty, 0d);
            PixelRectangle.SetValue(Canvas.TopProperty, 0d);
            PixelRectangle.Width = ImagePixelWidth;
            PixelRectangle.Height = ImagePixelHeight;

            DrawRectFromPixelRect();
        }
    }
}
