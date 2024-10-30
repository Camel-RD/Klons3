namespace KlonsA.Forms
{
    partial class FormA_TimeSheets
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormA_TimeSheets));
            bnavSar = new KlonsLIB.Components.MyBindingNavigator();
            bsSar = new KlonsLIB.Data.MyBindingSourceEf(components);
            dgvSar = new KlonsLIB.Components.MyDataGridView();
            dgcSarId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcSarYr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcSarMt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcSarSNR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcSarDescr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcSarDep = new System.Windows.Forms.DataGridViewComboBoxColumn();
            bsDep = new KlonsLIB.Data.MyBindingSourceEf(components);
            bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            tsbOpen = new System.Windows.Forms.ToolStripButton();
            tsbNew = new System.Windows.Forms.ToolStripButton();
            tsbDelete = new System.Windows.Forms.ToolStripButton();
            tsbSave = new System.Windows.Forms.ToolStripButton();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            tslPeriod = new System.Windows.Forms.ToolStripLabel();
            ((System.ComponentModel.ISupportInitialize)bnavSar).BeginInit();
            bnavSar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bsSar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvSar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsDep).BeginInit();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // bnavSar
            // 
            bnavSar.AddNewItem = null;
            bnavSar.BindingSource = bsSar;
            bnavSar.CountItem = bindingNavigatorCountItem;
            bnavSar.CountItemFormat = " no {0}";
            bnavSar.DataGrid = dgvSar;
            bnavSar.DeleteItem = null;
            bnavSar.Dock = System.Windows.Forms.DockStyle.Bottom;
            bnavSar.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            bnavSar.ImageScalingSize = new System.Drawing.Size(21, 21);
            bnavSar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { bindingNavigatorMoveFirstItem, bindingNavigatorMovePreviousItem, bindingNavigatorSeparator, bindingNavigatorPositionItem, bindingNavigatorCountItem, bindingNavigatorSeparator1, bindingNavigatorMoveNextItem, bindingNavigatorMoveLastItem, bindingNavigatorSeparator2, tsbOpen, tsbNew, tsbDelete, tsbSave });
            bnavSar.Location = new System.Drawing.Point(0, 411);
            bnavSar.MoveFirstItem = bindingNavigatorMoveFirstItem;
            bnavSar.MoveLastItem = bindingNavigatorMoveLastItem;
            bnavSar.MoveNextItem = bindingNavigatorMoveNextItem;
            bnavSar.MovePreviousItem = bindingNavigatorMovePreviousItem;
            bnavSar.Name = "bnavSar";
            bnavSar.Padding = new System.Windows.Forms.Padding(0, 0, 1, 2);
            bnavSar.PositionItem = bindingNavigatorPositionItem;
            bnavSar.SaveItem = null;
            bnavSar.Size = new System.Drawing.Size(710, 30);
            bnavSar.TabIndex = 0;
            bnavSar.Text = "myBindingNavigator1";
            bnavSar.ItemDeleting += bnavSar_ItemDeleting;
            // 
            // bsSar
            // 
            bsSar.MyDataSource = new KlonsLIB.ObjectSelector.KlonsDbSetSelector("KlonsAData", "A_TIMESHEET_LISTS");
            bsSar.Sort = "YR,MT,SNR";
            bsSar.UseDataGridView = dgvSar;
            bsSar.ListChanged += bsSar_ListChanged;
            // 
            // dgvSar
            // 
            dgvSar.AllowUserToAddRows = false;
            dgvSar.AllowUserToDeleteRows = false;
            dgvSar.AutoGenerateColumns = false;
            dgvSar.AutoSave = true;
            dgvSar.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            dgvSar.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dgvSar.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSar.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { dgcSarId, dgcSarYr, dgcSarMt, dgcSarSNR, dgcSarDescr, dgcSarDep });
            dgvSar.DataSource = bsSar;
            dgvSar.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvSar.Location = new System.Drawing.Point(0, 25);
            dgvSar.Name = "dgvSar";
            dgvSar.Size = new System.Drawing.Size(710, 386);
            dgvSar.TabIndex = 0;
            dgvSar.MyKeyDown += dgvSar_MyKeyDown;
            dgvSar.MyCheckForChanges += dgvSar_MyCheckForChanges;
            dgvSar.CellDoubleClick += dgvSar_CellDoubleClick;
            dgvSar.UserDeletingRow += dgvSar_UserDeletingRow;
            // 
            // dgcSarId
            // 
            dgcSarId.DataPropertyName = "ID";
            dgcSarId.HeaderText = "ID";
            dgcSarId.Name = "dgcSarId";
            dgcSarId.Visible = false;
            // 
            // dgcSarYr
            // 
            dgcSarYr.DataPropertyName = "YR";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dgcSarYr.DefaultCellStyle = dataGridViewCellStyle5;
            dgcSarYr.HeaderText = "gads";
            dgcSarYr.Name = "dgcSarYr";
            dgcSarYr.ReadOnly = true;
            dgcSarYr.Width = 50;
            // 
            // dgcSarMt
            // 
            dgcSarMt.DataPropertyName = "MT";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dgcSarMt.DefaultCellStyle = dataGridViewCellStyle6;
            dgcSarMt.HeaderText = "mēn";
            dgcSarMt.Name = "dgcSarMt";
            dgcSarMt.ReadOnly = true;
            dgcSarMt.Width = 40;
            // 
            // dgcSarSNR
            // 
            dgcSarSNR.DataPropertyName = "SNR";
            dgcSarSNR.HeaderText = "npk";
            dgcSarSNR.Name = "dgcSarSNR";
            dgcSarSNR.Width = 40;
            // 
            // dgcSarDescr
            // 
            dgcSarDescr.DataPropertyName = "DESCR";
            dgcSarDescr.HeaderText = "apraksts";
            dgcSarDescr.Name = "dgcSarDescr";
            dgcSarDescr.Width = 300;
            // 
            // dgcSarDep
            // 
            dgcSarDep.DataPropertyName = "Department";
            dgcSarDep.DataSource = bsDep;
            dgcSarDep.DisplayMember = "DESCR";
            dgcSarDep.DisplayStyleForCurrentCellOnly = true;
            dgcSarDep.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            dgcSarDep.HeaderText = "struktūrv.";
            dgcSarDep.Name = "dgcSarDep";
            dgcSarDep.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            dgcSarDep.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            dgcSarDep.ValueMember = "Me";
            dgcSarDep.Width = 200;
            // 
            // bsDep
            // 
            bsDep.MyDataSource = new KlonsLIB.ObjectSelector.KlonsDbSetSelector("KlonsAData", "A_DEPARTMENTS");
            bsDep.Sort = "ID";
            // 
            // bindingNavigatorCountItem
            // 
            bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            bindingNavigatorCountItem.Size = new System.Drawing.Size(50, 25);
            bindingNavigatorCountItem.Text = " no {0}";
            bindingNavigatorCountItem.ToolTipText = "Ierakstu skaits";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            bindingNavigatorMoveFirstItem.Image = (System.Drawing.Image)resources.GetObject("bindingNavigatorMoveFirstItem.Image");
            bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(25, 25);
            bindingNavigatorMoveFirstItem.Text = "Iet uz pirmo";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            bindingNavigatorMovePreviousItem.Image = (System.Drawing.Image)resources.GetObject("bindingNavigatorMovePreviousItem.Image");
            bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(25, 25);
            bindingNavigatorMovePreviousItem.Text = "Iet uz iepriekšējo";
            // 
            // bindingNavigatorSeparator
            // 
            bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 28);
            // 
            // bindingNavigatorPositionItem
            // 
            bindingNavigatorPositionItem.AutoSize = false;
            bindingNavigatorPositionItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 23);
            bindingNavigatorPositionItem.Text = "0";
            bindingNavigatorPositionItem.ToolTipText = "Pašreizējā pozīcija";
            // 
            // bindingNavigatorSeparator1
            // 
            bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // bindingNavigatorMoveNextItem
            // 
            bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            bindingNavigatorMoveNextItem.Image = (System.Drawing.Image)resources.GetObject("bindingNavigatorMoveNextItem.Image");
            bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(25, 25);
            bindingNavigatorMoveNextItem.Text = "Iet uz nākošo";
            // 
            // bindingNavigatorMoveLastItem
            // 
            bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            bindingNavigatorMoveLastItem.Image = (System.Drawing.Image)resources.GetObject("bindingNavigatorMoveLastItem.Image");
            bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(25, 25);
            bindingNavigatorMoveLastItem.Text = "Iet uz pēdējo";
            // 
            // bindingNavigatorSeparator2
            // 
            bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 28);
            // 
            // tsbOpen
            // 
            tsbOpen.Image = KlonsF.Properties.Resources.open;
            tsbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsbOpen.Name = "tsbOpen";
            tsbOpen.Size = new System.Drawing.Size(70, 25);
            tsbOpen.Text = "Atvērt";
            tsbOpen.ToolTipText = "Atvērt aprēķina lapu";
            tsbOpen.Click += tsbOpen_Click;
            // 
            // tsbNew
            // 
            tsbNew.Image = KlonsF.Properties.Resources._new;
            tsbNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsbNew.Name = "tsbNew";
            tsbNew.Size = new System.Drawing.Size(71, 25);
            tsbNew.Text = "Jauns";
            tsbNew.ToolTipText = "Jauna algas aprēķina lapa";
            tsbNew.Click += tsbNew_Click;
            // 
            // tsbDelete
            // 
            tsbDelete.Image = KlonsF.Properties.Resources.delete;
            tsbDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsbDelete.Name = "tsbDelete";
            tsbDelete.Size = new System.Drawing.Size(69, 25);
            tsbDelete.Text = "Dzēst";
            tsbDelete.Click += tsbDelete_Click;
            // 
            // tsbSave
            // 
            tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbSave.Image = (System.Drawing.Image)resources.GetObject("tsbSave.Image");
            tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsbSave.Name = "tsbSave";
            tsbSave.Size = new System.Drawing.Size(25, 25);
            tsbSave.Text = "Saglabāt";
            tsbSave.Click += tsbSave_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.AllowMerge = false;
            toolStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            toolStrip1.ImageScalingSize = new System.Drawing.Size(21, 21);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tslPeriod });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(710, 25);
            toolStrip1.TabIndex = 2;
            toolStrip1.Text = "toolStrip1";
            // 
            // tslPeriod
            // 
            tslPeriod.Name = "tslPeriod";
            tslPeriod.Size = new System.Drawing.Size(60, 22);
            tslPeriod.Text = "Periods:";
            // 
            // FormA_TimeSheets
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(710, 441);
            Controls.Add(dgvSar);
            Controls.Add(toolStrip1);
            Controls.Add(bnavSar);
            MyToolStrip = toolStrip1;
            Name = "FormA_TimeSheets";
            Text = "Darba laika uzskaites lapas";
            Load += Form_TimeSheets_Load;
            ((System.ComponentModel.ISupportInitialize)bnavSar).EndInit();
            bnavSar.ResumeLayout(false);
            bnavSar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bsSar).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvSar).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsDep).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private KlonsLIB.Components.MyBindingNavigator bnavSar;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private KlonsLIB.Components.MyDataGridView dgvSar;
        private KlonsLIB.Data.MyBindingSourceEf bsSar;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private KlonsLIB.Data.MyBindingSourceEf bsDep;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel tslPeriod;
        private System.Windows.Forms.ToolStripButton tsbOpen;
        private System.Windows.Forms.ToolStripButton tsbNew;
        private System.Windows.Forms.ToolStripButton tsbDelete;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcSarId;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcSarYr;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcSarMt;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcSarSNR;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcSarDescr;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgcSarDep;
    }
}