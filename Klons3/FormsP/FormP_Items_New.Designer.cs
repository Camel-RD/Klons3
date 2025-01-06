namespace KlonsP.Forms
{
    partial class FormP_Items_New
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            label1 = new System.Windows.Forms.Label();
            tbRegNr = new KlonsLIB.Components.MyTextBox();
            bsItem = new KlonsLIB.Data.MyBindingSourceToObj(components);
            itemsEventsData1 = new DataObjectsP.ItemsEventsData();
            label2 = new System.Windows.Forms.Label();
            tbName = new KlonsLIB.Components.MyTextBox();
            label3 = new System.Windows.Forms.Label();
            tbDate = new KlonsLIB.Components.MyTextBox();
            chAddEvent = new KlonsLIB.Components.MyCheckBox();
            cbCat1 = new KlonsLIB.Components.MyMcFlatComboBox();
            bsCat1 = new KlonsLIB.Data.MyBindingSourceEf(components);
            cbCatD = new KlonsLIB.Components.MyMcFlatComboBox();
            bsCatD = new KlonsLIB.Data.MyBindingSourceEf(components);
            cbCatT = new KlonsLIB.Components.MyMcFlatComboBox();
            bsCatT = new KlonsLIB.Data.MyBindingSourceEf(components);
            cbDep = new KlonsLIB.Components.MyMcFlatComboBox();
            bsDep = new KlonsLIB.Data.MyBindingSourceEf(components);
            cbPlace = new KlonsLIB.Components.MyMcFlatComboBox();
            bsPlace = new KlonsLIB.Data.MyBindingSourceEf(components);
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            tbValue = new KlonsLIB.Components.MyTextBox();
            label10 = new System.Windows.Forms.Label();
            tbDocNr = new KlonsLIB.Components.MyTextBox();
            cmOK = new System.Windows.Forms.Button();
            cmCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)bsItem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsCat1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsCatD).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsCatT).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsDep).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsPlace).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(14, 11);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(59, 17);
            label1.TabIndex = 13;
            label1.Text = "Reģ.nr.:";
            // 
            // tbRegNr
            // 
            tbRegNr.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            tbRegNr.DataBindings.Add(new System.Windows.Forms.Binding("Text", bsItem, "fITEM_REG_NR", true));
            tbRegNr.Location = new System.Drawing.Point(152, 9);
            tbRegNr.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tbRegNr.MaxLength = 20;
            tbRegNr.Name = "tbRegNr";
            tbRegNr.Size = new System.Drawing.Size(90, 23);
            tbRegNr.TabIndex = 0;
            tbRegNr.KeyDown += control_KeyDown;
            // 
            // bsItem
            // 
            bsItem.MyDataSource = itemsEventsData1;
            bsItem.Position = 0;
            // 
            // itemsEventsData1
            // 
            itemsEventsData1.fCAT1 = 0;
            itemsEventsData1.fCATD = 0;
            itemsEventsData1.fCATT = 0;
            itemsEventsData1.fDEPARTMENT = 0;
            itemsEventsData1.fDEPREC_0 = new decimal(new int[] { 0, 0, 0, 65536 });
            itemsEventsData1.fDEPREC_C = new decimal(new int[] { 0, 0, 0, 65536 });
            itemsEventsData1.fDESCR = "";
            itemsEventsData1.fDOCNR = "";
            itemsEventsData1.fDT = new System.DateTime(0L);
            itemsEventsData1.fDTREG = new System.DateTime(0L);
            itemsEventsData1.fEVENT = 0;
            itemsEventsData1.fID = 0;
            itemsEventsData1.fIDIT = 0;
            itemsEventsData1.fITEM_DATE1 = null;
            itemsEventsData1.fITEM_DATE2 = null;
            itemsEventsData1.fITEM_NAME = null;
            itemsEventsData1.fITEM_REG_NR = null;
            itemsEventsData1.fMT_TOTAL = 0;
            itemsEventsData1.fMT_USED = 0;
            itemsEventsData1.fPLACE = 0;
            itemsEventsData1.fRATE_D = 0F;
            itemsEventsData1.fRATE_D_MT = new decimal(new int[] { 0, 0, 0, 65536 });
            itemsEventsData1.fSELL_VALUE = new decimal(new int[] { 0, 0, 0, 65536 });
            itemsEventsData1.fSNR = 0;
            itemsEventsData1.fTAX_EACH = 0;
            itemsEventsData1.fTAX_RATE = 0F;
            itemsEventsData1.fTAX_VAL = new decimal(new int[] { 0, 0, 0, 65536 });
            itemsEventsData1.fTAX_VAL_C = new decimal(new int[] { 0, 0, 0, 65536 });
            itemsEventsData1.fTAX_VAL_LEFT = new decimal(new int[] { 0, 0, 0, 65536 });
            itemsEventsData1.fVALUE_0 = new decimal(new int[] { 0, 0, 0, 65536 });
            itemsEventsData1.fVALUE_C = new decimal(new int[] { 0, 0, 0, 65536 });
            itemsEventsData1.fVALUE_LEFT = new decimal(new int[] { 0, 0, 0, 65536 });
            itemsEventsData1.fZDT = null;
            itemsEventsData1.fZU = null;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(14, 41);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(86, 17);
            label2.TabIndex = 14;
            label2.Text = "Nosaukums:";
            // 
            // tbName
            // 
            tbName.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            tbName.DataBindings.Add(new System.Windows.Forms.Binding("Text", bsItem, "fITEM_NAME", true));
            tbName.Location = new System.Drawing.Point(152, 38);
            tbName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tbName.MaxLength = 150;
            tbName.Name = "tbName";
            tbName.Size = new System.Drawing.Size(279, 23);
            tbName.TabIndex = 1;
            tbName.KeyDown += control_KeyDown;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(14, 107);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(112, 17);
            label3.TabIndex = 15;
            label3.Text = "Iegādes datums:";
            // 
            // tbDate
            // 
            tbDate.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            tbDate.DataBindings.Add(new System.Windows.Forms.Binding("Text", bsItem, "fDT", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "dd.MM.yyyy"));
            tbDate.IsDate = true;
            tbDate.Location = new System.Drawing.Point(137, 104);
            tbDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tbDate.Name = "tbDate";
            tbDate.Size = new System.Drawing.Size(90, 23);
            tbDate.TabIndex = 2;
            tbDate.KeyDown += control_KeyDown;
            // 
            // chAddEvent
            // 
            chAddEvent.AutoSize = true;
            chAddEvent.Checked = true;
            chAddEvent.CheckState = System.Windows.Forms.CheckState.Checked;
            chAddEvent.Location = new System.Drawing.Point(14, 72);
            chAddEvent.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            chAddEvent.Name = "chAddEvent";
            chAddEvent.Size = new System.Drawing.Size(355, 18);
            chAddEvent.TabIndex = 12;
            chAddEvent.Text = "Izveidot iegādes un nodošanas ekspluatācijā notikumu";
            chAddEvent.UseVisualStyleBackColor = true;
            chAddEvent.KeyDown += control_KeyDown;
            // 
            // cbCat1
            // 
            cbCat1.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            cbCat1.ColumnNames = new string[]
    {
    "CODE",
    "DESCR"
    };
            cbCat1.ColumnWidths = "100;300";
            cbCat1.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", bsItem, "fCAT1", true));
            cbCat1.DataSource = bsCat1;
            cbCat1.DisplayMember = "CODE";
            cbCat1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cbCat1.DropDownHeight = 270;
            cbCat1.DropDownWidth = 419;
            cbCat1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cbCat1.GridLineColor = System.Drawing.Color.LightGray;
            cbCat1.GridLineHorizontal = false;
            cbCat1.GridLineVertical = false;
            cbCat1.IntegralHeight = false;
            cbCat1.Location = new System.Drawing.Point(480, 108);
            cbCat1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cbCat1.MaxDropDownItems = 15;
            cbCat1.Name = "cbCat1";
            cbCat1.Size = new System.Drawing.Size(136, 24);
            cbCat1.TabIndex = 5;
            cbCat1.ValueMember = "ID";
            cbCat1.KeyDown += control_KeyDown;
            // 
            // bsCat1
            // 
            bsCat1.MyDataSource = new KlonsLIB.ObjectSelector.KlonsDbSetSelector("KlonsPData", "P_CAT1");
            bsCat1.Sort = "CODE";
            // 
            // cbCatD
            // 
            cbCatD.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            cbCatD.ColumnNames = new string[]
    {
    "CODE",
    "DESCR"
    };
            cbCatD.ColumnWidths = "100;300";
            cbCatD.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", bsItem, "fCATD", true));
            cbCatD.DataSource = bsCatD;
            cbCatD.DisplayMember = "CODE";
            cbCatD.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cbCatD.DropDownHeight = 270;
            cbCatD.DropDownWidth = 419;
            cbCatD.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cbCatD.GridLineColor = System.Drawing.Color.LightGray;
            cbCatD.GridLineHorizontal = false;
            cbCatD.GridLineVertical = false;
            cbCatD.IntegralHeight = false;
            cbCatD.Location = new System.Drawing.Point(480, 138);
            cbCatD.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cbCatD.MaxDropDownItems = 15;
            cbCatD.Name = "cbCatD";
            cbCatD.Size = new System.Drawing.Size(136, 24);
            cbCatD.TabIndex = 6;
            cbCatD.ValueMember = "ID";
            cbCatD.KeyDown += control_KeyDown;
            // 
            // bsCatD
            // 
            bsCatD.MyDataSource = new KlonsLIB.ObjectSelector.KlonsDbSetSelector("KlonsPData", "P_CATD");
            bsCatD.Sort = "CODE";
            // 
            // cbCatT
            // 
            cbCatT.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            cbCatT.ColumnNames = new string[]
    {
    "CODE",
    "DESCR"
    };
            cbCatT.ColumnWidths = "100;300";
            cbCatT.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", bsItem, "fCATT", true));
            cbCatT.DataSource = bsCatT;
            cbCatT.DisplayMember = "CODE";
            cbCatT.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cbCatT.DropDownHeight = 270;
            cbCatT.DropDownWidth = 419;
            cbCatT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cbCatT.GridLineColor = System.Drawing.Color.LightGray;
            cbCatT.GridLineHorizontal = false;
            cbCatT.GridLineVertical = false;
            cbCatT.IntegralHeight = false;
            cbCatT.Location = new System.Drawing.Point(480, 167);
            cbCatT.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cbCatT.MaxDropDownItems = 15;
            cbCatT.Name = "cbCatT";
            cbCatT.Size = new System.Drawing.Size(136, 24);
            cbCatT.TabIndex = 7;
            cbCatT.ValueMember = "ID";
            cbCatT.KeyDown += control_KeyDown;
            // 
            // bsCatT
            // 
            bsCatT.MyDataSource = new KlonsLIB.ObjectSelector.KlonsDbSetSelector("KlonsPData", "P_CATT");
            bsCatT.Sort = "CODE";
            // 
            // cbDep
            // 
            cbDep.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            cbDep.ColumnNames = new string[]
    {
    "CODE",
    "DESCR"
    };
            cbDep.ColumnWidths = "100;300";
            cbDep.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", bsItem, "fDEPARTMENT", true));
            cbDep.DataSource = bsDep;
            cbDep.DisplayMember = "CODE";
            cbDep.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cbDep.DropDownHeight = 270;
            cbDep.DropDownWidth = 419;
            cbDep.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cbDep.GridLineColor = System.Drawing.Color.LightGray;
            cbDep.GridLineHorizontal = false;
            cbDep.GridLineVertical = false;
            cbDep.IntegralHeight = false;
            cbDep.Location = new System.Drawing.Point(480, 196);
            cbDep.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cbDep.MaxDropDownItems = 15;
            cbDep.Name = "cbDep";
            cbDep.Size = new System.Drawing.Size(136, 24);
            cbDep.TabIndex = 8;
            cbDep.ValueMember = "ID";
            cbDep.KeyDown += control_KeyDown;
            // 
            // bsDep
            // 
            bsDep.MyDataSource = new KlonsLIB.ObjectSelector.KlonsDbSetSelector("KlonsPData", "P_DEPARTMENTS");
            bsDep.Sort = "CODE";
            // 
            // cbPlace
            // 
            cbPlace.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            cbPlace.ColumnNames = new string[]
    {
    "CODE",
    "DESCR"
    };
            cbPlace.ColumnWidths = "100;300";
            cbPlace.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", bsItem, "fPLACE", true));
            cbPlace.DataSource = bsPlace;
            cbPlace.DisplayMember = "CODE";
            cbPlace.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cbPlace.DropDownHeight = 270;
            cbPlace.DropDownWidth = 419;
            cbPlace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cbPlace.GridLineColor = System.Drawing.Color.LightGray;
            cbPlace.GridLineHorizontal = false;
            cbPlace.GridLineVertical = false;
            cbPlace.IntegralHeight = false;
            cbPlace.Location = new System.Drawing.Point(480, 226);
            cbPlace.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cbPlace.MaxDropDownItems = 15;
            cbPlace.Name = "cbPlace";
            cbPlace.Size = new System.Drawing.Size(136, 24);
            cbPlace.TabIndex = 9;
            cbPlace.ValueMember = "ID";
            cbPlace.KeyDown += control_KeyDown;
            // 
            // bsPlace
            // 
            bsPlace.MyDataSource = new KlonsLIB.ObjectSelector.KlonsDbSetSelector("KlonsPData", "P_PLACES");
            bsPlace.Sort = "CODE";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(333, 112);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(76, 17);
            label4.TabIndex = 18;
            label4.Text = "Kategorija:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(333, 141);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(113, 17);
            label5.TabIndex = 19;
            label5.Text = "Nolietojuma kat.:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(333, 171);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(137, 17);
            label6.TabIndex = 20;
            label6.Text = "Noliet. kat. nod. vaj.:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(333, 200);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(103, 17);
            label7.TabIndex = 21;
            label7.Text = "Struktūrvienība";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(333, 230);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(110, 17);
            label8.TabIndex = 22;
            label8.Text = "Atrašanās vieta:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(14, 135);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(57, 17);
            label9.TabIndex = 16;
            label9.Text = "Vērtība:";
            // 
            // tbValue
            // 
            tbValue.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            tbValue.DataBindings.Add(new System.Windows.Forms.Binding("Text", bsItem, "fVALUE_0", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N2"));
            tbValue.Location = new System.Drawing.Point(137, 132);
            tbValue.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tbValue.Name = "tbValue";
            tbValue.Size = new System.Drawing.Size(90, 23);
            tbValue.TabIndex = 3;
            tbValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            tbValue.KeyDown += control_KeyDown;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(14, 163);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(105, 17);
            label10.TabIndex = 17;
            label10.Text = "Dokumenta nr.:";
            // 
            // tbDocNr
            // 
            tbDocNr.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            tbDocNr.DataBindings.Add(new System.Windows.Forms.Binding("Text", bsItem, "fDOCNR", true));
            tbDocNr.Location = new System.Drawing.Point(137, 161);
            tbDocNr.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tbDocNr.MaxLength = 50;
            tbDocNr.Name = "tbDocNr";
            tbDocNr.Size = new System.Drawing.Size(175, 23);
            tbDocNr.TabIndex = 4;
            tbDocNr.KeyDown += control_KeyDown;
            // 
            // cmOK
            // 
            cmOK.Location = new System.Drawing.Point(21, 263);
            cmOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cmOK.Name = "cmOK";
            cmOK.Size = new System.Drawing.Size(101, 41);
            cmOK.TabIndex = 10;
            cmOK.Text = "OK";
            cmOK.UseVisualStyleBackColor = true;
            cmOK.Click += cmOK_Click;
            cmOK.KeyDown += control_KeyDown;
            // 
            // cmCancel
            // 
            cmCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cmCancel.Location = new System.Drawing.Point(141, 263);
            cmCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cmCancel.Name = "cmCancel";
            cmCancel.Size = new System.Drawing.Size(101, 41);
            cmCancel.TabIndex = 11;
            cmCancel.Text = "Atcelt";
            cmCancel.UseVisualStyleBackColor = true;
            cmCancel.KeyDown += control_KeyDown;
            // 
            // FormP_Items_New
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = cmCancel;
            ClientSize = new System.Drawing.Size(627, 310);
            Controls.Add(cmCancel);
            Controls.Add(cmOK);
            Controls.Add(cbPlace);
            Controls.Add(cbDep);
            Controls.Add(cbCatT);
            Controls.Add(cbCatD);
            Controls.Add(cbCat1);
            Controls.Add(chAddEvent);
            Controls.Add(tbDocNr);
            Controls.Add(label10);
            Controls.Add(tbValue);
            Controls.Add(label9);
            Controls.Add(tbDate);
            Controls.Add(label3);
            Controls.Add(tbName);
            Controls.Add(label2);
            Controls.Add(tbRegNr);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormP_Items_New";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Jauns pamatlīdzeklis";
            Load += Form_Items_New_Load;
            KeyDown += control_KeyDown;
            ((System.ComponentModel.ISupportInitialize)bsItem).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsCat1).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsCatD).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsCatT).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsDep).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsPlace).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private KlonsLIB.Components.MyTextBox tbRegNr;
        private System.Windows.Forms.Label label2;
        private KlonsLIB.Components.MyTextBox tbName;
        private System.Windows.Forms.Label label3;
        private KlonsLIB.Components.MyTextBox tbDate;
        private KlonsLIB.Components.MyCheckBox chAddEvent;
        private KlonsLIB.Components.MyMcFlatComboBox cbCat1;
        private KlonsLIB.Components.MyMcFlatComboBox cbCatD;
        private KlonsLIB.Components.MyMcFlatComboBox cbCatT;
        private KlonsLIB.Components.MyMcFlatComboBox cbDep;
        private KlonsLIB.Components.MyMcFlatComboBox cbPlace;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private KlonsLIB.Components.MyTextBox tbValue;
        private System.Windows.Forms.Label label10;
        private KlonsLIB.Components.MyTextBox tbDocNr;
        private System.Windows.Forms.Button cmOK;
        private System.Windows.Forms.Button cmCancel;
        private KlonsLIB.Data.MyBindingSourceEf bsCat1;
        private KlonsLIB.Data.MyBindingSourceEf bsCatD;
        private KlonsLIB.Data.MyBindingSourceEf bsCatT;
        private KlonsLIB.Data.MyBindingSourceEf bsDep;
        private KlonsLIB.Data.MyBindingSourceEf bsPlace;
        private KlonsLIB.Data.MyBindingSourceToObj bsItem;
        public DataObjectsP.ItemsEventsData itemsEventsData1;
    }
}