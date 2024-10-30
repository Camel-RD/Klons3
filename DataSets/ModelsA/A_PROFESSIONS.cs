using System;
using System.Collections.Generic;
using KlonsA.Classes;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsA;

public partial class A_PROFESSIONS : EntityBase
{
    public string ID { get; set; }
    public string DESCR { get; set; }
    public short? CAT { get; set; }
    public short? USED { get; set; } = 1;
    public DateTime TS { get; set; }
    
    public override string GetInfoView()
    {
        var ret = $""""
                  [Profesiju klasifikatora ieraksts] 
                    Kods: {ID}
                    Kods: {CAT}
                    Nosaukums: {DESCR}
                  """";
        return ret;
    }

}