using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_PVNRATES : EntityBase
{
    public int ID { get; set; }
    public string CODE { get; set; } = "?";
    public string NAME { get; set; }
    public decimal RATE { get; set; }
    public int ISREVERSE { get; set; }
    public DateTime TS { get; set; }
    public virtual ObservableListViewSource<M_PVNRATES2> PVNRates2Rows { get; set; } = new ();
}
