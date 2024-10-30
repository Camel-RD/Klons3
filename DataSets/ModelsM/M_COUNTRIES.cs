using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_COUNTRIES : EntityBase
{
    public int ID { get; set; }
    public string NAME { get; set; }
    public string CODE3 { get; set; }
    public string CODE2 { get; set; }
    public DateTime TS { get; set; }
}
