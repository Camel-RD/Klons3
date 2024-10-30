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
using KlonsLIB.Forms;
using KlonsLIB.Misc;

namespace KlonsA.Forms
{
    public partial class FormA_PastData : MyFormBaseF
    {
        public FormA_PastData()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            bsPrevMonths.Fill();
        }

        private void Form_PastData_Load(object sender, EventArgs e)
        {
            InsertInToolStrip(toolStrip1, tbFilter, -1);
            CheckSave();
        }
        private void Form_PastData_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private int? LastIDP = null;
        private int? LastYR = null;

        private A_PERSONS LastPersonRow = null;

        private void BsPrevMonths_ListItemPropertyChanged(object sender, KlonsLIB.Data.MyItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(A_PASTDATA.Person) &&
                e.PropertyName != nameof(A_PASTDATA.YR)) return;
            var dr = e.Item as A_PASTDATA;
            //if (dr.RowState == DataRowState.Detached) return;
            LastPersonRow = dr.Person;
            LastIDP = dr.IDP;
            LastYR = dr.YR;
        }

        private void tbFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                string s = tbFilter.Text;
                if (s == "")
                {
                    bsPrevMonths.RemoveFilter();
                }
                else
                {
                    bsPrevMonths.SetFilter<A_PASTDATA>(x => x.ZNAME.ContainsCI(s));
                }
            }
        }

        public void GetIDPFromDialog()
        {
            if (bsPrevMonths.Count == 0 || bsPrevMonths.Current == null) return;
            if (dgvPrevMonths.CurrentRow == null || dgvPrevMonths.CurrentRow.IsNewRow) return;
            var fm = new FormA_Persons();
            fm.WhatToSelect = FormA_Persons.EWhatToSelect.Person;
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return;
            var dr = dgvPrevMonths.GetCurrentObjectViewItem<A_PASTDATA>();
            if (dgvPrevMonths.CurrentCell != null)
            {
                try
                {
                    dgvPrevMonths.BeginEdit(false);
                    var c = dgvPrevMonths.EditingControl as DataGridViewComboBoxEditingControl;
                    c.SelectedValue = fm.SelectedObject;
                    dgvPrevMonths.EndEdit();
                }
                catch (Exception) { }
            }
        }

        private void dgvPrevMonths_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPrevMonths.CurrentCell == null) return;
            if (e.RowIndex == -1 || e.RowIndex == dgvPrevMonths.NewRowIndex) return;
            if (e.ColumnIndex == dgcIDP.Index)
            {
                GetIDPFromDialog();
                return;
            }

        }

        private void dgvPrevMonths_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvPrevMonths.CurrentCell == null) return;
            if (e.KeyCode == Keys.F5)
            {
                if (dgvPrevMonths.CurrentCell.ColumnIndex == dgcIDP.Index)
                {
                    GetIDPFromDialog();
                    e.Handled = true;
                    return;
                }
            }
            if (e.KeyCode == Keys.Delete && e.Control)
            {
                DeleteCurrent();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                if (!dgvPrevMonths.EndEdit()) return;
                dgvPrevMonths.MoveToNewRow();
                e.Handled = true;
                return;
            }
        }


        private void bnavPrevMonths_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void dgvPrevMonths_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete())
                e.Cancel = true;
        }

        private void SetSaveButton(bool red)
        {
            bnavPrevMonths.SetSaveButton(tsbSave, red);
        }

        public override bool SaveData()
        {
            if (!dgvPrevMonths.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = bsPrevMonths.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsPrevMonths.HasChanges());
        }


        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void dgvPrevMonths_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            SaveData();
        }

        private void bsPrevMonths_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void dgvPrevMonths_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            object mt = dgvPrevMonths[dgcMT.Index, e.RowIndex]?.Value;
            if (mt == null || (int)mt < 1 || (int)mt > 12)
            {
                MyMainForm.ShowWarning("Nekorekts mēnesis.");
                e.Cancel = true;
                return;
            }
        }

        public void DeleteCurrent()
        {
            bnavPrevMonths.DeleteCurrent();
            SaveData();
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            DeleteCurrent();
        }

        private void dgvPrevMonths_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            if (LastIDP == null) return;
            e.Row.Cells[dgcIDP.Index].Value = LastIDP.Value;
            e.Row.Cells[dgcYR.Index].Value = LastYR.Value;
        }

    }
}
