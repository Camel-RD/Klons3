using KlonsA.Classes;
using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsA;

public partial class A_SALARY_PLUSMINUS : EntityBase
{
    public int ID { get; set; }
    public int IDP { get; set; }
    public int? IDA { get; set; }
    public int? IDAP { get; set; }
    public int? IDSX { get; set; }
    public int? IDSV { get; set; } = 1;
    public int? IDNO { get; set; }
    public int? IDIE { get; set; }
    public string DESCR { get; set; }
    public decimal RATE { get; set; }
    public short RATE_TYPE { get; set; }
    public decimal AMOUNT { get; set; }
    public short IS_INAVPAY { get; set; } = 1;
    public short IS_PAID { get; set; } = 1;
    public DateTime TS { get; set; }
    public virtual A_POSITIONS Position { get; set; }
    public virtual A_POSITIONS_PLUSMINUS PositionsPlusMinusRow { get; set; }
    public virtual A_PLUSMINUS_FROM PlusMinusFrom { get; set; }
    public virtual A_PERSONS Person { get; set; }
    public virtual A_PLUSMINUS_TYPES PlusMinusType { get; set; }
    public virtual A_SALARY_SHEETS_R SalarySheetRow { get; set; }

    public EBonusRateType XRateType
    {
        get
        {
            return (EBonusRateType)this.RATE_TYPE;
        }
        set
        {
            if (value == EBonusRateType.Money)
                this.RATE_TYPE = 0;
            else
                this.RATE_TYPE = 1;
        }
    }

    public EBonusFrom XBonusFrom
    {
        get
        {
            if (this.IDNO == null) return EBonusFrom.None;
            return (EBonusFrom)this.IDNO;
        }
        set
        {
            if (value == EBonusFrom.None)
                this.IDNO = null;
            else
                this.IDNO = (int)value;
        }
    }

    public EBonusType XBonusType
    {
        get
        {
            return (EBonusType)this.IDSV;
        }
        set
        {
            if (value == EBonusType.None)
                this.IDSV = null;
            else
                this.IDSV = (int)value;
        }
    }

    public override string GetInfoView()
    {
        var ret = $""""
                  [Algas piemaksas/atvilkumi] 
                    Saraksta datums: {Utils.DateNToString(SalarySheetRow?.SalarySheet?.DT2)}
                    Saraksta Nr.: {SalarySheetRow?.SalarySheet?.SNR}
                    Saraksta gads.: {SalarySheetRow?.SalarySheet?.YR}
                    Saraksta mēnesis: {SalarySheetRow?.SalarySheet?.MT}
                    Saraksta rindas Nr.: {SalarySheetRow?.SNR}
                    Persons: {Person?.YNAME}
                    Amats: {Position?.TITLE}
                    Aprēķina bāze: {PlusMinusFrom?.DESCR}
                    Piemaksas/atvilkums veids: {PlusMinusType?.DESCR}
                    Apraksts: {DESCR}
                    Likme: {RATE}
                    Likmes veids: {(RATE_TYPE == 0 ? "%" : "€")}
                    Summa: {AMOUNT}
                    ...
                  """";
        return ret;
    }

}