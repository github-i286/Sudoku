using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SummenSudoku
{
    public enum CheckResult { valid, invalid, undetermined };



    public class Condition
    {
        public virtual CheckResult Check()
        {
            return CheckResult.undetermined;
        }
    }

    public class SumCondition : Condition
    {
        public Fields FieldList;
        public int Sum;
        public SumCondition(int Sum, Fields FieldList)
        {
            this.FieldList = FieldList;
            this.Sum = Sum;
        }

        public override CheckResult Check()
        {
            int UnterterminedFields = 0;
            int UnterterminedMaxSum = 0;
            int UnterterminedMinSum = 0;
            int iSum = 0;
            foreach (Field f in FieldList)
            {
                if (f.IsUntetermined)
                {
                    UnterterminedMaxSum += 8 - UnterterminedFields;
                    UnterterminedMinSum += UnterterminedFields;
                    UnterterminedFields++;
                }
                else
                {
                    iSum += f.Number;
                }
            }
            if (UnterterminedFields > 0)
            {
                if (iSum > Sum)
                    return CheckResult.invalid;

                if ((iSum + UnterterminedMaxSum) < Sum)
                    return CheckResult.invalid;

                if ((iSum + UnterterminedMinSum) > Sum)
                    return CheckResult.invalid;

                return CheckResult.undetermined;
            }
            if (iSum == Sum) 
                return CheckResult.valid;
            else
                return CheckResult.invalid;
        }

        public override string ToString()
        {
            string s = "SumCondition sum=" + Sum.ToString() + " for";
            foreach (Field f in FieldList)
            {
                s += " " + f.cx.ToString(); 
            }
            return s;
        }

    }

    public class AllNumberCondition : Condition
    {
        public Fields FieldList;
        public int FromNumber;
        public int ToNumber;
        public AllNumberCondition(int ToNumber, Fields FieldList)
        {
            this.FieldList = FieldList;
            this.ToNumber = ToNumber;
        }
        public override CheckResult Check()
        {
            bool[] Exists = new bool[ToNumber];
            for (int i = 0; i < ToNumber; i++)
            {
                Exists[i] = false;
            }

            foreach (Field f in FieldList)
            {
                if (!f.IsUntetermined)
                {
                    if (Exists[f.Number])
                    {
                        return CheckResult.invalid;
                    }
                    else
                    {
                        Exists[f.Number] = true;
                    }
                }
            }
            for (int i = 0; i < ToNumber; i++)
            {
                if (!Exists[i])
                    return CheckResult.undetermined;
            }
            return CheckResult.valid;
        }
        public override string ToString()
        {
            string s = "AllNumberCondition ToNumber=" + ToNumber.ToString() + " for";
            foreach (Field f in FieldList)
            {
                s += " " + f.cx.ToString(); 
            }
            return s;
        }
    }

    public class SubSetNumberCondition : Condition
    {
        public Fields FieldList;
        public int[] Numbers;
        public SubSetNumberCondition(int[] Numbers, Fields FieldList)
        {
            this.FieldList = FieldList;
            this.Numbers = Numbers;
        }

        public override CheckResult Check()
        {
            bool Undetermined = false;
            foreach (Field f in FieldList)
            {
                if (!f.IsUntetermined)
                {
                    foreach (int i in Numbers)
                    {
                        if (i == f.Number) goto Found;
                    }
                    return CheckResult.invalid;
                }
                else
                    Undetermined = true;
            Found:
                continue;
            }
            if (Undetermined) return CheckResult.undetermined;
            return CheckResult.valid;
        }
        public override string ToString()
        {
            string s = "SubSetNumberCondition Numbers=";
            foreach (int i in Numbers)
            {
                s += " " + i.ToString();
            }
            s += " for";
            foreach (Field f in FieldList)
            {
                s += " " + f.cx.ToString(); 
            }
            return s;
        }
    }

    public class MultipleNumberCondition : Condition
    {
        public Fields FieldList;
        public int[][] NumbersArray;
        public MultipleNumberCondition(int[][] NumbersArray, Fields FieldList)
        {
            this.FieldList = FieldList;
            this.NumbersArray = NumbersArray;
        }

        public override CheckResult Check()
        {
            bool OnceUndetermined = false;
            foreach (int[] Numbers in NumbersArray)
            {
                bool Undetermined = false;
                bool Invalid = false;
                foreach (Field f in FieldList)
                {
                    if (!f.IsUntetermined)
                    {
                        foreach (int i in Numbers)
                        {
                            if (i == f.Number)
                            {
                                goto Found;
                            }
                        }
                        Invalid = true;
                    }
                    else
                    {
                        OnceUndetermined = Undetermined = true;
                    }
                Found:
                    continue;
                }
                if (!Invalid && !Undetermined)
                    return CheckResult.valid;
            }
            if (OnceUndetermined) return CheckResult.undetermined;
            return CheckResult.invalid;
        }
        public override string ToString()
        {
            string s = "MultipleNumberCondition Numbergroups=";
            foreach (int[] Numbers in NumbersArray)
            {
                bool first = true;
                foreach (int i in Numbers)
                {
                    if (first) s += " "; else s += "/";
                    s += i.ToString();
                    first = false;
                }
                s += " ";
            }
            s += "for";
            foreach (Field f in FieldList)
            {
                s += " " + f.cx.ToString(); 
            }
            return s;
        }
    }

    public class ExcludeCondition : Condition
    {
        public Field Field;
        public int Number;
        public ExcludeCondition(Field Field, int Number)
        {
            this.Field = Field;
            this.Number = Number;
        }

        public override CheckResult Check()
        {
            if (Field.IsUntetermined) return CheckResult.undetermined;
            if (Field.Number == Number) return CheckResult.invalid;
            return CheckResult.valid;
        }

        public override string ToString()
        {
            return "ExcludeCondition Number= " + Number.ToString() + " from " + Field.cx.ToString();
        }
    }
}
