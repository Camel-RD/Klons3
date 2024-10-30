using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class F_VW_OPS : EntityBase
{
    public int ID { get; set; }
    public int DOCID { get; set; }
    public DateTime DETE { get; set; }
    public string NRX { get; set; }
    public int ZNR { get; set; }
    public string DOCTYP { get; set; }
    public string DOCST { get; set; }
    public string DOCNR { get; set; }
    public string DOCTYP2 { get; set; }
    public string CLID { get; set; }
    public string NAME { get; set; }
    public string REGNR { get; set; }
    public string CLID2 { get; set; }
    public string NAME2 { get; set; }
    public string REGNR2 { get; set; }
    public string DESCR { get; set; }
    public string AC11 { get; set; }
    public string AC12 { get; set; }
    public string AC13 { get; set; }
    public string AC14 { get; set; }
    public string AC15 { get; set; }
    public string AC21 { get; set; }
    public string AC22 { get; set; }
    public string AC23 { get; set; }
    public string AC24 { get; set; }
    public string AC25 { get; set; }
    public string CUR { get; set; }
    public float? QV { get; set; }
    public decimal Summ { get; set; }
    public string DESCR2 { get; set; }
}