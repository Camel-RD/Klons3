using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Klons3.ModelsA;
using KlonsA.Classes;
using KlonsF.Classes;
using KlonsLIB.Data;

namespace KlonsA.Forms
{
    public partial class FormA_IncomeCodes : MyFormBaseF
    {
        public FormA_IncomeCodes()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            cbActive.SelectedIndex = 0;
        }

        private void Form_IncomeCodes_Load(object sender, EventArgs e)
        {
            InsertInToolStrip(bnList, cbActive, -1);

            CheckSave();
        }

        private void SelectCurrent()
        {
            if (dgvList.CurrentRow == null || dgvList.CurrentRow.IsNewRow) return;
            var dr = bsList.GetCurrentItem<A_INCOME_CODES>();
            if (!dgvList.EndEdit()) return;
            if (!SaveData()) return;
            if (bsList.IsItemDetached(dr)) return;
            string id = dr.ID;
            if (!this.IsMyDialog) return;
            SetSelectedValueStr(id);
        }

        private void dgvList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgcID.Index)
            {
                SelectCurrent();
            }
        }

        private void dgvList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Return)
            {
                SelectCurrent();
                e.Handled = true;
            }
        }

        private void dgvList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SetSelectedValueStr(null);
            }
        }

        private void bnList_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void dgvList_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete())
                e.Cancel = true;
        }

        private void SetSaveButton(bool red)
        {
            bnList.SetSaveButton(tsbSave, red);
        }

        public override bool SaveData()
        {
            if (!dgvList.EndEditX()) return false;
            var ret = bsList.SaveDbContext();
            CheckSave();
            return ret.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsList.HasChanges());
        }

        private void dgvList_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            SaveData();
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void bsList_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        public void CheckFilter()
        {
            if(cbActive.SelectedIndex == 0)
            {
                bsList.SetFilter<A_INCOME_CODES>(x => x.USED == 1);
            }
            else
            {
                bsList.RemoveFilter();
            }
        }

        private void cbActive_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckFilter();
        }
    }
}
