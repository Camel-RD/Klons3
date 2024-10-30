using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Klons3.ModelsF;
using Klons3.ModelsFRep;
using KlonsF.Classes;
using KlonsF.Forms;
using KlonsLIB.Forms;
using KlonsLIB.Misc;
using Microsoft.Reporting.WinForms;

namespace KlonsF.FormsReportParams
{
    public partial class FormRep_Koresp : MyFormBaseF
    {
        public FormRep_Koresp()
        {
            InitializeComponent();
            toolStrip1.Renderer = MyMainForm.MainMenuStrip.Renderer;
            CheckMyFontAndColors();
        }

        private DateTime startDate = DateTime.MinValue;
        private DateTime endDate = DateTime.MinValue;
        private string ac = "";
        private string clid = "";
        private string startDateStr = "";
        private string endDateStr = "";

        private void FormRepKoresp1_Load(object sender, EventArgs e)
        {
            lbACName.Text = "";
            lbClName.Text = "";
            lbCm.SelectedIndex = 0;
            LoadParams();
            SetControlsUpDownOrder(new Control[][]
            {
                new Control[] {tbSD, tbED},
                new Control[] {cbAC, cbAC},
                new Control[] {cbClid, cbClid},
                new Control[] {cmDoIt, cmDoIt}
            });
        }

        private void CheckAcName()
        {
            string s = cbAC.Text;
            if (s == "")
            {
                lbACName.Text = "";
                return;
            }
            lbACName.Text = DataTasks.GetAcName(s);
        }
        private void CheckClName()
        {
            string s = cbClid.Text;
            if (s == "")
            {
                lbClName.Text = "";
                return;
            }
            lbClName.Text = DataTasks.GetClName(s);
        }

        private void cbAC_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private void cbAC_TextChanged(object sender, EventArgs e)
        {
            CheckAcName();
        }
        private void cbClid_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private void cbClid_TextChanged(object sender, EventArgs e)
        {
            CheckClName();
        }
        private bool DoOnF5(ComboBox sender)
        {
            Action<string> act =
                value =>
                {
                    if (value != null)
                        sender.Text = value;
                };
            if (sender == cbAC)
            {
                MyMainForm.ShowFormAcplanDialog(act);
                return true;
            }

            if (sender == cbClid)
            {
                MyMainForm.ShowFormPersonsDialog(act);
                return true;
            }
            return false;
        }

        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            OnNaviKey(sender, e);
            switch (e.KeyCode)
            {
                case Keys.F5:
                    if (sender is ComboBox)
                        if (DoOnF5(sender as ComboBox))
                        {
                            e.Handled = true;
                        }
                    break;
            }
        }

        private void cbClid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DoOnF5(sender as ComboBox);
        }
        private void lbCm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                DoIt();

            });
        }

        private void LoadParams()
        {
            tbSD.Text = MyData.Params.RSD;
            tbED.Text = MyData.Params.RED;
            cbAC.Text = MyData.Params.RAC;
            cbClid.Text = MyData.Params.RPER;
            CheckAcName();
            CheckClName();
        }

        public override void SaveParams()
        {
            MyData.Params.RSD = startDateStr;
            MyData.Params.RED = endDateStr;
            MyData.Params.RAC = cbAC.Text;
            MyData.Params.RACNM = lbACName.Text;
            MyData.Params.RPER = cbClid.Text;
        }

        private string Check()
        {
            if (tbSD.Text == "" || tbED.Text == "")
                return "Jāievada datums.";

            if (!Utils.StringToDate(tbSD.Text, out startDate) ||
                !Utils.StringToDate(tbED.Text, out endDate) ||
                startDate > endDate)
                return "Nekorekts datums.";

            startDateStr = Utils.DateToString(startDate);
            endDateStr = Utils.DateToString(endDate);

            ac = cbAC.Text;
            if (ac == "") ac = "%";
            ac = ac.Replace('*', '%');

            clid = cbClid.Text;

            return "OK";
        }

        private void cmDoIt_Click(object sender, EventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                DoIt();

            });
        }

        private void DoIt()
        {
            string rt = Check();
            if (rt != "OK")
            {
                MyMainForm.ShowWarning(rt);
                return;
            }
            SaveParams();

            int repid = lbCm.SelectedIndex;
            if (cbClid.Text == "*") cbClid.Text = "";
            if (cbClid.Text != "") repid += 7;

            List<F_ROPS1A> rows_rops1a = null;
            List<F_ROPS2A> rows_rops2a = null;
            List<F_ROPS3A> rows_rops3a = null;


            ReportViewerData rd = new ReportViewerData();
            ReportViewerData rdsub = null;

            switch (repid)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    rows_rops1a = DataLoaderF.ROPS1A_GetBy_koresp_11(startDate, endDate, ac);
                    rows_rops2a = DataLoaderF.ROPS2A_GetBy_apgr_01(startDate, endDate, ac);
                    rows_rops3a = DataLoaderF.ROPS3A_GetBy_koresp_01(startDate, endDate, ac);
                    break;
                case 4:
                    rows_rops1a = DataLoaderF.ROPS1A_GetBy_koresp_11(startDate, endDate, ac);
                    rows_rops2a = DataLoaderF.ROPS2A_GetBy_apgr_02(startDate, endDate, ac);
                    rows_rops3a = DataLoaderF.ROPS3A_GetBy_koresp_02(startDate, endDate, ac);
                    break;
                case 6:
                    rows_rops2a = DataLoaderF.ROPS2A_GetBy_apgr_01(startDate, endDate, ac);
                    rows_rops3a = DataLoaderF.ROPS3A_GetBy_koresp_01(startDate, endDate, ac);
                    break;
                case 7:
                case 8:
                case 9:
                    rows_rops1a = DataLoaderF.ROPS1A_GetBy_koresp_11_clid(startDate, endDate, ac, clid);
                    rows_rops2a = DataLoaderF.ROPS2A_GetBy_apgr_01_clid(startDate, endDate, ac, clid);
                    rows_rops3a = DataLoaderF.ROPS3A_GetBy_koresp_01_clid(startDate, endDate, ac, clid);
                    break;
                case 10:
                    rows_rops1a = DataLoaderF.ROPS1A_GetBy_koresp_11_clid(startDate, endDate, ac, clid);
                    rows_rops2a = DataLoaderF.ROPS2A_GetBy_apgr_02_clid(startDate, endDate, ac, clid);
                    rows_rops3a = DataLoaderF.ROPS3A_GetBy_koresp_02_clid(startDate, endDate, ac, clid);
                    break;
                case 5:
                case 11:
                    rows_rops1a = DataLoaderF.ROPS1A_GetBy_koresp_11(startDate, endDate, ac);
                    rows_rops2a = DataLoaderF.ROPS2A_GetBy_apgr_02_clid(startDate, endDate, ac, clid);
                    rows_rops3a = DataLoaderF.ROPS3A_GetBy_koresp_02_clid(startDate, endDate, ac, clid);
                    break;
            }


            if (rows_rops1a != null)
                MyData.ReportHelperF.PrepareRops1a(rows_rops1a);
            MyData.ReportHelperF.PrepareRops2a(rows_rops2a);
            MyData.ReportHelperF.PrepareRops2aRAC(rows_rops2a);

            if (repid != 6)
                rd.Sources["DataSet1"] = rows_rops1a;
            rd.Sources["DataSet_2a"] = rows_rops2a;
            rd.Sources["DataSet_3a"] = rows_rops3a;

            rd.AddReportParameters(
                new string[]
                {
                    "RSD", startDateStr,
                    "RED", endDateStr,
                    "RAC", cbAC.Text,
                    "RACNM", lbACName.Text,
                    "CompanyName", MyData.Params.CompNameX,
                    "RPER", MyData.Params.RPER,
                    "RPERNM", lbClName.Text,
                    "PAC", null,
                    "PCLID", null,
                    "RCURRENCY", ""
                });


            switch (lbCm.SelectedIndex)
            {
                case 0:
                    rd.FileName = "Report_Koresp_1";
                    break;
                case 1:
                    rd.FileName = "Report_Koresp_1_a";
                    break;
                case 2:
                    rd.FileName = "Report_Koresp_2";
                    break;
                case 3:
                    rd.FileName = "Report_Koresp_1_full";
                    break;
                case 6:
                    rd.FileName = "Report_Koresp_3";
                    break;
                case 4:
                    rdsub = rd;
                    rdsub.FileName = "Report_Koresp_1";
                    rdsub.AddSubreportLink("DataSet1", "PAC", "Ac1");
                    rdsub.AddSubreportLink("DataSet_2a", "PAC", "AC");
                    rdsub.AddSubreportLink("DataSet_3a", "PAC", "Ac1");

                    rd = new ReportViewerData();
                    rd.Sources["DataSet_2a"] = rows_rops2a;
                    rd.FileName = "Report_Koresp_1_byac";
                    rd.SubReports[rdsub.FileName] = rdsub;
                    rd.AddReportParameters(
                        new string[]
                        {
                            "RSD", startDateStr,
                            "RED", endDateStr,
                            "RAC", cbAC.Text,
                            "RACNM", lbACName.Text,
                            "CompanyName", MyData.Params.CompNameX,
                            "RPER", MyData.Params.RPER,
                            "RPERNM", lbClName.Text,
                            "RCURRENCY", ""
                        });
                    break;
                case 5:
                    rdsub = rd;
                    rdsub.FileName = "Report_Koresp_1";
                    rdsub.AddSubreportLink("DataSet1", "PAC", "Ac1", "PCLID", "Clid");
                    rdsub.AddSubreportLink("DataSet_2a", "PAC", "AC", "PCLID", "Clid");
                    rdsub.AddSubreportLink("DataSet_3a", "PAC", "Ac1", "PCLID", "Clid");

                    rd = new ReportViewerData();
                    rd.Sources["DataSet_2a"] = rows_rops2a;
                    rd.FileName = "Report_Koresp_1_byacnadcl";
                    rd.SubReports[rdsub.FileName] = rdsub;
                    rd.AddReportParameters(
                        new string[]
                        {
                            "RSD", startDateStr,
                            "RED", endDateStr,
                            "RAC", cbAC.Text,
                            "RACNM", lbACName.Text,
                            "CompanyName", MyData.Params.CompNameX,
                            "RPER", "",
                            "RPERNM", "",
                            "RCURRENCY", ""
                        });
                    break;
            }

            MyMainForm.ShowReport(rd);
        }

        private void tsbPrevMonth_Click(object sender, EventArgs e)
        {
            if (Check() != "OK") return;
            var dt2 = startDate.FirstDayOfMonth().AddDays(-1);
            var dt1 = dt2.FirstDayOfMonth();
            tbSD.Text = Utils.DateToString(dt1);
            tbED.Text = Utils.DateToString(dt2);
        }

        private void tsbNextMonth_Click(object sender, EventArgs e)
        {
            if (Check() != "OK") return;
            var dt1 = startDate.LastDayOfMonth().AddDays(1);
            var dt2 = dt1.LastDayOfMonth();
            tbSD.Text = Utils.DateToString(dt1);
            tbED.Text = Utils.DateToString(dt2);
        }

    }
}
