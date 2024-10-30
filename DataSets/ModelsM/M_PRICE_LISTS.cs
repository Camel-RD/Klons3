using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_PRICE_LISTS : EntityBase
{
    public int ID { get; set; }
    public string CODE { get; set; } = "?";
    public string NAME { get; set; } = "?";
    public DateTime TS { get; set; }
    public virtual ObservableListViewSource<M_PRICE_LISTS_P> ForStores { get; set; } = new ();
    public virtual ObservableListViewSource<M_PRICE_LISTS_R> ForItems { get; set; } = new ();

    public override string GetInfoView()
    {
        var ret =
            $""""
            [Cenu lapa] 
              Kods: {CODE}
              Nosaukums: {NAME}
            """";
        return ret;
    }

}