using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_PERSONTYP : EntityBase
{
    public int ID { get; set; }
    public string IDX { get; set; }
    public string NAME { get; set; }
    public DateTime TS { get; set; }
}