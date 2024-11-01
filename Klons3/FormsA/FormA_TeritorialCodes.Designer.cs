﻿using KlonsLIB.Components;

namespace KlonsA.Forms
{
    partial class FormA_TeritorialCodes
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormA_TeritorialCodes));
            this.dgvTerKodi = new KlonsLIB.Components.MyDataGridView();
            this.dgcID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcDescr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsTerKodi = new KlonsLIB.Data.MyBindingSourceEf(this.components);
            this.bnavTerKodi = new KlonsLIB.Components.MyBindingNavigator();
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSave = new System.Windows.Forms.ToolStripButton();
            this.label1 = new System.Windows.Forms.Label();
            this.tbSearch = new KlonsLIB.Components.MyTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTerKodi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsTerKodi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bnavTerKodi)).BeginInit();
            this.bnavTerKodi.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvTerKodi
            // 
            this.dgvTerKodi.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTerKodi.AutoGenerateColumns = false;
            this.dgvTerKodi.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvTerKodi.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTerKodi.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgcID,
            this.dgcDescr});
            this.dgvTerKodi.DataSource = this.bsTerKodi;
            this.dgvTerKodi.Location = new System.Drawing.Point(0, 25);
            this.dgvTerKodi.Name = "dgvTerKodi";
            this.dgvTerKodi.Size = new System.Drawing.Size(510, 365);
            this.dgvTerKodi.TabIndex = 0;
            this.dgvTerKodi.MyKeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvTerKodi_MyKeyDown);
            this.dgvTerKodi.MyCheckForChanges += new System.EventHandler(this.dgvTerKodi_MyCheckForChanges);
            this.dgvTerKodi.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTerKodi_CellDoubleClick);
            this.dgvTerKodi.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dgvTerKodi_UserDeletingRow);
            this.dgvTerKodi.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvTerKodi_KeyPress);
            // 
            // dgcID
            // 
            this.dgcID.DataPropertyName = "ID";
            this.dgcID.HeaderText = "Kods";
            this.dgcID.MinimumWidth = 4;
            this.dgcID.Name = "dgcID";
            // 
            // dgcDescr
            // 
            this.dgcDescr.DataPropertyName = "DESCR";
            this.dgcDescr.HeaderText = "Nosaukums";
            this.dgcDescr.MinimumWidth = 4;
            this.dgcDescr.Name = "dgcDescr";
            this.dgcDescr.Width = 300;
            // 
            // bsTerKodi
            // 
            this.bsTerKodi.MyDataSource = new KlonsLIB.ObjectSelector.KlonsDbSetSelector("KlonsAData", "A_TERITORIAL_CODES");
            this.bsTerKodi.Sort = "ID";
            this.bsTerKodi.UseDataGridView = this.dgvTerKodi;
            this.bsTerKodi.ListChanged += new System.ComponentModel.ListChangedEventHandler(this.bsTerKodi_ListChanged);
            // 
            // bnavTerKodi
            // 
            this.bnavTerKodi.AddNewItem = this.bindingNavigatorAddNewItem;
            this.bnavTerKodi.BindingSource = this.bsTerKodi;
            this.bnavTerKodi.CountItem = this.bindingNavigatorCountItem;
            this.bnavTerKodi.CountItemFormat = " no {0}";
            this.bnavTerKodi.DataGrid = this.dgvTerKodi;
            this.bnavTerKodi.DeleteItem = this.bindingNavigatorDeleteItem;
            this.bnavTerKodi.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bnavTerKodi.ImageScalingSize = new System.Drawing.Size(21, 21);
            this.bnavTerKodi.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.bindingNavigatorAddNewItem,
            this.bindingNavigatorDeleteItem,
            this.tsbSave});
            this.bnavTerKodi.Location = new System.Drawing.Point(0, 386);
            this.bnavTerKodi.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bnavTerKodi.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bnavTerKodi.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bnavTerKodi.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bnavTerKodi.Name = "bnavTerKodi";
            this.bnavTerKodi.PositionItem = this.bindingNavigatorPositionItem;
            this.bnavTerKodi.Size = new System.Drawing.Size(510, 32);
            this.bnavTerKodi.TabIndex = 1;
            this.bnavTerKodi.Text = "myBindingNavigator1";
            this.bnavTerKodi.ItemDeleting += new System.ComponentModel.CancelEventHandler(this.bnavTerKodi_ItemDeleting);
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem.Image")));
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(91, 29);
            this.bindingNavigatorAddNewItem.Text = "Jauns";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(69, 29);
            this.bindingNavigatorCountItem.Text = " no {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Ierakstu skaits";
            // 
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(87, 29);
            this.bindingNavigatorDeleteItem.Text = "Dzēst";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(25, 29);
            this.bindingNavigatorMoveFirstItem.Text = "Iet uz pirmo";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(25, 29);
            this.bindingNavigatorMovePreviousItem.Text = "Iet uz iepriekšējo";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 32);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 30);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Pašreizējā pozīcija";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 32);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(25, 29);
            this.bindingNavigatorMoveNextItem.Text = "Iet uz nākošo";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(25, 29);
            this.bindingNavigatorMoveLastItem.Text = "Iet uz pēdējo";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 32);
            // 
            // tsbSave
            // 
            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSave.Image = ((System.Drawing.Image)(resources.GetObject("tsbSave.Image")));
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(25, 29);
            this.tsbSave.Text = "toolStripButton1";
            this.tsbSave.Click += new System.EventHandler(this.tsbSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Meklēt:";
            // 
            // tbSearch
            // 
            this.tbSearch.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.tbSearch.Location = new System.Drawing.Point(61, 2);
            this.tbSearch.Name = "tbSearch";
            this.tbSearch.Size = new System.Drawing.Size(119, 22);
            this.tbSearch.TabIndex = 3;
            this.tbSearch.Enter += new System.EventHandler(this.tbSearch_Enter);
            this.tbSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbSearch_KeyPress);
            // 
            // Form_TeritorialCodes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(510, 418);
            this.Controls.Add(this.tbSearch);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bnavTerKodi);
            this.Controls.Add(this.dgvTerKodi);
            this.Name = "Form_TeritorialCodes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Teritoriju kodi";
            this.Load += new System.EventHandler(this.Form_TeritorialCodes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTerKodi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsTerKodi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bnavTerKodi)).EndInit();
            this.bnavTerKodi.ResumeLayout(false);
            this.bnavTerKodi.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MyDataGridView dgvTerKodi;
        private MyBindingNavigator bnavTerKodi;
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
        private KlonsLIB.Data.MyBindingSourceEf bsTerKodi;
        private System.Windows.Forms.Label label1;
        private MyTextBox tbSearch;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcDescr;
    }
}