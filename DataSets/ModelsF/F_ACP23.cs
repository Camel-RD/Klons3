using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_ACP23 : EntityBase
{
    public string IDX { get; set; }
    public string NAME { get; set; }
    public string ID1 { get; set; }
    public int? ID2 { get; set; }
    public DateTime TS { get; set; }

    public override string GetInfoView()
    {
        var ret = $"""
                  [Darijumu žurnāla konts] 
                    Konds: {IDX}
                    Nosaukums: {NAME}
                  """;
        return ret;
    }

}
