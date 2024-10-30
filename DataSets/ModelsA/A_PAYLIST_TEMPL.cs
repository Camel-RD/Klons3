using System;
using System.Collections.Generic;
using System.Security.Policy;
using KlonsLIB.Data;
using KlonsLIB.Misc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Klons3.ModelsA;

public partial class A_PAYLIST_TEMPL : EntityBase
{
    public int ID { get; set; }
    public short SNR { get; set; } = 1;
    public string CODE { get; set; }
    public string DEP { get; set; }
    public string DESCR { get; set; }
    public short USED { get; set; } = 1;
    public DateTime TS { get; set; }
    public virtual ObservableListViewSource<A_PAYLIST_TEMPL_R> PayListTemplateRows { get; set; } = new ObservableListViewSource<A_PAYLIST_TEMPL_R>();
    public virtual A_DEPARTMENTS Department { get; set; }

    public override string GetInfoView()
    {
        var ret = $""""
                  [Maksājumu saraksta sagatave] 
                    Nr.: {SNR}
                    Kods: {CODE}
                    Nosaukums: {DESCR}
                    Struktūrvienība: {DEP}
                  """";
        return ret;
    }
}