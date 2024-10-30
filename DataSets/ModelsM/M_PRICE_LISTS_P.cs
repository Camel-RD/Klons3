using System;
using System.Collections.Generic;
using System.Xml.Linq;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_PRICE_LISTS_P : EntityBase
{
    public int ID { get; set; }
    public int IDL { get; set; }
    public int? IDSTORESCAT { get; set; }
    public int? IDSTORE { get; set; }
    public DateTime TS { get; set; }
    public virtual M_PRICE_LISTS PriceList { get; set; }
    public virtual M_STORES Store { get; set; }
    public virtual M_STORES_CAT StoresCategory { get; set; }

    public override string GetInfoView()
    {
        var ret =
            $""""
            [Cenu lapas rinda] 
              Lapas kods: {PriceList?.CODE}
              Lapas nosaukums: {PriceList?.NAME}
              Partnera/noliktavas kategorija: {StoresCategory?.NAME}
              Partneris/noliktava: {Store?.NAME}
            """";
        return ret;
    }

}