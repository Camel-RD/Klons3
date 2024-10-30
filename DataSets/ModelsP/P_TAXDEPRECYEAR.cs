using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsP;

public partial class P_TAXDEPRECYEAR : EntityBase
{
    public int ID { get; set; }
    public int YR { get; set; }
    public int CATT { get; set; }
    public float RATE { get; set; }
    public int KIND { get; set; }
    public decimal VALUE0 { get; set; }
    public decimal VALUE1 { get; set; }
    public decimal VALUEC { get; set; }
    public decimal DEPREC { get; set; }
    public int COUNT { get; set; }
    public decimal VALUE_NEW { get; set; }
    public decimal VALUE_ADD { get; set; }
    public decimal VALUE_REM { get; set; }
    public decimal VALUE_EXCL { get; set; }
    public decimal VALUE_COR { get; set; }
    public decimal VALUED { get; set; }
    public DateTime TS { get; set; }
    public virtual P_CATT CATTRow { get; set; }

    public string XCATT_CODE => CATTRow?.CODE;
    public string XCATT_NAME => CATTRow?.DESCR;
}