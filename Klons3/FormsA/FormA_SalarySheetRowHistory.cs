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
using KlonsLIB.MySourceGrid.GridRows;
using Klons3.ModelsA;

namespace KlonsA.Forms
{
    public partial class FormA_SalarySheetRowHistory : MyFormBaseF
    {
        public FormA_SalarySheetRowHistory()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            dgvRows.AutoGenerateColumns = false;
        }

        public int SelectedID = -1;

        private void Form_SalarySheetRowHistory_Load(object sender, EventArgs e)
        {

        }

        public static int Show(List<A_SALARY_SHEETS_R_HIST> table)
        {
            var form_hist = new FormA_SalarySheetRowHistory();
            var bl_table = new BindingList<A_SALARY_SHEETS_R_HIST>(table);
            form_hist.bsRows.SetDataSourceFromBindingList(bl_table);
            var rt = form_hist.ShowDialog(form_hist.MyMainForm);
            if (rt != DialogResult.OK) return -1;
            return form_hist.SelectedID;
        }

        private void TsbRestore_Click(object sender, EventArgs e)
        {
            if (bsRows.Current == null) return;
            var dr = bsRows.GetCurrentItem<A_SALARY_SHEETS_R_HIST>();
            if (dr == null) return;
            SelectedID = dr.IDH;
            DialogResult = DialogResult.OK;
        }

        private void DgvRows_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex > bsRows.Count) return;
            if (e.ColumnIndex == dgcSarAvPay.Index)
            {
                var dr = bsRows.GetItem<A_SALARY_SHEETS_R>(e.RowIndex);
                if (dr == null) return;
                decimal val =
                    dr.SALARY_AVPAY_FREE_DAYS +
                    dr.SALARY_AVPAY_WORK_DAYS +
                    dr.SALARY_AVPAY_WORK_DAYS_OVERTIME +
                    dr.SALARY_AVPAY_HOLIDAYS +
                    dr.SALARY_AVPAY_HOLIDAYS_OVERTIME;
                e.Value = val.ToString("# ##0.00;-# ##0.00;\"\"");
                e.FormattingApplied = true;
                return;
            }
            if (e.ColumnIndex == dgcSarPlus.Index)
            {
                var dr = bsRows.GetItem<A_SALARY_SHEETS_R>(e.RowIndex);
                if (dr == null) return;
                decimal val =
                    dr.PLUS_AUTHORS_FEES +
                    dr.PLUS_TAXED +
                    dr.PLUS_NOSAI +
                    dr.PLUS_NOTTAXED +
                    dr.PLUS_HI_NOTTAXED +
                    dr.PLUS_HI_TAXED +
                    dr.PLUS_LI_NOTTAXED +
                    dr.PLUS_LI_TAXED +
                    dr.PLUS_PF_NOTTAXED +
                    dr.PLUS_PF_TAXED;
                e.Value = val.ToString("# ##0.00;-# ##0.00;\"\"");
                e.FormattingApplied = true;
                return;
            }
        }
    }
}
