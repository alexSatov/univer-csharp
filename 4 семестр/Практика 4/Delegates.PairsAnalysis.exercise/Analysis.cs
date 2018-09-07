using System;
using System.Linq;
using System.Collections.Generic;

namespace Delegates.PairsAnalysis
{
    public class Analysis
    {
        public static int FindMaxPause(params DateTime[] data)
        {
            return data.Pairs().Select(p => (p.Item2 - p.Item1).TotalSeconds).MaxIndex();
        }

        public static double FindAverageRelativeDifference(params double[] data)
        {
            return data.Pairs().Select(p => (p.Item2 - p.Item1) / p.Item1).Average();
        }
    }

    public static class EnumerableExtensions
    {
        public static int MaxIndex<T>(this IEnumerable<T> data)
            where T : IComparable
        {
            var maxIndex = -1;
            var currentIndex = 0;
            var maxValue = default(T);
            var isFirst = true;

            foreach (var value in data)
            {
                if (isFirst)
                {
                    isFirst = false;
                    maxValue = value;
                    maxIndex = 0;
                }

                if (value.CompareTo(maxValue) == 1)
                {
                    maxIndex = currentIndex;
                    maxValue = value;
                }

                currentIndex++;
            }

            if (maxIndex == -1)
                throw new ArgumentException("Пустая коллекция");

            return maxIndex;
        }

        public static IEnumerable<Tuple<T, T>> Pairs<T>(this IEnumerable<T> data)
        {
            var pairs = new List<Tuple<T, T>>();
            var previous = default(T);
            var isFirst = true;

            foreach (var element in data)
            {
                if (isFirst)
                {
                    isFirst = false;
                    previous = element;
                    continue;
                }
                pairs.Add(Tuple.Create(previous, element));
                previous = element;
            }

            if (pairs.Count == 0)
                throw new ArgumentException("Недостаточное количество элементов в коллекции");

            return pairs;
        }
    }
}
