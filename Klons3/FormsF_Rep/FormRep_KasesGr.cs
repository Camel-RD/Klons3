﻿using System;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.WinForms;

namespace KlonsF.FormsReportParams
{
    public partial class FormRep_KasesGr : MyFormBaseF
    {
        public FormRep_KasesGr()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        private DateTime startDate = DateTime.MinValue;
        private DateTime endDate = DateTime.MinValue;
        private string ac = "";
        private string startDateStr = "";
        private string endDateStr = "";
        private int nr = 0;
        private int nr2 = 0;

        private void FormRepKoresp1_Load(object sender, EventArgs e)
        {
            lbACName.Text = "";
            lbCm.SelectedIndex = 0;
            LoadParams();
            SetControlsUpDownOrder(new Control[][]
            {
                new Control[] {tbSD, tbED},
                new Control[] {cbAC, cbAC},
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

        private void cbAC_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private void cbAC_TextChanged(object sender, EventArgs e)
        {
            CheckAcName();
        }
        private void cbAC_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DoOnF5(cbAC);
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
            CheckAcName();
        }

        public override void SaveParams()
        {
            MyData.Params.AVNorSD = startDateStr;
            MyData.Params.AVNorED = endDateStr;
            MyData.Params.AVRAC = cbAC.Text;
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

            if (ac == "" || lbACName.Text == "")
                return "Nekorekts konts.";

            if (tbNr.Text == "" || !int.TryParse(tbNr.Text, out nr) || nr < 1)
                return "Nekorekts pirmās lapas numurs.";
            
            if (tbNr2.Text == "" || !int.TryParse(tbNr2.Text, out nr2) || nr2 < 1)
                return "Nekorekts pirmā ordera numurs.";

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

            int selectedreport = lbCm.SelectedIndex;
            if (selectedreport == -1) return;

            List<F_ROPS1A> rows_rops1a = null;

            switch (selectedreport)
            {
                case 0:
                    rows_rops1a = DataLoaderF.ROPS1A_GetBy_kg_11(startDate, endDate, ac);
                    rd.FileName = "Report_KasesGr_1";
                    break;
                case 1:
                    rows_rops1a = DataLoaderF.ROPS1A_GetBy_kg_11(startDate, endDate, ac);
                    rd.FileName = "Report_KasesGr_2";
                    break;
                case 2:
                    rows_rops1a = DataLoaderF.ROPS1A_GetBy_kieo_21(startDate, endDate, ac);
                    rd.FileName = "Report_KIEO_1";
                    break;
                case 3:
                    rows_rops1a = DataLoaderF.ROPS1A_GetBy_kizo_21(startDate, endDate, ac);
                    rd.FileName = "Report_KIZO_1";
                    break;
                default:
                    return;
            }
            
            MyData.ReportHelperF.PrepareRops1aForKO(rows_rops1a);

            rd.Sources["DataSet1"] = rows_rops1a;


            decimal bal0;
            switch (selectedreport)
            {
                case 0:
                    bal0 = MyData.ReportHelperF.GetBal0ForDate(ac, startDate);
                    rd.AddReportParameters(
                        new string[]
                        {
                            "CompanyName", MyData.Params.CompNameX,
                            "RPAGENR", (nr-1).ToString(),
                            "RBAL0", bal0.ToString()
                        });
                    break;
                case 1:
                    bal0 = MyData.ReportHelperF.GetBal0ForDate(ac, startDate);
                    string speriod = Utils.DateToString(startDate) + 
                        " - " + Utils.DateToString(endDate);
                    rd.AddReportParameters(
                        new string[]
                        {
                            "CompanyName", MyData.Params.CompNameX,
                            "RPAGENR", (nr-1).ToString(),
                            "RBAL0", bal0.ToString(),
                            "RPERIOD", speriod
                        });
                    break;
                case 2:
                case 3:
                    rd.AddReportParameters(
                        new string[]
                        {
                            "RCOMPNAME", MyData.Params.CompName,
                            "RREGNR", MyData.Params.CompRegNr,
                            "RPVNREGNR", MyData.Params.CompPhone
                        });
                    break;
            }
            MyMainForm.ShowReport(rd);
        }

        private void cmNrKIEO_Click(object sender, EventArgs e)
        {
            string rt = Check();
            if (rt != "OK")
            {
                MyMainForm.ShowWarning(rt);
                return;
            }
            SaveParams();

            MyData.DbContextFRep.Database.ExecuteSql($"execute procedure SP_F_SET_KIEONR {startDate}, {endDate}, {ac}, {nr2}");

            MyMainForm.ShowInfo("Darīts!");
        }

        private void cmNrKIZO_Click(object sender, EventArgs e)
        {
            string rt = Check();
            if (rt != "OK")
            {
                MyMainForm.ShowWarning(rt);
                return;
            }
            SaveParams();

            MyData.DbContextFRep.Database.ExecuteSql($"execute procedure SP_F_SET_KIZONR {startDate}, {endDate}, {ac}, {nr2}");

            MyMainForm.ShowInfo("Darīts!");
        }


    }
}
