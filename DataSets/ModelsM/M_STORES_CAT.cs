using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_STORES_CAT : EntityBase
{
    public int ID { get; set; }
    public string CODE { get; set; } = "?";
    public string NAME { get; set; } = "?";
    public int ISGROUP { get; set; }
    public int? IDPRICES { get; set; }
    public int? IDDISCOUNTS { get; set; }
    public DateTime TS { get; set; }

    public override string GetInfoView()
    {
        var ret =
            $""""
            [Noliktavu kategorija] 
              Kods: {CODE}
              Nosaukums: {NAME}
            """";
        return ret;
    }

}
