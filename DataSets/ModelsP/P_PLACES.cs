using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsP;

public partial class P_PLACES : EntityBase
{
    public int? ID { get; set; }
    public string CODE { get; set; }
    public string DESCR { get; set; }
    public string ADDR { get; set; }
    public int GROUP { get; set; }
    public DateTime TS { get; set; }
    public override string GetInfoView()
    {
        var ret =
            $""""
            [Atrašanās vietas] 
              Kods: {CODE}
              Nosaukums: {DESCR}
            """";
        return ret;
    }

}
