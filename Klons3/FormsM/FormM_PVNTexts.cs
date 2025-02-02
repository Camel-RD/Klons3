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
using KlonsF.Classes;
using Klons3.ModelsM;

namespace KlonsM.FormsM
{
    public partial class FormM_PVNTexts : MyFormBaseF
    {
        public FormM_PVNTexts()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        private void FormM_PVNTexts_Load(object sender, EventArgs e)
        {

        }

        public static int? GetPVNText(int idpvntext)
        {
            var fm = new FormM_PVNTexts();
            fm.FindPVNTextId(idpvntext);
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return null;
            return fm.SelectedValueInt;
        }

        public bool FindPVNTextId(int idstore)
        {
            if (bsRows.Count == 0) return false;
            for (int i = 0; i < bsRows.Count; i++)
            {
                var dr = bsRows.GetItem<M_PVNTEXTS>(i);
                if (dr == null) continue;
                if (dr.ID == idstore)
                {
                    bsRows.Position = i;
                    return true;
                }
            }
            return false;
        }

        private void SelectCurrent()
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            var dr = bsRows.GetCurrentItem<M_PVNTEXTS>();
            if (!dgvRows.EndEdit()) return;
            if (!SaveData()) return;
            if (bsRows.IsItemDetached(dr)) return;
            int id = dr.ID;
            if (!this.IsMyDialog) return;
            SetSelectedValueInt(id);
        }

        public override bool SaveData()
        {
            if (!dgvRows.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = bsRows.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsRows.HasChanges());
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
                SetSelectedObject(null, true);
                e.Handled = true;
            }
        }

        private void dgvRows_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SetSelectedObject(null, true);
            }
        }

        private void dgvRows_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            if (e.ColumnIndex == dgcCode.Index)
            {
                SelectCurrent();
                return;
            }
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

        private void bsRows_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void dgvRows_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            e.Cancel = e.Row.IsNewRow || !AskCanDelete();
        }

        private void bNav_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }
    }
}
