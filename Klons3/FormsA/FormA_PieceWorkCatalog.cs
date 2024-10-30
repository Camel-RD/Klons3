using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KlonsA.Classes;
using KlonsF.Classes;
using KlonsLIB.Components;
using KlonsLIB.Data;
using KlonsLIB.Forms;
using Klons3.ModelsA;
using KlonsLIB.Misc;

namespace KlonsA.Forms
{
    public partial class FormA_PieceWorkCatalog : MyFormBaseF
    {
        public FormA_PieceWorkCatalog()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            cbCat.SelectedIndex = -1;
            cbActive.SelectedIndex = 0;

            var timeunits = SomeDataDefsA.MakeListB("0", "st", "1", "min", "2", "sec");
            dgcTimeUnit.DataSource = timeunits;
            dgcTimeUnit.DisplayMember = "Val";
            dgcTimeUnit.ValueMember = "Key";
        }

        private void Form_PieceWorkCatalog_Load(object sender, EventArgs e)
        {
            SetupToolStrips();
            CheckSave();
        }

        private void Form_PieceWorkCatalog_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void BsSar_ListItemPropertyChanged(object sender, KlonsLIB.Data.MyItemPropertyChangedEventArgs e)
        {
            var dr = e.Item as A_PIECEWORK_CATALOG;
            if (dr == null) return;

            if (e.PropertyName == nameof(A_PIECEWORK_CATALOG.TIMEUNIT) ||
                e.PropertyName == nameof(A_PIECEWORK_CATALOG.TIMEUSE))
            {
                float tm = dr.TIMEUSE;
                if (dr.TIMEUNIT == 1) tm /= 60.0f;
                else if (dr.TIMEUNIT == 2) tm /= 3600.0f;
                tm = (float)Math.Round(tm, 7);
                if (dr.TIMEUSEINHOURS != tm) dr.TIMEUSEINHOURS = tm;
            }
        }
        private void SetupToolStrips()
        {
            InsertInToolStrip(toolStrip1, cbCat, 0);
            InsertInToolStrip(toolStrip1, tbFilter, 2);
            InsertInToolStrip(toolStrip1, cbActive, 4);
        }

        private void SelectCurrent()
        {
            if (dgvSar.CurrentRow == null || dgvSar.CurrentRow.IsNewRow) return;
            var dr = bsSar.GetCurrentItem<A_PIECEWORK_CATALOG>();
            if (!dgvSar.EndEdit()) return;
            if (!SaveData()) return;
            if (bsSar.IsItemDetached(dr)) return;
            if (!this.IsMyDialog) return;
            SetSelectedObject(dr);
        }

        public void DeleteCurrent()
        {
            bnavSar.DeleteCurrent();
            SaveData();
        }

        private void dgvSar_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgcCode.Index || e.ColumnIndex == dgcDescr.Index)
            {
                SelectCurrent();
            }
            if (e.ColumnIndex == dgcIDS.Index)
            {
                GetIDKFromDialog();
            }
        }

        public void GetIDKFromDialog()
        {
            if (bsSar.Count == 0 || bsSar.Current == null) return;
            if (dgvSar.CurrentRow == null || dgvSar.CurrentRow.IsNewRow) return;
            var fm = new FormA_PieceWorkCatStruct();
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return;
            var dr = dgvSar.GetCurrentObjectViewItem<A_PIECEWORK_CATALOG>();
            if (dgvSar.CurrentCell != null)
            {
                try
                {
                    var dr_catstr = fm.SelectedObject;

                    dgvSar.BeginEdit(false);
                    var c = dgvSar.EditingControl as MyDgvMcComboBox;
                    c.SelectedValue = dr_catstr;

                    dgvSar.EndEdit();
                }
                catch (Exception) { }
            }
        }

        private void dgvSar_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvSar.CurrentCell == null) return;
            if (e.KeyCode == Keys.F5)
            {
                if (dgvSar.CurrentCell.ColumnIndex == dgcIDS.Index)
                {
                    GetIDKFromDialog();
                    e.Handled = true;
                    return;
                }
            }
            if (e.Control && e.KeyCode == Keys.Return)
            {
                SelectCurrent();
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                DeleteCurrent();
                e.Handled = true;
            }
        }

        private void dgvSar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SetSelectedObject(null);
            }
        }
        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            DeleteCurrent();
        }

        private void tbFilter_Enter(object sender, EventArgs e)
        {
            tbFilter.SelectAll();
        }

        private void CheckFilter()
        {
            string s1 = tbFilter.Text;
            int k = cbActive.SelectedIndex;
            var pred_list = new List<Predicate<A_PIECEWORK_CATALOG>>();
            if (k == 0)
            {
                pred_list.Add(x => x.USED == 1);
            }
            if (!s1.IsNOE())
            {
                pred_list.Add(x => x.CODE.ContainsCI(s1));
            }

            if (!string.IsNullOrEmpty(cbCat.Text) && cbCat.SelectedValue != null)
            {
                string s3 = (string)cbCat.SelectedValue;
                if (!string.IsNullOrEmpty(s3))
                {
                    pred_list.Add(x => x.KATCODE == s3);
                }
            }
            bsSar.SetFilter(pred_list);
        }

        private void cbActive_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckFilter();
        }

        private void tbFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                CheckFilter();
        }

        private void cbCat_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckFilter();
        }

        private void bnavSar_ItemDeleting(object sender, CancelEventArgs e)
        {
            if (dgvSar.CurrentRow == null || dgvSar.CurrentRow.IsNewRow)
            {
                e.Cancel = true;
                return;
            }
            e.Cancel = !AskCanDelete();
        }

        private void dgvSar_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete())
                e.Cancel = true;
        }

        private void SetSaveButton(bool red)
        {
            bnavSar.SetSaveButton(tsbSave, red);
        }

        public override bool SaveData()
        {
            if (!dgvSar.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = bsSar.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsSar.HasChanges() || bsKatStruct.HasChanges());
        }

        private void dgvSar_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void bsSar_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void dgvSar_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int ci = e.ColumnIndex;
            int ri = e.RowIndex;
            if (ri == -1 || ci == -1) return;
            dgvSar.InvalidateRow(ri);
        }

    }
}
