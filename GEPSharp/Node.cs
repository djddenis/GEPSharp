using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEPSharp
{
    public delegate object NodeFunction(object[] args);

    public class NodeBase
    {
        public string Name { get; private set; }

        public int Arity { get; private set; }

        public bool IsTerminal { get { return Arity == 0; } }

        public bool Evaluated { get; private set; }
        private object cachedValue = null;

        protected NodeFunction function;

        public virtual NodeBase MakeCopy()
        {
            return new NodeBase(Name, Arity, function);
        }

        public NodeBase(string name, object data)
            : this(name, 0, (i => data))
        {
        }

        public NodeBase(string name, int arity, NodeFunction function)
        {
            Name = name;
            Arity = arity;
            this.function = function;
            Evaluated = false;
        }

        public object Value(object[] args, bool fromCache = false)
        {
            if (fromCache && Evaluated)
                return cachedValue;

            cachedValue = function(args);
            Evaluated = true;
            return cachedValue;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
