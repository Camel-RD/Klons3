using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Klons3.ModelsA;
using KlonsA.Classes;
using KlonsF.Classes;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace KlonsA.Forms
{
    public partial class FormA_Professions : MyFormBaseF
    {
        public FormA_Professions()
        {
            InitializeComponent();
            CheckMyFontAndColors();

            var table = MyData.DbContextA.BL_A_PROFESSIONS;
            if (table.Count == 0)
            {
                bsProfCat.KillLists();
                bsProf.Fill();
                bsProfCat.RecreateLists();
            }

            bsProfCat.SetFilter<A_PROFESSIONS>(x => x.CAT == 1);

            dgvProf.AutoResizeRows();
        }

        private void Form_Professions_Load(object sender, EventArgs e)
        {
            CheckSave();
            dgvProf.AutoResizeRows();
        }

        private void SelectCurrent()
        {
            if (dgvProf.CurrentRow == null || dgvProf.CurrentRow.IsNewRow) return;
            var dr = bsProf.GetCurrentItem<A_PROFESSIONS>();
            if (!dgvProf.EndEdit()) return;
            if (!SaveData()) return;
            if (bsProf.IsItemDetached(dr)) return;
            if (!this.IsMyDialog) return;
            SetSelectedObject(dr);
        }

        private void dgvProf_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                SelectCurrent();
            }
        }

        private void dgvProf_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Return)
            {
                SelectCurrent();
                e.Handled = true;
            }
        }

        private void dgvProf_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SetSelectedObject(null);
            }
        }

        private void tbSearch_Enter(object sender, EventArgs e)
        {
            tbSearch.SelectAll();
        }

        private void tbSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                CheckFilter();
            }
        }

        private void bnavProf_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void dgvProf_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete())
                e.Cancel = true;
        }

        private void SetSaveButton(bool red)
        {
            bnavProf.SetSaveButton(tsbSave, red);
        }

        public override bool SaveData()
        {
            if (!dgvProf.EndEditX()) return false;
            var ret = bsProf.SaveDbContext();
            CheckSave();
            return ret.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsProf.HasChanges());
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
            CheckSave();
        }

        private void bsProf_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void dgvProf_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void CheckFilter(bool checkcat = false)
        {
            string sf, s = tbSearch.Text;
            string cat = cbCat.SelectedValue.AsString();
            if (cat == "") cat = null;
            if (checkcat && cat == "*")
            {
                tbSearch.Text = "";
                bsProf.RemoveFilter();
                return;
            }

            var pred_list = new List<Predicate<A_PROFESSIONS>>();

            if (!s.IsNOE())
                pred_list.Add(x => x.DESCR.ContainsCI(s));

            if (checkcat && cat == "**")
                pred_list.Add(x => x.USED == 1);

            bsProf.SetFilter(pred_list);
        }

        private void cbCat_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cat = cbCat.SelectedValue.AsString();
            if (string.IsNullOrEmpty(cat)) return;
            if (cat == "*" || cat == "**")
            {
                CheckFilter(true);
                return;
            }
            if (!string.IsNullOrEmpty(bsProf.Filter)) return;
            for (int i =0;i<bsProf.Count;i++)
            {
                var dr = bsProf.GetItem<A_PROFESSIONS>(i);
                if (dr == null) return;
                if (dr.ID == cat)
                {
                    bsProf.Position = i;
                }
            }
        }

    }
}
