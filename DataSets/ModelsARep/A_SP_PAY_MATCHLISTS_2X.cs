using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_SP_PAY_MATCHLISTS_2X : EntityBase
{
    public int ID_SHR { get; set; }
    public DateTime? DT1 { get; set; }
    public DateTime? DT2 { get; set; }

    public decimal IIN_RATE { get; set; }
    public decimal IIN_RATE2 { get; set; }
    public decimal SI_RATE { get; set; }
    public decimal PAY_TAXED { get; set; }
    public decimal PAY_NOSAI { get; set; }
    public decimal PAY_NOTTAXED { get; set; }
    public decimal NOTPAID_TAXED { get; set; }
    public decimal NOTPAID_NOSAI { get; set; }
    public decimal NOTPAID_NOTTAXED { get; set; }
    public decimal DNSI { get; set; }
    public decimal UNTAXED_MINIMUM { get; set; }
    public decimal IINEX_DEPENDANTS { get; set; }
    public decimal IINEX2 { get; set; }
    public short? IINEX2_TP { get; set; }
    public decimal PF_T { get; set; }
    public decimal LI_T { get; set; }
    public decimal HI_T { get; set; }
    public decimal PF_NT { get; set; }
    public decimal LI_NT { get; set; }
    public decimal HI_NT { get; set; }
    public decimal IINEX_EXP { get; set; }
    public decimal IIN { get; set; }
    public decimal PAY { get; set; }
}

