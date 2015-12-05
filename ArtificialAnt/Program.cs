using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GEPSharp;
using System.Diagnostics;
using System.IO;

namespace ArtificialAnt
{
    class Program
    {
        static int NumTests = 20;
        static double MaxFitness = 89;

        static void Main(string[] args)
        {
            Trail trail = new Trail();

            var funcInfos = new List<Tuple<string, int, EvaluationFunction>>();
            funcInfos.Add(new Tuple<string, int, EvaluationFunction>("Left", 0, AntFunctions.Left));
            funcInfos.Add(new Tuple<string, int, EvaluationFunction>("Right", 0, AntFunctions.Right));
            funcInfos.Add(new Tuple<string, int, EvaluationFunction>("Move", 0, AntFunctions.Move));
            funcInfos.Add(new Tuple<string, int, EvaluationFunction>("IfFoodAhead", 2, AntFunctions.IfFoodAhead));
            funcInfos.Add(new Tuple<string, int, EvaluationFunction>("Do2", 2, AntFunctions.Do2));
            funcInfos.Add(new Tuple<string, int, EvaluationFunction>("Do3", 3, AntFunctions.Do3));

            var nodes = Evaluable.MapToNodes(funcInfos);

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
                if(GEP)
                    TestHelper.TestGEP(outputFileName, MaxFitness, i, nodes, fitness);
                else
                    TestHelper.TestGP(outputFileName, MaxFitness, i, nodes, fitness);
            }
            if(GEP)
                TestHelper.AddFunctionsToCsvs(outputFileName, NumTests);
            else
                TestHelper.AddFunctionsToCsvs(outputFileName, NumTests);
        }

        public static double fitness(object result)
        {
            Evaluable strategy = result as Evaluable;
            Trail trail = new Trail();
            int foodEaten = 0;
            int turns = 0;
            var paramArray = new object[] { trail, turns };
            MaxFitness = Trail.StartFoodLocations.Count;
            while (foodEaten < MaxFitness && (int)paramArray[1] < 400)
                foodEaten += (int)strategy.Evaluate(paramArray);

            return foodEaten;
        }

        static void PrintBoard(Trail trail)
        {
            for (int i = 0; i < Trail.BoardEdgeLength; i++)
            {
                for (int j = 0; j < Trail.BoardEdgeLength; j++)
                {
                    if (trail.Ant.x == j && trail.Ant.y == i)
                        Console.Write("A");
                    else if(trail.FoodLocations.Contains(new Point(j, i)))
                        Console.Write("*");
                    else
                        Console.Write("0");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
