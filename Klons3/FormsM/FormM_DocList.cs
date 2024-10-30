﻿using System;
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
    public partial class FormM_DocList : MyFormBaseF, IMyDgvTextboxEditingControlEvents2
    {
        public FormM_DocList()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            dgvFilter.AutoGenerateColumns = false;

            dgcDocsState.ColorMarkNeeded += DgcDocsState_ColorMarkNeeded;

            dgcFilterDocState.DataSource = SomeDataDefsM.DocStates;
            dgcFilterDocState.ValueMember = "Key";
            dgcFilterDocState.DisplayMember = "Val";
            dgcFilterDocState.ColumnNames = new[] { "Key", "Val" };
            dgcFilterDocState.ColumnWidths = "0;150";

            LoadColumnWidthsFromSettings();
        }

        private void FormM_DocList_Load(object sender, EventArgs e)
        {
            LoadParams();
            LoadDataOnOpen();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            panel1.Height = dgvFilter.ColumnHeadersHeight + dgvFilter.RowTemplate.Height + 4 + 2;
        }

        private void LoadColumnWidthsFromSettings()
        {
            string scw = MyData.Settings.ColumnWidths_MDocs;
            (int ver, int[] cw) = dgvDocs.ParseColumnWidths(scw);
            if (ver == 1 && cw != null && cw.Length > 0)
                dgvDocs.SetColumnWidths(cw);

            scw = MyData.Settings.ColumnWidths_MDocsFilter;
            (ver, cw) = dgvFilter.ParseColumnWidths(scw);
            if (ver == 1 && cw != null && cw.Length > 0)
                dgvFilter.SetColumnWidths(cw);
        }

        private void SaveColumnWidthsToSettings()
        {
            MyData.Settings.ColumnWidths_MDocs = dgvDocs.GetColumnWidths2(10f, 1);
            MyData.Settings.ColumnWidths_MDocsFilter = dgvFilter.GetColumnWidths2(8.0f, 1);
        }

        private void LoadParams()
        {
            docFilterData1.Dt1 = MyData.Params.MFILTERDOCSDT1;
            docFilterData1.Dt2 = MyData.Params.MFILTERDOCSDT2;
            docFilterData1.DocType = MyData.Params.MFILTERDOCSTP;
            docFilterData1.DocState = MyData.Params.MFILTERDOCSSTATE;
            docFilterData1.IdStoreOut = MyData.Params.MFILTERDOCSOUT;
            docFilterData1.IdStoreIn = MyData.Params.MFILTERDOCSIN;
            docFilterData1.IdStoreOutOrIn = MyData.Params.MFILTERDOCSINOROUT;
        }

        public override void SaveParams()
        {
            MyData.Params.MFILTERDOCSDT1 = docFilterData1.Dt1;
            MyData.Params.MFILTERDOCSDT2 = docFilterData1.Dt2;
            MyData.Params.MFILTERDOCSTP = docFilterData1.DocType;
            MyData.Params.MFILTERDOCSSTATE = docFilterData1.DocState;
            MyData.Params.MFILTERDOCSOUT = docFilterData1.IdStoreOut;
            MyData.Params.MFILTERDOCSIN = docFilterData1.IdStoreIn;
            MyData.Params.MFILTERDOCSINOROUT = docFilterData1.IdStoreOutOrIn;

            SaveColumnWidthsToSettings();
        }

        public void LoadData()
        {
            if (!dgvFilter.EndEdit()) return;
            if (!SaveData()) return;
            bsDocs.KillLists();
            DataLoaderM.LoadDocsByFilter(
                docFilterData1.Dt1, 
                docFilterData1.Dt2,
                docFilterData1.DocType, 
                docFilterData1.DocState,
                docFilterData1.IdStoreOut, 
                docFilterData1.IdStoreIn,
                docFilterData1.IdStoreOutOrIn);
            bsDocs.RecreateLists();
        }

        public void LoadDataOnOpen()
        {
            if (!HasParamsSet()) return;
            if (MyData.DbContextM.BL_M_DOCS.Count > 0) return;
            if (MyData.DbContextM.HasChanges<M_DOCS>()) return;
            if (MyData.DbContextM.HasChanges<M_ROWS>()) return;
            LoadData();
        }

        public bool HasParamsSet()
        {
            return docFilterData1.Dt1 != null || docFilterData1.Dt2 != null ||
                docFilterData1.DocType != null || docFilterData1.DocState != null ||
                docFilterData1.IdStoreOut != null || docFilterData1.IdStoreIn != null ||
                docFilterData1.IdStoreOutOrIn != null;
        }


        private void DgcDocsState_ColorMarkNeeded(object sender, DataGridViewColorMarkColumnEventArgs e)
        {
            e.MarkColor = myConfigA1.DocStatusColor1;
            if (e.RowNr == dgvDocs.NewRowIndex) return;
            var dr_doc = bsDocs.GetItem<M_DOCS>(e.RowNr);
            if (dr_doc == null) return;
            if (dr_doc.XState == EDocState.Iegrāmatots)
                e.MarkColor = myConfigA1.DocStatusColor2;
            else if (dr_doc.XState == EDocState.Iekontēts)
                e.MarkColor = myConfigA1.DocStatusColor3;
        }

        private M_DOCS GetCurrentDocRow()
        {
            if (bsDocs.Count == 0 || bsDocs.Current == null ||
                dgvDocs.CurrentRow.Index == dgvDocs.NewRowIndex) return null;
            var dr_doc = bsDocs.GetCurrentItem<M_DOCS>();
            return dr_doc;
        }

        private M_DOCS GetGoodCurrentDocRow()
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

        public bool CanEditCurrentDoc()
        {
            if (dgvDocs.CurrentRow.Index == dgvDocs.NewRowIndex) return true;
            if (bsDocs.Count == 0 || bsDocs.Current == null) return false;
            var dr_doc = bsDocs.GetCurrentItem<M_DOCS>();
            if (dr_doc == null) return false;
            return dr_doc.IsOpenForChanges;
        }

        public M_STORES GetStore(M_STORES dr_store, EStoreType storefilter = EStoreType.Nenoteikts)
        {
            return FormM_Stores.GetStore(dr_store, storefilter);
        }

        public void GetDocStore()
        {
            var dv = (M_STORES)((MyDgvTextboxEditingControl2)ActiveControl).SelectedValue;
            var rt = GetStore(dv);
            if (rt == null) return;
            SetCurrentDocEditorValue(rt);
        }


        private void SetCurrentDocEditorValue(object value)
        {
            if (ActiveControl == null) return;
            try
            {
                if (ActiveControl is MyMcComboBox)
                {
                    var c = ActiveControl as MyMcComboBox;
                    c.SelectedValue = value;
                }
                else if (ActiveControl is MyPickRowTextBox2)
                {
                    var c = ActiveControl as MyPickRowTextBox2;
                    c.SelectedValue = value;
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
            var dr0 = bsDocs.GetItem<M_DOCS>(startindex);
            if (dr0 == null) return -1;
            var propdef = TypeDescriptor.GetProperties(dr0)[propname];
            int di = forward ? 1 : -1;
            string val;
            text = text.ToLower();
            for (int i = startindex; i >= 0 && i < bsDocs.Count; i += di)
            {
                var dr = bsDocs.GetItem<M_DOCS>(i);
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
                    case "IDSTOREOUT":
                        val = dr.StoreOut.CODE;
                        break;
                    case "IDSTOREIN":
                        val = dr.StoreIn.CODE;
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

            if (e.ColumnIndex == dgcDocsPVNType.Index)
            {
                int id = (int)e.Value;
                var table_pvntype = MyData.DbContextM.BL_M_PVNTYPE;
                var dr = table_pvntype.FindById(id);
                if (dr == null) return;
                e.Value = dr.CODE;
                e.FormattingApplied = true;
            }

            if (e.ColumnIndex == dgcDocsState.Index)
            {
                var xstate = (EDocState)((int)e.Value);
                e.Value = SomeDataDefsM.GetDocStateText(xstate);
                e.FormattingApplied = true;
            }

            if (e.ColumnIndex == dgcDocsTP.Index)
            {
                int tp = (int)e.Value;
                var name = MyData.DbContextM.BL_M_DOCTYPES.FindById(tp).NAME;
                e.Value = name;
                e.FormattingApplied = true;
            }
        }

        private void bniSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void dgvDocs_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!CanEditCurrentDoc()) return;
            if (e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void bNav_ItemDeleting(object sender, CancelEventArgs e)
        {
            if (!CanEditCurrentDoc()) return;
            e.Cancel = !AskCanDelete();
        }

        private void dgvDocs_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvDocs.CurrentCell == null) return;
            /*if (e.KeyCode == Keys.Insert && e.Shift)
            {
                e.Handled = true;
                return;
            }*/

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

        private void tsbFind_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == '\r')
            {
                SearchDocText(true, true);
                dgvDocs.Focus();
                e.Handled = true;
            }
        }

        private void dgvFilter_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.Value == DBNull.Value) return;

            if (e.ColumnIndex == dgcFilterDocState.Index)
            {
                var xstate = (EDocState)((int)e.Value);
                e.Value = SomeDataDefsM.GetDocStateText(xstate);
                e.FormattingApplied = true;
            }
            if (e.ColumnIndex == dgcFilterDocType.Index)
            {
                var xstate = (EDocType)((int)e.Value);
                e.Value = MyData.DbContextM.BL_M_DOCTYPES.FindById((int)xstate).CODE;
                e.FormattingApplied = true;
            }
        }

        private void dgvFilter_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if(e.ColumnIndex == dgcFilterDt1.Index ||
                e.ColumnIndex == dgcFilterDt2.Index)
            {
                Utils.DGVParseDateCell(e);
                return;
            }
        }

        void IMyDgvTextboxEditingControlEvents2.OnButtonClicked(MyDgvTextboxEditingControl2 control)
        {
            if (control.DataSource == bsStoreIn || control.DataSource == bsStoreOut)
            {
                GetDocStore();
                return;
            }
        }

        public void DoNewDoc()
        {
            var table_docs = MyData.DbContextM.BL_M_DOCS;
            var dr = table_docs.NewItem();
            dr.DT = DateTime.Today;
            dr.XDocType = EDocType.Nenoteikts;
            dr.XState = EDocState.Melnraksts;
            dr.XWeVATPayer = !MyData.Params.CompRegNrPVN.IsNOE();
            table_docs.Add(dr);
            var form = MyMainForm.ShowFormMDialog<FormM_Doc>();
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
            DataLoaderM.LoadRowsByFilter(dr.ID, true);
            FormM_Doc.ShowDocMyDialog(dr);
        }

        private void dgvDocs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                DoOpenDoc();
            }
        }

        private void btFilter_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void bniNew_Click(object sender, EventArgs e)
        {
            DoNewDoc();
        }

        private void tsbOpenDoc_Click(object sender, EventArgs e)
        {
            DoOpenDoc();
        }

    }

    public class MyConfigA : Component
    {
        public Color DocStatusColor1 { get; set; } = Color.LightYellow;
        public Color DocStatusColor2 { get; set; } = Color.LightBlue;
        public Color DocStatusColor3 { get; set; } = Color.LightGreen;
    }
}
