using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
    public partial class FormA_SalarySheetNewRow : MyFormBaseF
    {
        public FormA_SalarySheetNewRow()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            bsPersons.SetFilter<A_PERSONS>(x => x.USED == 1);
            bsAmati.SetFilter<A_POSITIONS>(x => x.USED == 1);
        }

        private void Form_SalarySheetNewRow_Load(object sender, EventArgs e)
        {
            PrepareForNew(SalarySheet);
        }

        private int _snr = 1;
        A_SALARY_SHEETS SalarySheet = null;

        public bool Execute(A_SALARY_SHEETS dr_sar, ref int snr, 
            out A_PERSONS dr_person, out A_POSITIONS dr_position)
        {
            SalarySheet = dr_sar;
            _snr = snr;
            var ret = this.ShowDialog(KlonsData.St.MyMainForm);
            if(ret != DialogResult.OK)
            {
                dr_person = null;
                dr_position = null;
                snr = -1;
                return false;
            }

            if (!int.TryParse(tbSnr.Text, out snr))
                snr = 1;

            dr_person = cbPerson.SelectedValue as A_PERSONS;
            dr_position = cbPosition.SelectedValue as A_POSITIONS;

            return true;
        }

        public void PrepareForNew(A_SALARY_SHEETS dr_sar)
        {
            /*
            int snr = 1;
            var dr_sar = MyData.DataSetKlonsA.SALARY_SHEETS.FindByID(id_sar);
            if(dr_sar != null)
            {
                snr = dr_sar.GetSALARY_SHEETS_RRowsByFK_SALARY_SHEETS_R_IDS().Count() + 1;
            }
            */
            tbSnr.Text = _snr.ToString();
        }

        private void cmDoIt_Click(object sender, EventArgs e)
        {
            if (cbPerson.SelectedValue == null ||
                cbPosition.SelectedValue == null)
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
