using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diplom
{

    public class MyCollection<TData> : Dictionary<int, TData>, IConnectionCollection<TData>
    { }

    public class BaseHyperGraphManipulation : IBaseHyperGraphManipulation
    {
        private PrimaryGraph _graph = new() 
        { 
            Branches = new MyCollection<Branch>(),
            Nodes = new MyCollection<Node>(),
            Edges = new MyCollection<Edge>(),
            SecondaryGraphs = new MyCollection<SecondaryGraph>(),
        };
        public PrimaryGraph BaseGraph
        {
            get => _graph;
            set => _graph = value;
        }

        public int LastId { private set; get; }
        private int GetId()
        {
            return ++LastId;
        }

        public SecondaryGraph AddSecondaryGraph()
        {
            return AddSecondaryGraph(CreateSecondaryGraph());
        }

        public SecondaryGraph AddSecondaryGraph(SecondaryGraph secondaryGraph)
        {
            secondaryGraph.Id = GetId();
            secondaryGraph.PrimaryGraph = _graph;

            _graph.SecondaryGraphs.Add(secondaryGraph.Id, secondaryGraph);

            return secondaryGraph;
        }
        public Branch AddBranch()
        {
            return AddBranch(CreateBranch());
        }
        public Branch AddBranch(Branch branch)
        {
            branch.Id = GetId();
            branch.PrimaryGraph = _graph;

            _graph.Branches.Add(branch.Id, branch);
            foreach (var i in branch.Edges)
            {
                i.Value.Branches.Add(branch.Id, branch);
            }

            branch.First.Branches.Add(branch.Id, branch);
            branch.Second.Branches.Add(branch.Id, branch);

            branch.SecondaryGraph.Branches.Add(branch.Id, branch);

            return branch;
        }

        public Node AddNode()
        {
            return AddNode(CreateNode());
        }

        public Node AddNode(Node node)
        {
            node.Id = GetId();
            node.PrimaryGraph = _graph;

            _graph.Nodes.Add(node.Id, node);

            return node;
        }

        public Edge AddEdge(Node first, Node second)
        {
            return AddEdge(CreateEdge(first, second));
        }

        public Edge AddEdge(Edge edge)
        {
            edge.Id = GetId();
            edge.PrimaryGraph = _graph;

            edge.First.Edges.Add(edge.Id, edge);
            edge.Second.Edges.Add(edge.Id, edge);

            _graph.Edges.Add(edge.Id, edge);

            return edge;
        }

        public Edge CreateEdge()
        {
            return new Edge()
            {
                Branches = new MyCollection<Branch>(),
                PrimaryGraph = _graph,
            };
        }

        public Edge CreateEdge(Node first, Node second)
        {
            Edge edge = CreateEdge();
            edge.First = first;
            edge.Second = second;

            return edge;
        }

        public Node CreateNode()
        {
            return new Node()
            {
                Edges = new MyCollection<Edge>(),
                Branches = new MyCollection<Branch>(),
                PrimaryGraph = _graph,
            };
        }

        public SecondaryGraph CreateSecondaryGraph()
        {
            return new SecondaryGraph()
            {
                Branches = new MyCollection<Branch>(),
                Nodes = new MyCollection<Node>(),
            };
        }

        public Branch CreateBranch()
        {
            return new Branch()
            {
                Edges = new MyCollection<Edge>(),
            };
        }

        public Branch CreateBranch(SecondaryGraph secondaryGraph)
        {
            return new Branch()
            {
                Edges = new MyCollection<Edge>(),
                SecondaryGraph = secondaryGraph,
                PrimaryGraph = _graph
            };
        }

        public Branch CreateBranch(Node first, Node second)
        {
            return new Branch()
            {
                Edges = new MyCollection<Edge>(),
                First = first,
                Second = second,
            };
        }

        public Branch CreateBranch(Node first, Node second, SecondaryGraph secondaryGraph)
        {
            return new Branch()
            {
                Edges = new MyCollection<Edge>(),
                SecondaryGraph = secondaryGraph,
                PrimaryGraph = _graph,
                First = first,
                Second = second,
            };
        }

        public void RemoveNode(Node node)
        {
            BaseGraph.Nodes.Remove(node.Id);
            var edges = BaseGraph.Edges.Values.Where(i => i.First == node || i.Second == node);
            var braches = BaseGraph.Branches.Values.Where(i => i.First == node || i.Second == node);

            foreach (var i in edges)
            {
                BaseGraph.Edges.Remove(i.Id);

                foreach (var j in i.Branches.Values)
                {
                    BaseGraph.Branches.Remove(j.Id);
                    j.SecondaryGraph.Branches.Remove(j.Id);
                }
            }

            foreach (var i in braches)
            {
                BaseGraph.Branches.Remove(i.Id);
            }

            foreach (var i in BaseGraph.SecondaryGraphs.Values)
            {
                i.Nodes.Remove(node.Id);
            }

            node.Edges = null;
            node.Branches = null;
            node.PrimaryGraph = null;
        }

        public void RemoveEdge(Edge edge)
        {
            BaseGraph.Edges.Remove(edge.Id);

            foreach (var j in edge.Branches.Values)
            {
                BaseGraph.Branches.Remove(j.Id);
                j.SecondaryGraph.Branches.Remove(j.Id);
            }

            edge.First.Edges.Remove(edge.Id);
            edge.Second.Edges.Remove(edge.Id);

            edge.First = null;
            edge.Second = null;
            edge.Branches = null;
            edge.PrimaryGraph = null;
        }

        public void RemoveBranch(Branch branch)
        {
            BaseGraph.Branches.Remove(branch.Id);

            branch.SecondaryGraph.Branches.Remove(branch.Id);

            branch.First.Branches.Remove(branch.Id);
            branch.Second.Branches.Remove(branch.Id);

            foreach (var i in branch.Edges.Values) 
            {
                i.Branches.Remove(branch.Id);
            }

            branch.First = null;
            branch.Second = null;
            branch.SecondaryGraph = null;
            branch.Edges = null;
            branch.PrimaryGraph = null;
        }

        public void RemoveSecondaryGraph(SecondaryGraph secondaryGraph)
        {
            BaseGraph.SecondaryGraphs.Remove(secondaryGraph.Id);

            foreach (var i in secondaryGraph.Branches.Values)
            {
                RemoveBranch(i);
            }

            secondaryGraph.Nodes = null;
            secondaryGraph.PrimaryGraph = null;
        }
    }
}

