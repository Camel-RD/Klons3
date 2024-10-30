using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using KlonsLIB.Forms;
using KlonsLIB.Data;
using KlonsLIB.Misc;
using FirebirdSql.Data.FirebirdClient;
using KlonsF.Classes;
using Klons3.ModelsM;
using Klons3.ModelsA;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using KlonsLIB;

namespace KlonsM.Classes;

public static class DataTasksM
{
    public static KlonsData MyData => KlonsData.St;

    public static M_STORES FindById(this BindingList<M_STORES> list, int id) =>
        MyData.DbContextM.M_STORES.Find(id);
    public static M_STORES_CAT FindById(this BindingList<M_STORES_CAT> list, int id) =>
        MyData.DbContextM.M_STORES_CAT.Find(id);
    public static M_ITEMS FindById(this BindingList<M_ITEMS> list, int id) =>
        MyData.DbContextM.M_ITEMS.Find(id);
    public static M_ITEMS_PER_STORE FindById(this BindingList<M_ITEMS_PER_STORE> list, int iditem, int idstore) =>
        MyData.DbContextM.M_ITEMS_PER_STORE.Find(iditem, idstore);
    public static M_UNITS FindById(this BindingList<M_UNITS> list, int id) =>
        MyData.DbContextM.M_UNITS.Find(id);
    public static M_ITEMS_CAT FindById(this BindingList<M_ITEMS_CAT> list, int id) =>
        MyData.DbContextM.M_ITEMS_CAT.Find(id);
    public static M_PVNRATES FindById(this BindingList<M_PVNRATES> list, int id) =>
        MyData.DbContextM.M_PVNRATES.Find(id);
    public static M_PVNTYPE FindById(this BindingList<M_PVNTYPE> list, int id) =>
        MyData.DbContextM.M_PVNTYPE.Find(id);
    public static M_DOCTYPES FindById(this BindingList<M_DOCTYPES> list, int id) =>
        MyData.DbContextM.M_DOCTYPES.Find(id);
    public static M_DOCS FindById(this BindingList<M_DOCS> list, int id) =>
        MyData.DbContextM.M_DOCS.Find(id);


    public static string GetStoreName(int id)
    {
        return MyData.DbContextM.BL_M_STORES.FindById(id)?.NAME;
    }

    public static string GetStoreCode(int id)
    {
        return MyData.DbContextM.BL_M_STORES.FindById(id)?.CODE;
    }

    public static string GetStoreCodeAndName(int id)
    {
        var dr_store = MyData.DbContextM.BL_M_STORES.FindById(id);
        if (dr_store == null) return null;
        return dr_store.CODE + " " + dr_store.NAME;
    }

    public static M_STORES GetStoreIdByCode(string code)
    {
        var dr = MyData.DbContextM.BL_M_STORES
            .Where(x => x.CODE == code)
            .FirstOrDefault();
        return dr;
    }

    public static string GetItemCodeAndName(int id)
    {
        var dr = MyData.DbContextM.BL_M_ITEMS.FindById(id);
        if (dr == null) return null;
        return $"{dr.BARCODE} {dr.NAME.Nz()}";
    }

    public static int GetNextIdSeq()
    {
        return MyData.DbContextM.Database.SqlQuery<int>($"SELECT ID FROM SP_M_GEN_DOCSEQ;").Single();
    }

    public static (int id1, int id2) GetNextIdSeqRange(int count)
    {
        int id2 = MyData.DbContextM.Database.SqlQuery<int>($"SELECT ID FROM SP_M_GEN_DOCSEQ2({count});").Single();
        int id1 = id2 - count + 1;
        return (id1, id2);
    }

    public static void UpdateIdSeq(M_DOCS dr_doc)
    {
        if (dr_doc.IDSEQ == -1)
            dr_doc.IDSEQ = GetNextIdSeq();
        var drs_rows = dr_doc.Rows.Where(x => x.IDSEQ == -1).ToList();
        int ct = drs_rows.Count;
        if (ct == 0) return;
        (int id1, int id2) = GetNextIdSeqRange(ct);
        int cid = id1;
        foreach (var dr_row in drs_rows)
        {
            dr_row.IDSEQ = cid;
            cid++;
        }
    }

    public static ErrorList CheckDocHeader(M_DOCS dr_doc)
    {
        var ret = new ErrorList();
        if (dr_doc.XDocType == EDocType.Nenoteikts)
        {
            ret.AddDocError(dr_doc.ID, "Jānorāda dokumenta veids");
            return ret;
        }
        if (dr_doc.IDSTOREIN == 1 || dr_doc.IDSTOREOUT == 1)
        {
            ret.AddDocError(dr_doc.ID, "Jānorāda noliktavas.");
            return ret;
        }
        if (!SomeDataDefsM.IsGoodStoreOut(dr_doc.XDocType, dr_doc.XStoreOutType))
        {
            ret.AddDocError(dr_doc.ID, "Jānorāda dokumenta veidam atbilstoša izdevēja noliktava.");
            return ret;
        }
        if (!SomeDataDefsM.IsGoodStoreIn(dr_doc.XDocType, dr_doc.XStoreInType))
        {
            ret.AddDocError(dr_doc.ID, "Jānorāda dokumenta veidam atbilstoša saņēmēja noliktava.");
            return ret;
        }
        if (SomeDataDefsM.IsCreditDoc(dr_doc.XDocType) && dr_doc.IDCREDDOC == null)
        {
            ret.AddDocError(dr_doc.ID, "Kredītrēķinam nav norādīts sākotnējais rēķins.");
            return ret;
        }
        if (!SomeDataDefsM.IsCreditDoc(dr_doc.XDocType) && dr_doc.IDCREDDOC != null)
        {
            ret.AddDocError(dr_doc.ID, "Tikai kredītrēķinam var būt norādīts sākotnējais rēķins.");
            return ret;
        }

        if (!IsGoodAcc(dr_doc.ACCOUT) || !IsGoodAcc(dr_doc.ACCIN))
        {
            ret.AddDocError(dr_doc.ID, "Izsniedzējam vai saņēmējam nav norādīti finanšu grāmatvedības konti.");
            return ret;
        }

        if (dr_doc.XDocType == EDocType.Pārvietots && dr_doc.ACCIN != dr_doc.ACCOUT)
        {
            ret.AddDocError(dr_doc.ID, "Pārvietošanas dokumentam izsniedzējam un saņēmējam finanšu konti nedrīkst atšķirties.");
            return ret;
        }
        return ret;
    }

    static bool IsGoodAcc(string acc) => !acc.IsNOE() && acc != "?";

    public static void UpdateDocAcc(M_DOCS dr_doc)
    {
        bool is_storeout_ours = SomeDataDefsM.IsStoreOurs(dr_doc.XStoreOutType);
        bool is_storein_ours = SomeDataDefsM.IsStoreOurs(dr_doc.XStoreInType);
        if (is_storeout_ours)
            dr_doc.ACCOUT = dr_doc.StoreOut.ACC21;
        if (is_storein_ours)
            dr_doc.ACCIN = dr_doc.StoreIn.ACC21;
        if (is_storeout_ours && is_storein_ours) return;
        bool is_storeout_partner = SomeDataDefsM.IsStorePartner(dr_doc.XStoreOutType);
        bool is_storein_partner = SomeDataDefsM.IsStorePartner(dr_doc.XStoreInType);
        if (SomeDataDefsM.IsDoc231(dr_doc.XDocType))
        {
            if (is_storeout_partner)
                dr_doc.ACCOUT = dr_doc.StoreOut.ACC23;
            if (is_storein_partner)
                dr_doc.ACCIN = dr_doc.StoreIn.ACC23;
        }
        else if (SomeDataDefsM.IsDoc531(dr_doc.XDocType))
        {
            if (is_storeout_partner)
                dr_doc.ACCOUT = dr_doc.StoreOut.ACC53;
            if (is_storein_partner)
                dr_doc.ACCIN = dr_doc.StoreIn.ACC53;
        }
    }

    public static ErrorList CheckRowsForBadAcc(M_DOCS dr_doc)
    {
        var ret = new ErrorList();
        var drs_rows = dr_doc.Rows;
        foreach (var dr_row in drs_rows)
        {
            if (!IsGoodAcc(dr_row.ACC6) || !IsGoodAcc(dr_row.ACC7))
                ret.AddItemError(dr_row.IDITEM, "Artikulam nav norādīti finanšu grāmatvedības konti.");
        }
        return ret;
    }

    public static ErrorList CheckRowsForRepeatingItems(M_DOCS dr_doc)
    {
        var ret = new ErrorList();
        var drs_rows = dr_doc.Rows.ToList();
        if (drs_rows.Count == 0) return ret;

        var ids_items_morethanone = drs_rows
            .GroupBy(x => x.IDITEM)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToList();

        foreach (int id_items_morethanone in ids_items_morethanone)
        {
            ret.AddItemError(id_items_morethanone, "Artikuls ievadīts vairāk kā vienu reizi.");
        }
        return ret;
    }

    public static ErrorList CheckDocForIepirkums(M_DOCS dr_doc)
    {
        var ret = new ErrorList();
        var drs_rows = dr_doc.Rows.ToList();
        if (drs_rows.Count == 0)
        {
            ret.AddError("dokuments", "Dokuments nesatur rindas.");
            return ret;
        }

        ret += CheckDocHeader(dr_doc);
        ret += CheckRowsForBadAcc(dr_doc);

        foreach (var dr_rows in drs_rows)
            ret += CheckRowForIepirkums(dr_rows);

        return ret;
    }

    public static ErrorList CheckRowForIepirkums(M_ROWS dr_row)
    {
        var ret = new ErrorList();
        var dr_doc = dr_row.Doc;
        var dr_item = dr_row.Item;
        int id_item = dr_item.ID;

        if (dr_row.AMOUNT <= 0)
        {
            ret.AddItemError(id_item, "Nekorekts daudzums.");
        }

        return ret;
    }

    public static ErrorList CheckDocForRealizacija(M_DOCS dr_doc)
    {
        var ret = new ErrorList();
        var drs_rows = dr_doc.Rows.ToList();
        if (drs_rows.Count == 0)
        {
            ret.AddError("dokuments", "Dokuments nesatur rindas.");
            return ret;
        }

        ret += CheckDocHeader(dr_doc);
        ret += CheckRowsForBadAcc(dr_doc);
        ret += CheckRowsForRepeatingItems(dr_doc);

        foreach (var dr_rows in drs_rows)
            ret += CheckRowForRealizacija(dr_rows);

        if (ret.HasErrors) return ret;

        var table_checkrows = MyData.DbContextM.Database
            .SqlQuery<SP_M_CHECK_RET_1>($"SELECT * FROM SP_M_MAKELINKS_02B({dr_doc.ID})")
            .ToList();

        foreach (var dr_checkrows in table_checkrows)
        {
            if (dr_checkrows.RESULT == "OK") continue;
            ret += GetErrorListFromMessage(dr_checkrows.RESULT);
        }

        return ret;
    }

    public static ErrorList CheckRowForRealizacija(M_ROWS dr_row)
    {
        var ret = new ErrorList();
        var dr_doc = dr_row.Doc;
        var dr_item = dr_row.Item;
        int id_storeout = dr_doc.IDSTOREOUT;
        int id_item = dr_item.ID;
        var table_itemsperstore = MyData.DbContextM.BL_M_ITEMS_PER_STORE;
        var dr_amount = table_itemsperstore.FindById(id_item, id_storeout);

        if (dr_amount == null || dr_row.AMOUNT <= 0)
        {
            ret.AddItemError(id_item, "Nekorekts daudzums.");
            return ret;
        }
        
        if (dr_item.XIsServices) return ret;

        if (dr_amount == null || dr_amount.AMOUNT < dr_row.AMOUNT)
        {
            ret.AddItemError(id_item, "Nepietiekams atlikums.");
            return ret;
        }

        return ret;
    }


    public static ErrorList CheckDocForRealizacijasKreditrekins(M_DOCS dr_doc)
    {
        var ret = new ErrorList();
        var drs_rows = dr_doc.Rows.ToList();
        if (drs_rows.Count == 0)
        {
            ret.AddError("dokuments", "Dokuments nesatur rindas.");
            return ret;
        }

        ret += CheckDocHeader(dr_doc);
        ret += CheckRowsForBadAcc(dr_doc);
        ret += CheckRowsForRepeatingItems(dr_doc);

        foreach (var dr_rows in drs_rows)
            ret += CheckRowForRealizacijasKreditrekins(dr_rows);

        if (ret.HasErrors) return ret;

        var table_checkrows = MyData.DbContextM.Database
            .SqlQuery<SP_M_CHECK_RET_2>($"SELECT * FROM SP_M_MAKELINKS_52({dr_doc.ID})")
            .ToList();

        foreach (var dr_checkrows in table_checkrows)
        {
            if (dr_checkrows.RESULT == "OK") continue;
            ret += GetErrorListFromMessage(dr_checkrows.RESULT);
        }

        return ret;
    }

    public static ErrorList CheckRowForRealizacijasKreditrekins(M_ROWS dr_row)
    {
        var ret = new ErrorList();
        var dr_item = dr_row.Item;
        int id_item = dr_item.ID;

        if (dr_row.AMOUNT >= 0)
        {
            ret.AddItemError(id_item, "Realizācijas kredītrēķinā uzrāda negatīvu daudzumu.");
            return ret;
        }

        return ret;
    }

    public static ErrorList CheckDocForPiegadatajaKreditrekins(M_DOCS dr_doc)
    {
        var ret = new ErrorList();
        var drs_rows = dr_doc.Rows.ToList();
        if (drs_rows.Count == 0)
        {
            ret.AddError("dokuments", "Dokuments nesatur rindas.");
            return ret;
        }

        ret += CheckDocHeader(dr_doc);
        ret += CheckRowsForBadAcc(dr_doc);
        ret += CheckRowsForRepeatingItems(dr_doc);

        foreach (var dr_rows in drs_rows)
            ret += CheckRowForPiegadatajaKreditrekins(dr_rows);

        if (ret.HasErrors) return ret;

        var table_checkrows = MyData.DbContextM.Database
            .SqlQuery<SP_M_CHECK_RET_2>($"SELECT * FROM SP_M_MAKELINKS_62({dr_doc.ID})")
            .ToList();

        foreach (var dr_checkrows in table_checkrows)
        {
            if (dr_checkrows.RESULT == "OK") continue;
            ret += GetErrorListFromMessage(dr_checkrows.RESULT);
        }

        return ret;
    }

    public static ErrorList CheckRowForPiegadatajaKreditrekins(M_ROWS dr_row)
    {
        var ret = new ErrorList();
        var dr_doc = dr_row.Doc;
        var dr_item = dr_row.Item;
        int id_storeout = dr_doc.IDSTOREOUT;
        int id_item = dr_item.ID;
        var table_itemsperstore = MyData.DbContextM.BL_M_ITEMS_PER_STORE;
        var dr_amount = table_itemsperstore.FindById(id_item, id_storeout);

        if (dr_amount == null || dr_row.AMOUNT <= 0)
        {
            ret.AddItemError(id_item, "Nekorekts daudzums.");
            return ret;
        }

        if (dr_item.XIsServices) return ret;

        if (dr_amount == null || dr_amount.AMOUNT < dr_row.AMOUNT)
        {
            ret.AddItemError(id_item, "Nepietiekams atlikums.");
            return ret;
        }

        return ret;
    }


    public static ErrorList CheckDocForAtgrieztsPiegadatajam(
        M_DOCS dr_doc, bool setprice)
    {
        var ret = new ErrorList();
        var drs_rows = dr_doc.Rows.ToList();
        if (drs_rows.Count == 0)
        {
            ret.AddError("dokuments", "Dokuments nesatur rindas.");
            return ret;
        }

        ret += CheckDocHeader(dr_doc);
        ret += CheckRowsForBadAcc(dr_doc);
        if (ret.HasErrors) return ret;

        var ids_items_morethanone = drs_rows
            .GroupBy(x => x.IDITEM)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToList();

        if (ids_items_morethanone.Count > 0)
        {
            foreach (int id_items_morethanone in ids_items_morethanone)
            {
                ret.AddItemError(id_items_morethanone, "Artikuls ievadīts vairāk kā vienu reizi.");
            }
            return ret;
        }

        var items_withallamount = drs_rows
            .GroupBy(x => x.IDITEM)
            .Select(x => (key: x.Key, amount: x.Sum(y => y.AMOUNT)));

        var drs_rows_withallamount = drs_rows
            .Join(items_withallamount, x => x.IDITEM, x => x.key, (dr, y) => (dr, y.amount));


        foreach (var dr_row_withallamount in drs_rows_withallamount)
        {
            ret += CheckRowForAtgrieztsPiegadatajam(dr_row_withallamount.dr, dr_row_withallamount.amount);
        }

        if (ret.HasErrors) return ret;

        var table_rowchecks = MyData.DbContextM.Database
            .SqlQuery<SP_M_CHECK_RET_1>($"SELECT * FROM SP_M_MAKELINKS_22B({dr_doc.ID})")
            .ToList();

        if (table_rowchecks.Count == 0)
        {
            ret.AddError("", "Nav atlikuma atgiešanai.");
            return ret;
        }

        foreach (var row_checks in table_rowchecks)
        {
            if (row_checks.RESULT == "OK") continue;
            ret += GetErrorListFromMessage(row_checks.RESULT);
        }

        if (ret.HasErrors) return ret;

        if (!setprice) return ret;
        
        foreach (var dr_row in drs_rows)
        {
            if (dr_row.XIsServices) continue;
            var dr_checks = table_rowchecks.Where(x => x.IDROW == dr_row.ID).Single();
            dr_row.PRICE0 = dr_checks.BUYPRICE;
            dr_row.DISCOUNT = 0f;
            dr_row.PRICE = dr_checks.BUYPRICE;
            dr_row.TPRICE = dr_checks.TBUYPRICE;
        }

        return ret;
    }

    public static ErrorList CheckRowForAtgrieztsPiegadatajam(M_ROWS dr_row, decimal allamount)
    {
        var ret = new ErrorList();
        var dr_doc = dr_row.Doc;
        var dr_item = dr_row.Item;
        int id_storeout = dr_doc.IDSTOREOUT;
        int id_item = dr_item.ID;
        var table_itemsperstore = MyData.DbContextM.BL_M_ITEMS_PER_STORE;
        var dr_amount = table_itemsperstore.FindById(id_item, id_storeout);

        if (dr_amount == null || dr_row.AMOUNT <= 0)
        {
            ret.AddItemError(id_item, "Nekorekts daudzums.");
            return ret;
        }

        if (dr_item.XIsServices) return ret;

        if (dr_amount == null || dr_amount.AMOUNT < dr_row.AMOUNT || allamount < dr_row.AMOUNT)
        {
            ret.AddItemError(id_item, "Nepietiekams atlikums.");
            return ret;
        }

        return ret;
    }


    public static ErrorList CheckDocForAtgrieztsNoPircēja(
        M_DOCS dr_doc, bool setprice)
    {
        var ret = new ErrorList();
        var drs_rows = dr_doc.Rows.ToList();
        if (drs_rows.Count == 0)
        {
            ret.AddError("dokuments", "Dokuments nesatur rindas.");
            return ret;
        }

        ret += CheckDocHeader(dr_doc);
        ret += CheckRowsForBadAcc(dr_doc);
        if (ret.HasErrors) return ret;

        var ids_items_morethanone = drs_rows
            .GroupBy(x => x.IDITEM)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToList();

        if (ids_items_morethanone.Count > 0)
        {
            foreach (int id_items_morethanone in ids_items_morethanone)
            {
                ret.AddItemError(id_items_morethanone, "Artikuls ievadīts vairāk kā vienu reizi.");
            }
            return ret;
        }

        var items_withallamount = drs_rows
            .GroupBy(x => x.IDITEM)
            .Select(x => (key: x.Key, amount: x.Sum(y => y.AMOUNT)));

        var drs_rows_withallamount = drs_rows
            .Join(items_withallamount, x => x.IDITEM, x => x.key, (dr, y) => (dr, y.amount));


        foreach (var dr_row_withallamount in drs_rows_withallamount)
        {
            ret += CheckRowForAtgrieztsNoPircēja(dr_row_withallamount.dr, dr_row_withallamount.amount);
        }
        if (ret.HasErrors) return ret;

        var table_rowchecks = MyData.DbContextM.Database
            .SqlQuery<SP_M_CHECK_RET_3>($"SELECT * FROM SP_M_MAKELINKS_33B({dr_doc.ID})")
            .ToList();

        if (table_rowchecks.Count == 0)
        {
            ret.AddError("", "Nav atlikuma atgiešanai.");
            return ret;
        }

        foreach (var row_checks in table_rowchecks)
        {
            if (row_checks.RESULT == "OK") continue;
            ret += GetErrorListFromMessage(row_checks.RESULT);
        }

        if (ret.HasErrors) return ret;

        if (!setprice) return ret;

        foreach (var dr_row in drs_rows)
        {
            if (dr_row.XIsServices) continue;
            var dr_checks = table_rowchecks.Where(x => x.IDROW == dr_row.ID).Single();
            dr_row.PRICE0 = -dr_checks.SELLPRICE;
            dr_row.DISCOUNT = 0f;
            dr_row.PRICE = -dr_checks.SELLPRICE;
            dr_row.TPRICE = -dr_checks.SELLTOTALPRICE;
            dr_row.OLDPRICE = dr_checks.SELLPRICE;
            dr_row.TOLDPRICE = dr_checks.SELLTOTALPRICE;
            dr_row.BUYPRICE = dr_checks.BUYPRICE;
            dr_row.TBUYPRICE = dr_checks.BUYTOTALPRICE;
        }

        return ret;
    }

    public static ErrorList CheckRowForAtgrieztsNoPircēja(M_ROWS dr_row, decimal allamount)
    {
        var ret = new ErrorList();
        var dr_doc = dr_row.Doc;
        var dr_item = dr_row.Item;
        int id_item = dr_item.ID;

        if (dr_row.AMOUNT >= 0)
        {
            ret.AddItemError(id_item, "No pircēja atgrieztas preces dokumentā uzrāda negatīvus daudzumus.");
            return ret;
        }

        //if (dr_item.XIsServices) return ret;

        return ret;
    }


    public static ErrorList CheckDocForPierakstīts(
        M_DOCS dr_doc, bool setprice)
    {
        var ret = new ErrorList();
        var drs_rows = dr_doc.Rows.ToList();
        if (drs_rows.Count == 0)
        {
            ret.AddError("dokuments", "Dokuments nesatur rindas.");
            return ret;
        }

        ret += CheckDocHeader(dr_doc);
        ret += CheckRowsForBadAcc(dr_doc);
        ret += CheckRowsForRepeatingItems(dr_doc);
        if (ret.HasErrors) return ret;


        foreach (var dr_row in drs_rows)
            ret += CheckRowForPierakstīts(dr_row);
        if (ret.HasErrors) return ret;

        var table_lastprices = MyData.DbContextM.Database
            .SqlQuery<SP_M_CHECK_RET_4>($"SELECT * FROM SP_M_MAKELINKS_12({dr_doc.ID})")
            .ToList();

        var drs_missinglastprice = drs_rows
            .Where(x => !table_lastprices.Where(y => y.IDITEM == x.IDITEM).Any())
            .ToList();

        foreach (var dr in drs_missinglastprice)
            ret.AddItemError(dr.IDITEM, "Nav atrasta pēdējā iepirkuma cena.");


        if (ret.HasErrors) return ret;

        if (!setprice) return ret;

        foreach (var dr_row in drs_rows)
        {
            if (dr_row.XIsServices) continue;
            var dr_lastprice = table_lastprices.Where(x => x.IDITEM == dr_row.IDITEM).FirstOrDefault();
            if(dr_lastprice == null)
            {
                dr_row.PRICE0 = 0M;
                dr_row.DISCOUNT = 0f;
                dr_row.PRICE = 0M;
                dr_row.TPRICE = 0M;
                dr_row.BUYPRICE = 0M;
                dr_row.TBUYPRICE = 0M;
            }
            else
            {
                var price = dr_lastprice.LASTBUYPRICE;
                var tprice = Math.Round(price * dr_row.AMOUNT, 2);
                dr_row.PRICE0 = price;
                dr_row.DISCOUNT = 0f;
                dr_row.PRICE = price;
                dr_row.TPRICE = tprice;
                dr_row.BUYPRICE = price;
                dr_row.TBUYPRICE = tprice;
            }
        }

        return ret;
    }

    public static ErrorList CheckRowForPierakstīts(M_ROWS dr_row)
    {
        var ret = new ErrorList();
        var dr_item = dr_row.Item;
        int id_item = dr_item.ID;

        if (dr_row.AMOUNT <= 0)
        {
            ret.AddItemError(id_item, "Daudzumam jābūt lielākam par 0.");
            return ret;
        }

        return ret;
    }


    public static ErrorList CheckDocForSaražots(
        M_DOCS dr_doc, bool setprice)
    {
        var ret = new ErrorList();
        var drs_rows = dr_doc.Rows.ToList();
        if (drs_rows.Count == 0)
        {
            ret.AddError("dokuments", "Dokuments nesatur rindas.");
            return ret;
        }

        ret += CheckDocHeader(dr_doc);
        ret += CheckRowsForBadAcc(dr_doc);
        ret += CheckRowsForRepeatingItems(dr_doc);
        if (ret.HasErrors) return ret;


        foreach (var dr_row in drs_rows)
            ret += CheckRowForSaražots(dr_row);
        if (ret.HasErrors) return ret;

        var table_prodcosts = MyData.DbContextM.Database
            .SqlQuery<SP_M_CHECK_RET_5>($"SELECT * FROM SP_M_MAKELINKS_14A({dr_doc.ID})")
            .ToList();

        var drs_missinglastprice = drs_rows
            .Where(x => !table_prodcosts.Where(y => y.IDITEM == x.IDITEM).Any())
            .ToList();

        foreach (var dr in drs_missinglastprice)
            ret.AddItemError(dr.IDITEM, "Nav atrasta pašizmaksa.");

        var iditems_zerocost = table_prodcosts
            .Where(x => x.PRODCOST == 0M)
            .Select(x => x.IDITEM)
            .ToList();

        foreach (var iditem in iditems_zerocost)
            ret.AddItemError(iditem, "Nav norādīta pašizmaksa.");

        if (ret.HasErrors) return ret;

        if (!setprice) return ret;

        foreach (var dr_row in drs_rows)
        {
            if (dr_row.XIsServices) continue;
            var dr_costs = table_prodcosts.Where(x => x.IDITEM == dr_row.IDITEM).FirstOrDefault();
            if (dr_costs == null)
            {
                dr_row.PRICE0 = 0M;
                dr_row.DISCOUNT = 0f;
                dr_row.PRICE = 0M;
                dr_row.TPRICE = 0M;
                dr_row.BUYPRICE = 0M;
                dr_row.TBUYPRICE = 0M;
            }
            else
            {
                var price = dr_costs.PRODCOST;
                var tprice = Math.Round(price * dr_row.AMOUNT, 2);
                dr_row.PRICE0 = price;
                dr_row.DISCOUNT = 0f;
                dr_row.PRICE = price;
                dr_row.TPRICE = tprice;
                dr_row.BUYPRICE = price;
                dr_row.TBUYPRICE = tprice;
            }
        }

        return ret;
    }

    public static ErrorList CheckRowForSaražots(M_ROWS dr_row)
    {
        var ret = new ErrorList();
        var dr_item = dr_row.Item;
        int id_item = dr_item.ID;

        if (dr_row.AMOUNT <= 0)
        {
            ret.AddItemError(id_item, "Daudzumam jābūt lielākam par 0.");
            return ret;
        }

        return ret;
    }

    public static ErrorList ProcessDocXXX(M_DOCS dr_doc,
        Action act)
    {
        var ret = new ErrorList();
        try
        {
            act();
            DataLoaderM.LoadDocAndRowsByFilter(dr_doc.ID, false);
            DataLoaderM.LoadLatestAmountsByDoc(dr_doc.ID);
            return ret;
        }
        catch (Exception ex)
        {
            ret += GetErrorListFromMessage(ex.Message);
            return ret;
        }
    }

    static int docstate_iegrāmatots = (int)EDocState.Iegrāmatots;
    public static ErrorList ProcessDocIepirkums(M_DOCS dr_doc)
    {
        var ret = ProcessDocXXX(dr_doc, 
            () => MyData.DbContextM.Database.ExecuteSql($"EXECUTE PROCEDURE SP_M_MAKELINKS_11 {dr_doc.ID}, {docstate_iegrāmatots}"));
        return ret;
    }

    public static ErrorList ProcessDocRealizacija(M_DOCS dr_doc)
    {
        var ret = ProcessDocXXX(dr_doc,
            () => MyData.DbContextM.Database.ExecuteSql($"EXECUTE PROCEDURE SP_M_MAKELINKS_01B {dr_doc.ID}, {docstate_iegrāmatots}"));
        return ret;
    }

    public static ErrorList ProcessDocRealizacijasKreditrekins(M_DOCS dr_doc)
    {
        var ret = ProcessDocXXX(dr_doc,
            () => MyData.DbContextM.Database.ExecuteSql($"EXECUTE PROCEDURE SP_M_MAKELINKS_51B {dr_doc.ID}, {docstate_iegrāmatots}"));
        return ret;
    }

    public static ErrorList ProcessDocPiegadatajaKreditrekins(M_DOCS dr_doc)
    {
        var ret = ProcessDocXXX(dr_doc,
            () => MyData.DbContextM.Database.ExecuteSql($"EXECUTE PROCEDURE SP_M_MAKELINKS_61B {dr_doc.ID}, {docstate_iegrāmatots}"));
        return ret;
    }

    public static ErrorList ProcessDocAtgrieztsPiegadatajam(M_DOCS dr_doc)
    {
        var ret = ProcessDocXXX(dr_doc,
            () => MyData.DbContextM.Database.ExecuteSql($"EXECUTE PROCEDURE SP_M_MAKELINKS_21B {dr_doc.ID}, {docstate_iegrāmatots}"));
        return ret;
    }
    
    public static ErrorList ProcessDocAtgrieztsNoPirceja(M_DOCS dr_doc)
    {
        var ret = ProcessDocXXX(dr_doc,
            () => MyData.DbContextM.Database.ExecuteSql($"EXECUTE PROCEDURE SP_M_MAKELINKS_31B {dr_doc.ID}, {docstate_iegrāmatots}"));
        return ret;
    }

    public static ErrorList ProcessDocPārvietots(M_DOCS dr_doc)
    {
        var ret = ProcessDocXXX(dr_doc,
            () => MyData.DbContextM.Database.ExecuteSql($"EXECUTE PROCEDURE SP_M_MAKELINKS_41 {dr_doc.ID}, {docstate_iegrāmatots}"));
        return ret;
    }

    public static ErrorList ProcessDocPierakstīts(M_DOCS dr_doc)
    {
        var ret = ProcessDocXXX(dr_doc,
            () => MyData.DbContextM.Database.ExecuteSql($"EXECUTE PROCEDURE SP_M_MAKELINKS_13 {dr_doc.ID}, {docstate_iegrāmatots}"));
        return ret;
    }

    public static ErrorList ProcessDocSaražots(M_DOCS dr_doc)
    {
        var ret = ProcessDocXXX(dr_doc,
            () => MyData.DbContextM.Database.ExecuteSql($"EXECUTE PROCEDURE SP_M_MAKELINKS_11 {dr_doc.ID}, {docstate_iegrāmatots}"));
        return ret;
    }

    public static int? GetIdFromErrorMessage(string msg, string idname)
    {
        if (msg.IsNOE()) return null;
        var s1 = $"{idname}:[";
        int k1 = msg.IndexOf(s1);
        if (k1 == -1) return null;
        k1 += s1.Length;
        int k2 = msg.IndexOf(']', k1);
        if (k2 == -1) return null;
        var s2 = msg.Substring(k1, k2 - k1);
        if (!int.TryParse(s2, out int ret)) return null;
        return ret;
    }

    public static string GetErrorInfoFromMessage(string msg)
    {
        var ret = msg;
        var s1 = msg;
        var ss = msg.Split("\r\n".ToArray(), StringSplitOptions.RemoveEmptyEntries);
        if (ss.Length > 1 && msg.StartsWith("EX_FAILURE"))
            s1 = ss[1];
        
        if (s1.StartsWith("Amount cant be < 0"))
            return "Daudzums nedrīkst būt negatīvs.";

        bool TestErrorA(string startswith, string msgformat)
        {
            if (!s1.StartsWith(startswith)) return false;
            int? iditem = GetIdFromErrorMessage(msg, "item");
            int? idstore = GetIdFromErrorMessage(msg, "store");
            if (iditem.HasValue && idstore.HasValue)
            {
                var itemdata = GetItemCodeAndName(iditem.Value);
                var storecode = GetStoreCode(idstore.Value);
                ret = $"{msgformat}\nNoliktava: {storecode}\nArtikuls: {itemdata}";
            }
            else if (iditem.HasValue)
            {
                var itemdata = GetItemCodeAndName(iditem.Value);
                ret = $"{msgformat}\nArtikuls: {itemdata}";
            }
            else
            {
                ret = msgformat;
            }
            return true;
        }

        if (TestErrorA("Not enough amount in stock", 
            "Nepietiekams atlikums noliktavā."))
            return ret;

        if (TestErrorA("Not enough amount recieved from supplier to return",
            "Nepietiekams daudzums saņemts no piegādātāja lai atgrieztu."))
            return ret;

        if (TestErrorA("Not enough amount sent to buyer to return",
            "Nepietiekams atlikums nosūtīts pircējam lai atgrieztu."))
            return ret;

        if (TestErrorA("Credited amount greater than sale",
            "Kredītrēķinā daudzums lielāks kā sākotnējā rēķinā."))
            return ret;

        if (TestErrorA("Used amount greater than sale",
            "Kredītrēķinos norādītais daudzums lielāks kā sākotnējā rēķinā."))
            return ret;

        if (TestErrorA("Credited amount greater than reveived",
            "Kredītrēķinā daudzums lielāks kā sākotnējā rēķinā."))
            return ret;

        if (TestErrorA("Used amount greater than received",
            "Kredītrēķinos un izejošajos rēķinos norādītais daudzums lielāks kā sākotnējā rēķinā."))
            return ret;
        
        if (TestErrorA("Items in row used in other doc",
            "Prece izmantota citā dokumentā."))
            return ret;
        
        if (TestErrorA("Creditdoc date older than original doc",
            "Kredītrēķina datums vecāks kā originālā dokumenta datums."))
            return ret;
        
        if (TestErrorA("Amount must be negative",
            "Realizācijas kredītrēķinā norādītajiem daudzumiem jābūt negatīviem."))
            return ret;
        return ret;
    }

    public static ErrorList GetErrorListFromMessage(string msg)
    {
        var ret = new ErrorList();
        var s1 = msg;
        var ss = msg.Split("\r\n".ToArray(), StringSplitOptions.RemoveEmptyEntries);
        if (ss.Length > 1 && msg.StartsWith("EX_FAILURE"))
            s1 = ss[1];

        if (s1.StartsWith("Amount cant be < 0"))
        {
            ret.AddError("", $"Daudzums nedrīkst būt negatīvs.");
            return ret;
        }

        bool TestErrorA(string startswith, string msgformat, string msgformat2)
        {
            if (!s1.StartsWith(startswith)) return false;
            int? iditem = GetIdFromErrorMessage(msg, "item");
            int? idstore = GetIdFromErrorMessage(msg, "store");
            if (iditem.HasValue && idstore.HasValue)
            {
                var storecode = GetStoreCode(idstore.Value);
                var fmsg = msgformat + string.Format(msgformat2, storecode);
                ret.AddItemError(iditem.Value, fmsg);
            }
            else if (iditem.HasValue)
            {
                ret.AddItemError(iditem.Value, msgformat);
            }
            else
            {
                ret.AddError("", msgformat);
            }
            return true;
        }

        if (TestErrorA("Not enough amount in stock", 
            "Nepietiekams atlikums noliktavā.", "\nNoliktava: {0}"))
            return ret;

        if (TestErrorA("Not enough amount recieved from supplier to return",
            "Nepietiekams daudzums saņemts no piegādātāja lai atgrieztu.", 
            "\nNoliktava: {0}"))
            return ret;

        if (TestErrorA("Not enough amount sent to buyer to return",
            "Nepietiekams atlikums nosūtīts pircējam lai atgrieztu.", "\nNoliktava: {0}"))
            return ret;

        if (TestErrorA("Credited amount greater than sale",
            "Kredītrēķinā daudzums lielāks kā sākotnējā rēķinā.",
            "\nNoliktava: {0}"))
            return ret;

        if (TestErrorA("Used amount greater than sale",
            "Kredītrēķinos norādītais daudzums lielāks kā sākotnējā rēķinā.", 
            "\nNoliktava: {0}"))
            return ret;

        if (TestErrorA("Credited amount greater than reveived",
            "Kredītrēķinā daudzums lielāks kā sākotnējā rēķinā.", 
            "\nNoliktava: {0}"))
            return ret;

        if (TestErrorA("Used amount greater than received",
            "Kredītrēķinos un izejošajos rēķinos norādītais daudzums lielāks kā sākotnējā rēķinā.", 
            "\nNoliktava: {0}"))
            return ret;
        
        if (TestErrorA("Items in row used in other doc",
            "Prece izmantota citā dokumentā.", ""))
            return ret;
        
        if (TestErrorA("Creditdoc date older than original doc",
            "Kredītrēķina datums vecāks kā originālā dokumenta datums.", ""))
            return ret;
        
        if (TestErrorA("Amount must be negative",
            "Realizācijas kredītrēķinā norādītajiem daudzumiem jābūt negatīviem.", ""))
            return ret;

        ret.AddError("", msg);
        return ret;
    }

    public static ErrorList ProcessDoc(M_DOCS dr_doc)
    {
        var err = CheckDocHeader(dr_doc);
        if (err.HasErrors) return err;
        if (!dr_doc.IsOpenForChanges)
        {
            err.AddDocError(dr_doc.ID, "Dokuments jau iegrāmatots.");
            return err;
        }
        if (dr_doc.XDocType == EDocType.Iepirkums ||
            dr_doc.XDocType == EDocType.Sākuma_atlikums ||
            dr_doc.XDocType == EDocType.No_noliktavas)
        {
            err = CheckDocForIepirkums(dr_doc);
            if (err.HasErrors) return err;
            err += ProcessDocIepirkums(dr_doc);
            return err;
        }
        if (dr_doc.XDocType == EDocType.Realizācija ||
            dr_doc.XDocType == EDocType.Norakstīts ||
            dr_doc.XDocType == EDocType.Uz_noliktavu ||
            dr_doc.XDocType == EDocType.Izlietots)
        {
            err = CheckDocForRealizacija(dr_doc);
            if (err.HasErrors) return err;
            err += ProcessDocRealizacija(dr_doc);
            return err;
        }
        if (dr_doc.XDocType == EDocType.Kredītrēķins_pircējam)
        {
            err = CheckDocForRealizacijasKreditrekins(dr_doc);
            if (err.HasErrors) return err;
            err += ProcessDocRealizacijasKreditrekins(dr_doc);
            return err;
        }
        if (dr_doc.XDocType == EDocType.Kredītrēķins_no_piegādātāja)
        {
            err = CheckDocForPiegadatajaKreditrekins(dr_doc);
            if (err.HasErrors) return err;
            err += ProcessDocPiegadatajaKreditrekins(dr_doc);
            return err;
        }
        if (dr_doc.XDocType == EDocType.Atgriezts_piegādātājam)
        {
            err = CheckDocForAtgrieztsPiegadatajam(dr_doc, false);
            if (err.HasErrors) return err;
            err += ProcessDocAtgrieztsPiegadatajam(dr_doc);
            return err;
        }
        if (dr_doc.XDocType == EDocType.Atgriezts_no_pircēja)
        {
            err = CheckDocForAtgrieztsNoPircēja(dr_doc, false);
            if (err.HasErrors) return err;
            err += ProcessDocAtgrieztsNoPirceja(dr_doc);
            return err;
        }
        if (dr_doc.XDocType == EDocType.Pārvietots)
        {
            err = ProcessDocAtgrieztsNoPirceja(dr_doc);
            return err;
        }
        if (dr_doc.XDocType == EDocType.Pierakstīts)
        {
            err = CheckDocForPierakstīts(dr_doc, false);
            if (err.HasErrors) return err;
            err += ProcessDocPierakstīts(dr_doc);
            return err;
        }
        if (dr_doc.XDocType == EDocType.Saražots)
        {
            err = CheckDocForSaražots(dr_doc, false);
            if (err.HasErrors) return err;
            err += ProcessDocSaražots(dr_doc);
            return err;
        }


        return err;
    }

    static void SP_M_REMOVELINKS_01(int doc_id, int newstate)
    {
        MyData.DbContextM.Database
            .ExecuteSql($"EXECUTE RPOCEDURE SP_M_REMOVELINKS_01 {doc_id}, {newstate}");
    }

    public static ErrorList OpenDoc(M_DOCS dr_doc)
    {
        var ret = new ErrorList();
        if (dr_doc.IsOpenForChanges)
            return ret;
        var newstate = (int)EDocState.Atvērts;
        try
        {
            SP_M_REMOVELINKS_01(dr_doc.ID, newstate);
            DataLoaderM.LoadDocAndRowsByFilter(dr_doc.ID, false);
            DataLoaderM.LoadLatestAmountsByDoc(dr_doc.ID);
            return ret;
        }
        catch (Exception ex)
        {
            ret += GetErrorListFromMessage(ex.Message);
        }
        return ret;
    }

    public static ErrorList RecalcDoc(M_DOCS dr_doc, EDocState newstate) 
    {
        var ret = new ErrorList();
        try
        {
            //MyData.KlonsMQueriesTableAdapter.SP_M_RECALCITEM_01C(dr_doc.ID, (int)newstate, 0);
            var rt = FullRecalcDocA(dr_doc.ID, newstate);
            if (rt != "OK") return ret;
            DataLoaderM.LoadDocAndRowsByFilter(dr_doc.ID, false);
            DataLoaderM.LoadLatestAmountsByDoc(dr_doc.ID);
            return ret;
        }
        catch (Exception ex)
        {
            ret += GetErrorListFromMessage(ex.Message);
        }
        return ret;
    }

    public static string CopyDoc(M_DOCS dr_doc,
        out M_DOCS new_dr_doc)
    {
        new_dr_doc = null;

        var table_docs = MyData.DbContextM.BL_M_DOCS;
        var table_rows = MyData.DbContextM.BL_M_ROWS;
        var drs_rows = dr_doc.Rows
            .OrderBy(x => x.IDSEQ)
            .ToList();

        new_dr_doc = table_docs.NewItem();
        new_dr_doc.DT = DateTime.Today;
        new_dr_doc.XDocType = dr_doc.XDocType;
        new_dr_doc.XDocType2 = dr_doc.XDocType2;
        new_dr_doc.XState = EDocState.Melnraksts;
        new_dr_doc.SR = dr_doc.SR;
        new_dr_doc.NR = dr_doc.NR;
        new_dr_doc.IDSTOREIN = dr_doc.IDSTOREIN;
        new_dr_doc.IDSTOREOUT = dr_doc.IDSTOREOUT;

        new_dr_doc.IDCARRIER = dr_doc.IDCARRIER;
        new_dr_doc.IDADDRESSOUT = dr_doc.IDADDRESSOUT;
        new_dr_doc.IDADDRESSIN = dr_doc.IDADDRESSIN;
        new_dr_doc.IDPAYMENTTYPE = dr_doc.IDPAYMENTTYPE;
        new_dr_doc.IDDRIVER = dr_doc.IDDRIVER;
        new_dr_doc.DUEDATE = dr_doc.DUEDATE;

        new_dr_doc.DocType = dr_doc.DocType;
        new_dr_doc.TransactionType = dr_doc.TransactionType;
        new_dr_doc.StoreIn = dr_doc.StoreIn;
        new_dr_doc.StoreOut = dr_doc.StoreOut;
        new_dr_doc.Carrier = dr_doc.Carrier;
        new_dr_doc.AddressIn = dr_doc.AddressIn;
        new_dr_doc.AddressOut = dr_doc.AddressOut;
        new_dr_doc.PaymentType = dr_doc.PaymentType;
        new_dr_doc.Driver = dr_doc.Driver;
        new_dr_doc.Vehicle = dr_doc.Vehicle;

        new_dr_doc.PVNTYPE = dr_doc.PVNTYPE;
        new_dr_doc.SUMM = dr_doc.SUMM;
        new_dr_doc.ACCIN = dr_doc.ACCOUT;
        new_dr_doc.ACCOUT = dr_doc.ACCIN;
        new_dr_doc.IDSEQ = GetNextIdSeq();
        //new_dr_doc.IDCREDDOC = dr_doc.IDCREDDOC;
        //new_dr_doc.CREDDOCDT = dr_doc.CREDDOCDT;
        //new_dr_doc.CREDDOCSR = dr_doc.SR;
        //new_dr_doc.CREDDOCNR = dr_doc.NR;
        table_docs.Add(new_dr_doc);

        foreach (var dr_row in drs_rows)
        {
            var new_dr_row = table_rows.NewItem();
            new_dr_row.IDDOC = new_dr_doc.ID;
            //new_dr_row.IDCREDROW = dr_row.IDCREDROW;
            new_dr_row.IDITEM = dr_row.IDITEM;
            new_dr_row.IDPVNRATE = dr_row.IDPVNRATE;
            new_dr_row.UNITS = dr_row.UNITS;
            //new_dr_row.IDSEQ = dr_row.IDSEQ;
            new_dr_row.AMOUNT = dr_row.AMOUNT;
            new_dr_row.DISCOUNT = dr_row.DISCOUNT;
            new_dr_row.PRICE0 = dr_row.PRICE;
            new_dr_row.PRICE = dr_row.PRICE;
            new_dr_row.TPRICE = dr_row.TPRICE;
            new_dr_row.BUYPRICE = dr_row.BUYPRICE;
            new_dr_row.TBUYPRICE = dr_row.TBUYPRICE;
            new_dr_row.ACC6 = dr_row.ACC6;
            new_dr_row.ACC7 = dr_row.ACC7;

            new_dr_row.Doc = new_dr_doc;
            new_dr_row.Item = dr_row.Item;
            new_dr_row.PVNRate = dr_row.PVNRate;
            new_dr_row.UnitsRow = dr_row.UnitsRow;


            table_rows.Add(new_dr_row);
        }

        UpdateIdSeq(new_dr_doc);

        return "OK";
    }

    public static string MakeCreditDoc(M_DOCS dr_doc,
        out M_DOCS new_dr_doc)
    {
        new_dr_doc = null;
        if (dr_doc.XState == EDocState.Melnraksts || dr_doc.XState == EDocState.Atvērts)
            return "Dokuments nav iegrāmatots.";
        if (dr_doc.XDocType != EDocType.Iepirkums && dr_doc.XDocType != EDocType.Realizācija)
            return "Kredītrēķinu var veidot tikai iepirkuma vai realizācijas dokumentam.";
        
        EDocType new_doctype = EDocType.Kredītrēķins_pircējam;
        if (dr_doc.XDocType == EDocType.Iepirkums)
            new_doctype = EDocType.Kredītrēķins_no_piegādātāja;
        
        var table_docs = MyData.DbContextM.BL_M_DOCS;
        var table_rows = MyData.DbContextM.BL_M_ROWS;
        var drs_rows = dr_doc.Rows
            .OrderBy(x => x.IDSEQ)
            .ToList();

        new_dr_doc = table_docs.NewItem();
        new_dr_doc.IDCREDDOC = dr_doc.ID;
        new_dr_doc.DT = DateTime.Today;
        new_dr_doc.XDocType = new_doctype;
        new_dr_doc.XDocType2 = dr_doc.XDocType2;
        new_dr_doc.XState = EDocState.Melnraksts;
        new_dr_doc.SR = dr_doc.SR;
        new_dr_doc.NR = dr_doc.NR;

        new_dr_doc.DocType = dr_doc.DocType;
        new_dr_doc.TransactionType = dr_doc.TransactionType;

        if (new_doctype == EDocType.Kredītrēķins_pircējam)
        {
            new_dr_doc.IDSTOREIN = dr_doc.IDSTOREIN;
            new_dr_doc.IDSTOREOUT = dr_doc.IDSTOREOUT;
            new_dr_doc.ACCIN = dr_doc.ACCIN;
            new_dr_doc.ACCOUT = dr_doc.ACCOUT;

            new_dr_doc.StoreIn = dr_doc.StoreIn;
            new_dr_doc.StoreOut = dr_doc.StoreOut;
        }
        else
        {
            new_dr_doc.IDSTOREIN = dr_doc.IDSTOREOUT;
            new_dr_doc.IDSTOREOUT = dr_doc.IDSTOREIN;
            new_dr_doc.ACCIN = dr_doc.ACCOUT;
            new_dr_doc.ACCOUT = dr_doc.ACCIN;

            new_dr_doc.StoreIn = dr_doc.StoreOut;
            new_dr_doc.StoreOut = dr_doc.StoreIn;
        }
        new_dr_doc.PVNTYPE = dr_doc.PVNTYPE;
        new_dr_doc.IDSEQ = GetNextIdSeq();
        new_dr_doc.CREDDOCDT = dr_doc.DT;
        new_dr_doc.CREDDOCSR = dr_doc.SR;
        new_dr_doc.CREDDOCNR = dr_doc.NR;

        table_docs.Add(new_dr_doc);

        foreach (var dr_row in drs_rows)
        {
            var new_dr_row = table_rows.NewItem();
            new_dr_row.IDDOC = new_dr_doc.ID;
            new_dr_row.IDCREDROW = dr_row.ID;
            new_dr_row.IDITEM = dr_row.IDITEM;
            new_dr_row.IDPVNRATE = dr_row.IDPVNRATE;
            new_dr_row.UNITS = dr_row.UNITS;
            //new_dr_row.IDSEQ = cidseq;
            new_dr_row.PRICE0 = dr_row.PRICE;
            new_dr_row.PRICE = dr_row.PRICE;

            new_dr_row.Doc = new_dr_doc;
            new_dr_row.Item = dr_row.Item;
            new_dr_row.PVNRate = dr_row.PVNRate;
            new_dr_row.UnitsRow = dr_row.UnitsRow;

            if (new_doctype == EDocType.Kredītrēķins_pircējam)
            {
                new_dr_row.AMOUNT = -dr_row.AMOUNT;
                new_dr_row.TPRICE = -dr_row.TPRICE;
            }
            else
            {
                new_dr_row.AMOUNT = dr_row.AMOUNT;
                new_dr_row.TPRICE = dr_row.TPRICE;
            }
            if (new_doctype == EDocType.Kredītrēķins_pircējam)
            {
                new_dr_row.OLDPRICE = dr_row.PRICE;
                new_dr_row.TOLDPRICE = dr_row.TPRICE;
            }
            new_dr_row.BUYPRICE = dr_row.BUYPRICE;
            new_dr_row.TBUYPRICE = dr_row.TBUYPRICE;
            new_dr_row.ACC6 = dr_row.ACC6;
            new_dr_row.ACC7 = dr_row.ACC7;
            
            table_rows.Add(new_dr_row);
        }

        UpdateIdSeq(new_dr_doc);

        return "OK";
    }


    public static ErrorList MakeCreditDocsFromAtgrieztsPiegadatajam(M_DOCS dr_doc)
    {
        var ret = new ErrorList();
        if (!dr_doc.IsOpenForChanges)
        {
            ret.AddError("", "Dokuments nevar būt iegrāmatots.");
            return ret;
        }
        if (dr_doc.XDocType != EDocType.Atgriezts_piegādātājam)
        {
            ret.AddError("", "Dokumenta veidam jābūt Atgriezts piegādātājam.");
            return ret;
        }

        var new_doctype = EDocType.Kredītrēķins_no_piegādātāja;

        var table_docs = MyData.DbContextM.BL_M_DOCS;
        var table_rows = MyData.DbContextM.BL_M_ROWS;
        var drs_rows = dr_doc.Rows
            .OrderBy(x => x.IDSEQ)
            .ToList();

        if(drs_rows.Count == 0)
        {
            ret.AddError("", "Dokumentam nav izveidotas rindas.");
            return ret;
        }

        var table_usedrows = MyData.DbContextM.Database
            .SqlQuery<SP_M_CHECK_RET_23B>($"SELECT * FROM SP_M_CHECK_RET_23B({dr_doc.ID})")
            .ToList();

        if (table_usedrows.Count == 0)
        {
            ret.AddError("", "Nav atlikumu atgiešanai.");
            return ret;
        }

        foreach (var dr_row in drs_rows)
        {
            var drs_usage = table_usedrows.Where(x => x.IDROW == dr_row.ID).ToList();
            var tamount = drs_usage.Count > 0 ? drs_usage.Sum(x => x.AMOUNT) : 0M;
            if (tamount != dr_row.AMOUNT)
            {
                ret.AddItemError(dr_row.IDITEM, "Nav atlikuma atgiešanai.");
            }
        }

        if (ret.HasErrors) return ret;

        var grps_cred = table_usedrows
            .GroupBy(x => x.CREDDOCID)
            .ToList();

        foreach(var grp_cred in grps_cred)
        {
            var new_dr_doc = table_docs.NewItem();
            var grp_cred_r1 = grp_cred.First();
            new_dr_doc.DT = DateTime.Today;
            new_dr_doc.XDocType = new_doctype;
            new_dr_doc.XDocType2 = dr_doc.XDocType2;
            new_dr_doc.XState = EDocState.Melnraksts;
            new_dr_doc.SR = "?";
            new_dr_doc.NR = "?";
            new_dr_doc.IDSTOREIN = dr_doc.IDSTOREIN;
            new_dr_doc.IDSTOREOUT = dr_doc.IDSTOREOUT;
            new_dr_doc.PVNTYPE = dr_doc.PVNTYPE;
            new_dr_doc.ACCIN = dr_doc.ACCIN;
            new_dr_doc.ACCOUT = dr_doc.ACCOUT;
            //new_dr_doc.IDSEQ = GetNextIdSeq();
            new_dr_doc.IDCREDDOC = grp_cred_r1.CREDDOCID;
            new_dr_doc.CREDDOCDT = grp_cred_r1.CREDDOCDT;
            new_dr_doc.CREDDOCSR = grp_cred_r1.CREDDOCSR;
            new_dr_doc.CREDDOCNR = grp_cred_r1.CREDDOCNR;

            new_dr_doc.DocType = dr_doc.DocType;
            new_dr_doc.TransactionType = dr_doc.TransactionType;
            new_dr_doc.StoreIn = dr_doc.StoreIn;
            new_dr_doc.StoreOut = dr_doc.StoreOut;

            table_docs.Add(new_dr_doc);

            foreach (var cred_row in grp_cred)
            {
                var cdr_row = drs_rows.Where(x => x.ID == cred_row.IDROW).First();
                var new_dr_row = table_rows.NewItem();
                new_dr_row.IDDOC = new_dr_doc.ID;
                new_dr_row.IDCREDROW = cred_row.CREDROWID;
                new_dr_row.IDITEM = cdr_row.IDITEM;
                new_dr_row.IDPVNRATE = cdr_row.IDPVNRATE;
                new_dr_row.UNITS = cdr_row.UNITS;
                //new_dr_row.IDSEQ = cdr_row.IDSEQ;
                new_dr_row.AMOUNT = cred_row.AMOUNT;
                new_dr_row.PRICE0 = cred_row.BUYPRICE;
                new_dr_row.PRICE = cred_row.BUYPRICE;
                new_dr_row.TPRICE = cred_row.TBUYPRICE;
                new_dr_row.BUYPRICE = cred_row.BUYPRICE;
                new_dr_row.TBUYPRICE = cred_row.TBUYPRICE;
                new_dr_row.ACC6 = cdr_row.ACC6;
                new_dr_row.ACC7 = cdr_row.ACC7;

                new_dr_row.Doc = new_dr_doc;
                new_dr_row.Item = cdr_row.Item;
                new_dr_row.UnitsRow = cdr_row.UnitsRow;
                new_dr_row.PVNRate = cdr_row.PVNRate;

                table_rows.Add(new_dr_row);
            }

            UpdateIdSeq(new_dr_doc);
        }

        return ret;
    }

    public static ErrorList MakeCreditDocsFromAtgrieztsNoPirceja(M_DOCS dr_doc)
    {
        var ret = new ErrorList();
        if (!dr_doc.IsOpenForChanges)
        {
            ret.AddError("", "Dokuments nevar būt iegrāmatots.");
            return ret;
        }
        if (dr_doc.XDocType != EDocType.Atgriezts_no_pircēja)
        {
            ret.AddError("", "Dokumenta veidam jābūt Atgriezts no pircēja.");
            return ret;
        }

        var new_doctype = EDocType.Kredītrēķins_pircējam;

        var table_docs = MyData.DbContextM.BL_M_DOCS;
        var table_rows = MyData.DbContextM.BL_M_ROWS;
        var drs_rows = dr_doc.Rows
            .OrderBy(x => x.IDSEQ)
            .ToList();

        if (drs_rows.Count == 0)
        {
            ret.AddError("", "Dokumentam nav izveidotas rindas.");
            return ret;
        }

        var table_usedrows = MyData.DbContextM.Database
            .SqlQuery<SP_M_MAKELINKS_32B>($"SELECT * FROM SP_M_MAKELINKS_32B({dr_doc.ID})")
            .ToList();


        if (table_usedrows.Count == 0)
        {
            ret.AddError("", "Nav atlikumu atgiešanai.");
            return ret;
        }

        foreach (var dr_row in drs_rows)
        {
            var drs_usage = table_usedrows.Where(x => x.IDROW == dr_row.ID).ToList();
            var tamount = drs_usage.Count > 0 ? drs_usage.Sum(x => x.AMOUNT) : 0M;
            if (tamount != -dr_row.AMOUNT)
            {
                ret.AddItemError(dr_row.IDITEM, "Nav atlikuma atgiešanai.");
            }
        }

        if (ret.HasErrors) return ret;

        var grps_cred = table_usedrows
            .GroupBy(x => x.CREDDOCID)
            .ToList();

        foreach (var grp_cred in grps_cred)
        {
            var new_dr_doc = table_docs.NewItem();
            var grp_cred_r1 = grp_cred.First();
            new_dr_doc.DT = DateTime.Today;
            new_dr_doc.XDocType = new_doctype;
            new_dr_doc.XDocType2 = dr_doc.XDocType2;
            new_dr_doc.XState = EDocState.Melnraksts;
            new_dr_doc.SR = "?";
            new_dr_doc.NR = "?";
            new_dr_doc.IDSTOREIN = dr_doc.IDSTOREIN;
            new_dr_doc.IDSTOREOUT = dr_doc.IDSTOREOUT;
            new_dr_doc.PVNTYPE = dr_doc.PVNTYPE;
            new_dr_doc.ACCIN = dr_doc.ACCIN;
            new_dr_doc.ACCOUT = dr_doc.ACCOUT;
            new_dr_doc.IDSEQ = GetNextIdSeq();
            new_dr_doc.IDCREDDOC = grp_cred_r1.CREDDOCID;
            new_dr_doc.CREDDOCDT = grp_cred_r1.CREDDOCDT;
            new_dr_doc.CREDDOCSR = grp_cred_r1.CREDDOCSR;
            new_dr_doc.CREDDOCNR = grp_cred_r1.CREDDOCNR;

            new_dr_doc.DocType = MyData.DbContextM.M_DOCTYPES.Find((int)new_doctype);
            new_dr_doc.TransactionType = dr_doc.TransactionType;
            new_dr_doc.StoreIn = dr_doc.StoreIn;
            new_dr_doc.StoreOut = dr_doc.StoreOut;

            table_docs.Add(new_dr_doc);

            foreach (var cred_row in grp_cred)
            {
                var cdr_row = drs_rows.Where(x => x.ID == cred_row.IDROW).First();
                var new_dr_row = table_rows.NewItem();
                new_dr_row.IDDOC = new_dr_doc.ID;
                new_dr_row.IDCREDROW = cred_row.CREDROWID;
                new_dr_row.IDITEM = cdr_row.IDITEM;
                new_dr_row.IDPVNRATE = cdr_row.IDPVNRATE;
                new_dr_row.UNITS = cdr_row.UNITS;
                //new_dr_row.IDSEQ = cdr_row.IDSEQ;
                new_dr_row.AMOUNT = -cred_row.AMOUNT;
                new_dr_row.PRICE0 = cred_row.PRICE;
                new_dr_row.PRICE = cred_row.PRICE;
                new_dr_row.TPRICE = -cred_row.TPRICE;
                new_dr_row.BUYPRICE = cred_row.PRICE;
                new_dr_row.TBUYPRICE = cred_row.TPRICE;
                new_dr_row.ACC6 = cdr_row.ACC6;
                new_dr_row.ACC7 = cdr_row.ACC7;

                new_dr_row.Doc = new_dr_doc;
                new_dr_row.Item = cdr_row.Item;
                new_dr_row.PVNRate = cdr_row.PVNRate;
                new_dr_row.UnitsRow = cdr_row.UnitsRow;

                table_rows.Add(new_dr_row);
            }

            UpdateIdSeq(new_dr_doc);
        }

        return ret;
    }

    public static void DeleteLocalFinDocByIdDocM(int iddocm)
    {
        var table_fdocs = MyData.DbContextF.F_OPSD.Local.ToList();
        var drs_doc = table_fdocs
            .Where(x => x.IDDOCM == iddocm)
            .ToList();
        if (drs_doc.Count == 0) return;
        foreach (var dr_doc in drs_doc)
        {
            MyData.DbContextF.DeleteItem(dr_doc);
        }
    }

    public static void DetachFinDocByIdDocM(int iddocm)
    {
        var table_fdocs = MyData.DbContextF.F_OPSD;
        var table_frows = MyData.DbContextF.F_OPS;
        var drs_doc = table_fdocs
            .Where(x => x.IDDOCM != null && x.IDDOCM == iddocm)
            .ToList();
        if (drs_doc.Count == 0) return;
        foreach (var dr_doc in drs_doc)
        {
            var drs_row = dr_doc.OPSRows.ToList();
            MyData.DbContextF.RemoveFromContextRange(drs_row);
            MyData.DbContextF.RemoveFromContext(dr_doc);
        }
    }


    public static int RowCountForFullRecalcEvents = -1;

    public static string FullRecalc()
    {
        try
        {
            return FullRecalcA();
        }
        catch (Exception ex)
        {
            return GetErrorInfoFromMessage(ex.Message);
        }
    }

    public static string RecalcAmounts()
    {
        try
        {
            return RecalcAmountsA();
        }
        catch (Exception ex)
        {
            return GetErrorInfoFromMessage(ex.Message);
        }
    }

    public static string RecalcIsGone(bool promptforconfirm)
    {
        try
        {
            return RecalcIsGoneA(promptforconfirm);
        }
        catch (Exception ex)
        {
            return GetErrorInfoFromMessage(ex.Message);
        }
    }

    static FbCommand Create_CM_SP_M_RECALCITEM_01A()
    {
        var cm = new FbCommand();
        cm.CommandText = "\"SP_M_RECALCITEM_01A\"";
        cm.CommandType = CommandType.StoredProcedure;
        var param = new FbParameter();
        param.ParameterName = "PSTARTDATE";
        param.DbType = DbType.DateTime;
        param.Size = 4;
        param.IsNullable = true;
        cm.Parameters.Add(param);
        param = new FbParameter();
        param.ParameterName = "PPROGRESSTEP";
        param.DbType = DbType.Int32;
        param.Size = 4;
        param.IsNullable = true;
        cm.Parameters.Add(param);
        return cm;
    }

    static FbCommand Create_CM_SP_M_RECALCITEM_01C()
    {
        var cm = new FbCommand();
        cm.CommandText = "\"SP_M_RECALCITEM_01C\"";
        cm.CommandType = CommandType.StoredProcedure;
        var param = new FbParameter();
        param.ParameterName = "PIDDOC";
        param.DbType = DbType.Int32;
        param.Size = 4;
        param.IsNullable = true;
        cm.Parameters.Add(param);
        param = new FbParameter();
        param.ParameterName = "PNEWDOCSTATE";
        param.DbType = DbType.Int32;
        param.Size = 4;
        param.IsNullable = true;
        cm.Parameters.Add(param);
        param.ParameterName = "PPROGRESSTEP";
        param.DbType = DbType.Int32;
        param.Size = 4;
        param.IsNullable = true;
        cm.Parameters.Add(param);
        return cm;
    }

    static int SP_M_GETCOUNTSFOREVENTS(int PTP, int PID)
    {
        int ret = MyData.DbContextM.Database
            .SqlQuery<int>($"SELECT RCOUNT FROM SP_M_GETCOUNTSFOREVENTS({PTP}, {PID})")
            .Single();
        return ret;
    }

    public static string FullRecalcA()
    {
        int progressstep = 0;
        if (RowCountForFullRecalcEvents == -1)
            RowCountForFullRecalcEvents = SP_M_GETCOUNTSFOREVENTS(1, 0);
        bool hasevents = RowCountForFullRecalcEvents > 50000;
        if (hasevents)
            progressstep = 15000;

        //Func<object> func = () => MyData.KlonsMQueriesTableAdapter.SP_M_RECALCITEM_01A(null, progressstep);

        var cm = Create_CM_SP_M_RECALCITEM_01A();
        cm.Parameters[0].Value = DBNull.Value;
        cm.Parameters[1].Value = progressstep;

        var frm = new FormsM.FormM_SPRunner();
        frm.SPCommand = cm;
        //frm.TaskFunc = func;
        frm.TaskText = "Pilns pārrēķins.";
        frm.TextDoneText = "Pārrēķins pabeigts.  (izpildes laiks: {0})";
        frm.CanCancel = true;
        frm.HasProgressEvents = hasevents;
        frm.MaxProgressValue = RowCountForFullRecalcEvents;
        frm.ProgressEventStep = 15000;
        
        var rt = frm.ShowDialog(MyData.MyMainForm);

        if (rt == System.Windows.Forms.DialogResult.Abort)
            return "Aborted";
        if (rt == System.Windows.Forms.DialogResult.Cancel)
            return "Canceled";
        DataLoaderM.LoadLatestItemsData();
        return "OK";
    }

    public static string FullRecalcDocA(int iddoc, EDocState newstate)
    {
        int progressstep = 0;
        if (RowCountForFullRecalcEvents == -1)
            RowCountForFullRecalcEvents = SP_M_GETCOUNTSFOREVENTS(1, 0);
        bool hasevents = RowCountForFullRecalcEvents > 50000;
        if (hasevents)
        {
            progressstep = 15000;
            int maxcount = SP_M_GETCOUNTSFOREVENTS(2, iddoc);
            hasevents = maxcount > 50000;
        }

        //Func<object> func = () => MyData.KlonsMQueriesTableAdapter.SP_M_RECALCITEM_01C(iddoc, (int)newstate, progressstep);

        var cm = Create_CM_SP_M_RECALCITEM_01C();
        cm.Parameters[0].Value = iddoc;
        cm.Parameters[2].Value = (int)newstate;
        cm.Parameters[3].Value = progressstep;

        var frm = new FormsM.FormM_SPRunner();
        frm.SPCommand = cm;
        //frm.TaskFunc = func;
        frm.TaskText = "Pilns dokumenta pārrēķins.";
        frm.TextDoneText = "Pārrēķins pabeigts.  (zpildes laiks: {0})";
        frm.CanCancel = true;
        frm.HasProgressEvents = hasevents;
        frm.MaxProgressValue = RowCountForFullRecalcEvents;
        frm.ProgressEventStep = 15000;

        var rt = frm.ShowDialog(MyData.MyMainForm);

        if (rt == System.Windows.Forms.DialogResult.Abort)
            return "Aborted";
        if (rt == System.Windows.Forms.DialogResult.Cancel)
            return "Canceled";
        DataLoaderM.LoadLatestItemsData();
        return "OK";
    }

    static int SP_M_RECALCAMOUNTS_1()
    {
        int ret = MyData.DbContextM.Database
            .SqlQuery<int>($"SELECT RET FROM SP_M_RECALCAMOUNTS_1")
            .Single();
        return ret;
    }

    public static string RecalcAmountsA()
    {
        Func<object> func = () => SP_M_RECALCAMOUNTS_1();
        var frm = new FormsM.FormM_SPRunner();
        frm.TaskText = "Atlikumu pārrēķins.";
        frm.TextDoneText = "Atlikumu pārrēķins pabeigts.";
        frm.CanCancel = false;
        frm.TaskFunc = func;
        frm.HasProgressEvents = false;
        var rt = frm.ShowDialog(MyData.MyMainForm);
        if (rt == System.Windows.Forms.DialogResult.Abort)
            return "Aborted";
        if (rt == System.Windows.Forms.DialogResult.Cancel)
            return "Canceled";
        DataLoaderM.LoadLatestItemsData();
        return "OK";
    }

    static int SP_M_RECALC_ROWSISGONE_1()
    {
        int ret = MyData.DbContextM.Database
            .SqlQuery<int>($"SELECT RET FROM SP_M_RECALC_ROWSISGONE_1")
            .Single();
        return ret;
    }

    public static string RecalcIsGoneA(bool promptforconfirm)
    {
        Func<object> func = () => SP_M_RECALC_ROWSISGONE_1();
        var frm = new FormsM.FormM_SPRunner();
        frm.TaskText = "Izlietojuma pazīmes pārrēķins.";
        frm.TextDoneText = "Pārrēķins pabeigts.";
        frm.CanCancel = false;
        frm.TaskFunc = func;
        frm.HasProgressEvents = false;
        if (!promptforconfirm)
        {
            frm.AutoStart = true;
            frm.CloseOnDone = true;
        }
        var rt = frm.ShowDialog(MyData.MyMainForm);
        if (rt == System.Windows.Forms.DialogResult.Abort)
            return "Aborted";
        if (rt == System.Windows.Forms.DialogResult.Cancel)
            return "Canceled";
        return "OK";
    }


}
