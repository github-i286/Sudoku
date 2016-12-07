using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SummenSudoku
{
    public class Coordinate
    {
        public int x;
        public int y;
        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        static public string[] ColumnsNames = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I" };

        public override string ToString()
        {
            return ColumnsNames[x] + y.ToString();
        }
        static public bool operator ==(Coordinate A, Coordinate B)
        {
            return (A.x == B.x) && (A.y == B.y);
        }
        static public bool operator !=(Coordinate A, Coordinate B)
        {
            return (A.x != B.x) || (A.y != B.y);
        }
        public override int GetHashCode()
        {
            return (x << 16) + y;
        }
        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return ((Coordinate)obj) == this;
        }

    }
}
