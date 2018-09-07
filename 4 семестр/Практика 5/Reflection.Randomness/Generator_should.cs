using System;
using NUnit.Framework;

namespace Reflection.Randomness
{
    class T1
    {
        [FromDistribution(typeof(NormalDistribution),1,2)]
        public double A { get; set; }
    }

    class T2
    {
        [FromDistribution(typeof(NormalDistribution),-1,2)]
        public double A { get; set; }
        [FromDistribution(typeof(ExponentialDistribution),4)]
        public double B { get; set; }
    }


    [TestFixture]
    public class Generator_should
    {
        
        [Test]
        public void GenerateT1()
        {
            var rnd = new Random(1);
            var e = new Generator<T1>().Generate(rnd);
            Assert.AreEqual(3.5609,e.A,1e-4);
        }

        [Test]
        public void GenerateT2()
        {
            var rnd = new Random(1);
            var e = new Generator<T2>().Generate(rnd);
            Assert.AreEqual(1.5609, e.A,1e-4);
            Assert.AreEqual(0.1903, e.B, 1e-4);
        }

        [Test]
        public void ReplaceGeneratorFor1Field()
        {
            var rnd = new Random(1);
            var generator = new Generator<T2>()
                .For(z => z.A)
                .Set(new NormalDistribution(0, 1));
            var e = generator.Generate(rnd);
            Assert.AreEqual(1.2804, e.A, 1e-4);
            Assert.AreEqual(0.1903, e.B, 1e-4);
        }

        [Test]
        public void ReplaceGeneratorFor2Field()
        {
            var rnd = new Random(1);
            var generator = new Generator<T2>()
                .For(z => z.A)
                .Set(new NormalDistribution(0, 1))
                .For(z => z.B)
                .Set(new NormalDistribution(1, 1));
            var e = generator.Generate(rnd);
            Assert.AreEqual(1.2804, e.A, 1e-4);
            Assert.AreEqual(1.1669, e.B, 1e-4);
        }
    }
}
