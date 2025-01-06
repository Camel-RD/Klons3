using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class SP_M_CURRENTSTOCK : EntityBase
{
    public int IDITEM { get; set; }
    public int IDSTORE { get; set; }
    public decimal AMOUNT { get; set; }
}