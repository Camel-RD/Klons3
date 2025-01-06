﻿
using System;
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
    public partial class Form_AcPlan : MyFormBaseF
    {
        public Form_AcPlan()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            bsAcPlan.Fill();
        }

        public static string GetAcP1(string acp1)
        {
            var fm = new Form_AcPlan();
            fm.SelectedValueStr = acp1;
            fm.StartPosition = FormStartPosition.CenterParent;
            fm.FindAcP1(acp1);
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return null;
            return fm.SelectedValueStr;
        }

        public void FindAcP1(string acp1)
        {
            if (bsAcPlan.Count == 0) return;
            if (acp1.IsNOE()) return;
            for (int i = 0; i < bsAcPlan.Count; i++)
            {
                var dr = bsAcPlan.GetItem<F_ACP21>(i);
                if (dr.AC == acp1)
                {
                    bsAcPlan.Position = i;
                    return;
                }
            }
        }

        private void FormAcPlan_Load(object sender, EventArgs e)
        {
            CheckSave();
        }

        private void SelectCurrent()
        {
            if (dgvAcp21.CurrentRow == null || dgvAcp21.CurrentRow.IsNewRow) return;
            if (!dgvAcp21.EndEdit()) return;
            string ac = dgvAcp21.CurrentRow.Cells[0].Value as string;
            if (!SaveData()) return;
            if (!this.IsMyDialog) return;
            SetSelectedValueStr(ac);
        }

        private void acP21DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                SelectCurrent();
            }
        }

        private void tbAcc_RowSelectedEvent(object sender, RowSelectedEventArgs e)
        {
            SetSelectedValueStr(e.Value);
        }

        private void acP21DataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Return)
            {
                SelectCurrent();
                e.Handled = true;
            }
        }

        private void acP21DataGridView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SetSelectedValueStr(null);
            }
        }

        private void tbSearch_Enter(object sender, EventArgs e)
        {
            tbSearch.SelectAll();
        }

        private void tbAcc_Enter(object sender, EventArgs e)
        {
            tbAcc.SelectAll();
        }

        private void tbSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                string s = tbSearch.Text;
                if (s == "")
                {
                    bsAcPlan.RemoveFilter();
                }
                else
                {
                    Predicate<ModelsF.F_ACP23> filter = x => x.NAME.ContainsCI(s);
                    bsAcPlan.SetFilter(filter);
                }
            }
            if (e.KeyChar == (char)Keys.Escape)
            {
                tbSearch.Text = null;
                bsAcPlan.RemoveFilter();
                e.Handled = true;
            }
        }

        private void acP21BindingNavigator_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void acP21DataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete())
                e.Cancel = true;
        }

        private void acP21DataGridView_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                dgvAcp21.MoveToNewRow();
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                if (dgvAcp21.CurrentRow != null && !dgvAcp21.CurrentRow.IsNewRow)
                {
                    if (!dgvAcp21.EndEdit()) return;
                    bnavAcp21.DeleteCurrent();
                    e.Handled = true;
                }
            }
        }

        private void SetSaveButton(bool red)
        {
            bnavAcp21.SetSaveButton(tsbSave, red);
        }

        public override bool SaveData()
        {
            if (!dgvAcp21.EndEditX()) return false;
            var ret = bsAcPlan.SaveDbContext();
            CheckSave();
            return ret.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsAcPlan.HasChanges());
        }

        private void dgvAcp21_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            SaveData();
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void bsAcPlan_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void toolStripButtonInfo_Click(object sender, EventArgs e)
        {
            if (!dgvAcp21.EndEdit()) return;
            bsAcPlan.ShowStats();
        }
    }
}
