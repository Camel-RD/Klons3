using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsM;

public partial class M_PVNRATES2 : EntityBase
{
    public int ID { get; set; }
    public int IDRATE { get; set; }
    public int IDTP { get; set; } = 1;
    public int IDTRTP { get; set; } = 1;
    public int INCURMT { get; set; }
    public int CHANGESIGN { get; set; }
    public int BASE_DEB_FIN { get; set; }
    public string BASE_DEB_PVN { get; set; }
    public int BASE_CRED_FIN { get; set; }
    public string BASE_CRED_PVN { get; set; }
    public int PVN_DEB_FIN { get; set; }
    public string PVN_DEB_PVN { get; set; }
    public int PVN_CRED_FIN { get; set; }
    public string PVN_CRED_PVN { get; set; }
    public int? IDPVNTEXT { get; set; }
    public DateTime TS { get; set; }
    public virtual M_PVNRATES PVNRate { get; set; }
    public virtual M_PVNTYPE PVNType { get; set; }

    public KlonsM.Classes.EPVNType XPvnType
    {
        get => (KlonsM.Classes.EPVNType)IDTP;
        set => IDTP = (int)value;
    }

    public KlonsM.Classes.EDocType XDocType
    {
        get => (KlonsM.Classes.EDocType)IDTRTP;
        set => IDTRTP = (int)value;
    }

    public KlonsM.Classes.EAccountsForTemplates XBaseDebFin
    {
        get => (KlonsM.Classes.EAccountsForTemplates)BASE_DEB_FIN;
        set => BASE_DEB_FIN = (int)value;
    }

    public KlonsM.Classes.EAccountsForTemplates XBaseCredFin
    {
        get => (KlonsM.Classes.EAccountsForTemplates)BASE_CRED_FIN;
        set => BASE_CRED_FIN = (int)value;
    }

    public KlonsM.Classes.EAccountsForTemplates XPvnDebFin
    {
        get => (KlonsM.Classes.EAccountsForTemplates)PVN_DEB_FIN;
        set => PVN_DEB_FIN = (int)value;
    }

    public KlonsM.Classes.EAccountsForTemplates XPvnCredFin
    {
        get => (KlonsM.Classes.EAccountsForTemplates)PVN_CRED_FIN;
        set => PVN_CRED_FIN = (int)value;
    }

    public bool XInCurrentMonth
    {
        get => INCURMT == 1;
        set => INCURMT = value ? 1 : 0;
    }

    public bool XChangeSign
    {
        get => CHANGESIGN == 1;
        set => CHANGESIGN = value ? 1 : 0;
    }
}