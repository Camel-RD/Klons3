using KlonsP.Classes;
using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsP;

public partial class P_EVENTS : EntityBase
{
    public int ID { get; set; }
    public string CODE { get; set; }
    public string DESCR { get; set; }
    public int SNR { get; set; }
    public int K1 { get; set; }
    public DateTime TS { get; set; }

    public EEvent XEvent
    {
        get { return (EEvent)this.ID; }
        set { this.ID = (int)value; }
    }

}
