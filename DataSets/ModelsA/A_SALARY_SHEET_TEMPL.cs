using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Klons3.ModelsA;

public partial class A_SALARY_SHEET_TEMPL : EntityBase
{
    public int ID { get; set; }
    public string CODE { get; set; }
    public string DEP { get; set; }
    public string DESCR { get; set; } = "?";
    public short? USED { get; set; } = 1;
    public short SNR { get; set; }
    public DateTime TS { get; set; }
    public virtual ObservableListViewSource<A_SALARY_SHEET_TEMPL_R> SalarySheetTemplateRows { get; set; } = new ObservableListViewSource<A_SALARY_SHEET_TEMPL_R>();
    public virtual A_DEPARTMENTS Department { get; set; }

    public override string GetInfoView()
    {
        var ret = $""""
                  [Algas aprēķina sagatave] 
                    Nr.: {SNR}
                    Kods: {CODE}
                    Nosaukums: {DESCR}
                    Struktūrvienība: {DEP}
                  """";
        return ret;
    }
}