using KlonsA.Classes;
using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_POSITIONS : EntityBase
{
    public int ID { get; set; }
    public int IDP { get; set; }
    public string IDDEP { get; set; }
    public string TITLE { get; set; } = "?";
    public short APPLY_IIN_EXEMPTIONS { get; set; } = 1;
    public decimal PAY0 { get; set; }
    public decimal IIN0 { get; set; }
    public decimal ADVANCE { get; set; }
    public short USED { get; set; } = 1;
    public DateTime? USED_DT1 { get; set; }
    public short? USED_DT2 { get; set; }
    public DateTime TS { get; set; }
    public virtual ObservableListSource<A_EVENTS> Events { get; set; } = new ObservableListSource<A_EVENTS>();
    public virtual ObservableListViewSource<A_POSITIONS_PLUSMINUS> PositionsPlusMinusRows { get; set; } = new ObservableListViewSource<A_POSITIONS_PLUSMINUS>();
    public virtual ObservableListViewSource<A_POSITIONS_R> PositionStateRows { get; set; } = new ObservableListViewSource<A_POSITIONS_R>();
    public virtual A_PERSONS Person { get; set; }
    public virtual A_DEPARTMENTS Department { get; set; }

    public string YNAME => $"{Person?.YNAME}, {TITLE}";

    public override string GetInfoView()
    {
        var ret = $""""
                  [Amats] 
                    Persona: {Person?.YNAME}
                    Amats: {TITLE}
                    Struktūrvienība: {IDDEP}
                  """";
        return ret;
    }

}