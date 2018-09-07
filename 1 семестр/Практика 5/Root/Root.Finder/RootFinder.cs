using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolynomRootFinder
{
    public class RootFinder
    {
        public static double PolynomSolver(double x, double[] a)
        {
            double result = 0;
            for (int i = 0; i < a.Length; i++)
                result += a[i] * Math.Pow(x, i);
            return result;
        }

        public static double Solution(double left, double right, double[] args)
        {            
            var leftSide = PolynomSolver(left, args);
            var rightSide = PolynomSolver(right, args);
            if (leftSide > rightSide)
            {
                double t = left;
                left = right;
                right = t;
            }            
            if (Math.Sign(leftSide) == Math.Sign(rightSide))
                return double.NaN;            
            while (Math.Abs(PolynomSolver(left, args)) > 0.000001)
            {
                double middle = (left + right) / 2;
                double middleValue = PolynomSolver(middle, args);
                if (middleValue > 0)
                    right = middle;
                else
                    left = middle;
            }
            return left;
        }
    }
}
