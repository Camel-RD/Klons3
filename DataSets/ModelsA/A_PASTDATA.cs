using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsA;

public partial class A_PASTDATA : EntityBase
{
    public int ID { get; set; }
    public int? IDP { get; set; }
    public int YR { get; set; }
    public int MT { get; set; }
    public decimal BRUTO { get; set; }
    public decimal PAY { get; set; }
    public int CALDAYS { get; set; }
    public int DAYSMT { get; set; }
    public float HOURSMT { get; set; }
    public int DAYS { get; set; }
    public float HOURS { get; set; }
    public int PLANDAYS { get; set; }
    public float PLANHOURS { get; set; }
    public decimal BRUTO2 { get; set; }
    public DateTime TS { get; set; }
    public virtual A_PERSONS Person { get; set; }

    public string YNAME => $"{Person?.YNAME}";
    public string ZNAME => $"{Person?.ZNAME}";

    public override string GetInfoView()
    {
        var ret = $""""
                  [Dati pirms uzskaites sākuma] 
                    Persona: {YNAME}
                    Gads: {YR}
                    Mēnesis: {MT}
                    Bruto: {BRUTO}
                    ...
                  """";
        return ret;
    }

}