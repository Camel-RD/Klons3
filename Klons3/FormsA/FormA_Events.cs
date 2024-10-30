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
using KlonsLIB.Forms;
using KlonsLIB.Misc;

namespace KlonsA.Forms
{
    public partial class FormA_Events : MyFormBaseF
    {
        public FormA_Events()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            LoadColumnWidthsFromSettings();
        }

        private void Form_Events_Load(object sender, EventArgs e)
        {
            SetupToolStrips();
            cbFilterEvent2.SelectedValue = null;
            cbFilterMode.SelectedValue = "0";
        }

        private void LoadColumnWidthsFromSettings()
        {
            string scw = MyData.Settings.ColumnWidths_AEvents;
            (int ver, int[] cw) = dgvEvents.ParseColumnWidths(scw);
            if (ver == 1 && cw != null && cw.Length > 0)
                dgvEvents.SetColumnWidths(cw);
        }

        private void SaveColumnWidthsToSettings()
        {
            MyData.Settings.ColumnWidths_AEvents = dgvEvents.GetColumnWidths2(10.0f, 1);
        }

        public override void SaveParams()
        {
            SaveColumnWidthsToSettings();
        }

        private void SetupToolStrips()
        {
            InsertInToolStrip(toolStrip1, tbDT1, 0);
            InsertInToolStrip(toolStrip1, tbDT2, 2);
            InsertInToolStrip(toolStrip1, cbFilterEvent, 4);
            InsertInToolStrip(toolStrip1, cbFilterEvent2, 5);
            InsertInToolStrip(toolStrip1, cbFilterMode, 6);
        }

        private void dgvEvents_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var dr = dgvEvents.GetObjectViewItem<A_EVENTS>(e.RowIndex);
            /*
            if (e.ColumnIndex == dgcIDP.Index)
            {
                var drp = MyData.DataSetKlonsA.PERSONS.FindByID(dr.IDP);
                if (drp == null) return;
                e.Value = string.Format("{0} {1}", drp.FNAME, drp.LNAME);
                e.FormattingApplied = true;
                return;
            }*/
            if (e.ColumnIndex == dgcIDA.Index)
            {
                if (dr.Position == null) return;
                e.Value = dr.Position?.TITLE;
                e.FormattingApplied = true;
                return;
            }
            if (e.ColumnIndex == dgcIDN.Index)
            {
                var drn = dr.EventType;
                if (drn == null) return;
                e.Value = drn.DESCR;
                e.FormattingApplied = true;
                return;
            }
        }

        private DateTime date1, date2;
        private bool filterEventCodes = false;
        private bool filterEventCodes2 = false;
        private int eventid1, eventid2;
        enum EFilterMode { ByFirstDate, ByState};
        private EFilterMode FilterMode = EFilterMode.ByFirstDate;

        private string CheckFilterParams()
        {
            DateTime dt1 = DateTime.MinValue, dt2 = DateTime.MaxValue;
            if ((!string.IsNullOrEmpty(tbDT1.Text) && !Utils.StringToDate(tbDT1.Text, out dt1)) ||
                (!string.IsNullOrEmpty(tbDT2.Text) && !Utils.StringToDate(tbDT2.Text, out dt2)) ||
                dt1 > dt2)
                return "Nekorekti norādīti datuni.";
            if (cbFilterEvent.SelectedValue == null)
            {
                filterEventCodes = false;
            }
            else
            {
                eventid1 = (int)cbFilterEvent.SelectedValue;
                filterEventCodes = true;
            }
            if (cbFilterEvent2.SelectedValue == null)
            {
                filterEventCodes2 = false;
            }
            else
            {
                eventid2 = (int)cbFilterEvent2.SelectedValue;
                filterEventCodes2 = true;
            }

            FilterMode = cbFilterMode.SelectedValue as string == "0" ? EFilterMode.ByFirstDate : EFilterMode.ByState;

            date1 = dt1;
            date2 = dt2;

            return "OK";
        }

        private void DoFilter()
        {
            var ret = CheckFilterParams();
            if (ret != "OK")
            {
                MyMainForm.ShowWarning(ret);
                return;
            }
            var filter_list = new List<Predicate<A_EVENTS>>();
            if(FilterMode == EFilterMode.ByFirstDate)
                filter_list.Add(x => x.DATE1 >= date1 && x.DATE1 <= date2);
            else
                filter_list.Add(x => x.DATE1 <= date2 && x.DATE2 != null && x.DATE2.Value >= date1);
            if (filterEventCodes)
                filter_list.Add(x => x.IDN == eventid1);
            if (filterEventCodes2)
                filter_list.Add(x => x.IDN2 == eventid2);
            bsEvents.SetFilter(filter_list);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DoFilter();
        }

    }
}
