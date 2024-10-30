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
using Klons3.ModelsA;

namespace KlonsA.Forms
{
    public partial class FormA_PayListsRecalcParams : MyFormBaseF
    {
        public FormA_PayListsRecalcParams()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            bsPersons.SetFilter<A_PERSONS>(x => x.USED == 1);
        }

        private void Form_PayListsRecalcParams_Load(object sender, EventArgs e)
        {
            cbPerson.Text = null;
            tbDate1.Text = Utils.DateToString(DateFrom);
            tbDate1.SelectAll();
        }

        public DateTime DateFrom = DateTime.MinValue;
        public A_PERSONS PersonRow = null;

        public static bool ShowDialog(out DateTime datefrom, out A_PERSONS dr_person)
        {
            var form = new FormA_PayListsRecalcParams();
            form.DateFrom = DataLoaderA.LoadedDT1;
            var rt = form.ShowMyDialogModal();
            datefrom = form.DateFrom;
            dr_person = form.PersonRow;
            return rt == DialogResult.OK;
        }

        private bool CheckParams()
        {
            DateFrom = DataLoaderA.LoadedDT1;
            if (!string.IsNullOrEmpty(tbDate1.Text))
                DateFrom = Utils.StringToDate(tbDate1.Text).Value;
            PersonRow = null;
            if (cbPerson.SelectedIndex > -1 && cbPerson.SelectedValue != null)
                PersonRow = cbPerson.SelectedValue as A_PERSONS;
            return true;
        }

        private void cmRecalc_Click(object sender, EventArgs e)
        {
            CheckParams();
            DialogResult = DialogResult.OK;
        }

    }
}
