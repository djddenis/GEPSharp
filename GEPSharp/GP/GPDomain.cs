using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEPSharp
{
    public class GPDomain : Domain
    {
        public Node Head { get; private set; }

        public List<Node> AllNodes { get; private set; }

        public int MaxDepth { get; private set; }

        public bool Full { get; private set; }

        public GPDomain(int maxDepth, bool fullTree, IEnumerable<Node> nodeSet)
            : base(nodeSet)
        {
            MaxDepth = maxDepth;
            Full = fullTree;
            AllNodes = new List<Node>();
            InitializeData();
            
        }

        protected void InitializeData()
        {
            Node headChoice = null;
            while (headChoice == null || headChoice.IsTerminal)
                headChoice = ((Node)RandomNodeFromSet());
            Head = (Node)headChoice.MakeCopy();
            AllNodes.Add(Head);
            Fill(Head);
        }

        private GPDomain(int maxDepth, List<Node> allNodes, IEnumerable<NodeBase> nodeSet, Node head, bool full)
            :base(nodeSet)
        {
            MaxDepth = maxDepth;
            Head = head;
            Full = full;
            AllNodes = allNodes;
        }

        public override Domain MakeCopy()
        {
            Node copiedHead = (Node)Head.MakeCopy();
            List<Node> copiedAllNodes = new List<Node>();
            Stack<Node> toAdd = new Stack<Node>();
            toAdd.Push(copiedHead);
            while (toAdd.Count > 0)
            {
                Node next = toAdd.Pop();
                copiedAllNodes.Add(next);
                for (int i = 0; i < next.Arity; i++)
                    if(next.Children[i] != null)
                        toAdd.Push(next.Children[i]);
            }
            return new GPDomain(MaxDepth, copiedAllNodes, nodeSet, copiedHead, Full);
        }

        private bool growComplete = false; //For grow initializations
        private void Fill(Node node)
        {
            bool terminalsOnly = false;
            if((!Full && growComplete) || node.Depth + 1 >= MaxDepth)
            {
                terminalsOnly = true;
                growComplete = true;
            }
            for (int i = 0; i < node.Arity; i++)
            {
                if (terminalsOnly)
                {
                    IEnumerable<Node> terminals = nodeSet.Where(j => j.IsTerminal).Cast<Node>();
                    node.Children[i] = (Node)terminals.ElementAt(StaticRandom.Next(terminals.Count())).MakeCopy();
                    AllNodes.Add(node.Children[i]);
                    node.Children[i].Parent = node;
                }
                else
                {
                    node.Children[i] = (Node)RandomNodeFromSet().MakeCopy();
                    AllNodes.Add(node.Children[i]);
                    node.Children[i].Parent = node;
                }
                node.Children[i].Depth = node.Depth + 1;
            }
            for (int i = 0; i < node.Arity; i++) //Loop again for calls to get better grow trees
                if (!node.Children[i].IsTerminal)
                    Fill(node.Children[i]);

            node.SubtreeDepth = node.Arity > 0 ? node.Children.Select(i => i.SubtreeDepth).Max() + 1 : 1;
        }

        internal void ReplaceNode(Node replacement, Node toReplace, int maxGrowDepth)
        {
            if (toReplace == Head)
            {
                Head = replacement;
                UpdateDepth(replacement, 0);
            }
            else
            {
                Node parent = toReplace.Parent;

                if ((parent.Depth + replacement.SubtreeDepth) > maxGrowDepth)
                    return;

                replacement.Parent = parent;
                UpdateDepth(replacement, toReplace.Depth);

                for (int i = 0; i < parent.Arity; i++)
                {
                    if (parent.Children[i] == toReplace)
                    {
                        parent.Children[i] = replacement;
                        break;
                    }
                }

                int treeDepth = replacement.SubtreeDepth;
                while (parent != null)
                {
                    int maxSubtreeDepth = treeDepth;
                    for (int i = 0; i < parent.Arity; i++)
                        if (parent.Children[i].SubtreeDepth > maxSubtreeDepth)
                            maxSubtreeDepth = parent.Children[i].SubtreeDepth;

                    parent.SubtreeDepth = maxSubtreeDepth + 1;
                    parent = parent.Parent;
                }
            }

            AllNodes.Clear();
            Stack<Node> toAdd = new Stack<Node>();
            toAdd.Push(Head);
            while (toAdd.Count > 0)
            {
                Node next = toAdd.Pop();
                AllNodes.Add(next);
                for (int i = 0; i < next.Arity; i++)
                    if(next.Children[i] != null)
                        toAdd.Push(next.Children[i]);
            }
        }

        private void UpdateDepth(Node node, int startDepth)
        {
            node.Depth = startDepth;
            for (int i = 0; i < node.Arity; i++)
                UpdateDepth(node.Children[i], node.Depth + 1);
        }
    }
}
