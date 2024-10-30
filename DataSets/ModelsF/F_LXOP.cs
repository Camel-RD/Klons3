using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_LXOP : EntityBase
{
    public int ID { get; set; }
    public int? AD { get; set; }
    public int? AR { get; set; }
    public DateTime TS { get; set; }
}