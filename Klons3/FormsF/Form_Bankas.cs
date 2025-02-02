﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KlonsF.Classes;
using KlonsLIB.Data;

namespace KlonsF.Forms
{
    public partial class Form_Bankas : MyFormBaseF
    {
        public Form_Bankas()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            bsBanks.Fill();
        }

        private void Form_Bankas_Load(object sender, EventArgs e)
        {
            CheckSave();
        }

        private void bnavBanks_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void dgvBanks_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!e.Row.IsNewRow)
            {
                if (!AskCanDelete()) e.Cancel = true;
            }
        }

        private void dgvBanks_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                dgvBanks.MoveToNewRow();
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                bnavBanks.DeleteCurrent();
                e.Handled = true;
            }
        }

        private void SetSaveButton(bool red)
        {
            bnavBanks.SetSaveButton(tsbSave, red);
        }

        public override bool SaveData()
        {
            if (!dgvBanks.EndEditX()) return false;
            var ret = bsBanks.SaveDbContext();
            CheckSave();
            return ret.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsBanks.HasChanges());
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void bsBanks_ListChanged(object sender, ListChangedEventArgs e)
        {
            CheckSave();
        }
    }
}
