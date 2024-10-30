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
    public partial class FormRep_PVNKops : MyFormBaseF
    {
        public FormRep_PVNKops()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        private DateTime startDate = DateTime.MinValue;
        private DateTime endDate = DateTime.MinValue;

        private void FormRepApgr1_Load(object sender, EventArgs e)
        {
            LoadParams();
            SetControlsUpDownOrder(new Control[][]
            {
                new Control[] {tbSD, tbED},
                new Control[] {cmDoIt, cmDoIt}
            });
        }


        private void LoadParams()
        {
            tbSD.Text = MyData.Params.RSD;
            tbED.Text = MyData.Params.RED;
        }

        public override void SaveParams()
        {
            MyData.Params.RSD = tbSD.Text;
            MyData.Params.RED = tbED.Text;
        }

        private string Check()
        {
            if (tbSD.Text == "" || tbED.Text == "")
                return "Jāievada datums.";

            if (!Utils.StringToDate(tbSD.Text, out startDate) ||
                !Utils.StringToDate(tbED.Text, out endDate))
                return "Nekorekts datums.";

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

            var rows_rops2a = DataLoaderF.ROPS2A_GetBy_pvn_10(startDate, endDate);

            SaveParams();

            ReportViewerData rd = new ReportViewerData();
            rd.FileName = "Report_PVN_kops";
            rd.Sources["DataSet1"] = rows_rops2a;
            rd.AddReportParameters(
                new string[]
                {
                    "RSD", MyData.Params.RSD,
                    "RED", MyData.Params.RED,
                    "CompanyName", MyData.Params.CompName
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
