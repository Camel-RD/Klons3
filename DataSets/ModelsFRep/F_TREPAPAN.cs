using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsFRep;

public partial class F_TREPAPAN : EntityBase
{
    public int ID { get; set; }
    public int? SPID { get; set; }
    public string AC1 { get; set; }
    public string AC2 { get; set; }
    public string AC3 { get; set; }
    public string AC4 { get; set; }
    public string AC5 { get; set; }
    public string CL { get; set; }
    public decimal? S1 { get; set; }
    public decimal? S2 { get; set; }
    public decimal? S3 { get; set; }
    public decimal? S4 { get; set; }
}