using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_ACP21 : EntityBase
{
    public string AC { get; set; }
    public string NAME { get; set; }
    public string ID1 { get; set; }
    public string ID2 { get; set; }
    public DateTime TS { get; set; }

    public override string GetInfoView()
    {
        var ret = $"""
                  [Kontu plāna konts] 
                    Konts: {AC}
                    Nosaukums: {NAME}
                  """;
        return ret;
    }


}
