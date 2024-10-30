using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KlonsP.Classes;
using KlonsF.Classes;
using KlonsLIB.Forms;
using KlonsLIB.Components;
using KlonsLIB.Misc;
using KlonsLIB.MySourceGrid.GridRows;
using KlonsLIB.Data;
using Klons3.ModelsP;
using Equin.ApplicationFramework;

namespace KlonsP.Forms
{
    public partial class FormP_Events : MyFormBaseF
    {
        public FormP_Events()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            dgvEvents.AutoGenerateColumns = false;

            dgvFilterBaseHeight = dgvFilter.Height;
            var hScrollbar = dgvFilter.Controls.OfType<HScrollBar>().First();
            hScrollbar.VisibleChanged += HScrollbar_VisibleChanged;
        }

        private int dgvFilterBaseHeight = 0;

        private void Form_Events_Load(object sender, EventArgs e)
        {
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            //dgvFilter.PerformLayout();
            dgvFilter.Height = dgvFilter.ColumnHeadersHeight + dgvFilter.RowTemplate.Height + 4;
        }

        private void CheckDGVFilterHeight()
        {
            var hScrollbar = dgvFilter.Controls.OfType<HScrollBar>().First();
            if (hScrollbar.Visible)
                dgvFilter.Height = dgvFilterBaseHeight + System.Windows.Forms.SystemInformation.HorizontalScrollBarHeight;
            else
                dgvFilter.Height = dgvFilterBaseHeight;
        }

        private void HScrollbar_VisibleChanged(object sender, EventArgs e)
        {
            CheckDGVFilterHeight();
        }

        private void dgvFilter_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if(e.ColumnIndex == dgcFilterDate1.Index || e.ColumnIndex == dgcFilterDate2.Index)
            {
                Utils.DGVParseDateCell(e);
                return;
            }
        }

        private void dgvEvents_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || bsRows.Count == 0) return;
            var dr = (bsRows[e.RowIndex] as ObjectView<P_ITEMS_EVENTS>)?.Object;
            if (dr == null) return;
            if (e.ColumnIndex == dgcEventsEvent.Index)
            {
                e.Value = dr.EVENTRow.CODE;
                e.FormattingApplied = true;
            }
            else if (e.ColumnIndex == dgcEventsCat1.Index)
            {
                e.Value = dr.CAT1Row.CODE;
                e.FormattingApplied = true;
            }
            else if (e.ColumnIndex == dgcEventsCatD.Index)
            {
                e.Value = dr.CATDRow.CODE;
                e.FormattingApplied = true;
            }
            else if (e.ColumnIndex == dgcEventsCatT.Index)
            {
                e.Value = dr.CATTRow.CODE;
                e.FormattingApplied = true;
            }
            else if (e.ColumnIndex == dgcEventsDepartment.Index)
            {
                e.Value = dr.DEPARTMENTRow.CODE;
                e.FormattingApplied = true;
            }
            else if (e.ColumnIndex == dgcEventsPlace.Index)
            {
                e.Value = dr.PLACERow.CODE;
                e.FormattingApplied = true;
            }

        }

        private void DoFilter2()
        {
            var table_events = MyData.DbContextP.P_ITEMS_EVENTS;
            var drs = table_events
                .Where(
                    d =>
                    (filterData1.fDATE1 == null || d.DT >= filterData1.fDATE1) &&
                    (filterData1.fDATE2 == null || d.DT <= filterData1.fDATE2) &&
                    (filterData1.fEVENT == null || d.EVENT == filterData1.fEVENT) &&
                    (filterData1.fCAT1 == null || d.CAT1 == filterData1.fCAT1) &&
                    (filterData1.fCATD == null || d.CATD == filterData1.fCATD) &&
                    (filterData1.fCATT == null || d.CATT == filterData1.fCATT) &&
                    (filterData1.fDEPARTMENT == null || d.DEPARTMENT == filterData1.fDEPARTMENT) &&
                    (filterData1.fPLACE == null || d.PLACE == filterData1.fPLACE))
                .ToList();
            var bl = new BindingList<P_ITEMS_EVENTS>(drs);
            bsRows2.DataSource = bl;
        }

        private string DateTosExprStr(DateTime dt) => $"#{dt:d}#";

        public void DoFilter()
        {
            string fs = "";
            var pfilters = new List<Predicate<P_ITEMS_EVENTS>>();
            if (filterData1.fEVENT != null)
            {
                pfilters.Add(x => x.EVENT == filterData1.fEVENT.Value);
            }
            if (filterData1.fDATE1 != null)
            {
                pfilters.Add(x => x.DT >= filterData1.fDATE1.Value);
            }
            if (filterData1.fDATE2 != null)
            {
                pfilters.Add(x => x.DT <= filterData1.fDATE2.Value);
            }
            if (filterData1.fCAT1 != null)
            {
                pfilters.Add(x => x.CAT1 == filterData1.fCAT1.Value);
            }
            if (filterData1.fCATD != null)
            {
                pfilters.Add(x => x.CATD == filterData1.fCATD.Value);
            }
            if (filterData1.fCATT != null)
            {
                pfilters.Add(x => x.CATT == filterData1.fCATT.Value);
            }
            if (filterData1.fDEPARTMENT != null)
            {
                pfilters.Add(x => x.DEPARTMENT == filterData1.fDEPARTMENT.Value);
            }
            if (filterData1.fPLACE != null)
            {
                pfilters.Add(x => x.PLACE == filterData1.fPLACE.Value);
            }

            if (pfilters.Count == 0)
            {
                bsRows.RemoveFilter();
            }
            else
            {
                var pred_filter = Utils.JoinPredicatesWitnAnd(true, pfilters.ToArray());
                bsRows.SetFilter(pred_filter);
            }
        }

        private void dgvFilter_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //DoFilter();
        }

        private void dgvFilter_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgcEventsCM.Index)
            {
                DoFilter();
            }
        }

        private void Form_Events_Resize(object sender, EventArgs e)
        {
            var hScrollbar = dgvFilter.Controls.OfType<HScrollBar>().First();
            if (hScrollbar.Visible)
            {
                if(Math.Abs(dgvFilter.Bottom - hScrollbar.Bottom) > 3)
                {
                    dgvFilter.PerformLayout();
                    return;
                }
            }
        }

        private void tsbOpenItem_Click(object sender, EventArgs e)
        {
            if (bsRows.DataSource == null || bsRows.Current == null) return;
            var dr = bsRows.GetCurrentItem<P_ITEMS_EVENTS>();
            var fm = MyMainForm.ShowForm(typeof(FormP_Items)) as FormP_Items;
            if (fm == null) return;
            fm.SelectItem(dr.ID);
        }

    }
}
