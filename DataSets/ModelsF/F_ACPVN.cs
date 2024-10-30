using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_ACPVN : EntityBase
{
    public string ID { get; set; }
    public string NM { get; set; }
    public float? T { get; set; }
    public string PZ1 { get; set; }
    public int PZ2 { get; set; }
    public int PZ2A { get; set; }
    public int PZ3 { get; set; }
    public int PZ4 { get; set; }
    public int PZ5 { get; set; }
    public DateTime TS { get; set; }
}