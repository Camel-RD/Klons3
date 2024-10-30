using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsA;

public partial class A_RATES : EntityBase
{
    public int ID { get; set; }
    public DateTime ONDATE { get; set; }
    public decimal IIN_LIKME { get; set; }
    public decimal SIDD_PAMATLIKME { get; set; }
    public decimal SIDN_PAMATLIKME { get; set; }
    public decimal SIDD_PENS { get; set; }
    public decimal SIDN_PENS { get; set; }
    public decimal SIDD_IZDPENS { get; set; }
    public decimal SIDN_IZDPENS { get; set; }
    public decimal SIDD_IESLODZ { get; set; }
    public decimal SIDN_IESLODZ { get; set; }
    public decimal SIDD_IESLODZ_PENS { get; set; }
    public decimal SIDN_IESLODZ_PENS { get; set; }
    public decimal NEPLIEK_MIN { get; set; }
    public decimal APGAD { get; set; }
    public decimal INVALID_12 { get; set; }
    public decimal INVALID_3 { get; set; }
    public decimal REPR { get; set; }
    public decimal PRET { get; set; }
    public decimal URN { get; set; }
    public decimal MIN_PAY_MONTH { get; set; }
    public decimal MIN_PAY_HOUR { get; set; }
    public decimal IIN_LIKME_2 { get; set; }
    public decimal IIN_LIKME_3 { get; set; }
    public decimal IIN_SLIEKSNIS_1 { get; set; }
    public decimal IIN_SLIEKSNIS_2 { get; set; }
    public DateTime TS { get; set; }

    public override string GetInfoView()
    {
        var ret = $""""
                  [Limes] 
                    Datums: {Utils.DateToString(ONDATE)}
                    ...
                  """";
        return ret;
    }

}