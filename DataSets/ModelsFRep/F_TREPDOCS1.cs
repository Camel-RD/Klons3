﻿using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsFRep;

public partial class F_TREPDOCS1 : EntityBase
{
    public int ID { get; set; }
    public int? SPID { get; set; }
    public string SR { get; set; }
    public string NR { get; set; }
    public decimal? S1 { get; set; }
    public decimal? S2 { get; set; }
}