using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_DOCTYPA : EntityBase
{
    public string ID { get; set; }
    public string NAME { get; set; }
    public DateTime TS { get; set; }
    public virtual ObservableListViewSource<F_DOCTYPB> DOCTYPBRows { get; set; } = new ObservableListViewSource<F_DOCTYPB>();

    public override string GetInfoView()
    {
        var ret = $"""
                  [Dokumenta veids 2] 
                    Konds: {ID}
                    Nosaukums: {NAME}
                  """;
        return ret;
    }

}
