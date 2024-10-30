using System.Drawing;
using System.Windows.Forms;

namespace KlonsF
{
    partial class Form_SomeTools
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            menuStrip1 = new MenuStrip();
            tasksToolStripMenuItem = new ToolStripMenuItem();
            miGetDbTableNames = new ToolStripMenuItem();
            miAddTsFields = new ToolStripMenuItem();
            miListUseeTable = new ToolStripMenuItem();
            miReportsProcess1 = new ToolStripMenuItem();
            miReportsProcess2 = new ToolStripMenuItem();
            miGenerateXSD = new ToolStripMenuItem();
            eFReloadAllDocsToolStripMenuItem = new ToolStripMenuItem();
            richTextBox1 = new RichTextBox();
            panel1 = new Panel();
            myBindingSourceEf1 = new KlonsLIB.Data.MyBindingSourceEf(components);
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)myBindingSourceEf1).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { tasksToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(4, 1, 0, 1);
            menuStrip1.Size = new Size(533, 25);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // tasksToolStripMenuItem
            // 
            tasksToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { miGetDbTableNames, miAddTsFields, miListUseeTable, miReportsProcess1, miReportsProcess2, miGenerateXSD, eFReloadAllDocsToolStripMenuItem });
            tasksToolStripMenuItem.Name = "tasksToolStripMenuItem";
            tasksToolStripMenuItem.Size = new Size(52, 23);
            tasksToolStripMenuItem.Text = "Tasks";
            // 
            // miGetDbTableNames
            // 
            miGetDbTableNames.Name = "miGetDbTableNames";
            miGetDbTableNames.Size = new Size(228, 24);
            miGetDbTableNames.Text = "Get Db Table Names";
            miGetDbTableNames.Click += miGetDbTableNames_Click;
            // 
            // miAddTsFields
            // 
            miAddTsFields.Name = "miAddTsFields";
            miAddTsFields.Size = new Size(228, 24);
            miAddTsFields.Text = "Add Ts Fields To Tables";
            miAddTsFields.Click += miAddTsFields_Click;
            // 
            // miListUseeTable
            // 
            miListUseeTable.Name = "miListUseeTable";
            miListUseeTable.Size = new Size(228, 24);
            miListUseeTable.Text = "Reports: List Used Tables";
            miListUseeTable.Click += miListUseeTable_Click;
            // 
            // miReportsProcess1
            // 
            miReportsProcess1.Name = "miReportsProcess1";
            miReportsProcess1.Size = new Size(228, 24);
            miReportsProcess1.Text = "Reports: Process 1";
            miReportsProcess1.Click += miReportsProcess1_Click;
            // 
            // miReportsProcess2
            // 
            miReportsProcess2.Name = "miReportsProcess2";
            miReportsProcess2.Size = new Size(228, 24);
            miReportsProcess2.Text = "Reports: Process 2";
            miReportsProcess2.Click += miReportsProcess2_Click;
            // 
            // miGenerateXSD
            // 
            miGenerateXSD.Name = "miGenerateXSD";
            miGenerateXSD.Size = new Size(228, 24);
            miGenerateXSD.Text = "Generate XSD";
            miGenerateXSD.Click += miGenerateXSD_Click;
            // 
            // eFReloadAllDocsToolStripMenuItem
            // 
            eFReloadAllDocsToolStripMenuItem.Name = "eFReloadAllDocsToolStripMenuItem";
            eFReloadAllDocsToolStripMenuItem.Size = new Size(228, 24);
            eFReloadAllDocsToolStripMenuItem.Text = "EF reload all docs";
            eFReloadAllDocsToolStripMenuItem.Click += eFReloadAllDocsToolStripMenuItem_Click;
            // 
            // richTextBox1
            // 
            richTextBox1.Dock = DockStyle.Fill;
            richTextBox1.Location = new Point(0, 25);
            richTextBox1.Margin = new Padding(2);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(533, 166);
            richTextBox1.TabIndex = 1;
            richTextBox1.Text = "";
            // 
            // panel1
            // 
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 191);
            panel1.Name = "panel1";
            panel1.Size = new Size(533, 49);
            panel1.TabIndex = 2;
            // 
            // myBindingSourceEf1
            // 
            myBindingSourceEf1.MyDataSource = new KlonsLIB.ObjectSelector.KlonsDbSetSelector("KlonsData", "F_OPSD");
            // 
            // Form_SomeTools
            // 
            AutoScaleDimensions = new SizeF(8F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(533, 240);
            Controls.Add(richTextBox1);
            Controls.Add(panel1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(2);
            Name = "Form_SomeTools";
            Text = "Form1";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)myBindingSourceEf1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem tasksToolStripMenuItem;
        private ToolStripMenuItem miGetDbTableNames;
        private RichTextBox richTextBox1;
        private ToolStripMenuItem miAddTsFields;
        private ToolStripMenuItem miListUseeTable;
        private ToolStripMenuItem miReportsProcess1;
        private ToolStripMenuItem miGenerateXSD;
        private Panel panel1;
        private ToolStripMenuItem miReportsProcess2;
        private ToolStripMenuItem eFReloadAllDocsToolStripMenuItem;
        private KlonsLIB.Data.MyBindingSourceEf myBindingSourceEf1;
    }
}
