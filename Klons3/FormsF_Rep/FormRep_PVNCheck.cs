using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Klons3.ModelsFRep;
using KlonsF.Classes;
using KlonsF.Forms;
using KlonsLIB.Misc;
using Microsoft.Reporting.WinForms;

namespace KlonsF.FormsReportParams
{
    public partial class FormRep_PVNCheck : MyFormBaseF
    {
        public FormRep_PVNCheck()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        private DateTime startDate = DateTime.MinValue;
        private DateTime endDate = DateTime.MinValue;
        private decimal likme = 21.0M;
        private decimal slieksnis = 0.05M;

        private void FormRepApgr1_Load(object sender, EventArgs e)
        {
            LoadParams();
            SetControlsUpDownOrder(new Control[][]
            {
                new Control[] {tbSD, tbED},
                new Control[] {tbLikme, tbLikme},
                new Control[] {tbSlieksnis, tbSlieksnis},
                new Control[] {cmDoIt, cmDoIt},
                new Control[] {cmDoIt2, cmDoIt2}
            });
        }


        private void LoadParams()
        {
            tbSD.Text = MyData.Params.RSD;
            tbED.Text = MyData.Params.RED;
            tbLikme.Text = MyData.Params.RpvnCHLik;
            tbSlieksnis.Text = MyData.Params.RpvnCHRG1;
            if (tbLikme.Text == "") tbLikme.Text = "21";
            if (tbSlieksnis.Text == "") tbSlieksnis.Text = "0.03";
        }

        public override void SaveParams()
        {
            MyData.Params.RSD = tbSD.Text;
            MyData.Params.RED = tbED.Text;
            MyData.Params.RpvnCHLik = tbLikme.Text;
            MyData.Params.RpvnCHRG1 = tbSlieksnis.Text;
        }

        private string Check()
        {
            if (tbSD.Text == "" || tbED.Text == "")
                return "Jāievada datums.";

            if (!Utils.StringToDate(tbSD.Text, out startDate) ||
                !Utils.StringToDate(tbED.Text, out endDate))
                return "Nekorekts datums.";
            if(tbLikme.Text == "" || !decimal.TryParse(tbLikme.Text, out likme))
                return "Norādīta nekorekta likme.";
            if (tbSlieksnis.Text == "" || !decimal.TryParse(tbSlieksnis.Text, out slieksnis))
                return "Norādīta nekorekta kļūdas robeža.";
            likme /= 100.0M;
            return "OK";
        }

        private void DoIt(int repid)
        {
            string rt = Check();
            if (rt != "OK")
            {
                MyMainForm.ShowWarning(rt);
                return;
            }
            SaveParams();

            List<F_ROPS1A> rows_rops1a = null;

            switch (repid)
            {
                case 1:
                    rows_rops1a = DataLoaderF.ROPS1A_GetBy_pvn_check_02(startDate, endDate, slieksnis, likme);
                    break;
                case 2:
                    rows_rops1a = DataLoaderF.ROPS1A_GetBy_pvn_check_01(startDate, endDate, slieksnis, likme);
                    break;
                default:
                    return;
            }
            var fm = MyMainForm.ShowForm(typeof(Form_PVN_Piel)) as Form_PVN_Piel;
            if (fm == null) return;
            fm.SetDataSource(rows_rops1a);
            fm.Text = "Dokumenti ar ķļudām";
        }

        private void cmDoIt_Click(object sender, EventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                DoIt(1);

            });
        }
        private void cmDoIt2_Click(object sender, EventArgs e)
        {
            MyData.ReportHelperF.CheckForErrors(() =>
            {
                DoIt(2);

            });
        }

        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            OnNaviKey(sender, e);
        }


    }
}
