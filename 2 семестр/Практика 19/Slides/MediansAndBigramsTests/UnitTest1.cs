using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slides;
using System.Linq;

namespace MediansAndBigramsTests
{
    [TestClass]
    public class MedianTests
    {
        [TestMethod]
        public void VerySimpleMedianTest1()
        {
            Assert.AreEqual(5, new double[] { 5 }.Median());
        }

        [TestMethod]
        public void VerySimpleMedianTest2()
        {
            Assert.AreEqual(double.NaN, new double[] { }.Median());
        }

        [TestMethod]
        public void SimpleMedianTest1()
        {
            Assert.AreEqual(2, new double[] { 1, 2, 3 }.Median());
        }

        [TestMethod]
        public void SimpleMedianTest2()
        {
            Assert.AreEqual(2, new double[] { 3, 1, 2 }.Median());
        }

        [TestMethod]
        public void SimpleMedianTest3()
        {
            Assert.AreEqual(2.5, new double[] { 3, 1, 2, 7 }.Median());
        }

        [TestMethod]
        public void HardMedianTest()
        {
            Assert.AreEqual(5, new double[] { 3, 1, 1, 5, 5, 5, 5, 100, 22, 2, 7, 1001 }.Median());
        }       
    }

    [TestClass]
    public class BigramsTests
    {
        [TestMethod]
        public void VerySimpleBigramsTest1()
        {
            Assert.AreEqual(null, new[] { 5 }.GetBigrams());
        }

        [TestMethod]
        public void VerySimpleBigramsTest2()
        {
            Assert.AreEqual(null, new string[] { }.GetBigrams());
        }

        [TestMethod]
        public void SimpleBigramsTest1()
        {
            var result = new[] { 1, 2, 3 }.GetBigrams();
            var expected = new[] { Tuple.Create(1, 2), Tuple.Create(2, 3) };
            bool equality = result.Intersect(expected).Count() == expected.Count();   
            Assert.IsTrue(equality);
        }

        [TestMethod]
        public void SimpleBigramsTest2()
        {
            var result = new string[] { "up", "down", "left", "right" }.GetBigrams();
            var expected = new[] { Tuple.Create("up", "down"), Tuple.Create("down", "left"), Tuple.Create("left", "right") };
            bool equality = result.Intersect(expected).Count() == expected.Count();
            Assert.IsTrue(equality);           
        }

        [TestMethod]
        public void DuplicateBigramsTest()
        {
            var result = new string[] { "ab", "ba", "ab", "ba", "ab", "ba" }.GetBigrams();
            var expected = new[] { Tuple.Create("ab", "ba"), Tuple.Create("ba", "ab") };
            bool equality = result.Intersect(expected).Count() == expected.Count();
            Assert.IsTrue(equality);            
        }
    }
}
