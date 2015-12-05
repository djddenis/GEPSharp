using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolicRegression
{
    public enum NodeValues
    {
        X,
        Plus,
        Minus,
        Mult,
        Divide,
        Sin,
        Cos,
        Exp,
        RLog
    }

    public class SymbolicNode
    {
        public int Arity { get { return Children.Length; } }
        public NodeValues Value { get; private set; }
        public SymbolicNode[] Children { get; private set; }

        public SymbolicNode(NodeValues value, SymbolicNode[] children)
        {
            Value = value;
            Children = children;
        }

        public double Evaluate(double x)
        {
            switch (Value)
            {
                case (NodeValues.X):
                    return x;
                case (NodeValues.Plus):
                    return Children[0].Evaluate(x) + Children[1].Evaluate(x);
                case (NodeValues.Minus):
                    return Children[0].Evaluate(x) - Children[1].Evaluate(x);
                case (NodeValues.Mult):
                    return Children[0].Evaluate(x) * Children[1].Evaluate(x);
                case (NodeValues.Divide):
                    double denom = Children[1].Evaluate(x);
                    if (denom == 0)
                        return 1;
                    return Children[0].Evaluate(x) / denom;
                case (NodeValues.Exp):
                    double input = Children[0].Evaluate(x);
                    double result = Math.Exp(input);
                    if(double.IsNaN(result) || double.IsInfinity(result))
                        return input;
                    return result;
                case (NodeValues.RLog):
                    double param = Children[0].Evaluate(x);
                    if (param == 0)
                        return 0;
                    return Math.Log(Math.Abs(param));
                case (NodeValues.Sin):
                    return Math.Sin(Children[0].Evaluate(x));
                case (NodeValues.Cos):
                    return Math.Cos(Children[0].Evaluate(x));
                default:
                    throw new ArgumentException();
            }
        }
    }
}
