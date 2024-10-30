using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klons3.ModelsA;

public class A_SP_STATS_01
{
    public int HIRED { get; set; }
    public int FIRED { get; set; }
    public int WORKING { get; set; }
    public int POS_ADDED { get; set; }
    public int POS_REMOVED { get; set; }
    public int POS_NO_TIME_TEMPL { get; set; }
    public int POS_NO_TIME { get; set; }
    public int POS_NO_SALARY_TEMPL { get; set; }
    public int POS_NO_SALARY { get; set; }
    public int POS_NO_PAY_TEMPL { get; set; }
    public int POS_NO_PAY { get; set; }
    public decimal SALARY_PAY { get; set; }
    public decimal PAID { get; set; }
    public decimal SALARY_PAY_PAID { get; set; }
}
