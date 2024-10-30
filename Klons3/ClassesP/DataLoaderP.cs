using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KlonsLIB.Misc;
using KlonsLIB.Data;
using KlonsLIB.Forms;
using KlonsF.Classes;

namespace KlonsP.Classes
{
    public static class DataLoaderP
    {
        public static KlonsData MyData => KlonsData.St;

        public static void ClearAll()
        {
            MyData.RecreateDbContextP();
        }

        public static bool FillAll()
        {
            var ctx = MyData.DbContextP;

            IQueryable[] tables = [
                ctx.P_CATD,
                ctx.P_CATT,
                ctx.P_DEPARTMENTS,
                ctx.P_PLACES,
                ctx.P_CAT1,
                ctx.P_EVENTS,
                ctx.P_TAXDEPRECYEAR,
                ctx.P_ITEMS,
                ctx.P_ITEMS_EVENTS,
                ];

            foreach (var table in tables)
                LoadTable(table);

            return true;
        }

        public static void LoadTable(IQueryable table)
        {
            foreach (var item in table) { }
        }

        public static bool ReLoadData()
        {
            try
            {
                ClearAll();
                return FillAll();
            }
            catch (Exception e)
            {
                var ex2 = new MyException("Neizdevās ielādēt datus", e);
                Form_Error.ShowException(ex2);
                return false;
            }
        }

        public static bool CheckData()
        {
            if (HasData()) return true;
            ClearAll();
            return FillAll();
        }

        public static bool HasData()
        {
            var ds = MyData.DbContextP;
            return ds.P_CAT1.Local.Count > 0;
        }



    }
}
