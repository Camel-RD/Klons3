using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_PERSONS : EntityBase
{
    public string CLID { get; set; }
    public string NAME { get; set; }
    public string REGNR { get; set; }
    public string PVNREGNR { get; set; }
    public string TP { get; set; }
    public string TP2 { get; set; }
    public string TP3 { get; set; }
    public string ADDR { get; set; }
    public string ADDR2 { get; set; }
    public string BANKID { get; set; }
    public string BANK { get; set; }
    public string BANKACC { get; set; }
    public string PHONE { get; set; }
    public string ATK { get; set; }
    public int? ACT { get; set; }
    public DateTime TS { get; set; }

    public override string GetInfoView()
    {
        var ret = $"""
                  [Persona] 
                    Konds: {CLID}
                    Nosaukums: {NAME}
                    Reģ.nr.: {REGNR}
                    PVN reģ.nr.: {PVNREGNR}
                  """;
        return ret;
    }

}
