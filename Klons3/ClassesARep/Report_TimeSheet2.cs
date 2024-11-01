﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KlonsA.Forms;
using KlonsLIB.Misc;
using KlonsLIB.Data;
using KlonsLIB.Forms;
using DataObjectsA;
using KlonsF.Classes;
using Klons3.ModelsA;

namespace KlonsA.Classes
{
    public class ReportTimeSheet2
    {
        public static KlonsData MyData => KlonsData.St;

        public List<TimeReportRow2> ReportRows = new List<TimeReportRow2>();
        public List<TimeReportRow2a> ReportRowsA = new List<TimeReportRow2a>();

        public void MakeReport(A_TIMESHEET_LISTS dr_sar, string title)
        {
            if (dr_sar == null) return;
            var drs_r = dr_sar.TimesheetListRows;
            var dt1 = new DateTime(dr_sar.YR, dr_sar.MT, 1);
            var dt2 = dt1.LastDayOfMonth();
            var sickc = new SickDayCalcRow();
            int vacdays = 0;
            float vachours = 0.0f;

            ReportRows = new List<TimeReportRow2>();
            ReportRowsA = new List<TimeReportRow2a>();

            foreach (var dr_r in drs_r)
            {
                var trr = new TimeReportRow2();
                var trr2 = new TimeReportRow2a(trr);

                trr.Caption = $"{dr_r.Person.FNAME} {dr_r.Person.LNAME}, {dr_r.Position.TITLE}";

                var rowset = dr_r.GetDLRowSet();
                rowset.CountPlan(trr, dt1, dt2);
                rowset.CountFact(trr, dt1, dt2);

                rowset.CountSickDays2(dt1, dt2, out int sickdaysA, out int sickdaysB, out int sickdaysN);
                trr._SICKDAYS = sickdaysA;
                trr2.SickdaysA = sickdaysA;
                trr2.SickdaysB = sickdaysB;
                trr2.SickdaysN = sickdaysN;

                rowset.CountVacationTime(dt1, dt2, out vacdays, out vachours);
                trr._VACATION_DAYS_CURRENT = vacdays;
                trr._VACATION_HOURS_CURRENT = vachours;

                ReportRows.Add(trr);
                ReportRowsA.Add(trr2);
            }

            ShowReport(dt1.Year, dt1.Month, dr_sar.DESCR);
        }

        public void ShowReport(int yr, int mt, string title)
        {
            var period = $"{yr}. gada {Utils.MonthNames[mt - 1]}";

            ReportViewerData rd = new ReportViewerData();
            rd.FileName = "ReportA_DarbaLaiks_2";
            rd.Sources["DataSet1"] = ReportRowsA;
            rd.AddReportParameters(new string[]
                {
                    "CompanyName", MyData.Params.CompNameX,
                    "RPerson", "",
                    "RPeriod", period,
                    "RTitle", title,
                    "RRemark", "",
                });
            MyData.MyMainForm.ShowReport(rd);
        }
    }

    public class TimeReportRow2 : IWorkTimeData
    {
        public string Caption { get; set; } = "";

        public int _CALENDAR_DAYS { get; set; } = 0;
        public int _CALENDAR_DAYS_USE { get; set; } = 0;
        public int _MONTH_WORKDAYS { get; set; } = 0;
        public float _MONTH_WORKHOURS { get; set; } = 0.0f;

        public int _PLAN_DAYS { get; set; } = 0;
        public float _PLAN_HOURS { get; set; } = 0.0f;
        public float _PLAN_HOURS_NIGHT { get; set; } = 0.0f;
        public float _PLAN_HOURS_OVERTIME { get; set; } = 0.0f;

        public int _FACT_DAYS { get; set; } = 0;
        public float _FACT_HOURS { get; set; } = 0.0f;
        public float _FACT_HOURS_NIGHT { get; set; } = 0.0f;
        public float _FACT_HOURS_OVERTIME { get; set; } = 0.0f;

        public int _PLAN_WORK_DAYS { get; set; } = 0;
        public float _PLAN_WORK_HOURS { get; set; } = 0.0f;
        public float _PLAN_WORK_HOURS_NIGHT { get; set; } = 0.0f;
        public float _PLAN_WORK_HOURS_OVERTIME { get; set; } = 0.0f;

        public int _FACT_WORK_DAYS { get; set; } = 0;
        public float _FACT_WORK_HOURS { get; set; } = 0.0f;
        public float _FACT_WORK_HOURS_NIGHT { get; set; } = 0.0f;
        public float _FACT_WORK_HOURS_OVERTIME { get; set; } = 0.0f;

        public int _PLAN_HOLIDAYS_DAYS { get; set; } = 0;
        public float _PLAN_HOLIDAYS_HOURS { get; set; } = 0.0f;
        public float _PLAN_HOLIDAYS_HOURS_NIGHT { get; set; } = 0.0f;
        public float _PLAN_HOLIDAYS_HOURS_OVERTIME { get; set; } = 0.0f;

        public int _FACT_HOLIDAYS_DAYS { get; set; } = 0;
        public float _FACT_HOLIDAYS_HOURS { get; set; } = 0.0f;
        public float _FACT_HOLIDAYS_HOURS_NIGHT { get; set; } = 0.0f;
        public float _FACT_HOLIDAYS_HOURS_OVERTIME { get; set; } = 0.0f;

        public int _SICKDAYS { get; set; } = 0;
        public int _ACCIDENT_DAYS { get; set; } = 0;
        public int _AVERAGE_INCOME_DAYS { get; set; } = 0;

        public int _FACT_AVPAY_FREE_DAYS { get; set; } = 0;
        public float _FACT_AVPAY_FREE_HOURS { get; set; } = 0.0f;
        public int _FACT_AVPAY_WORK_DAYS { get; set; } = 0;
        public int _FACT_AVPAY_WORKINHOLIDAYS { get; set; } = 0;

        public float _FACT_AVPAY_HOURS { get; set; } = 0.0f;
        public float _FACT_AVPAY_HOURS_OVERTIME { get; set; } = 0.0f;
        public float _FACT_AVPAY_HOLIDAYS_HOURS { get; set; } = 0.0f;
        public float _FACT_AVPAY_HOLIDAYS_HOURS_OVERT { get; set; } = 0.0f;

        public int _BUSINESS_TRIP_DAYS { get; set; } = 0;
        public int _PAID_HOLIDAYS { get; set; } = 0;

        public float _VACATION_DAYS_CURRENT { get; set; } = 0;
        public float _VACATION_DAYS_NEXT { get; set; } = 0;
        public float _VACATION_HOURS_CURRENT { get; set; } = 0.0F;
        public float _VACATION_HOURS_NEXT { get; set; } = 0.0F;

    }

    public class TimeReportRow2a
    {
        public TimeReportRow2 Row = null;
        public TimeReportRow2a() { }

        public TimeReportRow2a(TimeReportRow2 r)
        {
            Row = r;
        }

        public string Caption => Row.Caption;
        public int PlanDays => Row._PLAN_DAYS;
        public float PlanHours => Row._PLAN_HOURS;
        public float PlanHoursNight => Row._PLAN_HOURS_NIGHT;
        public float PlanHoursOvertime => Row._PLAN_HOURS_OVERTIME;
        public int FactDays => Row._FACT_DAYS - FactAvpayFreeDays;
        public float FactHours => Row._FACT_HOURS - FactAvpayFreeHours;
        public float FactHoursNight => Row._FACT_HOURS_NIGHT;
        public float FactHoursOvertime => Row._FACT_HOURS_OVERTIME;
        public int PlanWorkDays => Row._PLAN_WORK_DAYS;
        public float PlanWorkHours => Row._PLAN_WORK_HOURS;
        public float PlanWorkHoursNight => Row._PLAN_WORK_HOURS_NIGHT;
        public float PlanWorkHoursOvertime => Row._PLAN_WORK_HOURS_OVERTIME;
        public int FactWorkDays => Row._FACT_WORK_DAYS;
        public float FactWorkHours => Row._FACT_WORK_HOURS;
        public float FactWorkHoursNight => Row._FACT_WORK_HOURS_NIGHT;
        public float FactWorkHoursOvertime => Row._FACT_WORK_HOURS_OVERTIME;
        public int PlanHolidaysDays => Row._PLAN_HOLIDAYS_DAYS;
        public float PlanHolidaysHours => Row._PLAN_HOLIDAYS_HOURS;
        public float PlanHolidaysHoursNight => Row._PLAN_HOLIDAYS_HOURS_NIGHT;
        public float PlanHolidaysHoursOvertime => Row._PLAN_HOLIDAYS_HOURS_OVERTIME;
        public int FactHolidaysDays => Row._FACT_HOLIDAYS_DAYS;
        public float FactHolidaysHours => Row._FACT_HOLIDAYS_HOURS;
        public float FactHolidaysHoursNight => Row._FACT_HOLIDAYS_HOURS_NIGHT;
        public float FactHolidaysHoursOvertime => Row._FACT_HOLIDAYS_HOURS_OVERTIME;
        public int Sickdays => Row._SICKDAYS;
        public int SickdaysA { get; set; } = 0;
        public int SickdaysB { get; set; } = 0;
        public int SickdaysN { get; set; } = 0;
        public int AccidentDays => Row._ACCIDENT_DAYS;
        public int AverageIncomeDays => Row._AVERAGE_INCOME_DAYS;
        public int FactAvpayFreeDays => Row._FACT_AVPAY_FREE_DAYS;
        public float FactAvpayFreeHours => Row._FACT_AVPAY_FREE_HOURS;
        public int FactAvpayWorkDays => Row._FACT_AVPAY_WORK_DAYS;
        public int FactAvpayWorkinholidays => Row._FACT_AVPAY_WORKINHOLIDAYS;
        public float FactAvpayHours => Row._FACT_AVPAY_HOURS;
        public float FactAvpayHoursOvertime => Row._FACT_AVPAY_HOURS_OVERTIME;
        public float FactAvpayHolidaysHours => Row._FACT_AVPAY_HOLIDAYS_HOURS;
        public float FactAvpayHolidaysHoursOvert => Row._FACT_AVPAY_HOLIDAYS_HOURS_OVERT;
        public int BusinessTripDays => Row._BUSINESS_TRIP_DAYS;
        public int PaidHolidays => Row._PAID_HOLIDAYS;
        public float VacationDaysCurrent => Row._VACATION_DAYS_CURRENT;
        public float VacationDaysNext => Row._VACATION_DAYS_NEXT;
        public float VacationHoursCurrent => Row._VACATION_HOURS_CURRENT;
        public float VacationHoursNext => Row._VACATION_HOURS_NEXT;
    }
}
