using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klons3.ModelsM;

public class SP_M_ITEMMOVEMENT_01
{
    public int GTP { get; set; }
    public DateTime? DT { get; set; }
    public string SR { get; set; }
    public string NR { get; set; }
    public int TP { get; set; }
    public int? IDSTOREOUT { get; set; }
    public int? IDSTOREIN { get; set; }
    public decimal AMOUNT { get; set; }
}
