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
using Klons3.ModelsM;

namespace KlonsM.FormsM
{
    public partial class FormM_Params : MyFormBaseF
    {
        public FormM_Params()
        {
            InitializeComponent();
            CheckMyFontAndColors();

            LoadParams();

            myGrid1.MakeGrid2();
            myGrid1.LinkGrid();
            myGrid1.Refresh();

            grMainStore.ButtonClicked += GrMainStore_ButtonClicked;
        }

        private void FormM_Params_Load(object sender, EventArgs e)
        {

        }

        public void LoadParams()
        {
            paramsMData1._MainnStoreCode = MyData.Params.MAINSTORE;
            paramsMData1._DecimalsInPrices = MyData.Params.DECIMALSINPRICES;
            paramsMData1._CheckIsGoneOnSturtUp = MyData.Params.CHECKISGONE;
        }

        public void SaveParamsA()
        {
            MyData.Params.MAINSTORE = paramsMData1._MainnStoreCode;
            MyData.Params.DECIMALSINPRICES = paramsMData1._DecimalsInPrices;
            MyData.Params.CHECKISGONE = paramsMData1._CheckIsGoneOnSturtUp;
            MyData.Params.Save();
        }

        private void paramsMData1_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "_DecimalsInPrices")
            {
                int k = paramsMData1._DecimalsInPrices;
                k = k > 6 ? 6 : k;
                k = k < 2 ? 2 : k;
                if (k == paramsMData1._DecimalsInPrices) return;
                paramsMData1._DecimalsInPrices = k;
            }
        }

        private void GrMainStore_ButtonClicked(object sender, EventArgs e)
        {
            var dr_store = (M_STORES)grMainStore.Value;
            dr_store = FormM_Stores.GetStore(dr_store, EStoreType.Noliktava);
            if (dr_store == null) return;
            grMainStore.Value = dr_store;
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            SaveParamsA();
            DialogResult = DialogResult.OK;
        }

    }
}
