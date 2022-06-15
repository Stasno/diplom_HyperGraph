using diplom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperGraphLib
{
    public static class GraphReader
    {

        private static int currentLine = 0;

        private static Edge ItemsToEdge(Node firstItem, Node secondItem, long weight, IBaseHyperGraphManipulation graph)
        {
            Edge edge = graph.CreateEdge(firstItem, secondItem);
            edge.Weight = weight;

            //graph.AddEdge(edge);
            return edge;
        }

        private static Branch ItemsToBranch(Node firstItem, Node secondItem,SecondaryGraph secondaryGraph, long weight, IBaseHyperGraphManipulation graph)
        {
            Branch branch = graph.CreateBranch(firstItem, secondItem, secondaryGraph);
            branch.Weight = weight;

            //graph.AddBranch(branch);
            return branch;
        }

        public static async Task<PrimaryGraph> ReadGraphFromFileAsync(this IBaseHyperGraphManipulation graph, string filepath)
        {
            currentLine = 0;
            //var graph = new PrimaryGraph();
            using StreamReader stream = new(filepath);
            string input;
            int size;

            List<SecondaryGraph> secondaryGraphs = new();
            List<Node> nodes = new();
            List<Edge> edges = null;
            List<Branch> branches = new();

            // чтение кол-во вершин
            input = await ReadFormatedLineAsync(stream);
            if (int.TryParse(input, out size) == false)
            {
                ThrowException("Unexpected number");
            }

            for (int i = 0; i < size; i++)
            {
                nodes.Add(graph.CreateNode());
                graph.AddNode(nodes[i]);
            }

            // чтение типа входных данных (список смежности или матрица инцедентности)
            input = await ReadFormatedLineAsync(stream);
            switch (input)
            {
                case "list":
                    {
                        edges = await ReadEdgesList(stream, graph, nodes);

                        break;
                    }
                case "matrix":
                    {
                        edges = await ReadEdgeMatrix(stream, size, graph, nodes);

                        break;
                    }
                default:
                    ThrowException("Wrong input type, expected 'list' or 'matrix'");
                    break;
            }

            edges.ForEach(i => graph.AddEdge(i));

            // чтение кол-во подграфов
            input = await ReadFormatedLineAsync(stream);
            if (int.TryParse(input, out size) == false)
            {
                ThrowException("Unexpected number");
            }

            int subgraphCount = size;

            for (int i = 0; i < size; i++)
            {
                var result = await ReadSecondaryGraph(stream, subgraphCount, graph, nodes);
                graph.AddSecondaryGraph(result);
            }

            return graph.BaseGraph;
        }

        private static async Task<List<Edge>> ReadEdgeMatrix(StreamReader stream, int length, 
            IBaseHyperGraphManipulation graph,
            List<Node> nodes)
        {
            string input;

            List<Edge> result = new();

            for (int i = 0; i < length; i++)
            {

                input = await ReadFormatedLineAsync(stream);

                var inputArray = SplitStringToLongArray(input);

                if (inputArray.Length != length)
                {
                    ThrowException("Matrix array must have length " + length + ", insted of " + inputArray.Length);
                }

                for (int j = 0; j < length; j++)
                {
                    if (inputArray[j] > 0)
                    {
                        result.Add(ItemsToEdge(nodes[i], nodes[j], inputArray[j], graph));
                    }
                }

            }

            return result;
        }

        private static async Task<List<Edge>> ReadEdgesList(StreamReader stream, 
            IBaseHyperGraphManipulation graph,
            List<Node> nodes)
        {
            string input;
            List<Edge> result = new();

            input = await ReadFormatedLineAsync(stream);

            // Чтение длины списка смежности
            if (int.TryParse(input, out var length) == false)
            {
                ThrowException("Number expected");
            }

            // Чтение списка смежности в формате:  "{первая вершина} {вторая вершина} {вес ребра}"
            for (int i = 0; i < length; i++)
            {
                input = await ReadFormatedLineAsync(stream);

                var array = SplitStringToIntArray(input);

                if (array.Length != 3)
                {
                    ThrowException("List length too small");
                }

                if (array.Length != 3)
                {
                    ThrowException("List must have 2 or 3 digits");
                }

                if (!(array[0] > 0 && array[0] <= nodes.Count &&
                    array[1] > 0 && array[1] <= nodes.Count))
                {
                    ThrowException("Node id must be between 0 or " + nodes.Count);
                }

                long weight = array[2];
                int firstNode = array[0] - 1;
                int secondNode = array[1] - 1;

                result.Add(ItemsToEdge(nodes[firstNode], nodes[secondNode], weight, graph));
            }

            return result;
        }

        private static async Task<SecondaryGraph> ReadSecondaryGraph(StreamReader stream, 
            int length, 
            IBaseHyperGraphManipulation graph,
            List<Node> nodes)
        {
            SecondaryGraph result = graph.CreateSecondaryGraph();

            string input;
            int size;

            List<Branch> branches = null;

            // Чтение типа ввода ветвей (список/матрица) 
            input = await ReadFormatedLineAsync(stream);
            switch (input)
            {
                case "list":
                    {
                        branches = await ReadBranchesList(stream, graph, nodes, result);
                        break;
                    }
                case "shortedmatrix":
                    {
                        branches = await ReadBranchesMatrix(stream, graph, nodes, result);
                        break;
                    }
                default:
                    ThrowException("Wrong input type, expected 'list' or 'shortedmatrix'");
                    break;
            }

            branches.ForEach(i => graph.AddBranch(i));

            return result;
        }

        private static async Task<List<Branch>> ReadBranchesList(StreamReader stream,
            IBaseHyperGraphManipulation graph,
            List<Node> nodes,
            SecondaryGraph secondaryGraph)
        {
            string input;

            input = await ReadFormatedLineAsync(stream);

            // Чтение количества ветвей
            if (int.TryParse(input, out var length) == false)
            {
                ThrowException("Unexpected number");
            }

            List<Branch> result = new();

            // Чтение ветвей в формате: "{первая  вершина} {вторая вершина} {вес ветви}"
            for (int i = 0; i < length; i++)
            {
                input = await ReadFormatedLineAsync(stream);

                var array = SplitStringToIntArray(input);

                if (array.Length != 3)
                {
                    ThrowException("List must have 2 or 3 digits");
                }

                if (!(array[0] > 0 || array[0] < nodes.Count ||
                    array[1] > 0 || array[1] < nodes.Count))
                {
                    ThrowException("Node id must be between 0 or " + nodes.Count);
                }

                long weight = array[2];

                int firstNode = array[0] - 1;
                int secondNode = array[1] - 1;

                result.Add(ItemsToBranch(nodes[firstNode], nodes[secondNode], secondaryGraph, weight, graph));
            }

            // Чтение длины списка смежности
            input = await ReadFormatedLineAsync(stream);
            if (int.TryParse(input, out length) == false)
            {
                ThrowException("Unexpected number");
            }

            // Формат чтения ребер ветви: "{номер ветви} {первая вершина} {вторая вершина}"
            for (int i = 0; i < length; i++)
            {
                input = await ReadFormatedLineAsync(stream);

                var array = SplitStringToIntArray(input);

                if (array.Length != 3)
                {
                    ThrowException("List must have 2 or 3 digits");
                }

                if (!(array[0] > 0 || array[0] <= result.Count ||
                    array[1] > 0 || array[1] <= nodes.Count ||
                    array[2] > 0 || array[2] <= nodes.Count))
                {
                    ThrowException("Node id must be between 0 or " + nodes.Count);
                }

                var firstNode = nodes[array[1] - 1];
                var secondNode = nodes[array[2] - 1];

                var edge = firstNode.Edges
                    .Where(i => i.Value.First == secondNode || i.Value.Second == secondNode)
                    .Select(i => i.Value)
                    .FirstOrDefault();

                if (edge == null)
                {
                    ThrowException($"Edge between nodes {array[1]} and {array[2]} not found");
                }

                var branchNumber = array[0] - 1;

                result[branchNumber].Edges.Add(edge.Id, edge);

                secondaryGraph.Nodes.TryAdd(firstNode.Id, firstNode);
                secondaryGraph.Nodes.TryAdd(secondNode.Id, secondNode);
            }

            return result;
        }

        private static async Task<List<Branch>> ReadBranchesMatrix(StreamReader stream,
            IBaseHyperGraphManipulation graph,
            List<Node> nodes,
            SecondaryGraph secondaryGraph)
        {
            string input;

            // Чтение количества ветвей
            input = await ReadFormatedLineAsync(stream);
            if (int.TryParse(input, out var length) == false)
            {
                ThrowException("Unexpected number");
            }

            List<Branch> result = new();

            // Чтение ветвей в формате: " {первая вершина} {вторая вершина} {вес} : {Перечисление ребер ... (1, 2, 3, 4, 5)} " 
            // Есть краткая форма, состоящая из одного ребра " {первая вершина} {вторая вершина} {вес} "
            for (int i = 0; i < length; i++)
            {
                input = await ReadFormatedLineAsync(stream);

                var inputArray = input.Split(':', StringSplitOptions.RemoveEmptyEntries);

                if (inputArray.Length <= 0 || inputArray.Length > 2)
                {
                    ThrowException("Wrong matrix format");
                }

                // чтение и добавление ветви
                var longArray = SplitStringToLongArray(inputArray[0]);

                int firstNode = (int)longArray[0] - 1;
                int secondNode = (int)longArray[1] - 1;
                long weight = longArray[2];

                result.Add(ItemsToBranch(nodes[firstNode], nodes[secondNode], secondaryGraph, weight, graph));

                if (inputArray.Length == 1)
                {
                    var edge = nodes[firstNode].Edges
                        .Where(i => i.Value.First == nodes[firstNode] && i.Value.Second == nodes[secondNode]
                        || i.Value.Second == nodes[firstNode] && i.Value.First == nodes[secondNode])
                        .Select(i => i.Value)
                        .FirstOrDefault();

                    var AddedBranch = result[result.Count - 1];
                    AddedBranch.Edges.Add(edge.Id, edge);
                    continue;
                }

                // чтение пути ветви
                var intArray = SplitStringToIntArray(inputArray[1]);

                if (intArray.Length < 2)
                {
                    ThrowException("Branch path too small");
                }

                for (int j = 0; j < intArray.Length - 1; j++)
                {
                    int firstEdgeNode = intArray[j] - 1;
                    int secondEdgeNode = intArray[j + 1] - 1;

                    var edge = nodes[firstEdgeNode].Edges
                        .Where(i => i.Value.First == nodes[firstEdgeNode] && i.Value.Second == nodes[secondEdgeNode]
                        || i.Value.Second == nodes[firstEdgeNode] && i.Value.First == nodes[secondEdgeNode])
                        .Select(i => i.Value)
                        .FirstOrDefault();

                    if (edge == null)
                    {
                        ThrowException($"Edge between nodes {firstEdgeNode + 1} and {secondEdgeNode + 1} not found");
                    }

                    var AddedBranch = result[result.Count - 1];

                    try
                    {
                        AddedBranch.Edges.Add(edge.Id, edge);
                    } catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    secondaryGraph.Nodes.TryAdd(secondEdgeNode, edge.First.GetOtherNodeFromEdge(edge));
                }

                secondaryGraph.Nodes.TryAdd(intArray[0] - 1, nodes[intArray[0] - 1]);
                secondaryGraph.Nodes.TryAdd(firstNode, nodes[firstNode]);
                secondaryGraph.Nodes.TryAdd(secondNode, nodes[secondNode]);
            }

            return result;
        }

        private static int[] SplitStringToIntArray(string str)
        {
            int[] result;
            var splitedInput = str.Split(new char[] { ',', ' ', '.' },
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            result = Array.ConvertAll(splitedInput, s => {
                if (int.TryParse(s, out var result) == false)
                {
                    ThrowException("Except number");
                }
                return result;
            });

            return result;
        }

        private static long[] SplitStringToLongArray(string str)
        {
            long[] result;
            //var splitedInput = str.Split(new char[] { ',', ' ', '.', });
            var splitedInput = str.Split(new char[] { ',', ' ', '.' },
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            result = Array.ConvertAll(splitedInput, s => {
                if (long.TryParse(s, out var result) == false)
                {
                    ThrowException("Except number");
                }
                return result;
            });

            return result;
        }

        private static async Task<string> ReadFormatedLineAsync(StreamReader stream)
        {
            string input;

            do
            {
                currentLine += 1;
                input = await stream.ReadLineAsync();
                int commentIndex = input.IndexOf("//");
                if (commentIndex != -1)
                    input = input.Substring(0, commentIndex);
            } while (string.IsNullOrWhiteSpace(input));

            if (input == null)
            {
                ThrowException("Not enought data in file");
            }

            return (input).Trim().ToLower();
        }

        private static Exception ThrowException(string str)
        {
            throw new Exception(str + ". Line: " + currentLine);
        }
    }
}
