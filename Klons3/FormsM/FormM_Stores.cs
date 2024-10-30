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

namespace KlonsM.FormsM
{
    public partial class FormM_Stores : MyFormBaseF, IMyDgvTextboxEditingControlEvents2
    {
        public FormM_Stores()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            LoadColumnWidthsFromSettings();
            bsAccounts21.SetFilter<M_ACCOUNTS>(x => x.TP == 2 || x.TP == 1);
            bsAccounts23.SetFilter<M_ACCOUNTS>(x => x.TP == 5 || x.TP == 1);
            bsAccounts53.SetFilter<M_ACCOUNTS>(x => x.TP == 6 || x.TP == 1);
        }

        private void FormM_Stores_Load(object sender, EventArgs e)
        {
            tbCode.Focus();
            if (FilterStoreType == -1)
            {
                cbType.SelectedIndex = -1;
            }
            else
            {
                int k = bsStoreTypeFilter.Find("ID", FilterStoreType);
                cbType.SelectedIndex = k;
            }
            if (SelectedStore != null)
                FindStore(SelectedStore);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            panel1.Height = tbCode.Bottom + tbCode.Top + 1;
        }

        private void LoadColumnWidthsFromSettings()
        {
            string scw = MyData.Settings.ColumnWidths_MStores;
            (int ver, int[] cw) = dgvRows.ParseColumnWidths(scw);
            if (ver == 1 && cw != null && cw.Length > 0)
                dgvRows.SetColumnWidths(cw);
        }

        private void SaveColumnWidthsToSettings()
        {
            MyData.Settings.ColumnWidths_MStores = dgvRows.GetColumnWidths2(10f, 1);
        }

        public override void SaveParams()
        {
            SaveColumnWidthsToSettings();
        }

        private int FilterStoreType = -1;
        private string FilterText = null;
        private M_STORES SelectedStore = null;

        public static M_STORES GetStore(M_STORES dr_store, EStoreType storetypefilter = EStoreType.Nenoteikts)
        {
            var fm = new FormM_Stores();
            //fm.tbCode.Text = code;
            fm.SelectedStore = dr_store;
            if (storetypefilter != EStoreType.Nenoteikts)
            {
                fm.FilterStoreType = (int)storetypefilter;
                fm.DoFilterA();
            }
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return null;
            return (M_STORES)fm.SelectedObject;
        }

        private void SelectCurrent()
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            var dr = bsStores.GetCurrentItem<M_STORES>();
            if (!dgvRows.EndEdit()) return;
            if (!SaveData()) return;
            if (bsStores.IsItemDetached(dr)) return;
            SelectedStore = dr;
            if (!this.IsMyDialog) return;
            SetSelectedObject(dr);
        }

        public void FindStore(M_STORES dr_store)
        {
            bsStores.SelectItem(dr_store);
        }

        public void DoFilter()
        {
            int filterstoretype = -1;
            if (cbType.SelectedValue != null)
                filterstoretype = ((M_STORETYPE)cbType.SelectedValue).ID;
            string filtertext = tbFilter.Text;
            if (filterstoretype == FilterStoreType && filtertext == FilterText) return;
            FilterStoreType = filterstoretype;
            FilterText = filtertext;
            DoFilterA();
        }

        public void DoFilterA()
        {
            if (FilterStoreType == -1 && string.IsNullOrEmpty(FilterText))
            {
                bsStores.RemoveFilter();
                return;
            }
            var preds = new List<Predicate<M_STORES>>();
            if (FilterStoreType > -1)
                preds.Add(x => x.TP == FilterStoreType || x.ID == 1);
            if (!FilterText.IsNOE())
                preds.Add(x => x.NAME.ContainsCI(FilterText) || x.ADDR.ContainsCI(FilterText));
            bsStores.SetFilter(preds);
        }

        private int SearchText(string text, int colindex,
            int startindex = 0, bool forward = true)
        {
            string propname = dgvRows.Columns[colindex].DataPropertyName;
            if (bsStores.Count == 0) return -1;
            if (startindex < 0 || startindex >= bsStores.Count) return -1;
            if (string.IsNullOrEmpty(text)) return -1;
            if (startindex == 0 && !forward) return -1;
            if (startindex == bsStores.Count - 1 && forward) return -1;
            var dr0 = bsStores.GetItem<M_STORES>(startindex);
            if (dr0 == null) return -1;
            var propdef = TypeDescriptor.GetProperties(dr0)[propname];
            if (propdef == null) return -1;
            int di = forward ? 1 : -1;
            object o;
            string val;
            text = text.ToLower();
            for (int i = startindex; i >= 0 && i < bsStores.Count; i += di)
            {
                var dr = bsStores.GetItem<M_STORES>(i);
                o = propdef.GetValue(dr);
                if (o == null || o == DBNull.Value) continue;
                val = o.ToString().ToLower();
                if (val.Contains(text))
                {
                    return i;
                }
            }
            return -1;
        }

        private void SearchText(bool fromfirst = true, bool forward = true)
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            if (!dgvRows.EndEditX()) return;

            int startindex = dgvRows.CurrentRow.Index;
            startindex = fromfirst ? 0 : (forward ? startindex + 1 : startindex - 1);
            string text = tsbFind.Text;
            if (text == "") return;
            int newindex = SearchText(text, dgvRows.CurrentCell.ColumnIndex, startindex, forward);
            if (newindex == -1)
            {
                MyMainForm.ShowInfo("Teksts [" + text + "] netika atrasts.");
                return;
            }
            dgvRows.CurrentCell = dgvRows[dgvRows.CurrentCell.ColumnIndex, newindex];
        }

        public string GetAccountCode(string code, EAccountType accounttypefilter = EAccountType.Nenoteikts)
        {
            return FormM_Accounts.GetAccCode(code, accounttypefilter);
        }

        public M_STORETYPE GetStoreType(M_STORETYPE dr_storetype)
        {
            return FormM_StoreTypes.GetStoreType(dr_storetype);
        }

        public M_STORES_CAT GetStoreCat(M_STORES_CAT dr_storecat)
        {
            return FormM_StoresCat.GetStoresCat(dr_storecat);
        }

        public int? GetPVNTypeId(string code)
        {
            return FormM_PVNTypes.GetPVNTypeId(code);
        }

        private bool CanEditCurrentCell()
        {
            if (bsStores.Count == 0 || bsStores.Current == null) return false;
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return false;
            if (dgvRows.CurrentCell == null) return false;
            return true;
        }

        private void SetCurrentCellValue(object value)
        {
            if (dgvRows.CurrentCell == null) return;
            try
            {
                dgvRows.BeginEdit(false);
                if (dgvRows.EditingControl is MyMcComboBox c)
                    c.SelectedValue = value;
                if (dgvRows.EditingControl is MyPickRowTextBox2 c2)
                    c2.SelectedValue = value;
                dgvRows.EndEdit();
            }
            catch (Exception) { }
        }

        public void GetAccountCode()
        {
            if (!CanEditCurrentCell()) return;
            var dv = dgvRows.CurrentCell.FormattedValue as string;
            EAccountType accounttypefilter = EAccountType.Nenoteikts;
            int cind = dgvRows.CurrentCell.ColumnIndex;
            if (cind == dgcAcc21.Index) accounttypefilter = EAccountType.Noliktava;
            else if (cind == dgcAcc23.Index) accounttypefilter = EAccountType.Debitori;
            else if (cind == dgcAcc53.Index) accounttypefilter = EAccountType.Kreditori;
            var rt = GetAccountCode(dv, accounttypefilter);
            if (rt == null) return;
            SetCurrentCellValue(rt);
        }

        public void GetStoreType()
        {
            if (!CanEditCurrentCell()) return;
            var dv = (M_STORETYPE)dgvRows.CurrentCell.Value;
            var rt = GetStoreType(dv);
            if (dv == null) return;
            SetCurrentCellValue(rt);
        }

        public void GetStoreCat()
        {
            if (!CanEditCurrentCell()) return;
            var dv = (M_STORES_CAT)dgvRows.CurrentCell.Value;
            var rt = GetStoreCat(dv);
            if (rt == null) return;
            SetCurrentCellValue(rt);
        }

        public void GetPVNTypeId()
        {
            if (!CanEditCurrentCell()) return;
            var dv = dgvRows.CurrentCell.FormattedValue as string;
            var rt = GetPVNTypeId(dv);
            if (!rt.HasValue) return;
            SetCurrentCellValue(rt.Value);
        }

        public override bool SaveData()
        {
            if (!dgvRows.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = bsStores.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }


        private void CheckSave()
        {
            SetSaveButton(bsStores.HasChanges());
        }

        private void SetSaveButton(bool red)
        {
            bNav.SetSaveButtonRed(red);
        }

        public void DeleteCurrent()
        {
            bNav.DeleteCurrent();
            SaveData();
        }

        private bool IsRow0(int rownr)
        {
            if (rownr == dgvRows.NewRowIndex) return false;
            var dr = dgvRows.GetObjectViewItem<M_STORES>(rownr);
            if (dr == null) return false;
            return dr.ID == 1;
        }

        private void bindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void dgvRows_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void bsStores_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void dgvRows_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvRows.CurrentCell == null) return;
            if (e.KeyCode == Keys.F4)
            {
                int cind = dgvRows.CurrentCell.ColumnIndex;
                if (Utils.IN(cind, dgcAcc21.Index, dgcAcc23.Index, dgcAcc53.Index))
                {
                    GetAccountCode();
                    e.Handled = true;
                    return;
                }
                if (cind == dgcTP.Index)
                {
                    GetStoreType();
                    e.Handled = true;
                    return;
                }
                if (cind == dgcPVNTp.Index)
                {
                    GetPVNTypeId();
                    e.Handled = true;
                    return;
                }
                if (cind == dgcIdCat.Index)
                {
                    GetStoreCat();
                    e.Handled = true;
                    return;
                }
            }
            if (e.Control && e.KeyCode == Keys.Return)
            {
                SelectCurrent();
                e.Handled = true;
            }
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
            if (e.KeyCode == Keys.Escape)
            {
                SelectedStore = null;
                SetSelectedValueStr(null, true);
                e.Handled = true;
            }
        }

        private void dgvRows_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SelectedStore = null;
                SetSelectedValueStr(null, true);
            }
        }

        private void dgvRows_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            e.Cancel = e.Row.IsNewRow || IsRow0(e.Row.Index) || !AskCanDelete();
        }

        private void bNav_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = IsRow0(bsStores.Position) || !AskCanDelete();
        }

        private void dgvRows_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            if (e.ColumnIndex == dgcCode.Index)
            {
                SelectCurrent();
                return;
            }
            if (e.ColumnIndex == dgcAcc21.Index || e.ColumnIndex == dgcAcc23.Index ||
                e.ColumnIndex == dgcAcc53.Index)
            {
                GetAccountCode();
                //dgvRowsGetCellValue(sender, e.ColumnIndex);
                return;
            }
            if (e.ColumnIndex == dgcTP.Index)
            {
                GetStoreType();
                return;
            }
            if (e.ColumnIndex == dgcPVNTp.Index)
            {
                GetPVNTypeId();
                return;
            }
            if (e.ColumnIndex == dgcIdCat.Index)
            {
                GetStoreCat();
                return;
            }
        }

        private void dgvRows_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex == dgvRows.NewRowIndex) return;
            if (IsRow0(e.RowIndex))
                e.Cancel = true;
        }

        private void tbCode_RowSelectedEvent(object sender, RowSelectedEventArgs e)
        {
            if (e.RowNr == -1 || e.Value == null)
            {
                SelectedStore = null;
                SetSelectedValueStr(null, true);
                return;
            }
            SelectCurrent();
        }

        private void tbCode_Enter(object sender, EventArgs e)
        {
            tbCode.SelectAll();
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DoFilter();
        }

        private void tbFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                DoFilter();
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Escape)
            {
                tbFilter.Text = null;
                DoFilter();
                e.Handled = true;
            }
        }

        private void tbFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                SearchText();
                dgvRows.Focus();
                e.Handled = true;
                return;
            }
        }

        private void tsbFindPrev_Click(object sender, EventArgs e)
        {
            SearchText(false, false);
            dgvRows.Focus();
        }

        private void tsbFindNext_Click(object sender, EventArgs e)
        {
            SearchText(false, true);
            dgvRows.Focus();
        }

        private void tsbFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                SearchText(true, true);
                dgvRows.Focus();
                e.Handled = true;
            }
        }

        private void tsbFind_Enter(object sender, EventArgs e)
        {
            tsbFind.Text = "";
        }

        private void dgvRows_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            if (FilterStoreType > -1)
            {
                e.Row.Cells[dgcTP.Index].Value = FilterStoreType;
            }
        }

        public void ShowAktuālieAtlikumi()
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            var dr_store = bsStores.GetCurrentItem<M_STORES>();
            var frm = MyMainForm.ShowForm<FormM_StoreCurrentStock>();
            frm.MakeReport(dr_store.ID);
        }

        public void ShowAddresses()
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            var dr_store = bsStores.GetCurrentItem<M_STORES>();
            if (dr_store.ID == 1) return;
            FormM_Addresses.GetAddress(dr_store);
        }

        public void ShowContacts()
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            var dr_store = bsStores.GetCurrentItem<M_STORES>();
            if (dr_store.ID == 1) return;
            FormM_Contacts.GetContact(dr_store);
        }

        public void ShowBankAccounts()
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            var dr_store = bsStores.GetCurrentItem<M_STORES>();
            if (dr_store.ID == 1) return;
            FormM_BankAccounts.GetBankAccount(dr_store);
        }

        public void ShowVehicles()
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            var dr_store = bsStores.GetCurrentItem<M_STORES>();
            if (dr_store.ID == 1) return;
            FormM_Vehicles.GetVehicle(dr_store);
        }

        private void btAtlikumi_Click(object sender, EventArgs e)
        {
            ShowAktuālieAtlikumi();
        }

        private void btAddresses_Click(object sender, EventArgs e)
        {
            ShowAddresses();
        }

        private void btContacts_Click(object sender, EventArgs e)
        {
            ShowContacts();
        }

        private void btBankAccounts_Click(object sender, EventArgs e)
        {
            ShowBankAccounts();
        }

        private void btVehicles_Click(object sender, EventArgs e)
        {
            ShowVehicles();
        }

        void IMyDgvTextboxEditingControlEvents2.OnButtonClicked(MyDgvTextboxEditingControl2 control)
        {
            if (control.DataSource == bsStoreType)
            {
                GetStoreType();
                return;
            }
            if (control.DataSource == bsPVNType)
            {
                GetPVNTypeId();
                return;
            }
            if (control.DataSource == bsStoresCat)
            {
                GetStoreCat();
                return;
            }
            if (control.DataSource == bsAccounts21 ||
                control.DataSource == bsAccounts23 ||
                control.DataSource == bsAccounts53)
            {
                GetAccountCode();
                return;
            }
        }

        private void cbType_ButtonClicked(object sender, EventArgs e)
        {
            var dr_old = (M_STORETYPE)cbType.SelectedValue;
            var dr_new = FormM_StoreTypes.GetStoreType(dr_old);
            if (dr_new == null) return;
            cbType.SelectedValue = dr_new;
            DoFilter();
        }

        private void cbType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Delete)
            {
                cbType.Text = null;
                DoFilter();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Return)
            {
                e.Handled = true;
                DoFilter();
            }
        }

    }
}
