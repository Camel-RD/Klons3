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
using KlonsF.Classes;
using KlonsLIB.Data;
using KlonsLIB.Misc;
using ModelsF = Klons3.ModelsF;

namespace KlonsF.Forms
{
    public partial class Form_Bal0 : MyFormBaseF
    {
        public Form_Bal0()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            suspend_table_events = true;
            bsBal0.Fill();
            suspend_table_events = false;
            GetSums();
        }

        private void FormBal0_Load(object sender, EventArgs e)
        {
            CheckSave();
            WindowState = FormWindowState.Maximized;
            bsBal0.ListItemPropertyChanged += BsBal0_ListItemPropertyChanged;
        }

        private void Form_Bal0_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            bsBal0.ListItemPropertyChanged -= BsBal0_ListItemPropertyChanged;
        }


        private bool suspend_table_events = false;

        private void BsBal0_ListItemPropertyChanged(object sender, MyItemPropertyChangedEventArgs e)
        {
            if (suspend_table_events) return;

            var dr = e.Item as ModelsF.F_BAL0;
            if (dr == null) return;

            if (e.PropertyName == nameof(ModelsF.F_BAL0.SUMMCC) ||
                e.PropertyName == nameof(ModelsF.F_BAL0.SUMMDC) ||
                e.PropertyName == nameof(ModelsF.F_BAL0.CUR) ||
                e.PropertyName == nameof(ModelsF.F_BAL0.CUR))
            {
                dr.SUMMD = Math.Round(dr.SUMMDC * dr.CURRATE, 2);
                dr.SUMMC = Math.Round(dr.SUMMCC * dr.CURRATE, 2);
            }
       }

        private void bnavB0_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void dgvBal0_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            e.Cancel = e.Row.IsNewRow || !AskCanDelete();
        }

        private void dgvBal0_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvBal0.CurrentCell == null) return;
            if (e.KeyCode == Keys.F5)
            {
                dgvBal0GetCellValue(sender, dgvBal0.CurrentCell.ColumnIndex);
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                dgvBal0.MoveToNewRow();
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                if (!dgvBal0.EndEdit()) return;
                if (dgvBal0.CurrentRow != null && !dgvBal0.CurrentRow.IsNewRow)
                {
                    bnavB0.DeleteCurrent();
                    e.Handled = true;
                }
            }
        }

        private void GetSums()
        {
            if (bsBal0.Count == 0)
            {
                lbSumDeb.Text = "0.00";
                lbSumKred.Text = "0.00";
                return;
            }
            var blv = bsBal0.BindingListView as BindingListView<ModelsF.F_BAL0>;
            decimal sd = blv.Sum(x => x.SUMMD);
            decimal sc = blv.Sum(x => x.SUMMC);
            lbSumDeb.Text = sd.ToString("N2");
            lbSumKred.Text = sc.ToString("N2");
        }

        private void dgvBal0GetCellValue(object sender, int colind)
        {
            Action<string> act =
                value =>
                {
                    try
                    {
                        if (value != null && dgvBal0.CurrentCell != null)
                        {
                            dgvBal0.CurrentCell.Value = value;
                            dgvBal0.RefreshEdit();
                            dgvBal0.EndEdit();
                            dgvBal0.Focus();
                        }
                        dgvBal0.GoingToDialog = false;
                        dgvBal0.Select();
                        if (dgvBal0.EditingControl != null)
                            ActiveControl = dgvBal0.EditingControl;
                    }
                    finally
                    {
                        dgvBal0.GoingToDialog = false;
                    }
                };
            if (colind == dgcBal0AC11.Index)
            {
                dgvBal0.GoingToDialog = true;
                MyMainForm.ShowFormAcplanDialog(act);
                return;
            }

            if (colind == dgcBal0AC24.Index)
            {
                dgvBal0.GoingToDialog = true;
                MyMainForm.ShowFormAcp4Dialog(act);
                return;
            }

            if (colind == dgcBal0Clid.Index)
            {
                dgvBal0.GoingToDialog = true;
                MyMainForm.ShowFormPersonsDialog(act);
                return;
            }
        }

        private void dgvBal0_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvBal0GetCellValue(sender, e.ColumnIndex);
        }

        private void dgvBal0_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvBal0.CurrentRow == null ||
                dgvBal0.CurrentRow.IsNewRow) return;
            if(Utils.IN(e.ColumnIndex, dgcSummDC.Index, 
                dgcSummCC.Index, dgcCur.Index, dgcCurRate.Index))
            {
                dgvBal0.RefreshCurrent();
            }
        }

        private void dgvBal0_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex == -1 || e.RowIndex == dgvBal0.NewRowIndex || e.ColumnIndex == -1) return;
            var o = dgvBal0[e.ColumnIndex, e.RowIndex].Value;
            if (o == null || o == DBNull.Value) return;
            string s = o.ToString();
            if (string.IsNullOrEmpty(s)) return;
            if (e.ColumnIndex == dgcBal0AC11.Index)
            {
                e.ToolTipText = DataTasks.GetAcName(s);
                return;
            }
            if (e.ColumnIndex == dgcBal0AC24.Index)
            {
                e.ToolTipText = DataTasks.GetAc4Name(s);
                return;
            }
            if (e.ColumnIndex == dgcBal0Clid.Index)
            {
                e.ToolTipText = DataTasks.GetClName(s);
                return;
            }
        }

        private void SetSaveButton(bool red)
        {
            bnavB0.SetSaveButton(tsbSave, red);
        }

        public override bool SaveData()
        {
            if (!dgvBal0.EndEditX()) return false;
            var ret = bsBal0.SaveDbContext();
            CheckSave();
            return ret.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsBal0.HasChanges());
        }


        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
            SetSaveButton(bsBal0.HasChanges());
        }

        private void dgvBal0_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            SaveData();
        }
        private void bsBal0_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            SetSaveButton(bsBal0.HasChanges());
            GetSums();
        }

    }
}
