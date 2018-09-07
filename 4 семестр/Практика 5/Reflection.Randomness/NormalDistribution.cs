using System;

namespace Reflection.Randomness
{
    public class NormalDistribution : IContinousDistribution
    {
        public readonly double Sigma;
        public readonly double Mean;

        public NormalDistribution(double mean, double sigma)
        {
            Sigma = sigma;
            Mean = mean;
        }


        public double Generate(Random rnd)
        {
            var u = rnd.NextDouble();
            var v = rnd.NextDouble();
            var x = Math.Sqrt(-2 * Math.Log(u)) * Math.Cos(2 * Math.PI * v);
            return x * Sigma + Mean;
        }
    }
}
