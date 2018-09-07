using System;

namespace AngryBirds
{
	public class AngryBirdsTask
	{
		/// <returns>Угол прицеливания в радианах от 0 до Pi/2</returns>
		public double FindSightAngle(double v, double distance)
		{
            //TODO 
            // Реализуйте функцию расчета угла прицеливания, в зависимости от начальной скорости снаряда и дальности до цели.
            double g = 9.8;
            return Math.Asin((distance * g) / (v * v)) / 2;
		}
	}
}
