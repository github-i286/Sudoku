using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using SummenSudoku;

namespace SudokuUI
{
    public partial class SudokuUI : Form
    {
        public System.Windows.Forms.TextBox[,] InputFields;
        public SudokuUI()
        {
            InitializeComponent();

            InputFields = new System.Windows.Forms.TextBox[9, 9];
            InputFields[0, 0] = A1;
            InputFields[1, 0] = A2;
            InputFields[2, 0] = A3;
            InputFields[3, 0] = A4;
            InputFields[4, 0] = A5;
            InputFields[5, 0] = A6;
            InputFields[6, 0] = A7;
            InputFields[7, 0] = A8;
            InputFields[8, 0] = A9;
            InputFields[0, 1] = B1;
            InputFields[1, 1] = B2;
            InputFields[2, 1] = B3;
            InputFields[3, 1] = B4;
            InputFields[4, 1] = B5;
            InputFields[5, 1] = B6;
            InputFields[6, 1] = B7;
            InputFields[7, 1] = B8;
            InputFields[8, 1] = B9;
            InputFields[0, 2] = C1;
            InputFields[1, 2] = C2;
            InputFields[2, 2] = C3;
            InputFields[3, 2] = C4;
            InputFields[4, 2] = C5;
            InputFields[5, 2] = C6;
            InputFields[6, 2] = C7;
            InputFields[7, 2] = C8;
            InputFields[8, 2] = C9;
            InputFields[0, 3] = D1;
            InputFields[1, 3] = D2;
            InputFields[2, 3] = D3;
            InputFields[3, 3] = D4;
            InputFields[4, 3] = D5;
            InputFields[5, 3] = D6;
            InputFields[6, 3] = D7;
            InputFields[7, 3] = D8;
            InputFields[8, 3] = D9;
            InputFields[0, 4] = E1;
            InputFields[1, 4] = E2;
            InputFields[2, 4] = E3;
            InputFields[3, 4] = E4;
            InputFields[4, 4] = E5;
            InputFields[5, 4] = E6;
            InputFields[6, 4] = E7;
            InputFields[7, 4] = E8;
            InputFields[8, 4] = E9;
            InputFields[0, 5] = F1;
            InputFields[1, 5] = F2;
            InputFields[2, 5] = F3;
            InputFields[3, 5] = F4;
            InputFields[4, 5] = F5;
            InputFields[5, 5] = F6;
            InputFields[6, 5] = F7;
            InputFields[7, 5] = F8;
            InputFields[8, 5] = F9;
            InputFields[0, 6] = G1;
            InputFields[1, 6] = G2;
            InputFields[2, 6] = G3;
            InputFields[3, 6] = G4;
            InputFields[4, 6] = G5;
            InputFields[5, 6] = G6;
            InputFields[6, 6] = G7;
            InputFields[7, 6] = G8;
            InputFields[8, 6] = G9;
            InputFields[0, 7] = H1;
            InputFields[1, 7] = H2;
            InputFields[2, 7] = H3;
            InputFields[3, 7] = H4;
            InputFields[4, 7] = H5;
            InputFields[5, 7] = H6;
            InputFields[6, 7] = H7;
            InputFields[7, 7] = H8;
            InputFields[8, 7] = H9;
            InputFields[0, 8] = I1;
            InputFields[1, 8] = I2;
            InputFields[2, 8] = I3;
            InputFields[3, 8] = I4;
            InputFields[4, 8] = I5;
            InputFields[5, 8] = I6;
            InputFields[6, 8] = I7;
            InputFields[7, 8] = I8;
            InputFields[8, 8] = I9;
            // btnClear_Click(null, null);
            UnlockFields();
            lbConditions.Items.Clear();
            ws = new WorkingSet();
            ws.sku = new Sudoku9x9();
            ws.Level = 0;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    InputFields[x, y].Text = "";
                }
            }
            UnlockFields();
            lbConditions.Items.Clear();
            ws = new WorkingSet();
            ws.sku = new Sudoku9x9();
            ws.Level = 0;
        }

        private void UnlockFields()
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    InputFields[x, y].BackColor = Color.FromKnownColor(KnownColor.Window);
                    InputFields[x, y].Enabled = true;
                }
            }
            /*
            lbConditions.Items.Clear();
            ws = new WorkingSet();
            ws.sku = new Sudoku9x9();
            ws.Level = 0;
             * */
        }

        public static void Solve(object data)
        {
            WorkingSet ws = (WorkingSet)data;
            ws.Level = 0;
            ws.Result = TryThis(ws);
        }

        bool IsRunning = false;
        WorkingSet ws;
        Thread WorkerThread;
        void Stopped()
        {
            timerDisplayUpdate.Stop();
            if (WorkerThread != null) WorkerThread.Join();
            WorkerThread = null;
            IsRunning = false;
            btnSolve.Text = "Start Lösung suchen";
            pbSolve.Value = 0;
            lblLevel.Text = "0";
        }

        private void DisplayFields()
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    Field f = ws.sku.FieldList.At(x, y);
                    if (f.IsUntetermined)
                    {
                        InputFields[x, y].Text = string.Empty;
                    }
                    else
                    {
                        InputFields[x, y].Text = (f.Number + 1).ToString();
                    }
                    if (InputFields[x, y].BackColor != Color.FromKnownColor(KnownColor.Window))
                    {
                        InputFields[x, y].BackColor = Color.FromKnownColor(KnownColor.ControlLight);
                    }
                }
            }
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            if (IsRunning)
            {
                ws.Abort = true;
                Stopped();
            }
            else
            {
                IsRunning = false;

                try
                {
                    for (int x = 0; x < 9; x++)
                    {
                        for (int y = 0; y < 9; y++)
                        {
                            string sField = InputFields[x, y].Text;
                            if (sField.Length > 0)
                            {
                                int Number;
                                if (int.TryParse(sField, out Number))
                                {
                                    if (Number >= 1 && Number <= 9)
                                    {
                                        Field f = ws.sku.FieldList.At(x, y);
                                        f.Number = Number - 1;
                                        f.IsGiven = true;
                                    }

                                    else
                                        throw new ArgumentOutOfRangeException();
                                }
                                else
                                    throw new ArgumentOutOfRangeException();
                                InputFields[x, y].BackColor = Color.FromKnownColor(KnownColor.Window);
                            }
                            else
                            {
                                InputFields[x, y].BackColor = Color.FromKnownColor(KnownColor.ControlLight);
                                // InputFields[x, y].Enabled = false;
                            }

                        }
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    DisplayFields();
                    MessageBox.Show("Ungültige Eingabe", "Eingabefehler", MessageBoxButtons.OK);
                    UnlockFields();
                    return;
                }
                switch (ws.sku.Check())
                {
                    case CheckResult.invalid:
                        DisplayFields();
                        MessageBox.Show("Ungültige Eingabe, doppelte Nummern?", "Eingabefehler", MessageBoxButtons.OK);
                        UnlockFields();
                        return;
                    case CheckResult.valid:
                        DisplayFields();
                        MessageBox.Show("Bereits gelöst!", "OK", MessageBoxButtons.OK);
                        UnlockFields();
                        return;
                }
                if (cbHiddenConditions.Checked)
                {
                    ws.sku.FindHiddenConditions();
                }

                using (StreamWriter sw = new StreamWriter(@"C:\temp\SudokuConditions.txt"))
                {
                    foreach (Field f in ws.sku.FieldList)
                    {
                        sw.WriteLine(f.ToString());
                    }

                    foreach (Condition cnd in ws.sku.Conditions)
                    {
                        sw.WriteLine(cnd.ToString());
                    }
                    sw.Close();
                }

                UnchangedSince = new int[9, 9];
                for (int x = 0; x < 9; x++)
                {
                    for (int y = 0; y < 9; y++)
                    {
                        UnchangedSince[x, y] = 0;
                    }
                }

                ws.Abort = false;
                ws.progress = 0;
                ws.Result = CheckResult.invalid;

                WorkerThread = new Thread(Solve);
                IsRunning = true;
                btnSolve.Text = "Lösen abbrechnen";
                WorkerThread.Start(ws);
                timerDisplayUpdate.Start();
            }
        }

        int[,] UnchangedSince;
        static Color c = Color.FromKnownColor(KnownColor.ControlLight);

        private void timerDisplayUpdate_Tick(object sender, EventArgs e)
        {
            if (IsRunning)
            {
                if (WorkerThread.IsAlive)
                {
                    int Progress = (int)(ws.progress * 1000);
                    if (Progress >= pbSolve.Value)
                        pbSolve.Value = Progress;
                    else
                        pbSolve.Value -= 3;
                    lblLevel.Text = ws.Level.ToString();
                    for (int x = 0; x < 9; x++)
                    {
                        for (int y = 0; y < 9; y++)
                        {
                            if (InputFields[x, y].BackColor != Color.FromKnownColor(KnownColor.Window))
                            {
                                bool Changed = false;
                                Field f = ws.sku.FieldList.At(x, y);
                                if (!f.IsUntetermined)
                                {
                                    if (string.Compare(InputFields[x, y].Text, (f.Number + 1).ToString()) != 0)
                                    {
                                        InputFields[x, y].Text = (f.Number + 1).ToString();
                                        Changed = true;
                                    }
                                }
                                else
                                {
                                    if (InputFields[x, y].Text.Length > 0)
                                    {
                                        InputFields[x, y].Text = string.Empty;
                                        Changed = true;
                                    }
                                }
                                if (Changed)
                                {
                                    InputFields[x, y].BackColor = Color.FromKnownColor(KnownColor.Yellow);
                                }
                                else if (InputFields[x, y].BackColor != c)
                                {
                                    Color cf = InputFields[x, y].BackColor;
                                    int R = cf.R, G = cf.G, B=cf.B;
                                    if (cf.R > c.R) R -= 2;
                                    if (cf.R < c.R) R += 2;
                                    if (cf.G > c.G) G -= 2;
                                    if (cf.G < c.G) G += 2;
                                    if (cf.B > c.B) B -= 2;
                                    if (cf.B < c.B) B += 2;
                                    InputFields[x, y].BackColor = Color.FromArgb(R, G, B);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Stopped();
                    DisplayFields();

                    switch (ws.Result)
                    {
                        case CheckResult.valid:
                            MessageBox.Show("Gelöst!", "OK", MessageBoxButtons.OK);
                            break;
                        case CheckResult.invalid:
                            MessageBox.Show("Fehler!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            UnlockFields();
                            break;
                        case CheckResult.undetermined: // not possible case
                            MessageBox.Show("Kann nicht gelöst werden!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            UnlockFields();
                            break;
                    }
                }

            }
        }


        static CheckResult TryThis(WorkingSet ws)
        {
            switch (ws.sku.Check())
            {
                case CheckResult.invalid:
                    return CheckResult.invalid;
                case CheckResult.valid:
                    return CheckResult.valid;
            }
            ws.Level++;
            for (int x = 0; x < 9; x++)
            {
                if (ws.Abort) return CheckResult.invalid;
                for (int y = 0; y < 9; y++)
                {
                    if (ws.Abort) return CheckResult.invalid;
                    if (ws.sku.FieldList.At(x,y).IsUntetermined)
                    {
                        ws.progress = ((float)x * 9 + y)/81;
                        if (ws.Abort) return CheckResult.invalid;
                        for (int i = 0; i < 9; i++)
                        {
                            if (ws.Abort) return CheckResult.invalid;
                            {
                                if (!ws.sku.FieldList.At(x, y).IsValid(i))
                                    continue;

                                ws.sku.FieldList.At(x,y).Number = i;
                                switch (ws.sku.Check())
                                {
                                    case CheckResult.invalid:
                                        ws.sku.FieldList.At(x, y).Number = -1;
                                        continue;
                                    case CheckResult.valid:
                                        ws.Level--;
                                        return CheckResult.valid;
                                }
                                switch (TryThis(ws))
                                {
                                    case CheckResult.invalid:
                                        ws.sku.FieldList.At(x, y).Number = -1;
                                        continue;
                                    case CheckResult.valid:
                                        ws.Level--;
                                        return CheckResult.valid;
                                }
                            }
                        }
                        ws.sku.FieldList.At(x, y).Number = -1;
                        ws.sku.FieldList.At(x, y).IsGiven = false;
                        ws.Level--;
                        return CheckResult.invalid;
                    }
                }
            }
            ws.Level--;
            return CheckResult.invalid;
        }

        List<Coordinate> CoordinateList;

        private void Field_DoubleClick(object sender, EventArgs e)
        {
            int x, y;
            for (x = 0; x < 9; x++)
            {
                for (y = 0; y < 9; y++)
                {
                    if (InputFields[x, y] == sender)
                    {
                        goto Found;
                    }
                }
            }
            MessageBox.Show("Ungültige Eingabe, Feld?", "Eingabefehler", MessageBoxButtons.OK);
            return;

        Found:
            InputFields[x, y].BackColor = Color.FromKnownColor(KnownColor.Yellow);
            if (CoordinateList == null)
            {
                CoordinateList = new List<Coordinate>();
            }
            CoordinateList.Add(new Coordinate(x, y));
            return;
        }

        private void btnAddSum_Click(object sender, EventArgs e)
        {
            if (CoordinateList == null || CoordinateList.Count < 2)
            {
                MessageBox.Show("Keine 2 Felder selektiert", "Selektion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int Sum;
            if (!int.TryParse(txtSum.Text, out Sum) || Sum < 2)
            {
                MessageBox.Show("Keine gültige Summe", "Selektion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Fields FieldList = new Fields();
            string sFieldList = "";
            foreach (Coordinate crd in CoordinateList)
            {
                InputFields[crd.x, crd.y].BackColor = Color.FromKnownColor(KnownColor.LightPink);
                FieldList.Add(ws.sku.FieldList.At(crd));
                sFieldList += Coordinate.ColumnsNames[crd.x] + (crd.y + 1).ToString() + " ";
            }
            SumCondition cnd = new SumCondition(Sum - FieldList.Count, FieldList);
            ws.sku.Conditions.Add(cnd);

            sFieldList = "Sum: " + Sum.ToString() + " from " + sFieldList;
            lbConditions.Items.Add(sFieldList);

            FieldList = null; // reset field list
            txtSum.Text = "";
            CoordinateList = null;
        }

        private void btn1x9Block_Click(object sender, EventArgs e)
        {
            if (CoordinateList == null || CoordinateList.Count < 2)
            {
                MessageBox.Show("Keine 2 Felder selektiert", "Selektion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Fields FieldList = new Fields();
            string sFieldList = "";
            foreach (Coordinate crd in CoordinateList)
            {
                InputFields[crd.x, crd.y].BackColor = Color.FromKnownColor(KnownColor.LightPink);
                FieldList.Add(ws.sku.FieldList.At(crd));
                sFieldList += Coordinate.ColumnsNames[crd.x] + (crd.y + 1).ToString() + " ";
            }
            AllNumberCondition cnd = new AllNumberCondition(9, FieldList);

            sFieldList = "1-9 Block: " + sFieldList;
            lbConditions.Items.Add(sFieldList);

            FieldList = null; // reset field list
            txtSum.Text = "";
            CoordinateList = null;
        }

        private void Field_MouseMove(object sender, MouseEventArgs e)
        {
            int x, y;
            for (x = 0; x < 9; x++)
            {
                for (y = 0; y < 9; y++)
                {
                    if (InputFields[x, y] == sender)
                    {
                        goto Found;
                    }
                }
            }
            lblDetails.Text = "";
            return;

        Found:
            Field f = ws.sku.FieldList.At(x, y);
            lblDetails.Text = f.ToString();
        }
        private void SudokuUI_MouseEnter(object sender, EventArgs e)
        {
            lblDetails.Text = "";
        }


        private void btnExample1_Click(object sender, EventArgs e)
        {
            btnClear_Click(null, null);
            InputFields[0, 0].Text = "4";
            InputFields[0, 8].Text = "8";
            InputFields[1, 2].Text = "3";
            InputFields[1, 3].Text = "1";
            InputFields[1, 4].Text = "6";
            InputFields[1, 5].Text = "4";
            InputFields[1, 6].Text = "2";
            InputFields[2, 1].Text = "6";
            InputFields[2, 7].Text = "5";
            InputFields[3, 3].Text = "3";
            InputFields[3, 5].Text = "9";
            InputFields[4, 2].Text = "4";
            InputFields[4, 4].Text = "1";
            InputFields[4, 6].Text = "9";
            InputFields[5, 3].Text = "7";
            InputFields[5, 5].Text = "2";
            InputFields[6, 1].Text = "3";
            InputFields[6, 7].Text = "1";
            InputFields[7, 2].Text = "9";
            InputFields[7, 3].Text = "2";
            InputFields[7, 4].Text = "8";
            InputFields[7, 5].Text = "3";
            InputFields[7, 6].Text = "5";
            InputFields[8, 0].Text = "5";
            InputFields[8, 8].Text = "4";
        }

        private void btnExample2_Click(object sender, EventArgs e)
        {
            btnClear_Click(null, null);

            InputFields[0, 4].Text = "3";
            InputFields[4, 4].Text = "1";
            InputFields[8, 4].Text = "9";

            Fields FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(0, 0));
            FieldList.Add(ws.sku.FieldList.At(0, 1));
            FieldList.Add(ws.sku.FieldList.At(0, 2));
            FieldList.Add(ws.sku.FieldList.At(0, 3));
            ws.sku.Conditions.Add(new SumCondition(22 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(1, 0));
            FieldList.Add(ws.sku.FieldList.At(1, 1));
            ws.sku.Conditions.Add(new SumCondition(6 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(2, 0));
            FieldList.Add(ws.sku.FieldList.At(2, 1));
            FieldList.Add(ws.sku.FieldList.At(2, 2));
            FieldList.Add(ws.sku.FieldList.At(1, 2));
            ws.sku.Conditions.Add(new SumCondition(18 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(3, 0));
            FieldList.Add(ws.sku.FieldList.At(4, 0));
            ws.sku.Conditions.Add(new SumCondition(5 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(3, 1));
            FieldList.Add(ws.sku.FieldList.At(3, 2));
            ws.sku.Conditions.Add(new SumCondition(6 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(5, 0));
            FieldList.Add(ws.sku.FieldList.At(4, 1));
            FieldList.Add(ws.sku.FieldList.At(5, 1));
            FieldList.Add(ws.sku.FieldList.At(4, 2));
            ws.sku.Conditions.Add(new SumCondition(28 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(6, 0));
            FieldList.Add(ws.sku.FieldList.At(6, 1));
            ws.sku.Conditions.Add(new SumCondition(11 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(7, 0));
            FieldList.Add(ws.sku.FieldList.At(7, 1));
            FieldList.Add(ws.sku.FieldList.At(8, 0));
            FieldList.Add(ws.sku.FieldList.At(8, 1));
            ws.sku.Conditions.Add(new SumCondition(21 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(5, 2));
            FieldList.Add(ws.sku.FieldList.At(6, 2));
            FieldList.Add(ws.sku.FieldList.At(7, 2));
            FieldList.Add(ws.sku.FieldList.At(8, 2));
            ws.sku.Conditions.Add(new SumCondition(19 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(1, 3));
            FieldList.Add(ws.sku.FieldList.At(2, 3));
            ws.sku.Conditions.Add(new SumCondition(11 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(3, 3));
            FieldList.Add(ws.sku.FieldList.At(3, 4));
            ws.sku.Conditions.Add(new SumCondition(16 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(0, 4));
            FieldList.Add(ws.sku.FieldList.At(0, 5));
            FieldList.Add(ws.sku.FieldList.At(1, 4));
            FieldList.Add(ws.sku.FieldList.At(2, 4));
            ws.sku.Conditions.Add(new SumCondition(19 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(1, 5));
            FieldList.Add(ws.sku.FieldList.At(2, 5));
            ws.sku.Conditions.Add(new SumCondition(14 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(4, 3));
            FieldList.Add(ws.sku.FieldList.At(5, 3));
            FieldList.Add(ws.sku.FieldList.At(4, 4));
            FieldList.Add(ws.sku.FieldList.At(4, 5));
            FieldList.Add(ws.sku.FieldList.At(3, 5));
            ws.sku.Conditions.Add(new SumCondition(20 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(5, 4));
            FieldList.Add(ws.sku.FieldList.At(5, 5));
            ws.sku.Conditions.Add(new SumCondition(9 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(6, 3));
            FieldList.Add(ws.sku.FieldList.At(7, 3));
            ws.sku.Conditions.Add(new SumCondition(14 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(8, 3));
            FieldList.Add(ws.sku.FieldList.At(8, 4));
            FieldList.Add(ws.sku.FieldList.At(7, 4));
            FieldList.Add(ws.sku.FieldList.At(6, 4));
            ws.sku.Conditions.Add(new SumCondition(20 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(6, 5));
            FieldList.Add(ws.sku.FieldList.At(7, 5));
            ws.sku.Conditions.Add(new SumCondition(4 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(0, 6));
            FieldList.Add(ws.sku.FieldList.At(1, 6));
            FieldList.Add(ws.sku.FieldList.At(2, 6));
            FieldList.Add(ws.sku.FieldList.At(3, 6));
            ws.sku.Conditions.Add(new SumCondition(10 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(0, 7));
            FieldList.Add(ws.sku.FieldList.At(1, 7));
            FieldList.Add(ws.sku.FieldList.At(0, 8));
            FieldList.Add(ws.sku.FieldList.At(1, 8));
            ws.sku.Conditions.Add(new SumCondition(29 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(2, 7));
            FieldList.Add(ws.sku.FieldList.At(2, 8));
            ws.sku.Conditions.Add(new SumCondition(8 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(4, 6));
            FieldList.Add(ws.sku.FieldList.At(4, 7));
            FieldList.Add(ws.sku.FieldList.At(3, 7));
            FieldList.Add(ws.sku.FieldList.At(3, 8));
            ws.sku.Conditions.Add(new SumCondition(26 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(5, 6));
            FieldList.Add(ws.sku.FieldList.At(5, 7));
            ws.sku.Conditions.Add(new SumCondition(11 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(4, 8));
            FieldList.Add(ws.sku.FieldList.At(5, 8));
            ws.sku.Conditions.Add(new SumCondition(6 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(6, 6));
            FieldList.Add(ws.sku.FieldList.At(6, 7));
            FieldList.Add(ws.sku.FieldList.At(6, 8));
            FieldList.Add(ws.sku.FieldList.At(7, 6));
            ws.sku.Conditions.Add(new SumCondition(29 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(7, 7));
            FieldList.Add(ws.sku.FieldList.At(7, 8));
            ws.sku.Conditions.Add(new SumCondition(3 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(8, 5));
            FieldList.Add(ws.sku.FieldList.At(8, 6));
            FieldList.Add(ws.sku.FieldList.At(8, 7));
            FieldList.Add(ws.sku.FieldList.At(8, 8));
            ws.sku.Conditions.Add(new SumCondition(20 - FieldList.Count, FieldList));
        }

        private void btnExample3_Click(object sender, EventArgs e)
        {
            btnClear_Click(null, null);

            Fields FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(0, 0));
            FieldList.Add(ws.sku.FieldList.At(1, 0));
            FieldList.Add(ws.sku.FieldList.At(2, 0));
            ws.sku.Conditions.Add(new SumCondition(13 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(3, 0));
            FieldList.Add(ws.sku.FieldList.At(4, 0));
            ws.sku.Conditions.Add(new SumCondition(17 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(6, 0));
            FieldList.Add(ws.sku.FieldList.At(7, 0));
            ws.sku.Conditions.Add(new SumCondition(3 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(0, 1));
            FieldList.Add(ws.sku.FieldList.At(1, 1));
            FieldList.Add(ws.sku.FieldList.At(2, 1));
            FieldList.Add(ws.sku.FieldList.At(2, 2));
            ws.sku.Conditions.Add(new SumCondition(18 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(5, 0));
            FieldList.Add(ws.sku.FieldList.At(3, 1));
            FieldList.Add(ws.sku.FieldList.At(4, 1));
            FieldList.Add(ws.sku.FieldList.At(5, 1));
            ws.sku.Conditions.Add(new SumCondition(22 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(8, 0));
            FieldList.Add(ws.sku.FieldList.At(8, 1));
            FieldList.Add(ws.sku.FieldList.At(8, 2));
            ws.sku.Conditions.Add(new SumCondition(22 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(0, 2));
            FieldList.Add(ws.sku.FieldList.At(1, 2));
            FieldList.Add(ws.sku.FieldList.At(1, 3));
            FieldList.Add(ws.sku.FieldList.At(1, 4));
            ws.sku.Conditions.Add(new SumCondition(26 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(3, 2));
            FieldList.Add(ws.sku.FieldList.At(4, 2));
            FieldList.Add(ws.sku.FieldList.At(2, 3));
            FieldList.Add(ws.sku.FieldList.At(3, 3));
            FieldList.Add(ws.sku.FieldList.At(4, 3));
            FieldList.Add(ws.sku.FieldList.At(2, 4));
            ws.sku.Conditions.Add(new SumCondition(21 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(5, 2));
            FieldList.Add(ws.sku.FieldList.At(6, 2));
            FieldList.Add(ws.sku.FieldList.At(5, 3));
            ws.sku.Conditions.Add(new SumCondition(7 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(6, 1));
            FieldList.Add(ws.sku.FieldList.At(7, 1));
            FieldList.Add(ws.sku.FieldList.At(7, 2));
            FieldList.Add(ws.sku.FieldList.At(6, 3));
            FieldList.Add(ws.sku.FieldList.At(7, 3));
            ws.sku.Conditions.Add(new SumCondition(30 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(0, 3));
            FieldList.Add(ws.sku.FieldList.At(0, 4));
            FieldList.Add(ws.sku.FieldList.At(0, 5));
            ws.sku.Conditions.Add(new SumCondition(18 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(0, 6));
            FieldList.Add(ws.sku.FieldList.At(0, 7));
            FieldList.Add(ws.sku.FieldList.At(0, 8));
            ws.sku.Conditions.Add(new SumCondition(11 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(1, 5));
            FieldList.Add(ws.sku.FieldList.At(2, 5));
            FieldList.Add(ws.sku.FieldList.At(1, 6));
            FieldList.Add(ws.sku.FieldList.At(1, 7));
            FieldList.Add(ws.sku.FieldList.At(2, 7));
            ws.sku.Conditions.Add(new SumCondition(20 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(1, 8));
            FieldList.Add(ws.sku.FieldList.At(2, 8));
            ws.sku.Conditions.Add(new SumCondition(15 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(3, 4));
            FieldList.Add(ws.sku.FieldList.At(4, 4));
            FieldList.Add(ws.sku.FieldList.At(5, 4));
            ws.sku.Conditions.Add(new SumCondition(17 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(3, 5));
            FieldList.Add(ws.sku.FieldList.At(3, 6));
            FieldList.Add(ws.sku.FieldList.At(2, 6));
            ws.sku.Conditions.Add(new SumCondition(11 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(6, 4));
            FieldList.Add(ws.sku.FieldList.At(6, 5));
            FieldList.Add(ws.sku.FieldList.At(4, 5));
            FieldList.Add(ws.sku.FieldList.At(5, 5));
            FieldList.Add(ws.sku.FieldList.At(4, 6));
            FieldList.Add(ws.sku.FieldList.At(5, 6));
            ws.sku.Conditions.Add(new SumCondition(33 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(8, 3));
            FieldList.Add(ws.sku.FieldList.At(8, 4));
            FieldList.Add(ws.sku.FieldList.At(8, 5));
            ws.sku.Conditions.Add(new SumCondition(9 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(7, 4));
            FieldList.Add(ws.sku.FieldList.At(7, 5));
            FieldList.Add(ws.sku.FieldList.At(7, 6));
            FieldList.Add(ws.sku.FieldList.At(8, 6));
            ws.sku.Conditions.Add(new SumCondition(25 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(3, 7));
            FieldList.Add(ws.sku.FieldList.At(3, 8));
            FieldList.Add(ws.sku.FieldList.At(4, 7));
            FieldList.Add(ws.sku.FieldList.At(5, 7));
            ws.sku.Conditions.Add(new SumCondition(27 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(4, 8));
            FieldList.Add(ws.sku.FieldList.At(5, 8));
            ws.sku.Conditions.Add(new SumCondition(11 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(6, 6));
            FieldList.Add(ws.sku.FieldList.At(6, 7));
            FieldList.Add(ws.sku.FieldList.At(7, 7));
            FieldList.Add(ws.sku.FieldList.At(8, 7));
            ws.sku.Conditions.Add(new SumCondition(23 - FieldList.Count, FieldList));

            FieldList = new Fields();
            FieldList.Add(ws.sku.FieldList.At(6, 8));
            FieldList.Add(ws.sku.FieldList.At(7, 8));
            FieldList.Add(ws.sku.FieldList.At(8, 8));
            ws.sku.Conditions.Add(new SumCondition(6 - FieldList.Count, FieldList));
        }

        private void btnExample4_Click(object sender, EventArgs e)
        {
/*
            btnClear_Click(null, null);

            Conditions = new List<Condition>();

            InputFields[0, 4].Text = "8";
            InputFields[7, 4].Text = "7";
            InputFields[8, 4].Text = "9";
            InputFields[7, 5].Text = "8";
            InputFields[4, 5].Text = "9";
            InputFields[4, 6].Text = "8";
            InputFields[1, 3].Text = "9";
            InputFields[1, 4].Text = "2";

            SumCondition cnd = new SumCondition(12);
            cnd.FieldList.Add(new Coordinate(0, 0));
            cnd.FieldList.Add(new Coordinate(0, 1));
            cnd.FieldList.Add(new Coordinate(0, 2));
            cnd.FieldList.Add(new Coordinate(0, 3));
            Conditions.Add(cnd);

            cnd = new SumCondition(38);
            cnd.FieldList.Add(new Coordinate(1, 0));
            cnd.FieldList.Add(new Coordinate(2, 0));
            cnd.FieldList.Add(new Coordinate(3, 0));
            cnd.FieldList.Add(new Coordinate(3, 1));
            cnd.FieldList.Add(new Coordinate(3, 2));
            cnd.FieldList.Add(new Coordinate(3, 3));
            Conditions.Add(cnd);

            cnd = new SumCondition(24);
            cnd.FieldList.Add(new Coordinate(1, 1));
            cnd.FieldList.Add(new Coordinate(2, 1));
            cnd.FieldList.Add(new Coordinate(1, 2));
            cnd.FieldList.Add(new Coordinate(2, 2));
            Conditions.Add(cnd);            

            cnd = new SumCondition(12);
            cnd.FieldList.Add(new Coordinate(4, 0));
            cnd.FieldList.Add(new Coordinate(5, 0));
            cnd.FieldList.Add(new Coordinate(4, 1));
            Conditions.Add(cnd);

            cnd = new SumCondition(3);
            cnd.FieldList.Add(new Coordinate(6, 0));
            cnd.FieldList.Add(new Coordinate(7, 0));
            Conditions.Add(cnd);

            cnd = new SumCondition(22);
            cnd.FieldList.Add(new Coordinate(8, 0));
            cnd.FieldList.Add(new Coordinate(8, 1));
            cnd.FieldList.Add(new Coordinate(8, 2));
            cnd.FieldList.Add(new Coordinate(8, 3));
            Conditions.Add(cnd);

            cnd = new SumCondition(7);
            cnd.FieldList.Add(new Coordinate(4, 2));
            cnd.FieldList.Add(new Coordinate(4, 3));
            Conditions.Add(cnd);

            cnd = new SumCondition(18);
            cnd.FieldList.Add(new Coordinate(5, 2));
            cnd.FieldList.Add(new Coordinate(6, 2));
            cnd.FieldList.Add(new Coordinate(7, 2));
            cnd.FieldList.Add(new Coordinate(7, 3));
            Conditions.Add(cnd);

            cnd = new SumCondition(8);
            cnd.FieldList.Add(new Coordinate(2, 3));
            cnd.FieldList.Add(new Coordinate(2, 4));
            cnd.FieldList.Add(new Coordinate(2, 5));
            Conditions.Add(cnd);

            cnd = new SumCondition(22);
            cnd.FieldList.Add(new Coordinate(5, 3));
            cnd.FieldList.Add(new Coordinate(5, 4));
            cnd.FieldList.Add(new Coordinate(4, 4));
            cnd.FieldList.Add(new Coordinate(3, 4));
            cnd.FieldList.Add(new Coordinate(3, 5));
            Conditions.Add(cnd);

            cnd = new SumCondition(14);
            cnd.FieldList.Add(new Coordinate(6, 3));
            cnd.FieldList.Add(new Coordinate(6, 4));
            cnd.FieldList.Add(new Coordinate(6, 5));
            Conditions.Add(cnd);

            cnd = new SumCondition(25);
            cnd.FieldList.Add(new Coordinate(0, 5));
            cnd.FieldList.Add(new Coordinate(0, 6));
            cnd.FieldList.Add(new Coordinate(0, 7));
            cnd.FieldList.Add(new Coordinate(0, 8));
            Conditions.Add(cnd);

            cnd = new SumCondition(19);
            cnd.FieldList.Add(new Coordinate(1, 5));
            cnd.FieldList.Add(new Coordinate(1, 6));
            cnd.FieldList.Add(new Coordinate(2, 6));
            cnd.FieldList.Add(new Coordinate(3, 6));
            Conditions.Add(cnd);

            cnd = new SumCondition(22);
            cnd.FieldList.Add(new Coordinate(5, 5));
            cnd.FieldList.Add(new Coordinate(5, 6));
            cnd.FieldList.Add(new Coordinate(5, 7));
            cnd.FieldList.Add(new Coordinate(5, 8));
            cnd.FieldList.Add(new Coordinate(6, 8));
            cnd.FieldList.Add(new Coordinate(7, 8));
            Conditions.Add(cnd);

            cnd = new SumCondition(14);
            cnd.FieldList.Add(new Coordinate(8, 5));
            cnd.FieldList.Add(new Coordinate(8, 6));
            cnd.FieldList.Add(new Coordinate(8, 7));
            cnd.FieldList.Add(new Coordinate(8, 8));
            Conditions.Add(cnd);

            cnd = new SumCondition(11);
            cnd.FieldList.Add(new Coordinate(1, 7));
            cnd.FieldList.Add(new Coordinate(2, 7));
            cnd.FieldList.Add(new Coordinate(3, 7));
            Conditions.Add(cnd);

            cnd = new SumCondition(5);
            cnd.FieldList.Add(new Coordinate(1, 8));
            cnd.FieldList.Add(new Coordinate(2, 8));
            Conditions.Add(cnd);

            cnd = new SumCondition(19);
            cnd.FieldList.Add(new Coordinate(4, 7));
            cnd.FieldList.Add(new Coordinate(4, 8));
            cnd.FieldList.Add(new Coordinate(3, 8));
            Conditions.Add(cnd);

            cnd = new SumCondition(27);
            cnd.FieldList.Add(new Coordinate(6, 6));
            cnd.FieldList.Add(new Coordinate(7, 6));
            cnd.FieldList.Add(new Coordinate(6, 7));
            cnd.FieldList.Add(new Coordinate(7, 7));
            Conditions.Add(cnd);

            SubSetNumberCondition ssnc = new SubSetNumberCondition(new int[3] { 1, 3, 4 });
            ssnc.FieldList.Add(new Coordinate(2, 3));
            ssnc.FieldList.Add(new Coordinate(2, 4));
            ssnc.FieldList.Add(new Coordinate(2, 5));
            Conditions.Add(ssnc);

            ssnc = new SubSetNumberCondition(new int[2] { 7, 8 });
            ssnc.FieldList.Add(new Coordinate(3, 3));
            ssnc.FieldList.Add(new Coordinate(5, 3));
            Conditions.Add(ssnc);

            ssnc = new SubSetNumberCondition(new int[3] { 9, 8, 6 });
            ssnc.FieldList.Add(new Coordinate(5, 1));
            ssnc.FieldList.Add(new Coordinate(6, 1));
            ssnc.FieldList.Add(new Coordinate(7, 1));
            Conditions.Add(ssnc);
*/
        }

        private void btnCondDelete_Click(object sender, EventArgs e)
        {
            if (lbConditions.SelectedIndex >= 0)
            {
                ws.sku.Conditions.RemoveAt(ws.sku.Conditions.Count-lbConditions.Items.Count+lbConditions.SelectedIndex);
                lbConditions.Items.RemoveAt(lbConditions.SelectedIndex);
            }
        }


    }

    public class WorkingSet
    {
        public Sudoku9x9 sku;
        public bool Abort;
        public float progress;
        public CheckResult Result;
        public int Level;
    }
}
