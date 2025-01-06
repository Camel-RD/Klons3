﻿using KlonsLIB.Components;
using KlonsLIB.Data;
using KlonsF.Classes;
using Klons3.ModelsF;

namespace KlonsF.Forms
{
    partial class Form_Persons
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Persons));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            MyMcComboBox.MyItem myItem3 = new MyMcComboBox.MyItem();
            MyMcComboBox.MyItem myItem4 = new MyMcComboBox.MyItem();
            bnavPersons = new MyBindingNavigator();
            bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            bsPersons = new MyBindingSourceEf(components);
            bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            dgvPersons = new MyDataGridView();
            dgcClid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcRegNr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcPVNRegNr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcTP = new MyDgvMcCBColumn();
            dgcTP2 = new MyDgvMcCBColumn();
            dgcTP3 = new MyDgvCheckBoxColumn();
            dgcAddr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcAddr2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcATK = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcBankId = new MyDgvMcCBColumn();
            bsBanks = new MyBindingSourceEf(components);
            dgcBank = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcBankAcc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcPhone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcAct = new MyDgvCheckBoxColumn();
            bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            tsbSave = new System.Windows.Forms.ToolStripButton();
            tbSearch = new MyTextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            tbClid = new MyPickRowTextBox();
            cbAct = new MyMcFlatComboBox();
            label3 = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)bnavPersons).BeginInit();
            bnavPersons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bsPersons).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvPersons).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsBanks).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // bnavPersons
            // 
            bnavPersons.AddNewItem = bindingNavigatorAddNewItem;
            bnavPersons.BindingSource = bsPersons;
            bnavPersons.CountItem = bindingNavigatorCountItem;
            bnavPersons.CountItemFormat = " no {0}";
            bnavPersons.DataGrid = dgvPersons;
            bnavPersons.DeleteItem = bindingNavigatorDeleteItem;
            bnavPersons.Dock = System.Windows.Forms.DockStyle.Bottom;
            bnavPersons.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            bnavPersons.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { bindingNavigatorMoveFirstItem, bindingNavigatorMovePreviousItem, bindingNavigatorSeparator, bindingNavigatorPositionItem, bindingNavigatorCountItem, bindingNavigatorSeparator1, bindingNavigatorMoveNextItem, bindingNavigatorMoveLastItem, bindingNavigatorSeparator2, bindingNavigatorAddNewItem, bindingNavigatorDeleteItem, tsbSave });
            bnavPersons.Location = new System.Drawing.Point(0, 352);
            bnavPersons.MoveFirstItem = bindingNavigatorMoveFirstItem;
            bnavPersons.MoveLastItem = bindingNavigatorMoveLastItem;
            bnavPersons.MoveNextItem = bindingNavigatorMoveNextItem;
            bnavPersons.MovePreviousItem = bindingNavigatorMovePreviousItem;
            bnavPersons.Name = "bnavPersons";
            bnavPersons.PositionItem = bindingNavigatorPositionItem;
            bnavPersons.SaveItem = null;
            bnavPersons.Size = new System.Drawing.Size(913, 25);
            bnavPersons.TabIndex = 2;
            bnavPersons.Text = "bindingNavigator1";
            bnavPersons.ItemDeleting += bnavPersons_ItemDeleting;
            // 
            // bindingNavigatorAddNewItem
            // 
            bindingNavigatorAddNewItem.Image = (System.Drawing.Image)resources.GetObject("bindingNavigatorAddNewItem.Image");
            bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            bindingNavigatorAddNewItem.Size = new System.Drawing.Size(66, 22);
            bindingNavigatorAddNewItem.Text = "Jauns";
            bindingNavigatorAddNewItem.ToolTipText = "Jauns (Shift+Insert)";
            // 
            // bsPersons
            // 
            bsPersons.MyDataSource = new KlonsLIB.ObjectSelector.KlonsDbSetSelector("KlonsData", "F_PERSONS");
            bsPersons.ListChanged += bsPersons_ListChanged;
            // 
            // bindingNavigatorCountItem
            // 
            bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            bindingNavigatorCountItem.Size = new System.Drawing.Size(50, 22);
            bindingNavigatorCountItem.Text = " no {0}";
            bindingNavigatorCountItem.ToolTipText = "Skaits";
            // 
            // dgvPersons
            // 
            dgvPersons.AutoGenerateColumns = false;
            dgvPersons.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            dgvPersons.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvPersons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPersons.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { dgcClid, dgcName, dgcRegNr, dgcPVNRegNr, dgcTP, dgcTP2, dgcTP3, dgcAddr, dgcAddr2, dgcATK, dgcBankId, dgcBank, dgcBankAcc, dgcPhone, dgcAct });
            dgvPersons.DataSource = bsPersons;
            dgvPersons.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvPersons.Location = new System.Drawing.Point(0, 28);
            dgvPersons.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            dgvPersons.Name = "dgvPersons";
            dgvPersons.RowHeadersWidth = 62;
            dgvPersons.Size = new System.Drawing.Size(913, 324);
            dgvPersons.TabIndex = 1;
            dgvPersons.MyKeyDown += dgvPersons_MyKeyDown;
            dgvPersons.MyCheckForChanges += dgvPersons_MyCheckForChanges;
            dgvPersons.CellDoubleClick += dgvPersons_CellDoubleClick;
            dgvPersons.CellEndEdit += dgvPersons_CellEndEdit;
            dgvPersons.UserDeletingRow += dgvPersons_UserDeletingRow;
            dgvPersons.KeyDown += dgvPersons_KeyDown;
            dgvPersons.KeyPress += dgvPersons_KeyPress;
            // 
            // dgcClid
            // 
            dgcClid.DataPropertyName = "CLID";
            dgcClid.HeaderText = "kods";
            dgcClid.MinimumWidth = 9;
            dgcClid.Name = "dgcClid";
            dgcClid.Width = 108;
            // 
            // dgcName
            // 
            dgcName.DataPropertyName = "NAME";
            dgcName.HeaderText = "nosaukums";
            dgcName.MinimumWidth = 9;
            dgcName.Name = "dgcName";
            dgcName.Width = 281;
            // 
            // dgcRegNr
            // 
            dgcRegNr.DataPropertyName = "REGNR";
            dgcRegNr.HeaderText = "reģ.nr.";
            dgcRegNr.MinimumWidth = 9;
            dgcRegNr.Name = "dgcRegNr";
            dgcRegNr.ToolTipText = "Uzņēmuma reģistrācijas numurs, vai personas kods";
            dgcRegNr.Width = 101;
            // 
            // dgcPVNRegNr
            // 
            dgcPVNRegNr.DataPropertyName = "PVNREGNR";
            dgcPVNRegNr.HeaderText = "PVN reģ.nr.";
            dgcPVNRegNr.MinimumWidth = 9;
            dgcPVNRegNr.Name = "dgcPVNRegNr";
            dgcPVNRegNr.Width = 120;
            // 
            // dgcTP
            // 
            dgcTP.ColumnNames = new string[]
    {
    "col1",
    "col2"
    };
            dgcTP.ColumnWidths = "50;150";
            dgcTP.DataPropertyName = "TP";
            dgcTP.DisplayMember = "col1";
            dgcTP.HeaderText = "veids";
            dgcTP.ItemStrings = new string[]
    {
    "DB;debitors",
    "KR;kreditors"
    };
            dgcTP.MaxDropDownItems = 15;
            dgcTP.MinimumWidth = 9;
            dgcTP.Name = "dgcTP";
            dgcTP.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            dgcTP.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            dgcTP.ValueMember = "col1";
            dgcTP.Width = 54;
            // 
            // dgcTP2
            // 
            dgcTP2.ColumnNames = new string[]
    {
    "col1",
    "col2"
    };
            dgcTP2.DataPropertyName = "TP2";
            dgcTP2.DisplayMember = "col1";
            dgcTP2.HeaderText = "fp/jp";
            dgcTP2.ItemStrings = new string[]
    {
    "FP;fiziska persona",
    "JP;juridiska persona",
    "DA;darbinieks"
    };
            dgcTP2.MaxDropDownItems = 15;
            dgcTP2.MinimumWidth = 9;
            dgcTP2.Name = "dgcTP2";
            dgcTP2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            dgcTP2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            dgcTP2.ValueMember = "col1";
            dgcTP2.Width = 45;
            // 
            // dgcTP3
            // 
            dgcTP3.DataPropertyName = "TP3";
            dgcTP3.FalseValue = "Nav";
            dgcTP3.HeaderText = "pvn";
            dgcTP3.MinimumWidth = 9;
            dgcTP3.Name = "dgcTP3";
            dgcTP3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            dgcTP3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            dgcTP3.ToolTipText = "vai ir pvn maksātajs";
            dgcTP3.TrueValue = "Ir";
            dgcTP3.Width = 45;
            // 
            // dgcAddr
            // 
            dgcAddr.DataPropertyName = "ADDR";
            dgcAddr.HeaderText = "jur. adrese";
            dgcAddr.MinimumWidth = 9;
            dgcAddr.Name = "dgcAddr";
            dgcAddr.ToolTipText = "Juridiskā adrese";
            dgcAddr.Width = 180;
            // 
            // dgcAddr2
            // 
            dgcAddr2.DataPropertyName = "ADDR2";
            dgcAddr2.HeaderText = "fiz. adrese";
            dgcAddr2.MinimumWidth = 9;
            dgcAddr2.Name = "dgcAddr2";
            dgcAddr2.ToolTipText = "Struktūrvienības adrese (pavadzīmēm)";
            dgcAddr2.Width = 180;
            // 
            // dgcATK
            // 
            dgcATK.DataPropertyName = "ATK";
            dgcATK.HeaderText = "ATK";
            dgcATK.MinimumWidth = 9;
            dgcATK.Name = "dgcATK";
            dgcATK.Width = 90;
            // 
            // dgcBankId
            // 
            dgcBankId.ColumnNames = new string[]
    {
    "id",
    "name"
    };
            dgcBankId.ColumnWidths = "100;200";
            dgcBankId.DataPropertyName = "BANKID";
            dgcBankId.DataSource = bsBanks;
            dgcBankId.DisplayMember = "Id";
            dgcBankId.HeaderText = "bankas kods";
            dgcBankId.MaxDropDownItems = 15;
            dgcBankId.MinimumWidth = 9;
            dgcBankId.Name = "dgcBankId";
            dgcBankId.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            dgcBankId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            dgcBankId.ToolTipText = "Bankas BIC kods";
            dgcBankId.ValueMember = "Id";
            dgcBankId.Width = 126;
            // 
            // bsBanks
            // 
            bsBanks.MyDataSource = new KlonsLIB.ObjectSelector.KlonsDbSetSelector("KlonsData", "F_BANKS");
            // 
            // dgcBank
            // 
            dgcBank.DataPropertyName = "BANK";
            dgcBank.HeaderText = "bankas nosaukums";
            dgcBank.MaxInputLength = 50;
            dgcBank.MinimumWidth = 9;
            dgcBank.Name = "dgcBank";
            dgcBank.Width = 180;
            // 
            // dgcBankAcc
            // 
            dgcBankAcc.DataPropertyName = "BANKACC";
            dgcBankAcc.HeaderText = "bankas konts";
            dgcBankAcc.MinimumWidth = 9;
            dgcBankAcc.Name = "dgcBankAcc";
            dgcBankAcc.Width = 135;
            // 
            // dgcPhone
            // 
            dgcPhone.DataPropertyName = "PHONE";
            dgcPhone.HeaderText = "telefona";
            dgcPhone.MinimumWidth = 9;
            dgcPhone.Name = "dgcPhone";
            dgcPhone.Width = 90;
            // 
            // dgcAct
            // 
            dgcAct.DataPropertyName = "ACT";
            dgcAct.FalseValue = "0";
            dgcAct.HeaderText = "aktīvs";
            dgcAct.MinimumWidth = 9;
            dgcAct.Name = "dgcAct";
            dgcAct.TrueValue = "1";
            dgcAct.Width = 54;
            // 
            // bindingNavigatorDeleteItem
            // 
            bindingNavigatorDeleteItem.Image = (System.Drawing.Image)resources.GetObject("bindingNavigatorDeleteItem.Image");
            bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            bindingNavigatorDeleteItem.Size = new System.Drawing.Size(64, 22);
            bindingNavigatorDeleteItem.Text = "Dzēst";
            bindingNavigatorDeleteItem.ToolTipText = "Dzēst (Ctrl+Delete)";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            bindingNavigatorMoveFirstItem.Image = (System.Drawing.Image)resources.GetObject("bindingNavigatorMoveFirstItem.Image");
            bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 22);
            bindingNavigatorMoveFirstItem.Text = "Iet uz pirmo";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            bindingNavigatorMovePreviousItem.Image = (System.Drawing.Image)resources.GetObject("bindingNavigatorMovePreviousItem.Image");
            bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 22);
            bindingNavigatorMovePreviousItem.Text = "Iet uz iepriekšējo";
            // 
            // bindingNavigatorSeparator
            // 
            bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorPositionItem
            // 
            bindingNavigatorPositionItem.AccessibleName = "Position";
            bindingNavigatorPositionItem.AutoSize = false;
            bindingNavigatorPositionItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            bindingNavigatorPositionItem.Size = new System.Drawing.Size(56, 23);
            bindingNavigatorPositionItem.Text = "0";
            bindingNavigatorPositionItem.ToolTipText = "Pašreizējā pozīcija";
            // 
            // bindingNavigatorSeparator1
            // 
            bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorMoveNextItem
            // 
            bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            bindingNavigatorMoveNextItem.Image = (System.Drawing.Image)resources.GetObject("bindingNavigatorMoveNextItem.Image");
            bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 22);
            bindingNavigatorMoveNextItem.Text = "Iet uz nākošo";
            bindingNavigatorMoveNextItem.ToolTipText = "Iet uz nākošo";
            // 
            // bindingNavigatorMoveLastItem
            // 
            bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            bindingNavigatorMoveLastItem.Image = (System.Drawing.Image)resources.GetObject("bindingNavigatorMoveLastItem.Image");
            bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 22);
            bindingNavigatorMoveLastItem.Text = "Iet uz pēdējo";
            // 
            // bindingNavigatorSeparator2
            // 
            bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbSave
            // 
            tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbSave.Image = (System.Drawing.Image)resources.GetObject("tsbSave.Image");
            tsbSave.Name = "tsbSave";
            tsbSave.Size = new System.Drawing.Size(23, 22);
            tsbSave.Text = "Saglabāt datus";
            tsbSave.Click += tsbSave_Click;
            // 
            // tbSearch
            // 
            tbSearch.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            tbSearch.Location = new System.Drawing.Point(229, 3);
            tbSearch.Margin = new System.Windows.Forms.Padding(2);
            tbSearch.Name = "tbSearch";
            tbSearch.Size = new System.Drawing.Size(138, 23);
            tbSearch.TabIndex = 1;
            tbSearch.Enter += tbSearch_Enter;
            tbSearch.KeyPress += tbSearch_KeyPress;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(4, 5);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(42, 17);
            label1.TabIndex = 3;
            label1.Text = "kods:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(172, 5);
            label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(53, 17);
            label2.TabIndex = 4;
            label2.Text = "meklēt:";
            // 
            // tbClid
            // 
            tbClid.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            tbClid.DataMember = null;
            tbClid.DataPropertyName = "CLID";
            tbClid.DataSource = bsPersons;
            tbClid.Location = new System.Drawing.Point(50, 3);
            tbClid.Margin = new System.Windows.Forms.Padding(2);
            tbClid.Name = "tbClid";
            tbClid.Size = new System.Drawing.Size(108, 23);
            tbClid.TabIndex = 0;
            tbClid.RowSelectedEvent += tbClid_RowSelectedEvent;
            tbClid.Enter += tbClid_Enter;
            // 
            // cbAct
            // 
            cbAct.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            cbAct.ColumnNames = new string[]
    {
    "col1"
    };
            cbAct.ColumnWidths = "67";
            cbAct.DisplayMember = "col1";
            cbAct.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cbAct.DropDownHeight = 144;
            cbAct.DropDownStyle = MyMcComboBox.CustomDropDownStyle.DropDownListSimple;
            cbAct.DropDownWidth = 86;
            cbAct.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cbAct.GridLineColor = System.Drawing.Color.LightGray;
            cbAct.GridLineHorizontal = false;
            cbAct.GridLineVertical = false;
            cbAct.IntegralHeight = false;
            myItem3.Col1 = "Visus";
            myItem4.Col1 = "Aktīvos";
            cbAct.Items.AddRange(new object[] { myItem3, myItem4 });
            cbAct.ItemStrings = new string[]
    {
    "Visus",
    "Aktīvos"
    };
            cbAct.Location = new System.Drawing.Point(428, 2);
            cbAct.Margin = new System.Windows.Forms.Padding(2);
            cbAct.Name = "cbAct";
            cbAct.Size = new System.Drawing.Size(86, 24);
            cbAct.TabIndex = 2;
            cbAct.ValueMember = "col1";
            cbAct.SelectedIndexChanged += cbAct_SelectedIndexChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(384, 5);
            label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(40, 17);
            label3.TabIndex = 5;
            label3.Text = "rādīt:";
            // 
            // panel1
            // 
            panel1.Controls.Add(tbClid);
            panel1.Controls.Add(cbAct);
            panel1.Controls.Add(tbSearch);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(913, 28);
            panel1.TabIndex = 0;
            // 
            // Form_Persons
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(913, 377);
            Controls.Add(dgvPersons);
            Controls.Add(panel1);
            Controls.Add(bnavPersons);
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "Form_Persons";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Personas";
            Load += FormPersons_Load;
            Shown += FormPersons_Shown;
            ((System.ComponentModel.ISupportInitialize)bnavPersons).EndInit();
            bnavPersons.ResumeLayout(false);
            bnavPersons.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bsPersons).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvPersons).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsBanks).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MyBindingNavigator bnavPersons;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private MyDataGridView dgvPersons;
        private KlonsLIB.Data.MyBindingSourceEf bsPersons;
        private MyTextBox tbSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private MyPickRowTextBox tbClid;
        private MyMcFlatComboBox cbAct;
        private System.Windows.Forms.Label label3;
        private KlonsLIB.Data.MyBindingSourceEf bsBanks;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcClid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcRegNr;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcPVNRegNr;
        private MyDgvMcCBColumn dgcTP;
        private MyDgvMcCBColumn dgcTP2;
        private MyDgvCheckBoxColumn dgcTP3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcAddr;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcAddr2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcATK;
        private MyDgvMcCBColumn dgcBankId;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcBank;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcBankAcc;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcPhone;
        private MyDgvCheckBoxColumn dgcAct;
        private System.Windows.Forms.Panel panel1;
    }
}