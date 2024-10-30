using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_LOGX : EntityBase
{
    public int ID { get; set; }
    public string LID { get; set; }
    public DateTime? DT { get; set; }
    public string USR { get; set; }
    public DateTime TS { get; set; }
}