using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_BALA3 : EntityBase
{
    public int ID { get; set; }
    public int? ID2 { get; set; }
    public string TP { get; set; }
    public string AC { get; set; }
    public DateTime TS { get; set; }
    public virtual F_BALA2 BALA2Row { get; set; }
}
