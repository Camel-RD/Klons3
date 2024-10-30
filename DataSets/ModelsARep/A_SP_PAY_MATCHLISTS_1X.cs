using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_SP_PAY_MATCHLISTS_1X : EntityBase
{
    public int? IDP { get; set; }
    public int IDAM { get; set; }
    public DateTime? DT1 { get; set; }
    public DateTime? DT2 { get; set; }
    public int? IDSHR1 { get; set; }
    public int? IDSHR2 { get; set; }
    public decimal PAY0 { get; set; }
    public decimal PAY1 { get; set; }
    public decimal PAY2 { get; set; }
    public float R1 { get; set; }
    public float R2 { get; set; }
    public decimal IIN { get; set; }

    public DateTime? VA_SHEET_DT1 { get; set; }
    public DateTime? VA_SHEET_DT2 { get; set; }
    public decimal VA_IIN_RATE { get; set; }
    public decimal VA_IIN_RATE2 { get; set; }
    public decimal VA_SI_RATE { get; set; }
    public decimal VA1_PAY_TAXED { get; set; }
    public decimal VA1_PAY_NOSAI { get; set; }
    public decimal VA1_PAY_NOTTAXED { get; set; }
    public decimal VA1_NOTPAID_TAXED { get; set; }
    public decimal VA1_NOTPAID_NOSAI { get; set; }
    public decimal VA1_NOTPAID_NOTTAXED { get; set; }
    public decimal VA1_DNSI { get; set; }
    public decimal VA1_UNTAXED_MINIMUM { get; set; }
    public decimal VA1_IINEX_DEPENDANTS { get; set; }
    public decimal VA1_IINEX2{ get; set; }
    public short? VA1_IINEX2_TP { get; set; }
    public decimal VA1_PF_T { get; set; }
    public decimal VA1_LI_T { get; set; }
    public decimal VA1_HI_T { get; set; }
    public decimal VA1_PF_NT { get; set; }
    public decimal VA1_LI_NT { get; set; }
    public decimal VA1_HI_NT { get; set; }
    public decimal VA1_IINEX_EXP { get; set; }
    public decimal VA1_IIN { get; set; }
    public decimal VA1_PAY { get; set; }


    public decimal VA2_PAY_TAXED { get; set; }
    public decimal VA2_PAY_NOSAI { get; set; }
    public decimal VA2_PAY_NOTTAXED { get; set; }
    public decimal VA2_DNSI { get; set; }
    public decimal VA2_UNTAXED_MINIMUM { get; set; }
    public decimal VA2_IINEX_DEPENDANTS { get; set; }
    public decimal VA2_IINEX2 { get; set; }
    public decimal VA2_PF_NT { get; set; }
    public decimal VA2_LI_NT { get; set; }
    public decimal VA2_HI_NT { get; set; }
    public decimal VA2_IINEX_EXP { get; set; }
    public decimal VA2_IIN { get; set; }
    public decimal VA2_PAY { get; set; }


    public DateTime? VB_SHEET_DT1 { get; set; }
    public DateTime? VB_SHEET_DT2 { get; set; }
    public decimal VB_IIN_RATE { get; set; }
    public decimal VB_IIN_RATE2 { get; set; }
    public decimal VB_SI_RATE { get; set; }
    public decimal VB1_PAY_TAXED { get; set; }
    public decimal VB1_PAY_NOSAI { get; set; }
    public decimal VB1_PAY_NOTTAXED { get; set; }
    public decimal VB1_NOTPAID_TAXED { get; set; }
    public decimal VB1_NOTPAID_NOSAI { get; set; }
    public decimal VB1_NOTPAID_NOTTAXED { get; set; }
    public decimal VB1_DNSI { get; set; }
    public decimal VB1_UNTAXED_MINIMUM { get; set; }
    public decimal VB1_IINEX_DEPENDANTS { get; set; }
    public decimal VB1_IINEX2 { get; set; }
    public short? VB1_IINEX2_TP { get; set; }
    public decimal VB1_PF_T { get; set; }
    public decimal VB1_LI_T { get; set; }
    public decimal VB1_HI_T { get; set; }
    public decimal VB1_PF_NT { get; set; }
    public decimal VB1_LI_NT { get; set; }
    public decimal VB1_HI_NT { get; set; }
    public decimal VB1_IINEX_EXP { get; set; }
    public decimal VB1_IIN { get; set; }
    public decimal VB1_PAY { get; set; }


    public decimal VB2_PAY_TAXED { get; set; }
    public decimal VB2_PAY_NOSAI { get; set; }
    public decimal VB2_PAY_NOTTAXED { get; set; }
    public decimal VB2_DNSI { get; set; }
    public decimal VB2_UNTAXED_MINIMUM { get; set; }
    public decimal VB2_IINEX_DEPENDANTS { get; set; }
    public decimal VB2_IINEX2 { get; set; }
    public decimal VB2_PF_NT { get; set; }
    public decimal VB2_LI_NT { get; set; }
    public decimal VB2_HI_NT { get; set; }
    public decimal VB2_IINEX_EXP { get; set; }
    public decimal VB2_IIN { get; set; }
    public decimal VB2_PAY { get; set; }


}

