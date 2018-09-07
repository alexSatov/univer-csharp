using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generics.BinaryTrees
{
    [TestFixture]
    public class BinaryTree_should
    {
        [Test]
        public void BeEmptyWhenCreated()
        {
            CollectionAssert.AreEqual(new int[0], new BinaryTree<int>());
        }

        [Test]
        public void PlaceLesserElementToLeft()
        {
            var tree = new BinaryTree<int>();
            tree.Add(2);
            tree.Add(1);
            Assert.AreEqual(2,tree.Value);
            Assert.AreEqual(1,tree.Left.Value);
        }

        [Test]
        public void PlaceEqualElementToLeft()
        {
            var tree = new BinaryTree<int>();
            tree.Add(2);
            tree.Add(2);
            Assert.AreEqual(2, tree.Value);
            Assert.AreEqual(2, tree.Left.Value);
        }

        [Test]
        public void PlaceGreaterElementToRight()
        {
            var tree = new BinaryTree<int>();
            tree.Add(2);
            tree.Add(3);
            Assert.AreEqual(2, tree.Value);
            Assert.AreEqual(3, tree.Right.Value);
        }

        [Test]
        public void InitializeFromAnArrayAndSort1()
        {
            var tree = BinaryTree.Create(4, 3, 2, 1);
            foreach (var value in tree)
            {
                var i = 0;
            }
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, tree);
        }
    }
}
