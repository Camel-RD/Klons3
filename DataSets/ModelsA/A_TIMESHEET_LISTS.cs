using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_TIMESHEET_LISTS : EntityBase
{
    public int ID { get; set; }
    public int YR { get; set; }
    public int MT { get; set; }
    public short SNR { get; set; }
    public string DESCR { get; set; } = "?";
    public int? IDSH { get; set; }
    public string DEP { get; set; }
    public DateTime TS { get; set; }
    public virtual ObservableListSource<A_TIMESHEET_LISTS_R> TimesheetListRows { get; set; } = new ObservableListSource<A_TIMESHEET_LISTS_R>();
    public virtual A_DEPARTMENTS Department { get; set; }
    public bool ISFIRSTMT { get; set; }

    public override string GetInfoView()
    {
        var ret = $""""
                  [Darba laika uzskaites lapa] 
                    Gads: {YR}
                    Mēnesis: {MT}
                    Nr.: {SNR}
                    Apraksts: {DESCR}
                    Struktūrvienība: {DEP}
                  """";
        return ret;
    }

}