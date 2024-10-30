using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_BALA21 : EntityBase
{
    public int ID { get; set; }
    public int? SPID { get; set; }
    public int? BID { get; set; }
    public decimal? S1 { get; set; }
    public decimal? S2 { get; set; }
    public DateTime TS { get; set; }
}