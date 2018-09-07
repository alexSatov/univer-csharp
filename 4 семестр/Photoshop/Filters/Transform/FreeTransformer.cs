using System;
using System.Drawing;

namespace MyPhotoshop
{
    public class FreeTransformer : ITransformer<EmptyParameters>
    {
        public Size NewSize { get; private set; }

        private Size oldSize;
        private readonly Func<Size, Size> transformSize;
        private readonly Func<Point, Size, Point> transformPoint;

        public FreeTransformer(Func<Size, Size> transformSize, Func<Point, Size, Point> transformPoint)
        {
            this.transformSize = transformSize;
            this.transformPoint = transformPoint;
        }

        public void Prepare(Size oldSize, EmptyParameters parameters)
        {
            this.oldSize = oldSize;
            NewSize = transformSize(oldSize);
        }

        public Point? MapPoint(Point point)
        {
            return transformPoint(point, oldSize);
        }
    }
}