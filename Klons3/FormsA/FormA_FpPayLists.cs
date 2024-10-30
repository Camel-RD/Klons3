using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KlonsA.Classes;
using KlonsF.Classes;
using KlonsLIB.Data;
using KlonsLIB.Forms;
using KlonsLIB.Misc;
using DataObjectsA;
using Klons3.ModelsA;
using Equin.ApplicationFramework;

namespace KlonsA.Forms
{
    public partial class FormA_FpPayLists : MyFormBaseF
    {
        public FormA_FpPayLists()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            CheckDgvRowsAllowAddRows();

            try
            {
                MakeGrid();
            }
            catch (Exception ex)
            {
                Form_Error.ShowException(ex);
            }
        }

        private void CheckDgvRowsAllowAddRows()
        {
            dgvRows.AllowUserToAddRows = MyData.DbContextA.BL_A_PERSONS_FIZ.Count > 0;
        }

        private void Form_FpPayLists_Load(object sender, EventArgs e)
        {
            SetupToolStrips();

            dgcRowsTaxTP.DataSource = SomeDataDefsA.FPMaksNodVeids;
            dgcRowsTaxTP.DisplayMember = "Val";
            dgcRowsTaxTP.ValueMember = "Key";

            dgcRowsSIRateTP.DataSource = SomeDataDefsA.FPMaksSILikmesVeids;
            dgcRowsSIRateTP.DisplayMember = "Val";
            dgcRowsSIRateTP.ValueMember = "Key";

            ShowPayLists(MyData.Settings.ShowFpPayLists);
            ShowPayDataPanel(MyData.Settings.ShowFpPayDataPanel);
            tslPeriod.Text = DataLoaderA.GetPeriodStr();
            BindingListView<A_FP_PAYLISTS_R>.ListItemPropertyChangedGlobal += BLV_FP_PAYLISTS_R_ListItemPropertyChangedGlobal;
            rwSAITitle.Expanded = false;
            CheckEnableRows();
            CheckSave();
        }

        private void FormA_FpPayLists_Shown(object sender, System.EventArgs e)
        {
            if (bsPersons.Count == 0)
                MyMainForm.ShowWarning("Fizisko personu saraksts ir tukšs.");
        }

        private void BLV_FP_PAYLISTS_R_ListItemPropertyChangedGlobal(object sender, MyItemPropertyChangedEventArgs e)
        {
            if (sender == bsRows.List)
            {
                FP_PAYLISTS_R_ColumnChanged(e);
            }
        }

        private void Form_FpPayLists_FormClosed(object sender, FormClosedEventArgs e)
        {
            BindingListView<A_FP_PAYLISTS_R>.ListItemPropertyChangedGlobal -= BLV_FP_PAYLISTS_R_ListItemPropertyChangedGlobal;
        }


        private bool ignoreColumnChangeEvent = false;
        private bool InPAYLISTS_R_ColumnChanged = false;

        private void FP_PAYLISTS_R_ColumnChanged(MyItemPropertyChangedEventArgs e)
        {
            if (ignoreColumnChangeEvent) return;
            if (InPAYLISTS_R_ColumnChanged) return;
            InPAYLISTS_R_ColumnChanged = true;
            FP_PAYLISTS_R_ColumnChangedA(e);
            InPAYLISTS_R_ColumnChanged = false;
        }


        public void GetRates(DateTime dt, int rtp, out decimal riin,
            out decimal rsidd, out decimal rsidn)
        {
            riin = 0.0M;
            rsidd = 0.0M;
            rsidn = 0.0M;

            var dr = DataTasksA.GetRates(dt);
            if (dr == null) return;
            riin = dr.IIN_LIKME;
            switch (rtp)
            {
                case 0:
                    rsidd = dr.SIDD_PAMATLIKME;
                    rsidn = dr.SIDN_PAMATLIKME;
                    break;
                case 1:
                    rsidd = dr.SIDD_PENS;
                    rsidn = dr.SIDN_PENS;
                    break;
                case 2:
                    rsidd = dr.SIDD_IZDPENS;
                    rsidn = dr.SIDN_IZDPENS;
                    break;
                case 3:
                    rsidd = dr.SIDD_IESLODZ;
                    rsidn = dr.SIDN_IESLODZ;
                    break;
                case 4:
                    rsidd = dr.SIDD_IESLODZ_PENS;
                    rsidn = dr.SIDN_IESLODZ_PENS;
                    break;
            }

        }

        public void FullRecalc(A_FP_PAYLISTS_R dr, bool useiinrate = false)
        {
            decimal riin = 0M, rsidd = 0M, rsidn = 0M;
            decimal pay = 0M, sidd = 0M, sidn = 0M, iinex = 0m;
            decimal iinfrom = 0M, iin = 0M, cash = 0M;

            GetRates(dr.PAYDATE, dr.SIRATETP, out riin, out rsidd, out rsidn);
            if (useiinrate || dr.IIN_RATE != 0.0M)
                riin = dr.IIN_RATE;
            pay = dr.PAY0;
            if (dr.TAX_TP == 0)
            {
                sidd = Math.Round(pay * rsidd / 100.0M, 2);
                sidn = Math.Round(pay * rsidn / 100.0M, 2);
            }
            if (dr.TAX_TP != 2)
            {
                if (dr.IINEX_PERC == 0.0M)
                    iinex = dr.IINEX;
                else
                    iinex = Math.Round(dr.IINEX_PERC / 100.0M * pay, 2);
                iinfrom = pay - sidn - iinex;
                iin = Math.Round(iinfrom * riin / 100.0M, 2);
            }
            cash = pay - sidn - iin;

            if (dr.SIDD == sidd && dr.SIDN == sidn && dr.IINEX == iinex &&
                dr.IIN == iin && dr.CASH == cash) return;

            if (dr.SIRATEDD != rsidd) dr.SIRATEDD = rsidd;
            if (dr.SIRATEDN != rsidn) dr.SIRATEDN = rsidn;
            if (dr.IIN_RATE != riin) dr.IIN_RATE = riin;
            if (dr.SIDD != sidd) dr.SIDD = sidd;
            if (dr.SIDN != sidd) dr.SIDN = sidn;
            if (dr.IINEX != iinex) dr.IINEX = iinex;
            if (dr.IIN_FROM != iinfrom) dr.IIN_FROM = iinfrom;
            if (dr.IIN != iin) dr.IIN = iin;
            if (dr.CASH != cash) dr.CASH = cash;

            dgvList.RefreshCurrent();
        }

        private void FP_PAYLISTS_R_ColumnChangedA(MyItemPropertyChangedEventArgs e)
        {
            var dr = e.Item as A_FP_PAYLISTS_R;
            var table = MyData.DbContextA.BL_A_FP_PAYLISTS_R;

            bool dorecalc = false;
            bool userate = false;
            decimal iinexperc = 0.0M;
            decimal iinrate = 23.0M;

            if (e.PropertyName == nameof(A_FP_PAYLISTS_R.INCOME_ID))
            {
                if (dr.INCOME_ID == "1003")
                {
                    iinexperc = 15.0M;
                }
                else if (!string.IsNullOrEmpty(dr.INCOME_ID) && dr.INCOME_ID.StartsWith("3"))
                {
                    iinexperc = 0.0M;
                    iinrate = 0.0M;
                }
                else if (dr.INCOME_ID == "1010")
                {
                    iinexperc = 50.0M;
                    iinrate = 10.0M;
                }
                else
                {
                    iinexperc = 0.0M;
                    dr.IINEX = 0.0M;
                }

                if (dr.IINEX_PERC != iinexperc)
                {
                    dr.IINEX_PERC = iinexperc;
                    dorecalc = true;
                }
                if (dr.IIN_RATE != iinrate)
                {
                    dr.IIN_RATE = iinrate;
                    dorecalc = true;
                    userate = true;
                }

            }

            if (e.PropertyName == nameof(A_FP_PAYLISTS_R.IINEX_PERC))
            {
                if (dr.IINEX_PERC == 0.0M)
                {
                    dr.IINEX = 0.0M;
                    dorecalc = true;
                }
            }

            if (e.PropertyName == nameof(A_FP_PAYLISTS_R.IIN_RATE))
            {
                dorecalc = true;
                userate = true;
            }

            if (dorecalc ||
                e.PropertyName == nameof(A_FP_PAYLISTS_R.PAY0) ||
                e.PropertyName == nameof(A_FP_PAYLISTS_R.DATE1) ||
                e.PropertyName == nameof(A_FP_PAYLISTS_R.SIRATETP) ||
                e.PropertyName == nameof(A_FP_PAYLISTS_R.SIRATETP) ||
                e.PropertyName == nameof(A_FP_PAYLISTS_R.TAX_TP) ||
                e.PropertyName == nameof(A_FP_PAYLISTS_R.IINEX_PERC) ||
                e.PropertyName == nameof(A_FP_PAYLISTS_R.IINEX))
            {
                FullRecalc(dr, userate);
                return;
            }
        }

        private void SetupToolStrips()
        {
            InsertInToolStrip(toolStrip1, cbLists, 2);
        }

        private void MakeGrid()
        {
            sgrRow.MakeGrid();
            sgrRow.LinkGrid();
        }

        private void dgvList_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.ColumnIndex == dgcListPayDate.Index)
            {
                if (e.Value == null) return;
                if (!Utils.DGVParseDateCell(e))
                {
                    e.Value = null;
                    e.ParsingApplied = true;
                    return;
                }
            }
        }

        private void dgvRows_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.ColumnIndex == dgcRowsPayDate.Index ||
                e.ColumnIndex == dgcRowsDate1.Index ||
                e.ColumnIndex == dgcRowsDate2.Index)
            {
                if (!Utils.DGVParseDateCell(e))
                {
                    e.Value = null;
                    e.ParsingApplied = true;
                    return;
                }
            }
        }

        private void dgvList_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[dgcListYR.Index].Value = DataLoaderA.LoadedDT2.Year;
            e.Row.Cells[dgcListMT.Index].Value = DataLoaderA.LoadedDT2.Month;
            e.Row.Cells[dgcListSNR.Index].Value = DataTasksA.GetNextFpPayListNr(DataLoaderA.LoadedDT2.Year);
            e.Row.Cells[dgcListPayDate.Index].Value = DateTime.Today;
        }

        private void dgvRows_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[dgcRowsSNR.Index].Value = bsRows.Count;
            var dr_l = bsList.GetCurrentItem<A_FP_PAYLISTS>();
            e.Row.Cells[dgcRowsPayDate.Index].Value = dr_l.PAYDATE;
            var dt = new DateTime(dr_l.YR, dr_l.MT, 1);
            e.Row.Cells[dgcRowsDate1.Index].Value = dt;
            e.Row.Cells[dgcRowsDate2.Index].Value = dt.LastDayOfMonth();
        }

        private void dgvList_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void dgvRows_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete()) e.Cancel = true;
        }

        private void dgvList_KeyDown(object sender, KeyEventArgs e)
        {
            if (dgvList.CurrentCell == null) return;
            if (e.KeyCode == Keys.Delete && e.Control)
            {
                DeleteCurrent();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                if (!dgvList.EndEdit()) return;
                dgvList.MoveToNewRow();
                e.Handled = true;
                return;
            }
        }

        private void dgvRows_KeyDown(object sender, KeyEventArgs e)
        {
            if (dgvRows.CurrentCell == null) return;
            if (e.KeyCode == Keys.Delete && e.Control)
            {
                DeleteCurrent();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                if (!dgvRows.EndEdit()) return;
                dgvRows.MoveToNewRow();
                e.Handled = true;
                return;
            }
        }

        private void dgvRows_MyKeyDown(object sender, KeyEventArgs e)
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow ||
                dgvRows.CurrentCell == null) return;
            int colnr = dgvRows.CurrentCell.ColumnIndex;

            if (e.KeyCode == Keys.F5)
            {
                if (colnr != dgcRowsIDP.Index) return;
                GetIDPFromDialog();
                e.Handled = true;
                return;
            }

        }

        private void dgvRows_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow ||
                dgvRows.CurrentCell == null) return;
            int colnr = dgvRows.CurrentCell.ColumnIndex;
            if (colnr != e.ColumnIndex) return;

            if (colnr == dgcRowsIDP.Index)
            {
                GetIDPFromDialog();
            }
        }

        public void GetIDPFromDialog()
        {
            if (bsRows.Count == 0 || bsRows.Current == null) return;
            if (dgvRows.CurrentRow == null || dgvRows.CurrentRow.IsNewRow) return;
            var fm = new FormA_FizPersons();
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return;
            var dr = dgvRows.GetCurrentObjectViewItem<A_FP_PAYLISTS_R>();
            if (dgvRows.CurrentCell != null)
            {
                try
                {
                    dgvRows.BeginEdit(false);
                    var c = dgvRows.EditingControl as DataGridViewComboBoxEditingControl;
                    c.SelectedValue = fm.SelectedObject;
                    dgvRows.EndEdit();
                }
                catch (Exception) { }
            }
        }


        private void cbLists_Format(object sender, ListControlConvertEventArgs e)
        {
            var drv = e.ListItem as ObjectView<A_FP_PAYLISTS>;
            if (drv == null) return;
            var dr = drv.Object;
            if (dr == null) return;
            var row_state = MyData.DbContextA.Entry(dr).State;
            if (row_state == Microsoft.EntityFrameworkCore.EntityState.Detached ||
                row_state == Microsoft.EntityFrameworkCore.EntityState.Deleted)
            {
                e.Value = "?";
                return;
            }
            var s = $"{dr.PAYDATE:dd.MM.yyyy} {dr.SNR} {dr.DESCR}";
            e.Value = s;
        }

        public void ShowPayLists(bool b)
        {
            splitContainer1.Panel1Collapsed = !b;
            cbLists.Visible = !b;
            MyData.Settings.ShowFpPayLists = b;
            miShowList.Checked = b;
        }

        public void ShowPayDataPanel(bool b)
        {
            sgrRow.Visible = b;
            MyData.Settings.ShowFpPayDataPanel = b;
            miShowPanel.Checked = b;
        }

        private void CheckEnableRows()
        {
            bool enabled = 
                dgvList.CurrentCell != null && 
                bsList.Count > 0 && 
                dgvList.CurrentRow != null &&
                !dgvList.CurrentRow.IsNewRow;
            SetControlEnabled(dgvRows, enabled);
            CheckEnableSGR();
        }

        private void CheckEnableSGR()
        {
            bool enabled = dgvRows.Enabled && bsRows.Count > 0 &&
                dgvRows.CurrentRow != null && !dgvRows.CurrentRow.IsNewRow;
            SetControlEnabled(sgrRow, enabled);
        }

        private void dgvList_CurrentCellChanged(object sender, EventArgs e)
        {
            CheckEnableRows();
        }

        private void dgvRows_CurrentCellChanged(object sender, EventArgs e)
        {
            CheckEnableSGR();
        }

        private void dgvList_Enter(object sender, EventArgs e)
        {
            bnavList.BindingSource = bsList;
            bnavList.DataGrid = dgvList;
            tslGrid.Text = "Saraksti:";
        }

        private void dgvRows_Enter(object sender, EventArgs e)
        {
            bnavList.BindingSource = bsRows;
            bnavList.DataGrid = dgvRows;
            tslGrid.Text = "Rindas:";
        }


        public void DeleteCurrent()
        {
            bnavList.DeleteCurrent();
            SaveData();
        }

        public override bool SaveData()
        {
            if (!dgvList.EndEditX()) return false;
            if (!dgvRows.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = bsPersons.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private void SetSaveButton(bool red)
        {
            bnavList.SetSaveButton(tsbSave, red);
        }

        private bool HasChanges()
        {
            return bsPersons.HasChanges() || bsRows.HasChanges();
        }

        private void CheckSave()
        {
            SetSaveButton(HasChanges());
        }

        private void dgvList_MyCheckForChanges(object sender, EventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void bsList_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            CheckSave();
        }

        private void bsRows_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            if (e.ListChangedType == ListChangedType.Reset) return;
            CheckSave();
        }

        private void bnavList_ItemDeleting(object sender, CancelEventArgs e)
        {
            e.Cancel = !AskCanDelete();
        }

        private void miShowList_Click(object sender, EventArgs e)
        {
            ShowPayLists(splitContainer1.Panel1Collapsed);
        }

        private void miShowPanel_Click(object sender, EventArgs e)
        {
            ShowPayDataPanel(!sgrRow.Visible);
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            DeleteCurrent();
        }

        private void sgrRow_ConvertingValueToDisplayString(object sender, 
            DevAge.ComponentModel.ConvertingObjectEventArgs e)
        {
            if (bsRows.Count == 0 || bsRows.Current == null ||
                dgvRows.CurrentRow != null && dgvRows.CurrentRow.IsNewRow)
            {
                e.Value = "";
                e.ConvertingStatus = DevAge.ComponentModel.ConvertingStatus.Completed;
                return;
            }
            if (e.Value != null) return;
            if (sender == rwName)
            {
                int idp = (int)e.Value;
                var ss = DataTasksA.GetFPPersonNameAndPK(idp);
                e.Value = ss == null ? null : ss[0] + " " + ss[1];
                e.ConvertingStatus = DevAge.ComponentModel.ConvertingStatus.Completed;
                return;
            }
            if (sender == rwPK)
            {
                int idp = (int)e.Value;
                var ss = DataTasksA.GetFPPersonNameAndPK(idp);
                e.Value = ss == null ? "" : ss.GetValue(2);
                e.ConvertingStatus = DevAge.ComponentModel.ConvertingStatus.Completed;
                return;
            }
            if (sender == rwIncomeStr)
            {
                string incid = e.Value.AsString();
                e.Value = MyData.DbContextA.A_INCOME_CODES.Find(incid)?.DESCR;
                e.ConvertingStatus = DevAge.ComponentModel.ConvertingStatus.Completed;
                return;
            }
        }

        private void dgvRows_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var o = dgvRows[dgcRowsIDP.Index, e.RowIndex].Value;
            if (o == null || o == DBNull.Value) return;
            dgvRows.EndEditX();
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            bnavList.DataGrid.MoveToNewRow();
        }

        private decimal titleSAIDD = decimal.MinValue;
        private decimal titleSAIDN = decimal.MinValue;
        private decimal titleIIN = decimal.MinValue;

        private void UpdateTitleData()
        {
            if (titleSAIDD != fpPaylistsRowData1._SIDD ||
                titleSAIDN != fpPaylistsRowData1._SIDN)
            {
                titleSAIDD = fpPaylistsRowData1._SIDD;
                titleSAIDN = fpPaylistsRowData1._SIDN;
                rwSAITitle.ValueStr = $"{titleSAIDD}; {titleSAIDN}";
            }

            if (titleIIN != fpPaylistsRowData1._IIN)
            {
                titleIIN = fpPaylistsRowData1._IIN;
                rwIINTitle2.ValueStr = $"{titleIIN:N2}";
            }

        }

        private void sgrRow_ValueChanged(object sender, EventArgs e)
        {
            UpdateTitleData();
        }

        private void bsPersons_ListChanged(object sender, ListChangedEventArgs e)
        {
            CheckDgvRowsAllowAddRows();
        }

    }
}
