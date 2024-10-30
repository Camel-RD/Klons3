using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_PVNTEXTS : EntityBase
{
    public int ID { get; set; }
    public string CODE { get; set; } = "?";
    public string VERSION { get; set; }
    public string TAG1 { get; set; }
    public string TAG2 { get; set; }
    public DateTime TS { get; set; }
}