using KlonsLIB.Components;

namespace KlonsLIB.Forms
{
    partial class Form_Error2
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
            tbMsg = new MyTextBox();
            tbDescr = new FlatRichTextBox();
            cmClose = new Button();
            pictureBox1 = new PictureBox();
            cmDetails = new Button();
            cmRollBack = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // tbMsg
            // 
            tbMsg.BackColor = SystemColors.Control;
            tbMsg.BorderColor = SystemColors.Control;
            tbMsg.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold, GraphicsUnit.Point, 186);
            tbMsg.Location = new Point(59, 5);
            tbMsg.Margin = new Padding(3, 2, 3, 2);
            tbMsg.Multiline = true;
            tbMsg.Name = "tbMsg";
            tbMsg.ReadOnly = true;
            tbMsg.Size = new Size(349, 116);
            tbMsg.TabIndex = 3;
            // 
            // tbDescr
            // 
            tbDescr.BackColor = SystemColors.Control;
            tbDescr.BorderColor = SystemColors.ControlDarkDark;
            tbDescr.BorderStyle = BorderStyle.None;
            tbDescr.Location = new Point(3, 130);
            tbDescr.Margin = new Padding(3, 2, 3, 2);
            tbDescr.Name = "tbDescr";
            tbDescr.ReadOnly = true;
            tbDescr.Size = new Size(501, 177);
            tbDescr.TabIndex = 4;
            tbDescr.Text = "";
            tbDescr.WordWrap = false;
            // 
            // cmClose
            // 
            cmClose.DialogResult = DialogResult.Cancel;
            cmClose.Location = new Point(417, 2);
            cmClose.Margin = new Padding(3, 2, 3, 2);
            cmClose.Name = "cmClose";
            cmClose.Size = new Size(87, 34);
            cmClose.TabIndex = 0;
            cmClose.Text = "Aizvērt";
            cmClose.UseVisualStyleBackColor = true;
            cmClose.Click += cmClose_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Resource1.error3;
            pictureBox1.Location = new Point(5, 10);
            pictureBox1.Margin = new Padding(3, 2, 3, 2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(48, 48);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // cmDetails
            // 
            cmDetails.Location = new Point(417, 41);
            cmDetails.Margin = new Padding(3, 2, 3, 2);
            cmDetails.Name = "cmDetails";
            cmDetails.Size = new Size(87, 34);
            cmDetails.TabIndex = 1;
            cmDetails.Text = "Detaļas";
            cmDetails.UseVisualStyleBackColor = true;
            cmDetails.Click += cmDetails_Click;
            // 
            // cmRollBack
            // 
            cmRollBack.Location = new Point(417, 80);
            cmRollBack.Margin = new Padding(3, 2, 3, 2);
            cmRollBack.Name = "cmRollBack";
            cmRollBack.Size = new Size(87, 44);
            cmRollBack.TabIndex = 2;
            cmRollBack.Text = "Atcelt izmaiņas";
            cmRollBack.UseVisualStyleBackColor = true;
            cmRollBack.Visible = false;
            cmRollBack.Click += cmRollBack_Click;
            // 
            // Form_Error2
            // 
            AutoScaleDimensions = new SizeF(8F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cmClose;
            ClientSize = new Size(508, 310);
            Controls.Add(pictureBox1);
            Controls.Add(cmRollBack);
            Controls.Add(cmDetails);
            Controls.Add(cmClose);
            Controls.Add(tbDescr);
            Controls.Add(tbMsg);
            Font = new Font("Microsoft Sans Serif", 10F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form_Error2";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Kļūda!";
            Load += FormError_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MyTextBox tbMsg;
        private FlatRichTextBox tbDescr;
        private System.Windows.Forms.Button cmClose;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button cmDetails;
        private Button cmRollBack;
    }
}