using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_OPSD : EntityBase
{
    public int ID { get; set; }
    public DateTime DETE { get; set; }
    public string NRX { get; set; }
    public string DOCTYP { get; set; }
    public string DOCST { get; set; }
    public string DOCNR { get; set; }
    public string CLID { get; set; }
    public string CLID2 { get; set; }
    public string DESCR { get; set; }
    public decimal SUMM {  get; set; }
    public decimal PVN { get; set; }
    public int OPT { get; set; }
    public int ZNR { get; set; }
    public string ZU { get; set; }
    public DateTime ZDT { get; set; }
    public DateTime? DT2 { get; set; }
    public short? NL { get; set; }
    //public short? YR { get; set; }
    public int? IDDOCM { get; set; }
    public DateTime TS { get; set; }
    public virtual F_PERSONS PersonRow { get; set; }
    public virtual F_PERSONS Person2Row { get; set; }
    public virtual F_DOCTYP DocTypeRow { get; set; }
    public virtual ObservableListViewSource<F_OPS> OPSRows { get; set; } = new ObservableListViewSource<F_OPS>();

    public override string GetInfoView()
    {
        var ret = $"""
                  [Dokuments] 
                     Nr: {ZNR}, 
                     Datums: {DETE}, 
                     Dok.veids: {DOCTYP}, sērija: {DOCST}, nr.: {DOCNR}, 
                     Persona: {CLID}, 
                     Apraksts: {DESCR},
                     Summa: {SUMM:N2},
                     PVN: {PVN:N2}
                  """;
        return ret;
    }
}
