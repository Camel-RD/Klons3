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
using KlonsLIB.Components;
using Klons3.ModelsM;

namespace KlonsM.FormsM
{
    public partial class FormM_StoreCurrentStock : MyFormBaseF
    {
        public FormM_StoreCurrentStock()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            dgvRows.AutoGenerateColumns = false;
        }

        private void FormM_StoreCurrentStock_Load(object sender, EventArgs e)
        {

        }

        List<StoreCurrentStockRow> ReportRows = new List<StoreCurrentStockRow>();


        public void MakeReport(int idstore)
        {
            var rep_rows = new List<StoreCurrentStockRow>();
            ReportRows = new List<StoreCurrentStockRow>();
            dgvRows.DataSource = ReportRows;
            lbStoreName.Text = DataTasksM.GetStoreCodeAndName(idstore);
            var table_rows = DataLoaderM.GetBy_SP_M_CURRENTSTOCK_02(idstore);
            var table_items = MyData.DbContextM.BL_M_ITEMS;
            if (table_rows.Count == 0) return;
            foreach(var dr in table_rows)
            {
                var dr_item = table_items.FindById(dr.IDITEM);
                var rep_row = new StoreCurrentStockRow()
                {
                    ItemCode = dr_item.BARCODE,
                    ItemName = dr_item.NAME,
                    ItemCategory = dr_item.Category.CODE,
                    Amount = dr.AMOUNT
                };
                rep_rows.Add(rep_row);
            }
            rep_rows = rep_rows
                .OrderBy(x => x.ItemCategory)
                .ThenBy(x => x.ItemName)
                .ToList();
            ReportRows = rep_rows;
            dgvRows.DataSource = rep_rows;
        }

        public void DoFilter()
        {
            var fs = tbFilter.Text.ToLower().Zn();
            int kcat = tbItemsCatFilter.SelectedIndex;
            if (fs == null && kcat == -1)
            {
                dgvRows.DataSource = ReportRows;
                return;
            }
            string cat_code_filter = null;
            if(kcat > -1)
            {
                var dr_itemscat = bsItemsCatFilter.GetItem<M_ITEMS_CAT>(kcat);
                cat_code_filter = dr_itemscat?.CODE;
            }

            var rep_rows = ReportRows.Cast<StoreCurrentStockRow>();
            if (fs != null)
            {
                rep_rows = rep_rows
                    .Where(x => x.ItemCode.Contains(fs) || x.ItemName.ToLower().Contains(fs));
            }
            if (cat_code_filter != null)
            {
                rep_rows = rep_rows
                    .Where(x => x.ItemCategory.StartsWith(cat_code_filter));
            }
            dgvRows.DataSource = rep_rows.ToList();
        }

        public class StoreCurrentStockRow
        {
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public string ItemCategory { get; set; }
            public decimal Amount { get; set; }
        }

        private void tbFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                tbFilter.Text = "";
                DoFilter();
                return;
            }
            if (e.KeyCode == Keys.Return)
            {
                DoFilter();
                return;
            }
        }

        private void tbItemsCatFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            DoFilter();
        }

        private void tbItemsCatFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                tbItemsCatFilter.Text = null;
                DoFilter();
                e.Handled = true;
            }
        }

        private void tbItemsCatFilter_ButtonClicked(object sender, EventArgs e)
        {
            var dr_selected = (M_ITEMS_CAT)tbItemsCatFilter.SelectedValue;
            var dr_cat = FormM_ItemsCat.GetItemsCat(dr_selected);
            if (dr_cat == null) return;
            tbItemsCatFilter.SelectedValue = dr_cat;
        }
    }
}
