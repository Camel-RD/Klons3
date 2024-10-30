using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsFRep;

public partial class F_ROPS2B : EntityBase
{
    public int ID { get; set; }
    public int? SPID { get; set; }
    public string AC { get; set; }
    public string NAME { get; set; }
    public decimal? B0 { get; set; }
    public decimal? SDB { get; set; }
    public decimal? SCR { get; set; }
    public decimal? TDB { get; set; }
    public decimal? TCR { get; set; }
}