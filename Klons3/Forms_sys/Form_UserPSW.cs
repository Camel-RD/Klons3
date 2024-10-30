﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KlonsF.Classes;

namespace KlonsF.Forms
{
    public partial class Form_UserPSW : MyFormBaseF
    {
        public Form_UserPSW()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        public override string SelectedValueStr
        {
            get
            {
                return base.SelectedValueStr;
            }
            set
            {
                base.SelectedValueStr = value;
                myTextBox1.Text = value;
            }
        }

        private void myTextBox1_Leave(object sender, EventArgs e)
        {
            SelectedValueStr = myTextBox1.Text;
        }

        private void cmOK_Click(object sender, EventArgs e)
        {

        }

    }
}