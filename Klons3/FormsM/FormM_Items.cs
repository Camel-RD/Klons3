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
using KlonsF.Classes;
using KlonsLIB.Components;
using Cyotek.Windows.Forms;
using Klons3.ModelsM;
using Equin.ApplicationFramework;
using KlonsLIB.Misc;

namespace KlonsM.FormsM
{
    public partial class FormM_Items : MyFormBaseF, IMyDgvTextboxEditingControlEvents2
    {
        public FormM_Items()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            LoadColumnWidthsFromSettings();
            bsStore.SetFilter<M_STORES>(x => x.TP == 2 || x.ID == 1);
        }

        private void FormM_Items_Load(object sender, EventArgs e)
        {
            tbCode.Focus();
            tbItemsCatFilter.SelectedIndex = -1;
            if (SelectedItem != null)
                bsItems.SelectItem(SelectedItem);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            panel1.Height = tbCode.Bottom + tbCode.Top + 1;
        }

        private void FormM_Items_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void LoadColumnWidthsFromSettings()
        {
            string scw = MyData.Settings.ColumnWidths_MItems;
            (int ver, int[] cw) = dgvRows.ParseColumnWidths(scw);
            if (ver == 1 && cw != null && cw.Length > 0)
                dgvRows.SetColumnWidths(cw);
        }

        private void SaveColumnWidthsToSettings()
        {
            MyData.Settings.ColumnWidths_MItems = dgvRows.GetColumnWidths2(10f, 1);
        }

        public override void SaveParams()
        {
            SaveColumnWidthsToSettings();
        }

        private void BsItems_ListItemPropertyChanged(object sender, MyItemPropertyChangedEventArgs e)
        {
            var table = MyData.DbContextM.BL_M_ITEMS;
            var dr = e.Item as M_ITEMS;
            if (e.PropertyName == nameof(M_ITEMS.Category))
            {
                int isservice = dr.Category.ISSERVICES;
                int isproduced = dr.Category.ISPRODUCED;
                if (dr.ISSERVICE != isservice)
                    dr.ISSERVICE = isservice;
                if (dr.ISPRODUCED != isproduced)
                    dr.ISPRODUCED = isproduced;
            }
        }

        public M_ITEMS SelectedItem = null;
        private int FilterItemsCat = -1;
        private string FilterText = null;

        public static M_ITEMS GetItem(M_ITEMS dr_item)
        {
            var fm = new FormM_Items();
            //fm.tbCode.Text = code;
            fm.SelectedItem = dr_item;
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return null;
            return (M_ITEMS)fm.SelectedObject;
        }

        private void SelectCurrent()
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            var dr = bsItems.GetCurrentItem<M_ITEMS>();
            if (!dgvRows.EndEdit()) return;
            if (!SaveData()) return;
            if (bsItems.IsItemDetached(dr)) return;
            SelectedItem = dr;
            if (!this.IsMyDialog) return;
            SetSelectedObject(dr);
        }

        public void DoFilter()
        {
            int filteritemscat = tbItemsCatFilter.SelectedIndex;
            string filteritemscatcode = "";
            if (filteritemscat > -1)
            {
                var dr = (tbItemsCatFilter.SelectedItem as ObjectView<M_ITEMS_CAT>)?.Object;
                if (dr == null) return;
                filteritemscat = dr.ID;
                filteritemscatcode = dr.CODE;
            }
            string filtertext = tbFilter.Text;
            if (filteritemscat == FilterItemsCat && filtertext == FilterText) return;
            FilterItemsCat = filteritemscat;
            FilterText = filtertext;
            if (filteritemscat == -1 && string.IsNullOrEmpty(filtertext))
            {
                bsItems.RemoveFilter();
                return;
            }
            var preds = new List<Predicate<M_ITEMS>>();
            if (filteritemscat > -1)
                preds.Add(x => x.Category.CODE.StartsWithCI(filteritemscatcode));
            if (!string.IsNullOrEmpty(filtertext))
                preds.Add(x => x.NAME.ContainsCI(filtertext));
            bsItems.SetFilter(preds);
        }

        private int SearchText(string text, int colindex,
            int startindex = 0, bool forward = true)
        {
            string propname = dgvRows.Columns[colindex].DataPropertyName;
            if (bsItems.Count == 0) return -1;
            if (startindex < 0 || startindex >= bsItems.Count) return -1;
            if (string.IsNullOrEmpty(text)) return -1;
            if (startindex == 0 && !forward) return -1;
            if (startindex == bsItems.Count - 1 && forward) return -1;
            var dr0 = bsItemsCat.GetItem<M_ITEMS>(startindex);
            if (dr0 == null) return -1;
            var propdef = TypeDescriptor.GetProperties(dr0)[propname];
            if (propdef == null) return -1;
            int di = forward ? 1 : -1;
            object o;
            string val;
            text = text.ToLower();
            for (int i = startindex; i >= 0 && i < bsItems.Count; i += di)
            {
                var dr = bsItemsCat.GetItem<M_ITEMS>(i);
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

        public M_ITEMS_CAT GetCat(M_ITEMS_CAT dr_store_cur)
        {
            return FormM_ItemsCat.GetItemsCat(dr_store_cur);
        }

        public M_STORES GetStore(M_STORES dr_store_cur)
        {
            return FormM_Stores.GetStore(dr_store_cur, EStoreType.Noliktava);
        }

        public M_PVNRATES GetPVNRate(M_PVNRATES dr_pvnrate)
        {
            return FormM_PVNRates.GetPVNRate();
        }

        public M_UNITS GetUnit(M_UNITS dr_units)
        {
            return FormM_Units.GetUnitsId();
        }


        private bool CanEditCurrentCell()
        {
            if (bsItems.Count == 0 || bsItems.Current == null) return false;
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
                if (dgvRows.EditingControl is MyMcComboBox c1)
                    c1.SelectedValue = value;
                if (dgvRows.EditingControl is MyPickRowTextBox2 c2)
                    c2.SelectedValue = value;
                dgvRows.EndEdit();
            }
            catch (Exception) { }
        }

        public void GetCat()
        {
            if (!CanEditCurrentCell()) return;
            var dv = (M_ITEMS_CAT)dgvRows.CurrentCell.Value;
            var rt = GetCat(dv);
            if (rt == null) return;
            SetCurrentCellValue(rt);
        }

        public void GetStore()
        {
            if (!CanEditCurrentCell()) return;
            var dv = (M_STORES)dgvRows.CurrentCell.Value;
            var rt = GetStore(dv);
            if (rt == null) return;
            SetCurrentCellValue(rt);
        }

        public void GetPVNRate()
        {
            if (!CanEditCurrentCell()) return;
            var dv = (M_PVNRATES)dgvRows.CurrentCell.Value;
            var rt = GetPVNRate(dv);
            if (rt == null) return;
            SetCurrentCellValue(rt);
        }

        public void GetUnit()
        {
            if (!CanEditCurrentCell()) return;
            var dv = (M_UNITS)dgvRows.CurrentCell.Value;
            var rt = GetUnit(dv);
            if (rt == null) return;
            SetCurrentCellValue(rt);
        }


        private bool GetRowCellValue(object sender, int colind)
        {
            if (colind == dgcCat.Index)
            {
                GetCat();
                return true;
            }
            if (colind == dgcStore1.Index)
            {
                GetStore();
                return true;
            }
            if (colind == dgcPVNRate.Index)
            {
                GetPVNRate();
                return true;
            }
            if (colind == dgcUints.Index)
            {
                GetUnit();
                return true;
            }
            return false;
        }


        public override bool SaveData()
        {
            if (!dgvRows.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = bsItemsCat.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsItems.HasChanges());
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
            var dr = dgvRows.GetObjectViewItem<M_ITEMS>(rownr);
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

        private void bsItems_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void dgvRows_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvRows.CurrentCell == null) return;
            if (e.KeyCode == Keys.F4)
            {
                if (IsRow0(dgvRows.CurrentCell.ColumnIndex)) return;
                var rt = GetRowCellValue(dgvRows, dgvRows.CurrentCell.ColumnIndex);
                e.Handled = rt;
                return;
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
                SelectedItem = null;
                SetSelectedObject(null, true);
                e.Handled = true;
            }
        }

        private void dgvRows_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SelectedItem = null;
                SetSelectedObject(null, true);
            }
        }

        private void dgvRows_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            e.Cancel = e.Row.IsNewRow || IsRow0(e.Row.Index) || !AskCanDelete();
        }

        private void bNav_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = IsRow0(bsItems.Position) || !AskCanDelete();
        }

        private void dgvRows_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            if (e.ColumnIndex == dgcCode.Index)
            {
                SelectCurrent();
                return;
            }
            if (e.ColumnIndex == dgcCat.Index)
            {
                GetCat();
                return;
            }
            if (e.ColumnIndex == dgcStore1.Index)
            {
                GetStore();
                return;
            }
            if (e.ColumnIndex == dgcPVNRate.Index)
            {
                GetPVNRate();
                return;
            }
            if (e.ColumnIndex == dgcUints.Index)
            {
                GetUnit();
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
                SelectedItem = null;
                SetSelectedObject(null, true);
                return;
            }
            SelectCurrent();
        }

        private void tbCode_Enter(object sender, EventArgs e)
        {
            tbCode.SelectAll();
        }

        private void tbItemsCatFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tbItemsCatFilter.SelectedIndex == -1 && FilterItemsCat == -1) return;
            DoFilter();
        }

        private void tbItemsCatFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                tbItemsCatFilter.Text = null;
                DoFilter();
                e.Handled = true;
            }
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

        void IMyDgvTextboxEditingControlEvents2.OnButtonClicked(MyDgvTextboxEditingControl2 control)
        {
            if (control.DataSource == bsItemsCat)
            {
                GetCat();
                return;
            }
            if (control.DataSource == bsStore)
            {
                GetStore();
                return;
            }
            if (control.DataSource == bsPVNRate)
            {
                GetPVNRate();
                return;
            }
            if (control.DataSource == bsUnits)
            {
                GetUnit();
                return;
            }
        }

        public void DoItemCurrentStock()
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            var dr_item = bsItems.GetCurrentItem<M_ITEMS>();
            var fm = MyMainForm.ShowForm<FormM_ItemCurrentStock>();
            fm.MakeReport(dr_item.ID);

            //var fm = new FormM_ItemCurrentStock();
            //fm.MakeReport(dr_item.ID);
            //fm.ShowDialog(this);
        }

        public void DoImportSellPriceData()
        {
            if (bsItems.Count == 0) return;

            var fm = new FormM_ImportPrices();
            var rt = fm.ShowDialog();
            if (rt != DialogResult.OK || fm.Result.Count == 0) return;

            var table_items = MyData.DbContextM.BL_M_ITEMS;
            var dic_rowsbyid = table_items.ToDictionary(x => x.ID, x => x);

            foreach (var row_price in fm.Result)
            {
                int iditem = row_price.IdItem.Value;
                if (dic_rowsbyid.TryGetValue(iditem, out var dr_row))
                {
                    dr_row.SELLPRICE = row_price.Price;
                }
            }
            CheckSave();
            MyMainForm.ShowInfo("Datu imports pabeigts.", "", this);
        }


        public void DoImportCostData()
        {
            if (bsItems.Count == 0) return;

            var fm = new FormM_ImportPrices();
            var rt = fm.ShowDialog();
            if (rt != DialogResult.OK || fm.Result.Count == 0) return;

            var table_items = MyData.DbContextM.BL_M_ITEMS;
            var dic_rowsbyid = table_items.ToDictionary(x => x.ID, x => x);

            foreach (var row_price in fm.Result)
            {
                int iditem = row_price.IdItem.Value;
                if (dic_rowsbyid.TryGetValue(iditem, out var dr_row))
                {
                    dr_row.PRODCOST = row_price.Price;
                }
            }
            CheckSave();
            MyMainForm.ShowInfo("Datu imports pabeigts.", "", this);
        }

        private void tbItemsCatFilter_ButtonClicked(object sender, EventArgs e)
        {
            var dr_selected = (M_ITEMS_CAT)tbItemsCatFilter.SelectedValue;
            var dr_cat = FormM_ItemsCat.GetItemsCat(dr_selected);
            if (dr_cat == null) return;
            tbItemsCatFilter.SelectedValue = dr_cat;
        }

        private void miAktuālieArtikulaAtlikumi_Click(object sender, EventArgs e)
        {
            DoItemCurrentStock();
        }

        private void miImportētCenuLapu_Click(object sender, EventArgs e)
        {
            DoImportSellPriceData();
        }

        private void miImportētPašizmaksasDatus_Click(object sender, EventArgs e)
        {
            DoImportCostData();
        }

        private void miReloadFromDB_Click(object sender, EventArgs e)
        {
            DataLoaderM.LoadLatestItemsData();
        }
    }
}
