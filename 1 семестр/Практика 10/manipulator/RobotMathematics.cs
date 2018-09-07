using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manipulator
{
	/// <summary>
	/// Для лучшего понимания условий задачи см. чертеж.
	/// </summary>
	public static class RobotMathematics
	{
		public const double UpperArm = 15;
        public const double Forearm = 12;
        public const double Palm = 10;

		/// <summary>
		/// Возвращает угол (в радианах) между сторонами A и B в треугольнике со сторонами A, B, C 
		/// </summary>
		/// <param name="c"></param>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double GetAngle(double c, double a, double b)
		{
            if (a >= b + c || b >= a + c || c >= a + b)
                return double.NaN;
            else
                return Math.Acos((a*a + b*b - c* c)/(2*a*b));
		}

		/// <summary>
		/// Возвращает массив углов (Shoulder, Elbow, Wrist),
		/// необходимых для приведения эффектора манипулятора в точку x и y 
		/// с углом между последним суставом и горизонталью, равному angle (в радианах)
		/// См. чертеж Schematics.png!
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="angle"></param>
		/// <returns></returns>
		public static double[] MoveTo(double x, double y, double angle)
		{
            var angles = new double[3];
            var lineAD = Math.Sqrt(x * x + y * y);
            var beta = GetAngle(Math.Abs(y), lineAD, Math.Abs(x));
            var lineAC = Math.Sqrt(lineAD*lineAD + Palm*Palm - 2*lineAD*Palm*Math.Cos(beta + angle));
            angles[1] = GetAngle(lineAC, UpperArm, Forearm);
            var gamma = GetAngle(Forearm, UpperArm, lineAC);            
            angles[0] = beta + angle + gamma;
            angles[2] = 2*Math.PI - angles[1] - angles[0] - angle;                       
            return angles;
        }

	}
}
