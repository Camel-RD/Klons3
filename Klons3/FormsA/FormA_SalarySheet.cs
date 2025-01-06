using System;
using System.IO;
using System.Diagnostics;
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
using System.Threading.Tasks;
using Klons3.ModelsA;
using Equin.ApplicationFramework;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;

namespace KlonsA.Forms
{
    public partial class FormA_SalarySheet : MyFormBaseF
    {
        public FormA_SalarySheet()
        {
            //var sw = new Stopwatch();
            //sw.Start();

            //BindingListView<A_SALARY_SHEETS>.ShouldProvideViewGlobal += BindingListView_ShouldProvideViewGlobal;

            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                Form_Error.ShowException(e);
            }
            CheckMyFontAndColors();

            if (MyData.Params.HideTotalSheets)
                bsLapas.SetFilter<A_SALARY_SHEETS>(x => x.KIND == 0 && !x.ISFIRSTMT);
            else
                bsLapas.SetFilter<A_SALARY_SHEETS>(x => !x.ISFIRSTMT);

            tslPeriod.Text = DataLoaderA.GetPeriodStr();

            dgcPsRateType.DataSource = SomeDataDefsA.ProcOrEuro;
            dgcPsRateType.DisplayMember = "Val";
            dgcPsRateType.ValueMember = "Key";

            try
            {
                MakeGrid();
            }
            catch (Exception ex)
            {
                Form_Error.ShowException(ex);
            }

            tabControl1.SelectedIndex = 0;

            ShowBonusList(!MyData.Params.HideBonusList);
            ShowPositionTitleColumn(MyData.Params.SalarySheetShowPositionTitle);

            LoadColumnWidthsFromSettings();

            bsPapildsummasVeids.SetFilter<A_PLUSMINUS_TYPES>(x => x.TP3 == 1);
            
            //sw.Stop();
            //MessageBox.Show("" + sw.ElapsedMilliseconds);
        }

        public int ActiveList = 0;

        private void Form_SalarySheet_Load(object sender, EventArgs e)
        {
            SetupToolStrips();

            splitContainer2.SplitterDistance = splitContainer2.Height
                - (int)((float)80 * ScaleFactor.Height);
            //splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;

            IsLoading = false;
            OnSheetsCurrentChanged();
            OnSheetRowCurrentChanged();
            dgvLapa.Select();
        }

        private void LoadColumnWidthsFromSettings()
        {
            string scw = MyData.Settings.ColumnWidths_SalarySheet;
            (int ver, int[] cw) = dgvLapa.ParseColumnWidths(scw);
            if (ver == 1 && cw != null && cw.Length > 0)
                dgvLapa.SetColumnWidths(cw);

            scw = MyData.Settings.ColumnWidths_SalaryBonus;
            (ver, cw) = dgvPapildsummas.ParseColumnWidths(scw);
            if (ver == 1 && cw != null && cw.Length > 0)
                dgvPapildsummas.SetColumnWidths(cw);
        }

        private void SaveColumnWidthsToSettings()
        {
            MyData.Settings.ColumnWidths_SalarySheet = dgvLapa.GetColumnWidths2(10f, 1);
            MyData.Settings.ColumnWidths_SalaryBonus = dgvPapildsummas.GetColumnWidths2(10f, 1);
        }

        public override void SaveParams()
        {
            SaveColumnWidthsToSettings();
        }

        private void Form_SalarySheet_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void BsAlgasPapildsummas_ListItemPropertyChanged(object sender, MyItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(A_SALARY_PLUSMINUS.IDSV))
            {
                var dr = e.Item as A_SALARY_PLUSMINUS;
                if (dr.IDSV == null)
                {
                    if (dr.IDNO == null) return;
                    dr.IDNO = null;
                    return;
                }
                var bonustype = (EBonusType)dr.IDSV;
                var allowed = SomeDataDefsA.GetAllowedEBonusFrom(bonustype);
                var list = SomeDataDefsA.GetBonusFromItems(allowed);
                if (list == null || list.Length == 0)
                {
                    dr.IDNO = null;
                    return;
                }
                if (dr.XBonusFrom == EBonusFrom.None || !allowed.Contains(dr.XBonusFrom))
                {
                    dr.XBonusFrom = allowed[0];
                    return;
                }
            }
        }

        private void SetupToolStrips()
        {
            InsertInToolStrip(toolStrip1, cbLapas, 1);
            InsertInToolStrip(toolStrip2, cbAmati, -1);
        }

        private void MakeGrid()
        {
            sgrAprekins.MakeGrid();
            sgrAprekins.LinkGrid();

            sgrBonus.MakeGrid();
            grbIDSV.DataCell.Editor.DefaultValue = (int)1;
            sgrBonus.LinkGrid();

            ColapseGrid();
            //sgrAprekins.ArrangeLinkedControls();
        }

        private void ColapseGrid()
        {
            grPlanTitle.Expanded = false;
            grFactTitle.Expanded = false;
            grAvPayTimeTitle.Expanded = false;
            grSalaryTitle.Expanded = false;
            grSalaryAvPayTitle.Expanded = false;
            grVacationTitle.Expanded = false;
            grSickDaysTitle.Expanded = false;
            grVSAOITitle.Expanded = false;
            grPlusMinusTitle.Expanded = false;
            grIINExemptsTitle.Expanded = false;
            grReverseTitle.Expanded = false;
            grIINTitle.Expanded = false;
        }

        public void SelectSheet(A_SALARY_SHEETS dr_sheet)
        {
            for(int i = 0; i < bsLapas.Count; i++)
            {
                var dr = bsLapas.GetItem<A_SALARY_SHEETS>(i);
                if(dr == dr_sheet)
                {
                    bsLapas.Position = i;
                    return;
                }
            }
        }

        private void OnSheetsCurrentChanged()
        {
            if (bsLapas.Current == null) return;
            var dr = bsLapas.GetCurrentItem<A_SALARY_SHEETS>();
            if (dr.KIND == 0)
            {
                bsSarR.DataMember = "SalarySheetRowsView";
                dgcSarSnr.Visible = true;
            }
            else
            {
                bsSarR.DataMember = "SalarySheetTotalRowsView";
                dgcSarSnr.Visible = false;
            }
        }

        private void OnSheetRowCurrentChanged()
        {
            var table_sar_r = MyData.DbContextA.BL_A_SALARY_SHEETS_R;
            if (bsSarR.Current == null)
            {
                var blv1 = new BindingListView<A_SALARY_SHEETS_R>(table_sar_r);
                bsSarR2.DataSource = blv1;
                sgrAprekins.Visible = false;
                return;
            }
            sgrAprekins.Visible = true;
            var dr = bsSarR.GetCurrentItem<A_SALARY_SHEETS_R>();
            var blv = dr.RowSetTotalRow.RowSetRows.GetBindingListView();
            bsSarR2.DataSource = blv;

            if (bsSarR2.Count > 1)
            {
                tslAmati.Visible = true;
                cbAmati.Visible = true;
                for (int i = 0; i < bsSarR2.Count; i++)
                {
                    var dr2 = (bsSarR2[i] as ObjectView<A_SALARY_SHEETS_R>)?.Object;
                    if (dr2 == dr)
                    {
                        bsSarR2.Position = i;
                        break;
                    }
                }
            }
            else
            {
                tslAmati.Visible = false;
                cbAmati.Visible = false;
            }

            if (dr.Person == null) return;
            var blv_pos = new BindingListView<A_POSITIONS>(dr.Person.Positions);
            bsAmati.DataSource = blv_pos;
            
            //dgcPsIDA.DataSource = dv_am;
            //dgcPsIDA.ValueMember = "ID";
            //dgcPsIDA.DisplayMember = "TITLE";

            if (grbIDA.ComboBoxEditor != null)
            {
                grbIDA.ComboBoxEditor.Control.SetListBinding(blv_pos, "TITLE", "ID", grbIDA.ComboBoxEditor.Control.ColumnNames);
            }
        }

        private void OnSheetRowCurrentChanged2()
        {
            var table_sar_r = MyData.DbContextA.BL_A_SALARY_SHEETS_R;
            if (bsSarR.Current == null)
            {
                var blv1 = new BindingListView<A_SALARY_SHEETS_R>(table_sar_r);
                bsSarR2.DataSource = blv1;
                sgrAprekins.Visible = false;
                return;
            }
            sgrAprekins.Visible = true;
            var dr = bsSarR.GetCurrentItem<A_SALARY_SHEETS_R>();
            var blv = new BindingListView<A_SALARY_SHEETS_R>(table_sar_r);
            var drt = dr.RowSetTotalRow;
            var filter = BindingListView<A_SALARY_SHEETS_R>.CreateItemFilter(x => x.RowSetTotalRow == drt);
            blv.Filter = filter;
            bsSarR2.DataSource = blv;

            if (bsSarR2.Count > 1)
            {
                tslAmati.Visible = true;
                cbAmati.Visible = true;
                for (int i = 0; i < bsSarR2.Count; i++)
                {
                    var dr2 = (bsSarR2[i] as ObjectView<A_SALARY_SHEETS_R>)?.Object;
                    if (dr2 == dr)
                    {
                        bsSarR2.Position = i;
                        break;
                    }
                }
            }
            else
            {
                tslAmati.Visible = false;
                cbAmati.Visible = false;
            }

            if (dr.Person == null) return;
            var blv_pos = new BindingListView<A_POSITIONS>(dr.Person.Positions);
            bsAmati.DataSource = blv_pos;

            //dgcPsIDA.DataSource = dv_am;
            //dgcPsIDA.ValueMember = "ID";
            //dgcPsIDA.DisplayMember = "TITLE";

            if (grbIDA.ComboBoxEditor != null)
            {
                grbIDA.ComboBoxEditor.Control.SetListBinding(blv_pos, "TITLE", "ID", grbIDA.ComboBoxEditor.Control.ColumnNames);
            }
        }

        private void bsLapas_CurrentChanged(object sender, EventArgs e)
        {
            if (IsLoading) return;
            OnSheetsCurrentChanged();
        }

        private void bsSarR_CurrentChanged(object sender, EventArgs e)
        {
            if (IsLoading) return;
            OnSheetRowCurrentChanged();
        }

        private void bsAlgasPapildsummas_CurrentChanged(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckEnableSGR();
        }

        private void CheckEnableSGR()
        {
            bool b1 = bsSarR.Count > 0 && bsSarR.Current != null;
            bool b2 = b1 && bsAlgasPapildsummas.Current != null;
            SetControlEnabled(sgrAprekins, b1);
            SetControlEnabled(sgrBonus, b2);
        }

        private void cbLapas_Format(object sender, ListControlConvertEventArgs e)
        {
            var dr = (e.ListItem as ObjectView<A_SALARY_SHEETS>)?.Object;
            if (dr == null) return;
            var state = MyData.DbContextA.Entry(dr).State;
            if (state == EntityState.Detached || state == EntityState.Deleted)
                e.Value = "?";
            else
                e.Value = $"{dr.YR}.{dr.MT:00} {dr.SNR}. {dr.DESCR.Nz()}";
        }

        private void dgvLapa_Enter(object sender, EventArgs e)
        {
            ActiveList = 0;
            tslLabel.Text = "Aprēķins:";
            if (tabControl1.SelectedIndex != 0)
                tabControl1.SelectedIndex = 0;

            if (!IsLoading && CheckSave()) SaveData();
        }

        private void dgvPapildsummas_Enter(object sender, EventArgs e)
        {
            dgvPapildsummas.Focus();
            ActiveList = 1;
            tslLabel.Text = "Papildsummas:";
            if (tabControl1.SelectedIndex != 1)
                tabControl1.SelectedIndex = 1;
            CheckEnableSGR();
        }

        private void sgrAprekins_Enter(object sender, EventArgs e)
        {
            ActiveList = 0;
            tslLabel.Text = "Aprēķins:";
        }

        private void splitContainer2_Panel1_Enter(object sender, EventArgs e)
        {
            if(tabControl1.SelectedIndex != 0)
                tabControl1.SelectedIndex = 0;
        }

        private void splitContainer2_Panel2_Enter(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex != 1)
                tabControl1.SelectedIndex = 1;
        }

        private int IsSaveButtonRed = -1;
        private void SetSaveButton(bool red)
        {
            int isred = red ? 1 : 0;
            if (isred == IsSaveButtonRed) return;
            IsSaveButtonRed = isred;
            Image im = null;
            if (red)
                im = global::KlonsF.Properties.Resources.Save2;
            else
                im = global::KlonsF.Properties.Resources.Save1;
            tsbSave.Image = im;
        }

        public override bool SaveData()
        {
            if (!dgvLapa.EndEditX()) return false;
            if (!dgvPapildsummas.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = MyData.DbContextA.Save();
            CheckSave();
            return rt.IsSuccess();
        }

        private bool CheckSave()
        {
            bool ret = MyData.DbContextA.HasChanges<A_SALARY_SHEETS>() ||
                MyData.DbContextA.HasChanges<A_SALARY_SHEETS_R>() ||
                MyData.DbContextA.HasChanges<A_SALARY_PLUSMINUS>();
            SetSaveButton(ret);
            return ret;
        }

        private void dgvLapa_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            // its done in dgvLapa_Enter
            //if (CheckSave()) SaveData();
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            if (CheckSave()) SaveData();
            CheckSave();
        }

        private void bsSarR_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            if (e.ListChangedType == ListChangedType.Reset) return;
            CheckSave();
        }

        private void bsAlgasPapildsummas_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            if (e.ListChangedType == ListChangedType.Reset) return;
            CheckSave();
        }

        public void AddNew()
        {
            if (bsLapas.Current == null) return;
            var dr_lapa = bsLapas.GetCurrentItem<A_SALARY_SHEETS>();
            int ids = dr_lapa.ID;
            int snr = bsSarR.Count + 1;
            A_PERSONS dr_person = null;
            A_POSITIONS dr_postion = null;

            if(dr_lapa.XKind == ESalarySheetKind.Total)
            {
                MyMainForm.ShowWarning("Koplapa nav rediģējama.");
                return;
            }

            var f = new FormA_SalarySheetNewRow();
            var ret = f.Execute(dr_lapa, ref snr, out dr_person, out dr_postion);
            if (!ret) return;

            if (DataTasksA.HaveSalaryRRow(dr_lapa.DT1.Value, dr_lapa.DT2.Value, dr_person, dr_postion))
            {
                MyMainForm.ShowWarning("Šim darbiniekam-amatam algas aprēķins jau ir izveidots.");
                return;
            }

            var err = DataTasksA.MakeSalarySheetRow(dr_lapa, dr_person, dr_postion, snr);
            if (err != null && err.Count > 0)
            {
                Form_ErrorList.ShowErrorList(MyMainForm, err);
            }

            SaveData();
        }

        public void DeleteAlgasR()
        {
            if (bsSarR.Current == null) return;
            var dr_lapas_r = bsSarR.GetCurrentItem<A_SALARY_SHEETS_R>();
            if (dr_lapas_r.XType == ESalarySheetRowType.OneOfMany)
            {
                var rt = MyMessageBox.Show(
                    "Šim darbiniekam ir algas aprēķini vairākiem amatiem." +
                    "Dzēšot šo aprēķinu, pārējie var mainīties." +
                    "\nVai dzēst šo aprēķinu?",
                    "",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);
                if (rt != DialogResult.OK)
                    return;
            }
            DataTasksA.DeleteSalarySheetRow(dr_lapas_r);
            SaveData();
        }

        private void tsbNew_Click(object sender, EventArgs e)
        {
            if (!Validate()) return;
            if (bsLapas.Current == null) return;
            var dr_lapa = bsLapas.GetCurrentItem<A_SALARY_SHEETS>();
            if (dr_lapa.XKind == ESalarySheetKind.Total)
            {
                MyMainForm.ShowWarning("Koplapa nav rediģējama.");
                return;
            }

            if (ActiveList == 0)
            {
                AddNew();
                return;
            }
            if (ActiveList == 1)
            {
                AddNewBonusRow();
                return;
            }
        }

        public void AddNewBonusRow()
        {
            var dr = bsSarR.GetCurrentItem<A_SALARY_SHEETS_R>();
            var table_ps = MyData.DbContextA.BL_A_SALARY_PLUSMINUS;
            var new_dr = table_ps.NewItem();
            new_dr.Person = dr.Person;
            new_dr.SalarySheetRow = dr;
            if (dr.Person != null)
                new_dr.Person = dr.Person;
            table_ps.Add(new_dr);
            bsAlgasPapildsummas.Position = bsAlgasPapildsummas.Count - 1;
        }

        private void tsbDelete_Click(object sender, EventArgs e)
        {
            if (!Validate()) return;
            if (bsLapas.Current == null) return;
            var dr_lapa = bsLapas.GetCurrentItem<A_SALARY_SHEETS>();
            if (dr_lapa.XKind == ESalarySheetKind.Total)
            {
                MyMainForm.ShowWarning("Koplapa nav rediģējama.");
                return;
            }
            if (ActiveList == 1)
            {
                bsAlgasPapildsummas.RemoveCurrent();
                return;
            }
            DeleteAlgasR();
        }

        private void dgvPapildsummas_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == dgcPsIDNO.Index)
            {
                if (e.RowIndex == dgvPapildsummas.NewRowIndex)
                {
                    e.Cancel = true;
                    return;
                }
                var cell = dgvPapildsummas[e.ColumnIndex, e.RowIndex] as DataGridViewComboBoxCell;
                var dr = dgvPapildsummas.GetObjectViewItem<A_SALARY_PLUSMINUS>(e.RowIndex);

                var allowed = SomeDataDefsA.GetAllowedEBonusFrom(dr.XBonusType);
                if (allowed.Length == 0)
                {
                    e.Cancel = true;
                    return;
                }
                var list = SomeDataDefsA.GetBonusFromItems(allowed);
                cell.DataSource = list;
                cell.DisplayMember = "Val";
                cell.ValueMember = "Key";
            }
        }

        private void dgvPapildsummas_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgcPsIDNO.Index)
            {
                var cell = dgvPapildsummas[e.ColumnIndex, e.RowIndex] as DataGridViewComboBoxCell;
                cell.DataSource = bsPapildsummaNo;
                cell.DisplayMember = "DESCR";
                cell.ValueMember = "ID";
            }
        }

        private void sgrBonus_EditStarting(object sender, CancelEventArgs e)
        {
            var gridrow = sender as MyGridRowComboBoxB;
            if (gridrow != grbIDNO || bsAlgasPapildsummas.Count == 0 || bsAlgasPapildsummas.Current == null) return;

            var dr = bsAlgasPapildsummas.GetCurrentItem<A_SALARY_PLUSMINUS>();

            var allowed = SomeDataDefsA.GetAllowedEBonusFrom(dr.XBonusType);
            if (allowed.Length == 0)
            {
                e.Cancel = true;
                return;
            }
            var list = SomeDataDefsA.GetBonusFromItems(allowed);
            var c = gridrow.ComboBoxEditor.Control;
            c.SetListBinding(list, "Val", "Key", new[] { "Key", "Val" });
        }

        private void sgrBonus_EditEnded(object sender, EventArgs e)
        {
            var gridrow = sender as MyGridRowComboBoxB;
            if (gridrow != grbIDNO) return;
            var c = gridrow.ComboBoxEditor.Control;
            c.SetListBinding(bsPapildsummaNo, "DESCR", "ID", new[] { "ID", "DESCR" });
        }

        private void dgvPapildsummas_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            var dr = (bsSarR.Current as ObjectView<A_SALARY_SHEETS_R>)?.Object;
            if (dr == null) return;
            e.Row.Cells[dgcPsIDP.Index].Value = dr.IDP;
        }

        private int titlePlanDays = int.MinValue;
        private float titlePlanHours = float.MinValue;
        private int titleFactDays = int.MinValue;
        private float titleFactHours = float.MinValue;
        private int titleAvPayDays = int.MinValue;
        private float titleAvPayHours = float.MinValue;
        private decimal titleSalary = decimal.MinValue;
        private decimal titleSalaryAvPay = decimal.MinValue;
        private decimal titleVacation = decimal.MinValue;
        private decimal titleSickDays = decimal.MinValue;
        private decimal titleVSAOI = decimal.MinValue;
        private decimal titlePlusMinus = decimal.MinValue;
        private decimal titleIINExempts = decimal.MinValue;
        private decimal titleReverseSN = decimal.MinValue;
        private decimal titleReverseIIN = decimal.MinValue;
        private decimal titleIIN = decimal.MinValue;

        private void UpdateTitleData()
        {
            if (titlePlanDays != salaryData1._PLAN_DAYS ||
                titlePlanHours != salaryData1._PLAN_HOURS)
            {
                titlePlanDays = salaryData1._PLAN_DAYS;
                titlePlanHours = salaryData1._PLAN_HOURS;
                grPlanTitle.ValueStr = $"{titlePlanDays}; {titlePlanHours}";
            }

            if (titleFactDays != salaryData1._FACT_DAYS ||
                titleFactHours != salaryData1._FACT_HOURS)
            {
                titleFactDays = salaryData1._FACT_DAYS;
                titleFactHours = salaryData1._FACT_HOURS;
                grFactTitle.ValueStr = $"{titleFactDays}; {titleFactHours}";
            }

            int newtitleAvPayDays = salaryData1._FACT_AVPAY_FREE_DAYS + salaryData1._FACT_AVPAY_WORK_DAYS;
            float newtitleAvPayHours = salaryData1._FACT_AVPAY_FREE_HOURS + salaryData1._FACT_AVPAY_HOURS;
            if (titleAvPayDays != newtitleAvPayDays ||
                titleAvPayHours != newtitleAvPayHours)
            {
                titleAvPayDays = newtitleAvPayDays;
                titleAvPayHours = newtitleAvPayHours;
                grAvPayTimeTitle.ValueStr = $"{titleAvPayDays}; {titleAvPayHours}";
            }

            if (titleSalary != salaryData1._SALARY)
            {
                titleSalary = salaryData1._SALARY;
                grSalaryTitle.ValueStr = $"{titleSalary:N2}";
            }

            var newtitleSalaryAvPay = salaryData1._SALARY_AVPAY_FREE_DAYS +
                salaryData1._SALARY_AVPAY_WORK_DAYS;
            if (titleSalaryAvPay != newtitleSalaryAvPay)
            {
                titleSalaryAvPay = newtitleSalaryAvPay;
                grSalaryAvPayTitle.ValueStr = $"{titleSalaryAvPay:N2}";
            }
            
            if (titleVacation != salaryData1._VACATION_PAY_CURRENT)
            {
                titleVacation = salaryData1._VACATION_PAY_CURRENT;
                grVacationTitle.ValueStr = titleVacation.ToString("N2");
            }
            
            if (titleSickDays != salaryData1._SICKDAYS_PAY)
            {
                titleSickDays = salaryData1._SICKDAYS_PAY;
                grSickDaysTitle.ValueStr = titleSickDays.ToString("N2");
            }
            
            if (titleVSAOI != salaryData1._DNSN_AMOUNT)
            {
                titleVSAOI = salaryData1._DNSN_AMOUNT;
                grVSAOITitle.ValueStr = titleVSAOI.ToString("N2");
            }

            var newtitleIINExempts = salaryData1._IIN_EXEMPT_DEPENDANTS +
                salaryData1._IIN_EXEMPT_EXPENSES +
                salaryData1._IIN_EXEMPT_INVALIDITY +
                salaryData1._IIN_EXEMPT_NATIONAL_MOVEMENT +
                salaryData1._IIN_EXEMPT_RETALIATION +
                salaryData1._IIN_EXEMPT_UNTAXED_MINIMUM;
            if (titleIINExempts != newtitleIINExempts)
            {
                titleIINExempts = newtitleIINExempts;
                grIINExemptsTitle.ValueStr = titleIINExempts.ToString("N2");
            }

            if (titleIIN != salaryData1._IIN_AMOUNT)
            {
                titleIIN = salaryData1._IIN_AMOUNT;
                grIINTitle.ValueStr = titleIIN.ToString("N2");
            }

            if(titleReverseSN != (salaryData1._DNSN_AMOUNT_REVERSE + salaryData1._DDSN_AMOUNT_REVERSE) || 
                titleReverseIIN != salaryData1._IIN_AMOUNT_REVERSE)
            {
                titleReverseSN = salaryData1._DNSN_AMOUNT_REVERSE + salaryData1._DDSN_AMOUNT_REVERSE;
                titleReverseIIN = salaryData1._IIN_AMOUNT_REVERSE;
                grReverseTitle.ValueStr = $"{titleReverseSN:N0}; {titleReverseIIN:N0}";
            }

            var newtitlePlusMinus = salaryData1._PLUS_AUTHORS_FEES +
                salaryData1._PLUS_HI_NOTTAXED +
                salaryData1._PLUS_HI_TAXED +
                salaryData1._PLUS_LI_NOTTAXED +
                salaryData1._PLUS_LI_TAXED +
                salaryData1._PLUS_NOSAI +
                salaryData1._PLUS_NOTTAXED +
                salaryData1._PLUS_NOT_PAID +
                salaryData1._PLUS_PF_NOTTAXED +
                salaryData1._PLUS_PF_TAXED +
                salaryData1._PLUS_TAXED -
                salaryData1._MINUS_BEFORE_IIN;

            if (titlePlusMinus != newtitlePlusMinus)
            {
                titlePlusMinus = newtitlePlusMinus;
                grPlusMinusTitle.ValueStr = titlePlusMinus.ToString("N2");
            }
        }

        private void sgrAprekins_ValueChanged(object sender, EventArgs e)
        {
            UpdateTitleData();
        }

        private void dgvLapa_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex > bsSarR.Count) return;
            if (e.ColumnIndex == dgcSarWorkAvPay.Index)
            {
                var dr = bsSarR.GetItem<A_SALARY_SHEETS_R>(e.RowIndex);
                if (dr == null) return;
                decimal val = 
                    dr.SALARY_AVPAY_WORK_DAYS +
                    dr.SALARY_AVPAY_WORK_DAYS_OVERTIME +
                    dr.SALARY_AVPAY_HOLIDAYS +
                    dr.SALARY_AVPAY_HOLIDAYS_OVERTIME;
                e.Value = val.ToString("# ##0.00;-# ##0.00;\"\"");
                e.FormattingApplied = true;
                return;
            }
            if (e.ColumnIndex == dgcSarPlusTotal.Index)
            {
                var dr = bsSarR.GetItem<A_SALARY_SHEETS_R>(e.RowIndex);
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
            if (e.ColumnIndex == dgcSarsFactHours.Index)
            {
                var dr = bsSarR.GetItem<A_SALARY_SHEETS_R>(e.RowIndex);
                if (dr == null) return;
                float val =
                    dr.FACT_WORK_HOURS +
                    dr.FACT_WORK_HOURS_NIGHT +
                    dr.FACT_WORK_HOURS_OVERTIME;
                e.Value = val.ToString();
                e.FormattingApplied = true;
                return;
            }
        }


        private void SaveHistory()
        {
            var table = MyData.DbContextA.BL_A_SALARY_SHEETS_R;
            var changed_entries = MyData.DbContextA.GetChangedEntries<A_SALARY_SHEETS_R>()
                .Where(x => x.State != EntityState.Added)
                .ToList();
            if (changed_entries.Count == 0) return;

            var dt = DateTime.Now;
            var deleted_entries = changed_entries.Where(x => x.State == EntityState.Deleted).ToList();
            foreach (var entry in deleted_entries)
            {
                var dr = entry.Entity;
                DataLoaderA.SP_A_MAKE_SALARY_SHEET_R_HIST(dr.ID, dt, MyData.SalaryCalcHistoryInterval);
            }
            var modified_entries = changed_entries.Where(x => x.State == EntityState.Modified).ToList();
            var idsx_saved = new HashSet<int>();
            foreach (var entry in modified_entries)
            {
                var dr = entry.Entity;
                var old_dt = dr.DT_EDITED;
                dr.DT_EDITED = dt;
                if (old_dt == null) continue;
                if (dt > old_dt.Value &&
                    dt < old_dt.Value.AddDays(MyData.SalaryCalcHistoryInterval)) continue;
                if(dr.XType == ESalarySheetRowType.OnlyOne)
                {
                    DataLoaderA.SP_A_MAKE_SALARY_SHEET_R_HIST(dr.ID, dt, MyData.SalaryCalcHistoryInterval);
                }
                else
                {
                    if (idsx_saved.Contains(dr.IDSX.Value)) continue;
                    DataLoaderA.SP_A_MAKE_SALARY_SHEET_R_HIST_2(dr.IDSX.Value, dt, MyData.SalaryCalcHistoryInterval);
                    idsx_saved.Add(dr.IDSX.Value);
                }
            }
        }

        private bool ConfirmRecalc()
        {
            var rt = MyMessageBox.Show(
                "Veicot pārrēķinu pašreizējie aprēķina dati var nebūt atjaunojami.\n" +
                "Vai tiešām vēlaties veikt pārrēķinu?", "",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            return rt == DialogResult.Yes;
        }

        private void pārrēķinātDarbiniekamToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (bsSarR.Current == null) return;
            var dr_lapas_r = bsSarR.GetCurrentItem<A_SALARY_SHEETS_R>();
            if (dr_lapas_r == null) return;
            if (dr_lapas_r.DT_EDITED != null && 
                dr_lapas_r.DT_EDITED.Value.AddDays(MyData.SalaryCalcHistoryInterval) < DateTime.Now)
            {
                if (!ConfirmRecalc()) return;
            }
            var err = DataTasksA.CalcSalarySheetRow(dr_lapas_r);
            if(err.HasErrors)
                Form_ErrorList.ShowErrorList(MyMainForm, err);
            SaveHistory();
            SaveData();
        }

        private bool RecalcPossible()
        {
            foreach(var ov in bsSarR)
            {
                var dr = ((ObjectView<A_SALARY_SHEETS_R>)ov).Object;
                if (dr.DT_EDITED != null &&
                    dr.DT_EDITED.Value.AddDays(MyData.SalaryCalcHistoryInterval) < DateTime.Now)
                {
                    return true;
                }
            }
            return false;
        }

        private void pārrēķinātVisiemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bsSarR.Current == null) return;
            var dr_lapas_r = bsSarR.GetCurrentItem<A_SALARY_SHEETS_R>();
            if (dr_lapas_r == null) return;
            if (RecalcPossible())
            {
                if (!ConfirmRecalc()) return;
            }
            var err = DataTasksA.CalcSalarySheet(dr_lapas_r.SalarySheet);
            if (err.HasErrors)
                Form_ErrorList.ShowErrorList(MyMainForm, err);
            SaveHistory();
            SaveData();
        }

        public bool RestoreRow(A_SALARY_SHEETS_R row_cur, 
            A_SALARY_SHEETS_R_HIST row_hist)
        {
            var row_backup = new A_SALARY_SHEETS_R_HIST();
            try
            {
                MyData.DbContextA.CopyEntry(row_cur, row_backup, false);
                MyData.DbContextA.CopyEntry(row_hist, row_cur, false);
            }
            catch (Exception )
            {
                MyData.DbContextA.CopyEntry(row_backup, row_cur, false);
                return false;
            }
            return true;
        }

        private IEnumerable<decimal> GetEnumetatedHistRowData(A_SALARY_SHEETS_R_HIST row)
        {
            yield return row.SALARY;
            yield return row.SALARY_AVPAY_FREE_DAYS;
            yield return row.SALARY_AVPAY_HOLIDAYS;
            yield return row.SALARY_AVPAY_HOLIDAYS_OVERTIME;
            yield return row.SALARY_AVPAY_WORK_DAYS;
            yield return row.SALARY_AVPAY_WORK_DAYS_OVERTIME;
            yield return row.SALARY_DAY;
            yield return row.SALARY_NIGHT;
            yield return row.SALARY_OVERTIME;
            yield return row.SALARY_PIECEWORK;
            yield return row.SICKDAYS_PAY;
            yield return row.VACATION_PAY_CURRENT;

            yield return row.PLUS_AUTHORS_FEES;
            yield return row.PLUS_HI_NOTTAXED;
            yield return row.PLUS_HI_TAXED;
            yield return row.PLUS_LI_NOTTAXED;
            yield return row.PLUS_LI_TAXED;
            yield return row.PLUS_NOSAI;
            yield return row.PLUS_NOTTAXED;
            yield return row.PLUS_NOT_PAID;
            yield return row.PLUS_NP_NOSAI;
            yield return row.PLUS_NP_NOTTAXED;
            yield return row.PLUS_NP_TAXED;
            yield return row.PLUS_PF_NOTTAXED;
            yield return row.PLUS_PF_TAXED;
            yield return row.PLUS_TAXED;

            yield return row.TOTAL_BEFORE_TAXES;
            yield return row.AMOUNT_BEFORE_SN;
            yield return row.AMOUNT_BEFORE_IIN;

            yield return row.DNSN_AMOUNT;
            yield return row.DDSN_AMOUNT;

            yield return row.IIN_AMOUNT;
            yield return row.IIN_EXEMPT_2;
            yield return row.IIN_EXEMPT_DEPENDANTS;
            yield return row.IIN_EXEMPT_UNTAXED_MINIMUM;

            yield return row.ADVANCE;
            yield return row.PAY;
            yield return row.PAYT;
        }

        private bool TestHistSum(A_SALARY_SHEETS_R_HIST row_total,
            A_SALARY_SHEETS_R_HIST[] rows_linked)
        {
            var enumeratort = GetEnumetatedHistRowData(row_total);
            var enumerators = rows_linked
                .Select(d => GetEnumetatedHistRowData(d).GetEnumerator())
                .ToArray();
            foreach(decimal dt in enumeratort)
            {
                decimal sum = 0.0M;
                foreach (var enumeratorx in enumerators)
                {
                    if (!enumeratorx.MoveNext()) return false;
                    sum += enumeratorx.Current;
                }
                if (dt != sum) return false;
            }
            return true;
        }

        private bool RestoreRowsX(A_SALARY_SHEETS_R row_cur,
            List<A_SALARY_SHEETS_R_HIST> table_hist)
        {
            if (row_cur.XType == ESalarySheetRowType.OnlyOne) return false;

            var table_lapas_r = MyData.DbContextA.BL_A_SALARY_SHEETS_R;

            var drs_lapas_r = table_lapas_r.Where(
                d =>
                d.RowSetTotalRow == row_cur.RowSetTotalRow
            ).ToArray();

            var drs_total_r = drs_lapas_r.Where(
                d =>
                d.XType == ESalarySheetRowType.Total
            ).ToArray();

            var drs_linked_r = drs_lapas_r.Where(
                d =>
                d.XType != ESalarySheetRowType.Total
            ).ToArray();

            if (drs_total_r.Length != 1) return false;
            if (drs_linked_r.Length < 2) return false;

            var dr_total_r = drs_total_r[0];
            var lists_hist_r = new List<A_SALARY_SHEETS_R_HIST>[drs_lapas_r.Length];
            var hist_rs = new A_SALARY_SHEETS_R_HIST[drs_lapas_r.Length];

            var gr = table_hist
                .GroupBy(d => d.ID)
                .ToDictionary(
                    d => d.Key, 
                    d => d.OrderBy(d1 => d1.DT_EDITED).ToList());

            for(int i = 0; i < drs_lapas_r.Length; i++)
            {
                var dr = drs_lapas_r[i];
                if (!gr.TryGetValue(dr.ID, out var list_hist_r)) return false;
                lists_hist_r[i] = list_hist_r;
            }

            var dt = row_cur.DT_EDITED.Value;

            for (int i = 0; i < drs_lapas_r.Length; i++)
            {
                var dr = drs_lapas_r[i];
                var list_hist_r = lists_hist_r[i];
                if(list_hist_r.Count == 1)
                {
                    hist_rs[i] = list_hist_r[0];
                    continue;
                }
                hist_rs[i] = list_hist_r
                    .OrderBy(d => Math.Abs((d.DT_EDITED.Value - dt).TotalMilliseconds))
                    .First();
            }

            var hist_total_dr = hist_rs
                .Where(d => d.ID == dr_total_r.ID)
                .FirstOrDefault();

            var hist_linked_drs = hist_rs
                .Where(d => d.ID != dr_total_r.ID)
                .ToArray();

            if (!TestHistSum(hist_total_dr, hist_linked_drs)) return false;

            for (int i = 0; i < drs_lapas_r.Length; i++)
            {
                var dr = drs_lapas_r[i];
                var hist_r = hist_rs[i];
                if (!RestoreRow(dr, hist_r)) return false;
            }

            return true;
        }

        private void RādītAprēķinaIzmaiņuVēsturiToolStripMenuItem_Click(object sender, EventArgs e)
        {  
            if (bsSarR.Current == null) return;
            var dr_lapas_r = bsSarR.GetCurrentItem<A_SALARY_SHEETS_R>();
            if (dr_lapas_r == null) return;
            var table_hist = DataLoaderA.GetBy_SP_A_SALARY_SHEETS_R_HIST_01(dr_lapas_r.ID);
            if(table_hist.Count == 0)
            {
                MyMainForm.ShowInfo("Aprēķinam nav saglabāti izmaiņu ieraksti.");
                return;
            }
            int rt = FormA_SalarySheetRowHistory.Show(table_hist);
            if (rt == -1) return;
            var dr_hist = table_hist.Where(d => d.IDH == rt).FirstOrDefault();
            if (dr_hist == null) return;

            var rt2 = MyMessageBox.Show(
                "Veicot datu atjaunošanu, pašreizējie aprēķina dati var būt neatgūstami." +
                "Vai tiešām vēlaties veikt datu atjaunošanu uz saglabāto versiju?",
                "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (rt2 != DialogResult.Yes) return;

            bool restore_ok = false;
            if (dr_lapas_r.XType == ESalarySheetRowType.OnlyOne)
            {
                restore_ok = RestoreRow(dr_lapas_r, dr_hist);
            }
            else
            {
                DateTime dt = dr_hist.DT_EDITED.Value;
                DateTime dt1 = dt.AddSeconds(-5);
                DateTime dt2 = dt.AddSeconds(5);

                var table_hist2 = DataLoaderA.GetBy_SP_A_SALARY_SHEETS_R_HIST_02(dr_lapas_r.IDSX.Value, dt1, dt2);
                restore_ok = RestoreRowsX(dr_lapas_r, table_hist2);
            }
            if (!restore_ok)
            {
                MyMainForm.ShowWarning("Neizdevās pārkopēt aprēķina datus no iepriekšējās versijas.");
                return;
            }
            SaveHistory();
            SaveData();
        }

        private void vidējāsIzpeļņasAprēķinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bsSarR.Current == null) return;
            var dr_lapas_r = bsSarR.GetCurrentItem<A_SALARY_SHEETS_R>();
            if (dr_lapas_r == null) return;

            var sr = new SalarySheetRowInfo();
            var err = sr.SetUpFromRowX(dr_lapas_r);
            if (err.HasErrors)
            {
                Form_ErrorList.ShowErrorList(MyMainForm, err);
                return;
            }
            sr.CheckLinkedRows();

            var sc = new SalaryCalcTInfo(sr.SalarySheetRowSet, new SalaryInfo(), true);
            err = sc.FillRow();
            if (err.HasErrors)
            {
                Form_ErrorList.ShowErrorList(MyData.MyMainForm, err);
                return;
            }

            AvPayCalcInfo ap;

            if (sr.IsSingleRow())
            {
                var sc0 = sc.LinkedSCI[0];
                sc0.CheckBeforeReport();
                sc0.AvPayCalc.SetCurMonthPay(sr.SalarySheet.YR, sr.SalarySheet.MT, sc.TotalSI._TOTAL_BEFORE_TAXES, sc.TotalSI._PAY);
                ap = sc0.AvPayCalc;
            }
            else
            {
                sc.CheckBeforeReport();
                sc.AvPayCalc.SetCurMonthPay(sr.SalarySheet.YR, sr.SalarySheet.MT, sc.TotalSI._TOTAL_BEFORE_TAXES, sc.TotalSI._PAY);
                ap = sc.AvPayCalc;
            }

            if (ap == null) return;

            FormA_AvPayCalc.Show(ap);
        }

        /*
        private void vidējāsIzpeļņasAprēķinsToolStripMenuItem_Click2(object sender, EventArgs e)
        {
            if (bsSarR.Current == null) return;
            var dr_lapas_r = (bsSarR.Current as DataRowView)?.Row as KlonsADataSet.SALARY_SHEETS_RRow;
            if (dr_lapas_r == null) return;

            var ap = new AvPayCalcInfo(true);
            var err = ap.CalcList(dr_lapas_r);
            if (err.HasErrors)
            {
                Form_ErrorList.ShowErrorList(MyMainForm, err);
                return;
            }
            Form_AvPayCalc.Show(ap);
        }
        */

        private void slimībasNaudasAprēķinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bsSarR.Current == null) return;
            var dr_lapas_r = bsSarR.GetCurrentItem<A_SALARY_SHEETS_R>();
            if (dr_lapas_r == null) return;

            var sr = new SalarySheetRowInfo();
            var err = sr.SetUpFromRowX(dr_lapas_r);
            if (err.HasErrors)
            {
                Form_ErrorList.ShowErrorList(MyMainForm, err);
                return;
            }
            sr.CheckLinkedRows();

            var sc = new SalaryCalcTInfo(sr.SalarySheetRowSet, new SalaryInfo(), true);
            err = sc.FillRow();
            if (err.HasErrors)
            {
                Form_ErrorList.ShowErrorList(MyData.MyMainForm, err);
                return;
            }

            SickDayCalcInfo sdc = null;

            if (dr_lapas_r.XType == ESalarySheetRowType.OnlyOne) sdc = sc.LinkedSCI[0].SickDayCalc;
            else if (dr_lapas_r.XType == ESalarySheetRowType.Total) sdc = sc.SickDayCalc;
            else sdc = sc.LinkedSCI.Where(d => d.SR.Row == dr_lapas_r).FirstOrDefault()?.SickDayCalc;

            if (sdc == null) return;

            var person = $"{sr.DR_Person_r.FNAME} {sr.DR_Person_r.LNAME}, {sr.GetPositionTitle().Nz().ToLower()}";
            var period = $"{sr.SalarySheet.DT1:dd.MM.yyyy} - {sr.SalarySheet.DT2:dd.MM.yyyy}";
            FormA_SickDaysCalc.Show(sdc, person, period);
        }

        private void atvaļinājumaNaudasAprēķinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bsSarR.Current == null) return;
            var dr_lapas_r = bsSarR.GetCurrentItem<A_SALARY_SHEETS_R>();
            if (dr_lapas_r == null) return;

            var sr = new SalarySheetRowInfo();
            var err = sr.SetUpFromRowX(dr_lapas_r);
            if (err.HasErrors)
            {
                Form_ErrorList.ShowErrorList(MyData.MyMainForm, err);
                return;
            }
            sr.CheckLinkedRows();

            var sc = new SalaryCalcTInfo(sr.SalarySheetRowSet, new SalaryInfo(), true);
            err = sc.FillRow();
            if (err.HasErrors)
            {
                Form_ErrorList.ShowErrorList(MyData.MyMainForm, err);
                return;
            }

            VacationCalcInfo vc = null;

            if (dr_lapas_r.XType == ESalarySheetRowType.OnlyOne) vc = sc.LinkedSCI[0].VacationCalc;
            else if (dr_lapas_r.XType == ESalarySheetRowType.Total) vc = sc.VacationCalc;
            else vc = sc.LinkedSCI.Where(d => d.SR.Row == dr_lapas_r).FirstOrDefault()?.VacationCalc;

            if (vc == null) return;

            var person = $"{sr.DR_Person_r.FNAME} {sr.DR_Person_r.LNAME}, {sr.GetPositionTitle().Nz().ToLower()}";
            var period = $"{sr.SalarySheet.DT1:dd.MM.yyyy} - {sr.SalarySheet.DT2:dd.MM.yyyy}";
            FormA_VacationCalc.Show(vc, person, period);
        }

        private void darbaSamaksasAprēķinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bsSarR.Current == null) return;
            var dr_lapas_r = bsSarR.GetCurrentItem<A_SALARY_SHEETS_R>();
            if (dr_lapas_r == null) return;

            var sr = new SalarySheetRowInfo();
            var err = sr.SetUpFromRowX(dr_lapas_r);
            if (err.HasErrors)
            {
                Form_ErrorList.ShowErrorList(MyMainForm, err);
                return;
            }
            sr.CheckLinkedRows();

            var sc = new SalaryCalcTInfo(sr.SalarySheetRowSet, new SalaryInfo(), true);
            err = sc.FillRow();
            if (err.HasErrors)
            {
                Form_ErrorList.ShowErrorList(MyData.MyMainForm, err);
                return;
            }

            var person = $"{sr.DR_Person_r.FNAME} {sr.DR_Person_r.LNAME}, {sr.GetPositionTitle().Nz().ToLower()}";
            var period = $"{sr.SalarySheet.DT1:dd.MM.yyyy} - {sr.SalarySheet.DT2:dd.MM.yyyy}";

            if (dr_lapas_r.XType == ESalarySheetRowType.OnlyOne)
            {
                var wc = sc.LinkedSCI[0].WorkPayCalc;
                if (wc == null) return;
                FormA_WorkPayCalc.Show(wc, person, period);
            }
            else if (dr_lapas_r.XType == ESalarySheetRowType.Total)
            {
                var wc = sc.WorkPayCalc;
                if (wc == null) return;
                FormA_WorkPayCalc.Show(wc, person, period);
            }
            else
            {
                var wc = sc.LinkedSCI
                    .Where(d => d.SR.Row == dr_lapas_r)
                    .FirstOrDefault()?.WorkPayCalc;
                if (wc == null) return;
                FormA_WorkPayCalc.Show(wc, person, period);
            }
        }

        /*private void darbaSamaksasAprēķinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bsSarR.Current == null) return;
            var dr_lapas_r = (bsSarR.Current as DataRowView)?.Row as KlonsADataSet.SALARY_SHEETS_RRow;
            if (dr_lapas_r == null) return;

            var sr = new SalarySheetRowInfo();
            var err = sr.SetUpFromRowX(dr_lapas_r);
            if (err.HasErrors)
            {
                Form_ErrorList.ShowErrorList(MyMainForm, err);
                return;
            }
            sr.CheckLinkedRows(dr_lapas_r.IDP);

            var wc = new WorkPayCalcTInfo(true);

            sr.SalarySheetRowSet.TotalPersonPay = new SalaryInfo();
            sr.SalarySheetRowSet.TotalRow.TotalPositionPay = sr.SalarySheetRowSet.TotalPersonPay;
            wc.CalcWorkPay(sr.SalarySheetRowSet, sr.SalarySheetRowSet.TotalPersonPay);

            if (err.HasErrors)
            {
                Form_ErrorList.ShowErrorList(MyMainForm, err);
                return;
            }

            var person = string.Format("{0} {1}, {2}", sr.DR_Person_r.FNAME,
                sr.DR_Person_r.LNAME, sr.GetPositionTitle().Nz().ToLower());
            var period = string.Format("{0:dd.MM.yyyy} - {1:dd.MM.yyyy}",
                sr.SalarySheet.DT1, sr.SalarySheet.DT2);
            Form_WorkPayCalc.Show(wc, person, period);
        }*/

        private void miAprekinaSeciba_Click(object sender, EventArgs e)
        {
            if (bsSarR.Current == null) return;
            var dr_lapas_r = bsSarR.GetCurrentItem<A_SALARY_SHEETS_R>();
            if (dr_lapas_r == null) return;


            var sr = new SalarySheetRowInfo();
            var err = sr.SetUpFromRowX(dr_lapas_r);
            if (err.HasErrors)
            {
                Form_ErrorList.ShowErrorList(MyData.MyMainForm, err);
                return;
            }
            sr.CheckLinkedRows();

            var sc = new SalaryCalcTInfo(sr.SalarySheetRowSet, new SalaryInfo(), true);
            err = sc.FillRow();
            if (err.HasErrors)
            {
                Form_ErrorList.ShowErrorList(MyData.MyMainForm, err);
                return;
            }

            PayFxA pfxa = null;
            if (dr_lapas_r.XType == ESalarySheetRowType.OnlyOne) pfxa = sc.LinkedSCI[0].PFxA;
            else if (dr_lapas_r.XType == ESalarySheetRowType.Total) pfxa = sc.PFxB;
            else pfxa = sc.LinkedSCI.Where(d => d.SR.Row == dr_lapas_r).FirstOrDefault()?.PFxA;

            if (pfxa == null) return;

            var person = $"{sr.DR_Person_r.FNAME} {sr.DR_Person_r.LNAME}, {sr.GetPositionTitle().Nz().ToLower()}";
            var period = $"{sr.SalarySheet.DT1:dd.MM.yyyy} - {sr.SalarySheet.DT2:dd.MM.yyyy}";
            FormA_PayFx.Show(pfxa, person, period);
        }


        private void aprēķinaIzklāstsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bsSarR.Current == null) return;
            var dr_lapas_r = bsSarR.GetCurrentItem<A_SALARY_SHEETS_R>();
            if (dr_lapas_r == null) return;

            Report_SalaryCalc1.MakeReport1(dr_lapas_r);
        }
        
        private string GetAprekinaIzklastsPdfFolder()
        {
            string sf = MyData.Params.SalPDFFolder;
            if (string.IsNullOrEmpty(sf) || !Directory.Exists(sf))
                sf = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fd = new FolderBrowserDialog();
            fd.SelectedPath = sf;
            fd.Description = "Norādi mapi priekš PDF failiem";
            var ret = FolderBrowserLauncher.ShowFolderBrowser(fd, this);
            if (ret != DialogResult.OK) return null;
            sf = fd.SelectedPath;
            MyData.Params.SalPDFFolder = sf;
            return sf;
        }

        private void AprekinaIzklastsToPdf(A_SALARY_SHEETS_R dr_lapas_r, string foldername)
        {
            if (dr_lapas_r == null) return;
            if (bsLapas.Current == null) return;
            var dr_lapa = bsLapas.GetCurrentItem<A_SALARY_SHEETS>();

            string filename = $"{dr_lapa.YR}.{dr_lapa.MT} {dr_lapas_r.FNAME} {dr_lapas_r.LNAME}.pdf";
            filename = Path.Combine(foldername, filename);
            Report_SalaryCalc1.SaveReportToPdf(dr_lapas_r, filename);
        }

        private void AprekinaIzklastsAllToPdf(Form_Progress fpr, string foldername)
        {
            if (bsLapas.Current == null) return;
            if (bsSarR.Count == 0) return;
            if (string.IsNullOrEmpty(foldername)) return;

            foreach (var dr in bsSarR)
            {
                if (Cancel_AprekinaIzklastsAllToPdf)
                {
                    if (fpr != null) fpr.Close();
                    return;
                }
                var dr_lapas_r = (dr as ObjectView<A_SALARY_SHEETS_R>)?.Object;
                if (dr_lapas_r == null) continue;
                AprekinaIzklastsToPdf(dr_lapas_r, foldername);
                if (fpr != null) fpr.Progress = fpr.Progress + 1;
            }
        }

        private bool Cancel_AprekinaIzklastsAllToPdf = false;

        private void aprēķinaIzklāstsSaglabātVisiemUzPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bsLapas.Current == null) return;
            if (bsSarR.Count == 0) return;

            var foldername = GetAprekinaIzklastsPdfFolder();
            if (string.IsNullOrEmpty(foldername)) return;

            Form_Progress fpr = null;
            if(bsSarR.Count > 2)
            {
                fpr = new Form_Progress();
                fpr.Message = "Taisam pdf failus...";
                fpr.MaxProgress = bsSarR.Count;
                Cancel_AprekinaIzklastsAllToPdf = false;
                fpr.OnCancel = () => { Cancel_AprekinaIzklastsAllToPdf = true; };
                fpr.RunThis = () => 
                {
                    Task.Run(() =>
                    {
                        Task.Delay(1000);
                        AprekinaIzklastsAllToPdf(fpr, foldername);
                    }).ContinueWith((t) =>
                        { 
                            fpr.Close(); 
                        }
                    , TaskScheduler.FromCurrentSynchronizationContext());
                };
                fpr.ShowDialog(this);
            }
            else
            {
                AprekinaIzklastsAllToPdf(null, foldername);
            }
        }

        public void ShowBonusList(bool b)
        {
            splitContainer2.Panel2Collapsed = !b;
            miShoeBonusList.Checked = b;
            MyData.Params.HideBonusList = !b;
        }

        public void ShowPositionTitleColumn(bool b)
        {
            dgcSarPositionTitle.Visible = b;
            miRādītDarbiniekuAmatus.Checked = b;
            MyData.Params.SalarySheetShowPositionTitle = b;
        }

        public void ShowDataPanel(bool b)
        {
            tabControl1.Visible = b;
            miShowDataPanel.Checked = b;
        }

        private void miShoeBonusList_Click(object sender, EventArgs e)
        {
            ShowBonusList(splitContainer2.Panel2Collapsed);
        }

        private void miRādītDarbiniekuAmatus_Click(object sender, EventArgs e)
        {
            ShowPositionTitleColumn(!dgcSarPositionTitle.Visible);
        }

        private void MiShowDataPanel_Click(object sender, EventArgs e)
        {
            ShowDataPanel(!tabControl1.Visible);
        }

        private void arAmatiemBezParakstiemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bsLapas.Count == 0 || bsLapas.Current == null) return;
            var dr = bsLapas.GetCurrentItem<A_SALARY_SHEETS>();
            Report_Salary2.MakeReport(dr, Report_Salary2.ERepType.WithPosNoSign);
        }

        private void bezAmatiemArParakstiemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bsLapas.Count == 0 || bsLapas.Current == null) return;
            var dr = bsLapas.GetCurrentItem<A_SALARY_SHEETS>();
            Report_Salary2.MakeReport(dr, Report_Salary2.ERepType.NoPosWithSign);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.D))
            {
                ShowDataPanel(!tabControl1.Visible);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

    }


}
