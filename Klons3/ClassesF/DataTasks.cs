using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Klons3.ModelsF;
using Klons3.ModelsFRep;

namespace KlonsF.Classes
{
    public static class DataTasks
    {
        public static KlonsData MyData { get { return KlonsData.St; } }
        public static string GetAcName(string ac)
        {
            var dr = MyData.DbContextF.F_ACP21.Find(ac);
            if (dr == null) return null;
            return dr.NAME;
        }
        public static string GetAc3Name(string ac)
        {
            var dr = MyData.DbContextF.F_ACP23.Find(ac);
            if (dr == null) return null;
            return dr.NAME;
        }
        public static string GetAc4Name(string ac)
        {
            var dr = MyData.DbContextF.F_ACP24.Find(ac);
            if (dr == null) return null;
            return dr.NAME;
        }
        public static string GetAc5Name(string ac)
        {
            var dr = MyData.DbContextF.F_ACP25.Find(ac);
            if (dr == null) return null;
            return dr.NAME;
        }
        public static string GetBankName(string bankid)
        {
            var dr = MyData.DbContextF.F_BANKS.Find(bankid);
            if (dr == null) return null;
            return dr.NAME;
        }
        public static string GetClName(string cl)
        {
            var dr = MyData.DbContextF.F_PERSONS.Find(cl);
            if (dr == null) return null;
            return dr.NAME;
        }
        public static string GetDocTypName(string dt)
        {
            var dr = MyData.DbContextF.F_DOCTYP.Find(dt);
            if (dr == null) return null;
            return dr.NAME;
        }

        public static F_ACP24 GetAcP24Row(string ac4)
        {
            if (string.IsNullOrEmpty(ac4)) return null;
            return MyData.DbContextF.F_ACP24.Find(ac4);
        }
        public static string RemoveUnitFromDescr(string descr)
        {
            if (string.IsNullOrEmpty(descr))
                return descr;
            int k = descr.IndexOf('~');
            if (k == -1) return descr;
            if (k == descr.Length - 1) return null;
            return descr.Substring(k + 1);
        }
        public static string SetUnitInDescr(string descr, string unit)
        {
            descr = RemoveUnitFromDescr(descr);
            if (string.IsNullOrEmpty(unit)) return descr;
            if (string.IsNullOrEmpty(descr))
            {
                return unit + "~";
            }
            return unit + "~" + descr;
        }

        public static F_TREPOPSD MakeTRepOPSd(int docid)
        {
            var row = MyData.DbContextF.F_OPSD.Find(docid);
            if (row == null) return null;
            var row2 = new F_TREPOPSD();
            row2.ID = row.ID;
            row2.ZNR = row.ZNR;
            row2.NRX = row.NRX;
            row2.DETE = row.DETE;
            row2.DOCTYP = row.DOCTYP;
            row2.DOCST = row.DOCST;
            row2.DOCNR = row.DOCNR;
            row2.CLID = row.CLID;
            row2.CLID2 = row.CLID2;
            row2.DESCR = row.DESCR;
            row2.SUMM = row.SUMM;
            row2.PVN = row.PVN;
            row2.DT2 = row.DT2;
            return row2;
        }

        public static List<F_TREPOPS> MakeTRepOPS(int docid)
        {
            var row = MyData.DbContextF.F_OPSD.Find(docid);
            if (row == null) return null;
            var dr_ops = row.OPSRows
                .Select(x => new F_TREPOPS(){
                    ID = x.ID,
                    DOCID = x.DOCID,
                    DESCR = x.DESCR,
                    AC11 = x.AC11,
                    AC12 = x.AC12,
                    AC13 = x.AC13,
                    AC14 = x.AC14,
                    AC15 = x.AC15,
                    AC21 = x.AC21,
                    AC22 = x.AC22,
                    AC23 = x.AC23,
                    AC24 = x.AC24,
                    AC25 = x.AC25,
                    SUMMC = x.SUMMC,
                    CUR = x.CUR,
                    SUMM = x.SUMM,
                    QV = x.QV,
                    NL = x.NL,
                    ZDT = x.ZDT,
                })
                .OrderBy(x => x.ID)
                .ToList();
            return dr_ops;
        }

        #region **************  PVN ****************

        public static F_ACPVN GetPVNrow(string ac5)
        {
            if (string.IsNullOrEmpty(ac5)) return null;
            return MyData.DbContextF.F_ACPVN.Find(ac5);
        }

        public static int AcPVNPz3(string id)
        {
            var dr = MyData.DbContextF.F_ACPVN.Find(id);
            if (dr == null) return -1;
            return dr.PZ3;
        }

        public static int GetPVNTyp(string ac5)
        {
            var dr = GetPVNrow(ac5);
            if (dr == null) return -1;
            return dr.PZ5;
        }

        public static int GetPVNRate(string ac5)
        {
            var dr = GetPVNrow(ac5);
            if (dr == null) return -1;
            return (int)dr.T;
        }

        public static bool IsPVN(string ac5)
        {
            int k = GetPVNTyp(ac5);
            return k > 0;
        }

        public static bool IsGoodPVN(string ac5)
        {
            int k = GetPVNTyp(ac5);
            return k == 1 || k == 3;
        }

        public static bool IsIenemumiA(int ac5paz3)
        {
            return ac5paz3 == 1 || ac5paz3 == 71 ||
                   ac5paz3 == 72 || ac5paz3 == 8;
        }

        public static bool IsIenemumi(string ac5)
        {
            int k = AcPVNPz3(ac5);
            return IsIenemumiA(k);
        }
        public static int GetPVNRateA(string ac5, DateTime date)
        {
            var dr = GetPVNrow(ac5);
            if (dr == null) return 0;
            int k = dr.PZ5;
            if (k < 1) return 0;
            if (k > 3 || k == 1) return (int)dr.T;
            if (k == 2 || k == 3)
            {
                if (date < new DateTime(2011, 1, 1))
                {
                    return 21;
                }
                else if (date < new DateTime(2012, 8, 1))
                {
                    return 22;
                }
                else
                {
                    return 21;
                }
            }
            return 0;
        }
        #endregion

    }
}
