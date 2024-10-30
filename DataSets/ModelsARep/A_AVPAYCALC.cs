using KlonsA.Classes;
using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_AVPAYCALC : EntityBase
{
    public int ID { get; set; }
    public short? TYPE_TAG { get; set; }
    public int? IDP { get; set; }
    public int? IDAM { get; set; }
    public int YR { get; set; }
    public int MT { get; set; }
    public int CALENDAR_DAYS { get; set; }
    public int MONTH_WORKDAYS { get; set; }
    public float MONTH_WORKHOURS { get; set; }
    public int PLAN_DAYS { get; set; }
    public float PLAN_HOURS { get; set; }
    public int PLAN_WORKDAYS { get; set; }
    public float PLAN_WORKHOURS { get; set; }
    public decimal TOTAL_BEFORE_TAXES { get; set; }
    public decimal FORAVPAYCALC_BRUTO { get; set; }
    public decimal FORAVPAYCALC_PAYOUT { get; set; }
    public int FORAVPAYCALC_DAYS { get; set; }
    public float FORAVPAYCALC_HOURS { get; set; }
    public decimal AVPAYCALC_DAY { get; set; }
    public decimal AVPAYCALC_HOUR { get; set; }

}

