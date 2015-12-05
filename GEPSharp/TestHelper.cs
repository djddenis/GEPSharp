using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEPSharp
{
    public static class TestHelper
    {
        static Random r = new Random();
        public static int Population = 500;
        public static int Generations = 50;
        public static bool Parallel = false;
        public static bool NodeValueCaching = false;

        public static void TestGP(string name, double maxFitness, int testNum, IEnumerable<Node> nodes, FitnessFunction fitness)
        {
            var problem = new GPProblem(Population, fitness, 16, GPProblem.TreeGenerationStrategies.RampedHalfAndHalf, nodes);
            RunTest(problem, name, maxFitness, testNum);
        }

        public static void TestGEP(string name, double maxFitness, int testNum, IEnumerable<Node> nodes, FitnessFunction fitness)
        {
            var problem = new GEPProblem(Population, fitness, 16, nodes);
            RunTest(problem, name, maxFitness, testNum);
        }

        public static void AddFunctionsToCsvs(string outputName, int numTests)
        {
            using (StreamWriter file = new StreamWriter(Path.Combine("Test Output", outputName + ".csv"), true))
            {
                file.WriteLine("=MAX(A1:A" + numTests + "),=MIN(B1:B" + numTests + ")");
                file.WriteLine("=AVERAGE(A1:A" + numTests + "),=AVERAGE(B1:B" + numTests + ")");
                file.WriteLine("=STDEV(A1:A" + numTests + "),=STDEV(B1:B" + numTests + ")");
            }
        }

        
        private static void RunTest(Problem problem, string outputName, double maxFitness, int testNum)
        {
            problem.NodeValueCachingEnabled = NodeValueCaching;
            problem.ParallelFitnessEnabled = Parallel;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            problem.EvaluateRounds(Generations);
            timer.Stop();

            if(!Directory.Exists("Test Output"))
                Directory.CreateDirectory("Test Output");

            using (StreamWriter file = new StreamWriter(Path.Combine("Test Output", outputName + ".txt"), true))
            {
                file.WriteLine("Population " + Population + ". " + Generations + " Generations in " + TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds));
                file.WriteLine("Solution Fitness: " + problem.SolutionFitness + " out of a possible " + maxFitness);
                file.WriteLine("Solution:");
                file.WriteLine(problem.BestSoFar);
            }
            if (!Directory.Exists(Path.Combine("Test Output", "Graphs")))
                Directory.CreateDirectory(Path.Combine("Test Output", "Graphs"));
            using (StreamWriter file = new StreamWriter(Path.Combine("Test Output", "Graphs", outputName + testNum + ".gv"), true))
            {
                file.WriteLine(problem.BestSoFar.ToGraphVis());
            }
            using (StreamWriter file = new StreamWriter(Path.Combine("Test Output", outputName + ".csv"), true))
            {
                file.WriteLine(problem.SolutionFitness + "," + timer.ElapsedMilliseconds / 1000.0);
            }
        }
    }
}
