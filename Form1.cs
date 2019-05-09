using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace Transform
{
    public partial class Form1 : Form
    {

        private struct TargetRecord
        {
            public string AccountCode;
            public string Name;
            public string Type;
            public string OpenDate;
            public string Currency;
        }

        public Form1()
        {
            InitializeComponent();
        }


        #region TextAndStringFunctions  

        public List<string> ReadTextFile(string FName, bool _UTFEncoding = false)
        {
            List<string> SL = new List<string>();
            Encoding Enc = Encoding.Default;

            switch (_UTFEncoding)
            {
                case false:
                    {
                        Enc = Encoding.Default;
                        break;
                    }

                case true:
                    {
                        Enc = Encoding.UTF8;
                        break;
                    }
            }

            if (File.Exists(FName))
            {
                StreamReader SR = new StreamReader(FName, Enc);
                while (SR.Peek() >= 0)
                    SL.Add(SR.ReadLine());
                SR.Close();
            }
            else
                MessageBox.Show(FName + " not found!");
            Enc = null;
            return SL;
        }
        public string ParseStr(string TS, int WhichOne, string Delimiter = ";")
        {
            int I = 0;
            int l = 0;
            int Cnt = 0;
            int _Start = 0;
            int _End = 0;
            string S;

            S = TS;

            l = S.Length;

            if (S.Substring(S.Length - 1, 1) != Delimiter)
                S = S + Delimiter;

            for (I = 1; I <= l; I++)
            {
                if (S.Substring(I, 1) == Delimiter)
                    Cnt = Cnt + 1;
            }
            if (Cnt < WhichOne)
            {
                return("");
            }

            Cnt = 0;
            I = 1;
            while (Cnt < WhichOne)
            {
                if (S.Substring(I, 1) == Delimiter)
                {
                    Cnt = Cnt + 1;
                    if (Cnt < WhichOne)
                        _Start = I + 1;
                }
                I = I + 1;
            }
            _End = I - 1;
            return(S.Substring(_Start, _End - _Start));
        }

        public string FindInStringList(List<string> SL, string Key, int LookUpItem, int ReturnItem)
        {
            string Result = "";
            string LItm;

            for (var I = 0; I <= SL.Count - 1; I++)
            {
                LItm = ParseStr(SL[I], LookUpItem).Trim();

                if (LItm == Key)
                {
                    Result = ParseStr(SL[I], ReturnItem).Trim();
                    break;
                }
            }

            return (Result);
        }
        #endregion



        private void ProcessFiles(string File1, string File2)
        {
            const string CrLf = "\r\n";
            List<string> SL1 = new List<string>();
            List<string> SL2 = new List<string>();
            List<TargetRecord> Tbl = new List<TargetRecord>();
            TargetRecord  LocalRecord;
            string StrLine = "";
            string TargetCSV = "";

            SL1 = ReadTextFile(File1);
            SL2 = ReadTextFile(File2);

            for (var I = 1; I <= SL1.Count - 1; I++)
            {
                LocalRecord = new TargetRecord();
                StrLine = SL1[I].Trim();
                if (StrLine != "")
                {
                    LocalRecord.AccountCode = ParseStr(ParseStr(StrLine, 1), 2, "|");
                    LocalRecord.Name = ParseStr(StrLine, 2);
                    LocalRecord.OpenDate = ParseStr(StrLine, 4);
                    LocalRecord.Currency = ParseStr(StrLine, 5);
                    LocalRecord.Type = FindInStringList(SL2, LocalRecord.Name, 1, 2);
                    Tbl.Add(LocalRecord);
                }
                LocalRecord = default(TargetRecord);
            }

            SL1 = null;
            SL2 = null;

            LocalRecord = new TargetRecord();
            FieldInfo[] FieldNames = LocalRecord.GetType().GetFields();
            LocalRecord = default(TargetRecord);

            foreach (FieldInfo field in FieldNames)
            {
                TargetCSV = TargetCSV + field.Name + ";";

            }
            TargetCSV = TargetCSV + CrLf;

            for (var I = 0; I <= Tbl.Count - 1; I++)
                TargetCSV = TargetCSV + Tbl[I].AccountCode + ";" + Tbl[I].Name + ";" + Tbl[I].Type + ";" + Tbl[I].OpenDate + ";" + Tbl[I].Currency + CrLf;

            try
            {
                System.IO.StreamWriter objWriter = new System.IO.StreamWriter(Application.StartupPath + @"\Target.csv");
                objWriter.Write(TargetCSV);
                objWriter.Close();
                objWriter = null;
                MessageBox.Show("Target.csv file created!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR! : Target.csv file couldn't created!");
            }

            Tbl = null;
        }


        private void Button1_Click(object sender, EventArgs e)
        {
            ProcessFiles(Application.StartupPath + @"\File1.csv", Application.StartupPath + @"\File2.csv");
        }
    }
}
