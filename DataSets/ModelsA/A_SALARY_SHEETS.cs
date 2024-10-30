using KlonsA.Classes;
using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsA;

public partial class A_SALARY_SHEETS : EntityBase
{
    public int ID { get; set; }
    public int YR { get; set; } = 2020;
    public int MT { get; set; } = 1;
    public short SNR { get; set; }
    public DateTime? DT1 { get; set; }
    public DateTime? DT2 { get; set; }
    public string DESCR { get; set; }
    public string DEP { get; set; }
    public short KIND { get; set; }
    public short IS_TEMP { get; set; }
    public DateTime TS { get; set; }
    public virtual ObservableListViewSource<A_SALARY_SHEETS_R> SalarySheetRows { get; set; } = new ObservableListViewSource<A_SALARY_SHEETS_R>();
    public virtual ObservableListViewSource<A_SALARY_SHEETS_R> SalarySheetTotalRows { get; set; } = new ObservableListViewSource<A_SALARY_SHEETS_R>();
    public virtual A_DEPARTMENTS Department { get; set; }

    public ESalarySheetKind XKind
    {
        get { return (ESalarySheetKind)this.KIND; }
        set { this.KIND = (short)value; }
    }

    public bool ISFIRSTMT { get; set; }

    public override string GetInfoView()
    {
        var ret = $""""
                  [Algu lapa] 
                    Gads: {YR}
                    Mēnesis: {MT}
                    Datums: {Utils.DateNToString(DT2)}
                    Nr.: {SNR}
                    Apraksts: {DESCR}
                    Struktūrvienība: {DEP}
                  """";
        return ret;
    }


}