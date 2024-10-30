﻿using System;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using KlonsLIB.Data;
using KlonsLIB.Forms;
using KlonsLIB.Misc;
using System.Reflection;
using Equin.ApplicationFramework;
using Microsoft.EntityFrameworkCore;
using KlonsLIB.BindingListView;
using static KlonsLIB.Data.MyDbContext;

namespace KlonsLIB.Components
{
    public class MyDataGridView : DataGridView
    {
        private ContextMenuStrip myContextMenuStrip = null;
        private bool _useMyContextmenu = true;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool GoingToDialog { get; set; }

        [DefaultValue(true),
        Category("Behavior")]
        public bool TakeCtrlTabKey { get; set; }

        [DefaultValue(true)]
        [Category("Data")]
        public bool AutoSave { get; set; }

        [Category("My")]
        public event KeyEventHandler MyKeyDown;

        [Category("My")]
        public event EventHandler MyCheckForChanges;

        [Category("My")]
        [DefaultValue(true)]
        public bool UseMyContextmenu
        {
            get
            {
                return _useMyContextmenu;
            }
            set
            {
                if (value == _useMyContextmenu) return;
                _useMyContextmenu = value;
                if (value)
                {
                    this.ContextMenuStrip = myContextMenuStrip;
                }
                else
                {
                    this.ContextMenuStrip = null;
                }
            }
        }

        [Browsable(true)]
        [DefaultValue(DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText)]
        [Category("Behavior")]
        public new DataGridViewClipboardCopyMode ClipboardCopyMode
        {
            get { return base.ClipboardCopyMode; }
            set { base.ClipboardCopyMode = value; }
        }

        [Category("My")]
        [DefaultValue(true)]
        public bool AutoValidate {  get; set; } = true;

        public MyDataGridView() : base()
        {
            GoingToDialog = false;
            TakeCtrlTabKey = true;
            DoubleBuffered = true;
            AutoSave = true;
            DataError += MyDataGridView_DataError;
            BackgroundColor = SystemColors.Control;
            ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            myContextMenuStrip = new ContextMenuStrip();
            myContextMenuStrip.Items.Add("Kopēt (Ctrl + C)", null, OnCopy);
            myContextMenuStrip.Items.Add("Kopēt bez virsrakstiem", null, OnCopyWithoutHeaders);
            ContextMenuStrip = myContextMenuStrip;

            SetMyToolTip();

            //ShowCellToolTips = false;
        }

        protected void SetMyToolTip()
        {
            var fi = Utils.GetField(this.GetType(), "_toolTipControl");
            var dtt = fi.GetValue(this);
            var fi2 = Utils.GetProp(dtt.GetType(), "ToolTip");
            var dToolTip = new MyToolTip();
            dToolTip.ShowAlways = true;
            dToolTip.InitialDelay = 0;
            dToolTip.UseFading = false;
            dToolTip.UseAnimation = false;
            dToolTip.AutoPopDelay = 0;
            fi2.SetValue(dtt, dToolTip);
        }

        private void OnCopy(object sender, EventArgs e)
        {
            Copy(true);
        }

        private void OnCopyWithoutHeaders(object sender, EventArgs e)
        {
            Copy(false);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            clickTimer.Interval = SystemInformation.DoubleClickTime + 10;
            clickTimer.Tick += new EventHandler(OnClickTimerTick);
            //clickTimer.SynchronizingObject = this.FindForm();
            if (!DesignMode)
            {
                CheckSizes(10.0f);
            }
            Form f = this.FindForm();
            if(f != null)
                f.FormClosing += My_FormClosing;
        }

        protected override void DestroyHandle()
        {
            this.GetClipboardContent();
            clickTimer.Stop();
            clickTimer.Tick -= new EventHandler(OnClickTimerTick);
            clickTimer.Dispose();
            var frm = this.FindForm();
            if (frm != null)
                frm.FormClosing -= My_FormClosing;
            base.DestroyHandle();
        }


        protected SizeF _ScaleFactor = new SizeF(1.0f, 1.0f);
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SizeF ScaleFactor { get { return _ScaleFactor; } }
        
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);

            float width = factor.Width;
            float height = factor.Height;
            _ScaleFactor.Width *= width;
            _ScaleFactor.Height *= height;

            if (width != 1.0f)
            {
                foreach (DataGridViewColumn cmn in Columns)
                {
                    cmn.MinimumWidth = (int)((float)cmn.MinimumWidth * width);
                    cmn.Width = (int) ((float) cmn.Width*width);
                    if (cmn is MyDgvMcCBColumn)
                    {
                        (cmn as MyDgvMcCBColumn).ScaleColumn(factor);
                    }
                    if (cmn is MyDgvMcComboBoxColumn)
                    {
                        (cmn as MyDgvMcComboBoxColumn).ScaleColumn(factor);
                    }
                }
            }

            if (height != 1.0f)
            {
                foreach (var row in Rows)
                {
                    var r = row as DataGridViewRow;
                    if (r != null)
                        r.Height = (int)((float)(r.Height - 0) * height) + 0;
                }
                if(this.RowTemplate.Height != Font.Height + 9)
                    //this.RowTemplate.Height = (int) ((float)(RowTemplate.Height - 2) * height) + 2;
                    this.RowTemplate.Height = Font.Height + 9;
            }
        }

        [DefaultValue(1.0f)]
        [Category("Appearance")]
        public float ColumnHeadersFontScale { get; set; } = 1.0f;

        public void UpdateStyleFonts()
        {
            if (ColumnHeadersFontScale == 1.0f && Font == ColumnHeadersDefaultCellStyle.Font) return;
            float sz = Font.Size * ColumnHeadersFontScale;
            ColumnHeadersDefaultCellStyle.Font = new Font(Font.FontFamily, sz, Font.Style);
        }

        void ScaleCellStyle(DataGridViewCellStyle style, SizeF factor)
        {
            if (style?.Font == null || style.Font == Font) return;
            style.Font = ScaleFont(style.Font, factor);
        }

        Font ScaleFont(Font font, SizeF factor)
        {
            float sz = font.Size * factor.Height;
            return new Font(font.FontFamily, sz, font.Style);
        }

        public void CheckSizes(float basefontaize)
        {
            // ---- no need for this
            /*
            float f1 = 1.0f;
            if (MyFormBase.DPIFactor != -1.0f) 
                f1 = MyFormBase.DPIFactor;
            if (f1 == 1.0f) return;
            _ScaleFactor.Width *= f1;
            _ScaleFactor.Height *= f1;
            float w;
            foreach (DataGridViewColumn cmn in Columns)
            {
                w = (float)cmn.Width * f1;
                cmn.Width = (int)Math.Round(w, 0);
            }
            w = (float)this.RowTemplate.Height;
            w = w * f1;
            this.RowTemplate.Height = (int)Math.Round(w, 0);
            */
        }

        public string FormatColumnWidths(int ver, int[] w)
        {
            if (w == null) return ver.ToString();
            var sw = w.Select(x => x.ToString());
            return ver + ";" + string.Join(";", sw);
        }

        public (int ver, int[] w) ParseColumnWidths(string sw)
        {
            if(sw.IsNOE())
                return (0, null);
            var ssw = sw.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if(ssw.Where(x => !int.TryParse(x, out _)).Any())
                return (0, null);
            int ver = int.Parse(ssw[0]);
            if (ssw.Length == 1)
                return (ver, null);
            int[] w = ssw.Skip(1).Select(x => int.Parse(x)).ToArray();
            return (ver, w);
        }

        public int[] GetColumnWidths(float basefontaize)
        {
            //float f1 = 1.0f;
            //if (MyFormBase.DPIFactor != -1.0f)
            //    f1 = MyFormBase.DPIFactor;
            float w;
            int[] cw = new int[Columns.Count];
            for (int i = 0; i < Columns.Count; i++)
            {
                w = (float) Columns[i].Width/ScaleFactor.Width;
                cw[i] = (int) Math.Round(w, 0);
            }
            return cw;
        }

        public string GetColumnWidths2(float basefontaize, int ver)
        {
            var w = GetColumnWidths(basefontaize);
            return FormatColumnWidths(ver, w);
        }

        public void SetColumnWidths(int[] widths)
        {
            if (widths.Length != Columns.Count) return;
            for (int i = 0; i < Columns.Count; i++)
            {
                Columns[i].Width = (int)Math.Round((float)widths[i] * ScaleFactor.Width, 0);
            }
        }
        

        private int FirstVisibleColumn()
        {
            for (int i = 0; i < this.ColumnCount; i++)
            {
                if (this.Columns[i].Visible) return i;
            }
            return -1;
        }

        public void MoveToNewRow(int columnindex = -1)
        {
            if (!this.AllowUserToAddRows || this.ReadOnly) return;
            if (columnindex == -1)
            {
                columnindex = FirstVisibleColumn();
                if (columnindex == -1) return;
            }
            this.CurrentCell = this[columnindex, this.NewRowIndex];
        }

        public DataRow GetCurrentDataRow()
        {
            if (this.CurrentRow == null || 
                this.CurrentRow.IsNewRow) return null;
            try
            {
                object o = this.CurrentRow.DataBoundItem;
                if (o == null) return null;
                var drv = o as DataRowView;
                if (drv == null) return null;
                return drv.Row;
            }
            catch (Exception){}
            return null;
        }

        public DataRow GetDataRow(int k)
        {
            object o = GetDataItem(k);
            if (o == null) return null;
            var drv = o as DataRowView;
            if (drv == null) return null;
            return drv.Row;
        }

        public T GetObjectViewItem<T>(int k)
        {
            object o = GetDataItem(k);
            if (o == null) return default;
            var ov = o as ObjectView<T>;
            if (ov == null) return default;
            return ov.Object;
        }

        public T GetCurrentObjectViewItem<T>()
        {
            if (this.CurrentRow == null ||
                this.CurrentRow.IsNewRow) return default;
            try
            {
                object o = this.CurrentRow.DataBoundItem;
                if (o == null) return default;
                var ov = o as ObjectView<T>;
                if (ov == null) return default;
                return ov.Object;
            }
            catch (Exception) { }
            return default;
        }


        public object GetCurrentDataItem()
        {
            if (this.CurrentRow == null || this.CurrentRow.Index == -1 ||
                this.CurrentRow.IsNewRow) return null;
            try
            {
                CurrencyManager cm = (CurrencyManager)this.BindingContext[this.DataSource, this.DataMember];
                if (cm == null || this.CurrentRow.Index >= cm.Count) return null;
                return this.CurrentRow.DataBoundItem;
            }
            catch (Exception){}
            return null;
        }

        [System.Diagnostics.DebuggerHidden()]
        public object GetDataItem(int k)
        {
            try
            {
                if (k < 0 || k >= this.Rows.Count ||
                    k == this.NewRowIndex) return null;
                return this.Rows[k].DataBoundItem;
            }
            catch (Exception) { }
            return null;
        }

        private void My_FormClosing(Object sender, FormClosingEventArgs e)
        {
            this.EndEdit();
        }


        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.Tab:
                    if (e.Control && TakeCtrlTabKey)
                    {
                        e.IsInputKey = true;
                        return;
                    }
                    break;
            }
            base.OnPreviewKeyDown(e);
        }

        protected override bool ProcessDataGridViewKey(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Tab:
                    if (e.Control)
                    {
                        OnMyKeyDown(e);
                        return true;
                    }
                    break;
                case Keys.Enter:
                    if (!e.Control)
                    {
                        return ProcessRightKey(e.KeyData & ~(Keys.Control | Keys.Shift));
                        //return ProcessTabKey(e.KeyData & ~(Keys.Control | Keys.Shift));
                    }
                    break;
                case Keys.End:
                    if (!e.Control)
                    {
                        var ret = ProcessEndKey(e.KeyData);
                        if(ret) FirstDisplayedScrollingColumnIndex = CurrentCell.ColumnIndex;
                        return ret;
                    }
                    break;
                case Keys.Delete:
                    if (e.Control)
                    {
                        OnMyKeyDown(e);
                        return true;
                    }
                    break;
                case Keys.Insert:
                    if (e.Control || e.Shift)
                    {
                        OnMyKeyDown(e);
                        return true;
                    }
                    break;
            }

            if (e.Control && e.KeyCode == Keys.C)
            {
                return Copy(true);
            }

            return base.ProcessDataGridViewKey(e);
        }

        private object lastCurrentRow = null;
        private static DateTime lastCheckCurrentRowTime = DateTime.MinValue;

        protected void CheckCurrentRowChanged()
        {
            var o = GetCurrentDataItem();
            if (o == lastCurrentRow) return;
            if (lastCurrentRow == null)
            {
                lastCurrentRow = o;
                return;
            }
            lastCurrentRow = o;

            var bs = this.DataSource as MyBindingSourceEf;
            if (AutoSave && bs != null && this.CurrentRow != null && 
                !this.IsCurrentRowDirty && !CurrentRow.IsNewRow &&
                (DateTime.Now - lastCheckCurrentRowTime).Seconds > 4)
            {
                if (FindForm() is MyFormBase myform)
                {
                    myform.SaveData();
                }
                else
                {
                    bs.SaveDbContext();
                }
                lastCheckCurrentRowTime = DateTime.Now;
            }
            if (o != null && MyCheckForChanges != null)
                MyCheckForChanges(this, new EventArgs());
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            CheckCurrentRowChanged();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            CheckCurrentRowChanged();
        }

        /*
        private bool AllowUserToAddRowsA = true;
        protected override void OnCellBeginEdit(DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex != this.NewRowIndex)
            {
                AllowUserToAddRowsA = this.AllowUserToAddRows;
                this.AllowUserToAddRows = false;
            }
            base.OnCellBeginEdit(e);
        }

        protected override void OnCellEndEdit(DataGridViewCellEventArgs e)
        {
            this.AllowUserToAddRows = AllowUserToAddRowsA;
            base.OnCellEndEdit(e);
        }
        */

        public bool Copy(bool withheaders)
        {
            try
            {
                //DataFormats.UnicodeText - has tab separeted values
                var ccm = ClipboardCopyMode;
                if(withheaders)
                    ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
                else
                    ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
                string csv = (string) this.GetClipboardContent().GetData(DataFormats.UnicodeText);
                ClipboardCopyMode = ccm;
                /*
                var dataObject = new DataObject();
                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
                var stream = new System.IO.MemoryStream(bytes);
                dataObject.SetData(DataFormats.CommaSeparatedValue, stream);
                Clipboard.SetDataObject(dataObject, true);
                 */
                if (csv == null) return true;
                csv = csv.Replace(" ", "");
                if (string.IsNullOrEmpty(csv)) return true;
                var lines = csv.Split(new[] { "\r\n" }, StringSplitOptions.None);
                var checklines = lines.Where(d => d.Length == 0 || d[0] != '\t');
                if (!checklines.Any())
                {
                    var sb = new StringBuilder();
                    foreach (var line in lines)
                    {
                        sb.Append(line.Substring(1));
                        sb.Append("\r\n");
                    }
                    csv = sb.ToString();
                }
                Clipboard.SetText(csv);
            }
            catch (Exception)
            {
                
            }
            return true;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            Keys key = (keyData & Keys.KeyCode);
            switch (key)
            {
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                    var kev = new KeyEventArgs(keyData);
                    OnMyKeyDown(kev);
                    if (kev.Handled)
                        return true;
                    break;
                case Keys.Enter:
                    /*
                    if (ProcessNextKey(keyData & ~(Keys.Control | Keys.Shift)))
                    {
                        return true;
                    }*/
                    if ((keyData & Keys.Control) == 0)
                    {
                        if (ProcessRightKey(keyData & ~(Keys.Control | Keys.Shift)))
                        {
                            return true;
                        }
                    }
                    break;
            }

            var ret = base.ProcessDialogKey(keyData);
            CheckCurrentRowChanged();
            return ret;
        }

        public void RefreshCurrent()
        {
            if (CurrentRow == null || CurrentRow.IsNewRow) return;
            this.InvalidateRow(CurrentRow.Index);
        }

        private bool IsInPainting = false;
        protected override void OnPaint(PaintEventArgs pevent)
        {
            IsInPainting = true;
            try
            {
                base.OnPaint(pevent);
                var pen = new Pen(this.ForeColor, 1);
                pevent.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            }
            finally
            {
                IsInPainting = false;
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            if (!AllowUserToAddRows) return;
            if (Rows.Count == 0 || CurrentCell == null) return;
            if(NewRowIndex > -1)
                Rows[NewRowIndex].Selected = false;
            if (SelectedRows.Count > 0) return;
            if (SelectedCells.Count == 0) return;
            if (SelectedCells.Count == 1 && SelectedCells[0] == CurrentCell) return;
            ClearSelection();
        }

        public bool EndEditX()
        {
            if (!EndEdit()) return false;
            var bs = DataSource as BindingSource;
            if (bs == null || CurrentRow == null ||
                CurrentRow.IsNewRow ||
                bs.Current == null) return true;

            var drv = bs.Current as DataRowView;
            if (drv == null) return true;
            if (drv.Row.RowState == DataRowState.Detached) return true;

            try
            {
                bs.EndEdit();
                this.InvalidateRow(CurrentRow.Index);
                return true;
            }
            catch (Exception e)
            {
                Form_Error.ShowException(e, this);
                return false;
            }
        }




        private const int WM_CHAR = 0x0102;

        protected override void OnValidating(CancelEventArgs e)
        {
            if (GoingToDialog) return;
            base.OnValidating(e);
        }

        public void OnMyKeyDown(KeyEventArgs e)
        {
            if (MyKeyDown != null)
                MyKeyDown(this, e);
        }

        private void MyDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (IsInPainting)
            {
                e.Cancel = true;
                return;
            }
            if (e.Exception != null)
            {
                MyException mex = ExceptionHelper.TranslateException(e.Exception, this);
                Form_Error.ShowException(mex);
            }
            e.Cancel = true;
        }

        ItemRestrictions GetItemRestrictions()
        {
            if (DataSource == null) return ItemRestrictions.Default;
            if (DataSource is MyBindingSourceEf bsef)
            {
                if (bsef.BindingListView == null) return ItemRestrictions.Default;
                return bsef.GetItemPropertiesRestrictions();
            }
            if (DataSource is MyBindingSourceEf2 bsef2)
            {
                if (bsef2.DataSource == null) return ItemRestrictions.Default;
                return bsef2.GetItemPropertiesRestrictions();
            }
            if (DataSource is BindingSource bs)
            {
                if (bs.DataSource != null && bs.DataMember != null && bs.DataSource is MyBindingSourceEf bsef3)
                {
                    var ctx = bsef3.GetMyDbContext();
                    if (ctx == null) return ItemRestrictions.Default;
                    var tp = ListBindingHelper.GetListItemType(bs);
                    if (tp == null || !tp.IsGenericType || 
                        tp.GetGenericTypeDefinition() != typeof(ObjectView<>))
                        return ItemRestrictions.Default;
                    var ret = ctx.GetItemPropertiesRestrictions(tp);
                    return ret;
                }
            }
            return ItemRestrictions.Default;
        }

        ItemPropertyRestrictions GetItemPropertiesRestrictions(int rowindex, string propname)
        {
            if (DataSource == null) return ItemPropertyRestrictions.Default;
            if (DataSource is MyBindingSourceEf bsef)
            {
                if (bsef.BindingListView == null) return ItemPropertyRestrictions.Default;
                return bsef.GetItemPropertysRestrictions(propname);
            }
            if (DataSource is MyBindingSourceEf2 bsef2)
            {
                if (bsef2.DataSource == null) return ItemPropertyRestrictions.Default;
                return bsef2.GetItemPropertysRestrictions(propname);
            }
            if (DataSource is BindingSource bs)
            {
                if (bs.DataSource != null && bs.DataMember != null && bs.DataSource is MyBindingSourceEf bsef3)
                {
                    var ctx = bsef3.GetMyDbContext();
                    if (ctx == null) return ItemPropertyRestrictions.Default;
                    var item = (GetDataItem(rowindex) as IMyObjectView)?.Object;
                    if (item == null) return ItemPropertyRestrictions.Default;
                    var tp = item.GetType();
                    var ret = ctx.GetItemPropertyRestrictions(tp, propname);
                    return ret;
                }
            }
            return ItemPropertyRestrictions.Default;
        }

        protected override void OnCellValidating(DataGridViewCellValidatingEventArgs e)
        {
            base.OnCellValidating(e);
            if (e.Cancel || !AutoValidate || e.RowIndex == NewRowIndex) return;
            if (!this[e.ColumnIndex, e.RowIndex].IsInEditMode) return;
            var value = e.FormattedValue as string;
            if (value == null) return;
            if (DataSource == null) return;
            string propname = Columns[e.ColumnIndex].DataPropertyName;
            if (propname == null) return;
            var restrictions = GetItemPropertiesRestrictions(e.RowIndex, propname);
            if (!restrictions.AllowNull && value.IsNOE())
            {
                MyMainFormBase.MyInstance.ShowWarning("Datu lauks nevar būt tukšs", owner: FindForm());
                e.Cancel = true;
                return;
            }
            if (restrictions.MaxLength > 0 && value.Length > restrictions.MaxLength)
            {
                MyMainFormBase.MyInstance.ShowWarning($"Ievadītais teksts ir par garu (>{restrictions.MaxLength})", owner: FindForm());
                e.Cancel = true;
                return;
            }
            if (restrictions.IsKey)
            {
                MyDbContext ctx = null;
                if (DataSource is MyBindingSourceEf bsef)
                    ctx = bsef.GetMyDbContext();
                else if (DataSource is BindingSource bs && bs.DataSource is MyBindingSourceEf bsef2)
                    ctx = bsef2.GetMyDbContext();
                if (ctx == null) return;
                var item = (GetDataItem(e.RowIndex) as IMyObjectView)?.Object;
                if (item == null) return;
                if (!Utils.GetPublicPropertyValue(item, propname, out var cur_value)) return;
                if (Equals(cur_value, e.FormattedValue)) return;
                if (cur_value == null || (cur_value is int vint && vint == 0) || 
                    (cur_value is long vlong && vlong == 0)) return;
                var entry = ctx.Entry(item);
                if (entry == null) return;
                if (entry.State == EntityState.Detached) return;
                if (entry.State == EntityState.Added) return;
                MyMainFormBase.MyInstance.ShowWarning("Datu lauku nevar mainīt", owner: FindForm());
                e.Cancel = true;
                return;
            }
        }

        protected override void OnRowValidating(DataGridViewCellCancelEventArgs e)
        {
            //if grid is bad state - not synced with datasource
            //and OnPaint gets called (like when called MessageBox.ShowDialog)
            //we get some crap errors
            if (e.RowIndex == NewRowIndex) return;
            if (GetDataItem(e.RowIndex) == null) return;

            base.OnRowValidating(e);
            if (e.Cancel) return;

            if (e.RowIndex != CurrentRow.Index || !IsCurrentRowDirty) return;
            var dgv_row = this.Rows[e.RowIndex];
            if (dgv_row.IsNewRow) return;
            
            var o = GetDataItem(e.RowIndex);
            if (o == null || o is not IMyObjectView ov) return;
            if (ov.Object == null) return;
            var restr = GetItemRestrictions();
            if (restr == null) return;

            var restr_notnull = restr.Values.Where(x => !x.AllowNull).ToList();
            var props = TypeDescriptor.GetProperties(ov.Object);

            var column_props = new List<string>();
            for(int i = 0; i < Columns.Count; i++)
            {
                var column = Columns[i];
                if (column.DataPropertyName.IsNOE()) continue;
                column_props.Add(column.DataPropertyName);
            }
                
            foreach (var r in restr_notnull)
            {
                if (!column_props.Contains(r.PropertyName)) continue;
                var prop = props[r.PropertyName];
                if (prop == null || prop.IsReadOnly) continue;
                var val = prop.GetValue(ov.Object);
                if (val == null || val == DBNull.Value)
                {
                    MyMainFormBase.MyInstance.ShowWarning("Ierakstā ir viens vai vairāki datu lauki, kas nevar būt tukši", owner: FindForm());
                    e.Cancel = true;
                    return;
                }
            }

        }


        private System.Windows.Forms.Timer clickTimer = new System.Windows.Forms.Timer();
        private DataGridViewCellEventArgs cellClickEventArgs = null;
        
        protected override void OnCellClick(DataGridViewCellEventArgs e)
        {
            //base.OnCellClick(e);
            //return;

            if (clickTimer.Enabled)
            {
                clickTimer.Stop();
                if (e.ColumnIndex == cellClickEventArgs.ColumnIndex
                    && e.RowIndex == cellClickEventArgs.RowIndex)
                {
                    OnCellDoubleClick(e);
                    return;
                }
            }
            cellClickEventArgs = new DataGridViewCellEventArgs(e.ColumnIndex, e.RowIndex);
            clickTimer.Start();
        }

        protected override void OnCellDoubleClick(DataGridViewCellEventArgs e)
        {
            clickTimer.Stop();
            base.OnCellDoubleClick(e);
        }
        
        private void OnClickTimerTick(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated) return;
            if (this.RowCount == 0) return;
            clickTimer.Stop();
            if (CurrentCell == null) return;
            if (cellClickEventArgs.RowIndex < 0) return;
            if (cellClickEventArgs.RowIndex >= Rows.Count) return;
            base.OnCellClick(cellClickEventArgs);
        }


        public void DisableAllColumnSorting()
        {
            foreach (DataGridViewColumn column in Columns)
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

    }


}
