using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diplom
{

    public enum OutputType
    {
        List, // Список смежности
        Matrix, // Матрица инцидентности
        ShortedMatrix
    }
    public static class GraphWriter
    {

        public static async Task WriteGraphToFile(this IBaseHyperGraphManipulation graph, 
            string filepath, 
            OutputType edgeOutputType,
            OutputType branchOutputType)
        {
            using StreamWriter stream = new StreamWriter(filepath);
            string input;

            // пишется кол-во вершин
            stream.WriteLine(graph.BaseGraph.Nodes.Count);

            // пишется тип вывода
            stream.WriteLine(edgeOutputType.ToString());

            // выводится граф в виде OutputType
            switch (edgeOutputType)
            {
                case OutputType.List:
                    {
                        var list = graph.BaseGraph.AsList();
                        stream.WriteLine(list.Count);
                        foreach (var item in list)
                        {
                            stream.WriteLine(item.FirstNode.ToString() + ' '
                                + item.SecondNode.ToString() + ' '
                                + item.Weight.ToString());
                        }
                        break;
                    }
                case OutputType.Matrix:
                    {
                        var matrix = graph.BaseGraph.AsMatrix();

                        StringBuilder output = new(matrix.size*3);

                        foreach (var i in matrix.matrix)
                        {
                            
                            for (int j = 0; j < matrix.size; j++)
                            {
                                if (j == matrix.size - 1)
                                    output.Append(i[j]);
                                else
                                    output.Append(i[j] + " ");
                            }

                            stream.WriteLine(output);
                            output.Clear();
                        }

                        break;
                    }
                default:
                    throw new Exception("Unknown type of edge representation");
            }

            // Кол-во вторичных сетей
            stream.WriteLine(graph.BaseGraph.SecondaryGraphs.Count);

            foreach (var secondaryGraph in graph.BaseGraph.SecondaryGraphs.Values)
            {
                // Тип вывода ветвей
                stream.WriteLine(branchOutputType.ToString());

                // Количества ветвей
                stream.WriteLine(secondaryGraph.Branches.Count);

                foreach (var branch in secondaryGraph.Branches.Values)
                {
                    switch (branchOutputType)
                    {
                        case OutputType.ShortedMatrix:

                            StringBuilder output = new(10 + branch.Edges.Count * 3);

                            output.Append($"{branch.First.Id} {branch.Second.Id} {branch.Weight}: ");

                            var node = branch.First;

                            Edge pastEdge = null;

                            do
                            {
                                output.Append($"{node.Id} ");

                                pastEdge = branch.Edges
                                            .Where(i => (i.Value.First == node || i.Value.Second == node)
                                                && i.Value != pastEdge)
                                            .FirstOrDefault().Value;

                                if (pastEdge == null)
                                {
                                    break;
                                    throw new Exception("TODO: NAME ERROR");
                                }

                                node = node.GetOtherNodeFromEdge(pastEdge);

                            } while (true);

                            //output.Append(branch.Second.Id);

                            stream.WriteLine(output);
                            output.Clear();

                            break;
                        default:
                            throw new Exception("Unknown type of branch representation");
                    }
                }
            }
        }

    }
}
