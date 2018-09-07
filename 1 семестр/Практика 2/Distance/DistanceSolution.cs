using System;

namespace DistanceTask
{
    public class DistanceSolution
    {
        // –ассто€ние от точки (x, y) до отрезка AB с координатами A(aX, aY), B(bX, bY)
       
        static double Distance (double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x2-x1)*(x2-x1)+(y2-y1)*(y2-y1));
        }

        public static double GetDistanceToSegment(double aX, double aY, double bX, double bY, double x, double y)
        {
            double h = Math.Abs(((x - bX) * (aY - bY) - (aX - bX) * (y - bY))) / Distance(aX, aY, bX, bY);
            if (aX == bX && aY == bY)
                return Distance(aX, aY, x, y);
            if ((Math.Round(Distance(aX, aY, x, y)) + Math.Round(Distance(bX, bY, x, y)) == Math.Round(Distance(aX, aY, bX, bY)))) 
                return 0;
            if ((y / x == bY / bX) && (aY / aX == bY / bX))
                return Math.Min(Distance(aX, aY, x, y), Distance(bX, bY, x, y));
            if (Math.Pow(Math.Max(Distance(aX, aY, x, y), Distance(bX, bY, x, y)), 2) - h * h <= Distance(aX, aY, bX, bY) || Math.Round(Distance(aX, aY, x, y)) == Math.Round(Distance(bX, bY, x, y)) || Distance(aX, aY, bX, bY) == 0)
                return h;
            return Math.Min(Distance(aX, aY, x, y), Distance(bX, bY, x, y));
        }
    }
}