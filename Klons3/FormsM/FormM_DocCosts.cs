using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KlonsM.Classes;
using KlonsLIB.Forms;
using KlonsLIB.Data;
using KlonsLIB.Misc;
using KlonsF.Classes;
using KlonsLIB.Components;
using Klons3.ModelsM;

namespace KlonsM.FormsM
{
    public partial class FormM_DocCosts : MyFormBaseF
    {
        public FormM_DocCosts()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            dgvAcc.AutoGenerateColumns = false;
        }

        public static void ShowRep(M_DOCS dr_doc)
        {
            var fm = new FormM_DocCosts();
            fm.GetData(dr_doc);
            fm.ShowDialog(fm.MyMainForm);
        }

        private void FormM_DocCosts_Load(object sender, EventArgs e)
        {

        }

        public List<DocAccRepRow> DocAccRepRows = new List<DocAccRepRow>();


        public void GetData(M_DOCS dr_doc)
        {
            DocAccRepRows = new List<DocAccRepRow>();
            dgvAcc.DataSource = DocAccRepRows;
            if (dr_doc.XDocType != EDocType.Realizācija &&
                dr_doc.XDocType != EDocType.Atgriezts_no_pircēja &&
                dr_doc.XDocType != EDocType.Kredītrēķins_pircējam)
            {
                return;
            }
            var drs_rows = dr_doc.Rows
                .Where(x=>!x.Item.XIsServices)
                .ToList();
            if (drs_rows.Count == 0) return;
            var acc21 = dr_doc.ACCOUT;
            foreach(var dr_row in drs_rows)
            {
                var rep_row = new DocAccRepRow()
                {
                    DebFin = dr_row.ACC7,
                    CredFin = acc21,
                    Amount = dr_row.TBUYPRICE
                };
                DocAccRepRows.Add(rep_row);
            }
            dgvAcc.Refresh();
        }
    }
}
