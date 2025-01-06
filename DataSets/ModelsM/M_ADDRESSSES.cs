using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_ADDRESSSES : EntityBase
{
    public int ID { get; set; }
    public int? IDSTORE { get; set; }
    public int? IDCOUNTRY { get; set; }
    public string NAME { get; set; } = "?";
    public string ADDRESS { get; set; } = "?";
    public string STREET { get; set; }
    public string CITY { get; set; }
    public string STATE { get; set; }
    public string PARISH { get; set; }
    public string POSTALCODE { get; set; }
    public string COMMENTS { get; set; }


    public DateTime TS { get; set; }
    public virtual M_COUNTRIES Country { get; set; }
    public virtual M_STORES Store { get; set; }

    public override string GetInfoView()
    {
        var ret =
            $""""
            [Adrese] 
              Noliktava/persona: {Store?.NAME}
              Nosaukums: {NAME}
              Adrese: {ADDRESS}
            """";
        return ret;
    }

}
