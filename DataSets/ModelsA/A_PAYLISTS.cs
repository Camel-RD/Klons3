using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsA;

public partial class A_PAYLISTS : EntityBase
{
    public int ID { get; set; }
    public int SNR { get; set; } = 1;
    public int YR { get; set; } = 2020;
    public int MT { get; set; } = 1;
    public DateTime DT { get; set; }
    public string DEP { get; set; }
    public string DESCR { get; set; } = "?";
    public short DOPAY { get; set; }
    public decimal TOTAL_PAY { get; set; }
    public DateTime TS { get; set; }
    public virtual ObservableListViewSource<A_PAYLISTS_R> PayListRows { get; set; } = new ObservableListViewSource<A_PAYLISTS_R>();
    public virtual A_DEPARTMENTS Department { get; set; }

    public override string GetInfoView()
    {
        var ret = $""""
                  [Maksājumu saraksts] 
                    Nr.: {SNR}
                    Gads: {YR}
                    Mēnesis: {MT}
                    Datums: {Utils.DateToString(DT)}
                    Struktūrvienība: {DEP}
                    Apraksts: {DESCR}
                  """";
        return ret;
    }

}