using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEPSharp
{
    public class Terminal : Node
    {
        private object data;

        public override object Value(params object[] args)
        {
            return data;
        }

        public Terminal(string name, object data)
            : base(name, 0)
        {
            this.data = data;
        }
    }
}
