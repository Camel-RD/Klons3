using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KlonsA.Forms;
using KlonsLIB.Misc;
using KlonsLIB.Data;
using KlonsLIB.Forms;
using KlonsF.Classes;
using Klons3.ModelsA;

namespace KlonsA.Classes;

public class Report_Salary2
{
    public static KlonsData MyData => KlonsData.St;

    public enum ERepType
    {
        WithPosNoSign,
        NoPosWithSign
    }

    public static void MakeReport(A_SALARY_SHEETS dr_algas_lapa, ERepType tp)
    {
        var sh = new SalarySheetInfo(dr_algas_lapa);
        var er = sh.SetUpLightFromSarRow(dr_algas_lapa);
        if (er != "OK")
        {
            MyData.MyMainForm.ShowWarning(er);
            return;
        }
        var rep = Report_Salary2.MakeReport2(sh);
        if (rep.Count == 0) return;

        var period = MakePeriodString(sh.DT1, sh.DT2);

        ReportViewerData rd = new ReportViewerData();
        if(tp == ERepType.WithPosNoSign)
            rd.FileName = "ReportA_AlgasLapa_1";
        else
            rd.FileName = "ReportA_AlgasLapa_2";
        rd.Sources["dsSalaryReport"] = rep;
        rd.AddReportParameters(new string[]
            {
                "CompanyName", MyData.Params.CompNameX,
                "RNr", dr_algas_lapa.SNR.ToString(),
                "RPerson", "",
                "RPeriod", period,
                "RRemark", "",
            });
        MyData.MyMainForm.ShowReport(rd);
    }

    private static string MakePeriodString(DateTime dt1, DateTime dt2)
    {
        if (dt1.Day == 1 && dt1.LastDayOfMonth() == dt2)
            return $"{dt1.Year}. gada {Utils.MonthNamesAcc[dt1.Month - 1]}";
        else
            return $"{dt1:dd.MM.yyyy} - {dt2:dd.MM.yyyy}";
    }

    public static List<SalaryRepRow2> MakeReport2(SalarySheetInfo sh)
    {
        var ret = new List<SalaryRepRow2>();
        var drs = sh.DR_algas_lapa.KIND == 1 ?
            sh.DR_algas_lapa.SalarySheetRows.OrderBy(d => d.SNR) :
            sh.DR_algas_lapa.SalarySheetTotalRows.OrderBy(d => d.SNR);
        var drsa = drs.ToArray();
        for (int i = 0; i < drsa.Length; i++)
        {
            var dr = drsa[i];
            var si = new SalaryInfo();
            si.SetFromRow(dr);
            var reprow = new SalaryRepRow2();
            reprow.SetFrom(si);
            reprow.SNR = i + 1;
            ret.Add(reprow);
        }
        return ret;
    }

    public enum EGroupBy
    {
        None,
        YearAndMonth,
        Person,
        PersonAndPosition,
        Department
    }

    public static List<SalaryRepRow2> MakeAggregatedReport(DateTime dt1, DateTime dt2,
        A_PERSONS dr_person, A_POSITIONS dr_position, A_DEPARTMENTS dr_department, EGroupBy groupby)
    {
        var ret = new List<SalaryRepRow2>();

        int retyrmt = 0;
        int retidp = 0;
        int retidam = 0;
        int retiddep = 0;

        switch (groupby)
        {
            case EGroupBy.YearAndMonth:
                retyrmt = 1;
                break;
            case EGroupBy.Person:
                retidp = 1;
                break;
            case EGroupBy.PersonAndPosition:
                retidp = 1;
                retidam = 1;
                break;
            case EGroupBy.Department:
                retiddep = 1;
                break;
        }

        List<A_SP_REP_AGGREGATE> table = null;
        if (retiddep == 1 || dr_department != null)
            table = DataLoaderA.GetBy_SP_A_REP_AGGREGATE_02(dt1, dt2, dr_department?.ID, retyrmt, retidp, retidam, retiddep);
        else
            table = DataLoaderA.GetBy_SP_A_REP_AGGREGATE_01(dt1, dt2, dr_person?.ID, dr_position?.ID, retyrmt, retidp, retidam);

        for (int i = 0; i < table.Count; i++)
        {
            var dr = table[i];
            var reprow = new SalaryRepRow2();
            reprow.SetFrom(dr);
            reprow.SNR = i + 1;
            
            switch (groupby)
            {
                case EGroupBy.YearAndMonth:
                    reprow.Caption = $"{dr.YR} - {dr.MT:00}";
                    break;
                case EGroupBy.Person:
                    var rdr_person = MyData.DbContextA.BL_A_PERSONS.FindById(dr.IDP.Value);
                    reprow.Caption = rdr_person.YNAME;
                    break;
                case EGroupBy.PersonAndPosition:
                    rdr_person = MyData.DbContextA.BL_A_PERSONS.FindById(dr.IDP.Value);
                    var rdr_position = MyData.DbContextA.BL_A_POSITIONS.FindById(dr.IDAM.Value);
                    reprow.Caption = $"{rdr_person.YNAME}, {rdr_position.TITLE}";
                    break;
                case EGroupBy.Department:
                    var rdr_dep = MyData.DbContextA.A_DEPARTMENTS.Find(dr.IDDEP);
                    reprow.Caption = rdr_dep.DESCR;
                    break;
            }

            ret.Add(reprow);
        }

        ret = ret.OrderBy(d => d.Caption).ToList();
        return ret;
    }


}

public class SalaryRepRow2
{
    public int SNR { get; set; } = 0;
    public string Caption { get; set; } = "";
    public string Caption2 { get; set; } = "";
    public float WorkHours { get; set; } = 0.0f;
    public int WorkDays { get; set; } = 0;
    public int FreeAvPayDays { get; set; } = 0;
    public float VacationDays { get; set; } = 0f;
    public float VacationDaysComp { get; set; } = 0f;
    public int SickDays { get; set; } = 0;
    public decimal WorkPay { get; set; } = 0.0M;
    public decimal SickPay { get; set; } = 0.0M;
    public decimal VacationPay { get; set; } = 0.0M;
    public decimal VacationPayComp { get; set; } = 0.0M;
    public decimal FreeAvPay { get; set; } = 0.0M;
    public decimal WorkAvPay { get; set; } = 0.0M;
    public decimal PlusTaxed { get; set; } = 0.0M;
    public decimal PlusNotTaxed { get; set; } = 0.0M;
    public decimal PlusAuthorsFees { get; set; } = 0.0M;
    public decimal PlusNoSAI { get; set; } = 0.0M;
    public decimal TotalPay { get; set; } = 0.0M;
    public decimal ForSAI { get; set; } = 0.0M;
    public decimal DNSAI { get; set; } = 0.0M;
    public decimal DDSAI { get; set; } = 0.0M;
    public decimal SAI { get; set; } = 0.0M;
    public decimal UntaxedMinimum { get; set; } = 0.0M;
    public decimal ExDependants { get; set; } = 0.0M;
    public decimal ExInvalidity { get; set; } = 0.0M;
    public decimal ExRetaliation { get; set; } = 0.0M;
    public decimal ExNatMovement { get; set; } = 0.0M;
    public decimal ExExpenses { get; set; } = 0.0M;
    public decimal Ex2 => ExExpenses + ExNatMovement + ExRetaliation + ExInvalidity;
    public decimal IIN { get; set; } = 0.0M;
    public decimal MinusBeforeIIN { get; set; } = 0.0M;
    public decimal MinusAfterIIN { get; set; } = 0.0M;
    public decimal AdvanceOrDebt { get; set; } = 0.0M;
    public decimal Pay { get; set; } = 0.0M;
    public decimal PayT { get; set; } = 0.0M;
    public decimal Paid { get; set; } = 0.0M;

    public void SetFrom(SalaryInfo si)
    {
        SNR = si._SNR;
        Caption = $"{si._FNAME} {si._LNAME}, {si._POSITION_TITLE.ToLower()}";
        Caption2 = $"{si._FNAME} {si._LNAME}";
        WorkDays = si._FACT_DAYS;
        WorkHours = si._FACT_HOURS;
        FreeAvPayDays = si._FACT_AVPAY_FREE_DAYS;
        VacationDays = si._VACATION_DAYS_CURRENT;
        VacationDaysComp = si._VACATION_DAYS_COMP;
        SickDays = si._SICKDAYS;
        WorkPay = si._SALARY;
        SickPay = si._SICKDAYS_PAY;
        VacationPay = si._VACATION_PAY_CURRENT;
        VacationPayComp = si._VACATION_PAY_COMP;
        FreeAvPay = si._SALARY_AVPAY_FREE_DAYS;
        WorkAvPay = 
            si._SALARY_AVPAY_HOLIDAYS + 
            si._SALARY_AVPAY_HOLIDAYS_OVERTIME + 
            si._SALARY_AVPAY_WORK_DAYS + 
            si._SALARY_AVPAY_WORK_DAYS_OVERTIME;
        PlusTaxed = 
            si._PLUS_TAXED + 
            si._PLUS_PF_TAXED + 
            si._PLUS_LI_TAXED + 
            si._PLUS_HI_TAXED;
        PlusNotTaxed = 
            si._PLUS_NOTTAXED + 
            si._PLUS_PF_NOTTAXED + 
            si._PLUS_LI_NOTTAXED + 
            si._PLUS_HI_NOTTAXED;
        PlusNoSAI = si._PLUS_NOSAI;
        PlusAuthorsFees = si._PLUS_AUTHORS_FEES;
        TotalPay = si._TOTAL_BEFORE_TAXES;
        ForSAI = si._AMOUNT_BEFORE_SN;
        DDSAI = si._DDSN_AMOUNT;
        DNSAI = si._DNSN_AMOUNT;
        SAI = si._SN_AMOUNT;
        UntaxedMinimum = si._IIN_EXEMPT_UNTAXED_MINIMUM;
        ExDependants = si._IIN_EXEMPT_DEPENDANTS;
        ExInvalidity = si._IIN_EXEMPT_INVALIDITY;
        ExRetaliation = si._IIN_EXEMPT_RETALIATION;
        ExNatMovement = si._IIN_EXEMPT_NATIONAL_MOVEMENT;
        ExExpenses = si._IIN_EXEMPT_EXPENSES;
        IIN = si._IIN_AMOUNT;
        MinusBeforeIIN = si._MINUS_BEFORE_IIN;
        MinusAfterIIN = si._MINUS_AFTER_IIN;
        AdvanceOrDebt = si._ADVANCE;
        Pay = si._PAYT;
    }

    public void SetFrom(A_SP_REP_AGGREGATE dr)
    {
        WorkDays = dr.WORKDAYS;
        WorkHours = dr.WORKHOURS;
        FreeAvPayDays = dr.FREEAVPAYDAYS;
        VacationDays = dr.VACATIONDAYS;
        VacationDaysComp = dr.VACATIONDAYSCOMP;
        SickDays = dr.SICKDAYS;
        WorkPay = dr.WORKPAY;
        SickPay = dr.SICKPAY;
        VacationPay = dr.VACATIONPAY;
        VacationPayComp = dr.VACATIONPAYCOMP;
        FreeAvPay = dr.FREEAVPAY;
        WorkAvPay = dr.WORKAVPAY;
        PlusTaxed = dr.PLUS_TAXED;
        PlusNotTaxed = dr.PLUS_NOTTAXED;
        PlusNoSAI = dr.PLUS_NOSAI;
        PlusAuthorsFees = dr.PLUS_AUTHORS_FEES;
        TotalPay = dr.TOTALPAY;
        ForSAI = dr.FORSAI;
        DDSAI = dr.DDSN_AMOUNT;
        DNSAI = dr.DNSN_AMOUNT;
        SAI = DDSAI + DNSAI;
        UntaxedMinimum = dr.UNTAXED_MINIMUM;
        ExDependants = dr.IIN_EXEMPT_DEPENDANTS;
        ExInvalidity = dr.IIN_EXEMPT_INVALIDITY;
        ExRetaliation = dr.IIN_EXEMPT_RETALIATION;
        ExNatMovement = dr.IIN_EXEMPT_NATIONAL_MOVEMENT;
        ExExpenses = dr.IIN_EXEMPT_EXPENSES;
        IIN = dr.IIN_AMOUNT;
        MinusBeforeIIN = dr.MINUS_BEFORE_IIN;
        MinusAfterIIN = dr.MINUS_AFTER_IIN;
        Pay = dr.PAY;
        AdvanceOrDebt = dr.ADVANCE;
        PayT = dr.PAYT;
        Paid = dr.PAID;
    }

}
