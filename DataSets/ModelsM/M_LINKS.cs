using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_LINKS : EntityBase
{
    public int ID1 { get; set; }
    public int ID2 { get; set; }
    public int IDITEM { get; set; }
    public decimal AMOUNT { get; set; }
    public DateTime TS { get; set; }
    public virtual M_ROWS ID1Navigation { get; set; }
    public virtual M_ROWS ID2Navigation { get; set; }
}