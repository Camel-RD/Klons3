using System;
using System.Collections.Generic;
using System.Xml.Linq;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_BAL0 : EntityBase
{
    public int ID { get; set; }
    public string AC11 { get; set; }
    public string AC24 { get; set; }
    public string CLID { get; set; }
    public decimal SUMMDC { get; set; }
    public decimal SUMMCC { get; set; }
    public string CUR { get; set; }
    public decimal CURRATE { get; set; }
    public decimal SUMMD { get; set; }
    public decimal SUMMC { get; set; }
    public DateTime TS { get; set; }
    public virtual F_ACP21 AC11Row { get; set; }
    public virtual F_ACP24 AC24Row { get; set; }
    public virtual F_PERSONS PersonRow { get; set; }

    public override string GetInfoView()
    {
        var ret = $"""
                  [Bilances sākuma atlikumi] 
                    Konts: {AC11}, nozare \ produkts: {AC24}
                    Person: {CLID}
                    Summa debetā: {SUMMDC:N2}, kredītā: {SUMMDC:N2}
                  """;
        return ret;
    }

}
