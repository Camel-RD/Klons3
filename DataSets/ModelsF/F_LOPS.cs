﻿using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_LOPS : EntityBase
{
    public int IDL { get; set; }
    public int? ID { get; set; }
    public DateTime? DTL { get; set; }
    public DateTime? DTLD { get; set; }
    public string USL { get; set; }
    public int? TPL { get; set; }
    public int? DOCID { get; set; }
    public string AC11 { get; set; }
    public string AC12 { get; set; }
    public string AC13 { get; set; }
    public string AC14 { get; set; }
    public string AC15 { get; set; }
    public string AC21 { get; set; }
    public string AC22 { get; set; }
    public string AC23 { get; set; }
    public string AC24 { get; set; }
    public string AC25 { get; set; }
    public decimal SUMMC { get; set; }
    public string CUR { get; set; }
    public decimal SUMM { get; set; }
    public float? QV { get; set; }
    public short? NL { get; set; }
    public DateTime? ODT { get; set; }
}