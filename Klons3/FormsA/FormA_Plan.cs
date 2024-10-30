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
using KlonsLIB.Misc;
using KlonsLIB.Forms;
using Klons3.ModelsA;
using Equin.ApplicationFramework;

namespace KlonsA.Forms
{
    public partial class FormA_Plan : MyFormBaseF
    {
        public FormA_Plan()
        {
            InitializeComponent();
            CheckMyFontAndColors();

            myStyleDefs1.MakeStyles(dgvPlans);

            tslPeriod.Text = DataLoaderA.GetPeriodStr();

            //bsK1.DataSource = SomeDataDefs.DarbaLaiksK2a;
            //dgcKind1.ValueMember = "Key";
            //dgcKind1.DisplayMember = "Val";

            cbYR.Items.Clear();
            for (int i = DataLoaderA.LoadedDT1.Year; i <= DataLoaderA.LoadedDT2.Year; i++)
                cbYR.Items.Add(i);

            bsPlans.AllowNew = false;
            dgvPlans.AllowUserToAddRows = false;

            bsPlans.SetFilter<A_TIMESHEET>(x => x.KIND1 == 0 || x.KIND1 == 1);

            cbYR.SelectedIndex = cbYR.Items.Count - 1;
            cbMT.SelectedIndex = 0;
        }

        public int PYear = 0;
        public int PMonth = 0;
        public CalendarMonthInfo CalendarMonth = null;

        private void Form_Plan_Load(object sender, EventArgs e)
        {
            CheckSave();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            dgvPlans.DefaultCellStyle.Font = Font;
            dgvPlans.ColumnHeadersDefaultCellStyle.Font = Font;
            myStyleDefs1.HeaderHolyDay.Font = Font;
            myStyleDefs1.HeaderWeekEnd.Font = Font;
            myStyleDefs1.HolyDay.Font = Font;
            myStyleDefs1.SickDay.Font = Font;
            myStyleDefs1.FreeDay.Font = Font;
            myStyleDefs1.Vacation.Font = Font;
        }

        private void Form_Plans_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        bool IsVx(string sv)
        {
            if (sv.IsNOE() || sv.Length < 2 || sv[0] != 'V') return false;
            var sx = sv.Substring(1);
            if (!int.TryParse(sx, out int x)) return false;
            if (x < 1 || x > 31) return false;
            return true;
        }

        private void BsPlans_ListItemPropertyChanged(object sender, KlonsLIB.Data.MyItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(A_TIMESHEET.K1)) return;
            var dr = e.Item as A_TIMESHEET;
            if (IsVx(e.PropertyName))
            {
                float sum = 0.0f;
                for (int i = 1; i <= 31; i++)
                {
                    float v = dr.Vx[i];
                    if (v < 0.0f || v >= 24.0f) continue;
                    sum += v;
                }
                if (dr.K1 != sum)
                {
                    dr.K1 = sum;
                    RefreshCell(dr, dgcK1.Index);
                }
            }
        }

        public int FindRow(A_TIMESHEET dr)
        {
            if (dr == null) return -1;
            try
            {
                for (int i = 0; i < bsPlans.Count; i++)
                    if (bsPlans.GetItem<A_TIMESHEET>(i) == dr) return i;
            }
            catch (Exception)
            {
            }
            return -1;
        }

        public void RefreshCell(A_TIMESHEET dr, int colnr)
        {
            int rownr = FindRow(dr);
            if (rownr == -1) return;
            dgvPlans.InvalidateCell(colnr, rownr);
        }

        private bool CanDelete(A_TIMESHEET dr)
        {
            var table_tr = MyData.DbContextA.BL_A_TIMESHEET_LISTS_R;

            if ((dr.XKind1 == EKind1.PlanGroupaNight ||
                dr.XKind1 == EKind1.PlanIndividualNight) &&
                dr.TimePlan.NIGHT == 0)
            {
                return true;
            }

            int ct1 = table_tr.Where(
                d =>
                d.TimePlan == dr.TimePlan &&
                d.Timesheet.YR == dr.YR &&
                d.Timesheet.MT == dr.MT
                ).Count();

            return ct1 == 0;
        }

        public void DeleteCurrent()
        {
            if (bsPlans.Current == null) return;
            var dr = bsPlans.GetCurrentItem<A_TIMESHEET>();
            if (!CanDelete(dr))
            {
                MyMainForm.ShowWarning("Ierakstu nevar dzēst,\njo tas ir izmantots citur.");
                return;
            }
            bnavPlans.DeleteCurrent();
            SaveData();
        }

        private void bnavPlans_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void dgvPlans_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete())
                e.Cancel = true;
        }

        private void SetSaveButton(bool red)
        {
            bnavPlans.SetSaveButton(tsbSave, red);
        }

        public override bool SaveData()
        {
            if (!dgvPlans.EndEditX()) return false;
            if (!this.Validate()) return false;
            var ret = bsPlans.SaveDbContext();
            CheckSave();
            return ret.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsPlans.HasChanges());
        }

        private void dgvPlans_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void bsPlans_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        public void DoOpenList()
        {
            bsPlans.SetFilter<A_TIMESHEET>(x => (x.KIND1 == 0 || x.KIND1 == 1) && x.YR == PYear && x.MT == PMonth);

            bsPlans.AllowNew = true;
            dgvPlans.AllowUserToAddRows = true;

            CalendarMonth = new CalendarMonthInfo(PYear, PMonth);

            MakeColumnHeaders(CalendarMonth);
            dgvPlans.AutoResizeColumns();
        }

        public void MakePlans()
        {
            if (bsPlans.Count > 0)
            {
                MyMainForm.ShowWarning("Norādītajam laika periodam jau ir izveidoti plāna ieraksti.");
                return;
            }
            if(!DataLoaderA.IsMonthLoaded(PYear, PMonth))
            {
                MyMainForm.ShowWarning("Norādītajam laika periodam nav ielādēti dati.");
                return;
            }
            DataTasksA.MakePlans(CalendarMonth);
            SaveData();
            dgvPlans.AutoResizeColumns();
        }

        private void CheckPeriod()
        {
            int kyr = cbYR.SelectedIndex;
            int kmt = cbMT.SelectedIndex;
            if (kyr == -1 || kmt == -1) return;
            PYear = int.Parse(cbYR.Text);
            PMonth = int.Parse(cbMT.Text);
            if (DataLoaderA.IsMonthLoaded(PYear, PMonth))
            {
                dgvPlans.Enabled = true;
                bnavPlans.Enabled = true;
                DoOpenList();
            }
            else
            {
                MyMainForm.ShowWarning("Šim mēnesim dati nav ielādēdi.");
                dgvPlans.Enabled = false;
                bnavPlans.Enabled = false;
            }
        }

        private void cbYRMT_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckPeriod();
        }

        public void MakeColumnHeaders(CalendarMonthInfo calmt)
        {
            string[] daystr = SomeDataDefsA.GetDaysStr(calmt.WeekDays);
            int nr1 = dgcV1.Index, d1;
            string s;

            for (int i = nr1; i < nr1 + 31; i++)
            {
                var col = dgvPlans.Columns[i];
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
                    col.HeaderCell.Style = dgvPlans.ColumnHeadersDefaultCellStyle;
                }

            }
        }

        private void dgvPlans_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dgcKind1.Index)
            {
                if (e.Value == null || e.Value == DBNull.Value) return;
                if (!(e.Value is short)) return;
                EKind1 k = (EKind1)e.Value;
                string s;
                if (k == EKind1.PlanGroupDay || k == EKind1.PlanIndividualDay) s = "diena";
                else s = "nakts";
                e.Value = s;
                e.FormattingApplied = true;
                return;
            }
            if (e.ColumnIndex >= dgcV1.Index && e.ColumnIndex <= dgcV31.Index)
            {
                if (dgvPlans.Rows[e.RowIndex].IsNewRow) return;
                if (e.Value == null || e.Value == DBNull.Value) return;
                float v = (float)e.Value;

                int daynr = e.ColumnIndex - dgcV1.Index + 1;

                var dr = dgvPlans.GetObjectViewItem<A_TIMESHEET>(e.RowIndex);
                if (dr.XKind1 == EKind1.PlanGroupaNight)
                {
                    if (v == 0.0f)
                    {
                        e.Value = "";
                        e.FormattingApplied = true;
                    }
                    return;
                }

                EDayPlanId daycode = dr.DxPlan[daynr - 1];

                if (daycode == EDayPlanId.DD)
                {
                    return;
                }

                string daystr = SomeDataDefsA.GetPlanIdStr(daycode);

                if (daycode == EDayPlanId.BD || daycode == EDayPlanId.SD)
                {
                    if (v == 0.0f)
                        e.Value = daystr;
                    else
                        e.Value = $"{daystr} {v}";

                    e.FormattingApplied = true;
                    return;
                }

                e.Value = $"{daystr} {v}";

                e.FormattingApplied = true;
            }
        }

        private void dgvPlans_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.ColumnIndex >= dgcV1.Index && e.ColumnIndex <= dgcV31.Index)
            {
                if (e.Value == null || e.Value == DBNull.Value) return;

                if (e.Value is string)
                {
                    EDayPlanId daycode;
                    float hours;

                    if (!SomeDataDefsA.ParsePlanDayStr(e.Value as string, out daycode, out hours))
                    {
                        e.Value = 0.0f;
                        e.ParsingApplied = true;
                        return;
                    }

                    int daynr = e.ColumnIndex - dgcV1.Index + 1;

                    var dr = dgvPlans.GetObjectViewItem<A_TIMESHEET>(e.RowIndex);

                    EDayPlanId curdaycode = dr.DxPlan[daynr - 1];

                    if (daycode == EDayPlanId.None)
                        daycode = curdaycode;
                    if (daycode == EDayPlanId.BD || daycode == EDayPlanId.SD)
                        hours = 0.0f;

                    e.Value = hours;
                    e.ParsingApplied = true;

                    if (daycode != curdaycode)
                        dr.DxPlan[daynr - 1] = daycode;

                }
            }
        }

        private void dgvPlans_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[dgcYR.Index].Value = PYear;
            e.Row.Cells[dgcMT.Index].Value = PMonth;
        }

        private void cmsMenuMarkDay_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (bsPlans.Current == null || dgvPlans.CurrentCell == null || dgvPlans.CurrentRow.IsNewRow) return;
            var menuitems = new[] { miDD, miBD, miSD, miSDDD, miDDSD };
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
            var dr = bsPlans.GetCurrentItem<A_TIMESHEET>();

            EDayPlanId newdaycode = daycodes[k];
            int colnr = dgvPlans.CurrentCell.ColumnIndex;
            int daynr = colnr - dgcV1.Index + 1;
            if (dr.DxPlan[daynr - 1] != newdaycode)
            {
                dr.DxPlan[daynr - 1] = newdaycode;
                RefreshCell(dr, colnr);
                dgvPlans.AutoResizeColumn(colnr);
            }

        }

        private void dgvPlans_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1 || bsPlans.Current == null) return;
            if (dgvPlans.CurrentCell != null && dgvPlans.CurrentCell.IsInEditMode) return;

            var cell = dgvPlans[e.ColumnIndex, e.RowIndex];
            dgvPlans.CurrentCell = cell;
            if (dgvPlans.CurrentCell != null && dgvPlans.CurrentRow.IsNewRow) return;

            if (e.ColumnIndex >= dgcV1.Index && e.ColumnIndex <= dgcV31.Index)
            {
                var dr = bsPlans.GetCurrentItem<A_TIMESHEET>();
                if (dr.XKind1 != EKind1.PlanGroupDay) return;

                var bounds = dgvPlans.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                e.ContextMenuStrip = cmsMenuMarkDay;
            }
        }


        private void dgvPlans_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int curcol = dgvPlans.CurrentCell.ColumnIndex;
            if (curcol >= dgcV1.Index && curcol <= dgcV31.Index)
            {
                EDayPlanId daycode;
                float hours;
                if (!SomeDataDefsA.ParsePlanDayStr(e.Control.Text, out daycode, out hours)) return;
                e.Control.Text = hours.ToString();
            }
        }

        private void dgvPlans_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= dgcV1.Index && e.ColumnIndex <= dgcV31.Index)
            {
                dgvPlans.AutoResizeColumn(e.ColumnIndex);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.Cancel) return;
            dgvPlans.DataSource = null;
        }

        private void izveidotPlānaIerakstusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakePlans();
        }

        private void dgvPlans_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == dgcK1.Index)
                e.Cancel = true;
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            DeleteCurrent();
        }

        public void GetIDPFromDialog()
        {
            if (bsPlans.Count == 0 || bsPlans.Current == null) return;
            if (dgvPlans.CurrentRow == null || dgvPlans.CurrentRow.IsNewRow) return;
            var fm = new FormA_PlanList();
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return;
            if (dgvPlans.CurrentCell != null)
            {
                try
                {
                    var dr_pl = fm.SelectedObject;

                    dgvPlans.BeginEdit(false);
                    var c = dgvPlans.EditingControl as ComboBox;
                    c.SelectedValue = dr_pl;

                    dgvPlans.EndEdit();
                }
                catch (Exception) { }
            }
        }
        private void dgvPlans_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgcIDP.Index)
            {
                GetIDPFromDialog();
            }
        }

        private void dgvPlans_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvPlans.CurrentCell == null) return;
            if (e.KeyCode == Keys.F5)
            {
                if (dgvPlans.CurrentCell.ColumnIndex == dgcIDP.Index)
                {
                    GetIDPFromDialog();
                    e.Handled = true;
                    return;
                }
            }
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                DeleteCurrent();
                e.Handled = true;
            }
        }

        private void miFillRow_Click(object sender, EventArgs e)
        {
            if (!Validate()) return;
            if (CalendarMonth == null || bsPlans.Current == null) return;
            var dr = bsPlans.GetCurrentItem<A_TIMESHEET>();
            DataTasksA.FillPlans(CalendarMonth, dr);
            SaveData();
        }

        private void miFillList_Click(object sender, EventArgs e)
        {
            if (!Validate()) return;
            if (CalendarMonth == null || bsPlans.Count == 0) return;
            foreach(var drv in bsPlans)
            {
                var dr = (drv as ObjectView<A_TIMESHEET>)?.Object;
                if (dr == null) continue;
                DataTasksA.FillPlans(CalendarMonth, dr);
            }
            SaveData();
        }
    }
}
