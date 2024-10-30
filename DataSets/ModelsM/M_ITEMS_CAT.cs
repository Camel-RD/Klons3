using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_ITEMS_CAT : EntityBase
{
    public int ID { get; set; }
    public string NAME { get; set; } = "?";
    public string CODE { get; set; } = "?";
    public int ISGROUP { get; set; }
    public int METHOD { get; set; }
    public int ISSERVICES { get; set; }
    public int ISPRODUCED { get; set; }
    public string ACC6 { get; set; } = ".?";
    public string ACC7 { get; set; } = ".?";
    public DateTime TS { get; set; }

    public KlonsM.Classes.EStoreCalcMethod XMethod
    {
        get => (KlonsM.Classes.EStoreCalcMethod)METHOD;
        set => METHOD = (int)value;
    }

    public bool XIsServices
    {
        get => ISSERVICES == 1;
        set => ISSERVICES = value ? 1 : 0;
    }

    public bool XIsProduced
    {
        get => ISPRODUCED == 1;
        set => ISPRODUCED = value ? 1 : 0;
    }

    public override string GetInfoView()
    {
        var ret =
            $""""
            [Artikulu kategorija] 
              Kods: {CODE}
              Nosaukums: {NAME}
              ...
            """";
        return ret;
    }

}
