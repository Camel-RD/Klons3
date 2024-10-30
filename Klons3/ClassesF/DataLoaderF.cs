using Klons3.ModelsF;
using Klons3.ModelsFRep;
using KlonsLIB;
using KlonsLIB.Data;
using KlonsLIB.Forms;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsF = Klons3.ModelsF;
using ModelsFRep = Klons3.ModelsFRep;

namespace KlonsF.Classes
{
    public static class DataLoaderF
    {
        public static KlonsData MyData => KlonsData.St;


        public static void ClearAll()
        {
            MyData.RecreateDbContextF();
        }

        public static bool FillAll()
        {
            var ctx = MyData.DbContextF;

            IQueryable[] tables = [
                ctx.F_ACP21,
                ctx.F_ACP23,
                ctx.F_ACP24,
                ctx.F_ACP25,
                ctx.F_ACP25A,
                ctx.F_ACPVN,
                ctx.F_BANKS,
                ctx.F_PERSONTYP,
                ctx.F_PERSONS,
                ctx.F_CURRENCY,
                ctx.F_BAL0,
                ctx.F_BALA1,
                ctx.F_BALA2,
                ctx.F_BALA21,
                ctx.F_BALA3,
                ctx.F_DOCTYP,
                ctx.F_DOCTYPA,
                ctx.F_DOCTYPB,
                ctx.F_DOCS0,
            ];
            foreach (var table in tables)
                ctx.LoadTable(table);

            MyData.FillParamsForUser(MyData.CurrentUserName);

            return true;
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

        public static List<F_VW_OPS> VW_OPS_FillByAndFilter(
                    DateTime? PDATE1, DateTime? PDATE2, string PTEXT, string PCLID,
                    string PAC11, string PAC12, string PAC13, string PAC14, string PAC15,
                    string PAC21, string PAC22, string PAC23, string PAC24, string PAC25,
                    string PDOCGR)
        {
            //FbDbParameter f;
            var ret = MyData.DbContextF.F_VW_OPS
                .FromSql($"SELECT * FROM SP_F_VWOPS_FILTER_AND({PDATE1}, {PDATE2}, {PTEXT}, {PCLID}, {PAC11}, {PAC12}, {PAC13}, {PAC14}, {PAC15}, {PAC21}, {PAC22}, {PAC23}, {PAC24}, {PAC25}, {PDOCGR})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_VW_OPS> VW_OPS_FillByOrFilter(
                    DateTime? PDATE1, DateTime? PDATE2, string PTEXT, string PCLID,
                    string PAC11, string PAC12, string PAC13, string PAC14, string PAC15,
                    string PAC21, string PAC22, string PAC23, string PAC24,string PAC25,
                    string PDOCGR)
        {
            var ret = MyData.DbContextF.F_VW_OPS
                .FromSql($"SELECT * FROM SP_F_VWOPS_FILTER_OR({PDATE1}, {PDATE2}, {PTEXT}, {PCLID}, {PAC11}, {PAC12}, {PAC13}, {PAC14}, {PAC15}, {PAC21}, {PAC22}, {PAC23}, {PAC24}, {PAC25}, {PDOCGR})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static void ClearDocsAndOps()
        {
            int ct = MyData.DbContextF.F_OPSD.Local.Count + MyData.DbContextF.F_OPS.Local.Count;
            if (ct < 1000)
            {
                ClearDocsAndOps__2();
                return;
            }
            MyData.DbContextF.ClearLocalDbSet<F_OPS>();
            MyData.DbContextF.ClearLocalDbSet<F_OPSD>();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static void ClearDocsAndOps__2()
        {
            var docs = MyData.DbContextF.F_OPSD.Local.ToList();
            var ops = MyData.DbContextF.F_OPS.Local.ToList();
            foreach (var op in ops)
            {
                MyData.DbContextF.Entry(op).State = EntityState.Detached;
            }
            ops.Clear();
            foreach (var doc in docs)
            {
                MyData.DbContextF.Entry(doc).State = EntityState.Detached;
            }
            docs.Clear();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static void ClearRepLinkedDocsAndOps()
        {
            var docs = MyData.DbContextFRep.F_TREPOPSD.Local.ToList();
            var ops = MyData.DbContextFRep.F_TREPOPS.Local.ToList();
            foreach (var op in ops)
            {
                MyData.DbContextFRep.Entry(op).State = EntityState.Detached;
            }
            foreach (var doc in docs)
            {
                MyData.DbContextFRep.Entry(doc).State = EntityState.Detached;
            }
        }

        public static void ClearBalanceFormulas()
        {
            var bala1 = MyData.DbContextF.F_BALA1.Local.ToList();
            var bala2 = MyData.DbContextF.F_BALA2.Local.ToList();
            var bala3 = MyData.DbContextF.F_BALA3.Local.ToList();
            foreach (var row in bala3)
            {
                MyData.DbContextF.Entry(row).State = EntityState.Detached;
            }
            foreach (var row in bala2)
            {
                MyData.DbContextF.Entry(row).State = EntityState.Detached;
            }
            foreach (var row in bala1)
            {
                MyData.DbContextF.Entry(row).State = EntityState.Detached;
            }
        }

        public static List<F_OPS> OPS_FillByAndFilter(
                    DateTime? PDATE1, DateTime? PDATE2, string PTEXT, string PCLID,
                    string PAC11, string PAC12, string PAC13, string PAC14, string PAC15,
                    string PAC21, string PAC22, string PAC23, string PAC24, string PAC25,
                    string PDOCGR)
        {
            var ret = MyData.DbContextF.F_OPS
                .FromSql($"SELECT * FROM SP_F_OPS_FILTER_AND({PDATE1}, {PDATE2}, {PTEXT}, {PCLID}, {PAC11}, {PAC12}, {PAC13}, {PAC14}, {PAC15}, {PAC21}, {PAC22}, {PAC23}, {PAC24}, {PAC25}, {PDOCGR})")
                .ToList();
            return ret;
        }

        public static List<F_OPS> OPS_FillByOrFilter(
                    DateTime? PDATE1, DateTime? PDATE2, string PTEXT, string PCLID,
                    string PAC11, string PAC12, string PAC13, string PAC14, string PAC15,
                    string PAC21, string PAC22, string PAC23, string PAC24, string PAC25,
                    string PDOCGR)
        {
            var ret = MyData.DbContextF.F_OPS
                .FromSql($"SELECT * FROM SP_F_OPS_FILTER_OR({PDATE1}, {PDATE2}, {PTEXT}, {PCLID}, {PAC11}, {PAC12}, {PAC13}, {PAC14}, {PAC15}, {PAC21}, {PAC22}, {PAC23}, {PAC24}, {PAC25}, {PDOCGR})")
                .ToList();
            return ret;
        }

        public static List<F_OPSD> OPSD_FillByAndFilter(
                    DateTime? PDATE1, DateTime? PDATE2, string PTEXT, string PCLID,
                    string PAC11, string PAC12, string PAC13, string PAC14, string PAC15,
                    string PAC21, string PAC22, string PAC23, string PAC24, string PAC25,
                    string PDOCGR)
        {
            var ret = MyData.DbContextF.F_OPSD
                .FromSql($"SELECT * FROM SP_F_OPSD_FILTER_AND({PDATE1}, {PDATE2}, {PTEXT}, {PCLID}, {PAC11}, {PAC12}, {PAC13}, {PAC14}, {PAC15}, {PAC21}, {PAC22}, {PAC23}, {PAC24}, {PAC25}, {PDOCGR})")
                .ToList();
            return ret;
        }

        public static List<F_OPSD> OPSD_FillByOrFilter(
                    DateTime? PDATE1, DateTime? PDATE2, string PTEXT, string PCLID,
                    string PAC11, string PAC12, string PAC13, string PAC14, string PAC15,
                    string PAC21, string PAC22, string PAC23, string PAC24, string PAC25,
                    string PDOCGR)
        {
            var ret = MyData.DbContextF.F_OPSD
                .FromSql($"SELECT * FROM SP_F_OPSD_FILTER_OR({PDATE1}, {PDATE2}, {PTEXT}, {PCLID}, {PAC11}, {PAC12}, {PAC13}, {PAC14}, {PAC15}, {PAC21}, {PAC22}, {PAC23}, {PAC24}, {PAC25}, {PDOCGR})")
                .ToList();
            return ret;
        }

        public static List<F_ROPS1A> ROPS1A_FillBy_doks_11(
                    DateTime? PDATE1, DateTime? PDATE2, string PTEXT, string PCLID,
                    string PAC11, string PAC12, string PAC13, string PAC14, string PAC15,
                    string PAC21, string PAC22, string PAC23, string PAC24, string PAC25,
                    string PDOCGR, string PANDOR)
        {
            var ret = MyData.DbContextFRep.F_ROPS1A
                .FromSql($"SELECT * FROM SP_F_REP_DOKS_11({PDATE1}, {PDATE2}, {PTEXT}, {PCLID}, {PAC11}, {PAC12}, {PAC13}, {PAC14}, {PAC15}, {PAC21}, {PAC22}, {PAC23}, {PAC24}, {PAC25}, {PDOCGR}, {PANDOR})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_TREPOPSD> TREPOPSD_FillBy_linkeddocs_1(
                    DateTime? PSD, DateTime? PED, string PCLID, string PDOCNR)
        {
            var ret = MyData.DbContextFRep.F_TREPOPSD
                .FromSql($"SELECT * FROM SP_F_LINKEDDOCS_1({PSD}, {PED}, {PCLID}, {PDOCNR})")
                .ToList();
            return ret;
        }

        public static List<F_TREPOPS> TREPOPSD_FillBy_linkeddocs_2(
                    DateTime? PSD, DateTime? PED, string PCLID, string PDOCNR)
        {
            var ret = MyData.DbContextFRep.F_TREPOPS
                .FromSql($"SELECT * FROM SP_F_LINKEDDOCS_2({PSD}, {PED}, {PCLID}, {PDOCNR})")
                .ToList();
            return ret;
        }

        public static List<F_ROPS1A> ROPS1A_GetBy_kieo_1(int PDID)
        {
            var ret = MyData.DbContextFRep.F_ROPS1A
                .FromSql($"SELECT * FROM SP_F_REP_KIEO_1({PDID})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS1A> ROPS1A_GetBy_koresp_11_clid2(DateTime? PSD, DateTime? PED, 
            string PAC, string PCLID)
        {
            var ret = MyData.DbContextFRep.F_ROPS1A
                .FromSql($"SELECT * FROM SP_F_REP_KORESP_11_CLID2({PSD}, {PED}, {PAC}, {PCLID})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS2A> ROPS2A_GetBy_apgr_01_clid(DateTime? PSD, DateTime? PED,
            string PAC, string PCLID)
        {
            var ret = MyData.DbContextFRep.F_ROPS2A
                .FromSql($"SELECT * FROM SP_F_REP_APGR_01_CLID({PSD}, {PED}, {PAC}, {PCLID})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS2A> ROPS2A_GetBy_apgr_01_clid2(DateTime? PSD, DateTime? PED, 
            string PAC, string PCLID)
        {
            var ret = MyData.DbContextFRep.F_ROPS2A
                .FromSql($"SELECT * FROM SP_F_REP_APGR_01_CLID2({PSD}, {PED}, {PAC}, {PCLID})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS3A> ROPS3A_GetBy_koresp_01_clid2(DateTime? PSD, DateTime? PED,
            string PAC, string PCLID)
        {
            var ret = MyData.DbContextFRep.F_ROPS3A
                .FromSql($"SELECT * FROM SP_F_REP_KORESP_01_CLID2({PSD}, {PED}, {PAC}, {PCLID})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS2A> ROPS2A_GetBy_apgr_01(DateTime? PSD, DateTime? PED, string PAC)
        {
            var ret = MyData.DbContextFRep.F_ROPS2A
                .FromSql($"SELECT * FROM SP_F_REP_APGR_01({PSD}, {PED}, {PAC})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS2A> ROPS2A_GetBy_apgr_02(DateTime? PSD, DateTime? PED, string PAC)
        {
            var ret = MyData.DbContextFRep.F_ROPS2A
                .FromSql($"SELECT * FROM SP_F_REP_APGR_02({PSD}, {PED}, {PAC})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS2A> ROPS2A_GetBy_apgr_02_clid(DateTime? PSD, DateTime? PED, 
            string PAC, string PCLID)
        {
            var ret = MyData.DbContextFRep.F_ROPS2A
                .FromSql($"SELECT * FROM SP_F_REP_APGR_02_CLID({PSD}, {PED}, {PAC}, {PCLID})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS2A> ROPS2A_GetBy_apgr_11(DateTime? SDT, DateTime? EDT, string PAC)
        {
            var ret = MyData.DbContextFRep.F_ROPS2A
                .FromSql($"SELECT * FROM SP_F_REP_APGR_11({SDT}, {EDT}, {PAC})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS2A1> ROPS2A1_GetBy_apgr_11(DateTime? SDT, DateTime? EDT, string PAC)
        {
            var ret = MyData.DbContextFRep.Set<F_ROPS2A1>()
                .FromSql($"SELECT * FROM SP_F_REP_APGR_11({SDT}, {EDT}, {PAC})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS2A> ROPS2A_GetBy_apgr_21(DateTime? SDT, DateTime? EDT, string PAC, int PACLEN)
        {
            var ret = MyData.DbContextFRep.F_ROPS2A
                .FromSql($"SELECT * FROM SP_F_REP_APGR_21({SDT}, {EDT}, {PAC}, {PACLEN})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS3A> ROPS3A_GetBy_apgr_dz_11(DateTime? SDT, DateTime? EDT)
        {
            var ret = MyData.DbContextFRep.F_ROPS3A
                .FromSql($"SELECT * FROM SP_F_REP_APGR_DZ_11({SDT}, {EDT})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS2A> ROPS2A_GetBy_apgr_np_11(DateTime? SDT, DateTime? EDT, string PAC)
        {
            var ret = MyData.DbContextFRep.F_ROPS2A
                .FromSql($"SELECT * FROM SP_F_REP_APGR_NP_11({SDT}, {EDT}, {PAC})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS2A> ROPS2A_GetBy_apgr_np_12(DateTime? SDT, DateTime? EDT, string PAC, int PACLEN)
        {
            var ret = MyData.DbContextFRep.F_ROPS2A
                .FromSql($"SELECT * FROM SP_F_REP_APGR_NP_12({SDT}, {EDT}, {PAC}, {PACLEN})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS2A> ROPS2A_GetBy_apgr_dz_12(DateTime? SDT, DateTime? EDT)
        {
            var ret = MyData.DbContextFRep.F_ROPS2A
                .FromSql($"SELECT * FROM SP_F_REP_APGR_DZ_12({SDT}, {EDT})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS2A> ROPS2A_GetBy_currdiff_01(DateTime? SDT, DateTime? EDT, 
            string PAC, string PCLID, string PCUR)
        {
            var ret = MyData.DbContextFRep.F_ROPS2A
                .FromSql($"SELECT * FROM SP_F_REP_CURRDIFF_01({SDT}, {EDT}, {PAC}, {PCLID}, {PCUR})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS2A> ROPS2A_GetBy_apgr_cur_01_clid(DateTime? SDT, DateTime? EDT,
            string PAC, string PCLID, string PCUR)
        {
            var ret = MyData.DbContextFRep.F_ROPS2A
                .FromSql($"SELECT * FROM SP_F_REP_APGR_CUR_01_CLID({SDT}, {EDT}, {PAC}, {PCLID}, {PCUR})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS1A> ROPS1A_GetBy_koresp_11_cur(DateTime? SDT, DateTime? EDT,
            string PAC, string PCLID, string PCUR)
        {
            var ret = MyData.DbContextFRep.F_ROPS1A
                .FromSql($"SELECT * FROM SP_F_REP_KORESP_11_CUR({SDT}, {EDT}, {PAC}, {PCLID}, {PCUR})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS3A> ROPS3A_GetBy_koresp_01_cur(DateTime? SDT, DateTime? EDT,
            string PAC, string PCLID, string PCUR)
        {
            var ret = MyData.DbContextFRep.F_ROPS3A
                .FromSql($"SELECT * FROM SP_F_REP_KORESP_01_CUR({SDT}, {EDT}, {PAC}, {PCLID}, {PCUR})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS1A> ROPS1A_GetBy_curcheck_01(DateTime? SDT, DateTime? EDT,
            string PAC, string PCLID, string PCUR)
        {
            var ret = MyData.DbContextFRep.F_ROPS1A
                .FromSql($"SELECT * FROM SP_F_REP_CURCHECK_01({SDT}, {EDT}, {PAC}, {PCLID}, {PCUR})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_TREPDARZ1> TREPDARZ1_GetBy_darz_1(DateTime? SDT, DateTime? EDT,
            string PAC, string PAC2)
        {
            var ret = MyData.DbContextFRep.F_TREPDARZ1
                .FromSql($"SELECT * FROM SP_F_REP_DARZ_1({SDT}, {EDT}, {PAC}, {PAC2})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_TREPDARZ2> TREPDARZ2_GetBy_darz_2(DateTime? SDT, DateTime? EDT,
            string PAC, string PAC2)
        {
            var ret = MyData.DbContextFRep.F_TREPDARZ2
                .FromSql($"SELECT * FROM SP_F_REP_DARZ_2({SDT}, {EDT}, {PAC}, {PAC2})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_TREPDARZ1> TREPDARZ1_GetBy_darz_3(DateTime? SDT, DateTime? EDT)
        {
            var ret = MyData.DbContextFRep.F_TREPDARZ1
                .FromSql($"SELECT * FROM SP_F_REP_DARZ_3({SDT}, {EDT})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_TREPDARZ2> TREPDARZ2_GetBy_darz_4(DateTime? SDT, DateTime? EDT)
        {
            var ret = MyData.DbContextFRep.F_TREPDARZ2
                .FromSql($"SELECT * FROM SP_F_REP_DARZ_4({SDT}, {EDT})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_TREPAPAN> TREPAPAN_GetBy_apgr_full_11(DateTime? SDT, DateTime? EDT,
            string PAC1, string PAC2, string PAC3, string PAC4, string PAC5, string PCLID,
            int PXAC1, int PXAC2, int PXAC3, int PXAC4, int PXAC5, int PXCLID)

        {
            var ret = MyData.DbContextFRep.F_TREPAPAN
                .FromSql($"SELECT * FROM SP_F_REP_APGR_FULL_11({SDT}, {EDT}, {PAC1}, {PAC2}, {PAC3}, {PAC4}, {PAC5}, {PCLID}, {PXAC1}, {PXAC2}, {PXAC3}, {PXAC4}, {PXAC5}, {PXCLID})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_TREPMT> TREPMT_GetBy_apgr_mt_11(DateTime? SDT, DateTime? EDT, string PAC)
        {
            var ret = MyData.DbContextFRep.F_TREPMT
                .FromSql($"SELECT * FROM SP_F_REP_APGR_MT_11({SDT}, {EDT}, {PAC})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_TREPMT> TREPMT_GetBy_apgr_mt_12(DateTime? SDT, DateTime? EDT, string PAC)
        {
            var ret = MyData.DbContextFRep.F_TREPMT
                .FromSql($"SELECT * FROM SP_F_REP_APGR_MT_12({SDT}, {EDT}, {PAC})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_TREPMT> TREPMT_GetBy_apgr_mt_21(DateTime? SDT, DateTime? EDT, string PAC)
        {
            var ret = MyData.DbContextFRep.F_TREPMT
                .FromSql($"SELECT * FROM SP_F_REP_APGR_MT_21({SDT}, {EDT}, {PAC})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_TREPMT> TREPMT_GetBy_apgr_mt_22(DateTime? SDT, DateTime? EDT, string PAC)
        {
            var ret = MyData.DbContextFRep.F_TREPMT
                .FromSql($"SELECT * FROM SP_F_REP_APGR_MT_22({SDT}, {EDT}, {PAC})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static void BalA_Fill()
        {
            MyData.DbContextF.F_BALA1.Load();
            MyData.DbContextF.F_BALA2.Load();
            MyData.DbContextF.F_BALA3.Load();
        }

        public static List<F_ACP21> ACP21_GetBy_bal_13(string PBALID)
        {
            var ret = MyData.DbContextF.F_ACP21
                .FromSql($"SELECT * FROM SP_F_REP_BAL_13({PBALID})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_BALA2> BALA2_Get()
        {
            var ret = MyData.DbContextF.F_BALA2.AsNoTracking().ToList();
            return ret;
        }

        public static List<F_BALA3> BALA3_Get()
        {
            var ret = MyData.DbContextF.F_BALA3.AsNoTracking().ToList();
            return ret;
        }

        public static List<F_BALA21> BALA21_GetBy_bal_12(DateTime? SDT, DateTime? EDT, string PBALID)
        {
            var ret = MyData.DbContextF.F_BALA21
                .FromSql($"SELECT * FROM SP_F_REP_BAL_12({SDT}, {EDT}, {PBALID})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_BALA22> BALA22_GetBy_bal_22(DateTime? SDT, DateTime? EDT, string PBALID)
        {
            var ret = MyData.DbContextF.Set<F_BALA22>()
                .FromSql($"SELECT * FROM SP_F_REP_BAL_22({SDT}, {EDT}, {PBALID})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS1A> ROPS1A_GetBy_kg_11(DateTime? SDT, DateTime? EDT, string PAC)
        {
            var ret = MyData.DbContextFRep.F_ROPS1A
                .FromSql($"SELECT * FROM SP_F_REP_KG_11({SDT}, {EDT}, {PAC})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS1A> ROPS1A_GetBy_kieo_21(DateTime? SDT, DateTime? EDT, string PAC)
        {
            var ret = MyData.DbContextFRep.F_ROPS1A
                .FromSql($"SELECT * FROM SP_F_REP_KIEO_21({SDT}, {EDT}, {PAC})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS1A> ROPS1A_GetBy_kizo_21(DateTime? SDT, DateTime? EDT, string PAC)
        {
            var ret = MyData.DbContextFRep.F_ROPS1A
                .FromSql($"SELECT * FROM SP_F_REP_KIZO_21({SDT}, {EDT}, {PAC})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS1A> ROPS1A_GetBy_koresp_11(DateTime? PSD, DateTime? PED, string PAC)
        {
            var ret = MyData.DbContextFRep.F_ROPS1A
                .FromSql($"SELECT * FROM SP_F_REP_KORESP_11({PSD}, {PED}, {PAC})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS1A> ROPS1A_GetBy_koresp_11_clid(DateTime? PSD, DateTime? PED, 
            string PAC, string PCLID)
        {
            var ret = MyData.DbContextFRep.F_ROPS1A
                .FromSql($"SELECT * FROM SP_F_REP_KORESP_11_CLID({PSD}, {PED}, {PAC}, {PCLID})")
                .AsNoTracking()
                .ToList();
            return ret;
        }


        public static List<F_ROPS3A> ROPS3A_GetBy_koresp_01(DateTime? PSD, DateTime? PED, string PAC)
        {
            var ret = MyData.DbContextFRep.F_ROPS3A
                .FromSql($"SELECT * FROM SP_F_REP_KORESP_01({PSD}, {PED}, {PAC})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS3A> ROPS3A_GetBy_koresp_01_clid(DateTime? PSD, DateTime? PED, 
            string PAC, string PCLID)
        {
            var ret = MyData.DbContextFRep.F_ROPS3A
                .FromSql($"SELECT * FROM SP_F_REP_KORESP_01_CLID({PSD}, {PED}, {PAC}, {PCLID})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS3A> ROPS3A_GetBy_koresp_02(DateTime? PSD, DateTime? PED, string PAC)
        {
            var ret = MyData.DbContextFRep.F_ROPS3A
                .FromSql($"SELECT * FROM SP_F_REP_KORESP_02({PSD}, {PED}, {PAC})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS3A> ROPS3A_GetBy_koresp_02_clid(DateTime? PSD, DateTime? PED,
            string PAC, string PCLID)
        {
            var ret = MyData.DbContextFRep.F_ROPS3A
                .FromSql($"SELECT * FROM SP_F_REP_KORESP_02_CLID({PSD}, {PED}, {PAC}, {PCLID})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_TREPMT> TREPMT_GetBy_npmt_14(DateTime? SDT, DateTime? EDT)
        {
            var ret = MyData.DbContextFRep.F_TREPMT
                .FromSql($"SELECT * FROM SP_F_REP_NPMT_14({SDT}, {EDT})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_TREPMT> TREPMT_GetBy_npmt_24(DateTime? SDT, DateTime? EDT)
        {
            var ret = MyData.DbContextFRep.F_TREPMT
                .FromSql($"SELECT * FROM SP_F_REP_NPMT_24({SDT}, {EDT})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_TREPMT> TREPMT_GetBy_npmt_31(DateTime? SDT, DateTime? EDT)
        {
            var ret = MyData.DbContextFRep.F_TREPMT
                .FromSql($"SELECT * FROM SP_F_REP_NPMT_31({SDT}, {EDT})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_TREPMT> TREPMT_GetBy_npmt_44(DateTime? SDT, DateTime? EDT)
        {
            var ret = MyData.DbContextFRep.F_TREPMT
                .FromSql($"SELECT * FROM SP_F_REP_NPMT_44({SDT}, {EDT})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS2A> ROPS2A_GetBy_pers_12(DateTime? PSD, DateTime? PED,
            string PAC, string PCLID)
        {
            var ret = MyData.DbContextFRep.F_ROPS2A
                .FromSql($"SELECT * FROM SP_F_REP_PERS_12({PSD}, {PED}, {PAC}, {PCLID})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS2A> ROPS2A_GetBy_pers_11(DateTime? PSD, DateTime? PED,
            string PAC, string PCLID)
        {
            var ret = MyData.DbContextFRep.F_ROPS2A
                .FromSql($"SELECT * FROM SP_F_REP_PERS_11({PSD}, {PED}, {PAC}, {PCLID})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_TREPA1> TREPA1_GetBy_pers_13(DateTime? PSD, DateTime? PED,
            string PAC, string PCLID)
        {
            var ret = MyData.DbContextFRep.F_TREPA1
                .FromSql($"SELECT * FROM SP_F_REP_PERS_13({PSD}, {PED}, {PAC}, {PCLID})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_TREPA1> TREPA1_GetBy_pers_14(DateTime? PSD, DateTime? PED,
            string PAC, string PCLID)
        {
            var ret = MyData.DbContextFRep.F_TREPA1
                .FromSql($"SELECT * FROM SP_F_REP_PERS_14({PSD}, {PED}, {PAC}, {PCLID})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_TREPA1> TREPA1_GetBy_pers_15(DateTime? PSD, DateTime? PED,
            string PAC, string PCLID)
        {
            var ret = MyData.DbContextFRep.F_TREPA1
                .FromSql($"SELECT * FROM SP_F_REP_PERS_15({PSD}, {PED}, {PAC}, {PCLID})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_TREPA1> TREPA1_GetBy_pers_16(DateTime? PSD, DateTime? PED,
            string PAC, string PCLID)
        {
            var ret = MyData.DbContextFRep.F_TREPA1
                .FromSql($"SELECT * FROM SP_F_REP_PERS_16({PSD}, {PED}, {PAC}, {PCLID})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS1A> ROPS1A_GetBy_pvn_check_01(DateTime? PSD, DateTime? PED,
            decimal RG1, decimal LIKME)
        {
            var ret = MyData.DbContextFRep.F_ROPS1A
                .FromSql($"SELECT * FROM SP_F_REP_PVN_CHECK_01({PSD}, {PED}, {RG1}, {LIKME})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS1A> ROPS1A_GetBy_pvn_check_02(DateTime? PSD, DateTime? PED,
            decimal RG1, decimal LIKME)
        {
            var ret = MyData.DbContextFRep.F_ROPS1A
                .FromSql($"SELECT * FROM SP_F_REP_PVN_CHECK_02({PSD}, {PED}, {RG1}, {LIKME})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS2A> ROPS2A_GetBy_pvn_10(DateTime? PSD, DateTime? PED)
        {
            var ret = MyData.DbContextFRep.F_ROPS2A
                .FromSql($"SELECT * FROM SP_F_REP_PVN_10({PSD}, {PED})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS1A> ROPS1A_GetBy_pvn_22(DateTime? PSD, DateTime? PED,
            decimal RG1, decimal RG2)
        {
            var ret = MyData.DbContextFRep.F_ROPS1A
                .FromSql($"SELECT * FROM SP_F_REP_PVN_22({PSD}, {PED}, {RG1}, {RG2})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS1A> ROPS1A_GetBy_pvn_32(DateTime? PSD, DateTime? PED,
            decimal RG1, decimal RG2)
        {
            var ret = MyData.DbContextFRep.F_ROPS1A
                .FromSql($"SELECT * FROM SP_F_REP_PVN_32({PSD}, {PED}, {RG1}, {RG2})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS1A> ROPS1A_GetBy_pvn_41(DateTime? PSD, DateTime? PED,
            decimal RG1, decimal RG2)
        {
            var ret = MyData.DbContextFRep.F_ROPS1A
                .FromSql($"SELECT * FROM SP_F_REP_PVN_41({PSD}, {PED}, {RG1}, {RG2})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS1A> ROPS1A_GetBy_pvn_42(DateTime? PSD, DateTime? PED,
            decimal RG1, decimal RG2)
        {
            var ret = MyData.DbContextFRep.F_ROPS1A
                .FromSql($"SELECT * FROM SP_F_REP_PVN_42({PSD}, {PED}, {RG1}, {RG2})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS1A> ROPS1A_GetBy_pvn_52(DateTime? PSD, DateTime? PED,
            decimal RG1, decimal RG2)
        {
            var ret = MyData.DbContextFRep.F_ROPS1A
                .FromSql($"SELECT * FROM SP_F_REP_PVN_52({PSD}, {PED}, {RG1}, {RG2})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_TREPPVNZ1> TREPPVNZ1_GetBy_pvnz_01(DateTime? SDT, DateTime? EDT)
        {
            var ret = MyData.DbContextFRep.F_TREPPVNZ1
                .FromSql($"SELECT * FROM SP_F_REP_PVNZ_01({SDT}, {EDT})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS1A> ROPS1A_GetBy_skaidra_01(DateTime? SDT, DateTime? EDT,
            decimal PSUMM, string PPERTYP)
        {
            var ret = MyData.DbContextFRep.F_ROPS1A
                .FromSql($"SELECT * FROM SP_F_REP_SKAIDRA_01({SDT}, {EDT}, {PSUMM}, {PPERTYP})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_ROPS1A> ROPS1A_GetBy_skaidra_02(DateTime? SDT, DateTime? EDT,
            decimal PSUMM, string PPERTYP)
        {
            var ret = MyData.DbContextFRep.F_ROPS1A
                .FromSql($"SELECT * FROM SP_F_REP_SKAIDRA_02({SDT}, {EDT}, {PSUMM}, {PPERTYP})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_SP_DIFF_12> F_SP_DIFF_12_GetBy_SP_F_DIFF_12(DateTime? PDATE1, DateTime? PDATE2,
            DateTime? PDATE3, string PCLID, string PAC1, string PAC2, string PAC3, string PAC4, string PAC5)
        {
            var ret = MyData.DbContextFRep.Set<F_SP_DIFF_12>()
                .FromSql($"SELECT * FROM SP_F_DIFF_12({PDATE1}, {PDATE2}, {PDATE3}, {PCLID}, {PAC1}, {PAC2}, {PAC3}, {PAC4}, {PAC5})")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_LOPSD> F_LOPSD_GetBy_SP_F_DIFF_01(int PDOCID)
        {
            var ret = MyData.DbContextF.F_LOPSD
                .FromSql($"SELECT T1.*, 0 AS NL, 0 AS OPT FROM SP_F_DIFF_01({PDOCID}) AS T1")
                .AsNoTracking()
                .ToList();
            return ret;
        }

        public static List<F_LOPS> F_LOPS_GetBy_SP_F_DIFF_02(int PDOCID)
        {
            var ret = MyData.DbContextF.F_LOPS
                .FromSql($"SELECT T1.*, 0 AS NL FROM SP_F_DIFF_02({PDOCID}) AS T1")
                .AsNoTracking()
                .ToList();
            return ret;
        }

    }

}