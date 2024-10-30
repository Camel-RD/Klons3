using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using KlonsP.Classes;
using KlonsF.Classes;
using KlonsLIB.Forms;
using KlonsLIB.Components;
using KlonsLIB.Misc;
using KlonsLIB.MySourceGrid.GridRows;
using KlonsLIB.Data;
using Microsoft.Reporting.WinForms;
using Klons3.ModelsP;
using Equin.ApplicationFramework;
using Microsoft.EntityFrameworkCore;

namespace KlonsP.Forms
{
    public partial class FormP_Items : MyFormBaseF
    {
        public FormP_Items()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            try
            {
                MakeGrid();
            }
            catch (Exception ex)
            {
                Form_Error.ShowException(ex);
            }
            bsRows.CurrentChanged += bsRows_CurrentChanged;

            ShowDataPanel(MyData.Params.ShowItemDataPanel);
            ShowFilterPanel(MyData.Params.ShowItemsFilterPanel);

            dgvFilter.AutoGenerateColumns = false;
            dgcFilterSate.DataSource = SomeDataDefsP.FilterList;
            dgcFilterSate.ValueMember = "Key";
            dgcFilterSate.DisplayMember = "Val";

            LoadColumnWidthsFromSettings();
        }

        public DateTime CurrentDate { get; private set; } = DateTime.MinValue;

        private void Form_Items_Load(object sender, EventArgs e)
        {
            InsertInToolStrip(bnNav, tbDate, -1);

            bsEvents.ListItemPropertyChanged += BsEvents_ListItemPropertyChanged;

            mySplitContainer1.Panel1.Select();
            dgvItems.Select();

            IsLoading = false;
            SetDate(MyData.Params.ActiveDate);
            CheckSave();
        }

        private void Form_Items_FormClosed(object sender, FormClosedEventArgs e)
        {
            bsEvents.ListItemPropertyChanged -= BsEvents_ListItemPropertyChanged;
            bsRows.CurrentChanged -= bsRows_CurrentChanged;
        }

        private void MakeGrid()
        {
            sgrEvents.MakeGrid();
            sgrEvents.LinkGrid();
        }

        private void LoadColumnWidthsFromSettings()
        {
            string scw = MyData.Settings.ColumnWidths_PItems;
            (int ver, int[] cw) = dgvItems.ParseColumnWidths(scw);
            if (ver == 1 && cw != null && cw.Length > 0)
                dgvItems.SetColumnWidths(cw);

            scw = MyData.Settings.ColumnWidths_PEvents;
            (ver, cw) = dgvEvents.ParseColumnWidths(scw);
            if (ver == 1 && cw != null && cw.Length > 0)
                dgvEvents.SetColumnWidths(cw);
        }

        private void SaveColumnWidthsToSettings()
        {
            MyData.Settings.ColumnWidths_PItems = dgvItems.GetColumnWidths2(10.0f, 1);
            MyData.Settings.ColumnWidths_PEvents = dgvEvents.GetColumnWidths2(10.0f, 1);
        }


        public override void SaveParams()
        {
            MyData.Params.ActiveDate = CurrentDate;
            SaveColumnWidthsToSettings();
        }

        public void DeleteCurrent()
        {
            bnNav.DeleteCurrent();
            if (bnNav.BindingSource == bsRows)
            {
                RecalcCurrent();
            }
            else if (bnNav.BindingSource == bsItems)
            {

            }
            SaveData();
        }


        private int Ignore_ITEMS_EVENTS_ColumnChanged = 0;

        private void BsEvents_ListItemPropertyChanged(object sender, MyItemPropertyChangedEventArgs e)
        {
            if (Ignore_ITEMS_EVENTS_ColumnChanged > 0) return;
            Ignore_ITEMS_EVENTS_ColumnChanged++;
            BsEvents_ListItemPropertyChangedA(sender, e);
            Ignore_ITEMS_EVENTS_ColumnChanged--;
        }

        private void BsEvents_ListItemPropertyChangedA(object sender, MyItemPropertyChangedEventArgs e)
        {
            var dr = e.Item as P_ITEMS_EVENTS;

            if (e.PropertyName == nameof(P_ITEMS_EVENTS.VALUE_C))
            {
                dr.TAX_VAL_C = dr.VALUE_C;
            }

            if (e.PropertyName == nameof(P_ITEMS_EVENTS.DT) ||
                e.PropertyName == nameof(P_ITEMS_EVENTS.EVENT) ||
                e.PropertyName == nameof(P_ITEMS_EVENTS.CATD) ||
                e.PropertyName == nameof(P_ITEMS_EVENTS.MT_TOTAL) ||
                e.PropertyName == nameof(P_ITEMS_EVENTS.VALUE_0) ||
                e.PropertyName == nameof(P_ITEMS_EVENTS.VALUE_C) ||
                e.PropertyName == nameof(P_ITEMS_EVENTS.DEPREC_0) ||
                e.PropertyName == nameof(P_ITEMS_EVENTS.DEPREC_C) ||
                e.PropertyName == nameof(P_ITEMS_EVENTS.SELL_VALUE) ||
                e.PropertyName == nameof(P_ITEMS_EVENTS.TAX_VAL) ||
                e.PropertyName == nameof(P_ITEMS_EVENTS.TAX_VAL_LEFT))
            {
                RecalcCurrent();
                dgvEvents.RefreshCurrent();
            }

        }

        public int FindItem(int id)
        {
            for (int i = 0; i < bsItems.Count; i++)
            {
                var dr = (bsItems[i] as ObjectView<P_ITEMS>)?.Object;
                if (dr.ID == id) return i;
            }
            return -1;
        }

        public void SelectItem(int id)
        {
            int pos = FindItem(id);
            if (pos == -1) return;
            bsItems.Position = pos;
        }


        private P_ITEMS_EVENTS GetPrevRow(P_ITEMS_EVENTS dr)
        {
            var drs = bsRows.Cast<ObjectView<P_ITEMS_EVENTS>>()
                .Select(d => d.Object)
                .OrderBy(d => d.DT)
                .ThenBy(d => d.EVENTRow.SNR)
                .ToArray();
            int k = drs.IndexOf(dr);
            if (k < 1) return null;
            return drs[k - 1];
        }

        private P_ITEMS_EVENTS GetLastRow(P_ITEMS_EVENTS dr_excl)
        {
            var dr = bsRows.Cast<ObjectView<P_ITEMS_EVENTS>>()
                .Select(d => d.Object)
                .Where(d => d != dr_excl)
                .OrderBy(d => d.DT)
                .ThenBy(d => d.EVENTRow.SNR)
                .LastOrDefault();
            return dr;
        }

        private void CheckChangedColumns(P_ITEMS_EVENTS dr_cur, P_ITEMS_EVENTS dr_prev)
        {
            EChColInd ch = EChColInd.none;
            if(dr_prev == null)
            {
                if (dr_cur.VALUE_0 != 0.0M) ch |= EChColInd.value0;
                if (dr_cur.DEPREC_0 != 0.0M) ch |= EChColInd.deprec0;
                if (dr_cur.VALUE_C != 0.0M) ch |= EChColInd.valuec;
                if (dr_cur.DEPREC_C != 0.0M) ch |= EChColInd.deprecc;
                if (dr_cur.TAX_VAL != 0.0M) ch |= EChColInd.taxvalue;
                if (dr_cur.TAX_VAL_C != 0.0M) ch |= EChColInd.taxvaluec;
                dr_cur.XChColSet = ch;
                return;
            }
            if (dr_cur.CAT1 != dr_prev.CAT1) ch |= EChColInd.cat1;
            if (dr_cur.CATD != dr_prev.CATD) ch |= EChColInd.catd;
            if (dr_cur.CATT != dr_prev.CATT) ch |= EChColInd.catt;
            if (dr_cur.PLACE != dr_prev.PLACE) ch |= EChColInd.place;
            if (dr_cur.DEPARTMENT != dr_prev.DEPARTMENT) ch |= EChColInd.department;
            if (dr_cur.VALUE_C != 0.0M) ch |= EChColInd.valuec;
            if (dr_cur.DEPREC_C != 0.0M) ch |= EChColInd.deprecc;
            if (dr_cur.SELL_VALUE != dr_prev.SELL_VALUE) ch |= EChColInd.sellvalue;
            if (dr_cur.MT_TOTAL != dr_prev.MT_TOTAL) ch |= EChColInd.mttotal;
            if (dr_cur.TAX_VAL != dr_prev.MT_TOTAL) ch |= EChColInd.taxvalue;
            if (dr_cur.TAX_VAL_C != 0.0M) ch |= EChColInd.taxvaluec;
            dr_cur.XChColSet = ch;
        }


        private void dgvItems_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void dgvEvents_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void bnNav_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void dgvItems_Enter(object sender, EventArgs e)
        {
            bnNav.BindingSource = bsItems;
            bnNav.DataGrid = dgvItems;
            tslActiveTable.Text = "Pamatlīdzekļi:";
        }

        private void dgvEvents_Enter(object sender, EventArgs e)
        {
            bnNav.BindingSource = bsRows;
            bnNav.DataGrid = dgvEvents;
            tslActiveTable.Text = "Notikumi:";
        }

        private void dgvItems_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvItems.CurrentCell == null) return;
            if (e.KeyCode == Keys.Delete && e.Control)
            {
                DeleteCurrent();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                if (!dgvItems.EndEdit()) return;
                dgvItems.MoveToNewRow();
                e.Handled = true;
                return;
            }
        }

        private void dgvEvents_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvEvents.CurrentCell == null) return;
            if (e.KeyCode == Keys.Delete && e.Control)
            {
                DeleteCurrent();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                if (!dgvEvents.EndEdit()) return;
                dgvEvents.MoveToNewRow();
                e.Handled = true;
                return;
            }

            if (e.KeyCode == Keys.F5)
            {
                if(SetEventsCellFromDialog())
                    e.Handled = true;
                return;
            }

        }

        private void dgvEvents_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SetEventsCellFromDialog();
        }

        private bool SetEventsCellFromDialog()
        {
            if (bsEvents.Count == 0 || bsEvents.Current == null) return false;
            if (dgvEvents.CurrentRow == null || dgvEvents.CurrentRow.IsNewRow ||
                dgvEvents.CurrentCell == null) return false;
            int colnr = dgvEvents.CurrentCell.ColumnIndex;

            //var dr = (dgvEvents.CurrentRow.DataBoundItem as DataRowView).Row as KlonsPDataSet.ITEMS_EVENTSRow;
            object dr_selected = null;

            if (colnr == dgcEventsCat1.Index)
            {
                if (!GetCat1FromDialog(out var dr)) return false;
                dr_selected = dr;
            }
            else if (colnr == dgcEventsCatD.Index)
            {
                if (!GetCatDFromDialog(out var dr)) return false;
                dr_selected = dr;
            }
            else if (colnr == dgcEventsCatT.Index)
            {
                if (!GetCatTFromDialog(out var dr)) return false;
                dr_selected = dr;
            }
            else if (colnr == dgcEventsDepartment.Index)
            {
                if (!GetDepFromDialog(out var dr)) return false;
                dr_selected = dr;
            }
            else if (colnr == dgcEventsPlace.Index)
            {
                if (!GetPlaceFromDialog(out var dr)) return false;
                dr_selected = dr;
            }

            if (dr_selected == null) return false;

            try
            {
                dgvEvents.BeginEdit(false);
                var c = dgvEvents.EditingControl as MyDgvMcComboBox;
                c.SelectedValue = dr_selected;
                dgvEvents.EndEdit();
            }
            catch (Exception) { }

            return true;
        }

        private void dgvFilter_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SetFilterCellFromDialog();
        }

        private bool SetFilterCellFromDialog()
        {
            if (bsFilter.Count == 0 || bsFilter.Current == null) return false;
            if (dgvFilter.CurrentRow == null || dgvFilter.CurrentRow.IsNewRow ||
                dgvFilter.CurrentCell == null) return false;
            int colnr = dgvFilter.CurrentCell.ColumnIndex;

            //var dr = (dgvEvents.CurrentRow.DataBoundItem as DataRowView).Row as KlonsPDataSet.ITEMS_EVENTSRow;
            int? id = null;

            if (colnr == dgcFilterCat1.Index)
            {
                if (!GetCat1FromDialog(out var dr)) return false;
                id = dr.ID;
            }
            else if (colnr == dgcFilterCatD.Index)
            {
                if (!GetCatDFromDialog(out var dr)) return false;
                id = dr.ID;
            }
            else if (colnr == dgcFilterCatT.Index)
            {
                if (!GetCatTFromDialog(out var dr)) return false;
                id = dr.ID;
            }
            else if (colnr == dgcFilterDepartment.Index)
            {
                if (!GetDepFromDialog(out var dr)) return false;
                id = dr.ID;
            }
            else if (colnr == dgcFilterPlace.Index)
            {
                if (!GetPlaceFromDialog(out var dr)) return false;
                id = dr.ID;
            }

            if (id == null || id == 0) return false;

            try
            {
                dgvFilter.BeginEdit(false);
                var c = dgvFilter.EditingControl as MyDgvMcComboBox;
                c.SelectedValue = id.Value;
                dgvFilter.EndEdit();
            }
            catch (Exception) { }

            return true;
        }


        public bool GetCat1FromDialog(out P_CAT1 dr_cat1)
        {
            dr_cat1 = null;
            var fm = new FormP_Cat1();
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return false;
            dr_cat1 = (P_CAT1)fm.SelectedObject;
            return true;
        }

        public bool GetCatDFromDialog(out P_CATD dr_catd)
        {
            dr_catd = null;
            var fm = new FormP_CatD();
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return false;
            dr_catd = (P_CATD)fm.SelectedObject;
            return true;
        }

        public bool GetCatTFromDialog(out P_CATT dr_catt)
        {
            dr_catt = null;
            var fm = new FormP_CatT();
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return false;
            dr_catt = (P_CATT)fm.SelectedObject;
            return true;
        }

        public bool GetDepFromDialog(out P_DEPARTMENTS dr_dep)
        {
            dr_dep = null;
            var fm = new FormP_Departments();
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return false;
            dr_dep = (P_DEPARTMENTS)fm.SelectedObject;
            return true;
        }

        public bool GetPlaceFromDialog(out P_PLACES dr_place)
        {
            dr_place = null;
            var fm = new FormP_Places();
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return false;
            dr_place = (P_PLACES)fm.SelectedObject;
            return true;
        }

        private void dgvItems_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if(e.ColumnIndex == dgcItemsDate1.Index || e.ColumnIndex == dgcItemsDate2.Index)
            {
                Utils.DGVParseDateCell(e);
            }
        }

        private void dgvEvents_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.ColumnIndex == dgcEventsDT.Index || e.ColumnIndex == dgcEventsDtReg.Index)
            {
                Utils.DGVParseDateCell(e);
            }
        }

        private void CheckEnableEvents()
        {
            dgvEvents.Enabled = dgvItems.RowCount > 0 && dgvItems.CurrentRow != null &&
                !dgvItems.CurrentRow.IsNewRow;

            CheckEnableSGR();
        }

        private void CheckEnableSGR()
        {
            sgrEvents.Enabled = dgvEvents.Enabled && bsRows.Count > 0 &&
                dgvEvents.CurrentRow != null && !dgvEvents.CurrentRow.IsNewRow;
        }

        private void dgvItems_CurrentCellChanged(object sender, EventArgs e)
        {
            CheckEnableEvents();
        }

        private void dgvEvents_CurrentCellChanged(object sender, EventArgs e)
        {
            CheckEnableSGR();
        }

        public override bool SaveData()
        {
            if (!dgvItems.EndEditX()) return false;
            if (!dgvEvents.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = bsItems.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private void SetSaveButton(bool red)
        {
            bnNav.SetSaveButtonRed(red);
        }

        private bool HasChanges()
        {
            return MyData.DbContextP.HasChanges();
        }

        private void CheckSave()
        {
            SetSaveButton(HasChanges());
        }

        private void bnNav_SaveClicked(object sender, EventArgs e)
        {
            SaveData();
        }

        private void dgvItems_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            SaveData();
        }

        private void bsItems_ListChanged(object sender, ListChangedEventArgs e)
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

        private void SetItemEventRowFrom(P_ITEMS_EVENTS dr, P_ITEMS_EVENTS dr_from)
        {
            dr.CAT1 = dr_from.CAT1;
            dr.CATD = dr_from.CATD;
            dr.CATT = dr_from.CATT;
            dr.PLACE = dr_from.PLACE;
            dr.DEPARTMENT = dr_from.DEPARTMENT;

            dr.VALUE_0 = dr_from.VALUE_0;
            dr.DEPREC_0 = dr_from.DEPREC_0;
            dr.SELL_VALUE = dr_from.SELL_VALUE;
            dr.RATE_D = dr_from.RATE_D;
            dr.RATE_D_MT = dr_from.RATE_D_MT;

            dr.MT_TOTAL = dr_from.MT_TOTAL;

            dr.TAX_VAL = dr_from.TAX_VAL;
            dr.TAX_VAL = dr_from.TAX_VAL;
            dr.TAX_RATE = dr_from.TAX_RATE;
            dr.TAX_EACH = dr_from.TAX_EACH;
        }

        private void SetItemEventRowFrom(DataGridViewRow dr, P_ITEMS_EVENTS dr_from)
        {
            dr.Cells[dgcEventsCat1.Index].Value = dr_from.CAT1;
            dr.Cells[dgcEventsCatD.Index].Value = dr_from.CATD;
            dr.Cells[dgcEventsCatT.Index].Value = dr_from.CATT;
            dr.Cells[dgcEventsPlace.Index].Value = dr_from.PLACE;
            dr.Cells[dgcEventsDepartment.Index].Value = dr_from.DEPARTMENT;

            dr.Cells[dgcEventsValue0.Index].Value = dr_from.VALUE_0 + dr_from.VALUE_C;
            dr.Cells[dgcEventsDeprec0.Index].Value = dr_from.DEPREC_0 + dr_from.DEPREC_C;
            dr.Cells[dgcEventsSellValue.Index].Value = dr_from.SELL_VALUE;
            dr.Cells[dgcEventsRateD.Index].Value = dr_from.RATE_D;
            dr.Cells[dgcEventsRateDMt.Index].Value = dr_from.RATE_D_MT;

            dr.Cells[dgcEventsMtTotal.Index].Value = dr_from.MT_TOTAL;

            dr.Cells[dgcEventsTaxVal.Index].Value = dr_from.TAX_VAL + dr_from.TAX_VAL_C;
            dr.Cells[dgcEventsTaxRate.Index].Value = dr_from.TAX_RATE;
            dr.Cells[dgcEventsTaxEach.Index].Value = dr_from.TAX_EACH;
        }

        private void dgvEvents_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[dgcEventsDT.Index].Value = DateTime.Today;
            e.Row.Cells[dgcEventsDtReg.Index].Value = DateTime.Today;
            e.Row.Cells[dgcEventsZDT.Index].Value = DateTime.Now;
            e.Row.Cells[dgcEventsZU.Index].Value = MyData.CurrentUserName;
            if (bsRows.Count == 0) return;
            P_ITEMS_EVENTS dr = null;
            try { dr = (e.Row.DataBoundItem as ObjectView<P_ITEMS_EVENTS>)?.Object; }
            catch (Exception) { }
            var dr_last = GetLastRow(dr);
            if (dr_last == null) return;
            e.Row.Cells[dgcEventsSNR.Index].Value = dr_last.SNR + 1;
            SetItemEventRowFrom(e.Row, dr_last);
        }

        public void RecalcCurrent(P_ITEMS_EVENTS drtoinit = null)
        {
            if (bsItems.Count == 0 || bsItems.Current == null) return;
            if (dgvItems.CurrentRow == null || dgvItems.CurrentRow.IsNewRow) return;
            var dr = bsItems.GetCurrentItem<P_ITEMS>();
            var rt = RecalcItem(dr, drtoinit);
            if(rt != "OK")
            {
                MyMainForm.ShowWarning(rt);
                return;
            }
        }

        public void RecalcAll()
        {
            if (bsItems.Count == 0) return;
            var drs_items = MyData.DbContextP.P_ITEMS.Local.ToList();
            int k = 0;
            foreach (var dr in drs_items)
            {
                var rt = RecalcItem(dr);
                if (rt != "OK") k++;
            }
            if (k > 0)
            {
                MyMainForm.ShowWarning(
                    "Pārrēķinot datus, tika konstatētas kļūdas.\n" +
                    "Pamatlīdzekļu skaits ar kļūdainiem datiem: " + k);
            }
        }

        public string RecalcItem(P_ITEMS dritem, P_ITEMS_EVENTS drtoinit = null)
        {
            var it = new ItemInfo();
            it.SetFromRow(dritem);
            it.SetEventToInit(drtoinit);

            Ignore_ITEMS_EVENTS_ColumnChanged++;

            var rt = it.UpdateItemToDate(CurrentDate);
            if (rt != "OK")
            {
                Ignore_ITEMS_EVENTS_ColumnChanged--;
                return rt;
            }

            Ignore_ITEMS_EVENTS_ColumnChanged--;
            return "OK";
        }

        public void AddItem()
        {
            var fm = new FormP_Items_New();
            if(fm.ShowDialog() != DialogResult.OK) return;
            var dr_new_item = new P_ITEMS();

            Ignore_ITEMS_EVENTS_ColumnChanged++;

            dr_new_item.REG_NR = fm.itemsEventsData1.fITEM_REG_NR;
            dr_new_item.NAME = fm.itemsEventsData1.fITEM_NAME;
            MyData.DbContextP.P_ITEMS.Add(dr_new_item);
            if (fm.AddEvent)
            {
                var dr_new_event = new P_ITEMS_EVENTS();

                dr_new_event.IDIT = dr_new_item.ID;
                dr_new_event.ITEMRow = dr_new_item;
                dr_new_event.XEvent = EEvent.iegeks;
                dr_new_event.DT = fm.itemsEventsData1.fDT;
                dr_new_event.DTREG = fm.itemsEventsData1.fDT;
                dr_new_event.DOCNR = fm.itemsEventsData1.fDOCNR;
                dr_new_event.VALUE_C = fm.itemsEventsData1.fVALUE_0;
                dr_new_event.TAX_VAL_C = fm.itemsEventsData1.fVALUE_0;
                dr_new_event.CAT1 = fm.itemsEventsData1.fCAT1;
                dr_new_event.CATD = fm.itemsEventsData1.fCATD;
                dr_new_event.CATT = fm.itemsEventsData1.fCATT;
                dr_new_event.DEPARTMENT = fm.itemsEventsData1.fDEPARTMENT;
                dr_new_event.PLACE = fm.itemsEventsData1.fPLACE;

                dr_new_item.Events.Add(dr_new_event);
            }
            Ignore_ITEMS_EVENTS_ColumnChanged--;
            RecalcItem(dr_new_item);
            SaveData();
            SelectItem(dr_new_item.ID);
            if (bsItems.Count == 1)
                CheckChildGrid();
        }

        public void AddEvent()
        {
            if (bsItems.Current == null) return;
            var fm = new FormP_Items_NewEvent();
            var rt = fm.ShowDialog();
            if (rt != DialogResult.OK) return;
            var dr_item = bsItems.GetCurrentItem<P_ITEMS>();
            var dt = fm.Date;
            int evid = fm.EventId;
            var dr_new = new P_ITEMS_EVENTS();
            Ignore_ITEMS_EVENTS_ColumnChanged++;
            dr_new.IDIT = dr_item.ID;
            dr_new.DT = dt;
            dr_new.DTREG = dt;
            dr_new.EVENT = evid;
            dr_new.DESCR = dr_new.XEvent.ToMyStringFull();
            MyData.DbContextP.P_ITEMS_EVENTS.Add(dr_new);
            Ignore_ITEMS_EVENTS_ColumnChanged--;
            RecalcCurrent(dr_new);
        }

        public void ReplacePart()
        {
            if (bsItems.Current == null) return;
            var dr_item = bsItems.GetCurrentItem<P_ITEMS>();
            var fm = new FormP_ReplacePart();
            fm.InitData(dr_item.ID);
            var rt = fm.ShowDialog(MyMainForm);
            if (rt != DialogResult.OK) return;

            var dt = fm.replacePartData1.fDATE.Value;

            Ignore_ITEMS_EVENTS_ColumnChanged++;

            var dr_new = new P_ITEMS_EVENTS();
            dr_new.IDIT = dr_item.ID;
            dr_new.DT = dt;
            dr_new.DTREG = dt;
            dr_new.XEvent = EEvent.kapit;
            dr_new.DESCR = fm.replacePartData1.fDescr;
            dr_new.DOCNR = fm.replacePartData1.fDoc;
            dr_new.VALUE_C = fm.replacePartData1.fValueC;
            dr_new.DEPREC_C = fm.replacePartData1.fDeprecC;
            dr_new.TAX_VAL_C = fm.replacePartData1.fTaxValueC;
            MyData.DbContextP.P_ITEMS_EVENTS.Add(dr_new);
            RecalcCurrent(dr_new);

            Ignore_ITEMS_EVENTS_ColumnChanged--;
        }


        private EChColInd GetEventColInd(int ind)
        {
            if (ind == dgcEventsCat1.Index) return EChColInd.cat1;
            if (ind == dgcEventsCatD.Index) return EChColInd.catd;
            if (ind == dgcEventsCatT.Index) return EChColInd.catt;
            if (ind == dgcEventsPlace.Index) return EChColInd.place;
            if (ind == dgcEventsDepartment.Index) return EChColInd.department;
            if (ind == dgcEventsValue0.Index) return EChColInd.value0;
            if (ind == dgcEventsDeprec0.Index) return EChColInd.deprec0;
            if (ind == dgcEventsValueC.Index) return EChColInd.valuec;
            if (ind == dgcEventsDeprecC.Index) return EChColInd.deprecc;
            if (ind == dgcEventsSellValue.Index) return EChColInd.sellvalue;
            if (ind == dgcEventsTaxVal.Index) return EChColInd.taxvalue;
            if (ind == dgcEventsTaxValLeft.Index) return EChColInd.taxvalueleft;
            if (ind == dgcEventsTaxValC.Index) return EChColInd.taxvaluec;
            if (ind == dgcEventsMtTotal.Index) return EChColInd.mttotal;
            if (ind == dgcEventsMtUsed.Index) return EChColInd.mtused;
            return EChColInd.none;
        }

        private EChColInd GetEventColInd(MyGridRowBase row)
        {
            if (row == grCat1) return EChColInd.cat1;
            if (row == grCatD) return EChColInd.catd;
            if (row == grCatT) return EChColInd.catt;
            if (row == grPlace) return EChColInd.place;
            if (row == grDepartment) return EChColInd.department;
            if (row == grValue0) return EChColInd.value0;
            if (row == grDeprec0) return EChColInd.deprec0;
            if (row == grValueC) return EChColInd.valuec;
            if (row == grDeprecC) return EChColInd.deprecc;
            if (row == grSellValue) return EChColInd.sellvalue;
            if (row == grTaxVal) return EChColInd.taxvalue;
            if (row == grTaxValLeft) return EChColInd.taxvalueleft;
            if (row == grTaxValC) return EChColInd.taxvaluec;
            if (row == grMtTotal) return EChColInd.mttotal;
            if (row == grMtUsed) return EChColInd.mtused;
            return EChColInd.none;
        }

        private void dgvEvents_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            var dr = dgvEvents.GetObjectViewItem<P_ITEMS_EVENTS>(e.RowIndex);
            if (dr == null) return;
            var col_ind = GetEventColInd(e.ColumnIndex);
            if (col_ind == EChColInd.none) return;
            var cols_editable = SomeDataDefsP.GetEditableFields(dr.XEvent);
            if (!cols_editable.HasFlag(col_ind))
            {
                e.Cancel = true;
                return;
            }
        }

        private void sgrEvents_EditStarting(object sender, CancelEventArgs e)
        {
            if(bsItems.Current == null || bsRows.Current == null)
            {
                e.Cancel = true;
                return;
            }
            var dr = bsRows.GetCurrentItem<P_ITEMS_EVENTS>();
            if (dr == null) return;
            var col_ind = GetEventColInd(sender as MyGridRowBase);
            if (col_ind == EChColInd.none) return;
            var cols_editable = SomeDataDefsP.GetEditableFields(dr.XEvent);
            if (!cols_editable.HasFlag(col_ind))
            {
                e.Cancel = true;
                return;
            }

        }

        private void MarkEventRowCell(int rownr, int colnr, bool red)
        {
            var cell = dgvEvents[colnr, rownr];
            var column = dgvEvents.Columns[colnr];
            var cell_style = column.DefaultCellStyle;
            if (red)
            {
                cell_style = new DataGridViewCellStyle(cell_style);
                cell_style.ForeColor = MyMainForm.myStyleDefs.MarkedCellFore;
                cell_style.BackColor = MyMainForm.myStyleDefs.MarkedCellBack;
            }
            cell.Style = cell_style;
        }

        private void dgvEvents_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (e.ColumnIndex == dgcEventsRateD.Index || e.ColumnIndex == dgcEventsTaxRate.Index)
            {
                float f = (float)e.Value;
                e.Value = (f * 100.0f).ToString();
                e.FormattingApplied = true;
            }
            if(Utils.IN(e.ColumnIndex, 
                dgcEventsCat1.Index, 
                dgcEventsCatD.Index,
                dgcEventsCatT.Index,
                dgcEventsPlace.Index,
                dgcEventsDepartment.Index,
                dgcEventsValueC.Index,
                dgcEventsDeprecC.Index,
                dgcEventsSellValue.Index,
                dgcEventsMtTotal.Index,
                dgcEventsTaxValC.Index
               ))
            {
                var dr = dgvEvents.GetObjectViewItem<P_ITEMS_EVENTS>(e.RowIndex);
                if (dr == null) return;
                var propname = dgvEvents.Columns[e.ColumnIndex].DataPropertyName;
                Utils.GetPublicPropertyValue(dr, propname, out var val_cur);

                if (Utils.IN(e.ColumnIndex,
                    dgcEventsValueC.Index,
                    dgcEventsDeprecC.Index,
                    dgcEventsTaxValC.Index
                   ))
                {
                    MarkEventRowCell(e.RowIndex, e.ColumnIndex, (decimal)val_cur != 0.0M);
                    return;
                }

                var dr_prev = GetPrevRow(dr);
                if (dr_prev == null)
                {
                    MarkEventRowCell(e.RowIndex, e.ColumnIndex, false);
                }
                else
                {
                    Utils.GetPublicPropertyValue(dr_prev, propname, out var val_prev);
                    MarkEventRowCell(e.RowIndex, e.ColumnIndex, !object.Equals(val_prev, val_cur));
                }
            }
        }

        private MyGridRowPropEditorBase[] checkRedRows = null;
        private MyGridRowPropEditorBase[] checkRedRows2 = null;

        private void SetCheckRedRows()
        {
            checkRedRows = [grCat1, grCatD, grCatT, grPlace, grDepartment
                , grValueC ,grDeprecC, grSellValue, grTaxValC, grMtTotal];
            checkRedRows2 = [grValueC ,grDeprecC, grTaxValC];
        }

        public void CheckEventRedRows()
        {
            if (checkRedRows == null) SetCheckRedRows();
            var dr = bsRows.GetCurrentItem<P_ITEMS_EVENTS>();
            if (dr == null) return;
            if (bsRows.Position == 0)
            {
                sgrEvents.ClearRed();
            }
            else
            {
                var dr_prev = bsRows.GetItem<P_ITEMS_EVENTS>(bsRows.Position - 1);
                if (dr_prev == null) return;
                sgrEvents.CheckRedRows([dr_prev], [dr], [bsRows]
                    ,d =>
                    {
                        if (!checkRedRows.Contains(d)) return false;
                        if (checkRedRows2.Contains(d))
                            return (decimal)d.Value != 0.0M;
                        return d.IsRed([dr_prev], [dr], [bsRows]);
                    });
            }
        }

        public void CheckEventRedRows(MyGridRowPropEditorBase row)
        {
            if (row == null || !row.CheckRed) return;
            var dr = bsRows.GetCurrentItem<P_ITEMS_EVENTS>();
            if (dr == null) return;
            if (bsRows.Position == bsRows.Count - 1) return;
            var dr_prev = bsRows.GetItem<P_ITEMS_EVENTS>(bsRows.Position + 1);
            if (dr_prev == null) return;
            row.CheckRedRow([dr_prev], [dr], [bsRows]);
        }

        public void ShowDataPanel(bool b)
        {
            sgrEvents.Visible = b;
            MyData.Params.ShowItemDataPanel = b;
            rādītPaslēptDatuPaneliToolStripMenuItem.Checked = b;
        }

        public void ShowFilterPanel(bool b)
        {
            pnFilter.Visible = b;
            MyData.Params.ShowItemsFilterPanel = b;
            rādītPaslēptFiltraPaneliToolStripMenuItem.Checked = b;
        }

        private void bsItems_CurrentChanged(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckChildGrid();
        }

        private void bsRows_CurrentChanged(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckEventRedRows();
        }

        private void sgrEvents_EditEnded(object sender, EventArgs e)
        {
            var row = sender as MyGridRowPropEditorBase;
            CheckEventRedRows(row);
        }

        private bool detail_update_enabled = true;
        public void CheckChildGrid(bool setnull = false)
        {
            var dr_item = bsItems.GetCurrentItem<P_ITEMS>();
            if (dr_item == null || setnull)
            {
                //bsRows.DataSource = null;
                //bsRows.SetDataSourceFromBindingList(new BindingList<P_ITEMS_EVENTS>());
                dgvEvents.Enabled = false;
            }
            else
            {
                if (!detail_update_enabled) return;

                dgvEvents.Enabled = true;
                //var bl_events = dr_item.Events.GetBindingList();
                //bsRows.SetDataSourceFromBindingList(bl_events);
                //bsRows.DataSource = bl_events;
            }
        }

        public void EnableDetailUpdate(bool b)
        {
            detail_update_enabled = b;
            CheckChildGrid(!b);
        }

        public void SetDate(DateTime dt)
        {
            if (!SaveData()) return;
            if (bsItems.Count == 0) return;
            CurrentDate = dt;
            filterData1.fDATE1 = dt;
            var dts = Utils.DateToString(dt);
            if (tbDate.Text != dts) tbDate.Text = dts;
            RecalcAll();
            CheckChildGrid();
        }

        private void dgvItems_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex == dgcItemsState.Index)
            {
                if (e.Value == null || e.Value == DBNull.Value) return;
                int ist = (int)e.Value;
                EState est = (EState)ist;
                e.Value = est.ToMyString();
                e.FormattingApplied = true;
            }
        }

        private void myDataGridView1_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if(e.ColumnIndex == dgcFilterDate.Index)
            {
                Utils.DGVParseDateCell(e);
            }
        }

        private void dgvItems_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex == -1 || e.RowIndex == dgvItems.NewRowIndex || e.ColumnIndex == -1) return;
            var o = dgvItems[e.ColumnIndex, e.RowIndex].Value;
            if (o == null || o == DBNull.Value) return;
            if (e.ColumnIndex == dgcItemsCat1.Index)
            {
                int id = (int)o;
                e.ToolTipText = DataTasksP.GetCat1Name(id);
                return;
            }
            if (e.ColumnIndex == dgcItemsCatD.Index)
            {
                int id = (int)o;
                e.ToolTipText = DataTasksP.GetCatDName(id);
                return;
            }
            if (e.ColumnIndex == dgcItemsCatT.Index)
            {
                int id = (int)o;
                e.ToolTipText = DataTasksP.GetCatTName(id);
                return;
            }
            if (e.ColumnIndex == dgcItemsDepartment.Index)
            {
                int id = (int)o;
                e.ToolTipText = DataTasksP.GetDepName(id);
                return;
            }
            if (e.ColumnIndex == dgcItemsPlace.Index)
            {
                int id = (int)o;
                e.ToolTipText = DataTasksP.GetPlaceName(id);
                return;
            }
        }

        private void dgvEvents_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex == -1 || e.RowIndex == dgvEvents.NewRowIndex || e.ColumnIndex == -1) return;
            var o = dgvEvents[e.ColumnIndex, e.RowIndex].Value;
            if (o == null || o == DBNull.Value) return;
            if (e.ColumnIndex == dgcEventsCat1.Index)
            {
                var dr_cat1 = (P_CAT1)o;
                e.ToolTipText = dr_cat1?.DESCR;
                return;
            }
            if (e.ColumnIndex == dgcEventsCatD.Index)
            {
                var dr_catd = (P_CATD)o;
                e.ToolTipText = dr_catd?.DESCR;
                return;
            }
            if (e.ColumnIndex == dgcEventsCatT.Index)
            {
                var dr_catt = (P_CATT)o;
                e.ToolTipText = dr_catt?.DESCR;
                return;
            }
            if (e.ColumnIndex == dgcEventsDepartment.Index)
            {
                var dr_dep = (P_DEPARTMENTS)o;
                e.ToolTipText = dr_dep?.DESCR;
                return;
            }
            if (e.ColumnIndex == dgcEventsPlace.Index)
            {
                var dr_place = (P_PLACES)o;
                e.ToolTipText = dr_place?.DESCR;
                return;
            }
        }

        public void DoFilter()
        {
            if (!filterData1.fDATE1.HasValue) return;
            if (CurrentDate != filterData1.fDATE1.Value)
                SetDate(filterData1.fDATE1.Value);

            var pred_list = new List<Predicate<P_ITEMS>>();
            if (filterData1.fSTATE != null && filterData1.fSTATE.Value > 0)
            {
                if (filterData1.fSTATE.Value == 1)
                    pred_list.Add(x => x.TSTATE == 0 || x.TSTATE == 0);
                else
                    pred_list.Add(x => x.TSTATE == 4);
            }
            if (filterData1.fCAT1 != null)
            {
                var drf = MyData.DbContextP.P_CAT1.Find(filterData1.fCAT1.Value);
                if (drf.GROUP == 0)
                    pred_list.Add(x => x.TCAT1 == filterData1.fCAT1.Value);
                else
                    pred_list.Add(x => x.GetTCat1Row().CODE.StartsWithCI(drf.CODE));
            }
            if (filterData1.fCATD != null)
            {
                pred_list.Add(x => x.TCATD == filterData1.fCATD.Value);
            }
            if (filterData1.fCATT != null)
            {
                pred_list.Add(x => x.TCATT == filterData1.fCATT.Value);
            }
            if (filterData1.fDEPARTMENT != null)
            {
                var drf = MyData.DbContextP.P_DEPARTMENTS.Find(filterData1.fDEPARTMENT.Value);
                if (drf.GROUP == 0)
                    pred_list.Add(x => x.TDEPARTMENT == filterData1.fDEPARTMENT.Value);
                else
                    pred_list.Add(x => x.GetTDEPARTMENTSRow().CODE.StartsWithCI(drf.CODE));
            }
            if (filterData1.fPLACE != null)
            {
                var drf = MyData.DbContextP.P_PLACES.Find(filterData1.fPLACE.Value);
                if (drf.GROUP == 0)
                    pred_list.Add(x => x.TPLACE == filterData1.fPLACE.Value);
                else
                    pred_list.Add(x => x.GetTPLACESRow().CODE.StartsWithCI(drf.CODE));
            }

            bsItems.SetFilter(pred_list);
            CheckChildGrid();
        }

        private void dgvFilter_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (!filterData1.fDATE1.HasValue)
            {
                MyMainForm.ShowWarning("Janorāda aprēķina datums.");
                return;
            }
            DoFilter();
        }

        private void dgvFilter_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == dgcFilterDate.Index)
            {
                if (e.FormattedValue == null || e.FormattedValue == DBNull.Value)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void tbDate_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Return)
            {
                DateTime dt;
                if (tbDate.Text == "" || !Utils.StringToDate(tbDate.Text, out dt) 
                    || dt.Year < 1900 || dt.Year > 2100)
                {
                    MyMainForm.ShowWarning("Jānorāda korekts datums.");
                    tbDate.Text = Utils.DateToString(CurrentDate);
                    return;
                }
                SetDate(dt);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.D))
            {
                ShowDataPanel(!sgrEvents.Visible);
                return true;
            }
            if (keyData == (Keys.Control | Keys.G))
            {
                ShowFilterPanel(!pnFilter.Visible);
                return true;
            }
            if (keyData == (Keys.Control | Keys.F))
            {
                ShowSearchForm();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ShowSearchForm()
        {
            if (bsItems.Count == 0 || bsItems.Current == null) return;
            FormP_Search.Show(DoSearch, this);
        }

        public void DoSearch(string text, FormP_Search.ESerchDirection dir)
        {
            if (bsItems.Count == 0 || bsItems.Current == null) return;
            var stext = text.ToLower();
            int pos = dir == FormP_Search.ESerchDirection.FromTop ? 0 : bsItems.Position;
            int step = dir == FormP_Search.ESerchDirection.Up ? -1 : 1;
            pos += step;
            while(pos >= 0 && pos < bsItems.Count)
            {
                var dr = bsItems.GetItem<P_ITEMS>(pos);
                if (dr.REG_NR == text || (dr.NAME.ContainsCI(stext)))
                {
                    bsItems.Position = pos;
                    return;
                }
                pos += step;
            }
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            if (bnNav.BindingSource == bsRows)
            {
                AddEvent();
            }
            if (bnNav.BindingSource == bsItems)
            {
                AddItem();
            }
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            DeleteCurrent();
        }

        private void tsbInfo_Click(object sender, EventArgs e)
        {
            var msg = $"Pamatlīdzekļi: {bsItems.GetStats()}\nNotikumi: {bsRows.GetStats()}";
            MyMainForm.ShowInfo(msg);

            /*
            var sb = new StringBuilder();
            sb.AppendLine(msg);
            sb.AppendLine(MyData.DataSetKlonsP.ITEMS.GetChangesAsString(0));
            MyMainForm.ShowInfo(sb.ToString());
            */
        }

        private void MakeReportMT()
        {
            if (!SaveData()) return;
            if (bsItems.Count == 0 || bsItems.Current == null) return;
            var dr = bsItems.GetCurrentItem<P_ITEMS>();
            var it = new ItemInfo();
            it.SetFromRow(dr);
            var rt = it.MakeReport_MT(DateTime.MinValue, CurrentDate);
            if (rt != "OK")
            {
                MyMainForm.ShowWarning(rt);
                return;
            }
            var fm = MyMainForm.ShowForm(typeof(FormP_RepMT)) as FormP_RepMT;
            fm.SetItemDataMt(it, CurrentDate);
        }

        private void MakeReportYR()
        {
            if (!SaveData()) return;
            if (bsItems.Count == 0 || bsItems.Current == null) return;
            var dr = bsItems.GetCurrentItem<P_ITEMS>();
            var it = new ItemInfo();
            it.SetFromRow(dr);
            var rt = it.MakeReport_YR(DateTime.MinValue, CurrentDate);
            if (rt != "OK")
            {
                MyMainForm.ShowWarning(rt);
                return;
            }
            var fm = MyMainForm.ShowForm(typeof(FormP_RepMT)) as FormP_RepMT;
            fm.SetItemDataYr(it, new DateTime(CurrentDate.Year, 12, 31));
        }

        private void MakeReportYRMergedTable()
        {
            if (!SaveData()) return;
            if (bsItems.Count == 0 || bsItems.Current == null) return;
            var dr = bsItems.GetCurrentItem<P_ITEMS>();
            var it = new ItemInfo();
            it.SetFromRow(dr);
            var rt = it.MakeReportYrMerged(DateTime.MinValue, CurrentDate);
            if (rt != "OK")
            {
                MyMainForm.ShowWarning(rt);
                return;
            }
            var fm = MyMainForm.ShowForm(typeof(FormPRep_ItemMovement)) as FormPRep_ItemMovement;
            fm.SetItemDataYr(it, new DateTime(CurrentDate.Year, 12, 31));
        }

        private void MakeReportYRMergedPrint(int repnr)
        {
            if (!SaveData()) return;
            if (bsItems.Count == 0 || bsItems.Current == null) return;
            var dr = bsItems.GetCurrentItem<P_ITEMS>();
            var it = new ItemRepInfo();
            it.SetFromRow(dr);
            var rt = it.MakeReportYrMerged(DateTime.MinValue, CurrentDate);
            if (rt != "OK")
            {
                MyMainForm.ShowWarning(rt);
                return;
            }
            it.SetSFields();
            it.FormatDescr();

            ReportViewerData rd = new ReportViewerData();
            if (repnr == 1)
                rd.FileName = "ReportP_Kartina_1";
            else if (repnr == 2)
                rd.FileName = "ReportP_Kartina_2";
            else
                rd.FileName = "ReportP_Kartina_4";
            rd.Sources["DataSet1"] = it.GetEventRepList();
            rd.Sources["DataSetItem"] = new List<ItemRepInfo>() { it };
            rd.AddReportParameters(
                new string[]
                {
                    "PCompany", MyData.Params.CompNameX,
                    "PDate", Utils.DateToString(CurrentDate),
                    "PIDIT", "0"
                });
            MyMainForm.ShowReport(rd);

        }

        private void MakeReportYRMergedPrintSelected()
        {
            if (!SaveData()) return;
            if (bsItems.Count == 0)
            {
                MyMainForm.ShowWarning("Nav atlasīti pamatlīdzekļi.");
                return;
            }
            var list_it = new List<ItemRepInfo>();
            var dic_it = new Dictionary<int, ItemRepInfo>();
            foreach(var bsit in bsItems)
            {
                var dr = (bsit as ObjectView<P_ITEMS>).Object;
                var it = new ItemRepInfo();
                it.SetFromRow(dr);
                var rt = it.MakeReportYrMerged(DateTime.MinValue, CurrentDate);
                if (rt != "OK")
                {
                    rt = $"Pamatlīdzeklis:[{dr.REG_NR.Nz()}]:\n{rt}";
                    MyMainForm.ShowWarning(rt);
                    return;
                }
                it.SetSFields();
                it.FormatDescr();
                list_it.Add(it);
                dic_it[it.IdIt] = it;
            }

            ReportViewerData rd = new ReportViewerData();
            rd.FileName = "ReportP_Kartina_2x";
            rd.Sources["DataSet1"] = list_it;
            rd.AddReportParameters(
                new string[]
                {
                    "PCompany", MyData.Params.CompNameX,
                    "PDate", Utils.DateToString(CurrentDate)
                });
            rd.OnSubreport = p =>
            {
                int idit = int.Parse(p.Parameters["PIDIT"].Values[0]);
                var itc = dic_it[idit];
                p.DataSources.Add(new ReportDataSource("DataSetItem", new[] { itc }));
                p.DataSources.Add(new ReportDataSource("DataSet1", itc.GetEventRepList()));
            };
            MyMainForm.ShowReport(rd);

        }

        private void MakeReportTaxDeprec()
        {
            if (!SaveData()) return;
            if (bsItems.Count == 0 || bsItems.Current == null) return;
            var dr = bsItems.GetCurrentItem<P_ITEMS>();
            var it = new ItemRepInfo();
            it.SetFromRow(dr);
            var rt = it.MakeReport_YR(DateTime.MinValue, CurrentDate);
            if (rt != "OK")
            {
                MyMainForm.ShowWarning(rt);
                return;
            }

            ReportViewerData rd = new ReportViewerData();
            rd.FileName = "ReportP_NodNolPL_1";
            rd.Sources["dsItem"] = new List<ItemRepInfo>() { it };
            rd.Sources["dsEvents"] = it.GetEventRepList(); ;
            rd.AddReportParameters(
                new string[]
                {
                    "PCompany", MyData.Params.CompNameX
                });
            MyMainForm.ShowReport(rd);
        }

        private void MakeReportTaxDeprecSelected()
        {
            if (!SaveData()) return;
            if (bsItems.Count == 0)
            {
                MyMainForm.ShowWarning("Nav atlasīti pamatlīdzekļi.");
                return;
            }
            var list_it = new List<ItemRepInfo>();
            var dic_it = new Dictionary<int, ItemRepInfo>();
            foreach (var bsit in bsItems)
            {
                var dr = (bsit as ObjectView<P_ITEMS>).Object;
                var it = new ItemRepInfo();
                it.SetFromRow(dr);
                var rt = it.MakeReport_YR(DateTime.MinValue, CurrentDate);
                if (rt != "OK")
                {
                    rt = $"Pamatlīdzeklis:[{dr.REG_NR.Nz()}]:\n{rt}";
                    MyMainForm.ShowWarning(rt);
                    return;
                }
                list_it.Add(it);
                dic_it[it.IdIt] = it;
            }

            ReportViewerData rd = new ReportViewerData();
            rd.FileName = "ReportP_NodNolPL_1x";
            rd.Sources["DataSet1"] = list_it;
            rd.AddReportParameters(
                new string[]
                {
                    "PCompany", MyData.Params.CompNameX
                });
            rd.OnSubreport = p =>
            {
                int idit = int.Parse(p.Parameters["PIDIT"].Values[0]);
                var itc = dic_it[idit];
                p.DataSources.Add(new ReportDataSource("dsItem", new[] { itc }));
                p.DataSources.Add(new ReportDataSource("dsEvents", itc.GetEventRepList()));
            };
            MyMainForm.ShowReport(rd);

        }

        private void pārrēķinātToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RecalcCurrent();
        }
        private void jaunsPamatlīdzeklisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddItem();
        }
        private void jaunsNotikumsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddEvent();
        }
        private void amortizētāsAizstāšanasIzmaksasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReplacePart();
        }
        private void meklētPamatlīdzekliToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSearchForm();
        }
        private void rādītPaslēptDatuPaneliToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowDataPanel(!sgrEvents.Visible);
        }

        private void rādītPaslēptFiltraPaneliToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFilterPanel(!pnFilter.Visible);
        }

        private void aprēķinaIzklāstsPaMēnešiemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeReportMT();
        }

        private void aprēķinaIzklāstsPaGadiemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeReportYR();
        }

        private void miKartiņaTabula_Click(object sender, EventArgs e)
        {
            MakeReportYRMergedTable();
        }

        private void izdruka1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                MakeReportYRMergedPrint(2);
            });
        }

        private void izdruka2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                MakeReportYRMergedPrint(1);
            });
        }
        private void izdruka3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                MakeReportYRMergedPrint(3);
            });
        }

        private void izdruka1VisiemToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                MakeReportYRMergedPrintSelected();
            });
        }

        private void nolietojumsNodokļaVajadzībāmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                MakeReportTaxDeprec();
            });
        }

        private void nolietojumsNodokļaVajadzībāmToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                MakeReportTaxDeprecSelected();
            });
        }

    }
}
