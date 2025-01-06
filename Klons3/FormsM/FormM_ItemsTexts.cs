using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Klons3.ModelsM;
using KlonsM.Classes;
using KlonsLIB.Forms;
using KlonsLIB.Data;
using KlonsF.Classes;
using KlonsLIB.Misc;
using NPOI.SS.Formula.Functions;

namespace KlonsM.FormsM
{
    public partial class FormM_ItemsTexts : MyFormBaseF
    {
        public FormM_ItemsTexts()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        private void FormM_ItemsTexts_Load(object sender, EventArgs e)
        {
        }

        private void FormM_ItemsTexts_Shown(object sender, EventArgs e)
        {
            if (SelectedItem == null)
            {
                Close();
                return;
            }
            var rt = FindItem(SelectedItem);
            if (!rt)
            {
                Close();
                return;
            }
            tbCurrItem.Text = $"{SelectedItem.BARCODE}, {SelectedItem.NAME}";
            if (SelectedItemText != null)
            {
                FindItemText(SelectedItemText);
            }
        }


        public M_ITEMS SelectedItem = null;
        public M_ITEMS_TEXTS SelectedItemText = null;
        public bool SetResultNull = false;

        public static (bool, M_ITEMS_TEXTS) GetItemText(M_ITEMS dr_item, M_ITEMS_TEXTS dr_itemtext)
        {
            var fm = new FormM_ItemsTexts();
            fm.SelectedItem = dr_item;
            fm.SelectedItemText = dr_itemtext;
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return (false, null);
            return (true, fm.SetResultNull ? null : fm.SelectedItemText);
        }

        public bool FindItem(M_ITEMS dr_item)
        {
            return bsItems.SelectItem(dr_item);
        }

        public bool FindItemText(M_ITEMS_TEXTS dr_itemtext)
        {
            if (dr_itemtext == null) return false;
            for (int i = 0; i < bsTexts.Count; i++)
            {
                var t = bsTexts.GetItem<M_ITEMS_TEXTS>(i);
                if (t == dr_itemtext)
                {
                    bsTexts.Position = i;
                    return true;
                }
            }
            return false;
        }

        public bool FindItemTextById(int id)
        {
            for (int i = 0; i < bsTexts.Count; i++)
            {
                var t = bsTexts.GetItem<M_ITEMS_TEXTS>(i);
                if (t.ID == id)
                {
                    bsTexts.Position = i;
                    return true;
                }
            }
            return false;
        }

        public int FindItemTextByText(string text)
        {
            for (int i = 0; i < bsTexts.Count; i++)
            {
                var t = bsTexts.GetItem<M_ITEMS_TEXTS>(i);
                if (t.TEXT == text)
                {
                    bsTexts.Position = i;
                    return i;
                }
            }
            return -1;
        }

        private void cmFind_Click(object sender, EventArgs e)
        {
            var s = tbText.Text;
            if (s.IsNOE()) return;
            int k = FindItemTextByText(s);
            if (k == -1)
            {
                MyMainForm.ShowInfo("Meklētais teksts netika atrasts.");
                return;
            }
            bsTexts.Position = k;
        }

        private void cmAdd_Click(object sender, EventArgs e)
        {
            var s = tbText.Text;
            if (s.IsNOE()) return;
            int k = FindItemTextByText(s);
            if (k != -1)
            {
                MyMainForm.ShowWarning("Šāds teksts jau ir sarakstā.");
                return;
            }
            var table = MyData.DbContextM.BL_M_ITEMS_TEXTS;
            var dr_new = table.NewItem();
            dr_new.IDITEM = SelectedItem.ID;
            dr_new.Item = SelectedItem;
            dr_new.TEXT = s;
            table.Add(dr_new);
            FindItemText(dr_new);
        }

        private void SelectCurrent()
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            var dr = bsTexts.GetCurrentItem<M_ITEMS_TEXTS>();
            if (!dgvRows.EndEdit()) return;
            if (!SaveData()) return;
            if (bsTexts.IsItemDetached(dr)) return;
            SelectedItemText = dr;
            if (!this.IsMyDialog) return;
            SetSelectedObject(dr);
        }

        public override bool SaveData()
        {
            if (!dgvRows.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = bsItems.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsTexts.HasChanges());
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

        private void dgvRows_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }


        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            bNav.DeleteCurrent();
        }

        private void bindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void bsTexts_CurrentChanged(object sender, EventArgs e)
        {
            var dr_text = bsTexts.GetCurrentItem<M_ITEMS_TEXTS>();
            tbText.Text = dr_text?.TEXT;
        }

        private void dgvRows_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvRows.CurrentCell == null) return;

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
                SetSelectedValueStr(null, true);
                e.Handled = true;
            }
        }

        private void dgvRows_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SetSelectedValueStr(null, true);
            }
        }

        private void dgvRows_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            e.Cancel = e.Row.IsNewRow || !AskCanDelete();
        }

        private void bNav_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void dgvRows_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            SelectCurrent();
        }

        private void cmSetResultNull_Click(object sender, EventArgs e)
        {
            SetResultNull = true;
            SetSelectedValueInt(0, false);
        }
    }
}
