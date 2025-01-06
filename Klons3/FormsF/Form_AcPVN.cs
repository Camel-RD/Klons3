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
using KlonsLIB.Misc;
using ModelsF = Klons3.ModelsF;

namespace KlonsF.Forms
{
    public partial class Form_AcPVN : MyFormBaseF
    {
        public Form_AcPVN()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            bsAcP5.Fill();
            bsAcPVN.Fill();
        }


        private void FormAcPVN_Load(object sender, EventArgs e)
        {
            CheckSave();
            bsAcP5.ListItemPropertyChanged += BsAcP5_ListItemPropertyChanged;
        }

        private void Form_AcPVN_FormClosed(object sender, FormClosedEventArgs e)
        {
            bsAcP5.ListItemPropertyChanged -= BsAcP5_ListItemPropertyChanged;
        }

        private void BsAcP5_ListItemPropertyChanged(object sender, MyItemPropertyChangedEventArgs e)
        {
            var dr = e.Item as ModelsF.F_ACP25;
            if (e.PropertyName != nameof(ModelsF.F_ACP25.IDX)) return;
            var drp = MyData.DbContextF.F_ACPVN.Find(dr.IDX);
            if (drp == null) return;
            dr.NAME = drp.NM.LeftMax(150);
        }
        public static string GetAcPVN(string acpvn)
        {
            var fm = new Form_AcPVN();
            fm.SelectedValueStr = acpvn;
            fm.StartPosition = FormStartPosition.CenterParent;
            fm.FindAcPVN(acpvn);
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return null;
            return fm.SelectedValueStr;
        }

        public void FindAcPVN(string acppvn)
        {
            if (bsAcP5.Count == 0) return;
            if (acppvn.IsNOE()) return;
            for (int i = 0; i < bsAcP5.Count; i++)
            {
                var dr = bsAcP5.GetItem<F_ACP25>(i);
                if (dr.IDX == acppvn)
                {
                    bsAcP5.Position = i;
                    return;
                }
            }
        }

        private void SelectCurrent()
        {
            if (dgvAcPVN.CurrentRow == null || dgvAcPVN.CurrentRow.IsNewRow) return;
            string ac = dgvAcPVN.CurrentRow.Cells[dgcIdx.Index].Value.AsString();
            if (!dgvAcPVN.EndEdit()) return;
            if (!SaveData()) return;
            if (!this.IsMyDialog) return;
            SetSelectedValueStr(ac);
        }
        private void dgvAcPVN_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectCurrent();
        }
        private void tbIdx_RowSelectedEvent(object sender, RowSelectedEventArgs e)
        {
            SetSelectedValueStr(e.Value);
        }
        private void dgvAcPVN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Return)
            {
                SelectCurrent();
                e.Handled = true;
            }
        }

        private void tbIdx_Enter(object sender, EventArgs e)
        {
            tbIdx.SelectAll();
        }

        private void FormAcPVN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Escape)
            {
                SetSelectedValueStr(null);
            }
        }

        private void bnavAcp5_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void dgvAcPVN_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!e.Row.IsNewRow)
            {
                if (!AskCanDelete()) e.Cancel = true;
            }
        }

        private void dgvAcPVN_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                dgvAcPVN.MoveToNewRow();
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                bnavAcp5.DeleteCurrent();
                e.Handled = true;
            }
        }
        private void SetSaveButton(bool red)
        {
            bnavAcp5.SetSaveButton(tsbSave, red);
        }

        public override bool SaveData()
        {
            if (!dgvAcPVN.EndEditX()) return false;
            var ret = bsAcP5.SaveDbContext();
            CheckSave();
            return ret.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsAcP5.HasChanges());
        }

        private void dgvAcPVN_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            SaveData();
        }

        private void bsAcP5_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

    }
}
