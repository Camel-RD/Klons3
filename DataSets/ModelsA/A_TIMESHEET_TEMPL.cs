using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_TIMESHEET_TEMPL : EntityBase
{
    public int ID { get; set; }
    public string CODE { get; set; }
    public string DEP { get; set; }
    public string DESCR { get; set; } = "?";
    public short? USED { get; set; } = 1;
    public short SNR { get; set; }
    public DateTime TS { get; set; }
    public virtual ObservableListViewSource<A_TIMESHEET_TEMPL_R> TimeSheetTemplateRows { get; set; } = new ObservableListViewSource<A_TIMESHEET_TEMPL_R>();
    public virtual A_DEPARTMENTS DEPNavigation { get; set; }

    public override string GetInfoView()
    {
        var ret = $""""
                  [Darba laika uzskaites lapas sagatave] 
                    Nr.: {SNR}
                    Kods: {CODE}
                    Apraksts: {DESCR}
                    Struktūrvienība: {DEP}
                  """";
        return ret;
    }

}