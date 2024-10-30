﻿using System;
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
using Klons3.ModelsFRep;

namespace KlonsF.FormsReportParams
{
    public partial class FormRep_ApgrNP : MyFormBaseF
    {
        public FormRep_ApgrNP()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        private DateTime startDate = DateTime.MinValue;
        private DateTime endDate = DateTime.MinValue;
        private string ac = "";

        private void FormRepApgr1_Load(object sender, EventArgs e)
        {
            lbACName.Text = "";
            LoadParams();
            SetControlsUpDownOrder(new Control[][]
            {
                new Control[] {tbSD, tbED},
                new Control[] {cbAC, cbAC},
                new Control[] {cbCharCount, cbCharCount},
                new Control[] {cmDoIt, cmDoIt}
            });
            cbCharCount.Text = "*";
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

        private void cbAC_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private void cbAC_TextChanged(object sender, EventArgs e)
        {
            CheckAcName();
        }

        private void LoadParams()
        {
            tbSD.Text = MyData.Params.RSD;
            tbED.Text = MyData.Params.RED;
            cbAC.Text = MyData.Params.RACX;
            CheckAcName();
        }

        public override void SaveParams()
        {
            MyData.Params.RSD = tbSD.Text;
            MyData.Params.RED = tbED.Text;
            MyData.Params.RACX = cbAC.Text;
            MyData.Params.RACNM = lbACName.Text;
        }

        private string Check()
        {
            if (tbSD.Text == "" || tbED.Text == "")
                return "Jāievada datums.";

            if (!Utils.StringToDate(tbSD.Text, out startDate) ||
                !Utils.StringToDate(tbED.Text, out endDate))
                return "Nekorekts datums.";

            ac = cbAC.Text;
            if (ac == "") ac = "%";
            ac = ac.Replace('*', '%');

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

            List<F_ROPS2A> rows_rops2a = null;

            if (cbCharCount.Text == "*")
            {
                rows_rops2a = DataLoaderF.ROPS2A_GetBy_apgr_np_11(startDate, endDate, ac);
            }
            else
            {
                int char_count = int.Parse(cbCharCount.Text);
                rows_rops2a = DataLoaderF.ROPS2A_GetBy_apgr_np_12(startDate, endDate, ac, char_count);
            }

            SaveParams();

            MyData.ReportHelperF.PrepareRops2a(rows_rops2a);

            ReportViewerData rd = new ReportViewerData();
            rd.FileName = "Report_Apgr_NP";
            rd.Sources["DataSet1"] = rows_rops2a;
            rd.AddReportParameters(
                new string[]
                {
                    "RSD", MyData.Params.RSD,
                    "RED", MyData.Params.RED,
                    "RACX", MyData.Params.RACX,
                    "RACNM", MyData.Params.RACNM,
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

        private void cbAC_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DoOnF5(sender as ComboBox);
        }

    }
}
