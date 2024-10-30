﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KlonsM.Classes;
using KlonsLIB.Forms;
using KlonsLIB.Data;
using KlonsLIB.Misc;
using KlonsF.Classes;
using Klons3.ModelsM;

namespace KlonsM.FormsM
{
    public partial class FormM_Invoice : MyFormBaseF
    {
        public FormM_Invoice()
        {
            InitializeComponent();
            CheckMyFontAndColors();

        }

        public FormM_Invoice(M_DOCS dr_doc, int repid = 0)
        {
            InitializeComponent();
            CheckMyFontAndColors();
            Doc = dr_doc;
            ReportId = repid;
        }

        private void FormM_Invoice_Load(object sender, EventArgs e)
        {
            LoadParams();
            tbDate.Text = Utils.DateToString(DateTime.Today);
            cbTitle.SelectedIndex = 0;
            lbInvoiceForm.SelectedIndex = 0;
            SetControlsUpDownOrder([[tbDate], [tbSigner], [cbTitle], [btDoIt]]);
        }

        private M_DOCS Doc = null;
        private int ReportId = -1;


        private void LoadParams()
        {
            tbSigner.Text = MyData.Params.RekinaIzr;
        }

        public override void SaveParams()
        {
            MyData.Params.RekinaIzr = tbSigner.Text;
        }


        private string Check()
        {
            if (cbTitle.Text.IsNOE())
                return "Jānorāda dokumenta virstaksts";
            ReportId = lbInvoiceForm.SelectedIndex;
            return "OK";
        }

        private void DoIt()
        {
            string rt = Check();
            if (rt != "OK")
            {
                MyMainForm.ShowWarning(rt);
                return;
            }

            SaveParams();

            var dr_doc = Doc;
            if (dr_doc == null) return;
            var invoicedata = new InvoiceData();
            var err = invoicedata.ReadFrom(dr_doc);
            if (err.HasErrors)
            {
                FormM_ErrorList.ShowErrorList(this, err);
                return;
            }

            invoicedata.MainData.DocTitle = cbTitle.Text;
            invoicedata.MainData.Signer = tbSigner.Text;
            invoicedata.MainData.SigningDate = tbDate.Text;
            invoicedata.MainData.ShowCarrier = chShowCarrier.Checked;

            ReportViewerData rd = new ReportViewerData();

            switch (ReportId)
            {
                case 0:
                    rd.FileName = "ReportM_Rekins_PZ_1";
                    break;
                case 1:
                    rd.FileName = "ReportM_Rekins_PZ_2";
                    break;
                case 2:
                    rd.FileName = "ReportM_Rekins_1";
                    break;
                default:
                    return;
            }
            rd.Sources["MainData"] = invoicedata.MainData2;
            rd.Sources["RowData"] = invoicedata.RowData;
            rd.Sources["TotalsRows"] = invoicedata.TotalsData;

            MyMainForm.ShowReport(rd);

        }

        private void btDoIt_Click(object sender, EventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                DoIt();

            });
        }
    }
}
