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
using KlonsLIB.Forms;
using KlonsLIB.Misc;
using Klons3.ModelsFRep;
using Klons3.ModelsF;

namespace KlonsF.FormsReportParams
{
    public partial class FormRep_Rekins2 : MyFormBaseF
    {
        public FormRep_Rekins2()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }
        public FormRep_Rekins2(int docid, int repid = 0)
        {
            InitializeComponent();
            CheckMyFontAndColors();
            this.docid = docid;
            this.repid = repid;
        }

        private int docid = -1;
        private int repid = -1;
        private string clid = null;
        private F_TREPOPSD TREPOPSDRow = null;
        private List<F_TREPOPS> TREPOPSRows = null;

        private void FormRep_Rekins2_Load(object sender, EventArgs e)
        {
            LoadParams();
            SetControlsUpDownOrder([[tbSigner], [cmDoIt]]);
            if (docid == -1) return;
            TREPOPSDRow = DataTasks.MakeTRepOPSd(docid);
            if (TREPOPSDRow == null) return;
        }

        private void LoadParams()
        {
            tbSigner.Text = MyData.Params.RekinaIzr;
        }

        public override void SaveParams()
        {
            MyData.Params.RekinaIzr = tbSigner.Text;
        }
        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            OnNaviKey(sender, e);
        }

        private void cmDoIt_Click(object sender, EventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                DoIt();

            });
        }

        private void cmCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private string Check()
        {
            clid = TREPOPSDRow?.CLID;
            if (string.IsNullOrEmpty(clid))
                return "Dokumentam nav norādīta persona.";
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

            SaveParams();

            var dr_person = MyData.DbContextF.F_PERSONS.Find(clid);
            TREPOPSRows = DataTasks.MakeTRepOPS(docid);

            bool haspvn = false;
            decimal pvn = 0.00M;
            decimal total = 0.00M;
            var PVNRows = new List<F_TREPOPS>();

            foreach (var dr in TREPOPSRows)
            {
                total += dr.SUMM.Value;
                if (DataTasks.IsPVN(dr.AC15.Nz()) || DataTasks.IsPVN(dr.AC25.Nz()))
                {
                    haspvn = true;
                    pvn += dr.SUMM.Value;
                    PVNRows.Add(dr);
                    break;
                }
            }

            foreach (var dr in PVNRows)
            {
                TREPOPSRows.Remove(dr);
            }

            ReportViewerData rd = new ReportViewerData();

            switch (repid)
            {
                case 1:
                    rd.FileName = "Report_Rekins_2";
                    break;
                case 2:
                    rd.FileName = "Report_Rekins_3";
                    break;
                default:
                    return;
            }

            var rtagdigital = chDigitalDoc.Checked ? "Rēķins sagatavots un apstiprināts elektroniski" : null;

            rd.Sources["DataSet1"] = new List<F_TREPOPSD>() { TREPOPSDRow };
            rd.Sources["DataSet2"] = new List<F_PERSONS>() { dr_person };
            rd.Sources["DataSet3"] = TREPOPSRows;
            rd.AddReportParameters(
                new string[]
                {
                    "RCOMPNAME", MyData.Params.CompName,
                    "RREGNR", MyData.Params.CompRegNr,
                    "RPVNREGNR", MyData.Params.CompRegNrPVN,
                    "RADRESE", MyData.Params.CompAddr,
                    "RBANKASKODS", MyData.Params.BankId,
                    "RBANKA", MyData.Params.BankName,
                    "RKONTS", MyData.Params.BankAcc,
                    "RDESCR", "",
                    "RPVN", haspvn? pvn.ToString("F2") : "-",
                    "RAPMAKSAI", total.ToString("F2"),
                    "RSIGNER", tbSigner.Text,
                    "RTAGDIGITAL", rtagdigital,
                    "RPAYUNTIL", tbPayUntil.Text
                });
            MyMainForm.ShowReport(rd);

        }

    }
}
