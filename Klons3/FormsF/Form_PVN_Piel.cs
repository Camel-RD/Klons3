using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Klons3.ModelsFRep;
using KlonsF.Classes;

namespace KlonsF.Forms
{
    public partial class Form_PVN_Piel : MyFormBaseF
    {
        public Form_PVN_Piel()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        private void Form_PVN_Piel_Load(object sender, EventArgs e)
        {
        }

        public void SetDataSource(List<F_ROPS1A> rows_rops1a)
        {
            dgvROps1a.DataSource = rows_rops1a;
        }

    }
}
