﻿using System;
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
    public partial class FormARep_VacDays : MyFormBaseF
    {
        public FormARep_VacDays()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            tbDate.Text = Utils.DateToString(DateTime.Today);
            dgvRows.AutoGenerateColumns = false;
        }

        private void FormRep_VacDays_Load(object sender, EventArgs e)
        {
            InsertInToolStrip(toolStrip1, tbDate, 1);
        }

        public List<RepRowVacDays> RepRows = new List<RepRowVacDays>();

        private void tsbGetRows_Click(object sender, EventArgs e)
        {
            DateTime dt;
            if (!Utils.StringToDate(tbDate.Text, out dt))
            {
                MyMainForm.ShowWarning("Jānorāda aprēķina datums.");
                return;
            }
            
            var rep = new Report_VacDays();
            var er = rep.MakeReport(dt);
            var rowsf = rep.RepRows
                .Where(d => d.NotUsed != 0.0f)
                .OrderBy(d => d.Name);
            RepRows = new List<RepRowVacDays>(rowsf);
            bsRows.DataSource = RepRows;
        }

        private void tbDate_Enter(object sender, EventArgs e)
        {
            tbDate.SelectAll();
        }
    }
}
