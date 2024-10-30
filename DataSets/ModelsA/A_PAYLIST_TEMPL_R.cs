using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_PAYLIST_TEMPL_R : EntityBase
{
    public int ID { get; set; }
    public int IDS { get; set; }
    public int? IDP { get; set; }
    public int? IDAM { get; set; }
    public int SNR { get; set; } = 1;
    public DateTime TS { get; set; }
    public virtual A_POSITIONS Position { get; set; }
    public virtual A_PERSONS Person { get; set; }
    public virtual A_PAYLIST_TEMPL PaylistTemplate { get; set; }

    public override string GetInfoView()
    {
        var ret = $""""
                  [Maksājumu saraksta sagataves rinda] 
                    Saraksta kods: {PaylistTemplate.CODE}
                    Nr.: {SNR}
                    Persona: {Person?.YNAME}
                    Amats: {Position?.TITLE}
                  """";
        return ret;
    }
}