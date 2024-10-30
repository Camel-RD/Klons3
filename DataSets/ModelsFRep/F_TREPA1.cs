using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsFRep;

public partial class F_TREPA1 : EntityBase
{
    public int ID { get; set; }
    public int? SPID { get; set; }
    public string I1 { get; set; }
    public string NM { get; set; }
    public DateTime? DT { get; set; }
    public string ST { get; set; }
    public string NR { get; set; }
    public decimal? S1 { get; set; }
    public decimal? S2 { get; set; }
    public decimal? S3 { get; set; }
    public decimal? S4 { get; set; }
}