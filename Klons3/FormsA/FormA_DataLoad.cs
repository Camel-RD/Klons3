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
using KlonsLIB.Misc;
using KlonsLIB.Forms;

namespace KlonsA.Forms
{
    public partial class FormA_DataLoad : MyFormBaseF
    {
        public FormA_DataLoad()
        {
            InitializeComponent();
            CheckMyFontAndColors();

            int yr = DateTime.Today.Year;
            cbYr1.Items.Clear();
            cbYr2.Items.Clear();
            for (int i = yr - 5; i <= yr + 1; i++)
            {
                cbYr1.Items.Add(i.ToString());
                cbYr2.Items.Add(i.ToString());
            }


            if (DataLoaderA.HasDataA() && DataLoaderA.DataLoaded)
            {
                cbYr1.Text = DataLoaderA.LoadedDT1.Year.ToString();
                cbMt1.Text = DataLoaderA.LoadedDT1.Month.ToString("00");
                cbYr2.Text = DataLoaderA.LoadedDT2.Year.ToString();
                cbMt2.Text = DataLoaderA.LoadedDT2.Month.ToString("00");
            }
            else
            {
                GetParams();
            }

            DataLoaderA.RefreshMinMax();

            if (DataLoaderA.paminyr == 0)
                lbSalaryPeriod.Text = "nav";
            else
                lbSalaryPeriod.Text = string.Format("{0}.{1:00} - {2}.{3:00}",
                    DataLoaderA.paminyr, DataLoaderA.paminmt, DataLoaderA.pamaxyr, DataLoaderA.pamaxmt);

            if (DataLoaderA.pdlminyr == 0)
                lbTimeTablePeriod.Text = "nav";
            else
                lbTimeTablePeriod.Text = string.Format("{0}.{1:00} - {2}.{3:00}",
                    DataLoaderA.pdlminyr, DataLoaderA.pdlminmt, DataLoaderA.pdlmaxyr, DataLoaderA.pdlmaxmt);
        }

        private void Form_DataLoad_Load(object sender, EventArgs e)
        {
        }

        public void GetParams()
        {
            if(MyData.Params.LoadYR1 == 0)
            {
                var dt1 = DateTime.Today.AddMonths(1);
                var dt2 = dt1.AddMonths(-13);
                cbYr1.Text = dt2.Year.ToString();
                cbMt1.Text = dt2.Month.ToString("00");
                cbYr2.Text = dt1.Year.ToString();
                cbMt2.Text = dt1.Month.ToString("00");
            }
            else
            {
                cbYr1.Text = MyData.Params.LoadYR1.ToString();
                cbMt1.Text = MyData.Params.LoadMT1.ToString("00");
                cbYr2.Text = MyData.Params.LoadYR2.ToString();
                cbMt2.Text = MyData.Params.LoadMT2.ToString("00");
            }
        }
        public void SetParams()
        {
            if (!(DataLoaderA.HasDataA() && DataLoaderA.DataLoaded)) return;
            MyData.Params.LoadYR1 = DataLoaderA.LoadedDT1.Year;
            MyData.Params.LoadMT1 = DataLoaderA.LoadedDT1.Month;
            MyData.Params.LoadYR2 = DataLoaderA.LoadedDT2.Year;
            MyData.Params.LoadMT2 = DataLoaderA.LoadedDT2.Month;
        }

        private void cbYr1_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbYr2.SelectedIndex = cbYr1.SelectedIndex;
        }

        private void cbMt1_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbMt2.SelectedIndex = cbMt1.SelectedIndex;
        }

        public string CheckParams(out int yr1, out int mt1, out int yr2, out int mt2)
        {
            yr1 = 0;
            mt1 = 0;
            yr2 = 0;
            mt2 = 0;
            if(string.IsNullOrEmpty(cbYr1.Text) || !int.TryParse(cbYr1.Text, out yr1) ||
                string.IsNullOrEmpty(cbMt1.Text) || !int.TryParse(cbMt1.Text, out mt1) ||
                string.IsNullOrEmpty(cbYr2.Text) || !int.TryParse(cbYr2.Text, out yr2) ||
                string.IsNullOrEmpty(cbMt2.Text) || !int.TryParse(cbMt2.Text, out mt2) ||
                mt1 < 1 || mt1 > 12 || mt2 < 1 || mt2 > 12 ||
                (yr1 * 12 + mt1 > yr2 * 12 + mt2))
            {
                return "Norādīts nekorekts laika periods.";
            }
            return "OK";
        }

        private void cmOK_Click(object sender, EventArgs e)
        {
            int yr1 = 0;
            int mt1 = 0;
            int yr2 = 0;
            int mt2 = 0;
            var ret = CheckParams(out yr1, out mt1, out yr2, out mt2);
            if(ret != "OK")
            {
                MyMainForm.ShowWarning(ret);
                return;
            }
            if (DataLoaderA.LoadPeriod(yr1, mt1, yr2, mt2))
            {
                SaveParams();
                this.Close();
            }
        }
    }
}
