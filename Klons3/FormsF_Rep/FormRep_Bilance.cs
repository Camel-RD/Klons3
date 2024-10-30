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
using KlonsLIB.Data;
using KlonsLIB.Forms;
using KlonsLIB.Misc;

namespace KlonsF.Forms
{
    public partial class FormRep_Bilance : MyFormBaseF
    {
        public FormRep_Bilance()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }
        
        private DateTime startDate = DateTime.MinValue;
        private DateTime endDate = DateTime.MinValue;
        private string balid = "";
        private string rtitle = "";
        private string raktivs = "";
        private string rpasivs = "";


        private void Form_Bilance_Load(object sender, EventArgs e)
        {
            DataLoaderF.ClearBalanceFormulas();
            DataLoaderF.BalA_Fill();

            lbCM.SelectedIndex = 0;
            LoadParams();
            SetControlsUpDownOrder(new Control[][]
            {
                new Control[] {tbSD, tbED},
                new Control[] {cmReport, cmReport}
            });
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            dgvBalA1.DefaultCellStyle.Font = Font;
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

            if(bsBalA1.Count == 0)
                return "Sarakstā nav atskaišu.";

            if(dgvBalA1.CurrentRow == null)
                return "Jāizvēlas atskaite.";

            balid = dgvBalA1.CurrentRow.Cells[dgcBalA1balid.Index].Value.AsString();
            rtitle = dgvBalA1.CurrentRow.Cells[dgcBalA1Descr.Index].Value.AsString().Nz();
            raktivs = dgvBalA1.CurrentRow.Cells[dgcBalA1TA.Index].Value.AsString().Nz();
            rpasivs = dgvBalA1.CurrentRow.Cells[dgcBalA1TP.Index].Value.AsString().Nz();

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
            
            int selectedreport = lbCM.SelectedIndex;
            if (selectedreport == -1) return;

            SaveParams();

            if (selectedreport == 4)
            {
                DoIt3();
                return;
            }
            if (selectedreport == 0 || selectedreport == 1)
            {
                DoIt1(selectedreport);
                return;
            }
            if (selectedreport == 2 || selectedreport == 3)
            {
                DoIt2(selectedreport);
                return;
            }
        }

        private void DoItT()
        {
            string rt = Check();
            if (rt != "OK")
            {
                MyMainForm.ShowWarning(rt);
                return;
            }

            int selectedreport = lbCM.SelectedIndex;
            if (selectedreport == -1) return;

            SaveParams();

            if (selectedreport == 0 || selectedreport == 1)
            {
                DoIt1T(selectedreport);
                return;
            }
            if (selectedreport == 2 || selectedreport == 3)
            {
                DoIt2T(selectedreport);
                return;
            }
        }

        private void DoIt1(int selectedreport)
        {
            List<Klons3.ModelsF.F_BALA2> rows_bala2;
            List<Klons3.ModelsF.F_BALA21> rows_bala21;

            try
            {
                rows_bala2 = DataLoaderF.BALA2_Get();
                rows_bala21 = DataLoaderF.BALA21_GetBy_bal_12(startDate, endDate, balid);
            }
            catch (Exception)
            {
                MyMainForm.ShowWarning("Neizdevās sagatavot atskaiti.");
                return;
            }
            MyData.ReportHelperF.FillBalA2FromBalA21(balid, rows_bala2, rows_bala21);


            ReportViewerData rd = new ReportViewerData();

            switch (selectedreport)
            {
                case 0:
                    rd.FileName = "Report_Bilance_2";
                    break;
                case 1:
                    rd.FileName = "Report_Bilance_1";
                    break;
            }

            var dv = rows_bala2
                .Where(x => x.BALID == balid)
                .OrderBy(x => x.DC)
                .ThenBy(x => x.NR)
                .ToList();

            rd.Sources["DataSet1"] = dv;
            rd.AddReportParameters(
                new string[]
                {
                    "RSD", MyData.Params.RSD,
                    "RED", MyData.Params.RED,
                    "CompanyName", MyData.Params.CompNameX,
                    "RTITLE", rtitle,
                    "RAKTIVS", raktivs,
                    "RPASIVS", rpasivs
                });
            MyMainForm.ShowReport(rd);
        }

        private void DoIt1T(int selectedreport)
        {
            List<Klons3.ModelsF.F_BALA2> rows_bala2;
            List<Klons3.ModelsF.F_BALA21> rows_bala21;

            try
            {
                rows_bala2 = DataLoaderF.BALA2_Get();
                rows_bala21 = DataLoaderF.BALA21_GetBy_bal_12(startDate, endDate, balid);
            }
            catch (Exception)
            {
                MyMainForm.ShowWarning("Neizdevās sagatavot atskaiti.");
                return;
            }
            MyData.ReportHelperF.FillBalA2FromBalA21(balid, rows_bala2, rows_bala21);


            var reprows = rows_bala2
                .Where(x => x.BALID == balid)
                .OrderBy(x => x.DC)
                .ThenBy(x => x.NR)
                .Select(x => RepRowBilance1.MakeFrom(x, 0))
                .ToList();
            var reprows2 = new List<RepRowBilance1>();
            bool bak = false, bpa = false;
            foreach (var row in reprows)
            {
                if (row.Dc == "AK" && !bak)
                {
                    var reprowak = new RepRowBilance1()
                    {
                        Kind = 1,
                        Descr = raktivs
                    };
                    reprows2.Add(reprowak);
                    bak = true;
                }
                if (row.Dc == "PA" && !bpa)
                {
                    var reprowpa = new RepRowBilance1()
                    {
                        Kind = 1,
                        Descr = rpasivs
                    };
                    reprows2.Add(reprowpa);
                    bpa = true;
                }
                if (!(selectedreport == 0 && row.S1 == 0M && row.S1 == 0M && row.S3 == 0M))
                {
                    reprows2.Add(row);
                }
            }

            var frm = MyMainForm.ShowForm(typeof(FormRep_Bilance1)) as FormRep_Bilance1;
            frm.Text = rtitle;
            frm.SetRowSource(reprows2, MyData.Params.RSD, MyData.Params.RED);

        }

        private void DoIt2(int selectedreport)
        {
            List<Klons3.ModelsF.F_BALA2> rows_bala2;
            List<Klons3.ModelsF.F_BALA22> rows_bala22;

            try
            {
                rows_bala2 = DataLoaderF.BALA2_Get();
                rows_bala22 = DataLoaderF.BALA22_GetBy_bal_22(startDate, endDate, balid);
            }
            catch (Exception)
            {
                MyMainForm.ShowWarning("Neizdevās sagatavot atskaiti.");
                return;
            }

            List<RepRow_BalMT> reprows = null;

            ReportViewerData rd = new ReportViewerData();

            switch (selectedreport)
            {
                case 2:
                    reprows = PrepareReportMt_apgr(balid, rows_bala2, rows_bala22);
                    rd.FileName = "Report_Bilance_31";
                    break;
                case 3:
                    reprows = PrepareReportMt_atl(balid, rows_bala2, rows_bala22);
                    rd.FileName = "Report_Bilance_41";
                    break;
            }
            reprows = reprows.Where(x => !x.AllZeros).ToList();

            rd.Sources["DataSet1"] = reprows;
            rd.AddReportParameters(
                new string[]
                {
                    "RSD", MyData.Params.RSD,
                    "RED", MyData.Params.RED,
                    "CompanyName", MyData.Params.CompNameX,
                    "RTITLE", rtitle,
                    "RAKTIVS", raktivs,
                    "RPASIVS", rpasivs
                });
            MyMainForm.ShowReport(rd);
        }

        private void DoIt2T(int selectedreport)
        {
            List<Klons3.ModelsF.F_BALA2> rows_bala2;
            List<Klons3.ModelsF.F_BALA22> rows_bala22;

            try
            {
                rows_bala2 = DataLoaderF.BALA2_Get();
                rows_bala22 = DataLoaderF.BALA22_GetBy_bal_22(startDate, endDate, balid);
            }
            catch (Exception)
            {
                MyMainForm.ShowWarning("Neizdevās sagatavot atskaiti.");
                return;
            }

            List<RepRow_BalMT> reprows = null;

            switch (selectedreport)
            {
                case 2:
                    reprows = PrepareReportMt_apgr(balid, rows_bala2, rows_bala22);
                    break;
                case 3:
                    reprows = PrepareReportMt_atl(balid, rows_bala2, rows_bala22);
                    break;
            }
            reprows = reprows.Where(x => !x.AllZeros).ToList();

            int k = reprows.FindIndex(x => x.DC == "AK");
            if (k > -1)
            {
                var reprowak = new RepRow_BalMT()
                {
                    TP = "X",
                    DESCR = raktivs
                };
                reprows.Insert(k, reprowak);
            }
            k = reprows.FindIndex(x => x.DC == "PA");
            if (k > -1)
            {
                var reprowak = new RepRow_BalMT()
                {
                    TP = "X",
                    DESCR = rpasivs
                };
                reprows.Insert(k, reprowak);
            }

            var frm = MyMainForm.ShowForm(typeof(FormRep_Bilance2)) as FormRep_Bilance2;
            frm.Text = rtitle;
            frm.SetRowSource(reprows);
        }

        private bool WCompare(string s, string pattern)
        {
            if (s == pattern) return true;
            if (pattern == "*") return true;
            if (string.IsNullOrEmpty(s) && string.IsNullOrEmpty(pattern)) return true;
            if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(pattern)) return false;
            if (pattern.EndsWith("*"))
            {
                pattern = pattern.Substring(0, pattern.Length - 1);
                return s.StartsWith(pattern);
            }
            return false;
        }

        private void DoIt3()
        {
            var rows_bala2 = DataLoaderF.BALA2_Get();
            var rows_bala3 = DataLoaderF.BALA3_Get();
            var rows_rops2a1 = DataLoaderF.ROPS2A1_GetBy_apgr_11(startDate, endDate, "%");

            MyData.ReportHelperF.PrepareRops2a1(rows_rops2a1);

            var b1 = MyData.DbContextF.F_BALA1.Find(balid);
            if (b1 == null) return;
            var bs2 = b1.BALA2Rows.ToArray();
            bs2 = bs2.Where(d => d.TP == "S").ToArray();
            foreach (var drb2 in bs2)
            {
                string snr = drb2.NR;
                var bs3 = drb2.BALA3Rows.ToArray();
                foreach(var drb3 in bs3)
                {
                    string ac = drb3.AC;
                    string s;

                    var drs = rows_rops2a1.Where(
                        d => WCompare(d.AC, drb3.AC)
                    ).ToArray();

                    foreach(var dr in drs)
                    {
                        if (drb3.TP == "Db")
                        {
                            s = dr.DSTR;
                            if (string.IsNullOrEmpty(s))
                                dr.DSTR = snr;
                            else
                                dr.DSTR = s + ", " + snr;
                        }
                        else
                        {
                            s = dr.CSTR;
                            if (string.IsNullOrEmpty(s))
                                dr.CSTR = snr;
                            else
                                dr.CSTR = s + ", " + snr;
                        }
                    }
                }
            }

            ReportViewerData rd = new ReportViewerData();
            rd.FileName = "Report_Apgr_BilFormula";
            rd.Sources["DataSet1"] = rows_rops2a1;
            rd.AddReportParameters(
                new string[]
                {
                    "RSD", MyData.Params.RSD,
                    "RED", MyData.Params.RED,
                    "CompanyName", MyData.Params.CompNameX,
                    "PForReport", b1.DESCR
                });
            MyMainForm.ShowReport(rd);
        }

        public List<RepRow_BalMT> PrepareReportMt_apgr(string balid,
            List<Klons3.ModelsF.F_BALA2> rows_bala2,
            List<Klons3.ModelsF.F_BALA22> rows_bala22)
        {
            var ret = new List<RepRow_BalMT>();
            foreach(var dr in rows_bala22)
            {
                var drb = rows_bala2.Find(x => x.ID == dr.BID);
                if (drb == null) continue;
                var rr = new RepRow_BalMT()
                {
                    ID = dr.ID,
                    BID = dr.BID,
                    DC = drb.DC,
                    TP = drb.TP,
                    NR = drb.NR,
                    DESCR = drb.DESCR,
                    M0 = dr.M0,
                    M1 = dr.M1,
                    M2 = dr.M2,
                    M3 = dr.M3,
                    M4 = dr.M4,
                    M5 = dr.M5,
                    M6 = dr.M6,
                    M7 = dr.M7,
                    M8 = dr.M8,
                    M9 = dr.M9,
                    M10 = dr.M10,
                    M11 = dr.M11,
                    M12 = dr.M12
                };
                rr.M13 = rr.M1 + rr.M2 + rr.M3 + rr.M4 + rr.M5 + rr.M6 + rr.M7 + rr.M8 + rr.M9 + rr.M10 + rr.M11 + rr.M12;
                if (rr.TP == "V")
                {
                    rr.M0 = 0.0M;
                    rr.M1 = 0.0M;
                    rr.M2 = 0.0M;
                    rr.M3 = 0.0M;
                    rr.M4 = 0.0M;
                    rr.M5 = 0.0M;
                    rr.M6 = 0.0M;
                    rr.M7 = 0.0M;
                    rr.M8 = 0.0M;
                    rr.M9 = 0.0M;
                    rr.M10 = 0.0M;
                    rr.M11 = 0.0M;
                    rr.M12 = 0.0M;
                    rr.M13 = 0.0M;
                }
                ret.Add(rr);
            }
            return ret;
        }

        public List<RepRow_BalMT> PrepareReportMt_atl(string balid,
            List<Klons3.ModelsF.F_BALA2> rows_bala2,
            List<Klons3.ModelsF.F_BALA22> rows_bala22)
        {
            var ret = new List<RepRow_BalMT>();
            foreach (var dr in rows_bala22)
            {
                var drb = rows_bala2.Find(x => x.ID == dr.BID);
                if (drb == null) continue;
                var rr = new RepRow_BalMT()
                {
                    ID = dr.ID,
                    BID = dr.BID,
                    DC = drb.DC,
                    TP = drb.TP,
                    NR = drb.NR,
                    DESCR = drb.DESCR,
                    M0 = dr.M0,
                    M1 = dr.M1,
                    M2 = dr.M2,
                    M3 = dr.M3,
                    M4 = dr.M4,
                    M5 = dr.M5,
                    M6 = dr.M6,
                    M7 = dr.M7,
                    M8 = dr.M8,
                    M9 = dr.M9,
                    M10 = dr.M10,
                    M11 = dr.M11,
                    M12 = dr.M12
                };
                rr.M1 += rr.M0;
                rr.M2 += rr.M1;
                rr.M3 += rr.M2;
                rr.M4 += rr.M3;
                rr.M5 += rr.M4;
                rr.M6 += rr.M5;
                rr.M7 += rr.M6;
                rr.M8 += rr.M7;
                rr.M9 += rr.M8;
                rr.M10 += rr.M9;
                rr.M11 += rr.M10;
                rr.M12 += rr.M11;

                if (rr.TP == "V")
                {
                    rr.M0 = 0.0M;
                    rr.M1 = 0.0M;
                    rr.M2 = 0.0M;
                    rr.M3 = 0.0M;
                    rr.M4 = 0.0M;
                    rr.M5 = 0.0M;
                    rr.M6 = 0.0M;
                    rr.M7 = 0.0M;
                    rr.M8 = 0.0M;
                    rr.M9 = 0.0M;
                    rr.M10 = 0.0M;
                    rr.M11 = 0.0M;
                    rr.M12 = 0.0M;
                    rr.M13 = 0.0M;
                }

                ret.Add(rr);
            }
            return ret;
        }

        private void cmReport_Click(object sender, EventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                DoIt();
            });
        }

        private void cmTable_Click(object sender, EventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                DoItT();
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

        private void cmEdit_Click(object sender, EventArgs e)
        {
            MyMainForm.ShowForm(typeof (Form_BilancesFormula));
        }

    }

    public class RepRow_BalMT
    {
        public int ID { get; set; } = 0;
        public int BID { get; set; } = 0;
        public string NR { get; set; } = null;
        public string DC { get; set; } = null;
        public string TP { get; set; } = null;
        public string DESCR { get; set; } = null;
        public decimal M0 { get; set; } = 0.0M;
        public decimal M1 { get; set; } = 0.0M;
        public decimal M2 { get; set; } = 0.0M;
        public decimal M3 { get; set; } = 0.0M;
        public decimal M4 { get; set; } = 0.0M;
        public decimal M5 { get; set; } = 0.0M;
        public decimal M6 { get; set; } = 0.0M;
        public decimal M7 { get; set; } = 0.0M;
        public decimal M8 { get; set; } = 0.0M;
        public decimal M9 { get; set; } = 0.0M;
        public decimal M10 { get; set; } = 0.0M;
        public decimal M11 { get; set; } = 0.0M;
        public decimal M12 { get; set; } = 0.0M;
        public decimal M13 { get; set; } = 0.0M;

        public bool AllZeros => M1 == 0M && M2 == 0M && M3 == 0M && M4 == 0M && 
            M5 == 0M && M6 == 0M && M7 == 0M && M8 == 0M && M9 == 0M && M10 == 0M && 
            M11 == 0M && M12 == 0M && M13 == 0M;
    }

}
