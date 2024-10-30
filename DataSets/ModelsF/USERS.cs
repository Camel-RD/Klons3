using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class USERS : EntityBase
{
    public string NM { get; set; }
    public string PSW { get; set; }
    public string TP { get; set; } = "A";
    public int NR { get; set; }
}
