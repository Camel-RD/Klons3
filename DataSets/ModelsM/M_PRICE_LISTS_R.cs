using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_PRICE_LISTS_R : EntityBase
{
    public int ID { get; set; }
    public int IDL { get; set; }
    public int IDITEM { get; set; } = 1;
    public decimal PRICE { get; set; }
    public DateTime TS { get; set; }
    public virtual M_ITEMS Item { get; set; }
    public virtual M_PRICE_LISTS PriceList { get; set; }

    public override string GetInfoView()
    {
        var ret =
            $""""
            [Cenu lapas rinda] 
              Lapas kods: {PriceList?.CODE}
              Lapas nosaukums: {PriceList?.NAME}
              Artikuls: {Item?.BARCODE}
              Artikula nosaukums: {Item?.NAME}
              Cena: {PRICE}
            """";
        return ret;
    }

}