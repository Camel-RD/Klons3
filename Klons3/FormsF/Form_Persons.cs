using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Equin.ApplicationFramework;
using Klons3.ModelsF;
using KlonsF.Classes;
using KlonsLIB.Components;
using KlonsLIB.Data;
using KlonsLIB.Misc;
using ModelsF = Klons3.ModelsF;

namespace KlonsF.Forms
{
    public partial class Form_Persons : MyFormBaseF
    {
        public Form_Persons()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            bsBanks.Fill();
            bsPersons.Fill();
            cbAct.SelectedIndex = 0;
            CheckFilter();
            dgvPersons.CellValidating += DgvPersons_CellValidating;
        }

        public static string GetClId(string clid)
        {
            var fm = new Form_Persons();
            //fm.tbCode.Text = code;
            fm.SelectedValueStr = clid;
            fm.FindPerson(clid);
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return null;
            return fm.SelectedValueStr;
        }

        private void DgvPersons_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var col = dgvPersons.Columns[e.ColumnIndex];
            if (dgvPersons.DataSource is not MyBindingSourceEf bs) return;
            
        }

        private void FormPersons_Load(object sender, EventArgs e)
        {
            CheckSave();
            if (!SelectedValueStr.IsNOE())
                FindPerson(SelectedValueStr);
        }

        private void FormPersons_Shown(object sender, EventArgs e)
        {
        }

        public void FindPerson(string clid)
        {
            if (bsPersons.Count == 0) return;
            if (clid.IsNOE()) return;
            for (int i = 0; i < bsPersons.Count; i++)
            {
                var dr = bsPersons.GetItem<F_PERSONS>(i);
                if (dr.CLID == clid)
                {
                    bsPersons.Position = i;
                    return;
                }
            }
        }
        private void CheckFilter()
        {
            string s = tbSearch.Text.Nz().ToLower();
            Predicate<ModelsF.F_PERSONS> p1 = null, p2 = null;
            int k = cbAct.SelectedIndex;
            if (s == "" && k == 0)
            {
                bsPersons.RemoveFilter();
            }
            else
            {
                if (k == 1)
                    p1 = x => x.ACT == 1;
                if (s != "")
                    p2 = x => x.NAME.ContainsCI(s) || x.REGNR.ContainsCI(s);
                bsPersons.SetFilter(p1, p2);
            }
        }

        private void tbSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Return)
            {
                CheckFilter();
            }
            if (e.KeyChar == (char)Keys.Escape)
            {
                tbSearch.Text = null;
                CheckFilter();
                e.Handled = true;
            }
        }

        private void tbClid_Enter(object sender, EventArgs e)
        {
            tbClid.SelectAll();
        }

        private void tbSearch_Enter(object sender, EventArgs e)
        {
            tbSearch.SelectAll();
        }

        private void dgvPersons_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Return)
            {
                SelectCurrent();
                e.Handled = true;
            }
        }

        private void tbClid_RowSelectedEvent(object sender, RowSelectedEventArgs e)
        {
            SetSelectedValueStr(e.Value);
        }

        private void SelectCurrent()
        {
            if (dgvPersons.CurrentRow == null || dgvPersons.CurrentRow.IsNewRow) return;
            string clid = dgvPersons.CurrentRow.Cells[0].Value.AsString();
            if (!dgvPersons.EndEdit()) return;
            if (!SaveData()) return;
            if (!this.IsMyDialog) return;
            SetSelectedValueStr(clid);
        }

        private void dgvPersons_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectCurrent();
        }

        private void dgvPersons_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SetSelectedValueStr(null);
            }
        }

        private void bnavPersons_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void dgvPersons_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void dgvPersons_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                dgvPersons.MoveToNewRow();
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                bnavPersons.DeleteCurrent();
                e.Handled = true;
            }
        }

        private void cbAct_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckFilter();
        }

        private void dgvPersons_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == dgvPersons.NewRowIndex) return;

            var dr = dgvPersons.GetObjectViewItem<ModelsF.F_PERSONS>(e.RowIndex);
            if (dr == null) return;

            if (e.ColumnIndex == dgcBankId.Index)
            {
                string s = dgvPersons.CurrentRow.Cells[dgcBankId.Index].Value.AsString();
                dr.BANK = DataTasks.GetBankName(s);
                dgvPersons.InvalidateRow(e.RowIndex);
            }
            else if (e.ColumnIndex == dgcRegNr.Index)
            {
                string s = dgvPersons.CurrentRow.Cells[dgcRegNr.Index].Value.AsString();
                if (!string.IsNullOrEmpty(s) && s.Length == 11)
                {
                    dr.PVNREGNR = "LV" + s;
                    dgvPersons.InvalidateRow(e.RowIndex);
                }
            }
        }


        private void SetSaveButton(bool red)
        {
            bnavPersons.SetSaveButton(tsbSave, red);
        }

        public override bool SaveData()
        {
            if (!dgvPersons.EndEditX()) return false;
            var ret = bsPersons.SaveDbContext();
            CheckSave();
            return ret.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsPersons.HasChanges());
        }


        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void dgvPersons_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            SaveData();
        }

        private void bsPersons_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            SetSaveButton(bsPersons.HasChanges());
        }


    }
}
