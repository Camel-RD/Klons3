using System;
using System.Collections.Generic;
using System.Xml.Linq;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_DEPARTMENTS : EntityBase
{
    public string ID { get; set; }
    public string DESCR { get; set; }
    public string ACC1 { get; set; }
    public string ACC2 { get; set; }
    public string ACC3 { get; set; }
    public string ACC4 { get; set; }
    public string ACC5 { get; set; }
    public string ACC6 { get; set; }
    public short USED { get; set; } = 1;
    public DateTime? USED_DT1 { get; set; }
    public DateTime? USED_DT2 { get; set; }
    public DateTime TS { get; set; }

    public override string GetInfoView()
    {
        var ret = $"""
                  [Struktūrvienība] 
                    Kods: {ID}
                    Nosaukums: {DESCR}
                  """;
        return ret;
    }

}
