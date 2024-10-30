using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_REPORT_CODES : EntityBase
{
    public int ID { get; set; }
    public string CODE { get; set; } = "?";
    public string DESCR { get; set; }
    public int? USED { get; set; } = 1;
    public short? TP1 { get; set; }
    public short? TP2 { get; set; }
    public DateTime TS { get; set; }
}