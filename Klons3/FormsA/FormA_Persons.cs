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
using KlonsLIB.Misc;
using KlonsLIB.Forms;
using Klons3.ModelsA;
using Equin.ApplicationFramework;

namespace KlonsA.Forms
{
    public partial class FormA_Persons : MyFormBaseF
    {
        public FormA_Persons()
        {
            InitializeComponent();
            CheckMyFontAndColors();

            ShowOnlyActive = MyData.Params.PersListOnlyUsed;
        }

        public enum EWhatToSelect { Person, Position, Both }

        public EWhatToSelect WhatToSelect = EWhatToSelect.Person;

        public A_PERSONS SelectedPerson = null;
        public A_POSITIONS SelectedPosition = null;

        private void Form_Persons_Load(object sender, EventArgs e)
        {
            InsertInToolStrip(toolStrip1, tbID, 0);
            InsertInToolStrip(toolStrip1, tbSearch, 2);
            InsertInToolStrip(toolStrip1, cbActive, 4);

            MyData.DbContextA.A_PERSONS.Local.CollectionChanged += Persons_CollectionChanged;
            MyData.DbContextA.A_POSITIONS.Local.CollectionChanged += Positions_CollectionChanged;

            if (IsMdiChild) WindowState = FormWindowState.Minimized;

            splitContainer1.SplitterDistance = splitContainer1.Height
                - (int)((float)100 * ScaleFactor.Height);

            if (Modal || IsMyDialog)
                tbID.Select();

            CheckSave();
        }


        private void Form_Persons_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyData.DbContextA.A_PERSONS.Local.CollectionChanged -= Persons_CollectionChanged;
            MyData.DbContextA.A_POSITIONS.Local.CollectionChanged -= Positions_CollectionChanged;
        }

        private void Persons_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems == null) return;
                foreach (var item in e.NewItems)
                    OnPersonAdded(item as A_PERSONS);
            }
        }

        private void Positions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems == null) return;
                foreach (var item in e.NewItems)
                    OnPositionAdded(item as A_POSITIONS);
            }
        }

        private void OnPersonAdded(A_PERSONS dr_person)
        {
            var tablePersonsR = MyData.DbContextA.BL_A_PERSONS_R;
            var rwpr = tablePersonsR.NewItem();
            tablePersonsR.Add(rwpr);
            rwpr.Person = dr_person;
            rwpr.EDIT_DATE = DateTime.Today;
            if (!string.IsNullOrEmpty(dr_person.FNAME)) rwpr.FNAME = dr_person.FNAME;
            if (!string.IsNullOrEmpty(dr_person.LNAME)) rwpr.LNAME = dr_person.LNAME;
            if (!string.IsNullOrEmpty(dr_person.PK)) rwpr.PERSON_CODE = dr_person.PK;

            var tableAmati = MyData.DbContextA.BL_A_POSITIONS;
            var rwam = tableAmati.NewItem();
            tableAmati.Add(rwam);
            rwam.Person = dr_person;
            rwam.TITLE = "?";
        }

        private void OnPositionAdded(A_POSITIONS dr_position)
        {
            var tableAmatR = MyData.DbContextA.BL_A_POSITIONS_R;
            var rwamr = tableAmatR.NewItem();
            tableAmatR.Add(rwamr);
            rwamr.Position = dr_position;
            rwamr.EDIT_DATE = DateTime.Today;
            if (!dr_position.TITLE.IsNOE()) rwamr.TITLE = dr_position.TITLE;
            if (dr_position.Department != null) rwamr.Department = dr_position.Department;
        }

        private void SelectCurrent()
        {
            if (dgvPersons.CurrentRow == null || dgvPersons.CurrentRow.IsNewRow) return;
            if (dgvAmati.CurrentRow == null || dgvAmati.CurrentRow.IsNewRow) return;
            var drp = bsPersons.GetCurrentItem<A_PERSONS>();
            if (drp == null) return;
            var dram = bsAmati.GetCurrentItem<A_POSITIONS>();
            if (dram == null) return;
            if (!dgvPersons.EndEdit()) return;
            if (!dgvAmati.EndEdit()) return;
            //if (!SaveData()) return;
            if (bsPersons.IsItemDetached(drp)) return;
            if (bsPersons.IsItemDetached(dram)) return;

            if (!this.IsMyDialog) return;

            SelectedPerson = drp;
            SelectedPosition = dram;

            if (WhatToSelect == EWhatToSelect.Person)
            {
                //SetSelectedValueInt(drp.ID);
                SetSelectedObject(drp);
            }
            else
            {
                //SetSelectedValueInt(dram.ID);
                SetSelectedObject(dram);
            }
        }

        public void GetIDDepFromDialog()
        {
            if (bsAmati.Count == 0 || bsAmati.Current == null) return;
            if (dgvAmati.CurrentRow == null || dgvAmati.CurrentRow.IsNewRow) return;
            var fm = new FormA_Departments();
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return;
            if (dgvAmati.CurrentCell != null)
            {
                try
                {
                    dgvAmati.BeginEdit(false);
                    var c = dgvAmati.EditingControl as DataGridViewComboBoxEditingControl;
                    c.SelectedValue = fm.SelectedObject;
                    dgvAmati.EndEdit();
                }
                catch (Exception) { }
            }
        }

        private void dgvPersons_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (WhatToSelect != EWhatToSelect.Person) return;
            if (e.ColumnIndex == dgcFName.Index || e.ColumnIndex == dgcLName.Index)
                SelectCurrent();
        }

        private void dgvAmati_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvAmati.CurrentRow == null || dgvAmati.CurrentRow.IsNewRow ||
                dgvAmati.CurrentCell == null) return;
            int colnr = dgvAmati.CurrentCell.ColumnIndex;
            if (colnr != e.ColumnIndex) return;

            if (colnr == dgcAmatiTitle.Index)
            {
                if (WhatToSelect == EWhatToSelect.Person) return;
                SelectCurrent();
            }
            else if (colnr == dgcAmatiDep.Index)
            {
                GetIDDepFromDialog();
            }
        }

        private void tbID_RowSelectedEvent(object sender, KlonsLIB.Components.RowSelectedEventArgs e)
        {
            if (e.RowNr == -1)
            {
                SetSelectedValueInt(-1, true);
                return;
            }
            if (bsPersons.Current == null) return;
            SelectCurrent();
        }

        private void dgvPersons_KeyDown(object sender, KeyEventArgs e)
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

        private void dgvAmati_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Return &&
                WhatToSelect != EWhatToSelect.Person)
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

        private void dgvAmati_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvAmati.CurrentRow == null || dgvAmati.CurrentRow.IsNewRow ||
                dgvAmati.CurrentCell == null) return;
            int colnr = dgvAmati.CurrentCell.ColumnIndex;

            if (e.KeyCode == Keys.F5)
            {
                if (colnr != dgcAmatiDep.Index) return;
                GetIDDepFromDialog();
                e.Handled = true;
                return;
            }
        }


        private void dgvPersons_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                SetSelectedValueInt(-1, true);
        }

        private void dgvAmati_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                SetSelectedValueInt(-1, true);
        }

        private void tbSearch_Enter(object sender, EventArgs e)
        {
            tbSearch.SelectAll();
        }

        private void tbID_Enter(object sender, EventArgs e)
        {
            tbID.SelectAll();
        }

        private void tbSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                CheckFilter();
            }
        }

        public bool ShowOnlyActive
        {
            get { return MyData.Params.PersListOnlyUsed; }
            set
            {
                if (MyData.Params.PersListOnlyUsed != value)
                {
                    MyData.Params.PersListOnlyUsed = value;
                    CheckFilter();
                }
                int k = value ? 0 : 1;
                if (cbActive.SelectedIndex != k)
                    cbActive.SelectedIndex = k;
            }
        }

        private void cbActive_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowOnlyActive = cbActive.SelectedIndex == 0;
        }

        Predicate<A_POSITIONS> PositionsListFilter = null;

        private void CheckFilter()
        {
            var filter_persons = new List<Predicate<A_PERSONS>>();
            string s1 = tbSearch.Text;
            if (ShowOnlyActive)
            {
                filter_persons.Add(x => x.USED == 1);
                bsAmati.SetFilter<A_POSITIONS>(x => x.USED == 1);
            }
            else
            {
                bsAmati.RemoveFilter();
            }
            if (s1 != "")
            {
                filter_persons.Add(x => x.ZNAME.ContainsCI(s1));
            }
            bsPersons.SetFilter(filter_persons);
        }

        public void DeleteCurrent()
        {
            bnavNav.DeleteCurrent();
            SaveData();
        }

        public bool CanDelete()
        {
            if (!SaveData())
            {
                MyMainForm.ShowWarning("Neizdevās saglabāt izmaiņas.");
                return false; ;
            }
            if (MyMainForm.FindForm(typeof(FormA_PersonsR)) != null)
            {
                MyMainForm.ShowWarning("Vispirms jāaizver forma [Darbinieku dati].");
                return false; ;
            }
            if (bnavNav.BindingSource == bsPersons)
            {
                if (bsPersons.Current == null) return false;
                var dr_per = bsPersons.GetCurrentItem<A_PERSONS>();
                if (dr_per.Events.Count > 0 ||
                    dr_per.PastDataRows.Count > 0 ||
                    dr_per.PayListRows.Count > 0 ||
                    dr_per.PayListTemplateRows.Count > 0 ||
                    dr_per.PieceWorkRows.Count > 0 ||
                    dr_per.PositionsPlusMinusRows.Count > 0 ||
                    dr_per.SalaryPlusMinusRows.Count > 0 ||
                    dr_per.SalartSheetRows.Count > 0 ||
                    dr_per.SalartSheetTemplateRows.Count > 0 ||
                    dr_per.TimesheetListRows.Count > 0 ||
                    dr_per.TimeSheetTemplateRows.Count > 0 ||
                    dr_per.UntaxedMinRows.Count > 0)
                {
                    MyMainForm.ShowWarning("Nevar dzēst darbinieku ar datiem citās tabulās.");
                    return false;
                }
            }
            if (bnavNav.BindingSource == bsAmati)
            {
                if (bsAmati.Count == 1)
                {
                    MyMainForm.ShowWarning("Nevar dzēst darbinieka vienīgo amatu.");
                    return false;
                }
                if (bsAmati.Current == null) return false;
                var dr_pos = (bsAmati.Current as ObjectView<A_POSITIONS>)?.Object;
                if (dr_pos.Events.Count > 0 ||
                    dr_pos.PositionsPlusMinusRows.Count > 0)
                {
                    MyMainForm.ShowWarning("Nevar dzēst amatu ar datiem citās tabulās.");
                    return false;
                }

            }
            return true;
        }

        private void bnavNav_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !CanDelete() || !AskCanDelete();
        }

        private void dgvPersons_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!CanDelete() || e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void dgvAmati_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!CanDelete() || e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            DeleteCurrent();
        }

        private void SetSaveButton(bool red)
        {
            bnavNav.SetSaveButton(tsbSave, red);
        }

        public override bool SaveData()
        {
            if (!this.Validate()) return false;
            if (!dgvAmati.EndEditX()) return false;
            if (!dgvPersons.EndEditX()) return false;
            var rt = bsPersons.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
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
            CheckSave();
        }

        private void bsAmati_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void dgvPersons_CurrentCellChanged(object sender, EventArgs e)
        {
            var enabled = !(dgvPersons.CurrentRow == null || dgvPersons.RowCount == 1 ||
                               dgvPersons.CurrentRow.IsNewRow);
            SetControlEnabled(dgvAmati, enabled);
        }

        private void dgvPersons_Enter(object sender, EventArgs e)
        {
            bnavNav.BindingSource = bsPersons;
            bnavNav.DataGrid = dgvPersons;
            tslLabel.Text = "Darbinieki:";
        }

        private void dgvAmati_Enter(object sender, EventArgs e)
        {
            bnavNav.BindingSource = bsAmati;
            bnavNav.DataGrid = dgvAmati;
            tslLabel.Text = "Amati:";
        }

        private void dgvPersons_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.ColumnIndex == dgcUsedDt1.Index || e.ColumnIndex == dgcUsedDt2.Index ||
                e.ColumnIndex == dgcBirthDate.Index)
            {
                if (e.Value == null) return;
                if (!Utils.DGVParseDateCell(e))
                {
                    e.Value = null;
                    e.ParsingApplied = true;
                    return;
                }
            }
        }

        private void dgvAmati_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.ColumnIndex == dgcAmatiUsedDt1.Index || e.ColumnIndex == dgcAmatiUsedDt2.Index)
            {
                if (e.Value == null) return;
                if (!Utils.DGVParseDateCell(e))
                {
                    e.Value = null;
                    e.ParsingApplied = true;
                    return;
                }
            }
        }

        private bool IsLastUsedRow(A_POSITIONS dr)
        {
            foreach (var drv in bsAmati)
            {
                var dr2 = (drv as ObjectView<A_POSITIONS>)?.Object;
                if (dr2 == null || dr2 == dr) continue;
                if (dr2.USED == 1) return false;
            }
            return true;
        }

        private void dgvAmati_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.RowIndex == dgvAmati.NewRowIndex) return;
            if (e.ColumnIndex == dgcAmatiUsed.Index)
            {
                bool val = (bool)e.FormattedValue;
                if (val) return;
                var dr = dgvAmati.GetObjectViewItem<A_POSITIONS>(e.RowIndex);
                if (dr == null) return;
                if (IsLastUsedRow(dr))
                {
                    MyMainForm.ShowWarning("Jābūt vismaz vienam aktīvam amatam.");
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void bsPersons_CurrentChanged(object sender, EventArgs e)
        {

        }
    }
}
