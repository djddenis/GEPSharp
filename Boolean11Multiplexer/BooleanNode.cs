using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boolean11Multiplexer
{
    public enum NodeValues
    {
        A0,
        A1,
        A2,
        D0,
        D1,
        D2,
        D3,
        D4,
        D5,
        D6,
        D7,
        And,
        Or,
        Not,
        If
    }

    public class BooleanNode
    {
        public int Arity { get { return Children.Length; } }
        public NodeValues Value { get; private set; }
        public BooleanNode[] Children { get; private set; }

        public BooleanNode(NodeValues value, BooleanNode[] children)
        {
            Value = value;
            Children = children;
        }

        public bool Evaluate(bool[] address, bool[] data)
        {
            switch (Value)
            {
                case (NodeValues.A0):
                    return address[0];
                case (NodeValues.A1):
                    return address[1];
                case (NodeValues.A2):
                    return address[2];
                case (NodeValues.D0):
                    return data[0];
                case (NodeValues.D1):
                    return data[1];
                case (NodeValues.D2):
                    return data[2];
                case (NodeValues.D3):
                    return data[3];
                case (NodeValues.D4):
                    return data[4];
                case (NodeValues.D5):
                    return data[5];
                case (NodeValues.D6):
                    return data[6];
                case (NodeValues.D7):
                    return data[7];
                case (NodeValues.And):
                    return Children[0].Evaluate(address, data) && Children[1].Evaluate(address, data);
                case (NodeValues.Or):
                    return Children[0].Evaluate(address, data) || Children[1].Evaluate(address, data);
                case (NodeValues.Not):
                    return !Children[0].Evaluate(address, data);
                case (NodeValues.If):
                    return Children[0].Evaluate(address, data) ? 
                        Children[1].Evaluate(address, data) : 
                        Children[2].Evaluate(address, data);
                default:
                    throw new ArgumentException();
            }
        }
    }
}
