using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using KlonsLIB.Misc;
using KlonsLIB.Data;
using KlonsF.Classes;
using Klons3.ModelsA;

namespace KlonsA.Classes
{
    // initialization has to be done as required by called functions

    public class SalarySheetRowInfo
    {
        public static KlonsData MyData => KlonsData.St;

        public SalarySheetRowSetInfo SalarySheetRowSet = null;

        public SalarySheetInfo SalarySheet = null;

        public A_SALARY_SHEET_TEMPL_R DR_salary_sheet_r_templ = null;
        public A_PERSONS_R DR_Person_r = null;

        public A_SALARY_SHEETS_R Row = null;

        public PeriodInfo PersonR = new PeriodInfo();
        public PeriodInfo PositionsR = new PeriodInfo();

        public SalaryInfo TotalPositionPay = null;

        public TimeSheetRowSet DLRows = null;

        public EventsInfo Events = null;

        public SalarySheetRowInfo()
        {
        }

        public SalarySheetRowInfo(SalarySheetInfo salarySheet)
        {
            SalarySheet = salarySheet;
        }

        public SalarySheetRowInfo(SalarySheetInfo salarySheet, A_SALARY_SHEETS_R dr_r)
        {
            SalarySheet = salarySheet;
            Row = dr_r;
        }

        public ErrorList SetUpFromRow(A_SALARY_SHEETS_R dr_r)
        {
            Row = dr_r;
            return SetUp(dr_r.Person, dr_r.Position);
        }

        public ErrorList SetUpFromRowX(A_SALARY_SHEETS_R dr_r)
        {
            Row = dr_r;
            var dr_sar = dr_r.SalarySheet;
            var ssh = new SalarySheetInfo(dr_sar);
            SalarySheet = ssh;
            var er = ssh.SetUpLightFromSarRow(dr_sar);
            if (er != "OK")
                return new ErrorList("", er);

            if(dr_r.XType == ESalarySheetRowType.Total)
                return SetUpT(dr_r.Person);

            if (dr_r.IDAM is null)
                throw new Exception("Bad row.");

            return SetUp(dr_r.Person, dr_r.Position);
        }

        public ErrorList SetUpFromSHRow(A_SALARY_SHEET_TEMPL_R dr_sh_r)
        {
            DR_salary_sheet_r_templ = dr_sh_r;
            return SetUp(dr_sh_r.Person, dr_sh_r.Position);
        }

        /*
        public void GetPersonRow(int idp)
        {
            var table_persons = MyData.DbContextA.BL_A_PERSONS;
            var dt1 = SalarySheet.MDT1;
            var dt2 = SalarySheet.MDT2;

            var dr_person = table_persons.FindById(idp);
            var drs_persons_r = dr_person.PersonsRRows.OrderBy(d => d.EDIT_DATE).ToArray();

            DR_Person_r = PeriodInfo.FindNearestBefore(drs_persons_r,
                dt2, d => d.EDIT_DATE);
        }
        */

        public ErrorList SetUp(A_PERSONS dr_person, A_POSITIONS dr_amats)
        {
            if (SalarySheet == null)
                throw new Exception("Bad init.");

            var table_sar = MyData.DbContextA.GetBindingListByType<A_SALARY_SHEETS>();
            var table_sar_r = MyData.DbContextA.GetBindingListByType<A_SALARY_SHEETS_R>();
            var table_darba_laiks = MyData.DbContextA.GetBindingListByType<A_TIMESHEET>();
            var table_sh = MyData.DbContextA.GetBindingListByType<A_SALARY_SHEET_TEMPL>();
            var table_sh_r = MyData.DbContextA.GetBindingListByType<A_SALARY_SHEET_TEMPL_R>();
            var table_persons = MyData.DbContextA.GetBindingListByType<A_PERSONS>();
            var table_amati = MyData.DbContextA.GetBindingListByType<A_POSITIONS>();
            var table_dl_sar_r = MyData.DbContextA.GetBindingListByType<A_TIMESHEET_LISTS_R>();
            

            var error_list = new ErrorList();
            string error_source;

            int yr = SalarySheet.YR;
            int mt = SalarySheet.MT;
            var dt1 = SalarySheet.MDT1;
            var dt2 = SalarySheet.MDT2;

            //var dv_dl_sar_r = new DataView(table_dl_sar_r);
            //dv_dl_sar_r.RowFilter = string.Format("Parent(FK_TIMESHEET_LISTS_R_IDS).YR = {0} AND Parent(FK_TIMESHEET_LISTS_R_IDS).MT = {1}", yr, mt);

            if (dr_person == null)
            {
                error_list.AddError("Algas lapa", "Nav darbinieka");
                return error_list;
            }

            if (dr_amats == null)
            {
                error_list.AddError("Algas lapa", "Nav amata");
                return error_list;
            }

            if (DR_salary_sheet_r_templ == null)
            {
                error_source = $"Darbinieks :{dr_person.FNAME} {dr_person.LNAME} ({dr_amats.TITLE})";
            }
            else
            {
                error_source = string.Format("Lapa:{0} rinda:{1}",
                    DR_salary_sheet_r_templ.SalarySheetTemplate.SNR,
                    DR_salary_sheet_r_templ.SNR);
            }

            var drs_persons_r = dr_person.PersonsStateRows.OrderBy(d => d.EDIT_DATE).ToArray();


            DR_Person_r = PeriodInfo.FindNearestBefore(drs_persons_r,
                dt2, d => d.EDIT_DATE);

            if (DR_Person_r == null)
            {
                error_list.AddError(error_source, "Norādītajam periodam nav ievadīti darbinieka dati.");
                return error_list;
            }

            PersonR.DateFirst = dt1;
            PersonR.DateLast = dt2;

            var drs_person_r = dr_person.PersonsStateRows.OrderBy(d => d.EDIT_DATE).ToArray();

            //var rete = PersonR.ReadDateListFilter(drs_person_r, d => d.EDIT_DATE);
            var rete = PersonR.ReadDateListAll(drs_person_r, d => d.EDIT_DATE);
            if (rete != PeriodInfo.ERetReadStartEndList.OK || PersonR.LinkedPeriods[0].DateFirst > dt2)
            {
                error_list.AddError(error_source, "Norādītajam periodam nav ievadīti darbinieka dati.");
                return error_list;
            }


            PositionsR.DateFirst = dt1;
            PositionsR.DateLast = dt2;

            var drs_amats_r = dr_amats.PositionStateRows.OrderBy(d => d.EDIT_DATE).ToArray();

            rete = PositionsR.ReadDateListFilter(drs_amats_r, d => d.EDIT_DATE);
            if (rete != PeriodInfo.ERetReadStartEndList.OK)
            {
                error_list.AddError(error_source, "Norādītajam periodam nav ievadīti amata dati.");
                return error_list;
            }

            var drs_dl_sar_r = dr_person.TimesheetListRows
                .Where(
                    d =>
                    d.Timesheet.YR == yr &&
                    d.Timesheet.MT == mt &&
                    d.Position == dr_amats)
                 .ToArray();

            if (drs_dl_sar_r.Length == 0)
            {
                error_list.AddError(error_source, "Norādītajam periodam nav darba laika uzskaites datu.");
                return error_list;
            }

            if (drs_dl_sar_r.Length > 1)
            {
                error_list.AddError(error_source, "Darba laika uzskaites datu rindu skaits > 1.");
                return error_list;
            }

            var dr_dl_sar_r = drs_dl_sar_r[0];

            var dlrowset = dr_dl_sar_r.GetDLRowSet();
            DLRows = dlrowset;
            SetDLRows(dlrowset);

            if (DLRows == null || DLRows.Plan == null || DLRows.Fact == null ||
                (dr_dl_sar_r.NIGHT == 1 && (DLRows.PlanNight == null || DLRows.FactNight == null)) ||
                (dr_dl_sar_r.OVERTIME == 1 && DLRows.FactOvertime == null))
            {
                error_list.AddError(error_source, "Nekorekti darba laika uzskaites dati.");
                return error_list;
            }

            if(Events == null)
            {
                if (!GetEventList(error_list, dr_person))
                {
                    return error_list;
                }
            }

            if(IsRateTypeChangedDuringVacation(drs_amats_r))
            {
                error_list.AddError(error_source, "Atvaļinājuma laikā nevar mainīt algas likmes veidu.");
            }

            var err1 = CheckVacationTimePlan(dr_amats);
            if(err1 != "OK")
            {
                error_list.AddError(error_source, err1);
                return error_list;
            }

            if(Row != null)
            {
                TotalPositionPay = new SalaryInfo();
                TotalPositionPay.SetFromRow(Row);
            }

            return error_list;
        }

        public ErrorList SetUpT(A_PERSONS dr_person)
        {
            if (SalarySheet == null)
                throw new Exception("Bad init.");

            var table_persons = MyData.DbContextA.BL_A_PERSONS;

            var error_list = new ErrorList();
            string error_source;

            int yr = SalarySheet.YR;
            int mt = SalarySheet.MT;
            var dt1 = SalarySheet.MDT1;
            var dt2 = SalarySheet.MDT2;

            if (dr_person == null)
            {
                error_list.AddError("Algas lapa", "Nav darbinieka");
                return error_list;
            }

            error_source = $"Darbinieks :{dr_person.FNAME} {dr_person.LNAME})";
            var drs_persons_r = dr_person.PersonsStateRows.OrderBy(d => d.EDIT_DATE).ToArray();


            DR_Person_r = PeriodInfo.FindNearestBefore(drs_persons_r,
                dt2, d => d.EDIT_DATE);

            if (DR_Person_r == null)
            {
                error_list.AddError(error_source, "Norādītajam periodam nav ievadīti darbinieka dati.");
                return error_list;
            }

            PersonR.DateFirst = dt1;
            PersonR.DateLast = dt2;

            var drs_person_r = dr_person.PersonsStateRows.OrderBy(d => d.EDIT_DATE).ToArray();

            //var rete = PersonR.ReadDateListFilter(drs_person_r, d => d.EDIT_DATE);
            var rete = PersonR.ReadDateListAll(drs_person_r, d => d.EDIT_DATE);
            if (rete != PeriodInfo.ERetReadStartEndList.OK || PersonR.LinkedPeriods[0].DateFirst > dt2)
            {
                error_list.AddError(error_source, "Norādītajam periodam nav ievadīti darbinieka dati.");
                return error_list;
            }

            if (Events == null)
            {
                if (!GetEventList(error_list, dr_person))
                {
                    return error_list;
                }
            }

            if (Row != null)
            {
                TotalPositionPay = new SalaryInfo();
                TotalPositionPay.SetFromRow(Row);
            }

            return error_list;
        }

        //summētais <= => akordalga
        public IEnumerable<DateTime> GetPositionRateTypeChangeList(A_POSITIONS_R[] drs_amats_r)
        {
            /*
            var table_amati = MyData.DbContextA.POSITIONS;
            int idam = Row.IDAM;
            var dr_amats = table_amati.FindByID(idam);
            var drs_amats_r = dr_amats.GetPOSITIONS_RRows().OrderBy(d => d.EDIT_DATE).ToArray();
            */
            if (drs_amats_r.Length <= 1) yield break;
            var tp1 = drs_amats_r[0].XRateType;
            bool b1 = tp1 == ESalaryType.Aggregated;
            for (int i = 1; i < drs_amats_r.Length; i++)
            {
                var tp2 = drs_amats_r[i].XRateType;
                bool b2 = tp2 == ESalaryType.Aggregated;
                if (b1 == b2) continue;
                var dt = drs_amats_r[i].EDIT_DATE;
                yield return dt;
                tp1 = tp2;
                b1 = b2;
            }
        }

        public bool IsRateTypeChangedDuringVacation(A_POSITIONS_R[] drs_amats_r)
        {
            if (Events.Vacations.LinkedPeriods.Count == 0) return false;
            var dts = GetPositionRateTypeChangeList(drs_amats_r);
            foreach (var dt in dts)
                foreach (var lp in Events.Vacations.LinkedPeriods)
                    if (lp.ContainsDate(dt)) return true;
            return false;
        }

        public bool GetLastHireDate(DateTime dt, out DateTime hdt)
        {
            hdt = DateTime.MinValue;
            if (Events == null)
                throw new Exception("Bad call");
            var pi = Events.HireFire.LinkedPeriods.Where(p => p.ContainsDate(dt)).FirstOrDefault();
            if (pi == null) return false;
            hdt = pi.DateFirst;
            return true;
        }

        public TimeSheetRowSet GetDLRowSet(int addmt, A_POSITIONS dr_amats)
        {
            if (addmt == 0) return DLRows;
            int yr = SalarySheet.YR;
            int mt = SalarySheet.MT;
            Utils.AddMonths(ref yr, ref mt, addmt);
            return GetDLRowSet(yr, mt, dr_amats);
        }

        public TimeSheetRowSetList GetDLRowSetList(int addmt = 0)
        {
            if (IsSingleRow())
            {
                var ret = new TimeSheetRowSetList();
                if (addmt == 0)
                {
                    ret.Add(DLRows);
                }
                else
                {
                    int yr = SalarySheet.YR;
                    int mt = SalarySheet.MT;
                    Utils.AddMonths(ref yr, ref mt, addmt);
                    ret.Add(GetDLRowSet(yr, mt, Row.Position));
                }
                return ret;
            }
            else
            {
                return SalarySheetRowSet?.GetDLRowSetList(addmt);
            }
        }

        public TimeSheetRowSet GetDLRowSet(int yr, int mt, A_POSITIONS dr_amats)
        {
            var drs_dl_sar_r = DR_Person_r.Person.TimesheetListRows
                .Where(
                    d =>
                    d.Timesheet.YR == yr &&
                    d.Timesheet.MT == mt &&
                    d.Position == dr_amats)
                 .ToArray();
            if (drs_dl_sar_r.Length == 0) return null;
            return drs_dl_sar_r[0].GetDLRowSet();
        }


        public SalaryInfo GetPrevRow()
        {
            if (Row.XType != ESalarySheetRowType.Total) return GetPrevRowA();

            var dt1 = SalarySheet.DT1;
            var dt2 = SalarySheet.DT2;
            var dtp1 = dt2.FirstDayOfMonth().AddMonths(-1);
            var dtp2 = dtp1.LastDayOfMonth();
            var table_lapas_r = MyData.DbContextA.GetBindingListByType<A_SALARY_SHEETS_R>();

            var dr_lapas_r = table_lapas_r.Where(
                d =>
                d != Row &&
                d.Person == Row.Person &&
                d.Position == null &&
                d.SalarySheet.DT2 >= dtp1 &&
                d.SalarySheet.DT2 <= dtp2 &&
                d.SalarySheet.IS_TEMP == 0 &&
                d.SalarySheet.XKind == ESalarySheetKind.Total
            ).WithMaxOrDefault(d => d.SalarySheet.DT2);

            if (dr_lapas_r == null) return null;

            var ret = new SalaryInfo();
            ret.SetFromRow(dr_lapas_r);
            return ret;
        }

        private SalaryInfo GetPrevRowA()
        {
            var dt1 = SalarySheet.DT1;
            var dt2 = SalarySheet.DT2;
            var dtp1 = dt2.FirstDayOfMonth().AddMonths(-1);
            var dtp2 = dtp1.LastDayOfMonth();
            var table_lapas_r = MyData.DbContextA.GetBindingListByType<A_SALARY_SHEETS_R>();

            var dr_lapas_r = table_lapas_r.Where(
                d =>
                d != Row &&
                d.Person == Row.Person &&
                d.Position == Row.Position &&
                d.SalarySheet.DT2.Value.IsBetween(dtp1, dtp2) &&
                d.SalarySheet.IS_TEMP == 0 &&
                d.SalarySheet.XKind != ESalarySheetKind.Total
            ).WithMaxOrDefault(d => d.SalarySheet.DT2);

            if (dr_lapas_r == null) return null;

            var dtm = dr_lapas_r.SalarySheet.DT2;

            var ret = new SalaryInfo();
            ret.SetFromRow(dr_lapas_r);
            return ret;
        }

        public bool GetRow(int addmt, out SalarySheetRowInfo row)
        {
            row = null;
            var table_sar = MyData.DbContextA.GetBindingListByType<A_SALARY_SHEETS>();
            var table_sar_r = MyData.DbContextA.GetBindingListByType<A_SALARY_SHEETS_R>();

            int yr = SalarySheet.YR;
            int mt = SalarySheet.MT;

            Utils.AddMonths(ref yr, ref mt, addmt);

            var drs = table_sar_r.Where(
                d =>
                {
                    var dr_sar = d.SalarySheet;
                    return
                        d.Person == Row.Person &&
                        d.Position == Row.Position &&
                        dr_sar.YR == yr &&
                        dr_sar.MT == mt;
                }).ToArray();

            if (drs.Length == 0) return true;
            var dr = drs[0];

            var shr = new SalarySheetRowInfo();
            var err = shr.SetUpFromRowX(dr);
            if (err != null && err.Count > 0)
            {
                return false;
            }

            row = shr;

            return true;
        }

        public ErrorList CheckLinkedRows(bool force = false)
        {
            if (SalarySheetRowSet != null && !force) return new ErrorList();
            SalarySheetRowSet = new SalarySheetRowSetInfo();
            return SalarySheetRowSet.SetUpFromRowA(this);
        }

        public void MakeRowFromSH()
        {
            if (DR_salary_sheet_r_templ == null)
                throw new Exception("Shablon row not set.");
            MakeRow(DR_salary_sheet_r_templ.Person, 
                DR_salary_sheet_r_templ.Position, 
                DR_salary_sheet_r_templ.SNR);
        }

        public string GetPositionTitle()
        {
            if (Row == null) return null;
            if (Row.XType == ESalarySheetRowType.Total) return "KOPĀ";
            if (Row.IDAM == null) return null;
            var dr_amats = MyData.DbContextA.A_POSITIONS.Find(Row.IDAM);
            if (dr_amats == null) return null;
            return dr_amats.TITLE;
        }

        public void MakeRow(A_PERSONS dr_person, A_POSITIONS dr_position, int snr)
        {
            var table_sar_r = MyData.DbContextA.BL_A_SALARY_SHEETS_R;
            var new_dr_sar_r = table_sar_r.NewItem();

            //new_dr_sar_r.ID = (int)MyData.KlonsAQueriesTableAdapter.SP_A_GEN_SALARY_SHEETS_R_ID();
            new_dr_sar_r.RowSetTotalRow = new_dr_sar_r;
            new_dr_sar_r.SalarySheet = SalarySheet.DR_algas_lapa;
            new_dr_sar_r.Person = dr_person;
            new_dr_sar_r.Position = dr_position;
            new_dr_sar_r.SNR = (short)snr;
            new_dr_sar_r.FNAME = DR_Person_r.FNAME;
            new_dr_sar_r.LNAME = DR_Person_r.LNAME;
            new_dr_sar_r.IS_TEMP = SalarySheet.DR_algas_lapa.IS_TEMP;

            var s1 = dr_position.TITLE;
            if (s1 != null)
                new_dr_sar_r.POSITION_TITLE = s1;


            table_sar_r.Add(new_dr_sar_r);

            Row = new_dr_sar_r;

            CheckLinkedRows();
            CheckAlgasPS();

        }


        private void SetIntValue(ref int rec, int src)
        {
            if (rec != src) rec = src;
        }


        public A_POSITIONS_PLUSMINUS[] GetRelevantPersonPSRows()
        {
            var dt1 = SalarySheet.MDT1;
            var dt2 = SalarySheet.MDT2;

            var drs_amps = DR_Person_r.Person.PositionsPlusMinusRows.Where(
                d =>
                d.DATE1 <= dt2 &&
                d.DATE2 >= dt1
                ).ToArray();

            return drs_amps;
        }

        public A_SALARY_SHEETS_R GetTotalRow()
        {
            if (Row.RowSetTotalRow == null) return null;
            if (IsSingleRow()) return Row;
            return Row.RowSetTotalRow;
        }

        public A_SALARY_PLUSMINUS[] GetAlgasAllPSRows()
        {
            var totalrow = GetTotalRow();
            if (totalrow != null)
            {
                var drs = totalrow.SalaryPlusMinusRows
                    .Where(_d => true)
                    .OrderBy(_d => _d.ID)
                    .ToArray();
                return drs;
            }

            var drs2 = Row.SalaryPlusMinusRows
                .Where(_d => true)
                .OrderBy(_d => _d.ID)
                .ToArray();
            return drs2;
        }

        public A_SALARY_PLUSMINUS[] GetAlgasPSRowsT()
        {
            return GetAlgasAllPSRows().Where(
                d => d.IDA == null
                ).ToArray();
        }

        public A_SALARY_PLUSMINUS[] GetAlgasPSRows(int ida)
        {
            return GetAlgasAllPSRows().Where(
                d => 
                d.IDA != null &&
                d.IDA == ida
                ).ToArray();
        }

        public A_SALARY_PLUSMINUS[] GetAlgasPSRowsX(A_POSITIONS dr_position)
        {
            return GetAlgasAllPSRows().Where(
                d =>
                d.Position == null || 
                d.Position == dr_position
                ).ToArray();
        }

        public void CheckAlgasPS()
        {
            var table_algas_papildsummas = MyData.DbContextA.BL_A_SALARY_PLUSMINUS;
            var dt1 = SalarySheet.MDT1;
            var dt2 = SalarySheet.MDT2;

            var drs_amps = GetRelevantPersonPSRows();
            var drs_algasps = GetAlgasAllPSRows();

            var del_rows = new List<A_SALARY_PLUSMINUS>();

            // delete rows without template
            foreach(var dr in drs_algasps)
            {
                if (dr.PositionsPlusMinusRow == null) continue;
                var dr2 = drs_amps.Where(
                    d =>
                    d == dr.PositionsPlusMinusRow
                    ).FirstOrDefault();
                if(dr2 == null)
                    del_rows.Add(dr);
            }
            foreach (var dr in del_rows)
                table_algas_papildsummas.Remove(dr);

            // add rows from template
            // and update rows
            foreach (var dr in drs_amps)
            {
                var dr2 = drs_algasps.Where(
                    d =>
                    d.PositionsPlusMinusRow != null &&
                    d.PositionsPlusMinusRow == dr
                    ).FirstOrDefault();
                if (dr2 == null)
                {
                    dr2 = table_algas_papildsummas.NewItem();
                    table_algas_papildsummas.Add(dr2);
                    dr2.PositionsPlusMinusRow = dr;
                    dr2.SalarySheetRow = Row.RowSetTotalRow;
                    dr2.Person = Row.Person;
                }

                if (dr2.Position != dr.Position) dr2.Position = dr.Position;
                if (dr2.PlusMinusType != dr.PlusMinusType) dr2.PlusMinusType = dr.PlusMinusType;
                if (dr2.PlusMinusFrom != dr.PlusMinusFromRow) dr2.PlusMinusFrom = dr.PlusMinusFromRow;
                if (dr2.DESCR != dr.DESCR) dr2.DESCR = dr.DESCR;
                if (dr2.RATE != dr.RATE) dr2.RATE = dr.RATE;
                if (dr2.RATE_TYPE != dr.RATE_TYPE) dr2.RATE_TYPE = dr.RATE_TYPE;
                if (dr2.IS_PAID != dr.IS_PAID) dr2.IS_PAID = dr.IS_PAID;
                if (dr2.IS_INAVPAY != dr.IS_INAVPAY) dr2.IS_INAVPAY = dr.IS_INAVPAY;
            }
        }

        public void ChangeIDSXForAlgasPSRows(int new_idsx)
        {
            var drs_ps = Row.SalaryPlusMinusRows;
            foreach(var dr in drs_ps)
            {
                dr.IDSX = new_idsx;
            }
        }

        public void DeleteAlgasPSRows()
        {
            var drs_ps = Row.SalaryPlusMinusRows.ToList();
            foreach (var dr in drs_ps)
            {
                MyData.DbContextA.A_SALARY_PLUSMINUS.Remove(dr);
            }
        }

        public void SetDLRows(TimeSheetRowSet dlrowset)
        {
            DLRows = dlrowset;
        }

        public bool GetEventList(ErrorList errorlist, A_PERSONS dr_person)
        {
            Events = new EventsInfo();
            string ret = Events.ProcessData(dr_person, SalarySheet.DT1, SalarySheet.DT2);
            if (ret == "OK") return true;
            errorlist.AddPersonError(dr_person, ret);
            return false;
        }

        public int GetLinkedRowsCount()
        {
            if (SalarySheetRowSet == null || SalarySheetRowSet.DrLinkedRows == null)
                return 1;
            return SalarySheetRowSet.DrLinkedRows.Length;
        }

        public bool IsSingleRow()
        {
            return GetLinkedRowsCount() == 1;
        }

        public ErrorList FillRowX()
        {
            var err = CheckLinkedRows();
            if (err.HasErrors) return err;
            err = SalarySheetRowSet.FillRow();
            return err;
        }

        public int CountCalendarDays()
        {
            return this.Events.HireFire.CountDates(SalarySheet.DT1, SalarySheet.DT2);
        }

        public string CheckVacationTimePlan(A_POSITIONS dr_amats)
        {
            var dt1 = SalarySheet.DT1;
            var dt2 = SalarySheet.DT2;
            var mdt1 = SalarySheet.MDT1;
            var mdt2 = SalarySheet.MDT2;

            var ps = Events.Vacations.LinkedPeriods.Where(
                d =>
                {
                    if (!SomeDataDefsA.IsEventPaidVacation(d.EEventId)) return false;

                    var dr_not = (d.Item1 as A_EVENTS);
                    if (dr_not.DATE3 == null) return false;

                    return (d.DateFirst <= dt2 && d.DateLast >= dt1) ||
                        (dr_not.DATE3 >= dt1 && dr_not.DATE3 <= dt2);
                }).ToArray();


            if (ps.Length == 0) return "OK";

            var ps_next = ps.Where(
                d =>
                {
                    var dr_not = (d.Item1 as A_EVENTS);
                    if (dr_not.DATE3 == null) return false;
                    return (d.DateLast > dt2) &&
                        (dr_not.DATE3 >= dt1 && dr_not.DATE3 <= dt2);
                }).ToArray();


            foreach (var pi in ps_next)
            {
                var dtp1 = dt2.AddDays(1);
                var dtp2 = pi.DateLast;
                if (dtp1 < pi.DateFirst) dtp1 = pi.DateFirst;
                var dtpe = dtp1.LastDayOfMonth();
                if (dtp2 > dtpe) dtp2 = dtpe;

                while (true)
                {
                    int addmt = Utils.MonthDiff(dt1, dtp1);

                    var nextdlrowset = GetDLRowSet(addmt, dr_amats);
                    if (nextdlrowset == null || nextdlrowset.Plan == null)
                        return "Atvaļinājumam nav izveidots darba laika plāns.";

                    if (dtp2 == pi.DateLast) break;

                    dtp1 = dtpe.AddDays(1);
                    dtp2 = pi.DateLast;
                    dtpe = dtp1.LastDayOfMonth();
                    if (dtp2 > dtpe) dtp2 = dtpe;
                }

            }

            return "OK";
        }


        public TimeSheetRowSet[] GetAllDLRowSet()
        {
            if (SalarySheet == null || Row == null)
                throw new Exception("Bad initialization.");
            return Row.Person.TimesheetListRows.Where(d =>
            {
                var dr_sar = d.Timesheet;
                return dr_sar.YR == SalarySheet.YR &&
                    dr_sar.MT == SalarySheet.MT;
            }).Select(d => d.GetDLRowSet())
            .ToArray();
        }

    }

}
