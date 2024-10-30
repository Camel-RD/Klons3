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
    public partial class FormM_ItemCurrentStock : MyFormBaseF
    {
        public FormM_ItemCurrentStock()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            dgvRows.AutoGenerateColumns = false;
        }

        private void FormM_ItemCurrentStock_Load(object sender, EventArgs e)
        {
        }

        public void MakeReport(int iditem)
        {
            lbItemName.Text = DataTasksM.GetItemCodeAndName(iditem);
            var table_rows = DataLoaderM.GetBy_SP_M_CURRENTSTOCK_01(iditem);
            var table_stores = MyData.DbContextM.BL_M_STORES;
            if (table_rows.Count == 0) return;
            var rep_rows = table_rows.Select(x =>
            new ItemCurrentStockRow()
            {
                StoreCode = table_stores.FindById(x.IDSTORE).CODE,
                StoreName = table_stores.FindById(x.IDSTORE).NAME,
                Amount = x.AMOUNT
            })
            .OrderBy(x => x.StoreCode)
            .ToList();
            rep_rows.Add(new ItemCurrentStockRow()
            {
                StoreCode = "KOPĀ",
                Amount = rep_rows.Sum(x => x.Amount)
            });
            dgvRows.DataSource = rep_rows;
        }

        public class ItemCurrentStockRow
        {
            public string StoreCode { get; set; }
            public string StoreName { get; set; }
            public decimal Amount { get; set; }
        }
    }
}
