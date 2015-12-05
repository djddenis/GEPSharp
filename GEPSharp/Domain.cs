using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEPSharp
{
    public abstract class Domain
    {
        protected IEnumerable<NodeBase> nodeSet;

        public Domain(IEnumerable<NodeBase> nodeSet)
        {
            this.nodeSet = nodeSet;
        }

        public NodeBase RandomNodeFromSet()
        {
            return nodeSet.ElementAt(StaticRandom.Next(nodeSet.Count()));
        }

        public abstract Domain MakeCopy();
    }
}
