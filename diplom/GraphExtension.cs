using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diplom
{
    public static class GraphExtension
    {
        public static (long[][] matrix, int size) AsMatrix(this PrimaryGraph graph)
        {
            int size = graph.Nodes.Count;
            long[][] result = new long[size][];

            int index = -1;
            var nodeIndexes = graph.Nodes
                .OrderBy(i => i.Key)
                .ToDictionary(i => i.Key, i => ++index);

            var nodes = graph.Nodes
                .OrderBy(i => i.Key)
                .ToArray();


            for (int i = 0; i < size; i++)
            {
                var node = nodes[i].Value;

                result[i] = new long[size];

                for (int j = 0; j < size; j++)
                {
                    result[i][j] = 0;
                }

                foreach (var edge in node.Edges)
                {
                    var anotherNode = node.GetOtherNodeFromEdge(edge.Value);

                    result[i][nodeIndexes[anotherNode.Id]] = edge.Value.Weight;
                }
            }
            return (result, size);
        }

        public static List<(int FirstNode, int SecondNode, long Weight)> AsList(this PrimaryGraph graph)
        {
            List<(int, int, long)> result = new(graph.Edges.Count);

            var edges = graph.Edges.OrderBy(i => i.Key);

            foreach (var edge in edges)
            {
                result.Add((
                    edge.Value.First.Id, 
                    edge.Value.Second.Id, 
                    edge.Value.Weight));
            }

            return result;
        }

        public static IDictionary<int, (long Value, bool IsVisited)> DijkstrasAlgorithm(this PrimaryGraph graph,
            int FirstNode)
        {

            Dictionary<int, (long Value, bool IsVisited)> nodeValue = new();

            foreach (var node in graph.Nodes)
            {
                nodeValue.Add(node.Key, (-1, false));
            }

            Node findMin()
            {
                Node minNode = null;
                long minValue = -1;

                foreach(var nodes in nodeValue)
                {
                    if (!nodes.Value.IsVisited
                        && (nodes.Value.Value < minValue || minValue == -1)
                        && nodes.Value.Value != -1)
                    {
                        minValue = nodes.Value.Value;
                        minNode = graph.Nodes[nodes.Key];
                    }
                }

                return minNode;
            }

            Node currentNode = graph.Nodes
                .Where(i => i.Value.Id == FirstNode)
                .First()
                .Value;

            while (true)
            {
                var currentNodeValue = nodeValue[currentNode.Id];

                if (nodeValue[currentNode.Id].IsVisited)
                    continue;

                (Node firstNode, Node secondNode, long Weight)[] edgesWithBranches =
                    currentNode.Edges
                        .Select(i => (i.Value.First, i.Value.Second, i.Value.Weight))
                        .Concat(currentNode.Branches
                            .Select(i => (i.Value.First, i.Value.Second, i.Value.Weight)))
                        .OrderBy(i => i.Weight)
                        .ToArray();

                foreach (var i in edgesWithBranches)
                {
                    var otherNode = i.firstNode == currentNode ? i.secondNode : i.firstNode;
                    var otherNodeValue = nodeValue[otherNode.Id];

                    if (otherNodeValue.IsVisited)
                        continue;

                    if (otherNodeValue.Value == -1)
                    {
                        otherNodeValue.Value = currentNodeValue.Value + i.Weight;
                    }
                    else
                    {
                        if (currentNodeValue.Value + i.Weight < otherNodeValue.Value)
                        {
                            otherNodeValue.Value = currentNodeValue.Value + i.Weight;
                        }
                    }

                    nodeValue[otherNode.Id] = otherNodeValue;
                }

                currentNodeValue.IsVisited = true;
                nodeValue[currentNode.Id] = currentNodeValue;

                currentNode = findMin();

                if (currentNode == null)
                {
                    break;
                }
            }

            return nodeValue;
        }

        public static (List<Node> path, long length) GetShortestPath(this PrimaryGraph graph,
            IDictionary<int, (long Value, bool IsVisited)> nodeValues,
            Node nodeFrom,
            Node nodeTo)
        {
            (List<Node> path, long length) result = default;

            result.length = nodeValues[nodeFrom.Id].Value;



            return result;
        }

        public static PrimaryGraph CreatePrimaryGraphFromSecondary<T>(IEnumerable<SecondaryGraph> secondaryGraphs) where T : IBaseHyperGraphManipulation, new()
        {
            IBaseHyperGraphManipulation result = new T();

            int nodeCount = 0;

            var branches = secondaryGraphs.SelectMany(i => i.Branches)
                .Distinct();

            var newNodes = branches.Select(i => i.Value.First)
                .Union(branches.Select(i => i.Value.Second))
                .Distinct()
                .Select(i =>
                {
                    var copy = result.CreateNode();

                    copy.Name = i.Name;
                    copy.Id = i.Id;

                    return copy;
                });

            var newEdges = branches
                .Select(branch =>
                {
                    var firstNode = newNodes.First(i => i.Id == branch.Value.First.Id);
                    var secondNode = newNodes.First(i => i.Id == branch.Value.Second.Id);

                    var edge = result.CreateEdge(firstNode, secondNode);

                    edge.Id = branch.Key;
                    edge.Weight = branch.Value.Weight;

                    result.AddEdge(edge);

                    return edge;
                }).ToList();

            foreach (var node in newNodes)
            {
                result.AddNode(node);
            }

            return result.BaseGraph;
        }

        public static IEnumerable<(SecondaryGraph secondaryGraph, IEnumerable<Branch> branches)> SecondaryGraphConflicts(this PrimaryGraph graph,
            IEnumerable<Edge> edgesToRemove)
        {
            List<(SecondaryGraph secondaryGraph, IEnumerable<Branch> branches)> result = new();

            var branches = edgesToRemove.SelectMany(i => i.Branches.Values).Distinct().GroupBy(i => i.SecondaryGraph);

            foreach (var branch in branches)
            {
                result.Add(new (branch.Key, branch.AsEnumerable()));
            }

            return result;
        }

        public static IDictionary<int, (long Value, bool IsVisited)> СonnectivityСomponent(this PrimaryGraph graph)
        {
            return null;
        }
    }
}
