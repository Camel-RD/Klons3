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
using Klons3.ModelsM;

namespace KlonsM.FormsM
{
    public partial class FormM_DiscountList : MyFormBaseF, IMyDgvTextboxEditingControlEvents2
    {
        public FormM_DiscountList()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        private void FormM_DiscountList_Load(object sender, EventArgs e)
        {

        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            panel1.Height = tbLists.Bottom + tbLists.Top + 2;
        }

        private void CheckEnableRows()
        {
            bool enabledr = bsLists.Count > 0 && bsLists.Current != null;
            bool enabledp = bsLists.Count > 0 && bsLists.Current != null;
            SetControlEnabled(dgvRowsR, enabledr);
            SetControlEnabled(dgvRowsP, enabledp);
        }

        public override bool SaveData()
        {
            if (!dgvRowsR.EndEditX()) return false;
            if (!dgvRowsP.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = bsLists.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private bool HasChanges()
        {
            return bsLists.HasChanges() || bsRowsP.HasChanges() || bsRowsR.HasChanges();
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
            bNav.DeleteCurrent();
            SaveData();
        }

        public M_DISC_LISTS GetDiscountList()
        {
            return FormM_DiscountLists.GetListId();
        }

        public M_STORES GetStore(M_STORES dr_store, EStoreType storefilter = EStoreType.Partneris)
        {
            return FormM_Stores.GetStore(dr_store, storefilter);
        }

        public M_STORES_CAT GetStoreCat(M_STORES_CAT dr_storecat)
        {
            return FormM_StoresCat.GetStoresCat(dr_storecat);
        }

        public M_ITEMS GetItem(M_ITEMS dr_item)
        {
            return FormM_Items.GetItem(dr_item);
        }

        public M_ITEMS_CAT GetItemsCat(M_ITEMS_CAT dr_itemscat)
        {
            return FormM_ItemsCat.GetItemsCat(dr_itemscat);
        }

        private void SetCurrentCellValue(DataGridView dgv, object value)
        {
            if (dgv.CurrentCell == null) return;
            try
            {
                dgv.BeginEdit(false);
                if (dgv.EditingControl is MyMcComboBox c)
                    c.SelectedValue = value;
                if (dgv.EditingControl is MyPickRowTextBox2 c2)
                    c2.SelectedValue = value;
                dgv.EndEdit();
            }
            catch (Exception) { }
        }

        public void GetItem()
        {
            var cv = (M_ITEMS)dgvRowsR.CurrentCell.Value;
            var rt = GetItem(cv);
            if (rt == null) return;
            SetCurrentCellValue(dgvRowsR, rt);
        }

        public void GetItemsCat()
        {
            var dv = (M_ITEMS_CAT)dgvRowsR.CurrentCell.Value;
            var rt = GetItemsCat(dv);
            if (rt == null) return;
            SetCurrentCellValue(dgvRowsR, rt);
        }

        public void GetStoreCat()
        {
            var dv = (M_STORES_CAT)dgvRowsP.CurrentCell.Value;
            var rt = GetStoreCat(dv);
            if (rt == null) return;
            SetCurrentCellValue(dgvRowsP, rt);
        }

        public void GetStore()
        {
            var dv = (M_STORES)dgvRowsP.CurrentCell.Value;
            var rt = GetStore(dv);
            if (rt == null) return;
            SetCurrentCellValue(dgvRowsP, rt);
        }

        private void dgvRowsR_Enter(object sender, EventArgs e)
        {
            bNav.BindingSource = bsRowsR;
            bNav.DataGrid = dgvRowsR;
            tslActiveTable.Text = "Artikuli";
        }

        private void dgvRowsP_Enter(object sender, EventArgs e)
        {
            bNav.BindingSource = bsRowsP;
            bNav.DataGrid = dgvRowsP;
            tslActiveTable.Text = "Personas";
        }

        private int SearchRowTextR(string text, int colindex,
            int startindex = 0, bool forward = true)
        {
            string propname = dgvRowsR.Columns[colindex].DataPropertyName;
            if (bsRowsR.Count == 0) return -1;
            if (startindex < 0 || startindex >= bsRowsR.Count) return -1;
            if (string.IsNullOrEmpty(text)) return -1;
            if (startindex == 0 && !forward) return -1;
            if (startindex == bsRowsR.Count - 1 && forward) return -1;
            var dr0 = bsRowsR.GetItem<M_DISC_LISTS_R>(startindex);
            if (dr0 == null) return -1;
            var propdef = TypeDescriptor.GetProperties(dr0)[propname];
            if (propdef == null) return -1;
            int di = forward ? 1 : -1;
            string val;
            text = text.ToLower();
            for (int i = startindex; i >= 0 && i < bsRowsR.Count; i += di)
            {
                var dr = bsRowsR.GetItem<M_DISC_LISTS_R>(i);
                if (dr == null) continue;
                val = null;
                if (colindex == dgcRIdItemsCat.Index)
                    val = dr.ItemsCategory?.CODE;
                if (colindex == dgcRItemName.Index)
                    val = dr.Item?.NAME;
                else if (colindex == dgcRIdcItem.Index)
                    val = dr.Item?.BARCODE;
                if (val == null) continue;
                if (val.ToLower().Contains(text))
                {
                    return i;
                }
            }
            return -1;
        }

        private void SearchRowTextR(bool fromfirst = true, bool forward = true)
        {
            if (dgvRowsR.CurrentRow == null || dgvRowsR.CurrentRow.IsNewRow) return;
            if (!dgvRowsR.EndEditX()) return;

            int startindex = dgvRowsR.CurrentRow.Index;
            startindex = fromfirst ? 0 : (forward ? startindex + 1 : startindex - 1);
            string text = tsbFind.Text;
            if (text == "") return;
            int newindex = SearchRowTextR(text, dgvRowsR.CurrentCell.ColumnIndex, startindex, forward);
            if (newindex == -1)
            {
                MyMainForm.ShowInfo("Tekts [" + text + "] netika atrasts.");
                return;
            }
            dgvRowsR.CurrentCell = dgvRowsR[dgvRowsR.CurrentCell.ColumnIndex, newindex];
        }

        private int SearchRowTextP(string text, int colindex,
            int startindex = 0, bool forward = true)
        {
            string propname = dgvRowsP.Columns[colindex].DataPropertyName;
            if (bsRowsP.Count == 0) return -1;
            if (startindex < 0 || startindex >= bsRowsP.Count) return -1;
            if (string.IsNullOrEmpty(text)) return -1;
            if (startindex == 0 && !forward) return -1;
            if (startindex == bsRowsP.Count - 1 && forward) return -1;
            var dr0 = bsRowsP.GetItem<M_DISC_LISTS_P>(startindex);
            if (dr0 == null) return -1;
            var propdef = TypeDescriptor.GetProperties(dr0)[propname];
            if (propdef == null) return -1;
            int di = forward ? 1 : -1;
            string val;
            text = text.ToLower();
            for (int i = startindex; i >= 0 && i < bsRowsP.Count; i += di)
            {
                var dr = bsRowsP.GetItem<M_DISC_LISTS_P>(i);
                if (dr == null) continue;
                val = null;
                if (colindex == dgcPIdStore.Index)
                    val = dr.Store?.CODE;
                else if (colindex == dgcPIdStoresCat.Index)
                    val = dr.StoresCategory?.CODE;
                if (val == null) continue;
                if (val.ToLower().Contains(text))
                {
                    return i;
                }
            }
            return -1;
        }

        private void SearchRowTextP(bool fromfirst = true, bool forward = true)
        {
            if (dgvRowsP.CurrentRow == null || dgvRowsP.CurrentRow.IsNewRow) return;
            if (!dgvRowsP.EndEditX()) return;

            int startindex = dgvRowsP.CurrentRow.Index;
            startindex = fromfirst ? 0 : (forward ? startindex + 1 : startindex - 1);
            string text = tsbFind.Text;
            if (text == "") return;
            int newindex = SearchRowTextP(text, dgvRowsP.CurrentCell.ColumnIndex, startindex, forward);
            if (newindex == -1)
            {
                MyMainForm.ShowInfo("Tekts [" + text + "] netika atrasts.");
                return;
            }
            dgvRowsP.CurrentCell = dgvRowsP[dgvRowsP.CurrentCell.ColumnIndex, newindex];
        }

        private void dgvRowsR_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.Value == DBNull.Value) return;

            if (e.ColumnIndex == dgcRItemName.Index)
            {
                var dr = (M_ITEMS)e.Value;
                if (dr == null) return;
                e.Value = dr.NAME;
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
            bNav.DataGrid.MoveToNewRow();
        }

        private void bNav_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void dgvRowsR_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void dgvRowsP_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void dgvRowsR_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvRowsR.CurrentCell == null) return;
            if (e.KeyCode == Keys.Delete && e.Control)
            {
                DeleteCurrent();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                if (!dgvRowsR.EndEdit()) return;
                dgvRowsR.MoveToNewRow();
                e.Handled = true;
                return;
            }

            if (dgvRowsR.CurrentRow == null ||
                //dgvRows.CurrentRow.IsNewRow ||
                dgvRowsR.CurrentCell == null) return;
            int colnr = dgvRowsR.CurrentCell.ColumnIndex;

            if (e.KeyCode == Keys.F4)
            {
                if (colnr == dgcRIdcItem.Index)
                {
                    GetItem();
                    e.Handled = true;
                    return;
                }
                if (colnr == dgcRIdItemsCat.Index)
                {
                    GetItemsCat();
                    e.Handled = true;
                    return;
                }
            }
        }

        private void dgvRowsP_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvRowsP.CurrentCell == null) return;
            if (e.KeyCode == Keys.Delete && e.Control)
            {
                DeleteCurrent();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                if (!dgvRowsP.EndEdit()) return;
                dgvRowsP.MoveToNewRow();
                e.Handled = true;
                return;
            }

            if (dgvRowsP.CurrentRow == null ||
                //dgvRowsP.CurrentRow.IsNewRow ||
                dgvRowsP.CurrentCell == null) return;
            int colnr = dgvRowsP.CurrentCell.ColumnIndex;

            if (e.KeyCode == Keys.F4)
            {
                if (colnr == dgcPIdStore.Index)
                {
                    GetStore();
                    e.Handled = true;
                    return;
                }
                if (colnr == dgcPIdStoresCat.Index)
                {
                    GetStoreCat();
                    e.Handled = true;
                    return;
                }
            }
        }

        private void dgvRowsR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRowsR.CurrentRow == null ||
                //dgvRowsR.CurrentRow.IsNewRow ||
                dgvRowsR.CurrentCell == null) return;
            int colnr = dgvRowsR.CurrentCell.ColumnIndex;

            if (colnr == dgcRIdcItem.Index)
            {
                GetItem();
                return;
            }
            if (colnr == dgcRIdItemsCat.Index)
            {
                GetItemsCat();
                return;
            }
        }



        private void dgvRowsP_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRowsP.CurrentRow == null ||
                //dgvRowsP.CurrentRow.IsNewRow ||
                dgvRowsP.CurrentCell == null) return;
            int colnr = dgvRowsP.CurrentCell.ColumnIndex;

            if (colnr == dgcPIdStore.Index)
            {
                GetStore();
                return;
            }
            if (colnr == dgcPIdStoresCat.Index)
            {
                GetStoreCat();
                return;
            }
        }

        private void dgvRowsR_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void dgvRowsP_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void bsRowsR_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            if (e.ListChangedType == ListChangedType.Reset) return;
            CheckSave();
        }

        private void bsRowsP_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            if (e.ListChangedType == ListChangedType.Reset) return;
            CheckSave();
        }

        private void bsLists_ListChanged(object sender, ListChangedEventArgs e)
        {
            CheckEnableRows();
        }

        private void bsLists_CurrentChanged(object sender, EventArgs e)
        {
            CheckEnableRows();
        }

        void IMyDgvTextboxEditingControlEvents2.OnButtonClicked(MyDgvTextboxEditingControl2 control)
        {
            if (control.DataSource == bsStores)
            {
                GetStore();
                return;
            }
            if (control.DataSource == bsStoresCat)
            {
                GetStoreCat();
                return;
            }
            if (control.DataSource == bsItems)
            {
                GetItem();
                return;
            }
            if (control.DataSource == bsItemsCat)
            {
                GetItemsCat();
                return;
            }
        }

        private void tbLists_ButtonClicked(object sender, EventArgs e)
        {
            var rt = GetDiscountList();
            if (rt == null) return;
            tbLists.SelectedValue = rt;
        }

        private void tsbFindPrev_Click(object sender, EventArgs e)
        {
            if (bNav.BindingSource == bsRowsR)
            {
                SearchRowTextR(false, false);
                dgvRowsR.Focus();
            }
            else
            {
                SearchRowTextP(false, false);
                dgvRowsP.Focus();
            }
        }

        private void tsbFindNext_Click(object sender, EventArgs e)
        {
            if (bNav.BindingSource == bsRowsR)
            {
                SearchRowTextR(false, true);
                dgvRowsR.Focus();
            }
            else
            {
                SearchRowTextP(false, true);
                dgvRowsP.Focus();
            }
        }

        private void tsbFind_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                if (bNav.BindingSource == bsRowsR)
                {
                    SearchRowTextR();
                    dgvRowsR.Focus();
                }
                else
                {
                    SearchRowTextP();
                    dgvRowsP.Focus();
                }
                e.Handled = true;
                return;
            }
            if (e.KeyChar == (char)Keys.Escape)
            {
                bNav.DataGrid.Focus();
                e.Handled = true;
                return;
            }
        }

        private void tsbFind_Enter(object sender, EventArgs e)
        {
            tsbFind.Text = "";
        }

    }
}
