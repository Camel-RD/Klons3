using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_PAYSALDO : EntityBase
{
    public int? IDP { get; set; }
    public int IDAM { get; set; }
    public decimal? PAY0 { get; set; }
    public decimal? IIN0 { get; set; }
    public decimal? ADVANCE { get; set; }
    public decimal? WITHHOLDINGS { get; set; }


}
