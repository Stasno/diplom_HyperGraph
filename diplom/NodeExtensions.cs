using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diplom
{
    public static class NodeExtensions
    {

        public static bool IsFirstNodeInEdge(this Node node, Edge edge)
        {
            return node.Id == edge.First.Id;
        }

        public static bool IsEdgeBetweenNodes(this Edge edge, int firstId, int secondId)
        {
            return (edge.First.Id == firstId && edge.Second.Id == secondId)
                || (edge.First.Id == secondId && edge.Second.Id == firstId);
        }

        public static Node GetOtherNodeFromEdge(this Node node, Edge edge)
        {
            return node.Id == edge.First.Id 
                ? edge.Second : edge.First;
        }

        public static Node GetOtherNodeFromBranch(this Node node, Branch branch)
        {
            return node.Id == branch.First.Id
                ? branch.Second : branch.First;
        }

        public static Node MakeCopy(this Node node)
        {
            return new Node()
            {
                Branches = node.Branches,
                Edges = node.Edges,
                Id = node.Id,
                Name = node.Name,
                PrimaryGraph = node.PrimaryGraph,
            };
        }

    }
}
