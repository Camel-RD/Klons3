using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Klons3.ModelsM;
using NotyfyProp;

namespace DataObjectsFM
{
    public class DocData : BindableComponent
    {
        public int _ID { get; set; }
        public DateTime _DT { get; set; }
        public string _SR { get; set; }
        public string _NR { get; set; }
        public int _TP { get; set; }
        public int _PVNTYPE { get; set; }
        public int _STATE { get; set; }
        public int _IDSTOREIN { get; set; }
        public int _IDSTOREOUT { get; set; }
        public int? _IDCREDDOC { get; set; }
        public DateTime? _CREDDOCDT { get; set; }
        public string _CREDDOCSR { get; set; }
        public string _CREDDOCNR { get; set; }
        public string _ACCIN { get; set; }
        public string _ACCOUT { get; set; }
        public int? _IDCARRIER { get; set; }
        public int? _IDADDRESSOUT { get; set; }
        public int? _IDADDRESSIN { get; set; }
        public int? _IDVEHICLE { get; set; }
        public int _IDTRANSACTIONTYPE { get; set; }
        public int? _IDPAYMENTTYPE { get; set; }
        public int? _IDDRIVER { get; set; }
        public short _ACCOUNTINGTP { get; set; }
        public short _ACCTP1 { get; set; }
        public short _ACCTP2 { get; set; }
        public short _WEVATPAYER { get; set; }
        public DateTime? _DUEDATE { get; set; }
        public decimal _SUMM { get; set; }

        public virtual M_ADDRESSSES _AddressIn { get; set; }
        public virtual M_ADDRESSSES _AddressOut { get; set; }
        public virtual M_STORES _Carrier { get; set; }
        public virtual M_CONTACTS _Driver { get; set; }
        public virtual M_PAYMENTTYPE _PaymentType { get; set; }
        public virtual M_STORES _StoreIn { get; set; }
        public virtual M_STORES _StoreOut { get; set; }
        public virtual M_TRANSACTIONTYPE _TransactionType { get; set; }
        public virtual M_VEHICLES _Vehicle { get; set; }
    }
}
