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
    public partial class FormRep_Rekins1 : MyFormBaseF
    {
        public FormRep_Rekins1()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }
        
        public FormRep_Rekins1(int docid, int repid)
        {
            InitializeComponent();
            CheckMyFontAndColors();
            this.docid = docid;
            this.repid = repid;
        }
        
        public FormRep_Rekins1(int docid)
        {
            InitializeComponent();
            CheckMyFontAndColors();
            this.docid = docid;
            this.repid = 0;
        }

        private int docid = -1;
        private int repid = -1;
        private string clid = null;
        private F_TREPOPSD TREPOPSDRow = null;

        private void FormRep_Rekins1_Load(object sender, EventArgs e)
        {
            LoadParams();
            SetControlsUpDownOrder([[tbDescr], [tbSigner], [cmDoIt]]);
            if (docid == -1) return;
            TREPOPSDRow = DataTasks.MakeTRepOPSd(docid);
            if (TREPOPSDRow == null) return;
            tbDescr.Text = TREPOPSDRow.DESCR.Nz();
        }

        private void LoadParams()
        {
            tbSigner.Text = MyData.Params.RekinaIzr;
        }

        public override void SaveParams()
        {
            MyData.Params.RekinaIzr = tbSigner.Text;
        }

        private string Check()
        {
            if (tbDescr.Text == "")
                return "Jāievada apraksts.";
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

            var rtagdigital = chDigitalDoc.Checked ? "Rēķins sagatavots un apstiprināts elektroniski" : null;

            ReportViewerData rd = new ReportViewerData();
            rd.FileName = "Report_Rekins_1";
            rd.Sources["DataSet1"] = new List<F_TREPOPSD>() { TREPOPSDRow };
            rd.Sources["DataSet2"] = new List<F_PERSONS>() { dr_person };
            rd.AddReportParameters(
                new string[]
                {
                    "RCOMPNAME", MyData.Params.CompName,
                    "RREGNR", MyData.Params.CompRegNr,
                    "RPVNREGNR", MyData.Params.CompRegNrPVN,
                    "RADRESE", MyData.Params.CompAddr + ", " + MyData.Params.CompAddrInd,
                    "RBANKASKODS", MyData.Params.BankId,
                    "RBANKA", MyData.Params.BankName,
                    "RKONTS", MyData.Params.BankAcc,
                    "RDESCR", tbDescr.Text,
                    "RSIGNER", tbSigner.Text,
                    "RTAGDIGITAL", rtagdigital,
                    "RPAYUNTIL", tbPayUntil.Text
                });
            MyMainForm.ShowReport(rd);

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


    }
}
