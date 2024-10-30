using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_ACPLAN : EntityBase
{
    public string AC { get; set; }
    public string NAME { get; set; }
    public string AP1 { get; set; }
    public string AP2 { get; set; }
    public string AP3 { get; set; }
    public string AP4 { get; set; }
    public string AP5 { get; set; }
    public DateTime TS { get; set; }
}