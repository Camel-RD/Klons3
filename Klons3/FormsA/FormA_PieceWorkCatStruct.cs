﻿using System;
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
using KlonsLIB.Forms;
using Klons3.ModelsA;
using KlonsLIB.Misc;

namespace KlonsA.Forms
{
    public partial class FormA_PieceWorkCatStruct : MyFormBaseF
    {
        public FormA_PieceWorkCatStruct()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            cbActive.SelectedIndex = 0;
        }

        private void Form_PieceWorkCatStruct_Load(object sender, EventArgs e)
        {
            SetupToolStrips();
            CheckSave();
        }

        private void SetupToolStrips()
        {
            var si1 = InsertInToolStrip(toolStrip1, tbCode, 0);
            InsertInToolStrip(toolStrip1, tbFilter, 2);
            InsertInToolStrip(toolStrip1, cbActive, 4);
        }

        private void SelectCurrent()
        {
            if (dgvSar.CurrentRow == null || dgvSar.CurrentRow.IsNewRow) return;
            var dr = bsSar.GetCurrentItem<A_PIECEWORK_CATSTRUCT>();
            if (!dgvSar.EndEditX()) return;
            if (!SaveData()) return;
            if (bsSar.IsItemDetached(dr)) return;
            int id = dr.ID;
            if (!this.IsMyDialog) return;
            SetSelectedObject(dr);
        }

        public void DeleteCurrent()
        {
            bnavSar.DeleteCurrent();
            SaveData();
        }

        private void dgvSar_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgcCode.Index)
            {
                SelectCurrent();
            }
        }

        private void tbCode_RowSelectedEvent(object sender, KlonsLIB.Components.RowSelectedEventArgs e)
        {
            SelectCurrent();
        }

        private void dgvSar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Return)
            {
                SelectCurrent();
                e.Handled = true;
            }
        }

        private void dgvSar_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvSar.CurrentCell == null) return;
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                dgvSar.MoveToNewRow();
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                DeleteCurrent();
                e.Handled = true;
            }
        }

        private void dgvSar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SetSelectedObject(null);
            }
        }

        private void tbFilter_Enter(object sender, EventArgs e)
        {
            tbFilter.SelectAll();
        }

        private void tbFilter_MouseDown(object sender, MouseEventArgs e)
        {
            tbFilter.SelectAll();
        }

        private void tbCode_Enter(object sender, EventArgs e)
        {
            tbCode.SelectAll();
        }

        private void tbCode_MouseDown(object sender, MouseEventArgs e)
        {
            tbCode.SelectAll();
        }

        private void CheckFilter()
        {
            string s1 = tbFilter.Text;
            int k = cbActive.SelectedIndex;
            var pred_list = new List<Predicate<A_PIECEWORK_CATSTRUCT>>();
            if (k == 0)
                pred_list.Add(x => x.USED == 1);
            if (!s1.IsNOE())
                pred_list.Add(x => x.CODE.ContainsCI(s1));
            bsSar.SetFilter(pred_list);
        }

        private void cbActive_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckFilter();
        }

        private void tbFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                CheckFilter();
        }

        private void bnavSar_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void dgvSar_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete())
                e.Cancel = true;
        }

        private void SetSaveButton(bool red)
        {
            bnavSar.SetSaveButton(tsbSave, red);
        }

        public override bool SaveData()
        {
            if (!dgvSar.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = bsSar.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsSar.HasChanges());
        }

        private void dgvSar_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void bsSar_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            DeleteCurrent();
        }
    }
}
