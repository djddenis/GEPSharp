using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEPSharp
{
    public abstract class Individual : IComparable
    {
        public IEnumerable<Domain> Domains { get; private set; }

        internal bool useNodeValueCaching = false;
        /// <summary>
        /// If using node value caching, this is the root of the best tree in this individual.
        /// </summary>
        public NodeBase RootOfBestTree { get; internal set; }
        protected bool evaluated = false;
        protected object answer;
        public object Answer
        {
            get
            {
                if (!evaluated)
                {
                    answer = Evaluate(useNodeValueCaching);
                    evaluated = true;
                }
                return answer;
            }
        }

        internal double Fitness { get; set; }

        public abstract Individual MakeCopy();

        public Individual(params Domain[] domains)
            :this(domains.AsEnumerable())
        {
        }

        public Individual(IEnumerable<Domain> domains)
        {
            Domains = domains;
            Fitness = -1;
            answer = null;
        }

        protected Individual(IEnumerable<Domain> domains, bool evaluated, object answer, double fitness)
            :this(domains)
        {
            this.evaluated = evaluated;
            this.answer = answer;
            this.Fitness = fitness;
        }

        internal virtual void ResetAnswer()
        {
            evaluated = false;
            answer = null;
        }

        /// <summary>
        /// Set answer to something in here.
        /// </summary>
        protected abstract object Evaluate(bool useAllCachedSolutions = false);

        public int CompareTo(object obj)
        {
            return ((Individual)obj).Fitness.CompareTo(this.Fitness);
        }

        public abstract string ToGraphVis();
    }
}
