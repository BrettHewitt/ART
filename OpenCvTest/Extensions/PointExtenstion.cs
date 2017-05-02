using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AutomatedRodentTracker.Extensions
{
    public static class PointExtenstion
    {
        public static Point Center(this Rectangle rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        public static int Area(this Rectangle rect)
        {
            return rect.Width * rect.Height;
        }

        public static int AbsoluteDistance(this Point point, Point p)
        {
            return Math.Abs(point.X - p.X) + Math.Abs(point.Y - p.Y);
        }

        public static float Distance(this Point point, Point p)
        {
            float xDiff = Math.Abs(point.X - p.X);
            float yDiff = Math.Abs(point.Y - p.Y);
            return (float)Math.Sqrt((xDiff * xDiff) + (yDiff * yDiff));
        }

        public static float Distance(this Point point, PointF p)
        {
            float xDiff = Math.Abs(point.X - p.X);
            float yDiff = Math.Abs(point.Y - p.Y);
            return (float)Math.Sqrt((xDiff * xDiff) + (yDiff * yDiff));
        }

        public static double Distance(this PointF point, Point p)
        {
            float xDiff = Math.Abs(point.X - p.X);
            float yDiff = Math.Abs(point.Y - p.Y);
            return Math.Sqrt((xDiff * xDiff) + (yDiff * yDiff));
        }

        public static double Distance(this PointF point, PointF p)
        {
            float xDiff = Math.Abs(point.X - p.X);
            float yDiff = Math.Abs(point.Y - p.Y);
            return Math.Sqrt((xDiff * xDiff) + (yDiff * yDiff));
        }

        public static double Distance(this PointF point, System.Windows.Point p)
        {
            double xDiff = Math.Abs(point.X - p.X);
            double yDiff = Math.Abs(point.Y - p.Y);
            return Math.Sqrt((xDiff * xDiff) + (yDiff * yDiff));
        }

        public static double Distance(this System.Windows.Point point, PointF p)
        {
            double xDiff = Math.Abs(point.X - p.X);
            double yDiff = Math.Abs(point.Y - p.Y);
            return Math.Sqrt((xDiff * xDiff) + (yDiff * yDiff));
        }

        public static double Distance(this System.Windows.Point point, System.Windows.Point p)
        {
            double xDiff = Math.Abs(point.X - p.X);
            double yDiff = Math.Abs(point.Y - p.Y);
            return Math.Sqrt((xDiff * xDiff) + (yDiff * yDiff));
        }

        public static float DistanceSquared(this Point point, Point p)
        {
            float xDiff = Math.Abs(point.X - p.X);
            float yDiff = Math.Abs(point.Y - p.Y);
            return (xDiff * xDiff) + (yDiff * yDiff);
        }

        public static float DistanceSquared(this Point point, PointF p)
        {
            float xDiff = Math.Abs(point.X - p.X);
            float yDiff = Math.Abs(point.Y - p.Y);
            return (xDiff * xDiff) + (yDiff * yDiff);
        }

        public static float DistanceSquared(this PointF point, Point p)
        {
            float xDiff = Math.Abs(point.X - p.X);
            float yDiff = Math.Abs(point.Y - p.Y);
            return (xDiff * xDiff) + (yDiff * yDiff);
        }

        public static float DistanceSquared(this PointF point, PointF p)
        {
            float xDiff = Math.Abs(point.X - p.X);
            float yDiff = Math.Abs(point.Y - p.Y);
            return (xDiff * xDiff) + (yDiff * yDiff);
        }

        public static void NormalizePoints(Point[] points, Rectangle rectangle)
        {
            if (rectangle.Height == 0 || rectangle.Width == 0)
                return;

            Matrix m = new Matrix();
            m.Translate(rectangle.Center().X, rectangle.Center().Y);

            if (rectangle.Width > rectangle.Height)
                m.Scale(1, 1f * rectangle.Width / rectangle.Height);
            else
                m.Scale(1f * rectangle.Height / rectangle.Width, 1);

            m.Translate(-rectangle.Center().X, -rectangle.Center().Y);
            m.TransformPoints(points);
        }

        public static void NormalizePoints2(Point[] points, Rectangle rectangle, Rectangle needRectangle)
        {
            if (rectangle.Height == 0 || rectangle.Width == 0)
                return;

            float k1 = 1f * needRectangle.Width / rectangle.Width;
            float k2 = 1f * needRectangle.Height / rectangle.Height;
            float k = Math.Min(k1, k2);

            Matrix m = new Matrix();
            m.Scale(k, k);
            m.Translate(needRectangle.X / k - rectangle.X, needRectangle.Y / k - rectangle.Y);
            m.TransformPoints(points);
        }

        public static PointF Offset(this PointF p, float dx, float dy)
        {
            return new PointF(p.X + dx, p.Y + dy);
        }

        public static Point ToPoint(this PointF p)
        {
            return new Point((int)p.X, (int)p.Y);
        }

        public static PointF ToPointF(this Point p)
        {
            return new PointF(p.X, p.Y);
        }

        public static Point MidPoint(this Point p, Point point)
        {
            return new Point((p.X + point.X) / 2, (p.Y + point.Y) / 2);
        }

        public static PointF MidPoint(this PointF p, PointF point)
        {
            return new PointF((p.X + point.X)/2, (p.Y + point.Y)/2);
        }
    }
}
