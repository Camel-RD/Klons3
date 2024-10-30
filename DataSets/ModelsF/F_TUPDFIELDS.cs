using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_TUPDFIELDS : EntityBase
{
    public string ID { get; set; }
    public string TEXT { get; set; }
    public string N { get; set; }
    public DateTime TS { get; set; }
}