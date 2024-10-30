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
    public partial class FormRep_Currency : MyFormBaseF
    {
        public FormRep_Currency()
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
        private string currency = "";
        private decimal currrate = 1.0M;
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
                new Control[] {tbCurrency, tbCurrency},
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
            tbCurrency.Text = MyData.Params.RCURR;
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
            MyData.Params.RCURR = tbCurrency.Text;
        }

        private string Check()
        {
            if (tbSD.Text == "" || tbED.Text == "")
                return "Jāievada datums.";

            if (!Utils.StringToDate(tbSD.Text, out startDate) ||
                !Utils.StringToDate(tbED.Text, out endDate) ||
                startDate > endDate)
                return "Nekorekts datums.";

            if (tbCurrency.Text == "" || tbCurrency.Text.ToUpper() == "EUR" ||
                tbCurrency.Text.Length > 3)
                return "Nekorekts valūtas kods.";


            startDateStr = Utils.DateToString(startDate);
            endDateStr = Utils.DateToString(endDate);

            currency = tbCurrency.Text;
            
            int selectedreport = lbCm.SelectedIndex;
            if (selectedreport == 0)
            {
                var dr = MyData.DbContextF.F_CURRENCY.Find(currency, endDate);
                if(dr == null)
                {
                    return "Norādītajam beigu datumam un valūtai nav ievadīts konvertēšanas kurss.";
                }
                currrate = dr.RATE;
            }

            ac = cbAC.Text;
            if (ac == "") ac = "%";
            ac = ac.Replace('*', '%');

            clid = cbClid.Text;
            if (clid == "") clid = null;

            return "OK";
        }

        private void cmDoIt_Click(object sender, EventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                DoIt();

            });
        }


        private void DoCurrDiff()
        {
            var rows_rops2a = DataLoaderF.ROPS2A_GetBy_currdiff_01(startDate, endDate, ac, null, currency);

            if (rows_rops2a.Count == 0)
            {
                MyMainForm.ShowWarning("Ar dotajiem parametriem dati netika atrasti");
                return;
            }

            var dr = rows_rops2a[0];
            decimal d1, d2, d3, d4;
            d1 = dr.ADB - dr.ACR;
            d2 = dr.BDB - dr.BCR;
            string s = ac.Substring(0, 1);
            if (s != "1" && s != "2")
            {
                d1 = -d1;
                d2 = -d2;
            }
            d3 = Math.Round(d1*currrate, 2);
            d4 = d3 - d2;
            dr.ADB = d1;
            dr.ACR = d3;
            dr.TDB = d2;
            dr.TCR = d4;

            ReportViewerData rd = new ReportViewerData();
            rd.FileName = "Report_CurrDiff";
            rd.Sources["DataSet1"] = rows_rops2a;

            s = string.Format(
                "Valūta: {0}\nKonts: {1} {2}\nPersona: {3} {4}",
                currency, cbAC.Text, lbACName.Text,
                cbClid.Text, lbClName.Text);

            rd.AddReportParameters(
                new string[]
                {
                    "RSD", startDateStr,
                    "RED", endDateStr,
                    "RACX", cbAC.Text,
                    "RACNM", s,
                    "CompanyName", MyData.Params.CompNameX,
                    "RCURRENCY", currency
                });

            MyMainForm.ShowReport(rd);
        }

        private void DoApgr1()
        {
            var rows_rops2a = DataLoaderF.ROPS2A_GetBy_apgr_cur_01_clid(startDate, endDate, ac, null, currency);
            MyData.ReportHelperF.PrepareRops2a(rows_rops2a);

            ReportViewerData rd = new ReportViewerData();
            rd.FileName = "Report_Apgr_Bil";
            rd.Sources["DataSet1"] = rows_rops2a;

            rd.AddReportParameters(
                new string[]
                {
                    "RSD", startDateStr,
                    "RED", endDateStr,
                    "RACX", cbAC.Text,
                    "RACNM", lbACName.Text,
                    "CompanyName", MyData.Params.CompNameX,
                    "RCURRENCY", currency
                });

            MyMainForm.ShowReport(rd);
        }

        private void DoApgrPers1()
        {
            var rows_rops2a = DataLoaderF.ROPS2A_GetBy_apgr_cur_01_clid(startDate, endDate, ac, clid, currency);
            MyData.ReportHelperF.PrepareRops2a(rows_rops2a);

            ReportViewerData rd = new ReportViewerData();
            rd.FileName = "Report_Pers_1";
            rd.Sources["DataSet1"] = rows_rops2a;

            string s = $"{currency} Konts: [{cbAC.Text}]";
            if (!string.IsNullOrEmpty(lbACName.Text))
                s = $"{s} {lbACName.Text}";
            if (!string.IsNullOrEmpty(cbClid.Text))
                s = $"{s}\nPersona: [{cbClid.Text}]";
            if (!string.IsNullOrEmpty(lbClName.Text))
                s = $"{s} {lbClName.Text}";

            rd.AddReportParameters(
                new string[]
                {
                    "RSD", startDateStr,
                    "RED", endDateStr,
                    "RACX", cbAC.Text,
                    "RACNM", s,
                    "CompanyName", MyData.Params.CompNameX
                });

            MyMainForm.ShowReport(rd);
        }

        private void DoKoresp1()
        {
            var rows_rops1a = DataLoaderF.ROPS1A_GetBy_koresp_11_cur(startDate, endDate, ac, clid, currency);
            var rows_rops2a = DataLoaderF.ROPS2A_GetBy_apgr_cur_01_clid(startDate, endDate, ac, clid, currency);
            var rows_rops3a = DataLoaderF.ROPS3A_GetBy_koresp_01_cur(startDate, endDate, ac, clid, currency);

            MyData.ReportHelperF.PrepareRops1a(rows_rops1a);
            MyData.ReportHelperF.PrepareRops2a(rows_rops2a);
            MyData.ReportHelperF.PrepareRops2aRAC(rows_rops2a, cbAC.Text, lbACName.Text, cbClid.Text, lbClName.Text);

            ReportViewerData rd = new ReportViewerData();

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
                    "RCURRENCY", currency
                });

            rd.FileName = "Report_Koresp_1";

            MyMainForm.ShowReport(rd);
        }

        private void DoCurrCheck()
        {
            var rows_rops1a = DataLoaderF.ROPS1A_GetBy_curcheck_01(startDate, endDate, ac, null, currency);

            ReportViewerData rd = new ReportViewerData();
            rd.FileName = "Report_DokSar_1";
            rd.Sources["DataSet1"] = rows_rops1a;

            string paramstr = 
                $"Dokumenti ar valūtas konvertācijas kļūdām \nDatums:[{startDateStr}-{endDateStr}], konts: [{cbAC.Text}]";
                
            
            rd.AddReportParameters(
                new string[]
                {
                    "RSD", startDateStr,
                    "RED", endDateStr,
                    "RACX", "",
                    "RACNM", paramstr,
                    "CompanyName", MyData.Params.CompNameX
                });

            MyMainForm.ShowReport(rd);
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

            int selectedreport = lbCm.SelectedIndex;
            switch (selectedreport)
            {
                case 0:
                    DoCurrDiff();
                    break;
                case 1:
                    DoApgr1();
                    break;
                case 2:
                    DoApgrPers1();
                    break;
                case 3:
                    DoKoresp1();
                    break;
                case 4:
                    DoCurrCheck();
                    break;
            }
        }

    }
}
