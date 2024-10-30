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
using KlonsLIB.Misc;
using Klons3.ModelsA;
using Org.BouncyCastle.Crypto;

namespace KlonsA.Forms
{
    public partial class FormA_TimeSheetEdit : MyFormBaseF
    {
        public FormA_TimeSheetEdit()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            bsPersons.SetFilter<A_PERSONS>(x => x.USED == 1);
            bsPlan.SetFilter<A_TIMEPLAN_LIST>(x => x.USED == 1);
            bsAmati.SetFilter<A_POSITIONS>(x => x.USED == 1);
        }

        private void Form_TimeSheetEdit_Load(object sender, EventArgs e)
        {
            if (DoNew)
                PrepareForNew(TimeSheet);
            else
                PrepareForEdit(TimeSheetRow);
        }

        private A_TIMESHEET_LISTS TimeSheet = null;
        private A_TIMESHEET_LISTS_R TimeSheetRow = null;
        private bool DoNew = false;


        public bool Execute(bool donew, 
            A_TIMESHEET_LISTS dr_timesheet, A_TIMESHEET_LISTS_R dr_timesheetrow,
            out int snr, out A_PERSONS dr_person, out A_POSITIONS dr_position, 
            out A_TIMEPLAN_LIST dr_plan, out bool plind, out bool night, out bool overtime)
        {
            TimeSheet = dr_timesheet;
            TimeSheetRow = dr_timesheetrow;
            DoNew = donew;

            var ret = this.ShowDialog(KlonsData.St.MyMainForm);
            if(ret != DialogResult.OK)
            {
                dr_person = null;
                dr_position = null;
                dr_plan = null;
                snr = -1;
                plind = false;
                night = false;
                overtime = false;
                return false;
            }

            if (!int.TryParse(tbSnr.Text, out snr))
                snr = 1;
            dr_person = (A_PERSONS)cbPerson.SelectedValue;
            dr_position = (A_POSITIONS)cbPosition.SelectedValue;
            dr_plan = (A_TIMEPLAN_LIST)cbPlan.SelectedValue;
            plind = chPlanIndividual.Checked;
            night = chNight.Checked;
            overtime = chOvertime.Checked;

            return true;
        }

        public void PrepareForNew(A_TIMESHEET_LISTS dr_timesheet)
        {
            this.Text = "Jauns darba laika lapas ieraksts";
            cmDoIt.Text = "Pievienot";
            int snr = 1;
            if(dr_timesheet != null)
            {
                snr = dr_timesheet.TimesheetListRows.Count + 1;
            }
            tbSnr.Text = snr.ToString();
        }

        public void PrepareForEdit(A_TIMESHEET_LISTS_R dr_sar_r)
        {
            this.Text = "Rediģēt darba laika lapas ierakstu";
            cmDoIt.Text = "Saglabāt";

            var table_sar_r = MyData.DbContextA.BL_A_TIMESHEET_LISTS_R;

            tbSnr.Text = dr_sar_r.SNR.ToString();

            int nr_pers = FindPerson(dr_sar_r.Person);
            bsPersons.Position = nr_pers;

            int nr_amats = FindAmats(dr_sar_r.Position);
            bsAmati.Position = nr_amats;

            int nr_plan = FindPlan(dr_sar_r.TimePlan);
            bsPlan.Position = nr_plan;

            chPlanIndividual.Checked = dr_sar_r.XPlanType == EPlanType.Individual;
            chNight.Checked = dr_sar_r.NIGHT == 1;
            chOvertime.Checked = dr_sar_r.OVERTIME == 1;
        }

        private int FindPerson(A_PERSONS dr_person)
        {
            for (int i = 0; i < bsPersons.Count; i++)
            {
                var dr = bsPersons.GetItem<A_PERSONS>(i);
                if (dr == dr_person) return i;
            }
            return -1;
        }

        private int FindAmats(A_POSITIONS dr_position)
        {
            for (int i = 0; i < bsAmati.Count; i++)
            {
                var dr = bsAmati.GetItem<A_POSITIONS>(i);
                if (dr == dr_position) return i;
            }
            return -1;
        }
        private int FindPlan(A_TIMEPLAN_LIST dr_plan)
        {
            for (int i = 0; i < bsPlan.Count; i++)
            {
                var dr = bsPlan.GetItem<A_TIMEPLAN_LIST>(i);
                if (dr == dr_plan) return i;
            }
            return -1;
        }

        private void cmDoIt_Click(object sender, EventArgs e)
        {
            if (cbPerson.SelectedValue == null ||
                cbPosition.SelectedValue == null ||
                cbPlan.SelectedValue == null)
            {
                KlonsData.St.MyMainForm.ShowWarning("Datu lauki nav aizpildīti.");
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void cmCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        public void GetIDPFromDialog()
        {
            var fm = new FormA_Persons();
            fm.WhatToSelect = FormA_Persons.EWhatToSelect.Both;
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return;
            cbPerson.SelectedValue = fm.SelectedPerson;
            cbPosition.SelectedValue = fm.SelectedPosition;
        }

        private void cmGetPerson_Click(object sender, EventArgs e)
        {
            GetIDPFromDialog();
        }
    }
}
