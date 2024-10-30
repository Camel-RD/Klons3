using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_PASTDATA_2 : EntityBase
{
    public int ID { get; set; }
    public int IDP { get; set; }
    public DateTime DT { get; set; }
    public decimal PAY_TAXED { get; set; }
    public decimal PAY_NOSAI { get; set; }
    public decimal PAY_NOTTAXED { get; set; }
    public decimal UNTAXED_MINIMUM { get; set; }
    public decimal IINEX_DEPENDANTS { get; set; }
    public decimal IINEX2 { get; set; }
    public short IINEX2_TP { get; set; }
    public decimal PF_NT { get; set; }
    public decimal LI_NT { get; set; }
    public decimal HI_NT { get; set; }
    public decimal IINEX_EXP { get; set; }
    public decimal IIN { get; set; }
    public decimal PAY { get; set; }
    public decimal ADVANCE { get; set; }
    public decimal WITHHOLDINGS { get; set; }
    public decimal PAYT { get; set; }
    public decimal PAY_TAXED_P { get; set; }
    public decimal PAY_NOSAI_P { get; set; }
    public decimal PAY_NOTTAXED_P { get; set; }
    public decimal UNTAXED_MINIMUM_P { get; set; }
    public decimal IINEX_DPENDANTS_P { get; set; }
    public decimal IINEX2_P { get; set; }
    public decimal PF_NT_P { get; set; }
    public decimal LI_NT_P { get; set; }
    public decimal HI_NT_P { get; set; }
    public decimal IINEX_EXP_P { get; set; }
    public decimal IIN_P { get; set; }
    public decimal PAY_P { get; set; }
    public decimal ADVANCE_P { get; set; }
    public decimal WITHHOLDINGS_P { get; set; }
    public decimal PAYT_P { get; set; }
    public DateTime TS { get; set; }
    public virtual A_PERSONS IDPNavigation { get; set; }
}