using Equin.ApplicationFramework;
using KlonsA.Classes;
using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;
using System.Runtime.Intrinsics.Arm;

namespace Klons3.ModelsA;

public partial class A_SALARY_SHEETS_R : EntityBase
{
    public int ID { get; set; }
    public int? IDS { get; set; }
    public int? IDSX { get; set; }
    public int? IDST { get; set; }
    public short? IS_TEMP { get; set; }
    public short? TYPE_TAG { get; set; }
    public int? IDP { get; set; }
    public int? IDAM { get; set; }
    public short SNR { get; set; } = 1;
    public string FNAME { get; set; }
    public string LNAME { get; set; }
    public string POSITION_TITLE { get; set; }
    public string TERRITORIAL_CODE { get; set; }
    public int CALENDAR_DAYS { get; set; }
    public int CALENDAR_DAYS_USE { get; set; }
    public int MONTH_WORKDAYS { get; set; }
    public float MONTH_WORKHOURS { get; set; }
    public int PLAN_DAYS { get; set; }
    public float PLAN_HOURS { get; set; }
    public float PLAN_HOURS_NIGHT { get; set; }
    public float PLAN_HOURS_OVERTIME { get; set; }
    public int FACT_DAYS { get; set; }
    public float FACT_HOURS { get; set; }
    public float FACT_HOURS_NIGHT { get; set; }
    public float FACT_HOURS_OVERTIME { get; set; }
    public int PLAN_WORK_DAYS { get; set; }
    public float PLAN_WORK_HOURS { get; set; }
    public float PLAN_WORK_HOURS_NIGHT { get; set; }
    public float PLAN_WORK_HOURS_OVERTIME { get; set; }
    public int FACT_WORK_DAYS { get; set; }
    public float FACT_WORK_HOURS { get; set; }
    public float FACT_WORK_HOURS_NIGHT { get; set; }
    public float FACT_WORK_HOURS_OVERTIME { get; set; }
    public int PLAN_HOLIDAYS_DAYS { get; set; }
    public float PLAN_HOLIDAYS_HOURS { get; set; }
    public float PLAN_HOLIDAYS_HOURS_NIGHT { get; set; }
    public float PLAN_HOLIDAYS_HOURS_OVERTIME { get; set; }
    public int FACT_HOLIDAYS_DAYS { get; set; }
    public float FACT_HOLIDAYS_HOURS { get; set; }
    public float FACT_HOLIDAYS_HOURS_NIGHT { get; set; }
    public float FACT_HOLIDAYS_HOURS_OVERTIME { get; set; }
    public decimal SALARY { get; set; }
    public decimal SALARY_DAY { get; set; }
    public decimal SALARY_NIGHT { get; set; }
    public decimal SALARY_OVERTIME { get; set; }
    public decimal SALARY_HOLIDAYS_DAY { get; set; }
    public decimal SALARY_HOLIDAYS_NIGHT { get; set; }
    public decimal SALARY_HOLIDAYS_OVERTIME { get; set; }
    public decimal SALARY_PAID_HOLIDAYS_DAY { get; set; }
    public decimal SALARY_PAID_HOLIDAYS_NIGHT { get; set; }
    public decimal SALARY_PIECEWORK { get; set; }
    public int SICKDAYS { get; set; }
    public decimal SICKDAYS_PAY { get; set; }
    public int ACCIDENT_DAYS { get; set; }
    public decimal ACCIDENT_PAY { get; set; }
    public int AVERAGE_INCOME_DAYS { get; set; }
    public decimal AVERAGE_INCOME_PAY { get; set; }
    public int FACT_AVPAY_FREE_DAYS { get; set; }
    public float FACT_AVPAY_FREE_HOURS { get; set; }
    public int FACT_AVPAY_WORK_DAYS { get; set; }
    public int FACT_AVPAY_WORKINHOLIDAYS { get; set; }
    public float FACT_AVPAY_HOURS { get; set; }
    public float FACT_AVPAY_HOURS_OVERTIME { get; set; }
    public float FACT_AVPAY_HOLIDAYS_HOURS { get; set; }
    public float FACT_AVPAY_HOLIDAYS_HOURS_OVERT { get; set; }
    public decimal SALARY_AVPAY_FREE_DAYS { get; set; }
    public decimal SALARY_AVPAY_WORK_DAYS { get; set; }
    public decimal SALARY_AVPAY_WORK_DAYS_OVERTIME { get; set; }
    public decimal SALARY_AVPAY_HOLIDAYS { get; set; }
    public decimal SALARY_AVPAY_HOLIDAYS_OVERTIME { get; set; }
    public int BUSINESS_TRIP_DAYS { get; set; }
    public decimal BUSINESS_TRIP_PAY { get; set; }
    public int PAID_HOLIDAYS { get; set; }
    public decimal PAID_HOLIDAYS_PAY { get; set; }
    public float VACATION_DAYS_CURRENT { get; set; }
    public float VACATION_DAYS_NEXT { get; set; }
    public float VACATION_HOURS_CURRENT { get; set; }
    public float VACATION_HOURS_NEXT { get; set; }
    public decimal VACATION_PAY_CURRENT { get; set; }
    public decimal VACATION_PAY_NEXT { get; set; }
    public decimal VACATION_DNS_NEXT { get; set; }
    public decimal VACATION_DDS_NEXT { get; set; }
    public decimal VACATION_IIN_NEXT { get; set; }
    public decimal VACATION_IIN_REDUCE_NEXT { get; set; }
    public decimal VACATION_CASH_NEXT { get; set; }
    public decimal VACATION_PAY_PREV { get; set; }
    public decimal VACATION_DNS_PREV { get; set; }
    public decimal VACATION_DDS_PREV { get; set; }
    public decimal VACATION_IIN_PREV { get; set; }
    public decimal PLUS_TAXED { get; set; }
    public decimal PLUS_NOTTAXED { get; set; }
    public decimal PLUS_NOSAI { get; set; }
    public decimal PLUS_AUTHORS_FEES { get; set; }
    public decimal MINUS_BEFORE_IIN { get; set; }
    public decimal MINUS_AFTER_IIN { get; set; }
    public decimal PLUS_PF_NOTTAXED { get; set; }
    public decimal PLUS_PF_TAXED { get; set; }
    public decimal PLUS_LI_NOTTAXED { get; set; }
    public decimal PLUS_LI_TAXED { get; set; }
    public decimal PLUS_HI_NOTTAXED { get; set; }
    public decimal PLUS_HI_TAXED { get; set; }
    public decimal PLUS_NP_TAXED { get; set; }
    public decimal PLUS_NP_NOSAI { get; set; }
    public decimal PLUS_NP_NOTTAXED { get; set; }
    public decimal TOTAL_BEFORE_TAXES { get; set; }
    public decimal ADJUSTED_AMOUNT { get; set; }
    public decimal AMOUNT_BEFORE_SN { get; set; }
    public decimal AMOUNT_BEFORE_SN_REVERSE { get; set; }
    public decimal RATE_DNSN { get; set; }
    public decimal RATE_DDSN { get; set; }
    public decimal DNSN_AMOUNT { get; set; }
    public decimal DDSN_AMOUNT { get; set; }
    public decimal SN_AMOUNT { get; set; }
    public decimal DNSN_AMOUNT_REVERSE { get; set; }
    public decimal DDSN_AMOUNT_REVERSE { get; set; }
    public decimal SN_MAX_AMOUNT { get; set; }
    public decimal IIN_EXEMPT_UNTAXED_MINIMUM0 { get; set; }
    public decimal IIN_EXEMPT_DEPENDANTS0 { get; set; }
    public decimal IIN_EXEMPT_RETALIATION0 { get; set; }
    public decimal IIN_EXEMPT_INVALIDITY0 { get; set; }
    public decimal IIN_EXEMPT_NATIONAL_MOVEMENT0 { get; set; }
    public decimal IIN_EXEMPT_UNTAXED_MINIMUM { get; set; }
    public decimal IIN_EXEMPT_DEPENDANTS { get; set; }
    public decimal IIN_EXEMPT_RETALIATION { get; set; }
    public decimal IIN_EXEMPT_INVALIDITY { get; set; }
    public decimal IIN_EXEMPT_NATIONAL_MOVEMENT { get; set; }
    public decimal IIN_EXEMPT_EXPENSES { get; set; }
    public short IIN_EXEMPT_2TP { get; set; }
    public decimal IIN_EXEMPT_20 { get; set; }
    public decimal IIN_EXEMPT_2 { get; set; }
    public decimal AMOUNT_BEFORE_IIN { get; set; }
    public decimal AMOUNT_BEFORE_IIN_REVERSE { get; set; }
    public decimal RATE_IIN { get; set; }
    public decimal RATE_IIN2 { get; set; }
    public decimal IIN_AMOUNT { get; set; }
    public decimal IIN_AMOUNT_REVERSE { get; set; }
    public decimal URVN_AMAOUNT { get; set; }
    public decimal PLUS_NOT_PAID { get; set; }
    public decimal VACATION_ADVANCE_CURRENT { get; set; }
    public decimal VACATION_ADVANCE_NEXT { get; set; }
    public decimal VACATION_ADVANCE_PREV { get; set; }
    public decimal ADVANCE { get; set; }
    public decimal PAY0 { get; set; }
    public decimal PAY { get; set; }
    public decimal PAYT { get; set; }
    public DateTime? PAY_DATE { get; set; }
    public string COMMENTS { get; set; }
    public decimal WITHHOLD_FROM_PAY { get; set; }
    public decimal FORAVPAYCALC_BRUTO { get; set; }
    public decimal FORAVPAYCALC_PAYOUT { get; set; }
    public int FORAVPAYCALC_DAYS { get; set; }
    public float FORAVPAYCALC_HOURS { get; set; }
    public decimal AVPAYCALC_CALDAY { get; set; }
    public decimal AVPAYCALC_DAY { get; set; }
    public decimal AVPAYCALC_HOUR { get; set; }
    public DateTime? DT_EDITED { get; set; }
    public int CALC_VER { get; set; }
    public float VACATION_DAYS_COMP { get; set; }
    public decimal VACATION_PAY_COMP { get; set; }
    public DateTime TS { get; set; }
    public virtual ObservableListViewSource<A_SALARY_PLUSMINUS> SalaryPlusMinusRows { get; set; } = new ObservableListViewSource<A_SALARY_PLUSMINUS>();

    public virtual A_POSITIONS Position { get; set; }
    public virtual A_PERSONS Person { get; set; }
    public virtual A_SALARY_SHEETS SalarySheet { get; set; }
    public virtual A_SALARY_SHEETS SalarySheetTotal { get; set; }
    public virtual A_SALARY_SHEETS_R RowSetTotalRow { get; set; }

    public virtual ObservableListViewSource<A_SALARY_SHEETS_R> RowSetRows { get; set; } = new ObservableListViewSource<A_SALARY_SHEETS_R>();

    public ESalarySheetRowType XType
    {
        get { return (ESalarySheetRowType)this.TYPE_TAG; }
        set { this.TYPE_TAG = (short)value; }
    }

    public string YNAME => $"{FNAME} {LNAME}";
    public string ZNAME => $"{FNAME} {LNAME}, {POSITION_TITLE}";


    static A_SALARY_SHEETS_R()
    {
        BindingListView<A_SALARY_SHEETS_R>.DontProvideViewForProperties = ["RowSetRows"];
    }

    public override string GetInfoView()
    {
        var ret = $""""
                  [Algu lapas rinda] 
                    Lapas gads: {SalarySheet?.YR}
                    Lapas mēnesis: {SalarySheet?.MT}
                    Lapas datums: {Utils.DateNToString(SalarySheet?.DT2)}
                    Lapas Nr.: {SalarySheet?.SNR}
                    Lapas apraksts: {SalarySheet?.DESCR}
                    Rindas Nr.: {SNR}
                    Persons: {Person?.YNAME}
                    Amats: {Position?.TITLE}
                    Bruto alga: {TOTAL_BEFORE_TAXES}
                    ...
                  """";
        return ret;
    }


}