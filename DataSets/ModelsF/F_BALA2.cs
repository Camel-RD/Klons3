using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_BALA2 : EntityBase
{
    public int ID { get; set; }
    public string NR { get; set; }
    public string BALID { get; set; }
    public string DC { get; set; }
    public string TP { get; set; }
    public string DESCR { get; set; }
    public string NR1 { get; set; }
    public string NR2 { get; set; }
    public decimal? S1 { get; set; }
    public decimal? S2 { get; set; }
    public DateTime TS { get; set; }
    public virtual F_BALA1 BALA1Row { get; set; }
    public virtual ObservableListViewSource<F_BALA3> BALA3Rows { get; set; } = new ObservableListViewSource<F_BALA3>();
}
