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
    public partial class FormA_PersonNew : MyFormBaseF
    {
        public FormA_PersonNew()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        public string FName = null;
        public string LName = null;
        public DateTime BirthDate = DateTime.MinValue;
        public string PK = null;
        public bool Male = true;
        public A_PERSONS Person = null;

        public string PositionTitle = null;
        public A_DEPARTMENTS Department = null;

        public DateTime EventDate = DateTime.Today;
        public bool MakeEvents = true;
        public string RepCode = "11";
        public string ProfCode = "?";

        private void Form_PersonNew_Load(object sender, EventArgs e)
        {
            this.SetControlsUpDownOrder([
                    [ tbFName ],
                    [ tbLName ],
                    [ tbBirthDate ],
                    [ tbPK ],
                    [ chMale ],
                    [ chFemale ],
                    [ tbPosition ],
                    [ cbDep ],
                    [ tbDate ],
                    [ chMakeEvents ],
                    [ tbRepCode ],
                    [ tbProfCode ],
                    [ cmOK ],
                    [ cmCancel ]
                ]);

            tbDate.Text = Utils.DateToString(DateTime.Today);
            tbRepCode.Text = "11";
            tbProfCode.Text = "?";
        }

        public string Check()
        {
            FName = tbFName.Text;
            LName = tbLName.Text;
            PK = tbPK.Text;
            PositionTitle = tbPosition.Text;

            if (string.IsNullOrEmpty(FName) || string.IsNullOrEmpty(LName))
                return "Jānorāda vārds, uzvārds.";
            if (FName.Length > 20 || LName.Length > 20)
                return "Vārds un uzvārds nevar būt garāks par 20 burtiem.";
            if (string.IsNullOrEmpty(tbBirthDate.Text) ||
                !Utils.StringToDate(tbBirthDate.Text, out BirthDate))
                return "Jānorāda dzimšanas datums.";
            if (PK.Length > 20)
                return "Personas kods nevar būt garāks par 20 simboliem.";
            Male = chMale.Checked;

            if (string.IsNullOrEmpty(PositionTitle))
                return "Jānorāda amata nosaukums.";
            if (PositionTitle.Length > 50)
                return "Amata nosaukums par garu.";

            if (cbDep.SelectedIndex == -1 || cbDep.SelectedValue == null)
                return "Nav norādīts departaments.";

            Department = cbDep.SelectedValue as A_DEPARTMENTS;

            if (!string.IsNullOrEmpty(PK))
            {
                var tablePersons = MyData.DbContextA.BL_A_PERSONS;
                var drp = tablePersons.Where(
                    d =>
                    d.PK == PK).ToArray();
                if (drp.Length > 0)
                    return "Darbinieks ar šādu personas kodu jau ir uzskaitē.";
            }

            MakeEvents = chMakeEvents.Checked;
            Utils.StringToDate(tbDate.Text, out EventDate);
            RepCode = tbRepCode.Text;
            ProfCode = tbProfCode.Text;
            if (MakeEvents)
            {
                if (string.IsNullOrEmpty(ProfCode) || ProfCode == "?")
                    return "Jānorāda profesijas kods.";
                if (string.IsNullOrEmpty(RepCode))
                    return "Jānorāda ziņu kods.";
                if (RepCode.Length > 5)
                    return "Ziņu kods par garu";
                if (ProfCode.Length > 7)
                    return "Profesijas kods par garu";
            }
            return "OK";
        }

        public void DoAdd()
        {
            var tablePersons = MyData.DbContextA.BL_A_PERSONS;
            var tablePersonsR = MyData.DbContextA.BL_A_PERSONS_R;
            var tableAmati = MyData.DbContextA.BL_A_POSITIONS;
            var tableAmatiR = MyData.DbContextA.BL_A_POSITIONS_R;
            var tableEvents = MyData.DbContextA.BL_A_EVENTS;

            var dr_p = tablePersons.NewItem();
            dr_p.FNAME = FName;
            dr_p.LNAME = LName;
            dr_p.BIRTH_DATE = BirthDate;
            dr_p.PK = PK;
            dr_p.GENDER = (short)(Male ? 0 : 1);
            tablePersons.Add(dr_p);

            Person = dr_p;

            var dr_pr = tablePersonsR.NewItem();

            dr_pr.Person = dr_p;
            dr_pr.EDIT_DATE = EventDate;
            dr_pr.FNAME = FName;
            dr_pr.LNAME = LName;
            if (!string.IsNullOrEmpty(dr_p.PK)) dr_pr.PERSON_CODE = dr_p.PK;
            tablePersonsR.Add(dr_pr);

            var dr_am = tableAmati.NewItem();

            dr_am.Person = dr_p;
            dr_am.TITLE = PositionTitle;
            dr_am.Department = Department;
            tableAmati.Add(dr_am);

            var dr_amr = tableAmatiR.NewItem();

            dr_amr.Position = dr_am;
            dr_amr.EDIT_DATE = EventDate;
            if (!dr_am.TITLE.IsNOE()) dr_amr.TITLE = dr_am.TITLE;
            if (dr_am.Department != null) dr_amr.Department = dr_am.Department;
            tableAmatiR.Add(dr_amr);

            var dr_evHire = tableEvents.NewItem();

            dr_evHire.EventCode = EEventId.Pieņemts;
            dr_evHire.Person = dr_p;
            dr_evHire.DATE1 = EventDate;
            dr_evHire.SCODE = RepCode;
            dr_evHire.OCCUPATION_CODE = ProfCode;
            tableEvents.Add(dr_evHire);

            var dr_evAssign = tableEvents.NewItem();

            dr_evAssign.EventCode = EEventId.Piešķirts_amats;
            dr_evAssign.Person = dr_p;
            dr_evAssign.Position = dr_am;
            dr_evAssign.DATE1 = EventDate;
            tableEvents.Add(dr_evAssign);
        }

        private void chMale_CheckedChanged(object sender, EventArgs e)
        {
            chFemale.Checked = !chMale.Checked;
        }

        private void chFemale_CheckedChanged(object sender, EventArgs e)
        {
            chMale.Checked = !chFemale.Checked;
        }

        private void cmOK_Click(object sender, EventArgs e)
        {
            var er = Check();
            if(er != "OK")
            {
                MyMainForm.ShowWarning(er);
                return;
            }
            DoAdd();
            DialogResult = DialogResult.OK;
        }

        private void control_KeyDown(object sender, KeyEventArgs e)
        {
            OnNaviKey(sender, e);
        }
    }
}
