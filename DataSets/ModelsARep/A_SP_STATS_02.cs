using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klons3.ModelsA;

public class A_SP_STATS_02
{
    public int IDP { get; set; }
    public int? IDAM { get; set; }
    public string FNAME { get; set; }
    public string LNAME { get; set; }
    public string POSTITLE { get; set; }
    public int TAG { get; set; }
    public string ZNAME => $"{FNAME} {LNAME}";
}
