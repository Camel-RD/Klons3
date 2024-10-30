using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KlonsA.Classes;
using KlonsF.Classes;
using KlonsLIB.Data;
using Klons3.ModelsA;

namespace KlonsA.Forms
{
    public partial class FormA_Banks : MyFormBaseF
    {
        public FormA_Banks()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        private void Form_Banks_Load(object sender, EventArgs e)
        {

        }

        private void SelectCurrent()
        {
            if (dgvBanks.CurrentRow == null || dgvBanks.CurrentRow.IsNewRow) return;
            var dr = bsBanks.GetCurrentItem<A_BANKS>();
            if (!dgvBanks.EndEdit()) return;
            if (!SaveData()) return;
            if (bsBanks.IsItemDetached(dr)) return;
            int id = dr.ID;
            if (!this.IsMyDialog) return;
            SetSelectedValueInt(id);
        }

        private void dgvBanks_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvBanks.CurrentRow == null || dgvBanks.CurrentRow.IsNewRow) return;
            if (e.ColumnIndex == dgcSID.Index)
            {
                SelectCurrent();
            }
        }

        public void DeleteCurrent()
        {
            bnavBanks.DeleteCurrent();
            SaveData();
        }

        private void dgvBanks_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Return)
            {
                SelectCurrent();
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                DeleteCurrent();
                e.Handled = true;
            }
        }

        private void dgvBanks_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SetSelectedValueStr(null);
                e.Handled = true;
            }
        }

        private void bnavBanks_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void dgvBanks_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete())
            {
                e.Cancel = true;
            }
        }

        private void SetSaveButton(bool red)
        {
            bnavBanks.SetSaveButton(tsbSave, red);
        }

        public override bool SaveData()
        {
            if (!dgvBanks.EndEditX()) return false;
            var ret = bsBanks.SaveDbContext();
            return ret.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsBanks.HasChanges());
        }

        private void dgvBanks_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            SaveData();
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
            CheckSave();
        }

        private void bsBanks_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }


    }
}
