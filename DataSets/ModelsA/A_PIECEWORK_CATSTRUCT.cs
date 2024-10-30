using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Klons3.ModelsA;

public partial class A_PIECEWORK_CATSTRUCT : EntityBase
{
    public int ID { get; set; }
    public string CODE { get; set; } = "?";
    public string DESCR { get; set; } = "?";
    public short USED { get; set; } = 1;
    public DateTime TS { get; set; }
    public virtual ObservableListSource<A_PIECEWORK_CATALOG> CalatlogRows { get; set; } = new ObservableListSource<A_PIECEWORK_CATALOG>();

    public override string GetInfoView()
    {
        var ret = $""""
                  [Gabaldarba uzskaites kataloga sekcija] 
                    Kods: {CODE}
                    Nosaukums: {DESCR}
                  """";
        return ret;
    }
}