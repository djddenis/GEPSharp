using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEPSharp
{
    public class GEPProblem : Problem
    {
        public double MutationRate { get; set; }
        public double TranspositionRate { get; set; }
        public int MaxTranspositionLength { get; set; }
        public double RootTranspositionRate { get; set; }
        public int MaxRootTranspositionLength { get; set; }
        public double GeneTranspositionRate { get; set; }
        public double OnePointRecombinationRate { get; set; }
        public double TwoPointRecombinationRate { get; set; }
        public double GeneRecombinationRate { get; set; }

        public GEPProblem(int population, FitnessFunction fitFunc, int headLength, IEnumerable<NodeBase> nodeTypes)
            : base(fitFunc)
        {
            Population = population;
            var initialPopulation = new List<Individual>();
            for (int i = 0; i < Population; i++)
                initialPopulation.Add(new GEPIndividual(headLength, nodeTypes));

            Individuals = new List<Individual>(initialPopulation);
            BestSoFar = initialPopulation.FirstOrDefault();

            nextGen = new GEPIndividual[Population];

            ParallelFitnessEnabled = false;
            NodeValueCachingEnabled = false;

            MutationRate = 0.1;
            TranspositionRate = 0.1;
            RootTranspositionRate = 0.1;
            MaxTranspositionLength = 3;
            MaxRootTranspositionLength = 5;
            GeneTranspositionRate = 0;
            OnePointRecombinationRate = 0.4;
            TwoPointRecombinationRate = 0.2;
            GeneRecombinationRate = 0;
        }

        protected override void GeneticOperators()
        {
            Mutation();

            Transposition(false); 

            Transposition(true); //Root transposition

            GeneTransposition();

            OnePointRecombination();

            TwoPointRecombination();

            //Implement gene recombination when implementing multi-genic chromosones
        }

        private void TwoPointRecombination()
        {
            int numTwoPointRecombinations = (int)Math.Round(TwoPointRecombinationRate * Population);
            for (int i = 0; i < numTwoPointRecombinations; i++)
            {
                GEPIndividual first, second;
                ChoosePair(out first, out second);

                int pointOne = 0, pointTwo = 0;
                while (pointOne == pointTwo)
                {
                    pointOne = StaticRandom.Next(first.Tree.Length);
                    pointTwo = StaticRandom.Next(first.Tree.Length);
                }
                if (pointOne > pointTwo)
                {
                    int temp = pointOne;
                    pointOne = pointTwo;
                    pointTwo = temp;
                }

                Recombine(first, second, pointOne, pointTwo);
            }
        }

        private static void Recombine(GEPIndividual first, GEPIndividual second, int pointOne, int pointTwo)
        {
            NodeBase[] midFirst = first.DonateSequence(pointTwo - pointOne, pointOne);
            NodeBase[] midSecond = second.DonateSequence(pointTwo - pointOne, pointOne);
            first.ReceiveRecombinationSequence(midSecond, pointOne);
            second.ReceiveRecombinationSequence(midFirst, pointOne);
        }

        private void OnePointRecombination()
        {
            int numOnePointRecombinations = (int)Math.Round(OnePointRecombinationRate * Population);
            for (int i = 0; i < numOnePointRecombinations; i++)
            {
                GEPIndividual first, second;
                ChoosePair(out first, out second);

                int point = StaticRandom.Next(first.Tree.Length);

                Recombine(first, second, 0, point);
            }
        }

        private void ChoosePair(out GEPIndividual first, out GEPIndividual second)
        {
            first = (GEPIndividual)nextGen[StaticRandom.Next(Population)];
            second = first;
            while (second == first)
                second = (GEPIndividual)nextGen[StaticRandom.Next(Population)];
        }

        private void GeneTransposition()
        {
            int numGeneTranspositions = (int)Math.Round(GeneTranspositionRate * Population);
            for (int i = 0; i < numGeneTranspositions; i++)
            {
                int donorIndex = StaticRandom.Next(Population);
                int receiverIndex = donorIndex;
                while (donorIndex == receiverIndex)
                    receiverIndex = StaticRandom.Next(Population);
                nextGen[receiverIndex] = nextGen[donorIndex].MakeCopy();
            }
        }

        private void Transposition(bool root)
        {
            int numTranspositions = (int)Math.Round((root ? RootTranspositionRate : TranspositionRate) * Population);
            for (int i = 0; i < numTranspositions; i++)
            {
                GEPIndividual donor, receiver;
                ChoosePair(out donor, out receiver);
                int transpositionLength = StaticRandom.Next(root ? MaxRootTranspositionLength : MaxTranspositionLength) + 1;

                receiver.ReceiveTranspositionSequence(donor.DonateTranspositionSequence(transpositionLength, root));
            }
        }

        private void Mutation()
        {
            int numMutations = (int)Math.Round(MutationRate * Population);
            for (int i = 0; i < numMutations; i++)
                ((GEPIndividual)nextGen[StaticRandom.Next(Population)]).Mutate();
        }
    }
}
