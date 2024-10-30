using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_ACP24 : EntityBase
{
    public string IDX { get; set; }
    public string NAME { get; set; }
    public string UNIT { get; set; }
    public decimal? PRICE { get; set; }
    public DateTime TS { get; set; }

    public override string GetInfoView()
    {
        var ret = $"""
                  [Darijuma veids \ nozare] 
                    Konds: {IDX}
                    Nosaukums: {NAME}
                  """;
        return ret;
    }

}
