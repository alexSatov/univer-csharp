using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice
{
    public class Point
    {
        public double x;
        public double y;
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public bool IsOnSegment(Segment segment)
        {
            return Geometry.IsOnSegment(segment, this);
        }

        public void Rotate(double angle, Point rotationPoint)
        {
            var transitionMatrix = MathematicsRotation.getTransitionMatrix(angle, rotationPoint);
            var newPoint = MathematicsRotation.getNewPoint(new double[] { x, y, 1 }, transitionMatrix);
            x = newPoint.x;
            y = newPoint.y;
        }
    }

    public class Segment
    {
        public Point startPoint;
        public Point endPoint;
        public Segment(Point p1, Point p2)
        {
            startPoint = p1;
            endPoint = p2;
        }

        public bool IsOnSegment(Point point)
        {
            return Geometry.IsOnSegment(this, point);
        }

    }

    public class Rectangle
    {
        Point point1;
        Point point2;
        Point point3;
        Point point4;

        public Segment[] GetAllSides()
        {
            if (point4 == null)
                return null;
            return new Segment[] {
                new Segment(point1, point2),
                new Segment(point2, point3),
                new Segment(point3, point4),
                new Segment(point4, point1),
            };
        }

        public Rectangle(Point p1, Point p2, Point p3)
        {
            point1 = p1;
            point2 = p2;
            point3 = p3;
            var s1 = new Segment(p1, p2);
            var s2 = new Segment(p2, p3);
            var s3 = new Segment(p1, p3);
            if (Geometry.IsPerpendicular(s1, s2))
            {
                point4 = GetFourthPoint(p2, p1, p3);
            }
            if (Geometry.IsPerpendicular(s2, s3))
            {
                point4 = GetFourthPoint(p3, p1, p2);
            }
            if (Geometry.IsPerpendicular(s1, s3))
            {
                point4 = GetFourthPoint(p1, p2, p3);
            }
        }

        public Point GetFourthPoint(Point middlePoint, Point point1, Point point2)
        {
            return new Point(point1.x + point2.x - middlePoint.x, point1.y + point2.y - middlePoint.y);
        }

        public void Rotate(double angle, Point rotationPoint)
        {
            var transitionMatrix = MathematicsRotation.getTransitionMatrix(angle, rotationPoint);

            point1.Rotate(angle, rotationPoint);
            point2.Rotate(angle, rotationPoint);
            point3.Rotate(angle, rotationPoint);
            point4.Rotate(angle, rotationPoint);
        }
    }

    public static class Geometry
    {
        public static Segment CreateSegment(Point point1, Point point2)
        {
            return new Segment(point1, point2);
        }

        public static bool IsOnSegment(Segment segment, Point point)
        {
            var a = (segment.endPoint.y - segment.startPoint.y) / (segment.endPoint.x - segment.startPoint.x);
            var b = segment.startPoint.y - a * segment.startPoint.x;
            return (point.y == a * point.x + b) && IsOnGap(segment, point);
        }

        public static bool IsOnGap(Segment segment, Point point)
        {
            return point.x >= Math.Min(segment.startPoint.x, segment.endPoint.x) &&
            point.x <= Math.Max(segment.startPoint.x, segment.endPoint.x) &&
            point.y >= Math.Min(segment.startPoint.y, segment.endPoint.y) &&
            point.y <= Math.Max(segment.startPoint.y, segment.endPoint.y);
        }

        public static Rectangle CreateRectangle(Point point1, Point point2, Point point3)
        {
            var rectangle = new Rectangle(point1, point2, point3);
            return rectangle;
        }
        public static bool IsPerpendicular(Segment segment1, Segment segment2)
        {
            var x1 = segment1.endPoint.x - segment1.startPoint.x;
            var x2 = segment2.endPoint.x - segment2.startPoint.x;
            var y1 = segment1.endPoint.y - segment1.startPoint.y;
            var y2 = segment2.endPoint.y - segment2.startPoint.y;
            return (x1 * x2 + y1 * y2 == 0);
        }
    }

    static class MathematicsRotation
    {
        public static double[,] getTransitionMatrix(double angle, Point rotationPoint)
        {

            return new double[,]{
                    {Math.Cos(angle), Math.Sin(angle), 0},
                    {-1*Math.Sin(angle), Math.Cos(angle), 0},
                    {-1*rotationPoint.x*(Math.Cos(angle) - 1) + rotationPoint.y * Math.Sin(angle), -1 * rotationPoint.x * Math.Sin(angle) - rotationPoint.y * (Math.Cos(angle) - 1), 1}
            };

        }

        public static Point getNewPoint(double[] coordMatrix, double[,] transitionMatrix)
        {
            var result = new double[3];

            for (var i = 0; i < coordMatrix.Length; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    result[i] += coordMatrix[j] * transitionMatrix[j, i];
                }
            }

            var newX = result[0] / result[2];
            var newY = result[1] / result[2];
            return new Point(newX, newY);
        }
    }
}
