using System;
using System.Collections.Generic;
using KlonsLIB.Data;

namespace Klons3.ModelsF;

public partial class PARAMS : EntityBase
{
    public int ID { get; set; }
    public string PNAME { get; set; }
    public string PVALUE { get; set; }
    public string USR { get; set; }
    public virtual USERS USRRow { get; set; }
}
