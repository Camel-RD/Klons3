using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_LOPSD : EntityBase
{
    public int IDL { get; set; }
    public int? ID { get; set; }
    public DateTime? DTLD { get; set; }
    public string USL { get; set; }
    public int? TPL { get; set; }
    public DateTime? DETE { get; set; }
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
    public int ZNR { get; set; }
    public string ZU { get; set; }
    public DateTime? ZDT { get; set; }
    public short? NL { get; set; }
    public DateTime? ODT { get; set; }
}