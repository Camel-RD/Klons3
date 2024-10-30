using KlonsA.Classes;
using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_SP_GET_MINMAX : EntityBase
{
    public int RDLMINYR { get; set; }
    public int RDLMINMT { get; set; }
    public int RDLMAXYR { get; set; }
    public int RDLMAXMT { get; set; }
    public int RAMINYR { get; set; }
    public int RAMINMT { get; set; }
    public int RAMAXYR { get; set; }
    public int RAMAXMT { get; set; }
}

