using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KlonsF.Classes;
using KlonsF.Forms;
using KlonsLIB.Forms;
using KlonsLIB.Misc;
using Microsoft.Reporting.WinForms;

namespace KlonsF.FormsReportParams
{
    public partial class FormRep_PVNZ1 : MyFormBaseF
    {
        public FormRep_PVNZ1()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        private DateTime startDate = DateTime.MinValue;
        private DateTime endDate = DateTime.MinValue;
        private int firstPageNr = 1;
        private int firstPRowNr = 1;

        private void FormRepApgr1_Load(object sender, EventArgs e)
        {
            int yr = DateTime.Today.Year;
            cbYear.ItemStrings = new string[]{(yr-1).ToString(), yr.ToString()};
            LoadParams();
            SetControlsUpDownOrder(new Control[][]
            {
                new Control[] {cbYear, cbMonth},
                new Control[] {tbPageNr, tbPageNr},
                new Control[] {tbRowNr, tbRowNr},
                new Control[] {cmDoIt, cmDoIt}
            });
        }


        private void LoadParams()
        {
            string s = MyData.Params.RED;
            DateTime d1;
            if (!Utils.StringToDate(s, out d1)) return;
            startDate = new DateTime(d1.Year,d1.Month,1);
            endDate = startDate.AddMonths(1).AddDays(-1);
            cbYear.Text = startDate.Year.ToString();
            cbMonth.SelectedIndex = startDate.Month - 1;
        }

        public override void SaveParams()
        {
            MyData.Params.RED = Utils.DateToString(endDate);
        }

        private string Check()
        {
            string syear = cbYear.Text;
            string smonth = cbMonth.Text;

            if (syear == "" || smonth == "")
                return "Jāievada gads, mēnesis.";

            int year, month;

            if (!int.TryParse(syear, out year) || !int.TryParse(smonth, out month))
                return "Nekorekts gads vai mēnesis";
            
            if (year < 2000 || year >2100)
                return "Nekorekts gads.";

            if (!int.TryParse(tbPageNr.Text, out firstPageNr) || !int.TryParse(tbRowNr.Text, out firstPRowNr))
                return "Nekorekts lapas vai rindas numurs";

            startDate = new DateTime(year, month, 1);
            endDate = startDate.AddMonths(1).AddDays(-1);

            return "OK";
        }

        private void DoIt()
        {
            string rt = Check();
            if (rt != "OK")
            {
                MyMainForm.ShowWarning(rt);
                return;
            }

            string speriod = $"{startDate.Year}. gada {Utils.MonthNames[startDate.Month - 1]}";
            
            SaveParams();

            var rows_pvnz = DataLoaderF.TREPPVNZ1_GetBy_pvnz_01(startDate, endDate);

            MyData.ReportHelperF.PrepareTRepPVNZ1(rows_pvnz);

            ReportViewerData rd = new ReportViewerData();
            rd.FileName = "Report_PVNZ_1";
            rd.Sources["DataSet1"] = rows_pvnz;
            rd.AddReportParameters(
                new string[]
                {
                    "RPERIOD", speriod,
                    "CompanyName", MyData.Params.CompNameX,
                    "PRID0", firstPRowNr.ToString(),
                    "PFirstPageNr", firstPageNr.ToString()
                });

            MyMainForm.ShowReport(rd);
        }

        private void cmDoIt_Click(object sender, EventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                DoIt();

            });
        }

        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            OnNaviKey(sender, e);
        }

    }
}
