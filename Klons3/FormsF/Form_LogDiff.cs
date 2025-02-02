﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Klons3.ModelsF;
using Klons3.ModelsFRep;
using KlonsF.Classes;
using KlonsF.FormsReportParams;
using KlonsLIB.Data;
using KlonsLIB.Forms;
using KlonsLIB.Misc;
using NPOI.OpenXmlFormats.Spreadsheet;
using ModelsF = Klons3.ModelsF;

namespace KlonsF.Forms
{
    public partial class Form_LogDiff : MyFormBaseF
    {
        public Form_LogDiff()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            LinkToData();
            dgvRows.AutoGenerateColumns = false;
            RedBackStyle = new DataGridViewCellStyle(dgcDtl.DefaultCellStyle);
            RedBackStyle.BackColor = Color.IndianRed;
            RedBackStyle.ForeColor = Color.White;
        }

        private void Form_LogDiff_Load(object sender, EventArgs e)
        {
        }

        DateTime Dt1, Dt2, Dt3;
        string Clid, AC1, AC2, AC3, AC4, AC5;

        DataGridViewCellStyle RedBackStyle;

        private void dgvRows_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if (e.ColumnIndex == dgcDtl.Index)
            {
                var row = dgvRows.Rows[e.RowIndex].DataBoundItem as F_SP_DIFF_12;
                if (row == null) return;
                DateTime dt;
                if (row.TP == 1)
                {
                    var dt1 = row.DOCDTLD ?? row.DOCDTL.Value;
                    var dt2 = row.ROWDTLD ?? row.ROWDTL.Value;
                    dt = dt1 < dt2 ? dt2 : dt1;
                }
                else
                {
                    dt = row.ROWODT.Value;
                }
                e.Value = dt.ToString("dd.MM.yyyy HH:mm");
                var cell = dgvRows[e.ColumnIndex, e.RowIndex];
                if (row.DOCDTLD is not null || row.ROWDTLD is not null)
                    cell.Style = RedBackStyle;
                e.FormattingApplied = true;
            }
        }

        private void LinkToData()
        {
            var ctx = MyData.DbContextF;

            cbClid.DataSource = new BindingSource(ctx.GetBindingListByType<F_PERSONS>(), null);
            cbAC11.DataSource = new BindingSource(ctx.GetBindingListByType<F_ACP21>(), null);
            cbAC12.DataSource = new BindingSource(ctx.GetBindingListByType<F_ACP21>(), null);
            cbAC13.DataSource = new BindingSource(ctx.GetBindingListByType<F_ACP23>(), null);
            cbAC14.DataSource = new BindingSource(ctx.GetBindingListByType<F_ACP24>(), null);
            cbAC15.DataSource = new BindingSource(ctx.GetBindingListByType<F_ACP25>(), null);

            (cbClid.DataSource as BindingSource).Sort = "CLID";
            (cbAC11.DataSource as BindingSource).Sort = "AC";
            (cbAC12.DataSource as BindingSource).Sort = "AC";
            (cbAC13.DataSource as BindingSource).Sort = "IDX";
            (cbAC14.DataSource as BindingSource).Sort = "IDX";
            (cbAC15.DataSource as BindingSource).Sort = "IDX";

            cbClid.SelectedIndex = -1;
            cbAC11.SelectedIndex = -1;
            cbAC12.SelectedIndex = -1;
            cbAC13.SelectedIndex = -1;
            cbAC14.SelectedIndex = -1;
            cbAC15.SelectedIndex = -1;
        }

        public string CheckParams()
        {
            if (tbDate1.Text.IsNOE() || tbDate2.Text.IsNOE() || tbDate3.Text.IsNOE())
                return "Jānorāda datumi.";
            Dt1 = Utils.StringToDate(tbDate1.Text).Value;
            Dt2 = Utils.StringToDate(tbDate2.Text).Value;
            Dt3 = Utils.StringToDate(tbDate3.Text).Value;
            if (Dt1 >= Dt2 || Dt2 >= Dt3)
                return "Norādīti nekorekti datumi.";
            Clid = cbClid.Text.Zn();
            AC1 = cbAC11.Text.Zn();
            AC2 = cbAC12.Text.Zn();
            AC3 = cbAC13.Text.Zn();
            AC4 = cbAC14.Text.Zn();
            AC5 = cbAC15.Text.Zn();
            return "OK";
        }

        public void GetData()
        {
            var table = DataLoaderF.F_SP_DIFF_12_GetBy_SP_F_DIFF_12(Dt1, Dt2, Dt3, Clid,
                AC1, AC2, AC3, AC4, AC5);
            bsRows.DataSource = table;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var rt = CheckParams();
            if(rt != "OK")
            {
                MyMainForm.ShowWarning(rt);
                return;
            }
            GetData();
            ActiveControl = dgvRows;
        }

        private void tsbLogDoc_Click(object sender, EventArgs e)
        {
            if (bsRows.DataSource == null || bsRows.Count == 0 || bsRows.Position == -1) return;
            var row = bsRows.Current as F_SP_DIFF_12;
            int docid = row.DOCID;
            var frm = MyMainForm.ShowForm(typeof(Form_LogDoc)) as Form_LogDoc;
            frm.GetData2(docid);
        }

    }

}
