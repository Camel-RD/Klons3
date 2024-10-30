using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KlonsLIB.Misc;
using KlonsP.Classes;
using Klons3.ModelsP;

namespace KlonsP.Classes
{
    public class ItemRepInfo : ItemInfo
    {
        public string SCat1 { get; set; } = null;
        public string SCatD { get; set; } = null;
        public string SCatT { get; set; } = null;
        public string SDepartment { get; set; } = null;
        public string SPlace { get; set; } = null;

        public string SDateLiq
        {
            get
            {
                if (DateLiq == DateTime.MaxValue) return "";
                return Utils.DateToString(DateLiq);
            }
        }

        public void SetSFields()
        {
            var drcat1 = MyData.DbContextP.P_CAT1.Find(Cat1);
            SCat1 = drcat1.CODE + ": " + drcat1.DESCR.Nz();

            var drcatd = MyData.DbContextP.P_CATD.Find(CatD);
            SCatD = drcatd.CODE + ": " + drcatd.DESCR.Nz();

            var drcatt = MyData.DbContextP.P_CATT.Find(CatT);
            SCatT = drcatt.CODE + ": " + drcatt.DESCR.Nz();

            var drdep = MyData.DbContextP.P_DEPARTMENTS.Find(Department);
            SDepartment = drdep.CODE + ": " + drdep.DESCR.Nz();

            var drplace = MyData.DbContextP.P_PLACES.Find(Place);
            SPlace = drplace.CODE + ": " + drplace.DESCR.Nz();
        }

        public void FormatDescr()
        {
            foreach(var ev in Events2)
            {
                Descr = Descr.Nz();
            }
        }
    }
}
