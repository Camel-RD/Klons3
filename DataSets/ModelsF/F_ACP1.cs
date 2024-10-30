using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_ACP1 : EntityBase
{
    public string ACPID { get; set; }
    public string NAME { get; set; }
    public DateTime TS { get; set; }
}