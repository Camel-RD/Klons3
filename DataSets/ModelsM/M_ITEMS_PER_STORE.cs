using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_ITEMS_PER_STORE : EntityBase
{
    public int IDITEM { get; set; }
    public int IDSTORE { get; set; }
    public decimal AMOUNT { get; set; }
    public DateTime TS { get; set; }
    public virtual M_ITEMS Item { get; set; }
    public virtual M_STORES Store { get; set; }
}