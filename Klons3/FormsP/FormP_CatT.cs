using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KlonsP.Classes;
using KlonsF.Classes;
using KlonsLIB.Forms;
using Klons3.ModelsP;
using KlonsLIB.Data;

namespace KlonsP.Forms
{
    public partial class FormP_CatT : MyFormBaseF
    {
        public FormP_CatT()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        private void Form_CatT_Load(object sender, EventArgs e)
        {

        }

        private void SelectCurrent()
        {
            if (dgvCatT.CurrentRow == null || dgvCatT.CurrentRow.IsNewRow) return;
            var dr = bsCatT.GetCurrentItem<P_CATT>();
            if (!dgvCatT.EndEdit()) return;
            if (!SaveData()) return;
            if (bsCatT.IsItemDetached(dr)) return;
            if (!this.IsMyDialog) return;
            SetSelectedObject(dr);
        }


        public override bool SaveData()
        {
            if (!dgvCatT.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = bsCatT.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsCatT.HasChanges());
        }

        private void SetSaveButton(bool red)
        {
            bnCatT.SetSaveButtonRed(red);
        }

        private void bnCatT_SaveClicked(object sender, EventArgs e)
        {
            SaveData();
        }

        private void dgvCatT_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void bsCatT_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void dgvCatT_MyKeyDown(object sender, KeyEventArgs e)
        {
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
                if (!dgvCatT.EndEdit()) return;
                dgvCatT.MoveToNewRow();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Escape)
            {
                SetSelectedObject(null, true);
                e.Handled = true;
            }
        }

        private void dgvCatT_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SetSelectedObject(null, true);
            }
        }

        public void DeleteCurrent()
        {
            bnCatT.DeleteCurrent();
            SaveData();
        }

        private bool IsRow0(int rownr)
        {
            if (rownr == dgvCatT.NewRowIndex) return false;
            var dr = dgvCatT.GetObjectViewItem<P_CAT1>(rownr);
            if (dr == null) return false;
            return dr.ID == 0;
        }

        private void dgvCatT_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            e.Cancel = e.Row.IsNewRow || IsRow0(bsCatT.Position) || !AskCanDelete();
        }

        private void bnCatT_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = IsRow0(bsCatT.Position) || !AskCanDelete();
        }

        private void dgvCatT_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCatT.CurrentRow == null || dgvCatT.CurrentRow.IsNewRow) return;
            if (e.ColumnIndex == dgcCode.Index)
            {
                SelectCurrent();
            }
        }

        private void dgvCatT_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex == dgvCatT.NewRowIndex) return;
            if (IsRow0(e.RowIndex))
                e.Cancel = true;
        }
    }
}
