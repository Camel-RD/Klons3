using KlonsA.Classes;
using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using KlonsLIB.Misc;

namespace Klons3.ModelsA;

public partial class A_POSITIONS_PLUSMINUS : EntityBase
{
    public int ID { get; set; }
    public int IDP { get; set; }
    public int? IDA { get; set; }
    public DateTime? DATE1 { get; set; }
    public DateTime? DATE2 { get; set; }
    public int? IDSV { get; set; } = 1;
    public int? IDNO { get; set; } = 1;
    public int? IDIE { get; set; }
    public string DESCR { get; set; }
    public decimal RATE { get; set; }
    public short RATE_TYPE { get; set; } = 0;
    public short IS_INAVPAY { get; set; } = 1;
    public short IS_PAID { get; set; } = 1;
    public DateTime TS { get; set; }
    public virtual A_POSITIONS Position { get; set; }
    public virtual A_PLUSMINUS_FROM PlusMinusFromRow { get; set; }
    public virtual A_PERSONS Person { get; set; }
    public virtual A_PLUSMINUS_TYPES PlusMinusType { get; set; }

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
            if (this.IDSV == null) return EBonusType.None;
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
                  [Amata piemaksas/atvilkumi] 
                    Persona: {Person?.YNAME}
                    Amats: {Position?.TITLE}
                    Datums no: {Utils.DateNToString(DATE1)}
                    Datums līdz: {Utils.DateNToString(DATE2)}
                    Apraksts: {DESCR}
                    Aprēķina bāze: {PlusMinusFromRow?.DESCR}
                    Piemaksas/atvilkuma veids: {PlusMinusType?.DESCR}
                    Likme: {RATE}
                    Likmes veids: {(RATE_TYPE == 0 ? "%" : "€")}
                    Iekļaut vid.izp.apr.: {(IS_INAVPAY == 0? "nē" : "jā")}
                  """";
        return ret;
    }

}
