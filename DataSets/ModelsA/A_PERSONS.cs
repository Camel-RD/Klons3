using System;
using System.Collections.Generic;
using System.Drawing;
using KlonsLIB.Data;

namespace Klons3.ModelsA;

public partial class A_PERSONS : EntityBase
{
    public int ID { get; set; }
    public string FNAME { get; set; } = "?";
    public string LNAME { get; set; } = "?";
    public string NAME_DATIVE { get; set; }
    public string NAME_ACCUSATIVE { get; set; }
    public short GENDER { get; set; }
    public DateTime BIRTH_DATE { get; set; }
    public string PK { get; set; } = "?";
    public string ADDRESS { get; set; }
    public string CITY { get; set; }
    public string STATE { get; set; }
    public string COUNTRY { get; set; }
    public string POSTAL_CODE { get; set; }
    public string TERRITORIAL_CODE { get; set; }
    public string PASSPORT_NO { get; set; }
    public string PASSPORT_ISSUER { get; set; }
    public DateTime? PASSPORT_DATE { get; set; }
    public string PHONE { get; set; }
    public string EMAIL { get; set; }
    public int? BANK_ID { get; set; }
    public string BANK_ACC { get; set; }
    public string COMMENTS { get; set; }
    public decimal PAY0 { get; set; }
    public decimal IIN0 { get; set; }
    public decimal ADVANCE { get; set; }
    public float VACATION_DAYS { get; set; }
    public short USED { get; set; } = 1;
    public DateTime? USED_DT1 { get; set; }
    public DateTime? USED_DT2 { get; set; }
    public DateTime TS { get; set; }
    public virtual ObservableListViewSource<A_EVENTS> Events { get; set; } = new ObservableListViewSource<A_EVENTS>();
    public virtual ObservableListSource<A_PASTDATA> PastDataRows { get; set; } = new ObservableListSource<A_PASTDATA>();
    public virtual ObservableListSource<A_PASTDATA_2> PastDat2Rows { get; set; } = new ObservableListSource<A_PASTDATA_2>();
    public virtual ObservableListSource<A_PAYLISTS_R> PayListRows { get; set; } = new ObservableListSource<A_PAYLISTS_R>();
    public virtual ObservableListSource<A_PAYLIST_TEMPL_R> PayListTemplateRows { get; set; } = new ObservableListSource<A_PAYLIST_TEMPL_R>();
    public virtual ObservableListViewSource<A_PERSONS_R> PersonsStateRows { get; set; } = new ObservableListViewSource<A_PERSONS_R>();
    public virtual ObservableListSource<A_PIECEWORK> PieceWorkRows { get; set; } = new ObservableListSource<A_PIECEWORK>();
    public virtual ObservableListViewSource<A_POSITIONS> Positions { get; set; } = new ObservableListViewSource<A_POSITIONS>();
    public virtual ObservableListViewSource<A_POSITIONS_PLUSMINUS> PositionsPlusMinusRows { get; set; } = new ObservableListViewSource<A_POSITIONS_PLUSMINUS>();
    public virtual ObservableListViewSource<A_SALARY_PLUSMINUS> SalaryPlusMinusRows { get; set; } = new ObservableListViewSource<A_SALARY_PLUSMINUS>();
    public virtual ObservableListSource<A_SALARY_SHEETS_R> SalartSheetRows { get; set; } = new ObservableListSource<A_SALARY_SHEETS_R>();
    public virtual ObservableListSource<A_SALARY_SHEET_TEMPL_R> SalartSheetTemplateRows { get; set; } = new ObservableListSource<A_SALARY_SHEET_TEMPL_R>();
    public virtual ObservableListSource<A_TIMESHEET_LISTS_R> TimesheetListRows { get; set; } = new ObservableListSource<A_TIMESHEET_LISTS_R>();
    public virtual ObservableListSource<A_TIMESHEET_TEMPL_R> TimeSheetTemplateRows { get; set; } = new ObservableListSource<A_TIMESHEET_TEMPL_R>();
    public virtual ObservableListSource<A_UNTAXED_MIN> UntaxedMinRows { get; set; } = new ObservableListSource<A_UNTAXED_MIN>();

    public virtual A_BANKS BANK { get; set; }

    public string ZNAME => $"{FNAME} {LNAME} {PK}";
    public string YNAME => $"{FNAME} {LNAME}";

    public override string GetInfoView()
    {
        var ret = $""""
                  [Persons] 
                    Persona: {ZNAME}
                    ...
                  """";
        return ret;
    }



}