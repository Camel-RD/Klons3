using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KlonsA.Classes;
using KlonsF.Classes;
using DataObjectsA;
using KlonsLIB.Data;
using KlonsLIB.Forms;
using KlonsLIB.Misc;
using Klons3.ModelsA;
using NPOI.Util;

namespace KlonsA.Forms
{
    public partial class FormA_Rates : MyFormBaseF
    {
        public FormA_Rates()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            MakeGrid();
        }

        private void Form_Rates_Load(object sender, EventArgs e)
        {
            SetupToolStrips();

            bsLikmes.Position = bsLikmes.Count - 1;
        }

        private void SetupToolStrips()
        {
            var k = bnavLikmes.Items.IndexOf(this.bindingNavigatorMoveNextItem);
            InsertInToolStrip(bnavLikmes, cbDates, k);
        }

        private void MakeGrid()
        {
            myGrid1.MakeGrid();
            myGrid1.LinkGrid();
            myGrid1.ArrangeLinkedControls();
        }

        private void CheckRatesRedRows()
        {
            var dr = bsLikmes.GetCurrentItem<A_RATES>();
            if (dr == null) return;
            int k = bsLikmes.Position;
            if (k == -1) return;
            if (k == 0)
            {
                myGrid1.ClearRed();
            }
            else
            {
                var dr_prev = bsLikmes.GetItem<A_RATES>(k - 1);
                if (dr_prev == null) return;
                if (dr_prev.ONDATE > dr.ONDATE) return;
                myGrid1.CheckRedRows([ dr_prev ], [ dr ], [ bsLikmes ]);
            }
        }

        private void CheckRatesRowRed(KlonsLIB.MySourceGrid.GridRows.MyGridRowPropEditorBase row)
        {
            if (row == null) return;
            var dr = bsLikmes.GetCurrentItem<A_RATES>();
            if (dr == null) return;
            if (bsLikmes.Position == 0) return;
            var dr_prev = bsLikmes.GetItem<A_RATES>(bsLikmes.Position - 1);
            if (dr_prev == null) return;
            if (dr_prev.ONDATE > dr.ONDATE) return;
            row.CheckRedRow([ dr_prev ], [ dr ], [ bsLikmes ]);
        }

        private void bsLikmes_CurrentChanged(object sender, EventArgs e)
        {
            CheckRatesRedRows();
        }

        private void myGrid1_EditEnded(object sender, EventArgs e)
        {
            var row = sender as KlonsLIB.MySourceGrid.GridRows.MyGridRowPropEditorBase;
            if (row == null) return;
            CheckRatesRowRed(row);
        }

        private void SetSaveButton(bool red)
        {
            bnavLikmes.SetSaveButton(tsbSave, red);
        }

        public override bool SaveData()
        {
            var ret = bsLikmes.SaveDbContext();
            CheckSave();
            return ret.IsSuccess();
        }

        private void CheckSave()
        {
            SetSaveButton(bsLikmes.HasChanges());
        }

        public void DeletCurrent()
        {
            if (bsLikmes.Count < 2 || bsLikmes.Current == null) return;
            if (!Validate()) return;
            var dr = bsLikmes.GetCurrentItem<A_RATES>();
            bsLikmes.Remove(dr);
            SaveData();
            CheckSave();
        }

        private void bsLikmes_ListChanged(object sender, ListChangedEventArgs e)
        {
            CheckSave();
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
            CheckSave();
        }

        private void bnavLikmes_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            A_RATES prev_dr = null;
            if (bsLikmes.Count > 0)
            {
                prev_dr = bsLikmes.GetItem<A_RATES>(bsLikmes.Count - 1);
            }

            var mew_dr = new A_RATES();
            mew_dr.ONDATE = DateTime.Today;

            if (prev_dr != null) return;
            {
                bsLikmes.GetMyDbContext().CopyEntry(prev_dr, mew_dr);
            }
            MyData.DbContextA.BL_A_RATES.Add(mew_dr);

            bsLikmes.Position = bsLikmes.Count - 1;

            SaveData();
            CheckSave();
        }

        private void cbDates_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.Value == null || !(e.Value is DateTime)) return;
            e.Value = Utils.DateToString((DateTime)e.Value);
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            DeletCurrent();
        }

    }
}
