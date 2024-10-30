using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_ACP25 : EntityBase
{
    public string IDX { get; set; }
    public string NAME { get; set; }
    public string ID1 { get; set; }
    public DateTime TS { get; set; }

    public override string GetInfoView()
    {
        var ret = $"""
                  [PVN kods] 
                    Konds: {IDX}
                    Nosaukums: {NAME}
                  """;
        return ret;
    }

}
