using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsM;

public partial class M_ROWS : EntityBase
{
    public int ID { get; set; }
    public int IDDOC { get; set; }
    public int IDITEM { get; set; } = 1;
    public int IDPVNRATE { get; set; } = 1;
    public int UNITS { get; set; } = 1;
    public int? IDCREDROW { get; set; }
    public decimal AMOUNT { get; set; }
    public float DISCOUNT { get; set; }
    public decimal PRICE0 { get; set; }
    public decimal PRICE { get; set; }
    public decimal BUYPRICE { get; set; }
    public decimal OLDPRICE { get; set; }
    public decimal? TOLDPRICE { get; set; }
    public decimal TPRICE { get; set; }
    public decimal TBUYPRICE { get; set; }
    public long IDSEQ { get; set; } = -1;
    public string ACC6 { get; set; } = ".?";
    public string ACC7 { get; set; } = ".?";
    //public int? ISGONE { get; set; }
    public DateTime TS { get; set; }
    public virtual M_DOCS Doc { get; set; }
    public virtual M_ITEMS Item { get; set; }
    public virtual M_PVNRATES PVNRate { get; set; }
    public virtual M_UNITS UnitsRow { get; set; }

    public bool XIsServices => Item.XIsServices;

    public override string GetInfoView()
    {
        var ret =
            $""""
            [Dokumenta rinda] 
              Dolumenta datums: {Utils.DateNToString(Doc?.DT)}
              Dolumenta numurs: {Doc?.DocSrNr}
              Dolumenta veids: {Doc?.DocType?.NAME}
              Izsniegts: {Doc?.StoreOut?.NAME}
              Saņemts: {Doc?.StoreIn?.NAME}
              Artikuls: {Item?.BARCODE}
              Artikula nosaukums: {Item?.NAME}
              Daudzums: {AMOUNT}
              Cena bez atlaides: {PRICE0}
              Atlaide: {DISCOUNT}
              Cena ar atlaides: {PRICE}
              Summa: {TPRICE}
              ...
            """";
        return ret;
    }

}
