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
    public partial class FormRep_Darz1 : MyFormBaseF
    {
        public FormRep_Darz1()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        private DateTime startDate = DateTime.MinValue;
        private DateTime endDate = DateTime.MinValue;
        private string filterAc1 = null;
        private string filterAc3 = null;
        private string kaname = "";
        private string baname = "";
        private string cnname = "";

        private void FormRepApgr1_Load(object sender, EventArgs e)
        {
            lbCM.SelectedIndex = 0;
            int yr = DateTime.Today.Year;
            cbYear.ItemStrings = new string[]{(yr-1).ToString(), yr.ToString()};

            var table_acp = MyData.DbContextF.F_ACP21.Local.ToList();
            var list_ac = table_acp
                .Where(x => x.ID1 == "KA" || x.ID1 == "BA" || x.ID1 == "CN")
                .Select(x => x.AC);
            var list_ac1 = new[] { "*", "KA", "BA", "CN" };
            list_ac1 = list_ac1.Concat(list_ac).ToArray();
            cbFilter.ItemStrings = list_ac1;
            cbFilter.SelectedIndex = 0;

            LoadParams();
            SetControlsUpDownOrder(new Control[][]
            {
                new Control[] {cbYear, cbMonth},
                new Control[] {cbFilter, cmDoIt}
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

            if (!int.TryParse(syear, out year))
                return "Nekorekts gads";

            if (!int.TryParse(smonth, out month) && smonth != "Gads")
                return "Nekorekts mēnesis";

            if (year < 2000 || year >2100)
                return "Nekorekts gads.";

            if (smonth == "Gads")
            {
                startDate = new DateTime(year, 1, 1);
                endDate = new DateTime(year, 12, 31);
            }
            else
            {
                startDate = new DateTime(year, month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
            }

            var filter = cbFilter.Text;
            filterAc1 = null;
            filterAc3 = null;
            kaname = "";
            baname = "";
            cnname = "";

            if (filter == "KA" || filter == "BA" || filter == "CN")
            {
                filterAc3 = filter;
                if (filter == "KA") kaname = "KA";
                if (filter == "BA") baname = "BA";
                if (filter == "CN") cnname = "CN";
            }
            else if (filter != "*")
            {
                filterAc1 = filter;
                var dr_ac = MyData.DbContextF.F_ACP21.Find(filter);
                if(dr_ac != null)
                {
                    var ac3 = dr_ac.ID1;
                    var ac = dr_ac.AC;
                    if (ac3 == "KA") kaname = ac;
                    if (ac3 == "BA") baname = ac;
                    if (ac3 == "CN") cnname = ac;
                }
            }
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

            DateTime startDateOfYear = new DateTime(startDate.Year, 1, 1);
            DateTime endDateOfYear = new DateTime(startDate.Year, 12, 31);
            string speriod = "";
            if(startDate == startDateOfYear && endDateOfYear == endDate)
            {
                speriod = $"{startDate.Year}. gads";
            }
            else
            {
                speriod = $"{startDate.Year}. gada {Utils.MonthNames[startDate.Month - 1]}";
            }

            SaveParams();

            ReportViewerData rd = new ReportViewerData();

            List<F_TREPDARZ1> rows_trepdarz1 = null;
            List<F_TREPDARZ2> rows_trepdarz2 = null;

            switch (selectedReport)
            {
                case 0:
                case 1:
                case 2:
                    rows_trepdarz1 = DataLoaderF.TREPDARZ1_GetBy_darz_1(startDate, endDate, filterAc1, filterAc3);
                    rows_trepdarz2 = DataLoaderF.TREPDARZ2_GetBy_darz_2(startDateOfYear, startDate.AddDays(-1), filterAc1, filterAc3);
                    break;
                case 3:
                    rows_trepdarz1 = DataLoaderF.TREPDARZ1_GetBy_darz_3(startDate, endDate);
                    rows_trepdarz2 = DataLoaderF.TREPDARZ2_GetBy_darz_4(startDateOfYear, startDate.AddDays(-1));
                    break;
            }

            rd.FileName = selectedReport switch
            {
                0 => "Report_Darz_4",
                1 => "Report_Darz_3",
                2 => "Report_Darz_1",
                3 => "Report_Darz_2",
                _ => null
            };

            kaname = kaname.Nz();
            baname = baname.Nz();
            cnname = cnname.Nz();
            if (kaname != "") kaname = " - " + kaname;
            if (baname != "") baname = " - " + baname;
            if (cnname != "") cnname = " - " + cnname;

            MyData.ReportHelperF.PrepareTRepDarz1(rows_trepdarz1);

            rd.Sources["DataSet1"] = rows_trepdarz1;
            rd.Sources["DataSet2"] = rows_trepdarz2;
            rd.AddReportParameters(
                new string[]
                {
                        "RPERIOD", speriod,
                        "CompanyName", MyData.Params.CompNameX
                });
            if (rd.FileName == "Report_Darz_1" || rd.FileName == "Report_Darz_4")
            {
                rd.AddReportParameters(
                    new string[]
                    {
                        "KAName", kaname,
                        "BAName", baname,
                        "CNName", cnname
                    });
            }
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
