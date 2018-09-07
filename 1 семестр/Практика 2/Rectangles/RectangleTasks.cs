using System;
using System.Drawing;

namespace Rectangles
{
	public class RectangleTasks
	{
        static bool Intersect (int y1, int y2, int bt1, int bt2)
        {
            return !(y1 > bt2 || bt1 < y2);
        }
        static int RectangleSquare (int x1, int y1, int x2, int y2)
        {
            return (x2-x1)*(y2-y1);
        }
        // Пересекаются ли два прямоугольника (пересечение только по границе также считается пересечением)
        public bool AreIntersected(Rectangle r1, Rectangle r2)
		{
            return Intersect(r1.Y, r2.Y, r1.Bottom, r2.Bottom) && Intersect(r1.X, r2.X, r1.Right, r2.Right);
        }
        // Площадь пересечения прямоугольников
		public int IntersectionSquare(Rectangle r1, Rectangle r2)
		{
            if (r1.Y > r2.Bottom || r1.Bottom < r2.Y || r1.Right < r2.X || r1.X > r2.Right) 
                return 0;
            return RectangleSquare(Math.Max(r1.X, r2.X), Math.Max(r1.Y, r2.Y), Math.Min(r1.Right, r2.Right), Math.Min(r1.Bottom, r2.Bottom));
        }
        // Если один из прямоугольников целиком находится внутри другого — вернуть номер (с нуля) внутреннего.
		// Иначе вернуть -1
		public int IndexOfInnerRectangle(Rectangle r1, Rectangle r2)
		{
            if (r1.Y >= r2.Y && r1.Bottom <= r2.Bottom && r1.Right <= r2.Right && r1.X >= r2.X)
                return 0;
            if (r1.Y <= r2.Y && r1.Bottom >= r2.Bottom && r1.Right >= r2.Right && r1.X <= r2.X)
                return 1;
            return -1;
		}
	}
}