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
    public partial class FormM_StoreTypes : MyFormBaseF
    {
        public FormM_StoreTypes()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        private void FormM_StoreTypes_Load(object sender, EventArgs e)
        {
            if (SselectedStoreType != null)
                FindStoreType(SselectedStoreType);
        }

        public M_STORETYPE SselectedStoreType = null;

        public static M_STORETYPE GetStoreType(M_STORETYPE dr_storetype)
        {
            var fm = new FormM_StoreTypes();
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return null;
            return (M_STORETYPE)fm.SelectedObject;
        }

        public void FindStoreType(M_STORETYPE dr_storetype)
        {
            bsRows.SelectItem(dr_storetype);
        }

        void SelectCurrent()
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            var dr = bsRows.GetCurrentItem<M_STORETYPE>();
            if (!dgvRows.EndEdit()) return;
            if (!SaveData()) return;
            if (bsRows.IsItemDetached(dr)) return;
            SselectedStoreType = dr;
            if (!this.IsMyDialog) return;
            SetSelectedObject(dr);
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
                SselectedStoreType = null;
                SetSelectedObject(null, true);
                e.Handled = true;
            }
        }

        void dgvRows_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SselectedStoreType = null;
                SetSelectedObject(null, true);
            }
        }

        void dgvRows_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            if (e.ColumnIndex == dgcName.Index)
            {
                SelectCurrent();
                return;
            }
        }

    }
}
