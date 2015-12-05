using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEPSharp
{
    public class GPIndividual : Individual
    {
        public GPDomain Tree { get; private set; }

        public GPIndividual(int maxDepth, bool fullTree, IEnumerable<Node> nodeTypes)
            : base(new GPDomain(maxDepth, fullTree, nodeTypes))
        {
            Tree = (GPDomain)Domains.First();
        }

        private GPIndividual(IEnumerable<Domain> domains, bool evaluated, object answer, double fitness)
            :base(domains, evaluated, answer, fitness)
        {
            Tree = (GPDomain)Domains.First();
        }

        public override Individual MakeCopy()
        {
            return new GPIndividual(new Domain[] { Tree.MakeCopy() }, evaluated, answer, Fitness);
        }

        protected override object Evaluate(bool useAllCachedSolutions = false)
        {
            var headSolution = EvaluateNode(Tree.Head);
            if (useAllCachedSolutions)
            {
                List<Tuple<object, NodeBase>> solutions = new List<Tuple<object, NodeBase>>();
                foreach (Node node in Tree.AllNodes)
                    if (node.Evaluated)
                        solutions.Add(new Tuple<object, NodeBase>(node.Value(null, true), node));
                return solutions;
            }
            return headSolution;
        }

        private object EvaluateNode(Node node)
        {
            if (node.IsTerminal)
                return node.Value(null);

            object[] parameters = new object[node.Arity];
            for (int i = 0; i < node.Arity; i++)
                parameters[i] = EvaluateNode(node.Children[i]);
            return node.Value(parameters);
        }

        internal Node DonateSubtree()
        {
            int index = StaticRandom.Next(Tree.AllNodes.Count);
            return (Node)Tree.AllNodes[index].MakeCopy();
        }

        internal void ReceiveSubtree(Node replacement, int maxGrowDepth)
        {
            int index = StaticRandom.Next(Tree.AllNodes.Count);
            Tree.ReplaceNode(replacement, Tree.AllNodes[index], maxGrowDepth);
            answer = null;
            evaluated = false;
        }

        public override string ToString()
        {
            string ret = "";
            List<Node> toPrint = new List<Node>();
            toPrint.Add(Tree.Head);
            while (toPrint.Count > 0)
            {
                List<Node> children = new List<Node>();
                foreach (Node node in toPrint)
                {
                    ret += node.Name + ", ";
                    for (int i = 0; i < node.Arity; i++)
                        children.Add(node.Children[i]);
                }
                toPrint.Clear();
                toPrint.AddRange(children);
                ret += Environment.NewLine;
            }
            return ret;
        }

        public string ToStringDetailed()
        {
            string ret = "";
            List<Node> toPrint = new List<Node>();
            toPrint.Add(Tree.Head);
            while (toPrint.Count > 0)
            {
                List<Node> children = new List<Node>();
                foreach (Node node in toPrint)
                {
                    ret += node.Name + "(" + node.Depth + "," + node.SubtreeDepth + "," + (node.Parent == null ? "N/A" : node.Parent.Name) + "), ";
                    for (int i = 0; i < node.Arity; i++)
                        children.Add(node.Children[i]);
                }
                toPrint.Clear();
                toPrint.AddRange(children);
                ret += Environment.NewLine;
            }
            return ret;
        }

        public override string ToGraphVis()
        {
            string ret = "";
            ret += "digraph T {" + Environment.NewLine;
            ret += GraphVisLines(Tree.Head);
            ret += "}";
            return ret;
        }

        private static int nodeNum = 0;
        private string GraphVisLines(Node node)
        {
            int thisNodeNum = nodeNum++;
            string ret = "";
            int childNodeNum;
            for(int i = 0; i < node.Arity; i++)
            {
                childNodeNum = nodeNum;
                ret += GraphVisLines(node.Children[i]);
                ret += "\t" + node.Name + thisNodeNum + " -> " + node.Children[i].Name + childNodeNum + Environment.NewLine;
            }
            return ret;
        }
    }
}
