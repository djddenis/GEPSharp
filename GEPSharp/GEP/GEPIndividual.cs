using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEPSharp
{
    public class GEPIndividual : Individual
    {
        public KarvaDomain Tree { get; private set; }

        public GEPIndividual(int headLength, IEnumerable<NodeBase> nodeTypes)
            : base(new KarvaDomain(headLength, nodeTypes))
        {
            Tree = (KarvaDomain)Domains.First();
        }

        private GEPIndividual(IEnumerable<Domain> domains, bool evaluated, object answer, double fitness)
            :base(domains, evaluated, answer, fitness)
        {
            Tree = (KarvaDomain)Domains.First();
        }

        public override Individual MakeCopy()
        {
            var copiedDomains = new List<Domain>();
            foreach (Domain d in Domains)
                copiedDomains.Add(d.MakeCopy());
            return new GEPIndividual(copiedDomains, evaluated, answer, Fitness);
        }

        internal override void ResetAnswer()
        {
            base.ResetAnswer();
            Tree.CalculateArgumentIndicies();
        }

        protected override object Evaluate(bool useAllCachedSolutions = false)
        {
            var headSolution = EvaluateNode(0);
            if (useAllCachedSolutions)
            {
                List<Tuple<object, NodeBase>> solutions = new List<Tuple<object, NodeBase>>();
                for (int i = 0; i < Tree.HeadLength; i++)
                    if (Tree[i].Evaluated)
                        solutions.Add(new Tuple<object, NodeBase>(Tree[i].Value(null, true), Tree[i]));
                return solutions;
            }
            return headSolution;
        }

        private object EvaluateNode(int i)
        {
            int arity = Tree[i].Arity;
            object[] args = new object[arity];
            for (int k = 0; k < arity; k++)
                args[k] = EvaluateNode(Tree.Arguments(i)[k]);
            return Tree[i].Value(args);
        }

        internal void Mutate()
        {
            int index = StaticRandom.Next(Tree.Length);
            bool onlyTerminals = index >= Tree.HeadLength;

            NodeBase replacement = Tree.RandomNodeFromSet().MakeCopy();
            if (onlyTerminals)
                replacement = Tree.Terminals.ElementAt(StaticRandom.Next(Tree.Terminals.Count())).MakeCopy();

            Tree[index] = replacement;

            ResetAnswer();
        }

        internal void ReceiveTranspositionSequence(NodeBase[] sequence)
        {
            int sequenceLength = sequence.Length;
            int position = StaticRandom.Next(Tree.HeadLength - sequenceLength + 1);

            for (int i = Tree.HeadLength - 1 - sequenceLength; i >= position; i--)
                Tree[i + sequenceLength] = Tree[i];

            ReceiveSequence(sequence, sequenceLength, position);
        }

        internal void ReceiveRecombinationSequence(NodeBase[] sequence, int position)
        {
            int sequenceLength = sequence.Length;

            ReceiveSequence(sequence, sequenceLength, position);
        }

        private void ReceiveSequence(NodeBase[] sequence, int sequenceLength, int position)
        {
            for (int i = 0; i < sequenceLength; i++)
                Tree[position + i] = sequence[i];

            ResetAnswer();
        }

        internal NodeBase[] DonateTranspositionSequence(int seqLength, bool root)
        {
            int position =  root ?
                            StaticRandom.Next(Tree.HeadLength - seqLength + 1) :
                            StaticRandom.Next(Tree.Length - seqLength + 1);

            return DonateSequence(seqLength, position);
        }

        internal NodeBase[] DonateSequence(int seqLength, int position)
        {
            NodeBase[] donation = new NodeBase[seqLength];
            for (int i = 0; i < seqLength; i++)
                donation[i] = Tree[position + i].MakeCopy();

            return donation;
        }

        public override string ToString()
        {
            string ret = "";
            for (int i = 0; i < Tree.Length; i++)
                ret += Tree[i];
            return ret;
        }

        public override string ToGraphVis()
        {
            string ret = "";
            ret += "digraph T {" + Environment.NewLine;
            for (int i = 0; i < Tree.HeadLength; i++)
            {
                var args = Tree.argumentIndicies[i];
                for (int j = 0; j < args.Length; j++)
                {
                    ret += "\t" + Tree[i] + i + " -> ";
                    ret += Tree[args[j]].ToString() + args[j] + ";" + Environment.NewLine;
                }
            }
            ret += "}";
            return ret;
        }
    }
}
