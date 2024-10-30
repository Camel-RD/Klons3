using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsFRep;

public partial class F_ROPS3A : EntityBase
{
    public int ID { get; set; }
    public int? SPID { get; set; }
    public string RAC { get; set; }
    public string NAME { get; set; }
    public decimal? SDB { get; set; }
    public decimal? SCR { get; set; }
    public string AC1 { get; set; }
    public string CLID { get; set; }
}