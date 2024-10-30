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
using KlonsF.Classes;
using Klons3.ModelsM;

namespace KlonsM.FormsM
{
    public partial class FormM_DocTypes : MyFormBaseF
    {
        public FormM_DocTypes()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        private void FormM_DocTypes_Load(object sender, EventArgs e)
        {

        }

        public static int? GetDocTypeId(int? id)
        {
            var fm = new FormM_DocTypes();
            if (id.HasValue)
                fm.FindDocType(id.Value);
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return null;
            return fm.SelectedValueInt;
        }

        public void FindDocType(int id)
        {
            for (int i = 0; i < bsRows.Count; i++)
            {
                var dr = bsRows.GetItem<M_DOCTYPES>(i);
                if (dr.ID == id)
                {
                    bsRows.Position = i;
                    return;
                }
            }
        }

        void SelectCurrent()
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            var dr = bsRows.GetCurrentItem<M_DOCTYPES>();
            if (!dgvRows.EndEdit()) return;
            if (!SaveData()) return;
            if (bsRows.IsItemDetached(dr)) return;
            if (!this.IsMyDialog) return;
            SetSelectedValueInt(dr.ID);
        }

        void dgvRows_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvRows.CurrentCell == null) return;
            if (e.Control && e.KeyCode == Keys.Return)
            {
                SelectCurrent();
                e.Handled = true;
            }

            if (e.KeyCode == Keys.Escape)
            {
                SetSelectedObject(null, true);
                e.Handled = true;
            }
        }

        void dgvRows_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SetSelectedObject(null, true);
            }
        }

        void dgvRows_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            if (e.ColumnIndex == dgcCode.Index ||
                e.ColumnIndex == dgcName.Index)
            {
                SelectCurrent();
                return;
            }
        }
    }
}
