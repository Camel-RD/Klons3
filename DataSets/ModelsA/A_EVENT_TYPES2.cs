using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_EVENT_TYPES2 : EntityBase
{
    public int ID { get; set; }
    public string TAG { get; set; } = "?";
    public string DESCR { get; set; } = "?";
    public DateTime TS { get; set; }
}