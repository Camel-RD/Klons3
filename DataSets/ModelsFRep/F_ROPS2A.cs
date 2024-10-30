using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsFRep;

public partial class F_ROPS2A : EntityBase
{
    public int ID { get; set; }
    public int? SPID { get; set; }
    public string AC { get; set; }
    public string RAC { get; set; }
    public string NAME { get; set; }
    public decimal? B0 { get; set; }
    public decimal? SDB { get; set; }
    public decimal? SCR { get; set; }
    public decimal? TDB { get; set; }
    public decimal? TCR { get; set; }
    public string CLID { get; set; }
    public string NAME1 { get; set; }
    public string REGNR { get; set; }
    public string RNAME { get; set; }
    public decimal ADB { get; set; }
    public decimal ACR { get; set; }
    public decimal BDB { get; set; }
    public decimal BCR { get; set; }
}