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
using KlonsLIB.Misc;
using Klons3.ModelsA;

namespace KlonsA.Forms
{
    public partial class FormA_TimeSheets : MyFormBaseF
    {
        public FormA_TimeSheets()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            bsSar.SetFilter<A_TIMESHEET_LISTS>(x => !x.ISFIRSTMT);
        }

        private void Form_TimeSheets_Load(object sender, EventArgs e)
        {
            tslPeriod.Text = DataLoaderA.GetPeriodStr();
            CheckSave();
        }

        public bool CanDelete()
        {
            if (MyMainForm.FindForm(typeof(FormA_TimeSheet)) != null)
            {
                MyMainForm.ShowWarning("Vispirms jāaizver froma [Darba laika uzskaites lapa].");
                return false; ;
            }
            return true;
        }

        private void dgvSar_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!CanDelete() || e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void dgvSarR_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!CanDelete() || e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void bnavSar_ItemDeleting(object sender, CancelEventArgs e)
        {
            if (!CanDelete() || !AskCanDelete()) e.Cancel = true;
        }

        private void dgvSar_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvSar.CurrentRow == null || dgvSar.CurrentRow.IsNewRow ||
                dgvSar.CurrentCell == null) return;
            int colnr = dgvSar.CurrentCell.ColumnIndex;

            if (e.KeyCode == Keys.F5)
            {
                if (colnr != dgcSarDep.Index) return;
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
                MakeList();
                e.Handled = true;
                return;
            }
        }

        private void dgvSar_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSar.CurrentRow == null || dgvSar.CurrentRow.IsNewRow ||
                dgvSar.CurrentCell == null) return;
            int colnr = dgvSar.CurrentCell.ColumnIndex;
            if (colnr != e.ColumnIndex) return;

            if (colnr == dgcSarDep.Index)
            {
                GetIDDepFromDialog();
            }
        }

        public void GetIDDepFromDialog()
        {
            if (bsSar.Count == 0 || bsSar.Current == null) return;
            if (dgvSar.CurrentRow == null || dgvSar.CurrentRow.IsNewRow) return;
            var fm = new FormA_Departments();
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return;
            var dr = bsSar.GetCurrentItem<A_TIMESHEET_LISTS>();
            if (dgvSar.CurrentCell != null)
            {
                try
                {
                    dgvSar.BeginEdit(false);
                    var c = dgvSar.EditingControl as DataGridViewComboBoxEditingControl;
                    c.SelectedValue = fm.SelectedObject;
                    dgvSar.EndEdit();
                }
                catch (Exception) { }
            }
        }

        public override bool SaveData()
        {
            if (!dgvSar.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = bsSar.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private void SetSaveButton(bool red)
        {
            bnavSar.SetSaveButton(tsbSave, red);
        }

        private bool HasChanges()
        {
            return bsSar.HasChanges();
        }

        private void CheckSave()
        {
            SetSaveButton(HasChanges());
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void dgvSar_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }
        private void bsSar_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void AddNew(int yr, int mt, A_DEPARTMENTS dr_dep, string descr)
        {
            if (!SaveBeforeProceed()) return;
            var table_sar = MyData.DbContextA.BL_A_TIMESHEET_LISTS;
            var new_dr = table_sar.NewItem();
            new_dr.YR = yr;
            new_dr.MT = mt;
            new_dr.SNR = DataTasksA.GetNextTimeSheetNr(yr);
            new_dr.Department = dr_dep;
            new_dr.DESCR = descr.Nz();
            table_sar.Add(new_dr);
        }

        public void MakeListFromTempl(int yr, int mt, A_TIMESHEET_TEMPL dr_sh = null)
        {
            var ret = DataTasksA.MakeTimeSheetsFromTempl(yr, mt, dr_sh);
            if (ret != "OK")
            {
                MyMainForm.ShowWarning(ret);
                return;
            }
        }

        public void MakeList()
        {
            if (!SaveBeforeProceed()) return;
            var fm = new FormA_TimeSheetsNew();
            var ret = fm.ShowDialog(MyMainForm);
            if (ret != DialogResult.OK) return;

            var fmts = MyMainForm.FindForm(typeof(FormA_TimeSheet)) as FormA_TimeSheet;
            if (fmts != null) fmts.DoBeforeAddNewList();

            if (fm.MakeEmpty)
            {
                AddNew(fm.Year, fm.Month, fm.Department, fm.Descr);
            }
            else if (fm.MakeAll)
            {
                MakeListFromTempl(fm.Year, fm.Month);
            }
            else
            {
                MakeListFromTempl(fm.Year, fm.Month, fm.TimeSheetTemplate);
            }

            SaveData();

            if (fmts != null) fmts.DoAfterAddNewList();
        }

        public void OpenCurrent()
        {
            if (bsSar.Current == null) return;
            if (!SaveBeforeProceed()) return;
            var dr = bsSar.GetCurrentItem<A_TIMESHEET_LISTS>();
            var f = MyMainForm.FindForm(typeof(FormA_TimeSheet)) as FormA_TimeSheet;
            if (f == null)
                f = MyMainForm.ShowForm(typeof(FormA_TimeSheet)) as FormA_TimeSheet;
            f.Activate();
            f.SelectSheet(dr);
        }

        public void DeleteCurrent()
        {
            if (bsSar.Current == null || !CanDelete()) return;
            if (!SaveBeforeProceed()) return;
            if (!AskCanDelete()) return;
            bsSar.RemoveCurrent();
            SaveData();
        }

        private void tsbOpen_Click(object sender, EventArgs e)
        {
            OpenCurrent();
        }

        private void tsbNew_Click(object sender, EventArgs e)
        {
            MakeList();
        }

        private void tsbDelete_Click(object sender, EventArgs e)
        {
            DeleteCurrent();
        }

    }
}
