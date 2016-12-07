using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SummenSudoku
{
    public class ImpossibleConditionException : Exception
    {
    }

    public class Field
    {
        public Coordinate cx;
        public bool[] NumberArray;
        public bool IsGiven;
        public int Number;

        public Field(Coordinate cx, int number, bool IsGiven)
        {
            if (!IsGiven) throw new ArgumentException("IsGiven parameter muste be true");
            this.cx = cx;
            this.Number = number;
            IsGiven = true;
        }

        public Field(Coordinate cx, int ToNumber)
        {
            this.cx = cx;
            NumberArray = new bool[ToNumber];
            for (int i = 0; i < (ToNumber); i++)
            {
                NumberArray[i] = true; // assuem all values are possible
            }
            this.Number = -1; // unknown
            IsGiven = false;
        }

        public bool Exclude(int Number)
        {
            if (IsGiven && Number == this.Number)
            {
                throw new ImpossibleConditionException();
            }
            if (IsSingleNumber) return false; // is a single number anyway
            if (!NumberArray[Number]) return false;   // already excluded
            NumberArray[Number] = false;
            int LastNumber = -1;
            int Count = 0;
            for (int i = 0; i < NumberArray.Length; i++)
            {
                if (NumberArray[i])
                {
                    LastNumber = i;
                    Count++;
                }
            }
            if (Count == 1) // only one possible number
            {
                this.Number = LastNumber;
            }
            return true;
        }

        public bool IsSingleNumber
        {
            get
            {
                if (IsGiven || this.Number >= 0) return true;
                int count = 0;
                for (int i = 0; i < NumberArray.Length; i++)
                {
                    if (NumberArray[i]) count++;
                }
                if (count == 0) throw new ImpossibleConditionException();
                return count == 1;
            }
        }
        public bool IsPair
        {
            get
            {
                return GetValidNumbers().Length == 2;
            }
        }
        public bool IsTriple
        {
            get
            {
                return GetValidNumbers().Length == 3;
            }
        }
        public int[] GetValidNumbers()
        {
            if (IsSingleNumber)
                return new int[1] { Number };
            else
            {
                int count = 0;
                for (int i = 0; i < NumberArray.Length; i++)
                {
                    if (NumberArray[i]) count++;
                }
                int[] ret = new int[count];
                for (int i = 0, k = 0; i < NumberArray.Length; i++)
                {
                    if (NumberArray[i]) ret[k++] = i;
                }
                return ret;
            }
        }

        public bool IsValid(int TestNumber)
        {
            if (IsSingleNumber)
            {
                return TestNumber == this.Number;
            }
            return NumberArray[TestNumber];
        }

        public bool IsUntetermined
        {
            get { return this.Number < 0; }
        }

        static public bool operator ==(Field A, Field B) // check if A equals B
        {
            return A.cx == B.cx;
        }
        static public bool operator !=(Field A, Field B) // check if A equals B
        {
            return A.cx != B.cx;
        }


        public override string ToString()
        {
            string s = "Field at " + cx.ToString() + " ";
            if (IsGiven || (this.Number >= 0))
            {
                s += "given number " + (Number + 1).ToString();
            }
            else
            {
                int i = 1;
                bool first = true;
                foreach (bool b in NumberArray)
                {
                    if (b)
                    {
                        if (!first) s += "/";
                        s += i.ToString();
                        first = false;
                    }
                    i++;
                }
            }

            return s;
        }
    }

    public class Fields : List<Field>
    {
        public Fields Clone() // Copy operator
        {
            Fields f = new Fields();
            foreach (Field c in this)
            {
                f.Add(c);
            }
            return f;
        }

        public Field At(int x, int y)
        {
            Coordinate cx = new Coordinate(x,y);
            foreach (Field f in this)
            {
                if (f.cx == cx) return f;
            }
            return null;
        }

        public Field At(Coordinate cx)
        {
            foreach (Field f in this)
            {
                if (f.cx == cx) return f;
            }
            return null;
        }

        static public bool operator ==(Fields A, Fields B) // check if A equals B
        {
            foreach (Field a in A)
            {
                foreach (Field b in B)
                {
                    if (a == b) goto Found;
                }
                return false;
            Found:
                continue;
            }
            foreach (Field b in B)
            {
                foreach (Field a in A)
                {
                    if (a == b) goto Found;
                }
                return false;
            Found:
                continue;
            }
            return true;
        }

        static public bool operator !=(Fields A, Fields B) // check if A is not equals B
        {
            foreach (Field a in A)
            {
                foreach (Field b in B)
                {
                    if (a == b) goto Found;
                }
                return true;
            Found:
                continue;
            }
            foreach (Field b in B)
            {
                foreach (Field a in A)
                {
                    if (a == b) goto Found;
                }
                return true;
            Found:
                continue;
            }
            return false;
        }

        static public bool operator &(Fields A, Field C) // check if C is in A
        {
            foreach (Field c in A)
            {
                if (c == C)
                {
                    return true;
                }
            }
            return false;
        }

        static public Fields operator &(Fields A, Fields B) // Schnittmenge zwischen A nd B
        {
            Fields fs = new Fields();
            foreach (Field f in A)
            {
                if ((B & f)) // wenn f in B vorkommt, übernehmen
                {
                    fs.Add(f);
                }
            }
            return fs;
        }

        static public Fields operator +(Fields A, Fields B) // Union zwischen A und B
        {
            Fields fs = A.Clone();
            foreach (Field f in B)
            {
                if (!(A & f)) // nur wenn c in A nicht bereits vorkommt übernehmen
                {
                    fs.Add(f);
                }
            }
            return fs;
        }

        static public Fields operator -(Fields A, Fields B) // A minues Elemente von B
        {
            Fields fs = new Fields();
            foreach (Field f in A)
            {
                if (!(B & f)) // nur wenn c in B nicht vorkommt übernehmen
                {
                    fs.Add(f);
                }
            }
            return fs;
        }
    }

}
