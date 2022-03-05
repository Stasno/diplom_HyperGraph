using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diplom
{

    public interface IBaseHyperGraphManipulation
    {

        PrimaryGraph BaseGraph { get; set; }

        Node CreateNode();

        Edge CreateEdge();
        Edge CreateEdge(Node first, Node second);

        SecondaryGraph CreateSecondaryGraph();

        Branch CreateBranch();
        Branch CreateBranch(Node first, Node second);
        Branch CreateBranch(SecondaryGraph secondaryGraph);
        Branch CreateBranch(Node first, Node second, SecondaryGraph secondaryGraph);

        Node AddNode();
        Node AddNode(Node node);

        Edge AddEdge(Node first, Node second);
        Edge AddEdge(Edge edge);

        SecondaryGraph AddSecondaryGraph();
        SecondaryGraph AddSecondaryGraph(SecondaryGraph secondaryGraph);
        //SecondaryGraph AddSecondaryGraphWithBrances(SecondaryGraph secondaryGraph);

        Branch AddBranch();
        Branch AddBranch(Branch branch);


        void RemoveNode(Node node);
        void RemoveEdge(Edge edge);
        void RemoveBranch(Branch branch);
        void RemoveSecondaryGraph(SecondaryGraph secondaryGraph);

    }
}
