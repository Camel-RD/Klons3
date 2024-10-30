using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_SALARY_SHEET_TEMPL_R : EntityBase
{
    public int ID { get; set; }
    public int IDS { get; set; }
    public int IDP { get; set; }
    public int IDAM { get; set; }
    public int SNR { get; set; }
    public DateTime TS { get; set; }
    public virtual A_POSITIONS Position { get; set; }
    public virtual A_PERSONS Person { get; set; }
    public virtual A_SALARY_SHEET_TEMPL SalarySheetTemplate { get; set; }

    public override string GetInfoView()
    {
        var ret = $""""
                  [Algas aprēķina sagataves rinda] 
                    Saraksta Nr.: {SalarySheetTemplate?.SNR}
                    Saraksta kods: {SalarySheetTemplate?.CODE}
                    Saraksta nosaukums: {SalarySheetTemplate?.DESCR}
                    Nr.: {SNR}
                    Persons: {Person?.YNAME}
                    Amats: {Position?.TITLE}
                  """";
        return ret;
    }

}