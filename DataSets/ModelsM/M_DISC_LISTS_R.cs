using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_DISC_LISTS_R : EntityBase
{
    public int ID { get; set; }
    public int IDL { get; set; }
    public int? IDITEMSCAT { get; set; }
    public int? IDITEM { get; set; }
    public float DISCOUNT { get; set; }
    public DateTime TS { get; set; }
    public virtual M_ITEMS Item { get; set; }
    public virtual M_ITEMS_CAT ItemsCategory { get; set; }
    public virtual M_DISC_LISTS DiscountList { get; set; }

    public override string GetInfoView()
    {
        var ret =
            $""""
            [Atlaižu lapas artikula rinda] 
              Lapas kods: {DiscountList?.CODE}
              Lapas nosaukums: {DiscountList?.NAME}
              Artikulu kategorija: {ItemsCategory?.NAME}
              Artikula kods: {Item?.BARCODE}
              Artikula nosaukums: {Item?.NAME}
              Atlaide: {DISCOUNT}
            """";
        return ret;
    }

}