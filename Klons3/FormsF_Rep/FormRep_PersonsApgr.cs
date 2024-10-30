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

namespace KlonsF.FormsReportParams
{
    public partial class FormRep_PersonsApgr : MyFormBaseF
    {
        public FormRep_PersonsApgr()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            dgvRows.AutoGenerateColumns = false;
            dgvRows.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            BoldCellFont = new Font(dgvRows.Font, FontStyle.Bold);
        }

        Font BoldCellFont;

        public string Title
        {
            get => lbTitle.Text;
            set => lbTitle.Text = value;
        }

        public void SetRowSource(List<RepRowPersonsApgr> reprows)
        {
            dgvRows.DataSource = reprows;
            dgvRows.AutoResizeRows();
        }

        private void dgvRows_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == -1) return;
            var reprow = dgvRows.Rows[e.RowIndex].DataBoundItem as RepRowPersonsApgr;
            if (reprow.Kind == 1)
            {
                e.CellStyle.Font = BoldCellFont;
            }
            if (reprow.Kind == 2)
            {
                e.CellStyle.Font = BoldCellFont;
            }
        }
    }


    public class RepRowPersonsApgr
    {
        public int Kind = 0;
        public string Ac { get; set; }
        public string AcName { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Deb0 { get; set; } = 0.0M;
        public decimal Cred0 { get; set; } = 0.0M;
        public decimal DebCh { get; set; } = 0.0M;
        public decimal CredCh { get; set; } = 0.0M;
        public decimal Deb1 { get; set; } = 0.0M;
        public decimal Cred1 { get; set; } = 0.0M;

        public static RepRowPersonsApgr MakeFrom(F_ROPS2A dr, int kind)
        {
            var ret = new RepRowPersonsApgr()
            {
                Ac = dr.AC,
                AcName = dr.NAME,
                Code = dr.CLID,
                Name = dr.NAME1,
                Deb0 = dr.ADB,
                Cred0 = dr.ACR,
                DebCh = dr.TDB.Value,
                CredCh = dr.TCR.Value,
                Deb1 = dr.BDB,
                Cred1 = dr.BCR
            };
            return ret;
        }
    }

}
