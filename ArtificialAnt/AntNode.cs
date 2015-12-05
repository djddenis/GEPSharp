using GEPSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialAnt
{
    public static class AntFunctions
    {
        public static object Left(Evaluable[] Children, object[] outsideParams)
        {
            Trail trail = outsideParams[0] as Trail;
            int turnCount = (int)outsideParams[1];
            trail.TurnLeft();
            outsideParams[1] = turnCount+1;
            return 0;
        }

        public static object Right(Evaluable[] Children, object[] outsideParams)
        {
            Trail trail = outsideParams[0] as Trail;
            int turnCount = (int)outsideParams[1];
            trail.TurnRight();
            outsideParams[1] = turnCount + 1;
            return 0;
        }

        public static object Move(Evaluable[] Children, object[] outsideParams)
        {
            Trail trail = outsideParams[0] as Trail;
            int turnCount = (int)outsideParams[1];
            outsideParams[1] = turnCount + 1;
            if (trail.Move())
                return 1;
            return 0;
        }

        public static object IfFoodAhead(Evaluable[] Children, object[] outsideParams)
        {
            Trail trail = outsideParams[0] as Trail;
            int turnCount = (int)outsideParams[1];

            return trail.FoodAhead() ?
                Children[0].Evaluate(outsideParams) :
                Children[1].Evaluate(outsideParams);
        }

        public static object Do2(Evaluable[] Children, object[] outsideParams)
        {
            Trail trail = outsideParams[0] as Trail;
            int turnCount = (int)outsideParams[1];

            return (int)Children[0].Evaluate(outsideParams) + (int)Children[1].Evaluate(outsideParams);
        }

        public static object Do3(Evaluable[] Children, object[] outsideParams)
        {
            Trail trail = outsideParams[0] as Trail;
            int turnCount = (int)outsideParams[1];

            return (int)Children[0].Evaluate(outsideParams) + (int)Children[1].Evaluate(outsideParams) + (int)Children[2].Evaluate(outsideParams);
        }
    }
}
