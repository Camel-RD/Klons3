using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_DOCS0 : EntityBase
{
    public int ID { get; set; }
    public DateTime DETE { get; set; }
    public string DOCTYP { get; set; }
    public string DOCST { get; set; }
    public string DOCNR { get; set; }
    public string CLID { get; set; }
    public string DESCR { get; set; }
    public decimal SUMM {  get; set; }
    public decimal PVN { get; set; }
    public string AC { get; set; }
    public DateTime TS { get; set; }
    public virtual F_ACP21 ACRows { get; set; }
    public virtual F_PERSONS PersonRow { get; set; }
    public virtual F_DOCTYP DOCTYPRow { get; set; }

    public override string GetInfoView()
    {
        var ret = $"""
                  [Neapmaksātie rēķini] 
                    Datums: {DETE}
                    Dok.veids: {DOCTYP}, sērija: {DOCST}, nr.: {DOCNR}
                    Konts: {AC}
                    Person: {CLID}
                    Summa: {SUMM:N2}, PVN: {PVN:N2}
                  """;
        return ret;
    }

}
