using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_PVNTYPE : EntityBase
{
    public int ID { get; set; }
    public string CODE { get; set; }
    public string NAME { get; set; }
    public DateTime TS { get; set; }
    public virtual ObservableListSource<M_PVNRATES2> PVNRates2Rows { get; set; } = new();
}
