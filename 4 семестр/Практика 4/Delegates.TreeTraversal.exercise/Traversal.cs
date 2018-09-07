using System;
using System.Collections.Generic;

namespace Delegates.TreeTraversal
{
    public static class Traversal
    {
        private static IEnumerable<TResult> Traverse<TTreeNode, TResult>(
            TTreeNode root, Func<TTreeNode, IEnumerable<TTreeNode>> getConnectedNodes, Func<TTreeNode, IEnumerable<TResult>> selector)
        {
            var stack = new Stack<TTreeNode>();
            stack.Push(root);
            while (stack.Count != 0)
            {
                var node = stack.Pop();
                if (node == null) continue;

                foreach (var connectedNode in getConnectedNodes(node))
                {
                    stack.Push(connectedNode);
                }

                var result = selector(node);
                if (result == null) continue;

                foreach (var value in result)
                {
                    yield return value;
                }
            }
        }

        public static IEnumerable<int> GetBinaryTreeValues(BinaryTree<int> tree)
        {
            return Traverse(
                tree,
                node => new [] { node.Left, node.Right }, 
                node => new [] { node.Value });
        }

        public static IEnumerable<Job> GetEndJobs(Job mainJob)
        {
            return Traverse(
                mainJob,
                job => job.Subjobs,
                job => job.Subjobs.Count == 0 ? new [] { job } : null);
        }

        public static IEnumerable<Product> GetProducts(ProductCategory mainCategory)
        {
            return Traverse(
                mainCategory,
                category => category.Categories,
                category => category.Products);
        }
    }
}
