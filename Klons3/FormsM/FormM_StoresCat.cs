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
    public partial class FormM_StoresCat : MyFormBaseF
    {
        public FormM_StoresCat()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        private void FormM_Banks_Load(object sender, EventArgs e)
        {

        }

        public M_STORES_CAT SelectedStoreCat = null;

        public static M_STORES_CAT GetStoresCat(M_STORES_CAT dr_cat)
        {
            var fm = new FormM_StoresCat();
            fm.SelectedStoreCat = dr_cat;
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return null;
            return (M_STORES_CAT)fm.SelectedObject;
        }

        public void FindStoreCat(M_STORES_CAT dr_cat)
        {
            bsRows.SelectItem(dr_cat);
        }

        private void SelectCurrent()
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            var dr = bsRows.GetCurrentItem<M_STORES_CAT>();
            if (!dgvRows.EndEdit()) return;
            if (!SaveData()) return;
            if (bsRows.IsItemDetached(dr)) return;
            if (!this.IsMyDialog) return;
            SelectedStoreCat = dr;
            SetSelectedObject(dr);
        }

        public override bool SaveData()
        {
            if (!dgvRows.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = bsRows.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsRows.HasChanges());
        }

        private void SetSaveButton(bool red)
        {
            bNav.SetSaveButtonRed(red);
        }

        private bool IsRow0(int rownr)
        {
            if (rownr == dgvRows.NewRowIndex) return false;
            var dr = dgvRows.GetObjectViewItem<M_STORES_CAT>(rownr);
            if (dr == null) return false;
            return dr.ID == 1;
        }

        public void DeleteCurrent()
        {
            bNav.DeleteCurrent();
            SaveData();
        }

        private void dgvRows_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvRows.CurrentCell == null) return;
            if (e.Control && e.KeyCode == Keys.Return)
            {
                SelectCurrent();
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Delete && e.Control)
            {
                DeleteCurrent();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                if (!dgvRows.EndEdit()) return;
                dgvRows.MoveToNewRow();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Escape)
            {
                SelectedStoreCat = null;
                SetSelectedObject(null, true);
                e.Handled = true;
            }
        }

        private void dgvRows_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SelectedStoreCat = null;
                SetSelectedObject(null, true);
            }
        }

        private void dgvRows_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            if (e.ColumnIndex == dgcCode.Index)
            {
                SelectCurrent();
                return;
            }
        }

        private void bindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void dgvRows_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void bsRows_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void dgvRows_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            e.Cancel = e.Row.IsNewRow || IsRow0(e.Row.Index) || !AskCanDelete();
        }

        private void bNav_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = IsRow0(bsRows.Position) || !AskCanDelete();
        }

        private void dgvRows_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex == dgvRows.NewRowIndex) return;
            if (IsRow0(e.RowIndex))
                e.Cancel = true;
        }
    }
}
