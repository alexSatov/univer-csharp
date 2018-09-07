using System;
using System.Collections.Generic;

namespace SRP.ControlDigit
{
    public static class ControlDigitAlgo
    {
        public static int Upc(long number)
        {
            var sum = number.GetNumberDigits().WeightedSum(3, f => 4 - f);
            return sum.ComplementTo(10);
        }

        public static int Isbn10(long number)
        {
            var sum = number.GetNumberDigits().WeightedSum(2, f => f + 1);
            var result = sum.ComplementTo(11);
            return result == 10 ? 'X' : '0' + result;
        }

        public static int Isbn13(long number)
        {
            var sum = number.GetNumberDigits().WeightedSum(1, f => 4 - f);
            return sum.ComplementTo(10);
        }
    }

    public static class Extensions
    {
        public static IEnumerable<int> GetNumberDigits(this long number)
        {
            do
            {
                yield return (int)(number % 10);
                number /= 10;
            } while (number > 0);
        }

        public static int ComplementTo(this int number, int divider)
        {
            var result = number % divider;
            return result != 0 ? divider - result : result;
        }

        public static int WeightedSum(this IEnumerable<int> collection, int startWeight, Func<int, int> getNextWeight)
        {
            var sum = 0;
            var weight = startWeight;

            foreach (var e in collection)
            {
                sum += weight*e;
                weight = getNextWeight(weight);
            }

            return sum;
        }
    }
}