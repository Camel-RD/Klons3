﻿using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_BANKS : EntityBase
{
    public int ID { get; set; }
    public string CODE { get; set; }
    public string NAME { get; set; }
    public DateTime TS { get; set; }
}