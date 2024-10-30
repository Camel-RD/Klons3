using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_SP_PAY_MATCHLISTS : EntityBase
{
    public int? IDP { get; set; }
    public int IDAM { get; set; }
    public DateTime? DT1 { get; set; }
    public DateTime? DT2 { get; set; }
    public int IDSHR1 { get; set; }
    public int IDSHR2 { get; set; }
    public decimal? PAY0 { get; set; }
    public decimal? PAY1 { get; set; }
    public decimal? PAY2 { get; set; }
    public float R1 { get; set; }
    public float R2 { get; set; }
    public decimal? IIN { get; set; }
}

