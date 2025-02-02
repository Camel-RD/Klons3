﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KlonsLIB.Misc;
using KlonsLIB.Data;
using KlonsF.Classes;
using Klons3.ModelsA;

namespace KlonsA.Classes
{
    public class AvPayCalcInfo
    {
        public static KlonsData MyData => KlonsData.St;

        public bool PreparingReport = false;
        public List<AvPayCalcRow> ReportRows = null;
        public AvPayCalcRow TotalRow = null;
        public bool UsingMinRate = false;
        public decimal RateHour = 0.0M;
        public decimal RateDay = 0.0M;
        public decimal RateCalendarDay = 0.0M;
        public string FName = null;
        public string LName = null;
        public int Year = 0;
        public int Month = 0;

        public AvPayCalcInfo(bool filllist)
        {
            PreparingReport = filllist;
            if(PreparingReport)
                SetUp();
        }

        public void SetUp()
        {
            ReportRows = new List<AvPayCalcRow>();
            for(int i = 0; i < 6; i++)
            {
                var pm = new AvPayCalcRow();
                pm.SNR = i + 1;
                pm.Caption = pm.SNR.ToString() + ".";
                ReportRows.Add(pm);
            }
            TotalRow = new AvPayCalcRow();
            TotalRow.SNR = 7;
            TotalRow.Caption = "Kopā";
            ReportRows.Add(TotalRow);
        }

        public ErrorList CalcAvPay(SalarySheetRowSetInfo srs, SalaryInfo si)
        {
            var err_list = CalcList(srs.SalarySheet, srs.DR_Person, srs.TotalPersonPay);
            si._AVPAYCALC_HOUR = KlonsData.RoundA(RateHour, 4);
            si._AVPAYCALC_DAY = KlonsData.RoundA(RateDay, 4);
            return err_list;
        }

        public ErrorList CalcList(A_SALARY_SHEETS_R dr)
        {
            var dr_sar = dr.SalarySheet;
            var srs = new SalarySheetRowSetInfo();
            var err = srs.SetUpFromRowX(dr);
            if (err.HasErrors)
                return err;
            return CalcList(srs.SalarySheet, srs.DR_Person, srs.TotalPersonPay);
        }

        public ErrorList CalcList(SalarySheetRowInfo sr)
        {
            if (sr.SalarySheetRowSet == null)
                sr.CheckLinkedRows();
            if (sr.SalarySheetRowSet.TotalPersonPay._FNAME == null)
                sr.SalarySheetRowSet.TotalPersonPay._FNAME = sr.DR_Person_r.FNAME;
            if (sr.SalarySheetRowSet.TotalPersonPay._LNAME == null)
                sr.SalarySheetRowSet.TotalPersonPay._LNAME = sr.DR_Person_r.LNAME;
            return CalcList(sr.SalarySheet, sr.Row.Person, sr.SalarySheetRowSet.TotalPersonPay);
        }

        public ErrorList CalcList(SalarySheetInfo sh, A_PERSONS dr_person, SalaryInfo si)
        {
            var err_list = new ErrorList();

            var dt1 = sh.MDT1;
            var dt1a = sh.MDT1.AddMonths(-1);
            var dt2 = sh.MDT1.AddMonths(-5);
            var dt3 = sh.MDT1.AddMonths(-11);
            int yr1 = sh.CalendarMonth.Year;
            int mt1 = sh.CalendarMonth.Month;
            int yr1a = yr1;
            int mt1a = mt1;
            Utils.AddMonths(ref yr1a, ref mt1a, -1);
            int yr2 = dt2.Year;
            int mt2 = dt2.Month;
            int yr3 = dt3.Year;
            int mt3 = dt3.Month;

            if (PreparingReport)
            {
                this.Year = yr1;
                this.Month = mt1;
                this.FName = si._FNAME;
                this.LName = si._LNAME;
            }

            var drs_avpaycalc = DataLoaderA.AVPAYCALC_GetBy_SP_A_AVPAYCALC_01(dr_person.ID, yr3, mt3, yr1a, mt1a);

            int k1 = yr1 * 12 + mt1 - 1;
            int k2 = yr2 * 12 + mt2 - 1;
            int k3 = yr3 * 12 + mt3 - 1;

            decimal sbruto = 0.0M;
            decimal sbruto2 = 0.0M;
            int sdays = 0;
            int splandays = 0;
            float shours = 0.0f;

            bool backto12 = false;

            var drs_m = drs_avpaycalc.Where(
                d =>
                {
                    int k = (d.YR * 12 + d.MT - 1);
                    return k >= k2 && k < k1;
                }).ToArray();

            foreach (var dr in drs_m)
            {
                sbruto += dr.FORAVPAYCALC_BRUTO;
                sbruto2 += dr.TOTAL_BEFORE_TAXES;
                sdays += dr.FORAVPAYCALC_DAYS;
                splandays += dr.PLAN_WORKDAYS;
                shours += dr.FORAVPAYCALC_HOURS;
            }

            sbruto += si._FORAVPAYCALC_BRUTO;
            //jo aprēķina laikā kopējā bruto alga nav zināma
            if (sh.YR > 2022 || sh.YR == 2022 && sh.MT > 2)
                sbruto2 += si._FORAVPAYCALC_BRUTO;
            else
                sbruto2 += si._TOTAL_BEFORE_TAXES;
            sdays += si._FORAVPAYCALC_DAYS;
            splandays += si._PLAN_WORK_DAYS;
            shours += si._FORAVPAYCALC_HOURS;

            if (sdays == 0 || shours == 0.0f || sbruto2 == 0.0M)
            {
                backto12 = true;

                drs_m = drs_avpaycalc.Where(
                    d =>
                    {
                        int k = (d.YR * 12 + d.MT - 1);
                        return d.TYPE_TAG != (short)ESalarySheetRowType.OneOfMany && k >= k3 && k < k2;
                    }).ToArray();

                sbruto = 0.0M;
                sbruto2 = 0.0M;
                sdays = 0;
                splandays = 0;
                shours = 0.0f;

                foreach (var dr in drs_m)
                {
                    sbruto += dr.FORAVPAYCALC_BRUTO;
                    sbruto2 += dr.TOTAL_BEFORE_TAXES;
                    sdays += dr.FORAVPAYCALC_DAYS;
                    splandays += dr.PLAN_WORKDAYS;
                    shours += dr.FORAVPAYCALC_HOURS;
                }
            }

            if (sdays == 0 || shours == 0.0f || sbruto2 == 0.0M)
            {
                UsingMinRate = true;
                RateHour = sh.DR_Likmes.MIN_PAY_HOUR;
                RateDay = RateHour * 8;
                RateCalendarDay = sh.DR_Likmes.MIN_PAY_MONTH / 30.42M;
            }
            else
            {
                UsingMinRate = false;
                RateHour = KlonsData.RoundA(sbruto / (decimal)shours, 4);
                RateDay = KlonsData.RoundA(sbruto / (decimal)sdays, 4);
                RateCalendarDay = KlonsData.RoundA(sbruto / (decimal)splandays, 4);

                if (PreparingReport)
                {
                    var drl = ReportRows[5];
                    if (!backto12)
                    {
                        drl.Year = yr1;
                        drl.Month = mt1;
                        drl.CalendarDays = si._CALENDAR_DAYS_USE;
                        drl.DaysPlan = si._PLAN_DAYS;
                        drl.WorkDaysPlan = si._PLAN_WORK_DAYS;
                        drl.MonthWorkDays = si._MONTH_WORKDAYS;
                        drl.MonthWorkHours = si._MONTH_WORKHOURS;
                        drl.DaysFact = si._FORAVPAYCALC_DAYS;
                        drl.HoursPlan = si._PLAN_HOURS;
                        drl.WorkHoursPlan = si._PLAN_WORK_HOURS;
                        drl.HoursFact = si._FORAVPAYCALC_HOURS;
                        drl.Salary = si._FORAVPAYCALC_BRUTO;
                        drl.Salary2 = si._TOTAL_BEFORE_TAXES;
                        drl.Pay = si._FORAVPAYCALC_PAYOUT;
                        drl.Caption = $"{drl.SNR}. {drl.Year} {drl.Month:00}";
                    }

                    for (int i = 0; i < drs_m.Length; i++)
                    {
                        var dr = drs_m[drs_m.Length - i - 1];
                        var snr = (dr.YR * 12 + dr.MT - 1) - (backto12 ? k3 : k2) + 1;
                        if (snr < 1 || snr > 6) continue;
                        if(!backto12 && snr == 6) continue;

                        drl = ReportRows[snr-1];

                        drl.Year = dr.YR;
                        drl.Month = dr.MT;
                        drl.CalendarDays += dr.CALENDAR_DAYS;
                        drl.MonthWorkDays += dr.MONTH_WORKDAYS;
                        drl.MonthWorkHours += dr.MONTH_WORKHOURS;
                        drl.DaysPlan += dr.PLAN_DAYS;
                        drl.WorkDaysPlan += dr.PLAN_WORKDAYS;
                        drl.DaysFact += dr.FORAVPAYCALC_DAYS;
                        drl.HoursPlan += dr.PLAN_HOURS;
                        drl.WorkHoursPlan += dr.PLAN_WORKHOURS;
                        drl.HoursFact += dr.FORAVPAYCALC_HOURS;
                        drl.Salary += dr.FORAVPAYCALC_BRUTO;
                        drl.Salary2 += dr.TOTAL_BEFORE_TAXES;
                        drl.Pay += dr.FORAVPAYCALC_PAYOUT;
                        drl.Caption = $"{drl.SNR}. {drl.Year} {drl.Month:00}";
                    }

                    for (int i = 0; i < ReportRows.Count - 1; i++)
                    {
                        drl = ReportRows[i];
                        TotalRow.CalendarDays += drl.CalendarDays;
                        TotalRow.MonthWorkDays += drl.MonthWorkDays;
                        TotalRow.MonthWorkHours += drl.MonthWorkHours;
                        TotalRow.DaysPlan += drl.DaysPlan;
                        TotalRow.WorkDaysPlan += drl.WorkDaysPlan;
                        TotalRow.DaysFact += drl.DaysFact;
                        TotalRow.HoursPlan += drl.HoursPlan;
                        TotalRow.WorkHoursPlan += drl.WorkHoursPlan;
                        TotalRow.HoursFact += drl.HoursFact;
                        TotalRow.Salary += drl.Salary;
                        TotalRow.Salary2 += drl.Salary2;
                        TotalRow.Pay += drl.Pay;
                    }
                }
            }

            return err_list;
        }

        public void AddCurMonthPay(int yr, int mt, decimal pay)
        {
            foreach(var rr in ReportRows)
            {
                if (rr.Year == yr && rr.Month == mt)
                    rr.Pay += pay;
            }
            TotalRow.Pay += pay;
        }

        public void SetCurMonthPay(int yr, int mt, decimal bruto, decimal pay)
        {
            decimal ds2 = 0.0M;
            foreach (var rr in ReportRows)
            {
                if (rr.Year == yr && rr.Month == mt)
                {
                    ds2 = bruto - rr.Salary2;
                    rr.Salary2 = bruto;
                    rr.Pay = pay;
                }
            }
            TotalRow.Pay += pay;
            TotalRow.Salary2 += ds2;
        }

        public void SetAvPayTo(SalaryCalcInfo sc)
        {
            sc.SetAvPayFrom(this, RateHour, RateDay, RateCalendarDay);
        }

        public void SetAvPayTo(SalaryCalcTInfo sc)
        {
            sc.SetAvPayFrom(this, RateHour, RateDay, RateCalendarDay);
        }

    }

    public class AvPayCalcRow
    {
        public int SNR { get; set; } = 0;
        public int Year { get; set; }
        public int Month { get; set; }
        public string Caption { get; set; } = "";
        public decimal Salary { get; set; } = 0.0M;
        public decimal Salary2 { get; set; } = 0.0M;
        public decimal Pay { get; set; } = 0.0M;
        public int CalendarDays { get; set; } = 0;
        public int MonthWorkDays { get; set; } = 0;
        public float MonthWorkHours { get; set; } = 0.0f;
        public int DaysPlan { get; set; } = 0;
        public int WorkDaysPlan { get; set; } = 0;
        public int DaysFact { get; set; } = 0;
        public float HoursPlan { get; set; } = 0.0f;
        public float WorkHoursPlan { get; set; } = 0.0f;
        public float HoursFact { get; set; } = 0.0f;
    }

}
