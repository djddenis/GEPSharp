using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using GEPSharp;

namespace Optimal_Control
{
    public class Program
    {
        static Random r = new Random();
        static Tuple<double, double>[] FitnessCases;
        static int NumFitnessCases = 20;
        static int StepsAllowed = 500;
        static double CaptureRadius = 0.02;
        static int NumTests = 4;

        static void Main(string[] args)
        {
            var nodes = new List<Node>();
            FitnessCases = new Tuple<double, double>[NumFitnessCases];

            nodes.Add(new Node("Neg1", new CartNode(NodeValues.NegOne, null)));
            nodes.Add(new Node("X", new CartNode(NodeValues.X, null)));
            nodes.Add(new Node("V", new CartNode(NodeValues.V, null)));
            nodes.Add(new Node("plus", 2, (i => new CartNode(NodeValues.Plus, new CartNode[] { (CartNode)i[0], (CartNode)i[1] }))));
            nodes.Add(new Node("minus", 2, (i => new CartNode(NodeValues.Minus, new CartNode[] { (CartNode)i[0], (CartNode)i[1] }))));
            nodes.Add(new Node("GreaterThan", 2, (i => new CartNode(NodeValues.GreaterThan, new CartNode[] { (CartNode)i[0], (CartNode)i[1] }))));
            nodes.Add(new Node("Multiply", 2, (i => new CartNode(NodeValues.Mult, new CartNode[] { (CartNode)i[0], (CartNode)i[1] }))));
            nodes.Add(new Node("Divide", 2, (i => new CartNode(NodeValues.Divide, new CartNode[] { (CartNode)i[0], (CartNode)i[1] }))));
            nodes.Add(new Node("AbsoluteValue", 1, (i => new CartNode(NodeValues.Absolute, new CartNode[] { (CartNode)i[0] }))));

            Console.WriteLine("Type of genetic programming? (GEP/GP)");
            string answer = Console.ReadLine();
            bool GEP = answer.ToUpper() == "GEP";

            Console.WriteLine("Population?");
            TestHelper.Population = int.Parse(Console.ReadLine());

            Console.WriteLine("Generations?");
            TestHelper.Generations = int.Parse(Console.ReadLine());

            Console.WriteLine("Number of Tests?");
            NumTests = int.Parse(Console.ReadLine());

            Console.WriteLine("Node Value Caching? (true/false)");
            TestHelper.NodeValueCaching = bool.Parse(Console.ReadLine());

            Console.WriteLine("Parallel? (true/false)");
            TestHelper.Parallel = bool.Parse(Console.ReadLine());

            Console.WriteLine("Output File Name? (no extension)");
            string outputFileName = Console.ReadLine();

            Console.WriteLine("Running test.");

            for (int i = 0; i < NumTests; i++)
            {
                NewFitnessCases();
                if(GEP)
                    TestHelper.TestGEP(outputFileName, StepsAllowed * NumFitnessCases, i, nodes, fitness);
                else
                    TestHelper.TestGP(outputFileName, StepsAllowed * NumFitnessCases, i, nodes, fitness);
            }
            if(GEP)
                TestHelper.AddFunctionsToCsvs(outputFileName, NumTests);
            else
                TestHelper.AddFunctionsToCsvs(outputFileName, NumTests);
        }

        private static void NewFitnessCases()
        {
            for (int i = 0; i < NumFitnessCases; i++)
                FitnessCases[i] = new Tuple<double, double>((r.NextDouble() - 0.5) * 1.5, (r.NextDouble() - 0.5) * 1.5);
        }

        static double fitness(object result)
        {
            CartNode strategy = result as CartNode;
            double fitnessTotal = 0;
            for (int i = 0; i < NumFitnessCases; i++)
            {
                double x = FitnessCases[i].Item1;
                double v = FitnessCases[i].Item2;
                int stepsRemaining = StepsAllowed;
                while (stepsRemaining > 0)
                {
                    double oldVel = v;
                    v += 0.01 * strategy.NextTimestep(x, v);
                    x += oldVel;
                    stepsRemaining--;
                    if (Math.Sqrt(x * x + v * v) < CaptureRadius)
                        break;
                }
                fitnessTotal += stepsRemaining;
            }

            return fitnessTotal;
        }
    }
}
