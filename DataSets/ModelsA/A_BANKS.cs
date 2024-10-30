using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_BANKS : EntityBase
{
    public int ID { get; set; }
    public string SID { get; set; } = "?";
    public string NAME { get; set; } = "?";
    public DateTime TS { get; set; }
}
