using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_PERSONS_FIZ : EntityBase
{
    public int ID { get; set; }
    public string FNAME { get; set; } = "?";
    public string LNAME { get; set; } = "?";
    public string NAME_DATIVE { get; set; }
    public string NAME_ACCUSATIVE { get; set; }
    public short GENDER { get; set; }
    public DateTime BIRTH_DATE { get; set; }
    public string PK { get; set; }
    public string ADDRESS { get; set; }
    public string CITY { get; set; }
    public string STATE { get; set; }
    public string COUNTRY { get; set; }
    public string POSTAL_CODE { get; set; }
    public string TERRITORIAL_CODE { get; set; }
    public string PASSPORT_NO { get; set; }
    public string PASSPORT_ISSUER { get; set; }
    public DateTime? PASSPORT_DATE { get; set; }
    public string PHONE { get; set; }
    public string EMAIL { get; set; }
    public int? BANK_ID { get; set; }
    public string BANK_ACC { get; set; }
    public string TAXREG_NO { get; set; }
    public string COMMENTS { get; set; }
    public short USED { get; set; } = 1;
    public DateTime? USED_DT1 { get; set; }
    public DateTime? USED_DT2 { get; set; }
    public DateTime TS { get; set; }

    public virtual A_BANKS Bank { get; set; }

    public string ZNAME => $"{FNAME} {LNAME}";

    public override string GetInfoView()
    {
        var ret = $""""
                  [Fiziskas persons] 
                    Persona: {ZNAME}
                    ...
                  """";
        return ret;
    }

}