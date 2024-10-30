using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_TUDOCLISTS : EntityBase
{
    public int ID { get; set; }
    public string T1 { get; set; }
    public string DT1 { get; set; }
    public string DT2 { get; set; }
}