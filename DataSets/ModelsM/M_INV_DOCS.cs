using System;
using System.Collections.Generic;
using System.Xml.Linq;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsM;

public partial class M_INV_DOCS : EntityBase
{
    public int ID { get; set; }
    public DateTime DT { get; set; }
    public string NR { get; set; }
    public int IDSTORE { get; set; } = 1;
    public int STATE { get; set; }
    public string PERSONS { get; set; }
    public DateTime TS { get; set; }
    public virtual M_STORES Store { get; set; }
    public virtual ObservableListViewSource<M_INV_ROWS> Rows { get; set; } = new ();

    public KlonsM.Classes.EInventoryDocState XState
    {
        get => (KlonsM.Classes.EInventoryDocState)STATE;
        set => STATE = (int)value;
    }

    public override string GetInfoView()
    {
        var ret =
            $""""
            [Inventarizācijas akts] 
              Datums: {Utils.DateToString(DT)}
              Nr.: {NR}
              Noliktava: {Store?.NAME}
            """";
        return ret;
    }


}