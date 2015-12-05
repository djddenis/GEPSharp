using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEPSharp
{
    public class StaticRandom
    {
        private static Random statRand;

        static StaticRandom()
        {
            statRand = new Random();
        }

        public static int Next(int max)
        {
            return statRand.Next(max);
        }

        public static int Next(int min, int max)
        {
            return statRand.Next(min, max);
        }

        public static double Next(double max)
        {
            return statRand.NextDouble() * max;
        }
    }
}
