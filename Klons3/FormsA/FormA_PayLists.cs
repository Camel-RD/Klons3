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
using KlonsLIB.Misc;
using KlonsLIB.Components;
using System.Globalization;
using Klons3.ModelsA;
using Equin.ApplicationFramework;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;
using Klons3.ModelsF;

namespace KlonsA.Forms
{
    public partial class FormA_PayLists : MyFormBaseF
    {
        public FormA_PayLists()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            CheckDgvRowsAllowAddRows();

            try
            {
                MakeGrid();
            }
            catch (Exception ex)
            {
                Form_Error.ShowException(ex);
            }
        }

        private void Form_PayLists_Load(object sender, EventArgs e)
        {
            SetupToolStrips();

            ShowPayLists(MyData.Settings.ShowPayLists);
            ShowPayDataPanel(MyData.Settings.ShowPayDataPanel);
            ShowFilterPanel(MyData.Settings.ShowPayListFilterPanel);
            tslPeriod.Text = DataLoaderA.GetPeriodStr();

            MyData.DbContextA.A_PAYLISTS_R.Local.CollectionChanged += PAYLISTS_R_Local_CollectionChanged;

            CheckEnableRows();
            CheckEnableSGR();
            CheckSave();
        }

        private void FormA_PayLists_Shown(object sender, EventArgs e)
        {
            if (bsPersons.Count == 0)
            {
                MyMainForm.ShowWarning("Darbinieku saraksts ir tukšs.");
                Close();
            }
        }

        private void Form_PayLists_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyData.DbContextA.A_PAYLISTS_R.Local.CollectionChanged -= PAYLISTS_R_Local_CollectionChanged;
        }

        private void PAYLISTS_R_Local_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var dr_paylist = (e.NewItems[0] as A_PAYLISTS_R).PayList;
                CheckListTotal(dr_paylist);
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                var dr_paylist = (e.OldItems[0] as A_PAYLISTS_R).PayList;
                CheckListTotal(dr_paylist);
            }
        }

        private void BsLists_ListItemPropertyChanged(object sender, MyItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(A_PAYLISTS.DT))
            {
                var dr = e.Item as A_PAYLISTS;
                var dt = dr.DT;
                dr.YR = dt.Year;
                dr.MT = dt.Month;
            }
        }

        private void BsRows_ListItemPropertyChanged(object sender, MyItemPropertyChangedEventArgs e)
        {
            PAYLISTS_R_ColumnChanged(e);

            var dr = e.Item as A_PAYLISTS;
            if (dr == null) return;
            CheckListTotal(dr);
        }

        private void CheckListTotal(A_PAYLISTS listrow)
        {
            if (listrow == null) return;
            var row_state = MyData.DbContextA.Entry(listrow).State;
            if (row_state == EntityState.Detached ||
                row_state == EntityState.Deleted)
                return;
            var total = listrow.PayListRows.Sum(x => x.TPAY);
            if (listrow.TOTAL_PAY != total) listrow.TOTAL_PAY = total;
        }

        private void SetupToolStrips()
        {
            InsertInToolStrip(toolStrip1, cbLists, 2);
        }

        private void MakeGrid()
        {
            sgrPayRow.MakeGrid();
            sgrPayRow.LinkGrid();
        }

        private bool ignoreColumnChangeEvent = false;
        private bool InPAYLISTS_R_ColumnChanged = false;

        private void PAYLISTS_R_ColumnChanged(MyItemPropertyChangedEventArgs e)
        {
            if (ignoreColumnChangeEvent) return;
            if (InPAYLISTS_R_ColumnChanged) return;
            InPAYLISTS_R_ColumnChanged = true;
            PAYLISTS_R_ColumnChangedA(e);
            InPAYLISTS_R_ColumnChanged = false;
        }

        private void PAYLISTS_R_ColumnChangedA(MyItemPropertyChangedEventArgs e)
        {
            var table = MyData.DbContextA.BL_A_PAYLISTS_R;
            if (e.PropertyName == nameof(A_PAYLISTS_R.Person))
            {
                var dr = e.Item as A_PAYLISTS_R;
                dr.Position = dr.Person.Positions[0];
                dgvRows.RefreshCurrent();
                return;
            }

            /*
            if (e.Column == table.PAY0Column ||
                e.Column == table.ADVANCE0Column)
            {
                var dr = e.Row as KlonsADataSet.PAYLISTS_RRow;
                var d = dr.PAY0 + dr.ADVANCE0;
                if(d != dr.TPAY0)
                {
                    dr.BeginEdit();
                    dr.TPAY0 = d;
                    dr.EndEdit();
                    dgvRows.RefreshCurrent();
                }
                return;
            }
            */

            if (e.PropertyName == nameof(A_PAYLISTS_R.PAY) ||
                e.PropertyName == nameof(A_PAYLISTS_R.ADVANCE) ||
                e.PropertyName == nameof(A_PAYLISTS_R.WITHHOLDINGS))
            {
                var dr = e.Item as A_PAYLISTS_R;
                var pay = dr.PAY;
                bool b1 = false;
                if (dr.PAY > 0.0M && dr.PAY > dr.PAY0)
                {
                    b1 = true;
                    pay = dr.PAY0;
                }
                var d = pay + dr.ADVANCE - dr.WITHHOLDINGS;
                if (d != dr.TPAY || pay != dr.PAY)
                {
                    dr.TPAY = d;
                    dr.PAY = pay;
                    if (b1 || e.PropertyName == nameof(A_PAYLISTS_R.PAY))
                        UpdateListMatch(dr);
                    dgvRows.RefreshCurrent();
                }
                return;
            }

            if (e.PropertyName == nameof(A_PAYLISTS_R.TPAY))
            {
                var dr = e.Item as A_PAYLISTS_R;
                var d = dr.PAY + dr.ADVANCE;
                var AW0 = dr.ADVANCE0 - dr.WITHHOLDINGS0;

                if (dr.TPAY >= dr.TPAY0 || dr.PAY0 < 0.0M)
                {
                    dr.PAY = dr.PAY0;
                }
                else
                {
                    if (AW0 >= 0.0M)
                    {
                        if (dr.TPAY >= 0.0M)
                            dr.PAY = Math.Min(dr.PAY0, dr.TPAY);
                        else
                            dr.PAY = 0.0M;
                    }
                    else
                    {
                        if (dr.TPAY >= 0.0M)
                            dr.PAY = Math.Min(dr.PAY0, dr.TPAY - AW0);
                        else
                            dr.PAY = Math.Min(dr.PAY0, -AW0);
                    }
                }
                var AW = dr.TPAY - dr.PAY;
                if (AW > AW0 && dr.WITHHOLDINGS > 0.0M)
                {
                    dr.WITHHOLDINGS = Math.Max(dr.WITHHOLDINGS0 - (AW - AW0), 0.0M);
                }
                else
                {
                    dr.WITHHOLDINGS = dr.WITHHOLDINGS0;
                }
                dr.ADVANCE = AW + dr.WITHHOLDINGS;


                /*
                dr.IIN = dr.IIN0;
                if (dr.PAY0 < 0.0M)
                {
                    dr.IIN_REVERSE = dr.IIN0;
                    dr.PAY_REVERSE = dr.PAY0;
                }
                else if (dr.PAY0 == 0.0M)
                    dr.IIN = 0.0M;
                else
                    dr.IIN = Math.Round(dr.PAY / dr.PAY0 * dr.IIN0);
                */

                UpdateListMatch(dr);

                dgvRows.RefreshCurrent();

                return;
            }
        }

        public void UpdateListMatch(A_PAYLISTS_R dr)
        {
            /*
            var ad = new KlonsF.DataSets.KlonsARepDataSetTableAdapters.SP_PAY_MATCHLISTSTableAdapter();
            var tab = ad.GetData_SP_REP_PAY_MATCHLISTS_02(dr.IDAM, dr.PAY, dr.PAYLISTSRow.DT);
            var pr = tab[0];
            DataTasks.FillPayRowB(dr, pr);
            */

            var tab2 = DataLoaderA.SP_PAY_MATCHLISTS_1X_GetBy_SP_A_PAY_MATCHLISTS_12(dr.IDAM.Value, dr.PAY, dr.PayList.DT, dr.IDS);
            var pr2 = tab2[0];
            DataTasksA.FillPayRowC(dr, pr2);

        }

        public void DeleteCurrent()
        {
            bNav.DeleteCurrent();
            SaveData();
        }

        private void dgvLists_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void dgvRows_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void bNav_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void dgvLists_Enter(object sender, EventArgs e)
        {
            bNav.BindingSource = bsLists;
            bNav.DataGrid = dgvLists;
            tsLabel1.Text = "Saraksti:";
        }

        private void dgvRows_Enter(object sender, EventArgs e)
        {
            bNav.BindingSource = bsRows;
            bNav.DataGrid = dgvRows;
            tsLabel1.Text = "Rindas:";
        }

        private void dgvLists_KeyDown(object sender, KeyEventArgs e)
        {
            if (dgvLists.CurrentCell == null) return;
            if (e.KeyCode == Keys.Delete && e.Control)
            {
                DeleteCurrent();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                if (!dgvLists.EndEdit()) return;
                dgvLists.MoveToNewRow();
                e.Handled = true;
                return;
            }
        }

        private void dgvRows_KeyDown(object sender, KeyEventArgs e)
        {
            if (dgvRows.CurrentCell == null) return;
            if (e.KeyCode == Keys.Delete && e.Control)
            {
                DeleteCurrent();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                if (!dgvRows.EndEdit()) return;
                dgvRows.MoveToNewRow();
                e.Handled = true;
                return;
            }
        }

        private void dgvLists_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvLists.CurrentRow == null || dgvLists.CurrentRow.IsNewRow ||
                dgvLists.CurrentCell == null) return;
            int colnr = dgvLists.CurrentCell.ColumnIndex;

            if (e.KeyCode == Keys.F5)
            {
                if (colnr != dgcListsDep.Index) return;
                GetIDDepFromDialog();
                e.Handled = true;
                return;
            }
        }

        private void dgvRows_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow ||
                dgvRows.CurrentCell == null) return;
            int colnr = dgvRows.CurrentCell.ColumnIndex;

            if (e.KeyCode == Keys.F5)
            {
                if (colnr != dgcRowsIDP.Index && colnr != dgcRowsIDAM.Index) return;
                GetIDPFromDialog();
                e.Handled = true;
                return;
            }
        }

        private void dgvLists_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvLists.CurrentRow == null || dgvLists.CurrentRow.IsNewRow ||
                dgvLists.CurrentCell == null) return;
            int colnr = dgvLists.CurrentCell.ColumnIndex;
            if (colnr != e.ColumnIndex) return;

            if (colnr == dgcListsDep.Index)
            {
                GetIDDepFromDialog();
            }
        }

        private void dgvRows_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow ||
                dgvRows.CurrentCell == null) return;
            int colnr = dgvRows.CurrentCell.ColumnIndex;
            if (colnr != dgcRowsIDP.Index && colnr != dgcRowsIDAM.Index) return;
            GetIDPFromDialog();
        }

        public void GetIDPFromDialog()
        {
            if (bsRows.Count == 0 || bsRows.Current == null) return;
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            var fm = new FormA_Persons();
            fm.WhatToSelect = FormA_Persons.EWhatToSelect.Both;
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return;
            var dr = dgvRows.GetCurrentObjectViewItem<A_PAYLISTS_R>();
            if (dgvRows.CurrentCell != null)
            {
                try
                {
                    if (dgvRows.CurrentCell.OwningColumn == dgcRowsIDP)
                    {
                        dgvRows.BeginEdit(false);
                        var c = dgvRows.EditingControl as DataGridViewComboBoxEditingControl;
                        c.SelectedValue = fm.SelectedPerson;
                        dgvRows.EndEdit();

                        dr.Position = fm.SelectedPosition;
                    }
                    if (dgvRows.CurrentCell.OwningColumn == dgcRowsIDAM)
                    {
                        dgvRows.EndEdit();

                        dr.Person = fm.SelectedPerson;

                        dgvRows.BeginEdit(false);
                        var c = dgvRows.EditingControl as DataGridViewComboBoxEditingControl;
                        c.SelectedValue = fm.SelectedPosition;
                        dgvRows.EndEdit();
                    }
                }
                catch (Exception) { }
            }
        }

        public void GetIDDepFromDialog()
        {
            if (bsLists.Count == 0 || bsLists.Current == null) return;
            if (dgvLists.CurrentRow == null || dgvLists.CurrentRow.IsNewRow) return;
            var fm = new FormA_Departments();
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return;
            var dr = dgvLists.GetCurrentObjectViewItem<A_PAYLISTS>();
            if (dgvLists.CurrentCell != null)
            {
                try
                {
                    dgvLists.BeginEdit(false);
                    var c = dgvLists.EditingControl as MyMcComboBox;
                    c.SelectedValue = fm.SelectedObject;
                    dgvLists.EndEdit();
                }
                catch (Exception) { }
            }
        }

        private void CheckEnableRows()
        {
            var enabled = dgvLists.RowCount > 0 && dgvLists.CurrentRow != null &&
                dgvLists.CurrentRow != null && !dgvLists.CurrentRow.IsNewRow;
            SetControlEnabled(dgvRows, enabled);
            CheckEnableSGR();
        }

        private void CheckEnableSGR()
        {
            var enabled = dgvRows.Enabled && bsRows.Count > 0 &&
                dgvRows.CurrentRow != null && !dgvRows.CurrentRow.IsNewRow;
            SetControlEnabled(sgrPayRow, enabled);
        }

        private void dgvLists_CurrentCellChanged(object sender, EventArgs e)
        {
            CheckEnableRows();
        }

        private void dgvRows_CurrentCellChanged(object sender, EventArgs e)
        {
            CheckEnableSGR();
        }


        public override bool SaveData()
        {
            if (!dgvLists.EndEditX()) return false;
            if (!dgvRows.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = bsLists.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private void SetSaveButton(bool red)
        {
            bNav.SetSaveButton(tsbSave, red);
        }

        private bool HasChanges()
        {
            return bsLists.HasChanges() || bsRows.HasChanges();
        }

        private void CheckSave()
        {
            SetSaveButton(HasChanges());
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void dgvLists_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void bsLists_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void bsRows_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            if (e.ListChangedType == ListChangedType.Reset) return;
            CheckSave();
        }

        private void dgvRows_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if (e.ColumnIndex == dgcRowsIDAM.Index)
            {
                var val_idp = dgvRows[dgcRowsIDP.Index, e.RowIndex].Value;
                if (val_idp == null || val_idp == DBNull.Value)
                {
                    e.Cancel = true;
                    return;
                }
                var c0 = dgvRows[e.ColumnIndex, e.RowIndex];
                var c = c0 as DataGridViewComboBoxCell;
                if (c == null) return;
                var dr_person = val_idp as A_PERSONS;
                bsAmatiF.SetFilter<A_POSITIONS>(x => x.USED == 1 && x.Person == dr_person);
                c.DataSource = bsAmatiF;
            }
        }

        private void dgvRows_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgcRowsIDAM.Index)
            {
                var c = dgvRows[e.ColumnIndex, e.RowIndex] as DataGridViewComboBoxCell;
                c.DataSource = bsAmati;
            }
        }

        private void dgvLists_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[dgcListsYR.Index].Value = DataLoaderA.LoadedDT2.Year;
            e.Row.Cells[dgcListsMT.Index].Value = DataLoaderA.LoadedDT2.Month;
            e.Row.Cells[dgcListsSNR.Index].Value = DataTasksA.GetNextPayListNr(DataLoaderA.LoadedDT2.Year);
            e.Row.Cells[dgcListsDT.Index].Value = DateTime.Today;
        }

        private void dgvRows_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[dgcRowsSNR.Index].Value = dgvRows.Rows.Count;
        }

        private void dgvLists_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.ColumnIndex == dgcListsDT.Index)
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

        private void AddNew(int yr, int mt, DateTime dt, A_DEPARTMENTS dr_dep, string descr)
        {
            var table_paylists = MyData.DbContextA.BL_A_PAYLISTS;
            var new_dr = table_paylists.NewItem();
            new_dr.YR = yr;
            new_dr.MT = mt;
            new_dr.SNR = DataTasksA.GetNextPayListNr(yr);
            new_dr.DT = dt;
            new_dr.Department = dr_dep;
            new_dr.DESCR = descr.Nz();
            table_paylists.Add(new_dr);
        }

        public void MakeNewList()
        {
            if (MyData.DbContextA.BL_A_PAYLIST_TEMPL.Count == 0)
            {
                MyMainForm.ShowWarning("Nav izveidotas sagataves.");
                return;
            }

            string er = "OK";

            var fm = new FormA_PayListsNew();
            if (fm.ShowDialog(MyMainForm) != DialogResult.OK) return;

            ignoreColumnChangeEvent = true;

            if (fm.MakeEmpty)
            {
                AddNew(fm.Year, fm.Month, fm.PayDate, fm.Department, fm.Descr);
            }
            else if (fm.MakeAll)
            {
                er = DataTasksA.MakePayListsFromTempl(fm.PayDate, fm.Year, fm.Month, true);
            }
            else
            {
                var dr_t = fm.PayListTemplate;
                er = DataTasksA.MakePayListFromTempl(fm.PayDate, dr_t, fm.Year, fm.Month, true);
            }

            if (er != "OK")
            {
                MyMainForm.ShowWarning(er);
            }

            ignoreColumnChangeEvent = false;

            SaveData();
        }

        public bool CheckPayListIDs(A_PAYLISTS dr_paylist)
        {
            var err = DataTasksA.CheckPayListIDs(dr_paylist);
            if (err.HasErrors)
            {
                Form_ErrorList.ShowErrorList(MyMainForm, err);
                return false;
            }
            return true;
        }

        public void RecalcListA(bool fullrecalc)
        {
            if (bsLists.Count == 0 || bsLists.Current == null) return;
            if (!SaveBeforeProceed()) return;
            var dr = bsLists.GetCurrentItem<A_PAYLISTS>();

            if (!CheckPayListIDs(dr)) return;

            ignoreColumnChangeEvent = true;
            DataTasksA.FillPayListA(dr, fullrecalc);
            if (!SaveData())
            {
                ignoreColumnChangeEvent = false;
                return;
            }
            DataTasksA.FillPayListB(dr);
            ignoreColumnChangeEvent = false;
            dgvLists.RefreshCurrent();
            SaveData();
        }

        public void RecalcRowA(bool fullrecalc)
        {
            if (bsRows.Count == 0 || bsRows.Current == null) return;
            if (!SaveBeforeProceed()) return;
            var dr = (bsRows.Current as ObjectView<A_PAYLISTS_R>).Object;

            if (!CheckPayListIDs(dr.PayList)) return;

            ignoreColumnChangeEvent = true;
            DataTasksA.FillPayListRowA(dr, fullrecalc);
            if (!SaveData())
            {
                ignoreColumnChangeEvent = false;
                return;
            }
            DataTasksA.FillPayListRowB(dr);
            ignoreColumnChangeEvent = false;
            dgvRows.RefreshCurrent();
            SaveData();
        }

        public void RecalcListB()
        {
            if (bsLists.Count == 0 || bsLists.Current == null) return;
            if (!SaveBeforeProceed()) return;
            var dr = bsLists.GetCurrentItem<A_PAYLISTS>();

            if (!CheckPayListIDs(dr)) return;

            ignoreColumnChangeEvent = true;
            DataTasksA.FillPayListB(dr);
            ignoreColumnChangeEvent = false;
            dgvLists.RefreshCurrent();
            SaveData();
        }

        public void RecalcRowB()
        {
            if (bsRows.Count == 0 || bsRows.Current == null) return;
            if (!SaveBeforeProceed()) return;
            var dr = (bsRows.Current as ObjectView<A_PAYLISTS_R>).Object;

            if (!CheckPayListIDs(dr.PayList)) return;

            ignoreColumnChangeEvent = true;
            DataTasksA.FillPayListRowB(dr);
            ignoreColumnChangeEvent = false;
            dgvRows.RefreshCurrent();
            SaveData();
        }

        public void RecalcAllLists()
        {
            if (bsLists.Count == 0) return;
            if (!SaveBeforeProceed()) return;
            foreach (var item in bsLists)
            {
                var dr = (item as ObjectView<A_PAYLISTS>).Object;
                ignoreColumnChangeEvent = true;
                DataTasksA.FillPayListA(dr, false);
                if (!SaveData())
                {
                    ignoreColumnChangeEvent = false;
                    return;
                }
                DataTasksA.FillPayListB(dr);
                ignoreColumnChangeEvent = false;
            }

            dgvLists.Refresh();
            SaveData();
        }

        public void RecalcAskWhat()
        {
            if (bsLists.Count == 0) return;
            if (!SaveBeforeProceed()) return;

            bool rt = FormA_PayListsRecalcParams.ShowDialog(out var datefrom, out var dr_person);
            if (!rt) return;

            foreach (var item in bsLists)
            {
                var dr_paylist = (item as ObjectView<A_PAYLISTS>).Object;
                if (dr_paylist.DT < datefrom) continue;

                if (dr_person == null)
                {
                    ignoreColumnChangeEvent = true;
                    DataTasksA.FillPayListA(dr_paylist, false);
                    if (!SaveData())
                    {
                        ignoreColumnChangeEvent = false;
                        return;
                    }
                    DataTasksA.FillPayListB(dr_paylist);
                    ignoreColumnChangeEvent = false;
                }
                else
                {
                    var drs_paylistrow = dr_paylist.PayListRows;
                    foreach (var dr_paylistrow in drs_paylistrow)
                    {
                        if (dr_paylistrow.Person != dr_person) continue;
                        ignoreColumnChangeEvent = true;
                        DataTasksA.FillPayListRowA(dr_paylistrow, false);
                        if (!SaveData())
                        {
                            ignoreColumnChangeEvent = false;
                            return;
                        }
                        DataTasksA.FillPayListRowB(dr_paylistrow);
                        ignoreColumnChangeEvent = false;

                    }
                }
            }

            dgvLists.Refresh();
            SaveData();
        }

        private void myMcFlatComboBox1_Format(object sender, ListControlConvertEventArgs e)
        {
            var drv = e.ListItem as ObjectView<A_PAYLISTS>;
            if (drv == null)
            {
                e.Value = "?";
                return;
            }
            var dr = drv.Object;
            var dr_state = MyData.DbContextA.Entry(dr).State;
            if (dr_state == EntityState.Detached || dr_state == EntityState.Deleted)
            {
                e.Value = "?";
                return;
            }
            var s = $"{dr.DT:dd.MM.yyyy} {dr.SNR} {dr.DESCR}";
            e.Value = s;
        }

        public void ShowPayLists(bool b)
        {
            mySplitContainer1.Panel1Collapsed = !b;
            cbLists.Visible = !b;
            MyData.Settings.ShowPayLists = b;
            rādītPaslēptSarakstuTabuluToolStripMenuItem.Checked = b;
        }

        public void ShowPayDataPanel(bool b)
        {
            sgrPayRow.Visible = b;
            MyData.Settings.ShowPayDataPanel = b;
            rādītPaslēptDatuPaneliToolStripMenuItem.Checked = b;
        }

        public void ShowFilterPanel(bool b)
        {
            plFilter.Visible = b;
            MyData.Settings.ShowPayListFilterPanel = b;
            rādītPaslēptFiltraPaneliToolStripMenuItem.Checked = b;
        }

        public void DoFilter()
        {
            DateTime dt1 = DateTime.MinValue;
            DateTime dt2 = DateTime.MaxValue;
            if (!string.IsNullOrEmpty(tbDate1.Text))
                dt1 = Utils.StringToDate(tbDate1.Text).Value;
            if (!string.IsNullOrEmpty(tbDate2.Text))
                dt2 = Utils.StringToDate(tbDate2.Text).Value;
            A_DEPARTMENTS dr_dep = null;
            if (cbDep.SelectedIndex > -1 && cbDep.SelectedValue != null)
                dr_dep = (A_DEPARTMENTS)cbDep.SelectedValue;
            if (dt1 == DateTime.MinValue && dt2 == DateTime.MaxValue && dr_dep == null)
            {
                bsLists.RemoveFilter();
                return;
            }
            var pred_list = new List<Predicate<A_PAYLISTS>>();
            string fs = "";
            string sdt1 = dt1.ToString("d", CultureInfo.InvariantCulture);
            string sdt2 = dt2.ToString("d", CultureInfo.InvariantCulture);
            if (dt1 != DateTime.MinValue && dt2 != DateTime.MaxValue)
                pred_list.Add(x => x.DT >= dt1 && x.DT <= dt2);
            else if (dt1 != DateTime.MinValue)
                pred_list.Add(x => x.DT >= dt1);
            else
                pred_list.Add(x => x.DT <= dt2);
            if (dr_dep != null)
            {
                pred_list.Add(x => x.Department == dr_dep);
            }
            bsLists.SetFilter(pred_list);
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            DeleteCurrent();
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            if (bNav.BindingSource == bsRows)
            {
                dgvRows.MoveToNewRow();
                return;
            }
            else
            {
                MakeNewList();
            }
        }

        private void dgvRows_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 && e.ColumnIndex < 0) return;
            if (e.ColumnIndex == dgcRowsR1.Index || e.ColumnIndex == dgcRowsR2.Index)
            {
                if (e.Value == null) return;
                float f = (float)e.Value * 100.0f;
                e.Value = f.ToString("0.00");
                e.FormattingApplied = true;
            }
        }

        private void sgrPayRow_ConvertingValueToDisplayString(object sender, DevAge.ComponentModel.ConvertingObjectEventArgs e)
        {
            if (bsRows.Count == 0 || bsRows.Current == null ||
                dgvRows.CurrentRow != null && dgvRows.CurrentRow.IsNewRow)
            {
                e.Value = "";
                e.ConvertingStatus = DevAge.ComponentModel.ConvertingStatus.Completed;
                return;
            }
            if (sender == grRowFName)
            {
                int idp = (int)e.Value;
                var ss = DataTasksA.GetPersonNameAndPK(idp);
                e.Value = ss == null ? null : ss[0];
                e.ConvertingStatus = DevAge.ComponentModel.ConvertingStatus.Completed;
                return;
            }
            if (sender == grRowLName)
            {
                int idp = (int)e.Value;
                var ss = DataTasksA.GetPersonNameAndPK(idp);
                e.Value = ss == null ? null : ss[1];
                e.ConvertingStatus = DevAge.ComponentModel.ConvertingStatus.Completed;
                return;
            }
            if (sender == grRowPosition)
            {
                int idam = (int)e.Value;
                e.Value = DataTasksA.GetPositionTitle(idam);
                e.ConvertingStatus = DevAge.ComponentModel.ConvertingStatus.Completed;
                return;
            }

        }

        private Report_PayList FillReport()
        {
            if (bsLists.Count == 0 || bsLists.Current == null) return null;
            if (bsRows.Count == 0) return null;
            if (dgvLists.Rows.Count == 0 || dgvLists.CurrentRow == null) return null;

            var drs_list = new List<A_PAYLISTS>();
            if (dgvLists.SelectedRows.Count == 0)
            {
                drs_list.Add(bsLists.GetCurrentItem<A_PAYLISTS>());
            }
            else
            {
                for (int i = 0; i < dgvLists.SelectedRows.Count; i++)
                {
                    if (dgvLists.SelectedRows[i].IsNewRow) continue;
                    drs_list.Add((dgvLists.SelectedRows[i].DataBoundItem as ObjectView<A_PAYLISTS>).Object);
                }
            }

            drs_list = drs_list
                .OrderBy(d => d.SNR)
                .ToList();

            var rr = new Report_PayList();

            for (int i = 0; i < drs_list.Count; i++)
            {
                var dr_list = drs_list[i];
                var drs_rows = dr_list.PayListRows
                    .Where(d => d.TPAY > 0.0M)
                    .OrderBy(d => d.SNR)
                    .ToArray();
                rr.AddToReport(dr_list, drs_rows);
            }

            return rr;
        }

        private void ShowReport1()
        {
            var rr = FillReport();
            if (rr == null) return;
            rr.ShowReport1();
        }

        private void ShowReport2()
        {
            var rr = FillReport();
            if (rr == null) return;
            rr.ShowReport2();
        }

        private void ShowReport3()
        {
            var rr = FillReport();
            if (rr == null) return;
            rr.GroupWithoutPositions();
            rr.ShowReport1();
        }

        private void ShowReport4()
        {
            var rr = FillReport();
            if (rr == null) return;
            rr.GroupWithoutPositions();
            rr.ShowReport2();
        }

        private void RenumRows()
        {
            for (int i = 0; i < bsRows.Count; i++)
            {
                var dr = (bsRows[i] as ObjectView<A_PAYLISTS_R>).Object;
                if (dr.SNR != (short)(i + 1)) dr.SNR = (short)(i + 1);
            }
        }

        void RepSplitPay()
        {
            if (!SaveData()) return;
            if (bsRows.Current == null || dgvRows.CurrentRow == null ||
                dgvRows.CurrentRow.IsNewRow) return;

            var dr = (bsRows.Current as ObjectView<A_PAYLISTS_R>).Object;

            var tab2 = DataLoaderA.SP_PAY_MATCHLISTS_1X_GetBy_SP_A_PAY_MATCHLISTS_12(dr.IDAM.Value, dr.PAY, dr.PayList.DT, dr.IDS);
            var dr2 = tab2[0];
            int idam = dr.IDAM.Value;
            var dt = dr.PayList.DT;

            var ci = new PayListCalcInfo(true);
            var ret = ci.Calc(dr, dr2);
            if (ret != "OK")
            {
                MyMainForm.ShowWarning(ret);
                return;
            }
            FormA_PayCalc.Show(ci, FormA_PayCalc.EReportType.Splitpay);
        }

        void RepShList()
        {
            if (!SaveData()) return;
            if (bsRows.Current == null || dgvRows.CurrentRow == null ||
                dgvRows.CurrentRow.IsNewRow) return;

            var dr = (bsRows.Current as ObjectView<A_PAYLISTS_R>).Object;

            var ci = new PayListCalcInfo(true);
            ci.MakeReportB(dr);
            FormA_PayCalc.Show(ci, FormA_PayCalc.EReportType.SalarySheetList);
        }


        private void izveidotJaunuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeNewList();
        }

        private void pārrēķinātSarakstuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RecalcListA(true);
        }

        private void pārrēķinātDarbiniekamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RecalcRowA(true);
        }

        private void pārrēķinātSarakstuNemainotMaksājumuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RecalcListA(false);
        }

        private void pārrēķinātDarbiniekamNemainotMaksājumuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RecalcRowA(false);
        }

        private void PārrēķinātAtlasītosSarakstusNemainotMaksājumuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RecalcAskWhat();
        }

        private void miReport1_Click(object sender, EventArgs e)
        {
            ShowReport1();
        }

        private void miReport2_Click(object sender, EventArgs e)
        {
            ShowReport2();
        }

        private void miReport3_Click(object sender, EventArgs e)
        {
            ShowReport3();
        }

        private void miReport4_Click(object sender, EventArgs e)
        {
            ShowReport4();
        }


        private void miRepShList_Click(object sender, EventArgs e)
        {
            RepShList();
        }

        private void miRepSplitPay_Click(object sender, EventArgs e)
        {
            RepSplitPay();
        }

        private void rādītPaslēptSarakstuTabuluToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowPayLists(mySplitContainer1.Panel1Collapsed);
        }

        private void rādītPaslēptDatuPaneliToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowPayDataPanel(!sgrPayRow.Visible);
        }

        private void rādītPaslēptFiltraPaneliToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFilterPanel(!plFilter.Visible);
        }

        private void tsbRenum_Click(object sender, EventArgs e)
        {
            RenumRows();
        }

        private void cmFilter_Click(object sender, EventArgs e)
        {
            DoFilter();
        }

        private void sarakstaImportsNoTekstaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fm = new FormA_PayListImport();
            if (fm.ShowDialog(MyMainForm) != DialogResult.OK) return;
            var rows = fm.ListRows;
            var err = DataTasksA.MakePayListFromImportData(rows);
            if (err.HasErrors)
            {
                Form_ErrorList.ShowErrorList(MyMainForm, err);
                return;
            }
        }

        private void CheckDgvRowsAllowAddRows()
        {
            dgvRows.AllowUserToAddRows = MyData.DbContextA.BL_A_PERSONS.Count > 0;
        }

        private void bsPersons_ListChanged(object sender, ListChangedEventArgs e)
        {
            CheckDgvRowsAllowAddRows();
        }

    }
}
