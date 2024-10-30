using System;
using System.Collections.Generic;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_OPS : EntityBase
{
    public int ID { get; set; }
    public int DOCID { get; set; }
    public string DESCR { get; set; }
    public string AC11 { get; set; }
    public string AC12 { get; set; }
    public string AC13 { get; set; }
    public string AC14 { get; set; }
    public string AC15 { get; set; }
    public string AC21 { get; set; }
    public string AC22 { get; set; }
    public string AC23 { get; set; }
    public string AC24 { get; set; }
    public string AC25 { get; set; }
    public decimal SUMMC { get; set; }
    public string CUR { get; set; } = "EUR";
    public decimal SUMM { get; set; }
    public float? QV { get; set; }
    public short? NL { get; set; }
    public DateTime? ZDT { get; set; }
    public DateTime TS { get; set; }
    public virtual F_ACP21 AC11Row { get; set; }
    public virtual F_ACP21 AC12Row { get; set; }
    public virtual F_ACP23 AC13Row { get; set; }
    public virtual F_ACP24 AC14Row { get; set; }
    public virtual F_ACP25 AC15Row { get; set; }
    public virtual F_ACP21 AC21Row { get; set; }
    public virtual F_ACP21 AC22Row { get; set; }
    public virtual F_ACP23 AC23Row { get; set; }
    public virtual F_ACP24 AC24Row { get; set; }
    public virtual F_ACP25 AC25Row { get; set; }
    public virtual F_OPSD DOCRow { get; set; }

    public override string GetInfoView()
    {
        var ret = $"""
                  [Kontējums] 
                     Dok.nr: {DOCRow?.ZNR}, Dok.datums: {DOCRow?.DETE}, 
                     Debets: {AC11},{AC12??"__"},{AC13 ?? "__"},{AC14 ?? "__"},{AC15 ?? "__"}
                     Kredīts: {AC21},{AC22 ?? "__"},{AC23 ?? "__"},{AC24 ?? "__"},{AC25 ?? "__"}
                     Summa valūtā: {SUMMC:N2}, Valūts: {CUR}, Summa eiro: {SUMM:N2},
                     Daudzums: {QV},
                     Apraksts: {DESCR}
                  """;
        return ret;
    }

}
