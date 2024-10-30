using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsA;

public partial class A_FP_PAYLISTS_R : EntityBase
{
    public int ID { get; set; }
    public int IDS { get; set; }
    public int? IDP { get; set; }
    public int SNR { get; set; } = 1;
    public DateTime DATE1 { get; set; }
    public DateTime DATE2 { get; set; }
    public DateTime PAYDATE { get; set; }
    public short TAX_TP { get; set; } = 1;
    public string INCOME_ID { get; set; } = "1003";
    public string DESCR { get; set; }
    public decimal PAY0 { get; set; }
    public decimal TAXED { get; set; }
    public decimal NOSAI { get; set; }
    public decimal NOTTAXED { get; set; }
    public decimal AUTHORS_FEE { get; set; }
    public short SIRATETP { get; set; }
    public decimal SIRATEDD { get; set; }
    public decimal SIRATEDN { get; set; }
    public decimal SIDD { get; set; }
    public decimal SIDN { get; set; }
    public decimal IINEX_PERC { get; set; }
    public decimal IINEX { get; set; }
    public decimal IIN_FROM { get; set; }
    public decimal IIN_RATE { get; set; }
    public decimal IIN { get; set; }
    public decimal CASH { get; set; }
    public DateTime TS { get; set; }
    public virtual A_PERSONS_FIZ Person { get; set; }
    public virtual A_FP_PAYLISTS FpPaylist { get; set; }
    
    public override string GetInfoView()
    {
        var ret = $""""
                  [Maksājumu saraksta rinda fiziskām personām] 
                    Saraksta gads: {FpPaylist?.YR}
                    Saraksta mēnesis: {FpPaylist?.MT}
                    Saraksta Nr.: {FpPaylist?.SNR}
                    Rindas Nr.: {SNR}
                    Datums no: {Utils.DateToString(DATE1)}
                    Datums līdz: {Utils.DateToString(DATE2)}
                    Maksājuma datums: {Utils.DateToString(PAYDATE)}
                    Apraksts: {DESCR}
                    Ienākuma kods: {INCOME_ID}
                    Summa: {PAY0}
                    IIN: {IIN}
                    Izmaksāts: {CASH}
                    ...
                  """";
        return ret;
    }
}