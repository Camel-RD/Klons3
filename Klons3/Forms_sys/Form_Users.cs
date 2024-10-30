using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;
using Equin.ApplicationFramework;
using KlonsF.Classes;
using KlonsLIB.Data;
using KlonsLIB.Forms;
using Microsoft.EntityFrameworkCore;

namespace KlonsF.Forms
{
    public partial class Form_Users : MyFormBaseF
    {
        public Form_Users()
        {
            InitializeComponent();
            CheckMyFontAndColors();

            var blv = bsUsers.BindingListView as BindingListView<Klons3.ModelsF.USERS>;
            var filter = BindingListView<Klons3.ModelsF.USERS>.CreateItemFilter(x => string.Compare(x.NM, "SYSTEM", true) != 0);
            blv.Filter = filter;
        }

        private void FormUsers_Load(object sender, EventArgs e)
        {
            this.bsUsers.Fill();
        }

        private int CountAdmins()
        {
            var blv = bsUsers.BindingListView as BindingListView<Klons3.ModelsF.USERS>;
            int ct = blv.Count(x => x.TP == "A" && x.NM.ToUpper() != "SYSTEM");
            return ct;
        }

        private bool AskCanDeleteA()
        {
            if (bsUsers.Current == null || bsUsers.Count == 1) return false;
            if ((bsUsers.Current as ObjectView<Klons3.ModelsF.USERS>).Object.TP == "A")
            {
                if (CountAdmins() == 1)
                {
                    MyMainForm.ShowWarning("Nedrīkst izdzēst pēdējo administratoru.");
                    return false;
                }
            }
            
            if (!MyData.Params.AskBeforeDelete) return true;
            DialogResult response = MyMessageBox.Show("Vai dzēst lietotāju?",
                "",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2,
                MyMainForm);

            return response == DialogResult.Yes;
        }

        private void bnavUsers_ItemDeleting(object sender, CancelEventArgs e)
        {
            if (!dgvUsers.EndEditX()) return;
            e.Cancel = !AskCanDeleteA();
        }

        private void dgvUsers_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!e.Row.IsNewRow)
            {
                if (!AskCanDeleteA()) e.Cancel = true;
            }
        }

        private void dgvUsers_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                dgvUsers.MoveToNewRow();
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                bnavUsers.DeleteCurrent();
                e.Handled = true;
            }
        }

        private void dgvUsers_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dgvUsers.NewRowIndex == e.RowIndex ||
                !dgvUsers.IsCurrentRowDirty) return;
            var o = dgvUsers.CurrentRow.Cells[dgcUsersName.Index].Value;
            if (o == null || o == DBNull.Value || o.ToString() == "SYSTEM" || 
                o.ToString().Length > 15)
            {
                MyMainForm.ShowWarning("Nekorekts lietotāja vārds");
                e.Cancel = true;
            }
            if (CountAdmins() == 0)
            {
                MyMainForm.ShowWarning("Nedrīkst izdzēst pēdējo administratoru.");
                e.Cancel = true;
            }
        }


        private void SetSaveButton(bool red)
        {
            bnavUsers.SetSaveButton(tsbSave, red);
        }

        public override bool SaveData()
        {
            if (!dgvUsers.EndEditX()) return false;
            var ret = bsUsers.SaveDbContext();
            return ret.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsUsers.HasChanges());
        }


        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
            CheckSave();
        }

        private void dgvUsers_MyCheckForChanges(object sender, EventArgs e)
        {
            CheckSave();
        }

        private void bsUsers_ListChanged(object sender, ListChangedEventArgs e)
        {
            CheckSave();
        }


    }
}
