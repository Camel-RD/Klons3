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
    public class SalarySheetInfo
    {
        public static KlonsData MyData => KlonsData.St;

        public int YR;
        public int MT;
        public DateTime MDT1;
        public DateTime MDT2;
        public DateTime DT1;
        public DateTime DT2;

        public CalendarMonthInfo CalendarMonth = null;
        public A_RATES DR_Likmes = null;

        public A_SALARY_SHEET_TEMPL DR_algas_lapu_sh = null;
        public A_SALARY_SHEETS DR_algas_lapa = null;
        public List<SalarySheetRowInfo> Rows = new List<SalarySheetRowInfo>();

        public A_SALARY_SHEETS TotalsList = null;

        public SalarySheetInfo(int yr, int mt)
        {
            YR = yr;
            MT = mt;
            MDT1 = new DateTime(YR, MT, 1);
            MDT2 = MDT1.AddMonths(1).AddDays(-1);
            DT1 = MDT1;
            DT2 = MDT2;
            CalendarMonth = new CalendarMonthInfo(YR, MT);
        }

        public SalarySheetInfo(SalarySheetInfo si)
        {
            YR = si.YR;
            MT = si.MT;
            MDT1 = si.MDT1;
            MDT2 = si.MDT2;
            DT1 = MDT1;
            DT2 = MDT2;
            CalendarMonth = si.CalendarMonth;
        }

        public SalarySheetInfo(A_SALARY_SHEETS dr_lapa) :this(dr_lapa.YR, dr_lapa.MT)
        {
            DR_algas_lapa = dr_lapa;
            DT1 = dr_lapa.DT1.Value;
            DT2 = dr_lapa.DT2.Value;
        }


        public void MakeSheetFromSh(short snr0)
        {
            var table_sar = MyData.DbContextA.GetBindingListByType<A_SALARY_SHEETS>();
            var table_sar_r = MyData.DbContextA.GetBindingListByType<A_SALARY_SHEETS_R>();

            var dr_sh = DR_algas_lapu_sh;

            CheckTotalListForPeriod();

            var new_dr_sar = table_sar.NewItem();

            new_dr_sar.YR = YR;
            new_dr_sar.MT = MT;
            new_dr_sar.SNR = snr0;
            new_dr_sar.Department = dr_sh.Department;
            new_dr_sar.DESCR = dr_sh.DESCR;
            new_dr_sar.DT1 = MDT1;
            new_dr_sar.DT2 = MDT2;
            new_dr_sar.XKind = ESalarySheetKind.Normal;
            new_dr_sar.IS_TEMP = (short)((DT2 == DT2.LastDayOfMonth()) ? 0 : 1);

            table_sar.Add(new_dr_sar);

            DR_algas_lapa = new_dr_sar;

            foreach (var salary_sheet_row in Rows)
            {
                salary_sheet_row.MakeRowFromSH();
            }
        }


        public A_SALARY_SHEETS MakeTotalsSheet()
        {
            return SalarySheetInfo.MakeTotalsSheet(DT1, DT2);
        }

        public static A_SALARY_SHEETS MakeTotalsSheet(DateTime dt1, DateTime dt2)
        {
            var table_sar = MyData.DbContextA.GetBindingListByType<A_SALARY_SHEETS>();
            var new_dr_sar = table_sar.NewItem();

            new_dr_sar.YR = dt2.Year;
            new_dr_sar.MT = dt2.Month;
            new_dr_sar.SNR = 0;
            new_dr_sar.DESCR = "KOPĀ";
            new_dr_sar.DT1 = dt1;
            new_dr_sar.DT2 = dt2;
            new_dr_sar.XKind = ESalarySheetKind.Total;
            new_dr_sar.IS_TEMP = (short)((dt2 == dt2.LastDayOfMonth()) ? 0 : 1);

            table_sar.Add(new_dr_sar);

            return new_dr_sar;
        }

        public ErrorList SetUpFromSHRow(A_SALARY_SHEET_TEMPL dr_sh)
        {
            DR_algas_lapu_sh = dr_sh;

            var error_list = new ErrorList();
            var this_error_source = $"Lapa:{dr_sh.SNR}";

            var er = GetLikmes();
            if (er != "OK")
            {
                error_list.AddError(this_error_source, er);
                return error_list;
            }

            var drs_sh_r = dr_sh.SalarySheetTemplateRows.OrderBy(d => d.SNR);

            foreach (var dr_sh_r in drs_sh_r)
            {

                var salary_sheet_row = new SalarySheetRowInfo(this);
                var err = salary_sheet_row.SetUpFromSHRow(dr_sh_r);

                if (err.Count > 0)
                {
                    error_list.AddRange(err);
                    continue;
                }

                Rows.Add(salary_sheet_row);
            }

            return error_list;
        }

        public ErrorList SetUpFromSarRow(A_SALARY_SHEETS dr_lapa)
        {
            var error_list = new ErrorList();

            var er = SetUpLightFromSarRow(dr_lapa);
            if (er != "OK") return new ErrorList("", er);

            var drs_r = dr_lapa.XKind == ESalarySheetKind.Total ?
                dr_lapa.SalarySheetTotalRows.OrderBy(d => d.SNR) :
                dr_lapa.SalarySheetRows.OrderBy(d => d.SNR);

            foreach (var dr_r in drs_r)
            {
                var salary_sheet_row = new SalarySheetRowInfo(this);
                var err = salary_sheet_row.SetUpFromRowX(dr_r);
                if (err.HasErrors) return err;
                Rows.Add(salary_sheet_row);
            }

            return error_list;
        }

        public string SetUpLightFromSarRow(A_SALARY_SHEETS dr_lapa)
        {
            DR_algas_lapa = dr_lapa;
            DT1 = dr_lapa.DT1.Value;
            DT2 = dr_lapa.DT2.Value;
            var er = GetLikmes();
            if (er != "OK") return er;
            CheckTotalListForPeriod();
            return "OK";
        }


        public void CheckTotalListForPeriod()
        {
            if (TotalsList != null) return;
            var dr_total_sar = FindTotalListForPeriod();
            if(dr_total_sar != null)
            {
                TotalsList = dr_total_sar;
                return;
            }
            TotalsList = MakeTotalsSheet();
        }

        public static void CheckTotalListForPeriod(DateTime dt1, DateTime dt2)
        {
            var dr_total_sar = FindTotalListForPeriod(dt1, dt2);
            if (dr_total_sar != null) return;
            MakeTotalsSheet(dt1, dt2);
        }

        public ErrorList FillSheet()
        {
            if (DR_algas_lapa == null)
                throw new Exception("Bad init.");

            var error_list = new ErrorList();

            var drs = DR_algas_lapa.SalarySheetRows;
            foreach (var dr in drs)
            {
                var salary_sheet_row = new SalarySheetRowSetInfo();
                var err = salary_sheet_row.SetUpFromRowB(this, dr.Person);
                error_list.AddRange(err);
                if (err.Count > 0) continue;
                err = salary_sheet_row.FillRow();
                error_list.AddRange(err);
            }

            return error_list;
        }

        public void DeleteRow(A_SALARY_SHEETS_R dr_lapas_r)
        {
            var table_algas_ps = MyData.DbContextA.GetBindingListByType<A_SALARY_PLUSMINUS>();
            CheckTotalListForPeriod();
            var drs_ps = table_algas_ps.Where(
                d =>
                d.SalarySheetRow == dr_lapas_r.RowSetTotalRow &&
                d.Position != null &&
                d.Position == dr_lapas_r.Position
                ).ToArray();
            foreach (var dr in drs_ps)
                table_algas_ps.Remove(dr);
            MyData.DbContextA.A_SALARY_SHEETS_R.Remove(dr_lapas_r);
            var salary_sheet_row = new SalarySheetRowSetInfo();
            var err = salary_sheet_row.SetUpFromRowB(this, dr_lapas_r.Person);
            if (err.Count > 0) return;
            if (salary_sheet_row.DrLinkedRows.Length == 0 && 
                salary_sheet_row.DrTotalRow == null) return;
            salary_sheet_row.FillRow();
        }


        public void DeleteListRows()
        {
            if (DR_algas_lapa == null)
                throw new Exception("Bad init.");

            var drs = DR_algas_lapa.SalarySheetRows;
            foreach (var dr in drs)
            {
                DeleteRow(dr);
            }
        }

        public void DeleteList()
        {
            if (DR_algas_lapa == null)
                throw new Exception("Bad init.");
            DeleteListRows();
            MyData.DbContextA.A_SALARY_SHEETS.Remove(DR_algas_lapa);
            var drt = FindTotalListForPeriod();
            if (drt != null && !HasSalarySheetsForPeriod())
            {
                //DataTasks.DeleteSalarySheet(drt);
                MyData.DbContextA.A_SALARY_SHEETS.Remove(drt);
            }
        }

        public bool HasSalarySheetsForPeriod()
        {
            var table = MyData.DbContextA.GetBindingListByType<A_SALARY_SHEETS>();
            var drf = table.Where(
                d =>
                d.XKind == ESalarySheetKind.Normal &&
                d.YR == YR &&
                d.MT == MT &&
                d.DT1 == DT1 &&
                d.DT2 == DT2
                ).FirstOrDefault();
            return drf != null;
        }

        public void ChangeIDSXForAlgasPSRows(int new_idsx, A_SALARY_SHEETS_R dr_totalrow)
        {
            var drs_ps = dr_totalrow.SalaryPlusMinusRows;
            foreach (var dr in drs_ps)
            {
                if (dr.IDSX != new_idsx)
                    dr.IDSX = new_idsx;
            }
        }

        public A_SALARY_SHEETS_R MakeTotalsRow(A_PERSONS dr_person)
        {
            if (TotalsList == null)
                throw new Exception("Bad init.");

            var table_sar_r = MyData.DbContextA.GetBindingListByType<A_SALARY_SHEETS_R>();
            var new_dr_sar_r = table_sar_r.NewItem();

            //new_dr_sar_r.ID = (int)MyData.KlonsAQueriesTableAdapter.SP_A_GEN_SALARY_SHEETS_R_ID();
            new_dr_sar_r.SalarySheet = TotalsList;
            new_dr_sar_r.RowSetTotalRow = new_dr_sar_r;
            new_dr_sar_r.SalarySheetTotal = TotalsList;
            new_dr_sar_r.Person = dr_person;
            new_dr_sar_r.Position = null;
            new_dr_sar_r.XType = ESalarySheetRowType.Total;
            //new_dr_sar_r.FNAME = DR_Person_r.FNAME;
            //new_dr_sar_r.LNAME = DR_Person_r.LNAME;
            new_dr_sar_r.POSITION_TITLE = "KOPĀ";
            new_dr_sar_r.IS_TEMP = TotalsList.IS_TEMP;

            table_sar_r.Add(new_dr_sar_r);

            return new_dr_sar_r;
        }


        public A_SALARY_SHEETS FindTotalListForPeriod()
        {
            return SalarySheetInfo.FindTotalListForPeriod(DT1, DT2);
        }

        public static A_SALARY_SHEETS FindTotalListForPeriod(DateTime dt1, DateTime dt2)
        {
            var table_sar = MyData.DbContextA.GetBindingListByType<A_SALARY_SHEETS>();
            var drs_sar = table_sar.Where(d =>
                d.XKind == ESalarySheetKind.Total &&
                d.YR == dt2.Year &&
                d.MT == dt2.Month &&
                d.DT1 == dt1 &&
                d.DT2 == dt2
            ).ToArray();
            if (drs_sar.Length > 1)
                throw new Exception("More than 1 totals list.");
            if (drs_sar.Length == 0) return null;
            return drs_sar[0];
        }

        public string GetLikmes()
        {
            var drs_likmes = MyData.DbContextA.BL_A_RATES.OrderBy(d => d.ONDATE).ToArray();

            DR_Likmes = PeriodInfo.FindNearestBefore(drs_likmes,
                MDT1, d => d.ONDATE);

            if (DR_Likmes == null)
            {
                return "Likmes nav definētas.";
            }

            return "OK";
        }

    }
}
