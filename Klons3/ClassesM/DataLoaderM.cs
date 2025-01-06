using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KlonsLIB.Forms;
using KlonsF.Classes;
using KlonsLIB.Data;
using Klons3.ModelsM;
using KlonsLIB;
using Microsoft.EntityFrameworkCore;
using MergeTaskList = KlonsLIB.Data.MyDbContext.MergeTaskList;
using Org.BouncyCastle.Utilities.Collections;
using System.Windows.Forms;

namespace KlonsM.Classes;

public static class DataLoaderM
{
    public static KlonsData MyData => KlonsData.St;

    public static void ClearAll()
    {
        MyData.RecreateDbContextM();
    }


    public static bool FillAll()
    {
        return DoLoadCheckErrors(() =>
        {
            FillAllA();
        });
    }

    public static bool FillAllA()
    {
        var ctx = MyData.DbContextM;

        IQueryable[] tables = [
            ctx.M_ACCOUNTTYPE,
            ctx.M_ITEMS_CAT,
            ctx.M_DOCTYPES,
            ctx.M_ACCOUNTS,
            ctx.M_PVNTEXTS,
            ctx.M_PVNTYPE,
            ctx.M_PVNRATES,
            ctx.M_PVNRATES2,
            ctx.M_BANKS,
            ctx.M_COUNTRIES,
            ctx.M_STORETYPE,
            ctx.M_STORES_CAT,
            ctx.M_UNITS,
            ctx.M_PAYMENTTYPE,
            ctx.M_TRANSACTIONTYPE,
            ctx.M_DISC_LISTS,
            ctx.M_DISC_LISTS_P,
            ctx.M_DISC_LISTS_R,
            ctx.M_PRICE_LISTS,
            ctx.M_PRICE_LISTS_P,
            ctx.M_PRICE_LISTS_R,
            ctx.M_STORES,
            ctx.M_ITEMS,
            ctx.M_ITEMS_PER_STORE,
            ctx.M_ITEMS_TEXTS,
            ctx.M_BANKACCOUNTS,
            ctx.M_CONTACTS,
            ctx.M_ADDRESSSES,
            ctx.M_VEHICLES,
        ];

        foreach (var table in tables)
            ctx.LoadTable(table);

        return true;
    }

    public static bool RefillAfterLinking()
    {
        return DoLoadCheckErrors(() =>
        {
            RefillAfterLinkingA();
        });
    }

    public static bool RefillAfterLinkingA()
    {
        var ctx = MyData.DbContextM;
        var mergetasklist = new MergeTaskList(ctx);
        mergetasklist.AddTask<M_ITEMS>(x => x.ID);
        mergetasklist.AddTask<M_ITEMS_PER_STORE>(x => (x.IDITEM, x.IDSTORE));
        mergetasklist.AddTask<M_DOCS>(x => x.ID);
        mergetasklist.AddTask<M_ROWS>(x => x.ID);
        mergetasklist.Merge();
        return true;
    }

    private static bool DoLoadCheckErrors(Action act)
    {
        try
        {
            act();
            return true;
        }
        catch (Exception ex)
        {
            Form_Error.ShowException(ex, "Neizdevās ielādēt datus no datu bāzes.");
            return false;
        }
    }

    public static bool CheckLoad()
    {
        if (MyData.DbContextM.BL_M_STORETYPE.Count > 0) return true;
        if (MyData.Params.CHECKISGONE == 1)
        {
            DataTasksM.RecalcIsGone(false);
        }
        FillAll();
        return true;
    }
    
    public static bool LoadDocsByFilter(DateTime? dt1, DateTime? dt2, int? tp,
        int? state, int? idstoreout, int? idstorein, int? idstoreoutorin)
    {
        return DoLoadCheckErrors(() => 
        {
            LoadDocsByFilterA(dt1, dt2, tp, state, idstoreout, idstorein, idstoreoutorin);
        });
    }

    public static void LoadDocsByFilterA(DateTime? dt1, DateTime? dt2, int? tp,
        int? state, int? idstoreout, int? idstorein, int? idstoreoutorin)
    {
        var docs = MyData.DbContextM.BL_M_DOCS.ToList();
        var rows = MyData.DbContextM.BL_M_ROWS.ToList();
        MyData.DbContextM.RemoveFromContextRange(rows);
        MyData.DbContextM.RemoveFromContextRange(docs);
        docs.Clear();
        rows.Clear();
        GC.Collect();
        GC.WaitForPendingFinalizers();

        MyData.DbContextM.M_DOCS
            .FromSql($"SELECT * FROM SP_M_FILTER_DOCS_01({dt1}, {dt2}, {tp}, {state}, {idstoreout}, {idstorein}, {idstoreoutorin})")
            .Load();
    }

    public static bool LoadDocAndRowsByFilter(int iddoc, bool clearrowsbefore)
    {
        return DoLoadCheckErrors(() =>
        {
            LoadDocAndRowsByFilterA(iddoc, clearrowsbefore);
        });
    }

    public static void LoadDocAndRowsByFilterA(int iddoc, bool clearrowsbefore)
    {
        var dr_doc = MyData.DbContextM.M_DOCS.Find(iddoc);
        MyData.DbContextM.Entry(dr_doc).Reload();
        LoadRowsByFilterA(iddoc, clearrowsbefore);
    }

    public static bool LoadRowsByFilter(int iddoc, bool clearbefore)
    {
        return DoLoadCheckErrors(() =>
        {
            LoadRowsByFilterA(iddoc, clearbefore);
        });
    }

    public static void LoadRowsByFilterA(int iddoc, bool clearbefore)
    {
        var dr_doc = MyData.DbContextM.BL_M_DOCS.FindById(iddoc);
        var rows = MyData.DbContextM.BL_M_ROWS.ToList();
        if (clearbefore)
        {
            MyData.DbContextM.RemoveFromContextRange(rows);
            rows.Clear();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            MyData.DbContextM.M_ROWS
                .FromSql($"SELECT * FROM SP_M_FILTER_ROWS_01({iddoc})")
                .Load();
        }
        else
        {
            var rows_of_doc = dr_doc.Rows.ToList();
            var updated_row = MyData.DbContextM.M_ROWS
                .FromSql($"SELECT * FROM SP_M_FILTER_ROWS_01({iddoc})")
                .AsNoTracking()
                .ToList();
            MyData.DbContextM.UpdateEntryLists(updated_row, rows_of_doc, x => x.ID);
        }
    }

    public static bool LoadInvDocsByFilter(DateTime? dt1, DateTime? dt2, 
        int? state, int? idstore)
    {
        return DoLoadCheckErrors(() =>
        {
            LoadInvDocsByFilterA(dt1, dt2, state, idstore);
        });
    }

    public static void LoadInvDocsByFilterA(DateTime? dt1, DateTime? dt2,
        int? state, int? idstore)
    {
        var docs = MyData.DbContextM.BL_M_INV_DOCS.ToList();
        var rows = MyData.DbContextM.BL_M_INV_ROWS.ToList();
        MyData.DbContextM.RemoveFromContextRange(rows);
        MyData.DbContextM.RemoveFromContextRange(docs);
        docs.Clear();
        rows.Clear();
        GC.Collect();
        GC.WaitForPendingFinalizers();

        MyData.DbContextM.M_INV_DOCS
            .FromSql($"SELECT * FROM SP_M_FILTER_INVDOCS_1({dt1}, {dt2}, {state}, {idstore})")
            .Load();
    }

    public static bool LoadInvRowsByFilter(int iddoc, bool clearbefore)
    {
        return DoLoadCheckErrors(() =>
        {
            LoadInvRowsByFilterA(iddoc, clearbefore);
        });
    }

    public static void LoadInvRowsByFilterA(int iddoc, bool clearbefore)
    {
        var dr_doc = MyData.DbContextM.M_INV_DOCS.Find(iddoc);
        var rows = MyData.DbContextM.BL_M_INV_ROWS.ToList();
        if (clearbefore)
        {
            MyData.DbContextM.RemoveFromContextRange(rows);
            rows.Clear();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            MyData.DbContextM.M_ROWS
                .FromSql($"SELECT * FROM SP_M_FILTER_INVROWS_1({iddoc})")
                .Load();
        }
        else
        {
            var rows_of_doc = dr_doc.Rows.ToList();
            var updated_row = MyData.DbContextM.M_INV_ROWS
                .FromSql($"SELECT * FROM SP_M_FILTER_INVROWS_1({iddoc})")
                .AsNoTracking()
                .ToList();
            MyData.DbContextM.UpdateEntryLists(updated_row, rows_of_doc, x => x.ID);
        }
    }


    public static bool LoadLatestAmountsByDoc(int iddoc)
    {
        return DoLoadCheckErrors(() =>
        {
            LoadLatestAmountsByDocA(iddoc);
        });
    }

    public static void LoadLatestAmountsByDocA(int iddoc)
    {
        var dr_doc = MyData.DbContextM.M_DOCS.Find(iddoc);
        var drs_items = dr_doc.Rows.Select(x => x.Item).Distinct().ToList();
        var drs_items_per_store = drs_items.SelectMany(x => x.ItemsPerStore).Distinct().ToList();

        var updated_items = MyData.DbContextM.M_ITEMS
            .FromSql($"SELECT * FROM SP_M_FILTER_ITEMS_01({iddoc})")
            .AsNoTracking()
            .ToList();
        MyData.DbContextM.UpdateEntryLists(updated_items, drs_items, x => x.ID);

        var updated_items_per_store = MyData.DbContextM.M_ITEMS_PER_STORE
            .FromSql($"SELECT * FROM SP_M_FILTER_ITEMS_PER_STORE_01({iddoc})")
            .AsNoTracking()
            .ToList();
        MyData.DbContextM.UpdateEntryLists(updated_items_per_store, drs_items_per_store, x => (x.IDITEM, x.IDSTORE));


        /*foreach (var dr_item in drs_items)
        {
            MyData.DbContextM.Entry(dr_item).Reload();
        }
        foreach (var dr_items_per_store in drs_items_per_store)
        {
            MyData.DbContextM.Entry(dr_items_per_store).Reload();
        }*/
    }

    public static bool LoadLatestItemsData()
    {
        return DoLoadCheckErrors(() =>
        {
            LoadLatestItemsDataA();
        });
    }

    public static void LoadLatestItemsDataA()
    {
        var table_items = MyData.DbContextM.BL_M_ITEMS.ToList();
        var table_itemsperstore = MyData.DbContextM.BL_M_ITEMS_PER_STORE.ToList();
        var updated_items = MyData.DbContextM.M_ITEMS
            .FromSql($"SELECT * FROM M_ITEMS")
            .AsNoTracking()
            .ToList();
        MyData.DbContextM.MergeEntryLists(updated_items, table_items, x => x.ID);

        var updated_items_per_store = MyData.DbContextM.M_ITEMS_PER_STORE
            .FromSql($"SELECT * FROM M_ITEMS_PER_STORE)")
            .AsNoTracking()
            .ToList();
        MyData.DbContextM.MergeEntryLists(updated_items_per_store, table_itemsperstore, x => (x.IDITEM, x.IDSTORE));
    }


    public static List<SP_M_REP_COSTACC_1> GetBy_SP_M_REP_COSTACC_1(DateTime PDT1, DateTime PDT2)
    {
        var ret = MyData.DbContextM.Database
            .SqlQuery<SP_M_REP_COSTACC_1>($"SELECT * FROM SP_M_REP_COSTACC_1({PDT1}, {PDT2})")
            .ToList();
        return ret;
    }

    public static List<SP_M_REP_COSTACC_2> GetBy_SP_M_REP_COSTACC_2(
        DateTime PDT1, DateTime PDT2)
    {
        var ret = MyData.DbContextM.Database
            .SqlQuery<SP_M_REP_COSTACC_2>($"SELECT * FROM SP_M_REP_COSTACC_2({PDT1}, {PDT2})")
            .ToList();
        return ret;
    }

    public static List<SP_M_REP_ITEMLINKS_1> GetBy_SP_M_REP_ITEMLINKS_1(
        int PIDITEM, DateTime PDT1, DateTime PDT2)
    {
        var ret = MyData.DbContextM.Database
            .SqlQuery<SP_M_REP_ITEMLINKS_1>($"SELECT * FROM SP_M_REP_ITEMLINKS_1({PIDITEM}, {PDT1}, {PDT2})")
            .ToList();
        return ret;
    }

    public static List<SP_M_REP_ITEMSAMOUNTS_3> GetBy_SP_M_REP_ITEMSAMOUNTS_3(
        DateTime PDT1, DateTime PDT2, int? PCAT)
    {
        var ret = MyData.DbContextM.Database
            .SqlQuery<SP_M_REP_ITEMSAMOUNTS_3>($"SELECT * FROM SP_M_REP_ITEMSAMOUNTS_3({PDT1}, {PDT2}, {PCAT})")
            .ToList();
        return ret;
    }

    public static List<SP_M_REP_ITEMSAMOUNTS_3> GetBy_SP_M_REP_ITEMSAMOUNTS_6(
        DateTime PDT1, DateTime PDT2, int? PCAT, int? PIDSTORE)
    {
        var ret = MyData.DbContextM.Database
            .SqlQuery<SP_M_REP_ITEMSAMOUNTS_3>($"SELECT * FROM SP_M_REP_ITEMSAMOUNTS_6({PDT1}, {PDT2}, {PCAT}, {PIDSTORE})")
            .ToList();
        return ret;
    }

    public static List<SP_M_REP_ITEMSAMOUNTS_5> GetBy_SP_M_REP_ITEMSAMOUNTS_5(
        DateTime PDT1, DateTime PDT2, int? PCAT)
    {
        var ret = MyData.DbContextM.Database
            .SqlQuery<SP_M_REP_ITEMSAMOUNTS_5>($"SELECT * FROM SP_M_REP_ITEMSAMOUNTS_5({PDT1}, {PDT2}, {PCAT})")
            .ToList();
        return ret;
    }

    public static List<SP_M_REP_ITEMSAMOUNTS_4> GetBy_SP_M_REP_ITEMSAMOUNTS_4(
        DateTime PDT1, DateTime PDT2, int? PCAT)
    {
        var ret = MyData.DbContextM.Database
            .SqlQuery<SP_M_REP_ITEMSAMOUNTS_4>($"SELECT * FROM SP_M_REP_ITEMSAMOUNTS_4({PDT1}, {PDT2}, {PCAT})")
            .ToList();
        return ret;
    }

    public static List<SP_M_CURRENTSTOCK> GetBy_SP_M_CURRENTSTOCK_01(int PIDITEM)
    {
        var ret = MyData.DbContextM.Database
            .SqlQuery<SP_M_CURRENTSTOCK>($"SELECT * FROM SP_M_CURRENTSTOCK_01({PIDITEM})")
            .ToList();
        return ret;
    }

    public static List<SP_M_CURRENTSTOCK> GetBy_SP_M_CURRENTSTOCK_02(int PIDSTORE)
    {
        var ret = MyData.DbContextM.Database
            .SqlQuery<SP_M_CURRENTSTOCK>($"SELECT * FROM SP_M_CURRENTSTOCK_02({PIDSTORE})")
            .AsNoTracking()
            .ToList();
        return ret;
    }

    public static List<SP_M_ITEMMOVEMENT_01> GetBy_SP_M_ITEMMOVEMENT_01(
        int PIDITEM, DateTime PDATE1, DateTime PDATE2)
    {
        var ret = MyData.DbContextM.Database
            .SqlQuery<SP_M_ITEMMOVEMENT_01>($"SELECT * FROM SP_M_ITEMMOVEMENT_01({PIDITEM}, {PDATE1}, {PDATE2})")
            .ToList();
        return ret;
    }

    public static List<SP_M_REP_ITEMSINSTORE_1> GetBy_SP_M_REP_ITEMSINSTORE_1(
        int PIDSTORE)
    {
        var ret = MyData.DbContextM.Database
            .SqlQuery<SP_M_REP_ITEMSINSTORE_1>($"SELECT * FROM SP_M_REP_ITEMSINSTORE_1({PIDSTORE})")
            .ToList();
        return ret;
    }

    public static void Call_SP_M_DEL_FINDOC(int PIDDOC)
    {
        MyData.DbContextM.Database
            .ExecuteSql($"EXECUTE PROCEDURE SP_M_DEL_FINDOC {PIDDOC}");
    }

}
