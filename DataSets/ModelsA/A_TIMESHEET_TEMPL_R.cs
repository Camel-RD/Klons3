using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_TIMESHEET_TEMPL_R : EntityBase
{
    public int ID { get; set; }
    public int? IDS { get; set; }
    public int IDP { get; set; }
    public int IDAM { get; set; }
    public int IDPL { get; set; }
    public short SNR { get; set; }
    public short PLAN_TYPE { get; set; }
    public DateTime TS { get; set; }
    public virtual A_POSITIONS Position { get; set; }
    public virtual A_TIMEPLAN_LIST TimePlan { get; set; }
    public virtual A_PERSONS Person { get; set; }
    public virtual A_TIMESHEET_TEMPL TimeSheetTemplate { get; set; }

    public override string GetInfoView()
    {
        var ret = $""""
                  [Darba laika uzskaites lapas sagataves rinda] 
                    Lapas Nr.: {TimeSheetTemplate?.SNR}
                    Lapas kods: {TimeSheetTemplate?.CODE}
                    Lapas apraksts: {TimeSheetTemplate?.DESCR}
                    Lapas struktūrvienība: {TimeSheetTemplate?.DEP}
                    Rindas Nr.: {SNR}
                    Persona: {Person?.YNAME}
                    Amats: {Position?.TITLE}
                    Plāns: {TimePlan?.CODE}
                  """";
        return ret;
    }
}