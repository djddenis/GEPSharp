using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimal_Control
{
    public enum NodeValues
    {
        NegOne,
        X,
        V,
        Plus,
        Minus,
        Mult,
        Divide,
        Absolute,
        GreaterThan
    }
    public class CartNode
    {
        public int Arity { get { return Children.Length; } }
        public NodeValues Value { get; private set; }
        public CartNode[] Children { get; private set; }

        public CartNode(NodeValues value, CartNode[] children)
        {
            Value = value;
            Children = children;
        }

        public double NextTimestep(double x, double v)
        {
            switch (Value)
            {
                case(NodeValues.NegOne):
                    return -1;
                case (NodeValues.X):
                    return x;
                case (NodeValues.V):
                    return v;
                case (NodeValues.Plus):
                    return Children[0].NextTimestep(x, v) + Children[1].NextTimestep(x, v);
                case (NodeValues.Minus):
                    return Children[0].NextTimestep(x, v) - Children[1].NextTimestep(x, v);
                case (NodeValues.Mult):
                    return Children[0].NextTimestep(x, v) * Children[1].NextTimestep(x, v);
                case (NodeValues.Divide):
                    double denom = Children[1].NextTimestep(x, v);
                    if (denom == 0)
                        return 1;
                    return Children[0].NextTimestep(x, v) / denom;
                case (NodeValues.Absolute):
                    return Math.Abs(Children[0].NextTimestep(x, v));
                case(NodeValues.GreaterThan):
                    return Children[0].NextTimestep(x, v) > Children[1].NextTimestep(x, v) ? 1 : -1;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
