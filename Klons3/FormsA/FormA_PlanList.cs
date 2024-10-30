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
using KlonsLIB.Misc;

namespace KlonsA.Forms
{
    public partial class FormA_PlanList : MyFormBaseF
    {
        public FormA_PlanList()
        {
            InitializeComponent();
            CheckMyFontAndColors();

            dgcKind1.DataSource = SomeDataDefsA.LaikaPlanuSarK1;
            dgcKind1.DisplayMember = "Val";
            dgcKind1.ValueMember = "Key";

            dgcKind2.DataSource = SomeDataDefsA.LaikaPlanuSarK2;
            dgcKind2.DisplayMember = "Val";
            dgcKind2.ValueMember = "Key";

            cbActive.SelectedIndex = 0;

            var a = bsPlanuSar.List;
            var s = MyData.DbContextA.BL_A_TIMEPLAN_LIST;
        }

        private void Form_PlanList_Load(object sender, EventArgs e)
        {
            InsertInToolStrip(toolStrip1, tbSearch, 1);
            InsertInToolStrip(toolStrip1, cbActive, 2);

            CheckSave();
        }

        private void SelectCurrent()
        {
            if (dgvPlanuSar.CurrentRow == null || dgvPlanuSar.CurrentRow.IsNewRow) return;
            var dr = bsPlanuSar.GetCurrentItem<A_TIMEPLAN_LIST>();
            if (!dgvPlanuSar.EndEdit()) return;
            if (!SaveData()) return;
            if (bsPlanuSar.IsItemDetached(dr)) return;
            if (!this.IsMyDialog) return;
            SetSelectedObject(dr);
        }

        public void DeleteCurrent()
        {
            bnavPlanuSar.DeleteCurrent();
            SaveData();
        }

        private void dgvPlanuSar_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgcDescr.Index || e.ColumnIndex == dgcSnr.Index)
            {
                SelectCurrent();
            }
        }

        private void dgvPlanuSar_KeyDown(object sender, KeyEventArgs e)
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

        private void dgvPlanuSar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SetSelectedObject(null);
            }
        }

        private void tbSearch_Enter(object sender, EventArgs e)
        {
            tbSearch.SelectAll();
        }

        private void CheckFilter()
        {
            string s = tbSearch.Text;
            var pred_list = new List<Predicate<A_TIMEPLAN_LIST>>();
            if (!s.IsNOE())
                pred_list.Add(x => x.DESCR.ContainsCI(s));
            if (cbActive.SelectedIndex == 0)
                pred_list.Add(x => x.USED == 1);
            bsPlanuSar.SetFilter(pred_list);
        }

        private void tbSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                CheckFilter();
            }
        }

        private void cbActive_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckFilter();
        }

        private void bnavPlanuSar_ItemDeleting(object sender, CancelEventArgs e)
        {
            if (!CanDelete() || !AskCanDelete())
                e.Cancel = true;
        }

        public bool CanDelete()
        {
            if (MyMainForm.FindForm(typeof(FormA_Plan)) != null)
            {
                MyMainForm.ShowWarning("Vispirms jāaizver forma [Darba laika kopplāns].");
                return false; ;
            }
            return true;
        }

        private void dgvPlanuSar_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!CanDelete() || e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void SetSaveButton(bool red)
        {
            bnavPlanuSar.SetSaveButton(tsbSave, red);
        }

        public override bool SaveData()
        {
            if (!dgvPlanuSar.EndEditX()) return false;
            var ret = bsPlanuSar.SaveDbContext();
            CheckSave();
            return ret.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsPlanuSar.HasChanges());
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
            SetSaveButton(bsPlanuSar.HasChanges());
        }

        private void bsPlanuSar_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void dgvPlanuSar_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void dgvPlanuSar_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[dgcSnr.Index].Value = (short)dgvPlanuSar.Rows.Count;
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            DeleteCurrent();
        }

    }
}

