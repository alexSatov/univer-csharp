using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolynomRootFinder;

namespace RootTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ZeroTest()
        {
            double left = -100;
            double right = 100;
            double[] arguments = { 0, 1, -1, 1, -1, 1 }; 
            double result = RootFinder.Solution(left, right, arguments);
            Assert.AreEqual(0.0, result);
        }
        [TestMethod]
        public void Test()
        {
            double left = -100;
            double right = 100;
            double[] arguments = { 0, 1, -1, 1, -1, 1 };
            double result = RootFinder.Solution(left, right, arguments);
            Assert.AreEqual(0.0, result);
        }

    }
}
