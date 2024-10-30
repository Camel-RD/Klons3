using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsP;

public partial class P_CATT : EntityBase
{
    public int? ID { get; set; }
    public string CODE { get; set; }
    public string DESCR { get; set; }
    public float RATE { get; set; }
    public int KIND { get; set; }
    public decimal VALUE0 { get; set; }
    public DateTime TS { get; set; }

    public override string GetInfoView()
    {
        var ret = 
            $""""
            [Pamatlīdzekļu nolietojuma kategorija nodokļiem] 
              Kods: {CODE}
              Nosaukums: {DESCR}
              Likme: {RATE}
            """";
        return ret;
    }

}
