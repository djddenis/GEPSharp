using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace GEPSharp
{
    public abstract class LinearDomain : Domain
    {
        public event EventHandler DataChanged; 

        private NodeBase[] data;

        public int Length { get; private set; }

        public NodeBase this[int i] 
        {
            get { return data[i]; }
            set { data[i] = value; }
        }

        public LinearDomain(int length, IEnumerable<NodeBase> nodeSet)
            :base(nodeSet)
        {
            Length = length;
            data = new NodeBase[length];
        }

        protected LinearDomain(NodeBase[] data, int length, IEnumerable<NodeBase> nodeSet)
            :base(nodeSet)
        {
            this.data = data;
            Length = length;
        }

        protected virtual void OnDataChanged()
        {
            EventHandler handler = DataChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
