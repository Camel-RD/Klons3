using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Klons3.ModelsA;

public partial class A_FP_PAYLISTS : EntityBase
{
    public int ID { get; set; }
    public int SNR { get; set; } = 1;
    public int YR { get; set; }
    public int MT { get; set; }
    public DateTime PAYDATE { get; set; }
    public string DESCR { get; set; }
    public DateTime TS { get; set; }
    public virtual ObservableListViewSource<A_FP_PAYLISTS_R> FpPaylistRows { get; set; } = new();

    public override string GetInfoView()
    {
        var ret = $""""
                  [Maksājumu saraksts fiziskām personām] 
                    Gads: {YR}
                    Mēnesis: {MT}
                    Nr.: {SNR}
                    Maksājuma datums: {Utils.DateToString(PAYDATE)}
                    Apraksts: {DESCR}
                  """";
        return ret;
    }
}