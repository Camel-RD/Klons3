using KlonsA.Classes;
using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsA;

public partial class A_EVENTS : EntityBase
{
    public int ID { get; set; }
    public int? IDP { get; set; }
    public int? IDA { get; set; }
    public int IDN { get; set; }
    public int? IDN2 { get; set; }
    public DateTime DATE1 { get; set; }
    public DateTime? DATE2 { get; set; }
    public DateTime? DATE3 { get; set; }
    public string DESCR { get; set; }
    public string DOCNR { get; set; }
    public string SCODE { get; set; }
    public float DAYS { get; set; }
    public string OCCUPATION_CODE { get; set; }
    public float DAYS2 { get; set; }
    public DateTime TS { get; set; }
    public virtual A_POSITIONS Position { get; set; }
    public virtual A_EVENT_TYPES2 EvenType2 { get; set; }
    public virtual A_EVENT_TYPES EventType { get; set; }
    public virtual A_PERSONS Person { get; set; }

    public EEventId EventCode
    {
        get { return (EEventId)this.IDN; }
        set { this.IDN = (int)value; }
    }

    public string YNAME => $"{Person.YNAME}";

    public override string GetInfoView()
    {
        var ret = $""""
                  [Notikums] 
                    Persona: {Person?.YNAME}
                    Amats: {Position?.TITLE}
                    Notikums: {EventType.DESCR}
                    Notikums2: {EvenType2.DESCR}
                    Datums no: {Utils.DateToString(DATE1)}
                    Datums līdz: {Utils.DateNToString(DATE2)}
                    ...
                  """";
        return ret;
    }

}