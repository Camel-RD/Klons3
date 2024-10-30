using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using KlonsLIB.Misc;
using KlonsLIB.Data;
using DataObjectsA;
using KlonsF.Classes;
using System.ComponentModel;
using Klons3.ModelsA;

namespace KlonsA.Classes;

public static class DataTasksA
{
    public static KlonsData MyData => KlonsData.St;

    public static A_PERSONS FindById(this BindingList<A_PERSONS> list, int id) =>
        MyData.DbContextA.A_PERSONS.Find(id);
    public static A_TIMEPLAN_LIST FindById(this BindingList<A_TIMEPLAN_LIST> list, int id) =>
        MyData.DbContextA.A_TIMEPLAN_LIST.Find(id);
    public static A_TIMESHEET_TEMPL FindById(this BindingList<A_TIMESHEET_TEMPL> list, int id) =>
        MyData.DbContextA.A_TIMESHEET_TEMPL.Find(id);
    public static A_SALARY_SHEET_TEMPL FindById(this BindingList<A_SALARY_SHEET_TEMPL> list, int id) =>
        MyData.DbContextA.A_SALARY_SHEET_TEMPL.Find(id);
    public static A_SALARY_SHEETS_R FindById(this BindingList<A_SALARY_SHEETS_R> list, int id) =>
        MyData.DbContextA.A_SALARY_SHEETS_R.Find(id);
    public static A_SALARY_SHEETS FindById(this BindingList<A_SALARY_SHEETS> list, int id) =>
        MyData.DbContextA.A_SALARY_SHEETS.Find(id);
    public static A_TIMESHEET_LISTS FindById(this BindingList<A_TIMESHEET_LISTS> list, int id) =>
        MyData.DbContextA.A_TIMESHEET_LISTS.Find(id);
    public static A_TIMESHEET_LISTS_R FindById(this BindingList<A_TIMESHEET_LISTS_R> list, int id) =>
        MyData.DbContextA.A_TIMESHEET_LISTS_R.Find(id);
    public static A_POSITIONS FindById(this BindingList<A_POSITIONS> list, int id) =>
        MyData.DbContextA.A_POSITIONS.Find(id);
    public static A_PAYLIST_TEMPL FindById(this BindingList<A_PAYLIST_TEMPL> list, int id) =>
        MyData.DbContextA.A_PAYLIST_TEMPL.Find(id);
    public static A_PAYLISTS FindById(this BindingList<A_PAYLISTS> list, int id) =>
        MyData.DbContextA.A_PAYLISTS.Find(id);
    public static A_PAYLISTS_R FindById(this BindingList<A_PAYLISTS_R> list, int id) =>
        MyData.DbContextA.A_PAYLISTS_R.Find(id);
    public static A_EVENT_TYPES FindById(this BindingList<A_EVENT_TYPES> list, int id) =>
        MyData.DbContextA.A_EVENT_TYPES.Find(id);
    public static A_EVENTS FindById(this BindingList<A_EVENTS> list, int id) =>
        MyData.DbContextA.A_EVENTS.Find(id);

    public static A_PAYSALDO FindByIDAM(this List<A_PAYSALDO> list, int idam) =>
        list.Find(x => x.IDAM == idam);
    public static A_SP_PAY_MATCHLISTS_1X FindByIDAM(this List<A_SP_PAY_MATCHLISTS_1X> list, int idam) =>
        list.Find(x => x.IDAM == idam);

    public static void Delete(this A_TIMESHEET row)
    {
        var rt = MyData.DbContextA.BL_A_TIMESHEET.Remove(row);
        if (!rt) throw new InvalidOperationException("Delete row failed, row not found");
    }
    public static void Delete(this A_TIMESHEET_LISTS_R row)
    {
        var rt = MyData.DbContextA.BL_A_TIMESHEET_LISTS_R.Remove(row);
        if (!rt) throw new InvalidOperationException("Delete row failed, row not found");
    }

    public static string[] GetPersonNameAndPK(A_PERSONS dr_person)
    {
        if (dr_person == null) return null;
        return new[] { dr_person.FNAME, dr_person.LNAME, dr_person.PK };
    }

    public static string GetPositionTitle(int idam)
    {
        var dr_am = MyData.DbContextA.A_POSITIONS.Find(idam);
        if (dr_am == null) return null;
        return dr_am.TITLE;
    }

    public static string GetPersonName(A_PERSONS dr_person)
    {
        if (dr_person == null) return null;
        return $"{dr_person.FNAME} {dr_person.LNAME}";
    }

    public static string[] GetPersonNameAndPK(int idp)
    {
        var dr_p = MyData.DbContextA.A_PERSONS.Find(idp);
        if (dr_p == null) return null;
        return new[] { dr_p.FNAME, dr_p.LNAME, dr_p.PK };
    }

    public static string[] GetFPPersonNameAndPK(int idp)
    {
        var dr_p = MyData.DbContextA.A_PERSONS_FIZ.Find(idp);
        if (dr_p == null) return null;
        return new[] { dr_p.FNAME, dr_p.LNAME, dr_p.PK };
    }

    public static A_POSITIONS GetPersonsUsedPosition(A_PERSONS dr_person)
    {
        if (dr_person == null) return null;
        var dr_pos = dr_person.Positions
            .Where(x => x.USED == 1)
            .FirstOrDefault();
        if (dr_pos != null)
            return dr_pos;
        return dr_person.Positions[0];
    }


    public static A_RATES GetRates(DateTime dt1)
    {
        return MyData.DbContextA.BL_A_RATES
            .Where(d => d.ONDATE <= dt1)
            .WithMaxOrDefault(d => d.ONDATE);
    }

    public static A_PERSONS_R GetPersonsR(A_PERSONS dr_p, DateTime dt1)
    {
        if (dr_p == null) return null;
        return dr_p.PersonsStateRows
            .Where(d => d.EDIT_DATE <= dt1)
            .WithMaxOrDefault(d => d.EDIT_DATE);
    }

    public static A_PERSONS_R GetPersonsR(int idp, DateTime dt1)
    {
        var dr = MyData.DbContextA.A_PERSONS.Find(idp);
        return GetPersonsR(dr, dt1);
    }

    public static bool IsPersonWorking(A_PERSONS dr_person, DateTime dt1, DateTime dt2)
    {
        if (dr_person == null) return false;
        var drs_events = dr_person.Events;
        var drsn_hirefire = drs_events.Where(d =>
        {
            return
                Utils.IN(d.IDN, (int)EEventId.Pieņemts, (int)EEventId.Atlaists);
        }).OrderBy(d => d.DATE1).ToArray();

        if (drsn_hirefire.Length == 0)
            return false;

        var pi_hirefire = new PeriodInfo();

        var rt1 = pi_hirefire.ReadStartEndList(drsn_hirefire,
            isStartItem: it => it.IDN == (int)EEventId.Pieņemts,
            getItemDate: it => it.DATE1);

        if (rt1 != PeriodInfo.ERetReadStartEndList.OK) return false;

        var pi_f = pi_hirefire.FilterListWithDates(dt1, dt2);
        return pi_f.LinkedPeriods.Count > 0;
    }

    //vai ir nodokļu gr uz perioda 1. datumu vai darbā stāšanās datumu
    public static bool IsPersonWithTaxDoc(A_PERSONS dr_person, DateTime dt1, DateTime dt2)
    {
        if (dr_person == null) return false;
        var drs_events = dr_person.Events;
        var drsn_hirefire = drs_events.Where(d =>
        {
            return
                Utils.IN(d.IDN, (int)EEventId.Pieņemts, (int)EEventId.Atlaists);
        }).OrderBy(d => d.DATE1).ToArray();

        if (drsn_hirefire.Length == 0)
            return false;

        var pi_hirefire = new PeriodInfo();

        var rt1 = pi_hirefire.ReadStartEndList(drsn_hirefire,
            isStartItem: it => it.IDN == (int)EEventId.Pieņemts,
            getItemDate: it => it.DATE1);

        if (rt1 != PeriodInfo.ERetReadStartEndList.OK) return false;

        var pi_f = pi_hirefire.FilterListWithDates(dt1, dt2);
        if(pi_f.LinkedPeriods.Count == 0) return false;

        var pi1 = pi_f.LinkedPeriods[0];
        var dtx = pi1.DateFirst;
        var dr_porsonR =
            dr_person.PersonsStateRows
            .Where(d => d.EDIT_DATE <= dtx)
            .WithMaxOrDefault(d => d.EDIT_DATE);
        if (dr_porsonR == null) return false;
        return !string.IsNullOrEmpty(dr_porsonR.TAXDOC_NO);
    }

    public static short GetNextSalarySheetNr(int yr)
    {
        var table_sar = MyData.DbContextA.BL_A_SALARY_SHEETS;
        var e1 = table_sar.Where(d => d.YR == yr);
        if (e1.FirstOrDefault() == null) return 1;
        return (short)(e1.Max(d => d.SNR) + 1);
    }

    public static List<A_POSITIONS> CheckSalarySheetsTemplUsed(int yr, int mt, A_SALARY_SHEET_TEMPL[] drs_sh)
    {
        var ret = new List<A_POSITIONS>();
        var table_sar = MyData.DbContextA.BL_A_SALARY_SHEETS;
        var table_sar_r = MyData.DbContextA.BL_A_SALARY_SHEETS_R;
        var dt1 = new DateTime(yr, mt, 1);
        var dt2 = dt1.LastDayOfMonth();
        var drs_tsr = table_sar.Where(
            d =>
            d.XKind == ESalarySheetKind.Normal &&
            d.DT2 == dt2
            ).SelectMany(d => d.SalarySheetRows)
            .ToArray();
        foreach (var dr_sh in drs_sh)
        {
            var drs_sh_r = dr_sh.SalarySheetTemplateRows;
            foreach (var dr_sh_r in drs_sh_r)
            {
                var drs_cur = drs_tsr.Where(
                    d =>
                    d.Person == dr_sh_r.Person &&
                    d.Position == dr_sh_r.Position);
                if (drs_cur.Any())
                    if (!ret.Contains(dr_sh_r.Position))
                        ret.Add(dr_sh_r.Position);
            }
        }
        return ret;
    }

    public static ErrorList MakeSalarySheetsFromTemplate(int yr, int mt, A_SALARY_SHEET_TEMPL dr_sht)
    {
        var table_sar = MyData.DbContextA.BL_A_SALARY_SHEETS;
        var table_sar_r = MyData.DbContextA.BL_A_SALARY_SHEETS_R;
        var table_sh = MyData.DbContextA.BL_A_SALARY_SHEET_TEMPL;
        var table_sh_r = MyData.DbContextA.BL_A_SALARY_SHEET_TEMPL_R;

        var error_list = new ErrorList();

        var this_error_source = $"Algu apr. {yr}.{mt:00}";

        if (table_sh.Count == 0 || table_sh_r.Count == 0)
        {
            error_list.AddError(this_error_source, "Nav izveidotas lapu saraksta sagataves.");
            return error_list;
        }

        A_SALARY_SHEET_TEMPL[] drs_sh;
        if (dr_sht != null)
            drs_sh = [dr_sht];
        else
            drs_sh = table_sh.OrderBy(d => d.SNR).ToArray();

        if (drs_sh.Length == 0 || drs_sh[0] == null)
        {
            error_list.AddError(this_error_source, "Nav atrastas lapu saraksta sagataves.");
            return error_list;
        }

        var dt2 = (new DateTime(yr, mt, 1)).LastDayOfMonth();

        var drs_positions = CheckSalarySheetsTemplUsed(yr, mt, drs_sh);
        if (drs_positions.Count > 0)
        {
            var drp = drs_positions[0].Person;
            string ser = "";
            if (drs_positions.Count == 1)
                ser = $"Algas aprēķins priekš darbinieka {drp.YNAME} ir jau izveidots.";
            else
                ser = $"Algas aprēķins priekš darbinieka {drp.YNAME} \nun vēl {drs_positions.Count - 1} darbiniekiem ir jau izveidots.";
            error_list.AddError(this_error_source, ser);
            return error_list;
        }


        var salary_sheet_list = new List<SalarySheetInfo>();
        var m_salary_sheet = new SalarySheetInfo(yr, mt);

        var ret = m_salary_sheet.GetLikmes();

        if (ret != "OK")
        {
            error_list.AddError(this_error_source, ret);
            return error_list;
        }


        foreach (var dr_sh in drs_sh)
        {
            var salary_sheet = new SalarySheetInfo(m_salary_sheet);
            var err = salary_sheet.SetUpFromSHRow(dr_sh);

            salary_sheet_list.Add(salary_sheet);

            if (err.Count > 0)
            {
                error_list.AddRange(err);
                continue;
            }
        }

        if (error_list.Count > 0)
            return error_list;

        short snr0 = GetNextSalarySheetNr(yr);

        foreach (var salary_sheet in salary_sheet_list)
        {
            salary_sheet.MakeSheetFromSh(snr0);
            var err = salary_sheet.FillSheet();
            error_list.AddRange(err);
            snr0++;
        }

        return error_list;
    }

    public static ErrorList MakeSalarySheetRow(A_SALARY_SHEETS dr_lapa, 
        A_PERSONS dr_person, A_POSITIONS dr_position, int snr)
    {
        var error_list = new ErrorList();
        var this_error_source = "Algu apr.";

        var salary_sheet = new SalarySheetInfo(dr_lapa);
        var ret = salary_sheet.GetLikmes();
        if (ret != "OK")
        {
            error_list.AddError(this_error_source, ret);
            return error_list;
        }


        var salary_sheet_row = new SalarySheetRowInfo(salary_sheet);
        var err = salary_sheet_row.SetUp(dr_person, dr_position);

        if (err.Count > 0)
        {
            error_list.AddRange(err);
            return error_list;
        }

        salary_sheet.CheckTotalListForPeriod();
        salary_sheet_row.MakeRow(dr_person, dr_position, snr);

        err = salary_sheet_row.FillRowX();

        error_list.AddRange(err);

        return error_list;
    }

    public static void DeleteSalarySheetRow(A_SALARY_SHEETS_R dr_lapas_r)
    {
        var salary_sheet = new SalarySheetInfo(dr_lapas_r.SalarySheet);
        salary_sheet.DeleteRow(dr_lapas_r);
    }


    public static void DeleteSalarySheet(A_SALARY_SHEETS dr_lapa)
    {
        var salary_sheet = new SalarySheetInfo(dr_lapa);
        salary_sheet.DeleteList();
    }

    public static ErrorList CalcSalarySheetRow(A_SALARY_SHEETS_R dr_sar_r)
    {
        var error_list = new ErrorList();

        if(dr_sar_r == null)
        {
            error_list.AddError("", "Nav algas aprēķina.");
            return error_list;
        }

        var sshr = new SalarySheetRowSetInfo();
        var err = sshr.SetUpFromRowX(dr_sar_r);

        if (err.Count > 0)
        {
            error_list += err;
            return error_list;

        }

        err = sshr.FillRow();
        if (err.Count > 0)
        {
            error_list += err;
            return error_list;

        }

        return error_list;
    }

    public static ErrorList CalcSalarySheet(A_SALARY_SHEETS dr_sar)
    {
        var error_list = new ErrorList();

        if (dr_sar == null)
        {
            error_list.AddError("", "Nav algas aprēķina.");
            return error_list;
        }

        var ssh = new SalarySheetInfo(dr_sar);
        var er = ssh.SetUpLightFromSarRow(dr_sar);
        if(er != "OK")
        {
            error_list.AddError("", er);
            return error_list;
        }

        var drs_sar_r = dr_sar.SalarySheetRows;

        foreach (var dr_sar_r in drs_sar_r)
        {
            var sshr = new SalarySheetRowSetInfo();
            var err = sshr.SetUpFromRowB(ssh, dr_sar_r.Person);
            error_list += err;
            if (err.Count > 0) continue;
            error_list += sshr.FillRow();
        }

        return error_list;
    }

    public static string CheckPlans(int yr, int mt)
    {
        var table_plans = MyData.DbContextA.BL_A_TIMEPLAN_LIST;
        var table_sh_r = MyData.DbContextA.BL_A_TIMESHEET_TEMPL_R;
        var row_list_pl = new List<A_TIMEPLAN_LIST>();
        foreach (var dr in table_sh_r)
        {
            
            var dr_pl = dr.TimePlan;
            if (row_list_pl.Contains(dr_pl)) continue;
            row_list_pl.Add(dr_pl);
        }
        foreach (var dr in row_list_pl)
        {
            var ret = CheckPlan(dr, yr, mt);
            if (ret != "OK")
                return $"Nav izveidots plāns [{dr.DESCR}]";
        }
        return "OK";
    }

    public static string CheckPlan(A_TIMEPLAN_LIST dr_pl,
        int yr, int mt)
    {
        var drs_dl_pl = dr_pl.TimesheetRows.Where(
            d=>
            d.YR == yr &&
            d.MT == mt
            ).ToArray();

        int rownr_plan_day = -1;
        int rownr_plan_night = -1;

        for (int i = 0; i < drs_dl_pl.Length; i++)
        {
            var dr_dl = drs_dl_pl[i];
            if (dr_dl.XKind1 == EKind1.PlanGroupDay)
                rownr_plan_day = i;
            if (dr_dl.XKind1 == EKind1.PlanGroupaNight)
                rownr_plan_night = i;
        }
        if (rownr_plan_day == -1 || (dr_pl.NIGHT == 1 && rownr_plan_night == -1))
            return $"Nav izveidots plāns [{dr_pl.DESCR}]";

        return "OK";
    }

    public static void MakePlans(CalendarMonthInfo calmt)
    {
        var table_darba_laiks = MyData.DbContextA.BL_A_TIMESHEET;
        var table_plans = MyData.DbContextA.BL_A_TIMEPLAN_LIST;
        float[] hr;
        EDayTag[] daytags;
        int i;
        short snr = 1;

        var drs_pl = table_plans.Where(d => d.USED == 1).ToArray();

        foreach (var dr in drs_pl)
        {
            hr = calmt.MakeHours(dr, false);
            daytags = calmt.MakeDayTags(dr);

            var dr1 = table_darba_laiks.NewItem();
            dr1.TimePlan = dr;
            dr1.XKind1 = EKind1.PlanGroupDay;
            dr1.YR = calmt.Year;
            dr1.MT = calmt.Month;
            dr1.SNR = snr;
            snr++;

            for (i = 0; i < 31; i++)
            {
                dr1.Vx[i] = hr[i];
                switch (daytags[i])
                {
                    case EDayTag.None:
                        dr1.DxPlan[i] = EDayPlanId.None;
                        break;
                    case EDayTag.WorkDay:
                    case EDayTag.WorkDayBeforeHoliday:
                        dr1.DxPlan[i] = EDayPlanId.DD;
                        break;
                    case EDayTag.HolidayInFreeDay:
                        dr1.DxPlan[i] = EDayPlanId.SD;
                        break;
                    case EDayTag.HolidayInWorkDay:
                        dr1.DxPlan[i] = EDayPlanId.SDDD;
                        break;
                    case EDayTag.FreeDay:
                        dr1.DxPlan[i] = EDayPlanId.BD;
                        break;
                }

            }
            dr1.K1 = dr1.SumV();

            table_darba_laiks.Add(dr1);

            if (dr.NIGHT == 1)
            {
                var dr2 = table_darba_laiks.NewItem();
                dr2.TimePlan = dr;
                dr2.XKind1 = EKind1.PlanGroupaNight;
                dr2.YR = calmt.Year;
                dr2.MT = calmt.Month;
                dr2.SNR = snr;

                hr = calmt.MakeHours(dr, true);

                dr2.SetAllVx(hr);
                dr2.K1 = dr2.SumV();

                snr++;
                table_darba_laiks.Add(dr2);
            }
        }
    }

    public static void FillPlans(CalendarMonthInfo calmt, A_TIMESHEET dr)
    {
        //var table_plans = MyData.DbContextA.BL_A_TIMEPLAN_LIST;
        float[] hr;
        EDayTag[] daytags;
        int i;

        var dr_pl = dr.TimePlan;

        hr = calmt.MakeHours(dr_pl, false);
        daytags = calmt.MakeDayTags(dr_pl);

        if (dr.XKind1 == EKind1.PlanGroupDay || dr.XKind1 == EKind1.PlanIndividualDay)
        {
            for (i = 0; i < 31; i++)
            {
                dr.Vx[i] = hr[i];
                switch (daytags[i])
                {
                    case EDayTag.None:
                        dr.DxPlan[i] = EDayPlanId.None;
                        break;
                    case EDayTag.WorkDay:
                    case EDayTag.WorkDayBeforeHoliday:
                        dr.DxPlan[i] = EDayPlanId.DD;
                        break;
                    case EDayTag.HolidayInFreeDay:
                        dr.DxPlan[i] = EDayPlanId.SD;
                        break;
                    case EDayTag.HolidayInWorkDay:
                        dr.DxPlan[i] = EDayPlanId.SDDD;
                        break;
                    case EDayTag.FreeDay:
                        dr.DxPlan[i] = EDayPlanId.BD;
                        break;
                }

            }
            dr.K1 = dr.SumV();
        }

        if (dr_pl.NIGHT == 1 && (dr.XKind1 == EKind1.PlanGroupaNight || 
            dr.XKind1 == EKind1.PlanIndividualNight))
        {
            hr = calmt.MakeHours(dr_pl, fornight: true);
            dr.SetAllVx(hr);
            var sum = dr.SumV();
            if (dr.K1 != sum) dr.K1 = sum;
        }

    }

    public static short GetNextTimeSheetNr(int yr)
    {
        var table_sar = MyData.DbContextA.BL_A_TIMESHEET_LISTS;
        var e1 = table_sar.Where(d => d.YR == yr);
        if (e1.FirstOrDefault() == null) return 1;
        return (short)(e1.Max(d => d.SNR) + 1);
    }

    public static List<A_POSITIONS> CheckTimeSheetsTemplUsed(int yr, int mt, A_TIMESHEET_TEMPL[] drs_sh)
    {
        var ret = new List<A_POSITIONS>();
        var table_sar = MyData.DbContextA.BL_A_TIMESHEET_LISTS;
        var drs_tsr = table_sar.Where(
            d =>
            d.YR == yr &&
            d.MT == mt
            ).SelectMany(d => d.TimesheetListRows)
            .ToArray();
        foreach (var dr_sh in drs_sh)
        {
            var drs_sh_r = dr_sh.TimeSheetTemplateRows;
            foreach (var dr_sh_r in drs_sh_r)
            {
                var drs_cur = drs_tsr.Where(
                    d =>
                    d.Person == dr_sh_r.Person &&
                    d.Position == dr_sh_r.Position);
                if (drs_cur.Any())
                    if (!ret.Contains(dr_sh_r.Position))
                        ret.Add(dr_sh_r.Position);
            }
        }
        return ret;
    }

    public static string MakeTimeSheetsFromTempl(int yr, int mt, A_TIMESHEET_TEMPL dr_sht = null)
    {
        var table_sar = MyData.DbContextA.BL_A_TIMESHEET_LISTS;
        var table_sar_r = MyData.DbContextA.BL_A_TIMESHEET_LISTS_R;
        var table_darba_laiks = MyData.DbContextA.BL_A_TIMESHEET;
        var table_sh = MyData.DbContextA.BL_A_TIMESHEET_TEMPL;
        var table_sh_r = MyData.DbContextA.BL_A_TIMESHEET_TEMPL_R;
        var table_plans = MyData.DbContextA.BL_A_TIMEPLAN_LIST;

        if (table_sh.Count == 0 || table_sh_r.Count == 0)
            return "Nav izveidotas darba uzskaites lapu sagataves.";

        A_TIMESHEET_TEMPL[] drs_sh = null;
        if (dr_sht == null)
            drs_sh = table_sh.OrderBy(d => d.SNR).ToArray();
        else
            drs_sh = [dr_sht];

        if (drs_sh.Length == 0 || drs_sh[0] == null)
            return "Nav atrastas darba uzskaites lapu sagataves.";

        var ret = CheckPlans(yr, mt);
        if (ret != "OK")
            return ret;

        var cur_positions = CheckTimeSheetsTemplUsed(yr, mt, drs_sh);
        if (cur_positions.Count > 0)
        {
            var drp = cur_positions[0].Person;
            if(cur_positions.Count == 1)
                return $"Darba laika ieraksts priekš darbinieka {drp.YNAME} ir jau izveidots.";
            else
                return $"Darba laika ieraksts priekš darbinieka {drp.YNAME} \nun vēl {cur_positions.Count - 1} darbiniekiem ir jau izveidots.";
        }


        var snr = GetNextTimeSheetNr(yr);

        foreach (var dr_sh in drs_sh)
        {
            var new_dr_sar = table_sar.NewItem();

            new_dr_sar.DEP = dr_sh.DEP;
            new_dr_sar.DESCR = dr_sh.DESCR;
            new_dr_sar.SNR = snr;
            new_dr_sar.YR = yr;
            new_dr_sar.MT = mt;

            snr++;

            table_sar.Add(new_dr_sar);

            var drs_sh_r = dr_sh.TimeSheetTemplateRows;
            foreach (var dr_sh_r in drs_sh_r)
            {
                var new_dr_sar_r = table_sar_r.NewItem();

                var dr_pl = dr_sh_r.TimePlan;
                var drs_pl = dr_pl.TimesheetRows.Where(
                    d =>
                    d.YR == yr &&
                    d.MT == mt
                    ).ToArray();

                new_dr_sar_r.Timesheet = new_dr_sar;
                new_dr_sar_r.Person = dr_sh_r.Person;
                new_dr_sar_r.Position = dr_sh_r.Position;
                new_dr_sar_r.TimePlan = dr_sh_r.TimePlan;
                new_dr_sar_r.SNR = dr_sh_r.SNR;
                new_dr_sar_r.PLAN_TYPE = dr_sh_r.PLAN_TYPE;
                new_dr_sar_r.NIGHT = dr_pl.NIGHT.Value;

                table_sar_r.Add(new_dr_sar_r);

                A_TIMESHEET dr_new_plan_r = null;


                if (new_dr_sar_r.XPlanType == EPlanType.Individual)
                {
                    //make ind plan
                    foreach (var dr_pl_r in drs_pl)
                    {
                        var new_dr_dl_pl = table_darba_laiks.NewItem();

                        new_dr_dl_pl.TimeSheetListRow = new_dr_sar_r;
                        new_dr_dl_pl.YR = yr;
                        new_dr_dl_pl.MT = mt;
                        new_dr_dl_pl.SNR = dr_sh_r.SNR;
                        new_dr_dl_pl.Person = dr_sh_r.Person;

                        switch (dr_pl_r.XKind1)
                        {
                            case EKind1.PlanGroupDay:
                                new_dr_dl_pl.XKind1 = EKind1.PlanIndividualDay;
                                dr_new_plan_r = new_dr_dl_pl;
                                break;
                            case EKind1.PlanGroupaNight:
                                new_dr_dl_pl.XKind1 = EKind1.PlanIndividualNight;
                                break;
                        }

                        for (int i = 0; i < 31; i++)
                        {
                            new_dr_dl_pl.Vx[i] = dr_pl_r.Vx[i];
                            new_dr_dl_pl.DxPlan[i] = dr_pl_r.DxPlan[i];
                        }
                        new_dr_dl_pl.K1 = new_dr_dl_pl.SumV();

                        table_darba_laiks.Add(new_dr_dl_pl);
                    }
                }
                else
                {
                    //link group plan
                    foreach (var dr_pl_r in drs_pl)
                    {
                        switch (dr_pl_r.XKind1)
                        {
                            case EKind1.PlanGroupDay:
                                dr_new_plan_r = dr_pl_r;
                                break;
                            case EKind1.PlanGroupaNight:
                                break;
                        }
                    }
                }

                var new_dr_dl_day = table_darba_laiks.NewItem();

                new_dr_dl_day.TimeSheetListRow = new_dr_sar_r;
                new_dr_dl_day.YR = yr;
                new_dr_dl_day.MT = mt;
                new_dr_dl_day.SNR = dr_sh_r.SNR;
                new_dr_dl_day.Person = dr_sh_r.Person;
                new_dr_dl_day.Position = dr_sh_r.Position;
                new_dr_dl_day.XKind1 = EKind1.Fact;

                for (int i = 0; i < 31; i++)
                {
                    EDayFactId daycode = EDayFactId.None;
                    var dayplancode = dr_new_plan_r.DxPlan[i];
                    daycode = SomeDataDefsA.PlanIdToFactId(dayplancode);
                    new_dr_dl_day.DxFact[i] = daycode;
                }

                table_darba_laiks.Add(new_dr_dl_day);

                if (new_dr_sar_r.NIGHT == 1)
                {
                    var new_dr_dl_night = table_darba_laiks.NewItem();

                    new_dr_dl_night.TimeSheetListRow = new_dr_sar_r;
                    new_dr_dl_day.TimeSheetListRow = new_dr_sar_r;
                    new_dr_dl_night.YR = yr;
                    new_dr_dl_night.MT = mt;
                    new_dr_dl_night.SNR = dr_sh_r.SNR;
                    new_dr_dl_night.Person = dr_sh_r.Person;
                    new_dr_dl_day.Position = dr_sh_r.Position;
                    new_dr_dl_night.XKind1 = EKind1.FactNight;

                    table_darba_laiks.Add(new_dr_dl_night);
                }
            }

            CheckDarbaLaiksEvents(new_dr_sar, null, true, false);
        }

        return "OK";
    }

    public static bool CanChangeDayCode(EDayFactId dcbase, EDayFactId dcuser)
    {
        return SomeDataDefsA.CanChangeDayId(dcbase) && SomeDataDefsA.CanChangeDayId(dcuser);
    }

    public static ErrorList CheckDarbaLaiksEvents(A_TIMESHEET_LISTS dr_sar,
        A_TIMESHEET_LISTS_R dr_sarr = null, bool sethours = true, bool setdaycode = true)
    {
        var table_sar = MyData.DbContextA.BL_A_TIMESHEET_LISTS;
        var table_sar_r = MyData.DbContextA.BL_A_TIMESHEET_LISTS_R;
        var table_darba_laiks = MyData.DbContextA.BL_A_TIMESHEET;
        var table_plans = MyData.DbContextA.BL_A_TIMEPLAN_LIST;

        var table_persons = MyData.DbContextA.BL_A_PERSONS;
        var table_amati = MyData.DbContextA.BL_A_POSITIONS;

        A_TIMESHEET_LISTS_R[] drs_sar_r;
        if (dr_sarr != null)
        {
            drs_sar_r = [dr_sarr];
        }
        else
        {
            drs_sar_r = dr_sar.TimesheetListRows.ToArray();
        }

        int yr = dr_sar.YR;
        int mt = dr_sar.MT;

        DateTime dt1 = new DateTime(yr, mt, 1);
        DateTime dt2 = dt1.LastDayOfMonth();
        int daycount = dt2.Subtract(dt1).Days + 1;

        var err = new ErrorList();
        var events_info = new EventsInfo();

        foreach (var dr_sar_r in drs_sar_r)
        {

            var dr_pers = dr_sar_r.Person;
            var dr_amats = dr_sar_r.Position;

            var ret = events_info.ProcessData(dr_sar_r.Person, dt1, dt2);

            if (ret != "OK")
            {
                err.AddPersonError(dr_sar_r.Person, ret);
                continue;
            }

            var dlrowset = dr_sar_r.GetDLRowSet();

            var perioddayids = events_info.PositionDayIds[dr_sar_r.Position];
            var eventdayids = events_info.PositionDayIdsA[dr_sar_r.Position];

            for (int i = 0; i < daycount; i++)
            {
                var periodid = perioddayids[i];
                var eventid = eventdayids[i];
                EDayFactId dayidbase = SomeDataDefsA.GetDayCodeForEvent(periodid, eventid);
                EDayFactId dayiduser = dlrowset.Fact.DxFact[i];
                if (dayidbase == EDayFactId.Error) throw new Exception("Bad event id");

                if (dayidbase != dayiduser && !CanChangeDayCode(dayidbase, dayiduser))
                {
                    dlrowset.Fact.Vx[i] = 0.0f;
                    dlrowset.Fact.DxFact[i] = dayidbase;
                    dayiduser = dayidbase;

                    if (dlrowset.FactNight != null)
                    {
                        dlrowset.FactNight.Vx[i] = 0.0f;
                    }

                    if (dlrowset.FactOvertime != null)
                    {
                        dlrowset.FactOvertime.Vx[i] = 0.0f;
                    }
                }

                if (setdaycode)
                {
                    if (SomeDataDefsA.CanChangeDayId(dlrowset.Fact.DxFact[i]))
                    {
                        EDayFactId daycode = EDayFactId.None;
                        var dayplancode = dlrowset.Plan.DxPlan[i];
                        daycode = SomeDataDefsA.PlanIdToFactId(dayplancode);
                        if (dlrowset.Fact.DxFact[i] != daycode)
                        {
                            dlrowset.Fact.DxFact[i] = daycode;
                            dlrowset.Fact.Vx[i] = dlrowset.Plan.Vx[i];
                            dayiduser = daycode;
                        }
                    }
                }


                if (periodid == EPeriodId.Ir_pieņemts && !SomeDataDefsA.IsDayForWork(dayiduser))
                {
                    dlrowset.Fact.Vx[i] = 0.0f;

                    if (dlrowset.FactNight != null)
                    {
                        dlrowset.FactNight.Vx[i] = 0.0f;
                    }

                    if (dlrowset.FactOvertime != null)
                    {
                        dlrowset.FactOvertime.Vx[i] = 0.0f;
                    }

                }

                if (sethours)
                {
                    if (SomeDataDefsA.IsDayForWork(dlrowset.Fact.DxFact[i]))
                    {
                        dlrowset.Fact.Vx[i] = dlrowset.Plan.Vx[i];
                        if(dlrowset.PlanNight != null && dlrowset.FactNight != null)
                            dlrowset.FactNight.Vx[i] = dlrowset.PlanNight.Vx[i];
                    }
                    else
                    {
                        dlrowset.Fact.Vx[i] = 0.0f;
                        if (dlrowset.FactNight != null)
                            dlrowset.FactNight.Vx[i] = 0.0f;
                        if (dlrowset.FactOvertime != null)
                            dlrowset.FactOvertime.Vx[i] = 0.0f;
                    }

                    var sum = dlrowset.Fact.SumV();
                    if (dlrowset.Fact.K1 != sum)
                        dlrowset.Fact.K1 = sum;
                }
            }

        }

        return err;
    }

    public static void FillFactHours(CalendarMonthInfo calmt, A_TIMESHEET_LISTS_R dr_sar_r)
    {
        if (dr_sar_r == null)
        {
            throw new ArgumentException("Bad idsarr.");
        }

        var dlrowset = dr_sar_r.GetDLRowSet();
        if (dlrowset.Plan == null || dlrowset.Fact == null)
        {
            throw new ArgumentException("Bad DL rowset.");
        }

        int i;

        for (i = 0; i < calmt.DaysInMonth; i++)
        {
            var factcode = dlrowset.Fact.DxFact[i];
            if (SomeDataDefsA.IsDayForWork(factcode))
            {
                dlrowset.Fact.Vx[i] = dlrowset.Plan.Vx[i];
            }
            else
            {
                dlrowset.Fact.Vx[i] = 0.0f;
            }
        }
    }

    public static int GetNextPayListNr(int yr)
    {
        var table_sar = MyData.DbContextA.BL_A_PAYLISTS;
        var e1 = table_sar.Where(d => d.YR == yr);
        if (e1.FirstOrDefault() == null) return 1;
        return e1.Max(d => d.SNR) + 1;
    }

    public static int GetNextFpPayListNr(int yr)
    {
        var table_sar = MyData.DbContextA.BL_A_FP_PAYLISTS;
        var e1 = table_sar.Where(d => d.YR == yr);
        if (e1.FirstOrDefault() == null) return 1;
        return e1.Max(d => d.SNR) + 1;
    }

    // SaveData required before and after FillPayListA
    public static string MakePayListsFromTempl(DateTime dt, int yr, int mt, bool dofill)
    {
        var table_templ = MyData.DbContextA.BL_A_PAYLIST_TEMPL;
        var table_templ_r = MyData.DbContextA.BL_A_PAYLIST_TEMPL_R;
        var table_lists = MyData.DbContextA.BL_A_PAYLISTS;
        var table_lists_r = MyData.DbContextA.BL_A_PAYLISTS_R;

        if (table_templ.Count == 0 || table_templ_r.Count == 0)
            return "Nav izveidotas maksājumu sarakstu sagataves.";

        int snr0 = GetNextPayListNr(yr);

        var drs_t = table_templ.OrderBy(d => d.SNR);

        var new_lists_drs = new List<A_PAYLISTS>();

        foreach (var dr_t in drs_t)
        {
            var new_dr_list = table_lists.NewItem();

            new_dr_list.Department = dr_t.Department;
            new_dr_list.DESCR = dr_t.DESCR;
            new_dr_list.DT = dt;
            new_dr_list.YR = yr;
            new_dr_list.MT = mt;
            new_dr_list.SNR = snr0;

            snr0++;

            table_lists.Add(new_dr_list);
            new_lists_drs.Add(new_dr_list);

            var drs_t_r = dr_t.PayListTemplateRows;
            foreach(var dr_t_r in drs_t_r)
            {
                var new_dr_list_r = table_lists_r.NewItem();

                new_dr_list_r.PayList = new_dr_list;
                new_dr_list_r.Position = dr_t_r.Position;
                new_dr_list_r.Position = dr_t_r.Position;
                new_dr_list_r.SNR = dr_t_r.SNR;

                table_lists_r.Add(new_dr_list_r);
            }
        }

        var er = UpdateTable(table_lists);
        if (er != "OK") return er;
        er = UpdateTable(table_lists_r);
        if (er != "OK") return er;

        if (dofill)
        {
            foreach(var drs in new_lists_drs)
            {
                FillPayListsA(dt, drs, true);

                er = UpdateTable(table_lists_r);
                if (er != "OK") return er;

                FillPayListsB(dt, drs);

                er = UpdateTable(table_lists_r);
                if (er != "OK") return er;
            }
        }

        return "OK";
    }

    public static string MakePayListFromTempl(DateTime dt, 
        A_PAYLIST_TEMPL dr_t, int yr, int mt, bool dofill)
    {
        var table_templ = MyData.DbContextA.BL_A_PAYLIST_TEMPL;
        var table_templ_r = MyData.DbContextA.BL_A_PAYLIST_TEMPL_R;
        var table_lists = MyData.DbContextA.BL_A_PAYLISTS;
        var table_lists_r = MyData.DbContextA.BL_A_PAYLISTS_R;

        if (table_templ.Count == 0 || table_templ_r.Count == 0)
            return "Nav izveidotas maksājumu sarakstu sagataves.";

        if (dr_t == null)
            return "Sagatave nav atrasta.";

        var drs_t_r = dr_t.PayListTemplateRows;

        if (drs_t_r.Count == 0)
            return "Maksājuma saraksta sagatave ir tukša.";

        var drs_cur = table_lists_r
            .Where(
                d =>
                d.PayList.DT == dt &&
                drs_t_r.Where(
                    d1 =>
                    d1.Position == d.Position
                    ).FirstOrDefault() != null)
            .ToArray();

        // vajadzētu darboties arī vairākiem maksājumiem vienā datumā
        //if (drs_cur.Length > 0)
        //    return string.Format("Viens no darbiniekiem ir jau iekļauts\nmaksāumu sarakstā ar datumu {0:dd.MM.yyyy}.", dt);

        var new_dr_list = table_lists.NewItem();

        new_dr_list.Department = dr_t.Department;
        new_dr_list.DESCR = dr_t.DESCR;
        new_dr_list.DT = dt;
        new_dr_list.SNR = GetNextPayListNr(yr);
        new_dr_list.YR = yr;
        new_dr_list.MT = mt;

        table_lists.Add(new_dr_list);

        foreach (var dr_t_r in drs_t_r)
        {
            var new_dr_list_r = table_lists_r.NewItem();

            new_dr_list_r.PayList = new_dr_list;
            new_dr_list_r.Person = dr_t_r.Person;
            new_dr_list_r.Position = dr_t_r.Position;
            new_dr_list_r.SNR = dr_t_r.SNR;

            table_lists_r.Add(new_dr_list_r);
        }

        var er = UpdateTable(table_lists);
        if (er != "OK") return er;

        if (dofill)
        {
            FillPayListA(new_dr_list, true);

            er = UpdateTable(table_lists_r);
            if (er != "OK") return er;

            FillPayListB(new_dr_list);

            new_dr_list.TOTAL_PAY = new_dr_list.PayListRows.Sum(x => x.TPAY);

            er = UpdateTable(table_lists_r);
            if (er != "OK") return er;
        }

        return "OK";
    }
    
    public static ErrorList MakePayListFromImportData(IList<PaylistImportRow> rows)
    {
        var er = new ErrorList();
        var drsbydate = rows
            .GroupBy(x => x.Date)
            .OrderBy(x => x.Key);
        foreach(var dr_gr in drsbydate)
        {
            var drs = dr_gr.ToList();
            var dt = drs[0].Date.Value;
            var rt = MakePayListFromImportData(dt, dt.Year, dt.Month, drs);
            if(rt != "OK")
            {
                er.AddError("Saraksts: " + Utils.DateToString(dt), rt);
            }
        }
        return er;
    }

    public static string MakePayListFromImportData(DateTime dt, int yr, int mt, IList<PaylistImportRow> rows)
    {
        var table_lists = MyData.DbContextA.BL_A_PAYLISTS;
        var table_lists_r = MyData.DbContextA.BL_A_PAYLISTS_R;

        if (rows.Count == 0)
            return "Datu saraksts tukšs.";

        var new_dr_list = table_lists.NewItem();

        new_dr_list.DT = dt;
        new_dr_list.SNR = GetNextPayListNr(yr);
        new_dr_list.YR = yr;
        new_dr_list.MT = mt;

        table_lists.Add(new_dr_list);

        int k = 1;
        foreach (var dr_data in rows)
        {
            var new_dr_list_r = table_lists_r.NewItem();

            new_dr_list_r.PayList = new_dr_list;
            new_dr_list_r.Person = dr_data.Person;
            new_dr_list_r.Position = dr_data.Position;
            new_dr_list_r.TPAY = dr_data.Amount;
            new_dr_list_r.SNR = k;

            table_lists_r.Add(new_dr_list_r);
            k++;
        }

        var er = UpdateTable(table_lists);
        if (er != "OK") return er;
        er = UpdateTable(table_lists_r);
        if (er != "OK") return er;

        FillPayListA(new_dr_list, false);

        er = UpdateTable(table_lists_r);
        if (er != "OK") return er;

        FillPayListB(new_dr_list);

        new_dr_list.TOTAL_PAY = new_dr_list.PayListRows.Sum(x => x.TPAY);

        er = UpdateTable(table_lists_r);
        if (er != "OK") return er;

        return "OK";
    }

    public static ErrorList CheckPayListIDs(A_PAYLISTS dr_s)
    {
        var err = new ErrorList();
        if (dr_s == null) return err;

        var drs = dr_s.PayListRows;
        if (drs.Count == 0) return err;

        var baddrs = drs
            .GroupBy(x => (x.Person, x.Position))
            .Where(x => x.Count() > 1)
            .Select(x => x.First())
            .ToList();

        string errsrc = $"Maskājumu saraksts {dr_s.SNR} {dr_s.DT:dd.MM.yyyy}";
        foreach(var dr in baddrs)
        {
            var errstr = $"Darbiniekam divi maksājumi vienā sarakstā: {dr.Person.ZNAME}, {dr.Position.TITLE}";
            err.AddError(errsrc, errstr);
        }

        return err;
    }

    //FillPayListsA - updates only pay balance
    //FillPayListsB - after FillPayListsA updates list match
    public static void FillPayListsA(DateTime dt, A_PAYLISTS dr_paylist, bool fullrecalc)
    {
        var table_lists_r = MyData.DbContextA.BL_A_PAYLISTS_R;

        var drs = table_lists_r
            .Where(d => d.PayList == dr_paylist)
            .ToArray();

        if (drs.Length == 0) return;

        var dt1 = dt.FirstDayOfMonth();
        if (dt == dt.LastDayOfMonth()) dt1 = dt.AddDays(1);
        var table_paysaldo = DataLoaderA.PAYSALDO_GetBy_SP_A_PAY_SALDO_03(dt, dt1, dr_paylist.ID);

        if (table_paysaldo.Count == 0) return;

        foreach(var dr in drs)
        {
            var dr_ps = table_paysaldo.FindByIDAM(dr.IDAM.Value);
            FillPayRowA(dr, dr_ps, fullrecalc);
        }
    }

    public static void FillPayListsB(DateTime dt, A_PAYLISTS dr_paylist)
    {
        var table_lists_r = MyData.DbContextA.BL_A_PAYLISTS_R;

        var drs = table_lists_r
            .Where(d => d.PayList == dr_paylist)
            .ToArray();

        if (drs.Length == 0) return;

        var dt1 = dt.FirstDayOfMonth();
        if (dt == dt.LastDayOfMonth()) dt1 = dt.AddDays(1);

        var table_matchlists = DataLoaderA.SP_PAY_MATCHLISTS_1X_GetBy_SP_A_PAY_MATCHLISTS_14(dt, dr_paylist.ID);

        if (table_matchlists.Count == 0) return;

        foreach (var dr in drs)
        {
            var dr_ml = table_matchlists.FindByIDAM(dr.IDAM.Value);
            FillPayRowC(dr, dr_ml);
        }
    }

    public static void FillPayListA(A_PAYLISTS dr_paylist, bool fullrecalc)
    {
        var dr_s = dr_paylist;
        if (dr_s == null) return;

        var drs = dr_s.PayListRows;
        if (drs.Count == 0) return;

        var dt = dr_s.DT;
        var dt1 = dt.FirstDayOfMonth();
        if (dt == dt.LastDayOfMonth()) dt1 = dt.AddDays(1);
        var table_paysaldo = DataLoaderA.PAYSALDO_GetBy_SP_A_PAY_SALDO_02(dr_paylist.ID, dt, dt1);

        if (table_paysaldo.Count == 0) return;

        foreach (var dr in drs)
        {
            var dr_ps = table_paysaldo.FindByIDAM(dr.IDAM.Value);
            FillPayRowA(dr, dr_ps, fullrecalc);
        }
    }

    public static void FillPayListB(A_PAYLISTS dr_s)
    {
        if (dr_s == null) return;

        var drs = dr_s.PayListRows;
        if (drs.Count == 0) return;

        var dt = dr_s.DT;
        var dt1 = dt.FirstDayOfMonth();
        if (dt == dt.LastDayOfMonth()) dt1 = dt.AddDays(1);

        var table_matchlists = DataLoaderA.SP_PAY_MATCHLISTS_1X_GetBy_SP_A_PAY_MATCHLISTS_13(dr_s.ID, dt);

        if (table_matchlists.Count == 0) return;

        foreach (var dr in drs)
        {
            var dr_ml = table_matchlists.FindByIDAM(dr.IDAM.Value);
            FillPayRowC(dr, dr_ml);
        }
    }

    public static void FillPayListRowA(A_PAYLISTS_R dr, bool fullrecalc)
    {
        if (dr == null) return;
        var dr_s = dr.PayList;
        var dt = dr_s.DT;
        var dt1 = dt.FirstDayOfMonth();
        if (dt == dt.LastDayOfMonth()) dt1 = dt.AddDays(1);

        var table_paysaldo = DataLoaderA.PAYSALDO_GetBy_SP_A_PAY_SALDO_01(dr.IDAM.Value, dt, dt1, dr.IDS);
        if (table_paysaldo.Count == 0) return;
        var dr_ps = table_paysaldo[0];

        FillPayRowA(dr, dr_ps, fullrecalc);
    }

    public static void FillPayListRowB(A_PAYLISTS_R dr)
    {
        if (dr == null) return;
        var dr_s = dr.PayList;
        var dt = dr_s.DT;
        var dt1 = dt.FirstDayOfMonth();
        if (dt == dt.LastDayOfMonth()) dt1 = dt.AddDays(1);

        var table_matchlists = DataLoaderA.SP_PAY_MATCHLISTS_1X_GetBy_SP_A_PAY_MATCHLISTS_12(dr.IDAM.Value, dr.PAY, dt, dr.IDS);

        if (table_matchlists.Count == 0) return;

        var dr_ml = table_matchlists[0];

        FillPayRowC(dr, dr_ml);
    }


    public static void FillPayRowA(A_PAYLISTS_R dr, 
        A_PAYSALDO dr_ps, bool fullrecalc)
    {
        decimal PAY0 = 0.0M;
        decimal IIN0 = 0.0M;
        decimal ADVANCE0 = 0.0M;
        decimal WITHHOLDINGS0 = 0.0M;
        decimal TPAY0 = 0.0M;
        decimal PAY = 0.0M;
        decimal IIN = 0.0M;
        decimal ADVANCE = 0.0M;
        decimal WITHHOLDINGS = 0.0M;
        decimal TPAY = 0.0M;
        decimal IIN_REVERSE = 0.0M;
        decimal PAY_REVERSE = 0.0M;
        decimal AW0 = 0.0M;
        decimal AW = 0.0M;

        var act_has_changes = new Func<bool>(() =>
        {
            if (dr.PAY0 != PAY0) return true;
            if (dr.IIN0 != IIN0) return true;
            if (dr.ADVANCE0 != ADVANCE0) return true;
            if (dr.WITHHOLDINGS0 != WITHHOLDINGS0) return true;
            if (dr.TPAY0 != TPAY0) return true;
            if (dr.PAY != PAY) return true;
            if (dr.IIN != IIN) return true;
            if (dr.ADVANCE != ADVANCE) return true;
            if (dr.WITHHOLDINGS != WITHHOLDINGS) return true;
            if (dr.TPAY != TPAY) return true;
            if (dr.IIN_REVERSE != IIN_REVERSE) return true;
            if (dr.PAY_REVERSE != PAY_REVERSE) return true;
            return false;
        });

        var act_apply_changes = new Action(() =>
        {
            if (dr.PAY0 != PAY0) dr.PAY0 = PAY0;
            if (dr.IIN0 != IIN0) dr.IIN0 = IIN0;
            if (dr.ADVANCE0 != ADVANCE0) dr.ADVANCE0 = ADVANCE0;
            if (dr.WITHHOLDINGS0 != WITHHOLDINGS0) dr.WITHHOLDINGS0 = WITHHOLDINGS0;
            if (dr.TPAY0 != TPAY0) dr.TPAY0 = TPAY0;
            if (dr.PAY != PAY) dr.PAY = PAY;
            if (dr.IIN != IIN) dr.IIN = IIN;
            if (dr.ADVANCE != ADVANCE) dr.ADVANCE = ADVANCE;
            if (dr.WITHHOLDINGS != WITHHOLDINGS) dr.WITHHOLDINGS = WITHHOLDINGS;
            if (dr.TPAY != TPAY) dr.TPAY = TPAY;
            if (dr.IIN_REVERSE != IIN_REVERSE) dr.IIN_REVERSE = IIN_REVERSE;
            if (dr.PAY_REVERSE != PAY_REVERSE) dr.PAY_REVERSE = PAY_REVERSE;
        });

        if (dr_ps == null)
        {
            if (act_has_changes()) act_apply_changes();
            return;
        }

        PAY0 = dr_ps.PAY0.Value;
        IIN0 = dr_ps.IIN0.Value;
        ADVANCE0 = dr_ps.ADVANCE.Value;
        WITHHOLDINGS0 = dr_ps.WITHHOLDINGS.Value;
        TPAY0 = PAY0 + ADVANCE0 - WITHHOLDINGS0;
        AW0 = ADVANCE0 - WITHHOLDINGS0;

        if (fullrecalc)
            TPAY = Math.Max(0.0M, TPAY0);
        else
            TPAY = dr.TPAY;

        if (TPAY >= TPAY0 || PAY0 < 0.0M)
        {
            PAY = PAY0;
        }
        else
        {
            if (AW0 >= 0.0M)
            {
                if (TPAY >= 0.0M)
                    PAY = Math.Min(PAY0, TPAY);
                else
                    PAY = 0.0M;
            }
            else
            {
                if (TPAY >= 0.0M)
                    PAY = Math.Min(PAY0, TPAY - AW0);
                else
                    PAY = Math.Min(PAY0, - AW0);
            }
        }
        AW = TPAY - PAY;


        /*
        IIN = IIN0;
        if (PAY0 < 0.0M)
        {
            IIN_REVERSE = IIN0;
            PAY_REVERSE = PAY0;
        }
        else if (PAY0 == 0.0M)
            IIN = 0.0M;
        else
            IIN = Math.Round(PAY / PAY0 * IIN0);
        */

        if (AW > AW0 && WITHHOLDINGS > 0.0M)
        {
            WITHHOLDINGS = Math.Max(WITHHOLDINGS0 - (AW - AW0), 0.0M);
        }
        else
        {
            WITHHOLDINGS = WITHHOLDINGS0;
        }
        ADVANCE = AW + WITHHOLDINGS;

        if (act_has_changes()) act_apply_changes();
    }

    public static void FillPayRowB(A_PAYLISTS_R dr,
        A_SP_PAY_MATCHLISTS dr_m)
    {
        if (dr.DT1 == dr_m.DT1 &&
            dr.DT2 != dr_m.DT2 &&
            dr.R1 != dr_m.R1 &&
            dr.R2 != dr_m.R2 &&
            dr.S0 != dr_m.PAY0 &&
            dr.S1 != dr_m.PAY1 &&
            dr.S2 != dr_m.PAY2) return;

        if (dr.DT1 != dr_m.DT1) dr.DT1 = dr_m.DT1;
        if (dr.DT2 != dr_m.DT2) dr.DT2 = dr_m.DT2;
        if (dr.R1 != dr_m.R1) dr.R1 = dr_m.R1;
        if (dr.R2 != dr_m.R2) dr.R2 = dr_m.R2;
        if (dr.S0 != dr_m.PAY0) dr.S0 = dr_m.PAY0.Value;
        if (dr.S1 != dr_m.PAY1) dr.S1 = dr_m.PAY1.Value;
        if (dr.S2 != dr_m.PAY2) dr.S2 = dr_m.PAY2.Value;
    }

    public static void FillPayRowC(A_PAYLISTS_R dr,
        A_SP_PAY_MATCHLISTS_1X dr_x)
    {
        if (dr.DT1 == dr_x.DT1 &&
            dr.DT2 != dr_x.DT2 &&
            dr.IIN != dr_x.IIN &&
            dr.R1 != dr_x.R1 &&
            dr.R2 != dr_x.R2 &&
            dr.S0 != dr_x.PAY0 &&
            dr.S1 != dr_x.PAY1 &&
            dr.S2 != dr_x.PAY2) return;

        if (dr.DT1 != dr_x.DT1) dr.DT1 = dr_x.DT1;
        if (dr.DT2 != dr_x.DT2) dr.DT2 = dr_x.DT2;
        if (dr.IIN != dr_x.IIN) dr.IIN = dr_x.IIN;
        if (dr.R1 != dr_x.R1) dr.R1 = dr_x.R1;
        if (dr.R2 != dr_x.R2) dr.R2 = dr_x.R2;
        if (dr.S0 != dr_x.PAY0) dr.S0 = dr_x.PAY0;
        if (dr.S1 != dr_x.PAY1) dr.S1 = dr_x.PAY1;
        if (dr.S2 != dr_x.PAY2) dr.S2 = dr_x.PAY2;

        PayListCalcInfo.Static.Calc(dr, dr_x);

        decimal iin = dr.IIN + dr.IIN_1 + dr.IIN_2;
        if (dr.IIN != iin) dr.IIN = iin;
    }

    public static ErrorList CheckEventsSimple(A_PERSONS dr_person)
    {
        var err = new ErrorList();
        if (dr_person == null) return err;
        var drs_events = dr_person.Events;
        foreach(var dr in drs_events)
        {
            int idn = dr.IDN;
            var ev = (EEventId)idn;
            bool b = SomeDataDefsA.IsFromToEvent(ev);
            bool baddates = false;
            if (b)
            {
                if (dr.DATE2 == null)
                {
                    baddates = true;
                }
                else
                {
                    var dt1 = dr.DATE1;
                    var dt2 = dr.DATE2;
                    if (dt1 > dt2 || dt1.Year < 1950 || dt1.Year > 2100 ||
                        dt2.Value.Year < 1950 || dt2.Value.Year > 2100)
                    {
                        baddates = true;
                    }
                }
            }
            else
            {
                //if (dr.IsDATE1Null()) baddates = true;
            }
            if (baddates)
            {
                err.AddPersonError(dr_person, "Nekorekti norādīti datumi no - līdz.");
            }

            b = SomeDataDefsA.EventHasPayDate(ev);
            if (b)
            {
                if (dr.DATE3 == null)
                {
                    baddates = true;
                }
                else
                {
                    var dt3 = dr.DATE3;
                    if (dt3.Value.Year < 1950 || dt3.Value.Year > 2100)
                        baddates = true;
                }
            }
            if (baddates)
            {
                err.AddPersonError(dr_person, "Nekorekti norādīts izmaksas datums.");
            }

            if (ev == EEventId.Piešķirts_amats || ev == EEventId.Atbrīvots_no_amata)
            {
                if (dr.Person == null)
                {
                    err.AddPersonError(dr_person, "Janorāda amats.");
                }
            }

            if (SomeDataDefsA.EventHasSCode(ev))
            {
                if (string.IsNullOrEmpty(dr.SCODE))
                {
                    err.AddPersonError(dr_person, "Janorāda ziņu kods.");
                }
            }

            if (SomeDataDefsA.EventHasOccCode(ev))
            {
                if (string.IsNullOrEmpty(dr.OCCUPATION_CODE))
                {
                    err.AddPersonError(dr_person, "Janorāda profesijas kods.");
                }
            }

            if (ev == EEventId.Atvaļinājums && dr.DAYS == 0)
            {
                err.AddPersonError(dr_person, "Janorāda atvaļinājuma dienu skaits.");
            }

        }
        return err;
    }

    public static string CheckEventsA(A_PERSONS dr_person)
    {
        var events_info = new EventsInfo();
        if (dr_person == null) return "Nav darbinieka";
        var drs_events = dr_person.Events;

        var ret = events_info.AnalizeDLEvents(dr_person);

        if (ret != "OK") return ret;

        var drs_pr = dr_person.PersonsStateRows;
        if (drs_pr.Count == 0)
        {
            return "Darbinieka dati ir bojāti.";
        }
        var dr_pr_first = drs_pr.OrderBy(d => d.EDIT_DATE).First();
        var dtprfirst = dr_pr_first.EDIT_DATE;

        if(events_info.HireFire.LinkedPeriods.Count == 0 ||
            dtprfirst > events_info.HireFire.LinkedPeriods[0].DateFirst)
            return "Nav darbinieka datu uz pieņemšanas brīdi.";

        return "OK";
    }

    public static ErrorList CheckEvents(A_PERSONS dr_person)
    {
        var err = CheckEventsSimple(dr_person);
        if (err.HasErrors) return err;
        var es = CheckEventsA(dr_person);
        if(es != "OK")
        {
            err.AddPersonError(dr_person, es);
            return err;
        }
        return err;
    }

    public static ErrorList CheckEventsAll()
    {
        var err = new ErrorList();
        var table_persons = MyData.DbContextA.BL_A_PERSONS;
        foreach(var dr in table_persons)
        {
            var err1 = CheckEventsSimple(dr);
            err += err1;
            var es = CheckEventsA(dr);
            if (es != "OK")
                err.AddPersonError(dr, es);
        }
        return err;
    }

    public static bool HaveSalaryRRow(DateTime dt1, DateTime dt2, 
        A_PERSONS dr_person, A_POSITIONS dr_position)
    {
        var table_algas_r = MyData.DbContextA.BL_A_SALARY_SHEETS_R;
        return table_algas_r.Where(
            d =>
            {
                if (d.Person != dr_person || d.Position == null || d.Position != dr_position) return false;
                var dr_algas_lapa = d.SalarySheet;
                return dr_algas_lapa.DT1 != dt1 &&
                    dr_algas_lapa.DT2 == dt2;
            }).Count() > 0;
    }


    public static string UpdateTable(IBindingList table)
    {

        var rt = MyData.DbContextA.Save();
        if (rt.IsSuccess())
            return "OK";
        return "Neizdevās saglabāt datu tabulu.";
    }

}
