using KlonsLIB.Components;

namespace KlonsA.Forms
{
    partial class FormA_Persons
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormA_Persons));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            bsPersons = new KlonsLIB.Data.MyBindingSourceEf(components);
            dgvPersons = new MyDataGridView();
            dgcID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcFName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcLName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcBirthDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcPK = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcUsed = new MyDgvCheckBoxColumn();
            dgcUsedDt1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcUsedDt2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            bnavNav = new MyBindingNavigator();
            bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            tslLabel = new System.Windows.Forms.ToolStripLabel();
            bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            tsbSave = new System.Windows.Forms.ToolStripButton();
            tbSearch = new MyTextBox();
            tbID = new MyPickRowTextBox();
            cbActive = new MyMcFlatComboBox();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            dgvAmati = new MyDataGridView();
            dgcAmatiID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcAmatiTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcAmatiDep = new System.Windows.Forms.DataGridViewComboBoxColumn();
            bsDep = new KlonsLIB.Data.MyBindingSourceEf(components);
            dgcAmatiUsed = new MyDgvCheckBoxColumn();
            dgcAmatiUsedDt1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgcAmatiUsedDt2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            bsAmati = new KlonsLIB.Data.MyBindingSourceEf2(components);
            ((System.ComponentModel.ISupportInitialize)bsPersons).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvPersons).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bnavNav).BeginInit();
            bnavNav.SuspendLayout();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAmati).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsDep).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsAmati).BeginInit();
            SuspendLayout();
            // 
            // bsPersons
            // 
            bsPersons.MyDataSource = new KlonsLIB.ObjectSelector.KlonsDbSetSelector("KlonsAData", "A_PERSONS");
            bsPersons.Sort = "LNAME, FNAME";
            bsPersons.UseDataGridView = dgvPersons;
            bsPersons.CurrentChanged += bsPersons_CurrentChanged;
            bsPersons.ListChanged += bsPersons_ListChanged;
            // 
            // dgvPersons
            // 
            dgvPersons.AllowUserToDeleteRows = false;
            dgvPersons.AllowUserToResizeRows = false;
            dgvPersons.AutoGenerateColumns = false;
            dgvPersons.AutoSave = false;
            dgvPersons.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 186);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            dgvPersons.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvPersons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPersons.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { dgcID, dgcFName, dgcLName, dgcBirthDate, dgcPK, dgcUsed, dgcUsedDt1, dgcUsedDt2 });
            dgvPersons.DataSource = bsPersons;
            dgvPersons.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvPersons.Location = new System.Drawing.Point(0, 0);
            dgvPersons.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            dgvPersons.Name = "dgvPersons";
            dgvPersons.RowHeadersWidth = 62;
            dgvPersons.Size = new System.Drawing.Size(786, 335);
            dgvPersons.TabIndex = 3;
            dgvPersons.MyCheckForChanges += dgvPersons_MyCheckForChanges;
            dgvPersons.CellDoubleClick += dgvPersons_CellDoubleClick;
            dgvPersons.CellParsing += dgvPersons_CellParsing;
            dgvPersons.CurrentCellChanged += dgvPersons_CurrentCellChanged;
            dgvPersons.UserDeletingRow += dgvPersons_UserDeletingRow;
            dgvPersons.Enter += dgvPersons_Enter;
            dgvPersons.KeyDown += dgvPersons_KeyDown;
            dgvPersons.KeyPress += dgvPersons_KeyPress;
            // 
            // dgcID
            // 
            dgcID.DataPropertyName = "ID";
            dgcID.HeaderText = "ID";
            dgcID.MinimumWidth = 9;
            dgcID.Name = "dgcID";
            dgcID.Visible = false;
            dgcID.Width = 90;
            // 
            // dgcFName
            // 
            dgcFName.DataPropertyName = "FNAME";
            dgcFName.HeaderText = "vārds";
            dgcFName.MinimumWidth = 9;
            dgcFName.Name = "dgcFName";
            dgcFName.Width = 108;
            // 
            // dgcLName
            // 
            dgcLName.DataPropertyName = "LNAME";
            dgcLName.HeaderText = "uzvārds";
            dgcLName.MinimumWidth = 9;
            dgcLName.Name = "dgcLName";
            dgcLName.Width = 108;
            // 
            // dgcBirthDate
            // 
            dgcBirthDate.DataPropertyName = "BIRTH_DATE";
            dataGridViewCellStyle2.Format = "dd.MM.yyyy";
            dgcBirthDate.DefaultCellStyle = dataGridViewCellStyle2;
            dgcBirthDate.HeaderText = "dzimš. dat.";
            dgcBirthDate.MinimumWidth = 9;
            dgcBirthDate.Name = "dgcBirthDate";
            dgcBirthDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dgcBirthDate.ToolTipText = "dzimšanas datums";
            dgcBirthDate.Width = 85;
            // 
            // dgcPK
            // 
            dgcPK.DataPropertyName = "PK";
            dgcPK.HeaderText = "pers.kods";
            dgcPK.MinimumWidth = 9;
            dgcPK.Name = "dgcPK";
            dgcPK.Width = 126;
            // 
            // dgcUsed
            // 
            dgcUsed.DataPropertyName = "USED";
            dgcUsed.FalseValue = "0";
            dgcUsed.HeaderText = "aktīvs";
            dgcUsed.MinimumWidth = 9;
            dgcUsed.Name = "dgcUsed";
            dgcUsed.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            dgcUsed.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            dgcUsed.TrueValue = "1";
            dgcUsed.Width = 54;
            // 
            // dgcUsedDt1
            // 
            dgcUsedDt1.DataPropertyName = "USED_DT1";
            dataGridViewCellStyle3.Format = "dd.MM.yyyy";
            dgcUsedDt1.DefaultCellStyle = dataGridViewCellStyle3;
            dgcUsedDt1.HeaderText = "akrīvs no";
            dgcUsedDt1.MinimumWidth = 9;
            dgcUsedDt1.Name = "dgcUsedDt1";
            dgcUsedDt1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dgcUsedDt1.Width = 95;
            // 
            // dgcUsedDt2
            // 
            dgcUsedDt2.DataPropertyName = "USED_DT2";
            dataGridViewCellStyle4.Format = "dd.MM.yyyy";
            dgcUsedDt2.DefaultCellStyle = dataGridViewCellStyle4;
            dgcUsedDt2.HeaderText = "aktīvs līdz";
            dgcUsedDt2.MinimumWidth = 9;
            dgcUsedDt2.Name = "dgcUsedDt2";
            dgcUsedDt2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dgcUsedDt2.Width = 85;
            // 
            // bnavNav
            // 
            bnavNav.AddNewItem = bindingNavigatorAddNewItem;
            bnavNav.BindingSource = bsPersons;
            bnavNav.CountItem = bindingNavigatorCountItem;
            bnavNav.CountItemFormat = " no {0}";
            bnavNav.DeleteItem = null;
            bnavNav.Dock = System.Windows.Forms.DockStyle.Bottom;
            bnavNav.ImageScalingSize = new System.Drawing.Size(23, 26);
            bnavNav.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tslLabel, bindingNavigatorMoveFirstItem, bindingNavigatorMovePreviousItem, bindingNavigatorSeparator, bindingNavigatorPositionItem, bindingNavigatorCountItem, bindingNavigatorSeparator1, bindingNavigatorMoveNextItem, bindingNavigatorMoveLastItem, bindingNavigatorSeparator2, bindingNavigatorAddNewItem, bindingNavigatorDeleteItem, tsbSave });
            bnavNav.Location = new System.Drawing.Point(0, 497);
            bnavNav.MoveFirstItem = bindingNavigatorMoveFirstItem;
            bnavNav.MoveLastItem = bindingNavigatorMoveLastItem;
            bnavNav.MoveNextItem = bindingNavigatorMoveNextItem;
            bnavNav.MovePreviousItem = bindingNavigatorMovePreviousItem;
            bnavNav.Name = "bnavNav";
            bnavNav.PositionItem = bindingNavigatorPositionItem;
            bnavNav.SaveItem = null;
            bnavNav.Size = new System.Drawing.Size(786, 37);
            bnavNav.TabIndex = 3;
            bnavNav.Text = "myBindingNavigator1";
            bnavNav.ItemDeleting += bnavNav_ItemDeleting;
            // 
            // bindingNavigatorAddNewItem
            // 
            bindingNavigatorAddNewItem.Image = (System.Drawing.Image)resources.GetObject("bindingNavigatorAddNewItem.Image");
            bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            bindingNavigatorAddNewItem.Size = new System.Drawing.Size(70, 34);
            bindingNavigatorAddNewItem.Text = "Jauns";
            // 
            // bindingNavigatorCountItem
            // 
            bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            bindingNavigatorCountItem.Size = new System.Drawing.Size(49, 34);
            bindingNavigatorCountItem.Text = " no {0}";
            bindingNavigatorCountItem.ToolTipText = "Ierakstu skaits";
            // 
            // tslLabel
            // 
            tslLabel.Name = "tslLabel";
            tslLabel.Size = new System.Drawing.Size(73, 34);
            tslLabel.Text = "Darbinieki:";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            bindingNavigatorMoveFirstItem.Image = (System.Drawing.Image)resources.GetObject("bindingNavigatorMoveFirstItem.Image");
            bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(27, 34);
            bindingNavigatorMoveFirstItem.Text = "Iet uz pirmo";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            bindingNavigatorMovePreviousItem.Image = (System.Drawing.Image)resources.GetObject("bindingNavigatorMovePreviousItem.Image");
            bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(27, 34);
            bindingNavigatorMovePreviousItem.Text = "Iet uz iepriekšējo";
            // 
            // bindingNavigatorSeparator
            // 
            bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 37);
            // 
            // bindingNavigatorPositionItem
            // 
            bindingNavigatorPositionItem.AutoSize = false;
            bindingNavigatorPositionItem.Font = new System.Drawing.Font("Segoe UI", 11F);
            bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            bindingNavigatorPositionItem.Size = new System.Drawing.Size(56, 37);
            bindingNavigatorPositionItem.Text = "0";
            bindingNavigatorPositionItem.ToolTipText = "Pašreizējā pozīcija";
            // 
            // bindingNavigatorSeparator1
            // 
            bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 37);
            // 
            // bindingNavigatorMoveNextItem
            // 
            bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            bindingNavigatorMoveNextItem.Image = (System.Drawing.Image)resources.GetObject("bindingNavigatorMoveNextItem.Image");
            bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(27, 34);
            bindingNavigatorMoveNextItem.Text = "Iet uz nākošo";
            // 
            // bindingNavigatorMoveLastItem
            // 
            bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            bindingNavigatorMoveLastItem.Image = (System.Drawing.Image)resources.GetObject("bindingNavigatorMoveLastItem.Image");
            bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(27, 34);
            bindingNavigatorMoveLastItem.Text = "Iet uz pēdējo";
            // 
            // bindingNavigatorSeparator2
            // 
            bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 37);
            // 
            // bindingNavigatorDeleteItem
            // 
            bindingNavigatorDeleteItem.Image = (System.Drawing.Image)resources.GetObject("bindingNavigatorDeleteItem.Image");
            bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            bindingNavigatorDeleteItem.Size = new System.Drawing.Size(70, 34);
            bindingNavigatorDeleteItem.Text = "Dzēst";
            bindingNavigatorDeleteItem.Click += bindingNavigatorDeleteItem_Click;
            // 
            // tsbSave
            // 
            tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbSave.Image = (System.Drawing.Image)resources.GetObject("tsbSave.Image");
            tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsbSave.Name = "tsbSave";
            tsbSave.Size = new System.Drawing.Size(27, 34);
            tsbSave.Text = "toolStripButton1";
            tsbSave.Click += tsbSave_Click;
            // 
            // tbSearch
            // 
            tbSearch.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            tbSearch.Location = new System.Drawing.Point(278, 0);
            tbSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tbSearch.Name = "tbSearch";
            tbSearch.Size = new System.Drawing.Size(134, 23);
            tbSearch.TabIndex = 1;
            tbSearch.Enter += tbSearch_Enter;
            tbSearch.KeyPress += tbSearch_KeyPress;
            // 
            // tbID
            // 
            tbID.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            tbID.DataPropertyName = "LNAME";
            tbID.DataMember = null;
            tbID.DataSource = bsPersons;
            tbID.Location = new System.Drawing.Point(168, 0);
            tbID.Margin = new System.Windows.Forms.Padding(2);
            tbID.Name = "tbID";
            tbID.Size = new System.Drawing.Size(100, 23);
            tbID.TabIndex = 0;
            tbID.RowSelectedEvent += tbID_RowSelectedEvent;
            tbID.Enter += tbID_Enter;
            // 
            // cbActive
            // 
            cbActive.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            cbActive.ColumnWidths = "66";
            cbActive.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cbActive.DropDownHeight = 144;
            cbActive.DropDownStyle = MyMcComboBox.CustomDropDownStyle.DropDownListSimple;
            cbActive.DropDownWidth = 85;
            cbActive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cbActive.GridLineColor = System.Drawing.Color.LightGray;
            cbActive.GridLineHorizontal = false;
            cbActive.GridLineVertical = false;
            cbActive.IntegralHeight = false;
            cbActive.Items.AddRange(new object[] { "Aktīvie", "Visi" });
            cbActive.Location = new System.Drawing.Point(422, 0);
            cbActive.Margin = new System.Windows.Forms.Padding(2);
            cbActive.Name = "cbActive";
            cbActive.Size = new System.Drawing.Size(101, 24);
            cbActive.TabIndex = 2;
            cbActive.SelectedIndexChanged += cbActive_SelectedIndexChanged;
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new System.Drawing.Size(23, 26);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripLabel1, toolStripLabel2 });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Padding = new System.Windows.Forms.Padding(0, 1, 1, 1);
            toolStrip1.Size = new System.Drawing.Size(786, 25);
            toolStrip1.TabIndex = 4;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new System.Drawing.Size(58, 20);
            toolStripLabel1.Text = " Meklēt:";
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.Name = "toolStripLabel2";
            toolStripLabel2.Size = new System.Drawing.Size(21, 20);
            toolStripLabel2.Text = "   ";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 25);
            splitContainer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(dgvPersons);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(dgvAmati);
            splitContainer1.Size = new System.Drawing.Size(786, 472);
            splitContainer1.SplitterDistance = 335;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 2;
            // 
            // dgvAmati
            // 
            dgvAmati.AllowUserToDeleteRows = false;
            dgvAmati.AutoGenerateColumns = false;
            dgvAmati.AutoSave = false;
            dgvAmati.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 186);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            dgvAmati.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            dgvAmati.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvAmati.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { dgcAmatiID, dgcAmatiTitle, dgcAmatiDep, dgcAmatiUsed, dgcAmatiUsedDt1, dgcAmatiUsedDt2 });
            dgvAmati.DataSource = bsAmati;
            dgvAmati.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvAmati.Location = new System.Drawing.Point(0, 0);
            dgvAmati.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            dgvAmati.Name = "dgvAmati";
            dgvAmati.RowHeadersWidth = 62;
            dgvAmati.Size = new System.Drawing.Size(786, 132);
            dgvAmati.TabIndex = 4;
            dgvAmati.MyKeyDown += dgvAmati_MyKeyDown;
            dgvAmati.CellDoubleClick += dgvAmati_CellDoubleClick;
            dgvAmati.CellParsing += dgvAmati_CellParsing;
            dgvAmati.CellValidating += dgvAmati_CellValidating;
            dgvAmati.UserDeletingRow += dgvAmati_UserDeletingRow;
            dgvAmati.Enter += dgvAmati_Enter;
            dgvAmati.KeyDown += dgvAmati_KeyDown;
            dgvAmati.KeyPress += dgvAmati_KeyPress;
            // 
            // dgcAmatiID
            // 
            dgcAmatiID.DataPropertyName = "ID";
            dgcAmatiID.HeaderText = "ID";
            dgcAmatiID.MinimumWidth = 9;
            dgcAmatiID.Name = "dgcAmatiID";
            dgcAmatiID.Visible = false;
            dgcAmatiID.Width = 168;
            // 
            // dgcAmatiTitle
            // 
            dgcAmatiTitle.DataPropertyName = "TITLE";
            dgcAmatiTitle.HeaderText = "amata nosaukums";
            dgcAmatiTitle.MinimumWidth = 9;
            dgcAmatiTitle.Name = "dgcAmatiTitle";
            dgcAmatiTitle.Width = 225;
            // 
            // dgcAmatiDep
            // 
            dgcAmatiDep.DataPropertyName = "Department";
            dgcAmatiDep.DataSource = bsDep;
            dgcAmatiDep.DisplayMember = "ID";
            dgcAmatiDep.DisplayStyleForCurrentCellOnly = true;
            dgcAmatiDep.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            dgcAmatiDep.HeaderText = "struktūrv.";
            dgcAmatiDep.MinimumWidth = 9;
            dgcAmatiDep.Name = "dgcAmatiDep";
            dgcAmatiDep.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            dgcAmatiDep.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            dgcAmatiDep.ValueMember = "Me";
            dgcAmatiDep.Width = 225;
            // 
            // bsDep
            // 
            bsDep.MyDataSource = new KlonsLIB.ObjectSelector.KlonsDbSetSelector("KlonsAData", "A_DEPARTMENTS");
            // 
            // dgcAmatiUsed
            // 
            dgcAmatiUsed.DataPropertyName = "USED";
            dgcAmatiUsed.FalseValue = "0";
            dgcAmatiUsed.HeaderText = "aktīvs";
            dgcAmatiUsed.MinimumWidth = 9;
            dgcAmatiUsed.Name = "dgcAmatiUsed";
            dgcAmatiUsed.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            dgcAmatiUsed.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            dgcAmatiUsed.TrueValue = "1";
            dgcAmatiUsed.Width = 67;
            // 
            // dgcAmatiUsedDt1
            // 
            dgcAmatiUsedDt1.DataPropertyName = "USED_DT1";
            dataGridViewCellStyle6.Format = "dd.MM.yyyy";
            dgcAmatiUsedDt1.DefaultCellStyle = dataGridViewCellStyle6;
            dgcAmatiUsedDt1.HeaderText = "aktīvs no";
            dgcAmatiUsedDt1.MinimumWidth = 9;
            dgcAmatiUsedDt1.Name = "dgcAmatiUsedDt1";
            dgcAmatiUsedDt1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dgcAmatiUsedDt1.Width = 85;
            // 
            // dgcAmatiUsedDt2
            // 
            dgcAmatiUsedDt2.DataPropertyName = "USED_DT2";
            dataGridViewCellStyle7.Format = "dd.MM.yyyy";
            dgcAmatiUsedDt2.DefaultCellStyle = dataGridViewCellStyle7;
            dgcAmatiUsedDt2.HeaderText = "aktīvs līdz";
            dgcAmatiUsedDt2.MinimumWidth = 9;
            dgcAmatiUsedDt2.Name = "dgcAmatiUsedDt2";
            dgcAmatiUsedDt2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dgcAmatiUsedDt2.Width = 85;
            // 
            // bsAmati
            // 
            bsAmati.DataMember = "PositionsView";
            bsAmati.DataSource = bsPersons;
            bsAmati.ListChanged += bsAmati_ListChanged;
            // 
            // FormA_Persons
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(786, 534);
            Controls.Add(splitContainer1);
            Controls.Add(cbActive);
            Controls.Add(tbID);
            Controls.Add(tbSearch);
            Controls.Add(toolStrip1);
            Controls.Add(bnavNav);
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "FormA_Persons";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Darbinieku saraksts";
            FormClosed += Form_Persons_FormClosed;
            Load += Form_Persons_Load;
            ((System.ComponentModel.ISupportInitialize)bsPersons).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvPersons).EndInit();
            ((System.ComponentModel.ISupportInitialize)bnavNav).EndInit();
            bnavNav.ResumeLayout(false);
            bnavNav.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvAmati).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsDep).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsAmati).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private MyBindingNavigator bnavNav;
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
        private KlonsLIB.Data.MyBindingSourceEf bsPersons;
        private MyTextBox tbSearch;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private MyPickRowTextBox tbID;
        private MyMcFlatComboBox cbActive;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private MyDataGridView dgvPersons;
        private MyDataGridView dgvAmati;
        private KlonsLIB.Data.MyBindingSourceEf2 bsAmati;
        private System.Windows.Forms.ToolStripLabel tslLabel;
        private KlonsLIB.Data.MyBindingSourceEf bsDep;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcFName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcLName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcBirthDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcPK;
        private MyDgvCheckBoxColumn dgcUsed;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcUsedDt1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcUsedDt2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcAmatiID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcAmatiTitle;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgcAmatiDep;
        private MyDgvCheckBoxColumn dgcAmatiUsed;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcAmatiUsedDt1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcAmatiUsedDt2;
    }
}