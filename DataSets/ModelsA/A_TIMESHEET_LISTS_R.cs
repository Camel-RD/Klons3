using KlonsA.Classes;
using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using System.Runtime.Intrinsics.Arm;

namespace Klons3.ModelsA;

public partial class A_TIMESHEET_LISTS_R : EntityBase
{
    public int ID { get; set; }
    public int? IDS { get; set; }
    public int? IDP { get; set; }
    public int? IDAM { get; set; }
    public int? IDPL { get; set; }
    public short? SNR { get; set; }
    public short PLAN_TYPE { get; set; }
    public short NIGHT { get; set; }
    public short OVERTIME { get; set; }
    public DateTime TS { get; set; }
    public virtual ObservableListSource<A_TIMESHEET> TimeSheetRows { get; set; } = new ObservableListSource<A_TIMESHEET>();

    public virtual A_POSITIONS Position { get; set; }
    public virtual A_PERSONS Person { get; set; }
    public virtual A_TIMESHEET_LISTS Timesheet { get; set; }
    public virtual A_TIMEPLAN_LIST TimePlan { get; set; }

    public EPlanType XPlanType
    {
        get { return (EPlanType)this.PLAN_TYPE; }
        set { this.PLAN_TYPE = (short)value; }
    }

    public override string GetInfoView()
    {
        var ret = $""""
                  [Darba laika uzskaites lapas rinda] 
                    Lapas Nr.: {Timesheet?.SNR}
                    Lapas gads: {Timesheet?.YR}
                    Lapas mēnesis: {Timesheet?.MT}
                    Lapas apraksts: {Timesheet?.DESCR}
                    Rindas Nr.: {SNR}
                    Persona: {Person?.YNAME}
                    Amats: {Position?.TITLE}
                  """";
        return ret;
    }

}