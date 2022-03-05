using diplom;
using HyperGraphLib;

BaseHyperGraphManipulation graph = new();

_ = graph.ReadGraphFromFileAsync(@"C:\Users\stas\Desktop\vsprojects\diplom\ConsoleApp\input.txt").Result;

graph = graph;

graph.WriteGraphToFile(@"C:\Users\stas\Desktop\vsprojects\diplom\ConsoleApp\output.txt", OutputType.List, OutputType.ShortedMatrix).Wait();

var resultDijkstra = graph.BaseGraph.DijkstrasAlgorithm(1);
