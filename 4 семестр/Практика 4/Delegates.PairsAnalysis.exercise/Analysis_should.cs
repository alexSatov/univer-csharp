using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.PairsAnalysis
{
    [TestFixture]
    class Analysis_should
    {
        [Test]
        public void ProcessDatesCorrectly()
        {
            Assert.AreEqual(
                2,
                Analysis.FindMaxPause(
                    new DateTime(2001, 1, 1),
                    new DateTime(2001, 1, 2),
                    new DateTime(2001, 1, 3),
                    new DateTime(2001, 1, 5),
                    new DateTime(2001, 1, 6)));
        }

        [Test]
        public void ThrowsAtEmptyCollection()
        {
            Assert.Throws(typeof(ArgumentException),() => Analysis.FindMaxPause());
        }

        [Test]
        public void ThrowsAtOneElementCollection()
        {
            Assert.Throws(typeof(ArgumentException), () => Analysis.FindMaxPause(new DateTime(2001, 1, 1)));
        }

        [Test]
        public void ProcessDoublesCorrectly()
        {
            Assert.AreEqual(
                0.1,
                Analysis.FindAverageRelativeDifference(
                    1,
                    1.1,
                    1.21,
                    1.331),
                1e-5);
        }

    }
}
