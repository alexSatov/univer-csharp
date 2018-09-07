using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace PudgeClient
{
    public enum NodeType
    {
        empty = 1,
        ambush = 1,
        rune = 0,
    };

    public class Edge
    {
        public readonly Node From;
        public readonly Node To;
        public readonly Vector Vector;
        public double Weight { get { return (double)To.Type * this.Vector.Length; } }

        public Edge(Node first, Node second)
        {
            this.From = first;
            this.To = second;
            this.Vector = new Vector(first, second);
        }

        public bool IsIncident(Node node)
        {
            return From == node || To == node;
        }

        public Node OtherNode(Node node)
        {
            if (!IsIncident(node)) throw new ArgumentException();
            if (From == node) return To;
            return From;
        }

        public double getWeight()
        {
            return this.Vector.Length * (double)To.Type;
        }
    }

    public class Node
    {
        public List<Edge> Edges = new List<Edge>();
        public Point Position { get; set; }
        public NodeType Type { get; set; }

        public Node(Point point, string nodeType)
        {
            Position = point;
            switch (nodeType)
            {
                case "empty":
                    Type = NodeType.empty;
                    break;
                case "rune":
                    Type = NodeType.rune;
                    break;
                case "ambush":
                    Type = NodeType.ambush;
                    break;
            }
        }

        public Node(Point point)
        {
            Position = point;
        }

        public IEnumerable<Node> IncidentNodes
        {
            get
            {
                return Edges.Select(z => z.OtherNode(this));
            }
        }

        public IEnumerable<Edge> IncidentEdges
        {
            get
            {
                foreach (var e in Edges) yield return e;
            }
        }

        public static void Connect(Node node1, Node node2, Graph graph)
        {
            if (!graph.Nodes.Contains(node1) || !graph.Nodes.Contains(node2)) throw new ArgumentException();
            var edge1 = new Edge(node1, node2);
            var edge2 = new Edge(node2, node1);
            node1.Edges.Add(edge1);
            node2.Edges.Add(edge2);
        }

        public static void Disconnect(Edge edge)
        {
            edge.From.Edges.Remove(edge);
            edge.To.Edges.Remove(edge);
        }


        public override bool Equals(object obj)
        {
            return this.Position.X == ((Node)obj).Position.X && this.Position.Y == ((Node)obj).Position.Y;
        }
    }

    public class Graph
    {
        public Node[] Nodes { get; set; }

        public Graph(List<Node> nodes)
        {
            Nodes = nodes.ToArray();
        }

        public int Length { get { return Nodes.Length; } }

        public Node this[int index] { get { return Nodes[index]; } }

        public void Connect(int index1, int index2)
        {
            Node.Connect(Nodes[index1], Nodes[index2], this);
        }

        class DijkstraData
        {
            public Node Previous { get; set; }
            public double Price { get; set; }
        }

        public static List<Node> Dijkstra(Graph graph, Dictionary<Edge, double> weights, Node start, Node end)
        {
            var notVisited = graph.Nodes.ToList();
            var track = new Dictionary<Node, DijkstraData>();
            track[start] = new DijkstraData { Price = 0, Previous = null };

            while (true)
            {
                Node toOpen = null;
                var bestPrice = double.PositiveInfinity;
                foreach (var e in notVisited)
                {
                    if (track.ContainsKey(e) && track[e].Price < bestPrice)
                    {
                        bestPrice = track[e].Price;
                        toOpen = e;
                    }
                }

                if (toOpen == null) return null;
                if (toOpen.Equals(end))
                {
                    end = toOpen;
                    break;
                }

                foreach (var e in toOpen.IncidentEdges.Where(z => z.From == toOpen))
                {
                    var currentPrice = track[toOpen].Price + weights[e];
                    var nextNode = e.OtherNode(toOpen);
                    if (!track.ContainsKey(nextNode) || track[nextNode].Price > currentPrice)
                    {
                        track[nextNode] = new DijkstraData { Previous = toOpen, Price = currentPrice };
                    }
                }

                notVisited.Remove(toOpen);
            }

            var result = new List<Node>();
            while (end != null)
            {
                result.Add(end);
                end = track[end].Previous;
            }
            result.Reverse();
            return result;
        }

        public Node getNodeByLocation(Point position)
        {
            foreach (var node in Nodes)
                if (node.Position == position)
                    return node;

            return null;
        }
    }
}

