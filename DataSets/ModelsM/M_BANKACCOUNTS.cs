using System;
using System.Collections.Generic;
using System.Xml.Linq;
using KlonsLIB.Data;
using Microsoft.Reporting.Map.WebForms.BingMaps;

namespace Klons3.ModelsM;

public partial class M_BANKACCOUNTS : EntityBase
{
    public int ID { get; set; }
    public int? IDBANK { get; set; }
    public int? IDSTORE { get; set; }
    public string ACCOUNT { get; set; } = "?";
    public DateTime TS { get; set; }
    public virtual M_BANKS Bank { get; set; }
    public virtual M_STORES Store { get; set; }

    public override string GetInfoView()
    {
        var ret =
            $""""
            [Bankas konts] 
              Noliktava/persona: {Store?.NAME}
              Banka: {Bank?.NAME}
              Konts: {ACCOUNT}
            """";
        return ret;
    }

}