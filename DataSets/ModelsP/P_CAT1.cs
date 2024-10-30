using System;
using System.Collections.Generic;
using Klons3.ModelsA;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsP;

public partial class P_CAT1 : EntityBase
{
    public int? ID { get; set; }
    public string CODE { get; set; }
    public string DESCR { get; set; }
    public int KIND { get; set; }
    public int IDCD { get; set; }
    public int IDCT { get; set; }
    public int GROUP { get; set; }
    public DateTime TS { get; set; }
    public virtual P_CATD CATDRow { get; set; }
    public virtual P_CATT CATTRow { get; set; }

    public override string GetInfoView()
    {
        var ret = 
            $""""
            [Pamatlīdzekļu kategorija] 
              Kods: {CODE}
              Nosaukums: {DESCR}
            """";
        return ret;
    }

}
