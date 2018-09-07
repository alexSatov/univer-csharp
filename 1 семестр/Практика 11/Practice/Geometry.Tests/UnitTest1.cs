using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Practice;

namespace Geom.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GeometryIsOnSegmentTest1()
        {
            var a = new Point (0, 0);
            var b = new Point(6, 3);
            var result = Geometry.IsOnSegment(new Segment(a, b), new Point(4, 2));
            Assert.AreEqual(true, result);
        }
        [TestMethod]
        public void GeometryIsOnSegmentTest2()
        {
            var a = new Point(-3, -2);
            var b = new Point(-7, 2);
            var result = Geometry.IsOnSegment(new Segment(a, b), new Point(-4, 3));
            Assert.AreEqual(false, result);
        }
        [TestMethod]
        public void GeometryIsOnSegmentTest3()
        {
            var a = new Point(0, 0);
            var b = new Point(6, 3);
            var result = Geometry.IsOnSegment(new Segment(a, b), new Point(0, 0));
            Assert.AreEqual(true, result);
        }
        [TestMethod]
        public void GeometryIsOnSegmentTest4()
        {
            var a = new Point(0, 0);
            var b = new Point(6, 3);
            var result = Geometry.IsOnSegment(new Segment(a, b), new Point(8, 4));
            Assert.AreEqual(false, result);
        }
        [TestMethod]
        public void GeometryCreateRectangleTest1()
        {
            var a = new Point(-2, 1);
            var b = new Point(-2, 7);
            var c = new Point(2, 7);
            var result = Geometry.CreateRectangle(a, b, c);
            Assert.AreEqual(2, result.GetFourthPoint(b, a, c).x);
            Assert.AreEqual(1, result.GetFourthPoint(b, a, c).y);
        }
        [TestMethod]
        public void PointRotateTest1()
        {
            var a = new Point(-2, 1);
            var o = new Point(0, 0);
            a.Rotate(Math.PI, o);
            Assert.AreEqual(2, Math.Round(a.x));
            Assert.AreEqual(-1, Math.Round(a.y));
        }
        [TestMethod]
        public void PointRotateTest2()
        {
            var a = new Point(-2, 1);
            var o = new Point(0, 0);
            a.Rotate(Math.PI*2, o);
            Assert.AreEqual(-2, Math.Round(a.x));
            Assert.AreEqual(1, Math.Round(a.y));
        }
    }
}
