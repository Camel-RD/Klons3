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
    public partial class FormM_InvDocList : MyFormBaseF, IMyDgvTextboxEditingControlEvents2
    {
        public FormM_InvDocList()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            dgvFilter.AutoGenerateColumns = false;

            dgcDocsState.ColorMarkNeeded += DgcDocsState_ColorMarkNeeded;

            dgcFilterState.DataSource = SomeDataDefsM.InventoryDocStates;
            dgcFilterState.ValueMember = "Key";
            dgcFilterState.DisplayMember = "Val";
            dgcFilterState.ColumnNames = new[] { "Key", "Val" };
            dgcFilterState.ColumnWidths = "0;150";
        }

        private void FormM_InvDocList_Load(object sender, EventArgs e)
        {
            LoadParams();
            LoadDataOnOpen();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            panel1.Height = dgvFilter.ColumnHeadersHeight + dgvFilter.RowTemplate.Height + 6;
        }

        private void LoadParams()
        {
            docFilterData1.Dt1 = MyData.Params.MFILTERDOCSDT1;
            docFilterData1.Dt2 = MyData.Params.MFILTERDOCSDT2;
        }

        public override void SaveParams()
        {
            MyData.Params.MFILTERDOCSDT1 = docFilterData1.Dt1;
            MyData.Params.MFILTERDOCSDT2 = docFilterData1.Dt2;
        }

        public void LoadData()
        {
            if (!dgvFilter.EndEdit()) return;
            if (!SaveData()) return;
            bsDocs.KillLists();
            DataLoaderM.LoadInvDocsByFilter(
                docFilterData1.Dt1,
                docFilterData1.Dt2,
                docFilterData1.DocState,
                docFilterData1.IdStoreIn);
            bsDocs.RecreateLists();
        }

        public void LoadDataOnOpen()
        {
            if (!HasParamsSet()) return;
            if (MyData.DbContextM.BL_M_INV_DOCS.Count > 0) return;
            if (MyData.DbContextM.HasChanges<M_INV_DOCS>()) return;
            if (MyData.DbContextM.HasChanges<M_INV_ROWS>()) return;
            LoadData();
        }

        public bool HasParamsSet()
        {
            return docFilterData1.Dt1 != null || docFilterData1.Dt2 != null ||
                docFilterData1.DocState != null || docFilterData1.IdStoreIn != null;
        }

        private void DgcDocsState_ColorMarkNeeded(object sender, DataGridViewColorMarkColumnEventArgs e)
        {
            e.MarkColor = myConfigA1.DocStatusColor1;
            if (e.RowNr == dgvDocs.NewRowIndex) return;
            var dr_doc = bsDocs.GetItem<M_INV_DOCS>(e.RowNr);
            if (dr_doc == null) return;
            if (dr_doc.XState == EInventoryDocState.Iegrāmatots)
                e.MarkColor = myConfigA1.DocStatusColor3;
            else if (dr_doc.XState == EInventoryDocState.Apstiprināts ||
                dr_doc.XState == EInventoryDocState.Salīdzināts)
                e.MarkColor = myConfigA1.DocStatusColor2;
        }

        private M_INV_DOCS GetCurrentDocRow()
        {
            if (bsDocs.Count == 0 || bsDocs.Current == null ||
                dgvDocs.CurrentRow.Index == dgvDocs.NewRowIndex) return null;
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

        public override bool SaveData()
        {
            if (!dgvDocs.EndEditX()) return false;
            if (!Validate()) return false;
            var rt = bsDocs.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private bool HasChanges()
        {
            return bsDocs.HasChanges() || MyData.DbContextM.HasChanges<M_INV_ROWS>();
        }

        private void CheckSave()
        {
            SetSaveButton(HasChanges());
        }

        private void SetSaveButton(bool red)
        {
            bNav.SetSaveButtonRed(red);
        }

        public M_STORES GetStore(M_STORES dr_store, EStoreType storefilter = EStoreType.Nenoteikts)
        {
            return FormM_Stores.GetStore(dr_store, storefilter);
        }

        public void GetDocStore()
        {
            var dv = (M_STORES)((MyDgvTextboxEditingControl2)ActiveControl).SelectedValue;
            var rt = GetStore(dv, EStoreType.Noliktava);
            if (rt == null) return;
            SetCurrentDocEditorValue(rt);
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

        private int SearchDocText(string text, int colindex,
            int startindex = 0, bool forward = true)
        {
            string propname = dgvDocs.Columns[colindex].DataPropertyName;
            if (bsDocs.Count == 0) return -1;
            if (startindex < 0 || startindex >= bsDocs.Count) return -1;
            if (string.IsNullOrEmpty(text)) return -1;
            if (startindex == 0 && !forward) return -1;
            if (startindex == bsDocs.Count - 1 && forward) return -1;
            var dr0 = bsDocs.GetItem<M_INV_DOCS>(startindex);
            if (dr0 == null) return -1;
            var propdef = TypeDescriptor.GetProperties(dr0)[propname];
            if (propdef == null) return -1;
            int di = forward ? 1 : -1;
            string val;
            text = text.ToLower();
            for (int i = startindex; i >= 0 && i < bsDocs.Count; i += di)
            {
                var dr = bsDocs.GetItem<M_INV_DOCS>(i);
                if (dr == null) continue;
                val = null;
                switch (propname)
                {
                    case "DT":
                        val = dr.DT.ToString("dd.MM.yyyy");
                        break;
                    case "NR":
                        val = dr.NR;
                        break;
                    case "IDSTORE":
                        val = dr.Store?.CODE;
                        break;
                }
                if (val == null) continue;
                if (val.ToLower().Contains(text))
                {
                    return i;
                }
            }
            return -1;
        }

        private void SearchDocText(bool fromfirst = true, bool forward = true)
        {
            if (dgvDocs.CurrentRow == null || dgvDocs.CurrentRow.IsNewRow) return;
            if (!dgvDocs.EndEditX()) return;

            int startindex = dgvDocs.CurrentRow.Index;
            startindex = fromfirst ? 0 : (forward ? startindex + 1 : startindex - 1);
            string text = tsbFind.Text;
            if (text == "") return;
            int newindex = SearchDocText(text, dgvDocs.CurrentCell.ColumnIndex, startindex, forward);
            if (newindex == -1)
            {
                MyMainForm.ShowInfo("Tekts [" + text + "] netika atrasts.");
                return;
            }
            dgvDocs.CurrentCell = dgvDocs[dgvDocs.CurrentCell.ColumnIndex, newindex];
        }

        private void dgvDocs_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.Value == DBNull.Value) return;

            if (e.ColumnIndex == dgcDocsState.Index)
            {
                var xstate = (EInventoryDocState)((int)e.Value);
                e.Value = SomeDataDefsM.GetInventoryDocStateText(xstate);
                e.FormattingApplied = true;
            }

        }

        private void bniSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void dgvDocs_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void bNav_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void dgvDocs_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void bsDocs_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void tsbFind_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                SearchDocText();
                dgvDocs.Focus();
                e.Handled = true;
                return;
            }
            if (e.KeyChar == (char)Keys.Escape)
            {
                dgvDocs.Focus();
                e.Handled = true;
                return;
            }
        }

        private void tsbFind_Enter(object sender, EventArgs e)
        {
            tsbFind.Text = "";
        }

        private void tsbFindPrev_Click(object sender, EventArgs e)
        {
            SearchDocText(false, false);
            dgvDocs.Focus();
        }

        private void tsbFindNext_Click(object sender, EventArgs e)
        {
            SearchDocText(false, true);
            dgvDocs.Focus();
        }

        private void dgvFilter_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.Value == DBNull.Value) return;

            if (e.ColumnIndex == dgcFilterState.Index)
            {
                var xstate = (EInventoryDocState)((int)e.Value);
                e.Value = SomeDataDefsM.GetInventoryDocStateText(xstate);
                e.FormattingApplied = true;
            }
        }

        private void dgvFilter_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.ColumnIndex == dgcFilterDt1.Index ||
                e.ColumnIndex == dgcFilterDt2.Index)
            {
                Utils.DGVParseDateCell(e);
                return;
            }
        }

        void IMyDgvTextboxEditingControlEvents2.OnButtonClicked(MyDgvTextboxEditingControl2 control)
        {
            if (control.DataSource == bsStore)
            {
                GetDocStore();
                return;
            }
        }


        public void DoNewDoc()
        {
            var table_docs = MyData.DbContextM.BL_M_INV_DOCS;
            var dr = table_docs.NewItem();
            dr.DT = DateTime.Today;
            dr.XState = EInventoryDocState.Melnraksts;
            table_docs.Add(dr);
            var form = MyMainForm.ShowFormMDialog<FormM_InvDoc>();
            bool rt = form.FindDoc(dr);
            if (!rt)
            {
                form.Close();
                MyMainForm.ShowError("Neizdevās atvērt dokumentu.");
                return;
            }
        }

        public void DoOpenDoc()
        {
            if (!SaveData()) return;
            var dr = GetCurrentDocRow();
            if (dr == null) return;
            DataLoaderM.LoadInvRowsByFilter(dr.ID, true);
            FormM_InvDoc.ShowDocMyDialog(dr);
        }

        private void dgvDocs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgcDocsDT.Index ||
               e.ColumnIndex == dgcDocsNr.Index)
            {
                DoOpenDoc();
            }
        }

        private void btFilter_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void bniAdd_Click(object sender, EventArgs e)
        {
            DoNewDoc();
        }

        private void tsbOpenDoc_Click(object sender, EventArgs e)
        {
            DoOpenDoc();
        }
    }
}
