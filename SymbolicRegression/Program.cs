using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using GEPSharp;


namespace SymbolicRegression
{
    class Program
    {
        static Random r = new Random();
        static Tuple<double, double>[] FitnessCases;
        static int NumTests = 20;
        static int NumFitnessCases = 20;
        static double AcceptRadius = 0.01;

        static void Main(string[] args)
        {
            var nodes = new List<Node>();
            FitnessCases = new Tuple<double, double>[NumFitnessCases];

            nodes.Add(new Node("X", new SymbolicNode(NodeValues.X, null)));
            nodes.Add(new Node("plus", 2, (i => new SymbolicNode(NodeValues.Plus, new SymbolicNode[] { (SymbolicNode)i[0], (SymbolicNode)i[1] }))));
            nodes.Add(new Node("minus", 2, (i => new SymbolicNode(NodeValues.Minus, new SymbolicNode[] { (SymbolicNode)i[0], (SymbolicNode)i[1] }))));
            nodes.Add(new Node("e", 1, (i => new SymbolicNode(NodeValues.Exp, new SymbolicNode[] { (SymbolicNode)i[0]}))));
            nodes.Add(new Node("mult", 2, (i => new SymbolicNode(NodeValues.Mult, new SymbolicNode[] { (SymbolicNode)i[0], (SymbolicNode)i[1] }))));
            nodes.Add(new Node("div", 2, (i => new SymbolicNode(NodeValues.Divide, new SymbolicNode[] { (SymbolicNode)i[0], (SymbolicNode)i[1] }))));
            nodes.Add(new Node("ln", 1, (i => new SymbolicNode(NodeValues.RLog, new SymbolicNode[] { (SymbolicNode)i[0] }))));
            nodes.Add(new Node("sin", 1, (i => new SymbolicNode(NodeValues.Sin, new SymbolicNode[] { (SymbolicNode)i[0] }))));
            nodes.Add(new Node("cos", 1, (i => new SymbolicNode(NodeValues.Cos, new SymbolicNode[] { (SymbolicNode)i[0] }))));

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
                    TestHelper.TestGEP(outputFileName, (1.0 / AcceptRadius) * NumFitnessCases, i, nodes, fitness);
                else
                    TestHelper.TestGP(outputFileName, (1.0 / AcceptRadius) * NumFitnessCases, i, nodes, fitness);
            }
            if(GEP)
                TestHelper.AddFunctionsToCsvs(outputFileName, NumTests);
            else
                TestHelper.AddFunctionsToCsvs(outputFileName, NumTests);
        }

        private static void NewFitnessCases()
        {
            for (int i = 0; i < NumFitnessCases; i++)
            {
                double x = (r.NextDouble() * 2.0) - 1.0;
                double y = Math.Pow(x, 4) + Math.Pow(x, 3) + Math.Pow(x, 2) + x;
                FitnessCases[i] = new Tuple<double, double>(x, y);
            }
        }

        static double fitness(object result)
        {
            SymbolicNode function = result as SymbolicNode;
            double fitnessTotal = 0;
            for (int i = 0; i < NumFitnessCases; i++)
            {
                double x = FitnessCases[i].Item1;
                double y = FitnessCases[i].Item2;
                double calculated = function.Evaluate(x);
                if (double.IsInfinity(calculated) || double.IsNaN(calculated))
                    calculated = double.MaxValue;
                double score = Math.Abs(y - calculated);
                fitnessTotal += score <= AcceptRadius ? (1.0 / AcceptRadius) : 1.0 / score;
            }

            return fitnessTotal;
        }
    }
}
