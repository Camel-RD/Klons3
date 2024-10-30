using System;
using System.Collections.Generic;
using System.Xml.Linq;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_DOCTYPB : EntityBase
{
    public string IDA { get; set; }
    public string ID { get; set; }
    public DateTime TS { get; set; }
    public virtual F_DOCTYPA DOCTYPARow { get; set; }
    public virtual F_DOCTYP DOCTYPRows { get; set; }

    public override string GetInfoView()
    {
        var ret = $"""
                  [Dokumenta veidu sasaiste] 
                    Dokumets 2: {IDA}
                    Dokumets: {ID}
                  """;
        return ret;
    }

}
