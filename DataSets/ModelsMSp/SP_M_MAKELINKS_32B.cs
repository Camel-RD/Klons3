using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klons3.ModelsM;

public class SP_M_MAKELINKS_32B
{
    public int IDROW { get; set; }
    public int CREDDOCID { get; set; }
    public int CREDROWID { get; set; }
    public int IDITEM { get; set; }
    public decimal AMOUNT { get; set; }
    public decimal PRICE { get; set; }
    public decimal TPRICE { get; set; }
    public DateTime CREDDOCDT { get; set; }
    public string CREDDOCSR { get; set; }
    public string CREDDOCNR { get; set; }
}
