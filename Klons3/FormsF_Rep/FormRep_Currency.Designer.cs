﻿using KlonsLIB.Components;
using KlonsLIB.Data;

namespace KlonsF.FormsReportParams
{
    partial class FormRep_Currency
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
            cbAC = new MyMcFlatComboBox();
            bsAC = new MyBindingSourceEf(components);
            tbSD = new MyTextBox();
            label1 = new System.Windows.Forms.Label();
            tbED = new MyTextBox();
            label2 = new System.Windows.Forms.Label();
            lbACName = new System.Windows.Forms.Label();
            cmDoIt = new System.Windows.Forms.Button();
            cbClid = new MyMcFlatComboBox();
            bsClid = new MyBindingSourceEf(components);
            label3 = new System.Windows.Forms.Label();
            lbClName = new System.Windows.Forms.Label();
            lbCm = new System.Windows.Forms.ListBox();
            tbCurrency = new MyTextBox();
            label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)bsAC).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsClid).BeginInit();
            SuspendLayout();
            // 
            // cbAC
            // 
            cbAC.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            cbAC.ColumnNames = new string[]
    {
    "ac",
    "name"
    };
            cbAC.ColumnWidths = "80;300";
            cbAC.DataSource = bsAC;
            cbAC.DisplayMember = "AC";
            cbAC.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cbAC.DropDownHeight = 270;
            cbAC.DropDownWidth = 399;
            cbAC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cbAC.FormattingEnabled = false;
            cbAC.GridLineColor = System.Drawing.Color.LightGray;
            cbAC.GridLineHorizontal = false;
            cbAC.GridLineVertical = false;
            cbAC.IntegralHeight = false;
            cbAC.Location = new System.Drawing.Point(145, 53);
            cbAC.Margin = new System.Windows.Forms.Padding(2);
            cbAC.MaxDropDownItems = 15;
            cbAC.Name = "cbAC";
            cbAC.Size = new System.Drawing.Size(104, 24);
            cbAC.TabIndex = 2;
            cbAC.ValueMember = "AC";
            cbAC.SelectedIndexChanged += cbAC_SelectedIndexChanged;
            cbAC.TextChanged += cbAC_TextChanged;
            cbAC.KeyDown += Control_KeyDown;
            cbAC.MouseDoubleClick += cbClid_MouseDoubleClick;
            // 
            // bsAC
            // 
            bsAC.MyDataSource = new KlonsLIB.ObjectSelector.KlonsDbSetSelector("KlonsData", "F_ACP21");
            bsAC.Sort = "AC";
            // 
            // tbSD
            // 
            tbSD.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            tbSD.IsDate = true;
            tbSD.Location = new System.Drawing.Point(145, 19);
            tbSD.Margin = new System.Windows.Forms.Padding(2);
            tbSD.Name = "tbSD";
            tbSD.Size = new System.Drawing.Size(80, 23);
            tbSD.TabIndex = 0;
            tbSD.KeyDown += Control_KeyDown;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(11, 19);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(124, 17);
            label1.TabIndex = 7;
            label1.Text = "Datums (no - līdz):";
            // 
            // tbED
            // 
            tbED.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            tbED.IsDate = true;
            tbED.Location = new System.Drawing.Point(230, 19);
            tbED.Margin = new System.Windows.Forms.Padding(2);
            tbED.Name = "tbED";
            tbED.Size = new System.Drawing.Size(80, 23);
            tbED.TabIndex = 1;
            tbED.KeyDown += Control_KeyDown;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(11, 55);
            label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(48, 17);
            label2.TabIndex = 8;
            label2.Text = "Konts:";
            // 
            // lbACName
            // 
            lbACName.Location = new System.Drawing.Point(253, 55);
            lbACName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            lbACName.Name = "lbACName";
            lbACName.Size = new System.Drawing.Size(334, 18);
            lbACName.TabIndex = 11;
            lbACName.Text = "Konts:";
            // 
            // cmDoIt
            // 
            cmDoIt.Location = new System.Drawing.Point(353, 171);
            cmDoIt.Margin = new System.Windows.Forms.Padding(2);
            cmDoIt.Name = "cmDoIt";
            cmDoIt.Size = new System.Drawing.Size(133, 63);
            cmDoIt.TabIndex = 5;
            cmDoIt.Text = "Taisīt atskaiti";
            cmDoIt.UseVisualStyleBackColor = true;
            cmDoIt.Click += cmDoIt_Click;
            cmDoIt.KeyDown += Control_KeyDown;
            // 
            // cbClid
            // 
            cbClid.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            cbClid.ColumnNames = new string[]
    {
    "clid",
    "name"
    };
            cbClid.ColumnWidths = "120;300";
            cbClid.DataSource = bsClid;
            cbClid.DisplayMember = "clid";
            cbClid.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cbClid.DropDownHeight = 270;
            cbClid.DropDownWidth = 439;
            cbClid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cbClid.FormattingEnabled = false;
            cbClid.GridLineColor = System.Drawing.Color.LightGray;
            cbClid.GridLineHorizontal = false;
            cbClid.GridLineVertical = false;
            cbClid.IntegralHeight = false;
            cbClid.Location = new System.Drawing.Point(145, 87);
            cbClid.Margin = new System.Windows.Forms.Padding(2);
            cbClid.MaxDropDownItems = 15;
            cbClid.Name = "cbClid";
            cbClid.Size = new System.Drawing.Size(104, 24);
            cbClid.TabIndex = 3;
            cbClid.ValueMember = "clid";
            cbClid.SelectedIndexChanged += cbClid_SelectedIndexChanged;
            cbClid.TextChanged += cbClid_TextChanged;
            cbClid.KeyDown += Control_KeyDown;
            cbClid.MouseDoubleClick += cbClid_MouseDoubleClick;
            // 
            // bsClid
            // 
            bsClid.MyDataSource = new KlonsLIB.ObjectSelector.KlonsDbSetSelector("KlonsData", "F_PERSONS");
            bsClid.Sort = "CLID";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(11, 91);
            label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(65, 17);
            label3.TabIndex = 9;
            label3.Text = "Persona:";
            // 
            // lbClName
            // 
            lbClName.Location = new System.Drawing.Point(253, 90);
            lbClName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            lbClName.Name = "lbClName";
            lbClName.Size = new System.Drawing.Size(334, 18);
            lbClName.TabIndex = 12;
            lbClName.Text = "Konts:";
            // 
            // lbCm
            // 
            lbCm.BackColor = System.Drawing.SystemColors.Control;
            lbCm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lbCm.FormattingEnabled = false;
            lbCm.ItemHeight = 16;
            lbCm.Items.AddRange(new object[] { "Valūtas kursu svārstību rezultāts (+/-)", "Apgrozijuma pārskats", "Apgrozijuma pārskats pa personām", "Konta korespondences pārskats", "Valūtu konvertācijas kļūdas" });
            lbCm.Location = new System.Drawing.Point(15, 171);
            lbCm.Margin = new System.Windows.Forms.Padding(2);
            lbCm.Name = "lbCm";
            lbCm.Size = new System.Drawing.Size(324, 98);
            lbCm.TabIndex = 6;
            lbCm.MouseDoubleClick += lbCm_MouseDoubleClick;
            // 
            // tbCurrency
            // 
            tbCurrency.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            tbCurrency.Location = new System.Drawing.Point(145, 123);
            tbCurrency.Margin = new System.Windows.Forms.Padding(2);
            tbCurrency.Name = "tbCurrency";
            tbCurrency.Size = new System.Drawing.Size(80, 23);
            tbCurrency.TabIndex = 4;
            tbCurrency.KeyDown += Control_KeyDown;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(11, 123);
            label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(93, 17);
            label4.TabIndex = 10;
            label4.Text = "Valūtas kods:";
            // 
            // FormRep_Currency
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(604, 282);
            CloseOnEscape = true;
            Controls.Add(lbCm);
            Controls.Add(cmDoIt);
            Controls.Add(lbClName);
            Controls.Add(lbACName);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label4);
            Controls.Add(label1);
            Controls.Add(tbCurrency);
            Controls.Add(tbED);
            Controls.Add(tbSD);
            Controls.Add(cbClid);
            Controls.Add(cbAC);
            Name = "FormRep_Currency";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Valūtas kontu pārskati";
            Load += FormRepKoresp1_Load;
            ((System.ComponentModel.ISupportInitialize)bsAC).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsClid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MyMcFlatComboBox cbAC;
        private MyBindingSourceEf bsAC;
        private MyTextBox tbSD;
        private System.Windows.Forms.Label label1;
        private MyTextBox tbED;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbACName;
        private System.Windows.Forms.Button cmDoIt;
        private MyMcFlatComboBox cbClid;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbClName;
        private MyBindingSourceEf bsClid;
        private System.Windows.Forms.ListBox lbCm;
        private MyTextBox tbCurrency;
        private System.Windows.Forms.Label label4;
    }
}