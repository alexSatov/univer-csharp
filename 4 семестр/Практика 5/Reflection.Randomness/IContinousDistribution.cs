using System;

namespace Reflection.Randomness
{
    public interface IContinousDistribution
    {
        double Generate(Random rnd);
    }
}
