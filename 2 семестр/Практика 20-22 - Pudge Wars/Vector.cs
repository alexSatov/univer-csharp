using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PudgeClient
{
    public class Vector
    {
        public Vector(Node start, Node end)
        {
            X = end.Position.X - start.Position.X;
            Y = end.Position.Y - start.Position.Y;
        }

        public readonly double X;
        public readonly double Y;
        public double Length { get { return Math.Sqrt(X * X + Y * Y); } }
        public double Angle { get { return Math.Abs(Math.Atan2(Y, X)); } }

        public static double operator *(Vector a, Vector b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        /*public Vector Rotate(double angle)
        {
            return new Vector(X * Math.Cos(angle) - Y * Math.Sin(angle), X * Math.Sin(angle) + Y * Math.Cos(angle));
        }*/
    }
}
