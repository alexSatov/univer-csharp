using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using AVLTree;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAddOneHundredThousandKeys()
        {
            Assert.IsTrue(CheckTimeOfAdd(100000));
        }

        [TestMethod]
        public void TestAddTwoHundredThousandKeys()
        {
            Assert.IsTrue(CheckTimeOfAdd(200000));
        }

        [TestMethod]
        public void TestAddFourHundredThousandKeys()
        {
            Assert.IsTrue(CheckTimeOfAdd(400000));
        }

        public bool CheckTimeOfAdd(long keysNumber)
        {
            var rand = new Random((int)DateTime.Now.Ticks);
            var tree = new Tree<int>();
            var expectedTime = Math.Log(keysNumber, 2) * 1000;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            for (var i = 0; i < keysNumber; i++)
            {
                tree.Add(rand.Next(0, 1000));
            }
            stopWatch.Stop();

            return stopWatch.ElapsedMilliseconds <= expectedTime;
        }

        [TestMethod]
        public void TestsSimpleAdditionOfElements()
        {
            var simpleTree = BuildTree(new List<int>() { 10, 1, 12 });
            var rootKey = simpleTree.Root.Key;
            var leftKey = simpleTree.Root.Left.Key;
            var rightKey = simpleTree.Root.Right.Key;

            Assert.IsTrue(rootKey == 10 && leftKey == 1 && rightKey == 12);
        }

        [TestMethod]
        public void TestAdditionOfObjects()
        {
            var first = new Vector(new Point(0, 0), new Point(5, 5));
            var second = new Vector(new Point(0, 0), new Point(1, 1));
            var third = new Vector(new Point(0, 0), new Point(10, 10));

            var simpleTree = BuildTree(new List<Vector>() { first, second, third });
            var rootKey = simpleTree.Root.Key;
            var leftKey = simpleTree.Root.Left.Key;
            var rightKey = simpleTree.Root.Right.Key;

            Assert.IsTrue(rootKey.Equals(first) && leftKey.Equals(second) && rightKey.Equals(third));
        }

        [TestMethod]
        public void TestContainsMethod()
        {
            var first = new Vector(new Point(0, 0), new Point(5, 5));
            var second = new Vector(new Point(0, 0), new Point(1, 1));
            var third = new Vector(new Point(0, 0), new Point(10, 10));

            var vectTree = BuildTree(new List<Vector>() { first, second, third });
            var intTree = BuildTree(new List<int> { 5, 3, 7, 5, 2, 8 });

            var testVect = new Vector(new Point(0, 0), new Point(1, 1));
            Assert.IsTrue(intTree.Contains(5) && vectTree.Contains(testVect));
            testVect.X = 100;
            Assert.IsFalse(intTree.Contains(10) || vectTree.Contains(testVect));
        }

        [TestMethod]
        public void TestIEnumerable()
        {
            var tree = BuildTree(new List<int> { 5, 3, 7, 5, 2, 8 });
            var resultKeys = new List<int>();
            foreach (var key in tree)
                resultKeys.Add(key);
            var expected = new List<int>() { 2, 3, 5, 5, 7, 8 };

            var result = true;
            for (var i = 0; i < expected.Count; i++)
            {
                if (expected[i] != resultKeys[i])
                {
                    result = false;
                    break;
                }
            }
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void TestIndexer()
        {
            var tree = BuildTree(new List<int> { 5, 3, 7, 5, 2, 8 });
            Assert.AreEqual(7, tree[2]);
            Assert.AreEqual(8, tree[1]);

            try
            {
                var result = tree[-1];
                Assert.Fail("No exception thrown");

            }
            catch (Exception exeption)
            {
                Assert.IsTrue(exeption is IndexOutOfRangeException);
            }

            try
            {
                var result = tree[10];
                Assert.Fail("No exception thrown");

            }
            catch (Exception exeption)
            {
                Assert.IsTrue(exeption is IndexOutOfRangeException);
            }
        }

        [TestMethod]
        public void TestBalance()
        {
            var tree = BuildTree(new List<int>() { 11, 7, 14, 2, 8, 1, 5, 4 });
            Assert.AreEqual(11, tree.Root.Key);
        }


        public static Tree<T> BuildTree<T>(List<T> keys) where T : IComparable
        {
            var tree = new Tree<T>();
            foreach (var key in keys)
                tree.Add(key);
            return tree;
        }
    }

    public class Vector : IComparable
    {
        public Vector(Point start, Point end)
        {
            X = end.X - start.X;
            Y = end.Y - start.Y;
        }

        public double X;
        public double Y;
        public double Length { get { return Math.Sqrt(X * X + Y * Y); } }

        public int CompareTo(object obj)
        {
            var vector = obj as Vector;
            return this.Length.CompareTo(vector.Length);
        }

        public override bool Equals(object obj)
        {
            var vector = obj as Vector;
            return this.X == vector.X && this.Y == vector.Y;
        }
    }
}