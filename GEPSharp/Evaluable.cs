using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEPSharp
{
    public delegate object EvaluationFunction(Evaluable[] children, object[] outsideParams);
    public class Evaluable
    {
        public int Arity { get { return Children.Length; } }
        public Evaluable[] Children { get; private set; }
        public EvaluationFunction Function { get; private set; }

        public Evaluable(EvaluationFunction function, Evaluable[] children)
        {
            Children = children;
            Function = function;
        }

        public object Evaluate(object[] outsideParams)
        {
            return Function(Children, outsideParams);
        }

        public static IEnumerable<Node> MapToNodes(IEnumerable<Tuple<string, int, EvaluationFunction>> data)
        {
            foreach (Tuple<string, int, EvaluationFunction> tuple in data)
            {
                string name = tuple.Item1;
                int arity = tuple.Item2;
                EvaluationFunction func = tuple.Item3;
                if (arity == 0)
                    yield return new Node(name, new Evaluable(func, null));
                else
                    yield return new Node(name, arity, args =>
                        {
                            var childArray = new Evaluable[arity];
                            for (int i = 0; i < arity; i++)
                                childArray[i] = (Evaluable)args[i];
                            return new Evaluable(func, childArray);
                        });
            }
        }
    }
}
