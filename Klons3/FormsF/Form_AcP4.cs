﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Klons3.ModelsF;
using KlonsF.Classes;
using KlonsLIB.Components;
using KlonsLIB.Data;
using KlonsLIB.Misc;
using ModelsF = Klons3.ModelsF;

namespace KlonsF.Forms
{
    public partial class Form_AcP4 : MyFormBaseF
    {
        public Form_AcP4()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            bsAcP4.Fill();
            CheckSave();
        }

        public static string GetAcP4(string acp4)
        {
            var fm = new Form_AcP4();
            fm.SelectedValueStr = acp4;
            fm.StartPosition = FormStartPosition.CenterParent;
            fm.FindAcP4(acp4);
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return null;
            return fm.SelectedValueStr;
        }

        public void FindAcP4(string acp4)
        {
            if (bsAcP4.Count == 0) return;
            if (acp4.IsNOE()) return;
            for (int i = 0; i < bsAcP4.Count; i++)
            {
                var dr = bsAcP4.GetItem<F_ACP24>(i);
                if (dr.IDX == acp4)
                {
                    bsAcP4.Position = i;
                    return;
                }
            }
        }

        private void FormAcP4_Load(object sender, EventArgs e)
        {
            CheckSave();
        }

        private void SelectCurrent()
        {
            if (dgvAcP4.CurrentRow == null || dgvAcP4.CurrentRow.IsNewRow) return;
            if (!dgvAcP4.EndEdit()) return;
            string ac = dgvAcP4.CurrentRow.Cells[0].Value as string;
            if (!SaveData()) return;
            if (!this.IsMyDialog) return;
            SetSelectedValueStr(ac);
        }

        private void dgvAcP4_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectCurrent();
        }

        private void tbIdx_RowSelectedEvent(object sender, RowSelectedEventArgs e)
        {
            SetSelectedValueStr(e.Value);
        }

        private void dgvAcP4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Return)
            {
                SelectCurrent();
                e.Handled = true;
            }
        }

        private void tbIdx_Enter(object sender, EventArgs e)
        {
            tbIdx.SelectAll();
        }

        private void tbSearch_Enter(object sender, EventArgs e)
        {
            tbSearch.SelectAll();
        }

        private void tbSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                string s = tbSearch.Text;
                if (s == "")
                {
                    bsAcP4.RemoveFilter();
                }
                else
                {
                    Predicate<ModelsF.F_ACP23> filter = x => x.NAME.ContainsCI(s);
                    bsAcP4.SetFilter(filter);
                }
            }
        }

        private void FormAcP4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Escape)
            {
                SetSelectedValueStr(null);
            }
        }

        private void dgvAcP4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SetSelectedValueStr(null);
            }
        }

        private void dgvAcP4_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvAcP4.ReadOnly)
            {
                return;
            }
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                dgvAcP4.MoveToNewRow();
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                if (dgvAcP4.CurrentRow != null && !dgvAcP4.CurrentRow.IsNewRow)
                {
                    if (!dgvAcP4.EndEdit()) return;
                    bnavAcp4.DeleteCurrent();
                }
                e.Handled = true;
            }
        }

        private void bnavAcp4_ItemDeleting(object sender, CancelEventArgs e)
        {
            if (dgvAcP4.ReadOnly)
            {
                e.Cancel = true;
                return;
            }
            e.Cancel = !AskCanDelete();
        }

        private void dgvAcP4_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!e.Row.IsNewRow)
            {
                if (!AskCanDelete()) e.Cancel = true;
            }
        }

        private void SetSaveButton(bool red)
        {
            bnavAcp4.SetSaveButton(tsbSave, red);
        }

        public override bool SaveData()
        {
            if (!dgvAcP4.EndEditX()) return false;
            var ret = bsAcP4.SaveDbContext();
            CheckSave();
            return ret.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsAcP4.HasChanges());
        }

        private void dgvAcP4_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            SaveData();
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
            SetSaveButton(bsAcP4.HasChanges());
        }

        private void bsAcP4_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }
    }
}
