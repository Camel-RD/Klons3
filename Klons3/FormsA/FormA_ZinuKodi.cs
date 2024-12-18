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
using Klons3.ModelsA;
using KlonsLIB.Data;

namespace KlonsA.Forms
{
    public partial class FormA_ReportCodes : MyFormBaseF
    {
        public FormA_ReportCodes()
        {
            InitializeComponent();
            CheckMyFontAndColors();

            bsZinuKodi.Fill();
        }

        private void Form_ReportCodes_Load(object sender, EventArgs e)
        {
            InsertInToolStrip(toolStrip1, tbCode, 0);
            InsertInToolStrip(toolStrip1, cbUsed, 2);

            cbUsed.SelectedIndex = 1;
            CheckSave();
            tbCode.Select();
        }

        private void SelectCurrent()
        {
            if (dgvZinuKodi.CurrentRow == null || dgvZinuKodi.CurrentRow.IsNewRow) return;
            var dr = bsZinuKodi.GetCurrentItem<A_REPORT_CODES>();
            if (!dgvZinuKodi.EndEdit()) return;
            if (!SaveData()) return;
            if (bsZinuKodi.IsItemDetached(dr)) return;
            string id = dr.CODE;
            if (!this.IsMyDialog) return;
            SetSelectedObject(dr);
        }

        private void tbCode_RowSelectedEvent(object sender, KlonsLIB.Components.RowSelectedEventArgs e)
        {
            SetSelectedObject(e.Value);
        }

        private void dgvZinuKodi_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                SelectCurrent();
            }
        }

        private void dgvZinuKodi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Return)
            {
                SelectCurrent();
                e.Handled = true;
            }
        }

        private void dgvZinuKodi_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SetSelectedObject(null);
            }
        }

        private void tbCode_Enter(object sender, EventArgs e)
        {
            tbCode.SelectAll();
        }

        private void bnNav_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void dgvZinuKodi_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete())
                e.Cancel = true;
        }
        private void SetSaveButton(bool red)
        {
            bnNav.SetSaveButton(tsbSave, red);
        }

        public override bool SaveData()
        {
            if (!dgvZinuKodi.EndEditX()) return false;
            var ret = bsZinuKodi.SaveDbContext();
            CheckSave();
            return ret.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsZinuKodi.HasChanges());
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
            CheckSave();
        }

        private void dgvZinuKodi_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void bsZinuKodi_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void cbUsed_SelectedIndexChanged(object sender, EventArgs e)
        {
            int k = cbUsed.SelectedIndex;
            if (k == 0)
                bsZinuKodi.RemoveFilter();
            else
                bsZinuKodi.SetFilter<A_REPORT_CODES>(x => x.USED == 1);
        }
    }
}
