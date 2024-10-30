using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsFRep;

public partial class F_ROPS1A : EntityBase
{
    public int ID { get; set; }
    public int? SPID { get; set; }
    public int? DID { get; set; }
    public DateTime? DETE { get; set; }
    public string NRX { get; set; }
    public string DOCTYP { get; set; }
    public string DOCTYP1 { get; set; }
    public string DOCTYP2 { get; set; }
    public string DOCST { get; set; }
    public string DOCNR { get; set; }
    public string CLID { get; set; }
    public string CLID2 { get; set; }
    public string NAME { get; set; }
    public string NAME2 { get; set; }
    public string REGNR { get; set; }
    public string REGNR2 { get; set; }
    public decimal? B0 { get; set; }
    public string DESCR { get; set; }
    public string DESCR2 { get; set; }
    public string AC1 { get; set; }
    public string AC2 { get; set; }
    public string SAC1 { get; set; }
    public string SAC2 { get; set; }
    public decimal? SUMMC { get; set; }
    public string CUR { get; set; }
    public decimal? SUMM { get; set; }
    public decimal? PVN { get; set; }
    public string RAC { get; set; }
    public decimal? SDB { get; set; }
    public decimal? SCR { get; set; }
    public string RAC2 { get; set; }
    public string RAC3 { get; set; }
    public string RAC4 { get; set; }
    public string RAC5 { get; set; }
    public float? QV { get; set; }
    public int? ZNR { get; set; }
}