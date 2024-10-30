using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KlonsM.Classes;
using KlonsLIB.Forms;
using KlonsLIB.Data;
using KlonsF.Classes;
using Klons3.ModelsM;

namespace KlonsM.FormsM
{
    public partial class FormM_PVNRates : MyFormBaseF
    {
        public FormM_PVNRates()
        {
            InitializeComponent();
            CheckMyFontAndColors();
        }

        private void FormM_PVNRates_Load(object sender, EventArgs e)
        {

        }

        public M_PVNRATES SelectedPVNRate = null;

        public static M_PVNRATES GetPVNRate()
        {
            var fm = new FormM_PVNRates();
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return null;
            return (M_PVNRATES)fm.SelectedObject;
        }

        private void SelectCurrent()
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            var dr = bsPVNRate.GetCurrentItem<M_PVNRATES>();
            if (!dgvRows.EndEdit()) return;
            if (!SaveData()) return;
            if (bsPVNRate.IsItemDetached(dr)) return;
            SelectedPVNRate = dr;
            if (!this.IsMyDialog) return;
            SetSelectedObject(dr);
        }

        private void dgvRows_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvRows.CurrentCell == null) return;
            if (e.Control && e.KeyCode == Keys.Return)
            {
                SelectCurrent();
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Escape)
            {
                SelectedPVNRate = null;
                SetSelectedObject(null, true);
                e.Handled = true;
            }
        }

        private void dgvRows_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SelectedPVNRate = null;
                SetSelectedObject(null, true);
            }
        }

        private void dgvRows_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            if (e.ColumnIndex == dgcCode.Index)
            {
                SelectCurrent();
                return;
            }

        }
    }
}
