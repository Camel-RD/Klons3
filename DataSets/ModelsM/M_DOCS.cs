using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsM;

public partial class M_DOCS : EntityBase
{
    public int ID { get; set; }
    public DateTime DT { get; set; }
    public string SR { get; set; }
    public string NR { get; set; }
    public int TP { get; set; } = 1;
    public int PVNTYPE { get; set; } = 1;
    public int STATE { get; set; }
    public int IDSTOREIN { get; set; } = 1;
    public int IDSTOREOUT { get; set; } = 1;
    public int? IDCREDDOC { get; set; }
    public DateTime? CREDDOCDT { get; set; }
    public decimal SUMM { get; set; }
    public long IDSEQ { get; set; } = -1;
    public string ACCIN { get; set; } = ".?";
    public string ACCOUT { get; set; } = ".?";
    public string CREDDOCSR { get; set; }
    public string CREDDOCNR { get; set; }
    public int? IDCARRIER { get; set; }
    public int? IDADDRESSOUT { get; set; }
    public int? IDADDRESSIN { get; set; }
    public int? IDVEHICLE { get; set; }
    public int IDTRANSACTIONTYPE { get; set; } = 1;
    public int? IDPAYMENTTYPE { get; set; }
    public int? IDDRIVER { get; set; }
    public short ACCOUNTINGTP { get; set; } = 1;
    public short ACCTP1 { get; set; } = 1;
    public short ACCTP2 { get; set; } = 1;
    public short WEVATPAYER { get; set; } = 1;
    public DateTime? DUEDATE { get; set; }
    public DateTime TS { get; set; }
    public virtual M_ADDRESSSES AddressIn { get; set; }
    public virtual M_ADDRESSSES AddressOut { get; set; }
    public virtual M_STORES Carrier { get; set; }
    public virtual M_CONTACTS Driver { get; set; }
    public virtual M_PAYMENTTYPE PaymentType { get; set; }
    public virtual M_STORES StoreIn { get; set; }
    public virtual M_STORES StoreOut { get; set; }
    public virtual M_TRANSACTIONTYPE TransactionType { get; set; }
    public virtual M_VEHICLES Vehicle { get; set; }
    public virtual ObservableListViewSource<M_ROWS> Rows { get; set; } = new();
    public virtual M_DOCTYPES DocType { get; set; }

    public string DocSrNr => $"{SR} {NR}".Trim();


    public KlonsM.Classes.EDocState XState
    {
        get => (KlonsM.Classes.EDocState)STATE;
        set => STATE = (int)value;
    }

    public KlonsM.Classes.EPVNType XPVNType
    {
        get => (KlonsM.Classes.EPVNType)PVNTYPE;
        set => PVNTYPE = (int)value;
    }

    public KlonsM.Classes.EDocType XDocType
    {
        get => (KlonsM.Classes.EDocType)TP;
        set => TP = (int)value;
    }

    public KlonsM.Classes.EDocType2 XDocType2
    {
        get => (KlonsM.Classes.EDocType2)IDTRANSACTIONTYPE;
        set => IDTRANSACTIONTYPE = (int)value;
    }

    public KlonsM.Classes.EStoreType XStoreInType => StoreIn.XStoreType;

    public KlonsM.Classes.EStoreType XStoreOutType => StoreOut.XStoreType;

    public KlonsM.Classes.EPVNType XStoreInPVNType => StoreIn.XStorePVNType;

    public KlonsM.Classes.EPVNType XStoreOutPVNType => StoreOut.XStorePVNType;

    public KlonsM.Classes.EAccountingType XAccountingType
    {
        get => (KlonsM.Classes.EAccountingType)ACCOUNTINGTP;
        set => ACCOUNTINGTP = (short)value;
    }

    public bool XDoAutoFinOps
    {
        get => ACCTP1 == 1;
        set => ACCTP1 = (short)(value ? 1 : 0);
    }

    public bool XIncludeInCostCalc
    {
        get => ACCTP2 == 1;
        set => ACCTP2 = (short)(value ? 1 : 0);
    }

    public bool XWeVATPayer
    {
        get => WEVATPAYER == 1;
        set => WEVATPAYER = (short)(value ? 1 : 0);
    }

    public bool IsOpenForChanges =>
        XState == KlonsM.Classes.EDocState.Melnraksts ||
        XState == KlonsM.Classes.EDocState.Atvērts;

    public override string GetInfoView()
    {
        var ret =
            $""""
            [Dokuments] 
              Datums: {Utils.DateToString(DT)}
              Numurs: {DocSrNr}
              Veids: {DocType?.NAME}
              Izsniegts: {StoreOut?.NAME}
              Saņemts: {StoreIn?.NAME}
              Summa: {SUMM}
              ...
            """";
        return ret;
    }

}