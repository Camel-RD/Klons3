﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Equin.ApplicationFramework;
using KlonsF.Classes;
using KlonsLIB.Data;
using KlonsLIB.Forms;
using KlonsLIB.Misc;
using Klons3.ModelsF;
using KlonsLIB.Components;

namespace KlonsF.Forms
{
    public partial class Form_OpsFilter : MyFormBaseF
    {
        public Form_OpsFilter()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            MakeMoveSeq();
            LoadColumnWidthsFromSettings();
            cbDocGr.SelectedIndex = -1;

            LoadParams();
            FillData();
            CheckColumns();

            myToolStrip.Visible = true;
        }

        BindingSource bsRows = new BindingSource();
        BindingListView<F_VW_OPS> blvRows = null;
        BindingList<F_VW_OPS> blRows = null;

        private void CheckColumns()
        {
            string CHCOL = MyData.Params.CHCOL;
            bool b1 = CHCOL[0] == '1';
            bool b2 = CHCOL[1] == '1';
            bool b3 = CHCOL[2] == '1';
            bool b4 = CHCOL[3] == '1';
            dgcAC12.Visible = b1;
            dgcAC22.Visible = b1;
            dgcAC13.Visible = b2;
            dgcAC23.Visible = b2;
            dgcAC14.Visible = b3;
            dgcAC24.Visible = b3;
            dgcAC15.Visible = b4;
            dgcAC25.Visible = b4;
        }


        private void LoadParams()
        {
            tbDate1.Text = MyData.Params.OSD;
            tbDate2.Text = MyData.Params.OED;
            cbClid.Text = MyData.Params.OCL;
            cbAC11.Text = MyData.Params.OAC11;
            cbAC12.Text = MyData.Params.OAC12;
            cbAC13.Text = MyData.Params.OAC13;
            cbAC14.Text = MyData.Params.OAC14;
            cbAC15.Text = MyData.Params.OAC15;
            cbAC21.Text = MyData.Params.OAC21;
            cbAC22.Text = MyData.Params.OAC22;
            cbAC23.Text = MyData.Params.OAC23;
            cbAC24.Text = MyData.Params.OAC24;
            cbAC25.Text = MyData.Params.OAC25;
            cbDocGr.Text = MyData.Params.ODOCGR;

            cmAndOr.Text = MyData.Params.OOR? "vai" : "un";

            rādītPilnuPersonasNosaukumuToolStripMenuItem.Checked = MyData.Settings.OpsShowFullPersonName;
            rādītPersonasReģistrācijasNumuruToolStripMenuItem.Checked = MyData.Settings.OpsShowPersonsRegNum;
            rādītPersonasPVNReģistrācijasNumuruToolStripMenuItem.Checked = MyData.Settings.OpsShowPersonsPVNRegNum;

            dgcClid.DataPropertyName = MyData.Settings.OpsShowFullPersonName ? "Name" : "Clid";
            dgcRegNr.Visible = MyData.Settings.OpsShowPersonsRegNum;
            dgcPVNRegNr.Visible = MyData.Settings.OpsShowPersonsPVNRegNum;
        }

        public override void SaveParams()
        {
            MyData.Params.OSD = tbDate1.Text;
            MyData.Params.OED = tbDate2.Text;
            MyData.Params.OCL = cbClid.Text;
            MyData.Params.OAC11 = cbAC11.Text;
            MyData.Params.OAC12 = cbAC12.Text;
            MyData.Params.OAC13 = cbAC13.Text;
            MyData.Params.OAC14 = cbAC14.Text;
            MyData.Params.OAC15 = cbAC15.Text;
            MyData.Params.OAC21 = cbAC21.Text;
            MyData.Params.OAC22 = cbAC22.Text;
            MyData.Params.OAC23 = cbAC23.Text;
            MyData.Params.OAC24 = cbAC24.Text;
            MyData.Params.OAC25 = cbAC25.Text;
            MyData.Params.ODOCGR = cbDocGr.Text;
            MyData.Params.OOR = cmAndOr.Text == "vai";
        }

        private void LoadColumnWidthsFromSettings()
        {
            string scw = MyData.Settings.ColumnWidths_Dops2;
            (int ver, int[] cw) = dgvOPS.ParseColumnWidths(scw);
            if (cw.Length > 0)
            {
                dgvOPS.SetColumnWidths(cw);
            }
        }

        private void SaveColumnWidthsToSettings()
        {
            MyData.Settings.ColumnWidths_Dops2 = dgvOPS.GetColumnWidths2(10.0f, 1);
        }

        private string ZN(string value)
        {
            string s = string.IsNullOrEmpty(value) ? null : value;
            if (s != null)
                s = s.Replace('*', '%');
            return s;
        }

        private void FillData()
        {
            DateTime? dt1, dt2;
            dt1 = Utils.StringToDate(tbDate1.Text);
            dt2 = Utils.StringToDate(tbDate2.Text);
            string sac11, sac12, sac13, sac14, sac15;
            string sac21, sac22, sac23, sac24, sac25;

            string sclid = ZN(cbClid.Text);
            string stext = ZN(tbText.Text);
            string sdocgr = ZN(cbDocGr.Text);

            sac11 = ZN(cbAC11.Text);
            sac12 = ZN(cbAC12.Text);
            sac13 = ZN(cbAC13.Text);
            sac14 = ZN(cbAC14.Text);
            sac15 = ZN(cbAC15.Text);

            sac21 = ZN(cbAC21.Text);
            sac22 = ZN(cbAC22.Text);
            sac23 = ZN(cbAC23.Text);
            sac24 = ZN(cbAC24.Text);
            sac25 = ZN(cbAC25.Text);

            bool b1 = (sac11 == null && sac12 == null && sac13 == null && sac14 == null && sac15 == null)
                      || (sac21 == null && sac22 == null && sac23 == null && sac24 == null && sac25 == null);

            List<F_VW_OPS> rows = null;
            if (cmAndOr.Text == "un" || b1)
            {

                rows = DataLoaderF.VW_OPS_FillByAndFilter(
                    dt1, dt2, stext, sclid,
                    sac11, sac12, sac13, sac14, sac15,
                    sac21, sac22, sac23, sac24, sac25, sdocgr);
            }
            else
            {
                rows = DataLoaderF.VW_OPS_FillByOrFilter(
                    dt1, dt2, stext, sclid,
                    sac11, sac12, sac13, sac14, sac15,
                    sac21, sac22, sac23, sac24, sac25, sdocgr);
            }

            bsRows.DataSource = null;
            if (blRows != null) blRows.Clear();
            blRows = new BindingList<F_VW_OPS>(rows);
            if (blvRows != null) blvRows.KillView();
            blvRows = new BindingListView<F_VW_OPS>(blRows);
            bsRows.DataSource = blvRows;
            dgvOPS.DataSource = bsRows;

            int k = rows.Count;
            decimal sum = 0.00M, amount = 0.00M;
            if (k > 0)
            {
                sum = rows.Sum(x => x.Summ);
                amount = (decimal)rows.Sum(x => x.QV ?? 0f);
            }
            tbSum.Text = sum.ToString("N2");
            tbAmount.Text = amount.ToString();
        }

        private void FillDocs()
        {
            if (!SaveData()) return;
            if (MyData.DbContextF.HasChanges())
            {
                MyMainForm.ShowWarning(
                    "Ir nesaglabāti dati. Lai veiktu datu pārlasi, izmaiņām ir " +
                    "jābūt sagalbātām, vai arī jāveic pilna datu atkārtota ielāde.");
                return;
            }

            var fdocs = MyMainForm.FindFormDocs();
            try
            {
                if (fdocs != null)
                {
                    fdocs.EnableDetailUpdate(false);
                    fdocs.DisconnectGrids();
                }
                FillDocsA();
            }
            finally
            {
                if (fdocs != null)
                {
                    fdocs.ConnectGrids();
                    fdocs.EnableDetailUpdate(true);
                }
            }
        }

        private void FillDocsA()
        {
            DateTime? dt1, dt2;
            dt1 = Utils.StringToDate(tbDate1.Text);
            dt2 = Utils.StringToDate(tbDate2.Text);
            string sac11, sac12, sac13, sac14, sac15;
            string sac21, sac22, sac23, sac24, sac25;

            string sclid = ZN(cbClid.Text);
            string stext = ZN(tbText.Text);
            string sdocgr = ZN(cbDocGr.Text);

            sac11 = ZN(cbAC11.Text);
            sac12 = ZN(cbAC12.Text);
            sac13 = ZN(cbAC13.Text);
            sac14 = ZN(cbAC14.Text);
            sac15 = ZN(cbAC15.Text);

            sac21 = ZN(cbAC21.Text);
            sac22 = ZN(cbAC22.Text);
            sac23 = ZN(cbAC23.Text);
            sac24 = ZN(cbAC24.Text);
            sac25 = ZN(cbAC25.Text);

            bool b1 = (sac11 == null && sac12 == null && sac13 == null && sac14 == null && sac15 == null)
                      || (sac21 == null && sac22 == null && sac23 == null && sac24 == null && sac25 == null);

            DataLoaderF.ClearDocsAndOps();

            if (cmAndOr.Text == "un" || b1)
            {
                DataLoaderF.OPSD_FillByAndFilter(
                    dt1, dt2, stext, sclid,
                    sac11, sac12, sac13, sac14, sac15,
                    sac21, sac22, sac23, sac24, sac25, sdocgr);

                DataLoaderF.OPS_FillByAndFilter(
                    dt1, dt2, stext, sclid,
                    sac11, sac12, sac13, sac14, sac15,
                    sac21, sac22, sac23, sac24, sac25, sdocgr);
            }
            else
            {
                DataLoaderF.OPSD_FillByOrFilter(
                    dt1, dt2, stext, sclid,
                    sac11, sac12, sac13, sac14, sac15,
                    sac21, sac22, sac23, sac24, sac25, sdocgr);

                DataLoaderF.OPS_FillByOrFilter(
                    dt1, dt2, stext, sclid,
                    sac11, sac12, sac13, sac14, sac15,
                    sac21, sac22, sac23, sac24, sac25, sdocgr);
            }
        }

        public override bool SaveData()
        {
            var ret = MyData.DbContextF.Save();
            return ret.IsSuccess();
        }

        private void DoReportDoks1()
        {
            DateTime? dt1, dt2;
            dt1 = Utils.StringToDate(tbDate1.Text);
            dt2 = Utils.StringToDate(tbDate2.Text);
            string sac11, sac12, sac13, sac14, sac15;
            string sac21, sac22, sac23, sac24, sac25;

            string sclid = ZN(cbClid.Text);
            string stext = ZN(tbText.Text);
            string sdocgr = ZN(cbDocGr.Text);

            sac11 = ZN(cbAC11.Text);
            sac12 = ZN(cbAC12.Text);
            sac13 = ZN(cbAC13.Text);
            sac14 = ZN(cbAC14.Text);
            sac15 = ZN(cbAC15.Text);

            sac21 = ZN(cbAC21.Text);
            sac22 = ZN(cbAC22.Text);
            sac23 = ZN(cbAC23.Text);
            sac24 = ZN(cbAC24.Text);
            sac25 = ZN(cbAC25.Text);

            string s = "";
            if (dt1.HasValue) s = Utils.DateToString(dt1.Value);
            s += " - ";
            if (dt2.HasValue) s += Utils.DateToString(dt2.Value);

            string paramstr =
                $"Datums:[{s}], persona:[{cbClid.Text}]," +
                $" dokumentu grupa:[{cbDocGr.Text}]" +
                $" debets:[{cbAC11.Text},{cbAC12.Text},{cbAC13.Text},{cbAC14.Text},{cbAC15.Text}] {cmAndOr.Text}" +
                $" kredīts:[{cbAC21.Text},{cbAC22.Text},{cbAC23.Text},{cbAC24.Text},{cbAC25.Text}]";

            bool b1 = (sac11 == null && sac12 == null && sac13 == null && sac14 == null && sac15 == null)
                      || (sac21 == null && sac22 == null && sac23 == null && sac24 == null && sac25 == null);

            string pandor = (cmAndOr.Text == "un" || b1) ? "AND" : "OR";

            var rows = DataLoaderF.ROPS1A_FillBy_doks_11(
                dt1, dt2, stext, sclid,
                sac11, sac12, sac13, sac14, sac15,
                sac21, sac22, sac23, sac24, sac25, sdocgr, pandor);

            ReportViewerData rd = new ReportViewerData();
            rd.FileName = "Report_DokSar_1";
            rd.Sources["DataSet1"] = rows;
            rd.AddReportParameters(
                new string[]
                {
                    "RSD", MyData.Params.RSD,
                    "RED", MyData.Params.RED,
                    "RACX", "",
                    "RACNM", paramstr,
                    "CompanyName", MyData.Params.CompNameX
                });

            MyMainForm.ShowReport(rd);
        }

        private void DoReportDoks2()
        {
            DateTime? dt1, dt2;
            dt1 = Utils.StringToDate(tbDate1.Text);
            dt2 = Utils.StringToDate(tbDate2.Text);
            string sac11, sac12, sac13, sac14, sac15;
            string sac21, sac22, sac23, sac24, sac25;

            string sclid = ZN(cbClid.Text);
            string stext = ZN(tbText.Text);
            string sdocgr = ZN(cbDocGr.Text);

            sac11 = ZN(cbAC11.Text);
            sac12 = ZN(cbAC12.Text);
            sac13 = ZN(cbAC13.Text);
            sac14 = ZN(cbAC14.Text);
            sac15 = ZN(cbAC15.Text);

            sac21 = ZN(cbAC21.Text);
            sac22 = ZN(cbAC22.Text);
            sac23 = ZN(cbAC23.Text);
            sac24 = ZN(cbAC24.Text);
            sac25 = ZN(cbAC25.Text);

            string s = "";
            if (dt1.HasValue) s = Utils.DateToString(dt1.Value);
            s += " - ";
            if (dt2.HasValue) s += Utils.DateToString(dt2.Value);

            string paramstr =
                $"Datums:[{s}], persona:[{cbClid.Text}]," +
                $" dokumentu grupa:[{sdocgr}]" +
                $" debets:[{cbAC11.Text},{cbAC12.Text},{cbAC13.Text},{cbAC14.Text},{cbAC15.Text}] {cmAndOr.Text}" +
                $" kredīts:[{cbAC21.Text},{cbAC22.Text},{cbAC23.Text},{cbAC24.Text},{cbAC25.Text}]";

            if (blRows == null) blRows = new BindingList<F_VW_OPS>();

            ReportViewerData rd = new ReportViewerData();
            rd.FileName = "Report_Ieraksti_1";
            rd.Sources["DataSet1"] = blRows;
            rd.AddReportParameters(
                new string[]
                {
                    "RSD", MyData.Params.RSD,
                    "RED", MyData.Params.RED,
                    "RACX", "",
                    "RACNM", paramstr,
                    "CompanyName", MyData.Params.CompNameX
                });

            MyMainForm.ShowReport(rd);
        }

        private void MakeMoveSeq()
        {
            SetControlsUpDownOrder(new Control[][]
            {
                new Control[] {tbDate1, tbDate2, cbClid, tbText, tbText, cbDocGr},
                new Control[] {cbAC11, cbAC12, cbAC13, cbAC14, cbAC15, cbAC15},
                new Control[] {cbAC21, cbAC22, cbAC23, cbAC24, cbAC25, cbAC25}
            });
        }



        private void FormOpsFilter_Load(object sender, EventArgs e)
        {
        }

        private void FormOpsFilter_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveParams();
            if (blvRows != null) blvRows.KillView();
        }

        private void cmAndOr_Click(object sender, EventArgs e)
        {
            cmAndOr.Text = cmAndOr.Text == "vai" ? "un" : "vai";
        }

        private void tsbFilter_Click(object sender, EventArgs e)
        {
            FillData();
            SaveParams();
        }
        private void tsbFilterDocs_Click(object sender, EventArgs e)
        {
            try
            {
                FillDocs();
            }
            catch (Exception ex)
            {
                Form_Error.ShowException(MyMainForm, ex);
            }
            MyMainForm.ShowFormDocs();
        }

        private void control_KeyDown(object sender, KeyEventArgs e)
        {
            Control control = sender as Control;
            
            OnNaviKey(sender, e);
            if (e.Handled) return;

            switch (e.KeyCode)
            {
                case Keys.F5:
                    if (sender is MyPickRowTextBox2 tb2)
                        if (DoOnF5(tb2))
                            e.Handled = false;
                    break;
                default:
                    e.Handled = false;
                    break;
            }
        }

        private bool DoOnF5(TextBox sender)
        {
            Action<string> act =
                value =>
                {
                    if (value != null)
                        sender.Text = value;
                };
            if (sender == cbClid)
            {
                MyMainForm.ShowFormPersonsDialog(act);
                return true;
            }

            if (sender == cbAC11 || sender == cbAC12 ||
                sender == cbAC21 || sender == cbAC22)
            {
                MyMainForm.ShowFormAcplanDialog(act);
                return true;
            }

            if (sender == cbAC13 || sender == cbAC23)
            {
                MyMainForm.ShowFormAcp3Dialog(act);
                return true;
            }

            if (sender == cbAC14 || sender == cbAC24)
            {
                MyMainForm.ShowFormAcp4Dialog(act);
                return true;
            }

            if (sender == cbAC15 || sender == cbAC25)
            {
                MyMainForm.ShowFormAcp5Dialog(act);
                return true;
            }

            return false;
        }

        private void cbClid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string s = cbClid.Text;
            
            MyMainForm.ShowFormPersonsDialog(
                value =>
                {
                    if (value != null)
                        cbClid.SelectedValue = value;
                });
             
        }

        private void cbAC_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!(sender is ComboBox)) return;
            MyMainForm.ShowFormAcplanDialog(
                value =>
                {
                    if (value != null)
                        (sender as ComboBox).Text = value;
                });
        }
        private void cbACp3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!(sender is ComboBox)) return;
            MyMainForm.ShowFormAcp3Dialog(
                value =>
                {
                    if (value != null)
                        (sender as ComboBox).Text = value;
                });
        }

        private void cbACp4_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!(sender is ComboBox)) return;
            MyMainForm.ShowFormAcp4Dialog(
                value =>
                {
                    if (value != null)
                        (sender as ComboBox).Text = value;
                });
        }
        private void cbACp5_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!(sender is ComboBox)) return;
            MyMainForm.ShowFormAcp5Dialog(
                value =>
                {
                    if (value != null)
                        (sender as ComboBox).Text = value;
                });
        }

        private void dgvOPS_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1) return;
            string s = dgvOPS[e.ColumnIndex, e.RowIndex]?.Value?.ToString();
            if (string.IsNullOrEmpty(s)) return;
            if (e.ColumnIndex == dgcDocTyp.Index)
            {
                e.ToolTipText = DataTasks.GetDocTypName(s);
                return;
            }
            if (e.ColumnIndex == dgcClid.Index)
            {
                e.ToolTipText = DataTasks.GetClName(s);
                return;
            }
            if (e.ColumnIndex == dgcAC11.Index ||
                e.ColumnIndex == dgcAC12.Index ||
                e.ColumnIndex == dgcAC21.Index ||
                e.ColumnIndex == dgcAC22.Index)
            {
                e.ToolTipText = DataTasks.GetAcName(s);
                return;
            }
            if (e.ColumnIndex == dgcAC13.Index ||
                e.ColumnIndex == dgcAC23.Index)
            {
                e.ToolTipText = DataTasks.GetAc3Name(s);
                return;
            }
            if (e.ColumnIndex == dgcAC14.Index ||
                e.ColumnIndex == dgcAC24.Index)
            {
                e.ToolTipText = DataTasks.GetAc4Name(s);
                return;
            }
            if (e.ColumnIndex == dgcAC15.Index ||
                e.ColumnIndex == dgcAC25.Index)
            {
                e.ToolTipText = DataTasks.GetAc5Name(s);
                return;
            }
        }

        private void dgvOPS_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            return;
            /*
            if (e.ColumnIndex == dgcClid.Index)
            {
                if (!MyData.Settings.OpsShowFullPersonName) return;
                string clid = e.Value as string;
                if (clid.IsNOE()) return;
                var dr_person = MyData.DataSetKlonsF.Persons.FindByClId(clid);
                if (dr_person == null) return;
                e.Value = dr_person.Name;
                e.FormattingApplied = true;
            }
            */
        }


        private void tsbDocs_Click(object sender, EventArgs e)
        {
            MyMainForm.ShowFormDocs();
            
        }

        private void dokumentuSarakstsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoReportDoks1();
        }
        private void ierakstuIzrakstsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoReportDoks2();
        }

        private void SearchDoc()
        {
            if (dgvOPS.CurrentRow == null) return;
            try
            {
                var rv = dgvOPS.CurrentRow.DataBoundItem as ObjectView<F_VW_OPS>;
                var row = rv.Object;
                int id = row.DOCID;
                if (MyData.DbContextF.F_OPSD.Find(id) == null)
                {
                    FillDocs();
                }
                var f = MyMainForm.ShowFormDocs();
                if (f == null) return;
                f.SearchId(id);
            }
            catch (ConstraintException)
            {
                return;
            }
        }

        private void tsbFindInDocs_Click(object sender, EventArgs e)
        {
            SearchDoc();
        }

        private void Form_OpsFilter_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveColumnWidthsToSettings();
        }

        private void tbDate1_Leave(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;
            var s = tb.Text;
            if (string.IsNullOrEmpty(s)) return;
            DateTime dt;
            if(!Utils.StringToDate(s, out dt))
            {
                tb.Text = "";
                return;
            }
            tb.Text = Utils.DateToString(dt);
        }

        private void rādītPilnuPersonasNosaukumuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool b = !rādītPilnuPersonasNosaukumuToolStripMenuItem.Checked;
            rādītPilnuPersonasNosaukumuToolStripMenuItem.Checked = b;
            MyData.Settings.OpsShowFullPersonName = b;
            int w = b ? 200 : 110;
            w = (int)((float)w * ScaleFactor.Width);
            dgcClid.DataPropertyName = b ? "Name" : "Clid";
            dgcClid.Width = w;
        }

        private void rādītPersonasReģistrācijasNumuruToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool b = !rādītPersonasReģistrācijasNumuruToolStripMenuItem.Checked;
            rādītPersonasReģistrācijasNumuruToolStripMenuItem.Checked = b;
            MyData.Settings.OpsShowPersonsRegNum = b;
            dgcRegNr.Visible = b;
        }

        private void rādītPersonasPVNReģistrācijasNumuruToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool b = !rādītPersonasPVNReģistrācijasNumuruToolStripMenuItem.Checked;
            rādītPersonasPVNReģistrācijasNumuruToolStripMenuItem.Checked = b;
            MyData.Settings.OpsShowPersonsPVNRegNum = b;
            dgcPVNRegNr.Visible = b;
        }

        private void CbXXX_ButtonClicked(object sender, EventArgs e)
        {
            string cur_value = ((Control)sender).Text;
            string new_value = null;
            if (sender == cbAC11 || sender == cbAC12 || sender == cbAC21 || sender == cbAC22)
            {
                new_value = Form_AcPlan.GetAcP1(cur_value);
            }
            else if (sender == cbAC13 || sender == cbAC23)
            {
                new_value = Form_AcP3.GetAcP3(cur_value);
            }
            else if (sender == cbAC14 || sender == cbAC24)
            {
                new_value = Form_AcP4.GetAcP4(cur_value);
            }
            else if (sender == cbAC15 || sender == cbAC25)
            {
                new_value = Form_AcPVN.GetAcPVN(cur_value);
            }
            else if (sender == cbClid)
            {
                new_value = Form_Persons.GetClId(cur_value);
            }
            if (new_value.IsNOE()) return;
            ((Control)sender).Text = new_value;
        }
    }
}

