using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Klons3.ModelsP;

public partial class P_DEPARTMENTS : EntityBase
{
    public int? ID { get; set; }
    public string CODE { get; set; }
    public string DESCR { get; set; }
    public int GROUP { get; set; }
    public DateTime TS { get; set; }

    public override string GetInfoView()
    {
        var ret = 
            $""""
            [Struktūrvienība] 
              Kods: {CODE}
              Nosaukums: {DESCR}
            """";
        return ret;
    }

}

