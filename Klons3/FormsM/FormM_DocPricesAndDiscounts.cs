using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KlonsM.Classes;
using KlonsLIB.Forms;
using KlonsLIB.Data;
using KlonsLIB.Misc;
using KlonsF.Classes;
using Klons3.ModelsM;

namespace KlonsM.FormsM
{
    public partial class FormM_DocPricesAndDiscounts : MyFormBaseF
    {
        public FormM_DocPricesAndDiscounts()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            dgvRows.AutoGenerateColumns = false;

            HeaderCellStyle = dgvRows.DefaultCellStyle.Clone();
            HeaderCellStyle.Font = new Font(HeaderCellStyle.Font, HeaderCellStyle.Font.Style | FontStyle.Bold);
        }

        private void FormM_DocPricesAndDiscounts_Load(object sender, EventArgs e)
        {
            GetData(DrDoc);
        }

        private void FormM_DocPricesAndDiscounts_Shown(object sender, EventArgs e)
        {
            if (ErrorList.HasErrors)
            {
                FormM_ErrorList.ShowErrorList(this, ErrorList);
                return;
            }
        }

        public static void ApplyPricesAndDiscounts(M_DOCS dr_doc)
        {
            var fm = new FormM_DocPricesAndDiscounts();
            fm.DrDoc = dr_doc;
            fm.ShowDialog();
        }

        public M_DOCS DrDoc = null;

        public BindingList<AppliedListsRow> ListsAll = new BindingList<AppliedListsRow>();
        public List<AppliedListsRow> ListsPrices = new List<AppliedListsRow>();
        public List<AppliedListsRow> ListsDiscounts = new List<AppliedListsRow>();
        public ErrorList ErrorList = new ErrorList();

        public DataGridViewCellStyle HeaderCellStyle = null;

        public void GetData(M_DOCS dr_doc)
        {
            DrDoc = dr_doc;
            ListsAll = new BindingList<AppliedListsRow>();
            ListsPrices = new List<AppliedListsRow>();
            ListsDiscounts = new List<AppliedListsRow>();
            ErrorList = new ErrorList();

            var table_disclists = MyData.DbContextM.BL_M_DISC_LISTS;
            var table_pricelists = MyData.DbContextM.BL_M_PRICE_LISTS;
            var table_storescat = MyData.DbContextM.BL_M_STORES_CAT;
            var table_itemscat = MyData.DbContextM.BL_M_ITEMS_CAT;
            var drs_rows = DrDoc.Rows.ToList();
            var dr_store = DrDoc.StoreIn;
            string code_storecat = DrDoc.StoreIn.Category.CODE;

            bool StoreCatStartsWith(M_STORES_CAT dr_cat)
            {
                var code = dr_cat?.CODE;
                if (code.IsNOE()) return false;
                return code_storecat.StartsWith(code, StringComparison.CurrentCulture);
            }

            List<M_ROWS> MatchingDiscountList(M_DISC_LISTS_R dr_disc_r)
            {
                var ret = new List<M_ROWS>();
                string disccatcode = null;
                if (dr_disc_r.ItemsCategory != null)
                    disccatcode = dr_disc_r.ItemsCategory.CODE;
                if (dr_disc_r.Item != null)
                {
                    var drs1 = drs_rows
                        .Where(x => x.Item == dr_disc_r.Item);
                    ret.AddRange(drs1);
                }
                if (!disccatcode.IsNOE())
                {
                    var drs1 = drs_rows
                        .Where(x =>
                        !x.Item.Category.CODE.IsNOE() &&
                        x.Item.Category.CODE.StartsWith(disccatcode));
                    ret.AddRange(drs1);
                }
                ret = ret.Distinct().ToList();
                return ret;
            }

            var drs_check_discount_lists = table_disclists
                .SelectMany(x => x.ForStores)
                .Where(x =>
                    x.Store != null && x.Store == dr_store ||
                    x.StoresCategory != null && StoreCatStartsWith(x.StoresCategory))
                .Select(x => x.DiscountList)
                .Distinct()
                .ToList();

            var drs_check_price_lists = table_pricelists
                .SelectMany(x => x.ForStores)
                .Where(x =>
                    x.Store != null && x.Store == dr_store ||
                    x.StoresCategory != null && StoreCatStartsWith(x.StoresCategory))
                .Select(x => x.PriceList)
                .Distinct()
                .ToList();

            var drs_applicable_disccount_rows = drs_check_discount_lists
                .SelectMany(x => x.ForItems)
                .Select(x => (dr_disc_r: x, drs_rows: MatchingDiscountList(x)))
                .Where(x => x.drs_rows.Count > 0)
                .ToList();

            var drs_applicable_price_rows = drs_check_price_lists
                .SelectMany(x => x.ForItems)
                .Select(x => (dr_price_r: x, drs_rows: drs_rows.Where(y => y.Item == x.Item).ToList()))
                .Where(x => x.drs_rows.Count > 0)
                .ToList();

            var drs_with_multiple_discounts = drs_applicable_disccount_rows
                .SelectMany(x => x.drs_rows.Select(y => (x, y)))
                .GroupBy(x => x.y)
                .Where(x => x.Count() > 1)
                .ToList();

            var drs_with_multiple_prices = drs_applicable_price_rows
                .SelectMany(x => x.drs_rows.Select(y => (x, y)))
                .GroupBy(x => x.y)
                .Where(x => x.Count() > 1)
                .ToList();


            foreach (var dr in drs_applicable_disccount_rows)
            {
                var list_row = new AppliedListsRow()
                {
                    Type = EAppliedListsRowType.DiscountList,
                    ListName = dr.dr_disc_r.DiscountList.NAME,
                    Rows = dr.drs_rows,
                    IdList = dr.dr_disc_r.IDL,
                    RowDiscount = dr.dr_disc_r
                };
                ListsDiscounts.Add(list_row);
            }

            foreach (var dr in drs_applicable_price_rows)
            {
                var list_row = new AppliedListsRow()
                {
                    Type = EAppliedListsRowType.PriceList,
                    ListName = dr.dr_price_r.PriceList.NAME,
                    Rows = dr.drs_rows,
                    IdList = dr.dr_price_r.IDL,
                    RowPrice = dr.dr_price_r
                };
                ListsPrices.Add(list_row);
            }

            if (ListsPrices.Count > 0)
            {
                var list_row_h = new AppliedListsRow()
                {
                    Type = EAppliedListsRowType.PriceListHeader,
                    ListName = "Cenu lapas"
                };
                ListsAll.Add(list_row_h);
                ListsPrices.ForEach(x => ListsAll.Add(x));
            }

            if (ListsDiscounts.Count > 0)
            {
                var list_row_h = new AppliedListsRow()
                {
                    Type = EAppliedListsRowType.DiscountListHeader,
                    ListName = "Atlaižu lapas"
                };
                ListsAll.Add(list_row_h);
                ListsDiscounts.ForEach(x => ListsAll.Add(x));
            }

            drs_with_multiple_prices
                .Select(x => (x.Key.Item.BARCODE, string.Join(", ", x)))
                .ForEach(x => ErrorList.AddError(x.BARCODE, "Artikuls ir vairākās cenu lapās: " + x.Item2));

            drs_with_multiple_discounts
                .Select(x => (x.Key.Item.BARCODE, string.Join(", ", x)))
                .ForEach(x => ErrorList.AddError(x.BARCODE, "Artikuls ir vairākās atlaižu lapās: " + x.Item2));

            dgvRows.DataSource = ListsAll;
        }

        public void OnHeaderClick(AppliedListsRow row)
        {
            var tp = EAppliedListsRowType.None;
            if (row.Type == EAppliedListsRowType.DiscountListHeader)
                tp = EAppliedListsRowType.DiscountList;
            else if (row.Type == EAppliedListsRowType.PriceListHeader)
                tp = EAppliedListsRowType.PriceList;
            if (tp == EAppliedListsRowType.None) return;
            foreach (var list in ListsAll)
            {
                if (list.Type == tp)
                    list.IsUsed = row.IsUsed;
            }
        }

        public void ApplyPriceLists()
        {
            var lists_prices = ListsPrices.Where(x => x.IsUsed).ToList();
            if (lists_prices.Count == 0) return;
            var row_x_price = lists_prices
                .SelectMany(x => x.Rows.Select(y => (x.RowPrice, y)))
                .GroupBy(x => x.y)
                .Select(x => (dr_docrow: x.Key, x.WithMinOrDefault(y => y.RowPrice.PRICE).RowPrice))
                .ToList();
            foreach (var row in row_x_price)
            {
                row.dr_docrow.PRICE0 = row.RowPrice.PRICE;
            }
        }

        public void ApplyDiscountLists()
        {
            var lists_discounts = ListsDiscounts.Where(x => x.IsUsed).ToList();
            if (lists_discounts.Count == 0) return;
            var row_x_discount = lists_discounts
                .SelectMany(x => x.Rows.Select(y => (x.RowDiscount, y)))
                .GroupBy(x => x.y)
                .Select(x => (dr_docrow: x.Key, x.WithMaxOrDefault(y => y.RowDiscount.DISCOUNT).RowDiscount))
                .ToList();
            foreach (var row in row_x_discount)
            {
                row.dr_docrow.DISCOUNT = row.RowDiscount.DISCOUNT;
            }
        }

        public void CheckLists()
        {
            ErrorList.Clear();

            var lists_prices = ListsPrices.Where(x => x.IsUsed).ToList();
            var lists_discounts = ListsDiscounts.Where(x => x.IsUsed).ToList();
            if (lists_prices.Count == 0 && lists_discounts.Count == 0) return;

            var row_x_price = lists_prices
                .SelectMany(x => x.Rows.Select(y => (x.RowPrice, y)))
                .GroupBy(x => x.y);

            var row_x_discount = lists_discounts
                .SelectMany(x => x.Rows.Select(y => (x.RowDiscount, y)))
                .GroupBy(x => x.y);

            foreach (var gr in row_x_price)
            {
                var msg = string.Join(", ", gr.Select(x => x.RowPrice.PriceList.NAME));
                msg = "Artikuls ir vairākās cenu lapās: " + msg;
                var code = gr.Key.Item.BARCODE;
                ErrorList.AddError(code, msg);
            }

            foreach (var gr in row_x_discount)
            {
                var msg = string.Join(", ", gr.Select(x => x.RowDiscount.DiscountList.NAME));
                msg = "Artikuls ir vairākās atlaižu lapās: " + msg;
                var code = gr.Key.Item.BARCODE;
                ErrorList.AddError(code, msg);
            }
        }

        public enum EAppliedListsRowType { None, PriceList, DiscountList, PriceListHeader, DiscountListHeader }
        public class AppliedListsRow : INotifyPropertyChanged
        {
            private bool _IsUsed = true;
            public FormM_DocPricesAndDiscounts Form = null;
            public EAppliedListsRowType Type { get; set; } = EAppliedListsRowType.None;

            public int IdList = 0;
            public M_PRICE_LISTS_R RowPrice = null;
            public M_DISC_LISTS_R RowDiscount = null;
            public string ListName { get; set; }
            public bool IsUsed
            {
                get => _IsUsed;
                set
                {
                    _IsUsed = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUsed)));
                    if (Type == EAppliedListsRowType.DiscountListHeader ||
                        Type == EAppliedListsRowType.PriceListHeader)
                    {
                        if (Form != null) Form.OnHeaderClick(this);
                    }
                }
            }

            public List<M_ROWS> Rows = null;

            public event PropertyChangedEventHandler PropertyChanged;

        }

        private void btApply_Click(object sender, EventArgs e)
        {
            ApplyPriceLists();
            ApplyDiscountLists();
        }

        private void btCheck_Click(object sender, EventArgs e)
        {
            CheckLists();
            if (!ErrorList.HasErrors)
            {
                MyMainForm.ShowInfo("Neviens artikuls neatrodas vairākās cenu un atlaižu lapās.", "");
                return;
            }
            FormM_ErrorList.ShowErrorList(this, ErrorList);
        }

        private void dgvRows_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1) return;
            var row = dgvRows.Rows[e.RowIndex].DataBoundItem as AppliedListsRow;
            if (row.Type == EAppliedListsRowType.DiscountListHeader ||
                row.Type == EAppliedListsRowType.PriceListHeader)
            {
                e.CellStyle = HeaderCellStyle;
                e.FormattingApplied = true;
            }
        }

    }

}
