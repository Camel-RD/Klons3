using System;
using System.Collections.Generic;
using System.Xml.Linq;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_DISC_LISTS_P : EntityBase
{
    public int ID { get; set; }
    public int IDL { get; set; }
    public int? IDSTORESCAT { get; set; }
    public int? IDSTORE { get; set; }
    public DateTime TS { get; set; }
    public virtual M_DISC_LISTS DiscountList { get; set; }
    public virtual M_STORES Store { get; set; }
    public virtual M_STORES_CAT StoresCategory { get; set; }

    public override string GetInfoView()
    {
        var ret =
            $""""
            [Atlaižu lapas noliktavas/personas rinda] 
              Lapas kods: {DiscountList?.CODE}
              Lapas nosaukums: {DiscountList?.NAME}
              Noliktavas/personas kategorija: {StoresCategory?.NAME}
              Noliktava/persona: {Store?.NAME}
            """";
        return ret;
    }

}