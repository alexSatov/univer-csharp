using System;
using System.Collections;
using System.Collections.Generic;

namespace Generics.BinaryTrees
{
    public class Node<T>
        where T : IComparable
    {
        public T Value { get; }
        public Node<T> Right { get; set; }
        public Node<T> Left { get; set; }

        public Node(T value)
        {
            Value = value;
        }
    }

    public class BinaryTree<T> : IEnumerable<T>
        where T : IComparable
    {
        public T Value => root.Value;
        public Node<T> Right => root.Right;
        public Node<T> Left => root.Left;

        private Node<T> root;

        public void Add(T value)
        {
            if (root == null)
                root = new Node<T>(value);
            else
                AddTo(root, value);
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (root == null) yield break;

            foreach (var value in EnumerateRecursive(root))
                yield return value;
        }

        private IEnumerable<T> EnumerateRecursive(Node<T> node)
        {
            if (node.Left != null)
                foreach (var value in EnumerateRecursive(node.Left))
                    yield return value;

            if (node.Right != null)
                foreach (var value in EnumerateRecursive(node.Right))
                    yield return value;

            yield return node.Value;
        }

        private void AddTo(Node<T> node, T value)
        {
            if (value.CompareTo(node.Value) < 1)
            {
                if (node.Left == null)
                    node.Left = new Node<T>(value);
                else
                    AddTo(node.Left, value);
            }
            else
            {
                if (node.Right == null)
                    node.Right = new Node<T>(value);
                else
                    AddTo(node.Right, value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class BinaryTree
    {
        public static BinaryTree<T> Create<T>(params T[] values) 
            where T : IComparable
        {
            var tree = new BinaryTree<T>();
            foreach (var value in values)
            {
                tree.Add(value);
            }
            return tree;
        }
    }
}
