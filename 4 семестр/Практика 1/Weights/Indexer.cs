using System;

namespace Incapsulation.Weights
{
    public class Indexer
    {
        public readonly int Length;

        private readonly double[] array;
        private readonly int offsetIndex;

        public Indexer(double[] array, int start, int end)
        {
            if (start < 0 || end < 0 || end >= array.Length)
                throw new ArgumentException();

            Length = end - start + 1;
            this.array = array;
            offsetIndex = start;
        }

        public double this[int index]
        {
            get
            {
                Check(index);
                return array[index+offsetIndex];
            }
            set
            {
                Check(index);
                array[index+offsetIndex] = value;
            }
        }

        private static void Check(int index)
        {
            if (index < 0)
                throw new IndexOutOfRangeException();
        }
    }
}
