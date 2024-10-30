using KlonsLIB.Misc;
using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using System.Xml.Linq;

namespace Klons3.ModelsA;

public partial class A_PERSONS_R : EntityBase
{
    public int ID { get; set; }
    public int IDP { get; set; }
    public DateTime EDIT_DATE { get; set; }
    public string FNAME { get; set; } = "?";
    public string LNAME { get; set; } = "?";
    public string PERSON_CODE { get; set; } = "?";
    public string TERRITORIAL_CODE { get; set; }
    public string TAXDOC_SERIAL { get; set; }
    public string TAXDOC_NO { get; set; }
    public string TAXDOC_ISSUER { get; set; }
    public string TAXREG_NO { get; set; }
    public short INVALID { get; set; }
    public short PENSIONER { get; set; }
    public short PRISONER { get; set; }
    public short REPRES { get; set; }
    public short PRET { get; set; }
    public short APGAD_SK { get; set; }
    public short PENSIONER_SP { get; set; }
    public short NOT_OSA { get; set; }
    public DateTime TS { get; set; }
    public virtual A_PERSONS Person { get; set; }

    public string EDIT_DATE_STR => Utils.DateToString(EDIT_DATE);

    public override string GetInfoView()
    {
        var ret = $""""
                  [Persons datu labojuma ieraksts] 
                    Persona: {Person?.ZNAME}
                    Vārds: {FNAME}
                    Uzvārds: {LNAME}
                    PK: {PERSON_CODE}
                    Labojuma datums: {Utils.DateToString(EDIT_DATE)}
                    ...
                  """";
        return ret;
    }

}