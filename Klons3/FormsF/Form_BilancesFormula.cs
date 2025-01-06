using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Equin.ApplicationFramework;
using Klons3.ModelsF;
using KlonsF.Classes;
using KlonsLIB.Components;
using KlonsLIB.Data;
using KlonsLIB.Forms;
using KlonsLIB.Misc;
using ModelsF = Klons3.ModelsF;

namespace KlonsF.Forms
{
    public partial class Form_BilancesFormula : MyFormBaseF
    {
        public Form_BilancesFormula()
        {
            InitializeComponent();
            CheckMyFontAndColors();

            DataLoaderF.ClearBalanceFormulas();
            DataLoaderF.BalA_Fill();

            cbReportPart.SelectedIndex = 0;
            CheckReportPart();
        }

        private void Form_Bilance_Load(object sender, EventArgs e)
        {
            dgvBalA3.Top = dgvBalA2.Top;
            panel1.Top = dgvBalA3.Top;
            panel1.Left = dgvBalA3.Left;
            panel2.Top = dgvBalA3.Top;
            panel2.Left = dgvBalA3.Left;
            dgcBalA3id.Visible = false;
            dgcBalA3id2.Visible = false;

            CheckSave();
        }

        private void dgvBalA1_Enter(object sender, EventArgs e)
        {
            bnavBalsA1.BindingSource = bsBalA1;
            bnavBalsA1.DataGrid = dgvBalA1;
            tslbActiveTable.Text = "Atskaites:";
        }

        private void dgvBalA2_Enter(object sender, EventArgs e)
        {
            bnavBalsA1.BindingSource = bsBalA2;
            bnavBalsA1.DataGrid = dgvBalA2;
            tslbActiveTable.Text = "Rindas:";
        }

        private void dgvBalA3_Enter(object sender, EventArgs e)
        {
            bnavBalsA1.BindingSource = bsBalA3;
            bnavBalsA1.DataGrid = dgvBalA3;
            tslbActiveTable.Text = "Summējums:";
        }

        private void bsBalA2_CurrentItemChanged(object sender, EventArgs e)
        {
            ObjectView<F_BALA2> o = bsBalA2.Current as ObjectView<F_BALA2>;
            if (o == null)
            {
                panel1.Visible = false;
                dgvBalA3.Visible = false;
                return;
            }
            var dr = o.Object;
            if (dr == null) return;
            switch (dr.TP)
            {
                case null:
                case "V":
                    panel1.Visible = false;
                    panel2.Visible = true;
                    dgvBalA3.Visible = false;
                    break;
                case "S":
                    panel1.Visible = false;
                    panel2.Visible = false;
                    dgvBalA3.Visible = true;
                    break;
                case "K":
                    panel1.Visible = true;
                    panel2.Visible = false;
                    dgvBalA3.Visible = false;
                    break;
                default:
                    panel1.Visible = false;
                    panel2.Visible = false;
                    dgvBalA3.Visible = false;
                    break;
            }
            panel1.Top = dgvBalA3.Top;
            panel1.Left = dgvBalA3.Left;
            panel2.Top = dgvBalA3.Top;
            panel2.Left = dgvBalA3.Left;
        }

        private void CheckReportPart()
        {
            int k = cbReportPart.SelectedIndex;
            if (k == -1) return;
            string s = k == 0 ? "AK" : "PA";
            Predicate<F_BALA2> filter = x => x.DC == s;
            var blv = bsBalA2.List as BindingListView<F_BALA2>;
            if (blv == null) return;
            var item_filter = BindingListView<F_BALA2>.CreateItemFilter(filter);
            blv.Filter = item_filter;
        }

        private void cbReportPart_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckReportPart();
        }

        private void dgvBalA2_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            int k = cbReportPart.SelectedIndex;
            string s = k == 1 ? "PA" : "AK";
            e.Row.Cells[dgcBalA2dc.Index].Value = s;
        }

        private void bnavBalsA1_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void dgvBalA1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void dgvBalA1_MyKeyDown(object sender, KeyEventArgs e)
        {
            var dgv = sender as MyDataGridView;
            if (dgv == null) return;
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                dgv.MoveToNewRow();
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                if (!dgvBalA1.EndEdit()) return;
                if (!dgvBalA2.EndEdit()) return;
                if (!dgvBalA3.EndEdit()) return;
                if (bnavBalsA1.BindingSource == bsBalA1 &&
                    dgvBalA1.CurrentRow != null && !dgvBalA1.CurrentRow.IsNewRow ||
                    bnavBalsA1.BindingSource == bsBalA2 &&
                    dgvBalA2.CurrentRow != null && !dgvBalA2.CurrentRow.IsNewRow ||
                    bnavBalsA1.BindingSource == bsBalA3 &&
                    dgvBalA3.CurrentRow != null && !dgvBalA3.CurrentRow.IsNewRow)
                {
                    bnavBalsA1.DeleteCurrent();
                    e.Handled = true;
                }
            }
        }

        private void DoCopy()
        {
            if (!SaveData()) return;
            var dr = dgvBalA1.GetCurrentObjectViewItem<F_BALA1>();
            if (dr == null) return;
            Form_InputBox f = new Form_InputBox("Bilances atskaites", "Jaunās atskaites kods", "");
            if (f.ShowDialog(MyMainForm) != DialogResult.OK) return;
            string newid = f.SelectedValueStr;
            if (newid == "" || newid.Length > 10 ||
                MyData.DbContextF.F_BALA1.Find(newid) != null)
            {
                MyMainForm.ShowWarning("Nekorekts atskaites kods: " + newid);
                return;
            }
            try
            {
                CopyA1(dr, newid);
            }
            catch (Exception e)
            {
                Form_Error.ShowException(MyMainForm, e);
            }
            SaveData();
            Refresh();
        }

        private void CopyA1(F_BALA1 row_from, string newid)
        {
            var row_to = new F_BALA1();
            MyData.DbContextF.CopyEntry(row_from, row_to);
            row_to.BALID = newid;
            var rows_a2 = row_from.BALA2Rows.ToList();
            foreach (var row_a2_from in rows_a2)
            {
                var row_a2_to = CopyA2(row_a2_from);
                MyData.DbContextF.CopyEntry(row_a2_from, row_a2_to);
                row_a2_to.BALID = row_to.BALID;
                row_a2_to.BALA1Row = row_to;
                row_to.BALA2Rows.Add(row_a2_to);
            }
            MyData.DbContextF.F_BALA1.Add(row_to);
        }

        private F_BALA2 CopyA2(F_BALA2 row_from)
        {
            var row_to = new F_BALA2();
            MyData.DbContextF.CopyEntry(row_from, row_to);
            var rows_a3 = row_from.BALA3Rows.ToList();
            foreach (var row_a3_from in rows_a3)
            {
                var row_a3_to = new F_BALA3();
                MyData.DbContextF.CopyEntry(row_a3_from, row_a3_to);
                row_a3_to.ID2 = row_to.ID;
                row_a3_to.BALA2Row = row_to;
                row_to.BALA3Rows.Add(row_a3_to);
            }
            return row_to;
        }

        private void CheckFormulas(bool pza = false)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                CheckFormulasA(pza);
            });
        }

        private void CheckFormulasA(bool pza = false)
        {
            if (!SaveData()) return;
            var o = dgvBalA1.GetCurrentDataItem();
            if (o == null) return;

            string balid = (string) dgvBalA1.CurrentRow.Cells[dgcBalA1balid.Index].Value;
            
            var dt = DataLoaderF.ACP21_GetBy_bal_13(balid);
            if (pza)
            {
                dt = dt.Where(x => !x.AC.IsLessThan("6") && x.AC.IsLessThan("9")).ToList();
            }
            int k = dt.Count;
            if (k == 0)
            {
                MyMainForm.ShowInfo("Kļūdas netika atrastas\n(bet tas nenozīmē kā tādu tur nav)");
                return;
            }
            if (k > 20) k = 20;
            StringBuilder sb = new StringBuilder();
            sb.Append("Šie konti nav iekļauti formulās:\n");
            for (int i = 0; i < k; i++)
            {
                sb.Append(dt[i].AC);
                if (i < k - 1)
                    sb.Append(", ");
            }
            if (k < dt.Count)
                sb.Append(", ...");

            MyMainForm.ShowInfo(sb.ToString());
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            CheckFormulas(false);
        }

        private void pārbaudītFormulasPZAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckFormulas(true);
        }

        private void kopētAtskaitesFormuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoCopy();
        }

        private void pārbaudītFormulasBilanceiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckFormulas(false);
        }

        private void dgvBalA1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvBalA1.CurrentCell == null) return;
            SetControlEnabled(dgvBalA2, !dgvBalA1.CurrentRow.IsNewRow);
            SetControlEnabled(dgvBalA3, !(dgvBalA1.CurrentRow.IsNewRow ||
                (dgvBalA2.CurrentRow != null && dgvBalA2.CurrentRow.IsNewRow)));
            SetControlEnabled(panel1, dgvBalA2.Enabled);
            SetControlEnabled(panel2, dgvBalA2.Enabled);
            dgcBalA1balid.ReadOnly =
                dgvBalA1.NewRowIndex != dgvBalA1.CurrentCell.RowIndex;
        }
        private void dgvBalA1_Leave(object sender, EventArgs e)
        {
            if (dgvBalA1.CurrentRow == null) return;
            if (dgvBalA1.CurrentRow.IsNewRow)
            {
                if (dgvBalA1.RowCount == 1) return;
                dgvBalA1.CurrentCell = dgvBalA1[0, dgvBalA1.RowCount - 2];
            }
        }

        private void dgvBalA2_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvBalA2.CurrentCell == null) return;
            SetControlEnabled(dgvBalA3, !(dgvBalA2.CurrentRow.IsNewRow));
        }

        private void dgvBalA2_Leave(object sender, EventArgs e)
        {
            if (dgvBalA2.CurrentRow == null) return;
            if (dgvBalA2.CurrentRow.IsNewRow)
            {
                if (dgvBalA2.RowCount == 1) return;
                dgvBalA2.CurrentCell = dgvBalA2[0, dgvBalA2.RowCount - 2];
            }
        }

        private void bsBalA1_CurrentChanged(object sender, EventArgs e)
        {
            //CheckGridA1();
        }

        private void bsBalA2_CurrentChanged(object sender, EventArgs e)
        {
            //CheckGridA2();
        }

        public override bool SaveData()
        {
            if (!dgvBalA1.EndEditX()) return false;
            if (!dgvBalA2.EndEditX()) return false;
            if (!dgvBalA3.EndEditX()) return false;
            var ret = bsBalA1.SaveDbContext();
            CheckSave();
            return ret.IsSuccess();
        }

        private bool HasChanges()
        {
            return bsBalA1.HasChanges() ||
                bsBalA2.HasChanges() ||
                bsBalA3.HasChanges();
        }
        private void SetSaveButton(bool red)
        {
            bnavBalsA1.SetSaveButton(tsbSave, red);
        }

        private void CheckSave()
        {
            SetSaveButton(HasChanges());
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
            CheckSave();
        }

        private void bsBalA1_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void bsBalA2_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void bsBalA3_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void dgvBalA3_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[dgcBalA3tp.Index].Value = cbReportPart.SelectedIndex == 0 ? "Db" : "Kr";
        }
    }
}
