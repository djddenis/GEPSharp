using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace GEPSharp
{
    public class KarvaDomain : LinearDomain
    {
        public int HeadLength { get; private set; }
        public int TailLength { get; private set; }

        internal int[][] argumentIndicies;

        public IEnumerable<NodeBase> Terminals { get; private set; }

        public KarvaDomain(int headLength, IEnumerable<NodeBase> nodeTypes)
            : base(headLength + CalculateTailLength(headLength, nodeTypes), nodeTypes)
        {
            HeadLength = headLength;
            TailLength = Length - HeadLength;
            Terminals = nodeSet.Where(i => i.IsTerminal);
            InitializeData();
        }

        private KarvaDomain(int headLength, int length, IEnumerable<NodeBase> nodeTypes, IEnumerable<NodeBase> terminals,
            int[][] argumentIndicies, NodeBase[] data)
            : base(data, length, nodeTypes)
        {
            HeadLength = headLength;
            TailLength = Length - HeadLength;
            this.Terminals = terminals;
            this.argumentIndicies = argumentIndicies;
        }

        public override Domain MakeCopy()
        {
            NodeBase[] copiedData = new NodeBase[Length];
            for(int i = 0; i < Length; i++)
                copiedData[i] = this[i].MakeCopy();
            int[][] copiedArgIndicies = new int[HeadLength][];
            for(int i = 0; i < HeadLength; i++)
            {
                copiedArgIndicies[i] = new int[argumentIndicies[i].Length];
                for(int j = 0; j < argumentIndicies[i].Length; j++)
                    copiedArgIndicies[i][j] = argumentIndicies[i][j];
            }
            return new KarvaDomain(HeadLength, Length, nodeSet, Terminals, copiedArgIndicies, copiedData);
        }

        protected void InitializeData()
        {
            for(int i=0; i < HeadLength; i++)
                this[i] = RandomNodeFromSet().MakeCopy();
            for (int i = HeadLength; i < Length; i++)
                this[i] = Terminals.ElementAt(StaticRandom.Next(Terminals.Count())).MakeCopy();

            CalculateArgumentIndicies();
        }

        internal void CalculateArgumentIndicies()
        {
            argumentIndicies = new int[HeadLength][];
            int argIndex = 1;
            for (int i = 0; i < HeadLength; i++)
            {
                argumentIndicies[i] = new int[this[i].Arity];

                for (int j = 0; j < this[i].Arity; j++)
                    argumentIndicies[i][j] = argIndex++;
            }
        }

        public int[] Arguments(int index)
        {
            return argumentIndicies[index];
        }

        public static int CalculateTailLength(int headLength, IEnumerable<NodeBase> nodeTypes)
        {
            return CalculateTailLength(headLength, nodeTypes.Select(i => i.Arity).Max());
        }

        public static int CalculateTailLength(int headLength, int maxDegree)
        {
            return headLength * (maxDegree - 1) + 1;
        }
    }
}
