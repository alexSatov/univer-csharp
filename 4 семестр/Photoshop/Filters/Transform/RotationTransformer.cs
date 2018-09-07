using System;
using System.Drawing;

namespace MyPhotoshop
{
    public class RotationTransformer : ITransformer<RotationParameters>
    {
        public Size NewSize { get; private set; }
        
        private double angle;
        private Size oldSize;

        public void Prepare(Size oldSize, RotationParameters parameters)
        {
            this.oldSize = oldSize;
            angle = Math.PI * parameters.Angle / 180;
            NewSize = new Size(
                (int)(oldSize.Width * Math.Abs(Math.Cos(angle)) + oldSize.Height * Math.Abs(Math.Sin(angle))),
                (int)(oldSize.Height * Math.Abs(Math.Cos(angle)) + oldSize.Width * Math.Abs(Math.Sin(angle))));
        }

        public Point? MapPoint(Point point)
        {
            point = new Point(point.X - NewSize.Width / 2, point.Y - NewSize.Height / 2);
            var x = oldSize.Width / 2 + (int)(point.X * Math.Cos(angle) + point.Y * Math.Sin(angle));
            var y = oldSize.Height / 2 + (int)(-point.X * Math.Sin(angle) + point.Y * Math.Cos(angle));

            if (x < 0 || x >= oldSize.Width || y < 0 || y >= oldSize.Height)
                return null;
            return new Point(x, y);
        }
    }
}