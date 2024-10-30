using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsM;

public partial class M_INV_ROWS : EntityBase
{
    public int ID { get; set; }
    public int IDDOC { get; set; }
    public int IDITEM { get; set; } = 1;
    public int IDUNITS { get; set; } = 1;
    public decimal AM_COUNTED_1 { get; set; }
    public decimal? AM_COUNTED_2 { get; set; }
    public decimal AM_CALC { get; set; }
    public decimal AM_DIFF { get; set; }
    public DateTime TS { get; set; }
    public virtual M_INV_DOCS Doc { get; set; }
    public virtual M_ITEMS Item { get; set; }
    public virtual M_UNITS Units { get; set; }

    public override string GetInfoView()
    {
        var ret =
            $""""
            [Inventarizācijas akta rinda] 
              Akta datums: {Utils.DateNToString(Doc?.DT)}
              Akta Nr.: {Doc?.NR}
              Noliktava: {Doc?.Store?.NAME}
              Artikuls: {Item?.BARCODE}
              Artikula nosaukums: {Item?.NAME}
              Skaitīts 1: {AM_COUNTED_1}
              Skaitīts 2: {AM_COUNTED_2}
              Aprēķināts: {AM_CALC}
            """";
        return ret;
    }

}