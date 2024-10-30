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
    public partial class FormRep_AvNor : MyFormBaseF
    {
        public FormRep_AvNor()
        {
            InitializeComponent();
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
                new Control[] {tbNr, tbNr},
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
            tbSD.Text = MyData.Params.AVNorSD;
            tbED.Text = MyData.Params.AVNorED;
            cbAC.Text = MyData.Params.AVRAC;
            cbClid.Text = MyData.Params.NorPers;
            tbNr.Text = MyData.Params.AVNorNR;
            CheckAcName();
            CheckClName();
        }

        public override void SaveParams()
        {
            MyData.Params.AVNorSD = startDateStr;
            MyData.Params.AVNorED = endDateStr;
            MyData.Params.AVRAC = cbAC.Text;
            MyData.Params.NorPers = cbClid.Text;
            MyData.Params.AVNorNR = tbNr.Text;
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
            clid = cbClid.Text;

            if (ac == "" || lbACName.Text == "")
                return "Nekorekts konts.";

            if (clid == "" || lbClName.Text == "")
                return "Nekorekta persona.";

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

            ReportViewerData rd = new ReportViewerData();

            var rows_top1a = DataLoaderF.ROPS1A_GetBy_koresp_11_clid2(startDate, endDate, ac, clid);
            var rows_top2a = DataLoaderF.ROPS2A_GetBy_apgr_01_clid2(startDate, endDate, ac, clid);
            var rows_top3a = DataLoaderF.ROPS3A_GetBy_koresp_01_clid2(startDate, endDate, ac, clid);

            MyData.ReportHelperF.PrepareRops1a(rows_top1a);
            MyData.ReportHelperF.PrepareRops2a(rows_top2a);
            MyData.ReportHelperF.PrepareRops2aRAC(rows_top2a);

            rd.Sources["DataSet1"] = rows_top1a;
            rd.Sources["DataSet_2a"] = rows_top2a;
            rd.Sources["DataSet_3a"] = rows_top3a;
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
                    "AVNR", tbNr.Text,
                    "PAC", null,
                    "PCLID", null
                });

            if (lbCm.SelectedIndex == 0)
                rd.FileName = "Report_AvNor_1";
            else
                rd.FileName = "Report_AvNor_2";

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
