﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using KlonsA.Classes;
using KlonsF.Classes;
using KlonsLIB.Data;
using KlonsLIB.Forms;
using KlonsLIB.Misc;
using Klons3.ModelsA;

namespace KlonsA.Forms
{
    public partial class FormA_UntaxedMinimumImport : MyFormBaseF
    {
        public FormA_UntaxedMinimumImport()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            dgvRows.AutoGenerateColumns = false;
        }

        private void Form_UntaxedMinimumImport_Load(object sender, EventArgs e)
        {

        }

        List<UntMinImportData> Changes = null;


        public void DoRead()
        {
            if (tbDate1.Text.IsNOE() || tbDate2.Text.IsNOE())
            {
                MyMainForm.ShowWarning("Nav norādīti datumi.");
                return;
            }

            if (!Utils.StringToDate(tbDate1.Text, out var dt1) ||
                !Utils.StringToDate(tbDate2.Text, out var dt2) ||
                dt2 < dt1)
            {
                MyMainForm.ShowWarning("Norādīts nekorekts datums");
                return;
            }

            var ofd = new OpenFileDialog();
            ofd.Filter = "XML faili (*.xml)|*.xml";
            //ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            ofd.CheckFileExists = true;
            ofd.Multiselect = false;
            ofd.Title = "Norādi EDS pārskata xml failu";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog(MyMainForm) != DialogResult.OK) return;
            var fnm = ofd.FileName;

            var importer = new EGrmUntMinImporter();
            var rt_msg = importer.GetUntMinResult(fnm, dt1, dt2, out var missingnames, out var changes);
            Changes = changes;
            bsRows.DataSource = changes;
            if (rt_msg != "OK")
            {
                MyMainForm.ShowWarning(rt_msg);
                return;
            }
            if (missingnames.Count > 0)
            {
                var msg = string.Join("\n", missingnames);
                MyMainForm.ShowInfo("Nav datu par šiem darbiniekiem:\n" + msg);
            }
            if (changes.Count == 0)
            {
                MyMainForm.ShowInfo("Pārskats nesatur jaunus datus.");
            }
        }

        public void UpdateDB()
        {
            if(Changes == null || Changes.Count == 0)
            {
                MyMainForm.ShowInfo("Iegrāmatošanai nav datu.");
                return;
            }
            var table = MyData.DbContextA.BL_A_UNTAXED_MIN;
            foreach(var change in Changes)
            {
                var dr_um = table
                    .Where(d => d.Person == change.PersonRow && d.ONDATE == change.Dt)
                    .FirstOrDefault();
                if(dr_um == null)
                {
                    dr_um = table.NewItem();
                    dr_um.Person = change.PersonRow;
                    dr_um.ONDATE = change.Dt;
                    dr_um.UNTAXED_MIN = change.UntMin;
                    dr_um.IIN_RATE_TYPE = change.IINRateType;
                    table.Add(dr_um);
                }
                else
                {
                    dr_um.UNTAXED_MIN = change.UntMin;
                    dr_um.IIN_RATE_TYPE = change.IINRateType;
                }

                MyData.DbContextA.Save();
            }
        }

        private void RefreshUntMinForm()
        {
            var frm = MyMainForm.FindForm(typeof(FormA_UntaxedMinimum)) as FormA_UntaxedMinimum;
            if (frm == null) return;
            frm.DoCmFilte();
        }

        private void cmLoadFromFile_Click(object sender, EventArgs e)
        {
            DoRead();
        }

        private void cmUpdateDB_Click(object sender, EventArgs e)
        {
            UpdateDB();
            RefreshUntMinForm();
            Close();
        }

        private void dgvRows_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if(e.ColumnIndex == dgcIINRate.Index)
            {
                int val = (int)e.Value;
                var sval = val == 1 ? "23" : "20";
                e.Value = sval;
                e.FormattingApplied = true;
            }
        }
    }


}
