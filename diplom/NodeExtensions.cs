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

    }
}
