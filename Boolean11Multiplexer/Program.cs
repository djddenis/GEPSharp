using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using GEPSharp;

namespace Boolean11Multiplexer
{
    class Program
    {
        static Random r = new Random();
        static bool[][] FitnessCases;
        static int NumFitnessCases = 2048;
        static int NumTests = 20;

        static void Main(string[] args)
        {
            var nodes = new List<Node>();
            FitnessCases = new bool[NumFitnessCases][];

            nodes.Add(new Node("A0", new BooleanNode(NodeValues.A0, null)));
            nodes.Add(new Node("A1", new BooleanNode(NodeValues.A1, null)));
            nodes.Add(new Node("A2", new BooleanNode(NodeValues.A2, null)));
            nodes.Add(new Node("D0", new BooleanNode(NodeValues.D0, null)));
            nodes.Add(new Node("D1", new BooleanNode(NodeValues.D1, null)));
            nodes.Add(new Node("D2", new BooleanNode(NodeValues.D2, null)));
            nodes.Add(new Node("D3", new BooleanNode(NodeValues.D3, null)));
            nodes.Add(new Node("D4", new BooleanNode(NodeValues.D4, null)));
            nodes.Add(new Node("D5", new BooleanNode(NodeValues.D5, null)));
            nodes.Add(new Node("D6", new BooleanNode(NodeValues.D6, null)));
            nodes.Add(new Node("D7", new BooleanNode(NodeValues.D7, null)));
            nodes.Add(new Node("AND", 2, (i => new BooleanNode(NodeValues.And, new BooleanNode[] { (BooleanNode)i[0], (BooleanNode)i[1] }))));
            nodes.Add(new Node("OR", 2, (i => new BooleanNode(NodeValues.Or, new BooleanNode[] { (BooleanNode)i[0], (BooleanNode)i[1] }))));
            nodes.Add(new Node("IF", 3, (i => new BooleanNode(NodeValues.If, new BooleanNode[] { (BooleanNode)i[0], (BooleanNode)i[1], (BooleanNode)i[2] }))));
            nodes.Add(new Node("NOT", 1, (i => new BooleanNode(NodeValues.Not, new BooleanNode[] { (BooleanNode)i[0] }))));

            for (uint i = 0; i < NumFitnessCases; i++)
            {
                FitnessCases[i] = new bool[12]; //Twelfth bit is the correct answer
                string bitString = Convert.ToString(i, 2);
                char[] bitArray = bitString.Reverse().ToArray();
                for (int j = 0; j < 11; j++)
                    FitnessCases[i][j] = j < bitArray.Length ? bitArray[j] == '1' : false;

                string convertString = new string(bitArray);
                convertString += "0000000";
                convertString = new string(convertString.Substring(0, 3).Reverse().ToArray()); 
                FitnessCases[i][11] = FitnessCases[i][3 + Convert.ToInt32(convertString, 2)];
            }


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
                    TestHelper.TestGEP(outputFileName, NumFitnessCases, i, nodes, fitness);
                else
                    TestHelper.TestGP(outputFileName, NumFitnessCases, i, nodes, fitness);
            }
            if(GEP)
                TestHelper.AddFunctionsToCsvs(outputFileName, NumTests);
            else
                TestHelper.AddFunctionsToCsvs(outputFileName, NumTests);
        }

        static double fitness(object result)
        {
            BooleanNode strategy = result as BooleanNode;
            double casesFailed = 0;
            for (int i = 0; i < NumFitnessCases; i++)
            {
                bool[] address = new bool[3];
                bool[] data = new bool[8];
                bool answer = FitnessCases[i][11];
                for(int k = 0; k < 11; k++)
                {
                    if(k < 3)
                        address[k] = FitnessCases[i][k];
                    else
                        data[k - 3] = FitnessCases[i][k];
                }
                bool calculated = strategy.Evaluate(address, data);
                if (calculated != answer)
                    casesFailed++;
            }
            return Math.Sqrt(Math.Pow(NumFitnessCases, 2) - Math.Pow(casesFailed, 2));
        }
    }
}
