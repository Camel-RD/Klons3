using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KlonsM.Classes;
using KlonsLIB.Forms;
using KlonsLIB.Data;
using KlonsLIB.Misc;
using KlonsF.Classes;
using KlonsLIB.Components;
using Klons3.ModelsM;
using KlonsLIB.MySourceGrid.GridRows;

namespace KlonsM.FormsM
{
    public partial class FormM_InvDoc : MyFormBaseF, IMyDgvTextboxEditingControlEvents2
    {
        public FormM_InvDoc()
        {
            InitializeComponent();
            CheckMyFontAndColors();

            MakeGrid();
        }

        private void FormM_InvDoc_Load(object sender, EventArgs e)
        {
            CheckSave();
            CheckEnableRows();

            grDocStore.ButtonClicked += GrDocStore_ButtonClicked;
            grDocIdStoreNor.ButtonClicked += GrDocStore_ButtonClicked;
            grDocIdStorePier.ButtonClicked += GrDocStore_ButtonClicked;
        }

        private void FormM_InvDoc_FormClosed(object sender, FormClosedEventArgs e)
        {
            grDocStore.ButtonClicked -= GrDocStore_ButtonClicked;
            grDocIdStoreNor.ButtonClicked -= GrDocStore_ButtonClicked;
            grDocIdStorePier.ButtonClicked -= GrDocStore_ButtonClicked;
        }

        private void MakeGrid()
        {
            sgrDoc.MakeGrid2();
            sgrDoc.LinkGrid();
        }

        public bool FindDoc(M_INV_DOCS dr_doc)
        {
            return bsDocs.SelectItem(dr_doc);
        }

        public static bool ShowDocMyDialog(M_INV_DOCS dr_doc)
        {
            var form = KlonsData.St.MyMainForm.ShowFormMDialog<FormM_InvDoc>();
            bool rt = form.FindDoc(dr_doc);
            if (!rt)
            {
                form.Close();
                KlonsData.St.MyMainForm.ShowError("Neizdevās atvērt dokumentu.");
                return false;
            }
            form.CheckColumnsVisible();
            return true;
        }

        private bool ignoreColumnChangeEvent = false;
        private bool InRows_ColumnChanged = false;

        private void BsRows_ListItemPropertyChanged(object sender, MyItemPropertyChangedEventArgs e)
        {
            if (ignoreColumnChangeEvent) return;
            if (InRows_ColumnChanged) return;
            InRows_ColumnChanged = true;
            BsRows_ListItemPropertyChangedA(e);
            InRows_ColumnChanged = false;
        }

        private void BsRows_ListItemPropertyChangedA(MyItemPropertyChangedEventArgs e)
        {
            var table = MyData.DbContextM.M_INV_ROWS;
            var table_items = MyData.DbContextM.BL_M_ITEMS;
            var dr = e.Item as M_INV_ROWS;
            if (e.PropertyName == nameof(M_INV_ROWS.Item))
            {
                var dr_item = dr.Item;
                if (dr_item == null) return;
                dr.Units = dr_item.UnitsRow;
                dgvRows.RefreshCurrent();
            }
            if (e.PropertyName == nameof(M_INV_ROWS.AM_COUNTED_2))
            {
                var am_counted = dr.AM_COUNTED_2 == null ? dr.AM_COUNTED_1 : dr.AM_COUNTED_2.Value;
                dr.AM_DIFF = dr.AM_CALC - am_counted;
                dgvRows.RefreshCurrent();
            }
        }

        public void CheckColumnsVisible()
        {
            var dr_doc = GetCurrentDocRow();
            if (dr_doc == null) return;
            var b = dr_doc.XState != EInventoryDocState.Melnraksts;
            dgcRowsAmCounted2.Visible = b;
            dgcRowsAmCalc.Visible = b;
            dgcRowsAmDiff.Visible = b;
        }

        private void GetDocStoreCellValue(MyGridRowPickRowTextBox grow)
        {
            var dr_cur = (M_STORES)grow.Value;
            var dr_new = FormM_Stores.GetStore(dr_cur, EStoreType.Noliktava);
            if (dr_new == null) return;
            grow.Value = dr_new;
        }

        private void GrDocStore_ButtonClicked(object sender, EventArgs e)
        {
            var grow = (MyGridRowPickRowTextBox)sender;
            GetDocStoreCellValue(grow);
        }


        private M_INV_DOCS GetCurrentDocRow()
        {
            if (bsDocs.Count == 0 || bsDocs.Current == null) return null;
            var dr_doc = bsDocs.GetCurrentItem<M_INV_DOCS>();
            return dr_doc;
        }

        private M_INV_DOCS GetGoodCurrentDocRow()
        {
            var dr_doc = GetCurrentDocRow();
            if (dr_doc == null) return null;
            if (!SaveData())
            {
                MyMainForm.ShowWarning("Neizdevās nesaglabāt datus.");
                return null;
            }
            if (HasChanges())
            {
                MyMainForm.ShowWarning("Ir nesaglabāti dati.");
                return null;
            }
            return dr_doc;
        }


        private void CheckEnableRows()
        {
            bool sgrDoc_Visible = bsDocs.Count > 0 && bsDocs.Current != null;
            bool dgvRows_Enabled = bsDocs.Count > 0 && bsDocs.Current != null;
            SetControlVisible(sgrDoc, sgrDoc_Visible);
            SetControlEnabled(dgvRows, dgvRows_Enabled);
        }

        public override bool SaveData()
        {
            if (!dgvRows.EndEditX()) return false;
            if (!Validate()) return false;
            var rt = bsDocs.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private bool HasChanges()
        {
            return bsDocs.HasChanges() || bsRows.HasChanges();
        }

        private void CheckSave()
        {
            SetSaveButton(HasChanges());
        }

        private void SetSaveButton(bool red)
        {
            bNav.SetSaveButtonRed(red);
        }

        public void DeleteCurrent()
        {
            if (!CanEditCurrentDoc()) return;
            var dr_doc = GetCurrentDocRow();
            if (dr_doc.XState != EInventoryDocState.Melnraksts)
            {
                MyMainForm.ShowWarning("Pēc salīdzīnāšanas ierakstus dzēst nevar.", owner: this);
                return;
            }
            bNav.DeleteCurrent();
            SaveData();
        }

        public bool IsOpenToChanges(M_INV_DOCS dr_doc)
        {
            return dr_doc.XState != EInventoryDocState.Apstiprināts &&
                dr_doc.XState != EInventoryDocState.Iegrāmatots;
        }

        public bool CanEditDoc(M_INV_DOCS dr_doc)
        {
            if (dr_doc == null) return false;
            return IsOpenToChanges(dr_doc);
        }

        public bool CanEditCurrentDoc()
        {
            if (bsDocs.Count == 0 || bsDocs.Current == null) return false;
            var dr_doc = bsDocs.GetCurrentItem<M_INV_DOCS>();
            if (dr_doc == null) return false;
            return IsOpenToChanges(dr_doc);
        }

        public bool CanDeleteCurrentRow()
        {
            if (bsDocs.Count == 0 || bsDocs.Current == null) return false;
            var dr_doc = bsDocs.GetCurrentItem<M_INV_DOCS>();
            if (dr_doc == null) return false;
            if (bsRows.Count == 0 || bsRows.Current == null) return false;
            return dr_doc.XState == EInventoryDocState.Melnraksts;
        }

        public M_STORES GetStore(M_STORES dr_store, EStoreType storefilter = EStoreType.Nenoteikts)
        {
            return FormM_Stores.GetStore(dr_store, storefilter);
        }

        public M_ITEMS GetItem(M_ITEMS dr_item)
        {
            return FormM_Items.GetItem(dr_item);
        }

        private bool CanEditDocsCurrentCell()
        {
            if (bsDocs.Count == 0 || bsDocs.Current == null) return false;
            return true;
        }

        private bool CanEditRowsCurrentCell()
        {
            if (bsRows.Count == 0 || bsRows.Current == null) return false;
            if (dgvRows.CurrentRow == null) return false;
            //if (dgvRows.CurrentRow.IsNewRow) return false;
            if (dgvRows.CurrentCell == null) return false;
            return true;
        }

        private void SetCurrentDocEditorValue(object value)
        {
            if (ActiveControl == null) return;
            try
            {
                if (ActiveControl is MyMcComboBox c1)
                {
                    c1.SelectedValue = value;
                }
                else if (ActiveControl is MyPickRowTextBox2 c2)
                {
                    c2.SelectedValue = value;
                }
            }
            catch (Exception) { }
        }

        private void SetCurrentRowsItem(M_ITEMS dr_item)
        {
            if (dgvRows.CurrentCell == null) return;
            try
            {
                if (dr_item == null) return;
                dgvRows.BeginEdit(false);
                if (dgvRows.EditingControl is MyMcComboBox c1)
                {
                    c1.SelectedValue = dr_item;
                }
                else if (dgvRows.EditingControl is MyPickRowTextBox2 c2)
                {
                    c2.SelectedValue = dr_item;
                }
                dgvRows.EndEdit();
            }
            catch (Exception) { }
        }

        public void GetDocStore()
        {
            if (!CanEditDocsCurrentCell()) return;
            var dv = (M_STORES)((MyDgvTextboxEditingControl2)ActiveControl).SelectedValue;
            var rt = GetStore(dv, EStoreType.Noliktava);
            if (rt == null) return;
            SetCurrentDocEditorValue(rt);
        }

        public void GetRowItem()
        {
            if (!CanEditRowsCurrentCell()) return;
            var cv = (M_ITEMS)dgvRows.CurrentCell.Value;
            var ret = GetItem(cv);
            if (ret == null) return;
            SetCurrentRowsItem(ret);
        }

        private int SearchRowText(string text, int colindex,
            int startindex = 0, bool forward = true)
        {
            string propname = dgvRows.Columns[colindex].DataPropertyName;
            if (bsRows.Count == 0) return -1;
            if (startindex < 0 || startindex >= bsRows.Count) return -1;
            if (string.IsNullOrEmpty(text)) return -1;
            if (startindex == 0 && !forward) return -1;
            if (startindex == bsRows.Count - 1 && forward) return -1;
            var dr0 = bsDocs.GetItem<M_INV_ROWS>(startindex);
            if (dr0 == null) return -1;
            var propdef = TypeDescriptor.GetProperties(dr0)[propname];
            if (propdef == null) return -1;
            int di = forward ? 1 : -1;
            string val;
            text = text.ToLower();
            for (int i = startindex; i >= 0 && i < bsRows.Count; i += di)
            {
                var dr = bsRows.GetItem<M_INV_ROWS>(i);
                if (dr == null) continue;
                val = null;
                if (colindex == dgcRowsItemName.Index)
                    val = dr.Item?.NAME;
                else if (colindex == dgcRowsIdItem.Index)
                    val = dr.Item?.BARCODE;
                if (val == null) continue;
                if (val.ToLower().Contains(text))
                {
                    return i;
                }
            }
            return -1;
        }

        private void SearchRowText(bool fromfirst = true, bool forward = true)
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            if (!dgvRows.EndEditX()) return;

            int startindex = dgvRows.CurrentRow.Index;
            startindex = fromfirst ? 0 : (forward ? startindex + 1 : startindex - 1);
            string text = tsbFind.Text;
            if (text == "") return;
            int newindex = SearchRowText(text, dgvRows.CurrentCell.ColumnIndex, startindex, forward);
            if (newindex == -1)
            {
                MyMainForm.ShowInfo("Tekts [" + text + "] netika atrasts.");
                return;
            }
            dgvRows.CurrentCell = dgvRows[dgvRows.CurrentCell.ColumnIndex, newindex];
        }

        private void dgvRows_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.Value == DBNull.Value) return;

            if (e.ColumnIndex == dgcRowsItemName.Index)
            {
                var dr = (M_ITEMS)e.Value;
                if (dr == null) return;
                e.Value = dr.NAME;
                e.FormattingApplied = true;
            }
            if (e.ColumnIndex == dgcRowsIdUnits.Index)
            {
                var dr = (M_UNITS)e.Value;
                if (dr == null) return;
                e.Value = dr.CODE;
                e.FormattingApplied = true;
            }
        }

        private void bniSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void bniDelete_Click(object sender, EventArgs e)
        {
            DeleteCurrent();
        }

        private void bniNew_Click(object sender, EventArgs e)
        {
            if (dgvRows.AllowUserToAddRows)
                dgvRows.MoveToNewRow();
        }

        private void dgvRows_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!CanDeleteCurrentRow()) return;
            if (e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void bNav_ItemDeleting(object sender, CancelEventArgs e)
        {
            if (!CanDeleteCurrentRow()) return;
            e.Cancel = !AskCanDelete();
        }

        private void dgvRows_MyKeyDown(object sender, KeyEventArgs e)
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

            if (dgvRows.CurrentRow == null ||
                //dgvRows.CurrentRow.IsNewRow ||
                dgvRows.CurrentCell == null) return;
            int colnr = dgvRows.CurrentCell.ColumnIndex;

            if (e.KeyCode == Keys.F4)
            {
                if (!CanEditCurrentDoc()) return;
                if (colnr == dgcRowsIdItem.Index)
                {
                    GetRowItem();
                    e.Handled = true;
                    return;
                }
            }
        }

        private void dgvRows_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRows.CurrentRow == null ||
                //dgvRows.CurrentRow.IsNewRow ||
                dgvRows.CurrentCell == null) return;
            if (!CanEditCurrentDoc()) return;
            int colnr = dgvRows.CurrentCell.ColumnIndex;

            if (colnr == dgcRowsIdItem.Index)
            {
                GetRowItem();
                return;
            }
        }

        private void dgvRows_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void bsDocs_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckEnableRows();
            CheckSave();
        }

        private void bsDocs_CurrentChanged(object sender, EventArgs e)
        {
            CheckEnableRows();
        }

        private void bsRows_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            if (e.ListChangedType == ListChangedType.Reset) return;
            CheckSave();
        }

        private void dgvRows_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {

        }

        void IMyDgvTextboxEditingControlEvents2.OnButtonClicked(MyDgvTextboxEditingControl2 control)
        {
            if (control.DataSource == bsStore)
            {
                GetDocStore();
                return;
            }
            if (control.DataSource == bsItems)
            {
                GetRowItem();
                return;
            }
        }

        private bool CanEditAmCounted2(M_INV_DOCS dr_doc)
        {
            return dr_doc.XState == EInventoryDocState.Salīdzināts;
        }

        private void dgvRows_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            var dr_row = bsRows.GetItem<M_INV_ROWS>(e.RowIndex);
            if (dr_row == null) return;
            var dr_doc = dr_row.Doc;
            if (!CanEditDoc(dr_doc))
                e.Cancel = true;
            if (e.ColumnIndex == dgcRowsAmCounted2.Index && !CanEditAmCounted2(dr_doc))
                e.Cancel = true;
            if (e.ColumnIndex == dgcRowsAmCounted1.Index && CanEditAmCounted2(dr_doc))
                e.Cancel = true;
            if (e.ColumnIndex == dgcRowsIdItem.Index && CanEditAmCounted2(dr_doc))
                e.Cancel = true;
        }

        private void sgrDoc_ConvertingValueToDisplayString(object sender, DevAge.ComponentModel.ConvertingObjectEventArgs e)
        {
            if (sender == grDocState)
            {
                var state = (EInventoryDocState)e.Value;
                var val = SomeDataDefsM.GetInventoryDocStateText(state);
                e.Value = val;
                e.ConvertingStatus = DevAge.ComponentModel.ConvertingStatus.Completed;
            }
        }

        private void tsbFindPrev_Click(object sender, EventArgs e)
        {
            SearchRowText(false, false);
            dgvRows.Focus();
        }

        private void tsbFindNext_Click(object sender, EventArgs e)
        {
            SearchRowText(false, true);
            dgvRows.Focus();
        }

        private void tsbFind_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                SearchRowText();
                dgvRows.Focus();
                e.Handled = true;
                return;
            }
            if (e.KeyChar == (char)Keys.Escape)
            {
                dgvRows.Focus();
                e.Handled = true;
                return;
            }
        }

        private void tsbFind_Enter(object sender, EventArgs e)
        {
            tsbFind.Text = "";
        }

        private void sgrDoc_EditStarting(object sender, CancelEventArgs e)
        {
            if (!CanEditCurrentDoc())
            {
                e.Cancel = true;
            }
        }

        public ErrorList CheckOneEntryPerItem(M_INV_DOCS dr_doc)
        {
            var ret = new ErrorList();
            var drs_rows = dr_doc.Rows;
            var bad_items = drs_rows
                .GroupBy(x => x.Item)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();
            foreach (var item in bad_items)
                ret.AddItemError(item, "Artikuls ir ievadīts vairāk kā 1 reizi.");
            return ret;
        }

        public void DoCompareWithDB()
        {
            var err = new ErrorList();
            var dr_doc = GetGoodCurrentDocRow();
            if (dr_doc == null) return;
            if (bsRows.Count == 0)
            {
                MyMainForm.ShowWarning("Dokumentā nav ievadīti dati.", owner: this);
                return;
            }
            if(dr_doc.XState != EInventoryDocState.Melnraksts)
            {
                MyMainForm.ShowWarning("Datu salīdzināšana jau ir veikta.", owner: this);
                return;
            }
            err = CheckOneEntryPerItem(dr_doc);
            if (err.HasErrors)
            {
                FormM_ErrorList.ShowErrorList(this, err);
                return;
            }
            var table_rows = MyData.DbContextM.BL_M_INV_ROWS;
            var table_items = MyData.DbContextM.BL_M_ITEMS;
            var table_rep = DataLoaderM.GetBy_SP_M_REP_ITEMSINSTORE_1(dr_doc.IDSTORE);
            var drs_rows = dr_doc.Rows.ToList();
            var dic_drs_rows = drs_rows.ToDictionary(x => x.IDITEM);
            var dic_drs_rep = table_rep.ToDictionary(x => x.IDITEM);

            table_rep
               .Where(x => table_items.FindById(x.IDITEM) == null)
               .ForEach(x => err.AddError($"{x.ITEMCODE}", "Artiukuls nav atrasts artikulu sarakstā."));
            if (err.HasErrors)
            {
                FormM_ErrorList.ShowErrorList(this, err);
                return;
            }

            ignoreColumnChangeEvent = true;
            foreach (var dr_rep in table_rep)
            {
                if (dic_drs_rows.TryGetValue(dr_rep.IDITEM, out var dr_row))
                {
                    dr_row.AM_CALC = dr_rep.AMOUNT;
                    dr_row.AM_DIFF = dr_rep.AMOUNT - dr_row.AM_COUNTED_1;
                }
                else
                {
                    var dr_new = table_rows.NewItem();
                    dr_new.IDDOC = dr_doc.ID;
                    dr_new.IDITEM = dr_rep.IDITEM;
                    var dr_item = table_items.FindById(dr_rep.IDITEM);
                    dr_new.IDUNITS = dr_item == null ? 1 : dr_item.UNITS;
                    dr_new.AM_COUNTED_1 = 0M;
                    dr_new.AM_COUNTED_2 = 0M;
                    dr_new.AM_CALC = dr_rep.AMOUNT;
                    dr_new.AM_DIFF = dr_rep.AMOUNT;

                    dr_new.Doc = dr_doc;
                    dr_new.Item = dr_item;
                    dr_new.Units = MyData.DbContextM.BL_M_UNITS.FindById(dr_new.IDUNITS);

                    table_rows.Add(dr_new);
                }
            }
            foreach (var dr_row in drs_rows)
            {
                if (!dic_drs_rep.TryGetValue(dr_row.IDITEM, out var dr_rep))
                {
                    dr_row.AM_CALC = 0M;
                    dr_row.AM_DIFF = -dr_row.AM_COUNTED_1;
                }
            }

            ignoreColumnChangeEvent = false;
            dr_doc.XState = EInventoryDocState.Salīdzināts;
            if (!SaveData())
            {
                dr_doc.XState = EInventoryDocState.Melnraksts;
                return;
            }
            dgvRows.AllowUserToAddRows = false;
            CheckColumnsVisible();
            dgvRows.Refresh();
        }

        public void DoCloseDoc()
        {
            var dr_doc = GetGoodCurrentDocRow();
            if (dr_doc == null) return;
            if (bsRows.Count == 0)
            {
                MyMainForm.ShowWarning("Dokumentā nav ievadīti dati.", owner: this);
                return;
            }
            if (dr_doc.XState != EInventoryDocState.Salīdzināts)
            {
                MyMainForm.ShowWarning("Datu salīdzināšana nav veikta.", owner: this);
                return;
            }
            if (dr_doc.XState == EInventoryDocState.Apstiprināts ||
                dr_doc.XState == EInventoryDocState.Iegrāmatots)
            {
                MyMainForm.ShowWarning("Dokuments jau ir apstiprināts.", owner: this);
                return;
            }
            dr_doc.XState = EInventoryDocState.Apstiprināts;
            SaveData();
            CheckColumnsVisible();
        }

        public void DoMakeStoreDoc()
        {
            ErrorList err;
            var dr_doc = GetGoodCurrentDocRow();
            if (dr_doc == null) return;
            if (bsRows.Count == 0)
            {
                MyMainForm.ShowWarning("Dokumentā nav ievadīti dati.", owner: this);
                return;
            }
            if (dr_doc.XState != EInventoryDocState.Melnraksts)
            {
                MyMainForm.ShowWarning("Datu salīdzināšana nav veikta.", owner: this);
                return;
            }
            if (dr_doc.XState != EInventoryDocState.Apstiprināts)
            {
                MyMainForm.ShowWarning("Dokuments nav apstiprināts.", owner: this);
                return;
            }
            if (dr_doc.XState == EInventoryDocState.Iegrāmatots)
            {
                bool rt = MyMainForm.AskSomething("Dokuments jau ir iegrāmatots.\n"+
                    "Vai veidot norakstīšanas / pierakstīšanas atkārtoti?", owner: this);
                if (!rt) return;
            }
            err = CheckOneEntryPerItem(dr_doc);
            if (err.HasErrors)
            {
                FormM_ErrorList.ShowErrorList(this, err);
                return;
            }
            if (inventoryDocData1._IDSTORE_NOR == 1 || inventoryDocData1._IDSTORE_PIER == 1)
            {
                MyMainForm.ShowWarning("Jānorāda kodi uz kuriem tiks veikta norakstīšana / pierakstīšana. ", owner: this);
                return;
            }

            if (inventoryDocData1._NR_NOR.IsNOE() || inventoryDocData1._NR_PIER.IsNOE())
            {
                MyMainForm.ShowWarning("Jānorāda norakstīšanas / pierakstīšanas dokumentu numuri. ", owner: this);
                return;
            }

            var dr_store_nor = MyData.DbContextM.BL_M_STORES.FindById(inventoryDocData1._IDSTORE_NOR);
            var dr_store_pier = MyData.DbContextM.BL_M_STORES.FindById(inventoryDocData1._IDSTORE_PIER);

            if (dr_store_nor.XStoreType != EStoreType.Citi || dr_store_pier.XStoreType != EStoreType.Citi)
            {
                MyMainForm.ShowWarning("Norakstīšanas / pierakstīšanas noliktavas veidam jābūt 'Cits'. ", owner: this);
                return;
            }

            var table_docs = MyData.DbContextM.BL_M_DOCS;
            var table_rows = MyData.DbContextM.BL_M_ROWS;
            var drs_rows = dr_doc.Rows;
            var drs_rows_nor = drs_rows
                .Where(x => x.AM_DIFF < 0M)
                .ToList();
            var drs_rows_pier = drs_rows
                .Where(x => x.AM_DIFF > 0M)
                .ToList();

            if(drs_rows_nor.Count > 0)
            {
                var drnew_doc = table_docs.NewItem();
                drnew_doc.DT = DateTime.Today;
                drnew_doc.NR = inventoryDocData1._NR_NOR;
                drnew_doc.XDocType = EDocType.Norakstīts;
                drnew_doc.XState = EDocState.Melnraksts;
                drnew_doc.IDSTOREOUT = dr_doc.IDSTORE;
                drnew_doc.IDSTOREIN = inventoryDocData1._IDSTORE_NOR;
                table_docs.Add(drnew_doc);
                foreach (var dr_nor in drs_rows_nor)
                {
                    var drnew_row = table_rows.NewItem();
                    drnew_row.IDDOC = drnew_doc.ID;
                    drnew_row.Doc = drnew_doc;
                    drnew_row.IDITEM = dr_nor.IDITEM;
                    drnew_row.UNITS = dr_nor.IDUNITS;
                    drnew_row.AMOUNT = -dr_nor.AM_DIFF;
                    drnew_row.PRICE0 = dr_nor.Item.SELLPRICE;
                    drnew_row.PRICE = dr_nor.Item.SELLPRICE;
                    drnew_row.TPRICE = drnew_row.PRICE * -dr_nor.AM_DIFF;
                    table_rows.Add(drnew_row);
                }
            }
            if (drs_rows_pier.Count > 0)
            {
                var drnew_doc = table_docs.NewItem();
                drnew_doc.DT = DateTime.Today;
                drnew_doc.NR = inventoryDocData1._NR_PIER;
                drnew_doc.XDocType = EDocType.Pierakstīts;
                drnew_doc.XState = EDocState.Melnraksts;
                drnew_doc.IDSTOREOUT = inventoryDocData1._IDSTORE_NOR;
                drnew_doc.IDSTOREIN = dr_doc.IDSTORE;
                table_docs.Add(drnew_doc);
                foreach (var dr_nor in drs_rows_nor)
                {
                    var drnew_row = table_rows.NewItem();
                    drnew_row.IDDOC = drnew_doc.ID;
                    drnew_row.Doc = drnew_doc;
                    drnew_row.IDITEM = dr_nor.IDITEM;
                    drnew_row.UNITS = dr_nor.IDUNITS;
                    drnew_row.AMOUNT = dr_nor.AM_DIFF;
                    drnew_row.PRICE0 = dr_nor.Item.SELLPRICE;
                    drnew_row.PRICE = dr_nor.Item.SELLPRICE;
                    drnew_row.TPRICE = drnew_row.PRICE * -dr_nor.AM_DIFF;
                    table_rows.Add(drnew_row);
                }
            }
            dr_doc.XState = EInventoryDocState.Iegrāmatots;
            SaveData();
            MyMainForm.ShowInfo("Norakstīšanas / pierakstīšanas dokumenti izveidoti.", owner: this);
        }

        public void DoFilter(bool applyfilter)
        {
            if (!applyfilter)
            {
                bsRows.RemoveFilter();
                return;
            }
            bsRows.SetFilter<M_INV_ROWS>(x => x.AM_DIFF != 0);
        }

        private void miCompareWithDB_Click(object sender, EventArgs e)
        {
            DoCompareWithDB();
        }

        private void miCloseDoc_Click(object sender, EventArgs e)
        {
            DoCloseDoc();
        }

        private void miMakeStoreDoc_Click(object sender, EventArgs e)
        {
            DoMakeStoreDoc();
        }

        private void miFilterRows_Click(object sender, EventArgs e)
        {
            bool b = !miFilterRows.Checked;
            miFilterRows.Checked = b;
            DoFilter(b);
        }
    }
}
