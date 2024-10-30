using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KlonsA.Classes;
using KlonsF.Classes;
using KlonsLIB.Data;
using KlonsLIB.Forms;
using KlonsLIB.Misc;
using KlonsLIB.Components;
using System.Globalization;
using DataObjectsA;
using Klons3.ModelsA;

namespace KlonsA.Forms
{
    public partial class FormA_PayListImport : MyFormBaseF
    {
        public FormA_PayListImport()
        {
            InitializeComponent();
            CheckMyFontAndColors();

            bsRows.DataSource = ListRows;
        }

        private void Form_PayListImport_Load(object sender, EventArgs e)
        {
        }

        public BindingList<PaylistImportRow> ListRows = new BindingList<PaylistImportRow>();

        private void dgvRows_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if (e.ColumnIndex == dgcIDAM.Index)
            {
                var dr_person = dgvRows[dgcIDP.Index, e.RowIndex].Value as A_PERSONS;
                if (dr_person == null)
                {
                    e.Cancel = true;
                    return;
                }
                var c0 = dgvRows[e.ColumnIndex, e.RowIndex];
                var c = c0 as DataGridViewComboBoxCell;
                if (c == null) return;
                bsAmatiF.SetFilter<A_POSITIONS>(x => x.USED == 1 && x.Person == dr_person);
                c.DataSource = bsAmatiF;
            }

        }

        private void dgvRows_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgcIDP.Index)
            {
                var dr = dgvRows.CurrentRow.DataBoundItem as PaylistImportRow;
                var dr_person = dgvRows[dgcIDP.Index, e.RowIndex].Value as A_PERSONS;
                var dr_position = DataTasksA.GetPersonsUsedPosition(dr_person);
                dr.Position = dr_position;
                dgvRows.RefreshCurrent();
                return;
            }
            if (e.ColumnIndex == dgcIDAM.Index)
            {
                var c = dgvRows[e.ColumnIndex, e.RowIndex] as DataGridViewComboBoxCell;
                c.DataSource = bsAmati;
                return;
            }
        }

        private void dgvRows_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.ColumnIndex == dgcDate.Index)
            {
                if (e.Value == null) return;
                if (!Utils.DGVParseDateCell(e))
                {
                    e.Value = null;
                    e.ParsingApplied = true;
                    return;
                }
            }
        }

        public void GetIDPFromDialog()
        {
            if (bsRows.Count == 0 || bsRows.Current == null) return;
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            var fm = new FormA_Persons();
            fm.WhatToSelect = FormA_Persons.EWhatToSelect.Both;
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return;
            var dr = dgvRows.CurrentRow.DataBoundItem as PaylistImportRow;
            if (dgvRows.CurrentCell != null)
            {
                try
                {
                    if (dgvRows.CurrentCell.OwningColumn == dgcIDP)
                    {
                        dgvRows.BeginEdit(false);
                        var c = dgvRows.EditingControl as DataGridViewComboBoxEditingControl;
                        c.SelectedValue = fm.SelectedPerson;
                        dgvRows.EndEdit();
                        dr.Position = fm.SelectedPosition;
                    }
                    if (dgvRows.CurrentCell.OwningColumn == dgcIDAM)
                    {
                        dgvRows.EndEdit();
                        dr.Person = fm.SelectedPerson;
                        dgvRows.BeginEdit(false);
                        var c = dgvRows.EditingControl as DataGridViewComboBoxEditingControl;
                        c.SelectedValue = fm.SelectedPosition;
                        dgvRows.EndEdit();
                    }
                }
                catch (Exception) { }
            }
        }

        private void dgvRows_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow ||
                dgvRows.CurrentCell == null) return;
            int colnr = dgvRows.CurrentCell.ColumnIndex;
            if (colnr != dgcIDP.Index && colnr != dgcIDAM.Index) return;
            GetIDPFromDialog();
        }

        public string ParseText(string text)
        {
            if (text.IsNOE())
                return "Nav iekopēti dati teksta laukā";
            ListRows.Clear();
            var lines = text.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach(var line in lines)
            {
                var parts = line.Split('\t');
                DateTime dt;
                string name = parts[1];
                decimal amount = 0M;
                A_PERSONS dr_person = null;
                A_POSITIONS dr_position = null;

                if(parts.Length != 3 ||
                    !Utils.StringToDate(parts[0], out dt) ||
                    name.IsNOE() ||
                    parts[2].IsNOE() ||
                    !decimal.TryParse(parts[2], out amount))
                {
                    return "Kļūda rindā:\n" + string.Join(" ", parts);
                }

                var table_persons = MyData.DbContextA.BL_A_PERSONS;
                dr_person = table_persons
                    .Where(x => x.FNAME + " " + x.LNAME == name)
                    .FirstOrDefault();

                if(dr_person != null)
                {
                    dr_position = dr_person.Positions
                        .Where(x => x.USED == 1)
                        .FirstOrDefault();
                }

                var new_row = new PaylistImportRow()
                {
                    Date = dt,
                    Name = parts[1],
                    Amount = amount,
                    Person = dr_person,
                    Position = dr_position
                };
                ListRows.Add(new_row);
            }

            return "OK";
        }

        public string CheckRows()
        {
            if (ListRows.Count == 0)
                return "Saraksts ir tukšs";
            foreach(var row in ListRows)
            {
                if (row.Date == null)
                    return "Rindai nav norādīts datums";
                if (row.Person == null)
                    return "Rindai nav norādīts darbinieks";
                if (row.Position == null)
                    return "Rindai nav norādīts darbinieka amats";
            }
            var gr = ListRows
                .GroupBy(x => (x.Date, x.Person, x.Position))
                .Where(x => x.Count() > 1)
                .Select(x => x.First())
                .Select(x => Utils.DateToString(x.Date.Value) + " " + x.Name)
                .ToList();
            
            if(gr.Count > 0)
            {
                var sgr = string.Join("\r\n", gr);
                return "Darbiniekam vairāki maksājumi vienā datumā:\r\n" + sgr;
            }
            return "OK";
        }

        private void tbReadText_Click(object sender, EventArgs e)
        {
            var rt = ParseText(tbText.Text);
            if(rt != "OK")
            {
                MyMainForm.ShowWarning(rt);
                return;
            }
        }

        private void tbMakeLists_Click(object sender, EventArgs e)
        {
            var rt = CheckRows();
            if (rt != "OK")
            {
                MyMainForm.ShowWarning(rt);
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }


}
