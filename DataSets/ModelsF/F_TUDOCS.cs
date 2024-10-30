using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_TUDOCS : EntityBase
{
    public int ID { get; set; }
    public int? LD { get; set; }
    public string T1 { get; set; }
    public string T2 { get; set; }
    public string T3 { get; set; }
    public string DT1 { get; set; }
    public string SR { get; set; }
    public short? CT { get; set; }
    public string N1 { get; set; }
    public string N2 { get; set; }
    public float? S1 { get; set; }
    public float? S2 { get; set; }
}