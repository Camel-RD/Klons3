﻿using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_BANKS : EntityBase
{
    public string ID { get; set; }
    public string NAME { get; set; }
    public DateTime TS { get; set; }
}