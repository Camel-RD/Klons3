using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsM;

public partial class M_ITEMS : EntityBase
{
    public int ID { get; set; }
    public string BARCODE { get; set; } = "?";
    public string NAME { get; set; } = "?";
    public int CAT { get; set; } = 1;
    public int UNITS { get; set; } = 1;
    public int STORE1 { get; set; } = 1;
    public int PVNRATE { get; set; } = 1;
    public int ISSERVICE { get; set; }
    public int ISPRODUCED { get; set; }
    public decimal SELLPRICE { get; set; }
    public decimal BUYPRICE { get; set; }
    public decimal PRODCOST { get; set; }
    public decimal LASTBUYPRICE { get; set; }
    public decimal AMOUNTINSTORE { get; set; }
    public DateTime? LASTSALEDATE { get; set; }
    public DateTime? LASTBUYDATE { get; set; }
    public DateTime TS { get; set; }
    public virtual M_ITEMS_CAT Category { get; set; }
    public virtual ObservableListSource<M_DISC_LISTS_R> M_DISC_LISTS_R { get; set; } = new();
    public virtual ObservableListSource<M_ITEMS_PER_STORE> ItemsPerStore { get; set; } = new();
    public virtual ObservableListSource<M_PRICE_LISTS_R> M_PRICE_LISTS_R { get; set; } = new();
    public virtual ObservableListViewSource<M_ITEMS_TEXTS> ItemTexts { get; set; } = new();


    public virtual M_PVNRATES PVNRateRow { get; set; }
    public virtual M_STORES StoreMain { get; set; }
    public virtual M_UNITS UnitsRow { get; set; }

    public bool XIsServices => Category.XIsServices;
    public bool XIsProduced => Category.XIsProduced;

    public override string GetInfoView()
    {
        var ret =
            $""""
            [Artikuls] 
              Kods: {BARCODE}
              Nosaukums: {NAME}
              Kategorija: {Category?.NAME}
              Mērvienība: {UnitsRow?.NAME}
              Galvenā noliktava: {StoreMain?.NAME}
              Cena: {SELLPRICE}
              ...
            """";
        return ret;
    }


}
