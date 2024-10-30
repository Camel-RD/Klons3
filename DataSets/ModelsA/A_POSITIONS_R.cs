using KlonsA.Classes;
using KlonsLIB.Misc;
using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Klons3.ModelsA;

public partial class A_POSITIONS_R : EntityBase
{
    public int ID { get; set; }
    public int IDA { get; set; }
    public string IDDEP { get; set; }
    public string TITLE { get; set; } = "?";
    public string TITLE_DATIVE { get; set; } = "?";
    public string TITLE_ACCUSATIVE { get; set; } = "?";
    public short SALARY_TYPE { get; set; }
    public short SIXDAYWEEK { get; set; }
    public int NORMAL_DAY_HOURS { get; set; } = 8;
    public int NORMAL_WEEK_HOURS { get; set; } = 40;
    public decimal RATE { get; set; }
    public decimal RATE_NIGHT { get; set; } = 50M;
    public short RATE_NIGHT_TYPE { get; set; }
    public decimal RATE_OVERTIME { get; set; } = 200M;
    public short RATE_OVERTIME_TYPE { get; set; }
    public decimal RATE_HOLIDAY { get; set; } = 100M;
    public short RATE_HOLIDAY_TYPE { get; set; }
    public decimal RATE_HOLIDAY_NIGHT { get; set; }
    public short RATE_HOLIDAY_NIGHT_TYPE { get; set; }
    public decimal RATE_HOLIDAY_OVERTIME { get; set; }
    public short RATE_HOLIDAY_OVERTIME_TYPE { get; set; }
    public short APPLY_IIN_EXEMPTIONS { get; set; } = 1;
    public decimal ADVAMCE { get; set; }
    public string OCCUPATION_CODE { get; set; }
    public short USED { get; set; } = 1;
    public DateTime EDIT_DATE { get; set; }
    public DateTime TS { get; set; }
    public virtual A_POSITIONS Position { get; set; }
    public virtual A_DEPARTMENTS Department { get; set; }

    public ESalaryType XRateType
    {
        get { return (ESalaryType)this.SALARY_TYPE; }
        set { this.SALARY_TYPE = (short)value; }
    }

    public string EDIT_DATE_STR => Utils.DateToString(EDIT_DATE);

    public override string GetInfoView()
    {
        string[] algas_veids = ["mēneša", "dienas", "stundas", "summētais", "gabaldarbs"];
        var ret = $""""
                  [Amata datu labojuma ieraksts] 
                    Persona: {Position?.Person?.YNAME}
                    Amats: {TITLE}
                    Labojuma datumso: {Utils.DateToString(EDIT_DATE)}
                    Struktūrvienība: {IDDEP}
                    Algas aprēķina veids: {algas_veids[SALARY_TYPE]}
                    Algas likme: {RATE}
                    Likme: {RATE}
                    6 darba dienu nedēļa: {(SIXDAYWEEK == 0 ? "nē" : "jā")}
                    Stundas dienā.: {NORMAL_DAY_HOURS}
                    Stundas nedēļā: {NORMAL_WEEK_HOURS}
                    ...
                  """";
        return ret;
    }

}