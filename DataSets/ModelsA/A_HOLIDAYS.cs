using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_HOLIDAYS : EntityBase
{
    public DateTime DT { get; set; }
    public short? TAG { get; set; }
    public DateTime TS { get; set; }
}