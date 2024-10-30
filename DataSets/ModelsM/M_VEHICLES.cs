using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_VEHICLES : EntityBase
{
    public int ID { get; set; }
    public int? IDSTORE { get; set; }
    public string NAME { get; set; } = "?";
    public string REGNR { get; set; }
    public DateTime TS { get; set; }
    public virtual M_STORES Store { get; set; }

    public override string GetInfoView()
    {
        var ret =
            $""""
            [Transporta līdzeklis] 
              Partneris\noliktava: {Store?.NAME}
              Nosaukums: {NAME}
            """";
        return ret;
    }

}
