﻿using System;
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
    public partial class FormM_ItemMovement : MyFormBaseF
    {
        public FormM_ItemMovement()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            LoadParams();
            dgvRows.AutoGenerateColumns = false;
        }

        private void FormM_ItemMovement_Load(object sender, EventArgs e)
        {

        }

        public DateTime Date1;
        public DateTime Date2;
        public int iditem;

        private void LoadParams()
        {
            tbDT1.Text = MyData.Params.RSD;
            tbDT2.Text = MyData.Params.RED;
        }

        public override void SaveParams()
        {
            MyData.Params.RSD = tbDT1.Text;
            MyData.Params.RED = tbDT2.Text;
        }

        public string CheckParams()
        {
            if (tbDT1.Text.IsNOE() || tbDT2.Text.IsNOE())
                return "Jānorāda sākuma un beigu datums.";
            if (tbCode.SelectedIndex == -1)
                return "Jānorāda artikuls";
            var dr_item = (M_ITEMS)tbCode.SelectedValue;
            iditem = dr_item?.ID ?? -1;
            if (!Utils.StringToDate(tbDT1.Text, out Date1) ||
                !Utils.StringToDate(tbDT2.Text, out Date2))
                return "Nekorekts datuma formāts.";
            if (Date1 > Date2)
                return "Sākuma datums lielāks par beigu datumu.";
            return "OK";
        }

        public void MakeReport()
        {
            dgvRows.DataSource = new List<ItemMovementRow>();
            var table_rows = DataLoaderM.GetBy_SP_M_ITEMMOVEMENT_01(iditem, Date1, Date2);
            var rep_rows = table_rows.Select(x => new ItemMovementRow(x)).ToList();
            var ids1 = rep_rows.Select(x => x.IdStoreOut).Distinct();
            var ids2 = rep_rows.Select(x => x.IdStoreIn).Distinct();
            var ids3 = ids1.Union(ids2)
                .Where(x => IsStoreTracked(x));
            var lookup = new Dictionary<int, decimal>();
            ids3.ForEach(x => lookup[x] = 0M);

            bool IsStoreTracked(int idstore)
            {
                var dr = KlonsData.St.DbContextM.M_STORES.Find(idstore);
                if (dr == null) return false;
                return dr.StoreType.TRACKSTOCK == 1;
            }

            decimal AddAmount(int idstore, decimal am)
            {
                if (idstore == -1) return 0M;
                if (!lookup.TryGetValue(idstore, out var am1)) return 0M;
                am1 += am;
                lookup[idstore] = am1;
                return am1;
            }

            foreach (var rep_row in rep_rows)
            {
                rep_row.Saldo1 = AddAmount(rep_row.IdStoreOut, -rep_row.Amount);
                rep_row.Saldo2 = AddAmount(rep_row.IdStoreIn, rep_row.Amount);
            }
            
            var saldo2 = lookup
                .Select(x => new ItemMovementRow()
                {
                    Gtp = 2,
                    IdStoreIn = x.Key,
                    IdStoreOut = -1,
                    CodeStoreIn = DataTasksM.GetStoreCode(x.Key),
                    Amount = x.Value
                })
                .OrderBy(x => x.CodeStoreIn)
                .ToList();

            rep_rows.AddRange(saldo2);
            rep_rows.ForEach(x => x.UpdateSGtp());
            dgvRows.DataSource = rep_rows;
        }

        public class ItemMovementRow
        {
            public int Gtp { get; set; }
            public string SGtp { get; set; }
            public DateTime? Dt { get; set; }
            public string SR { get; set; }
            public string NR { get; set; }
            public EDocType DocType { get; set; }
            public string SDocType { get; set; }
            public int IdStoreOut { get; set; } = -1;
            public int IdStoreIn { get; set; } = -1;
            public string CodeStoreOut { get; set; }
            public string CodeStoreIn { get; set; }
            public decimal Amount { get; set; } = 0M;
            public decimal Saldo1 { get; set; } = 0M;
            public decimal Saldo2 { get; set; } = 0M;

            public ItemMovementRow() { }

            public ItemMovementRow(SP_M_ITEMMOVEMENT_01 dr)
            {
                SetFrom(dr);
            }

            public void UpdateSGtp()
            {
                if (Gtp == 0) SGtp = "sākumā";
                else if (Gtp == 1) SGtp = SDocType;
                else if (Gtp == 2) SGtp = "beigās";
            }

            public void SetFrom(SP_M_ITEMMOVEMENT_01 dr)
            {
                Gtp = dr.GTP;
                Dt = dr.DT;
                SR = dr.SR;
                NR = dr.NR;
                if(dr.TP == -1)
                    dr.TP = (int)EDocType.Sākuma_atlikums;
                DocType = (EDocType)dr.TP;
                SDocType = KlonsData.St.DbContextM.M_DOCTYPES.Find(dr.TP).NAME;
                IdStoreOut = dr.IDSTOREOUT ?? -1;
                IdStoreIn = dr.IDSTOREIN ?? -1;
                if(IdStoreOut != -1)
                    CodeStoreOut = DataTasksM.GetStoreCode(IdStoreOut);
                if(IdStoreIn != -1)
                    CodeStoreIn = DataTasksM.GetStoreCode(IdStoreIn);
                Amount = dr.AMOUNT;
            }
        }

        private void cmGetData_Click(object sender, EventArgs e)
        {
            var rt = CheckParams();
            if(rt != "OK")
            {
                MyMainForm.ShowWarning(rt);
                return;
            }
            MakeReport();
        }

        private void tbCode_ButtonClicked(object sender, EventArgs e)
        {
            var dr_item = FormM_Items.GetItem(null);
            if (dr_item == null) return;
            tbCode.SelectedValue = dr_item;
        }

        private void tbCode_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                tbCode.SelectedIndex = -1;
                return;
            }
        }

        private void dgvRows_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.Value == DBNull.Value) return;

            if (e.ColumnIndex == dgcAmount.Index ||
                e.ColumnIndex == dgcSaldo1.Index ||
                e.ColumnIndex == dgcSaldo2.Index)
            {
                decimal val = (decimal)e.Value;
                e.Value = (val == 0M) ? "" : val.ToString();
                e.FormattingApplied = true;
            }
        }
    }
}
