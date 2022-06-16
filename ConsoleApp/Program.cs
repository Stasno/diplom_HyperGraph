using diplom;
using HyperGraphLib;

BaseHyperGraphManipulation graph = new();


_ = graph.ReadGraphFromFileAsync(@"C:\Users\user\Desktop\vsprojects\diplom\ConsoleApp\testinput2.txt").Result;

//graph.RemoveSecondaryGraph(graph.BaseGraph.SecondaryGraphs.First().Value);
graph.RemoveBranch(graph.BaseGraph.Branches.ElementAt(1).Value);

graph.BaseGraph.WriteGraphToFile(@"C:\Users\user\Desktop\vsprojects\diplom\ConsoleApp\output.txt", OutputType.List, OutputType.ShortedMatrix).Wait();

var newGraph = GraphExtension.CreatePrimaryGraphFromSecondary<BaseHyperGraphManipulation>(graph.BaseGraph.SecondaryGraphs.Values);

newGraph.WriteGraphToFile(@"C:\Users\user\Desktop\vsprojects\diplom\ConsoleApp\output2.txt", OutputType.List, OutputType.ShortedMatrix).Wait();

graph.BaseGraph.SecondaryGraphConflicts(graph.BaseGraph
    .Edges
    .Values
    .Where(i => i.IsEdgeBetweenNodes(1, 6)
        || i.IsEdgeBetweenNodes(3, 4))
    .ToList());

var resultDijkstra = graph.BaseGraph.DijkstrasAlgorithm(2);

