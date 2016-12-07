using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SummenSudoku
{
    public class Sudoku2D
    {
        public Fields FieldList;
        public List<Condition> Conditions;
    }

    public class Sudoku9x9 : Sudoku2D
    {
        public Sudoku9x9()
        {
            Conditions = new List<Condition>();
            FieldList = new Fields();
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    Field f = new Field(new Coordinate(x, y), 9);
                    FieldList.Add(f);
                }
            }

            // square blocks
            AddAllNumberAndBlockBlockCondition(9, 0, 2, 0, 2);
            AddAllNumberAndBlockBlockCondition(9, 3, 5, 0, 2);
            AddAllNumberAndBlockBlockCondition(9, 6, 8, 0, 2);
            AddAllNumberAndBlockBlockCondition(9, 0, 2, 3, 5);
            AddAllNumberAndBlockBlockCondition(9, 3, 5, 3, 5);
            AddAllNumberAndBlockBlockCondition(9, 6, 8, 3, 5);
            AddAllNumberAndBlockBlockCondition(9, 0, 2, 6, 8);
            AddAllNumberAndBlockBlockCondition(9, 3, 5, 6, 8);
            AddAllNumberAndBlockBlockCondition(9, 6, 8, 6, 8);

            // line blocks horizonatlly
            AddAllNumberAndBlockBlockCondition(9, 0, 8, 0, 0);
            AddAllNumberAndBlockBlockCondition(9, 0, 8, 1, 1);
            AddAllNumberAndBlockBlockCondition(9, 0, 8, 2, 2);
            AddAllNumberAndBlockBlockCondition(9, 0, 8, 3, 3);
            AddAllNumberAndBlockBlockCondition(9, 0, 8, 4, 4);
            AddAllNumberAndBlockBlockCondition(9, 0, 8, 5, 5);
            AddAllNumberAndBlockBlockCondition(9, 0, 8, 6, 6);
            AddAllNumberAndBlockBlockCondition(9, 0, 8, 7, 7);
            AddAllNumberAndBlockBlockCondition(9, 0, 8, 8, 8);

            // line blocks vertically
            AddAllNumberAndBlockBlockCondition(9, 0, 0, 0, 8);
            AddAllNumberAndBlockBlockCondition(9, 1, 1, 0, 8);
            AddAllNumberAndBlockBlockCondition(9, 2, 2, 0, 8);
            AddAllNumberAndBlockBlockCondition(9, 3, 3, 0, 8);
            AddAllNumberAndBlockBlockCondition(9, 4, 4, 0, 8);
            AddAllNumberAndBlockBlockCondition(9, 5, 5, 0, 8);
            AddAllNumberAndBlockBlockCondition(9, 6, 6, 0, 8);
            AddAllNumberAndBlockBlockCondition(9, 7, 7, 0, 8);
            AddAllNumberAndBlockBlockCondition(9, 8, 8, 0, 8);
        }

        public void AddSumBlockCondition(int Sum, int xFrom, int xTo, int yFrom, int yTo)
        {
            Fields ConditionFields = new Fields();
            for (int x = xFrom; x <= xTo; x++)
            {
                for (int y = yFrom; y <= yTo; y++)
                {
                    ConditionFields.Add(FieldList.At(x, y));
                }
            }
            SumCondition cnd = new SumCondition(Sum, ConditionFields);
            Conditions.Add(cnd);
        }

        public void AddAllNumberBlockCondition(int toNumber, int xFrom, int xTo, int yFrom, int yTo)
        {
            Fields ConditionFields = new Fields();
            for (int x = xFrom; x <= xTo; x++)
            {
                for (int y = yFrom; y <= yTo; y++)
                {
                    ConditionFields.Add(FieldList.At(x, y));
                }
            }
            AllNumberCondition cnd = new AllNumberCondition(toNumber, ConditionFields);
            Conditions.Add(cnd);
        }
        public void AddAllNumberAndBlockBlockCondition(int toNumber, int xFrom, int xTo, int yFrom, int yTo)
        {
            int iSum = 0;
            for (int i = 0; i < toNumber; i++)
            {
                iSum += i;
            }
            AddAllNumberBlockCondition(toNumber, xFrom, xTo, yFrom, yTo);
            AddSumBlockCondition(iSum, xFrom, xTo, yFrom, yTo);
        }

        private bool FieldInList(Field f, Fields Group)
        {
            return (Group & f); // When cx in Group: true
        }

        private bool CheckInAllNumberBlock(Fields FieldList)
        {
            foreach (Condition cnd in Conditions)
            {
                if (cnd is AllNumberCondition)
                {
                    AllNumberCondition ancnd = (AllNumberCondition)cnd;
                    if ((FieldList & ancnd.FieldList) == FieldList)
                        return true;
                }
            }
            return false;
        }

        public void FindHiddenConditions()
        {
            List<Condition> HiddenConditions = new List<Condition>();

            foreach (Condition cnd in Conditions)
            {
                if (cnd is SumCondition)
                {
                    SumCondition scnd = (SumCondition) cnd;
                    SubSetNumberCondition ssncnd;
                    MultipleNumberCondition mnc;
                    switch (scnd.FieldList.Count)
                    {
                        case 1:
                            ssncnd = new SubSetNumberCondition(new int[1] { scnd.Sum }, scnd.FieldList);
                            HiddenConditions.Add(ssncnd);
                            break;
                        case 2:
                            if (CheckInAllNumberBlock(scnd.FieldList))
                            {
                                switch (scnd.Sum)
                                {
                                    case 1:
                                        ssncnd = new SubSetNumberCondition(new int[2] { 0, 1 }, scnd.FieldList);
                                        ssncnd.FieldList = scnd.FieldList;
                                        HiddenConditions.Add(ssncnd);
                                        break;
                                    case 2:
                                        ssncnd = new SubSetNumberCondition(new int[2] { 0, 2 }, scnd.FieldList);
                                        ssncnd.FieldList = scnd.FieldList;
                                        HiddenConditions.Add(ssncnd);
                                        break;
                                    case 3:
                                        mnc = new MultipleNumberCondition(new int[2][] { new int[2] { 0, 3 }, new int[2] { 1, 2 } }, scnd.FieldList);
                                        mnc.FieldList = scnd.FieldList;
                                        HiddenConditions.Add(mnc);
                                        break;
                                    case 4:
                                        mnc = new MultipleNumberCondition(new int[2][] { new int[2] { 0, 4 }, new int[2] { 1, 3 } }, scnd.FieldList);
                                        mnc.FieldList = scnd.FieldList;
                                        HiddenConditions.Add(mnc);
                                        break;
                                    case 13:
                                        mnc = new MultipleNumberCondition(new int[2][] { new int[2] { 8, 5 }, new int[2] { 7, 6 } }, scnd.FieldList);
                                        mnc.FieldList = scnd.FieldList;
                                        HiddenConditions.Add(mnc);
                                        break;
                                    case 12:
                                        mnc = new MultipleNumberCondition(new int[2][] { new int[2] { 8, 4 }, new int[2] { 7, 5 } }, scnd.FieldList);
                                        mnc.FieldList = scnd.FieldList;
                                        HiddenConditions.Add(mnc);
                                        break;
                                    case 15:
                                        ssncnd = new SubSetNumberCondition(new int[2] { 7, 8 }, scnd.FieldList);
                                        ssncnd.FieldList = scnd.FieldList;
                                        HiddenConditions.Add(ssncnd);
                                        break;
                                    case 14:
                                        ssncnd = new SubSetNumberCondition(new int[2] { 6, 8 }, scnd.FieldList);
                                        ssncnd.FieldList = scnd.FieldList;
                                        HiddenConditions.Add(ssncnd);
                                        break;
                                }
                                break;
                            }
                            break;
                        case 3:
                            if (CheckInAllNumberBlock(scnd.FieldList))
                            {
                                switch (scnd.Sum)
                                {
                                    case 3:
                                        ssncnd = new SubSetNumberCondition(new int[3] { 0, 1, 2 }, scnd.FieldList);
                                        ssncnd.FieldList = scnd.FieldList;
                                        HiddenConditions.Add(ssncnd);
                                        break;
                                    case 4:
                                        ssncnd = new SubSetNumberCondition(new int[3] { 0, 1, 3 }, scnd.FieldList);
                                        ssncnd.FieldList = scnd.FieldList;
                                        HiddenConditions.Add(ssncnd);
                                        break;
                                    case 5:
                                        mnc = new MultipleNumberCondition(new int[2][] { new int[3] { 0, 1, 4 }, new int[3] { 0, 2, 3 } }, scnd.FieldList);
                                        mnc.FieldList = scnd.FieldList;
                                        HiddenConditions.Add(mnc);
                                        break;
                                    case 19:
                                        mnc = new MultipleNumberCondition(new int[2][] { new int[3] { 8, 7, 4 }, new int[3] { 8, 6, 5 } }, scnd.FieldList);
                                        mnc.FieldList = scnd.FieldList;
                                        HiddenConditions.Add(mnc);
                                        break;
                                    case 21:
                                        ssncnd = new SubSetNumberCondition(new int[3] { 6, 7, 8 }, scnd.FieldList);
                                        ssncnd.FieldList = scnd.FieldList;
                                        HiddenConditions.Add(ssncnd);
                                        break;
                                    case 20:
                                        ssncnd = new SubSetNumberCondition(new int[3] { 5, 7, 8 }, scnd.FieldList);
                                        ssncnd.FieldList = scnd.FieldList;
                                        HiddenConditions.Add(ssncnd);
                                        break;
                                }
                                break;
                            }
                            break;
                    }
                }
            }
            foreach (Condition cnd in HiddenConditions)
            {
                Conditions.Add(cnd);
            }

            bool ExcludesFound = true;
            while (ExcludesFound)
            {
                ExcludesFound = false;
                foreach (Condition cnd in Conditions)
                {
                    if (cnd is AllNumberCondition)
                    {
                        AllNumberCondition anc = (AllNumberCondition)cnd;
                        List<int[]> Pairs = new List<int[]>();
                        List<int[]> Triples = new List<int[]>();

                        foreach (Condition scnd in Conditions)
                        {
                            if (scnd is SubSetNumberCondition)
                            {
                                SubSetNumberCondition ssnc = (SubSetNumberCondition)scnd;
                                foreach (Field f in ssnc.FieldList)
                                {
                                    for (int i = 0; i < 9; i++)
                                    {
                                        foreach (int n in ssnc.Numbers)
                                        {
                                            if (i == n)
                                                goto Found;
                                        }
                                        if (f.Exclude(i))
                                        {
                                            ExcludesFound = true;
                                        }
                                     Found:
                                        continue;
                                    }
                                }
                                if ((anc.FieldList & ssnc.FieldList) == ssnc.FieldList) // ssnc all containted in anc
                                {
                                    foreach (Field f in anc.FieldList)
                                    {
                                        if (!(ssnc.FieldList & f))
                                        {
                                            foreach (int n in ssnc.Numbers)
                                            {
                                                if (f.Exclude(n))
                                                {
                                                    ExcludesFound = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }


                        foreach (Field f in anc.FieldList)
                        {
                            if (f.IsSingleNumber)
                            {
                                foreach (Field f2 in anc.FieldList)
                                {
                                    if (f2 != f) // all otheres except the original
                                    {
                                        if (f2.Exclude(f.Number))
                                        {
                                            ExcludesFound = true;
                                        }
                                    }
                                }
                            }
                            else if (f.IsPair)  // detact pairs of values
                            {
                                Pairs.Add(f.GetValidNumbers());
                            }
                            else if (f.IsTriple)
                            {
                                Triples.Add(f.GetValidNumbers());
                            }
                        }
                        foreach (int[] pair in Pairs)   // check if 2 fields have the same pair
                        {
                            foreach (int[] pair2 in Pairs)
                            {
                                if (pair != pair2)
                                {
                                    if (CompareIntArrays(pair, pair2))  // yes, 2 fieds have the same pair
                                    {
                                        // now exlude each other from that all number group from this pairs
                                        foreach (Field f in anc.FieldList)
                                        {
                                            if (!CompareIntArrays(pair, f.GetValidNumbers()))    // not the paired-fields
                                            {
                                                foreach (int iEx in pair)   // exlcude the numbers
                                                {
                                                    if (f.Exclude(iEx))
                                                    {
                                                        ExcludesFound = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        foreach (int[] triple1 in Triples)   // check if 3 fields have the same triple
                        {
                            foreach (int[] triple2 in Triples)
                            {
                                if (triple1 != triple2)
                                {
                                    if (CompareIntArrays(triple1, triple2))  // yes, 2 fieds have the same triple 
                                    {
                                        foreach (int[] triple3 in Triples)
                                        {
                                            if (triple3 != triple2 && triple3 != triple1)
                                            {
                                                if (CompareIntArrays(triple1, triple3))  // yes, all 3 fieds have the same triple
                                                {
                                                    // now exlude each other from that all number group from this pairs
                                                    foreach (Field f in anc.FieldList)
                                                    {
                                                        if (!CompareIntArrays(triple1, f.GetValidNumbers()))    // not the tripled-fields
                                                        {
                                                            foreach (int iEx in triple1)   // exlcude the numbers
                                                            {
                                                                if (f.Exclude(iEx))
                                                                {
                                                                    ExcludesFound = true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        static bool CompareIntArrays(int[] A, int[] B)
        {
            if (A.Length != B.Length) return false;
            foreach (int a in A)
            {
                foreach (int b in B)
                {
                    if (a == b) goto Found;
                }
                return false;
            Found:
                continue;
            }
            foreach (int b in B)
            {
                foreach (int a in A)
                {
                    if (a == b) goto Found;
                }
                return false;
            Found:
                continue;
            }
            return true;
        }

        public CheckResult Check()
        {
            CheckResult cr = CheckResult.valid;

            // check if all conditions are valid
            foreach (Field f in FieldList)
            {
                
            }

            foreach (Condition cnd in Conditions)
            {
                switch (cnd.Check())
                {
                    case CheckResult.valid:
                        break;
                    case CheckResult.invalid:
                        return CheckResult.invalid;
                    case CheckResult.undetermined:
                        cr = CheckResult.undetermined;
                        break;
                }
            }
            return cr;
        }


    }
}
