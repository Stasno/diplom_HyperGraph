using System;
using System.Collections.Generic;

namespace diplom
{

    public interface IConnectionCollection<TData> : IDictionary<int, TData>
    { }

    public interface IConnectionFactory
    {
        IConnectionCollection<TData> CreateConnectionCollection<TData>();
    }

    public class PrimaryGraph
    {
        public IConnectionCollection<Node> Nodes { get; set; }
        public IConnectionCollection<Edge> Edges { get; set; }
        public IConnectionCollection<SecondaryGraph> SecondaryGraphs { get; set; }
        public IConnectionCollection<Branch> Branches { get; set; }

        public int NodeCount { get => Nodes.Count; }

    }

    public class SecondaryGraph
    {
        public int Id { get; set; }
        public IConnectionCollection<Branch> Branches { get; set; }
        public IConnectionCollection<Node> Nodes { get; set; }
        public PrimaryGraph PrimaryGraph { get; set; }
    }

    public class Node
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IConnectionCollection<Edge> Edges { get; set; }
        public IConnectionCollection<Branch> Branches { get; set; }
        public PrimaryGraph PrimaryGraph { get; set; }
    }

    public class Edge
    {
        public int Id { get; set; }
        public long Weight { get; set; }
        public Node First { get; set; }
        public Node Second { get; set; }
        public IConnectionCollection<Branch> Branches { get; set; }
        public PrimaryGraph PrimaryGraph { get; set; }

        public Edge() { }
        public Edge(Node first, Node second) 
        {
            First = first;
            Second = second;
        }

        public string DebugEdgeInfo { get => First.Id + ":" + Second.Id; }
    }

    public class Branch
    {
        public int Id { get; set; }
        public long Weight { get; set; }
        public Node First { get; set; }
        public Node Second { get; set; }
        public IConnectionCollection<Edge> Edges { get; set; }
        public SecondaryGraph SecondaryGraph { get; set; }
        public PrimaryGraph PrimaryGraph { get; set; }


        public string DebugEdgeInfo { get => First.Id + ":" + Second.Id; }
    }


}
