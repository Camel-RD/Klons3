using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Klons3.ModelsM;

public partial class M_STORES : EntityBase
{
    public int ID { get; set; }
    public int TP { get; set; } = 1;
    public int IDCAT { get; set; } = 1;
    public int PVNTP { get; set; } = 1;
    public string CODE { get; set; } = "?";
    public string NAME { get; set; } = "?";
    public string REGNR { get; set; }
    public string PVNREGNR { get; set; }
    public string ADDR { get; set; }
    public string ACC21 { get; set; } = ".?";
    public string ACC23 { get; set; } = ".?";
    public string ACC53 { get; set; } = ".?";

    public int? IDCOUNTRY { get; set; }
    public string STREET { get; set; }
    public string CITY { get; set; }
    public string STATE { get; set; }
    public string PARISH { get; set; }
    public string POSTALCODE { get; set; }
    public string EMAIL { get; set; }

    public DateTime TS { get; set; }
    public virtual M_STORES_CAT Category { get; set; }
    public virtual ObservableListViewSource<M_ADDRESSSES> Addresses { get; set; } = new();
    public virtual ObservableListViewSource<M_BANKACCOUNTS> BankAccounts { get; set; } = new();
    public virtual ObservableListViewSource<M_CONTACTS> Contacts { get; set; } = new();
    public virtual ObservableListSource<M_ITEMS> Items { get; set; } = new();
    public virtual ObservableListSource<M_ITEMS_PER_STORE> M_ITEMS_PER_STORE { get; set; } = new();
    public virtual ObservableListViewSource<M_VEHICLES> Vehicles { get; set; } = new();

    public virtual M_PVNTYPE PVNType { get; set; }
    public virtual M_STORETYPE StoreType { get; set; }
    public virtual M_COUNTRIES Country { get; set; }

    public KlonsM.Classes.EStoreType XStoreType
    {
        get => (KlonsM.Classes.EStoreType)TP;
        set => TP = (int)value;
    }

    public KlonsM.Classes.EPVNType XStorePVNType
    {
        get => (KlonsM.Classes.EPVNType)PVNTP;
        set => PVNTP = (int)value;
    }

    public override string GetInfoView()
    {
        string[] store_tytpes = ["?",
            "Nenoteikts",
            "Noliktava",
            "Darbinieks",
            "Partneris",
            "Partneris_turētājs",
            "Pakalpojumi",
            "Citi",
            "Noliktava_ārpus",
            "Ražošana"];
        var ret =
            $""""
            [Partneris/noliktava] 
              Veids: {store_tytpes[TP]}
              Kods: {CODE}
              Nosaukums: {NAME}
              ...
            """";
        return ret;
    }

}
