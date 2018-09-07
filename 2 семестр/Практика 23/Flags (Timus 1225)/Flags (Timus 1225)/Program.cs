using System;

namespace Flags
{
    class Program
    {
        public static long GetFlagsCount (int stripsCount)
        {
            var cache = new long[stripsCount + 2];
            long flagsCount = 1;
            cache[1] = 1;
            cache[2] = 1;
            for (int i = 1; i < stripsCount - 1; i++)
            {
                if (cache[i] != 0)
                    flagsCount += cache[i];
                else
                {
                    cache[i] = cache[i - 1] + cache[i - 2];
                    flagsCount += cache[i];
                }
            }
            return flagsCount;
        }

        static void Main(string[] args)
        {
            int stripsCount = int.Parse(Console.ReadLine());
            var flagsCount = GetFlagsCount(stripsCount);
            Console.WriteLine(flagsCount * 2);
            Console.ReadKey();
        }
    }
}
