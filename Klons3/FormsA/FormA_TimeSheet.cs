using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using KlonsA.Classes;
using KlonsF.Classes;
using KlonsLIB.Data;
using KlonsLIB.Misc;
using KlonsLIB.Forms;
using Klons3.ModelsA;
using Equin.ApplicationFramework;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;

namespace KlonsA.Forms
{
    public partial class FormA_TimeSheet : MyFormBaseF
    {
        public FormA_TimeSheet()
        {
            InitializeComponent();
            CheckMyFontAndColors();

            ColorThemeHelper.ApplyToControl(cmsMenuMarkDayFact, MyColorTheme);
            ColorThemeHelper.ApplyToControl(cmsMenuMarkDayPlan, MyColorTheme);
            cmsMenuMarkDayFact.Font = Font;
            cmsMenuMarkDayPlan.Font = Font;

            dgvLapa.AutoGenerateColumns = false;
            myStyleDefs1.MakeStyles(dgvLapa);
            MakeColumnHeaders(CalendarMonth);
            tslPeriod.Text = DataLoaderA.GetPeriodStr();
            bsLapuSar.SetFilter<A_TIMESHEET_LISTS>(x => !x.ISFIRSTMT);
            //RefreshHeaderFont();
        }

        public CalendarMonthInfo CalendarMonth = null;

        //BindingListView<A_TIMESHEET> TimeSheetRows = new();
        DLJoinView DLJoinView = new();

        private void Form_TimeSheet_Load(object sender, EventArgs e)
        {
            SetupToolStrips();

            IsLoading = false;
            DLJoinView.ListItemPropertyChanged += TimeSheetRows_ListItemPropertyChanged;
            //MyData.DataSetKlonsA.TIMESHEET.ColumnChanged += TIMESHEET_ColumnChanged;
            SetCurrentList();
            CheckSave();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            dgvLapa.DefaultCellStyle.Font = Font;
            dgvLapa.ColumnHeadersDefaultCellStyle.Font = Font;
            myStyleDefs1.HeaderHolyDay.Font = Font;
            myStyleDefs1.HeaderWeekEnd.Font = Font;
            myStyleDefs1.HolyDay.Font = Font;
            myStyleDefs1.SickDay.Font = Font;
            myStyleDefs1.FreeDay.Font = Font;
            myStyleDefs1.Vacation.Font = Font;
            dgvLapa.AutoResizeColumns();
        }

        private void Form_TimeSheet_FormClosed(object sender, FormClosedEventArgs e)
        {
            //MyData.DataSetKlonsA.TIMESHEET.ColumnChanged -= TIMESHEET_ColumnChanged;
            bsLapa2.DataSource = DLJoinView.Empty;
            DLJoinView.ListItemPropertyChanged -= TimeSheetRows_ListItemPropertyChanged;
            DLJoinView.Dispose();
        }

        private int disableColumnChangedEvent = 0;
        private void TimeSheetRows_ListItemPropertyChanged(object sender, MyItemPropertyChangedEventArgs e)
        {
            TIMESHEET_ColumnChanged(sender, e);
        }

        private void TIMESHEET_ColumnChanged(object sender, MyItemPropertyChangedEventArgs e)
        {
            if (disableColumnChangedEvent < 0) disableColumnChangedEvent = 0;
            if (disableColumnChangedEvent > 0) return;

            disableColumnChangedEvent++;
            TIMESHEET_ColumnChangedA(sender, e);
            disableColumnChangedEvent--;
        }

        private void TIMESHEET_ColumnChangedA(object sender, MyItemPropertyChangedEventArgs e)
        {
            var dr = e.Item as TimesheetJoinedRow;
            if (A_TIMESHEET.IsVx(e.PropertyName))
            {
                RefreshK1(dr.DRT);
                return;
            }

            if (e.PropertyName == nameof(A_TIMESHEET.K1))
            {
                SpreadHours(dr.K1.Value);
                RefreshK1(dr.DRT);
                return;
            }
        }

        private void RefreshK1(A_TIMESHEET dr)
        {
            var sum = dr.SumV();
            if (dr.K1 != sum)
            {
                dr.K1 = sum;
                RefreshCell(dr, dgcK1.Index);
            }
        }

        private void SetupToolStrips()
        {
            InsertInToolStrip(toolStrip1, cbLapuSar, 1);
        }

        public void SelectSheet(A_TIMESHEET_LISTS dr_sar)
        {
            for (int i = 0; i < bsLapuSar.Count; i++)
            {
                var dr = bsLapuSar.GetItem<A_TIMESHEET_LISTS>(i);
                if (dr == dr_sar)
                {
                    bsLapuSar.Position = i;
                    return;
                }
            }
        }

        private void bnavLapa_ItemDeleting(object sender, CancelEventArgs e)
        {
            if (dgvLapa.CurrentRow == null || dgvLapa.CurrentRow.IsNewRow || !AskCanDelete())
                e.Cancel = true;
        }

        private void dgvLapa_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void SetSaveButton(bool red)
        {
            if (red)
                tsbSave.Image = global::KlonsF.Properties.Resources.Save2;
            else
                tsbSave.Image = global::KlonsF.Properties.Resources.Save1;
        }

        public override bool SaveData()
        {
            if (!dgvLapa.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = bsLapuSar.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsLapuSar.HasChanges());
        }

        private void dgvLapa_MyCheckForChanges(object sender, EventArgs e)
        {
            CheckSave();
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void bsLapa_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        public void RefreshHeaderFont()
        {
            var cs1 = new DataGridViewCellStyle(dgvLapa.ColumnHeadersDefaultCellStyle);
            cs1.BackColor = Color.Green;
            cs1.ForeColor = Color.White;

            var cs2 = new DataGridViewCellStyle(dgvLapa.ColumnHeadersDefaultCellStyle);
            cs2.BackColor = Color.DarkGoldenrod;
            cs2.ForeColor = Color.White;

            int nr1 = dgcV1.Index;

            for (int i = nr1; i < nr1 + 31; i++)
            {
                var col = dgvLapa.Columns[i];
                if (col.HeaderCell.Style.BackColor == cs1.BackColor)
                {
                    col.HeaderCell.Style = cs1;
                }
                if (col.HeaderCell.Style.BackColor == cs2.BackColor)
                {
                    col.HeaderCell.Style = cs2;
                }
            }
        }

        public void MakeColumnHeaders(CalendarMonthInfo calmt)
        {
            int nr1 = dgcV1.Index, d1;
            string s;

            if (calmt == null || myStyleDefs1.HeaderHolyDay == null)
            {
                for (int i = nr1; i < nr1 + 31; i++)
                {
                    var col = dgvLapa.Columns[i];
                    col.HeaderText = (i - nr1 + 1).ToString();
                    col.Visible = true;
                }
                return;
            }

            string[] daystr = SomeDataDefsA.GetDaysStr(calmt.WeekDays);

            for (int i = nr1; i < nr1 + 31; i++)
            {
                var col = dgvLapa.Columns[i];
                s = daystr[i - nr1] + "\n" + (i - nr1 + 1);
                d1 = calmt.WeekDays[i - nr1];
                if (d1 == -1)
                    s = "";
                col.Visible = d1 != -1;
                col.HeaderText = s;
                if (calmt.HolyDays[i - nr1] == EHolyDay.Holiday)
                {
                    col.HeaderCell.Style = myStyleDefs1.HeaderHolyDay;
                        
                }
                else if (Utils.IN(d1, 6, 7))
                {
                    col.HeaderCell.Style = myStyleDefs1.HeaderWeekEnd;
                }
                else
                {
                    col.HeaderCell.Style = dgvLapa.ColumnHeadersDefaultCellStyle;
                }

            }
        }

        private void cbLapuSar_Format(object sender, ListControlConvertEventArgs e)
        {
            var drv = e.ListItem as ObjectView<A_TIMESHEET_LISTS>;
            if (drv == null) return;
            var dr = drv.Object;
            if (dr == null) return;
            var state = MyData.DbContextA.Entry(dr).State;
            if (state == EntityState.Deleted || state == EntityState.Detached)
                e.Value = "?";
            else
                e.Value = $"{dr.YR}.{dr.MT:00} {dr.SNR}. {dr.DESCR.Nz()}";
        }

        private bool SkipSetCurrentList = false;

        public void DoBeforeAddNewList()
        {
            SkipSetCurrentList = true;
        }

        public void DoAfterAddNewList()
        {
            SkipSetCurrentList = false;
            SetCurrentList();
        }

        public void SetCurrentList()
        {
            if(SkipSetCurrentList) return;

            bsLapa2.DataSource = DLJoinView.Empty;
            if (bsLapuSar.Current == null)
            {
                DLJoinView.ClearList();
                CalendarMonth = null;
                return;
            }
            var dr = bsLapuSar.GetCurrentItem<A_TIMESHEET_LISTS>();
            if (dr == null) return;

            DLJoinView.MakeList(dr);

            CheckEvents();
            bsLapa2.DataSource = DLJoinView;
            //dgvLapa.Refresh();

            int yr = dr.YR;
            int mt = dr.MT;

            CalendarMonth = new CalendarMonthInfo(yr, mt);

            MakeColumnHeaders(CalendarMonth);
            dgvLapa.AutoResizeColumns();
        }

        private void bsLapuSar_CurrentChanged(object sender, EventArgs e)
        {
            if (IsLoading) return;
            SetCurrentList();
        }


        bool IsTheSameCellValue(int column, int row)
        {
            if (row < 1 || row >= dgvLapa.Rows.Count) return false;
            DataGridViewCell cell1 = dgvLapa[column, row];
            DataGridViewCell cell2 = dgvLapa[column, row - 1];
            if (cell1.Value == null || cell2.Value == null) return false;
            if (cell1.Value.ToString() == cell2.Value.ToString()) return true;
            return false;
        }

        bool IsTheSameCellValue(int column, int column2, int row)
        {
            if (row < 1 || row >= dgvLapa.Rows.Count) return false;
            DataGridViewCell cell1 = dgvLapa[column, row];
            DataGridViewCell cell2 = dgvLapa[column, row - 1];
            if (cell1.Value == null || cell2.Value == null) return false;
            if (cell1.Value.ToString() != cell2.Value.ToString()) return false;

            cell1 = dgvLapa[column2, row];
            cell2 = dgvLapa[column2, row - 1];
            if (cell1.Value == null || cell2.Value == null) return false;
            if (cell1.Value.ToString() != cell2.Value.ToString()) return false;
            return true;
        }

        int GetPositionInGroup(int column, int column2, int row)
        {
            if(row < 0 || row >= dgvLapa.Rows.Count) return -1;
            if(row == 0) return 0;
            var cell1 = dgvLapa[column, row];
            var cell1a = dgvLapa[column2, row];
            int k = row;
            while (true)
            {
                k--;
                var cell2 = dgvLapa[column, k];
                var cell2a = dgvLapa[column2, k];
                if (!object.Equals(cell1.Value, cell2.Value) ||
                    !object.Equals(cell1a.Value, cell2a.Value))
                    return row - k - 1;
                if(k == 0)
                    return row;
            }
        }

        private void dataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex != dgcTitle.Index &&
                e.ColumnIndex != dgcSNRX.Index) return;

            e.AdvancedBorderStyle.Bottom = IsTheSameCellValue(dgcIDX.Index, e.ColumnIndex, e.RowIndex + 1) ?
                DataGridViewAdvancedCellBorderStyle.None :
                dgvLapa.AdvancedCellBorderStyle.Top;

        }

        private void EditCurrent()
        {
            if (dgvLapa.CurrentRow == null || dgvLapa.CurrentRow.IsNewRow) return;
            var jr = dgvLapa.CurrentRow.DataBoundItem as TimesheetJoinedRow;
            if (jr == null) return;
            var fm = new FormA_TimeSheetEdit();
            var prs = TypeDescriptor.GetProperties(jr);
            var id = jr.IDX;

            var table_sar_r = MyData.DbContextA.BL_A_TIMESHEET_LISTS_R;
            var dr_sar_r = jr.DRL;

            int snr = dr_sar_r.SNR.Value;
            var dr_person = dr_sar_r.Person;
            var dr_position = dr_sar_r.Position;
            var dr_plan = dr_sar_r.TimePlan;
            bool plind = dr_sar_r.XPlanType == EPlanType.Individual;
            bool cur_plind = plind;
            bool night = dr_sar_r.NIGHT == 1;
            bool overtime = dr_sar_r.OVERTIME == 1;

            int old_snr = snr;
            var old_dr_person = dr_person;
            var old_dr_position = dr_position;
            var old_dr_plan = dr_plan;

            bool ret = fm.Execute(false, dr_sar_r.Timesheet, dr_sar_r, 
                out snr, out dr_person, out dr_position, out dr_plan, out plind, out night, out overtime);
            if (ret == false) return;

            var er = DLJoinView.EditCurrent(jr, snr, dr_person, dr_position, dr_plan, plind, night, overtime);
            if(er != "OK")
            {
                MyMainForm.ShowWarning(er);
                return;
            }

            if (old_dr_person != dr_person || old_dr_position != dr_position || old_dr_plan != dr_plan)
            {
                CheckEvents(dr_sar_r, true, true);
            }
            else
            {
                CheckEvents(dr_sar_r);
            }
            RefreshRowSet(jr.RowSet);
        }


        private bool HasAnyTimePlan(A_TIMESHEET_LISTS row)
        {
            var rr = MyData.DbContextA.BL_A_TIMESHEET.Where(d =>
            {
                return d.YR == row.YR && d.MT == row.MT &&
                (d.XKind1 == EKind1.PlanGroupDay || d.XKind1 == EKind1.PlanIndividualDay);
            });
            return rr.FirstOrDefault() != null;
        }

        private bool HasTimePlan(A_TIMESHEET_LISTS row, A_TIMEPLAN_LIST dr_plan)
        {
            var rr = MyData.DbContextA.BL_A_TIMESHEET.Where(d =>
            {
                return d.YR == row.YR && d.MT == row.MT &&
                (d.XKind1 == EKind1.PlanGroupDay || d.XKind1 == EKind1.PlanIndividualDay) &&
                (d.TimePlan != null && d.TimePlan == dr_plan);
            });
            return rr.FirstOrDefault() != null;
        }

        private void AddNew()
        {
            if (bsLapuSar.Current == null) return;
            var dr_sar = bsLapuSar.GetCurrentItem<A_TIMESHEET_LISTS>();

            if (!HasAnyTimePlan(dr_sar))
            {
                MyMainForm.ShowWarning("Mēnesim nav izveidots darba laika plāns");
                return;
            }
            
            var fm = new FormA_TimeSheetEdit();

            A_TIMESHEET_LISTS_R new_dr_timesheet = null;
            int snr = 1;
            A_PERSONS dr_person = null;
            A_POSITIONS dr_position = null;
            A_TIMEPLAN_LIST dr_plan = null;
            bool plind = false;
            bool night = false;
            bool overtime = false;

            bool ret = fm.Execute(true, dr_sar, null, out snr, 
                out dr_person, out dr_position, out dr_plan, out plind, out night, out overtime);
            if (ret == false) return;

            if (!HasTimePlan(dr_sar, dr_plan))
            {
                MyMainForm.ShowWarning("Mēnesim nav izveidots darba laika plāns");
                return;
            }

            var er = DLJoinView.AddNew(dr_sar, out new_dr_timesheet, 
                snr, dr_person, dr_position, dr_plan, plind, night, overtime);
            if (er != "OK")
            {
                MyMainForm.ShowWarning(er);
                return;
            }
            CheckEvents(new_dr_timesheet, true, true);
            dgvLapa.AutoResizeColumns();
        }

        public void DeleteCurrent()
        {
            if (dgvLapa.CurrentRow == null || dgvLapa.CurrentRow.IsNewRow) return;
            var jr = dgvLapa.CurrentRow.DataBoundItem as TimesheetJoinedRow;
            if (jr == null) return;
            DLJoinView.DeleteRowSet(jr);
            SaveData();
        }


        private void tsbEdit_Click(object sender, EventArgs e)
        {
            EditCurrent();
        }

        private void tsbNew_Click(object sender, EventArgs e)
        {
            AddNew();
        }

        private void tsbDelete_Click(object sender, EventArgs e)
        {
            if (dgvLapa.CurrentRow == null || dgvLapa.CurrentRow.IsNewRow || !AskCanDelete()) return;
            DeleteCurrent();
        }

        public int FindRow(TimesheetJoinedRow jr)
        {
            if (jr == null) return -1;
            try
            {
                for (int i = 0; i < bsLapa2.Count; i++)
                    if (bsLapa2[i] == jr) return i;
            }
            catch (Exception)
            {
            }
            return -1;
        }

        public int FindRow(A_TIMESHEET dr)
        {
            if (dr == null) return -1;
            try
            {
                for (int i = 0; i < bsLapa2.Count; i++)
                    if ((bsLapa2[i] as TimesheetJoinedRow)?.DRT == dr) return i;
            }
            catch (Exception)
            {
            }
            return -1;
        }

        public void RefreshCell(TimesheetJoinedRow jr, int daynr)
        {
            int rownr = FindRow(jr);
            if (rownr == -1) return;
            dgvLapa.InvalidateCell(dgcV1.Index + daynr - 1, rownr);
        }

        public void RefreshCell(A_TIMESHEET dr, int colnr)
        {
            int rownr = FindRow(dr);
            if (rownr == -1) return;
            dgvLapa.InvalidateCell(colnr, rownr);
        }

        private void RefreshRowSet(TimesheetJoinedRowSet rowset)
        {
            for (int i = 0; i < dgvLapa.Rows.Count; i++)
            {
                var jr = dgvLapa.GetDataItem(i) as TimesheetJoinedRow;
                if (jr == null) continue;
                if (jr.RowSet == rowset)
                    dgvLapa.InvalidateRow(i);
            }
        }

        private void dgvLapa_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var jr = dgvLapa.GetDataItem(e.RowIndex) as TimesheetJoinedRow;
            var dr = jr.DRT;
            if (dr == null) return;
            EKind1 kind1 = dr.XKind1;

            if (e.ColumnIndex >= dgcV1.Index &&  e.ColumnIndex <= dgcV31.Index)
            {
                if (kind1 == EKind1.PlanGroupDay ||
                    kind1 == EKind1.PlanGroupaNight)
                { 
                    e.Cancel = true;
                    return;
                }

                if (kind1 == EKind1.PlanIndividualDay ||
                    kind1 == EKind1.PlanIndividualNight)
                {
                    return;
                }

                int daynr = e.ColumnIndex - dgcV1.Index + 1;
                var daycode = jr.RowSet.Fact.DRT.DxFact[daynr - 1];

                //if (!SomeDataDefs.IsDayForWork(daycode))
                if (!SomeDataDefsA.CanEditFact(daycode))
                {
                    e.Cancel = true;
                    return;
                }
            }
            if (e.ColumnIndex == dgcK1.Index)
            {
                if (kind1 != EKind1.Fact)
                {
                    e.Cancel = true;
                    return;
                }

            }

        }


        public bool CheckEvents(A_TIMESHEET_LISTS_R dr_sar_r, bool sethours = false, bool setdaycode = false)
        {
            if (bsLapuSar.Current == null) return true;
            var dr_sar = bsLapuSar.GetCurrentItem<A_TIMESHEET_LISTS>();
            var err = DataTasksA.CheckDarbaLaiksEvents(dr_sar, dr_sar_r, sethours, setdaycode);

            if (err.HasErrors)
            {
                Form_ErrorList.ShowErrorList(MyMainForm, err);
                return false;
            }
            SaveData();
            return true;
        }

        public bool CheckEvents(bool sethours = false, bool setdaycode = false)
        {
            if (bsLapuSar.Current == null) return true;
            var dr_sar = bsLapuSar.GetCurrentItem<A_TIMESHEET_LISTS>();
            var err = DataTasksA.CheckDarbaLaiksEvents(dr_sar, sethours: sethours, setdaycode: setdaycode);

            if (err.HasErrors)
            {
                Form_ErrorList.ShowErrorList(MyMainForm, err);
                return false;
            }
            SaveData();
            return true;
        }

        private void dgvLapa_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex > 0 && e.ColumnIndex == dgcSNRX.Index &&
                IsTheSameCellValue(dgcIDX.Index, e.ColumnIndex, e.RowIndex))
            {
                e.Value = "";
                e.FormattingApplied = true;
                return;
            }
            if (e.RowIndex > 0 && e.ColumnIndex == dgcTitle.Index &&
                IsTheSameCellValue(dgcIDX.Index, e.ColumnIndex, e.RowIndex))
            {
                int cp = GetPositionInGroup(dgcIDX.Index, e.ColumnIndex, e.RowIndex);
                if (cp == 1)
                    e.Value = "  " + dgvLapa[dgcPosition.Index, e.RowIndex].Value;
                else
                    e.Value = "";
                e.FormattingApplied = true;
                return;
            }

            if (e.ColumnIndex == dgcKind1.Index)
            {
                if (e.Value == null || e.Value == DBNull.Value) return;
                if (!(e.Value is short)) return;
                EKind1 k = (EKind1)e.Value;
                var s = SomeDataDefsA.DarbaLaiksK1[(int)k].Val;
                e.Value = s;
                e.FormattingApplied = true;
                return;
            }

            if (e.ColumnIndex >= dgcV1.Index && e.ColumnIndex <= dgcV31.Index)
            {
                var cell = dgvLapa[e.ColumnIndex, e.RowIndex];

                if (e.Value == null || e.Value == DBNull.Value)
                {
                    cell.Style = dgvLapa.DefaultCellStyle;
                    e.Value = "";
                    e.FormattingApplied = true;
                    return;
                }

                float v = (float)e.Value;

                int daynr = e.ColumnIndex - dgcV1.Index + 1;

                var jr = dgvLapa.GetDataItem(e.RowIndex) as TimesheetJoinedRow;
                var dr = jr.DRT;

                if (jr.RowSet?.Fact?.DRT == null)
                {
                    cell.Style = dgvLapa.DefaultCellStyle;
                    e.Value = "";
                    e.FormattingApplied = true;
                    return;
                }

                short daycode = (short)dr.Dx[daynr - 1];

                if (dr.XKind1 == EKind1.Fact)
                {
                    var daycodefact = dr.DxFact[daynr - 1];
                    if (SomeDataDefsA.IsDayVacation(daycodefact))
                        cell.Style = myStyleDefs1.Vacation;
                    else if (SomeDataDefsA.IsSickDay(daycodefact))
                        cell.Style = myStyleDefs1.SickDay;
                    else if (daycodefact == EDayFactId.B)
                        cell.Style = myStyleDefs1.FreeDay;
                    else if (daycodefact == EDayFactId.S)
                        cell.Style = myStyleDefs1.HolyDay;
                    else
                        cell.Style = dgvLapa.DefaultCellStyle;
                }
                else if (dr.XKind1 == EKind1.PlanIndividualDay || dr.XKind1 == EKind1.PlanGroupDay)
                {
                    var daycodeplan = dr.DxPlan[daynr - 1];
                    if (daycodeplan == EDayPlanId.BD)
                        cell.Style = myStyleDefs1.FreeDay;
                    else if (daycodeplan == EDayPlanId.SD || daycodeplan == EDayPlanId.DDSD || daycodeplan == EDayPlanId.SDDD)
                        cell.Style = myStyleDefs1.HolyDay;
                    else
                        cell.Style = dgvLapa.DefaultCellStyle;
                }
                else if (dr.XKind1 == EKind1.FactNight || dr.XKind1 == EKind1.FactOvertime)
                {
                    var daycodefact = jr.RowSet.Fact?.DRT == null ? 0 :
                        jr.RowSet.Fact.DRT.DxFact[daynr - 1];
                    daycode = (short)daycodefact;
                }
                else if (dr.XKind1 == EKind1.PlanIndividualNight || dr.XKind1 == EKind1.PlanGroupaNight)
                {
                    var daycodeplan = 
                        jr.RowSet.Plan.DRT == null ? 
                        0 :
                        jr.RowSet.Plan.DRT.DxPlan[daynr - 1];
                    daycode = (short)daycodeplan;
                    if (daycodeplan == EDayPlanId.BD)
                        cell.Style = myStyleDefs1.HeaderWeekEnd;
                }
                e.Value = FormatCell(v, daycode, dr.XKind1);
                e.FormattingApplied = true;
                return;
            }
        }

        public string FormatCell(float v, short daycode, EKind1 kind1)
        {
            string ret = v.ToString();

            if (kind1 == EKind1.PlanIndividualDay || kind1 == EKind1.PlanGroupDay)
            {
                var daycodeplan = (EDayPlanId)daycode;
                if (daycodeplan == EDayPlanId.DD)
                {
                    if (v == 0.0F) return "";
                    return ret;
                }
                string daystr = SomeDataDefsA.GetPlanIdStr(daycodeplan);
                if (daycodeplan == EDayPlanId.BD || daycodeplan == EDayPlanId.SD)
                {
                    if (v == 0.0f) ret = $"{daystr}";
                    else ret = $"{daystr} {v}";
                    return ret;
                }
                ret = $"{daystr} {v}";
                return ret;
            }

            if (kind1 == EKind1.Fact)
            {
                var daycodefact = (EDayFactId)daycode;
                if (daycodefact == EDayFactId.D)
                {
                    if (v == 0.0F) return "";
                    return ret;
                }
                string daystr = SomeDataDefsA.GetFactIdStr(daycodefact);
                if (v == 0.0f) ret = daystr;
                else ret = $"{daystr} {v}";
                return ret;
            }

            if (v == 0.0f) return "";
            return ret;
        }


        private void dgvLapa_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.ColumnIndex >= dgcV1.Index && e.ColumnIndex <= dgcV31.Index)
            {
                if (e.Value == null || e.Value == DBNull.Value)
                {
                    e.Value = 0.0f;
                    e.ParsingApplied = true;
                    return;
                }

                if (!(e.Value is string)) return;

                float hours;

                int daynr = e.ColumnIndex - dgcV1.Index + 1;
                var jr = dgvLapa.Rows[e.RowIndex].DataBoundItem as TimesheetJoinedRow;
                var dr = jr.DRT;

                if (dr.XKind1 == EKind1.PlanIndividualDay)
                {
                    EDayPlanId daycodeplan;
                    EDayPlanId curdaycodeplan = dr.DxPlan[daynr - 1];

                    if (!SomeDataDefsA.ParsePlanDayStr(e.Value as string, out daycodeplan, out hours))
                    {
                        e.Value = 0.0f;
                        e.ParsingApplied = true;
                        return;
                    }

                    e.Value = hours;
                    e.ParsingApplied = true;

                    if (daycodeplan == EDayPlanId.None)
                        daycodeplan = curdaycodeplan;
                    if (daycodeplan != curdaycodeplan)
                        dr.DxPlan[daynr - 1] = daycodeplan;

                    if (!SomeDataDefsA.CanPlanHaveHours(dr.DxPlan[daynr - 1]))
                    {
                        e.Value = 0.0f;
                        e.ParsingApplied = true;
                        if(jr.RowSet.PlanNight != null && 
                            jr.RowSet.PlanNight.DRT.Vx[daynr - 1] != 0.0f)
                        {
                            jr.RowSet.PlanNight.DRT.Vx[daynr - 1] = 0.0f;
                            RefreshCell(jr.RowSet.PlanNight, daynr);
                        }
                    }

                    return;
                }

                if (dr.XKind1 == EKind1.PlanIndividualNight)
                {
                    if (jr.RowSet?.Plan?.DRT == null) return;
                    EDayPlanId daycodeplan = jr.RowSet.Plan.DRT.DxPlan[daynr - 1];
                    if (!SomeDataDefsA.CanPlanHaveHours(daycodeplan))
                    {
                        e.Value = 0.0f;
                        e.ParsingApplied = true;
                    }
                    return;
                }


                if (dr.XKind1 == EKind1.Fact)
                {
                    EDayFactId daycodefact;
                    EDayFactId curdaycodefact = dr.DxFact[daynr - 1];

                    if (!(SomeDataDefsA.CanChangeDayId(curdaycodefact) ||
                        SomeDataDefsA.CanEditHours(curdaycodefact))) return;

                    if (!SomeDataDefsA.ParseFactDayStr(e.Value as string, out daycodefact, out hours))
                    {
                        e.Value = 0.0f;
                        e.ParsingApplied = true;
                        return;
                    }

                    e.Value = hours;
                    e.ParsingApplied = true;

                    if (daycodefact == EDayFactId.None)
                        daycodefact = curdaycodefact;

                    if (daycodefact != curdaycodefact)
                        dr.DxFact[daynr - 1] = daycodefact;

                    if (!SomeDataDefsA.CanFactHaveHours(dr.DxFact[daynr - 1]))
                    {
                        e.Value = 0.0f;
                        e.ParsingApplied = true;

                        if (jr.RowSet.FactNight != null && 
                            jr.RowSet.FactNight.DRT.Vx[daynr - 1] != 0.0f)
                        {
                            jr.RowSet.FactNight.DRT.Vx[daynr - 1] = 0.0f;
                            RefreshCell(jr.RowSet.FactNight, daynr);
                        }

                        if (jr.RowSet.FactOvertime != null &&
                            jr.RowSet.FactOvertime.DRT.Vx[daynr - 1] != 0.0f)
                        {
                            jr.RowSet.FactOvertime.DRT.Vx[daynr - 1] = 0.0f;
                            RefreshCell(jr.RowSet.FactOvertime, daynr);
                        }
                    }

                    return;
                }

                if (dr.XKind1 == EKind1.FactNight)
                {
                    if (jr.RowSet?.Fact?.DRT == null) return;
                    EDayFactId daycodefact = jr.RowSet.Fact.DRT.DxFact[daynr - 1];

                    if (!SomeDataDefsA.CanEditHours(daycodefact)) return;

                    if (!SomeDataDefsA.CanFactHaveHours(daycodefact))
                    {
                        e.Value = 0.0f;
                        e.ParsingApplied = true;
                    }

                    return;
                }

                if (dr.XKind1 == EKind1.FactOvertime)
                {
                    if (jr.RowSet?.Fact?.DRT == null) return;
                    EDayFactId daycodefact = jr.RowSet.Fact.DRT.DxFact[daynr - 1];

                    if (!SomeDataDefsA.CanEditHours(daycodefact)) return;

                    if (!SomeDataDefsA.CanFactHaveHours(daycodefact))
                    {
                        e.Value = 0.0f;
                        e.ParsingApplied = true;
                    }

                    return;
                }

            }
        }

        private void dgvLapa_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1 || bsLapa2.Current == null) return;
            if (dgvLapa.CurrentCell != null && dgvLapa.CurrentCell.IsInEditMode) return;

            var cell = dgvLapa[e.ColumnIndex, e.RowIndex];
            dgvLapa.CurrentCell = cell;
            if (dgvLapa.CurrentCell != null && dgvLapa.CurrentRow.IsNewRow) return;

            if (e.ColumnIndex >= dgcV1.Index && e.ColumnIndex <= dgcV31.Index)
            {
                var jr = bsLapa2.Current as TimesheetJoinedRow;
                var dr = jr.DRT;
                var bounds = dgvLapa.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

                if (dr.XKind1 == EKind1.PlanIndividualDay)
                {
                    e.ContextMenuStrip = cmsMenuMarkDayPlan;
                    return;
                }
                if (dr.XKind1 == EKind1.Fact)
                {
                    e.ContextMenuStrip = cmsMenuMarkDayFact;
                    return;
                }

            }
        }

        private void SpreadHours()
        {
            if (bsLapa2.Count == 0 || bsLapa2.Current == null) return;

            var fm = new Form_InputBox("", "Stundas:", "0");
            if (fm.ShowDialog(MyMainForm) != DialogResult.OK) return;
            if (string.IsNullOrEmpty(fm.SelectedValueStr)) return;
            float hr = 0.0f;
            if (!float.TryParse(fm.SelectedValueStr, out hr)) return;
            if (hr < 0.0f) return;
            SpreadHours(hr);
        }

        private void SpreadHours(float hr)
        {
            if (bsLapa2.Count == 0 || bsLapa2.Current == null) return;
            var jr = bsLapa2.Current as TimesheetJoinedRow;

            float shr = 0.0f;
            int i, dct = 0;
            float[] hr1 = new float[31];

            if (hr > 0.0f)
            {

                for (i = 0; i < 31; i++)
                {
                    var dayfactid = jr.RowSet.Fact.DRT.DxFact[i];
                    var dayplanid = jr.RowSet.Plan.DRT.DxPlan[i];
                    if (!SomeDataDefsA.CanEditFact(dayfactid)) continue;
                    if (dayplanid != EDayPlanId.DD) continue;
                    hr1[i] = -jr.RowSet.Plan.DRT.Vx[i];
                    if (hr1[i] == 0.0f) continue;
                    dct++;
                }
                if (dct == 0) return;
                var rnd = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
                while (true)
                {
                    if (dct == 0 || shr == hr) break;
                    float lhr = hr - shr;
                    int k = rnd.Next(dct);
                    int k1 = 0;
                    for (i = 0; i < 31; i++)
                    {
                        if (hr1[i] < 0.0f)
                        {
                            if (k1 == k)
                            {
                                var h = -hr1[i];
                                if (h > lhr) h = lhr;
                                hr1[i] = h;
                                shr += h;
                                dct--;
                                break;
                            }
                            k1++;
                        }
                    }

                }

            }

            for (i = 0; i < 31; i++)
            {
                if (hr1[i] > 0.0f)
                {
                    jr.RowSet.Fact.DRT.DxFact[i] = EDayFactId.D;
                    jr.RowSet.Fact.DRT.Vx[i] = hr1[i];
                }
                else if(hr1[i] <= 0.0f)
                {
                    jr.RowSet.Fact.DRT.Vx[i] = 0.0f;
                    if (SomeDataDefsA.IsDayForWork(jr.RowSet.Fact.DRT.DxFact[i]))
                        jr.RowSet.Fact.DRT.DxFact[i] = EDayFactId.B;
                }
            }

            var sh = jr.RowSet.Fact.DRT.SumV();

            if (jr.RowSet.Fact.DRT.K1 != sh)
                jr.RowSet.Fact.DRT.K1 = sh;

            int rownr = FindRow(jr.RowSet.Fact.DRT);
            if (rownr == -1) return;
            dgvLapa.InvalidateRow(rownr);

        }

        public void ChangeFactDayCode(int daynr, EDayFactId newdaycode)
        {
            if (bsLapa2.Count == 0 || bsLapa2.Current == null) return;
            var jr = bsLapa2.Current as TimesheetJoinedRow;
            var dr = jr.DRT;
            var curdaycode = dr.DxFact[daynr - 1];

            if (curdaycode == newdaycode) return;
            if (!SomeDataDefsA.CanChangeDayId(curdaycode)) return;

            var currentv = dr.Vx[daynr - 1];
            var newv = currentv;
            if (!SomeDataDefsA.CanFactHaveHours(newdaycode)) newv = 0.0f;
            int colnr = dgcV1.Index + daynr - 1;

            dr.DxFact[daynr - 1] = newdaycode;
            if (newv != currentv) dr.Vx[daynr - 1] = newv;

            if (!SomeDataDefsA.CanFactHaveHours(newdaycode))
            {
                if (jr.RowSet.FactNight != null &&
                    jr.RowSet.FactNight.DRT.Vx[daynr - 1] != 0.0f)
                {
                    jr.RowSet.FactNight.DRT.Vx[daynr - 1] = 0.0f;
                    RefreshCell(jr.RowSet.FactNight, daynr);
                }
                if (jr.RowSet.FactOvertime != null &&
                    jr.RowSet.FactOvertime.DRT.Vx[daynr - 1] != 0.0f)
                {
                    jr.RowSet.FactOvertime.DRT.Vx[daynr - 1] = 0.0f;
                    RefreshCell(jr.RowSet.FactOvertime, daynr);
                }
            }

            RefreshCell(jr, daynr);
            dgvLapa.AutoResizeColumn(colnr);
        }

        public void ChangePlanDayCode(int daynr, EDayPlanId newdaycode)
        {
            if (bsLapa2.Count == 0 || bsLapa2.Current == null) return;
            var jr = bsLapa2.Current as TimesheetJoinedRow;
            var dr = jr.DRT;
            var curdaycode = dr.DxPlan[daynr - 1];

            if (curdaycode == newdaycode) return;

            var currentv = dr.Vx[daynr - 1];
            var newv = currentv;
            if (!SomeDataDefsA.CanPlanHaveHours(newdaycode)) newv = 0.0f;
            int colnr = dgcV1.Index + daynr - 1;

            dr.DxPlan[daynr - 1] = newdaycode;
            if (newv != currentv) dr.Vx[daynr - 1] = newv;

            if (!SomeDataDefsA.CanPlanHaveHours(newdaycode))
            {
                if (jr.RowSet.PlanNight != null &&
                    jr.RowSet.PlanNight.DRT.Vx[daynr - 1] != 0.0f)
                {
                    jr.RowSet.PlanNight.DRT.Vx[daynr - 1] = 0.0f;
                    RefreshCell(jr.RowSet.PlanNight, daynr);
                }
            }

            RefreshCell(jr, daynr);
            dgvLapa.AutoResizeColumn(colnr);
        }

        private void cmsMenuMarkDayFact_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (bsLapa2.Current == null || dgvLapa.CurrentCell == null || dgvLapa.CurrentRow.IsNewRow) return;
            var menuitems = new[] { miFactD, miFactB, miFactK, miFactSVVI, miFactVI };
            var daycodes = new[] {EDayFactId.D, EDayFactId.B, EDayFactId.K,
                EDayFactId.S, EDayFactId.V, EDayFactId.DS, EDayFactId.KS};

            int i, k = -1;
            for (i = 0; i < menuitems.Length; i++)
            {
                if (menuitems[i] != e.ClickedItem) continue;
                k = i;
                break;
            }
            if (k == -1) return;
            var newdaycode = daycodes[k];
            int colnr = dgvLapa.CurrentCell.ColumnIndex;
            int daynr = colnr - dgcV1.Index + 1;

            ChangeFactDayCode(daynr, newdaycode);
        }

        private void cmsMenuMarkDayPlan_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (bsLapa2.Current == null || dgvLapa.CurrentCell == null || dgvLapa.CurrentRow.IsNewRow) return;

            var menuitems = new[] { miPlanDD, miPlanBD, miPlanSD, miPlanSDDD, miPlanDDSD };
            var daycodes = new[] {EDayPlanId.DD, EDayPlanId.BD,
                EDayPlanId.SD, EDayPlanId.DDSD, EDayPlanId.SDDD };

            int i, k = -1;
            for (i = 0; i < menuitems.Length; i++)
            {
                if (menuitems[i] == e.ClickedItem)
                {
                    k = i;
                    break;
                }
            }
            if (k == -1) return;

            int colnr = dgvLapa.CurrentCell.ColumnIndex;
            int daynr = colnr - dgcV1.Index + 1;

            ChangePlanDayCode(daynr, daycodes[k]);
        }

        private void dgvLapa_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int curcol = dgvLapa.CurrentCell.ColumnIndex;
            if (curcol >= dgcV1.Index && curcol <= dgcV31.Index)
            {
                float hours;

                var jr = dgvLapa.CurrentRow.DataBoundItem as TimesheetJoinedRow;
                var dr = jr.DRT;

                if(dr.XKind1 == EKind1.PlanIndividualDay)
                {
                    EDayPlanId daycodeplan;
                    if (!SomeDataDefsA.ParsePlanDayStr(e.Control.Text, out daycodeplan, out hours)) return;
                    e.Control.Text = hours.ToString();
                    return;
                }

                if (dr.XKind1 == EKind1.Fact)
                {
                    EDayFactId daycodefact;
                    if (!SomeDataDefsA.ParseFactDayStr(e.Control.Text, out daycodefact, out hours)) return;
                    e.Control.Text = hours.ToString();
                    return;
                }

            }
        }

        private void dgvLapa_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= dgcV1.Index && e.ColumnIndex <= dgcV31.Index)
            {
                var cell = dgvLapa[e.ColumnIndex, e.RowIndex];
                if(cell.PreferredSize.Width > cell.Size.Width)
                    dgvLapa.AutoResizeColumn(e.ColumnIndex);
            }
        }


        private void tsmiSheetCheckEvents_Click(object sender, EventArgs e)
        {
            CheckEvents(sethours: false, setdaycode: true);
            dgvLapa.Refresh();
            dgvLapa.AutoResizeColumns();
        }

        private void tsmiSheetFillFact_Click(object sender, EventArgs e)
        {
            CheckEvents(sethours: true, setdaycode: false);
            dgvLapa.Refresh();
            dgvLapa.AutoResizeColumns();
        }


        private void tsmiPersonCheckEvents_Click(object sender, EventArgs e)
        {
            if (dgvLapa.CurrentRow == null || dgvLapa.CurrentRow.IsNewRow) return;
            var jr = dgvLapa.GetCurrentDataItem() as TimesheetJoinedRow;
            if (jr == null) return;
            CheckEvents(jr.DRL, sethours: false, setdaycode: true);
            RefreshRowSet(jr.RowSet);
            dgvLapa.AutoResizeColumns();
        }

        private void tsmiPersonFillFact_Click(object sender, EventArgs e)
        {
            if (dgvLapa.CurrentRow == null || dgvLapa.CurrentRow.IsNewRow) return;
            var jr = dgvLapa.GetCurrentDataItem() as TimesheetJoinedRow;
            if (jr == null) return;
            CheckEvents(jr.DRL, sethours: true, setdaycode: false);
            RefreshRowSet(jr.RowSet);
            dgvLapa.AutoResizeColumns();
        }

        private void MakeReportTimeSheet1(ReportTimeSheet1.ERoportType roporttype)
        {
            if (bsLapuSar.Count == 0 || bsLapuSar.Current == null) return;
            var dr = bsLapuSar.GetCurrentItem<A_TIMESHEET_LISTS>();

            if (DLJoinView.Count == 0)
            {
                MyMainForm.ShowWarning("Tukša atskaite.");
                return;
            }
            if (CalendarMonth == null) return;
            var tr = new ReportTimeSheet1();
            tr.MakeReport(DLJoinView, CalendarMonth, dr.DESCR, roporttype, myStyleDefsForReport);
        }

        private void darbaLaikaLapaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeReportTimeSheet1(ReportTimeSheet1.ERoportType.Simple);
        }

        private void DarbaLaikaLapaArKrāsāmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakeReportTimeSheet1(ReportTimeSheet1.ERoportType.WithColors);
        }

        private void darbaLaikaKopsummasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bsLapuSar.Count == 0 || bsLapuSar.Current == null) return;
            var dr = bsLapuSar.GetCurrentItem<A_TIMESHEET_LISTS>();

            if (DLJoinView.Count == 0)
            {
                MyMainForm.ShowWarning("Tukša atskaite.");
                return;
            }
            var tr = new ReportTimeSheet2();
            tr.MakeReport(dr, dr.DESCR);
        }

        private void dgvLapa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F1)
                SpreadHours();
        }

    }


}
