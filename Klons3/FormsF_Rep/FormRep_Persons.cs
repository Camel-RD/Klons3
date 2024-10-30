﻿using System;
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
    public partial class FormRep_Persons : MyFormBaseF
    {
        public FormRep_Persons()
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
            cbAC.Text = MyData.Params.RACX;
            cbClid.Text = MyData.Params.RPER;
            CheckAcName();
            CheckClName();
        }

        public override void SaveParams()
        {
            MyData.Params.RSD = tbSD.Text;
            MyData.Params.RED= tbED.Text;
            MyData.Params.RACX = cbAC.Text;
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

            ac = cbAC.Text.Replace('*','%');
            clid = cbClid.Text;

            if (ac == "") ac = "%";

            if (clid == "" || clid == "*")
                clid = null;

            return "OK";
        }

        private void cmDoIt_Click(object sender, EventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                DoIt();
            });
        }

        private void cmShowTable_Click(object sender, EventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                PrepareTableReport();
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

            string s = $"Konts: [{cbAC.Text}]";
            if (!string.IsNullOrEmpty(lbACName.Text))
                s = $"{s} {lbACName.Text}";
            if (!string.IsNullOrEmpty(cbClid.Text))
                s = $"{s}\nPersona: [{cbClid.Text}]";
            if (!string.IsNullOrEmpty(lbClName.Text))
                s = $"{s} {lbClName.Text}";

            string rtitle = "";

            List<F_ROPS2A> rows_rops2a = null;
            List<F_TREPA1> rows_trepa1 = null;

            switch (selectedreport)
            {
                case 0:
                    rows_rops2a = DataLoaderF.ROPS2A_GetBy_pers_12(startDate, endDate, ac, clid);
                    MyData.ReportHelperF.PrepareRops2a(rows_rops2a);
                    rd.Sources["DataSet1"] = rows_rops2a;
                    rd.FileName = "Report_Pers_1";
                    break;
                case 1:
                    rows_rops2a = DataLoaderF.ROPS2A_GetBy_pers_11(startDate, endDate, ac, clid);
                    MyData.ReportHelperF.PrepareRops2a(rows_rops2a);
                    rd.Sources["DataSet1"] = rows_rops2a;
                    rd.FileName = "Report_Pers_2";
                    break;
                case 2:
                    rows_trepa1 = DataLoaderF.TREPA1_GetBy_pers_14(startDate, endDate, ac, clid);
                    rd.Sources["DataSet1"] = rows_trepa1;
                    rd.FileName = "Report_Pers_4";
                    rtitle = "Rēķinu saraksts";
                    break;
                case 3:
                    rows_trepa1 = DataLoaderF.TREPA1_GetBy_pers_15(startDate, endDate, ac, clid);
                    rd.Sources["DataSet1"] = rows_trepa1;
                    rd.FileName = "Report_Pers_4";
                    rtitle = "Rēķinu saraksts";
                    break;
                case 4:
                    rows_trepa1 = DataLoaderF.TREPA1_GetBy_pers_13(startDate, endDate, ac, clid);
                    rd.Sources["DataSet1"] = rows_trepa1;
                    rd.FileName = "Report_Pers_4";
                    rtitle = "Neapmaksātie rēķini";
                    break;
                case 5:
                    rows_trepa1 = DataLoaderF.TREPA1_GetBy_pers_16(startDate, endDate, ac, clid);
                    rd.Sources["DataSet1"] = rows_trepa1;
                    rd.FileName = "Report_Pers_4";
                    rtitle = "Neapmaksātie rēķini";
                    break;
            }


            switch (selectedreport)
            {
                case 0:
                case 1:
                    rd.AddReportParameters(
                        new string[]
                        {
                            "RSD", startDateStr,
                            "RED", endDateStr,
                            "RACX", cbAC.Text,
                            "RACNM", s,
                            "CompanyName", MyData.Params.CompNameX
                        });
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                    rd.AddReportParameters(
                        new string[]
                        {
                            "RSD", startDateStr,
                            "RED", endDateStr,
                            "RTITLE", rtitle,
                            "RACNM", s,
                            "CompanyName", MyData.Params.CompNameX
                        });
                    break;
            }
            MyMainForm.ShowReport(rd);
        }

        public void PrepareTableReport()
        {
            string rt = Check();
            if (rt != "OK")
            {
                MyMainForm.ShowWarning(rt);
                return;
            }
            SaveParams();

            int selectedreport = lbCm.SelectedIndex;

            string s = $"Periods: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}\n Konts: [{cbAC.Text}]";
            if (!string.IsNullOrEmpty(lbACName.Text))
                s = $"{s} {lbACName.Text}";
            if (!string.IsNullOrEmpty(cbClid.Text))
                s = $"{s}\n Persona: [{cbClid.Text}]";
            if (!string.IsNullOrEmpty(lbClName.Text))
                s = $"{s} {lbClName.Text}";

            string rtitle = "";

            List<F_ROPS2A> rows_rops2a = null;
            List<F_TREPA1> rows_trepa1 = null;

            List<RepRowPersonsApgr> reprows = null;
            List<RepRowPersonsDocs> reprowsdocs = null;

            switch (selectedreport)
            {
                case 0:
                    rows_rops2a = DataLoaderF.ROPS2A_GetBy_pers_12(startDate, endDate, ac, clid);
                    MyData.ReportHelperF.PrepareRops2a(rows_rops2a);
                    reprows = PrepareRepApgr1(rows_rops2a);
                    break;
                case 1:
                    rows_rops2a = DataLoaderF.ROPS2A_GetBy_pers_11(startDate, endDate, ac, clid);
                    MyData.ReportHelperF.PrepareRops2a(rows_rops2a);
                    reprows = PrepareRepApgr2(rows_rops2a);
                    break;
                case 2:
                    rows_trepa1 = DataLoaderF.TREPA1_GetBy_pers_14(startDate, endDate, ac, clid);
                    rtitle = "Rēķinu saraksts";
                    reprowsdocs = PrepareRepDocs1(rows_trepa1);
                    break;
                case 3:
                    rows_trepa1 = DataLoaderF.TREPA1_GetBy_pers_15(startDate, endDate, ac, clid);
                    rtitle = "Rēķinu saraksts";
                    reprowsdocs = PrepareRepDocs1(rows_trepa1);
                    break;
                case 4:
                    rows_trepa1 = DataLoaderF.TREPA1_GetBy_pers_13(startDate, endDate, ac, clid);
                    rtitle = "Neapmaksātie rēķini";
                    reprowsdocs = PrepareRepDocs1(rows_trepa1);
                    break;
                case 5:
                    rows_trepa1 = DataLoaderF.TREPA1_GetBy_pers_16(startDate, endDate, ac, clid);
                    rtitle = "Neapmaksātie rēķini";
                    reprowsdocs = PrepareRepDocs1(rows_trepa1);
                    break;
            }

            switch (selectedreport)
            {
                case 0:
                case 1:
                    var frm = MyMainForm.ShowForm(typeof(FormRep_PersonsApgr)) as FormRep_PersonsApgr;
                    frm.Title = s;
                    frm.SetRowSource(reprows);
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                    var frm2 = MyMainForm.ShowForm(typeof(FormRep_PersonsDocs)) as FormRep_PersonsDocs;
                    frm2.Text = rtitle;
                    frm2.Title = s;
                    frm2.SetRowSource(reprowsdocs, selectedreport >= 4);
                    break;
            }

        }
            
        public List<RepRowPersonsApgr> PrepareRepApgr1(List<F_ROPS2A> rows_rops2a)
        {
            var reprows = rows_rops2a.Select(x => RepRowPersonsApgr.MakeFrom(x, 0)).ToList();
            var reptotalrow = new RepRowPersonsApgr()
            {
                Kind = 2,
                Code = "KOPĀ"
            };
            foreach(var row in reprows)
            {
                reptotalrow.Deb0 += row.Deb0;
                reptotalrow.Cred0 += row.Cred0;
                reptotalrow.DebCh += row.DebCh;
                reptotalrow.CredCh += row.CredCh;
                reptotalrow.Deb1 += row.Deb1;
                reptotalrow.Cred1 += row.Cred1;
            }
            reprows.Add(reptotalrow);
            return reprows;
        }

        public List<RepRowPersonsApgr> PrepareRepApgr2(List<F_ROPS2A> rows_rops2a)
        {
            var reprows = rows_rops2a.Select(x => RepRowPersonsApgr.MakeFrom(x, 0)).ToList();
            var grrows = reprows.GroupBy(x => x.Ac);
            var reprows2 = new List<RepRowPersonsApgr>();
            var repttotalrow = new RepRowPersonsApgr()
            {
                Kind = 2,
                Code = "KOPĀ"
            };
            foreach (var gr in grrows)
            {
                var row1 = gr.First();
                var reptitlerow = new RepRowPersonsApgr()
                {
                    Kind = 1,
                    Code = row1.Ac,
                    Name = row1.AcName
                };
                reprows2.Add(reptitlerow);
                foreach (var row in gr)
                {
                    reprows2.Add(row);
                    reptitlerow.Deb0 += row.Deb0;
                    reptitlerow.Cred0 += row.Cred0;
                    reptitlerow.DebCh += row.DebCh;
                    reptitlerow.CredCh += row.CredCh;
                    reptitlerow.Deb1 += row.Deb1;
                    reptitlerow.Cred1 += row.Cred1;

                    repttotalrow.Deb0 += row.Deb0;
                    repttotalrow.Cred0 += row.Cred0;
                    repttotalrow.DebCh += row.DebCh;
                    repttotalrow.CredCh += row.CredCh;
                    repttotalrow.Deb1 += row.Deb1;
                    repttotalrow.Cred1 += row.Cred1;
                }
            }
            reprows2.Add(repttotalrow);
            return reprows2;
        }

        public List<RepRowPersonsDocs> PrepareRepDocs1(List<F_TREPA1> rows_trepa1)
        {
            var reprows = rows_trepa1.Select(x => RepRowPersonsDocs.MakeFrom(x, 0)).ToList();
            var grrows = reprows.GroupBy(x => x.Code);
            var reprows2 = new List<RepRowPersonsDocs>();
            var repemptylrow = new RepRowPersonsDocs()
            {
                Kind = 3
            };
            var reptotalrow = new RepRowPersonsDocs()
            {
                Kind = 2,
                Code = "KOPĀ"
            };
            bool firstrow = true;
            foreach (var gr in grrows)
            {
                var row1 = gr.First();
                var repcodetotalrow = new RepRowPersonsDocs()
                {
                    Kind = 2,
                    Name = "Kopā"
                };
                if (firstrow)
                {
                    firstrow = false;
                }
                else
                {
                    reprows2.Add(repemptylrow);
                }
                bool firstingroup = true;
                int ct = 0;
                foreach (var row in gr)
                {
                    if (firstingroup)
                    {
                        row.IsFirstInGroup = true;
                        firstingroup = false;
                    }
                    reprows2.Add(row);
                    repcodetotalrow.Deb += row.Deb;
                    repcodetotalrow.Cred += row.Cred;

                    reptotalrow.Deb += row.Deb;
                    reptotalrow.Cred += row.Cred;
                    ct++;
                }
                if (ct > 0)
                    reprows2.Add(repcodetotalrow);
            }
            reprows2.Add(reptotalrow);
            return reprows2;
        }
    }
}
