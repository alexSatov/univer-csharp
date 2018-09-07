using System;
using System.Numerics;

namespace Incapsulation.RationalNumbers
{
    public class Rational
    {
        public readonly int Numerator;
        public readonly int Denominator;
        public readonly bool IsNan;

        public Rational(int numerator)
        {
            Numerator = numerator;
            Denominator = 1;
        }

        public Rational(int numerator, int denominator)
        {
            IsNan = denominator == 0;
            if (IsNan) return;

            if (numerator == 0)
            {
                Numerator = 0;
                Denominator = 1;
            }
            else
            {
                var sign = Math.Sign(numerator) * Math.Sign(denominator);
                var rational = Reduce(Math.Abs(numerator), Math.Abs(denominator));
                Numerator = rational.Item1 * sign;
                Denominator = rational.Item2;
            }
        }

        public static Rational operator +(Rational r1, Rational r2)
        {
            if (r1.IsNan || r2.IsNan)
                return new Rational(0, 0);
            var newNum1 = r2.Denominator * r1.Numerator;
            var newNum2 = r1.Denominator * r2.Numerator;
            return new Rational(newNum1 + newNum2, r1.Denominator*r2.Denominator);
        }

        public static Rational operator -(Rational r1, Rational r2)
        {
            if (r1.IsNan || r2.IsNan)
                return new Rational(0, 0);
            return r1 + new Rational(-r2.Numerator, r2.Denominator);
        }

        public static Rational operator *(Rational r1, Rational r2)
        {
            if (r1.IsNan || r2.IsNan)
                return new Rational(0, 0);
            return new Rational(r1.Numerator * r2.Numerator, r1.Denominator * r2.Denominator);
        }

        public static Rational operator /(Rational r1, Rational r2)
        {
            if (r1.IsNan || r2.IsNan)
                return new Rational(0, 0);
            return r1 * new Rational(r2.Denominator, r2.Numerator);
        }

        public static implicit operator Rational(int i)
        {
            return new Rational(i);
        }

        public static implicit operator int(Rational rational)
        {
            if (rational.Numerator % rational.Denominator != 0)
                throw new ArgumentException();
            return rational.Numerator / rational.Denominator;
        }

        public static implicit operator double(Rational rational)
        {
            return (double)rational.Numerator / rational.Denominator;
        }

        private static Tuple<int, int> Reduce(int numerator, int denominator)
        {
            var gcd = (int)BigInteger.GreatestCommonDivisor(numerator, denominator);
            numerator /= gcd;
            denominator /= gcd;
            return Tuple.Create(numerator, denominator);
        }
    }
}
