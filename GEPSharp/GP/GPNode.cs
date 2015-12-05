using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEPSharp
{
    public class Node : NodeBase
    {
        public Node[] Children { get; private set; }

        public Node Parent { get; set; }

        public int Depth { get; set; }

        public int SubtreeDepth { get; set; }

        public Node(string name, object data, Node parent = null)
            : base(name, data)
        {
            Setup(parent);
        }

        public Node(string name, int arity, NodeFunction function, Node parent = null)
            :base(name, arity, function)
        {
            Setup(parent);
        }

        private Node(string name, int arity, NodeFunction function, int depth, int subtreeDepth, Node[] children, Node parent)
            :base(name, arity, function)
        {
            Depth = depth;
            Children = children;
            SubtreeDepth = subtreeDepth;
            Parent = parent;
            for (int i = 0; i < Arity; i++)
                if(Children[i] != null)
                    Children[i].Parent = this;
        }

        public override NodeBase MakeCopy()
        {
            Node[] copiedChildren = new Node[Arity];
            for (int i = 0; i < Arity; i++)
                copiedChildren[i] = Children[i] == null ? null : (Node)Children[i].MakeCopy();
            return new Node(Name, Arity, function, Depth, SubtreeDepth, copiedChildren, Parent);
        }

        private void Setup(Node parent)
        {
            Children = new Node[Arity];
            Depth = 0;
            SubtreeDepth = Arity > 0 ? 2 : 1;
            Parent = parent;
        }
    }
}
