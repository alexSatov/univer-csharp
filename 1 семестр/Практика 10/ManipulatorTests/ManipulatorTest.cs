using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Manipulator;

namespace ManipulatorTests
{
    [TestClass]
    public class ManipulatorTest
    {
        [TestMethod]
        public void GetAngleTest1()
        {
            var result = RobotMathematics.GetAngle(5, 5, 5);
            Assert.AreEqual(Math.PI/3, result, 1e-6);
        }
        [TestMethod]
        public void GetAngleTest2()
        {
            var result = RobotMathematics.GetAngle(5, 3, 4);
            Assert.AreEqual(Math.PI / 2, result, 1e-6);
        }
        [TestMethod]
        public void GetAngleTest3()
        {
            var result = RobotMathematics.GetAngle(1, 10, 4);
            Assert.AreEqual(double.NaN, result);
        }
        [TestMethod]
        public void GetAngleTest4()
        {
            var result = RobotMathematics.GetAngle(3, 5, 4);
            Assert.AreEqual(Math.Acos(4.0 / 5), result, 1e-6);
        }

        [TestMethod]
        public void MoveToTest1()
        {
            var result = RobotMathematics.MoveTo(17, -14, Math.PI / 6);           
            Assert.AreEqual(2*Math.PI, result[0] + result[1] + result[2] + Math.PI / 6, 1e-6);
        }
    }
}
