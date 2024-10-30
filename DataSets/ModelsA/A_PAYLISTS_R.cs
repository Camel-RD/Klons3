using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsA;

public partial class A_PAYLISTS_R : EntityBase
{
    public int ID { get; set; }
    public int IDS { get; set; }
    public int? IDP { get; set; }
    public int? IDAM { get; set; }
    public int SNR { get; set; } = 1;
    public decimal PAY0 { get; set; }
    public decimal IIN0 { get; set; }
    public decimal ADVANCE0 { get; set; }
    public decimal WITHHOLDINGS0 { get; set; }
    public decimal TPAY0 { get; set; }
    public decimal PAY { get; set; }
    public decimal IIN { get; set; }
    public decimal ADVANCE { get; set; }
    public decimal WITHHOLDINGS { get; set; }
    public decimal TPAY { get; set; }
    public decimal PAY_REVERSE { get; set; }
    public decimal IIN_REVERSE { get; set; }
    public DateTime? DT1 { get; set; }
    public DateTime? DT2 { get; set; }
    public float R1 { get; set; }
    public float R2 { get; set; }
    public decimal S0 { get; set; }
    public decimal S1 { get; set; }
    public decimal S2 { get; set; }
    public int? ID_SHR_1 { get; set; }
    public int? ID_SHR_2 { get; set; }
    public decimal PAY_TAXED_1 { get; set; }
    public decimal PAY_NOSAI_1 { get; set; }
    public decimal PAY_NOTTAXED_1 { get; set; }
    public decimal UNTAXED_MINIMUM_1 { get; set; }
    public decimal IINEX_DEPENDANTS_1 { get; set; }
    public decimal IINEX2_1 { get; set; }
    public short IINEX2_TP_1 { get; set; }
    public decimal DNSI_1 { get; set; }
    public decimal PFNT_1 { get; set; }
    public decimal LINT_1 { get; set; }
    public decimal HINT_1 { get; set; }
    public decimal IINEX_EXP_1 { get; set; }
    public decimal IIN_1 { get; set; }
    public decimal PAY_TAXED_2 { get; set; }
    public decimal PAY_NOSAI_2 { get; set; }
    public decimal PAY_NOTTAXED_2 { get; set; }
    public decimal UNTAXED_MINIMUM_2 { get; set; }
    public decimal IINEX_DEPENDANTS_2 { get; set; }
    public decimal IINEX2_2 { get; set; }
    public short IINEX2_TP_2 { get; set; }
    public decimal DNSI_2 { get; set; }
    public decimal PFNT_2 { get; set; }
    public decimal LINT_2 { get; set; }
    public decimal HINT_2 { get; set; }
    public decimal IINEX_EXP_2 { get; set; }
    public decimal IIN_2 { get; set; }
    public DateTime TS { get; set; }
    public virtual A_POSITIONS Position { get; set; }
    public virtual A_PERSONS Person { get; set; }
    public virtual A_PAYLISTS PayList { get; set; }

    public override string GetInfoView()
    {
        var ret = $""""
                  [Maksājumu saraksta rinda] 
                    Saraksta Nr.: {PayList?.SNR}
                    Saraksta gads: {PayList?.YR}
                    Saraksta mēnesis: {PayList?.MT}
                    Saraksta Datums: {Utils.DateNToString(PayList?.DT)}
                    Nr.: {SNR}
                    Neizmaksāts: {TPAY0}
                    Maksāt: {TPAY}
                    ...
                  """";
        return ret;
    }

}