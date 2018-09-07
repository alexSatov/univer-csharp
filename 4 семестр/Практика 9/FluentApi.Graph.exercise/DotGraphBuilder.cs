using System;
using System.Linq;
using System.Collections.Generic;

namespace FluentApi.Graph
{
    public enum NodeShape
    {
        Box,
        Ellipse
    }

    public class DotGraphBuilder
    {
        private readonly Graph graph;

        protected DotGraphBuilder(Graph graph)
        {
            this.graph = graph;
        }

        public static DotGraphBuilder DirectedGraph(string graphName)
        {
            return new DotGraphBuilder(new Graph(graphName, true, false));
        }

        public static DotGraphBuilder NondirectedGraph(string graphName)
        {
            return new DotGraphBuilder(new Graph(graphName, false, false));
        }

        public DotGraphNodeAddingBuilder AddNode(string nodeName)
        {
            var nodeBuilder = new DotGraphNodeAddingBuilder(graph, nodeName);
            graph.AddNode(nodeName);
            return nodeBuilder;
        }

        public DotGraphEdgeAddingBuilder AddEdge(string nodeFromName, string nodeToName)
        {
            var edgeBuilder = new DotGraphEdgeAddingBuilder(graph, nodeFromName, nodeToName);
            graph.AddEdge(nodeFromName, nodeToName);
            return edgeBuilder;
        }

        public string Build()
        {
            return graph.ToDotFormat();
        }
    }

    public class DotGraphNodeAddingBuilder : DotGraphBuilder
    {
        private readonly Graph graph;
        private readonly string nodeName;

        public DotGraphNodeAddingBuilder(Graph graph, string nodeName)
            : base(graph)
        {
            this.graph = graph;
            this.nodeName = nodeName;
        }

        public DotGraphBuilder With(Action<NodeBuilder> nodeCustomizer)
        {
            var node = graph.Nodes.First(n => n.Name == nodeName);
            nodeCustomizer(new NodeBuilder(node));
            return this;
        }
    }

    public class DotGraphEdgeAddingBuilder : DotGraphBuilder
    {
        private readonly Graph graph;
        private readonly string nodeFromName;
        private readonly string nodeToName;

        public DotGraphEdgeAddingBuilder(Graph graph, string nodeFromName, string nodeToName)
            : base(graph)
        {
            this.graph = graph;
            this.nodeFromName = nodeFromName;
            this.nodeToName = nodeToName;
        }

        public DotGraphBuilder With(Action<EdgeBuilder> edgeCustomizer)
        {
            var edge = graph.Edges.First(n => n.SourceNode == nodeFromName && n.DestinationNode == nodeToName);
            edgeCustomizer(new EdgeBuilder(edge));
            return this;
        }
    }

    public class GraphElementBuilder<T>
        where T : class
    {
        protected Dictionary<string, string> Attributes = new Dictionary<string, string>();

        public T Color(string color)
        {
            Attributes["Color"] = color;
            return this as T;
        }

        public T FontSize(int fontSize)
        {
            Attributes["fontsize"] = fontSize.ToString();
            return this as T;
        }

        public T Label(string label)
        {
            Attributes["label"] = label;
            return this as T;
        }
    }

    public class NodeBuilder : GraphElementBuilder<NodeBuilder>
    {
        public NodeBuilder(GraphNode node)
        {
            Attributes = node.Attributes;
        }

        public NodeBuilder Shape(NodeShape shape)
        {
            Attributes["shape"] = shape.ToString().ToLower();
            return this;
        }
    }

    public class EdgeBuilder : GraphElementBuilder<EdgeBuilder>
    {
        public EdgeBuilder(GraphEdge edge)
        {
            Attributes = edge.Attributes;
        }

        public EdgeBuilder Weight(int weight)
        {
            Attributes["weight"] = weight.ToString();
            return this;
        }
    }
}