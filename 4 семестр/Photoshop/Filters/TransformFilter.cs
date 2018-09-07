using System;
using System.Drawing;

namespace MyPhotoshop
{
    public class TransformFilter : TransformFilter<EmptyParameters>
    {
        public TransformFilter(string filterName, Func<Size, Size> transformSize, Func<Point, Size, Point> transformPoint) 
            : base(filterName, new FreeTransformer(transformSize, transformPoint))
        {
        }
    }
}