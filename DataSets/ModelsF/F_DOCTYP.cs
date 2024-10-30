using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_DOCTYP : EntityBase
{
    public string ID { get; set; }
    public string NAME { get; set; }
    public string ID1 { get; set; }
    public string NAME1 { get; set; }
    public string TPZ { get; set; }
    public string PVNPAZ { get; set; }
    public DateTime TS { get; set; }

    public override string GetInfoView()
    {
        var ret = $"""
                  [Dokumenta veids] 
                    Konds: {ID}
                    Nosaukums: {NAME}
                  """;
        return ret;
    }

}
