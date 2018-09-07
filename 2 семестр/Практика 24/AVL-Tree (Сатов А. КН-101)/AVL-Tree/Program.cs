using System;
using System.Collections.Generic;
using System.Collections;

namespace AVLTree
{   
    public class TreeNode<T>
        where T : IComparable
    {
        public T Key { get; set; }
        public TreeNode<T> Left { get; set; }
        public TreeNode<T> Right { get; set; }
        public TreeNode<T> Parent { get; set; }
        public int Height { get; set; }
        public int BFactor { get {
                var leftH = Left != null ? Left.Height : 0;
                var rightH = Right != null ? Right.Height : 0;
                return rightH - leftH; } }

        public TreeNode(T key)
        {
            Key = key;
            Height = 1;
        }

        public void FixHeight()
        {
            var leftH = Left != null ? Left.Height : 0;
            var rightH = Right != null ? Right.Height : 0;
            Height = leftH > rightH ? leftH : rightH + 1;
        }
    }

    public class Tree<T> : IEnumerable
        where T : IComparable
    {
        public TreeNode<T> Root { get; set; }
        public int TotalKeys { get; set; }

        public bool Contains(T key)
        {
            var current = Root;
            while (current != null)
            {
                if (current.Key.CompareTo(key) == 0)
                    return true;
                else if (current.Key.CompareTo(key) > 0)
                    current = current.Left;
                else
                    current = current.Right;
            }
            return false;
        }

        public TreeNode<T> Add(T key)
        {
            var newNode = new TreeNode<T>(key);
            if (Root == null)
                Root = newNode;
            else
            {
                var current = Root;
                while (true)
                {
                    if (key.CompareTo(current.Key) < 0)
                    {
                        if (current.Left == null)
                        {
                            current.Left = newNode;
                            newNode.Parent = current;
                            break;
                        }
                        current = current.Left;
                    }
                    else
                    {
                        if (current.Right == null)
                        {
                            current.Right = newNode;
                            newNode.Parent = current;
                            break;
                        }
                        current = current.Right;
                    }
                }

            }
            TotalKeys++;
            Balance(newNode);          
            return newNode;
        }

        private void Balance(TreeNode<T> node)
        {
            while (node != null)
            {
                node.FixHeight();
                if (node.BFactor == 2)
                {
                    if (node.Right.BFactor < 0)
                        RotateRight(node.Right);
                    RotateLeft(node);
                }
                if (node.BFactor == -2)
                {
                    if (node.Left.BFactor > 0)
                        RotateLeft(node.Left);
                    RotateRight(node);
                }
                node = node.Parent;
            }
        }

        private void RotateRight(TreeNode<T> pivot)
        {
            var current = pivot.Left;
            current.Parent = pivot.Parent;
            if (current.Parent == null)
                Root = current;
            else
            {
                if (current.Parent.Left == pivot)
                    current.Parent.Left = current;
                else if (current.Parent.Right == pivot)
                    current.Parent.Right = current;
            }
            pivot.Left = current.Right;
            if (pivot.Left != null)
                pivot.Left.Parent = pivot;
            current.Right = pivot;
            pivot.Parent = current;
            pivot.FixHeight();
            current.FixHeight();

        }

        private void RotateLeft(TreeNode<T> pivot)
        {
            var current = pivot.Right;
            current.Parent = pivot.Parent;
            if (current.Parent == null)
                Root = current;
            else
            {
                if (current.Parent.Left == pivot)
                    current.Parent.Left = current;
                else if (current.Parent.Right == pivot)
                    current.Parent.Right = current;
            }
            pivot.Right = current.Left;
            if (pivot.Right != null)
                pivot.Right.Parent = pivot;
            current.Left = pivot;
            pivot.Parent = current;
            pivot.FixHeight();
            current.FixHeight();

        }

        public void Print(TreeNode<T> node)
        {
            Console.WriteLine(node.Key);
            if (node.Left != null)
                Print(node.Left);
            if (node.Right != null)
                Print(node.Right);
        }

        private IEnumerable<T> Sorted()
        {
            if (Root == null)
                yield break;

            var stack = new Stack<TreeNode<T>>();
            var current = Root;

            while (stack.Count > 0 || current != null)
            {
                if (current == null)
                {
                    current = stack.Pop();
                    yield return current.Key;
                    current = current.Right;
                }
                else
                {
                    stack.Push(current);
                    current = current.Left;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Sorted().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private int TreeSize(TreeNode<T> node)
        {
            if (node == null)
                return 0;
            return 1 + TreeSize(node.Left) + TreeSize(node.Right);
        }

        private T FindElement(TreeNode<T> node, int k)
        {
            if (node == null)
                return default(T);
            int toLeft = TreeSize(node.Left);
            if (toLeft == k - 1)
                return node.Key;
            else if (toLeft > k - 1)
                return FindElement(node.Right, k);
            else
                return FindElement(node.Right, k - toLeft - 1);
        }


        public T this[int index]
        {
            get
            {
                if (index > TotalKeys || index < 0)
                    throw new IndexOutOfRangeException();
                var result = FindElement(Root, index);
                return result;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {            
        }
    }
}
