using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_BALA1 : EntityBase
{
    public string BALID { get; set; }
    public string DESCR { get; set; }
    public string TA { get; set; }
    public string TP { get; set; }
    public DateTime TS { get; set; }
    public virtual ObservableListViewSource<F_BALA2> BALA2Rows { get; set; } = new ObservableListViewSource<F_BALA2>();
}
