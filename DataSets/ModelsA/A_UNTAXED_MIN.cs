using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsA;

public partial class A_UNTAXED_MIN : EntityBase
{
    public int ID { get; set; }
    public int IDP { get; set; }
    public DateTime ONDATE { get; set; }
    public decimal UNTAXED_MIN { get; set; }
    public int IIN_RATE_TYPE { get; set; }
    public DateTime TS { get; set; }
    public virtual A_PERSONS Person { get; set; }
    public int FilterTag { get; set; }

    public override string GetInfoView()
    {
        var ret = $""""
                  [Neapliekamā minimuma rinda] 
                    Persona: {Person?.YNAME}
                    Datums no: {(Utils.DateToString(ONDATE))}
                    Summa: {UNTAXED_MIN}
                  """";
        return ret;
    }

}