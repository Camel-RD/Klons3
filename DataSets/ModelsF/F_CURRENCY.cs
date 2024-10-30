using KlonsLIB.Misc;
using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_CURRENCY : EntityBase
{
    public string ID { get; set; }
    public DateTime DETE { get; set; }
    public decimal RATE { get; set; }
    public DateTime TS { get; set; }

    public override string GetInfoView()
    {
        var ret = $"""
                  [Valūta] 
                    Valūta: {ID}
                    Datums: {Utils.DateToString(DETE)}
                    Likme: {RATE}
                  """;
        return ret;
    }

}