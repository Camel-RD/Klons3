using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klons3.ModelsM;

public class SP_M_REP_COSTACC_2
{
    public int ID { get; set; }
    public DateTime DT { get; set; }
    public string SR { get; set; }
    public string NR { get; set; }
    public int TP { get; set; }
    public int IDSTOREOUT { get; set; }
    public int IDSTOREIN { get; set; }
    public string ACC1 { get; set; }
    public string ACC2 { get; set; }
    public decimal COST { get; set; }
}
