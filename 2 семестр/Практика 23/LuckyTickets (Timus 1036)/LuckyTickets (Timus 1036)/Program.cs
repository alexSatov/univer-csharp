using System;
using System.Numerics;

namespace LuckyTickets
{
    class Program
    {
        static public BigInteger Calc(int n, int s, BigInteger[,] memory)
        {
            
            BigInteger z = 0;
            if (memory[n, s].CompareTo(z) >= 0)
                return memory[n, s];
            if (n == 0)
            {
                if (s == 0) return BigInteger.One;
                return z;
            }
            memory[n, s] = z;
            for (int i = 0; i < 10; i++)
            {
                if (s - i >= 0)
                    memory[n, s] = BigInteger.Add(memory[n, s], Calc(n - 1, s - i, memory));
            }
            return memory[n, s];
        }

        public static bool IsCorrectSum (int sum)
        {
            return sum % 2 != 0;
        }

        public static BigInteger[,] InitMemory()
        {
            int maxN = 50;
            int maxSum = 1000;
            BigInteger one = BigInteger.MinusOne;
            BigInteger[,] memory = new BigInteger[maxN + 1, maxSum + 1];

            for (int i = 0; i < 51; i++)
                for (int j = 0; j < 1001; j++)
                    memory[i, j] = one;
            return memory;
        }

        static void Main(string[] args)
        {
            var tokens = Console.ReadLine().Split(' ');
            int n = int.Parse(tokens[0]);
            int sum = int.Parse(tokens[1]);
            

            if (IsCorrectSum(sum))
            {
                Console.WriteLine(0);
                return;
            }

            sum /= 2;
            var memory = InitMemory();
            BigInteger result = Calc(n, sum, memory);
            result = BigInteger.Multiply(result, result);
            Console.WriteLine(result);
        }
    }
}
