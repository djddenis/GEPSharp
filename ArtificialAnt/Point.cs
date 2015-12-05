using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialAnt
{
    public class Point
    {
        public int x;
        public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Point()
            : this(0, 0)
        { }

        public override bool Equals(object obj)
        {
            Point other = obj as Point;
            return other == null ? false : other.x == x && other.y == y;
        }

        public override int GetHashCode()
        {
            return 17 * x + 31 * y;
        }
    }
}
