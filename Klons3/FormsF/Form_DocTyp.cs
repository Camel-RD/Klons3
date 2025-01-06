using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KlonsF.Classes;
using KlonsLIB.Components;
using KlonsLIB.Data;
using KlonsLIB.Misc;
using Microsoft.EntityFrameworkCore;
using ModelsF = Klons3.ModelsF;

namespace KlonsF.Forms
{
    public partial class Form_DocTyp : MyFormBaseF
    {
        public Form_DocTyp()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            bsDocTyp.Fill();
            bsDocTypA.Fill();
            MyData.DbContextF.F_DOCTYPB.Load();
        }

        private void FormDocTyp_Load(object sender, EventArgs e)
        {
            //this.bsDocTypA.CurrentChanged += new System.EventHandler(this.bsDocTypA_CurrentChanged);
            //ChecChildGrid();
            CheckSave();
        }

        private void FormDocTyp_Shown(object sender, EventArgs e)
        {
        }


        private void tbSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Return)
            {
                string s = tbSearch.Text;
                if (s == "")
                {
                    bsDocTyp.RemoveFilter();
                }
                else
                {
                    Predicate<ModelsF.F_DOCTYP> p1 = x => x.NAME.ContainsCI(s);
                    bsDocTyp.SetFilter(p1);
                }
            }
        }

        private void tbId_Enter(object sender, EventArgs e)
        {
            tbId.SelectAll();
        }

        private void tbSearch_Enter(object sender, EventArgs e)
        {
            tbSearch.SelectAll();
        }

        private void SelectCurrent()
        {
            if (dgvDocTyp.CurrentRow == null || dgvDocTyp.CurrentRow.IsNewRow) return;
            string doctp = dgvDocTyp.CurrentRow.Cells[0].Value.AsString();
            if (doctp.IsNOE()) return;
            if (!dgvDocTyp.EndEdit()) return;
            if (!SaveData()) return;
            if (!this.IsMyDialog) return;
            SetSelectedValueStr(doctp);
        }

        private void dgvDocTyp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Return)
            {
                SelectCurrent();
                e.Handled = true;
            }
        }

        private void tbId_RowSelectedEvent(object sender, RowSelectedEventArgs e)
        {
            SetSelectedValueStr(e.Value);
        }

        private void dgvDocTyp_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                SetSelectedValueStr(dgvDocTyp.CurrentRow.Cells[0].Value as string);
            }
        }

        private void dgvDocTyp_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SetSelectedValueStr(null);
            }
        }

        private void dgvDocTyp_Enter(object sender, EventArgs e)
        {
            bnavDocTyp.BindingSource = bsDocTyp;
            bnavDocTyp.DataGrid = dgvDocTyp;
            toolStripLabel1.Text = "Dokumentu veids:";
        }

        private void dgvDocTypA_Enter(object sender, EventArgs e)
        {
            bnavDocTyp.BindingSource = bsDocTypA;
            bnavDocTyp.DataGrid = dgvDocTypA;
            toolStripLabel1.Text = "Dokumentu grupa:";
            dgvDocTypB.Enabled = true;
        }

        private void dgvDocTypA_CurrentCellChanged(object sender, EventArgs e)
        {
            SetControlEnabled(dgvDocTypB, !(dgvDocTypA.CurrentRow == null ||
                dgvDocTypA.RowCount == 1 || dgvDocTypA.CurrentRow.IsNewRow));
            dgcDocTypAId.ReadOnly = dgvDocTypA.CurrentRow == null || !dgvDocTypA.CurrentRow.IsNewRow;
        }

        private void dgvDocTypB_Enter(object sender, EventArgs e)
        {
            bnavDocTyp.BindingSource = bsDocTypB;
            bnavDocTyp.DataGrid = dgvDocTypB;
            toolStripLabel1.Text = "Grupā ietilpst:";
        }

        private void bnavDocTyp_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void dgvDocTyp_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void dgvDocTypA_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void dgvDocTypB_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void dgvDocTyp_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                dgvDocTyp.MoveToNewRow();
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                bnavDocTyp.DeleteCurrent();
                e.Handled = true;
            }
        }

        private void dgvDocTypA_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                dgvDocTypA.MoveToNewRow();
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                if (dgvDocTypA.CurrentRow != null && !dgvDocTypA.CurrentRow.IsNewRow)
                {
                    if (!dgvDocTypA.EndEdit()) return;
                    bnavDocTyp.DeleteCurrent();
                    e.Handled = true;
                }
            }
        }

        private void dgvDocTypB_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                dgvDocTypB.MoveToNewRow();
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                if (dgvDocTypB.CurrentRow != null && !dgvDocTypB.CurrentRow.IsNewRow)
                {
                    if (!dgvDocTypB.EndEdit()) return;
                    bnavDocTyp.DeleteCurrent();
                    e.Handled = true;
                }
            }
        }

        private void dgvDocTypA_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dgvDocTypA[dgcDocTypAId.Index, e.RowIndex].Value == null)
                e.Cancel = true;
        }


        private void SetSaveButton(bool red)
        {
            bnavDocTyp.SetSaveButton(tsbSave, red);
        }

        public override bool SaveData()
        {
            if (!dgvDocTyp.EndEditX()) return false;
            if (!dgvDocTypA.EndEditX()) return false;
            if (!dgvDocTypB.EndEditX()) return false;
            var ret = bsDocTyp.SaveDbContext();
            CheckSave();
            return ret.IsSuccess();
        }

        private bool HasChanges()
        {
            return bsDocTyp.HasChanges() ||
                bsDocTypA.HasChanges() ||
                bsDocTypB.HasChanges();
        }

        private void CheckSave()
        {
            SetSaveButton(HasChanges());
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void dgvDocTyp_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            SaveData();
        }

        private void bsDocTyp_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        public void ChecChildGrid()
        {
            DataRowView dv = bsDocTypA.Current as DataRowView;
            if (dv == null
                || dv.Row.RowState == DataRowState.Deleted
                )
            {
                bsDocTypB.DataSource = null;
                SetControlEnabled(dgvDocTypB, false);
            }
            else
            {
                DataRelation rel = dv.Row.Table.ChildRelations["FK_DOCTYPB_IDA_DOCTYPA_ID"];
                DataView dv1 = dv.CreateChildView(rel);
                dgvDocTypB.Enabled = true;
                bsDocTypB.DataSource = dv1;
            }
        }

        private void bsDocTypA_CurrentChanged(object sender, EventArgs e)
        {
            //ChecChildGrid();
        }

    }
}
