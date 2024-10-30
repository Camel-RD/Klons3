using Klons3.ModelsF;
using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsFRep;

public partial class F_TREPOPSD : EntityBase
{
    public int ID { get; set; }
    public DateTime DETE { get; set; }
    public string NRX { get; set; }
    public string DOCTYP { get; set; }
    public string DOCST { get; set; }
    public string DOCNR { get; set; }
    public string CLID { get; set; }
    public string CLID2 { get; set; }
    public string DESCR { get; set; }
    public decimal SUMM { get; set; }
    public decimal PVN { get; set; }
    public int? OPT { get; set; }
    public int? ZNR { get; set; }
    public string ZU { get; set; }
    public DateTime? ZDT { get; set; }
    public DateTime? DT2 { get; set; }
    public short? NL { get; set; }
    public int? YR { get; set; }
}