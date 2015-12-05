using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEPSharp
{
    public delegate double FitnessFunction(object result);

    public abstract class Problem
    {
        public int Population { get; protected set; }

        public List<Individual> Individuals { get; protected set; }

        public FitnessFunction FitFunc { get; private set; }

        public Individual BestSoFar { get; protected set; }

        public object Solution { get { return BestSoFar == null ? null : BestSoFar.Answer; } }

        public double SolutionFitness { get { return BestSoFar == null ? -1 : BestSoFar.Fitness; } }

        public double TotalPopulationFitness { get; private set; }

        public bool ParallelFitnessEnabled { get; set; }

        public bool NodeValueCachingEnabled 
        {
            get 
            { 
                return Individuals[0].useNodeValueCaching; 
            } 
            set 
            { 
                foreach (Individual ind in Individuals) 
                    ind.useNodeValueCaching = value; 
            } 
        }

        protected Individual[] nextGen;

        public Problem(IEnumerable<Individual> individuals, FitnessFunction fitFunc, bool parallelFitnessEnabled = false, bool nodeValueCachingEnabled = false)
            : this(fitFunc)
        {
            Individuals = new List<Individual>(individuals);
            Population = Individuals.Count();
            BestSoFar = Individuals.FirstOrDefault();
            nextGen = new Individual[Population];
            ParallelFitnessEnabled = parallelFitnessEnabled;
            NodeValueCachingEnabled = nodeValueCachingEnabled;
            EvaluateRound();
        }

        protected Problem(FitnessFunction fitFunc)
        {
            FitFunc = fitFunc;
            BestSoFar = null;
        }

        public void EvaluateRounds(int n)
        {
            for (int i = 0; i < n; i++)
            {
                CreateNextGeneration();
                EvaluateRound();
            }
        }

        public void EvaluateRounds(int n, int goalFitness)
        {
            for (int i = 0; i < n; i++)
            {
                CreateNextGeneration();
                EvaluateRound();
                if (SolutionFitness >= goalFitness)
                    return;
            }
        }

        public void EvaluateRound()
        {
            TotalPopulationFitness = 0;
            if(NodeValueCachingEnabled)
            {
                foreach (Individual ind in Individuals)
                {
                    FitnessForNodeValueCachedIndividual(ind);
                }
            }

            if(ParallelFitnessEnabled)
                Parallel.ForEach<Individual>(Individuals, i =>
                    {
                        if (NodeValueCachingEnabled)
                            FitnessForNodeValueCachedIndividual(i);
                        else
                            i.Fitness = FitFunc(i.Answer);
                    });
            foreach(Individual ind in Individuals)
            {
                if(!ParallelFitnessEnabled && !NodeValueCachingEnabled)
                    ind.Fitness = FitFunc(ind.Answer);
                TotalPopulationFitness += ind.Fitness;
                if (ind.Fitness > SolutionFitness)
                    BestSoFar = ind;
            }
        }

        private void FitnessForNodeValueCachedIndividual(Individual ind)
        {
            double bestFitness = Double.MinValue;
            foreach (Tuple<object, NodeBase> answer in (List<Tuple<object, NodeBase>>)ind.Answer)
            {
                double fitness = FitFunc(answer.Item1);
                if (fitness > bestFitness)
                {
                    bestFitness = fitness;
                    ind.RootOfBestTree = answer.Item2;
                }
            }
            ind.Fitness = bestFitness;
        }

        private void CreateNextGeneration()
        {
            Selection();

            GeneticOperators();

            bool caching = NodeValueCachingEnabled;
            Individuals.Clear();
            Individuals.AddRange(nextGen);
            NodeValueCachingEnabled = caching;
        }

        protected virtual void GeneticOperators()
        {
        }

        protected virtual void Selection()
        {
            Individuals.Sort(); //Sorts by fitness
            for (int i = 1; i < Population; i++)
            {
                if (TotalPopulationFitness == 0)
                {
                    nextGen[i] = Individuals[i].MakeCopy();
                    continue;
                }
                nextGen[i] = RouletteSelect(Individuals).MakeCopy();
            }

            nextGen[0] = BestSoFar; //Elitism
        }

        protected Individual RouletteSelect(List<Individual> byFitness)
        {
            if (TotalPopulationFitness == 0)
                return byFitness.First();

            double rouletteFitness = StaticRandom.Next(TotalPopulationFitness);
            var enumer = byFitness.GetEnumerator();
            Individual selected = enumer.Current;
            while (rouletteFitness > 0 && enumer.MoveNext())
            {
                selected = enumer.Current;
                rouletteFitness -= selected.Fitness;
            }
            return selected;
        }
    }
}
