using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_ACCOUNTS : EntityBase
{
    public string ID { get; set; }
    public string NAME { get; set; } = "?";
    public int? TP { get; set; } = 1;
    public DateTime TS { get; set; }
    public virtual M_ACCOUNTTYPE AccountType { get; set; }

    public KlonsM.Classes.EAccountType XAccountType
    {
        get => (KlonsM.Classes.EAccountType)TP;
        set => TP = (int)value;
    }

    public override string GetInfoView()
    {
        var ret =
            $""""
            [Kontu plāna konts] 
              Kods: {ID}
              Nosaukums: {NAME}
            """";
        return ret;
    }


}