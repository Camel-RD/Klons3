using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_EVENT_TYPES : EntityBase
{
    public int ID { get; set; }
    public string DESCR { get; set; }
    public int? SN { get; set; }
    public short? IS_DATE_INT { get; set; }
    public int? TP1 { get; set; }
    public int TP2 { get; set; }
    public string SCODE { get; set; }
    public DateTime TS { get; set; }
}