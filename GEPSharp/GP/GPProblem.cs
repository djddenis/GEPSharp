using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEPSharp
{
    public class GPProblem : Problem
    {
        public enum TreeGenerationStrategies
        {
            Full,
            Grow,
            RampedHalfAndHalf
        }

        public double CrossoverRate { get; set; }

        public int MaxTreeGrowDepth { get; set; }

        public GPProblem(int population, FitnessFunction fitFunc, int maxInitialDepth, TreeGenerationStrategies strategy, IEnumerable<Node> nodeTypes)
            : base(fitFunc)
        {
            Population = population;
            var initialPopulation = new List<GPIndividual>();

            if (strategy != TreeGenerationStrategies.RampedHalfAndHalf)
            {
                bool full = strategy == TreeGenerationStrategies.Full;
                for (int i = 0; i < Population; i++)
                    initialPopulation.Add(new GPIndividual(maxInitialDepth, full, nodeTypes));
            }
            else //Ramped half and half
            {
                int popPerDepth = Population / (maxInitialDepth - 1); //Truncate
                for (int depth = 2; depth <= maxInitialDepth; depth++)
                    for (int i = 0; i < popPerDepth; i++)
                        initialPopulation.Add(new GPIndividual(depth, i % 2 == 0, nodeTypes)); //Alternate grow and full
                //If truncated division resulted in fewer individuals than intended, fill out extras at the max depth
                while (initialPopulation.Count < Population)
                    initialPopulation.Add(new GPIndividual(maxInitialDepth, StaticRandom.Next(1) > 0.5, nodeTypes));
            }

            Individuals = new List<Individual>(initialPopulation);
            BestSoFar = initialPopulation.FirstOrDefault();

            ParallelFitnessEnabled = false;
            NodeValueCachingEnabled = false;

            CrossoverRate = 0.90;
            MaxTreeGrowDepth = 17;

            nextGen = new GPIndividual[Population];
        }

        protected override void GeneticOperators()
        {
            Crossover();
        }

        private void Crossover()
        {
            var byFitness = nextGen.ToList();
            byFitness.Sort();

            int numCrossOvers = (int)Math.Round(CrossoverRate * Population);
            for (int i = 0; i < numCrossOvers; i++)
            {
                GPIndividual parentA = (GPIndividual)RouletteSelect(byFitness);
                GPIndividual parentB = (GPIndividual)RouletteSelect(byFitness);

                Node donationA = parentA.DonateSubtree(), donationB = parentB.DonateSubtree();

                parentA.ReceiveSubtree(donationB, MaxTreeGrowDepth);
                parentB.ReceiveSubtree(donationA, MaxTreeGrowDepth);
            }
        }
    }
}
