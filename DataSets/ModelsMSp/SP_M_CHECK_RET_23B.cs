using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klons3.ModelsM;

public class SP_M_CHECK_RET_23B
{
    public int IDROW { get; set; }
    public int CREDDOCID { get; set; }
    public int CREDROWID { get; set; }
    public int IDITEM { get; set; }
    public decimal AMOUNT { get; set; }
    public decimal BUYPRICE { get; set; }
    public decimal TBUYPRICE { get; set; }
    public DateTime CREDDOCDT { get; set; }
    public string CREDDOCSR { get; set; }
    public string CREDDOCNR { get; set; }
}
