using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_ITEMS_TEXTS : EntityBase
{
    public int ID { get; set; }
    public int IDITEM { get; set; }
    public string TEXT { get; set; }
    public virtual M_ITEMS Item { get; set; }
    public DateTime TS { get; set; }
}

