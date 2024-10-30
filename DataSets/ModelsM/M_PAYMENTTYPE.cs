using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_PAYMENTTYPE : EntityBase
{
    public int ID { get; set; }
    public string NAME { get; set; }
    public DateTime TS { get; set; }
}
