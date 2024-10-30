using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsFRep;

public partial class F_TREPDOCSS : EntityBase
{
    public int ID { get; set; }
    public int? SPID { get; set; }
    public int? DID { get; set; }
    public decimal? S1 { get; set; }
    public decimal? S2 { get; set; }
    public decimal? S3 { get; set; }
}