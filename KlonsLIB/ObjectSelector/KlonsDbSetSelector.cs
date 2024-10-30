using KlonsLIB.Data;
using KlonsLIB.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlonsLIB.ObjectSelector
{
    public class KlonsDbSetSelector : ObjectSelector
    {
        public static readonly KlonsDbSetSelector Empty = new KlonsDbSetSelector(null, null);
        public KlonsDbSetSelector(string pdatasource, string pdatamember) 
            : base(pdatasource, pdatamember) { }
        public override List<string> GetDataSources()
        {
            return KlonsDataModuleEF.St.GetDataSetNames()?.ToList(); ;
        }

        public override List<string> GetDatamembers(string dataset)
        {
            return KlonsDataModuleEF.St.GetTableNames(dataset)?.ToList();
        }

        public bool IsNotSet => DataSource.Value.IsNOE() || DataMember.Value.IsNOE();
    }
}
