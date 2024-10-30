using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KlonsLIB.Data;

namespace Klons3.ModelsFRep
{
    public partial class F_SP_DIFF_12 : EntityBase
    {
        public int TP { get; set; }
        public int DOCID { get; set; }
        public int ROWID { get; set; }
        public DateTime? DOCDTL { get; set; }
        public DateTime? DOCDTLD { get; set; }
        public DateTime? DOCODT { get; set; }
        public DateTime? ROWDTL { get; set; }
        public DateTime? ROWDTLD { get; set; }
        public DateTime? ROWODT { get; set; }
        public DateTime DETE { get; set; }
        public int ZNR { get; set; }
        public string NRX { get; set; }
        public string DOCTYP { get; set; }
        public string DOCST { get; set; }
        public string DOCNR { get; set; }
        public string DOCTYP2 { get; set; }
        public string CLID { get; set; }
        public string CLID2 { get; set; }
        public string DESCR { get; set; }
        public decimal DOCSUMM { get; set; }
        public decimal DOCPVN { get; set; }
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
        public decimal SUMMC { get; set; }
        public decimal SUMM { get; set; }
        public float? QV { get; set; }
        public string DESCR2 { get; set; }

    }
}
