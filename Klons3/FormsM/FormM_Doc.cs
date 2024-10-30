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
    public partial class FormM_Doc : MyFormBaseF, IMyDgvTextboxEditingControlEvents2
    {
        public FormM_Doc()
        {
            InitializeComponent();
            CheckMyFontAndColors();

            DecimalsInPrices = MyData.Params.DECIMALSINPRICES;
            string price_format = "N" + DecimalsInPrices;
            dgcRowsPrice0.DefaultCellStyle.Format = price_format;
            dgcRowsPrice.DefaultCellStyle.Format = price_format;
            dgcRowsBuyPrice.DefaultCellStyle.Format = price_format;

            miSplitPVN.Visible = !AreWeVATPayer;

            LoadColumnWidthsFromSettings();
            MakeGrid();
        }

        private int DecimalsInPrices = 2;

        private void LoadColumnWidthsFromSettings()
        {
            string scw = MyData.Settings.ColumnWidths_MDoc;
            (int ver, int[] cw) = dgvRows.ParseColumnWidths(scw);
            if (ver == 1 && cw != null && cw.Length > 0)
                dgvRows.SetColumnWidths(cw);
        }

        private void SaveColumnWidthsToSettings()
        {
            MyData.Settings.ColumnWidths_MDoc = dgvRows.GetColumnWidths2(10f, 1);
        }

        public override void SaveParams()
        {
            SaveColumnWidthsToSettings();
        }

        private void MakeGrid()
        {
            sgrDocA.MakeGrid2();
            sgrDocA.LinkGrid();
            int h = 0;
            for (int i = 0; i < sgrDocA.RowsCount; i++)
                h += sgrDocA.Rows[i].Height;
            mySplitContainer1.SplitterDistance = h + SystemInformation.HorizontalScrollBarHeight + 6;
            //grDocPVNType.DataCell.View.WordWrap = true;
        }

        public bool FindDoc(M_DOCS dr_doc)
        {
            return bsDocs.SelectItem(dr_doc);
        }

        public static bool ShowDocMyDialog(M_DOCS dr_doc)
        {
            var form = KlonsData.St.MyMainForm.ShowFormMDialog<FormM_Doc>();
            bool rt = form.FindDoc(dr_doc);
            if (!rt)
            {
                form.Close();
                KlonsData.St.MyMainForm.ShowError("Neizdevās atvērt dokumentu.");
                return false;
            }
            return true;
        }

        private void FormM_Doc_Load(object sender, EventArgs e)
        {
            CheckSave();
            CheckEnableRows();
            CheckEnableDocsCheckBoxes();

            MyData.DbContextM.M_ROWS.Local.CollectionChanged += M_ROWS_Local_CollectionChanged;

            grDocTP.ButtonClicked += GrDocTP_ButtonClicked;
            grDocTransType.ButtonClicked += GrDocTransType_ButtonClicked;
            grDocStoreOut.ButtonClicked += GrDocStore_ButtonClicked;
            grDocStoreIn.ButtonClicked += GrDocStore_ButtonClicked;
            grDocCarrier.ButtonClicked += GrDocStore_ButtonClicked;
            grDocAddressOut.ButtonClicked += GrDocAddressInOut_ButtonClicked;
            grDocAddressIn.ButtonClicked += GrDocAddressInOut_ButtonClicked;
            grDocDriver.ButtonClicked += GrDocDriver_ButtonClicked;
            grDocVehicle.ButtonClicked += GrDocVehicle_ButtonClicked;
            grDocAccIn.ButtonClicked += GrDocAcc_ButtonClicked;
            grDocAccOut.ButtonClicked += GrDocAcc_ButtonClicked;
            grDocPVNType.ButtonClicked += GrDocPVNType_ButtonClicked;
        }

        private void FormM_Doc_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyData.DbContextM.M_ROWS.Local.CollectionChanged -= M_ROWS_Local_CollectionChanged;

            grDocTP.ButtonClicked -= GrDocTP_ButtonClicked;
            grDocTransType.ButtonClicked -= GrDocTransType_ButtonClicked;
            grDocStoreOut.ButtonClicked -= GrDocStore_ButtonClicked;
            grDocStoreIn.ButtonClicked -= GrDocStore_ButtonClicked;
            grDocCarrier.ButtonClicked -= GrDocStore_ButtonClicked;
            grDocAddressOut.ButtonClicked -= GrDocAddressInOut_ButtonClicked;
            grDocAddressIn.ButtonClicked -= GrDocAddressInOut_ButtonClicked;
            grDocDriver.ButtonClicked -= GrDocDriver_ButtonClicked;
            grDocVehicle.ButtonClicked -= GrDocVehicle_ButtonClicked;
            grDocAccIn.ButtonClicked -= GrDocAcc_ButtonClicked;
            grDocAccOut.ButtonClicked -= GrDocAcc_ButtonClicked;
            grDocPVNType.ButtonClicked -= GrDocPVNType_ButtonClicked;
        }

        private void M_ROWS_Local_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var dr = e.NewItems[0] as M_ROWS;
                UpdateDocSums(dr.Doc);
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                var dr = e.OldItems[0] as M_ROWS;
                UpdateDocSums(dr.Doc);
            }
        }

        private bool ignoreColumnChangeEvent = false;
        private bool InDocs_ColumnChanged = false;
        private bool InRows_ColumnChanged = false;

        private void BsDocs_ListItemPropertyChanged(object sender, MyItemPropertyChangedEventArgs e)
        {
            if (ignoreColumnChangeEvent) return;
            if (InDocs_ColumnChanged) return;
            InDocs_ColumnChanged = true;
            M_DOCS_ColumnChangedA(e);
            InDocs_ColumnChanged = false;
        }

        private void BsRows_ListItemPropertyChanged(object sender, MyItemPropertyChangedEventArgs e)
        {
            if (ignoreColumnChangeEvent) return;
            if (InRows_ColumnChanged) return;
            InRows_ColumnChanged = true;
            try
            {
                M_ROWS_ColumnChangedA(e);
            }
            finally
            {
                InRows_ColumnChanged = false;
            }
        }


        private void M_DOCS_ColumnChangedA(MyItemPropertyChangedEventArgs e)
        {
            var table = MyData.DbContextM.BL_M_DOCS;
            var dr = e.Item as M_DOCS;
            if (e.PropertyName == nameof(M_DOCS.TP) ||
                e.PropertyName == nameof(M_DOCS.StoreIn) ||
                e.PropertyName == nameof(M_DOCS.StoreOut))
            {
                var doctype = dr.XDocType;
                var storeintype = dr.XStoreInType;
                var storeouttype = dr.XStoreOutType;
                var storeinpvntype = dr.XStoreInPVNType;
                var storeoutpvntype = dr.XStoreOutPVNType;
                var pvntype = SomeDataDefsM.GetPVNType(doctype, storeintype, storeouttype,
                    storeinpvntype, storeoutpvntype);
                dr.XPVNType = pvntype;
                DataTasksM.UpdateDocAcc(dr);
            }
            if (e.PropertyName == nameof(M_DOCS.StoreIn))
            {
                dr.AddressIn = null;
            }
            if (e.PropertyName == nameof(M_DOCS.StoreOut))
            {
                dr.AddressOut = null;
            }
            if (e.PropertyName == nameof(M_DOCS.Carrier))
            {
                dr.Driver = null;
                dr.Vehicle = null;
            }
            if (e.PropertyName == nameof(M_DOCS.TP))
            {
                if(dr.XDocType != EDocType.Kredītrēķins_no_piegādātāja &&
                    dr.XDocType != EDocType.Kredītrēķins_pircējam &&
                    dr.CREDDOCDT != null)
                {
                    dr.IDCREDDOC = null;
                    dr.CREDDOCDT = null;
                    dr.CREDDOCSR = null;
                    dr.CREDDOCNR = null;
                }
            }
        }

        private decimal RoundPrice(decimal price)
        {
            return Math.Round(price, DecimalsInPrices);
        }

        private void M_ROWS_ColumnChangedA(MyItemPropertyChangedEventArgs e)
        {
            var table = MyData.DbContextM.BL_M_ROWS;
            var table_items = MyData.DbContextM.BL_M_ITEMS;
            var dr_doc = GetCurrentDocRow();
            if (e.PropertyName == nameof(M_ROWS.Item))
            {
                var dr = e.Item as M_ROWS;
                var dr_item = dr.Item;
                if (dr_item == null) return;
                if(dr_doc.XDocType == EDocType.Realizācija ||
                    dr_doc.XDocType == EDocType.Sniegti_pakalpojumi ||
                    dr_doc.XDocType == EDocType.Pārvietots)
                {
                    dr.PRICE0 = RoundPrice(dr_item.SELLPRICE);
                    dr.PRICE = RoundPrice(dr_item.SELLPRICE);
                    dr.DISCOUNT = 0f;
                }
                else if (dr_doc.XDocType == EDocType.Saražots)
                {
                    dr.PRICE0 = RoundPrice(dr_item.PRODCOST);
                    dr.PRICE = RoundPrice(dr_item.PRODCOST);
                    dr.DISCOUNT = 0f;
                }
                else
                {
                    dr.DISCOUNT = 0f;
                    dr.PRICE0 = 0M;
                    dr.PRICE = 0M;
                }
                dr.UNITS = dr_item.UNITS;
                dr.UnitsRow = dr_item.UnitsRow;
                dr.IDPVNRATE = dr_item.PVNRATE;
                dr.PVNRate = dr_item.PVNRateRow;
                dr.ACC6 = dr_item.Category.ACC6;
                dr.ACC7 = dr_item.Category.ACC7;
                dgvRows.RefreshCurrent();
            }

            if (e.PropertyName == nameof(M_ROWS.Item) ||
                e.PropertyName == nameof(M_ROWS.PRICE0) ||
                e.PropertyName == nameof(M_ROWS.DISCOUNT) ||
                e.PropertyName == nameof(M_ROWS.AMOUNT))
            {
                var dr = e.Item as M_ROWS;
                var price = RoundPrice(dr.PRICE0 + (decimal)dr.DISCOUNT / 100M * dr.PRICE0);
                var tprice = Math.Round(dr.AMOUNT * price, 2);
                if (dr.PRICE != price) dr.PRICE = price;
                if (dr.TPRICE != tprice) dr.TPRICE = tprice;
                if (SomeDataDefsM.PriceIsBuyPrice(dr_doc.XDocType))
                {
                    if (dr.PRICE != dr.BUYPRICE) dr.BUYPRICE = dr.PRICE;
                    if (dr.TPRICE != dr.TBUYPRICE) dr.TBUYPRICE = dr.TPRICE;
                }
                UpdateDocSums(dr_doc);
                dgvRows.RefreshCurrent();
                //TODO -- dgvDocs.RefreshCurrent();
            }

        }

        private bool UpdateDocSums(M_DOCS dr_doc)
        {
            var drs_rows = dr_doc.Rows.ToList();
            var err = DataTasksM.CheckDocHeader(dr_doc);
            if (err.HasErrors)
            {
                if (dr_doc.SUMM != 0M) dr_doc.SUMM = 0M;
                FormM_ErrorList.ShowErrorList(this, err);
                return false;
            }
            if (drs_rows.Count == 0)
            {
                if (dr_doc.SUMM != 0M) dr_doc.SUMM = 0M;
                return true;
            }
            var docpvndata = PVNCalc.GetPVNData(dr_doc);
            if (docpvndata.Err.HasErrors)
            {
                if (dr_doc.SUMM != 0M) dr_doc.SUMM = 0M;
                FormM_ErrorList.ShowErrorList(this, docpvndata.Err);
                return false;
            }
            decimal summ = docpvndata.PVNBase + docpvndata.PVN;
            if (dr_doc.SUMM != summ)
                dr_doc.SUMM = summ;
            return true;
        }


        private void GetDocTpCellValue(MyGridRowPickRowTextBox grow)
        {
            var id = (int)grow.Value;
            int? new_id = FormM_DocTypes.GetDocTypeId(id);
            if (!new_id.HasValue) return;
            grow.Value = new_id.Value;
        }

        private void GetDocTransTypeCellValue(MyGridRowPickRowTextBox grow)
        {
            var dr_trtype = (M_TRANSACTIONTYPE)grow.Value;
            var dr_New = FormM_TransactionType.GetTransactionType(dr_trtype);
            if (dr_New == null) return;
            grow.Value = dr_New;
        }

        private void GetDocStoreCellValue(MyGridRowPickRowTextBox grow)
        {
            var dr_cur = (M_STORES)grow.Value;
            var dr_new = FormM_Stores.GetStore(dr_cur);
            if (dr_new == null) return;
            grow.Value = dr_new;
        }

        private void GrDocTP_ButtonClicked(object sender, EventArgs e)
        {
            var grow = (MyGridRowPickRowTextBox)sender;
            GetDocTpCellValue(grow);
        }

        private void GetDocAccCellValue(MyGridRowPickRowTextBox grow)
        {
            var code = (string)grow.Value;
            string new_code = FormM_Accounts.GetAccCode(code);
            if (new_code.IsNOE()) return;
            grow.Value = new_code;
        }

        private void GetDocPVNTypeCellValue(MyGridRowPickRowTextBox grow)
        {
            var id = (int)grow.Value;
            int? new_id = FormM_PVNTypes.GetPVNTypeId(id);
            if (!new_id.HasValue) return;
            grow.Value = new_id.Value;
        }


        private void GrDocTransType_ButtonClicked(object sender, EventArgs e)
        {
            var grow = (MyGridRowPickRowTextBox)sender;
            GetDocTransTypeCellValue(grow);
        }

        private void GrDocStore_ButtonClicked(object sender, EventArgs e)
        {
            var grow = (MyGridRowPickRowTextBox)sender;
            GetDocStoreCellValue(grow);
        }

        private void GrDocAddressInOut_ButtonClicked(object sender, EventArgs e)
        {
            var grow = (MyGridRowPickRowTextBox)sender;
            if (grow != grDocAddressOut && grow != grDocAddressIn) return;
            var dr_store = grow == grDocAddressOut ? docData1._StoreOut : docData1._StoreIn;
            var mainstore = MyData.Params.MAINSTORE;
            var store_type = dr_store.XStoreType;
            if (!mainstore.IsNOE() && store_type == EStoreType.Noliktava)
            {
                var dr_store_main = DataTasksM.GetStoreIdByCode(mainstore);
                if (dr_store_main != null)
                    dr_store = dr_store_main;
            }
            var dr_selected = FormM_Addresses.GetAddress(dr_store);
            if (dr_selected == null) return;
            grow.Value = dr_selected;
        }

        private void GrDocDriver_ButtonClicked(object sender, EventArgs e)
        {
            var grow = sender as MyGridRowPickRowTextBox;
            if (grow != grDocDriver) return;
            var dr_store = docData1._Carrier;
            if (dr_store == null) return;
            var mainstore = MyData.Params.MAINSTORE;
            var store_type = dr_store.XStoreType;
            if (!mainstore.IsNOE() && store_type == EStoreType.Noliktava)
            {
                var dr_store_main = DataTasksM.GetStoreIdByCode(mainstore);
                if (dr_store_main != null)
                    dr_store = dr_store_main;
            }
            var dr_selected = FormM_Contacts.GetContact(dr_store);
            if (dr_selected == null) return;
            grow.Value = dr_selected;
        }

        private void GrDocVehicle_ButtonClicked(object sender, EventArgs e)
        {
            var grow = sender as MyGridRowPickRowTextBox;
            if (grow != grDocVehicle) return;
            var dr_store = docData1._Carrier;
            if (dr_store == null) return;
            var mainstore = MyData.Params.MAINSTORE;
            var store_type = dr_store.XStoreType;
            if (!mainstore.IsNOE() && store_type == EStoreType.Noliktava)
            {
                var dr_store_main = DataTasksM.GetStoreIdByCode(mainstore);
                if (dr_store_main != null)
                    dr_store = dr_store_main;
            }
            var dr_selected = FormM_Vehicles.GetVehicle(dr_store);
            if (dr_selected == null) return;
            grow.Value = dr_selected;
        }

        private void GrDocAcc_ButtonClicked(object sender, EventArgs e)
        {
            var grow = (MyGridRowPickRowTextBox)sender;
            GetDocAccCellValue(grow);
        }

        private void GrDocPVNType_ButtonClicked(object sender, EventArgs e)
        {
            var grow = (MyGridRowPickRowTextBox)sender;
            GetDocPVNTypeCellValue(grow);
        }

        private M_DOCS GetCurrentDocRow()
        {
            if (bsDocs.Count == 0 || bsDocs.Current == null) return null;
            var dr_doc = bsDocs.GetCurrentItem<M_DOCS>();
            return dr_doc;
        }

        private M_DOCS GetGoodCurrentDocRow()
        {
            var dr_doc = GetCurrentDocRow();
            if (dr_doc == null) return null;
            if (!SaveData())
            {
                return null;
            }
            if (bsDocs.HasChanges(false))
            {
                MyMainForm.ShowWarning("Ir nesaglabāti dati.");
                return null;
            }
            return dr_doc;
        }


        private void CheckEnableRows()
        {
            bool sgrdoca_visible = bsDocs.Count > 0 && bsDocs.Current != null;
            bool dgvRows_enabled = bsDocs.Count > 0 && bsDocs.Current != null;
            SetControlVisible(sgrDocA, sgrdoca_visible);
            SetControlEnabled(dgvRows, dgvRows_enabled);
        }

        private void CheckEnableDocsCheckBoxes()
        {
            var b = CanEditCurrentDoc();
            grDocWeVATPayer.ReadOnly = !b;
        }

        public override bool SaveData()
        {
            if (!dgvRows.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = bsDocs.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private bool HasChanges()
        {
            return bsDocs.HasChanges(false);

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
            bNav.DeleteCurrent();
            SaveData();
        }

        public bool CanEditDoc(M_DOCS dr_doc)
        {
            if (dr_doc == null) return false;
            return dr_doc.IsOpenForChanges;
        }

        public bool CanEditCurrentDoc()
        {
            if (bsDocs.Count == 0 || bsDocs.Current == null) return false;
            var dr_doc = bsDocs.GetCurrentItem<M_DOCS>();
            if (dr_doc == null) return false;
            return dr_doc.IsOpenForChanges;
        }

        public M_STORES GetStoreId(M_STORES dr_store, EStoreType storefilter = EStoreType.Nenoteikts)
        {
            return FormM_Stores.GetStore(dr_store, storefilter);
        }

        public M_ITEMS GetItemId(M_ITEMS dr_item)
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

        private void SetCurrentRowsItemId(M_ITEMS dr_item)
        {
            if (dgvRows.CurrentCell == null) return;
            try
            {
                if (dr_item == null) return;
                dgvRows.BeginEdit(false);
                var s = dgvRows.EditingControl.GetType().Name;
                if (dgvRows.EditingControl is MyMcComboBox)
                {
                    var c = dgvRows.EditingControl as MyMcComboBox;
                    c.SelectedValue = dr_item;
                }
                else if (dgvRows.EditingControl is MyPickRowTextBox2)
                {
                    var c = dgvRows.EditingControl as MyPickRowTextBox2;
                    c.SelectedValue = dr_item;
                }
                dgvRows.EndEdit();
            }
            catch (Exception) { }
        }


        public void GetDocStore()
        {
            if (!CanEditDocsCurrentCell()) return;
            var dv = (M_STORES)((MyDgvTextboxEditingControl2)ActiveControl).SelectedValue;
            var rt = GetStoreId(dv);
            if (rt == null) return;
            SetCurrentDocEditorValue(rt);
        }

        public void GetRowItem()
        {
            if (!CanEditRowsCurrentCell()) return;
            var cv = (M_ITEMS)dgvRows.CurrentCell.Value;
            var ret = GetItemId(cv);
            if (ret == null) return;
            SetCurrentRowsItemId(ret);
        }

        public void SelectDoc(M_DOCS dr_doc)
        {
            bsDocs.SelectItem(dr_doc);
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
            var dr0 = bsRows.GetItem<M_ROWS>(startindex);
            if (dr0 == null) return -1;
            var propdef = TypeDescriptor.GetProperties(dr0)[propname];
            if (propdef == null) return -1;
            int di = forward ? 1 : -1;
            string val;
            text = text.ToLower();
            for (int i = startindex; i >= 0 && i < bsRows.Count; i += di)
            {
                var dr = bsRows.GetItem<M_ROWS>(i);
                if (dr == null) continue;
                val = null;
                if (colindex == dgcRowsItemName.Index)
                    val = dr.Item.NAME;
                else if (colindex == dgcRowsIdItem.Index)
                    val = dr.Item.BARCODE;
                else if (colindex == dgcRowsIdPVNRate.Index)
                    val = dr.PVNRate.CODE;
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
            if (e.ColumnIndex == dgcRowsUnits.Index)
            {
                var dr = (M_UNITS)e.Value;
                if (dr == null) return;
                e.Value = dr.CODE;
                e.FormattingApplied = true;
            }
            if (e.ColumnIndex == dgcRowsIdPVNRate.Index)
            {
                var dr = (M_PVNRATES)e.Value;
                if (dr == null) return;
                e.Value = dr.CODE;
                e.FormattingApplied = true;
            }

        }

        private void dgvRows_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.Value == null || e.Value == DBNull.Value) return;
            if (e.ColumnIndex == dgcRowsPrice0.Index)
            {
                decimal price;
                if (e.Value is string sval)
                {
                    if (!decimal.TryParse(sval, out price)) return;
                }
                else if (e.Value is decimal)
                {
                    price = (decimal)e.Value;
                }
                else
                    return;
                decimal rprice = RoundPrice(price);
                if (price == rprice) return;
                e.Value = rprice;
                e.ParsingApplied = true;
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
            dgvRows.MoveToNewRow();
        }

        private void dgvRows_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!CanEditCurrentDoc()) return;
            if (e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void bNav_ItemDeleting(object sender, CancelEventArgs e)
        {
            if (!CanEditCurrentDoc()) return;
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
            CheckEnableDocsCheckBoxes();
            CheckSave();
        }

        private void bsDocs_CurrentChanged(object sender, EventArgs e)
        {
            CheckEnableRows();
            CheckEnableDocsCheckBoxes();
        }

        private void bsRows_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            if (e.ListChangedType == ListChangedType.Reset) return;
            CheckSave();
        }

        private void dgvRows_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            var table_rows = MyData.DbContextM.BL_M_ROWS;
            var sq = table_rows.Any() ? table_rows.Max(x => x.IDSEQ) + 1 : 1;
            e.Row.Cells[dgcRowsIdSeq.Index].Value = sq;
        }

        void IMyDgvTextboxEditingControlEvents2.OnButtonClicked(MyDgvTextboxEditingControl2 control)
        {
            if (control.DataSource == bsStoreIn || control.DataSource == bsStoreOut)
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

        private void dgvRows_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            var dr_row = bsRows.GetItem<M_ROWS>(e.RowIndex);
            if (dr_row == null) return;
            var dr_doc = GetCurrentDocRow();
            if (!CanEditDoc(dr_doc))
                e.Cancel = true;
        }

        private void sgrDocA_ConvertingValueToDisplayString(object sender, DevAge.ComponentModel.ConvertingObjectEventArgs e)
        {
            if (sender == grDocState)
            {
                var state = (EDocState)e.Value;
                var val = SomeDataDefsM.GetDocStateText(state);
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

        public bool DoIegrāmatot()
        {
            var dr_doc = GetGoodCurrentDocRow();
            if (dr_doc == null) return false;
            if (!dr_doc.IsOpenForChanges)
            {
                MyMainForm.ShowWarning("Dokuments jau ir iegrāmatots.");
                return false;
            }
            if (!UpdateDocSums(dr_doc)) return false;
            if (!SaveData())
            {
                MyMainForm.ShowError("Neizdevās saglabāt izmaiņas.");
                return false;
            }
            var err = DataTasksM.ProcessDoc(dr_doc);
            CheckSave();
            if (err.HasErrors)
            {
                FormM_ErrorList.ShowErrorList(this, err);
                return false;
            }
            return true;
        }

        public void DoAtvērt()
        {
            var dr_doc = GetGoodCurrentDocRow();
            if (dr_doc == null) return;
            if (dr_doc.IsOpenForChanges)
            {
                MyMainForm.ShowWarning("Dokuments jau ir atvērts.");
                return;
            }
            if (!SaveData())
            {
                MyMainForm.ShowError("Neizdevās saglabāt izmaiņas.");
                return;
            }
            var err = DataTasksM.OpenDoc(dr_doc);
            CheckSave();
            if (err.HasErrors)
            {
                FormM_ErrorList.ShowErrorList(this, err);
                return;
            }
            DataTasksM.DetachFinDocByIdDocM(dr_doc.ID);
        }

        public bool DoIegrānatotVeicotPilnuAprēķinu()
        {
            var dr_doc = GetGoodCurrentDocRow();
            if (dr_doc == null) return false;
            if (!dr_doc.IsOpenForChanges)
            {
                MyMainForm.ShowWarning("Dokuments nav atvērts.");
                return false;
            }
            if (!UpdateDocSums(dr_doc)) return false;
            DataTasksM.UpdateIdSeq(dr_doc);
            if (!SaveData())
            {
                MyMainForm.ShowError("Neizdevās saglabāt izmaiņas.");
                return false;
            }
            var err = DataTasksM.RecalcDoc(dr_doc, EDocState.Iegrāmatots);
            CheckSave();
            if (err.HasErrors)
            {
                FormM_ErrorList.ShowErrorList(this, err);
                return false;
            }
            return true;
        }

        public bool DoAtvērtVeicotPilnuAprēķinu()
        {
            var dr_doc = GetGoodCurrentDocRow();
            if (dr_doc == null) return false;
            if (dr_doc.IsOpenForChanges)
            {
                MyMainForm.ShowWarning("Dokuments jau ir atvērts.");
                return false;
            }
            if (!UpdateDocSums(dr_doc)) return false;
            if (!SaveData())
            {
                MyMainForm.ShowError("Neizdevās saglabāt izmaiņas.");
                return false;
            }
            var err = DataTasksM.RecalcDoc(dr_doc, EDocState.Atvērts);
            CheckSave();
            if (err.HasErrors)
            {
                FormM_ErrorList.ShowErrorList(this, err);
                return false;
            }
            return true;
        }

        public bool DoIzveidotKredītrēķinu()
        {
            var dr_doc = GetGoodCurrentDocRow();
            if (dr_doc == null) return false;
            if (!UpdateDocSums(dr_doc)) return false;
            if (!SaveData())
            {
                MyMainForm.ShowError("Neizdevās saglabāt izmaiņas.");
                return false;
            }
            var err = DataTasksM.MakeCreditDoc(dr_doc, out var new_dr_doc);
            CheckSave();
            if (err != "OK")
            {
                MyMainForm.ShowWarning(err);
                return false;
            }
            SelectDoc(new_dr_doc);
            return true;
        }

        public bool DoPrečuAtgriešanaCenasAprēķins()
        {
            var dr_doc = GetGoodCurrentDocRow();
            if (dr_doc == null) return false;
            if (!UpdateDocSums(dr_doc)) return false;
            if (!SaveData())
            {
                MyMainForm.ShowError("Neizdevās saglabāt izmaiņas.");
                return false;
            }
            if (!dr_doc.IsOpenForChanges)
            {
                MyMainForm.ShowWarning("Dokuments nav atvērts rediģēšnai.");
                return false;
            }
            ErrorList err;
            if (dr_doc.XDocType == EDocType.Atgriezts_piegādātājam)
            {
                err = DataTasksM.CheckDocForAtgrieztsPiegadatajam(dr_doc, true);
            }
            else if (dr_doc.XDocType == EDocType.Atgriezts_no_pircēja)
            {
                err = DataTasksM.CheckDocForAtgrieztsNoPircēja(dr_doc, true);
            }
            else
            {
                MyMainForm.ShowWarning("Dokumenta vaidam jābūt Atgriezts piegādātājam vai Atgriezts no pircēja.");
                return false;
            }
            CheckSave();
            if (err.HasErrors)
            {
                FormM_ErrorList.ShowErrorList(this, err);
                return false;
            }
            return true;
        }

        public bool DoPrečuAtgriešanaIzveidotKredītrēķinus()
        {
            var dr_doc = GetGoodCurrentDocRow();
            if (dr_doc == null) return false;
            if (!UpdateDocSums(dr_doc)) return false;
            if (!SaveData())
            {
                MyMainForm.ShowError("Neizdevās saglabāt izmaiņas.");
                return false;
            }
            if (!dr_doc.IsOpenForChanges)
            {
                MyMainForm.ShowWarning("Dokuments nav atvērts rediģēšnai.");
                return false;
            }
            ErrorList err;
            if (dr_doc.XDocType == EDocType.Atgriezts_piegādātājam)
            {
                err = DataTasksM.CheckDocForAtgrieztsPiegadatajam(dr_doc, true);
                if (!err.HasErrors)
                    err = DataTasksM.MakeCreditDocsFromAtgrieztsPiegadatajam(dr_doc);
            }
            else if (dr_doc.XDocType == EDocType.Atgriezts_no_pircēja)
            {
                err = DataTasksM.CheckDocForAtgrieztsNoPircēja(dr_doc, true);
                if (!err.HasErrors)
                    err = DataTasksM.MakeCreditDocsFromAtgrieztsNoPirceja(dr_doc);
            }
            else
            {
                MyMainForm.ShowWarning("Dokumenta vaidam jābūt Atgriezts piegādātājam vai Atgriezts no pircēja.");
                return false;
            }
            CheckSave();
            if (err.HasErrors)
            {
                FormM_ErrorList.ShowErrorList(this, err);
                return false;
            }
            return true;
        }

        public bool CanDoAccounting()
        {
            var dr_doc = GetGoodCurrentDocRow();
            if (dr_doc == null) return false;
            if (dr_doc.XState != EDocState.Iegrāmatots &&
                dr_doc.XState != EDocState.Iekontēts)
            {
                MyMainForm.ShowWarning("Dokuments nav iegrāmatots.");
                return false;
            }
            return true;
        }

        public void DoAccounting(bool after_iegramatots)
        {
            var dr_doc = GetGoodCurrentDocRow();
            if (dr_doc == null) return;
            if (!SomeDataDefsM.AutoMakeFinOps(dr_doc.XDocType))
            {
                if (after_iegramatots)
                {
                    dr_doc.XState = EDocState.Iegrāmatots;
                    dr_doc.XDoAutoFinOps = false;
                    SaveData();
                }
                else
                {
                    MyMainForm.ShowWarning("Šis dokuments veids netiek kontēts.\n" +
                        "Kontēts tiek mēneša kopsavilkums.");
                }
                return;
            }
            FormM_DocFin.ShowDialog(dr_doc);
            CheckSave();
        }

        public void DoCopyDoc()
        {
            var dr = GetCurrentDocRow();
            if (dr == null) return;
            if (!SaveData()) return;
            if (!MyMainForm.AskSomething("Vai kopēt dokumentu?", this)) return;
            DataTasksM.CopyDoc(dr, out var dr_new);
            FindDoc(dr_new);
        }

        public void DoDeleteDoc()
        {
            var dr = GetCurrentDocRow();
            if (dr == null) return;
            if (!dr.IsOpenForChanges)
            {
                MyMainForm.ShowWarning("Dokuments nav atvērts rediģēšanai.");
                return;
            }
            if (!AskCanDelete()) return;
            bsDocs.RemoveCurrent();
            if (!SaveData()) return;
            Close();
        }

        public void DoPrint()
        {
            if (!SaveData()) return;
            var dr_doc = GetCurrentDocRow();
            if (dr_doc == null) return;
            MyMainForm.ShowFormDialog(() => new FormM_Invoice(dr_doc, 0));
        }

        public void DoCostRep()
        {
            var dr_doc = GetCurrentDocRow();
            if (dr_doc == null) return;
            FormM_DocCosts.ShowRep(dr_doc);
        }

        public bool AreWeVATPayer => !MyData.Params.CompRegNrPVN.IsNOE();

        public void DoSplitPVN()
        {
            var dr_doc = GetCurrentDocRow();
            if (dr_doc == null) return;
            if (!dr_doc.IsOpenForChanges)
            {
                MyMainForm.ShowWarning("Dokuments nav atvērts rediģēšnai.");
                return;
            }
            if (dr_doc.XWeVATPayer)
            {
                MyMainForm.ShowWarning("Dokumenta ir atzīme, ka esam PVN maksātājs.\n" +
                    "Cenas pārrēķins pieskaitot PVN nav jāveic.");
                return;
            }
            if(dr_doc.XDocType != EDocType.Iepirkums)
            {
                MyMainForm.ShowWarning("Cenas pārrēķins pieskaitot PVN būtu jāveic tikai iepirkuma reķīniem.");
                return;
            }
            FormM_DocSplitPVN.ShowRep(dr_doc);
        }

        public void DoPricesAndDiscounts()
        {
            var dr_doc = GetCurrentDocRow();
            if (dr_doc == null) return;
            if (!dr_doc.IsOpenForChanges)
            {
                MyMainForm.ShowWarning("Dokuments nav atvērts rediģēšnai.");
                return;
            }
            if (dr_doc.XDocType != EDocType.Realizācija)
            {
                MyMainForm.ShowWarning("Cenas un atlaides tiek piemērotas tikai realizācijas dokumentiem.");
                return;
            }
            FormM_DocPricesAndDiscounts.ApplyPricesAndDiscounts(dr_doc);
        }

        public void DoImportRows()
        {
            var dr_doc = GetCurrentDocRow();
            if (dr_doc == null) return;
            if (!dr_doc.IsOpenForChanges)
            {
                MyMainForm.ShowWarning("Dokuments nav atvērts rediģēšnai.");
                return;
            }

            var fm = new FormM_ImportPrices(FormM_ImportPrices.EImportMode.DocRows);
            var rt = fm.ShowDialog();
            if (rt != DialogResult.OK || fm.Result.Count == 0) return;

            var table_rows = MyData.DbContextM.BL_M_ROWS;

            foreach (var row_data in fm.Result)
            {
                int iditem = row_data.IdItem.Value;
                var dr_new = table_rows.NewItem();
                dr_new.IDDOC = dr_doc.ID;
                dr_new.Doc = dr_doc;
                dr_new.IDITEM = iditem;
                dr_new.AMOUNT = row_data.Amount;
                dr_new.PRICE = row_data.Price;
                table_rows.Add(dr_new);
            }
            CheckSave();
            MyMainForm.ShowInfo("Datu imports pabeigts.", "", this);
        }

        private void sgrDocA_EditStarting(object sender, CancelEventArgs e)
        {
            var dr_doc = GetCurrentDocRow();
            if (!CanEditCurrentDoc() || !CanEditDoc(dr_doc))
            {
                e.Cancel = true;
            }
        }

        private void kopētDokumentuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoCopyDoc();
        }
        private void dzēstDokumentuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoDeleteDoc();
        }
        private void iegrāmatotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool rt = DoIegrāmatot();
            if (!rt) return;
            DoAccounting(true);
        }
        private void atvērtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoAtvērt();
        }
        private void atvērtRediģēšanaiVeicotPilnuPārrēķinuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoAtvērtVeicotPilnuAprēķinu();
        }
        private void iegrāmatotVeicotPilnuPārrēķinuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool rt = DoIegrānatotVeicotPilnuAprēķinu();
            if (!rt) return;
            DoAccounting(true);
        }
        private void izveidotKredītrēķinuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoIzveidotKredītrēķinu();
        }
        private void prečuAtgriešanaCenasAprēķinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoPrečuAtgriešanaCenasAprēķins();
        }
        private void prečuAtgriešanaIzveidotKredītrēķinusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoPrečuAtgriešanaIzveidotKredītrēķinus();
        }
        private void kontējumsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoAccounting(false);
        }
        private void miIzmaksuKopsavilkums_Click(object sender, EventArgs e)
        {
            DoCostRep();
        }
        private void pavadzīmeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoPrint();
        }
        private void miSplitPVN_Click(object sender, EventArgs e)
        {
            DoSplitPVN();
        }

        private void miPricesAndDiscounts_Click(object sender, EventArgs e)
        {
            DoPricesAndDiscounts();
        }

        private void miImportRows_Click(object sender, EventArgs e)
        {
            DoImportRows();
        }
    }
}
