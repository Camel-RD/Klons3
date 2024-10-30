using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsA;

public partial class A_PIECEWORK_CATALOG : EntityBase
{
    public int ID { get; set; }
    public int? IDS { get; set; }
    public string CODE { get; set; } = "?";
    public string DESCR { get; set; } = "?";
    public string UNIT { get; set; } = "gab.";
    public decimal RATE { get; set; }
    public int TIMEUNIT { get; set; }
    public float TIMEUSE { get; set; }
    public float TIMEUSEINHOURS { get; set; }
    public short USED { get; set; } = 1;
    public DateTime TS { get; set; }

    public virtual A_PIECEWORK_CATSTRUCT CatalogSection { get; set; }

    public string KATCODE => $"{CatalogSection?.CODE}";

    public override string GetInfoView()
    {
        var ret = $""""
                  [Gabaldarba uzskaites katalogs] 
                    Kataloga sekcija: {CatalogSection?.CODE}
                    Kods: {CODE}
                    Nosaukums: {DESCR}
                    Vienības: {UNIT}
                    Likme: {RATE}
                    ...
                  """";
        return ret;
    }
}