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
using KlonsLIB.Data;
using KlonsLIB.Forms;
using KlonsLIB.Misc;
using Klons3.ModelsA;

namespace KlonsA.Forms
{
    public partial class FormA_UntaxedMinimum : MyFormBaseF
    {
        public FormA_UntaxedMinimum()
        {
            InitializeComponent();
            CheckMyFontAndColors();

            dgcIINRateType.DataSource = SomeDataDefsA.IINLikmesVeids;
            dgcIINRateType.DisplayMember = "Val";
            dgcIINRateType.ValueMember = "Key";

        }

        DateTime FilterDate1 = DateTime.MinValue;
        DateTime FilterDate2 = DateTime.MaxValue;
        DateTime LastDate = DateTime.MinValue;

        private void Form_UntaxedMinimum_Load(object sender, EventArgs e)
        {
            cbPerson.Text = null;
        }


        public void MarkRowsForFilter(DateTime dt1, DateTime dt2)
        {
            var table = MyData.DbContextA.BL_A_UNTAXED_MIN;
            foreach(var dr in table.Where(d => d.ONDATE > dt2))
            {
                dr.FilterTag = 0;
            }
            foreach (var dr in table.Where(d => d.ONDATE > dt1 && d.ONDATE <= dt1))
            {
                dr.FilterTag = 1;
            }

            var drs_gr = table
                .Where(d => d.ONDATE <= dt1)
                .GroupBy(d => d.Person);
            
            foreach(var gr in drs_gr)
            {
                var dr_person = gr.Key;
                if (!DataTasksA.IsPersonWorking(dr_person, dt1, dt2))
                {                     
                    foreach (var gr2 in gr)
                    {
                        gr2.FilterTag = 0;
                    }
                    continue;
                }
                var gr1 = gr.OrderByDescending(d => d.ONDATE);
                var gr1_first = gr1.First();
                gr1_first.FilterTag = 1;
                var gr1_rest = gr1.Skip(1);
                foreach (var gr2 in gr1_rest)
                {
                    gr2.FilterTag = 0;
                }
            }

        }

        public void DoFilter()
        {
            DateTime dt1 = DateTime.MinValue;
            DateTime dt2 = DateTime.MaxValue;
            if (!string.IsNullOrEmpty(tbDate1.Text))
                dt1 = Utils.StringToDate(tbDate1.Text).Value;
            if (!string.IsNullOrEmpty(tbDate2.Text))
                dt2 = Utils.StringToDate(tbDate2.Text).Value;
            A_PERSONS dr_person = null;
            if (cbPerson.SelectedIndex > -1 && cbPerson.SelectedValue != null)
                dr_person = cbPerson.SelectedValue as A_PERSONS;
            if (dt1 == DateTime.MinValue && dt2 == DateTime.MaxValue && dr_person == null)
            {
                bsRows.RemoveFilter();
                FilterDate1 = DateTime.MinValue;
                FilterDate2 = DateTime.MaxValue;
                return;
            }
            var pred_list = new List<Predicate<A_UNTAXED_MIN>>();
            if (dr_person != null)
            {
                pred_list.Add(x => x.Person == dr_person);
            }
            if (dt1 != DateTime.MinValue || dt2 != DateTime.MaxValue)
            {
                MarkRowsForFilter(dt1, dt2);
                pred_list.Add(x => x.FilterTag == 1);
                FilterDate1 = dt1;
                FilterDate2 = dt2;
                LastDate = dt1;
            }
            bsRows.SetFilter(pred_list);
        }

        private void dgvRows_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if(e.ColumnIndex == dgcOnDate.Index)
            {
                Utils.DGVParseDateCell(e);
                if (e.Value != null && e.Value != DBNull.Value && e.Value is DateTime)
                    LastDate = (DateTime)e.Value;
                return;
            }
        }

        public void DeleteCurrent()
        {
            bNav.DeleteCurrent();
            SaveData();
            CheckSave();
        }

        private void SetSaveButton(bool red)
        {
            bNav.SetSaveButton(bniSave, red);
        }

        public override bool SaveData()
        {
            if (!dgvRows.EndEditX()) return false;
            var ret = bsRows.SaveDbContext();
            CheckSave();
            return ret.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsRows.HasChanges());
        }

        private void bniSave_Click(object sender, EventArgs e)
        {
            SaveData();
            CheckSave();
        }

        private void bsRows_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void bNav_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void dgvRows_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete())
            {
                e.Cancel = true;
            }
        }

        private void dgvRows_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void dgvRows_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                DeleteCurrent();
                e.Handled = true;
            }
        }

        public void DoCmFilte()
        {
            if (!SaveData()) return;
            DoFilter();
            CheckSave();
        }

        private void cmFilter_Click(object sender, EventArgs e)
        {
            DoCmFilte();
        }

        private void dgvRows_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            DateTime dt = DateTime.MinValue;
            if (LastDate != DateTime.MinValue) dt = LastDate;
            else if (FilterDate1 != DateTime.MinValue) dt = FilterDate1;
            else dt = DateTime.Today;
            e.Row.Cells[dgcOnDate.Index].Value = dt;
        }

        private void bniXMLImport_Click(object sender, EventArgs e)
        {
            if (!SaveData()) return;
            var form = MyMainForm.ShowFormDialog<FormA_UntaxedMinimumImport>();
        }
    }
}
