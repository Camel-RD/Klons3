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
using Klons3.ModelsA;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1;
using Equin.ApplicationFramework;
using Klons3.ModelsF;

namespace KlonsA.Forms
{
    public partial class FormA_PersonsR : MyFormBaseF
    {
        public FormA_PersonsR()
        {
            InitializeComponent();
            try
            {
            }
            catch (Exception e)
            {
                Form_Error.ShowException(e);
            }
            CheckMyFontAndColors();

            bsPapildSummasVeids.SetFilter<A_PLUSMINUS_TYPES>(x => x.TP3 == 1);
            ShowOnlyUsed = MyData.Params.PersDataOnlyUsed;
        }

        private ToolStripControlHost dateSelectorItem = null;
        private bool InSwitchingTabs = false;

        public enum ESelectedTab
        {
            pamatdati,
            piemaksas,
            atvilkumi,
            pieņemts_atlaists,
            atvaļinājumi,
            slimības,
            citi,
            visi_notikumi
        }

        public ESelectedTab SelectedTab = ESelectedTab.pamatdati;

        private void Form_PersonsR_Load(object sender, EventArgs e)
        {
            SetupToolStrips();

            if (bsPersons.Count == 0)
            {
                MyMainForm.ShowInfo("Darbinieku saraksts it tukšs.");
                this.Close();
                return;
            }

            try
            {
                MakeGrid();
            }
            catch (Exception ex)
            {
                Form_Error.ShowException(ex);
            }

            cbSelectTab.SelectedIndex = 0;

            dgcPSRateType.DataSource = SomeDataDefsA.ProcOrEuro;
            dgcPSRateType.DisplayMember = "Val";
            dgcPSRateType.ValueMember = "Key";

            sgrPersR.Select();

            LoadSettings();
            CheckEventsForAll();
            CheckPersonRedRows();

            this.bsPersonsR.CurrentChanged += new System.EventHandler(this.bsPersonsR_CurrentChanged);

            if (!mySplitContainer1.Panel1Collapsed)
            {

            }
        }

        public void LoadSettings()
        {
            ShowPersonsList(MyData.Settings.ShowPersonList);
        }

        public override void SaveParams()
        {
            MyData.Settings.ShowPersonList = !mySplitContainer1.Panel1Collapsed;
        }

        private void Form_PersonsR_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void BsPapildSummas_ListItemPropertyChanged(object sender, MyItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(A_POSITIONS_PLUSMINUS.IDSV))
            {
                var dr = e.Item as A_POSITIONS_PLUSMINUS;
                if (dr.IDSV == null)
                {
                    if (dr.IDNO == null) return;
                    dr.IDNO = null;
                    return;
                }
                var bonustype = dr.XBonusType;
                var allowed = SomeDataDefsA.GetAllowedEBonusFrom(bonustype);
                var list = SomeDataDefsA.GetBonusFromItems(allowed);
                if (list == null || list.Length == 0)
                {
                    dr.IDNO = null;
                    return;
                }
                if (dr.XBonusFrom == EBonusFrom.None || !allowed.Contains(dr.XBonusFrom))
                {
                    dr.XBonusFrom = allowed[0];
                    return;
                }
            }
        }

        private void SetupToolStrips()
        {
            var k = bnavNav.Items.IndexOf(this.bnavNavPosItem);
            dateSelectorItem = InsertInToolStrip(bnavNav, cbDates, k);

            InsertInToolStrip(toolStrip1, cbPersons, 0);
            InsertInToolStrip(toolStrip1, cbAmati, 2);
            InsertInToolStrip(toolStrip1, cbSelectTab, 4);
        }

        private void MakeGrid()
        {
            sgrPersR.MakeGrid();
            sgrAmati.MakeGrid();

            sgrPersR.LinkGrid();
            sgrPersR.ArrangeLinkedControls();

            sgrAmati.LinkGrid();
            sgrAmati.ArrangeLinkedControls();
        }

        public override bool SaveData()
        {
            if (!this.Validate()) return false;
            var rt = bsPersons.SaveDbContext();
            CheckSave();
            return rt.IsSuccess();
        }

        private void SetSaveButton(bool red)
        {
            bnavNav.SetSaveButton(tsbSave, red);
        }

        private void CheckSave()
        {
            SetSaveButton(bsPersons.HasChanges() || bsPersonsR.HasChanges()
                || bsAmati.HasChanges() || bsNotikumi.HasChanges() 
                || bsPapildSummas.HasChanges());
        }


        private void SetNavPosItemType()
        {
            var b1 = bnavNav.BindingSource == bsPersonsR || bnavNav.BindingSource == bsAmatiR;
            bnavNavPosItem.Visible = !b1;
            bnavNavCountItem.Visible = !b1;
            dateSelectorItem.Visible = b1;
            if (bnavNav.BindingSource == bsPersonsR)
            {
                int k = bsPersonsR.Position;
                cbDates.DataSource = bsPersonsR;
                cbDates.DisplayMember = "EDIT_DATE_STR";
                cbDates.ValueMember = "EDIT_DATE";
                //bsPersonsR.Position = k;
                cbDates.SelectedIndex = k;
            }
            else if (bnavNav.BindingSource == bsAmatiR)
            {
                int k = bsAmatiR.Position;
                cbDates.DataSource = bsAmatiR;
                cbDates.DisplayMember = "EDIT_DATE_STR";
                cbDates.ValueMember = "EDIT_DATE";
                //bsAmatiR.Position = k;
                cbDates.SelectedIndex = k;
            }
        }

        private void sgrPersR_Enter(object sender, EventArgs e)
        {
            bnavNav.BindingSource = bsPersonsR;
            tslLabel.Text = "Darbinieks:";
            SetNavPosItemType();
        }

        private void sgrAmati_Enter(object sender, EventArgs e)
        {
            bnavNav.BindingSource = bsAmatiR;
            tslLabel.Text = "Amats:";
            SetNavPosItemType();
        }
        private void dgvPapildSummas_Enter(object sender, EventArgs e)
        {
            bnavNav.BindingSource = bsPapildSummas;
            tslLabel.Text = "Summas:";
            SetNavPosItemType();
        }
        private void dgvNotikumi_Enter(object sender, EventArgs e)
        {
            bnavNav.BindingSource = bsNotikumi;
            tslLabel.Text = "Notikumi:";
            SetNavPosItemType();
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void bsPersonsR_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            if (e.ListChangedType == ListChangedType.Reset) return;
            CheckSave();
        }

        private void bsAmatiR_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            if (e.ListChangedType == ListChangedType.Reset) return;
            CheckSave();
        }

        private void bsNotikumi_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            if (e.ListChangedType == ListChangedType.Reset) return;
            CheckSave();
        }

        private void bsPapildSummas_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (IsLoading) return;
            if (e.ListChangedType == ListChangedType.Reset) return;
            CheckSave();
        }
        private void bsPersons_CurrentChanged(object sender, EventArgs e)
        {
            if (bsPersonsR.Count > 0)
                bsPersonsR.Position = 0;
        }
        private void bsAmati_CurrentChanged(object sender, EventArgs e)
        {
            if (bsAmatiR.Count > 0)
                bsAmatiR.Position = 0;
        }

        private void AddPersonsRRow()
        {
            var row_cur_p = bsPersons.GetCurrentItem<A_PERSONS>();
            if (row_cur_p == null) return;

            if (bsPersonsR.Count == 0) return;

            var drv_last_pr = bsPersonsR[0] as ObjectView<A_PERSONS_R>;
            var row_last_pr = drv_last_pr.Object;

            var lastdt = row_last_pr.EDIT_DATE;
            if (lastdt == DateTime.Today)
            {
                MyMainForm.ShowWarning("Jaunu labojumu nevar izveidot, jo\n" +
                                       "labojums ar šodienas dautunu jau ir izveidots.");
                return;
            }

            var table_pr = MyData.DbContextA.BL_A_PERSONS_R;
            var row_new_pr = table_pr.NewItem();
            table_pr.Add(row_new_pr);
            MyData.DbContextA.CopyEntry(row_last_pr, row_new_pr);

            row_new_pr.EDIT_DATE = DateTime.Today;
            if (!string.IsNullOrEmpty(row_cur_p.FNAME)) row_new_pr.FNAME = row_cur_p.FNAME;
            if (!string.IsNullOrEmpty(row_cur_p.LNAME)) row_new_pr.LNAME = row_cur_p.LNAME;
            if (!string.IsNullOrEmpty(row_cur_p.PK)) row_new_pr.PERSON_CODE = row_cur_p.PK;

            bsPersonsR.Position = 0;
        }

        private void AddAmatiRRow()
        {
            var drv_cur_am = bsAmati.Current as ObjectView<A_POSITIONS>;
            if (drv_cur_am == null) return;
            var row_cur_am = drv_cur_am.Object;
            if (row_cur_am == null) return;

            if (bsAmatiR.Count == 0) return;

            var drv_last_amr = bsAmatiR[0] as ObjectView<A_POSITIONS_R>;
            var row_last_amr = drv_last_amr.Object;

            var lastdt = row_last_amr.EDIT_DATE;
            if (lastdt == DateTime.Today)
            {
                MyMainForm.ShowWarning("Jaunu labojumu nevar izveidot, jo\n" +
                                       "labojums ar šodienas dautunu jau ir izveidots.");
                return;
            }

            var table_amr = MyData.DbContextA.BL_A_POSITIONS_R;
            var row_new_amr = table_amr.NewItem();
            MyData.DbContextA.CopyEntry(row_last_amr, row_new_amr);

            row_new_amr.EDIT_DATE = DateTime.Today;
            if (!row_cur_am.TITLE.IsNOE()) row_new_amr.TITLE = row_cur_am.TITLE;
            if (row_cur_am.Department != null) row_new_amr.Department = row_cur_am.Department;
            table_amr.Add(row_new_amr);

            bsAmatiR.Position = 0;
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            if (!Validate()) return;
            if (bnavNav.BindingSource == bsPersonsR)
            {
                AddPersonsRRow();
            }
            if (bnavNav.BindingSource == bsAmatiR)
            {
                AddAmatiRRow();
            }
            if (bnavNav.BindingSource == bsPapildSummas)
            {
                dgvPapildSummas.MoveToNewRow();
            }
            if (bnavNav.BindingSource == bsNotikumi)
            {
                dgvNotikumi.MoveToNewRow();
            }
        }

        public void DeleteCurrent()
        {
            if (!Validate()) return;
            if (bnavNav.BindingSource.Current == null) return;
            if (!AskCanDelete()) return;
            if (bnavNav.BindingSource == bsPersonsR || bnavNav.BindingSource == bsAmatiR)
            {
                if (bnavNav.BindingSource.Count == 1)
                {
                    MyMainForm.ShowWarning("Nedrīkst dzēst vienīgo datu redakcijas ierakstu.");
                    return;
                }
            }
            if (bnavNav.BindingSource == bsPapildSummas)
            {
                if (dgvPapildSummas.CurrentRow == null || dgvPapildSummas.CurrentRow.IsNewRow)
                    return;
            }
            if (bnavNav.BindingSource == bsNotikumi)
            {
                if (dgvNotikumi.CurrentRow == null || dgvNotikumi.CurrentRow.IsNewRow)
                    return;
            }

            if (bnavNav.BindingSource.Current is IMyObjectView ov)
            {
                if (bnavNav.BindingSource is MyBindingSourceEf2 bs1)
                {
                    bs1.GetMyDbContext().Remove(ov.Object);
                }
            }


            //bnavNav.BindingSource.RemoveCurrent();
            SaveData();
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            DeleteCurrent();
        }

        private void dgvPapildSummas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && e.Control)
            {
                DeleteCurrent();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                if (!dgvPapildSummas.EndEdit()) return;
                dgvPapildSummas.MoveToNewRow();
                e.Handled = true;
                return;
            }
        }

        private void dgvNotikumi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && e.Control)
            {
                DeleteCurrent();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Insert && e.Shift)
            {
                if (!dgvNotikumi.EndEdit()) return;
                dgvNotikumi.MoveToNewRow();
                e.Handled = true;
                return;
            }
        }


        private void cbDates_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.Value == null || !(e.Value is DateTime)) return;
            e.Value = Utils.DateToString((DateTime)e.Value);
        }

        private int GetPapildSummasTP1(int id)
        {
            var dr = MyData.DbContextA.A_PLUSMINUS_TYPES.Find(id);
            if (dr == null) return -1;
            return dr.TP1.Value;
        }
        private int GetNotikumaVeidsTP1(int id)
        {
            var dr = MyData.DbContextA.A_EVENT_TYPES.Find(id);
            if (dr == null) return -1;
            return dr.TP1.Value;
        }

        private void cbSelectTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            int k = cbSelectTab.SelectedIndex;
            int tp1;
            if (k == 0)
            {
                InSwitchingTabs = true;
                tabControl1.SelectedIndex = 0;
                SelectedTab = ESelectedTab.pamatdati;
                InSwitchingTabs = false;
                return;
            }

            if (bsPersons.Current == null) return;

            if (k == 1 || k == 2)
            {

                tp1 = k == 1 ? 1 : 0;
                bsPapildSummasVeids.RemoveFilter();
                bsPapildSummas.SetFilter<A_POSITIONS_PLUSMINUS>(x => x.PlusMinusType == null || x.PlusMinusType.TP1 == tp1);
                bsPapildSummasVeids.SetFilter<A_PLUSMINUS_TYPES>(x => x.TP3 == 1 && x.TP1 == tp1);
                SelectedTab = k == 1 ? ESelectedTab.piemaksas : ESelectedTab.atvilkumi;
                InSwitchingTabs = true;
                tabControl1.SelectedIndex = 1;
                InSwitchingTabs = false;
                return;
            }
            if (k >= 3 && k <= 7)
            {
                switch (k)
                {
                    case 3:
                        tp1 = 0;
                        SelectedTab = ESelectedTab.pieņemts_atlaists;
                        break;
                    case 4:
                        tp1 = 1;
                        SelectedTab = ESelectedTab.atvaļinājumi;
                        break;
                    case 5:
                        tp1 = 2;
                        SelectedTab = ESelectedTab.slimības;
                        break;
                    case 6:
                        tp1 = 3;
                        SelectedTab = ESelectedTab.citi;
                        break;
                    case 7:
                        tp1 = -1;
                        SelectedTab = ESelectedTab.visi_notikumi;
                        break;
                    default:
                        tp1 = -1;
                        break;
                }

                if (tp1 == -1)
                {
                    bsNotikumuVeidi.RemoveFilter();
                    bsNotikumi.RemoveFilter();
                }
                else
                {
                    bsNotikumuVeidi.RemoveFilter();
                    bsNotikumi.SetFilter<A_EVENTS>(x => x.EventType == null || x.EventType.TP1 == tp1);
                    bsNotikumuVeidi.SetFilter<A_EVENT_TYPES>(x => x.TP1 == tp1);
                }

                InSwitchingTabs = true;
                tabControl1.SelectedIndex = 2;
                InSwitchingTabs = false;

                dgcNotIDA.Visible = SelectedTab != ESelectedTab.atvaļinājumi &&
                    SelectedTab != ESelectedTab.slimības;
                dgcNotIDN2.Visible = SelectedTab == ESelectedTab.citi ||
                    SelectedTab == ESelectedTab.visi_notikumi;
                dgcNotDate2.Visible = SelectedTab != ESelectedTab.pieņemts_atlaists;
                dgcNotDT3.Visible = SelectedTab == ESelectedTab.atvaļinājumi;
                dgcNotDays.Visible = SelectedTab == ESelectedTab.atvaļinājumi ||
                    SelectedTab == ESelectedTab.pieņemts_atlaists;
                dgcNotDays2.Visible = dgcNotDays.Visible;
                dgcNotOcc.Visible = SelectedTab == ESelectedTab.pieņemts_atlaists;
                return;
            }
        }
    

        private void dgvPapildSummas_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.ColumnIndex == dgcPSDate1.Index || e.ColumnIndex == dgcPSDate2.Index)
            {
                Utils.DGVParseDateCell(e);
            }
        }

        private void dgvNotikumi_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.ColumnIndex == dgcNotDate1.Index ||
                e.ColumnIndex == dgcNotDate2.Index ||
                e.ColumnIndex == dgcNotDT3.Index)
            {
                Utils.DGVParseDateCell(e);
            }
        }

        private void dgvPapildSummas_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete())
                e.Cancel = true;
        }

        private void dgvNotikumi_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow || !AskCanDelete())
                e.Cancel = true;
        }

        private void dgvNotikumi_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            if (bsPersons.Current == null) return;
            var dr_person = bsPersons.GetCurrentItem<A_PERSONS>();
            e.Row.Cells[dgcNotIDP.Index].Value = dr_person;
            int idn = bsNotikumuVeidi.GetItem<A_EVENT_TYPES>(0).ID;
            e.Row.Cells[dgcNotIDN.Index].Value = idn;
            e.Row.Cells[dgcNotDate1.Index].Value = DateTime.Today;
        }

        private void CheckdgvNotikumiColumns()
        {
            if (dgvNotikumi.CurrentRow == null || dgvNotikumi.CurrentRow.IsNewRow)
            {
                dgcNotDT3.Visible = false;
                return;
            }
            object oidn = dgvNotikumi.CurrentRow.Cells[dgcNotIDN.Index].Value;
            bool b = oidn != null && SomeDataDefsA.EventHasPayDate((EEventId)oidn);
            dgcNotDT3.Visible = b;
            b = oidn != null && SomeDataDefsA.IsFromToEvent((EEventId)oidn);
            dgcNotDate2.Visible = b;
            b = oidn != null && ((EEventId)oidn) == EEventId.Atvaļinājums;
            dgcNotDays.Visible = b;
            dgcNotDays2.Visible = b;
        }

        private void bsNotikumi_CurrentChanged(object sender, EventArgs e)
        {
            //CheckdgvNotikumiColumns();
        }

        private void dgvNotikumi_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgcNotDate1.Index)
            {
                object oidn = dgvNotikumi.CurrentRow.Cells[dgcNotIDN.Index].Value;
                if (oidn != null && SomeDataDefsA.EventHasPayDate((EEventId)oidn))
                {
                    var dt = dgvNotikumi[e.ColumnIndex, e.RowIndex].Value;
                    if (dt != null)
                        dt = ((DateTime)dt).AddDays(-1);
                    dgvNotikumi[dgcNotDT3.Index, e.RowIndex].Value = dt;
                }
                return;
            }
            if (e.ColumnIndex == dgcNotIDN.Index)
            {
                object oidn = dgvNotikumi.CurrentRow.Cells[dgcNotIDN.Index].Value;
                if (oidn == null || !SomeDataDefsA.IsFromToEvent((EEventId)oidn))
                {
                    dgvNotikumi[dgcNotDate2.Index, e.RowIndex].Value = null;
                    dgvNotikumi[dgcNotDT3.Index, e.RowIndex].Value = null;
                    dgvNotikumi[dgcNotDays.Index, e.RowIndex].Value = 0;
                    dgvNotikumi[dgcNotDays2.Index, e.RowIndex].Value = 0;
                }

                return;
            }
        }

        private void dgvNotikumi_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (IsLoading || InSwitchingTabs || ActiveControl != mySplitContainer1 || !dgvNotikumi.Focused) return;
            if (e.RowIndex == -1 || dgvNotikumi.NewRowIndex == e.RowIndex) return;
            object oidn = dgvNotikumi.CurrentRow.Cells[dgcNotIDN.Index].Value;
            if (oidn == null)
            {
                e.Cancel = true;
                return;
            }
            var ev = (EEventId)oidn;
            bool b = SomeDataDefsA.IsFromToEvent(ev);
            object odt1 = dgvNotikumi[dgcNotDate1.Index, e.RowIndex].Value;
            object odt2 = dgvNotikumi[dgcNotDate2.Index, e.RowIndex].Value;
            if (odt1 == null) odt1 = null;
            if (odt2 == null) odt2 = null;
            bool baddates = false;
            if (b)
            {
                if (odt1 == null || odt2 == null)
                {
                    baddates = true;
                }
                else
                {
                    var dt1 = (DateTime)odt1;
                    var dt2 = (DateTime)odt2;
                    if (dt1 > dt2 || dt1.Year < 1950 || dt1.Year > 2100 ||
                        dt2.Year < 1950 || dt2.Year > 2100)
                    {
                        baddates = true;
                    }
                }
            }
            else
            {
                if (odt1 == null) baddates = true;
            }
            if (baddates)
            {
                MyMainForm.ShowWarning("Nekorekti norādīti datumi no - līdz.");
                e.Cancel = true;
                return;
            }

            b = SomeDataDefsA.EventHasPayDate(ev);
            if (b)
            {
                object odt3 = dgvNotikumi[dgcNotDT3.Index, e.RowIndex].Value;
                if (odt3 == null)
                {
                    baddates = true;
                }
                else
                {
                    var dt3 = (DateTime)odt3;
                    if (dt3.Year < 1950 || dt3.Year > 2100 ||
                        (odt1 != null && (DateTime)odt1 < dt3))
                        baddates = true;
                }
            }
            if (baddates)
            {
                MyMainForm.ShowWarning("Nekorekti norādīts izmaksas datums.");
                e.Cancel = true;
                return;
            }

            if (ev == EEventId.Piešķirts_amats || ev == EEventId.Atbrīvots_no_amata)
            {
                object oida = dgvNotikumi[dgcNotIDA.Index, e.RowIndex].Value;
                if (oida == null)
                {
                    MyMainForm.ShowWarning("Janorāda amats.");
                    e.Cancel = true;
                    return;
                }
            }

            if (SomeDataDefsA.EventHasSCode(ev))
            {
                string scode = dgvNotikumi[dgcNotSCode.Index, e.RowIndex].Value.AsString();
                if (string.IsNullOrEmpty(scode))
                {
                    MyMainForm.ShowWarning("Janorāda ziņu kods.");
                    e.Cancel = true;
                    return;
                }
            }

            if (SomeDataDefsA.EventHasOccCode(ev))
            {
                string occcode = dgvNotikumi[dgcNotOcc.Index, e.RowIndex].Value.AsString();
                if (string.IsNullOrEmpty(occcode))
                {
                    MyMainForm.ShowWarning("Janorāda profesijas kods.");
                    e.Cancel = true;
                    return;
                }
            }

        }

        private void dgvPapildSummas_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            if (bsPersons.Current == null) return;
            var dr_p = bsPersons.GetCurrentItem<A_PERSONS>();
            e.Row.Cells[dgcPSPersonRow.Index].Value = dr_p;
        }

        private void dgvPapildSummas_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == dgcPSIDNO.Index)
            {
                if (e.RowIndex == dgvPapildSummas.NewRowIndex)
                {
                    e.Cancel = true;
                    return;
                }
                var cell = dgvPapildSummas[e.ColumnIndex, e.RowIndex] as DataGridViewComboBoxCell;
                var dr = dgvPapildSummas.GetObjectViewItem<A_POSITIONS_PLUSMINUS>(e.RowIndex);

                if (dr.IDSV == null)
                {
                    e.Cancel = true;
                    return;
                }
                var allowed = SomeDataDefsA.GetAllowedEBonusFrom(dr.XBonusType);
                if (allowed.Length == 0)
                {
                    e.Cancel = true;
                    return;
                }
                var list = SomeDataDefsA.GetBonusFromItems(allowed);
                cell.DataSource = list;
                cell.DisplayMember = "Val";
                cell.ValueMember = "Key";
            }
        }

        private void dgvPapildSummas_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgcPSIDNO.Index)
            {
                var cell = dgvPapildSummas[e.ColumnIndex, e.RowIndex] as DataGridViewComboBoxCell;
                cell.DataSource = bsPapildSummaNo;
                cell.DisplayMember = "DESCR";
                cell.ValueMember = "ID";
            }
        }

        public void ShowPersonsList(bool b)
        {
            rādītPaslēptDarbiniekuSarakstuToolStripMenuItem.Checked = b;
            mySplitContainer1.Panel1Collapsed = !b;
            cbPersons.Visible = !b;
            lbArrow1.Visible = !b;
        }

        public bool CheckEventsForCurrent()
        {
            if (bsPersons.Count == 0 || bsPersons.Current == null) return true;
            var dr = bsPersons.GetCurrentItem<A_PERSONS>();
            var err = DataTasksA.CheckEvents(dr);
            if (err.HasErrors)
            {
                Form_ErrorList.ShowErrorList(MyMainForm, err);
                return false;
            }
            return true;
        }

        public bool CheckEventsForAll()
        {
            if (bsPersons.Count == 0) return true;
            var err = DataTasksA.CheckEventsAll();
            if (err.HasErrors)
            {
                Form_ErrorList.ShowErrorList(MyMainForm, err);
                return false;
            }
            return true;
        }

        public void SelectPerson(A_PERSONS dr_person)
        {
            if (bsPersons.Count == 0) return;
            for (int i = 0; i < bsPersons.Count; i++)
            {
                var drp = bsPersons.GetItem<A_PERSONS>(i);
                if (drp == null) continue;
                if (drp == dr_person)
                {
                    bsPersons.Position = i;
                    return;
                }
            }
        }

        public void SelectPosition(A_POSITIONS dr_position)
        {
            if (bsPersons.Count == 0 || bsPersons.Current == null ||
                bsAmati.Count == 0) return;
            for (int i = 0; i < bsAmati.Count; i++)
            {
                var dram = bsAmati.GetItem<A_POSITIONS>(i);
                if (dram == null) continue;
                if (dram == dr_position)
                {
                    bsAmati.Position = i;
                    return;
                }
            }
        }

        public void DoAddPerson()
        {
            if (!SaveData()) return;
            var fp = MyMainForm.FindForm(typeof(FormA_Persons));
            if (fp != null)
            {
                MyMainForm.ShowWarning("Darbinieku saraksta forma ir atvērta.");
                return;
            }
            var fpn = new FormA_PersonNew();
            var rt = fpn.ShowDialog(MyMainForm);
            if (rt != DialogResult.OK) return;
            SelectPerson(fpn.Person);
            SaveData();
        }


        public void DoAddPosition()
        {
            if (!SaveData()) return;
            var fp = MyMainForm.FindForm(typeof(FormA_Persons));
            if (fp != null)
            {
                MyMainForm.ShowWarning("Darbinieku saraksta forma ir atvērta.");
                return;
            }
            if (bsPersons.Count == 0 || bsPersons.Current == null) return;
            var drp = bsPersons.GetCurrentItem<A_PERSONS>();
            if (drp == null) return;

            var fpn = new FormA_PersonNewPos();
            fpn.PersonName = drp.FNAME + " " + drp.LNAME;
            var rt = fpn.ShowDialog(MyMainForm);
            if (rt != DialogResult.OK) return;

            var tableAmati = MyData.DbContextA.BL_A_POSITIONS;
            var tableAmatiR = MyData.DbContextA.BL_A_POSITIONS_R;

            var dr_am = tableAmati.NewItem();
            tableAmati.Add(dr_am);

            dr_am.Person = drp;
            dr_am.TITLE = fpn.PositionTitle;
            dr_am.Department = fpn.Department;

            var dr_amr = tableAmatiR.NewItem();

            dr_amr.Position = dr_am;
            dr_amr.EDIT_DATE = DateTime.Today;
            dr_amr.TITLE = dr_am.TITLE;
            dr_amr.Department = dr_am.Department;
            dr_am.PositionStateRows.Add(dr_amr);

            SelectPosition(dr_am);

            SaveData();
        }

        private void rādītDarbiniekuSarakstuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool b = mySplitContainer1.Panel1Collapsed;
            ShowPersonsList(b);
        }

        private void pārbaudītNotikumusDarbiniekamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckEventsForCurrent();
        }

        private void pārbaudītNotikumusVisiemDarbiniekiemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckEventsForAll();
        }

        private void Form_PersonsR_FormClosing(object sender, FormClosingEventArgs e)
        {
            CheckEventsForAll();
        }

        private void miAddPerson_Click(object sender, EventArgs e)
        {
            DoAddPerson();
        }

        private void miAddPosition_Click(object sender, EventArgs e)
        {
            DoAddPosition();
        }

        private void darbiniekaKartīteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bsPersons.Count == 0 || bsPersons.Current == null) return;
            var dr = bsPersons.GetCurrentItem<A_PERSONS>();
            var rep = new Report_PersonData();
            rep.ShowReport(dr, DateTime.Today);
        }

        public bool ShowOnlyUsed
        {
            get { return MyData.Params.PersDataOnlyUsed; }
            set
            {
                if (value)
                {
                    bsPersons.SetFilter<A_PERSONS>(x => x.USED == 1);
                    bsAmati.SetFilter<A_POSITIONS>(x => x.USED == 1);
                }
                else
                {
                    bsPersons.RemoveFilter();
                    bsAmati.RemoveFilter();
                }
                MyData.Params.PersDataOnlyUsed = value;
                miShowOnlyUsed.Checked = value;
            }
        }

        private void miShowOnlyUsed_Click(object sender, EventArgs e)
        {
            ShowOnlyUsed = !ShowOnlyUsed;
        }

        public void CheckPersonRedRows()
        {
            var dr = (bsPersonsR.Current as ObjectView<A_PERSONS_R>)?.Object;
            if (dr == null) return;
            if (bsPersonsR.Position == bsPersonsR.Count - 1)
            {
                sgrPersR.ClearRed();
            }
            else
            {
                var dr_prev = (bsPersonsR[bsPersonsR.Position + 1] as ObjectView<A_PERSONS_R>)?.Object;
                if (dr_prev == null) return;
                if (dr_prev.EDIT_DATE > dr.EDIT_DATE) return;
                sgrPersR.CheckRedRows([ dr_prev ], [ dr ], [ bsPersonsR ]);
            }
        }

        public void CheckPersonRowRed(KlonsLIB.MySourceGrid.GridRows.MyGridRowPropEditorBase row)
        {
            if (row == null) return;
            var dr = (bsPersonsR.Current as ObjectView<A_PERSONS_R>)?.Object;
            if (dr == null) return;
            if (bsPersonsR.Position == bsPersonsR.Count - 1) return;
            var dr_prev = (bsPersonsR[bsPersonsR.Position + 1] as ObjectView<A_PERSONS_R>)?.Object;
            if (dr_prev == null) return;
            if (dr_prev.EDIT_DATE > dr.EDIT_DATE) return;
            row.CheckRedRow([ dr_prev ], [ dr ], [ bsPersonsR ]);
        }

        public void CheckPositionRedRows()
        {
            var dr = (bsAmatiR.Current as ObjectView<A_POSITIONS_R>)?.Object;
            if (dr == null) return;
            if (bsAmatiR.Position == bsAmatiR.Count - 1)
            {
                sgrAmati.ClearRed();
            }
            else
            {
                var dr_prev = (bsAmatiR[bsAmatiR.Position + 1] as ObjectView<A_POSITIONS_R>)?.Object;
                if (dr_prev == null) return;
                if (dr_prev.EDIT_DATE > dr.EDIT_DATE) return;
                sgrAmati.CheckRedRows([ dr_prev ], [ dr ], [ bsAmatiR ]);
            }
        }

        public void CheckPositionRowRed(KlonsLIB.MySourceGrid.GridRows.MyGridRowPropEditorBase row)
        {
            if (row == null) return;
            var dr = (bsAmatiR.Current as ObjectView<A_POSITIONS_R>)?.Object;
            if (dr == null) return;
            if (bsAmatiR.Position == bsAmatiR.Count - 1) return;
            var dr_prev = (bsAmatiR[bsAmatiR.Position + 1] as ObjectView<A_POSITIONS_R>)?.Object;
            if (dr_prev == null) return;
            if (dr_prev.EDIT_DATE > dr.EDIT_DATE) return;
            row.CheckRedRow(new[] { dr_prev }, new[] { dr }, new[] { bsAmatiR });
        }

        private void bsPersonsR_CurrentChanged(object sender, EventArgs e)
        {
            CheckPersonRedRows();
        }

        private void sgrPersR_EditEnded(object sender, EventArgs e)
        {
            var row = sender as KlonsLIB.MySourceGrid.GridRows.MyGridRowPropEditorBase;
            CheckPersonRowRed(row);
        }

        private void bsAmatiR_CurrentChanged(object sender, EventArgs e)
        {
            CheckPositionRedRows();
        }

        private void sgrAmati_EditEnded(object sender, EventArgs e)
        {
            var row = sender as KlonsLIB.MySourceGrid.GridRows.MyGridRowPropEditorBase;
            CheckPositionRowRed(row);
        }

        private string GetTerrCode()
        {
            var fm = new FormA_TeritorialCodes();
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return null;
            return fm.SelectedValueStr;
        }

        private string GetDepCode()
        {
            var fm = new FormA_Departments();
            var ret = fm.ShowMyDialogModal();
            if (ret != DialogResult.OK) return null;
            return fm.SelectedValueStr;
        }

        private void sgrPersR_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            var pos = sgrPersR.Selection.ActivePosition;
            if (pos.Column != 2) return;

            if (e.KeyCode == Keys.F5)
            {
                if (pos.Row == rwTerCode.RowNr)
                {
                    var new_code = GetTerrCode();
                    if (new_code == null) return;
                    rwTerCode.Value = new_code;
                }
            }
        }

        private void sgrPersR_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var pos = sgrPersR.Selection.ActivePosition;
            if (pos.Column != 2) return;

            if (pos.Row == rwTerCode.RowNr)
            {
                var new_code = GetTerrCode();
                if (new_code == null) return;
                rwTerCode.Value = new_code;
            }
        }

        private void sgrAmati_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            var pos = sgrAmati.Selection.ActivePosition;
            if (pos.Column != 2) return;

            if (e.KeyCode == Keys.F5)
            {
                if (pos.Row == rwAmatiDep.RowNr)
                {
                    var new_code = GetDepCode();
                    if (new_code == null) return;
                    rwAmatiDep.Value = new_code;
                }
            }
        }

        private void sgrAmati_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var pos = sgrAmati.Selection.ActivePosition;
            if (pos.Column != 2) return;

            if (pos.Row == rwAmatiDep.RowNr)
            {
                var new_code = GetDepCode();
                if (new_code == null) return;
                rwAmatiDep.Value = new_code;
            }
        }

        public int FindPerson(bool forward)
        {
            if (bsPersons.Position == -1 || bsPersons.Current == null || !Validate()) return -1;
            var s = tbFindPerson.Text;
            if (string.IsNullOrEmpty(s)) return -1;
            int step = forward ? 1 : -1;
            for (int i = bsPersons.Position + step; i >= 0 && i < bsPersons.Count; i += step)
            {
                var dr = bsPersons.GetItem<A_POSITIONS>(i);
                if (dr.YNAME.IndexOf(s, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    bsPersons.Position = i;
                    return i;
                }
            }
            return -1;
        }

        public void ShowVacationUseData()
        {
            if (bsPersons.Position == -1 || bsPersons.Current == null || !Validate()) return;
            var dr = bsPersons.GetCurrentItem<A_PERSONS>();
            var rrvd = new RepRowVacDays();
            DateTime dt = DateTime.Today;
            var fim = new Form_InputBox("Atvaļinājuma dienu aprēķins", "Aprēķina datums:", Utils.DateToString(dt));
            var rt = fim.ShowMyDialogModal();
            if (rt != DialogResult.OK) return;
            if (!Utils.StringToDate(fim.SelectedValueStr, out dt))
            {
                MyMainForm.ShowWarning("Norādīts nekorekts datums.");
                return;
            }
            var er1 = Report_VacDays.GetVacDaysNotUsed(dr, dt, rrvd);
            if (er1 == "OK")
            {
                string msg = $"Darbinieks: {dr.YNAME}\n";
                msg += $"Datums: {Utils.DateToString(dt)}\n\n";
                msg += $"Pienākas atvaļinājuma dienas: {rrvd.ToUse}\n";
                msg += $"Izmantotās atvaļinājuma dienas: {rrvd.Used}\n";
                msg += $"Neizmantotās atvaļinājuma dienas: {rrvd.ToUse - rrvd.Used}\n";
                msg += $"Kompensētās atvaļinājuma dienas: {rrvd.Compansated}\n";
                msg += $"Atlikušās atvaļinājuma dienas: {rrvd.NotUsed}";
                MyMainForm.ShowInfo(msg, "Atvaļinājuma dati:");
            }
            else
            {
                MyMainForm.ShowWarning(er1);
            }
        }


        private void tsbPrevPerson_Click(object sender, EventArgs e)
        {
            FindPerson(false);
        }

        private void tsbNextPerson_Click(object sender, EventArgs e)
        {
            FindPerson(true);
        }

        private void tstbFindPerson_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Return)
            {
                FindPerson(true);
                e.Handled = true;
            }
        }

        private void neizmantotāsAtvaļinājumaDienasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowVacationUseData();
        }
    }
}
