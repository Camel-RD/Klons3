using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Klons3.ModelsFRep;
using KlonsF.Classes;
using KlonsF.Forms;
using KlonsLIB.Forms;
using KlonsLIB.Misc;
using Microsoft.Reporting.WinForms;

namespace KlonsF.FormsReportParams
{
    public partial class FormRep_NPMT : MyFormBaseF
    {
        public FormRep_NPMT()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        private DateTime startDate = DateTime.MinValue;
        private DateTime endDate = DateTime.MinValue;

        private void FormRepApgr1_Load(object sender, EventArgs e)
        {
            lbCM.SelectedIndex = 0;
            int yr = DateTime.Today.Year;
            cbYear.ItemStrings = new string[]{(yr-1).ToString(), yr.ToString()};
            LoadParams();
            SetControlsUpDownOrder(new Control[][]
            {
                new Control[] {cbYear, cbMonth},
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

            startDate = new DateTime(year, 1, 1);
            endDate = new DateTime(year, month, 1);
            endDate = endDate.AddMonths(1).AddDays(-1);

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

            int selectedReport = lbCM.SelectedIndex;
            if (selectedReport == -1) return;

            string speriod = string.Format("{0}. gada jan.-{1}.", 
                startDate.Year,
                Utils.MonthNames[endDate.Month - 1].LeftMax(3));
            
            SaveParams();

            ReportViewerData rd = new ReportViewerData();

            List<F_TREPMT> row_trepmt = null;

            switch (selectedReport)
            {
                case 0:
                    row_trepmt = DataLoaderF.TREPMT_GetBy_npmt_14(startDate, endDate);
                    MyData.ReportHelperF.PrepareTRepMTForNPMT1(row_trepmt, endDate.Month);
                    rd.FileName = "Report_NPMT_1";
                    break;
                case 1:
                    row_trepmt = DataLoaderF.TREPMT_GetBy_npmt_24(startDate, endDate);
                    MyData.ReportHelperF.PrepareTRepMTForNPMT1(row_trepmt, endDate.Month);
                    rd.FileName = "Report_NPMT_2";
                    break;
                case 2:
                    row_trepmt = DataLoaderF.TREPMT_GetBy_npmt_44(startDate, endDate);
                    MyData.ReportHelperF.PrepareTRepMTForNPMT1(row_trepmt, endDate.Month);
                    rd.FileName = "Report_NPMT_1";
                    break;
                case 3:
                    row_trepmt = DataLoaderF.TREPMT_GetBy_npmt_31(startDate, endDate);
                    MyData.ReportHelperF.PrepareTRepMTForNPMT3(row_trepmt);
                    rd.FileName = "Report_NPMT_3";
                    break;
            }


            rd.Sources["DataSet1"] = row_trepmt;
            rd.AddReportParameters(
                new string[]
                {
                    "RPERIOD", speriod,
                    "CompanyName", MyData.Params.CompNameX
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

        private void lbCM_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                DoIt();

            });
        }

    }
}
