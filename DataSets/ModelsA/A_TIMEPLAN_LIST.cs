using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsA;

public partial class A_TIMEPLAN_LIST : EntityBase
{
    public int ID { get; set; }
    public string CODE { get; set; }
    public string DESCR { get; set; }
    public short? KIND1 { get; set; }
    public short? KIND2 { get; set; }
    public float HOURS { get; set; }
    public short? USED { get; set; } = 1;
    public short? NIGHT { get; set; }
    public short? SNR { get; set; }
    public float HOURS_NIGHT { get; set; }
    public DateTime TS { get; set; }
    public virtual ObservableListViewSource<A_TIMESHEET> TimesheetRows { get; set; } = new ObservableListViewSource<A_TIMESHEET>();

    public override string GetInfoView()
    {
        var ret = $""""
                  [Darba laika plāna veids] 
                    Kods: {CODE}
                    Apraksts: {DESCR}
                    Stundas dienā: {HOURS}
                    ...
                  """";
        return ret;
    }
}
