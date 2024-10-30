using KlonsA.Classes;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using static System.Windows.Forms.DataFormats;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_SP_REP_AGGREGATE : EntityBase
{
    public int? IDP { get; set; }
    public int? IDAM { get; set; }
    public string? IDDEP { get; set; }
    public int YR { get; set; }
    public int MT { get; set; }
    public float WORKHOURS { get; set; }
    public int WORKDAYS { get; set; }
    public int FREEAVPAYDAYS { get; set; }
    public float VACATIONDAYS { get; set; }
    public float VACATIONDAYSCOMP { get; set; }
    public int SICKDAYS { get; set; }
    public decimal WORKPAY { get; set; }
    public decimal SICKPAY { get; set; }
    public decimal VACATIONPAY { get; set; }
    public decimal VACATIONPAYCOMP { get; set; }
    public decimal FREEAVPAY { get; set; }
    public decimal WORKAVPAY { get; set; }
    public decimal PLUS_TAXED { get; set; }
    public decimal PLUS_NOTTAXED { get; set; }
    public decimal PLUS_AUTHORS_FEES { get; set; }
    public decimal PLUS_NOSAI { get; set; }
    public decimal TOTALPAY { get; set; }
    public decimal FORSAI { get; set; }
    public decimal DNSN_AMOUNT { get; set; }
    public decimal DDSN_AMOUNT { get; set; }
    public decimal UNTAXED_MINIMUM { get; set; }
    public decimal IIN_EXEMPT_DEPENDANTS { get; set; }
    public decimal IIN_EXEMPT_INVALIDITY { get; set; }
    public decimal IIN_EXEMPT_NATIONAL_MOVEMENT { get; set; }
    public decimal IIN_EXEMPT_RETALIATION { get; set; }
    public decimal IIN_EXEMPT_EXPENSES { get; set; }
    public decimal PLUS_PF_NOTTAXED { get; set; }
    public decimal PLUS_LI_NOTTAXED { get; set; }
    public decimal PLUS_HI_NOTTAXED { get; set; }
    public decimal IIN_AMOUNT { get; set; }
    public decimal MINUS_BEFORE_IIN { get; set; }
    public decimal MINUS_AFTER_IIN { get; set; }
    public decimal PAY { get; set; }
    public decimal ADVANCE { get; set; }
    public decimal PAYT { get; set; }
    public decimal PAID { get; set; }

}

