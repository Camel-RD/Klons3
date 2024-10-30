using KlonsP.Classes;
using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsP;

public partial class P_ITEMS : EntityBase
{
    public int ID { get; set; }
    public string REG_NR { get; set; }
    public string NAME { get; set; }
    public DateTime? DATE1 { get; set; }
    public DateTime? DATE2 { get; set; }
    public int CAT1 { get; set; }
    public int CATD { get; set; }
    public int CATT { get; set; }
    public int PLACE { get; set; }
    public int DEPARTMENT { get; set; }
    public decimal VALUE0 { get; set; }
    public decimal DEPREC { get; set; }
    public decimal VALUE_LEFT { get; set; }
    public int OPT { get; set; }
    public int STATE { get; set; } = 1;
    public DateTime? ZDT { get; set; }
    public string ZU { get; set; }
    public DateTime TS { get; set; }
    public virtual P_CAT1 Cat1Row { get; set; }
    public virtual P_CATD CatDRow { get; set; }
    public virtual P_CATT CatTRow { get; set; }
    public virtual P_DEPARTMENTS DepartmentRow { get; set; }
    public virtual P_PLACES PlaceRow { get; set; }
    public virtual ObservableListViewSource<P_ITEMS_EVENTS> Events { get; set; } = new ObservableListViewSource<P_ITEMS_EVENTS>();


    public DateTime? TDATE1 { get; set; }
    public DateTime? TDATE2 { get; set; }
    public int TCAT1 { get; set; }
    public int TCATD { get; set; }
    public int TCATT { get; set; }
    public int TPLACE { get; set; }
    public int TDEPARTMENT { get; set; }
    public decimal TVALUE0 { get; set; }
    public decimal TDEPREC0 { get; set; }
    public decimal TVALUE_LEFT { get; set; }
    public int TSTATE { get; set; } = 1;
    public string TERROR { get; set; }

    public EState XState
    {
        get { return (EState)this.STATE; }
        set
        {
            if (this.STATE == (int)value) return;
            this.STATE = (int)value;
        }
    }

    public EState XTState
    {
        get { return (EState)this.TSTATE; }
        set
        {
            if (this.TSTATE == (int)value) return;
            this.TSTATE = (int)value;
        }
    }

    public override string GetInfoView()
    {
        var ret = 
            $""""
            [Pamatlīdzeklis] 
              Nr.: {REG_NR}
              Nosaukums: {NAME}
              Uzskaites sākums: {Utils.DateNToString(DATE1)}
            """";
        return ret;
    }

}