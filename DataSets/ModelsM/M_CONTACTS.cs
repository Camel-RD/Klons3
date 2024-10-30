using System;
using System.Collections.Generic;
using System.Security.Principal;
using KlonsLIB.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Klons3.ModelsM;

public partial class M_CONTACTS : EntityBase
{
    public int ID { get; set; }
    public int? IDSTORE { get; set; }
    public string NAME { get; set; } = "?";
    public string DESCR { get; set; }
    public string PHONE { get; set; }
    public string EMAIL { get; set; }
    public DateTime TS { get; set; }
    public virtual M_STORES Store { get; set; }

    public override string GetInfoView()
    {
        var ret =
            $""""
            [Kontakti] 
              Noliktava/persona: {Store?.NAME}
              Nosaukums: {NAME}
              Apraksts: {DESCR}
              Telefons: {PHONE}
              E-pasts: {EMAIL}
            """";
        return ret;
    }

}
