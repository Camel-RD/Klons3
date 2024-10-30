using KlonsP.Classes;
using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Klons3.ModelsP;

public partial class P_ITEMS_EVENTS : EntityBase
{
    public int ID { get; set; }
    public int IDIT { get; set; }
    public int SNR { get; set; } = 1;
    public int EVENT { get; set; }
    public DateTime DT { get; set; }
    public DateTime DTREG { get; set; }
    public string DESCR { get; set; }
    public string DOCNR { get; set; }
    public int CAT1 { get; set; }
    public int CATD { get; set; }
    public int CATT { get; set; }
    public int PLACE { get; set; }
    public int DEPARTMENT { get; set; }
    public decimal VALUE_0 { get; set; }
    public decimal DEPREC_0 { get; set; }
    public decimal VALUE_LEFT { get; set; }
    public decimal VALUE_C { get; set; }
    public decimal DEPREC_C { get; set; }
    public decimal SELL_VALUE { get; set; }
    public float RATE_D { get; set; }
    public decimal RATE_D_MT { get; set; }
    public int MT_TOTAL { get; set; }
    public int MT_USED { get; set; }
    public decimal TAX_VAL { get; set; }
    public decimal TAX_VAL_LEFT { get; set; }
    public decimal TAX_VAL_C { get; set; }
    public float TAX_RATE { get; set; }
    public int TAX_EACH { get; set; }
    public int CHCOLSET { get; set; }
    public DateTime? ZDT { get; set; }
    public string ZU { get; set; }
    public DateTime TS { get; set; }
    public virtual P_CAT1 CAT1Row { get; set; }
    public virtual P_CATD CATDRow { get; set; }
    public virtual P_CATT CATTRow { get; set; }
    public virtual P_DEPARTMENTS DEPARTMENTRow { get; set; }
    public virtual P_EVENTS EVENTRow { get; set; }
    public virtual P_ITEMS ITEMRow { get; set; }
    public virtual P_PLACES PLACERow { get; set; }

    public EEvent XEvent
    {
        get { return (EEvent)this.EVENT; }
        set
        {
            if (this.EVENT == (int)value) return;
            this.EVENT = (int)value;
        }
    }

    public EChColInd XChColSet
    {
        get { return (EChColInd)this.CHCOLSET; }
        set
        {
            if (this.CHCOLSET == (int)value) return;
            this.CHCOLSET = (int)value;
        }
    }

    public string XREGNR => ITEMRow?.REG_NR;
    public string XNAME => ITEMRow?.NAME;
    public int XCAT1 => ITEMRow.CAT1;
    public int XCATD => ITEMRow.CATD;
    public int XCATT => ITEMRow.CATT;
    public int XPLACE => ITEMRow.PLACE;
    public int XDEPARTMENT => ITEMRow.DEPARTMENT;

    public static string ToMyStringFull(EEvent ev)
    {
        switch (ev)
        {
            case EEvent.error: return "kļūda";
            case EEvent.nenoteikts: return "?";
            case EEvent.pienuzsk: return "pieņemts uzskaitē";
            case EEvent.iegeks: return "iegāde";
            case EEvent.ieg: return "iegāde";
            case EEvent.izv: return "izveide";
            case EEvent.eks: return "nodots ekspluatācijā";
            case EEvent.vieta: return "pārvietots";
            case EEvent.parvert: return "pārvērtēšana";
            case EEvent.rekat: return "kategorijas maiņa";
            case EEvent.kapit: return "kapitālremonts";
            case EEvent.nelieto: return "pārtrauc lietošanu";
            case EEvent.lieto: return "atsāk lietošanau";
            case EEvent.likvid: return "likvidēts";
            case EEvent.noliet: return "nolietojums";
            case EEvent.apr: return "aprēķins";
            default: return"?";
        }
    }

    public override string GetInfoView()
    {
        var ret =
            $""""
            [Pamatlīdzekļa uzskaites notikums] 
              Pl. Nr.: {ITEMRow?.REG_NR}
              Nosaukums: {ITEMRow?.NAME}
              Datums: {Utils.DateToString(DT)}
              Notikums: {ToMyStringFull(XEvent)}
              ...
            """";
        return ret;
    }


}