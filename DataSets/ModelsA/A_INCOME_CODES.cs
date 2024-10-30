using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_INCOME_CODES : EntityBase
{
    public string ID { get; set; }
    public string DESCR { get; set; } = "?";
    public int P1 { get; set; }
    public int P2 { get; set; }
    public int USED { get; set; } = 1;
    public DateTime TS { get; set; }
}