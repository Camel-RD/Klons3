using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Klons3.ModelsA;
using KlonsF.Classes;
using Microsoft.EntityFrameworkCore;
using KlonsLIB.Misc;
using KlonsLIB.Forms;

namespace KlonsA.Classes;

public static class DataLoaderA
{
    public static KlonsData MyData => KlonsData.St;
		
    public static DateTime LoadedDT0;
    public static DateTime LoadedDT1;
    public static DateTime LoadedDT2;

    public static int paminyr, paminmt, pamaxyr, pamaxmt;
    public static int pdlminyr, pdlminmt, pdlmaxyr, pdlmaxmt;

    public static bool DataLoaded { get; set; } = false;

    public static void ResetState()
    {
        DataLoaded = false;
        LoadedDT0 = default;
        LoadedDT1 = default;
        LoadedDT2 = default;
    }
		
		
    public static void RefreshMinMax()
    {
        var ret = SP_GET_MINMAX_GetBy_SP_A_GET_MINMAX()[0];
        pdlminyr = ret.RDLMINYR;
        pdlminmt = ret.RDLMINMT;
        pdlmaxyr = ret.RDLMAXYR;
        pdlmaxmt = ret.RDLMAXMT;
        paminyr = ret.RAMINYR;
        paminmt = ret.RAMINMT;
        pamaxyr = ret.RAMAXYR;
        pamaxmt = ret.RAMAXMT;
    }

    public static void GetMonthList(out string[] ss, out int[,] yrmt)
    {
        int ct = (LoadedDT2.Year - LoadedDT1.Year) * 12 + LoadedDT2.Month - LoadedDT1.Month + 1;
        ss = new string[ct];
        yrmt = new int[ct, 2];
        int yr = LoadedDT1.Year;
        int mt = LoadedDT1.Month;
        int i = 0;
        while (true)
        {
            ss[i] = $"{yr}.{mt:00}";
            yrmt[i, 0] = yr;
            yrmt[i, 1] = mt;
            if (yr == LoadedDT2.Year && mt == LoadedDT2.Month) break;
            i++;
            mt++;
            if(mt == 13)
            {
                mt = 1;
                yr++;
            }
        }
    }

    public static bool IsMonthLoaded(int yr, int mt)
    {
        if(!HasDataA() || !DataLoaded) return false;
        int ct1 = (LoadedDT2.Year - LoadedDT1.Year) * 12 + LoadedDT2.Month - LoadedDT1.Month + 1;
        int ct2 = (yr - LoadedDT1.Year) * 12 + mt - LoadedDT1.Month + 1;
        return ct2 >= 0 && ct2 <= ct1;
    }

    public static bool IsPeriodLoaded(DateTime dt1, DateTime dt2)
    {
        if (!HasDataA() || !DataLoaded) return false;
        return LoadedDT1 <= dt1 && LoadedDT2 >= dt2;
    }

    public static string GetPeriodStr()
    {
        return string.Format("Periods: [{0}.{1:00} - {2}.{3:00}]",
            LoadedDT1.Year, LoadedDT1.Month, LoadedDT2.Year, LoadedDT2.Month);
    }

    public static bool ReLoadData()
    {
        try
        {
            ClearAll();
            return FillA();
        }
        catch (Exception e)
        {
            Form_Error.ShowException(e, "Neizdevās ielādēt datus.");
            return false;
        }
    }

    public static bool HasDataA()
    {
        var ds = MyData.DbContextA;
        return ds.A_EVENT_TYPES.Local.Count > 0;
    }

    public static void ClearAll()
	{
        MyData.RecreateDbContextA();
    }			

    public static void ClearB()
    {
        var ctx = MyData.DbContextA;

        IQueryable[] tables = [
            ctx.A_PIECEWORK,
            ctx.A_SALARY_SHEETS_R,
            ctx.A_SALARY_PLUSMINUS,
            ctx.A_SALARY_SHEETS,
            ctx.A_TIMESHEET,
            ctx.A_TIMESHEET_LISTS_R,
            ctx.A_TIMESHEET_LISTS,
            ctx.A_PAYLISTS,
            ctx.A_PAYLISTS_R,
            ctx.A_FP_PAYLISTS,
            ctx.A_FP_PAYLISTS_R
        ];

        foreach (var t in tables)
            ctx.RemoveFromContextRange(t);
    }

    public static bool FillA()
    {
        var ctx = MyData.DbContextA;

        IQueryable[] tables = [
            ctx.A_RATES,
            ctx.A_HOLIDAYS,
            ctx.A_BANKS,
            ctx.A_DEPARTMENTS,
            ctx.A_INCOME_CODES,
            ctx.A_EVENT_TYPES,
            ctx.A_TERITORIAL_CODES,
            ctx.A_REPORT_CODES,
            ctx.A_EVENT_TYPES2,
            ctx.A_PLUSMINUS_FROM,
            ctx.A_PLUSMINUS_TYPES,
            ctx.A_PIECEWORK_CATSTRUCT,
            ctx.A_PIECEWORK_CATALOG,

            ctx.A_PERSONS_FIZ,
            ctx.A_PERSONS,
            ctx.A_PERSONS_R,
            ctx.A_PASTDATA,
            ctx.A_POSITIONS,
            ctx.A_POSITIONS_PLUSMINUS,
            ctx.A_POSITIONS_R,
            ctx.A_EVENTS,
            ctx.A_UNTAXED_MIN,
            ctx.A_PAYLIST_TEMPL,
            ctx.A_PAYLIST_TEMPL_R,
            ctx.A_SALARY_SHEET_TEMPL,
            ctx.A_SALARY_SHEET_TEMPL_R,
            ctx.A_TIMESHEET_TEMPL,
            ctx.A_TIMESHEET_TEMPL_R,
            ctx.A_TIMEPLAN_LIST,
            //ctx.A_PROFESSIONS, -- loaded in form
        ];

        foreach (var table in tables)
            ctx.LoadTable(table);

        DataLoaded = false;

        return true;
    }
    
    public static bool LoadPeriod(int yr1, int mt1, int yr2, int mt2)
    {
        try
        {
            return LoadPeriodA(yr1, mt1, yr2, mt2);
        }
        catch (Exception e)
        {
            Form_Error.ShowException(e, "Neizdevās ielādēt datus.");
            return false;
        }
    }

    private static bool LoadPeriodA(int yr1, int mt1, int yr2, int mt2)
    {
        ClearAll();
        FillA();

        DataLoaded = false;

        int yr0 = yr1;
        int mt0 = mt1 - 1;
        if(mt0 == 0)
        {
            mt0 = 12;
            yr0--;
        }

        var dt0 = new DateTime(yr0, mt0, 1);
        var dt2 = new DateTime(yr2, mt2, 1).LastDayOfMonth();

        TIMESHEET_LISTS_FillBy_SP_A_TIMESHEET_LISTS_01(yr0, mt0, yr2, mt2);
        TIMESHEET_LISTS_R_FillBy_SP_A_TIMESHEET_LISTS_R_01(yr0, mt0, yr2, mt2);
        TIMESHEET_FillBy_SP_A_TIMESHEET_PER_01(yr0, mt0, yr2, mt2);
        SALARY_SHEETS_FillBy_SP_A_SALARY_SHEETS_PER_01(yr0, mt0, yr2, mt2);
        SALARY_SHEETS_R_FillBy_SP_A_SALARY_SHEETS_R_PER_01(yr0, mt0, yr2, mt2);
        SALARY_PLUSMINUS_FillBy_SP_A_SALARY_PLUSMINUS_PER_01(yr0, mt0, yr2, mt2);
        PIECEWORK_R_FillBy_SP_A_PIECEWORK_PER_01(dt0, dt2);
        PAYLISTS_FillBy_SP_A_PAYLISTS_PER_01(dt0, dt2);
        PAYLISTS_R_FillBy_SP_A_PAYLISTS_R_PER_01(dt0, dt2);
        FP_PAYLISTS_FillBy_SP_A_FP_PAYLISTS_PER_01(yr0, mt0, yr2, mt2);
        FP_PAYLISTS_R_FillBy_SP_A_FP_PAYLISTS_PER_R_01(yr0, mt0, yr2, mt2);

        LoadedDT0 = new DateTime(yr0, mt0, 1);
        LoadedDT1 = new DateTime(yr1, mt1, 1);
        LoadedDT2 = new DateTime(yr2, mt2, 1).LastDayOfMonth();

        MarkFirstMonth();

        DataLoaded = true;

        return true;
    }

    public static void MarkFirstMonth()
    {
        var tableSar = MyData.DbContextA.BL_A_TIMESHEET_LISTS;
        var tableAlgasLapas = MyData.DbContextA.BL_A_SALARY_SHEETS;

        foreach (var dr in tableSar)
            dr.ISFIRSTMT = dr.YR == LoadedDT0.Year && dr.MT == LoadedDT0.Month;

        foreach (var dr in tableAlgasLapas)
            dr.ISFIRSTMT = dr.YR == LoadedDT0.Year && dr.MT == LoadedDT0.Month;
    }

    public static bool LoadSomeData()
    {
        int mtct = MyData.Params.LoadMonths;
        var dt1 = DateTime.Today.AddMonths(1);
        var dt2 = dt1.AddMonths(-mtct);
        return LoadPeriod(dt2.Year, dt2.Month, dt1.Year, dt1.Month);
    }


    public static void TIMESHEET_LISTS_FillBy_SP_A_TIMESHEET_LISTS_01(
        int PYR1, int PMT1, int PYR2, int PMT2)
    {
        MyData.DbContextA.A_TIMESHEET_LISTS
            .FromSql($"SELECT * FROM SP_A_TIMESHEET_LISTS_01({PYR1}, {PMT1}, {PYR2}, {PMT2})")
            .Load();
    }

    public static void TIMESHEET_LISTS_R_FillBy_SP_A_TIMESHEET_LISTS_R_01(
        int PYR1, int PMT1, int PYR2, int PMT2)
    {
        MyData.DbContextA.A_TIMESHEET_LISTS_R
            .FromSql($"SELECT * FROM SP_A_TIMESHEET_LISTS_R_01({PYR1}, {PMT1}, {PYR2}, {PMT2})")
            .Load();
    }

    public static void TIMESHEET_FillBy_SP_A_TIMESHEET_PER_01(
        int PYR1, int PMT1, int PYR2, int PMT2)
    {
        MyData.DbContextA.A_TIMESHEET
            .FromSql($"SELECT * FROM SP_A_TIMESHEET_PER_01({PYR1}, {PMT1}, {PYR2}, {PMT2})")
            .Load();
    }

    public static void SALARY_SHEETS_FillBy_SP_A_SALARY_SHEETS_PER_01(
        int PYR1, int PMT1, int PYR2, int PMT2)
    {
        MyData.DbContextA.A_SALARY_SHEETS
            .FromSql($"SELECT * FROM SP_A_SALARY_SHEETS_PER_01({PYR1}, {PMT1}, {PYR2}, {PMT2})")
            .Load();
    }

    public static void SALARY_SHEETS_R_FillBy_SP_A_SALARY_SHEETS_R_PER_01(
        int PYR1, int PMT1, int PYR2, int PMT2)
    {
        MyData.DbContextA.A_SALARY_SHEETS_R
            .FromSql($"SELECT * FROM SP_A_SALARY_SHEETS_R_PER_01({PYR1}, {PMT1}, {PYR2}, {PMT2})")
            .Load();
    }

    public static void SALARY_PLUSMINUS_FillBy_SP_A_SALARY_PLUSMINUS_PER_01(
        int PYR1, int PMT1, int PYR2, int PMT2)
    {
        MyData.DbContextA.A_SALARY_PLUSMINUS
            .FromSql($"SELECT * FROM SP_A_SALARY_PLUSMINUS_PER_01({PYR1}, {PMT1}, {PYR2}, {PMT2})")
            .Load();
    }

    public static void PAYLISTS_FillBy_SP_A_PAYLISTS_PER_01(
        DateTime? PDATE1, DateTime? PDATE2)
    {
        MyData.DbContextA.A_PAYLISTS
            .FromSql($"SELECT * FROM SP_A_PAYLISTS_PER_01({PDATE1}, {PDATE2})")
            .Load();
    }

    public static void PAYLISTS_R_FillBy_SP_A_PAYLISTS_R_PER_01(
        DateTime? PDATE1, DateTime? PDATE2)
    {
        MyData.DbContextA.A_PAYLISTS_R
            .FromSql($"SELECT * FROM SP_A_PAYLISTS_R_PER_01({PDATE1}, {PDATE2})")
            .Load();
    }

    public static void PIECEWORK_R_FillBy_SP_A_PIECEWORK_PER_01(
        DateTime? PDATE1, DateTime? PDATE2)
    {
        MyData.DbContextA.A_PIECEWORK
            .FromSql($"SELECT * FROM SP_A_PIECEWORK_PER_01({PDATE1}, {PDATE2})")
            .Load();
    }

    public static void FP_PAYLISTS_FillBy_SP_A_FP_PAYLISTS_PER_01(
        int PYR1, int PMT1, int PYR2, int PMT2)
    {
        MyData.DbContextA.A_FP_PAYLISTS
            .FromSql($"SELECT * FROM SP_A_FP_PAYLISTS_PER_01({PYR1}, {PMT1}, {PYR2}, {PMT2})")
            .Load();
    }

    public static void FP_PAYLISTS_R_FillBy_SP_A_FP_PAYLISTS_PER_R_01(
        int PYR1, int PMT1, int PYR2, int PMT2)
    {
        MyData.DbContextA.A_FP_PAYLISTS_R
            .FromSql($"SELECT * FROM SP_A_FP_PAYLISTS_PER_R_01({PYR1}, {PMT1}, {PYR2}, {PMT2})")
            .Load();
    }



    public static List<A_AVPAYCALC> AVPAYCALC_GetBy_SP_A_AVPAYCALC_01(
        int PIDP, int PYR1, int PMT1, int PYR2, int PMT2)
    {
        var ret = MyData.DbContextA.Database
            .SqlQuery<A_AVPAYCALC>($"SELECT * FROM SP_A_AVPAYCALC_01({PIDP}, {PYR1}, {PMT1}, {PYR2}, {PMT2})")
            .ToList();
        return ret;
    }

    public static List<A_PAYSALDO> PAYSALDO_GetBy_SP_A_PAY_SALDO_01(
        int PIDAM, DateTime? PDATE, DateTime? PDATET, int PIDPSH)
    {
        var ret = MyData.DbContextA.Database
            .SqlQuery<A_PAYSALDO>($"SELECT * FROM SP_A_PAY_SALDO_01({PIDAM}, {PDATE}, {PDATET}, {PIDPSH})")
            .ToList();
        return ret;
    }

    public static List<A_PAYSALDO> PAYSALDO_GetBy_SP_A_PAY_SALDO_02(
        int PIDS, DateTime? PDATE, DateTime? PDATET)
    {
        var ret = MyData.DbContextA.Database
            .SqlQuery<A_PAYSALDO>($"SELECT * FROM SP_A_PAY_SALDO_02({PIDS}, {PDATE}, {PDATET})")
            .ToList();
        return ret;
    }

    public static List<A_PAYSALDO> PAYSALDO_GetBy_SP_A_PAY_SALDO_03(
        DateTime? PDATE, DateTime? PDATET, int PIDPSH)
    {
        var ret = MyData.DbContextA.Database
            .SqlQuery<A_PAYSALDO>($"SELECT * FROM SP_A_PAY_SALDO_03({PDATE}, {PDATET}, {PIDPSH})")
            .ToList();
        return ret;
    }

    public static List<A_SP_PAY_MATCHLISTS_1X> SP_PAY_MATCHLISTS_1X_GetBy_SP_A_PAY_MATCHLISTS_12(
        int PIDAM, decimal PPAY, DateTime? PDATE, int PIDPSH)
    {
        var ret = MyData.DbContextA.Database
            .SqlQuery<A_SP_PAY_MATCHLISTS_1X>($"SELECT * FROM SP_A_PAY_MATCHLISTS_12({PIDAM}, {PPAY}, {PDATE}, {PIDPSH})")
            .ToList();
        return ret;
    }

    public static List<A_SP_PAY_MATCHLISTS_1X> SP_PAY_MATCHLISTS_1X_GetBy_SP_A_PAY_MATCHLISTS_13(
        int PIDS, DateTime? PDATE)
    {
        var ret = MyData.DbContextA.Database
            .SqlQuery<A_SP_PAY_MATCHLISTS_1X>($"SELECT * FROM SP_A_PAY_MATCHLISTS_13({PIDS}, {PDATE})")
            .ToList();
        return ret;
    }

    public static List<A_SP_PAY_MATCHLISTS_1X> SP_PAY_MATCHLISTS_1X_GetBy_SP_A_PAY_MATCHLISTS_14(
        DateTime? PDATE, int PIDPSH)
    {
        var ret = MyData.DbContextA.Database
            .SqlQuery<A_SP_PAY_MATCHLISTS_1X>($"SELECT * FROM SP_A_PAY_MATCHLISTS_14({PDATE}, {PIDPSH})")
            .ToList();
        return ret;
    }

    public static List<A_SP_PAY_MATCHLISTS_2X> SP_PAY_MATCHLISTS_2X_GetBy_SP_A_PAY_MATCHLISTS_21(
        int PID)
    {
        var ret = MyData.DbContextA.Database
            .SqlQuery<A_SP_PAY_MATCHLISTS_2X>($"SELECT * FROM SP_A_PAY_MATCHLISTS_21({PID}")
            .ToList();
        return ret;
    }

    public static List<A_SP_GET_MINMAX> SP_GET_MINMAX_GetBy_SP_A_GET_MINMAX()
    {
        var ret = MyData.DbContextA.Database
            .SqlQuery<A_SP_GET_MINMAX>($"SELECT * FROM SP_A_GET_MINMAX")
            .ToList();
        return ret;
    }

    public static void SP_A_MAKE_SALARY_SHEET_R_HIST(
        int PID, DateTime PDTCUR, float PSKIP_INTERVAL)
    {
        MyData.DbContextA.Database
            .ExecuteSql($"EXECUTE PROCEDURE SP_A_MAKE_SALARY_SHEET_R_HIST {PID}, {PDTCUR}, {PSKIP_INTERVAL}");
    }

    public static void SP_A_MAKE_SALARY_SHEET_R_HIST_2(
        int PID, DateTime PDTCUR, float PSKIP_INTERVAL)
    {
        MyData.DbContextA.Database
            .ExecuteSql($"EXECUTE PROCEDURE SP_A_MAKE_SALARY_SHEET_R_HIST_2 {PID}, {PDTCUR}, {PSKIP_INTERVAL}");
    }

    public static List<A_SALARY_SHEETS_R_HIST> GetBy_SP_A_SALARY_SHEETS_R_HIST_01(int PID)
    {
        var ret = MyData.DbContextA.A_SALARY_SHEETS_R_HIST
            .FromSql($"SELECT * FROM SP_A_SALARY_SHEETS_R_HIST_01({PID})")
            .AsNoTracking()
            .ToList();
        return ret;
    }

    public static List<A_SALARY_SHEETS_R_HIST> GetBy_SP_A_SALARY_SHEETS_R_HIST_02(
        int PIDSX, DateTime PTIME1, DateTime PTIME2)
    {
        var ret = MyData.DbContextA.A_SALARY_SHEETS_R_HIST
            .FromSql($"SELECT * FROM SP_A_SALARY_SHEETS_R_HIST_01({PIDSX}, {PTIME1}, {PTIME2})")
            .AsNoTracking()
            .ToList();
        return ret;
    }

    public static List<A_SP_REP_AGGREGATE> GetBy_SP_A_REP_AGGREGATE_01(
        DateTime PDT1, DateTime PDT2, int? PIDP, int? PIDAM, int PRETYRMT, int PRETIDP, int PRETIDAM)
    {
        var ret = MyData.DbContextA.Database
            .SqlQuery<A_SP_REP_AGGREGATE>($"SELECT * FROM SP_A_REP_AGGREGATE_01({PDT1}, {PDT2}, {PIDP}, {PIDAM}, {PRETYRMT}, {PRETIDP}, {PRETIDAM})")
            .ToList();
        return ret;
    }

    public static List<A_SP_REP_AGGREGATE> GetBy_SP_A_REP_AGGREGATE_02(
        DateTime PDT1, DateTime PDT2, string PIDDEP, int PRETYRMT, int PRETIDP, int PRETIDAM, int PRETIDDEP)
    {
        var ret = MyData.DbContextA.Database
            .SqlQuery<A_SP_REP_AGGREGATE>($"SELECT * FROM SP_A_REP_AGGREGATE_02({PDT1}, {PDT2}, {PIDDEP}, {PRETYRMT}, {PRETIDP}, {PRETIDAM}, {PRETIDDEP})")
            .ToList();
        return ret;
    }

    public static List<A_SP_STATS_01> GetBy_A_SP_STATS_01(
        DateTime PDT1, DateTime PDT2)
    {
        var ret = MyData.DbContextA.Database
            .SqlQuery<A_SP_STATS_01>($"SELECT * FROM SP_A_STATS_01({PDT1}, {PDT2})")
            .ToList();
        return ret;
    }

    public static List<A_SP_STATS_02> GetBy_A_SP_STATS_02(
        DateTime PDT1, DateTime PDT2)
    {
        var ret = MyData.DbContextA.Database
            .SqlQuery<A_SP_STATS_02>($"SELECT * FROM SP_A_STATS_02({PDT1}, {PDT2})")
            .ToList();
        return ret;
    }

    public static List<SP_A_REP_IINK_01> GetBy_SP_A_REP_IINK_02(
        DateTime PDT1, DateTime PDT2, DateTime PDTY)
    {
        var ret = MyData.DbContextA.Database
            .SqlQuery<SP_A_REP_IINK_01>($"SELECT * FROM SP_A_REP_IINK_02({PDT1}, {PDT2}, {PDTY})")
            .ToList();
        return ret;
    }

    public static List<SP_A_REP_IINK_01> GetBy_SP_A_REP_IINK_03(
        DateTime PDT1, DateTime PDT2)
    {
        var ret = MyData.DbContextA.Database
            .SqlQuery<SP_A_REP_IINK_01>($"SELECT * FROM SP_A_REP_IINK_03({PDT1}, {PDT2})")
            .ToList();
        return ret;
    }

    public static List<SP_A_REP_IINK_01> GetBy_SP_A_REP_IINK_32(
        DateTime PDT1, DateTime PDT2, DateTime PDTX)
    {
        var ret = MyData.DbContextA.Database
            .SqlQuery<SP_A_REP_IINK_01>($"SELECT * FROM SP_A_REP_IINK_32({PDT1}, {PDT2}, {PDTX})")
            .ToList();
        return ret;
    }

    public static List<SP_A_REP_IINK_01> GetBy_SP_A_REP_IINK_33(
        DateTime PDT1, DateTime PDT2, DateTime PDTX)
    {
        var ret = MyData.DbContextA.Database
            .SqlQuery<SP_A_REP_IINK_01>($"SELECT * FROM SP_A_REP_IINK_33({PDT1}, {PDT2}, {PDTX})")
            .ToList();
        return ret;
    }

    public static List<SP_A_REP_IINK_21> GetBy_SP_A_REP_IINK_21(
        int PYR, int PMT, DateTime PDT1, DateTime PDT2)
    {
        var ret = MyData.DbContextA.Database
            .SqlQuery<SP_A_REP_IINK_21>($"SELECT * FROM SP_A_REP_IINK_21({PYR}, {PMT}, {PDT1}, {PDT2})")
            .ToList();
        return ret;
    }

    public static List<SP_A_REP_IINK_21> GetBy_SP_A_REP_IINK_22(
        int PYR, DateTime PDT1, DateTime PDT2)
    {
        var ret = MyData.DbContextA.Database
            .SqlQuery<SP_A_REP_IINK_21>($"SELECT * FROM SP_A_REP_IINK_22({PYR}, {PDT1}, {PDT2})")
            .ToList();
        return ret;
    }

}
