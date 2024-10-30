using System;
using System.Collections.Generic;
using System.Xml.Linq;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsA;

public partial class A_PIECEWORK : EntityBase
{
    public int ID { get; set; }
    public int? IDK { get; set; }
    public int? IDP { get; set; }
    public int? IDA { get; set; }
    public DateTime DT { get; set; }
    public string UNIT { get; set; } = "gab.";
    public decimal QUANTITY { get; set; }
    public decimal RATE { get; set; }
    public float BONUS { get; set; }
    public decimal PAY { get; set; }
    public int TIMEUNIT { get; set; }
    public float TIMEUSE { get; set; }
    public float TIMEUSEINHOURS { get; set; }
    public DateTime TS { get; set; }
    public virtual A_POSITIONS Position { get; set; }
    public virtual A_PIECEWORK_CATALOG Catalog { get; set; }
    public virtual A_PERSONS Person { get; set; }

    public string KATCODE => $"{Catalog?.CODE}";

    public override string GetInfoView()
    {
        var ret = $""""
                  [Gabaldarba uzskaites ieraksts] 
                    Persona: {Person?.ZNAME}
                    Amats: {Position?.TITLE}
                    Datums: {Utils.DateToString(DT)}
                    Vienības: {UNIT}
                    Daudzums: {QUANTITY}
                    Likme: {RATE}
                    ...
                  """";
        return ret;
    }


}