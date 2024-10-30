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
using KlonsLIB.Forms;
using Microsoft.Reporting.WinForms;

namespace KlonsA.Forms
{
    public partial class FormA_AvPayCalc : MyFormBaseF
    {
        public FormA_AvPayCalc()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        private AvPayCalcInfo AvPayCalcInfo = null;

        private void Form_AvPayCalc_Load(object sender, EventArgs e)
        {
            lbTitle.Font = new Font(this.Font, lbTitle.Font.Style);
        }

        public static void Show(AvPayCalcInfo ap)
        {
            var fm = new FormA_AvPayCalc();
            fm.AvPayCalcInfo = ap;
            fm.dgvSar.AutoGenerateColumns = false;
            fm.dgvSar.DataSource = ap.ReportRows;
            fm.tbRateDay.Text = ap.RateDay.ToString("N4");
            fm.tbRateCalDay.Text = ap.RateCalendarDay.ToString("N4");
            fm.tbRateHour.Text = ap.RateHour.ToString("N4");
            fm.lbTitle.Text = $"{ap.FName} {ap.LName}, par {ap.Year}.{ap.Month:00}";
            if (ap.UsingMinRate)
                fm.lbRem.Text = "Izmantota minimālā stundas un dienas likme.";
            fm.ShowDialog(fm.MyMainForm);
        }

        private void DoIt()
        {
            ReportViewerData rd = new ReportViewerData();
            rd.FileName = "ReportA_VidIzp_1";
            rd.Sources["DataSet1"] = AvPayCalcInfo.ReportRows;
            rd.AddReportParameters(
                new string[]
                {
                    "CompanyName", MyData.Params.CompNameX,
                    "RPerson", $"{AvPayCalcInfo.FName} {AvPayCalcInfo.LName}",
                    "RPeriod", $"{AvPayCalcInfo.Year}.{AvPayCalcInfo.Month:00}",
                    "RRateHour", AvPayCalcInfo.RateHour.ToString("N2"),
                    "RRateDay", AvPayCalcInfo.RateDay.ToString("N2"),
                    "RRateCalDay", AvPayCalcInfo.RateCalendarDay.ToString("N2"),
                    "RRemark", AvPayCalcInfo.UsingMinRate? "Izmantotās minimālās likmes.":""
                });

            MyMainForm.ShowReport(rd);
        }
        private void cmReport_Click(object sender, EventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                DoIt();
            });
        }
    }
}
