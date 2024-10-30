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
using KlonsLIB.Forms;
using Klons3.ModelsA;
using Equin.ApplicationFramework;
using Klons3.ModelsF;
using System.Collections;

namespace KlonsA.Forms
{
    public partial class FormA_PayListsTempl : MyFormBaseF
    {
        public FormA_PayListsTempl()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            bsAmati.SetFilter<A_POSITIONS>(x => x.USED == 1);
            bsAmatiF.SetFilter<A_POSITIONS>(x => x.USED == 1);
            CheckDgvRowsAllowAddRows();
        }

        private void Form_PayListsTempl_Load(object sender, EventArgs e)
        {
            CheckEnableGrid();
            CheckSave();
        }

        private void FormA_PayListsTempl_Shown(object sender, EventArgs e)
        {
            if (bsPers.Count == 0)
            {
                MyMainForm.ShowWarning("Darbinieku saraksts ir tukšs.");
                Close();
            }
        }

        private void BsShR_ListItemPropertyChanged(object sender, MyItemPropertyChangedEventArgs e)
        {
            PAYLIST_TEMPL_R_ColumnChanged(e);
        }

        private void PAYLIST_TEMPL_R_ColumnChanged(MyItemPropertyChangedEventArgs e)
        {
            var dr = e.Item as A_PAYLIST_TEMPL_R;
            if (e.PropertyName == nameof(A_PAYLIST_TEMPL_R.Person))
            {
                if (dr.Person == null)
                {
                    dr.Position = null;
                }
                else
                {
                    var drp = dr.Person;
                    if (drp != null && drp.Positions.Count > 0)
                    {
                        var dr_am = drp.Positions[0];
                        dr.Position = dr_am;
                    }
                }
                int k = FindRowInBsShR(dr);
                if (k != -1)
                    dgvShR.InvalidateCell(dgcShRIdAM.Index, k);
            }
        }

        private int FindRowInBsShR(A_PAYLIST_TEMPL_R dr)
        {
            if (dr == null || bsShR.Count == 0) return -1;
            for (int i = 0; i < bsShR.Count; i++)
            {
                var dr2 = bsShR.GetItem<A_PAYLIST_TEMPL_R>(i);
                if (dr == dr2) return i;
            }
            return -1;
        }

        private void SelectCurrent()
        {
            if (dgvShL.CurrentRow == null || dgvShL.CurrentRow.IsNewRow) return;
            var dr = bsShL.GetCurrentItem<A_PAYLIST_TEMPL>();
            if (!dgvShL.EndEdit()) return;
            if (!SaveData()) return;
            if (bsShL.IsItemDetached(dr)) return;
            if (!this.IsMyDialog) return;
            SetSelectedObject(dr);
        }

        private void dgvShL_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete())
                e.Cancel = true;
        }

        private void dgvShR_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete())
                e.Cancel = true;
        }

        private void bnavSh_ItemDeleting(object sender, CancelEventArgs e)
        {
            if (!AskCanDelete()) e.Cancel = true;
        }

        private void dgvShL_Enter(object sender, EventArgs e)
        {
            bnavSh.BindingSource = bsShL;
            bnavSh.DataGrid = dgvShL;
            toolStripLabel1.Text = "Saraksti:";
        }

        private void dgvShR_Enter(object sender, EventArgs e)
        {
            bnavSh.BindingSource = bsShR;
            bnavSh.DataGrid = dgvShR;
            toolStripLabel1.Text = "Saraksta rindas:";
        }

        public void DeleteCurrent()
        {
            bnavSh.DeleteCurrent();
            SaveData();
        }

        private void dgvShL_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvShL.CurrentRow == null || dgvShL.CurrentRow.IsNewRow ||
                dgvShL.CurrentCell == null) return;
            int colnr = dgvShL.CurrentCell.ColumnIndex;

            if (e.KeyCode == Keys.F5)
            {
                if (colnr != dgcShLDep.Index) return;
                GetIDDepFromDialog();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Delete && e.Control)
            {
                DeleteCurrent();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                if (!dgvShL.EndEdit()) return;
                dgvShL.MoveToNewRow();
                e.Handled = true;
                return;
            }
            if (e.Control && e.KeyCode == Keys.Return)
            {
                SelectCurrent();
                e.Handled = true;
            }
        }

        private void dgvShR_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvShR.CurrentRow == null || dgvShR.CurrentRow.IsNewRow ||
                dgvShR.CurrentCell == null) return;
            int colnr = dgvShR.CurrentCell.ColumnIndex;

            if (e.KeyCode == Keys.F5)
            {
                if (colnr != dgcShRIdP.Index && colnr != dgcShRIdAM.Index) return;
                GetIDPFromDialog();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Delete && e.Control)
            {
                DeleteCurrent();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                if (!dgvShR.EndEdit()) return;
                dgvShR.MoveToNewRow();
                e.Handled = true;
                return;
            }
        }

        private void dgvShL_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvShL.CurrentRow == null || dgvShL.CurrentRow.IsNewRow ||
                dgvShL.CurrentCell == null) return;
            int colnr = dgvShL.CurrentCell.ColumnIndex;
            if (colnr != e.ColumnIndex) return;

            if (colnr == dgcShLDep.Index)
            {
                GetIDDepFromDialog();
            }
            if (colnr == dgcShLCode.Index || colnr == dgcShLSnr.Index)
            {
                SelectCurrent();
            }
        }

        private void dgvShR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvShR.CurrentRow == null || dgvShR.CurrentRow.IsNewRow ||
                dgvShR.CurrentCell == null) return;
            int colnr = dgvShR.CurrentCell.ColumnIndex;
            if (colnr != dgcShRIdP.Index && colnr != dgcShRIdAM.Index) return;
            GetIDPFromDialog();
        }

        public void GetIDPFromDialog()
        {
            if (bsShR.Count == 0 || bsShR.Current == null) return;
            if (dgvShR.CurrentRow == null || dgvShR.CurrentRow.IsNewRow) return;
            var fm = new FormA_Persons();
            fm.WhatToSelect = FormA_Persons.EWhatToSelect.Both;
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return;
            var dr = dgvShR.GetCurrentObjectViewItem<A_PAYLIST_TEMPL_R>();
            if (dgvShR.CurrentCell != null)
            {
                try
                {
                    if (dgvShR.CurrentCell.OwningColumn == dgcShRIdP)
                    {
                        dgvShR.BeginEdit(false);
                        var c = dgvShR.EditingControl as DataGridViewComboBoxEditingControl;
                        c.SelectedValue = fm.SelectedPerson;
                        dgvShR.EndEdit();

                        dr.Person = fm.SelectedPerson;
                    }
                    if (dgvShR.CurrentCell.OwningColumn == dgcShRIdAM)
                    {
                        dgvShR.EndEdit();

                        dr.Person = fm.SelectedPerson;

                        dgvShR.BeginEdit(false);
                        var c = dgvShR.EditingControl as DataGridViewComboBoxEditingControl;
                        c.SelectedValue = fm.SelectedPosition;
                        dgvShR.EndEdit();
                    }
                }
                catch (Exception) { }
            }
        }

        public void GetIDDepFromDialog()
        {
            if (bsShL.Count == 0 || bsShL.Current == null) return;
            if (dgvShL.CurrentRow == null || dgvShL.CurrentRow.IsNewRow) return;
            var fm = new FormA_Departments();
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return;
            var dr = dgvShL.GetCurrentObjectViewItem<A_PAYLIST_TEMPL>();
            if (dgvShL.CurrentCell != null)
            {
                try
                {
                    dgvShL.BeginEdit(false);
                    var c = dgvShL.EditingControl as DataGridViewComboBoxEditingControl;
                    c.SelectedValue = fm.SelectedObject;
                    dgvShL.EndEdit();
                }
                catch (Exception) { }
            }
        }

        private void CheckEnableGrid()
        {
             var enabled = !(dgvShL.CurrentRow == null ||
                dgvShL.CurrentRow.IsNewRow);
            SetControlEnabled(dgvShR, enabled);
        }

        private void dgvShL_CurrentCellChanged(object sender, EventArgs e)
        {
            CheckEnableGrid();
        }

        public override bool SaveData()
        {
            if (!dgvShL.EndEditX()) return false;
            if (!dgvShR.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = bsShL.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private void SetSaveButton(bool red)
        {
            bnavSh.SetSaveButton(tsbSave, red);
        }
        private bool HasChanges()
        {
            return bsShL.HasChanges() || bsShR.HasChanges();
        }

        private void CheckSave()
        {
            SetSaveButton(HasChanges());
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void dgvShL_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void dgvShR_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if (e.ColumnIndex == dgcShRIdAM.Index)
            {
                var val_dr = dgvShR[dgcShRIdP.Index, e.RowIndex].Value;
                if (val_dr == null || val_dr == DBNull.Value)
                {
                    e.Cancel = true;
                    return;
                }
                var c0 = dgvShR[e.ColumnIndex, e.RowIndex];
                var c = c0 as DataGridViewComboBoxCell;
                if (c == null) return;
                var dr_person = val_dr as A_PERSONS;
                bsAmatiF.SetFilter<A_POSITIONS>(x => x.USED == 1 && x.Person == dr_person);
                c.DataSource = bsAmatiF;
            }
        }

        private void dgvShR_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgcShRIdAM.Index)
            {
                var c = dgvShR[e.ColumnIndex, e.RowIndex] as DataGridViewComboBoxCell;
                c.DataSource = bsAmati;
            }
        }

        private void dgvShL_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[dgcShLSnr.Index].Value = dgvShL.Rows.Count;
        }

        private void dgvShR_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[dgcShRSNR.Index].Value = dgvShR.Rows.Count;
            /*
            var dr_person = bsPers.GetDataRow<A_PERSONS>(0);
            if (dr_person != null)
            {
                e.Row.Cells[dgcShRIdP.Index].Value = dr_person.ID;
                e.Row.Cells[dgcShRIdAM.Index].Value = dr_person.PositionsRows[0].ID;
            }
            */
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            DeleteCurrent();
        }

        private void bsShL_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void bsShR_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            if (e.ListChangedType == ListChangedType.Reset) return;
            CheckSave();
        }

        private void RenumRows()
        {
            for (int i = 0; i < bsShR.Count; i++)
            {
                var dr = (bsShR[i] as ObjectView<A_PAYLIST_TEMPL_R>).Object;
                if (dr.SNR != (short)(i + 1)) dr.SNR = (short)(i + 1);
            }
        }

        private void tsbRenum_Click(object sender, EventArgs e)
        {
            RenumRows();
        }

        private void CheckDgvRowsAllowAddRows()
        {
            dgvShR.AllowUserToAddRows = MyData.DbContextA.BL_A_PERSONS.Count > 0;
        }

        private void bsPers_ListChanged(object sender, ListChangedEventArgs e)
        {
            CheckDgvRowsAllowAddRows();
        }

    }
}
