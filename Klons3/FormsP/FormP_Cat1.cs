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
    public partial class FormP_Cat1 : MyFormBaseF
    {
        public FormP_Cat1()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        private void Form_Cat1_Load(object sender, EventArgs e)
        {

        }

        private void SelectCurrent()
        {
            if (dgvCat1.CurrentRow == null || dgvCat1.CurrentRow.IsNewRow) return;
            var dr = bsCat1.GetCurrentItem<P_CAT1>();
            if (!dgvCat1.EndEdit()) return;
            if (!SaveData()) return;
            if (bsCat1.IsItemDetached(dr)) return;
            if (!this.IsMyDialog) return;
            SetSelectedObject(dr);
        }


        public override bool SaveData()
        {
            if (!dgvCat1.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = bsCat1.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsCat1.HasChanges());
        }

        private void SetSaveButton(bool red)
        {
            bnCat1.SetSaveButtonRed(red);
        }

        private void bnCat1_SaveClicked(object sender, EventArgs e)
        {
            SaveData();
        }

        private void dgvCat1_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void bsCat1_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void dgvCat1_MyKeyDown(object sender, KeyEventArgs e)
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
                if (!dgvCat1.EndEdit()) return;
                dgvCat1.MoveToNewRow();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Escape)
            {
                SetSelectedObject(null, true);
                e.Handled = true;
            }
        }

        private void dgvCat1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SetSelectedObject(null, true);
            }
        }

        public void DeleteCurrent()
        {
            bnCat1.DeleteCurrent();
            SaveData();
        }

        private bool IsRow0(int rownr)
        {
            if (rownr == dgvCat1.NewRowIndex) return false;
            var dr = dgvCat1.GetObjectViewItem<P_CAT1>(rownr);
            if (dr == null) return false;
            return dr.ID == 0;
        }

        private void dgvCat1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            e.Cancel = e.Row.IsNewRow || IsRow0(e.Row.Index) || !AskCanDelete();
        }

        private void bnCat1_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = IsRow0(bsCat1.Position) || !AskCanDelete();
        }

        private void dgvCat1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCat1.CurrentRow == null || dgvCat1.CurrentRow.IsNewRow) return;
            if (e.ColumnIndex == dgcCode.Index)
            {
                SelectCurrent();
            }
        }

        private void dgvCat1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex == dgvCat1.NewRowIndex) return;
            if (IsRow0(e.RowIndex))
                e.Cancel = true;
        }
    }
}
